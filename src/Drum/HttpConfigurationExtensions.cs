using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Drum
{
    public static class HttpConfigurationExtensions
    {
        public static UriMakerFactory MapHttpAttributeRoutesAndUseUriMaker(
            this HttpConfiguration configuration,
            IDirectRouteProvider directRouteProvider = null)
        {
            directRouteProvider = directRouteProvider ?? new DefaultDirectRouteProvider();
            var decorator = new DecoratorRouteProvider(directRouteProvider);
            configuration.MapHttpAttributeRoutes(decorator);
            var uriMakerFactory = new UriMakerFactory(decorator.Mapper);
            configuration.MessageHandlers.Add(new SomethingInjectionHandler(uriMakerFactory));
            return uriMakerFactory;
        }
    }
}