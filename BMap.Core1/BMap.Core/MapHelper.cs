#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 11:29:51 
* 文件名：MapHelper 
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
using BMap.Core.Model;
using System.Linq;

namespace BMap.Core
{
    /// <summary>
    /// 地图通用帮助类
    /// </summary>
    public class MapHelper
    {
        /// <summary>
        /// 圆周率
        /// </summary>
        public const double PI = Math.PI;



        #region 火星坐标(GCJ-02) 地球坐标(WGS-84)互转
        //
        // Krasovsky 1940
        //
        // a = 6378245.0, 1/f = 298.3
        // b = a * (1 - f)
        // ee = (a^2 - b^2) / a^2;
        const double a = 6378245.0;
        const double ee = 0.00669342162296594323;

        //
        // World Geodetic System ==> Mars Geodetic System
        public static void Transform(double wgLat, double wgLon, out double mgLat, out double mgLon)
        {
            if (OutOfChina(wgLat, wgLon))
            {
                mgLat = wgLat;
                mgLon = wgLon;
                return;
            }
            double dLat = TransformLat(wgLon - 105.0, wgLat - 35.0);
            double dLon = TransformLon(wgLon - 105.0, wgLat - 35.0);
            double radLat = wgLat / 180.0 * PI;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * PI);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * PI);
            mgLat = wgLat + dLat;
            mgLon = wgLon + dLon;
        }

        static bool OutOfChina(double lat, double lon)
        {
            if (lon < 72.004 || lon > 137.8347)
                return true;
            if (lat < 0.8293 || lat > 55.8271)
                return true;
            return false;
        }

        static double TransformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * PI) + 40.0 * Math.Sin(y / 3.0 * PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * PI) + 320 * Math.Sin(y * PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        static double TransformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * PI) + 40.0 * Math.Sin(x / 3.0 * PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * PI) + 300.0 * Math.Sin(x / 30.0 * PI)) * 2.0 / 3.0;
            return ret;
        }

        #region 火星坐标转 (GCJ-02)地球坐标(WGS-84)
        /// <summary>
        /// 火星坐标转 (GCJ-02)地球坐标(WGS-84)
        /// </summary>
        /// <param name="gcjPoint">火星坐标转 (GCJ-02)</param>
        /// <returns>地球坐标(WGS-84)</returns>
        public static PointLatLng GCJ02ToWGS84(PointLatLng gcjPoint)
        {
            if (OutOfChina(gcjPoint.LatY, gcjPoint.LngX))
            {
                return gcjPoint;
            }
            double mglat, mglng;
            Transform(gcjPoint.LatY, gcjPoint.LngX, out mglat, out mglng);
            return new PointLatLng(mglat * 2 - gcjPoint.LatY, mglng * 2 - gcjPoint.LngX);
        }
        #endregion
        #region 地球坐标(WGS-84)转火星坐标 (GCJ-02)
        /// <summary>
        /// 地球坐标(WGS-84)转火星坐标 (GCJ-02)
        /// </summary>
        /// <param name="wgsPoint">地球坐标(WGS-84)</param>
        /// <returns>火星坐标 (GCJ-02)</returns>
        public static PointLatLng WGS84ToGCJ02(PointLatLng wgsPoint)
        {
            //if (OutOfChina(wgsPoint.Lat, wgsPoint.Lng))
            //{
            //    return wgsPoint;
            //}
            double mglat, mglng;
            Transform(wgsPoint.LatY, wgsPoint.LngX, out mglat, out mglng);

            return new PointLatLng(mglat, mglng);
        }
        #endregion
        #endregion

        /// <summary>
        /// 越界截取
        /// </summary>
        /// <param name="n"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }
        /// <summary>
        /// 获取指定层级的瓦片像素矩阵范围
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static SizeLong GetTileMatrixSizePixel(int zoom,SizeInt tileSize)
        {
            SizeLong s = GetTileMatrixSizeXY(zoom);
            return new SizeLong(s.Width * tileSize.Width, s.Height * tileSize.Height);
        }
        /// <summary>
        /// 获取指定层级下的瓦片索引矩阵范围
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static SizeLong GetTileMatrixSizeXY(int zoom)
        {
            SizeLong sMin = GetTileMatrixMinXY(zoom);
            SizeLong sMax = GetTileMatrixMaxXY(zoom);

            return new SizeLong(sMax.Width - sMin.Width + 1, sMax.Height - sMin.Height + 1);
        }

        /// <summary>
        /// 获取指定层级瓦片数量
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public long GetTileItemCount(int zoom)
        {
            BSize<long> s = GetTileMatrixSizeXY(zoom);
            return (s.Width * s.Height);
        }

        public static SizeLong GetTileMatrixMinXY(int zoom)
        {
            return new SizeLong(0, 0);
        }

        public static SizeLong GetTileMatrixMaxXY(int zoom)
        {
            long xy = (1 << zoom);
            return new SizeLong(xy - 1, xy - 1);
        }


       /// <summary>
       /// 根据像素坐标和瓦片尺寸获取瓦片索引
       /// </summary>
       /// <param name="p"></param>
       /// <param name="tileSize"></param>
       /// <returns></returns>
        public static PointInt64 PixelToTileXY(PointDouble p,SizeInt tileSize)
        {
            return new PointInt64((long)(p.X / tileSize.Width), (long)(p.Y / tileSize.Height));
        }

        /// <summary>
        /// 根据瓦片索引和瓦片尺寸获取像素坐标
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static PointInt64 TileXYToPixel(PointInt64 p, SizeInt tileSize)
        {
            return new PointInt64((p.X * tileSize.Width), (p.Y * tileSize.Height));
        }

        static readonly double R2D = 180 / Math.PI;
        static readonly double D2R = Math.PI / 180;

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double deg)
        {
            return (D2R * deg);
        }
        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double rad)
        {
            return (R2D * rad);
        }



        #region 向量法判断点是否在凸多边形内
        //向量叉积,判断P2是否在P1-P3向量右侧true，左侧false
        static public double cross(BPoint<double> p1, BPoint<double> p2, BPoint<double> p3)
        {
            return (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
        }
        //向量法判断点在凸多边形内
        static public bool Compl_inside_convex(BPoint<double> p, ref BPoint<double>[] con, int n)
        {
            if (n < 3) return false;
            if (cross(con[0], p, con[1]) >= 0) return false;
            if (cross(con[0], p, con[n - 1]) <= 0) return false;

            int i = 2, j = n - 1;
            int line = -1;

            while (i <= j)
            {
                int mid = (i + j) >> 1;
                if (cross(con[0], p, con[mid]) >= 0)
                {
                    line = mid;
                    j = mid - 1;
                }
                else i = mid + 1;
            }
            return cross(con[line - 1], p, con[line]) < 0;
        }
        #endregion

        #region 射线法判断点在任意多边形内 http://blog.csdn.net/yanjunmu/article/details/46723407
        /// <summary>
        /// 射线法判断点在任意多边形内
        /// </summary>
        /// <param name="p"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool IsPointInsidePolygon(BPoint<double> p, ref BPoint<double>[] polygon)
        {
            int cross_count = 0;

            for (int i = 0; i < polygon.Length; i++)
            {
                BPoint<double> p1 = polygon[i];
                BPoint<double> p2 = polygon[(i + 1) % polygon.Length];

                //线段水平
                if (p1.Y == p2.Y)
                    continue;

                //射线在线段外
                if (p.Y >= Math.Max(p1.Y, p2.Y) || p.Y <= Math.Min(p1.Y, p2.Y))
                    continue;

                double dy = (p2.X - p1.X) / (p2.Y - p1.Y);
                double x = (p.Y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X; ;

                if (x > p.X)
                    cross_count++;


            }

            return cross_count % 2 == 1;

        }
        #endregion

        /// <summary>
        /// 判断点和直线的位置关系
        /// </summary>
        /// <param name="LinePntA">直线上的一点</param>
        /// <param name="LinePntB">直线上的另一点</param>
        /// <param name="PntM">需要判断的点</param>
        /// <returns></returns>
        public  static int JudgePointToLine(PointInt LinePntA, PointInt LinePntB, PointInt PntM)
        {
            int nResult = 0;
            double ax = LinePntB.X - LinePntA.X;
            double ay = LinePntB.Y - LinePntA.Y;
            double bx = PntM.X - LinePntA.X;
            double by = PntM.Y - LinePntA.Y;
            double judge = ax * by - ay * bx;
            if (judge > 0)
            {
                nResult = 1;
            }
            else if (judge < 0)
            {
                nResult = -1;
            }
            else
            {
                nResult = 0;
            }
            return nResult;
        }

        /// <summary>
        /// 获取旋转后屏幕坐标点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static PointInt GetRotatedPoint(PointInt point,PointInt pCenter, float angle)
        {
            PointInt rp = new PointInt();

            if (angle != 0)
            {
                int centerX = pCenter.X / 2;//屏幕中心X
                int centerY = pCenter.Y/ 2;//屏幕中心Y
                int offsetCenterX = point.X - centerX;//距离屏幕中心X偏移值
                int offsetCenterY = point.Y - centerY;//距离屏幕中心Y偏移值
                switch (angle)
                {
                    case 90:
                    case -270:
                        rp.X = centerX - offsetCenterY;
                        rp.Y = centerY + offsetCenterX;
                        break;
                    case -90:
                    case 270:
                        rp.X = centerX + offsetCenterY;
                        rp.Y = centerY - offsetCenterX;
                        break;
                    case -180:
                    case 180:
                        rp.X = centerX - offsetCenterX;
                        rp.Y = centerY - offsetCenterY;
                        break;

                    default:
                        //屏幕坐标系旋转公式（顺时针）
                        //x1=(x-x0)*cos(angle)-(y-y0)*sin(angle) + x0
                        //y1=(x-x0)*sin(angle)+(y-y0)*cos(angle) + y0
                        double radian = DegreesToRadians(angle);//角度转弧度
                        rp.X = (int)(offsetCenterX * Math.Cos(radian) - offsetCenterY * Math.Sin(radian) + centerX);
                        rp.Y = (int)(offsetCenterX * Math.Sin(radian) + offsetCenterY * Math.Cos(radian) + centerY);
                        break;
                }


            }
            else
                return point;

            return rp;
        }

        /// <summary>
        /// 判断瓦片是否与矩形相交（旋转某个角度后的矩形）
        /// </summary>
        /// <param name="tileRect">当前瓦片所在矩形</param>
        /// <param name="areaRect">大矩形</param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static bool CheckTileCrossRect(RectInt tileRect,RectInt areaRect, float angle)
        {
            bool b = false;
            if (tileRect != null && areaRect != null)
            {
                var centerPoint = new PointInt((int)areaRect.Center.X, (int)areaRect.Center.Y);
                var tPoint1 = GetRotatedPoint(tileRect.LeftTop, centerPoint, angle);
                var tPoint2 = GetRotatedPoint(tileRect.LeftTop.GetOffSet(tileRect.Width, 0), centerPoint, angle);
                var tPoint3 = GetRotatedPoint(tileRect.LeftTop.GetOffSet(tileRect.Width, tileRect.Height), centerPoint, angle);
                var tPoint4 = GetRotatedPoint(tileRect.LeftTop.GetOffSet(0, tileRect.Height), centerPoint, angle);
                List<PointInt> lstTP = new List<PointInt>() { tPoint1, tPoint2, tPoint3, tPoint4 };




                foreach (var item in lstTP)
                {
                    if (areaRect.Contains(item))
                    {
                        b = true;
                        break;
                    }
                }
            }
            return b;
        }

        
    }
}
