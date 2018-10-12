#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/29 17:19:29 
* 文件名：EditData 
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

namespace BMap.WinForm
{
    public class EditData
    {
        /// <summary>
        /// 操作过程坐标点
        /// </summary>
        public List<PointLatLng> LstPoint { get; set; } = new List<PointLatLng>();
        /// <summary>
        /// 是否画图完成
        /// </summary>
        public bool IsCompleted { get; set; }
        /// <summary>
        /// 总距离 米
        /// </summary>
        public double Dist { get; set; }
        /// <summary>
        /// 重绘区域
        /// </summary>
        public Region RefreshRegion { get; set; }

        /// <summary>
        /// 重置
        /// </summary>
        public void ReSet()
        {
            LstPoint.Clear();
            IsCompleted = false;
            Dist = 0;
        }
    }
}
