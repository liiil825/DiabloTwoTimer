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
    private readonly IKeyMapRepository _keyMapRepository;

    // UI 控件
    private FlowLayoutPanel _breadcrumbPanel = null!;
    private FlowLayoutPanel _itemsPanel = null!;

    // 【新增】输入框
    private ThemedTextBox _inputTextBox = null!;
    private Label _inputHintLabel = null!;
    private Panel _inputContainer = null!;

    // 状态数据
    private readonly Stack<KeyMapNode> _pathStack = new();
    private List<KeyMapNode> _currentNodes = null!;
    private List<KeyMapNode> _rootNodes = new();

    // 【新增】当前等待输入的节点
    private KeyMapNode? _pendingInputNode = null;

    private System.Windows.Forms.Timer _animTimer = null!;
    private bool _isClosing = false;
    private const double TARGET_OPACITY = 0.95; // 目标透明度
    private const double ANIMATION_SPEED = 0.15; // 动画速度 (越大越快)

    public LeaderKeyForm(ICommandDispatcher? commandDispatcher = null, IKeyMapRepository? keyMapRepository = null)
    {
        _commandDispatcher = commandDispatcher ?? throw new ArgumentNullException(nameof(commandDispatcher));
        _keyMapRepository = keyMapRepository ?? throw new ArgumentNullException(nameof(keyMapRepository));

        this.FormBorderStyle = FormBorderStyle.None;
        this.ShowInTaskbar = false;
        this.TopMost = true;
        this.StartPosition = FormStartPosition.Manual;
        this.BackColor = AppTheme.Colors.Background;
        this.Opacity = 0.95;
        this.DoubleBuffered = true;

        _animTimer = new System.Windows.Forms.Timer { Interval = 10 }; // 10ms 刷新一次，保证流畅
        _animTimer.Tick += AnimationTimer_Tick;

        InitializeControls();
        RefreshData();
    }

    private void InitializeControls()
    {
        var screen = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
        this.Width = screen.Width;
        this.Height = (int)(screen.Height * 0.2);
        this.Location = new Point(screen.X, screen.Bottom - this.Height);

        // --- 1. 面包屑 ---
        _breadcrumbPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 40,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(20, 10, 0, 0),
            BackColor = AppTheme.Colors.ControlBackground,
        };

        // --- 2. 列表面板 ---
        _itemsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(20, 20, 20, 20),
            AutoScroll = true,
        };

        // --- 3. 输入容器 (默认隐藏) ---
        _inputContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Visible = false,
            Padding = new Padding(0, 30, 0, 0), // 垂直居中一点
        };

        _inputHintLabel = new ThemedLabel
        {
            Text = "请输入参数...",
            Font = new Font(AppTheme.Fonts.FontFamily, 14, FontStyle.Regular),
            AutoSize = true,
            Location = new Point(screen.Width / 2 - 100, 20),
            Anchor = AnchorStyles.Top,
        };

        _inputTextBox = new ThemedTextBox
        {
            Font = new Font("Consolas", 16),
            Size = new Size(400, 40),
            Location = new Point(screen.Width / 2 - 200, 60),
        };
        _inputTextBox.KeyDown += InputTextBox_KeyDown;

        _inputContainer.Controls.Add(_inputHintLabel);
        _inputContainer.Controls.Add(_inputTextBox);

        // 调整控件层级
        this.Controls.Add(_itemsPanel);
        this.Controls.Add(_inputContainer); // 放在 itemsPanel 同级
        this.Controls.Add(_breadcrumbPanel);
    }

    private void AnimationTimer_Tick(object? sender, EventArgs e)
    {
        if (_isClosing)
        {
            // 淡出 (Fade Out)
            if (this.Opacity > 0)
            {
                this.Opacity -= ANIMATION_SPEED;
            }
            else
            {
                _animTimer.Stop();
                base.Hide(); // 真正隐藏窗体
                _isClosing = false;
                ResetState(); // 隐藏后重置状态
            }
        }
        else
        {
            // 淡入 (Fade In)
            if (this.Opacity < TARGET_OPACITY)
            {
                this.Opacity += ANIMATION_SPEED;
            }
            else
            {
                this.Opacity = TARGET_OPACITY;
                _animTimer.Stop();
            }
        }
    }

    private void RefreshUI()
    {
        // 如果正在输入模式，不需要刷新列表
        if (_inputContainer.Visible)
            return;

        _breadcrumbPanel.SuspendLayout();
        _itemsPanel.SuspendLayout();

        // 1. 更新面包屑
        _breadcrumbPanel.Controls.Clear();
        AddBreadcrumbItem("Leader");
        foreach (var node in _pathStack.Reverse())
        {
            AddBreadcrumbItem(" > ");
            AddBreadcrumbItem(node.Text);
        }

        // 2. 更新按键列表
        _itemsPanel.Controls.Clear();

        if (_currentNodes != null)
        {
            // 【修改】直接遍历，移除 .OrderBy(n => n.Key)
            // 这样顺序将完全取决于 KeyMapRepository 加载出来的 List 顺序 (YAML顺序)
            foreach (var node in _currentNodes)
            {
                var itemControl = CreateItemControl(node);
                _itemsPanel.Controls.Add(itemControl);
            }

            if (_currentNodes.Count == 0)
            {
                var lbl = new ThemedLabel
                {
                    Text = "没有可用操作",
                    AutoSize = true,
                    ForeColor = Color.Gray,
                };
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
            Margin = new Padding(0),
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
            BackColor = Color.Transparent,
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
            BorderStyle = BorderStyle.FixedSingle,
        };

        var lblText = new Label
        {
            Text = node.Text,
            Font = AppTheme.Fonts.Regular,
            ForeColor = AppTheme.Colors.Text,
            Location = new Point(keyBoxSize + ScaleHelper.Scale(5), 0),
            Size = new Size(itemWidth - keyBoxSize - ScaleHelper.Scale(5), itemHeight),
            TextAlign = ContentAlignment.MiddleLeft,
        };

        panel.Controls.Add(lblKey);
        panel.Controls.Add(lblText);
        return panel;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // 输入模式下，将所有按键交给 TextBox 处理 (除了 ESC)
        if (_inputContainer.Visible)
        {
            if (keyData == Keys.Escape)
            {
                ExitInputMode();
                return true;
            }
            return false; // 让 TextBox 正常处理输入
        }

        // 导航模式下处理 ESC
        if (keyData == Keys.Escape)
        {
            if (_pathStack.Count > 0)
            {
                _pathStack.Pop();
                if (_pathStack.Count > 0)
                    _currentNodes = _pathStack.Peek().Children ?? [];
                else
                    _currentNodes = _rootNodes;

                RefreshUI();
            }
            else
            {
                CloseWithAnimation();
            }
            return true;
        }

        // 字符匹配
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
        if (_currentNodes == null)
            return;

        var targetNode = _currentNodes.FirstOrDefault(n => n.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        if (targetNode == null)
            return;

        if (targetNode.IsLeaf)
        {
            // 【修改】检查是否需要输入参数
            if (targetNode.RequiresInput)
            {
                EnterInputMode(targetNode);
            }
            else
            {
                ExecuteAction(targetNode);
            }
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

    // --- 新增：输入模式逻辑 ---

    private void EnterInputMode(KeyMapNode node)
    {
        _pendingInputNode = node;

        // 界面切换
        _itemsPanel.Visible = false;
        _inputContainer.Visible = true;

        // 设置提示
        _inputHintLabel.Text = string.IsNullOrEmpty(node.InputHint) ? $"请输入 [{node.Text}] 的参数:" : node.InputHint;

        // 居中调整 (简单计算)
        _inputHintLabel.Left = (this.Width - _inputHintLabel.Width) / 2;

        _inputTextBox.Clear();
        _inputTextBox.Focus();
    }

    private void ExitInputMode()
    {
        _pendingInputNode = null;
        _inputContainer.Visible = false;
        _itemsPanel.Visible = true;
        _inputTextBox.Clear();
        // 恢复焦点到窗体以便接收 ProcessCmdKey
        this.Focus();
    }

    private void InputTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true; // 防止嘟嘟声
            if (_pendingInputNode != null)
            {
                string inputVal = _inputTextBox.Text;
                ExecuteAction(_pendingInputNode, inputVal);
            }
        }
    }

    // 【修改】增加 args 参数
    private void ExecuteAction(KeyMapNode node, object? args = null)
    {
        CloseWithAnimation();
        // 确保隐藏后状态复原，下次打开不残留输入框
        ExitInputMode();

        if (_commandDispatcher != null && !string.IsNullOrEmpty(node.Action))
        {
            _ = _commandDispatcher.ExecuteAsync(node.Action, args);
        }
        else
        {
            string msg = $"[演示模式] 执行命令:\n{node.Action}";
            if (args != null)
                msg += $"\n参数: {args}";
            ThemedMessageBox.Show(msg, "Leader Key Action");
        }

        LogManager.WriteDebugLog("LeaderKeyForm", $"执行操作: {node.Action} 参数: {args}");
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
        CloseWithAnimation();
        ResetState();
    }

    private string GetCharFromKeys(Keys key)
    {
        var k = key & ~Keys.Modifiers;
        if (k >= Keys.A && k <= Keys.Z)
            return k.ToString().ToLower();
        if (k >= Keys.D0 && k <= Keys.D9)
            return k.ToString().Replace("D", "");
        if (k >= Keys.NumPad0 && k <= Keys.NumPad9)
            return k.ToString().Replace("NumPad", "");
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
            _rootNodes = [new KeyMapNode { Text = $"配置加载失败: {ex.Message}", Key = "!" }];
        }
        // 调用 ResetState 将 _currentNodes 指向新的 _rootNodes
        ResetState();
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);

        // 当窗体变为可见时
        if (this.Visible)
        {
            // 确保从 0 开始淡入
            this.Opacity = 0;
            _isClosing = false;

            // 如果之前有重置逻辑，确保在这里调用或保留
            ResetState();

            _animTimer.Start();
        }
    }

    /// <summary>
    /// 优雅关闭：淡出动画
    /// </summary>
    private void CloseWithAnimation()
    {
        if (!_isClosing)
        {
            _isClosing = true;
            _animTimer.Start();
        }
    }
}
