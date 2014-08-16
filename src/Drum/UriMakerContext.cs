using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Routing;

namespace Drum
{
    public class UriMakerContext
    {
        private readonly Func<MethodInfo, RouteEntry> _routeMap;

        public Func<MethodInfo, RouteEntry> RouteMap { get { return _routeMap; } }

        public UriMakerContext(Func<MethodInfo,RouteEntry> routeMap)
        {
            _routeMap = routeMap;
        }

        public UriMaker<TController> NewUriMakerFor<TController>(UrlHelper urlHelper)
        {
            return new UriMaker<TController>(_routeMap, urlHelper);
        }

        public UriMaker<TController> NewUriMakerFor<TController>(HttpRequestMessage request)
        {
            return new UriMaker<TController>(_routeMap, new UrlHelper(request));
        }
    }
}