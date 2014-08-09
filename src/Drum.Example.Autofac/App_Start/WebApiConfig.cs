using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;

namespace Drum.Example.Autofac
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Autofac
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            
            // Web API routes
            var factory = config.MapHttpAttributeRoutesAndUseUriMaker();
            builder.RegisterInstance(factory).ExternallyOwned();
            builder.RegisterHttpRequestMessage(config);
            builder.RegisterGeneric(typeof(UriMaker<>)).AsSelf().InstancePerRequest();
            
            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
