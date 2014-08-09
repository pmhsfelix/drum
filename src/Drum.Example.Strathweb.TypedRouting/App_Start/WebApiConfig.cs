using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Drum.Example.Strathweb.TypedRouting.Controllers;
using Strathweb.TypedRouting;

namespace Drum.Example.Strathweb.TypedRouting
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutesAndUseUriMaker(new TypedDirectRouteProvider());
            config.TypedRoute("api/resources", r => r.Action<ResourcesController>(c => c.GetAll()));
            config.TypedRoute("api/resources", r => r.Action<ResourcesController>(c => c.GetPaged(Param.Any<int>(), Param.Any<int>())));
            config.TypedRoute("api/resouces/{id:int}", r => r.Action<ResourcesController>(c => c.GetById(Param.Any<int>())));
            config.TypedRoute("api/resouces/{id:int}", r => r.Action<ResourcesController>(c => c.GetByIdDetailed(Param.Any<int>(),Param.Any<bool>())));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
