#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/30 15:09:25 
* 文件名：RectMarker 
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
    public class RectMarker : MarkerBase
    {
        public RectMarker(string name) : base(name)
        {
        }
        
        /// <summary>
        /// 边宽
        /// </summary>
        public int BorderWidth { get; set; } = 2;
        /// <summary>
        /// 填充颜色
        /// </summary>
        public Brush FillBrush { get; set; } = new SolidBrush(Color.FromArgb(0x44, Color.Blue));
        /// <summary>
        /// 边框颜色
        /// </summary>
        public Brush BorderBrush { get; set; } = new SolidBrush(Color.FromArgb(0x44, Color.White));

        public override bool Contains(Point p)
        {
            return ScreenArea.Contains(p);
        }

        public override void Draw(BMapControl c, Graphics g)
        {
            var p = c.LatLngToPoint(Position);
            ScreenArea = new Rectangle(p, Size);
            RefreshRegion = new Region(ScreenArea);
            g.FillRectangle(FillBrush, ScreenArea);
            g.DrawRectangle(new Pen(BorderBrush,BorderWidth), ScreenArea);
        }
    }
}
