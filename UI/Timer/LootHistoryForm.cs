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

public partial class LootHistoryForm : System.Windows.Forms.Form
{
    private readonly IProfileService _profileService;
    private readonly ISceneService _sceneService;
    private readonly IStatisticsService _statisticsService;

    private Button btnToday = null!;
    private Button btnWeek = null!;
    private Button btnCustom = null!;

    // 动画定时器
    private System.Windows.Forms.Timer _fadeInTimer;

    private enum ViewMode
    {
        Today,
        Week,
        Custom,
    }

    private ViewMode _currentMode = ViewMode.Today;

    public LootHistoryForm(
            IProfileService profileService,
            ISceneService sceneService,
            IStatisticsService statisticsService
        )
    {
        _profileService = profileService;
        _sceneService = sceneService;
        _statisticsService = statisticsService;

        InitializeComponent();
        InitializeToggleButtons();
        D2ScrollHelper.Attach(this.gridLoot, this);

        LanguageManager.OnLanguageChanged += LanguageChanged;

        // 绑定布局事件
        this.SizeChanged += LootHistoryForm_SizeChanged;

        // 设置Esc键关闭窗口
        this.CancelButton = btnClose;

        // --- 1. 动画初始化 ---
        this.Opacity = 0;
        _fadeInTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _fadeInTimer.Tick += FadeInTimer_Tick;

        UpdateLanguageText();
        SwitchMode(ViewMode.Today);
    }

