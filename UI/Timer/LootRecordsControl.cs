using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class LootRecordsControl : UserControl
{
    private CharacterProfile? _currentProfile;
    private IProfileService _profileService = null!;
    private ISceneService _sceneService = null!;

    private List<LootRecord> _displayRecords = [];
    private string _currentScene = string.Empty;

    // 【新增】防抖标志位
    private bool _isDeleting = false;

    public event EventHandler? InteractionOccurred = null;

    public LootRecordsControl()
    {
        InitializeComponent();
        D2ScrollHelper.Attach(this.gridLoot, this);
    }

    public void Initialize(IProfileService profileService, ISceneService sceneService)
    {
        _profileService = profileService;
        _sceneService = sceneService;
    }

    private void GridLoot_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
    {
        if (_displayRecords == null || e.RowIndex >= _displayRecords.Count)
            return;

        var record = _displayRecords[e.RowIndex];
        switch (e.ColumnIndex)
        {
            case 0: // Index
                e.Value = record.RunCount.ToString();
                break;
            case 1: // Name
                e.Value = record.Name;
                break;
            case 2: // Time
                e.Value = record.DropTime.ToString("MM-dd HH:mm");
                break;
        }
    }

    public void UpdateLootRecords(CharacterProfile? profile, string currentScene)
    {
        _currentProfile = profile;
        _currentScene = currentScene;

        if (_currentProfile == null)
        {
            _displayRecords = [];
            RefreshGrid();
            return;
        }

        string pureEnglishCurrentScene = _sceneService.GetEnglishSceneName(_currentScene);

        var query = _currentProfile.LootRecords.AsEnumerable();

        // 场景过滤
        if (!string.IsNullOrEmpty(_currentScene))
        {
            query = query.Where(r => r.SceneName == pureEnglishCurrentScene);
        }

        _displayRecords = [.. query.OrderBy(r => r.DropTime)];

        RefreshGrid();
    }

    private void RefreshGrid()
    {
        gridLoot.SafeInvoke(() =>
        {
            gridLoot.ClearSelection();
            gridLoot.CurrentCell = null;
            gridLoot.RowCount = _displayRecords.Count;
            gridLoot.Invalidate();
        });
    }

    public void SelectLastRow()
    {
        if (!this.IsHandleCreated || !this.Visible || this.Height <= 0)
            return;

        gridLoot.SafeInvoke(() =>
        {
            if (gridLoot.RowCount > 0)
            {
                if (gridLoot.DisplayedRowCount(false) == 0)
                    return;

                int lastIndex = gridLoot.RowCount - 1;
                gridLoot.FirstDisplayedScrollingRowIndex = lastIndex;
                gridLoot.ClearSelection();
                gridLoot.Rows[lastIndex].Selected = true;
                if (gridLoot.Visible)
                    gridLoot.CurrentCell = gridLoot.Rows[lastIndex].Cells[0];
            }
        });
    }

    public void ScrollToBottom()
    {
        if (!this.IsHandleCreated || !this.Visible || this.Height <= 0)
            return;

        gridLoot.SafeInvoke(() =>
        {
            if (gridLoot.RowCount > 0)
            {
                int displayCount = gridLoot.DisplayedRowCount(false);
                if (displayCount == 0)
                    return;

                int firstVisible = gridLoot.RowCount - displayCount;
                if (firstVisible < 0)
                    firstVisible = 0;
                gridLoot.FirstDisplayedScrollingRowIndex = firstVisible;
                int lastIndex = gridLoot.RowCount - 1;
                // 为了配合【优化2】，这里最好也做一个限制，不要直接置顶最后一行
                // 但简单起见，直接设为 lastIndex 也是可以的，ScrollHelper 会尽力修正
                gridLoot.FirstDisplayedScrollingRowIndex = lastIndex;
            }
        });
    }

    public Task<bool> DeleteSelectedLootAsync()
    {
        if (_isDeleting)
            return Task.FromResult(false);
        if (_currentProfile == null || gridLoot.SelectedRows.Count == 0)
            return Task.FromResult(false);
        try
        {
            // 【新增】设置标志位
            _isDeleting = true;

            int visualIndex = gridLoot.SelectedRows[0].Index;
            if (visualIndex < 0 || visualIndex >= _displayRecords.Count)
                return Task.FromResult(false);

            var recordToDelete = _displayRecords[visualIndex];

            // 构建信息
            string sceneName = _sceneService.GetLocalizedShortSceneName(recordToDelete.SceneName);
            string dropTime = recordToDelete.DropTime.ToString("yyyy-MM-dd HH:mm:ss");
            string message = LanguageManager.GetString("DeleteLootConfirm", recordToDelete.Name, sceneName, dropTime);

            // 显示弹窗 (模态)
            var result = ThemedMessageBox.Show(
                message,
                LanguageManager.GetString("DeleteConfirmTitle"),
                MessageBoxButtons.YesNo
            );

            if (result != DialogResult.Yes)
            {
                return Task.FromResult(false);
            }

            // 执行删除
            bool removed = _currentProfile.LootRecords.Remove(recordToDelete);

            if (removed)
            {
                _profileService.SaveCurrentProfile();
                UpdateLootRecords(_currentProfile, _currentScene);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        finally
        {
            // 【新增】无论结果如何，必须重置标志位
            _isDeleting = false;
        }
    }

    public void ClearSelection()
    {
        gridLoot.SafeInvoke(() =>
        {
            gridLoot.ClearSelection();
            gridLoot.CurrentCell = null;
        });
    }

    public bool HasFocus => gridLoot.ContainsFocus;

    public bool HasSelectedRecords => gridLoot.SelectedRows.Count > 0;
}
