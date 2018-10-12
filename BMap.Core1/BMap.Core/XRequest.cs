#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/14 15:44:36 
* 文件名：XRequest 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
namespace BMap.Core
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    public class XRequest
    {
        private string _contentType;
        private CookieContainer _cookieContainer;
        private Encoding _encoding;
        private string _referer;
        private int _timeout;
        private string _userAgent;


        public XRequest()
        {
            this._userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; InfoPath.2)";
            this._timeout = 0xea60;
            this._encoding = Encoding.UTF8;
            this._contentType = "application/x-www-form-urlencoded";
            ServicePointManager.Expect100Continue = false;
            this._cookieContainer = new CookieContainer();
            this.TimeOut = 120 * 1000;
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
        }

        public XRequest(CookieContainer ccontainer)
        {
            this._userAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; InfoPath.2)";
            this._timeout = 0xea60;
            this._encoding = Encoding.UTF8;
            this._contentType = "application/x-www-form-urlencoded";
            this._cookieContainer = ccontainer;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(this.CheckValidationResult);
        }

        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public string GetHtml(string url)
        {
            return this.GetHtml(url, this._encoding);
        }

        public string GetHtml(string url, Encoding enc)
        {
            //try
            //{
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.CookieContainer = this._cookieContainer;
            request.Timeout = this._timeout;
            request.ReadWriteTimeout = this._timeout;
            request.UserAgent = this._userAgent;
            request.Referer = this._referer;
            //request.Connection = "Close";
            request.KeepAlive = true;
            request.ProtocolVersion = HttpVersion.Version10;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Cookies = this._cookieContainer.GetCookies(request.RequestUri);



            StreamReader reader = new StreamReader(response.GetResponseStream(), enc);
            var str = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return str;
            //}
            //catch(Exception ex)
            //{
            //    return null;
            //}
        }

        public Stream GetStream(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = this._cookieContainer;
            request.Timeout = this._timeout;
            request.ReadWriteTimeout = this._timeout;
            request.UserAgent = this._userAgent;
            request.Referer = this._referer;
            request.KeepAlive = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();

        }

        /// <summary>
        /// 读取流到内存中
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="SeekOriginBegin"></param>
        /// <returns></returns>
        public MemoryStream ReadStream(Stream inputStream, bool SeekOriginBegin)
        {
            const int readSize = 32 * 1024;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();
            {
                int count = 0;
                while ((count = inputStream.Read(buffer, 0, readSize)) > 0)
                {
                    ms.Write(buffer, 0, count);
                }
            }
            buffer = null;
            if (SeekOriginBegin)
            {
                inputStream.Seek(0, SeekOrigin.Begin);
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        //public string postHtml(string url, string args)
        //{
        //    return this.postHtml(url, args, this._encoding);
        //}

        //public string postHtml(string url, string args, Encoding enc)
        //{
        //    string resStr = null;
        //    if (ValidationHelper.IsURL(url) && !string.IsNullOrEmpty(args))
        //    {
        //        new ASCIIEncoding();
        //        byte[] bytes = enc.GetBytes(args);
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //        request.KeepAlive = false;
        //        request.ProtocolVersion = HttpVersion.Version10;
        //        request.CookieContainer = this._cookieContainer;

        //        request.Timeout = this._timeout;
        //        request.ReadWriteTimeout = this._timeout;
        //        request.UserAgent = this._userAgent;
        //        request.Referer = this._referer;
        //        request.ContentType = this._contentType;
        //        request.Headers.Add("Cache-control: no-cache");
        //        request.Headers.Add("Accept-Language: zh-cn");
        //        request.Accept = "*/*";
        //        request.ContentLength = bytes.Length;
        //        request.Method = "POST";

        //        //request.Connection = "keep-alive";
        //        using (Stream stream = request.GetRequestStream())
        //        {
        //            stream.Write(bytes, 0, bytes.Length);
        //        }
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        StreamReader reader = new StreamReader(response.GetResponseStream(), enc);
        //        resStr = reader.ReadToEnd();
        //    }
        //    return resStr;
        //}

        public string ContentType
        {
            set
            {
                this._contentType = value;
            }
        }

        public string Referer
        {
            get
            {
                return this._referer;
            }
            set
            {
                this._referer = value;
            }
        }

        public string UserAgent
        {
            get
            {
                return this._userAgent;
            }
            set
            {
                this._userAgent = value;
            }
        }
        /// <summary>
        /// 超时 毫秒
        /// </summary>
        public int TimeOut
        {
            get
            {
                return this._timeout;
            }
            set
            {
                this._timeout = value;
            }
        }

        public CookieContainer Cookie
        {
            get
            {
                return this._cookieContainer;
            }
            set
            {
                this._cookieContainer = value;
            }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return this._encoding;
            }
            set
            {
                this._encoding = value;
            }
        }

    }
}
