using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        //读线程锁
        static ReaderWriterLock m_ReadLock = new ReaderWriterLock();
        static ReaderWriterLock m_WriteLock = new ReaderWriterLock();
        //资源
        static int m_nResource = 0;

        //读取资源线程
        static void ReadProc(object o)
        {
            for (int i = 0; i < 5; i++)
            {
                ReadResource(5000);
                Thread.Sleep(500);
            }
        }

        //写入资源线程
        static void WriteProc()
        {
            for (int i = 0; i < 5; i++)
            {
                WriteResource(5000);
                Thread.Sleep(500);
            }
        }

        //读取资源
        private static bool ReadResource(int timeout)
        {
            try
            {
                m_ReadLock.AcquireReaderLock(timeout);
                try
                {
                    Console.WriteLine("成功获得读线程锁, 资源值:{0}", m_nResource);
                }
                finally
                {
                    m_ReadLock.ReleaseReaderLock();
                }
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("获取锁超时:{0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取锁异常:{0}", ex.Message);
            }
            return true;
        }

        //写入资源
        private static bool WriteResource(int timeout)
        {
            try
            {
                m_ReadLock.AcquireWriterLock(timeout);
                try
                {
                    Console.WriteLine("成功获得写线程锁, 资源值:{0}", m_nResource);
                    m_nResource++;
                    Console.WriteLine("写入资源 {0}", m_nResource);
                }
                finally
                {
                    m_ReadLock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine("获取锁超时:{0}", ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取锁异常:{0}", ex.Message);
            }
            return true;
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(ReadProc);
            }
            //Thread t1 = new Thread(new ThreadStart(ReadProc));
            Thread t2 = new Thread(new ThreadStart(WriteProc));
            //t1.Start();
            t2.Start();

            Thread.Sleep(5000);
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }
    }
}
