using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BMap.Core;
using BMap.WinForm;
using WinformTest.Markers;

namespace WinformTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
            InitializeComponent();

           

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TileManager.Singleton.TileLoaders = new List<TileImgLoaderBase>
            {
                //new TileImgFileLoader("map"){ Priority=999,ThreadCount=5},
                new GoogleChinaHybridTileLoader(){Priority = 9,ThreadCount=10},
                //new TileImgHttpLoader("http://127.0.0.1:8888/map"){ Priority=1},
            };

            this.bMapControl1.IsShowMapMsg = true;
            this.bMapControl1.IsShowRenderMsg = true;
            this.bMapControl1.IsShowGrid = false;
            this.bMapControl1.TileType = TileType.GCJ02;

            this.bMapControl1.MapCenter = new BMap.Core.Model.PointLatLng(113.415041, 23.1824994);
            this.bMapControl1.Zoom = 17;
            
            //this.bMapControl1.GridLineColor = Color.FromArgb(0x55, 0x99, 0x99, 0x99);


            //this.bMapControl1.GridLineColor =Color.Blue;
            var m = new AreaMarker("多边形1");
            m.lstPLatLng = new List<BMap.Core.Model.PointLatLng>() { new BMap.Core.Model.PointLatLng(113.415041, 23.1824994),
                new BMap.Core.Model.PointLatLng(113.415241, 23.1824994),
                 new BMap.Core.Model.PointLatLng(113.415241, 23.1824194),
            new BMap.Core.Model.PointLatLng(113.415041, 23.1823594)};

            layer1.AddMarker(m);
            var pm = new PointMarker("点1") { Position = new BMap.Core.Model.PointLatLng(113.415441, 23.1823494) };
            layer1.AddMarker(pm);
            bMapControl1.AddLayer(layer1);

            bMapControl1.UserDraw = new UserDrawing(bMapControl1);
            //bMapControl1.UserDraw.DrawMode = EditMode.DrawPie;
        }

        private MapLayer layer1 = new MapLayer("layer1");

        private void button1_Click(object sender, EventArgs e)
        {
            bMapControl1.RotationAngle += 30;
            button1.Text = bMapControl1.RotationAngle.ToString();
        }
    }
}
