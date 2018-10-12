#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/14 11:12:39 
* 文件名：Singleton 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/ 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace BMap.Core
{
    /// <summary>
    /// 单例泛型基类
    /// </summary>
    public class SingletonBase<T> where T:new()
    {
        #region 单例
        private static object locker = new object();
        private static T _singleton;

        /// <summary>
        /// 单例
        /// </summary>
        public static T Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    lock (locker)
                    {
                        if (_singleton == null)
                        {
                            _singleton = new T();
                        }
                    }
                }
                return _singleton;
            }
        }

        protected SingletonBase()
        {
            if (_singleton != null)
            {
                throw (new System.Exception("已存在单例，不能再新建实例"));
            }
        }


        #endregion
    }
}
