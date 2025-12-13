using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class HistoryControl : UserControl
{
    private ITimerHistoryService? _historyService = null!;
    private IProfileService _profileService = null!;

    private bool _isInitialized = false;

    private CharacterProfile? _currentProfile = null;
    private string? _currentScene = null;
    private GameDifficulty _currentDifficulty = GameDifficulty.Hell;

    public event EventHandler? InteractionOccurred = null;

    public int RunCount => _historyService?.RunCount ?? 0;
    public TimeSpan FastestTime => _historyService?.FastestTime ?? TimeSpan.Zero;
    public TimeSpan AverageTime => _historyService?.AverageTime ?? TimeSpan.Zero;
    public List<TimeSpan> RunHistory => _historyService?.RunHistory ?? [];

    private bool _isDeleting = false;

    public HistoryControl()
    {
        InitializeComponent();
    }

    public void Initialize(ITimerHistoryService historyService, IProfileService profileService)
    {
        if (_isInitialized || historyService == null)
            return;
        _historyService = historyService;
        _profileService = profileService;
        _isInitialized = true;
        LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        _historyService.HistoryDataChanged += OnHistoryDataChanged;
        UpdateColumnHeaderLanguage();
    }

    private void GridRunHistory_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (_historyService == null || _historyService.RunHistory == null)
            return;
        if (e.RowIndex >= 0 && e.RowIndex < _historyService.RunHistory.Count)
        {
            if (e.ColumnIndex == 0)
                e.Value = (e.RowIndex + 1).ToString();
            else if (e.ColumnIndex == 1)
                e.Value = FormatTime(_historyService.RunHistory[e.RowIndex]);
        }
    }

    public bool LoadProfileHistoryData(
        CharacterProfile? profile,
        string scene,
        string characterName,
        GameDifficulty difficulty
    )
    {
        if (_historyService == null)
            return false;
        _currentProfile = profile;
        _currentScene = scene;
        _currentDifficulty = difficulty;
        bool result = _historyService.LoadProfileHistoryData(profile, scene, characterName, difficulty);
        if (result)
            RefreshGridRowCount();
        return result;
    }

    private void RefreshGridRowCount()
    {
        gridRunHistory.SafeInvoke(() =>
        {
            var count = _historyService?.RunHistory?.Count ?? 0;
            LogManager.WriteDebugLog("RefreshGridRowCount", $"刷新运行记录网格行计数: {count}");
            gridRunHistory.RowCount = count;
            gridRunHistory.Invalidate();
        });
    }

    public void SelectLastRow()
    {
        // 关键防御：如果控件还没显示或没高度，不要操作
        if (!this.IsHandleCreated || !this.Visible || this.Height <= 0) return;

        gridRunHistory.SafeInvoke(() =>
        {
            if (_historyService != null && gridRunHistory.RowCount != _historyService.RunHistory.Count)
            {
                gridRunHistory.RowCount = _historyService.RunHistory.Count;
            }

            if (gridRunHistory.RowCount > 0)
            {
                int lastIndex = gridRunHistory.RowCount - 1;
                // 再次检查 DisplayedRowCount
                if (gridRunHistory.DisplayedRowCount(false) > 0)
                {
                    gridRunHistory.FirstDisplayedScrollingRowIndex = lastIndex;
                    gridRunHistory.ClearSelection();
                    if (gridRunHistory.Rows.Count > lastIndex)
                    {
                        gridRunHistory.Rows[lastIndex].Selected = true;
                        // 只有当 Visible 时才尝试 Focus，否则会报错
                        if (gridRunHistory.Visible)
                            gridRunHistory.CurrentCell = gridRunHistory.Rows[lastIndex].Cells[0];
                    }
                }
            }
        });
    }

    public void ScrollToBottom()
    {
        // 关键防御
        if (!this.IsHandleCreated || !this.Visible || this.Height <= 0) return;

        gridRunHistory.SafeInvoke(() =>
        {
            if (gridRunHistory.RowCount > 0)
            {
                int displayedCount = gridRunHistory.DisplayedRowCount(false);
                if (displayedCount == 0) return;

                int firstVisible = gridRunHistory.RowCount - displayedCount;
                if (firstVisible < 0) firstVisible = 0;

                gridRunHistory.FirstDisplayedScrollingRowIndex = firstVisible;
            }
        });
    }
    public void ClearSelection()
    {
        gridRunHistory.SafeInvoke(() =>
        {
            gridRunHistory.ClearSelection();
            gridRunHistory.CurrentCell = null;
        });
    }

    public Task<bool> DeleteSelectedRecordAsync()
    {
        if (_isDeleting) return Task.FromResult(false);

        if (_historyService == null || gridRunHistory.SelectedRows.Count == 0 || _currentProfile == null)
            return Task.FromResult(false);

        try
        {
            // 【新增】设置标志位
            _isDeleting = true;

            int index = gridRunHistory.SelectedRows[0].Index;
            if (index < 0 || index >= _historyService.RunHistory.Count)
                return Task.FromResult(false);

            var timeSpan = _historyService.RunHistory[index];
            string timeFormatted = FormatTime(timeSpan);
            int runNumber = index + 1;

            string message = LanguageManager.GetString("DeleteHistoryConfirm", runNumber, timeFormatted);

            // 显示弹窗
            var result = ThemedMessageBox.Show(message, LanguageManager.GetString("DeleteConfirmTitle"), MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
            {
                return Task.FromResult(false);
            }

            // 执行删除
            bool success = _historyService.DeleteHistoryRecordByIndex(
                _currentProfile,
                _currentScene!,
                _currentDifficulty,
                index
            );

            if (success)
            {
                _profileService.SaveCurrentProfile();
                RefreshGridRowCount();
            }
            return Task.FromResult(success);
        }
        finally
        {
            // 【新增】重置标志位
            _isDeleting = false;
        }
    }

    private void OnHistoryDataChanged(object? sender, HistoryChangedEventArgs e)
    {
        if (e == null) return;
        RefreshGridRowCount();
    }

    private string FormatTime(TimeSpan time) =>
        string.Format(
            "{0:00}:{1:00}:{2:00}.{3}",
            time.Hours,
            time.Minutes,
            time.Seconds,
            (int)(time.Milliseconds / 100)
        );

    private void UpdateColumnHeaderLanguage()
    {
        if (gridRunHistory.Columns.Count > 1)
            gridRunHistory.Columns[1].HeaderText = Utils.LanguageManager.GetString("Time", "Time");
    }

    private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateColumnHeaderLanguage();
        gridRunHistory.Invalidate();
    }

    public void AddRunRecord(TimeSpan runTime) => _historyService?.AddRunRecord(runTime);

    public void UpdateHistory(List<TimeSpan> runHistory) => _historyService?.UpdateHistory(runHistory);

    public void RefreshUI() => RefreshGridRowCount();
}