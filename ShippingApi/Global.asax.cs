using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShippingApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static string Token;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
