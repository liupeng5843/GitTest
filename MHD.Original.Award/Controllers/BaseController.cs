using SimpleComm.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MHD.Original.Award.Controllsers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            SimpleComm.Files.LogHelper.Write(filterContext.Exception);
            base.OnException(filterContext);
        }
        protected class LoginException : Exception
        {

        }
    }
}
