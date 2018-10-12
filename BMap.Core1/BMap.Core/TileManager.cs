#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/14 11:10:57 
* 文件名：TileManager 
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
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Linq;

namespace BMap.Core
{
    /// <summary>
    /// 瓦片管理器
    /// </summary>
    public class TileManager:SingletonBase<TileManager>
    {
        public TileManager()
        {

        }

        public ReaderWriterLock TilesFastLock = new ReaderWriterLock();
        private int _lockTimeOut = 2000;
        
        private List<TileImgLoaderBase> _tileLoaders;
        /// <summary>
        /// 瓦片缓存字典
        /// </summary>
        public Dictionary<int, Dictionary<PointInt64, Tile>> dicTile { get; set; } = new Dictionary<int, Dictionary<PointInt64, Tile>>();
        public List<TileImgLoaderBase> TileLoaders
        {
            get { return _tileLoaders; }
            set {
                _tileLoaders = value;
                if (_tileLoaders!=null&&_tileLoaders.Count>0)
                {
                    _tileLoaders.Sort((t1, t2) => t1.Priority.CompareTo(t2.Priority));//按权重升序
                    foreach (var loader in _tileLoaders)
                    {
                        if (loader!=null)
                        {
                            loader.Start();
                            loader.LoadCompleted += Loader_LoadCompleted; ;
                            loader.LoadFailed += Loader_LoadFailed; ;
                        }
                       
                    }
                }
            }
        }

        /// <summary>
        /// 瓦片添加到管理器中触发
        /// </summary>
        public event Action<Tile> TileAdded;

       

        private void Loader_LoadFailed(TileImgLoaderBase arg1, Tile arg2)
        {
            //前一个加载器加载失败，用下一个加载器加载
            var i = _tileLoaders.IndexOf(arg1)+1;
            if (i< _tileLoaders.Count)
            {
                _tileLoaders[i].Add(arg2);
            }
        }

        private void Loader_LoadCompleted(TileImgLoaderBase arg1, Tile arg2)
        {
            AddTile(arg2);
        }

        


        /// <summary>
        /// 从管理器中获取瓦片（不加锁）
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public Tile GetTileNoLock(int zoom,PointInt64 tileIndex,bool ifNotExitsLoad=true)
        {
            Tile t = null;
            Dictionary<PointInt64, Tile> dic1;
            if(dicTile.TryGetValue(zoom,out dic1))
            {
                dic1.TryGetValue(tileIndex, out t);
            }
            else
            {
                dic1 = new Dictionary<PointInt64, Tile>();
                dicTile.Add(zoom, dic1);
            }
            if (t==null)
            {
                //瓦片不存在则添加到加载器中
                t = new Tile(tileIndex, zoom);
                if (ifNotExitsLoad)
                {
                    LoadTile(t);
                }
            }
            return t;
        }
        /// <summary>
        /// 从管理器中获取瓦片（支持多线程并发获取）
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public Tile GetTile(int zoom,PointInt64 tileIndex,bool ifNotExitsLoad = true)
        {
            Tile t = new Tile(tileIndex, zoom); ;
            TilesFastLock.AcquireReaderLock(_lockTimeOut);
            try
            {
                t = GetTileNoLock(zoom, tileIndex,ifNotExitsLoad);
            }
            finally
            {
                TilesFastLock.ReleaseReaderLock();
            }
            return t;
        }

        /// <summary>
        /// 添加瓦片到管理器
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ifExitsUpdate">如果存在则更新</param>
        public void AddTile(Tile t,bool ifExitsUpdate=true)
        {
            TilesFastLock.AcquireWriterLock(_lockTimeOut);
            var dt = DateTime.Now;
            try
            {
                if (t!=null)
                {
                    Dictionary<PointInt64, Tile> dic1;
                    if (dicTile.ContainsKey(t.Zoom))
                    {
                        dic1 = dicTile[t.Zoom];
                    }
                    else
                    {
                        dic1 = new Dictionary<PointInt64, Tile>();
                        dicTile.Add(t.Zoom, dic1);
                    }
                    if (!dic1.ContainsKey(t.TileIndex))
                    {
                        dic1.Add(t.TileIndex, t);
                    }
                    else if(ifExitsUpdate)
                    {
                        dic1[t.TileIndex] = t;
                    }

                }
                //Debug.WriteLine(string.Format("{0}  瓦片已经【{1}】添加到管理器！耗时（ms）:{2}",  DateTime.Now.ToString("HH:mm:ss.fff"), t,(DateTime.Now - dt).TotalMilliseconds));

            }
            finally
            {
                TilesFastLock.ReleaseWriterLock();
            }
            if (TileAdded!=null)
            {
                TileAdded(t);
            }
        }

        /// <summary>
        /// 加载瓦片
        /// </summary>
        /// <param name="t"></param>
        public void LoadTile(Tile t)
        {
            if (TileLoaders!=null&&TileLoaders.Count>0)
            {
                var tileLoader1 = TileLoaders[0];
                tileLoader1.Add(t);
            }
            else
            {
                Debug.WriteLine("没有找到瓦片加载器！");
            }
        }
    }
}
