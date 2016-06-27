using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WindowsServer.Configuration;

namespace ShiJu.Portal.Models
{
    public class ImageHubAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewData["ImageHubBaseUrl"] = ConfigurationCenter.Global["ImageHubBaseUrl"];
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}