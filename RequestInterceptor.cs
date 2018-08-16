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
            string headerKey = ConfigurationManager.AppSettings["AuthHeader"];
            string landingPage = ConfigurationManager.AppSettings["LandingPage"];
            string errorPage = ConfigurationManager.AppSettings["ErrorPage"];

            try
            {

                string headerText = "{0} {1}";

                string headerValue = context.Request.Headers.Get(headerKey);

                System.Collections.Specialized.NameValueCollection nvc = context.Request.Form;
                string joRedirect = "";
                string dataType = "";
                string redirectPath = string.Empty;

                // Data is in a FORM post
                if (context.Request.Form.Count != 0)
                {
                    joRedirect = context.Request["joRedirect"];
                    redirectPath = context.Request["redirectPath"];
                } else
                // Data is raw in the body
                {
                    string rawBody = new System.IO.StreamReader(context.Request.InputStream).ReadToEnd();
                    dynamic json = JObject.Parse(rawBody);
                    joRedirect = Convert.ToString(json.joRedirect);
                    redirectPath = Convert.ToString(json.redirectPath);
                }
                
                // If JWT is in header, then replace
                joRedirect = setJWTToken(joRedirect, headerValue);

                // If the POST message specifies a new redirect path then use that instead of default configured path
                if (!String.IsNullOrEmpty(redirectPath))
                {
                    landingPage = redirectPath;
                }
                context.Response.Write(string.Format(generateScriptTags(), setLocalStorage("actionType", joRedirect), getRedirectionUrl(landingPage)));
            }
            catch (Exception ex)
            {
                // Redirect to the configured error page
                context.Response.Write(string.Format(generateScriptTags(), getErrorMessageContext(ex), getRedirectionUrl(errorPage)));
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

        private string getErrorMessageContext(Exception ex) {
            return setLocalStorage("errorMessage", ex.ToString());
        }


        private string setLocalStorage(string headerKey, string headerValue)
        {
            return string.Format("localStorage.setItem(\'{0}\', \'{1}\');", headerKey, headerValue);
        }
    }
}
