using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Xunit;

namespace Drum.Tests
{
    public class MultipleRouteTests
    {
        private readonly HttpConfiguration _config;
        private readonly UriMakerContext _factory;

        [RoutePrefix("MultipleRouteTests")]
        public class MultipleRouteResourceController : ApiController
        {
            [Route("v1")]
            [Route("v2")]
            public HttpResponseMessage GetAll()
            {
                return null;
            }

            [Route("v1/{id}", Name="v1")]
            [Route("v2/{id}")]
            public void GetById(int id) { }
        }

        [Fact]
        public void Can_make_uri_for_v1()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "http://example.org/MultipleRouteTests/v1");
            req.SetRequestContext(new HttpRequestContext
            {
                VirtualPathRoot = "/"
            });
            req.SetRouteData(_config.Routes.GetRouteData(req));
            req.SetConfiguration(_config);
            var uriMaker = _factory.NewUriMakerFor<MultipleRouteTests.MultipleRouteResourceController>(req);
            var uri = uriMaker.UriFor(c => c.GetById(123));
            Assert.Equal("http://example.org/MultipleRouteTests/v1/123",uri.ToString());
        }

        [Fact]
        public void Can_make_uri_for_v2()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "http://example.org/MultipleRouteTests/v2");
            req.SetRequestContext(new HttpRequestContext
            {
                VirtualPathRoot = "/"
            });
            req.SetRouteData(_config.Routes.GetRouteData(req));
            req.SetConfiguration(_config);
            var uriMaker = _factory.NewUriMakerFor<MultipleRouteTests.MultipleRouteResourceController>(req);
            var uri = uriMaker.UriFor(c => c.GetById(123));
            Assert.Equal("http://example.org/MultipleRouteTests/v2/123", uri.ToString());
        }

        private RouteEntry Selector(HttpRequestMessage req, ICollection<RouteEntry> routes)
        {
            var v = req.RequestUri.AbsolutePath.Contains("/v2") ? "v2" : "v1";
            return routes.First(r => r.Route.RouteTemplate.Contains(v));
        }

        public MultipleRouteTests()
        {
            _config = new HttpConfiguration();
            _factory = _config.MapHttpAttributeRoutesAndUseUriMaker(new DefaultDirectRouteProvider(), Selector);
            _config.EnsureInitialized();
        }
    }
}
