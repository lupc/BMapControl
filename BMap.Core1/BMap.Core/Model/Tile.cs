#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/13 9:50:22 
* 文件名：Tile 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/ 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace BMap.Core.Model
{
    public class Tile
    {
        private ReaderWriterLock _fastLock = new ReaderWriterLock();

        public static readonly Tile Empty=new Tile();
        
        public Tile()
        {
            TileIndex = new PointInt64();
        }
        public Tile(PointInt64 tileIndex,int zoom)
        {
            TileIndex = tileIndex;
            Zoom = zoom;
        }
        public Tile(long x,long y,int zoom)
        {
            TileIndex = new PointInt64(x,y);
            Zoom = zoom;
        }

        /// <summary>
        /// 瓦片索引
        /// </summary>
        public PointInt64 TileIndex { get; set; }

        /// <summary>
        /// 瓦片X索引
        /// </summary>
        //public long X { get; set; }
        /// <summary>
        /// 瓦片Y索引
        /// </summary>
        //public long Y { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int Zoom { get; set; }

        private byte[] _imgData;
        /// <summary>
        /// 瓦片数据
        /// </summary>
        public byte[] ImgData
        {
            get {
                //_fastLock.AcquireReaderLock(100);
                //try
                //{
                    return _imgData;
                //}
                //finally
                //{
                //    _fastLock.ReleaseReaderLock();
                //}
            }
            set
            {
                //_fastLock.AcquireWriterLock(100);
                //try
                //{
                    _imgData = value;
                //}
                //finally
                //{
                //    _fastLock.ReleaseWriterLock();
                //}
            }
        }

       
        

        //public Image MyProperty { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool b = false;
            Tile t = obj as Tile;
            if (t!=null)
            {
                b = t.TileIndex.Equals(this.TileIndex) && Zoom == t.Zoom;
            }
            return b;
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.Equals(Empty);
            }
        }

        public override string ToString()
        {
            return string.Format("X:{0},Y:{1},Zoom:{2}",TileIndex.X, TileIndex.Y, Zoom);
        }
    }
}
