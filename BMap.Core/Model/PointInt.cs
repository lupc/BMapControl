#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 10:51:01 
* 文件名：PointInt 
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
    public class PointInt:BPoint<int>
    {
        public PointInt()
        {

        }

        public PointInt(int x,int y):base(x,y)
        {

        }
        public override int GetHashCode()
        {
            return (int)(X ^ Y);
        }
        

        public override bool Equals(object obj)
        {
            bool ret = false;
            PointInt p = obj as PointInt;
            if (p != null)
            {
                ret = this.X == p.X && this.Y == p.Y;
            }

            return ret;
        }

        /// <summary>
        /// 获取偏移后的坐标
        /// </summary>
        /// <param name="widthLng"></param>
        /// <param name="heightLat"></param>
        /// <returns></returns>
        public PointInt GetOffSet(int w, int h)
        {
            return new PointInt(this.X + w, this.Y + h);
        }

        internal void Offset(int x, int y)
        {
            this.X += x;
            this.Y += y;
        }
    }
}
