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

namespace BMap.WinForm.Control
{
    public partial class BMapScale : UserControl
    {
        public BMapScale()
        {
            this.MinimumSize = new Size(100, 30);
            Font = new Font("微软雅黑", 10, FontStyle.Bold);
            InitializeComponent();
            
        }

        /// <summary>
        /// 比例尺长度
        /// </summary>
        public int ScaleLen { get; set; } = 50;
     
        private double _dis;
        /// <summary>
        /// 实际距离 单位米
        /// </summary>
        public double Distance
        {
            get { return _dis; }
            set {
                if (_dis!=value)
                {
                    _dis = value;
                    //Refresh();
                }
                //_dis = value;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            int scaleLen = ScaleLen;

            //画背景色
            if (BackColor!=Color.Transparent)
            {
                g.FillRectangle(new SolidBrush(BackColor), this.ClientRectangle);
            }
           

            Point p = new Point(10, 10);
            Point[] ps = new Point[4];
            ps[0] = p;
            p.Offset(0, 5);
            ps[1] = p;
            p.Offset(scaleLen, 0);
            ps[2] = p;
            p.Offset(0, -5);
            ps[3] = p;
            var brush = new SolidBrush(this.ForeColor);
            var pen = new Pen(brush, 2);
            g.DrawLines(pen, ps);
            p.Offset(5, -10);
            g.DrawString(Distance.ToString("0.00") + "米", Font, brush, p);
        }
    }
}
