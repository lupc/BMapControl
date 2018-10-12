#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/10/11 16:07:57 
* 文件名：Distance 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMap.Core.Model
{
    /// <summary>
    /// 距离
    /// </summary>
    public class Distance
    {
        /// <summary>
        /// 表示距离的对象
        /// </summary>
        /// <param name="m">米</param>
        public Distance(double m)
        {
            Meter = m;
        }

        public double Meter { get; set; }

        public override string ToString()
        {
            string str = Meter.ToString("0.00") +"m";
            if(Meter >= 1000)
            {
                str = (Meter / 1000).ToString("0.00") + "km";
            }
            return str;
        }

    }
}
