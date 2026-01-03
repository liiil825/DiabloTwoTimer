using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class AboutSettingsControl : UserControl
{
    // 定义跳转链接
    private const string GithubUrl = "https://github.com/liiil825/diablotwotimer";
    private const string BilibiliUrl = "https://space.bilibili.com/3250094";

    public AboutSettingsControl()
    {
        InitializeComponent();
        UpdateUI();
    }

    public void UpdateUI()
    {
        // 设置软件名称
        lblAppName.Text = "Diablo II Timer";

        // 获取版本号
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";
        lblVersion.Text = LanguageManager.GetString("VersionLabel", version);

        lblAuthor.Text = LanguageManager.GetString("AuthorLabel") ?? "Author: liiil825";

        btnGithub.Text = LanguageManager.GetString("VisitGithub") ?? "GitHub";
        btnBilibili.Text = LanguageManager.GetString("VisitBilibili") ?? "Bilibili";
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
                    UseShellExecute = true
                }
            );
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("AboutSettingsControl", $"无法打开链接: {url}", ex);
            Toast.Error($"无法打开链接: {ex.Message}");
        }
    }
}