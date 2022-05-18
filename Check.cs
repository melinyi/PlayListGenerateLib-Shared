using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
#pragma warning disable SYSLIB0014 // 类型或成员已过时
namespace PlayListLib
{
    public class CheckClient
    {
        private static HttpWebRequest response;

        public static bool ExistanceCheck(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            url = url.Trim();
            if (url.StartsWith("\"") && url.EndsWith("\"")) url = url.Substring(1, url.Length - 2);

            try
            {

                response = WebRequest.CreateHttp(url);

                response.Timeout = 1000;
                var res = (HttpWebResponse)response.GetResponse();

                var success = res.StatusCode == HttpStatusCode.OK;

                res.Close();

                return success;
            }
            catch
            {
                try
                {
                    if (Path.IsPathRooted(url)) return File.Exists(url);
                }
                catch { }

            }
            //(Exception ex) { Console.WriteLine(ex); }
            finally
            {
                response = null;
            }

            return false;
        }
    }
}
#pragma warning restore SYSLIB0014 // 类型或成员已过时