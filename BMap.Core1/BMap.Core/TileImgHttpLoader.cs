#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/14 14:45:35 
* 文件名：TileImgHttpLoader 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using BMap.Core.Model;

namespace BMap.Core
{
    /// <summary>
    /// Http瓦片加载器
    /// </summary>
    public class TileImgHttpLoader : TileImgLoaderBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pathUrl">瓦片根目录下的url</param>
        public TileImgHttpLoader(string pathUrl)
        {
            PathUrl = pathUrl;
            ThreadCount = 10;
        }
        private XRequest _httpReq = new XRequest();
        /// <summary>
        /// 瓦片根目录下的url
        /// </summary>
        public string PathUrl { get; set; }
        public override bool LoadOneTile(Tile t)
        {
            bool isSuc = false;
            if (t!=null&&!t.IsEmpty)
            {
                try
                {
                    var url = GetFullUrl(t);
                    using (var stream = _httpReq.GetStream(url))
                    {
                        using (var ms = _httpReq.ReadStream(stream, false))
                        {
                            if (ms != null && ms.Length > 0)
                                t.ImgData = ms.GetBuffer();
                            
                        }
                            
                    }
                       
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("从Http加载地图瓦片出错！" + ex.ToString());
                }
            
                
            }


            return isSuc;
        }
        private string GetFullUrl(Tile t)
        {
            return string.Format("{0}/{1}/{2}/{3}.jpg",PathUrl,t.Zoom,t.TileIndex.X,t.TileIndex.Y);
        }
    }
}
