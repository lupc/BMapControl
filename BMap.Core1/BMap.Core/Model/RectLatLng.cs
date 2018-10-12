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
    public class RectLatLng
    {
        public RectLatLng()
        {

        }
        public RectLatLng(PointLatLng lefttop,double width,double height)
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
        public RectLatLng(double lngX,double latY, double width, double height)
        {
            From(new PointLatLng(lngX, latY), width, height);
        }

        public void From(PointLatLng lefttop, double width, double height)
        {
            this.LeftTop = lefttop;
            this.WidthLng = width;
            this.HeightLat = height;
        }

        /// <summary>
        /// 左上角坐标点
        /// </summary>
        public PointLatLng LeftTop { get; set; }
        /// <summary>
        /// 右下角坐标点
        /// </summary>
        public PointLatLng RightBottom {
            get
            {
                return LeftTop.GetOffSet(WidthLng,HeightLat);
            }
        }
        /// <summary>
        /// 坐标宽度
        /// </summary>
        public double WidthLng { get; set; }
        /// <summary>
        /// 坐标高度
        /// </summary>
        public double HeightLat { get; set; }
        
        /// <summary>
        /// 矩形中点经纬度
        /// </summary>
        public PointLatLng Center
        {
            get
            {
                PointLatLng ret = this.LeftTop.GetOffSet(WidthLng / 2, HeightLat / 2);
                return ret;
            }
        }




        public bool Contains(double lng, double lat)
        {
            return ((((this.LeftTop.LngX <= lng) && (lng < (this.LeftTop.LngX + this.WidthLng))) && (this.LeftTop.LatY >= lat)) && (lat > (this.LeftTop.LatY - this.HeightLat)));
        }

        public bool Contains(PointLatLng pt)
        {
            return this.Contains(pt.LngX, pt.LatY);
        }
    }
}
