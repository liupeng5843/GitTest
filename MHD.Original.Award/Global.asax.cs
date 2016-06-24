using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using SimpleComm.Files;

namespace MHD.Original.Award
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            AuthorizeRequest += MvcApplication_AuthorizeRequest;
            BeginRequest += MvcApplication_BeginRequest;
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void MvcApplication_BeginRequest(object sender, EventArgs e)
        {
            //var httpApplication = (HttpApplication)sender;


            //string session_param = "ASP.NET_SessionId";
            //string cookie_original_param = "original";

            //if (httpApplication.Request.Cookies[session_param] != null)
            //{
            //    UpdateCookie(httpApplication, session_param, httpApplication.Request.Form[session_param]);
            //}
            //else if (httpApplication.Request.QueryString[session_param] != null)
            //{
            //    UpdateCookie(httpApplication, session_param, httpApplication.Request.QueryString[session_param]);
            //}

            //if (httpApplication.Request.Cookies[cookie_original_param] != null)
            //{
            //    UpdateCookie(httpApplication, cookie_original_param, httpApplication.Request.Form[cookie_original_param]);
            //}
            //else if (httpApplication.Request.QueryString[cookie_original_param] != null)
            //{
            //    UpdateCookie(httpApplication, cookie_original_param, httpApplication.Request.QueryString[cookie_original_param]);
            //}

        }
        void MvcApplication_AuthorizeRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var formsIdetity = HttpContext.Current.User.Identity as FormsIdentity;

                OriginalIdentity originalIdentity = new OriginalIdentity(formsIdetity.Ticket.UserData)
                {
                    AuthenticationType = formsIdetity.AuthenticationType,
                    IsAuthenticated = formsIdetity.IsAuthenticated,
                    Name = formsIdetity.Name,
                    Ticket = formsIdetity.Ticket,
                };
                OriginalPrincipal originalPrincipal = new OriginalPrincipal() { Identity = originalIdentity, };
                HttpContext.Current.User = originalPrincipal;
            }
        }

        private void UpdateCookie(HttpApplication application, string cookie_name, string cookie_value)
        {
            var httpApplication = (HttpApplication)application;

            HttpCookie cookie = httpApplication.Request.Cookies.Get(cookie_name);
            if (null == cookie)
            {
                cookie = new HttpCookie(cookie_name);
            }
            cookie.Value = cookie_value;
            httpApplication.Request.Cookies.Set(cookie);
        }
    }
}