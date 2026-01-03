using System;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.Interfaces;

public interface IMainService
{
    // 初始化应用逻辑，传入窗口句柄用于注册热键
    void InitializeApplication(IntPtr windowHandle);

    // 处理关闭逻辑
    void HandleApplicationClosing();

    // 处理 Windows 消息（热键）
    void ProcessHotKeyMessage(Message msg);

    // 应用窗口设置（变为无状态的辅助方法，或者通过事件通知）
    void ApplyWindowSettings(Form form);

    // 触发 UI 刷新的请求
    void RequestRefresh();

    // --- 事件定义：用于通知 UI 层执行操作 ---

    void SetActiveTabPage(Models.TabPage tabPage);

    // 请求切换 Tab
    event Action<Models.TabPage>? OnRequestTabChange;

    // 请求 UI 刷新
    event Action? OnRequestRefreshUI;

    // 请求删除历史记录 (对应原来的 DeleteSelectedRecordAsync)
    event Action? OnRequestDeleteHistory;
    void RequestDeleteSelectedRecord();

    // 请求删除最后一个时间记录
    event Action? OnRequestDeleteLastHistory;
    void RequestDeleteLastHistory();

    // 请求删除最后一个掉落记录
    event Action? OnRequestDeleteLastLoot;
    void RequestDeleteLastLoot();

    void ReloadHotkeys();

    void UpdateWindowHandle(IntPtr newHandle);

    // 设置番茄钟模式
    void SetPomodoroMode(Models.PomodoroMode mode);

    void RequestShowSettings();
}
