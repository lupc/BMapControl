#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/30 9:49:39 
* 文件名：UserDrawing 
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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BMap.Core;
using BMap.Core.Model;


namespace BMap.WinForm
{
    /// <summary>
    /// 用户在地图上画图封装
    /// </summary>
    public class UserDrawing
    {
        private BMapControl mapObj;
        public UserDrawing(BMapControl map)
        {
            mapObj = map;
        }
        /// <summary>
        /// 画图模式
        /// </summary>
        public EditMode DrawMode { get; set; }
        /// <summary>
        /// 用户画图中产生的标记点集合
        /// </summary>
        public List<PointLatLng> LstMarkPoint { get; set; } = new List<PointLatLng>();
        /// <summary>
        /// 是否完成画图
        /// </summary>
        public bool IsCompleted { get; set; }

        public Pen DrawPen { get; set; } = new Pen(Brushes.White, 2);
        /// <summary>
        /// 填充颜色
        /// </summary>
        public Brush FillBrush { get; set; } = new SolidBrush(Color.FromArgb(0x33, Color.Blue));

        public event Action<List<PointLatLng>> DrawComplete;

        /// <summary>
        /// 折线总距离
        /// </summary>
        private double _linesDist;


        private GraphicsPath PolyonPath;

        public void ReSet(BMapControl c)
        {
            _linesDist = 0;
            _drawCompleteToolTip = new InfoToolTip();
            IsCompleted = false;
            LstMarkPoint.Clear();
            c.Refresh();
        }

        public void OnMouseMove(BMapControl c, MouseEventArgs e)
        {
            _curMousePoint = e.Location;
            switch (DrawMode)
            {

                case EditMode.DrawLine:
                    if (LstMarkPoint.Count == 1)
                    {
                        var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                        c.Invalidate(DrawHelper.GetLineRegion(p1, _lastMousePoint));
                        c.Update();
                    }


                    break;
                case EditMode.DrawLines:
                    if (LstMarkPoint.Count>0)
                    {
                        var p1 = c.LatLngToPoint(LstMarkPoint.Last());
                        c.Invalidate(DrawHelper.GetLineRegion(p1, _lastMousePoint));
                        c.Update();
                    }
                    break;
                case EditMode.DrawRect:
                    if (LstMarkPoint.Count == 1)
                    {
                        var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                        c.Invalidate(DrawHelper.GetLineRegion(p1, _lastMousePoint));
                        //c.Invalidate(DrawHelper.GetLineRegion(p1, _curMousePoint));
                        c.Update();
                    }
                    break;
                case EditMode.DrawPolyon:
                    if (LstMarkPoint.Count>0)
                    {
                        var p1 = c.LatLngToPoint(LstMarkPoint.Last());
                        c.Invalidate(DrawHelper.GetLineRegion(p1, _lastMousePoint));
                        c.Update();
                    }
                    break;
                case EditMode.DrawCircle:
                    if (LstMarkPoint.Count == 1)
                    {
                        var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                        var region = DrawHelper.GetCircleRegion(p1, _lastMousePoint);
                        //var region2 = DrawHelper.GetCircleRegion(p1, _lastMousePoint);
                        c.Invalidate(region);
                        //c.Invalidate(region2);
                        c.Update();
                    }
                    break;
                case EditMode.DrawPie:
                    if (LstMarkPoint.Count == 1 )
                    {
                        var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                        var curLatLng = c.MouseLatLng;
                        var region = DrawHelper.GetCircleRegion(p1, _lastMousePoint);
                        c.Invalidate(region);
                        c.Update();
                        
                    }
                    else if (LstMarkPoint.Count == 2)
                    {
                        var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                        var p2 = c.LatLngToPoint(LstMarkPoint[1]);
                        var region = DrawHelper.GetCircleRegion(p1, p2);
                        c.Invalidate(region);
                        c.Update();
                        //_curMouseToolTip.Info = string.Format("角度：{0:0.00}°", MercatorHelper.GetDistance(LstMarkPoint[0], curLatLng), MercatorHelper.GetBearing(LstMarkPoint[0], curLatLng));

                    }
                    break;
                case EditMode.None:
                default:
                    
                    break;
            }
            if (DrawMode != EditMode.None)
            {
                //Refresh();
                //刷新之前鼠标信息区域

                c.Invalidate(_curMouseToolTip.RefreshRegion);//刷掉之前的绘图
                _curMouseToolTip.ShowPoint = e.Location;
                _curMouseToolTip.Info = string.Format("经度：{0:0.0000000}\r\n纬度：{1:0.0000000}", c.MouseLatLng.LngX, c.MouseLatLng.LatY);
                //c.Invalidate(_curMouseToolTip.RefreshRegion);//刷掉之前的绘图
                c.Update();
                //var p = _lastMousePoint;
                //p.Offset(-1, -80);
                //var rect = new Rectangle(p.X, p.Y, 200, 82);
                //c.Invalidate(rect);
                ////刷新当前鼠标信息区域
                //p = _curMousePoint;
                //p.Offset(-1, -80);
                //var curRect = new Rectangle(p.X, p.Y, 200, 82);
                //c.Invalidate(curRect);
                //c.Update();
            }

            _lastMousePoint = e.Location;
        }

