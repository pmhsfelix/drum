using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Drum
{
    public class DecoratorRouteProvider : IDirectRouteProvider
    {
        private readonly IDirectRouteProvider _provider;
        private readonly IDictionary<MethodInfo, RouteEntry> _map = new Dictionary<MethodInfo, RouteEntry>(); 

        public DecoratorRouteProvider(IDirectRouteProvider provider)
        {
            _provider = provider;
            Mapper = methodInfo =>
            {
                RouteEntry entry;
                return _map.TryGetValue(methodInfo, out entry) ? entry : null;
            };
        }

        public IReadOnlyList<RouteEntry> GetDirectRoutes(HttpControllerDescriptor controllerDescriptor, IReadOnlyList<HttpActionDescriptor> actionDescriptors,
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
                if (descs.Length > 1)
                {
                    throw new Exception("more than one action on the same route is not supported");   
                }
                var desc = descs[0] as ReflectedHttpActionDescriptor;
                if (desc == null)
                {
                    continue;
                }
                
                _map.Add(desc.MethodInfo, newRoute);
            }
            return list;
        }

        public Func<MethodInfo, RouteEntry> Mapper
        {
            get; private set;
        }
    }
}