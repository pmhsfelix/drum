using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Routing;

namespace Drum
{
    public class UriMaker
    {
        private readonly UrlHelper _urlHelper;

        private static readonly MethodInfo DictionaryAddMethod = typeof(Dictionary<string, object>).GetMethod(
            "Add",
            new [] { typeof(string), typeof(object) });

        private readonly Func<MethodInfo, RouteEntry> _mapper;


        public UriMaker(Func<MethodInfo, RouteEntry> mapper, UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
            _mapper = mapper;
        }

        public Uri UriFor<T>(Expression<Action<T>> action)
        {
            var methodCall = action.Body as MethodCallExpression;
            if (methodCall == null)
            {
                throw new ArgumentException("The expression must be a method call.");
            }

            var actionMethod = methodCall.Method;
            var actionParameters = actionMethod.GetParameters();

            // Get the route
            var route = _mapper(actionMethod);
            if(route == null)
            {
                throw new Exception("Unable to find route");
            }
            
            if (methodCall.Arguments.Count == 0)
            {
                return new Uri(_urlHelper.Link(route.Name, new { }));
            }

            // Build an expression that creates a dictionary with the following structure:
            //
            // { <actionParamaterName>, <actionParameterValue> }
            var routeValuesExpr = Expression.Lambda<Func<Dictionary<string, object>>>
                (
                    Expression.ListInit(
                        Expression.New(typeof(Dictionary<string, object>)),
                        methodCall.Arguments.Select((a, i) => Expression.ElementInit(
                            DictionaryAddMethod,
                            Expression.Constant(actionParameters[i].Name),
                            Expression.Convert(a, typeof(object))))
                        )
                );

            // Compile and evaluate the expressions
            var routeValuesGetter = routeValuesExpr.Compile();
            var routeValues = routeValuesGetter();

            return new Uri(_urlHelper.Link(route.Name, routeValues));
        }
    }

    public class UriMaker<TController>
    {
        private readonly UriMaker _uriMaker;

        public Uri UriFor(Expression<Action<TController>> action)
        {
            return _uriMaker.UriFor(action);
        }

        public UriMaker(Func<MethodInfo,RouteEntry> mapper, UrlHelper urlHelper)
        {
            _uriMaker = new UriMaker(mapper, urlHelper);
        }

        public UriMaker(UriMakerContext fact, HttpRequestMessage req)
            : this(fact.RouteMap, new UrlHelper(req))
        {}
    }
}