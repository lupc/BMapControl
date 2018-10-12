#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/14 14:46:42 
* 文件名：TileImgFileLoader 
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
using System.IO;
using System.Text;
using BMap.Core.Model;

namespace BMap.Core
{
    public class TileImgFileLoader : TileImgLoaderBase
    {
        public TileImgFileLoader(string tilePath)
        {
            TilePath = tilePath;
            ThreadCount = 5;
        }
        public string TilePath { get; set; }
        private string GetImgPath(Tile t)
        {
            return string.Format("{0}\\{1}\\{2}\\{3}.jpg", TilePath, t.Zoom, t.TileIndex.X, t.TileIndex.Y);

        }
        public override bool LoadOneTile(Tile t)
        {
            bool issuc = false;
            if (t != null && !t.IsEmpty)
            {
                try
                {
                    //DateTime dt = DateTime.Now;
                    var imgPath = GetImgPath(t);
                    if (File.Exists(imgPath))
                    {
                        t.ImgData = File.ReadAllBytes(imgPath);
                        issuc = true;
                    }
                    //Debug.WriteLine("加载文件耗时：" + (DateTime.Now-dt).Milliseconds.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("从文件加载地图瓦片出错！" + ex.ToString());
                }
                
            }
            

            return issuc;
        }
    }
}
