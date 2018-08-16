using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

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

                //if (!string.IsNullOrEmpty(nvc["joRedirect"]))
                //{
                //    joRedirect = nvc["joRedirect"];
                //}

                context.Response.Write(string.Format(generateScriptTags(), setLocalStorage("actionType", joRedirect), getRedirectionUrl(landingPage)));


                //if (!string.IsNullOrWhiteSpace(headerValue))
                //{
                //    headerValue = string.Format(headerText, headerKey, headerValue);
                //    context.Response.Write(string.Format(generateScriptTags(), setLocalStorage(headerKey, headerValue), getRedirectionUrl(landingPage)));
                //}
                //else
                //{
                //    context.Response.Write(string.Format(generateScriptTags(), string.Empty, getRedirectionUrl(errorPage)));
                   
                //}
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

        private string getAuthToken(HttpContext context)
        {
            // if the Authtoken is in the header, then get it
            if ((context.Request.Headers["Authorization"] != null) && (context.Request.Headers["Authorization"].Length > 0))
            {
                return context.Request.Headers["Authorization"].Replace("Bearer ", "");
            }
            else
                return context.Request["authenticationToken"];
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
