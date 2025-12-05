using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public class LootHistoryForm : System.Windows.Forms.Form
{
    private readonly IProfileService _profileService;
    private readonly ISceneService _sceneService;
    private readonly IStatisticsService _statisticsService;

    // --- UI Controls ---
    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private FlowLayoutPanel pnlToggles = null!;
    private Button btnToday = null!;
    private Button btnWeek = null!;
    private Button btnCustom = null!;

    // 底部关闭按钮
    private Button btnClose = null!;

    // 自定义时间面板
    private Panel pnlCustomDate = null!;
    private ThemedDateTimePicker dtpStart = null!;
    private ThemedDateTimePicker dtpEnd = null!;
    private ThemedButton btnSearch = null!;
    private Label lblFrom = null!;
    private Label lblTo = null!;

    // 数据显示容器
    private Panel pnlGridContainer = null!;
    private ThemedDataGridView gridLoot = null!;

    // 状态
    private enum ViewMode { Today, Week, Custom }
    private ViewMode _currentMode = ViewMode.Today;

    // 常量定义
    private const int HeaderHeight = 100;
    private const int CustomDatePanelHeight = 50;
    private const int GridTopPadding = 10;

    public LootHistoryForm(
        IProfileService profileService,
        ISceneService sceneService,
        IStatisticsService statisticsService)
    {
        _profileService = profileService;
        _sceneService = sceneService;
        _statisticsService = statisticsService;

        InitializeComponent();

        // 订阅语言变化事件
        LanguageManager.OnLanguageChanged += LanguageChanged;

        // 默认加载今日数据
        SwitchMode(ViewMode.Today);
    }

    private void LanguageChanged(object? sender, EventArgs e)
    {
        // 更新标题文本
        lblTitle.Text = LanguageManager.GetString("LootHistoryTitle");

        // 更新切换按钮文本
        btnToday.Text = LanguageManager.GetString("LootToday");
        btnWeek.Text = LanguageManager.GetString("LootThisWeek");
        btnCustom.Text = LanguageManager.GetString("LootCustom");

        // 更新搜索按钮文本
        btnSearch.Text = LanguageManager.GetString("LootSearch");

        // 更新关闭按钮文本
        btnClose.Text = LanguageManager.GetString("LootClose");

        // 更新表格列标题
        SetupGridColumns();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // 1. 窗体基础设置
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.FromArgb(28, 28, 28);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.DoubleBuffered = true;
        this.TopMost = true; // 确保在最上层

        // 2. 顶部 Header 区域
        pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = HeaderHeight,
            BackColor = Color.Transparent,
        };

        lblTitle = new Label
        {
            AutoSize = false,
            Size = new Size(300, 40),
            Font = new Font("微软雅黑", 14F, FontStyle.Bold),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = LanguageManager.GetString("LootHistoryTitle"),
            Location = new Point(20, 30)
        };

        // 切换按钮容器
        pnlToggles = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
        };

        btnToday = CreateToggleButton(LanguageManager.GetString("LootToday"), ViewMode.Today);
        btnWeek = CreateToggleButton(LanguageManager.GetString("LootThisWeek"), ViewMode.Week);
        btnCustom = CreateToggleButton(LanguageManager.GetString("LootCustom"), ViewMode.Custom);

        pnlToggles.Controls.Add(btnToday);
        pnlToggles.Controls.Add(btnWeek);
        pnlToggles.Controls.Add(btnCustom);

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(pnlToggles);

        // 3. 自定义时间选择面板
        pnlCustomDate = new Panel
        {
            Dock = DockStyle.Top,
            Height = CustomDatePanelHeight,
            Visible = false, // 初始隐藏，但我们会预留它的位置
            BackColor = Color.Transparent
        };

        // 初始化时间控件
        lblFrom = new ThemedLabel { Text = "From:", AutoSize = true };
        dtpStart = new ThemedDateTimePicker { Width = 180 };
        lblTo = new ThemedLabel { Text = "To:", AutoSize = true };
        dtpEnd = new ThemedDateTimePicker { Width = 180 };

        btnSearch = new ThemedButton
        {
            Text = LanguageManager.GetString("LootSearch"),
            Size = new Size(80, 30),
            BorderRadius = 4
        };
        btnSearch.Click += (s, e) => LoadData(dtpStart.Value, dtpEnd.Value);

        pnlCustomDate.Controls.AddRange(new Control[] { lblFrom, dtpStart, lblTo, dtpEnd, btnSearch });

        // 4. 数据表格容器
        pnlGridContainer = new Panel
        {
            BackColor = Color.Transparent,
        };

        gridLoot = new ThemedDataGridView();
        SetupGridColumns();
        pnlGridContainer.Controls.Add(gridLoot);

        // 5. 底部关闭按钮
        btnClose = CreateFlatButton(LanguageManager.GetString("LootClose"), Color.IndianRed);
        btnClose.Click += (s, e) => this.Close();

        // 6. 添加控件 (注意 Dock 顺序: 也就是代码中添加的顺序是反的，最后添加的在最顶层)
        // 这里我们要确保 CustomDate 在 Header 下面
        this.Controls.Add(pnlGridContainer); // Fill
        this.Controls.Add(pnlCustomDate);    // Top (Bottom of Dock Stack)
        this.Controls.Add(pnlHeader);        // Top (Top of Dock Stack)
        this.Controls.Add(btnClose);         // Manual

        // 7. 绑定布局事件
        this.SizeChanged += LootHistoryForm_SizeChanged;

        this.ResumeLayout(false);
    }

    // --- 样式工厂方法 ---

    private Button CreateToggleButton(string text, ViewMode tag)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(120, 43),
            Font = new Font("微软雅黑", 10F),
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

    private Button CreateFlatButton(string text, Color hoverColor)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(160, 50),
            Font = new Font("微软雅黑", 11F),
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            BackColor = Color.FromArgb(60, 60, 60),
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = hoverColor;
        return btn;
    }

    // --- 布局逻辑 (核心修改) ---

    private void LootHistoryForm_SizeChanged(object? sender, EventArgs e)
    {
        int cx = this.ClientSize.Width / 2;
        int w = this.ClientSize.Width;
        int h = this.ClientSize.Height;

        // 1. Header 居中 Toggles
        pnlToggles.Location = new Point(cx - (pnlToggles.Width / 2), 30);

        // 2. Custom Date Panel 居中
        // 即使不可见，我们计算它的布局逻辑也不影响性能
        int spacing = 15;
        int datePanelContentWidth = lblFrom.Width + dtpStart.Width + lblTo.Width + dtpEnd.Width + btnSearch.Width + (spacing * 4);
        int startX = (w - datePanelContentWidth) / 2;
        int dateY = 10; // 面板内的相对 Y

        lblFrom.Location = new Point(startX, dateY + 4);
        dtpStart.Location = new Point(lblFrom.Right + spacing, dateY);
        lblTo.Location = new Point(dtpStart.Right + spacing, dateY + 4);
        dtpEnd.Location = new Point(lblTo.Right + spacing, dateY);
        btnSearch.Location = new Point(dtpEnd.Right + spacing, dateY - 2);

        // 3. 关闭按钮 (底部居中)
        int btnY = h - 100;
        btnClose.Location = new Point(cx - (btnClose.Width / 2), btnY);

        // --- 核心修改：固定 Grid 的起始位置 ---

        // 无论 pnlCustomDate 是否可见，我们总是把 Grid 放在 Header + CustomDatePanel 之下
        // 这样切换时，表格顶部位置永远不动，上方只是留白或显示控件
        int fixedGridTop = pnlHeader.Bottom + CustomDatePanelHeight + GridTopPadding;

        int gridBottom = btnY - 30;
        int gridHeight = gridBottom - fixedGridTop;

        // 限制表格最大宽度
        int maxGridWidth = 1000;
        int gridWidth = Math.Min(w - 100, maxGridWidth);

        pnlGridContainer.Size = new Size(gridWidth, Math.Max(100, gridHeight));
        pnlGridContainer.Location = new Point(cx - (gridWidth / 2), fixedGridTop);

        gridLoot.Dock = DockStyle.Fill;
    }

    // --- 业务逻辑 ---

    private void SwitchMode(ViewMode mode)
    {
        _currentMode = mode;
        UpdateButtonStyles();

        // 切换时间面板的可见性
        pnlCustomDate.Visible = (mode == ViewMode.Custom);

        // 注意：由于我们现在的布局逻辑是固定的，
        // 这里切换 Visible 不会导致 Grid 跳动，只会填补空白区域。

        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;

        switch (mode)
        {
            case ViewMode.Today:
                start = DateTime.Today;
                end = DateTime.Now;
                LoadData(start, end);
                break;
            case ViewMode.Week:
                start = _statisticsService.GetStartOfWeek();
                end = DateTime.Now;
                LoadData(start, end);
                break;
            case ViewMode.Custom:
                dtpStart.Value = DateTime.Today.AddDays(-1);
                dtpEnd.Value = DateTime.Now;
                LoadData(dtpStart.Value, dtpEnd.Value);
                break;
        }
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

    private void SetupGridColumns()
    {
        gridLoot.Columns.Clear();
        gridLoot.ColumnHeadersHeight = 40;
        gridLoot.RowTemplate.Height = 35;
        gridLoot.BackgroundColor = Color.FromArgb(32, 32, 32);

        var colTime = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableDropTime"),
            Width = 200,
            DataPropertyName = "DropTime",
            DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm", Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        colTime.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

        var colName = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableItemName"),
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            DataPropertyName = "Name"
        };
        colName.DefaultCellStyle.Font = new Font(AppTheme.MainFont, FontStyle.Bold);
        colName.DefaultCellStyle.ForeColor = AppTheme.AccentColor;

        var colScene = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableScene"),
            Width = 220,
            DataPropertyName = "SceneName",
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        colScene.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

        var colRun = new DataGridViewTextBoxColumn
        {
            HeaderText = LanguageManager.GetString("LootTableRun"),
            Width = 100,
            DataPropertyName = "RunCount",
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        };
        colRun.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

        gridLoot.Columns.AddRange([colTime, colName, colScene, colRun]);
    }

    private void LoadData(DateTime start, DateTime end)
    {
        var profile = _profileService.CurrentProfile;
        if (profile == null) return;

        var records = profile.LootRecords
            .Where(r => r.DropTime >= start && r.DropTime <= end)
            .OrderByDescending(r => r.DropTime)
            .ToList();

        var displayList = records.Select(r => new
        {
            r.DropTime,
            r.Name,
            SceneName = _sceneService.GetLocalizedShortSceneName(r.SceneName),
            r.RunCount
        }).ToList();

        var bindingSource = new BindingSource();
        bindingSource.DataSource = displayList;
        gridLoot.DataSource = bindingSource;
    }
}