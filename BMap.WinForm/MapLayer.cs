#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/17 17:05:19 
* 文件名：Layer 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMap.WinForm
{
    /// <summary>
    /// 图层
    /// </summary>
    public class MapLayer
    {
        public MapLayer(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 图层名称 不能重复
        /// </summary>
        public string Name { get; set; }

        private bool _isVisible = true;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
        /// <summary>
        /// 地图控件实例
        /// </summary>
        public BMapControl MapObject { get; set; }

        #region 事件
        /// <summary>
        /// 标记点击事件
        /// </summary>
        public event Action<MarkerBase, MouseEventArgs> MarkerClick;
        /// <summary>
        /// 标记进入事件
        /// </summary>
        public event Action<MarkerBase> MarkerEnter;

        /// <summary>
        /// 标记离开事件
        /// </summary>
        public event Action<MarkerBase> MarkerLeave;

        /// <summary>
        /// 提示打开事件
        /// </summary>
        public event Action<MarkerToolTip> ToolTipOpened;

        /// <summary>
        /// 提示关闭事件
        /// </summary>
        public event Action<MarkerToolTip> ToolTipCloseed;

        /// <summary>
        /// 提示点击事件
        /// </summary>
        public event Action<MarkerToolTip> ToolTipClick;

        #endregion



        private Dictionary<string, MarkerBase> _dicMarker = new Dictionary<string, MarkerBase>();
        /// <summary>
        /// 获取标记 线程安全
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MarkerBase GetMarker(string name)
        {
            MarkerBase m = null;
            lock (_dicMarker)
            {
                _dicMarker.TryGetValue(name, out m);
            }
            return m;
        }
        /// <summary>
        /// 添加标记 线程安全
        /// </summary>
        /// <param name="marker"></param>
        public void AddMarker(MarkerBase marker,bool ifExitsUpdate = false)
        {
            if (marker != null && !string.IsNullOrEmpty(marker.Name))
            {
                marker.Owner = this;
               
                lock (_dicMarker)
                {
                    if (_dicMarker.ContainsKey(marker.Name))
                    {
                        if (ifExitsUpdate)
                        {
                            _dicMarker[marker.Name] = marker;
                            AddToolTipEvent(marker.ToolTip);
                        }
                    }
                    else
                    {
                        _dicMarker.Add(marker.Name, marker);
                        AddToolTipEvent(marker.ToolTip);

                    }
                        
                }

            }
            else
            {
                throw new Exception("marker.Name不能为空！");
            }

        }
        /// <summary>
        /// 清空标记 线程安全
        /// </summary>
        public void ClearMarker()
        {
            lock (_dicMarker)
            {
                _dicMarker.Clear();
            }
        }
        public void Draw(BMapControl c, Graphics g)
        {
            if (IsVisible)
            {

                lock (_dicMarker)
                {
                    foreach (var item in _dicMarker.Values)
                    {
                        if (item.IsVisible)
                        {
                            item.Draw(c,g);
                        }

                    }
                }
            }

        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="c"></param>
        /// <param name="e"></param>
        public void OnMouseClick(BMapControl c, MouseEventArgs e)
        {
            if (IsVisible)
            {
                lock (_dicMarker)
                {
                    foreach (var m in _dicMarker.Values)
                    {
                        //检测点击Marker
                        if (m!=null&&m.Contains(e.Location))
                        {
                            m.OnMouseClick(c, e);
                            if (MarkerClick!=null)
                            {
                                MarkerClick(m, e);
                            }
                        }

                        //检测点击ToolTip
                        var tip = m.ToolTip;

                        if (tip!=null && !tip.ViewRect.IsEmpty&&tip.ViewRect.Contains(e.Location))
                        {
                            tip.OnMouseClick(c, e);
                            if (ToolTipClick!=null)
                            {
                                ToolTipClick(tip);
                            }
                        }
                    }
                }
            }
            
        }

       

        public void OnMouseMove(BMapControl c, MouseEventArgs e)
        {
            if (IsVisible)
            {
                if (_dicMarker != null&& _dicMarker.Count>0)
                {
                    List<MarkerBase> lstMarker = null;
                    lock (_dicMarker)
                    {
                        lstMarker = _dicMarker.Values.ToList();
                    }
                    if (lstMarker!=null)
                    {
                        foreach (var m in lstMarker)
                        {
                            if (m != null && m.IsVisible)
                            {
                                //判断鼠标经过Marker
                                if (!m.IsMouseOver)
                                {
                                    if (m.Contains(e.Location))
                                    {
                                        m.OnMouseEnter(c, e);
                                        
                                        RefreshMarker(m);
                                        if (MarkerEnter!=null)
                                        {
                                            MarkerEnter(m);
                                        }
                                      
                                    }
                                }
                                else
                                {
                                    if (!m.Contains(e.Location))
                                    {
                                        //m.IsMouseOver = false;
                                        m.OnMouseLeave(c, e);
                                        RefreshMarker(m);
                                        if (MarkerLeave!=null)
                                        {
                                            MarkerLeave(m);
                                        }
                                    }
                                }

                                //判断鼠标经过tooltip
                                var tip = m.ToolTip;
                                if (tip != null && tip.IsOpen)
                                {
                                    if (!tip.IsMouseOver)
                                    {
                                        if (tip.ViewRect.Contains(e.Location))
                                        {
                                            tip.IsMouseOver = true;
                                            RefreshToolTip(tip);
                                        }
                                    }
                                    else
                                    {
                                        if (!tip.ViewRect.Contains(e.Location))
                                        {
                                            tip.IsMouseOver = false;
                                            RefreshToolTip(tip);
                                        }
                                    }
                                }



                            }
                        }
                    }
                    
                }
                
            }
        }
        /// <summary>
        /// 注册ToolTip事件
        /// </summary>
        private void AddToolTipEvent(MarkerToolTip tip)
        {
            if (tip != null)
            {
                if (this.ToolTipCloseed != null)
                {
                    tip.ToolTipClosed += this.ToolTipCloseed;
                }
                if (ToolTipOpened != null)
                {
                    tip.ToolTipOpened += this.ToolTipOpened;
                }
            }
        }

        private void RefreshMarker(MarkerBase m)
        {
            if (m.ScreenArea.IsEmpty)
            {
                MapObject.Refresh();
            }
            else
            {
                MapObject.Invalidate(m.ScreenArea);
               
                //if (m.ToolTip!=null)
                //{
                //    MapObject.Invalidate(m.ToolTip.ViewRect);
                //}

                MapObject.Update();
            }

        }

        private void RefreshToolTip(MarkerToolTip tip)
        {
            if (!tip.ViewRect.IsEmpty)
            {
                MapObject.Invalidate(tip.ViewRect);
                MapObject.Update();
            }
        }

    }
}
