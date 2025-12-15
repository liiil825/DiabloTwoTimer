using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Utils; // 引用 LogManager

namespace DiabloTwoMFTimer.Services;

public class CommandDispatcher : ICommandDispatcher
{
    // 统一存储结构：所有命令都视为接受 object? 返回 Task
    private readonly Dictionary<string, Func<object?, Task>> _commands = new(StringComparer.OrdinalIgnoreCase);

    public void Register(string commandId, Func<Task> action)
    {
        // 包装：忽略参数，直接执行无参逻辑
        Register(commandId, _ => action());
    }

    public void Register(string commandId, Action action)
    {
        // 包装：忽略参数，包装同步 Action
        Register(
            commandId,
            _ =>
            {
                action();
                return Task.CompletedTask;
            }
        );
    }

    public void Register(string commandId, Func<object?, Task> action)
    {
        if (string.IsNullOrWhiteSpace(commandId) || action == null)
            return;
        _commands[commandId] = action;
    }

    public void Register(string commandId, Action<object?> action)
    {
        if (action == null)
            return;
        Register(
            commandId,
            (args) =>
            {
                action(args);
                return Task.CompletedTask;
            }
        );
    }

    public async Task ExecuteAsync(string commandId, object? args = null)
    {
        if (string.IsNullOrWhiteSpace(commandId))
            return;

        if (_commands.TryGetValue(commandId, out var action))
        {
            try
            {
                await action(args);
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("CommandDispatcher", $"执行命令失败: {commandId}", ex);
                Utils.Toast.Error($"执行命令失败: {commandId}");
            }
        }
        else
        {
            LogManager.WriteDebugLog("CommandDispatcher", $"未找到命令: {commandId}");
        }
    }

    public bool HasCommand(string commandId)
    {
        return !string.IsNullOrWhiteSpace(commandId) && _commands.ContainsKey(commandId);
    }
}
