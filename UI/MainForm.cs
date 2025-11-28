using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Services;

namespace DiabloTwoMFTimer.UI;
public partial class MainForm : Form
{
    // 服务实例
    private readonly IMainServices _mainServices;

    public MainForm(IMainServices mainServices)
    {
        // 获取服务实例并设置主窗体引用
        _mainServices = mainServices;

        InitializeComponent();
        // 设置窗体属性
        InitializeForm();
        // 添加Shown事件处理，确保窗口显示后正确应用位置设置
        this.Shown += (sender, e) => OnMainForm_Shown(sender ?? this, e);
    }

    // UI控件 - 改为internal以便服务类访问
    internal TabControl? TabControl => tabControl;
    internal TabPage? TabProfilePage => tabProfilePage;
    internal TabPage? TabTimerPage => tabTimerPage;
    internal TabPage? TabPomodoroPage => tabPomodoroPage;
    internal TabPage? TabSettingsPage => tabSettingsPage;

    private void OnMainForm_Shown(object sender, EventArgs e)
    {
        // 窗口显示后再次应用窗口位置设置，确保正确显示在右上角
        _mainServices.ApplyWindowSettings();
    }

    private void InitializeForm()
    {
        var width = UISizeConstants.MainFormWidth;
        var settings = Services.SettingsManager.LoadSettings();
        var height = settings.TimerShowLootDrops
            ? UISizeConstants.MainFormHeightWithLoot
            : UISizeConstants.MainFormHeightWithoutLoot;
        this.Size = new Size(width, height);
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = true;
        this.Visible = true;
    }

    /// <summary>
    /// 更新窗体标题（供服务类调用）
    /// </summary>
    public void UpdateFormTitle(string title)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action<string>(UpdateFormTitle), title);
        }
        else
        {
            this.Text = title;
        }
    }

    /// <summary>
    /// 刷新UI（供外部调用）
    /// </summary>
    public void RefreshUI()
    {
        _mainServices.RefreshUI();
    }

    private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _mainServices.HandleTabChanged();
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        _mainServices.ProcessHotKeyMessage(m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _mainServices.HandleApplicationClosing();
        base.OnFormClosing(e);
    }
}
