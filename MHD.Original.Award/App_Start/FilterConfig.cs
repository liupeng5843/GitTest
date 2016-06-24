using System;
using System.Web;
using System.Web.Mvc;

namespace MHD.Original.Award
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomExceptionAttribute());
        }

        public class CustomExceptionAttribute : FilterAttribute, IExceptionFilter
        {
            public void OnException(ExceptionContext filterContext)
            {
                if (filterContext.ExceptionHandled == true)
                {
                    return;
                }
                //filterContext.ExceptionHandled = true;
                Exception exception = filterContext.Exception;

                HttpException httpException = new HttpException(null, exception);

                if (httpException != null && (httpException.GetHttpCode() == 400 || httpException.GetHttpCode() == 404))
                {
                    filterContext.HttpContext.Response.StatusCode = 404;
                    filterContext.HttpContext.Response.Redirect("~/home/except");
                }
                else
                {
                    filterContext.HttpContext.Response.StatusCode = 500;
                    filterContext.HttpContext.Response.Redirect("~/home/except");
                }
                SimpleComm.Files.LogHelper.Write(filterContext.Exception);
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}