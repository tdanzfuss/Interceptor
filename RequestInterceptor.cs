using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace Interceptor
{
    public class RequestInterceptor : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string headerKey = ConfigurationManager.AppSettings["AuthHeader"];
                string landingPage = ConfigurationManager.AppSettings["LandingPage"];
                string errorPage = ConfigurationManager.AppSettings["ErrorPage"];
                string headerText = "{0} {1}";

                string headerValue = context.Request.Headers.Get(headerKey);

                System.Collections.Specialized.NameValueCollection nvc = context.Request.Form;
                string joRedirect = "";
                string dataType = "";

                joRedirect = context.Request["joRedirect"];
                // If JWT is in header, then replace
                joRedirect = setJWTToken(joRedirect, headerValue);

                // If the POST message specifies a new redirect path then use that instead of default configured path
                if (!String.IsNullOrEmpty(context.Request["redirectPath"]))
                {
                    landingPage = context.Request["redirectPath"];
                }
                context.Response.Write(string.Format(generateScriptTags(), setLocalStorage("actionType", joRedirect), getRedirectionUrl(landingPage)));

            }
            catch (HttpRequestValidationException validatorEx)
            {
                //Log validatorEx.Message
            }
            catch (HttpParseException parserEx)
            {
                //Log parserEx.Message
            }
            catch (Exception ex)
            {
                //Log ex.Message
            }
        }

        private string setJWTToken(string joRedirect, string headerValue)
        {
            if (!String.IsNullOrEmpty(headerValue))
            {
                dynamic json = JObject.Parse(joRedirect);
                json.jwt = headerValue;
                joRedirect = ((JObject)json).ToString();
            }
            
            return joRedirect;
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
            return string.Format("window.location = \"{0}\";", landingPage);
        }

        private string setLocalStorage(string headerKey, string headerValue)
        {
            return string.Format("localStorage.setItem(\'{0}\', \'{1}\');", headerKey, headerValue);
        }
    }
}
