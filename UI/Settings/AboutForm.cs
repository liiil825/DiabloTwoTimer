using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Form;

public partial class AboutForm : BaseForm
{
    private const string GithubUrl = "https://github.com/liiil825/diablotwotimer";
    private const string BilibiliUrl = "https://space.bilibili.com/3250094";

    public AboutForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!DesignMode)
        {
            UpdateUI();
        }
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        this.Text = LanguageManager.GetString("AboutTitle") ?? "关于";

        // 获取版本号
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";
        lblVersion.Text = LanguageManager.GetString("VersionLabel", version);
        lblAuthor.Text = LanguageManager.GetString("AuthorLabel") ?? "Author: liiil825";
        btnGithub.Text = LanguageManager.GetString("VisitGithub") ?? "GitHub";
        btnBilibili.Text = LanguageManager.GetString("VisitBilibili") ?? "Bilibili";

        // 我们不需要底部的 Confirm/Cancel 按钮，在这里可以再次确保隐藏
        if (btnConfirm != null)
            btnConfirm.Visible = false;
        if (btnCancel != null)
            btnCancel.Visible = false;
    }

    private void BtnGithub_Click(object sender, EventArgs e)
    {
        OpenUrl(GithubUrl);
    }

    private void BtnBilibili_Click(object sender, EventArgs e)
    {
        OpenUrl(BilibiliUrl);
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true, // .NET Core/5+ 需要设置为 true 才能打开 URL
                }
            );
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("AboutForm", $"无法打开链接: {url}", ex);
            Toast.Error($"无法打开链接: {ex.Message}");
        }
    }
}
