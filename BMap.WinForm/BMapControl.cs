using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BMap.Core;
using BMap.Core.Model;
using System.Threading;
using System.Collections.Concurrent;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace BMap.WinForm
{
    [ToolboxItem(true)]
    public partial class BMapControl : UserControl
    {
        public BMapControl()
        {
            ThreadPool.QueueUserWorkItem(RefreshByThread);
            TileManager.Singleton.RefreshTile += Singleton_TileAdded;
            map.TilesUpdateComplete += Map_TilesUpdateComplete;
            
            InitializeComponent();
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //this.SetStyle(ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            //this.SetStyle(ControlStyles.Opaque, true);
            //ResizeRedraw = true;
           

            //this.bMapScale1.BackColor = Color.Transparent;
            //this.bMapScale1.ForeColor = Color.FromArgb(0xFF, 0xff, 0xff, 0xff);
           
        }
        
        private Map map = new Map();
        /// <summary>
        /// 图层集合
        /// </summary>
        private List<MapLayer> _lstLayler = new List<MapLayer>();
        private object _bitmapLocker = new object();
        private DateTime _lastRefreshTime = DateTime.Now;
        #region 属性

        public float RotationAngle
        {
            get
            {
                return map.RotationAngle;
            }
            set
            {
                if (map.RotationAngle != value)
                {
                    map.RotationAngle = value;
                    Refresh();
                }
            }
        }
        /// <summary>
        /// 获取设置地图中心经纬度坐标
        /// </summary>
        public PointLatLng MapCenter
        {
            get
            {
                return map.MapCenter;
            }
            set
            {
                if (map.MapCenter!=value)
                {
                    map.MapCenter = value;
                    Refresh();
                }
                
                
            }
        }
        public int Zoom
        {
            get
            {
                return map.Zoom;
            }
            set
            {
                if (map.Zoom != value)
                {
                    map.Zoom = value;
                    //bMapScale1.Distance = MercatorHelper.GetResolution(value, MapCenter.LatY) * bMapScale1.ScaleLen;
                    Refresh();
                }
            }
        }
      
        /// <summary>
        /// 获取地图中心像素坐标
        /// </summary>
        public BPoint<double> MapCenterPix
        {
            get { return map.MapCenterPix; }
        }
        /// <summary>
        /// 获取鼠标所在经纬度
        /// </summary>
        public PointLatLng MouseLatLng
        {
            get
            {
                return PointToLatLng(_curMousePoint);
            }
        }
        /// <summary>
        /// 网格线颜色
        /// </summary>
        public Color GridLineColor { get; set; } = Color.FromArgb(0x44, 0x11, 0x11, 0xff);
        /// <summary>
        /// 网格线宽度
        /// </summary>
        public float GridLineWidth { get; set; } = 0.5f;
        /// <summary>
        /// 是否正在拖动
        /// </summary>
        public bool IsDragging { get { return map.IsDragging; } }

        /// <summary>
        /// 是否显示网格
        /// </summary>
        public bool IsShowGrid { get; set; } = true;


        /// <summary>
        /// 是否显示渲染信息
        /// </summary>
        public bool IsShowRenderMsg
        {
            get;
            set;
        }
        /// <summary>
        /// 是否显示地图信息
        /// </summary>
        public bool IsShowMapMsg {
            get;
            set;
        }
        /// <summary>
        /// 是否显示地图比例尺
        /// </summary>
        public bool IsShowMapScale {
            get;
            set;
        }
        /// <summary>
        /// 用户操作模式
        /// </summary>
        //public EditMode UserEditMode { get; set; }

        /// <summary>
        /// 用户画图
        /// </summary>
        public UserDrawing UserDraw { get; set; }

        //public EditData UserEdit { get; set; } = new EditData();
        //public Pen EditPen { get; set; } = new Pen(Brushes.White, 2);

            /// <summary>
            /// 瓦片类型
            /// </summary>
        public TileType TileType
        {
            get { return map.TileType; }
            set { map.TileType = value; }
        }
        #endregion



        #region 事件

        /// <summary>
        /// 地图中心变更事件
        /// </summary>        
        public event Action<BMapControl, PointLatLng> MapCenterChanged;
        /// <summary>
        /// 层级改变事件
        /// </summary>
        public event Action<BMapControl, int> ZoomChanged;

        #endregion

        #region 方法




        /// <summary>
        /// 添加图层 线程安全
        /// </summary>
        /// <param name="marker"></param>
        public void AddLayer(MapLayer lay)
        {
            lay.MapObject = this;
            lock (_lstLayler)
            {
                _lstLayler.Add(lay);
            }
        }
        /// <summary>
        /// 移除图层 线程安全
        /// </summary>
        /// <param name="marker"></param>
        public void RemoveLayer(MapLayer lay)
        {
            lock (_lstLayler)
            {
                _lstLayler.Remove(lay);
            }
        }
        /// <summary>
        /// 清空所有标记 线程安全
        /// </summary>
        public void ClearLayer()
        {
            lock (_lstLayler)
            {
                _lstLayler.Clear();
            }
        }
        /// <summary>
        /// 获取图层 线程安全
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MapLayer GetLayer(string name)
        {
            lock (_lstLayler)
            {
               return _lstLayler.FirstOrDefault(t => t.Name == name);
            }
        }


        /// <summary>
        /// 获取旋转后屏幕坐标点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point GetRotatedPoint(Point point)
        {
            return GetRotatedPoint(point, RotationAngle);
        }
        /// <summary>
        /// 获取旋转后屏幕坐标点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Point GetRotatedPoint(Point point, float angle)
        {
            Point rp = new Point();

            if (angle != 0)
            {
                int centerX = Size.Width / 2;//屏幕中心X
                int centerY = Size.Height / 2;//屏幕中心Y
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
                        double radian = AngleToRadian(angle);//角度转弧度
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
        /// 经纬度转屏幕坐标(里面考虑了旋转情况)
        /// </summary>
        /// <param name="latlng"></param>
        /// <returns></returns>
        public Point LatLngToPoint(PointLatLng latlng)
        {
            return GetRotatedPoint( map.LatLngToPoint(latlng).ToPoint());
        }
        /// <summary>
        /// 屏幕坐标转经纬度(里面考虑了旋转情况)
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public PointLatLng PointToLatLng(Point p)
        {
            var np = GetRotatedPoint(p, -RotationAngle);
            return map.PointToLatLng(new PointInt(np.X, np.Y));
        }
        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private double AngleToRadian(float angle)
        {
            return angle / 180 * Math.PI;
        }




        #endregion









        #region 画图方法
        private bool _isNeedRefresh = false;
        private void RefreshByThread(object o)
        {
            while (true)
            {
                try
                {

                    if (_isNeedRefresh)
                    {
                        if (this.IsHandleCreated)
                        {
                            this.Invoke(new Action(() =>
                            {
                                //DrawTilesToBitmap();
                                Refresh();

                                //_lastRefreshMousePoint = _lastMousePoint;
                            }));
                        }

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("定时更新瓦片出错！"+ex.ToString());
                }
                finally
                {
                    Thread.Sleep(200);//每隔100ms更新一次瓦片
                }
            }
        }
        private void Map_TilesUpdateComplete(Dictionary<Tile,DrawTile> obj)
        {
            new Action(DrawTilesToBitmap).BeginInvoke(null, null);
            //DrawTilesToBitmap();
        }

        //private Queue<DrawTile> queTileToDeaw = new Queue<DrawTile>();
        private void Singleton_TileAdded(Tile obj)
        {
            if (obj != null)
            {
                var t = map.GetDrawTile(obj);
                if (t != null)
                {
                    if (this.IsHandleCreated)
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (_allTilesBitmap != null)
                            {
                                using (var g = Graphics.FromImage(_allTilesBitmap))
                                {
                                    DrawMapTile(t, g, Point.Empty);
                                    _isNeedRefresh = true;
                                }
                            }
                        }));
                    }
                }
            }
          
        }


        RectInt _lastTilesArea = null;
        private void DrawTilesToBitmap()
        {
            lock (_bitmapLocker)
            {
                swCreateBitmap.Restart();

                //if (_allTilesBitmap==null)
                //{

                //}
                //if (map.TilesArea != _lastTilesArea)
                //{
                //    if (map.RectTilesBitmap != null)
                //    {
                //        _allTilesBitmap = new Bitmap(map.RectTilesBitmap.Width, map.RectTilesBitmap.Height);
                //    }
                //    _lastTilesArea = map.TilesArea;
                //}
                if ( map.RectTilesBitmap != null)
                {
                    _allTilesBitmap = new Bitmap(map.RectTilesBitmap.Width, map.RectTilesBitmap.Height);
                }
                if (_allTilesBitmap!=null)
                {
                    using (var g = Graphics.FromImage(_allTilesBitmap))
                        {
                            g.SmoothingMode = SmoothingMode.HighSpeed;
                            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                            if (IsShowGrid)
                            {
                                DrawMapTiles(g);
                                DrawMapGrid(g);
                            }
                            else
                            {
                                DrawMapGrid(g);
                                DrawMapTiles(g);
                            }

                        }

                }
                swCreateBitmap.Stop();

            }


        }

      
        

        /// <summary>
        /// 描绘网格
        /// </summary>
        /// <param name="g"></param>
        private void DrawMapGrid(Graphics g)
        {
            map.TilesUpdateLock.AcquireReaderLock(200);
            if (map.GridInfo != null)
            {
                var pen = new Pen(GridLineColor, GridLineWidth);
                {
                    //var rightBottom = new Point(map.GridInfo.XCount * map.GridInfo.GridSize.Width, map.GridInfo.YCount * map.GridInfo.GridSize.Height);
                    //for (int x = (int)map.GridInfo.LeftTop.X; x <= rightBottom.X; x += map.GridInfo.GridSize.Width)
                    //{
                    //    g.DrawLine(pen, new Point(x, 0), new Point(x, rightBottom.Y));
                    //}
                    //for (int y = (int)map.GridInfo.LeftTop.Y; y <= rightBottom.Y; y += map.GridInfo.GridSize.Height)
                    //{
                    //    g.DrawLine(pen, new Point(0, y), new Point(rightBottom.X, y));
                    //}

                    //画竖线
                    for (int x = 0; x <= _allTilesBitmap.Width; x+=map.TileSize.Width)
                    {
                        g.DrawLine(pen, new Point(x, 0), new Point(x, _allTilesBitmap.Height));
                    }
                    //画横线
                    for (int y = 0; y <= _allTilesBitmap.Height; y+=map.TileSize.Height)
                    {
                        g.DrawLine(pen, new Point(0, y), new Point(_allTilesBitmap.Width, y));
                    }

                    //g.DrawLine(pen, new Point(5, 5), new Point(50, 50));
                }
            }
            map.TilesUpdateLock.ReleaseReaderLock();
        }

        private Stopwatch swCreateBitmap = new Stopwatch();
        /// <summary>
        /// 绘制所有图层瓦片
        /// </summary>
        private void DrawMapTiles(Graphics g)
        {
            map.TilesUpdateLock.AcquireReaderLock(200);
            
            if (map.DicDrawTile != null)
            {
                //using (g)
                {
                    foreach (var rt in map.DicDrawTile.Values)
                    {
                        
                            DrawMapTile(rt, g, Point.Empty);
                    }
                }
            }
            
            map.TilesUpdateLock.ReleaseReaderLock();
        }
        /// <summary>
        /// 绘制单块瓦片
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="g"></param>
        private void DrawMapTile(DrawTile rt,Graphics g,Point offset)
        {

            if (rt !=null )
            {
                //var p = rt.DrawPosition.GetOffSet(0 - map.TilesArea.LeftTop.X, 0 - map.TilesArea.LeftTop.Y).ToPoint();
                var p = rt.DrawPosition.ToPoint();//绘图位置
                if (!offset.IsEmpty)
                {
                    p.Offset(offset);
                }

                if (rt.Tile != null && !rt.Tile.IsEmpty)
                {
                    rt.Tile.DrawTime = DateTime.Now;
                    //rt.Tile.DrawCount++;
                    if (rt.Tile.TileImg!=null)
                    {
                        g.DrawImage(rt.Tile.TileImg, p);
                        
                    }
                    else
                    {
                        //没图片
                        using (var f = new Font("宋体", 11))
                        {
                            p.X += 5;
                            p.Y += 5;
                            g.DrawString(rt.Tile.ToString(), f, new SolidBrush(GridLineColor), p);

                            //if (rt.Tile.IsLoading)
                            //{
                            //    p.Offset(0, 30);
                            //    g.DrawString("Loading...", f, new SolidBrush(GridLineColor), p);
                            //}
                            //else if (!string.IsNullOrWhiteSpace(rt.Tile.ErrorMsg))
                            //{
                            //    p.Offset(0, 30);
                            //    g.DrawString(rt.Tile.ErrorMsg, f, new SolidBrush(Color.FromArgb(0x88, 0xff, 0, 0)), p);
                            //}
                        }
                    }
                    
                }
               
            }

        }

        /// <summary>
        /// 画图层
        /// </summary>
        /// <param name="g"></param>
        private void DrawLaylers(Graphics g)
        {

            if (_lstLayler != null && _lstLayler.Count > 0)
            {
                lock (_lstLayler)
                {
                    foreach (var item in _lstLayler)
                    {
                        item.Draw(this,g);
                    }
                }
            }
        }

        

      

        private Stopwatch _swRenser = new Stopwatch();
        private SolidBrush msgBrush = new SolidBrush(Color.FromArgb(0xdd, 0xff, 0xff, 0xff));
        private SolidBrush msgBackBrush = new  SolidBrush(Color.FromArgb(0x33, 0x00, 0x00, 0x00));
        private Font msgFont = new Font("微软雅黑", 9);
        private void DrawRenderMsg(Graphics g)
        {
            string msg = string.Format("瓦片贴图耗时:{0}ms,渲染耗时:{1}ms", swCreateBitmap.ElapsedMilliseconds, _swRenser.ElapsedMilliseconds);
            var size = g.MeasureString(msg, msgFont);
            Point msgPoint = new Point(this.Width - (int)size.Width - 10, 5);
            DrawMsg(g, msgPoint, msg,msgFont,size);
            //swCreateBitmap.Reset();
            //_swRenser.Reset();
        }

        private void DrawMapMsg(Graphics g)
        {
            string msg = string.Format("层级:{0},地图中心点:{1}", Zoom, MapCenter);
            var size = g.MeasureString(msg, msgFont);
            DrawMsg(g, new Point(5, 5), msg,msgFont,size);
        }

        private void DrawMsg(Graphics g,Point location,string msg,Font font,SizeF msgSize)
        {
            g.ResetTransform();
            //g.DrawString(string.Format("层级:{0},地图中心点:{1},鼠标点:{2}", Zoom ,MapCenter,MouseLatLng), Font, Brushes.White, new Point(5, 5));
            
            var size = msgSize;
            var sizeInt = new Size((int)size.Width + 10, (int)size.Height + 2);
            Point msgPoint = location;
            var rect = new Rectangle(msgPoint, sizeInt);

            using (var gp = DrawHelper.GetRoundRect(rect, sizeInt.Height / 2))
            {
                g.FillPath(msgBackBrush, gp);
            }

            //g.FillRectangle(msgBackBrush, new RectangleF(msgPoint, size));
            g.DrawString(msg, font, msgBrush, msgPoint.GetOffSet(5, 1));
        }


        private void DrawMapScale(Graphics g)
        {
            g.ResetTransform();
            int scaleLen = 50;
            int meter = 0;
            switch (Zoom)
            {
                case 22:
                    meter = 1;
                    break;
                case 21:
                case 20:
                    meter = 5;
                    break;
                case 19:
                    meter = 10;
                    break;
                case 18:
                    meter = 20;
                    break;
                case 17:
                    meter = 40;
                    break;
                case 16:
                    meter = 80;
                    break;
                case 15:
                    meter = 160;
                    break;
                case 14:
                    meter = 300;
                    break;
                case 13:
                    meter = 600;
                    break;
                case 12:
                    meter = 1000;
                    break;
                case 11:
                    meter = 2000;
                    break;
                case 10:
                    meter = 5000;
                    break;
                case 9:
                    meter = 10000;
                    break;
                case 8:
                    meter = 20000;
                    break;
                case 7:
                    meter = 40000;
                    break;
                case 6:
                    meter = 80000;
                    break;
                case 5:
                    meter = 150000;
                    break;
                case 4:
                    meter = 300000;
                    break;
                case 3:
                    meter = 600000;
                    break;
                case 2:
                    meter = 1000000;
                    break;
                case 1:
                    meter = 2000000;
                    break;
                default:
                    scaleLen = 50;
                    break;
            }
            if (meter!=0)
            {
                scaleLen = (int)(meter / MercatorHelper.GetResolution(Zoom, MapCenter.LatY) + 0.5);
            }
            
            var dist = new Distance(meter);
            var str = dist.ToString();
            //var str = distance.ToString("0.00") + "米";
            var font = new Font("微软雅黑", 10, FontStyle.Bold);
            var strSize = g.MeasureString(str, font);
            var rect = new Rectangle(new Point(5, this.Height - 30), new Size(scaleLen + 10 + (int)strSize.Width, (int)strSize.Height + 5));
            using (var gp = DrawHelper.GetRoundRect(rect, 5))
            {
                g.FillPath(new SolidBrush(Color.FromArgb(0x44, Color.Black)), gp);
            }

            Point p = new Point(10, this.Height - 20);
            Point[] ps = new Point[4];
            ps[0] = p;
            p.Offset(0, 5);
            ps[1] = p;
            p.Offset(scaleLen, 0);
            ps[2] = p;
            p.Offset(0, -5);
            ps[3] = p;
            var brush = new SolidBrush(Color.FromArgb(0xEE, Color.White));
            var pen = new Pen(brush, 2);
            g.DrawLines(pen, ps);
            p.Offset(5, -10);
            g.DrawString(str,font,brush,p);
        }

        /// <summary>
        /// 屏幕可见区域瓦片大图
        /// </summary>
        private Bitmap _allTilesBitmap ;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _swRenser.Restart();
            if (_allTilesBitmap != null)
            {
                //Bitmap bitmap = null;
                //lock (_bitmapLocker)
                //{
                //    bitmap = _allTilesBitmap.Clone() as Bitmap;
                //}


                //using (var gb = Graphics.FromImage(_allTilesBitmap))
                //{


                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clip = new Region(this.ClientRectangle);
                g.CompositingQuality = CompositingQuality.HighQuality;

                //整个地图控件GDI旋转
                if (RotationAngle != 0)
                {
                    float transX = this.Width / 2;
                    float transY = this.Height / 2;
                    g.TranslateTransform(transX, transY);//屏幕中心旋转
                    g.RotateTransform(RotationAngle);
                    g.TranslateTransform(-transX, -transY);
                    if (map.IsDragging)
                    {
                        g.TranslateTransform(_tileTranslate.X, _tileTranslate.Y);
                    }
                }


                //g.DrawImage(_allTilesBitmap, map.RectTilesBitmap.ToRectangle());
                var srcRect = map.TilesArea.ToRectangle();
                srcRect.Offset(new Point(-map.RectTilesBitmap.LeftTop.X, -map.RectTilesBitmap.LeftTop.Y));
                lock (_bitmapLocker)
                {
                    g.DrawImage(_allTilesBitmap, map.TilesArea.ToRectangle(), srcRect, GraphicsUnit.Pixel);

                }

                g.ResetTransform();

                DrawLaylers(g);
                if (UserDraw != null)
                {
                    UserDraw.Draw(this, g);
                }

                if (IsShowMapMsg)
                {
                    DrawMapMsg(g);
                }
                _swRenser.Stop();
                if (IsShowRenderMsg)
                {
                    DrawRenderMsg(g);
                }
                if (IsShowMapScale)
                {
                    DrawMapScale(g);
                }
                //}
            }
        }




        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    _swRenser.Restart();
        //    var bitmap = new Bitmap(map.TilesArea.Width, map.TilesArea.Height);
        //    using (var gb = Graphics.FromImage(bitmap))
        //    {
        //        //g.SmoothingMode = SmoothingMode.HighQuality;
        //        //g.PixelOffsetMode = PixelOffsetMode.Half;//防止旋转后图片边缘出现空白
        //        DrawMapGrid(gb);
        //        DrawMapTiles(gb);
        //        DrawLaylers(gb);

        //        var g = e.Graphics;
        //        g.SmoothingMode = SmoothingMode.HighQuality;
        //        //整个地图控件GDI旋转
        //        if (RotationAngle != 0)
        //        {
        //            float transX = this.Width / 2;
        //            float transY = this.Height / 2;
        //            g.TranslateTransform(transX, transY);//屏幕中心旋转
        //            g.RotateTransform(RotationAngle);
        //            g.TranslateTransform(-transX, -transY);
        //        }
        //        g.DrawImage(bitmap, map.TilesArea.ToRectangle());
        //        _swRenser.Stop();
        //        DrawRenderMsg(g);
        //    }
        //    bitmap.Dispose();
        //}

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);
        //    _swRenser.Restart();

        //    using (var g = e.Graphics)
        //    {
        //        g.SmoothingMode = SmoothingMode.HighQuality;
        //        g.PixelOffsetMode = PixelOffsetMode.Half;//防止旋转后图片边缘出现空白
        //        //整个地图控件GDI旋转
        //        if (RotationAngle != 0)
        //        {
        //            float transX = this.Width / 2;
        //            float transY = this.Height / 2;
        //            g.TranslateTransform(transX, transY);//屏幕中心旋转
        //            g.RotateTransform(RotationAngle);
        //            g.TranslateTransform(-transX, -transY);
        //        }
        //        DrawMapGrid(g);
        //        DrawMapTiles(g);
        //        DrawLaylers(g);

        //        _swRenser.Stop();
        //        DrawRenderMsg(g);

        //    }
        //}

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    _swRenser.Restart();
        //    BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
        //    BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
        //    Graphics g = myBuffer.Graphics;
        //    g.SmoothingMode = SmoothingMode.HighQuality;
        //    g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
        //    g.Clear(this.BackColor);
        //    //整个地图控件GDI旋转
        //    if (RotationAngle != 0)
        //    {
        //        float transX = this.Width / 2;
        //        float transY = this.Height / 2;
        //        g.TranslateTransform(transX, transY);//屏幕中心旋转
        //        g.RotateTransform(RotationAngle);
        //        g.TranslateTransform(-transX, -transY);
        //    }
        //    DrawMapGrid(g);
        //    DrawMapTiles(g);
        //    DrawLaylers(g);




        //    _swRenser.Stop();
        //    DrawRenderMsg(g);
        //    myBuffer.Render(e.Graphics);
        //    g.Dispose();
        //    myBuffer.Dispose();

        //}


        protected override void OnMouseClick(MouseEventArgs e)
        {
            lock (_lstLayler)
            {
                foreach (var item in _lstLayler)
                {
                    if (item != null)
                    {

                        item.OnMouseClick(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// 记录鼠标最后所在点
        /// </summary>
        Point _lastMousePoint = Point.Empty;
        Point _lastMouseDownPoint = Point.Empty;
        Point _tileTranslate = new Point();
        Point _curMousePoint = Point.Empty;
        //Point _lastRefreshMousePoint = Point.Empty;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _curMousePoint = e.Location;
            var rp = GetRotatedPoint(e.Location, -RotationAngle);
            if (e.Button == MouseButtons.Left)
            {

                map.IsDragging = true;
                this.Cursor = Cursors.SizeAll;
                //bool isUpdateTiles = RotationAngle == 0;
                //if (RotationAngle==0)
                //{
                //    map.CenterOffSet(_lastMousePoint.X - rp.X, _lastMousePoint.Y - rp.Y);
                //}
                //else
                //{
                var rpLast = GetRotatedPoint(_lastMousePoint, - RotationAngle);
                var dx = rpLast.X - rp.X;
                var dy = rpLast.Y - rp.Y;
                if (RotationAngle != 0)
                {
                    _tileTranslate.Offset(-dx, -dy);
                }
                map.CenterMove(dx, dy, RotationAngle == 0);

                //}
                //_isNeedRefresh = true;
                Refresh();
            }
            else
            {
                map.IsDragging = false;
                this.Cursor = Cursors.Default;


                if (_lstLayler != null)
                {
                    foreach (var item in _lstLayler)
                    {
                        item.OnMouseMove(this, e);
                    }
                }

                
            }

            if (UserDraw != null)
            {
                UserDraw.OnMouseMove(this, e);
            }
            base.OnMouseMove(e);
            _lastMousePoint = e.Location;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (UserDraw != null)
            {
                UserDraw.OnMouseDoubleClick(this, e);
            }

           
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //if (e.Button== MouseButtons.Left)
            //{
                
            //    //map.IsDragging = true;
            //}
            _lastMouseDownPoint = e.Location;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Location.Equals(_lastMouseDownPoint))
            {
                if (UserDraw!=null)
                {
                    UserDraw.OnMouseClick(this, e);
                }
            }

            switch (e.Button)
            {
                case MouseButtons.Left:
                    map.IsDragging = false;
                    if (RotationAngle != 0)
                    {
                        //map.CenterOffSet(_lastMouseDownPoint.X - _lastMousePoint.X, _lastMouseDownPoint.Y - _lastMousePoint.Y);
                        _tileTranslate = new Point();
                        map.UpdateDrawTiles();
                        Refresh();
                    }
                    break;
                case MouseButtons.None:
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    break;
                case MouseButtons.XButton1:
                    break;
                case MouseButtons.XButton2:
                    break;
                default:
                    break;
            }


            base.OnMouseUp(e);
        }

        private float _bitmapScale = 0;
        //private Point _bitmapOffSet;
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var rp = GetRotatedPoint(e.Location, -RotationAngle);
            int scaleValue = 0;
            if (e.Delta > 0)
            {
                scaleValue = 1;
                //map.PointZoom(new PointInt(rp.X, rp.Y), 1);

            }
            else if (e.Delta < 0)
            {
                scaleValue = -1;
                //map.PointZoom(new PointInt(rp.X, rp.Y), -1);
            }
            //bMapScale1.Distance = MercatorHelper.GetResolution(Zoom, MapCenter.LatY) * bMapScale1.ScaleLen;

            //ScaleTilesBitmap(scaleValue, rp);
            //Refresh();
            //Thread.Sleep(1000);
            map.PointZoom(new PointInt(rp.X, rp.Y), scaleValue);
            Refresh();
            base.OnMouseWheel(e);
        }

        //private void ScaleTilesBitmap(int scaleValue,Point p)
        //{
        //    int oldWidth = _allTilesBitmap.Width;
        //    int oldHeight = _allTilesBitmap.Height;
        //    var scale = (float)Math.Pow(2d, (double)scaleValue);

        //    var p1 = p.GetOffSet(-map.RectTilesBitmap.LeftTop.X, -map.RectTilesBitmap.LeftTop.Y);

        //    Bitmap screenBitmap = new Bitmap(this.Width, this.Height);
        //    this.DrawToBitmap(screenBitmap, this.ClientRectangle);

        //    var scaleBitmap = DrawHelper.ScaleImage(screenBitmap, scale,scale);
        //    if (scaleBitmap != null)
        //    {
        //        var halfW = screenBitmap.Width / 2;
        //        var halfSW = scaleBitmap.Width / 2;
        //        var halfH = screenBitmap.Height / 2;
        //        var halfSH = scaleBitmap.Height / 2;
        //        Point offset = new Point();
        //        offset.X = (int)(halfW - halfSW + (p1.X - halfW) - (p1.X - halfW) * scale);
        //        offset.Y = (int)(halfH - halfSH + (p1.Y - halfH) - (p1.Y - halfH) * scale);

        //        //offset.X = (int)(halfW - halfSW)-map.RectTilesBitmap.LeftTop.X;
        //        //offset.Y = (int)(halfH - halfSH)- map.RectTilesBitmap.LeftTop.Y;

        //        using (var g = Graphics.FromImage(_allTilesBitmap))
        //        {
        //            g.Clear(this.BackColor);
        //            g.DrawImage(scaleBitmap, offset);
        //        }
        //    }
        //    else
        //        _allTilesBitmap = null;


        //    //int newWidth = (int)(_allTilesBitmap.Width * scale);
        //    //int newHeight = (int)(_allTilesBitmap.Height * scale);
        //    ////var scaleBitmap = DrawHelper.ZoomPicture(this.bitmap, newWidth, newHeight);
        //    //var scaleBitmap = new Bitmap((int)(this.Width ), (int)(this.Height));
        //    //this.DrawToBitmap(scaleBitmap,this.ClientRectangle);
        //    //scaleBitmap = DrawHelper.ZoomPicture(scaleBitmap, (int)(this.Width*scale), (int)(this.Height*scale));
        //    //_bitmapOffSet = new Point( (int)((oldWidth -newWidth)*scale) + map.RectTilesBitmap.LeftTop.X, (int)((oldHeight - newHeight)*scale)+map.RectTilesBitmap.LeftTop.Y);
        //    //_bitmapOffSet.Offset(this.Width / 2 - _allTilesBitmap.Width / 2 , this.Height / 2 - _allTilesBitmap.Height / 2 );
        //    ////if (_bitmapScale > 0)
        //    ////{
        //    ////    _bitmapOffSet.Offset(-(p.X - _allTilesBitmap.Width / 2) , -(p.Y - _allTilesBitmap.Height / 2) );
        //    ////}
        //    ////else if (_bitmapScale < 0)
        //    ////{
        //    ////    _bitmapOffSet.Offset((p.X - this.Width / 2), (p.Y - this.Height / 2));
        //    ////}
        //    //using (var g = Graphics.FromImage(_allTilesBitmap))
        //    //{
        //    //    g.Clear(this.BackColor);
        //    //    g.DrawImage(scaleBitmap, _bitmapOffSet);
        //    //}

          
        //}
        



        protected override void OnSizeChanged(EventArgs e)
        {
            map.ReSetMapSize(this.Width, this.Height);
       
                //this.bMapScale1.Location = new Point(5, this.Height - bMapScale1.Height - 5);
            Refresh();
            base.OnSizeChanged(e);
        }

        public override void Refresh()
        {
            //if ((DateTime.Now - _lastRefreshTime).Milliseconds>50 || map.IsDragging)
            //{
                base.Refresh();
                _lastRefreshTime = DateTime.Now;
            _isNeedRefresh = false;
            //}
        }
        #endregion


    }
}
