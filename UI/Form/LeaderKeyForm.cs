using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks; // 引用 Task
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Repositories;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Form;

public partial class LeaderKeyForm : System.Windows.Forms.Form
{
    private readonly ICommandDispatcher? _commandDispatcher;

    // UI 控件
    private FlowLayoutPanel _breadcrumbPanel = null!; // 面包屑导航栏
    private FlowLayoutPanel _itemsPanel = null!;      // 按键选项区域

    // 状态数据
    private readonly Stack<KeyMapNode> _pathStack = new(); // 记录当前路径
    private List<KeyMapNode> _currentNodes = null!;       // 当前显示的节点列表

    // 【新增】根节点缓存，用于重置状态时恢复
    private List<KeyMapNode> _rootNodes = new();

    private readonly IKeyMapRepository _keyMapRepository;

    // 构造函数
    public LeaderKeyForm(ICommandDispatcher? commandDispatcher = null, IKeyMapRepository? keyMapRepository = null)
    {
        _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        _keyMapRepository = keyMapRepository ?? throw new ArgumentNullException(nameof(keyMapRepository));

        // 1. 窗体基础设置
        this.FormBorderStyle = FormBorderStyle.None;
        this.ShowInTaskbar = false;
        this.TopMost = true;
        this.StartPosition = FormStartPosition.Manual;
        this.BackColor = AppTheme.Colors.Background; // 深灰背景
        this.Opacity = 0.95; // 稍微一点透明度，增加现代感
        this.DoubleBuffered = true;

        // 2. 初始化 UI
        InitializeControls();

        // 3. 加载初始数据
        RefreshData();
        // RefreshData 内部会调用 ResetState -> RefreshUI，所以这里不需要再调 RefreshUI
    }

    private void InitializeControls()
    {
        // 计算尺寸：宽度全屏，高度 20%
        var screen = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
        this.Width = screen.Width;
        this.Height = (int)(screen.Height * 0.2);

        // 定位到底部
        this.Location = new Point(screen.X, screen.Bottom - this.Height);

        // --- 顶部：面包屑导航 ---
        _breadcrumbPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 40,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(20, 10, 0, 0),
            BackColor = AppTheme.Colors.ControlBackground
        };

