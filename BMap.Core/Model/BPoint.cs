#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 14:44:19 
* 文件名：PointBase 
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
    public abstract class BPoint<T>where T: struct
    {
       
        public BPoint()
        {
                
        }
        /// <summary>
        /// 新建点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public BPoint(T x,T y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// X坐标
        /// </summary>
        public T X { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public T Y { get; set; }
        public override string ToString()
        {
            return string.Format("X:{0},Y:{1}", X, Y);
        }

        
    }
}
