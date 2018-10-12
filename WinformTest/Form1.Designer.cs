namespace WinformTest
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            BMap.Core.Model.PointLatLng pointLatLng1 = new BMap.Core.Model.PointLatLng();
            this.bMapControl1 = new BMap.WinForm.BMapControl();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bMapControl1
            // 
            this.bMapControl1.BackColor = System.Drawing.SystemColors.Menu;
            this.bMapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bMapControl1.GridLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(255)))));
            this.bMapControl1.GridLineWidth = 0.5F;
            this.bMapControl1.IsShowGrid = true;
            this.bMapControl1.IsShowMapMsg = false;
            this.bMapControl1.IsShowMapScale = true;
            this.bMapControl1.IsShowRenderMsg = false;
            this.bMapControl1.Location = new System.Drawing.Point(0, 0);
            pointLatLng1.LatY = 23.1D;
            pointLatLng1.LngX = 113.1D;
            this.bMapControl1.MapCenter = pointLatLng1;
            this.bMapControl1.Name = "bMapControl1";
            this.bMapControl1.RotationAngle = 0F;
            this.bMapControl1.Size = new System.Drawing.Size(800, 450);
            this.bMapControl1.TabIndex = 0;
            this.bMapControl1.TileType = BMap.Core.TileType.WGS84;
            this.bMapControl1.UserDraw = null;
            this.bMapControl1.Zoom = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 363);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.bMapControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private BMap.WinForm.BMapControl bMapControl1;
        private System.Windows.Forms.Button button1;
    }
}

