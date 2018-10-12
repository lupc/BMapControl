#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/18 10:07:18 
* 文件名：EditMode 
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

namespace BMap.Core
{
    /// <summary>
    /// 操作模式
    /// </summary>
    public enum EditMode
    {

		None =0,
        /// <summary>
        /// 画直线
        /// </summary>
        DrawLine,
        /// <summary>
        /// 画折线
        /// </summary>
        DrawLines,
        /// <summary>
        /// 画矩形
        /// </summary>
        DrawRect,
        /// <summary>
        /// 画多边形
        /// </summary>
        DrawPolyon,
        /// <summary>
        /// 画圆
        /// </summary>
        DrawCircle,
        /// <summary>
        /// 画扇形
        /// </summary>
        DrawPie,
    }
}
