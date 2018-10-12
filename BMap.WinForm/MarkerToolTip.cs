#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/28 15:49:30 
* 文件名：MarkerToolTip 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMap.WinForm
{
    /// <summary>
    /// 标记ToolTip
    /// </summary>
    public class MarkerToolTip
    {
        /// <summary>
        /// 所属标记
        /// </summary>
        public MarkerBase Marker { get; set; }
        /// <summary>
        /// 位置偏移
        /// </summary>
        public Point Offset { get; set; } = new Point(0,-15);
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } 
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 字体
        /// </summary>
        public Font Font { get; set; } = new Font("宋体", 11, FontStyle.Regular);
        /// <summary>
        /// 边
        /// </summary>
        public Pen Stroke { get; set; } = null;

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Brush Background { get; set; } = new SolidBrush(Color.FromArgb(0xaa, 0xe0, 0x66, 0xff));
        /// <summary>
        /// 前景色
        /// </summary>
        public Brush Foreground { get; set; }=new SolidBrush(Color.FromArgb(0xFF, 0xff, 0xFF, 0xFF));
        /// <summary>
        /// 标题边距
        /// </summary>
        public Size TitlePadding { get; set; } = new Size(5, 5);
        /// <summary>
        /// 内容边距
        /// </summary>
        public Size ContentPadding { get; set; } = new Size(8, 8);
        /// <summary>
        /// 最大宽度
        /// </summary>
        public double MaxWidth { get; set; } = 400;
        /// <summary>
        /// 最大高度
        /// </summary>
        public double MaxHeight { get; set; } = 300;
        /// <summary>
        /// 屏幕可见区域
        /// </summary>
        public Rectangle ViewRect { get; set; }
        /// <summary>
        /// 重绘区域
        /// </summary>
        public Region RefreshRegion { get; set; }
        /// <summary>
        /// 关闭按钮区域
        /// </summary>
        public Rectangle CloseButtonRect { get; set; }
        /// <summary>
        /// 箭头尺寸
        /// </summary>
        public Size ArrowSize { get; set; } = new Size(20, 10);

        public bool IsMouseOver { get; set; }

        private bool _isOpen;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    var c = Marker.Owner.MapObject;
                    c.Invalidate(RefreshRegion);
                    c.Update();

                    if (_isOpen)
                    {
                        if (ToolTipOpened!=null)
                        {
                            ToolTipOpened(this);
                        }
                    }
                    else
                    {
                        if (ToolTipClosed!=null)
                        {
                            ToolTipClosed(this);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 提示关闭事件
        /// </summary>
        public event Action<MarkerToolTip> ToolTipClosed;
        /// <summary>
        /// 提示打开事件
        /// </summary>
        public event Action<MarkerToolTip> ToolTipOpened;
        public MarkerToolTip(MarkerBase m)
        {
            Marker = m;
            //Stroke = new Pen(new SolidBrush(Color.FromArgb(0xee, 0xff, 0x83, 0xfa)), 1);
        }

       
        public void OnMouseClick(BMapControl c, MouseEventArgs e)
        {
            if (!this.CloseButtonRect.IsEmpty&&CloseButtonRect.Contains(e.Location))
            {
                //关闭
                IsOpen = false;
            }
        }


        public virtual void Draw(BMapControl c, Graphics g)
        {
            var p = Marker.ScreenPosition.GetOffSet(Offset.X, Offset.Y);
            var titleSize = g.MeasureString(Title, this.Font).ToSize();
            var contentSize = g.MeasureString(Content, Font).ToSize();
            int w = Math.Max(titleSize.Width + TitlePadding.Width * 2, contentSize.Width + ContentPadding.Width * 2);
            w = (int)Math.Min(w, MaxWidth);
            int h = (int)Math.Min(titleSize.Height + TitlePadding.Height * 2 + contentSize.Height + ContentPadding.Height * 2 + ArrowSize.Height, MaxHeight);
            ViewRect = new Rectangle(p.X - (int)w / 2, p.Y - h, w, h);




            using (GraphicsPath gp = new GraphicsPath())
            {

                BuildPath(gp);
                RefreshRegion = new Region(gp);
                

                if ((IsOpen || Marker.ToolTipMode == ToolTipShowMode.Always) && Marker.ToolTipMode != ToolTipShowMode.Never)
                {

                    if (Background != null)
                    {
                        g.FillPath(Background, gp);
                    }

                    if (Stroke != null)
                    {
                        g.DrawPath(Stroke, gp);
                    }

                    if (!string.IsNullOrEmpty(Title))
                    {
                        g.DrawString(Title, new Font(Font, FontStyle.Bold), Foreground, ViewRect.Location.GetOffSet(TitlePadding.Width, TitlePadding.Height));

                    }

                    //画标题与内容中间的分割线
                    var pl1 = ViewRect.Location.GetOffSet(0, titleSize.Height + TitlePadding.Height * 2);
                    var pl2 = pl1.GetOffSet(w, 0);
                    g.DrawLine(new Pen(Foreground, 1), pl1, pl2);


                    if (!string.IsNullOrEmpty(Content))
                    {
                        var pContent = ViewRect.Location.GetOffSet(ContentPadding.Width, titleSize.Height + TitlePadding.Height * 2 + ContentPadding.Height);
                        g.DrawString(Content, Font, Foreground, pContent);
                    }

                    DrawCloseButton(g);
                }
            }
        }

        /// <summary>
        /// 构建外壳路径
        /// </summary>
        /// <param name="gp"></param>
        protected virtual void BuildPath(GraphicsPath gp)
        {
            List<Point> ps = new List<Point>();
            var p = Marker.ScreenPosition.GetOffSet(Offset.X, Offset.Y);
            var w = ViewRect.Width;
            var h = ViewRect.Height;
            ps.Add(p);//箭头指向点
            p.Offset(ArrowSize.Width / 2, -ArrowSize.Height);//箭头右侧
            ps.Add(p);
            p.Offset((w - ArrowSize.Width) / 2, 0);//右下角
            ps.Add(p);
            p.Offset(0, -(h - ArrowSize.Height));//右上角
            ps.Add(p);
            p.Offset(-w, 0);//左上角
            ps.Add(p);
            p.Offset(0, (h - ArrowSize.Height));//左下角
            ps.Add(p);
            p.Offset((w - ArrowSize.Width) / 2, 0);//箭头左侧
            ps.Add(p);
            gp.AddPolygon(ps.ToArray());
        }

        protected virtual void DrawCloseButton(Graphics g)
        {

            //画关闭按钮
            switch (Marker.ToolTipMode)
            {
                case ToolTipShowMode.MouseClick:
                //case ToolTipShowMode.Always:
                    //显示关闭按钮
                    CloseButtonRect = new Rectangle(ViewRect.Right - 25, ViewRect.Top + 5, 20, 20);
                    Pen pen = new Pen(Foreground, 2);
                    if (IsMouseOver)
                    {
                        g.FillEllipse(new SolidBrush(Color.FromArgb(0x88, Color.Red)), CloseButtonRect);
                    }
                    var p = CloseButtonRect.Location.GetOffSet(4, 4);
                    g.DrawLine(pen, p, p.GetOffSet(12, 12));
                    p.Offset(12, 0);
                    g.DrawLine(pen, p, p.GetOffSet(-12, 12));
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 提示显示方式
    /// </summary>
    public enum ToolTipShowMode
    {
        /// <summary>
        /// 鼠标经过过显示
        /// </summary>
        MouseOver,
        /// <summary>
        /// 鼠标点击显示
        /// </summary>
        MouseClick,
        /// <summary>
        /// 一直显示
        /// </summary>
        Always,
        /// <summary>
        /// 一直不显示
        /// </summary>
        Never,
    }
}
