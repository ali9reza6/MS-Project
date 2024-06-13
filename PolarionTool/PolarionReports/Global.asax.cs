using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using PolarionReports.App_Start;
using Serilog;
using PolarionReports.Custom_Helpers;
using System.Configuration;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;

namespace PolarionReports
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DisableApplicationInsightsOnDebug();
            // WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("c:\\logs\\logfile.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Debug("Start");

            Topol.PolarionServer = ConfigurationManager.AppSettings["PolarionServer"];
            if (string.IsNullOrEmpty(Topol.PolarionServer))
            {
                Topol.PolarionServer = "zkwsvpol01";
            }
        }


        [Conditional("DEBUG")]
        private static void DisableApplicationInsightsOnDebug()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }
    }
}
