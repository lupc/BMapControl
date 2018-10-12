#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/14 9:09:56 
* 文件名：Map 
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
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;

namespace BMap.Core
{
    public class Map
    {
        public Map()
        {
            TileSize = MercatorHelper.TileSize;
        }

        //private ReaderWriterLock 

        #region 属性

        public ReaderWriterLock TilesUpdateLock { get; set; } = new ReaderWriterLock();
        public int MinZoom { get; set; } = 1;
        public int MaxZomm { get; set; } = 20;

        private int _zoom=12;
        /// <summary>
        /// 层级
        /// </summary>
        public int Zoom
        {
            get { return _zoom; }
            set
            {
                if (_zoom != value)
                {
                    var temp = Math.Min(MaxZomm, Math.Max(MinZoom, value));
                    if (temp != _zoom)
                    {
                        _zoom = temp;
                        UpdateDrawTiles();
                    }
                }

            }
        }

        private PointLatLng _mapCenter = new Core.Model.PointLatLng(113.1, 23.1);
        /// <summary>
        /// 地图中心经纬度
        /// </summary>
        public PointLatLng MapCenter
        {
            get { return _mapCenter; }
            set
            {
                if (_mapCenter != value)
                {
                    _mapCenter = value;
                    _isNeedToUpdateTiles = true;

                   

                    //重新计算像素中心点
                    UpdateDrawTiles();
                }
            }
        }
        
        /// <summary>
        /// 获取地图中心像素坐标
        /// </summary>
        public PointDouble MapCenterPix
        {
            get { return MercatorHelper.LatLngToPixel(_mapCenter, _zoom); }
           
        }

        /// <summary>
        /// 屏幕中心瓦片像素坐标
        /// </summary>
        public PointDouble TileCenterPix
        {
            get
            {
                PointDouble pd = new PointDouble();
                switch (TileType)
                {
                    case TileType.WGS84:
                        pd = MapCenterPix;
                        break;
                    case TileType.GCJ02:
                        var gcjPoint = MapHelper.WGS84ToGCJ02(_mapCenter);
                        pd = MercatorHelper.LatLngToPixel(gcjPoint, _zoom);
                        break;
                    default:
                        break;
                }
                return pd;
            }
        }

        /// <summary>
        /// 瓦片区域
        /// </summary>
        public RectInt TilesArea { get; set; }
        /// <summary>
        /// 贴图瓦片集合
        /// </summary>
        public Dictionary<Tile,DrawTile> LstDrawTile { get; set; }
        /// <summary>
        /// 网格绘制信息
        /// </summary>
        public DrawGrid GridInfo { get; set; }

        /// <summary>
        /// 屏幕显示区域
        /// </summary>
        public RectInt ScreenArea { get; set; }

        public SizeInt TileSize { get; set; }
        /// <summary>
        /// 地图编辑类型
        /// </summary>
        public EditMode EditType { get; set; }
        /// <summary>
        /// 地图是否拖动中
        /// </summary>
        public bool IsDragging { get; set; }
        /// <summary>
        /// 瓦片是否更新中
        /// </summary>
        public bool IsTilesUpdating { get; private set; }
        
        private float _rotationAngle;
        /// <summary>
        /// 旋转角度
        /// </summary>
        public float RotationAngle
        {
            get { return _rotationAngle; }
            set {
                if (_rotationAngle!=value)
                {
                    _rotationAngle = value % 360;
                    

                    switch (_rotationAngle)
                    {
                        case 0:
                            _isNeedToUpdateTiles = true;
                            break;
                        default:
                            break;
                    }

                    if (_isNeedToUpdateTiles)
                    {
                        GetTilesArea();
                        UpdateDrawTiles();
                    }
                }
            }
        }

        /// <summary>
        /// 瓦片类型
        /// </summary>
        public TileType TileType { get; set; }



        public event Action<Dictionary<Tile,DrawTile>> TilesUpdateComplete;
        
        #endregion

