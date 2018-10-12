#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/30 10:17:56 
* 文件名：InfoToolTip 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMap.WinForm
{
    public class InfoToolTip
    {
        public InfoToolTip()
        {

        }
        /// <summary>
        /// 显示的信息
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 显示位置
        /// </summary>
        public Point ShowPoint { get; set; }
        /// <summary>
        /// 背景色
        /// </summary>
        public Brush Background { get; set; } = new SolidBrush(Color.FromArgb(0x77, Color.Blue));
        /// <summary>
        /// 前景色
        /// </summary>
        public Brush Foreground { get; set; } = Brushes.White;
        /// <summary>
        /// 字体
        /// </summary>
        public Font Font { get; set; } = new Font("微软雅黑", 10, FontStyle.Regular);
        /// <summary>
        /// 字体外边距
        /// </summary>
        public Size InfoPadding { get; set; } = new Size(5, 5);
        /// <summary>
        /// 重绘区域
        /// </summary>
        public Region RefreshRegion { get; set; }
        public virtual void Draw(BMapControl c, Graphics g)
        {
            if (!string.IsNullOrEmpty(Info))
            {
                var size = g.MeasureString(Info, Font);
                int w = (int)size.Width + InfoPadding.Width * 2;
                int h = (int)size.Height + InfoPadding.Height * 2;
                var pen = new Pen(Foreground, 1);
                var p = ShowPoint;

                var pRT = ShowPoint.GetOffSet(w + 20, -(h + 20));

                if (c.ClientRectangle.Right < pRT.X)
                {
                    //屏幕右边越界


                    if (c.ClientRectangle.Top > pRT.Y)
                    {
                        //屏幕上面越界
                        g.DrawLine(pen, p, p = p.GetOffSet(-20, 20));
                        g.DrawLine(pen, p, p = p.GetOffSet(-w, 0));
                        RefreshRegion = new Region(new Rectangle(ShowPoint.X - 20 - w, ShowPoint.Y , w + 20, h + 20));
                    }
                    else
                    {
                        
                        g.DrawLine(pen, p, p = p.GetOffSet(-20, -20));
                        g.DrawLine(pen, p, p = p.GetOffSet(-w, 0));
                        p.Offset(0, -h);
                        RefreshRegion = new Region(new Rectangle(ShowPoint.X-20-w, ShowPoint.Y-20-h, w + 20, h + 20));
                    }

                }
                else
                {

                    if (c.ClientRectangle.Top > pRT.Y)
                    {
                        //屏幕上面越界
                        g.DrawLine(pen, p, p = p.GetOffSet(20, 20));
                        g.DrawLine(pen, p, p.GetOffSet(w, 0));
                        //p.Offset(0, 0);
                        RefreshRegion = new Region(new Rectangle(ShowPoint.X, ShowPoint.Y, w + 20, h + 20));
                    }
                    else
                    {
                        g.DrawLine(pen, p, p = p.GetOffSet(20, -20));
                        g.DrawLine(pen, p, p.GetOffSet(w, 0));
                        p.Offset(0, -h);
                        RefreshRegion = new Region(new Rectangle(ShowPoint.X, ShowPoint.Y - 20 - h, w + 20, h + 20));
                    }

                }

                //g.DrawLine(pen, p, p = p.GetOffSet(20, -20));
                //g.DrawLine(pen, p, p.GetOffSet(w, 0));
                //p.Offset(0, -h);
                

                var rect = new Rectangle(p, new Size(w, h));
                g.FillRectangle(Background, rect);
                p.Offset(InfoPadding.Width, InfoPadding.Height);
                g.DrawString(Info, Font, Foreground, p);

                

            }


           
        }
    }
}
