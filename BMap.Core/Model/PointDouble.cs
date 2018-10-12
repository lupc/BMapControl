#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/21 10:57:28 
* 文件名：PointDouble 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/ 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace BMap.Core.Model
{
    public class PointDouble:BPoint<double>
    {

        public PointDouble()
        {

        }
        public PointDouble(double x,double y)
        {
            X = x;
            Y = y;
        }
        public static readonly PointDouble Empty = new PointDouble();
        public bool IsEmpty
        {
            get { return this.Equals(Empty); }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            bool b = false;
            PointDouble p = obj as PointDouble;
            if (p!=null)
            {
                b = this.X == p.X && this.Y == p.Y;
            }
            return b;
        }

        public PointDouble GetOffSet(double dx,double dy)
        {
            return new PointDouble(this.X + dx, this.Y + dy);
        }
    }
}
