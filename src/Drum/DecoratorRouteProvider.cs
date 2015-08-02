using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;

namespace Drum
{
    public class MethodHandler
    {
        public MethodHandler(MethodInfo method, RouteEntry newRoute, Func<HttpRequestMessage, ICollection<RouteEntry>, RouteEntry> routeSelector)
        {
            _routeSelector = routeSelector ?? ((req, coll) => coll.First());
            RouteEntries = new Collection<RouteEntry> { newRoute };
            ParameterHandlers = method.GetParameters().Select(p => new ParameterHandler(p)).ToList();
        }

        public ICollection<RouteEntry> RouteEntries { get; private set; }
        public IReadOnlyCollection<ParameterHandler> ParameterHandlers { get; private set; }

        private static readonly IDictionary<string, object> EmptyMap = new Dictionary<string, object>();
        private readonly Func<HttpRequestMessage, ICollection<RouteEntry>, RouteEntry> _routeSelector;

        public Uri MakeUriFor(ReadOnlyCollection<Expression> arguments, UrlHelper urlHelper)
        {
            var argumentValues = ParameterHandlers.Any() ? ComputeRouteMap(ComputeArgumentValues(arguments)) : EmptyMap;
            var map = new Dictionary<string, object>();
            if (urlHelper.Request.GetRouteData() != null && urlHelper.Request.GetRouteData().GetSubRoutes() != null)
            {
                foreach (var p in urlHelper.Request.GetRouteData().GetSubRoutes().SelectMany(r => r.Values))
                {
                    map[p.Key] = p.Value;
                }
            }
            foreach (var p in argumentValues)
            {
                map[p.Key] = p.Value;
            }

            var routeEntry = _routeSelector(urlHelper.Request, RouteEntries);
            return new Uri(urlHelper.Link(routeEntry.Name, map));
        }

        public IDictionary<string, object> GetRouteMapFor(ReadOnlyCollection<Expression> arguments)
        {
            return ComputeRouteMap(ComputeArgumentValues(arguments));
        }

        private IDictionary<string, object> ComputeRouteMap(object[] argValues)
        {
            return ParameterHandlers
                .Where(ph => ph.IsFromUri)
                .SelectMany((ph, i) => ph.GetRouteValues.Select(rv => new
                {
                    Name = rv.Name,
                    Getter = new Func<object, object>(rv.GetFromArgumentValue),
                    Object = argValues[i]
                }))
                .ToDictionary(_ => _.Name, _ => _.Getter(_.Object));
        }

        private object[] ComputeArgumentValues(IEnumerable<Expression> arguments)
        {
            return Expression.Lambda<Func<object[]>>
                (
                    Expression.NewArrayInit(
                        typeof(object),
                        Enumerable.Zip(ParameterHandlers, arguments, (ph, a) => new
                        {
                            ParameterHandler = ph,
                            Argument = a
                        })
                            .Where(_ => _.ParameterHandler.IsFromUri)
                            .Select(_ => Expression.Convert(_.Argument, typeof(object))))
                )
                .Compile()();
        }
    }

    public class ParameterHandler
    {
        public ParameterHandler(ParameterInfo parameterInfo)
        {
            if (IsSimpleType(parameterInfo.ParameterType))
            {
                GetRouteValues =
                    new ReadOnlyCollection<RouteValueHandler>(new List<RouteValueHandler>()
                    {
                        new RouteValueHandler(parameterInfo.Name, v => v)
                    });
            }
            else
            {
                var fromUriAttributes = parameterInfo.GetCustomAttributes(typeof(FromUriAttribute), false);
                var fromUriAttribute = fromUriAttributes.FirstOrDefault() as FromUriAttribute;
                var nameFormat = fromUriAttribute != null && !string.IsNullOrEmpty(fromUriAttribute.Name)? string.Format("{0}.{{0}}", fromUriAttribute.Name) : "{0}";

                var type = parameterInfo.ParameterType;
                var typeDesc = new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
                var propDescs = typeDesc.GetProperties();
                GetRouteValues = propDescs.OfType<PropertyDescriptor>()
                    .Select(desc => new RouteValueHandler(string.Format(nameFormat, desc.Name), desc.GetValue)).ToList();
            }

            IsFromUri = true;
        }

        private bool IsSimpleType(Type parameterType)
        {
            return TypeDescriptor.GetConverter(parameterType).CanConvertFrom(typeof(string));
        }

        public bool IsFromUri { get; private set; }

        public IReadOnlyCollection<RouteValueHandler> GetRouteValues { get; private set; }

    }

    public class RouteValueHandler
    {
        private readonly Func<object, object> _getter;
        public string Name { get; private set; }

        public object GetFromArgumentValue(object argument)
        {
            return _getter(argument);
        }

        public RouteValueHandler(string name, Func<object, object> getter)
        {
            _getter = getter;
            Name = name;
        }
    }

    internal class DecoratorRouteProvider : IDirectRouteProvider
    {
        private readonly IDirectRouteProvider _provider;
        private readonly Func<HttpRequestMessage, ICollection<RouteEntry>, RouteEntry> _routeSelector;
        private readonly IDictionary<MethodInfo, MethodHandler> _map = new Dictionary<MethodInfo, MethodHandler>();

        public DecoratorRouteProvider(IDirectRouteProvider provider, Func<HttpRequestMessage, ICollection<RouteEntry>, RouteEntry> routeSelector = null)
        {
            _provider = provider;
            _routeSelector = routeSelector;
            RouteMap = methodInfo =>
            {
                MethodHandler entry;
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
                    MethodHandler prevEntry;
                    if (_map.TryGetValue(method, out prevEntry))
                    {
                        prevEntry.RouteEntries.Add(newRoute);
                        //throw new MultipleRoutesForSameMethodException(reflDesc, prevEntry.RouteEntry, newRoute);
                    }
                    else
                    {
                        _map.Add(method, new MethodHandler(method, newRoute, _routeSelector));
                    }
                }
            }
            return list;
        }

        public Func<MethodInfo, MethodHandler> RouteMap
        {
            get;
            private set;
        }
    }
}