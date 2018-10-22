#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/17 15:12:17 
* 文件名：DrawGrid 
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
using System.Text;

namespace BMap.Core.Model
{
    public class DrawGrid
    {
        /// <summary>
        /// 左上角坐标点
        /// </summary>
        public PointInt LeftTop { get; set; }
        /// <summary>
        /// 右下角坐标
        /// </summary>
        public PointInt RightBottom { get; set; }
        /// <summary>
        /// 单格大小
        /// </summary>
        public SizeInt GridSize { get; set; }
        /// <summary>
        /// X方向网格线数量
        /// </summary>
        public int XCount { get; set; }
        /// <summary>
        /// Y方向网格线数量
        /// </summary>
        public int YCount { get; set; }
    }
}
