using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Drum
{
    public class UriMaker
    {
        private readonly UrlHelper _urlHelper;

        private static readonly MethodInfo DictionaryAddMethod = typeof(Dictionary<string, object>).GetMethod(
            "Add",
            new Type[] { typeof(string), typeof(object) });

        public UriMaker(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;

        }

        public Uri UriFor<T>(Expression<Action<T>> action)
        {
            var methodCall = action.Body as MethodCallExpression;
            if (methodCall == null)
            {
                throw new ArgumentException("The expression must be a method call.", "method");
            }

            var actionMethod = methodCall.Method;
            var actionParameters = actionMethod.GetParameters();

            // Get the route
            var route = actionMethod.GetCustomAttributes(typeof(RouteAttribute), true)
                .Cast<RouteAttribute>()
                .FirstOrDefault(r => !String.IsNullOrEmpty(r.Name));
            if (route == null)
            {
                throw new InvalidOperationException("Action method must have a named Route attribute");
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

        public UriMaker(UrlHelper urlHelper)
        {
            _uriMaker = new UriMaker(urlHelper);
        }
    }
}