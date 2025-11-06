using System;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Input;
using System.Runtime.InteropServices;
using DTwoMFTimerHelper.Resources;
using AntdUI;
using DTwoMFTimerHelper.Settings;

namespace DTwoMFTimerHelper
{
    public partial class MainForm : Form
    {
        // 各个功能控件
        private ProfileManager? profileManager;
        private PomodoroControl? pomodoroControl;
        private TimerControl? timerControl;
        private SettingsControl? settingsControl;
        private AppSettings? appSettings;

        public MainForm()
        {
            InitializeComponent();
            
            // 加载设置
            LoadSettings();
            
            // 启动测试：直接加载角色档案并显示详细信息
            LoadCharacterProfile();
            
            InitializeControls();
            InitializeLanguageSupport();
            
            // 确保窗口可见并具有合理的大小和位置
            this.Size = new Size(480, 600);
            this.StartPosition = FormStartPosition.Manual;
            
            // 应用保存的窗口位置
            if (appSettings != null && Screen.PrimaryScreen != null)
            {
                var position = SettingsManager.StringToWindowPosition(appSettings.WindowPosition);
                settingsControl?.MoveWindowToPosition(this, position);
            }
            
            this.ShowInTaskbar = true;
            this.Visible = true;
            
            // 加载上次使用的角色档案
            profileManager?.LoadLastUsedProfile();
        }
        
        private void LoadSettings()
        {
            appSettings = SettingsManager.LoadSettings();
            
            // 应用设置
            if (appSettings != null)
            {
                // 应用窗口置顶设置
                this.TopMost = appSettings.AlwaysOnTop;
            }
        }
        