        // --- 底部：按键列表 ---
        _itemsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(20, 20, 20, 20),
            AutoScroll = true
        };

        this.Controls.Add(_itemsPanel);
        this.Controls.Add(_breadcrumbPanel);
    }

    /// <summary>
    /// 核心方法：刷新界面显示
    /// </summary>
    private void RefreshUI()
    {
        _breadcrumbPanel.SuspendLayout();
        _itemsPanel.SuspendLayout();

        // 1. 更新面包屑
        _breadcrumbPanel.Controls.Clear();
        // 添加根节点标志
        AddBreadcrumbItem("Leader");
        foreach (var node in _pathStack.Reverse())
        {
            AddBreadcrumbItem(" > ");
            AddBreadcrumbItem(node.Text);
        }

        // 2. 更新按键列表
        _itemsPanel.Controls.Clear();

        // 排序：有 Key 的排前面，按字母顺序
        if (_currentNodes != null)
        {
            foreach (var node in _currentNodes.OrderBy(n => n.Key))
            {
                var itemControl = CreateItemControl(node);
                _itemsPanel.Controls.Add(itemControl);
            }

            // 如果没有子节点了
            if (_currentNodes.Count == 0)
            {
                var lbl = new ThemedLabel { Text = "没有可用操作", AutoSize = true, ForeColor = Color.Gray };
                _itemsPanel.Controls.Add(lbl);
            }
        }

        _breadcrumbPanel.ResumeLayout();
        _itemsPanel.ResumeLayout();
    }

    private void AddBreadcrumbItem(string text)
    {
        var lbl = new Label
        {
            Text = text,
            AutoSize = true,
            Font = new Font(AppTheme.Fonts.FontFamily, 12, FontStyle.Bold),
            ForeColor = AppTheme.Colors.Primary,
            Margin = new Padding(0)
        };
        _breadcrumbPanel.Controls.Add(lbl);
    }

    private Control CreateItemControl(KeyMapNode node)
    {
        int keyBoxSize = ScaleHelper.Scale(50);
        int itemWidth = ScaleHelper.Scale(180);
        int itemHeight = ScaleHelper.Scale(50);

        var panel = new Panel
        {
            Size = new Size(itemWidth, itemHeight),
            Margin = new Padding(0, 0, 15, 15),
            BackColor = Color.Transparent
        };

        var lblKey = new Label
        {
            Text = $"[{node.Key.ToUpper()}]",
            Font = new Font("Consolas", 12, FontStyle.Bold),
            ForeColor = AppTheme.Colors.Primary,
            Location = new Point(0, 0),
            Size = new Size(keyBoxSize, keyBoxSize),
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            BorderStyle = BorderStyle.FixedSingle
        };

        var lblText = new Label
        {
            Text = node.Text,
            Font = AppTheme.Fonts.Regular,
            ForeColor = AppTheme.Colors.Text,
            Location = new Point(keyBoxSize + ScaleHelper.Scale(5), 0),
            Size = new Size(itemWidth - keyBoxSize - ScaleHelper.Scale(5), itemHeight),
            TextAlign = ContentAlignment.MiddleLeft
        };

        panel.Controls.Add(lblKey);
        panel.Controls.Add(lblText);
        return panel;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // 1. 处理 ESC
        if (keyData == Keys.Escape)
        {
            if (_pathStack.Count > 0)
            {
                _pathStack.Pop();
                // 重新获取上一层的节点列表
                if (_pathStack.Count > 0)
                    _currentNodes = _pathStack.Peek().Children ?? [];
                else
                    _currentNodes = _rootNodes; // 返回根节点

                RefreshUI();
            }
            else
            {
                this.Hide();
            }
            return true;
        }

        // 2. 将按键转换为字符
        string keyChar = GetCharFromKeys(keyData);
        if (!string.IsNullOrEmpty(keyChar))
        {
            HandleInput(keyChar);
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void HandleInput(string key)
    {
        if (_currentNodes == null) return;

        // 查找匹配的节点
        var targetNode = _currentNodes.FirstOrDefault(n => n.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (targetNode == null) return;

        if (targetNode.IsLeaf)
        {
            ExecuteAction(targetNode);
        }
        else
        {
            if (targetNode.Children != null && targetNode.Children.Count > 0)
            {
                _pathStack.Push(targetNode);
                _currentNodes = targetNode.Children;
                RefreshUI();
            }
        }
    }

    private void ExecuteAction(KeyMapNode node)
    {
        this.Hide();

        if (_commandDispatcher != null && !string.IsNullOrEmpty(node.Action))
        {
            _ = _commandDispatcher.ExecuteAsync(node.Action);
        }
        else
        {
            ThemedMessageBox.Show($"[演示模式] 执行命令:\n{node.Action}\n\n描述: {node.Text}", "Leader Key Action");
        }

        LogManager.WriteDebugLog("LeaderKeyForm", $"执行操作: {node.Action}");
        ResetState();
    }

    private void ResetState()
    {
        LogManager.WriteDebugLog("LeaderKeyForm", $"重置状态: 清空路径栈");
        _pathStack.Clear();

        // 【核心修复】将当前节点重置为根节点缓存
        _currentNodes = _rootNodes ?? [];

        RefreshUI();
    }

    protected override void OnDeactivate(EventArgs e)
    {
        base.OnDeactivate(e);
        this.Hide();
        ResetState();
    }

    private string GetCharFromKeys(Keys key)
    {
        var k = key & ~Keys.Modifiers;
        if (k >= Keys.A && k <= Keys.Z) return k.ToString().ToLower();
        if (k >= Keys.D0 && k <= Keys.D9) return k.ToString().Replace("D", "");
        if (k >= Keys.NumPad0 && k <= Keys.NumPad9) return k.ToString().Replace("NumPad", "");
        return string.Empty;
    }

    public void RefreshData()
    {
        try
        {
            // 【修改】加载数据到 _rootNodes 缓存
            _rootNodes = _keyMapRepository.LoadKeyMap();
        }
        catch (Exception ex)
        {
            _rootNodes =
            [
                new KeyMapNode { Text = $"配置加载失败: {ex.Message}", Key = "!" }
            ];
        }
        // 调用 ResetState 将 _currentNodes 指向新的 _rootNodes
        ResetState();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);

        if (this.Visible)
        {
            ResetState();
        }
    }
}