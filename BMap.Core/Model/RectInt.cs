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
    public class RectInt
    {
        public RectInt()
        {

        }
        public RectInt(PointInt lefttop,int width,int height)
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
        public RectInt(int x, int y, int width, int height)
        {
            From(new PointInt(x, y), width, height);
        }

        public void From(PointInt lefttop, int width, int height)
        {
            this.LeftTop = lefttop;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// 左上角坐标点
        /// </summary>
        public PointInt LeftTop { get; set; }
        /// <summary>
        /// 右下角坐标点
        /// </summary>
        public PointInt RightBottom {
            get
            {
                return LeftTop.GetOffSet(Width,Height);
            }
        }
       
        /// <summary>
        /// 坐标宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 坐标高度
        /// </summary>
        public int Height { get; set; }
        
        /// <summary>
        /// 矩形中点经纬度
        /// </summary>
        public PointDouble Center
        {
            get
            {
                PointDouble ret = new PointDouble(LeftTop.X+this.Width/2,LeftTop.Y+this.Height/2);
                return ret;
            }
        }




        public bool Contains(int x, int y)
        {
            return ((((this.LeftTop.X <= x) && (x < (this.LeftTop.X + this.Width))) && (this.LeftTop.Y >= y)) && (y > (this.LeftTop.Y - this.Height)));
        }

        public bool Contains(PointInt pt)
        {
            return this.Contains(pt.X, pt.Y);
        }

        public bool Contains(RectInt rect)
        {
            bool b = false;

            if (rect!=null)
            {
                b = this.LeftTop.X <= rect.LeftTop.X 
                    && this.LeftTop.Y <= rect.LeftTop.Y 
                    && this.RightBottom.X >= rect.RightBottom.X 
                    && this.RightBottom.Y >= rect.RightBottom.Y;
            }

            return b;
        }

        public void OffSet(int x,int y)
        {
            this.LeftTop.Offset(x, y);
        }

        public override bool Equals(object obj)
        {
            var rect = obj as RectInt;
            return (rect != null && this.LeftTop.Equals(rect.LeftTop) && this.Width == rect.Width && this.Height == rect.Height);
        }
    }
}
