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
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Threading;
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
            ThreadCount = 20;
        }
        protected XRequest _httpReq = new XRequest();
        /// <summary>
        /// 瓦片根目录下的url
        /// </summary>
        public string PathUrl { get; set; }
        public override bool LoadTile(Tile t)
        {
            bool isSuc = false;
            if (t!=null&&!t.IsEmpty)
            {
                try
                {
                    //Thread.Sleep(1000);
                    if (LstLayer!=null&&LstLayer.Count>0)
                    {
                        //加载多图层合成
                        List<Image> lstImg = new List<Image>();
                        foreach (var layerLoader in LstLayer)
                        {
                            var httpLoader = layerLoader as TileImgHttpLoader;
                            if (httpLoader!=null)
                            {
                                lstImg.Add(httpLoader.LoadTileImg(t));
                            }
                        }
                        t.TileImg = CombineBitMap(lstImg);
                        isSuc = true;
                    }
                    else
                    {

                        t.TileImg = LoadTileImg(t);
                        isSuc = true;
                    }
                   
                    
                }
                catch (Exception ex)
                {
                    t.ErrorMsg = "HTTP加载失败：\r\n"+ ex.Message;
                    Debug.WriteLine("从Http加载地图瓦片出错！" + ex.ToString());
                }
            
                
            }


            return isSuc;
        }

        /// <summary>
        /// 图片合成
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        protected Image CombineBitMap(List<Image> lstPic)
        {
            Image bmp = null;
            if (lstPic != null )
            {
                if (lstPic.Count==1)
                {
                    bmp = lstPic[0];
                }
                else if(lstPic.Count>1)
                {
                    var first = lstPic[0];
                    bmp = new Bitmap(first.Width, first.Height, PixelFormat.Format32bppArgb);

                    Graphics g = Graphics.FromImage(bmp);
                    g.Clear(Color.Transparent);
                    foreach (var pic in lstPic)
                    {
                        g.DrawImage(pic, new Rectangle(0, 0, pic.Width, pic.Height));
                    }
                    g.Dispose();
                }
                
            }
            return bmp;
        }

        protected virtual Image LoadTileImg(Tile t)
        {
            Image img = null;
            var url = GetFullUrl(t);
            using (var stream = _httpReq.GetStream(url))
            {
                using (var ms = _httpReq.ReadStream(stream, false))
                {
                    if (ms != null && ms.Length > 0)
                    {
                        Thread.Sleep(100);
                        img = Image.FromStream(ms);
                    }

                }

            }

            return img;
        }

        public virtual string GetFullUrl(Tile t)
        {
            return string.Format("{0}/{1}/{2}/{3}.jpg",PathUrl,t.Zoom,t.TileIndex.X,t.TileIndex.Y);
        }

        public override bool CacheTile(Tile t)
        {
            return true;
        }
    }
}
