using System;
using System.IO;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Utils
{
    /// <summary>
    /// 日志管理类，提供统一的日志记录功能
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// 控制是否启用调试日志记录
        /// </summary>
        public static bool IsDebugEnabled { get; set; } = false;

        /// <summary>
        /// 写入调试日志（仅在调试模式启用时记录）
        /// </summary>
        /// <param name="className">调用类名</param>
        /// <param name="message">日志消息</param>
        public static void WriteDebugLog(string className, string message)
        {
            // 只有在调试模式启用时才记录调试日志
            if (!IsDebugEnabled)
                return;

            try
            {
                // 调试日志路径
                string debugLogPath = Path.Combine(Environment.CurrentDirectory, "debug_log.txt");
                using StreamWriter writer = new StreamWriter(debugLogPath, true);
                writer.WriteLine($"[{DateTime.Now}] [{className}] {message}");
            }
            catch (Exception logEx)
            {
                // 避免日志系统本身的异常影响主流程
                Console.WriteLine($"写入日志失败: {logEx.Message}");
            }
        }

        internal static void WriteDebugLog(string v)
        {
            // 调用主调试日志方法，使用默认类名
            WriteDebugLog("Internal", v);
        }

        /// <summary>
        /// 写入错误日志
        /// </summary>
        /// <param name="className">调用类名</param>
        /// <param name="message">日志消息</param>
        /// <param name="ex">异常对象</param>
        public static void WriteErrorLog(string className, string message, Exception? ex = null)
        {
            try
            {
                // 错误日志路径
                string errorLogPath = Path.Combine(Environment.CurrentDirectory, "error_log.txt");
                // 错误日志：记录文件路径和当前时间
                using StreamWriter writer = new StreamWriter(errorLogPath, true);
                writer.WriteLine($"[{DateTime.Now}] [{className}] {message}");
                if (ex != null)
                {
                    writer.WriteLine($"[{DateTime.Now}] [{className}] 异常消息: {ex.Message}");
                    writer.WriteLine($"[{DateTime.Now}] [{className}] 异常堆栈: {ex.StackTrace}");
                }
            }
            catch (Exception logEx)
            {
                // 避免日志系统本身的异常影响主流程
                Console.WriteLine($"写入日志失败: {logEx.Message}");
            }
        }

        internal static void WriteErrorLog(string v)
        {
            // 调用主错误日志方法，使用默认类名
            WriteErrorLog("Internal", v);
        }
    }
}
