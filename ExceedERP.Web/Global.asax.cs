using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Kendo.Mvc.Infrastructure;
using Newtonsoft.Json;
using Telerik.Reporting.Services.WebApi;

namespace ExceedERP.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Telerik.Reporting.Services.WebApi.ReportsControllerConfiguration.RegisterRoutes(System.Web.Http.GlobalConfiguration.Configuration);
           // ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings
            .PreserveReferencesHandling = PreserveReferencesHandling.All;

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings
                    .ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

            Kendo.Mvc.Infrastructure.DI.Current.Register<IJavaScriptInitializer>(() => new CustomKendoInitializer());

        }

    }
}
