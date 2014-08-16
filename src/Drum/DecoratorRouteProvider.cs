using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Drum
{
    internal class DecoratorRouteProvider : IDirectRouteProvider
    {
        private readonly IDirectRouteProvider _provider;
        private readonly IDictionary<MethodInfo, RouteEntry> _map = new Dictionary<MethodInfo, RouteEntry>(); 

        public DecoratorRouteProvider(IDirectRouteProvider provider)
        {
            _provider = provider;
            RouteMap = methodInfo =>
            {
                RouteEntry entry;
                return _map.TryGetValue(methodInfo, out entry) ? entry : null;
            };
        }

        IReadOnlyList<RouteEntry> IDirectRouteProvider.GetDirectRoutes(HttpControllerDescriptor controllerDescriptor, IReadOnlyList<HttpActionDescriptor> actionDescriptors,
            IInlineConstraintResolver constraintResolver)
        {
            var routes = _provider.GetDirectRoutes(controllerDescriptor, actionDescriptors, constraintResolver);
            var list = new List<RouteEntry>();
            foreach (var route in routes)
            {
                var newRoute = new RouteEntry(route.Name ?? Guid.NewGuid().ToString(), route.Route);
                list.Add(newRoute);
                var descs = route.Route.GetTargetActionDescriptors();
                if (descs.Length == 0)
                {
                    continue;
                }
                foreach (var desc in descs)
                {
                    var reflDesc = desc as ReflectedHttpActionDescriptor;
                    if (reflDesc == null)
                    {
                        continue;
                    }
                    var method = reflDesc.MethodInfo;
                    RouteEntry prevEntry;
                    if (_map.TryGetValue(method, out prevEntry))
                    {
                        throw new MultipleRoutesForSameMethodException(reflDesc, prevEntry, newRoute);
                    }
                    _map.Add(method, newRoute);
                }
            }
            return list;
        }

        public Func<MethodInfo, RouteEntry> RouteMap
        {
            get; private set;
        }
    }
}