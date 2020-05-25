using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;
using System.Collections.Generic;

namespace ChatServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );
        }
    }
}
