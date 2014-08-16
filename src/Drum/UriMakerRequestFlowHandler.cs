using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Drum
{
    public class UriMakerRequestFlowHandler : DelegatingHandler
    {
        private readonly UriMakerContext _context;
        private const string Key = "UriMakerContext";

        public UriMakerRequestFlowHandler(UriMakerContext context)
        {
            _context = context;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Properties.Add(Key, _context);
            return base.SendAsync(request, cancellationToken);
        }
        
        public static UriMakerContext TryGetUriMakerFactory(HttpRequestMessage req)
        {
            object value;
            if (!req.Properties.TryGetValue(Key, out value))
            {
                return null;
            }
            return value as UriMakerContext;
        }
    }
}