#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 10:45:38 
* 文件名：LatLng 
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
    public class PointLatLng
    {
        public PointLatLng()
        {

        }
        public PointLatLng(double lngX, double latY)
        {
            LatY = latY;
            LngX = lngX;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public double LatY { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public double LngX { get; set; }

        /// <summary>
        /// 获取偏移后的经纬度坐标
        /// </summary>
        /// <param name="widthLng"></param>
        /// <param name="heightLat"></param>
        /// <returns></returns>
        public PointLatLng GetOffSet(double widthLng,double heightLat)
        {
            return new PointLatLng(this.LngX + widthLng,this.LatY + heightLat);
        }
        /// <summary>
        /// 获取偏移后的经纬度坐标
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public PointLatLng OffSet(SizeLatLng size)
        {
            return GetOffSet(size.WidthLng, size.HeightLat);
        }

        public override string ToString()
        {
            return string.Format("经度:{0:f7},纬度:{1:f7}", LngX,LatY);
        }
    }
}
