using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace Interceptor
{
    public class RequestDebugInterceptor : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string headers = string.Empty;
            foreach (string hKey in context.Request.Headers.AllKeys)
            {
                headers += hKey + " : " + context.Request.Headers.Get(hKey);
                headers += "<br />";
            }

            context.Response.Clear();
            context.Response.Write(headers);
            context.Response.Flush();

        }
    }
}
