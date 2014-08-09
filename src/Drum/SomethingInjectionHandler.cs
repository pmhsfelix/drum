using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Drum
{
    public class SomethingInjectionHandler : DelegatingHandler
    {
        private readonly UriMakerFactory _factory;
        private const string Key = "UriMakerFactory";

        public SomethingInjectionHandler(UriMakerFactory factory)
        {
            _factory = factory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Properties.Add(Key, _factory);
            return base.SendAsync(request, cancellationToken);
        }
        
        public static UriMakerFactory TryGetUriMakerFactory(HttpRequestMessage req)
        {
            object value;
            if (!req.Properties.TryGetValue(Key, out value))
            {
                return null;
            }
            return value as UriMakerFactory;
        }
    }
}