    // --- 2. 动画逻辑 ---
    private void FadeInTimer_Tick(object? sender, EventArgs e)
    {
        if (this.Opacity < 1)
        {
            this.Opacity += 0.08;
        }
        else
        {
            this.Opacity = 1;
            _fadeInTimer.Stop();
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        // 确保布局在显示前完成一次计算
        LootHistoryForm_SizeChanged(this, EventArgs.Empty);
        _fadeInTimer.Start();
    }

    private void InitializeToggleButtons()
    {
        btnToday = CreateToggleButton("Today", ViewMode.Today);
        btnWeek = CreateToggleButton("Week", ViewMode.Week);
        btnCustom = CreateToggleButton("Custom", ViewMode.Custom);

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
            // 增加内边距让按钮看起来更宽敞
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

    // --- 核心布局逻辑 ---
    private void LootHistoryForm_SizeChanged(object? sender, EventArgs e)
    {
        // --- 3. 挂起布局 ---
        this.SuspendLayout();
        try
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;
            int cx = w / 2;

            // 1. 设置 Header 高度 (固定)
            int headerHeight = ScaleHelper.Scale(110);
            headerControl.Height = headerHeight;

            // 【解决问题1：顶部按钮居中】
            // 访问 ThemedWindowHeader 暴露的 FlowLayoutPanel
            var togglePanel = headerControl.TogglePanel;
            if (togglePanel != null)
            {
                // 确保 Panel 大小已计算
                togglePanel.PerformLayout();
                // 计算居中 X 坐标
                togglePanel.Left = (w - togglePanel.Width) / 2;
            }

            // 2. 预留给自定义日期栏的高度 (固定保留，解决表格跳动问题)
            int datePanelHeight = ScaleHelper.Scale(60);
            int datePanelTop = headerHeight; // 紧接 Header 下方

            // 配置 pnlCustomDate (始终占据这个位置和大小)
            pnlCustomDate.Location = new Point(0, datePanelTop);
            pnlCustomDate.Size = new Size(w, datePanelHeight);

            // 【解决问题2 & 3：对齐日期控件和按钮】
            LayoutCustomDatePanel(w, datePanelHeight);

            // 3. 配置表格容器
            // 【解决问题4：表格位移】
            // 表格的 Top 永远是 (Header高度 + DatePanel高度 + 间距)，无论 DatePanel 是否可见
            int gridTopPadding = ScaleHelper.Scale(10);
            int gridTop = datePanelTop + datePanelHeight + gridTopPadding;

            // 底部留白给关闭按钮
            int bottomMargin = ScaleHelper.Scale(100);
            int gridHeight = h - gridTop - bottomMargin;
            if (gridHeight < 100)
                gridHeight = 100;

            // 限制表格最大宽度，避免在 4K 屏上太宽难看
            int maxGridWidth = ScaleHelper.Scale(1200);
            int gridWidth = Math.Min(w - ScaleHelper.Scale(60), maxGridWidth); // 左右留30间距

            pnlGridContainer.Size = new Size(gridWidth, gridHeight);
            pnlGridContainer.Location = new Point((w - gridWidth) / 2, gridTop);

            // 4. 配置关闭按钮 (底部居中)
            int btnY = h - ScaleHelper.Scale(80);
            btnClose.Location = new Point(cx - (btnClose.Width / 2), btnY);
        }
        finally
        {
            // --- 4. 恢复布局 ---
            this.ResumeLayout();
        }
    }

    private void LayoutCustomDatePanel(int panelWidth, int panelHeight)
    {
        // 1. 【核心】统一参数设置
        // 增加一点高度，让控件看起来更饱满
        int ctrlHeight = ScaleHelper.Scale(36);
        int datePickerWidth = ScaleHelper.Scale(160);
        int btnWidth = ScaleHelper.Scale(80);
        int spacing = ScaleHelper.Scale(10); // 间距稍微收紧一点

        // 2. 【核心】统一字体 (这是解决字体大小不一致的关键)
        // 强制所有控件使用相同的主题字体，确保视觉大小一致
        Font unifiedFont = AppTheme.MainFont;

        lblFrom.Font = unifiedFont;
        lblTo.Font = unifiedFont;
        dtpStart.Font = unifiedFont;
        dtpEnd.Font = unifiedFont;
        btnSearch.Font = unifiedFont;

        // 3. 【核心】统一 Label 样式以实现完美垂直居中
        // 关闭 AutoSize，手动设置高度与输入框一致，并开启垂直居中对齐
        void ConfigureLabel(Label lbl)
        {
            lbl.AutoSize = false;
            lbl.Height = ctrlHeight;
            lbl.TextAlign = ContentAlignment.MiddleRight; // 文字靠右，紧贴输入框
            // 动态计算文字所需宽度 + 少量留白
            Size size = TextRenderer.MeasureText(lbl.Text, unifiedFont);
            lbl.Width = size.Width + ScaleHelper.Scale(5);
        }

        ConfigureLabel(lblFrom);
        ConfigureLabel(lblTo);

        // 4. 设置输入控件尺寸
        dtpStart.Size = new Size(datePickerWidth, ctrlHeight);
        dtpEnd.Size = new Size(datePickerWidth, ctrlHeight);
        btnSearch.Size = new Size(btnWidth, ctrlHeight);

        // 5. 计算居中位置
        int totalContentWidth =
            lblFrom.Width
            + spacing
            + dtpStart.Width
            + spacing
            + lblTo.Width
            + spacing
            + dtpEnd.Width
            + spacing
            + btnSearch.Width;

        int startX = (panelWidth - totalContentWidth) / 2;

        // 【核心】因为所有控件高度一致(ctrlHeight)，这里只需要计算一次 Y 坐标
        int commonY = (panelHeight - ctrlHeight) / 2;

        // 6. 逐个定位 (所有控件的 Y 坐标都使用 commonY，绝对对齐)
        lblFrom.Location = new Point(startX, commonY);

        dtpStart.Location = new Point(lblFrom.Right + spacing, commonY);

        lblTo.Location = new Point(dtpStart.Right + spacing, commonY);

        dtpEnd.Location = new Point(lblTo.Right + spacing, commonY);

        btnSearch.Location = new Point(dtpEnd.Right + spacing, commonY);
    }

    private void SwitchMode(ViewMode mode)
    {
        _currentMode = mode;
        UpdateButtonStyles();

        // 切换日期面板可见性
        pnlCustomDate.Visible = (mode == ViewMode.Custom);

        // 注意：LootHistoryForm_SizeChanged 中 gridTop 是根据固定高度计算的
        // 所以即使 pnlCustomDate 隐藏了，那里也是留白的，表格不会跳动。

        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;

        switch (mode)
        {
            case ViewMode.Today:
                start = DateTime.Today;
                LoadData(start, end);
                break;
            case ViewMode.Week:
                start = _statisticsService.GetStartOfWeek();
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

    private void LanguageChanged(object? sender, EventArgs e)
    {
        UpdateLanguageText();
        // 语言改变可能导致 Label 宽度变化，重新布局以保持居中
        LootHistoryForm_SizeChanged(this, EventArgs.Empty);
    }

    private void UpdateLanguageText()
    {
        this.SafeInvoke(() =>
        {
            headerControl.Title = LanguageManager.GetString("LootHistoryTitle");
            btnToday.Text = LanguageManager.GetString("LootToday");
            btnWeek.Text = LanguageManager.GetString("LootThisWeek");
            btnCustom.Text = LanguageManager.GetString("LootCustom");
            btnSearch.Text = LanguageManager.GetString("LootSearch");
            btnClose.Text = LanguageManager.GetString("LootClose");
            SetupGridColumns();
        });
    }

    private void BtnSearch_Click(object sender, EventArgs e)
    {
        LoadData(dtpStart.Value, dtpEnd.Value);
    }

    private void BtnClose_Click(object sender, EventArgs e)
    {
        // 简单的淡出 (可选，如果觉得太复杂直接 Close 也可以)
        this.Close();
    }

    private void SetupGridColumns()
    {
        gridLoot.Columns.Clear();
        gridLoot.ColumnHeadersHeight = ScaleHelper.Scale(40);
        gridLoot.RowTemplate.Height = ScaleHelper.Scale(35);
        gridLoot.BackgroundColor = Color.FromArgb(32, 32, 32);

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

        gridLoot.Columns.AddRange([colTime, colName, colScene, colRun]);
    }

    private void LoadData(DateTime start, DateTime end)
    {
        // 4. 数据加载也挂起布局
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
                .Select(r => new
                {
                    r.DropTime,
                    r.Name,
                    SceneName = _sceneService.GetLocalizedShortSceneName(r.SceneName),
                    r.RunCount,
                })
                .ToList();

            var bindingSource = new BindingSource();
            bindingSource.DataSource = displayList;
            gridLoot.DataSource = bindingSource;
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _fadeInTimer.Stop();
        LanguageManager.OnLanguageChanged -= LanguageChanged;
        base.OnFormClosed(e);
    }
}