        private void LoadCharacterProfile()
         {
             try
             {
                 Console.WriteLine("[启动测试] 开始加载角色档案...");
                 // 调用DataManager加载角色档案，分别测试includeHidden=true和false
                 Console.WriteLine("[启动测试] 测试1: 只加载非隐藏角色 (includeHidden=false)");
                 var profilesVisible = DTwoMFTimerHelper.Data.DataManager.LoadAllProfiles(includeHidden: false);
                 Console.WriteLine($"[启动测试] 测试1结果: 找到 {profilesVisible.Count} 个非隐藏角色档案");
                 
                 Console.WriteLine("\n[启动测试] 测试2: 加载所有角色包括隐藏的 (includeHidden=true)");
                 var profilesAll = DTwoMFTimerHelper.Data.DataManager.LoadAllProfiles(includeHidden: true);
                 Console.WriteLine($"[启动测试] 测试2结果: 找到 {profilesAll.Count} 个角色档案（包括隐藏的）");
                 
                 // 显示每个角色的详细信息
                 Console.WriteLine("\n[启动测试] 所有角色详细信息:");
                 foreach (var profile in profilesAll)
                 {
                     Console.WriteLine($"[启动测试] - 角色: {profile.Name}, 职业: {profile.Class}, IsHidden: {profile.IsHidden}");
                 }
                 
                 Console.WriteLine("[启动测试] 角色档案加载完成");
             }
            catch (Exception ex)
            {
                Console.WriteLine("[启动测试] 加载角色档案失败: {0}", ex.Message);
                Console.WriteLine("[启动测试] 异常堆栈: {0}", ex.StackTrace);
                // 在调试模式下显示错误对话框
#if DEBUG
                MessageBox.Show("[启动测试] 加载角色档案失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                // 生产环境只记录日志，不显示错误
#endif
            }
        }

        private void InitializeComponent()
        {
            tabControl = new TabControl();
            tabProfilePage = new System.Windows.Forms.TabPage();
            tabTimerPage = new System.Windows.Forms.TabPage();
            tabPomodoroPage = new System.Windows.Forms.TabPage();
            tabSettingsPage = new System.Windows.Forms.TabPage();
            tabControl.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabProfilePage);
            tabControl.Controls.Add(tabTimerPage);
            tabControl.Controls.Add(tabPomodoroPage);
            tabControl.Controls.Add(tabSettingsPage);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Margin = new Padding(6);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(894, 878);
            tabControl.TabIndex = 1;
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
            // 
            // tabProfilePage
            // 
            tabProfilePage.Location = new Point(4, 37);
            tabProfilePage.Name = "tabProfilePage";
            tabProfilePage.Size = new Size(886, 813);
            tabProfilePage.TabIndex = 0;
            tabProfilePage.Text = LanguageManager.GetString("TabProfile");
            tabProfilePage.UseVisualStyleBackColor = true;
            // 
            // tabTimerPage
            // 
            tabTimerPage.Location = new Point(4, 37);
            tabTimerPage.Name = "tabTimerPage";
            tabTimerPage.Size = new Size(886, 813);
            tabTimerPage.TabIndex = 1;
            tabTimerPage.UseVisualStyleBackColor = true;
            // 
            // tabPomodoroPage
            // 
            tabPomodoroPage.Location = new Point(4, 37);
            tabPomodoroPage.Name = "tabPomodoroPage";
            tabPomodoroPage.Size = new Size(886, 813);
            tabPomodoroPage.TabIndex = 2;
            tabPomodoroPage.UseVisualStyleBackColor = true;
            // 
            // tabSettingsPage
            // 
            tabSettingsPage.Location = new Point(4, 37);
            tabSettingsPage.Name = "tabSettingsPage";
            tabSettingsPage.Size = new Size(886, 837);
            tabSettingsPage.TabIndex = 3;
            tabSettingsPage.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(894, 878);
            Controls.Add(tabControl);
            Margin = new Padding(6);
            Name = "MainForm";
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        // 全局热键相关常量和方法
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;
        
        private const int HOTKEY_ID_STARTSTOP = 1;
        private const int HOTKEY_ID_PAUSE = 2;
        
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        // 当前注册的快捷键
        private Keys currentStartStopHotkey = Keys.Q | Keys.Alt;
        private Keys currentPauseHotkey = Keys.Space | Keys.Control;

        private void InitializeControls()
        {
            // 初始化各个功能控件
            profileManager = new ProfileManager();
            timerControl = new TimerControl();
            pomodoroControl = new PomodoroControl();
            settingsControl = new SettingsControl();

            // 设置控件的Dock属性
            profileManager.Dock = DockStyle.Fill;
            timerControl.Dock = DockStyle.Fill;
            pomodoroControl.Dock = DockStyle.Fill;
            settingsControl.Dock = DockStyle.Fill;

            // 标签页已经在InitializeComponent中添加，这里不需要重复添加
            
            // 添加到对应的TabPage
            if (tabProfilePage != null)
            {
                tabProfilePage.Controls.Add(profileManager);
                profileManager.TimerStateChanged += OnCountTimerStateChanged;
            }
            if (tabTimerPage != null)
            {
                tabTimerPage.Controls.Add(timerControl);
                timerControl.TimerStateChanged += OnTimerTimerStateChanged;
            }
            if (tabPomodoroPage != null)
            {
                tabPomodoroPage.Controls.Add(pomodoroControl);
                pomodoroControl.TimerStateChanged += OnPomodoroTimerStateChanged;
                pomodoroControl.PomodoroCompleted += OnPomodoroCompleted;
            }
            if (tabSettingsPage != null)
            {
                tabSettingsPage.Controls.Add(settingsControl);
                settingsControl.WindowPositionChanged += OnWindowPositionChanged;
                settingsControl.LanguageChanged += OnLanguageChanged;
                settingsControl.AlwaysOnTopChanged += OnAlwaysOnTopChanged;
                settingsControl.StartStopHotkeyChanged += OnStartStopHotkeyChanged;
                settingsControl.PauseHotkeyChanged += OnPauseHotkeyChanged;
            }
            
            // 注册默认快捷键
            RegisterHotkeys();
        }
        
        private void RegisterHotkeys()
        {
            // 先注销可能存在的热键
            UnregisterHotKeys();
            
            // 注册开始/停止热键
            RegisterHotKey(currentStartStopHotkey, HOTKEY_ID_STARTSTOP);
            
            // 注册暂停热键
            RegisterHotKey(currentPauseHotkey, HOTKEY_ID_PAUSE);
        }
        
        private void UnregisterHotKeys()
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID_STARTSTOP);
            UnregisterHotKey(this.Handle, HOTKEY_ID_PAUSE);
        }
        
