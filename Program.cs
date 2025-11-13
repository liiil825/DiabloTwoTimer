using System;
using System.Windows.Forms;
using System.IO;

namespace DTwoMFTimerHelper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 添加全局异常处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // 检查命令行参数
            if (args.Length > 0 && args[0].Equals("--debug", StringComparison.CurrentCultureIgnoreCase))
            {
                Utils.LogManager.IsDebugEnabled = true;
            }
            try
            {
                Application.Run(new UI.MainForm());
            }
            catch (Exception ex)
            {
                string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
                File.WriteAllText(errorLogPath, ex.ToString());
                MessageBox.Show($"发生未处理的异常。错误详情已保存到 {errorLogPath}", "应用程序错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thread_error_log.txt");
            File.WriteAllText(errorLogPath, e.Exception.ToString());
            MessageBox.Show($"发生线程异常。错误详情已保存到 {errorLogPath}", "应用程序错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string errorLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "domain_error_log.txt");
            File.WriteAllText(errorLogPath, e.ExceptionObject.ToString());
            // 主线程异常可能导致应用程序崩溃，所以这里只记录日志
        }
    }
}