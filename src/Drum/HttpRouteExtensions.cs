using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Drum
{
    public static class HttpRouteExtensions
    {
        public static HttpActionDescriptor[] GetTargetActionDescriptors(this IHttpRoute route)
        {
            IDictionary<string, object> dataTokens = route.DataTokens;

            if (dataTokens == null)
            {
                return null;
            }

            object value;
            
            if (!dataTokens.TryGetValue("actions", out value))
            {
                return null;
            }
            return value as HttpActionDescriptor[];
        }
    }
}