#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/30 13:55:14 
* 文件名：DrawHelper 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BMap.WinForm
{
    public class DrawHelper
    {
        /// <summary>
        /// 根据两点获取线段所在刷新区域
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Region GetLineRegion(Point p1,Point p2)
        {
            int left = Math.Min(p1.X, p2.X)-5;
            int top = Math.Min(p1.Y, p2.Y)-5;
            int right = Math.Max(p1.X, p2.X) + 5;
            int bottom = Math.Max(p1.Y, p2.Y) +5;

            //using (var gp = new GraphicsPath())
            //{
            //    gp.AddLine()

            //    var p1 = c.LatLngToPoint(LstMarkPoint[0]);
            //    //gp.(p1, _lastMousePoint);
            //    //gp.AddLine(p1.GetOffSet(-5,0), p1=p1.GetOffSet(5,0));
            //    //gp.AddLine(p1, p1 = _lastMousePoint.GetOffSet(5,0));
            //    //gp.AddLine(p1, p1 = p1.GetOffSet(-10, 0));
            //    //gp.CloseFigure();
            //    c.Invalidate(DrawHelper.GetRegion(p1, _lastMousePoint));
            //    //gp.ClearMarkers();
            //    //gp.AddLine(LatLngToPoint(UserEdit.LstPoint[0]), _curMousePoint);
            //    //this.Invalidate(new Region(gp));


            //}

            return new Region(new Rectangle(left, top, right - left, bottom - top));
        }

        public static Region GetCircleRegion(Point pCenter,Point pOutSide)
        {
            Region reg = new Region();
            using (var gp = new GraphicsPath())
            {
                var rect2 = GetEllipseRect(pCenter, pOutSide);
                rect2.Offset(-5, -5);
                rect2.Width += 10;
                rect2.Height += 10;
                gp.AddEllipse(rect2);
                
                //gp.Widen(pen);
                reg = new Region(gp);
            }

            return reg;
        }

        public static Rectangle GetEllipseRect(Point pCenter, Point pOutSide)
        {
            var r = (int)Math.Round(GetDistance(pCenter, pOutSide));
            return new Rectangle(pCenter.X - r, pCenter.Y - r, r * 2, r * 2);
        }

        public static double GetDistance(Point p1,Point p2)
        {
            var rect  = GetRect(p1,p2);
            return Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height);
        }

        /// <summary>
        /// 根据两点获取矩形
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Rectangle GetRect(Point p1, Point p2)
        {
            int left = Math.Min(p1.X, p2.X);
            int top = Math.Min(p1.Y, p2.Y);
            int right = Math.Max(p1.X, p2.X) ;
            int bottom = Math.Max(p1.Y, p2.Y);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public static float GetAngle(Point p1,Point p2)
        {
            float angle = 0;
            if (!p1.Equals(p2))
            {
               
                //两点的x、y值
                double x = p2.X - p1.X;
                double y = ( p2.Y - p1.Y);

                var radian = Math.Atan2(y ,x);


                //double hypotenuse = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                ////斜边长度
                //double cos = x / hypotenuse;
                //double radian = Math.Acos(cos);
                ////求出弧度
                ////angle = (float)BMap.Core.MapHelper.DegreesToRadians(radian);
                angle = (float)(180 / (Math.PI / radian)) % 360;
                ////用弧度算出角度       
                //if (y < 0)
                //{
                //    angle = -angle;
                //}
                //else if ((y == 0) && (x < 0))
                //{
                //    angle = 180;
                //}
            }
            
            return angle;
        }

        public static double GetAngle(Point cen, Point first, Point second)
        {
            double angle = 0;
            angle = Math.Abs( GetAngle(cen, first) - GetAngle(cen, second));
            if (angle > 180)
            {
                angle = 360 - angle;
            }
            return angle;
        }

        /// <summary>
        /// 获取圆角矩形路径
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="r">圆角半径</param>
        /// <returns></returns>
        public static GraphicsPath GetRoundRect(Point location,Size size,int r)
        {
            GraphicsPath gp = new GraphicsPath();
            var p = location;
            var R = 2 * r;
            float startAngle = 180;
            AddRound(gp, p, R, startAngle);
            p.Offset(r, 0);
            gp.AddLine(p, p = p.GetOffSet(size.Width - R, 0));
            p.Offset(-r, 0);
            startAngle += 90;
            AddRound(gp, p, R, startAngle);
            p.Offset(R, r);
            gp.AddLine(p, p = p.GetOffSet(0, size.Height - R));
            p.Offset(-R, -r);
            startAngle += 90;
            AddRound(gp, p, R, startAngle);
            p.Offset(r, R);
            gp.AddLine(p, p = p.GetOffSet(-(size.Width - R), 0));
            p.Offset(-r, -R);
            startAngle += 90;
            AddRound(gp, p, R, startAngle);
            p.Offset(0, r);
            gp.AddLine(p, p = p.GetOffSet(0, size.Height - R));

            gp.CloseFigure();



            return gp;
        }
        /// <summary>
        /// 获取圆角矩形路径
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="r">圆角半径</param>
        /// <returns></returns>
        public static GraphicsPath GetRoundRect(Rectangle rect,int r)
        {
            return GetRoundRect(rect.Location, rect.Size, r);
        }

        public static void AddRound(GraphicsPath gp, Point p, int R, float startAngle)
        {
            if (R > 0)
            {
                gp.AddArc(p.X, p.Y, R, R, startAngle%360, 90);
            }
        }

    }
}
