using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class HistoryControl : UserControl
    {
        private ListBox? lstRunHistory;

        // 历史记录数据
        public List<TimeSpan> RunHistory { get; set; } = [];

        // 历史记录统计信息
        public int RunCount { get; private set; } = 0;
        public TimeSpan FastestTime { get; private set; } = TimeSpan.MaxValue;
        public TimeSpan AverageTime { get; private set; } = TimeSpan.Zero;

        public HistoryControl()
        {
            InitializeComponent();
            // 注册语言变更事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        }
        
        /// <summary>
        /// 从角色档案加载特定场景的历史数据
        /// </summary>
        /// <param name="profile">角色档案</param>
        /// <param name="scene">场景名称</param>
        /// <param name="characterName">角色名称</param>
        /// <param name="difficulty">游戏难度</param>
        /// <returns>是否成功加载历史数据</returns>
        public bool LoadProfileHistoryData(CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty)
        {   
            // 重置当前的统计数据
            ResetHistoryData();
            
            // 如果有当前角色档案和场景，加载历史数据
            if (profile != null && !string.IsNullOrEmpty(scene))
            {   
                try
                {   
                    // 添加详细调试日志
                    LogManager.WriteDebugLog("HistoryControl", $"调试 - 当前场景名称: '{scene}'");
                    LogManager.WriteDebugLog("HistoryControl", $"调试 - 档案总记录数: {profile.Records.Count}");
                    
                    // 输出档案中所有记录的场景名称用于调试
                    for (int i = 0; i < profile.Records.Count; i++)
                    {   
                        var record = profile.Records[i];
                        LogManager.WriteDebugLog("HistoryControl", $"调试 - 档案记录 {i}: {record.SceneName}, 难度: {record.Difficulty}, 完成: {record.IsCompleted}");
                    }
                    
                    // 从角色档案中过滤出同场景记录
                    // 使用LanguageManager获取纯英文场景名称进行匹配
                    string pureEnglishSceneName = DTwoMFTimerHelper.Utils.LanguageManager.GetPureEnglishSceneName(scene);
                    
                    // 日志记录当前匹配的场景名称
                    LogManager.WriteDebugLog("HistoryControl", $"匹配场景名称: '{pureEnglishSceneName}'");
                    
                    // 修改过滤条件，处理记录场景名称可能为空的情况，或使用配置中的sceneName，并添加难度匹配
                    var sceneRecords = profile.Records.Where(r => 
                        (string.IsNullOrEmpty(r.SceneName) || 
                         r.SceneName.Equals(pureEnglishSceneName, StringComparison.OrdinalIgnoreCase) ||
                         r.SceneName.Trim('"', '\'').Equals(pureEnglishSceneName, StringComparison.OrdinalIgnoreCase)) && 
                        r.IsCompleted &&
                        r.Difficulty == difficulty).ToList();
                    
                    LogManager.WriteDebugLog("HistoryControl", $"从角色档案中加载了 {sceneRecords.Count} 条记录: {characterName} - {pureEnglishSceneName}, 难度: {difficulty}");
                    Console.WriteLine($"从角色档案中加载了 {sceneRecords.Count} 条记录: {characterName} - {pureEnglishSceneName}, 难度: {difficulty}");
                    
                    // 如果有记录，更新统计数据
                    if (sceneRecords.Count > 0)
                    {   
                        RunCount = sceneRecords.Count;
                        
                        // 用于计算平均时间的总和
                        double totalSeconds = 0;
                        
                        // 转换为TimeSpan并添加到历史记录
                        foreach (var record in sceneRecords)
                        {   
                            // 添加详细的初始值日志，记录record对象在赋值前的状态
                            LogManager.WriteDebugLog("HistoryControl", $"[加载记录前状态] ID: {record.GetHashCode()}, StartTime: {record.StartTime}, EndTime: {(record.EndTime.HasValue ? record.EndTime.Value.ToString() : "null")}, LatestTime: {(record.LatestTime.HasValue ? record.LatestTime.Value.ToString() : "null")}, ElapsedTime: {(record.ElapsedTime.HasValue ? record.ElapsedTime.Value.ToString() : "null")}, SceneName: {record.SceneName}");
                            
                            // 添加详细的调试日志
                            Console.WriteLine($"[调试] 加载记录 - StartTime: {record.StartTime}, EndTime: {record.EndTime}, LatestTime: {record.LatestTime}, ElapsedTime: {record.ElapsedTime}");
                            
                            // 手动计算正确的持续时间
                            double correctDuration;
                            int recordIndex = sceneRecords.IndexOf(record);
                            LogManager.WriteDebugLog("HistoryControl", $"[加载记录 #{recordIndex + 1}] 开始计算持续时间");
                            LogManager.WriteDebugLog("HistoryControl", $"[加载记录 #{recordIndex + 1}] StartTime: {record.StartTime}, EndTime: {record.EndTime}, LatestTime: {record.LatestTime}, ElapsedTime: {record.ElapsedTime}");
                            
                            if (record.ElapsedTime.HasValue && record.ElapsedTime.Value > 0 && record.LatestTime.HasValue && record.EndTime.HasValue)
                            {   
                                double latestToEnd = (record.EndTime.Value - record.LatestTime.Value).TotalSeconds;
                                correctDuration = record.ElapsedTime.Value + latestToEnd;
                                LogManager.WriteDebugLog("HistoryControl", $"[加载记录 #{recordIndex + 1}] 计算路径: ElapsedTime + (EndTime - LatestTime) = {record.ElapsedTime.Value} + {latestToEnd} = {correctDuration}秒");
                            }
                            else if (record.EndTime.HasValue)
                            {   
                                correctDuration = (record.EndTime.Value - record.StartTime).TotalSeconds;
                                LogManager.WriteDebugLog("HistoryControl", $"[加载记录 #{recordIndex + 1}] 计算路径: EndTime - StartTime = {correctDuration}秒");
                            }
                            else
                            {   
                                correctDuration = 0;
                                LogManager.WriteDebugLog("HistoryControl", $"[加载记录 #{recordIndex + 1}] 计算路径: 持续时间为0");
                            }
                            
                            TimeSpan duration = TimeSpan.FromSeconds(correctDuration);
                            RunHistory.Add(duration);
                            totalSeconds += correctDuration;
                            LogManager.WriteDebugLog("HistoryControl", $"[加载记录 #{recordIndex + 1}] 成功添加到RunHistory: {duration}");
                            
                            // 更新最快时间
                            if (duration < FastestTime)
                            {   
                                FastestTime = duration;
                                LogManager.WriteDebugLog("HistoryControl", $"[加载记录 #{recordIndex + 1}] 更新最快时间为: {FastestTime}");
                            }
                        }
                        
                        // 计算平均时间
                        if (RunCount > 0)
                        {
                            AverageTime = TimeSpan.FromSeconds(totalSeconds / RunCount);
                        }
                        
                        // 按时间从旧到新排序，让时间靠前的记录排在前面
                        RunHistory.Sort((a, b) => a.CompareTo(b));
                        
                        LogManager.WriteDebugLog("HistoryControl", "[加载完成] RunHistory排序后内容:");
                        for (int i = 0; i < RunHistory.Count; i++)
                        {   
                            LogManager.WriteDebugLog("HistoryControl", $"[加载完成] 记录 #{i + 1}: {RunHistory[i]}");
                        }
                        LogManager.WriteDebugLog("HistoryControl", $"[加载完成] 运行次数: {RunCount}, 最快时间: {FastestTime}, 平均时间: {AverageTime}");
                        Console.WriteLine($"加载完成，运行次数: {RunCount}, 最快时间: {FastestTime}, 平均时间: {AverageTime}");
                        
                        // 更新UI
                        UpdateUI();
                        return true;
                    }
                }
                catch (Exception ex)
                {   
                    Console.WriteLine($"加载历史数据失败: {ex.Message}");
                    LogManager.WriteDebugLog("HistoryControl", $"加载历史数据异常: {ex.Message}, 堆栈: {ex.StackTrace}");
                }
            }
            return false;
        }
        
        /// <summary>
        /// 重置历史数据
        /// </summary>
        private void ResetHistoryData()
        {   
            RunHistory.Clear();
            RunCount = 0;
            FastestTime = TimeSpan.MaxValue;
            AverageTime = TimeSpan.Zero;
        }
        
        public void UpdateUI()
        {            // 更新历史记录列表
            if (lstRunHistory != null)
            {
                lstRunHistory.Items.Clear();
                for (int i = 0; i < RunHistory.Count; i++)
                {
                    var time = RunHistory[i];
                    string timeFormatted = string.Format("{0:00}:{1:00}:{2:00}.{3}",
                        time.Hours, time.Minutes, time.Seconds,
                        (int)(time.Milliseconds / 100));

                    string runText = Utils.LanguageManager.GetString("RunNumber", i + 1, timeFormatted);
                    if (string.IsNullOrEmpty(runText) || runText == "RunNumber")
                    {
                        runText = $"Run {i + 1}: {timeFormatted}";
                    }
                    lstRunHistory.Items.Add(runText);
                }

                // 确保最新记录在顶部
                if (lstRunHistory.Items.Count > 0)
                {
                    lstRunHistory.SelectedIndex = 0;
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {            if (disposing)
            {
                // 取消注册语言变更事件
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // 历史记录列表
            lstRunHistory = new ListBox
            {
                FormattingEnabled = true,
                ItemHeight = 15,
                Location = new Point(0, 0),
                Name = "lstRunHistory",
                Size = new Size(290, 90),
                TabIndex = 0,
                Parent = this
            };

            // 控件设置
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(290, 90);
            Name = "HistoryControl";
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateHistory(List<TimeSpan> runHistory)
        {
            // 更新历史记录数据
            this.RunHistory = runHistory;
            this.RunCount = runHistory.Count;
            
            // 重新计算统计数据
            if (RunCount > 0)
            {
                FastestTime = TimeSpan.MaxValue;
                double totalSeconds = 0;
                
                foreach (var time in runHistory)
                {
                    if (time < FastestTime)
                    {
                        FastestTime = time;
                    }
                    totalSeconds += time.TotalSeconds;
                }
                
                AverageTime = TimeSpan.FromSeconds(totalSeconds / RunCount);
            }
            else
            {
                FastestTime = TimeSpan.MaxValue;
                AverageTime = TimeSpan.Zero;
            }
            
            // 更新UI
            UpdateUI();
        }
        
        /// <summary>
        /// 添加新的运行记录
        /// </summary>
        /// <param name="runTime">运行时间</param>
        public void AddRunRecord(TimeSpan runTime)
        {
            // 添加到历史记录
            RunHistory.Insert(0, runTime);
            RunCount++;
            
            // 更新最快时间
            if (runTime < FastestTime)
            {
                FastestTime = runTime;
            }
            
            // 更新平均时间
            double totalSeconds = 0;
            foreach (var time in RunHistory)
            {
                totalSeconds += time.TotalSeconds;
            }
            AverageTime = TimeSpan.FromSeconds(totalSeconds / RunCount);
            
            // 更新UI
            UpdateUI();
        }
    }
}