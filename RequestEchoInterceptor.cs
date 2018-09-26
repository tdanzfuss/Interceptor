﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

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
            string authToken = context.Request.Headers.Get("Authorization"); // JWT must be injected by Webseal
            if ((authToken == null) || (authToken.Length <= 0))
            {
                authToken = "Im not a  JWT...";
            }

            dynamic json = JObject.Parse("{}");
            json.jwt = authToken; //Set the JWT received from Webseal
            json.dataType = "bpmRedirect"; // determines what to do next in CR1
            json.partyId = getECNNumber(context.Request.Path);
            //if ((authToken != null) && (authToken.Length > 0))
            //{            
                context.Response.Clear();
                context.Response.Write(context.Request.Path + " <b>REDIRECT WITH JWT: </b>" + authToken);
                context.Response.Write(string.Format(generateScriptTags(),
                    setLocalStorage("actionType", ((JObject)json).ToString (Newtonsoft.Json.Formatting.None)),
                    getRedirectionUrl(context.Request.Url.AbsolutePath)));
                context.Response.Flush();
            //}
            /*if (context.Request.Headers != null && context.Request.Headers.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in context.Request.Headers.AllKeys)
                {
                    sb.Append(key).Append(" : ").Append(context.Request.Headers[key]).AppendLine();
                }
                context.Response.Clear();
                context.Response.Write(sb.ToString());
                context.Response.Flush();
            }*/
        }

        private string getECNNumber(string urlPath)
        {
            try
            {
                string searchPath = "application/";
                int startIdx = urlPath.LastIndexOf(searchPath);
                int ecnStartIdx = startIdx + searchPath.Length;
                int ecnEndIdx = urlPath.IndexOf('/', ecnStartIdx + 1);

                return urlPath.Substring(ecnStartIdx, (ecnEndIdx - ecnStartIdx));
            }
            catch (Exception ex)
            {
                return "0";
            }

        }

        private string generateScriptTags()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("{0}");
            sb.AppendLine("{1}");
            sb.AppendLine("</script>");
            return sb.ToString();
        }

        private string getRedirectionUrl(string landingPage)
        {
            // strip the redirect URL out of the incomming URL
            return string.Format("window.location = \"{0}\";", landingPage.Replace("/echo.html",""));
        }

        private string setLocalStorage(string headerKey, string headerValue)
        {
            return string.Format("localStorage.setItem(\'{0}\', \'{1}\');", headerKey, headerValue);
        }
    }
}
