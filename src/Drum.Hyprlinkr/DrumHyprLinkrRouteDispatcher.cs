using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ploeh.Hyprlinkr;

namespace Drum.Hyprlinkr
{
    public class DrumHyprLinkrRouteDispatcher : IRouteDispatcher
    {
        private readonly UriMakerContext _ctx;

        public Rouple Dispatch(MethodCallExpression method, IDictionary<string, object> routeValues)
        {
            var handler = _ctx.RouteMap(method.Method);
            if (handler == null)
            {
                throw new InvalidOperationException("Unable to handle that method");
            }
            routeValues = handler.GetRouteMapFor(method.Arguments);
            return new Rouple(handler.RouteEntry.Name, routeValues);
        }

        public DrumHyprLinkrRouteDispatcher(UriMakerContext ctx)
        {
            _ctx = ctx;
        }
    }
}
