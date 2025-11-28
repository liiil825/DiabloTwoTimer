using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer
{
    public partial class LootRecordsControl : UserControl
    {
        private CharacterProfile? _currentProfile;
        private List<LootRecord> _displayRecords = [];
        private string _currentScene = string.Empty;

        public event EventHandler? InteractionOccurred;

        public LootRecordsControl()
        {
            InitializeComponent();
        }

        private void GridLoot_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
        {
            if (_displayRecords == null || e.RowIndex >= _displayRecords.Count) return;

            var record = _displayRecords[e.RowIndex];
            switch (e.ColumnIndex)
            {
                case 0: e.Value = record.RunCount.ToString(); break;
                case 1: e.Value = record.Name; break;
                case 2: e.Value = record.DropTime.ToString("MM-dd HH:mm"); break;
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

            string pureEnglishCurrentScene = SceneHelper.GetEnglishSceneName(_currentScene);

            var query = string.IsNullOrEmpty(_currentScene)
                ? _currentProfile.LootRecords
                : _currentProfile.LootRecords.Where(r => r.SceneName == pureEnglishCurrentScene);

            _displayRecords = query.OrderByDescending(r => r.DropTime).ToList();

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            if (gridLoot.InvokeRequired)
            {
                gridLoot.Invoke(new Action(RefreshGrid));
                return;
            }
            gridLoot.RowCount = _displayRecords.Count;
            gridLoot.Invalidate();
            // 【关键修改】这里不再强制清除选中，避免打断父控件的焦点控制逻辑
        }

        public async Task<bool> DeleteSelectedLootAsync()
        {
            if (_currentProfile == null || gridLoot.SelectedRows.Count == 0)
                return false;

            int visualIndex = gridLoot.SelectedRows[0].Index;
            if (visualIndex < 0 || visualIndex >= _displayRecords.Count) return false;

            var recordToDelete = _displayRecords[visualIndex];
            bool removed = _currentProfile.LootRecords.Remove(recordToDelete);

            if (removed)
            {
                DataHelper.SaveProfile(_currentProfile);
                UpdateLootRecords(_currentProfile, _currentScene);
                return true;
            }
            return await Task.FromResult(false);
        }

        public void ClearSelection()
        {
            if (gridLoot.InvokeRequired)
            {
                gridLoot.Invoke(new Action(ClearSelection));
                return;
            }
            gridLoot.ClearSelection();
            gridLoot.CurrentCell = null;
        }

        public bool HasFocus => gridLoot.ContainsFocus;
    }
}