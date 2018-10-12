#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/20 16:48:21 
* 文件名：AreaMarker 
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
    public class AreaMarker : MarkerBase

    {
        public AreaMarker(string name):base(name)
        {

        }
        public List<PointLatLng> lstPLatLng = new List<PointLatLng>();
        private List<Point> lstPoint = null;
        public override bool Contains(Point p)
        {
            return false;
            //bool r = false;
            //if (lstPoint!=null)
            //{
            //    r = BMap.Core.MapHelper.IsPointInsidePolygon(p, lstPoint.ToArray());
            //}

            //return r;
        }

        public override void Draw(BMapControl c, Graphics g)
        {
            lstPoint = new List<Point>();
            foreach (var item in lstPLatLng)
            {
                lstPoint.Add(c.LatLngToPoint(item));
            }

            using (var p = new Pen(Brushes.Yellow,1))
            {
                g.DrawPolygon(p, lstPoint.ToArray());
            }
            
        }

        public override void OnMouseClick(BMapControl c, MouseEventArgs e)
        {
           
        }
    }
}