        public void OnMouseClick(BMapControl c, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (IsCompleted)
                {
                    ReSet(c);
                }

                switch (DrawMode)
                {

                    case EditMode.DrawLine:
                        if (LstMarkPoint.Count < 2)
                        {
                            LstMarkPoint.Add(c.PointToLatLng(e.Location));
                        }
                        else
                        {
                            
                            LstMarkPoint.Add(c.PointToLatLng(e.Location));
                        }
                        if (LstMarkPoint.Count == 2)
                        {
                            IsCompleted = true;
                            var p1 = LstMarkPoint[0];
                            var p2 = LstMarkPoint[1];
                            //c.Invalidate(DrawHelper.GetLineRegion(c.LatLngToPoint(p1), c.LatLngToPoint(p2)));

                            _drawCompleteToolTip.Info = string.Format("距离：{0:0.00} 米\r\n正北角：{1:0.0} °"
                                ,MercatorHelper.GetDistance(p1, p2),MercatorHelper.GetBearing(p1,p2));
                        }
                        break;
                    case EditMode.DrawLines:
                        PointLatLng lastP = null;
                        if (LstMarkPoint.Count > 0)
                        {
                            lastP = LstMarkPoint.Last();
                        }
                        var curP = c.PointToLatLng(e.Location);
                        LstMarkPoint.Add(curP);
                        //计算距离
                        if (lastP != null)
                        {
                            _linesDist += MercatorHelper.GetDistance(lastP, curP);
                        }
                        _drawCompleteToolTip.Info = string.Format("总距离：{0:0.00} 米", _linesDist);

                        break;
                    case EditMode.DrawRect:
                        if (LstMarkPoint.Count < 2)
                        {
                            LstMarkPoint.Add(c.PointToLatLng(e.Location));
                        }
                        if (LstMarkPoint.Count == 2)
                        {
                            IsCompleted = true;
                            //var p1 = LstMarkPoint[0];
                            //var p2 = LstMarkPoint[1];
                            ////c.Invalidate(DrawHelper.GetLineRegion(c.LatLngToPoint(p1), c.LatLngToPoint(p2)));
                          
                            //_drawCompleteToolTip.Info = string.Format("面积：{0:0.00} m²\r\n正北角：{1:0.0} °"
                            //    , MercatorHelper.GetDistance(p1, p2)* MercatorHelper.GetBearing(p1, p2));
                        }
                        break;
                    case EditMode.DrawPolyon:
                        LstMarkPoint.Add(c.PointToLatLng(e.Location));
                        break;
                    case EditMode.DrawCircle:
                        LstMarkPoint.Add(c.PointToLatLng(e.Location));
                        if (LstMarkPoint.Count==2)
                        {
                            IsCompleted = true;
                            var p1 = LstMarkPoint[0];
                            var p2 = LstMarkPoint[1];
                            _drawCompleteToolTip.Info = string.Format("半径：{0:0.00}米", MercatorHelper.GetDistance(p1, p2));
                        }
                        break;
                    case EditMode.DrawPie:
                        LstMarkPoint.Add(c.PointToLatLng(e.Location));
                        if (LstMarkPoint.Count==3)
                        {
                            IsCompleted = true;
                            var p1 = c.LatLngToPoint( LstMarkPoint[0]);
                            var p2 = c.LatLngToPoint(LstMarkPoint[1]);
                            var p3 = c.LatLngToPoint(LstMarkPoint[2]);

                            _drawCompleteToolTip.Info = string.Format("坐标：{0}\r\n半径：{1:0.00}米\r\n中线方向：{2:0.00}°\r\n角度宽：{3:0.00}°",
                                LstMarkPoint[0], 
                                MercatorHelper.GetDistance(LstMarkPoint[0], LstMarkPoint[1]),
                                MercatorHelper.GetBearing(LstMarkPoint[0], LstMarkPoint[1]),
                                2*DrawHelper.GetAngle(p1,p2,p3));
                           
                        }
                        break;
                    case EditMode.None:
                    default:

                        break;
                }

                


            }
            else if (e.Button == MouseButtons.Right)
            {
                IsCompleted = true;
            }