        private void RegisterHotKey(Keys keys, int id)
        {
            int modifiers = 0;
            int keyCode = 0;
            
            // 提取修饰键
            if ((keys & Keys.Alt) == Keys.Alt)
                modifiers |= MOD_ALT;
            if ((keys & Keys.Control) == Keys.Control)
                modifiers |= MOD_CONTROL;
            if ((keys & Keys.Shift) == Keys.Shift)
                modifiers |= MOD_SHIFT;
            
            // 提取主按键
            keyCode = (int)(keys & Keys.KeyCode);
            
            // 注册热键
            RegisterHotKey(this.Handle, id, modifiers, keyCode);
        }
        
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                
                switch (id)
                {
                    case HOTKEY_ID_STARTSTOP:
                        // 触发计时器开始/停止
                        timerControl?.ToggleTimer();
                        break;
                    case HOTKEY_ID_PAUSE:
                        // 触发计时器暂停
                        timerControl?.TogglePause();
                        break;
                }
            }
        }
        
        private void tabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // 当标签页切换时，可以在这里添加额外的逻辑
            // 根据当前选中的选项卡更新UI
            UpdateUI();
        }

        private void InitializeLanguageSupport()
        {
            // 订阅语言改变事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            
            // 初始化UI文本
            UpdateUI();
            
            // 初始化窗口置顶状态
            this.TopMost = true;
        }

        private void UpdateUI()
        {
            // 更新界面文本
            this.Text = LanguageManager.GetString("FormTitle");
            
            // 更新选项卡标题
            if (tabControl != null && tabControl.TabPages.Count >= 4)
            {
                tabControl.TabPages[0].Text = LanguageManager.GetString("TabProfile");
                tabControl.TabPages[1].Text = "计时器";
                tabControl.TabPages[2].Text = LanguageManager.GetString("TabPomodoro");
                tabControl.TabPages[3].Text = LanguageManager.GetString("TabSettings");
            }
            
            // 更新各功能控件的UI
            profileManager?.UpdateUI();
            timerControl?.UpdateUI();
            pomodoroControl?.UpdateUI();
            settingsControl?.UpdateUI();
        }

        // 事件处理方法
        private void OnCountTimerStateChanged(object? sender, EventArgs e)
        {
            // 计数计时器状态改变时的处理
            // 可以在这里添加跨组件的交互逻辑
        }

        private void OnPomodoroTimerStateChanged(object? sender, EventArgs e)
        {
            // 番茄时钟状态改变时的处理
            // 可以在这里添加跨组件的交互逻辑
        }

        private void OnPomodoroCompleted(object? sender, EventArgs e)
        {
            // 番茄时钟完成时的处理
            // 可以在这里添加提示或其他操作
        }
        
        private void OnTimerTimerStateChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }


        private void OnLanguageChanged(object? sender, SettingsControl.LanguageChangedEventArgs e)
        {
            // 语言改变时的处理
            if (e.Language == SettingsControl.LanguageOption.Chinese)
            {
                LanguageManager.SwitchLanguage(LanguageManager.Chinese);
            }
            else
            {
                LanguageManager.SwitchLanguage(LanguageManager.English);
            }
            
            // 保存设置
            if (appSettings != null)
            {
                appSettings.Language = SettingsManager.LanguageToString(e.Language);
                SettingsManager.SaveSettings(appSettings);
            }
        }
        
        private void OnAlwaysOnTopChanged(object? sender, SettingsControl.AlwaysOnTopChangedEventArgs e)
        {
            // 窗口置顶状态改变时的处理
            this.TopMost = e.IsAlwaysOnTop;
            
            // 保存设置
            if (appSettings != null)
            {
                appSettings.AlwaysOnTop = e.IsAlwaysOnTop;
                SettingsManager.SaveSettings(appSettings);
            }
        }
        
        private void OnStartStopHotkeyChanged(object? sender, SettingsControl.HotkeyChangedEventArgs e)
        {
            // 更新并重新注册开始/停止快捷键
            currentStartStopHotkey = e.Hotkey;
            RegisterHotkeys();
        }
        
        private void OnPauseHotkeyChanged(object? sender, SettingsControl.HotkeyChangedEventArgs e)
        {
            // 更新并重新注册暂停快捷键
            currentPauseHotkey = e.Hotkey;
            RegisterHotkeys();
        }
        
        private void OnWindowPositionChanged(object? sender, SettingsControl.WindowPositionChangedEventArgs e)
        {
            // 窗口位置改变时的处理
            settingsControl?.MoveWindowToPosition(this, e.Position);
            
            // 保存设置
            if (appSettings != null)
            {
                appSettings.WindowPosition = SettingsManager.WindowPositionToString(e.Position);
                SettingsManager.SaveSettings(appSettings);
            }
        }
        


        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            // 语言改变时更新UI
            UpdateUI();
        }
        private System.Windows.Forms.TabControl? tabControl;
        private System.Windows.Forms.TabPage? tabProfilePage;
        private System.Windows.Forms.TabPage? tabTimerPage;
        private System.Windows.Forms.TabPage? tabPomodoroPage;
        private System.Windows.Forms.TabPage? tabSettingsPage;
        
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 程序关闭时注销热键
            UnregisterHotKeys();
            base.OnFormClosing(e);
        }
    }
}