#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/29 9:23:38 
* 文件名：MarkerRoundedToolTip 
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

namespace BMap.WinForm
{
    /// <summary>
    /// 圆角Tooltip
    /// </summary>
    public class MarkerRoundedToolTip : MarkerToolTip
    {
        public MarkerRoundedToolTip(MarkerBase m):base(m)
        {

        }

        public int Radius { get; set; } = 5;
        

        protected override void BuildPath(GraphicsPath gp)
        {
            int r = Radius;
            int R = Radius * 2;//直径
            var p = Marker.ScreenPosition.GetOffSet(Offset.X, Offset.Y);
            var w = ViewRect.Width;
            var h = ViewRect.Height;
            var startAngle = 0;
            var rh = h - ArrowSize.Height;
            gp.AddLine(p, p = p.GetOffSet(ArrowSize.Width / 2, -ArrowSize.Height));
            gp.AddLine(p, p = p.GetOffSet((w - ArrowSize.Width) / 2 - r, 0));
            p.Offset(-r, -R);
            AddRound(gp, p, R, startAngle);//画右下圆角
            p.Offset(R, r);
            gp.AddLine(p, p = p.GetOffSet(0, -(rh - R)));
            p.Offset(-R, -r);
            startAngle -= 90;
            AddRound(gp, p, R, startAngle);//画右上圆角
            p.Offset(r, 0);
            gp.AddLine(p, p = p.GetOffSet(-(w - R), 0));
            p.Offset(-r, 0);
            startAngle -= 90;
            AddRound(gp, p, R, startAngle);//画左上圆角
            p.Offset(0, r);
            gp.AddLine(p, p = p.GetOffSet(0, rh - R));
            p.Offset(0, -r);
            startAngle -= 90;
            AddRound(gp, p, R, startAngle);//画左下圆角
            p.Offset(r, R);
            gp.AddLine(p, p = p.GetOffSet((w - ArrowSize.Width) / 2 - r, 0));


            //gp.AddLine(p, p.GetAndSetOffSet(ArrowSize.Width/ 2, ArrowSize.Height);



            gp.CloseFigure();
        }
        private void AddRound(GraphicsPath gp, Point p, int R, float startAngle)
        {
            if (R>0)
            {
                gp.AddArc(p.X, p.Y, R, R, startAngle, 90);
            }
        }
    }
}
