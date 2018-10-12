#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/10/11 9:24:59 
* 文件名：AMapTileLoader 
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
using BMap.Core.Model;

namespace WinformTest
{
    /// <summary>
    /// google 中国卫星地图瓦片加载器（含基本道路信息）
    /// </summary>
    public class GoogleChinaHybridTileLoader : GoogleChinaTileLoader
    {
        public GoogleChinaHybridTileLoader()
        {
            this.lyrs = "h@218";
            this.LstLayer = new List<BMap.Core.TileImgLoaderBase>() { new GoogleChinaSatelliteTileLoader(), this };

        }

       
    }
}
