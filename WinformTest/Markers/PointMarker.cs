#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/21 11:39:15 
* 文件名：PointMarkers 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BMap.WinForm;

namespace WinformTest.Markers
{
    public class PointMarker : BMap.WinForm.MarkerBase
    {
        public PointMarker(string name):base(name)
        {
            Size = new Size(18, 18);
            OffSet = new Point(-9, -9);
            ToolTip = new MarkerRoundedToolTip(this);
            ToolTip.Title = "测试标题";
            ToolTip.Content = "测试内容测试内容1";
            ToolTip.Content += "\r\n测试内容测试内容1dddddddddd";
            ToolTip.Content += "\r\n测试内容测试内容1sssss";
            ToolTipMode = ToolTipShowMode.MouseOver;
        }
        protected BMapControl map = null;
        public override bool Contains(Point p)
        {
                return ScreenArea.Contains(p);
        }

        public override void Draw(BMapControl c, Graphics g)
        {
            if (IsVisible)
            {
                this.map = c;
                ScreenPosition = map.LatLngToPoint(Position);
                Point p = ScreenPosition.GetOffSet(OffSet.X,OffSet.Y);
                var rect = ScreenArea = new Rectangle(p, Size);
                if (IsMouseOver)
                {
                    g.FillEllipse(new SolidBrush(Color.FromArgb(0xaa, 0xff, 0xff, 0xff)), rect);

                    //if (ToolTip!=null)
                    //{
                    //    ToolTip.IsOpen=IsMouseOver;
                    //}
                }
                
                p.Offset(3, 3);
                var rect1 = new Rectangle(p.X,p.Y, Size.Width-6, Size.Height-6);
                g.FillEllipse(new SolidBrush(Color.FromArgb(0xFF, 0x11, 0x11, 0xff)), rect1);


                ToolTip.Draw(c,g);
            }
        }

        public override void OnMouseClick(BMapControl c, MouseEventArgs e)
        {
            base.OnMouseClick(c, e);
            //MessageBox.Show("test");
        }
    }
}
