using ShiJu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData.Builder;

namespace ShiJu.Portal
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Party>("PartiesOData");
            modelBuilder.EntitySet<User>("UsersOData");
            modelBuilder.EntitySet<PartyCommentDto>("PartyCommentsOData");
            config.Routes.MapODataRoute("odata", "odata", modelBuilder.GetEdmModel());
        }
    }
}