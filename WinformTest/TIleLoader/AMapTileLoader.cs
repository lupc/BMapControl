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
    public class AMapTileLoader : BMap.Core.TileImgHttpLoader
    {
        public AMapTileLoader(string pathUrl) : base(pathUrl)
        {
        }

        public override string GetFullUrl(Tile t)
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            int serverNo = rd.Next(1, 4);
            var num = (t.TileIndex.X + t.TileIndex.Y) % 4 + 1;
            //string url = string.Format(UrlFormat, num, pos.X, pos.Y, zoom);
            string url = string.Format(UrlFormat, t.TileIndex.X, t.TileIndex.Y, t.Zoom, serverNo);
            return url;
        }

        static readonly string UrlFormat = "http://webrd{3:d2}.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=7&x={0}&y={1}&z={2}";
    }
}
