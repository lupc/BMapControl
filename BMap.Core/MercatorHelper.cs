#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 14:20:26 
* 文件名：MercatorHelper 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using BMap.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace BMap.Core
{
    /// <summary>
    /// 墨卡托投影帮助类
    /// </summary>
    public class MercatorHelper
    {
        /// <summary>
        /// 最小纬度
        /// </summary>
        public  const double MinLatY = -85.05112878;
        /// <summary>
        /// 最大纬度
        /// </summary>
        public const double MaxLatY = 85.05112878;
        /// <summary>
        /// 最小经度
        /// </summary>
        public const double MinLngX = -180;
        /// <summary>
        /// 最大经度
        /// </summary>
        public const double MaxLngX = 180;
        /// <summary>
        /// 瓦片大小
        /// </summary>
        public static readonly SizeInt TileSize = new SizeInt(256, 256);
        /// <summary>
        /// 轴线
        /// </summary>
        public const double Axis = 6378137.0;
        /// <summary>
        /// 扁率
        /// </summary>
        public const double Flattening = (1.0 / 298.257223563);

        /// <summary>
        /// 单位为米，20037508.3427892表示地图周长的一半，以地图中心点做为（0，0）坐标。
        /// </summary>
        public const double L = 20037508.3427892;//单位为米，20037508.3427892表示地图周长的一半，以地图中心点做为（0，0）坐标。
        /// <summary>
        /// 坐标范围
        /// </summary>
        public static RectLatLng Bounds
        {
            get
            {
                return new RectLatLng(MinLngX, MaxLatY, MaxLngX-MinLngX, MaxLatY- MinLatY);
            }
        }
        /// <summary>
        /// 经纬度坐标转像素坐标
        /// </summary>
        /// <returns></returns>
        public static PointDouble LatLngToPixel(PointLatLng pLatLng,int zoom)
        {
            PointDouble pPix = new PointDouble();
            if (pLatLng!=null)
            {
                var lat = MapHelper.Clip(pLatLng.LatY, MinLatY, MaxLatY);
                var lng = MapHelper.Clip(pLatLng.LngX, MinLngX, MaxLngX);

                double x = (lng + 180) / 360;
                double sinLatitude = Math.Sin(lat * Math.PI / 180);
                double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

                BSize<long> s = MapHelper.GetTileMatrixSizePixel(zoom, TileSize);
                double mapSizeX = s.Width;
                double mapSizeY = s.Height;

                //pPix.X = MapHelper.Clip(x * mapSizeX + 0.5, 0, mapSizeX - 1);
                //pPix.Y = MapHelper.Clip(y * mapSizeY + 0.5, 0, mapSizeY - 1);
                pPix.X = MapHelper.Clip(x * mapSizeX, 0, mapSizeX - 1);
                pPix.Y = MapHelper.Clip(y * mapSizeY, 0, mapSizeY - 1);
            }
            
            return pPix;
        }

        /// <summary>
        /// 像素坐标转经纬度坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static PointLatLng PixelToLatLng(PointDouble pPix, int zoom)
        {
            PointLatLng ret = new PointLatLng();
            if (pPix!=null)
            {
                BSize<long> s = MapHelper.GetTileMatrixSizePixel(zoom, TileSize);
                double mapSizeX = s.Width;
                double mapSizeY = s.Height;

                double xx = (MapHelper.Clip(pPix.X, 0, mapSizeX - 1) / mapSizeX)-0.5 ;
                double yy = 0.5 - (MapHelper.Clip(pPix.Y, 0, mapSizeY - 1) / mapSizeY);

                ret.LatY = 90 - 360 * Math.Atan(Math.Exp(-yy * 2 * Math.PI)) / Math.PI;
                ret.LngX = 360 * xx;
            }
            

            return ret;
        }
        /// <summary>
        /// 经纬度坐标转大地坐标
        /// </summary>
        /// <param name="latlng"></param>
        /// <returns></returns>
        public static PointDouble LatLngToPointGeo(PointLatLng latlng)
        {
            PointDouble pd = new PointDouble(L * latlng.LngX / 180, Axis * Math.Log(Math.Tan((45.0 + latlng.LatY / 2) * (Math.PI / 180))));
            
            return pd;
        }
        /// <summary>
        /// 大地坐标转经纬度坐标
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static PointLatLng PointGetToLatLng(PointDouble p)
        {
            PointLatLng pll = new PointLatLng(p.X * 180 / L, (Math.Atan(Math.Pow(Math.E, p.Y / Axis)) * (180 / Math.PI) - 45) * 2);

            return pll;
        }
        /// <summary>
        /// 大地坐标转像素坐标
        /// </summary>
        /// <param name="pg">大地坐标</param>
        /// <param name="zoom">层级</param>
        /// <returns></returns>
        public static PointDouble PointGeoToPointPix(PointDouble pg,int zoom)
        {
            PointDouble pPix = new PointDouble();
            pPix.X = (L + pg.X) / GetResolution(zoom);
            pPix.Y = ((L - pg.Y) / GetResolution(zoom));
            return pPix;
        }
        /// <summary>
        /// 像素坐标转大地坐标
        /// </summary>
        /// <param name="pPix">像素坐标</param>
        /// <param name="zoom">层级</param>
        /// <returns></returns>
        public static PointDouble PointPixToPointGeo(PointDouble pPix, int zoom)
        {
            PointDouble pg = new PointDouble();

            return pg;
        }

        /// <summary>
        /// 根据像素坐标和瓦片尺寸获取瓦片索引
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static PointInt64 PixelToTileXY(PointDouble p)
        {
            return MapHelper.PixelToTileXY(p,TileSize);
        }

        /// <summary>
        /// 根据瓦片索引和瓦片尺寸获取像素坐标
        /// </summary>
        /// <param name="p"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static PointInt64 TileXYToPixel(PointInt64 p)
        {
            return MapHelper.TileXYToPixel(p,TileSize);
        }


        /// <summary>
        /// 根据层级获取矩形区域内的所有瓦片索引
        /// </summary>
        public static List<PointInt64> GetAreaTileList(RectLatLng rect, int zoom, int padding = 0)
        {
            List<PointInt64> ret = new List<PointInt64>();

            PointInt64 topLeft = PixelToTileXY(LatLngToPixel(rect.LeftTop, zoom));
            PointInt64 rightBottom = PixelToTileXY(LatLngToPixel(rect.RightBottom, zoom));

            for (long x = (topLeft.X - padding); x <= (rightBottom.X + padding); x++)
            {
                for (long y = (topLeft.Y - padding); y <= (rightBottom.Y + padding); y++)
                {
                    PointInt64 p = new PointInt64(x, y);
                    if (!ret.Contains(p) && p.X >= 0 && p.Y >= 0)
                    {
                        ret.Add(p);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 获取像素分辨率（每像素代表多少米）
        /// </summary>
        /// <param name="zoom">层级</param>
        /// <param name="latitude">纬度</param>
        /// <returns></returns>
        public static double GetResolution(int zoom, double latitude)
        {
            return (Math.Cos(latitude * (Math.PI / 180)) * 2 * Math.PI * Axis) / MapHelper.GetTileMatrixSizePixel(zoom,TileSize).Width;
        }
        public static double GetResolution(int zoom)
        {
            return L * 2 / TileSize.Width / Math.Pow(2, zoom);
        }
        /// <summary>
        /// 获取两点距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetDistance(PointLatLng p1, PointLatLng p2)
        {
            double dLat1InRad = p1.LatY * (Math.PI / 180);
            double dLong1InRad = p1.LngX * (Math.PI / 180);
            double dLat2InRad = p2.LatY * (Math.PI / 180);
            double dLong2InRad = p2.LngX * (Math.PI / 180);
            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;
            double a = Math.Pow(Math.Sin(dLatitude / 2), 2) + Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) * Math.Pow(Math.Sin(dLongitude / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double dDistance = Axis  * c;
            return dDistance;
        }
        /// <summary>
        /// 获取两坐标点连线与正北的夹角
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double GetBearing(PointLatLng p1, PointLatLng p2)
        {
            var latitude1 = MapHelper.DegreesToRadians(p1.LatY);
            var latitude2 = MapHelper.DegreesToRadians(p2.LatY);
            var longitudeDifference = MapHelper.DegreesToRadians(p2.LngX - p1.LngX);

            var y = Math.Sin(longitudeDifference) * Math.Cos(latitude2);
            var x = Math.Cos(latitude1) * Math.Sin(latitude2) - Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longitudeDifference);

            return (MapHelper.RadiansToDegrees(Math.Atan2(y, x)) + 360) % 360;
        }
    }
}
