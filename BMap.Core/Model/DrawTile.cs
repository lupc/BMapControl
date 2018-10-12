#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 9:50:22 
* 文件名：Tile 
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
    public class DrawTile
    {
        public DrawTile()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">屏幕X坐标</param>
        /// <param name="y">屏幕Y坐标</param>
        public DrawTile(int x,int y)
        {
            DrawPosition = new PointInt(x, y);
        }

        public Tile Tile { get; set; }
        /// <summary>
        /// 贴图位置（屏幕坐标系）
        /// </summary>
        public PointInt DrawPosition { get; set; }

       
        public override string ToString()
        {
            return string.Format("贴图位置:{0}\r\n瓦片:{1},",DrawPosition,Tile);
        }
    }
}
