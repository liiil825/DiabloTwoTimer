using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
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

    // 【新功能】暴露交互事件
    public event EventHandler? InteractionOccurred;

    // 属性保持不变...
    public int RunCount => _historyService?.RunCount ?? 0;
    public TimeSpan FastestTime => _historyService?.FastestTime ?? TimeSpan.Zero;
    public TimeSpan AverageTime => _historyService?.AverageTime ?? TimeSpan.Zero;
    public List<TimeSpan> RunHistory => _historyService?.RunHistory ?? [];

    public HistoryControl()
    {
        InitializeComponent();
    }

    // Initialize, GridRunHistory_CellValueNeeded 等方法保持不变 ...
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

    // ... LoadProfileHistoryData, RefreshGridRowCount, DeleteSelectedRecordAsync 保持不变 ...
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
            // 关键 1：先清除选中项，防止索引越界
            // gridRunHistory.ClearSelection();
            // gridRunHistory.CurrentCell = null;
            gridRunHistory.RowCount = count;
            gridRunHistory.Invalidate();
        });
    }

    public void SelectLastRow()
    {
        gridRunHistory.SafeInvoke(() =>
        {
            // 同步行数防止异步问题
            if (_historyService != null && gridRunHistory.RowCount != _historyService.RunHistory.Count)
            {
                gridRunHistory.RowCount = _historyService.RunHistory.Count;
            }

            if (gridRunHistory.RowCount > 0)
            {
                int lastIndex = gridRunHistory.RowCount - 1;
                gridRunHistory.Focus();
                gridRunHistory.FirstDisplayedScrollingRowIndex = lastIndex;
                gridRunHistory.CurrentCell = gridRunHistory.Rows[lastIndex].Cells[0];
                gridRunHistory.Rows[lastIndex].Selected = true;
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

    // ... 其他辅助方法保持不变 ...

    public async Task<bool> DeleteSelectedRecordAsync()
    {
        if (_historyService == null || gridRunHistory.SelectedRows.Count == 0 || _currentProfile == null)
            return false;
        int index = gridRunHistory.SelectedRows[0].Index;
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
        return await Task.FromResult(success);
    }

    // 响应 Service 数据变更
    private void OnHistoryDataChanged(object? sender, HistoryChangedEventArgs e)
    {
        if (e == null)
            return;
        switch (e.ChangeType)
        {
            // 注意：这里我们不再在 Add 事件里处理 SelectLastRow，
            // 因为我们希望由 TimerControl 统一调度（添加后，清除Loot选中，再选中History），
            // 避免事件冲突。所以这里只负责更新行数。
            case HistoryChangeType.Add:
            case HistoryChangeType.FullRefresh:
            default:
                RefreshGridRowCount();
                break;
        }
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            if (_historyService != null)
                _historyService.HistoryDataChanged -= OnHistoryDataChanged;
        }
        base.Dispose(disposing);
    }
}
