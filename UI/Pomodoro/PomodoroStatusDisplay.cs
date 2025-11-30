using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Components
{
    /// <summary>
    /// 番茄钟状态可视化组件
    /// 将完成数量转换为图标显示 (每4个小番茄 = 1个大番茄)
    /// </summary>
    public class PomodoroStatusDisplay : Control
    {
        private int _totalCompletedCount = 0;
        private int _iconSize = 24;
        private int _iconSpacing = 5;
        private Image? _smallIconImage;
        private Image? _bigIconImage;

        public PomodoroStatusDisplay()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(
                ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint,
                true
            );
            this.DoubleBuffered = true;

            // 2. 设置默认背景色为透明
            this.BackColor = Color.Transparent;
            this.Size = new Size(300, 40);
        }

        #region 公开属性

        [Category("Pomodoro Data")]
        [Description("总共完成的番茄钟数量")]
        public int TotalCompletedCount
        {
            get => _totalCompletedCount;
            set
            {
                if (_totalCompletedCount != value)
                {
                    _totalCompletedCount = value;
                    Invalidate(); // 触发重绘
                }
            }
        }

        [Category("Pomodoro Appearance")]
        [Description("图标的大小 (像素)")]
        public int IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                Invalidate();
            }
        }

        [Category("Pomodoro Appearance")]
        [Description("图标之间的间距")]
        public int IconSpacing
        {
            get => _iconSpacing;
            set
            {
                _iconSpacing = value;
                Invalidate();
            }
        }

        [Category("Pomodoro Appearance")]
        [Description("自定义小番茄图片 (如果为空，则绘制默认图形)")]
        public Image? SmallIconImage
        {
            get => _smallIconImage;
            set
            {
                _smallIconImage = value;
                Invalidate();
            }
        }

        [Category("Pomodoro Appearance")]
        [Description("自定义大番茄图片 (代表4个周期，如果为空，则绘制默认图形)")]
        public Image? BigIconImage
        {
            get => _bigIconImage;
            set
            {
                _bigIconImage = value;
                Invalidate();
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // --- 核心修改：计算居中位置 ---

            int bigCount = _totalCompletedCount / 4;
            int smallCount = _totalCompletedCount % 4;
            int totalIconsToDraw = bigCount + smallCount;

            if (totalIconsToDraw == 0)
                return;

            // 1. 计算内容总宽度
            // 总宽度 = (图标数量 * 图标大小) + ((图标数量 - 1) * 间距)
            // 还要加上大小图标之间额外的间距(如果有混合显示)
            int contentWidth = (totalIconsToDraw * _iconSize) + ((totalIconsToDraw - 1) * _iconSpacing);
            if (bigCount > 0 && smallCount > 0)
            {
                contentWidth += _iconSpacing; // 额外加一点混合间距
            }

            // 2. 计算起始 X 坐标 (让内容在控件宽度内居中)
            int startX = (this.Width - contentWidth) / 2;
            int y = (this.Height - _iconSize) / 2;

            int currentX = startX;

            // --- 绘制大番茄 ---
            for (int i = 0; i < bigCount; i++)
            {
                DrawIcon(g, currentX, y, true);
                currentX += _iconSize + _iconSpacing;
            }

            // 如果混合显示，加额外间距
            if (bigCount > 0 && smallCount > 0)
            {
                currentX += _iconSpacing;
            }

            // --- 绘制小番茄 ---
            for (int i = 0; i < smallCount; i++)
            {
                DrawIcon(g, currentX, y, false);
                currentX += _iconSize + _iconSpacing;
            }
        }

        private void DrawIcon(Graphics g, int x, int y, bool isBig)
        {
            // 策略：如果有图片就画图片，没有就画默认矢量图

            // --- 大番茄逻辑 ---
            if (isBig)
            {
                if (_bigIconImage != null)
                {
                    g.DrawImage(_bigIconImage, new Rectangle(x, y, _iconSize, _iconSize));
                }
                else
                {
                    // 默认绘制：金色的圆形 (代表成就)
                    using (var brush = new SolidBrush(Color.Gold))
                    using (var pen = new Pen(Color.Orange, 2))
                    {
                        g.FillEllipse(brush, x, y, _iconSize, _iconSize);
                        g.DrawEllipse(pen, x, y, _iconSize, _iconSize);

                        // 画个简单的叶子
                        using (var leafBrush = new SolidBrush(Color.ForestGreen))
                        {
                            g.FillPie(leafBrush, x + _iconSize / 2 - 5, y, 10, 10, 225, 90);
                        }
                    }
                }
            }
            // --- 小番茄逻辑 ---
            else
            {
                if (_smallIconImage != null)
                {
                    g.DrawImage(_smallIconImage, new Rectangle(x, y, _iconSize, _iconSize));
                }
                else
                {
                    // 默认绘制：红色的圆形
                    using (var brush = new SolidBrush(Color.Tomato))
                    using (var pen = new Pen(Color.DarkRed, 1))
                    {
                        g.FillEllipse(brush, x + 2, y + 2, _iconSize - 4, _iconSize - 4); // 稍小一点
                        g.DrawEllipse(pen, x + 2, y + 2, _iconSize - 4, _iconSize - 4);

                        // 画个简单的叶子
                        using (var leafBrush = new SolidBrush(Color.Green))
                        {
                            g.FillPie(leafBrush, x + _iconSize / 2 - 3, y + 1, 6, 6, 225, 90);
                        }
                    }
                }
            }
        }
    }
}
