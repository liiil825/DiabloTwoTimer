using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;

namespace DTwoMFTimerHelper.UI.Timer {
    public partial class HistoryControl : UserControl {
        private System.Windows.Forms.ListBox lstRunHistory;
        private System.Windows.Forms.Label _loadingIndicator;

        private ITimerHistoryService? _historyService;
        private bool _isInitialized = false;

        private const int PageSize = 20;
        private int _displayStartIndex = 0;
        private bool _isLoading = false;
        private bool _isProcessingHistoryChange = false;
        private bool _isFirstLoad = true; // 【新增】标记首次加载

        private CharacterProfile? _currentProfile = null;
        private string? _currentCharacterName = null;
        private string? _currentScene = null;
        private GameDifficulty _currentDifficulty = GameDifficulty.Hell;

        public int RunCount => _historyService?.RunCount ?? 0;
        public TimeSpan FastestTime => _historyService?.FastestTime ?? TimeSpan.Zero;
        public TimeSpan AverageTime => _historyService?.AverageTime ?? TimeSpan.Zero;
        public List<TimeSpan> RunHistory => _historyService?.RunHistory ?? new List<TimeSpan>();

        public HistoryControl() {
            InitializeComponent();
            // 【新增】确保控件已创建
            if (!this.DesignMode) {
                this.Load += HistoryControl_Load;
            }
        }

        // 【新增】控件加载完成事件
        private void HistoryControl_Load(object? sender, EventArgs e) {
            // 确保控件句柄已创建
            if (lstRunHistory != null && lstRunHistory.IsHandleCreated) {
                // 可以在这里执行一些初始化后的操作
            }
        }

        public void Initialize(ITimerHistoryService historyService) {
            if (_isInitialized || historyService == null) return;

            _historyService = historyService;
            _isInitialized = true;

            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            _historyService.HistoryDataChanged += OnHistoryDataChanged;
        }

        private void InitializeComponent() {
            this.lstRunHistory = new System.Windows.Forms.ListBox();
            this._loadingIndicator = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lstRunHistory
            this.lstRunHistory.FormattingEnabled = true;
            this.lstRunHistory.HorizontalScrollbar = true;
            this.lstRunHistory.ItemHeight = 20;
            this.lstRunHistory.Location = new System.Drawing.Point(0, 0);
            this.lstRunHistory.Name = "lstRunHistory";
            this.lstRunHistory.Size = new System.Drawing.Size(290, 90);
            this.lstRunHistory.TabIndex = 0;
            this.lstRunHistory.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.LstRunHistory_MouseWheel);

            // _loadingIndicator
            this._loadingIndicator.AutoSize = true;
            this._loadingIndicator.BackColor = System.Drawing.Color.Transparent;
            this._loadingIndicator.Dock = System.Windows.Forms.DockStyle.Top;
            this._loadingIndicator.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._loadingIndicator.Location = new System.Drawing.Point(0, 0);
            this._loadingIndicator.Name = "_loadingIndicator";
            this._loadingIndicator.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this._loadingIndicator.Size = new System.Drawing.Size(66, 24);
            this._loadingIndicator.TabIndex = 1;
            this._loadingIndicator.Text = "加载中...";
            this._loadingIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._loadingIndicator.Visible = false;

            // HistoryControl
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._loadingIndicator);
            this.Controls.Add(this.lstRunHistory);
            this.Name = "HistoryControl";
            this.Size = new System.Drawing.Size(290, 90);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void AddRunRecord(TimeSpan runTime) {
            if (_historyService == null) return;
            _historyService.AddRunRecord(runTime);
        }

        public void UpdateHistory(List<TimeSpan> runHistory) {
            if (_historyService == null) return;
            _historyService.UpdateHistory(runHistory);
        }

        public async Task<bool> DeleteSelectedRecordAsync() {
            if (_historyService == null) return false;

            if (lstRunHistory == null || lstRunHistory.SelectedIndex == -1 || _currentProfile == null)
                return false;

            try {
                _isLoading = true;
                int actualIndex = _displayStartIndex + lstRunHistory.SelectedIndex;

                if (actualIndex >= 0 && actualIndex < _historyService.RunHistory.Count && _currentScene != null) {
                    bool deleteSuccess = _historyService.DeleteHistoryRecordByIndex(
                        _currentProfile,
                        _currentScene,
                        _currentDifficulty,
                        actualIndex);

                    if (deleteSuccess) {
                        Services.DataService.SaveProfile(_currentProfile);
                        _displayStartIndex = Math.Max(0, Math.Min(_displayStartIndex, _historyService.RunHistory.Count - 1));
                        _isLoading = false;
                        await UpdateUIAsync();
                        return true;
                    }
                }
                return false;
            }
            finally {
                _isLoading = false;
            }
        }

