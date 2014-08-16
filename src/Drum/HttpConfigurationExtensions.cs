using System.Web.Http;
using System.Web.Http.Routing;

namespace Drum
{
    public static class HttpConfigurationExtensions
    {
        public static UriMakerContext MapHttpAttributeRoutesAndUseUriMaker(
            this HttpConfiguration configuration,
            IDirectRouteProvider directRouteProvider = null)
        {
            directRouteProvider = directRouteProvider ?? new DefaultDirectRouteProvider();
            var decorator = new DecoratorRouteProvider(directRouteProvider);
            configuration.MapHttpAttributeRoutes(decorator);
            var uriMakerContext = new UriMakerContext(decorator.RouteMap);
            return uriMakerContext;
        }

        public static void FlowUriMakerContextOnRequests(this HttpConfiguration configuration, UriMakerContext uriMakerContext)
        {
            configuration.MessageHandlers.Add(new UriMakerRequestFlowHandler(uriMakerContext));
        }
    }
}