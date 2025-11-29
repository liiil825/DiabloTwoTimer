using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI;

namespace DiabloTwoMFTimer;

static class Program
{
    private static IServiceProvider? _serviceProvider;

    [STAThread]
    static void Main(string[] args)
    {
        // 1. 全局异常处理
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // 2. 基础 UI 设置
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 3. 调试模式检查
        if (args.Length > 0 && args[0].Equals("--debug", StringComparison.CurrentCultureIgnoreCase))
        {
            Utils.LogManager.IsDebugEnabled = true;
        }

        try
        {
            // 4. 配置依赖注入
            _serviceProvider = ServiceConfiguration.ConfigureServices();

            // 5. 启动应用程序
            // 这里的 GetRequiredService 会自动触发 MainForm 的构造函数
            // 从而自动创建并注入 ProfileManager, TimerControl, MainServices 等所有依赖
            var mainForm = _serviceProvider.GetRequiredService<MainForm>();

            // 注意：MainServices 的初始化逻辑 (InitializeApplication) 
            // 现在在 MainForm 的 Shown 事件中触发，因为需要等待窗口句柄创建完成。

            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            HandleFatalException(ex);
        }
    }

    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        LogAndShowError("thread_error_log.txt", e.Exception.ToString(), "发生线程异常");
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        string exceptionContent = e.ExceptionObject?.ToString() ?? "未知异常（ExceptionObject 为 null）";
        LogAndShowError("domain_error_log.txt", exceptionContent, "发生未处理的异常");
    }

    private static void HandleFatalException(Exception ex)
    {
        LogAndShowError("startup_error_log.txt", ex.ToString(), "程序启动失败");
    }

    private static void LogAndShowError(string fileName, string content, string title)
    {
        try
        {
            string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            File.WriteAllText(errorLogPath, content);
            MessageBox.Show(
                $"{title}。错误详情已保存到 {errorLogPath}\n\n{content}",
                "应用程序错误",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        catch
        {
            // 如果连日志都写不进去，就只弹窗
            MessageBox.Show(
                $"{title}。\n\n{content}",
                "应用程序严重错误",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}