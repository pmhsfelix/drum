using System.Net.Http;
using System.Web.Http.Routing;

namespace Drum
{
    public static class HttpRequestExtensions
    {
        public static UriMaker<T> TryGetUriMakerFor<T>(this HttpRequestMessage req)
        {
            var factory =  SomethingInjectionHandler.TryGetUriMakerFactory(req);
            return factory != null ? factory.NewUriMakerFor<T>(req) : null;
        }
    }
}