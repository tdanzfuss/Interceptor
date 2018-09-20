using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Interceptor
{
    public class RequestEchoInterceptor : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }   

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Headers != null && context.Request.Headers.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in context.Request.Headers.AllKeys)
                {
                    sb.Append(key).Append(" : ").Append(context.Request.Headers[key]).AppendLine();
                }
                context.Response.Clear();
                context.Response.Write(sb.ToString());
                context.Response.Flush();
            }
        }
    }
}
