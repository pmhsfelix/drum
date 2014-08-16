using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Drum
{
    public class MultipleRoutesForSameMethodException : UnsupportedException
    {
        public ReflectedHttpActionDescriptor Action { get; private set; }
        public RouteEntry PrevRoute { get; private set; }
        public RouteEntry NewRoute { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("Multiple routes for the same action. Action: {0}, previous route {1}, new route: {2}",
                    Action.ActionName, PrevRoute.Route.RouteTemplate, NewRoute.Route.RouteTemplate);
            }
        }

        public MultipleRoutesForSameMethodException(ReflectedHttpActionDescriptor action, RouteEntry prevRoute, RouteEntry newRoute)
        {
            Action = action;
            PrevRoute = prevRoute;
            NewRoute = newRoute;
        }
    }
}