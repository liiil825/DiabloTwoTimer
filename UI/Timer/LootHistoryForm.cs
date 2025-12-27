using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class LootHistoryForm : System.Windows.Forms.Form
{
    private readonly IMainService _mainService = null!;
    private readonly IProfileService _profileService = null!;
    private readonly ISceneService _sceneService = null!;
    private readonly IStatisticsService _statisticsService = null!;
    private readonly IMessenger _messenger = null!;

    private Button btnToday = null!;
    private Button btnWeek = null!;
    private Button btnCustom = null!;

    private System.Windows.Forms.Timer _fadeInTimer;
    private readonly List<Label> _shortcutBadges = new();
    private bool _showShortcuts = false;

    // --- 新增：专门用于当前行的动态 Badge ---
    private Label? _lblRowEditBadge;
    private Label? _lblRowDelBadge;

    private const string ICON_CLOSE = "\uE711";

    private enum ViewMode
    {
        Today,
        Week,
        Custom,
    }

    private ViewMode _currentMode = ViewMode.Today;

    private class LootViewModel
    {
        public LootRecord OriginalRecord { get; set; } = null!;
        public DateTime DropTime => OriginalRecord.DropTime;
        public string Name
        {
            get => OriginalRecord.Name;
            set => OriginalRecord.Name = value;
        }
        public string SceneName { get; set; } = string.Empty;
        public int RunCount => OriginalRecord.RunCount;
    }

    public LootHistoryForm(
        IMainService mainService,
        IProfileService profileService,
        ISceneService sceneService,
        IStatisticsService statisticsService,
        IMessenger messenger
    )
    {
        _profileService = profileService;
        _sceneService = sceneService;
        _statisticsService = statisticsService;
        _mainService = mainService;
        _messenger = messenger;

        InitializeComponent();
        InitializeToggleButtons();
        ApplyScaledLayout();

        btnClose.Font = Theme.AppTheme.Fonts.SegoeIcon;
        btnClose.Text = ICON_CLOSE;
        btnClose.Width = ScaleHelper.Scale(50);
        btnClose.Height = ScaleHelper.Scale(50);

        D2ScrollHelper.Attach(this.gridLoot, this.pnlGridContainer);

        LanguageManager.OnLanguageChanged += LanguageChanged;

        this.Resize += LootHistoryForm_Resize;

        this.KeyPreview = true;
        this.KeyDown += LootHistoryForm_KeyDown;

        AttachKeyBadge(dtpStart, "F", () => dtpStart.OpenDropdown());
        AttachKeyBadge(dtpEnd, "G", () => dtpEnd.OpenDropdown());
        AttachKeyBadge(btnSearch, "R", () => btnSearch.PerformClick());
        AttachKeyBadge(btnClose, "X", () => btnClose.PerformClick());

        gridLoot.AutoGenerateColumns = false;
        // 既然没有按钮列了，CellClick 就不需要处理编辑删除了，全部走键盘或双击
        // 为了方便鼠标用户，可以保留双击编辑
        gridLoot.CellDoubleClick += (s, e) => PerformEdit();

        // --- 新增：监听表格事件以更新动态 Badge 位置 ---
        gridLoot.SelectionChanged += (s, e) => UpdateRowBadges();
        gridLoot.Scroll += (s, e) => UpdateRowBadges();
        gridLoot.Resize += (s, e) => UpdateRowBadges();

        this.CancelButton = btnClose;

        this.Opacity = 0;
        _fadeInTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _fadeInTimer.Tick += FadeInTimer_Tick;

        pnlGridContainer.TabIndex = 0;
        gridLoot.TabIndex = 0;
        headerControl.TabIndex = 1;
        pnlCustomDate.TabIndex = 2;
        panelButtons.TabIndex = 3;

        UpdateLanguageText();
        SwitchMode(ViewMode.Today);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        bool isDateFocus = pnlCustomDate.Visible && (dtpStart.ContainsFocus || dtpEnd.ContainsFocus);
        bool isGridFocus = gridLoot.ContainsFocus;

        if (isDateFocus || isGridFocus)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.W:
                        msg.WParam = (IntPtr)Keys.Up;
                        return base.ProcessCmdKey(ref msg, Keys.Up);
                    case Keys.S:
                        msg.WParam = (IntPtr)Keys.Down;
                        return base.ProcessCmdKey(ref msg, Keys.Down);
                }
            }
        }
        if (isDateFocus)
        {
            switch (keyData)
            {
                case Keys.A:
                    msg.WParam = (IntPtr)Keys.Left;
                    return base.ProcessCmdKey(ref msg, Keys.Left);
                case Keys.D:
                    msg.WParam = (IntPtr)Keys.Right;
                    return base.ProcessCmdKey(ref msg, Keys.Right);
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void LootHistoryForm_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.H:
                ToggleShortcutsVisibility();
                e.SuppressKeyPress = true;
                break;
            case Keys.D1:
            case Keys.NumPad1:
                btnToday?.PerformClick();
                break;
            case Keys.D2:
            case Keys.NumPad2:
                btnWeek?.PerformClick();
                break;
            case Keys.D3:
            case Keys.NumPad3:
                btnCustom?.PerformClick();
                break;

            case Keys.F:
                if (pnlCustomDate.Visible)
                {
                    dtpStart.OpenDropdown();
                    e.SuppressKeyPress = true;
                }
                break;
            case Keys.G:
                if (pnlCustomDate.Visible && !dtpEnd.ContainsFocus)
                {
                    dtpEnd.OpenDropdown();
                    e.SuppressKeyPress = true;
                }
                break;
            case Keys.R:
                if (pnlCustomDate.Visible)
                {
                    btnSearch.PerformClick();
                    e.SuppressKeyPress = true;
                    gridLoot.Focus();

                    // 【优化】如果有数据，自动选中第一行，方便直接开始 W/S 导航
                    if (gridLoot.Rows.Count > 0 && gridLoot.Columns.Count > 0)
                    {
                        // 确保没有选中其他奇怪的地方，重置到起点
                        gridLoot.CurrentCell = gridLoot.Rows[0].Cells[0];
                    }
                }
                break;

            // --- 修改：D 键现在触发删除 ---
            case Keys.D:
                PerformDelete();
                e.SuppressKeyPress = true;
                break;

            // --- 修改：E 键触发编辑 ---
            case Keys.E:
                PerformEdit();
                e.SuppressKeyPress = true;
                break;

            case Keys.X:
                if (btnClose.Visible && btnClose.Enabled)
                    btnClose.PerformClick();
                break;
        }
    }

    private LootViewModel? GetSelectedViewModel()
    {
        if (gridLoot.CurrentRow == null || gridLoot.CurrentRow.DataBoundItem == null)
            return null;
        return gridLoot.CurrentRow.DataBoundItem as LootViewModel;
    }

    private void PerformEdit()
    {
        var item = GetSelectedViewModel();
        if (item == null)
            return;

        using var editForm = new EditNameForm(item.Name);
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
            item.Name = editForm.NewName;
            _profileService.SaveCurrentProfile();
            gridLoot.Refresh();
            gridLoot.Focus();
        }
    }

    private void PerformDelete()
    {
        var item = GetSelectedViewModel();
        if (item == null)
            return;

        string msgTemplate =
            LanguageManager.GetString("LootDeleteConfirm") ?? "Are you sure you want to delete item:\n'{0}'?";
        string msgTitle = LanguageManager.GetString("LootDeleteTitle") ?? "Delete Confirmation";
        var result = ThemedMessageBox.Show(string.Format(msgTemplate, item.Name), msgTitle, MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
            _profileService.CurrentProfile!.LootRecords.Remove(item.OriginalRecord);
            _profileService.SaveCurrentProfile();
            if (gridLoot.DataSource is BindingSource bs)
                bs.Remove(item);
            gridLoot.Focus();
        }
    }

    // --- Badge 系统 ---

    private void ToggleShortcutsVisibility()
    {
        _showShortcuts = !_showShortcuts;

        // 1. 全局静态 Badge
        foreach (var badge in _shortcutBadges)
        {
            if (badge.Parent != null && badge.Parent.Visible)
                badge.Visible = _showShortcuts;
            else
                badge.Visible = false;
        }

        // 2. 更新动态行 Badge
        UpdateRowBadges();
    }

    // --- 核心新功能：动态行 Badge ---
    // 这个方法会计算当前选中行的位置，把 [E 编辑] [D 删除] 贴在行旁边
    private void UpdateRowBadges()
    {
        // 如果 H 模式未开启，或者没有选中行，直接隐藏
        if (!_showShortcuts || gridLoot.CurrentRow == null || !gridLoot.Focused)
        {
            if (_lblRowEditBadge != null)
                _lblRowEditBadge.Visible = false;
            if (_lblRowDelBadge != null)
                _lblRowDelBadge.Visible = false;
            return;
        }

        // 初始化动态 Badge
        if (_lblRowEditBadge == null)
        {
            string editText = LanguageManager.GetString("LootEditBadge") ?? "E Edit";
            _lblRowEditBadge = CreateBadge(editText); // 带文字说明
            _lblRowEditBadge.BackColor = AppTheme.AccentColor; // 用醒目颜色
            _lblRowEditBadge.ForeColor = Color.Black;
            pnlGridContainer.Controls.Add(_lblRowEditBadge);
            _lblRowEditBadge.BringToFront();
        }
        if (_lblRowDelBadge == null)
        {
            string delText = LanguageManager.GetString("LootDeleteBadge") ?? "D Delete";
            _lblRowDelBadge = CreateBadge(delText);
            _lblRowDelBadge.BackColor = Color.IndianRed;
            _lblRowDelBadge.ForeColor = Color.White;
            pnlGridContainer.Controls.Add(_lblRowDelBadge);
            _lblRowDelBadge.BringToFront();
        }

        // 获取当前行的矩形区域
        // 注意：GetRowDisplayRectangle 只返回可见部分的坐标
        Rectangle rect = gridLoot.GetRowDisplayRectangle(gridLoot.CurrentRow.Index, false);

        // 如果行完全滚出去了 (Height = 0)
        if (rect.Height == 0)
        {
            _lblRowEditBadge.Visible = false;
            _lblRowDelBadge.Visible = false;
            return;
        }

        // 计算坐标 (相对于 pnlGridContainer)
        // gridLoot.Location 应该接近 (0,0)
        int baseY = gridLoot.Location.Y + rect.Top + (rect.Height - _lblRowEditBadge.Height) / 2;

        // 放在行的最右侧，向左排列
        int rightMargin = gridLoot.Width - 20; // 留点边距

        // D (Delete) 在最右
        _lblRowDelBadge.Location = new Point(rightMargin - _lblRowDelBadge.Width, baseY);

        // E (Edit) 在 D 的左边
        _lblRowEditBadge.Location = new Point(_lblRowDelBadge.Left - 10 - _lblRowEditBadge.Width, baseY);

        _lblRowEditBadge.Visible = true;
        _lblRowDelBadge.Visible = true;
    }

    private Label CreateBadge(string text)
    {
        return new Label
        {
            Text = text,
            Font = new Font("Consolas", 8F, FontStyle.Bold),
            ForeColor = Color.Gold,
            BackColor = Color.FromArgb(200, 0, 0, 0),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            Visible = false,
        };
    }

    private void AttachKeyBadge(Control target, string keyText, Action triggerAction)
    {
        var lblBadge = CreateBadge(keyText);
        // 先不设置 Location
        lblBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblBadge.Click += (s, e) => triggerAction();

        // 先加入控件，让它计算出自己的 Width
        target.Controls.Add(lblBadge);
        lblBadge.BringToFront();

        lblBadge.Location = new Point(target.Width - lblBadge.Width - 2, 2);

        _shortcutBadges.Add(lblBadge);
    }

    private void SetupGridColumns()
    {
        gridLoot.Columns.Clear();
        gridLoot.ColumnHeadersHeight = ScaleHelper.Scale(40);
        gridLoot.RowTemplate.Height = ScaleHelper.Scale(35);
        gridLoot.BackgroundColor = Color.FromArgb(32, 32, 32);

        // --- 核心修改：移除 Edit 和 Del 列 ---

        var colTime = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableDropTime"),
            Width = ScaleHelper.Scale(200),
            DataPropertyName = "DropTime",
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "yyyy-MM-dd HH:mm",
                Alignment = DataGridViewContentAlignment.MiddleCenter,
            },
        };
        var colName = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableItemName"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            DataPropertyName = "Name",
        };
        colName.DefaultCellStyle.Font = new Font(AppTheme.MainFont, FontStyle.Bold);
        colName.DefaultCellStyle.ForeColor = AppTheme.AccentColor;
        var colScene = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableScene"),
            Width = ScaleHelper.Scale(220),
            DataPropertyName = "SceneName",
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
        };
        var colRun = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableRun"),
            Width = ScaleHelper.Scale(100),
            DataPropertyName = "RunCount",
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
        };

        // 只添加数据列
        gridLoot.Columns.AddRange([colTime, colName, colScene, colRun]);
    }

    // --- ApplyScaledLayout, Resize, LoadData, etc. 保持不变 ---
    // 为了完整性，保留核心布局代码
    private void ApplyScaledLayout()
    {
        mainLayout.RowStyles[0].Height = ScaleHelper.Scale(110);
        mainLayout.RowStyles[1] = new RowStyle(SizeType.Absolute, ScaleHelper.Scale(60));
        int ctrlHeight = ScaleHelper.Scale(20);
        dtpStart.Size = new Size(ScaleHelper.Scale(160), ctrlHeight);
        dtpEnd.Size = new Size(ScaleHelper.Scale(160), ctrlHeight);
        btnSearch.AutoSize = false;
        btnSearch.Size = new Size(ScaleHelper.Scale(80), ctrlHeight);
        lblSeparator.AutoSize = false;
        lblSeparator.Size = new Size(ScaleHelper.Scale(20), ctrlHeight);
        lblSeparator.TextAlign = ContentAlignment.MiddleCenter;
        lblSeparator.Text = "-";
        int spacing = ScaleHelper.Scale(10);
        dtpStart.Margin = new Padding(0, 0, spacing, 0);
        lblSeparator.Margin = new Padding(0, 0, spacing, 0);
        dtpEnd.Margin = new Padding(0, 0, spacing, 0);

        pnlGridContainer.Margin = new Padding(0, ScaleHelper.Scale(10), 0, ScaleHelper.Scale(30));

        // 底部按钮区域
        mainLayout.Padding = new Padding(0);
    }

    private void LootHistoryForm_Resize(object? sender, EventArgs e)
    {
        if (headerControl != null && headerControl.TogglePanel != null)
            headerControl.TogglePanel.Left = (headerControl.Width - headerControl.TogglePanel.Width) / 2;
        if (pnlGridContainer != null)
        {
            int max = ScaleHelper.Scale(1200);
            int w = Math.Min(this.ClientSize.Width - ScaleHelper.Scale(60), max);
            pnlGridContainer.Width = w;
        }
        // 窗口大小改变时，也要更新动态 Badge 位置
        UpdateRowBadges();
    }

    private class EditNameForm : System.Windows.Forms.Form
    {
        public string NewName => txtName.Text.Trim();
        private ThemedTextBox txtName;
        private ThemedModalButton btnSave;
        private ThemedModalButton btnCancel;
        private readonly Font _iconFont = Theme.AppTheme.Fonts.SegoeIcon;
        private const string ICON_CHECK = "\uE73E";
        private const string ICON_CANCEL = "\uE711";

        public EditNameForm(string oldName)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(ScaleHelper.Scale(420), ScaleHelper.Scale(280));
            this.BackColor = Color.FromArgb(40, 40, 40);
            this.Paint += (s, e) => e.Graphics.DrawRectangle(Pens.Gray, 0, 0, Width - 1, Height - 1);
            var lblTitle = new ThemedLabel
            {
                Text = LanguageManager.GetString("LootEditTitle") ?? "Edit Item Name",
                Font = AppTheme.BigTitleFont,
                Location = new Point(20, 25),
                AutoSize = true,
            };
            txtName = new ThemedTextBox
            {
                Text = oldName,
                Location = new Point(20, 120),
                Width = this.Width - 40,
                Font = AppTheme.MainFont,
            };
            int btnSize = ScaleHelper.Scale(45);
            int marginBottom = ScaleHelper.Scale(30);
            int btnY = this.Height - btnSize - marginBottom;
            int spacing = 20;
            int cancelX = this.Width - 25 - btnSize;
            int saveX = cancelX - spacing - btnSize;
            btnSave = new ThemedModalButton
            {
                Text = ICON_CHECK,
                Font = _iconFont,
                Location = new Point(saveX, btnY),
                Width = btnSize,
                Height = btnSize,
            };
            btnSave.SetThemePrimary();
            btnSave.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            btnCancel = new ThemedModalButton
            {
                Text = ICON_CANCEL,
                Font = _iconFont,
                Location = new Point(cancelX, btnY),
                Width = btnSize,
                Height = btnSize,
            };
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtName);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtName.Focus();
            txtName.SelectAll();
        }
    }

    // 省略其他未变更代码 (LoadData, SwitchMode 等)...
    private void LoadData(DateTime start, DateTime end)
    {
        this.SuspendLayout();
        try
        {
            var profile = _profileService.CurrentProfile;
            if (profile == null)
                return;
            var records = profile
                .LootRecords.Where(r => r.DropTime >= start && r.DropTime <= end)
                .OrderByDescending(r => r.DropTime)
                .ToList();
            var displayList = records
                .Select(r => new LootViewModel
                {
                    OriginalRecord = r,
                    SceneName = _sceneService.GetLocalizedShortSceneName(r.SceneName),
                })
                .ToList();
            var bs = new BindingSource();
            bs.DataSource = displayList;
            gridLoot.DataSource = bs;
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    private void SwitchMode(ViewMode mode)
    {
        _currentMode = mode;
        UpdateButtonStyles();
        pnlCustomDate.Visible = (mode == ViewMode.Custom);
        if (_showShortcuts)
            ToggleShortcutsVisibility();
        DateTime s = DateTime.Now,
            e = DateTime.Now;
        switch (mode)
        {
            case ViewMode.Today:
                s = DateTime.Today;
                break;
            case ViewMode.Week:
                s = _statisticsService.GetStartOfWeek();
                break;
            case ViewMode.Custom:
                s = DateTime.Today.AddDays(-1);
                break;
        }
        LoadData(s, e);
    }

    private void UpdateButtonStyles()
    {
        HighlightButton(btnToday, _currentMode == ViewMode.Today);
        HighlightButton(btnWeek, _currentMode == ViewMode.Week);
        HighlightButton(btnCustom, _currentMode == ViewMode.Custom);
    }

    private void HighlightButton(Button btn, bool isActive)
    {
        if (isActive)
        {
            btn.BackColor = Color.Gray;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Color.White;
        }
        else
        {
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.Gray;
            btn.FlatAppearance.BorderColor = Color.Gray;
        }
    }

    private void LanguageChanged(object? sender, EventArgs e) => UpdateLanguageText();

    private void UpdateLanguageText()
    {
        this.SafeInvoke(() =>
        {
            headerControl.Title = LanguageManager.GetString("LootHistoryTitle");
            btnToday.Text = LanguageManager.GetString("LootToday");
            btnWeek.Text = LanguageManager.GetString("LootThisWeek");
            btnCustom.Text = LanguageManager.GetString("LootCustom");
            SetupGridColumns();
        });
    }

    private void BtnSearch_Click(object sender, EventArgs e) => LoadData(dtpStart.Value, dtpEnd.Value);

    private void BtnClose_Click(object sender, EventArgs e) => this.Close();

    private void FadeInTimer_Tick(object? sender, EventArgs e)
    {
        if (this.Opacity < 1)
            this.Opacity += 0.08;
        else
        {
            this.Opacity = 1;
            _fadeInTimer.Stop();
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        _fadeInTimer.Start();
        LootHistoryForm_Resize(this, EventArgs.Empty);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _fadeInTimer.Stop();
        _mainService.SetActiveTabPage(Models.TabPage.Timer);
        _messenger.Publish(new ShowMainWindowMessage());
        LanguageManager.OnLanguageChanged -= LanguageChanged;
        base.OnFormClosed(e);
    }

    private void InitializeToggleButtons()
    {
        btnToday = CreateToggleButton("Today", ViewMode.Today);
        AttachKeyBadge(btnToday, "1", () => btnToday.PerformClick());
        btnWeek = CreateToggleButton("Week", ViewMode.Week);
        AttachKeyBadge(btnWeek, "2", () => btnWeek.PerformClick());
        btnCustom = CreateToggleButton("Custom", ViewMode.Custom);
        AttachKeyBadge(btnCustom, "3", () => btnCustom.PerformClick());
        headerControl.AddToggleButton(btnToday);
        headerControl.AddToggleButton(btnWeek);
        headerControl.AddToggleButton(btnCustom);
    }

    private Button CreateToggleButton(string text, ViewMode tag)
    {
        var btn = new Button
        {
            Text = text,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(
                ScaleHelper.Scale(25),
                ScaleHelper.Scale(5),
                ScaleHelper.Scale(25),
                ScaleHelper.Scale(5)
            ),
            MinimumSize = new Size(0, ScaleHelper.Scale(43)),
            Font = AppTheme.MainFont,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter,
            UseCompatibleTextRendering = true,
            Tag = tag,
        };
        btn.FlatAppearance.BorderSize = 1;
        btn.Click += (s, e) => SwitchMode(tag);
        return btn;
    }
}
