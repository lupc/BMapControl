#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/17 15:44:20 
* 文件名：MarkerBase 
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMap.WinForm
{
    /// <summary>
    /// 地图标记基类
    /// </summary>
    public abstract class MarkerBase
    {
        public MarkerBase(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 所属图层
        /// </summary>
        public MapLayer Owner { get; set; }
        /// <summary>
        /// 名称，不能重复
        /// </summary>
        public string Name { get; set; }
        ///// <summary>
        ///// 图标
        ///// </summary>
        //public Image Icon { get; set; }
        /// <summary>
        /// 屏幕位置
        /// </summary>
        public Point ScreenPosition { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public Size Size { get; set; }
        /// <summary>
        /// 所属屏幕区域（局部刷新用）
        /// </summary>
        public Rectangle ScreenArea { get; protected set; } = Rectangle.Empty;
        /// <summary>
        /// 重绘区域
        /// </summary>
        public Region RefreshRegion { get;protected set; }
        /// <summary>
        /// 经纬度坐标
        /// </summary>
        public PointLatLng Position { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public Point OffSet { get; set; }
        /// <summary>
        /// 是否鼠标经过
        /// </summary>
        public bool IsMouseOver { get; set; }
        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; } = true;
        /// <summary>
        /// 提示
        /// </summary>
        public MarkerToolTip ToolTip { get; set; }
        /// <summary>
        /// 提示弹出方式
        /// </summary>
        public ToolTipShowMode ToolTipMode { get; set; } = ToolTipShowMode.MouseOver;
        /// <summary>
        /// 绘制标记
        /// </summary>
        /// <param name="g"></param>
        public abstract void Draw(BMapControl c, Graphics g);
        /// <summary>
        /// 点是否包含在标记范围内
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract bool Contains(Point p);
        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="c"></param>
        /// <param name="e"></param>
        public virtual void OnMouseClick(BMapControl c, MouseEventArgs e)
        {
            if (ToolTip != null && ToolTipMode == ToolTipShowMode.MouseClick)
            {
                ToolTip.IsOpen = !ToolTip.IsOpen;
            }
        }
        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="c"></param>
        public virtual void OnMouseEnter(BMapControl c, MouseEventArgs e)
        {
           
            IsMouseOver = true;
            if (ToolTip != null && ToolTipMode == ToolTipShowMode.MouseOver)
            {
                ToolTip.IsOpen = IsMouseOver;
            }
        }
        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="c"></param>
        public virtual void OnMouseLeave(BMapControl c, MouseEventArgs e)
        {
            IsMouseOver = false;
            if (ToolTip != null && ToolTipMode == ToolTipShowMode.MouseOver)
            {
                ToolTip.IsOpen = IsMouseOver;
            }
        }


    }

   
}
