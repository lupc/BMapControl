#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 10:52:56 
* 文件名：RectLatLng 
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
    public class RectInt64
    {
        public RectInt64()
        {

        }
        public RectInt64(PointInt64 lefttop,long width,long height)
        {
            From(lefttop, width, height);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public RectInt64(long x,long y, long width, long height)
        {
            From(new PointInt64(x, y), width, height);
        }

        public void From(PointInt64 lefttop, long width, long height)
        {
            this.LeftTop = lefttop;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// 左上角坐标点
        /// </summary>
        public PointInt64 LeftTop { get; set; }
        /// <summary>
        /// 右下角坐标点
        /// </summary>
        public PointInt64 RightBottom {
            get
            {
                return LeftTop.GetOffSet(Width,Height);
            }
        }
        /// <summary>
        /// 坐标宽度
        /// </summary>
        public long Width { get; set; }
        /// <summary>
        /// 坐标高度
        /// </summary>
        public long Height { get; set; }
        
        /// <summary>
        /// 矩形中点经纬度
        /// </summary>
        public PointInt64 Center
        {
            get
            {
                PointInt64 ret = this.LeftTop.GetOffSet(Width / 2, Height / 2);
                return ret;
            }
        }




        public bool Contains(long x, long y)
        {
            return ((((this.LeftTop.X <= x) && (x < (this.LeftTop.X + this.Width))) && (this.LeftTop.Y >= y)) && (y > (this.LeftTop.Y - this.Height)));
        }

        public bool Contains(PointInt64 pt)
        {
            return this.Contains(pt.X, pt.Y);
        }
    }
}
