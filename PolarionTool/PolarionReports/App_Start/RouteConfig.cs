using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PolarionReports
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{docFilter}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, docFilter = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Plan",
                url: "{controller}/{action}/{source}/{target}"
                );
        }
    }
}
