using System;
using System.Threading.Tasks;

namespace DiabloTwoMFTimer.Interfaces;

public interface ICommandDispatcher
{
    // --- 保持原有接口兼容性 ---
    void Register(string commandId, Func<Task> action);
    void Register(string commandId, Action action);

    // --- 新增带参注册接口 ---
    /// <summary>
    /// 注册一个接受参数的异步命令
    /// </summary>
    void Register(string commandId, Func<object?, Task> action);

    /// <summary>
    /// 注册一个接受参数的同步命令
    /// </summary>
    void Register(string commandId, Action<object?> action);

    // --- 修改执行接口 ---
    /// <summary>
    /// 执行命令 (可选参数)
    /// </summary>
    Task ExecuteAsync(string commandId, object? args = null);

    bool HasCommand(string commandId);
}
