#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/19 13:38:24 
* 文件名：CommonHelper 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using BMap.Core.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMap.WinForm
{
    /// <summary>
    /// 扩展方法类
    /// </summary>
    public static class Extension
    {
        public static Point ToPoint(this PointInt p)
        {
            return new Point(p.X, p.Y);
        }
        public static Point ToPoint(this PointInt64 p)
        {
            return new Point((int)p.X, (int)p.Y);
        }
        public static PointF ToPointF(this PointDouble p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }
        public static Point ToPoint(this PointDouble p)
        {
            return new Point((int)Math.Round(p.X), (int)Math.Round(p.Y));
        }
        public static Size ToSize(this SizeInt s)
        {
            return new Size(s.Width, s.Height);
        }
        public static Size ToSize(this BSize<int> s)
        {
            return new Size((int)s.Width, (int)s.Height);
        }
        public static Size ToSize(this BSize<long> s)
        {
            return new Size((int)s.Width, (int)s.Height);
        }
        public static Rectangle ToRectangle(this RectInt r)
        {
            return new Rectangle(r.LeftTop.X,r.LeftTop.Y, (int)r.Width, (int)r.Height);
        }
        public static Rectangle ToRectangle(this RectInt64 r)
        {
            return new Rectangle((int)r.LeftTop.X, (int)r.LeftTop.Y, (int)r.Width, (int)r.Height);
        }

        public static Point GetOffSet(this Point ip,int dx,int dy)
        {
            ip.Offset(dx, dy);
            return ip;
        }
      
    }
}
