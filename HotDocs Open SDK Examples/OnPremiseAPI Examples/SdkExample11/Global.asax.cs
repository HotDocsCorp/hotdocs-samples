using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HotDocs.Sdk;

namespace SdkExample10
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            TemplateLocation.RegisterLocation(typeof (PackagePathTemplateLocation));
            TemplateLocation.RegisterLocation(typeof(WebServiceTemplateLocation));
        }
    }
}