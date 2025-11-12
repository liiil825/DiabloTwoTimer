using System;
using System.IO;
using DTwoMFTimerHelper.Models;

namespace DTwoMFTimerHelper.Utils
{
    /// <summary>
    /// 日志管理类，提供统一的日志记录功能
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// 写入调试日志
        /// </summary>
        /// <param name="className">调用类名</param>
        /// <param name="message">日志消息</param>
        public static void WriteDebugLog(string className, string message)
        {
            try
            {
                // 调试日志路径
                string debugLogPath = Path.Combine(Environment.CurrentDirectory, "debug_log.txt");
                // 调试日志：记录文件路径和当前时间
                Console.WriteLine($"[调试] 日志文件路径: {debugLogPath}, 当前时间: {DateTime.Now}");
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
            throw new NotImplementedException();
        }
        /// <summary>
        /// 写入调试日志
        /// </summary>
        /// <param name="className">调用类名</param>
        /// <param name="message">日志消息</param>
        public static void WriteErrorLog(string className, string message, Exception? ex = null)
        {
            try
            {
                // 错误日志路径
                string errorLogPath = Path.Combine(Environment.CurrentDirectory, "error_log.txt");
                // 错误日志：记录文件路径和当前时间
                Console.WriteLine($"[错误] 日志文件路径: {errorLogPath}, 当前时间: {DateTime.Now}");
                using StreamWriter writer = new StreamWriter(errorLogPath, true);
                writer.WriteLine($"[{DateTime.Now}] [{className}] {message} {ex?.Message ?? string.Empty}");
            }
            catch (Exception logEx)
            {
                // 避免日志系统本身的异常影响主流程
                Console.WriteLine($"写入日志失败: {logEx.Message}");
            }
        }

        internal static void WriteErrorLog(string v)
        {
            throw new NotImplementedException();
        }
    }
}