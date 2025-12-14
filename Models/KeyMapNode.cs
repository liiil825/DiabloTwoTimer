using System.Collections.Generic;

namespace DiabloTwoMFTimer.Models;

public class KeyMapNode
{
    /// <summary>
    /// 触发按键 (例如 "t", "s", "1")
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 显示的名称 (例如 "计时器", "开始")
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 要执行的命令ID (例如 "Timer.Start")
    /// 如果此节点是父菜单，则此项为空
    /// </summary>
    public string? Action { get; set; }

    // --- 新增字段 Start ---
    /// <summary>
    /// 是否需要用户输入参数 (例如 "SetOpacity" 需要输入数值)
    /// </summary>
    public bool RequiresInput { get; set; } = false;

    /// <summary>
    /// 输入提示文本 (例如 "请输入透明度 (0-100)...")
    /// </summary>
    public string? InputHint { get; set; }
    // --- 新增字段 End ---

    /// <summary>
    /// 子菜单节点
    /// </summary>
    public List<KeyMapNode>? Children { get; set; }

    /// <summary>
    /// 是否是叶子节点 (可执行操作)
    /// </summary>
    public bool IsLeaf => !string.IsNullOrEmpty(Action) && (Children == null || Children.Count == 0);
}
