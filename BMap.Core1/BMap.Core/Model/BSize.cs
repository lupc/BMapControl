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
    public abstract class BSize<T>where T:struct
    {
        public BSize()
        {

        }
        /// <summary>
        /// 新建尺寸
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public BSize(T w,T h)
        {
            this.Width = w;
            this.Height = h;
        }

        /// <summary>
        /// 宽
        /// </summary>
        public T Width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public T Height { get; set; }
        public override string ToString()
        {
            return string.Format("宽:{0},高:{1}", Width, Height);
        }
    }
}