        public async Task<bool> LoadProfileHistoryDataAsync(
            CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty) {
            if (_historyService == null) return false;

            // 【修改】防止重复加载
            if (_isLoading) return false;

            _isLoading = true;
            ShowLoadingIndicator(true);

            try {
                _currentProfile = profile;
                _currentCharacterName = characterName;
                _currentScene = scene;
                _currentDifficulty = difficulty;

                bool result = _historyService.LoadProfileHistoryData(profile, scene, characterName, difficulty);
                if (result) {
                    // 【修改】重置显示起始位置，确保显示最新数据
                    int totalCount = _historyService.RunHistory.Count;
                    _displayStartIndex = Math.Max(0, totalCount - PageSize);

                    // 【新增】强制刷新UI
                    await ForceRefreshUIAsync();
                }
                return result;
            }
            finally {
                _isLoading = false;
                ShowLoadingIndicator(false);
                _isFirstLoad = false;
            }
        }

        // 【新增】强制刷新UI方法
        private async Task ForceRefreshUIAsync() {
            if (_historyService == null || lstRunHistory == null) return;

            await SafeInvokeAsync(() => {
                lstRunHistory.Items.Clear();

                var currentHistory = _historyService.RunHistory;
                if (currentHistory == null || currentHistory.Count == 0) return;

                _displayStartIndex = Math.Max(0, currentHistory.Count - PageSize);
                int displayCount = Math.Min(currentHistory.Count - _displayStartIndex, PageSize);

                for (int i = _displayStartIndex; i < _displayStartIndex + displayCount; i++) {
                    if (i >= 0 && i < currentHistory.Count) {
                        var time = currentHistory[i];
                        string timeFormatted = FormatTime(time);
                        string runText = GetRunText(i + 1, timeFormatted);
                        lstRunHistory.Items.Add(runText);
                    }
                }

                // 【修改】确保滚动到底部
                if (lstRunHistory.Items.Count > 0) {
                    lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                    lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - (lstRunHistory.Height / lstRunHistory.ItemHeight));
                }
            });
        }

        // 【新增】安全的UI调用方法
        private async Task SafeInvokeAsync(Action action) {
            if (lstRunHistory == null) return;

            try {
                if (lstRunHistory.InvokeRequired) {
                    await Task.Run(() => lstRunHistory.Invoke(action));
                }
                else {
                    action();
                }
            }
            catch (Exception ex) {
                // 【新增】记录异常以便调试
                System.Diagnostics.Debug.WriteLine($"[HistoryControl] UI更新异常: {ex.Message}");
            }
        }

        // 【新增】显示/隐藏加载指示器
        private void ShowLoadingIndicator(bool show) {
            if (_loadingIndicator == null) return;

            if (_loadingIndicator.InvokeRequired) {
                _loadingIndicator.Invoke(new Action<bool>(ShowLoadingIndicator), show);
            }
            else {
                _loadingIndicator.Visible = show;
                _loadingIndicator.BringToFront();
            }
        }

        public bool LoadProfileHistoryData(CharacterProfile? profile, string scene, string characterName, GameDifficulty difficulty) {
            // 【修改】同步方法也使用异步方式
            return LoadProfileHistoryDataAsync(profile, scene, characterName, difficulty)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private async Task UpdateUIAsync() {
            if (_historyService == null || lstRunHistory == null) return;
            if (_isLoading) return;

            _isLoading = true;

            try {
                await SafeInvokeAsync(() => {
                    lstRunHistory.Items.Clear();

                    var currentHistory = _historyService.RunHistory;
                    if (currentHistory == null) return;

                    _displayStartIndex = Math.Max(0, Math.Min(_displayStartIndex, currentHistory.Count - 1));
                    int displayCount = Math.Min(currentHistory.Count - _displayStartIndex, PageSize);

                    for (int i = _displayStartIndex; i < _displayStartIndex + displayCount; i++) {
                        if (i >= 0 && i < currentHistory.Count) {
                            var time = currentHistory[i];
                            string timeFormatted = FormatTime(time);
                            string runText = GetRunText(i + 1, timeFormatted);
                            lstRunHistory.Items.Add(runText);
                        }
                    }

                    if (lstRunHistory.Items.Count > 0) {
                        lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                        lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
                    }
                });
            }
            finally {
                _isLoading = false;
            }
        }

        private void ScrollToBottom() {
            if (lstRunHistory != null && lstRunHistory.Items.Count > 0 && lstRunHistory.IsHandleCreated) {
                lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
            }
        }

        public async Task RefreshUIAsync() {
            if (_historyService == null || lstRunHistory == null) return;
            await ForceRefreshUIAsync();
        }

        private void UpdateListItems(List<TimeSpan> currentHistory) {
            if (_historyService == null || lstRunHistory == null) return;
            for (int i = 0; i < lstRunHistory.Items.Count; i++) {
                int actualIndex = _displayStartIndex + i;
                if (actualIndex < currentHistory.Count) {
                    var time = currentHistory[actualIndex];
                    string timeFormatted = FormatTime(time);
                    string runText = GetRunText(actualIndex + 1, timeFormatted);
                    lstRunHistory.Items[i] = runText;
                }
            }
        }

        public void RefreshUI() => _ = RefreshUIAsync();

        private void AddSingleRunRecord(TimeSpan runTime) {
            if (_historyService == null || lstRunHistory == null) return;
            var currentHistory = _historyService.RunHistory;
            if (currentHistory == null) return;

            int newIndex = currentHistory.Count - 1;
            _displayStartIndex = Math.Max(0, newIndex - PageSize + 1);

            string timeFormatted = FormatTime(runTime);
            string runText = GetRunText(newIndex + 1, timeFormatted);

            SafeInvokeAsync(() => {
                if (lstRunHistory.Items.Count >= PageSize)
                    lstRunHistory.Items.RemoveAt(0);

                lstRunHistory.Items.Add(runText);
                lstRunHistory.SelectedIndex = lstRunHistory.Items.Count - 1;
                lstRunHistory.TopIndex = Math.Max(0, lstRunHistory.Items.Count - 1);
            }).ConfigureAwait(false);
        }

        private async void LstRunHistory_MouseWheel(object? sender, MouseEventArgs e) {
            if (_historyService == null || lstRunHistory == null) return;
            if (e != null && e.Delta > 0 && _displayStartIndex > 0 && !_isLoading && lstRunHistory.TopIndex < 5) {
                await LoadMoreHistoryAsync();
            }
        }

        private async Task LoadMoreHistoryAsync() {
            if (_historyService == null || lstRunHistory == null || _loadingIndicator == null || _displayStartIndex <= 0 || _isLoading) return;

            _isLoading = true;
            ShowLoadingIndicator(true);

            try {
                int newStartIndex = Math.Max(0, _displayStartIndex - PageSize);
                int addedCount = _displayStartIndex - newStartIndex;
                var currentHistory = _historyService.RunHistory;
                if (currentHistory == null || currentHistory.Count == 0) return;

                await SafeInvokeAsync(() => {
                    var newItems = new List<string>(addedCount);
                    for (int i = newStartIndex; i < _displayStartIndex; i++) {
                        if (i >= 0 && i < currentHistory.Count) {
                            var time = currentHistory[i];
                            string timeFormatted = FormatTime(time);
                            string runText = GetRunText(i + 1, timeFormatted);
                            newItems.Add(runText);
                        }
                    }

                    if (newItems.Count == 0) return;
                    _displayStartIndex = newStartIndex;

                    var currentDisplayItems = new List<string>();
                    foreach (var item in lstRunHistory.Items)
                        currentDisplayItems.Add(item?.ToString() ?? string.Empty);

                    lstRunHistory.Items.Clear();
                    foreach (var item in newItems) lstRunHistory.Items.Add(item);
                    foreach (var item in currentDisplayItems) lstRunHistory.Items.Add(item);

                    if (newItems.Count > 0) {
                        lstRunHistory.TopIndex = 0;
                        lstRunHistory.SelectedIndex = -1;
                    }
                });
            }
            finally {
                _isLoading = false;
                ShowLoadingIndicator(false);
            }
        }

        private string FormatTime(TimeSpan time) {
            return string.Format("{0:00}:{1:00}:{2:00}.{3}",
                time.Hours, time.Minutes, time.Seconds,
                (int)(time.Milliseconds / 100));
        }

        private string GetRunText(int runNumber, string timeFormatted) {
            string runText = Utils.LanguageManager.GetString("RunNumber", runNumber, timeFormatted);
            if (string.IsNullOrEmpty(runText) || runText == "RunNumber") {
                runText = $"Run {runNumber}: {timeFormatted}";
            }
            return runText;
        }

        private void OnHistoryDataChanged(object? sender, HistoryChangedEventArgs e) {
            if (e == null) return;

            // 【修改】增加调试日志
            System.Diagnostics.Debug.WriteLine($"[HistoryControl] 收到历史数据变更事件: {e.ChangeType}");

            if (InvokeRequired) {
                BeginInvoke(new Action<HistoryChangedEventArgs>(ProcessHistoryChange), e);
            }
            else {
                ProcessHistoryChange(e);
            }
        }

        private async void ProcessHistoryChange(HistoryChangedEventArgs e) {
            if (e == null) return;
            if (_isProcessingHistoryChange) return;

            _isProcessingHistoryChange = true;
            try {
                System.Diagnostics.Debug.WriteLine($"[HistoryControl] 处理历史数据变更: {e.ChangeType}");

                switch (e.ChangeType) {
                    case HistoryChangeType.Add:
                        if (e.AddedRecord.HasValue) {
                            AddSingleRunRecord(e.AddedRecord.Value);
                        }
                        break;
                    case HistoryChangeType.FullRefresh:
                    default:
                        _isLoading = false;
                        if (_historyService != null && _historyService.RunHistory != null) {
                            _displayStartIndex = Math.Max(0, _historyService.RunHistory.Count - PageSize);
                            await ForceRefreshUIAsync();
                        }
                        break;
                }
            }
            finally {
                _isProcessingHistoryChange = false;
            }
        }

        private async void LanguageManager_OnLanguageChanged(object? sender, EventArgs e) {
            if (e == null) return;
            await RefreshUIAsync();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                if (_historyService != null) _historyService.HistoryDataChanged -= OnHistoryDataChanged;
                if (lstRunHistory != null) lstRunHistory.MouseWheel -= LstRunHistory_MouseWheel;
                this.Load -= HistoryControl_Load;
            }
            base.Dispose(disposing);
        }
    }
}