            if (DrawMode != EditMode.None)
            {
                c.Refresh();
            }

            if (IsCompleted)
            {
                if (DrawComplete!=null)
                {
                    DrawComplete(LstMarkPoint);
                }
            }
        }

        public void OnMouseDoubleClick(BMapControl c, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                //左键双击结束画图
                DrawMode = EditMode.None;
            }
        }

        private Point _curMousePoint;
        private Point _lastMousePoint;

        private InfoToolTip _curMouseToolTip = new InfoToolTip();
        private InfoToolTip _drawCompleteToolTip = new InfoToolTip();

        public void Draw(BMapControl c, Graphics g)
        {
            if (DrawMode != EditMode.None)
            {
                Point lastPoint = Point.Empty;
                if (LstMarkPoint.Count>0)
                {
                    lastPoint = c.LatLngToPoint(LstMarkPoint.Last());
                }

                string editModeMsg = string.Empty;
                
                switch (DrawMode)
                {
                    case EditMode.None:
                        break;
                    case EditMode.DrawLine:
                        if (LstMarkPoint.Count == 1)
                        {
                            var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                            DrawDashLine(g, p1, _curMousePoint);
                        }
                        else if (LstMarkPoint.Count == 2)
                        {
                            var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                            var p2 = c.LatLngToPoint(LstMarkPoint[1]);
                            g.DrawLine(DrawPen, p1, p2);
                            _drawCompleteToolTip.ShowPoint = p2;
                        }
                        editModeMsg = "画线模式（测距，测正北角）";
                        break;
                    case EditMode.DrawLines:
                        if (LstMarkPoint.Count > 0)
                        {
                            var lst = new List<Point>();
                            //double dist = 0;
                            //var tempLatLng = new PointLatLng();
                            foreach (var item in LstMarkPoint)
                            {
                                lst.Add(c.LatLngToPoint(item));
                            }
                            if (lst.Count > 1)
                            {
                                g.DrawLines(DrawPen, lst.ToArray());

                            }
                        }
                        if (IsCompleted)
                        {
                            _drawCompleteToolTip.ShowPoint = lastPoint;
                        }
                        editModeMsg = "画折线模式（测距）";
                        break;
                    case EditMode.DrawRect:
                        if (LstMarkPoint.Count == 1)
                        {
                            var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                            DrawDashLine(g, p1, _curMousePoint);

                            DrawPen.DashStyle = DashStyle.Solid;
                        }
                        else if (LstMarkPoint.Count == 2)
                        {
                            var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                            var p2 = c.LatLngToPoint(LstMarkPoint[1]);
                            var rect = DrawHelper.GetRect(p1, p2);
                            g.FillRectangle(FillBrush, rect);
                            g.DrawRectangle(DrawPen, rect);
                            //_drawCompleteToolTip.ShowPoint = p2;
                        }
                        editModeMsg = "画矩形模式";
                        break;
                    case EditMode.DrawPolyon:
                        
                        if (LstMarkPoint.Count>1)
                        {
                            List<Point> lstp = new List<Point>();
                            foreach (var item in LstMarkPoint)
                            {
                                lstp.Add(c.LatLngToPoint(item));
                            }
                            
                            PolyonPath = new GraphicsPath();
                            var ps = lstp.ToArray();
                            
                            if (!IsCompleted)
                            {
                                g.DrawLines(DrawPen, ps);
                                //DrawDashLine(g, lastPoint, _curMousePoint);
                            }
                            else
                            {
                                if (ps.Length>2)
                                {
                                    g.DrawPolygon(DrawPen, ps);
                                }
                                
                            }
                            if (ps.Length>2)
                            {
                                PolyonPath.AddPolygon(ps);
                                PolyonPath.CloseFigure();
                                g.FillPolygon(FillBrush, ps);
                            }
                        }
                        
                        editModeMsg = "画多边形模式";
                        break;
                    case EditMode.DrawCircle:
                        if (LstMarkPoint.Count == 1)
                        {

                            var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                            var rect = DrawHelper.GetEllipseRect(p1, _curMousePoint);

                            DrawPen.DashStyle = DashStyle.DashDot;
                            DrawPen.DashPattern = new float[] { 5, 2 };
                            g.FillEllipse(FillBrush, rect);
                            g.DrawEllipse(DrawPen, rect);
                            DrawPen.DashStyle = DashStyle.Solid;
                            
                        }
                        else if (LstMarkPoint.Count == 2)
                        {
                            var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                            var p2 = c.LatLngToPoint(LstMarkPoint[1]);
                            var rect = DrawHelper.GetEllipseRect(p1, p2);
                            g.FillEllipse(FillBrush, rect);
                            g.DrawEllipse(DrawPen, rect);
                            //g.FillEllipse(DrawPen.Brush, new Rectangle(p1.GetOffSet(-5, -5), new Size(10, 10)));//画圆心
                            DrawDashLine(g,p1, p2);
                            _drawCompleteToolTip.ShowPoint = p2;

                        }
                        editModeMsg = "画圆模式";
                        break;
                    case EditMode.DrawPie:
                        switch (LstMarkPoint.Count)
                        {
                            case 1:
                                {
                                    var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                                    var curLatLng = c.MouseLatLng;
                                    DrawPen.DashStyle = DashStyle.DashDot;
                                    DrawPen.DashPattern = new float[] { 5, 2 };
                                    DrawPie(g, p1, _curMousePoint);
                                    DrawDashLine(g,p1, _curMousePoint);
                                    _curMouseToolTip.Info = string.Format("距离：{0:0.00}米\r\n正北角：{1:0.00}°", MercatorHelper.GetDistance(LstMarkPoint[0], curLatLng), MercatorHelper.GetBearing(LstMarkPoint[0], curLatLng));
                                }

                                break;
                            case 2:
                                {
                                    var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                                    var p2 = c.LatLngToPoint(LstMarkPoint[1]);

                                    var angle = DrawHelper.GetAngle(p1, p2, _curMousePoint);
                                    float angleWidth = 2 * (float)angle;
                                    DrawPen.DashStyle = DashStyle.DashDot;
                                    DrawPen.DashPattern = new float[] { 5, 2 };
                                    DrawPie(g, p1, p2, angleWidth);
                                    
                                    DrawDashLine(g, p1, p2);
                                    //DrawDashLine(g, p1, _curMousePoint);

                                    _curMouseToolTip.Info = string.Format("角度：{0:0.00}°", angleWidth);
                                }
                               
                                break;
                            case 3:
                                {
                                    var p1 = c.LatLngToPoint(LstMarkPoint[0]);
                                    var p2 = c.LatLngToPoint(LstMarkPoint[1]);
                                    var p3= c.LatLngToPoint(LstMarkPoint[2]);
                                    var angle = DrawHelper.GetAngle(p1, p2, p3);
                                    float angleWidth = 2 * (float)angle;
                                    DrawPen.DashStyle = DashStyle.Solid;
                                    DrawPie(g, p1, p2, angleWidth);
                                    //g.DrawPie(DrawPen,)

                                    _drawCompleteToolTip.ShowPoint = p1;
                                }
                                break;
                            default:
                                break;
                        }
                        editModeMsg = "画扇形模式";
                        break;
                    default:
                        break;
                }

                //画坐标点信息
                _curMouseToolTip.Draw(c, g);

                if (IsCompleted)
                {
                    //_drawCompleteToolTip.ShowPoint = lastPoint;
                    _drawCompleteToolTip.Draw(c, g);//画完成信息
                }
                else if(!lastPoint.IsEmpty)
                {
                    switch (DrawMode)
                    {
                        case EditMode.None:
                            break;
                        case EditMode.DrawLine:
                        case EditMode.DrawLines:
                        case EditMode.DrawPolyon:
                        case EditMode.DrawCircle:
                        //case EditMode.DrawPie:
                            //画虚线
                            DrawDashLine(g, lastPoint, _curMousePoint);
                            break;
                        
                        default:
                            break;
                    }

                }

                var pMode = new Point(c.Width / 2 - 50, 10);
                g.DrawString(editModeMsg, new Font("微软雅黑", 12, FontStyle.Bold), Brushes.White, pMode);//画模式
            }
        }

        private void DrawPie(Graphics g,Point p1, Point p2, float angleWidth = 20)
        {
            if (p1!=p2&&angleWidth>0)
            {
                var rect = DrawHelper.GetEllipseRect(p1, p2);
                float startAngle = DrawHelper.GetAngle(p1, p2) - angleWidth/2;
                g.FillPie(FillBrush, rect, startAngle, angleWidth);
                g.DrawPie(DrawPen, rect, startAngle, angleWidth);
            }
            
        }

        /// <summary>
        /// 画虚线
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void DrawDashLine(Graphics g, Point p1,Point p2)
        {
            DrawPen.DashStyle = DashStyle.DashDot;
            DrawPen.DashPattern = new float[] { 5, 2 };
            g.DrawLine(DrawPen, p1, p2);
            DrawPen.DashStyle = DashStyle.Solid;
        }
        
    }
}
