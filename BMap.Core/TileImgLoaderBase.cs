#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/14 14:30:16 
* 文件名：TileImgLoader 
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

namespace BMap.Core
{
    /// <summary>
    /// 瓦片加载器抽象类
    /// </summary>
    public abstract class TileImgLoaderBase
    {
        /// <summary>
        /// 瓦片集合栈
        /// </summary>
        protected Queue<Tile> _queTile = new Queue<Tile>();
        protected Thread[] _threads;
        protected AutoResetEvent autoResetEvent = new AutoResetEvent(true);
        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadCount { get; set; } = 5;
        /// <summary>
        /// 是否启动了
        /// </summary>
        public bool IsStarted { get; set; }
        /// <summary>
        /// 优先级（存在多个加载器时按优先级对图片加载直到加载成功，数值越大优先级越大）
        /// </summary>
        public uint Priority { get; set; }
        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; } = true;
        /// <summary>
        /// 多图层
        /// </summary>
        public List<TileImgLoaderBase> LstLayer { get; set; }
        /// <summary>
        /// 加载瓦片完成事件
        /// </summary>
        public event Action<TileImgLoaderBase, Tile> LoadCompleted;
        /// <summary>
        /// 加载失败事件
        /// </summary>
        public event Action<TileImgLoaderBase,Tile> LoadFailed;


        
        /// <summary>
        /// 开始加载器
        /// </summary>
        public void Start()
        {
            try
            {
                _threads = new Thread[ThreadCount];
                IsStarted = true;
                for (int i = 0; i < _threads.Length; i++)
                {
                    var t = _threads[i] = new Thread(ExecuteLoad);
                    t.IsBackground = true;
                    t.Name = "加载器线程" + i.ToString();
                    t.Start();
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("开启加载器错误!" + ex.ToString());
            }
            
        }
        /// <summary>
        /// 停止加载器
        /// </summary>
        public void Stop()
        {
            IsStarted = false;
        }
        /// <summary>
        /// 执行多线程加载
        /// </summary>
        public void ExecuteLoad()
        {
            while (IsStarted)
            {
                Tile t = Dequeue();
                if (t!=null&&!t.IsEmpty)
                {
                    var dt = DateTime.Now;
                    if (LoadTile(t))
                    {
                        //Debug.WriteLine(string.Format("{2}  【{0}】加载器加载图片【{1}】成功！耗时（ms）:{3}", this, t, DateTime.Now.ToString("HH:mm:ss.fff"), (DateTime.Now - dt).TotalMilliseconds));
                        //加载成功
                        if (LoadCompleted != null)
                        {
                            LoadCompleted(this,t);
                        }
                        //Debug.WriteLine(string.Format("{2}  【{0}】加载器加载图片【{1}】成功！耗时（ms）:{3}", this, t,DateTime.Now.ToString("HH:mm:ss.fff"),(DateTime.Now-dt).TotalMilliseconds));
                    }
                    else
                    {
                        //加载失败
                        if (LoadFailed!=null)
                        {
                            LoadFailed(this,t);
                        }
                        //Debug.WriteLine(string.Format("【{0}】加载器加载图片【{1}】失败！", this, t));
                    }
                }
                else
                {
                    autoResetEvent.WaitOne(2000);
                    //Thread.Sleep(111);
                }
            }
        }
        /// <summary>
        /// 加载单张瓦片，由派生类实现具体逻辑
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract bool LoadTile(Tile t);

        /// <summary>
        /// 缓存瓦片
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public abstract bool CacheTile(Tile t);
     


        /// <summary>
        /// 把要加载的瓦片加入到加载器中等待加载。
        /// </summary>
        /// <param name="t"></param>
        public void Add(Tile t)
        {
            if (t!=null&&!t.IsEmpty)
            {
                lock (_queTile)
                {
                    _queTile.Enqueue(t);
                    //Debug.WriteLine(string.Format("{2}  【{0}】加载器添加图片【{1}】", this, t, DateTime.Now.ToString("HH:mm:ss.fff")));
                    autoResetEvent.Set();
                }
            }
            
        }
        /// <summary>
        /// 从栈钟弹出一张瓦片
        /// </summary>
        /// <returns></returns>
        protected Tile Dequeue()
        {
            Tile t = null;
            lock (_queTile)
            {
                if (_queTile.Count>0)
                {
                    t = _queTile.Dequeue();
                }
            }
            return t;
        }
    }
}
