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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using BMap.Core.Model;

namespace BMap.Core
{
    public class TileImgFileLoader : TileImgLoaderBase
    {
        public TileImgFileLoader(string tilePath)
        {
            TilePath = tilePath;
            ThreadCount = 5;

            //ThreadPool.QueueUserWorkItem(SaveTileByThread);
        }
        public string TilePath { get; set; }
        protected virtual string GetImgPath(Tile t)
        {
            return string.Format("{0}\\{1}\\{2}\\{3}.jpg", TilePath, t.Zoom, t.TileIndex.X, t.TileIndex.Y);

        }
        public override bool LoadTile(Tile t)
        {
            bool issuc = false;
            if (t != null && !t.IsEmpty)
            {
                _tileSaveLock.AcquireReaderLock(2000);
                try
                {
                    //DateTime dt = DateTime.Now;
                    var imgPath = GetImgPath(t);
                    if (File.Exists(imgPath))
                    {
                        
                        t.TileImg = Image.FromFile(imgPath);

                        issuc = true;
                    }
                    //Debug.WriteLine("加载文件耗时：" + (DateTime.Now-dt).Milliseconds.ToString());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("从文件加载地图瓦片出错！" + ex.ToString());
                }
                finally
                {
                    _tileSaveLock.ReleaseReaderLock();
                }
                
            }
            

            return issuc;
        }

        
        private ReaderWriterLock _tileSaveLock = new ReaderWriterLock();
        public override bool CacheTile(Tile t)
        {
            bool issuc = false;
            if (IsCache)
            {
                var path = GetImgPath(t);
                FileInfo fi = new FileInfo(path);
                //_tileSaveLock.AcquireWriterLock(1000);
                try
                {
                    if (!Directory.Exists(fi.DirectoryName))
                    {
                        Directory.CreateDirectory(fi.DirectoryName);
                    }
                    if (!fi.Exists)
                    {
                        using (Bitmap bmp = new Bitmap(t.TileImg))
                        {
                            bmp.Save(path);
                            issuc = true;
                        }
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    //_tileSaveLock.ReleaseWriterLock();
                }
               
            }
            return issuc;
           
        }

       
    }
}