        /// <summary>
        /// 地图中心偏移
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void CenterOffSet(double x, double y, bool isUpdateTiles = true)
        {
            var centerPix = MapCenterPix;
            centerPix = MapCenterPix.GetOffSet(x, y);
            _mapCenter = MercatorHelper.PixelToLatLng(centerPix, Zoom);
            _isNeedToUpdateTiles = true;
            //Debug.WriteLine(string.Format("****缩放：鼠标点:【{0}】,中心坐标点:【{1}】,中心像素点:【{2}】,鼠标像素点【{3}】，鼠标经纬度【{8}】，层级:【{4}】,屏幕中心点：【{5}】\r\n像素偏移{6},屏幕坐标点偏移{7}"));


            if (isUpdateTiles)
            {
                UpdateDrawTiles();
            }
        }
        /// <summary>
        /// 按屏幕上某点缩放地图
        /// </summary>
        /// <param name="mp">屏幕上某点</param>
        /// <param name="n">缩放量（正数表示Zoom增加1，负数表示Zoom减少1）</param>
        public void PointZoom(PointInt mp,int n)
        {
            if (mp!=null)
            {
                
                if (n > 0)
                {
                    if (Zoom<MaxZomm)
                    {
                        CenterOffSet((mp.X - ScreenArea.Width / 2) / 2, (mp.Y - ScreenArea.Height / 2) / 2, false);
                        //var mpLatLng = PointToLatLng(mp);
                        this.Zoom++;
                        //GetCenterOfterZoom(mp, mpLatLng);
                    }
                    
                }
                else if (n < 0)
                {
                    if (Zoom>MinZoom)
                    {
                        CenterOffSet(-(mp.X - ScreenArea.Width / 2), -(mp.Y - ScreenArea.Height / 2), false);
                        //var mpLatLng = PointToLatLng(mp);
                        this.Zoom--;
                        //GetCenterOfterZoom(mp, mpLatLng);
                    }
                   
                }

                
            }
           
        }
        /// <summary>
        /// 获取层级变更后的地图中心点
        /// </summary>
        /// <param name="mp">鼠标缩放所在屏幕坐标</param>
        /// <param name="mpLatLng">鼠标缩放前所在经纬度坐标</param>
        //private void GetCenterOfterZoom(PointInt mp,PointLatLng mpLatLng)
        //{
        //    var centerOffSet = new SizeDouble((double)(mp.X - ScreenArea.Center.X), (double)(mp.Y - ScreenArea.Center.Y));
           
        //    var mpPix = MercatorHelper.LatLngToPixel(mpLatLng, _zoom);
        //    var cPix = mpPix.GetOffSet(-centerOffSet.Width, -centerOffSet.Height);
        //    _mapCenter = MercatorHelper.PixelToLatLng(cPix, _zoom);
        //    //Debug.WriteLine(string.Format("****缩放：鼠标点:【{0}】,中心坐标点:【{1}】,中心像素点:【{2}】,鼠标像素点【{3}】，鼠标经纬度【{8}】，层级:【{4}】,屏幕中心点：【{5}】\r\n像素偏移{6},屏幕坐标点偏移{7}"
        //    //    , mp, MapCenter, MapCenterPix, mpPix,Zoom,ScreenArea.Center,mpPix.GetOffSet(-MapCenterPix.X,-MapCenterPix.Y),centerOffSet, mpLatLng));
        //    UpdateDrawTiles();
        //}


        /// <summary>
        /// 经纬度转屏幕坐标
        /// </summary>
        /// <param name="latlng"></param>
        /// <returns></returns>
        public PointInt LatLngToPoint(PointLatLng latlng)
        {
            var pPix = MercatorHelper.LatLngToPixel(latlng, Zoom).GetOffSet(-MapCenterPix.X, -MapCenterPix.Y).GetOffSet(ScreenArea.Width / 2, ScreenArea.Height / 2);
            return new PointInt((int)pPix.X, (int)pPix.Y);
        }
        /// <summary>
        /// 屏幕坐标转经纬度
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public PointLatLng PointToLatLng(PointInt p)
        {
            PointLatLng pll = null;
            var pPix = MapCenterPix.GetOffSet(p.X-ScreenArea.Width/2, p.Y-ScreenArea.Height/2);
            pll = MercatorHelper.PixelToLatLng(pPix, Zoom);
            return pll;
        }

        ///// <summary>
        ///// //重新计算像素中心点
        ///// </summary>
        //public void ReSetMapCenterPix()
        //{

        //    UpdateDrawTiles();
        //}

        //bool _isDrawTilesUpdating = false;
        /// <summary>
        /// 重新设定地图控件大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ReSetMapSize(int width,int height)
        {
            _isNeedToUpdateTiles = true;
            ScreenArea = new RectInt(0, 0, width, height);
            GetTilesArea();
            UpdateDrawTiles();
        }

        public DrawTile GetDrawTile(Tile t)
        {
            DrawTile dt = null;
            if (t!=null)
            {
                TilesUpdateLock.AcquireReaderLock(2000);
                try
                {
                    
                    LstDrawTile.TryGetValue(t,out dt);
                }
                finally
                {
                    TilesUpdateLock.ReleaseReaderLock();
                }
            }
            return dt;
        }

