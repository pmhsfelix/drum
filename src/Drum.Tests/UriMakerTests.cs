using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Xunit;

namespace Drum.Tests
{
    [FromUri]
    public class PageInfo
    {
        public int page { get; set; }
        public int count { get; set; }
    }

    public class UriMakerTests
    {
        [RoutePrefix("api/UriMakerTests/resources")]
        public class ResourceController : ApiController
        {
            [Route("")]
            public async Task<HttpResponseMessage> GetAll()
            {
                await Task.Delay(0);
                return null;
            }

            [Route("", Name="GetPaged")]
            public void GetPaged(int page, int count) { }

            [Route("")]
            public void GetPaged(PageInfo page, bool detailed) { }

            [Route("{id}")]
            public void GetById(int id) { }

            [Route("{id}")]
            public void GetById(int id, bool detailed) { }
        }
        
        [Fact]
        public async Task Can_make_uri_for_action_without_prms()
        {
            await Task.Delay(0);
            var uri = _uriMaker.UriFor(c => c.GetAll());
            Assert.Equal("http://example.org/api/UriMakerTests/resources", uri.ToString());
        }

        [Fact]
        public void Can_make_uri_for_action_with_multiple_prms()
        {
            // using UrlHelper 
            var uri1 = _urlHelper.Link("GetPaged", new { page = 0, count = 10 });

            // using UriMaker
            var uri2 = _uriMaker.UriFor(c => c.GetPaged(0, 10));
            
            Assert.Equal(uri1,uri2.ToString());
            Assert.Equal("http://example.org/api/UriMakerTests/resources?page=0&count=10", uri2.ToString());
        }

        [Fact]
        public void Can_make_uri_for_action_with_template_prms()
        {
            var uri = _uriMaker.UriFor(c => c.GetById(123));
            Assert.Equal("http://example.org/api/UriMakerTests/resources/123", uri.ToString());
        }

        [Fact]
        public void Can_make_uri_for_action_with_template_prms_and_qs_prms()
        {
            var uri = _uriMaker.UriFor(c => c.GetById(123, true));
            Assert.Equal("http://example.org/api/UriMakerTests/resources/123?detailed=True", uri.ToString());
        }

        [Fact]
        public void Can_make_uri_for_action_with_complex_uri_parameters()
        {
            var uri = _uriMaker.UriFor(c => c.GetPaged(new PageInfo{page = 2,count = 10}, true));
            Assert.Equal("http://example.org/api/UriMakerTests/resources?page=2&count=10&detailed=True", uri.ToString());
        }
        
        public UriMakerTests()
        {
            var config = new HttpConfiguration();
            var factory = config.MapHttpAttributeRoutesAndUseUriMaker(new DefaultDirectRouteProvider());
            config.EnsureInitialized();
            var req = new HttpRequestMessage(HttpMethod.Get,"http://example.org");
            req.SetConfiguration(config);
            _urlHelper = new UrlHelper(req);
            _uriMaker = factory.NewUriMakerFor<ResourceController>(req);
        }

        private readonly UriMaker<ResourceController> _uriMaker;
        private readonly UrlHelper _urlHelper;
    }
}
