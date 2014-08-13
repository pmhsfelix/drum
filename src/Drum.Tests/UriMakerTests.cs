using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Xunit;

namespace Drum.Tests
{
    public class UriMakerTests
    {
        [RoutePrefix("api/UriMakerTests/resources")]
        public class ResourceController : ApiController
        {
            [Route("")]
            public void GetAll() { }

            [Route("")]
            public void GetPaged(int page, int count) { }

            [Route("{id}")]
            public void GetById(int id) { }

            [Route("{id}")]
            public void GetById(int id, bool detailed) { }
        }
        
        [Fact]
        public void Can_make_uri_for_action_without_prms()
        {
            var uri = _uriMaker.UriFor(c => c.GetAll());
            Assert.Equal("http://example.org/api/UriMakerTests/resources", uri.ToString());
        }

        [Fact]
        public void Can_make_uri_for_action_with_multiple_prms()
        {
            var uri = _uriMaker.UriFor(c => c.GetPaged(0, 10));
            Assert.Equal("http://example.org/api/UriMakerTests/resources?page=0&count=10", uri.ToString());
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
        
        public UriMakerTests()
        {
            var config = new HttpConfiguration();
            var factory = config.MapHttpAttributeRoutesAndUseUriMaker(new DefaultDirectRouteProvider());
            config.EnsureInitialized();
            var req = new HttpRequestMessage(HttpMethod.Get,"http://example.org");
            req.SetConfiguration(config);
            _uriMaker = factory.NewUriMakerFor<ResourceController>(req);
        }

        private readonly UriMaker<ResourceController> _uriMaker;
    }
}