        public void UpdateDrawTiles()
        {
            
            IsTilesUpdating = true;
            TilesUpdateLock.AcquireWriterLock(1000);
            try
            {
                
                if (ScreenArea != null)
                {
                    
                    //计算要显示的绘图瓦片
                    if (LstDrawTile == null)
                    {
                        LstDrawTile = new Dictionary<Tile, DrawTile>();

                    }
                    LstDrawTile.Clear();

                    var width = TilesArea.Width;
                    var height = TilesArea.Height;

                    

                    var pointLeftTopPix = TileCenterPix.GetOffSet(-width / 2, -height / 2);//左上角像素坐标
                    var tileXYLeftTop = MercatorHelper.PixelToTileXY(pointLeftTopPix);//左上角的瓦片索引
                    var tilePixLeftTop = MercatorHelper.TileXYToPixel(tileXYLeftTop);//左上角瓦片像素坐标
                    var tileDrawPositionLeftTop = new PointInt((int)(tilePixLeftTop.X - pointLeftTopPix.X + 0.5), (int)(tilePixLeftTop.Y - pointLeftTopPix.Y + 0.5));//屏幕坐标系下左上角瓦片的屏幕坐标
                    //var tileDrawPositionLeftTop = TilesArea.LeftTop.GetOffSet((int)(tilePixLeftTop.X - pointLeftTopPix.X), (int)(tilePixLeftTop.Y - pointLeftTopPix.Y));

                    //var tileXYCenter = MercatorHelper.PixelToTileXY(MapCenterPix);//中心瓦片索引
                    //var tilePixCenter = MercatorHelper.TileXYToPixel(tileXYCenter);//中心瓦片左上角所在像素坐标

                    var xCount = (int)Math.Ceiling((double)width / TileSize.Width)+1;//X方向的瓦片数量
                    var yCount = (int)Math.Ceiling((double)height / TileSize.Height)+1;//Y方向的瓦片数量

                    GridInfo = new DrawGrid
                    {
                        GridSize = TileSize,
                        LeftTop = tileDrawPositionLeftTop,
                        XCount = xCount ,
                        YCount = yCount ,
                    };
                    var minTileIndex = MapHelper.GetTileMatrixMinXY(Zoom);
                    var maxTileIndex = MapHelper.GetTileMatrixMaxXY(Zoom);
                    List<Tile> lstWantToLoad = new List<Tile>();
                    for (int x = 0; x < xCount; x++)
                    {
                        for (int y = 0; y < yCount; y++)
                        {
                            var drawPostiton = tileDrawPositionLeftTop.GetOffSet(x * TileSize.Width, y * TileSize.Height);
                            //Tile t = new Tile(tileXYLeftTop.X + x, tileXYLeftTop.Y + y, Zoom);
                            Tile t = TileManager.Singleton.GetTile(Zoom, new PointInt64(tileXYLeftTop.X + x, tileXYLeftTop.Y+y),false);
                            if (t.TileIndex.X>=minTileIndex.Width&&t.TileIndex.Y>=minTileIndex.Height 
                                && t.TileIndex.X<=maxTileIndex.Width&&t.TileIndex.Y<=maxTileIndex.Height)//范围判断
                            {
                                var dt = new DrawTile()
                                {
                                    DrawPosition = drawPostiton,
                                    Tile = t
                                };

                                //if (RotationAngle == 0 || MapHelper.CheckTileCrossRect(new RectInt(dt.DrawPosition.GetOffSet(TilesArea.LeftTop.X, TilesArea.LeftTop.Y), TileSize.Width, TileSize.Height), ScreenArea, RotationAngle))
                                {
                                    LstDrawTile.Add(t, dt);
                                    if (t.TileImg == null)
                                    {
                                        lstWantToLoad.Add(t);

                                    }
                                }
                            }
                        }
                    }
                   
                    //TilesArea = new RectInt((int)tileDrawPositionLeftTop.X,(int)tileDrawPositionLeftTop.Y, xCount * TileSize.Width, yCount * TileSize.Height);
                    if (TilesUpdateComplete!=null)
                    {
                        TilesUpdateComplete(LstDrawTile);
                    }

                    lstWantToLoad.ForEach(a => { TileManager.Singleton.LoadTile(a); });
                    
                }
            }
            finally
            {
                IsTilesUpdating = false;
                TilesUpdateLock.ReleaseWriterLock();
            }


            
           
            
            
        }

        private bool _isNeedToUpdateTiles = false;
        /// <summary>
        /// 根据旋转角度和屏幕大小计算贴图区域
        /// </summary>
        private void GetTilesArea()
        {
            //计算贴图区域
            switch (RotationAngle)
            {
                case 90:
                case 270:
                    TilesArea = new RectInt(ScreenArea.Width / 2 - ScreenArea.Height / 2, ScreenArea.Height / 2 - ScreenArea.Width / 2, ScreenArea.Height, ScreenArea.Width);
                    
                    break;
                case 0:
                case 360:
                    TilesArea = new RectInt(0, 0, ScreenArea.Width, ScreenArea.Height);
                    
                    break;
                default:
                    //int aw = (int)Math.Ceiling(ScreenArea.Width / Math.Cos(MapHelper.DegreesToRadians(RotationAngle)));
                    //int aw = (int)Math.Ceiling(ScreenArea.Width / Math.Cos(MapHelper.DegreesToRadians(RotationAngle)));
                    int r = (int)Math.Sqrt(ScreenArea.Width * ScreenArea.Width + ScreenArea.Height * ScreenArea.Height);//对角线长度
                    TilesArea = new RectInt(ScreenArea.Width / 2 - r / 2, ScreenArea.Height / 2 - r / 2, r, r);
                    _isNeedToUpdateTiles = false;
                    break;
            }
            //TilesArea = new RectInt((int)tileDrawPositionLeftTop.X, (int)tileDrawPositionLeftTop.Y, xCount * TileSize.Width, yCount * TileSize.Height);
        }


    }
}
