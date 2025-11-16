using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class HistoryControl : UserControl
    {
        private ListBox? lstRunHistory;
        private Button? btnLoadMore;
        private Label? lblLoading;

        // 历史记录服务
        private readonly TimerHistoryService _historyService;

        // 分页相关变量
        private const int PageSize = 20; // 每页显示20条记录
        private int _displayStartIndex = 0; // 当前显示的起始索引
        private bool _isLoading = false; // 是否正在加载数据
        private Label? _loadingIndicator; // 加载状态指示器

        public int RunCount => _historyService.RunCount;
        public TimeSpan FastestTime => _historyService.FastestTime;
        public TimeSpan AverageTime => _historyService.AverageTime;
        public List<TimeSpan> RunHistory => _historyService.RunHistory;

        public HistoryControl()
        {
            InitializeComponent();
            _historyService = TimerHistoryService.Instance;
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            _historyService.HistoryDataChanged += OnHistoryDataChanged;
        }

        /// <summary>
        /// 添加单条运行记录
        /// </summary>
        public void AddRunRecord(TimeSpan runTime)
        {
            _historyService.AddRunRecord(runTime);
        }

        /// <summary>
        /// 更新历史记录
        /// </summary>
        public void UpdateHistory(List<TimeSpan> runHistory)
        {
            _historyService.UpdateHistory(runHistory);
        }

        /// <summary>
        /// 从角色档案加载特定场景的历史数据（异步分页加载）
        /// </summary>
        public async Task<bool> LoadProfileHistoryDataAsync(CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty)
        {
            // 设置加载状态
            _isLoading = true;
            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    _loadingIndicator.Visible = true;
                }));
            }
            else
            {
                _loadingIndicator.Visible = true;
            }
            
            try
            {
                bool result = _historyService.LoadProfileHistoryData(profile, scene, characterName, difficulty);
                if (result)
                {
                    // 重置显示起始索引，初始只显示最近的记录
                    _displayStartIndex = Math.Max(0, _historyService.RunHistory.Count - PageSize);
                    await UpdateUIAsync();
                }
                return result;
            }
            finally
            {
                // 隐藏加载指示器
                _isLoading = false;
                if (InvokeRequired)
                {
                    Invoke(new Action(() => {
                        _loadingIndicator.Visible = false;
                    }));
                }
                else
                {
                    _loadingIndicator.Visible = false;
                }
            }
        }

        /// <summary>
        /// 同步版本的加载方法，供需要立即执行的场景使用
        /// </summary>
        public bool LoadProfileHistoryData(CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty)
        {
            bool result = _historyService.LoadProfileHistoryData(profile, scene, characterName, difficulty);
            if (result)
            {
                // 重置显示起始索引，初始只显示最近的记录
                _displayStartIndex = Math.Max(0, _historyService.RunHistory.Count - PageSize);
                UpdateUI();
            }
            return result;
        }

        /// <summary>
        /// 异步更新UI显示
        /// </summary>
        private async Task UpdateUIAsync()
        {
            if (lstRunHistory == null) return;

            // 清空当前列表
            lstRunHistory.Items.Clear();
            
            // 获取要显示的数据范围
            int count = _historyService.RunHistory.Count;
            int displayCount = Math.Min(count - _displayStartIndex, PageSize);
            
            // 异步处理数据格式化，添加边界检查
            var itemsToAdd = await Task.Run(() =>
            {
                var items = new List<string>(displayCount);
                var currentHistory = _historyService.RunHistory;
                int currentCount = currentHistory.Count;
                
                for (int i = _displayStartIndex; i < _displayStartIndex + displayCount; i++)
                {
                    // 添加严格的边界检查，确保索引有效
                    if (i >= 0 && i < currentCount)
                    {
                        var time = currentHistory[i];
                        string timeFormatted = FormatTime(time);
                        string runText = GetRunText(i + 1, timeFormatted);
                        items.Add(runText);
                    }
                    else
                    {
                        // 索引无效时退出循环
                        break;
                    }
                }
                return items;
            });
            
            // 在UI线程添加项目
            foreach (var item in itemsToAdd)
            {
                lstRunHistory.Items.Add(item);
            }
            
            // 滚动到最新记录
            if (lstRunHistory.Items.Count > 0)
            {
                lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
            }
        }

        /// <summary>
        /// 同步更新UI显示
        /// </summary>
        private void UpdateUI()
        {
            if (lstRunHistory == null) return;

            // 清空当前列表
            lstRunHistory.Items.Clear();
            
            // 获取要显示的数据范围
            int count = _historyService.RunHistory.Count;
            int displayCount = Math.Min(count - _displayStartIndex, PageSize);
            
            // 添加记录到列表，添加边界检查
            var currentHistory = _historyService.RunHistory;
            int currentCount = currentHistory.Count;
            
            for (int i = _displayStartIndex; i < _displayStartIndex + displayCount; i++)
            {
                // 添加严格的边界检查，确保索引有效
                if (i >= 0 && i < currentCount)
                {
                    var time = currentHistory[i];
                    string timeFormatted = FormatTime(time);
                    string runText = GetRunText(i + 1, timeFormatted);
                    lstRunHistory.Items.Add(runText);
                }
                else
                {
                    // 索引无效时退出循环
                    break;
                }
            }
            
            // 滚动到最新记录
            if (lstRunHistory.Items.Count > 0)
            {
                lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
            }
        }

        /// <summary>
        /// 只更新UI显示，不重新加载数据
        /// </summary>
        public void RefreshUI()
        {
            if (lstRunHistory == null) return;

            // 只更新现有项目的文本（用于语言切换等）
            for (int i = 0; i < lstRunHistory.Items.Count; i++)
            {
                int actualIndex = _displayStartIndex + i;
                if (actualIndex < _historyService.RunHistory.Count)
                {
                    var time = _historyService.RunHistory[actualIndex];
                    string timeFormatted = FormatTime(time);
                    string runText = GetRunText(actualIndex + 1, timeFormatted);
                    lstRunHistory.Items[i] = runText;
                }
            }
        }

        /// <summary>
        /// 添加单条记录 - 只添加不刷新（类似React的单项更新）
        /// </summary>
        private void AddSingleRunRecord(TimeSpan runTime)
        {
            if (lstRunHistory == null) return;

            // 获取新记录的索引
            int newIndex = _historyService.RunHistory.Count - 1;
            
            // 确保显示最新的记录
            _displayStartIndex = Math.Max(0, newIndex - PageSize + 1);
            
            // 格式化时间
            string timeFormatted = FormatTime(runTime);
            string runText = GetRunText(newIndex + 1, timeFormatted);
            
            // 如果列表已满，移除最旧的记录
            if (lstRunHistory.Items.Count >= PageSize)
            {
                lstRunHistory.Items.RemoveAt(0);
            }
            
            // 添加新记录到列表
            lstRunHistory.Items.Add(runText);
            
            // 滚动到最新记录
            lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
            lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
        }

        /// <summary>
        /// 滚动事件处理，用于异步加载更多历史记录
        /// </summary>
        private void lstRunHistory_MouseWheel(object sender, MouseEventArgs e)
        {
            // 检查是否需要加载更多历史记录
            // 当向上滚动（e.Delta > 0）并且接近列表顶部时加载更多
            if (e.Delta > 0 && _displayStartIndex > 0 && !_isLoading && lstRunHistory.TopIndex < 5)
            {
                LoadMoreHistoryAsync();
            }
        }

        /// <summary>
        /// 异步加载更多历史记录（向上加载）
        /// </summary>
        private async Task LoadMoreHistoryAsync()
        {
            if (lstRunHistory == null || _displayStartIndex <= 0 || _isLoading) return;

            _isLoading = true;
            
            // 在UI线程显示加载指示器
            if (InvokeRequired)
            {
                Invoke(new Action(() => {
                    _loadingIndicator.Visible = true;
                }));
            }
            else
            {
                _loadingIndicator.Visible = true;
            }
            
            try
            {
                // 计算要加载的记录范围
                int newStartIndex = Math.Max(0, _displayStartIndex - PageSize);
                int addedCount = _displayStartIndex - newStartIndex;
                
                // 异步处理数据格式化
                var newItems = await Task.Run(() =>
                {
                    var items = new List<string>(addedCount);
                    for (int i = newStartIndex; i < _displayStartIndex; i++)
                    {
                        if (i < _historyService.RunHistory.Count)
                        {
                            var time = _historyService.RunHistory[i];
                            string timeFormatted = FormatTime(time);
                            string runText = GetRunText(i + 1, timeFormatted);
                            items.Add(runText);
                        }
                    }
                    return items;
                });
                
                // 更新起始索引
                _displayStartIndex = newStartIndex;
                
                // 保存当前列表项
                var oldItems = new string[lstRunHistory.Items.Count];
                for (int i = 0; i < lstRunHistory.Items.Count; i++)
                {
                    oldItems[i] = lstRunHistory.Items[i].ToString();
                }
                
                // 清空列表并重新填充
                lstRunHistory.Items.Clear();
                
                // 添加新加载的历史记录
                foreach (var item in newItems)
                {
                    lstRunHistory.Items.Add(item);
                }
                
                // 添加之前的记录
                foreach (var item in oldItems)
                {
                    lstRunHistory.Items.Add(item);
                }
                
                // 调整滚动位置到新加载内容的末尾
                lstRunHistory.TopIndex = newItems.Count;
            }
            finally
            {
                _isLoading = false;
                
                // 在UI线程隐藏加载指示器
                if (InvokeRequired)
                {
                    Invoke(new Action(() => {
                        _loadingIndicator.Visible = false;
                    }));
                }
                else
                {
                    _loadingIndicator.Visible = false;
                }
            }
        }

        private string FormatTime(TimeSpan time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3}",
                time.Hours, time.Minutes, time.Seconds,
                (int)(time.Milliseconds / 100));
        }

        private string GetRunText(int runNumber, string timeFormatted)
        {
            string runText = Utils.LanguageManager.GetString("RunNumber", runNumber, timeFormatted);
            if (string.IsNullOrEmpty(runText) || runText == "RunNumber")
            {
                runText = $"Run {runNumber}: {timeFormatted}";
            }
            return runText;
        }

        /// <summary>
        /// 处理历史数据变更事件 - 根据变更类型决定如何更新UI
        /// </summary>
        private void OnHistoryDataChanged(object? sender, HistoryChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    ProcessHistoryChange(e);
                }));
            }
            else
            {
                ProcessHistoryChange(e);
            }
        }
        
        /// <summary>
        /// 处理历史记录变更
        /// </summary>
        private async void ProcessHistoryChange(HistoryChangedEventArgs e)
        {
            switch (e.ChangeType)
            {
                case HistoryChangeType.Add:
                    // 只添加单条记录，不刷新整个列表
                    if (e.AddedRecord.HasValue)
                    {
                        AddSingleRunRecord(e.AddedRecord.Value);
                    }
                    break;
                case HistoryChangeType.FullRefresh:
                default:
                    // 异步全量刷新UI，显示加载状态
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => {
                            _loadingIndicator.Visible = true;
                        }));
                    }
                    else
                    {
                        _loadingIndicator.Visible = true;
                    }
                    
                    try
                    {
                        _displayStartIndex = Math.Max(0, _historyService.RunHistory.Count - PageSize);
                        await UpdateUIAsync();
                    }
                    finally
                    {
                        // 隐藏加载指示器
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => {
                                _loadingIndicator.Visible = false;
                            }));
                        }
                        else
                        {
                            _loadingIndicator.Visible = false;
                        }
                    }
                    break;
            }
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
                HorizontalScrollbar = true
            };
            // 创建并初始化加载指示器
            _loadingIndicator = new Label
            {
                Text = "加载中...",
                AutoSize = true,
                Visible = false,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            Controls.Add(_loadingIndicator);
            
            // 添加鼠标滚轮事件来检测滚动行为
            lstRunHistory.MouseWheel += lstRunHistory_MouseWheel;

            // 添加到控件
            Controls.Add(lstRunHistory);

            // 控件设置
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Size = new Size(290, 90);
            Name = "HistoryControl";
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            RefreshUI(); // 只刷新显示文本，不重新加载数据
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                _historyService.HistoryDataChanged -= OnHistoryDataChanged;

                // 移除鼠标滚轮事件订阅
            if (lstRunHistory != null)
            {
                lstRunHistory.MouseWheel -= lstRunHistory_MouseWheel;
            }
            }
            base.Dispose(disposing);
        }
    }
}
