#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/10/11 14:45:22 
* 文件名：GoogleChinaTileLoader 
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformTest
{
    public class GoogleChinaTileLoader : BMap.Core.TileImgHttpLoader
    {
        public GoogleChinaTileLoader() : base(string.Empty)
        {
        }
        

        public override string GetFullUrl(Tile t)
        {
            string sec1 = string.Empty; // after &x=...
            string sec2 = string.Empty; // after &zoom=...
            GetSecureWords(t.TileIndex, out sec1, out sec2);

            return string.Format(UrlFormat, UrlFormatServer, GetServerNum(t.TileIndex, 4), UrlFormatRequest, lyrs, t.TileIndex.X, sec1, t.TileIndex.Y, t.Zoom, sec2, ServerChina);
        }
        protected static int GetServerNum(PointInt64 pos, int max)
        {
            return (int)(pos.X + 2 * pos.Y) % max;
        }
        protected void GetSecureWords(PointInt64 pos, out string sec1, out string sec2)
        {
            sec1 = string.Empty; // after &x=...
            sec2 = string.Empty; // after &zoom=...
            int seclen = (int)((pos.X * 3) + pos.Y) % 8;
            sec2 = SecureWord.Substring(0, seclen);
            if (pos.Y >= 10000 && pos.Y < 100000)
            {
                sec1 = Sec1;
            }
        }

        protected string lyrs = "m@218";
        
        public string SecureWord = "Galileo";
        static readonly string ServerChina = "google.cn";
        static readonly string Sec1 = "&s=";
        static readonly string UrlFormatServer = "mt";
        static readonly string UrlFormatRequest = "vt";
        static readonly string UrlFormat = "http://{0}{1}.{9}/{2}/lyrs={3}&gl=cn&x={4}{5}&y={6}&z={7}&s={8}";
    }

    
}
