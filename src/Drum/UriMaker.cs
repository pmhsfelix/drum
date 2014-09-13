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

        private readonly Func<MethodInfo, MethodHandler> _mapper;

        public UriMaker(Func<MethodInfo, MethodHandler> mapper, UrlHelper urlHelper)
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
            
            var methodHandler = _mapper(actionMethod);
            if(methodHandler == null)
            {
                throw new RouteNotFoundException(actionMethod);
            }

            return methodHandler.MakeUriFor(methodCall.Arguments, _urlHelper);

            return methodCall.Arguments.Count == 0 ? 
                new Uri(_urlHelper.Link(methodHandler.RouteEntry.Name, new { })) 
                : methodHandler.MakeUriFor(methodCall.Arguments, _urlHelper);
        }
    }

    public class UriMaker<TController>
    {
        private readonly UriMaker _uriMaker;

        public Uri UriFor(Expression<Action<TController>> action)
        {
            return _uriMaker.UriFor(action);
        }

        public UriMaker(Func<MethodInfo, MethodHandler> mapper, UrlHelper urlHelper)
        {
            _uriMaker = new UriMaker(mapper, urlHelper);
        }

        public UriMaker(UriMakerContext fact, HttpRequestMessage req)
            : this(fact.RouteMap, new UrlHelper(req))
        {}
    }
}