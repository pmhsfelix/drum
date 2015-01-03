using System;
using System.Collections.Generic;
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
    public class RouteDataPropagationTests
    {

        private readonly HttpConfiguration _config;
        private readonly UriMakerContext _factory;

        [RoutePrefix("{tenantId}/MultiTenantTests")]
        public class MultipleTenantController : ApiController
        {
            [Route("")]
            public HttpResponseMessage GetAll()
            {
                return null;
            }

            [Route("{id}")]
            public void GetById(int id) { }
        }

        [Fact]
        public void Can_make_uri_for_tenant1()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "http://example.org/tenant1/MultiTenantTests");
            req.SetRequestContext(new HttpRequestContext
            {
                VirtualPathRoot = "/"
            });
            req.SetRouteData(_config.Routes.GetRouteData(req));
            req.SetConfiguration(_config);
            var uriMaker = _factory.NewUriMakerFor<MultipleTenantController>(req);
            var uri = uriMaker.UriFor(c => c.GetById(123));
            Assert.Equal("http://example.org/tenant1/MultiTenantTests/123", uri.ToString());
        }

        [Fact]
        public void Can_make_uri_for_tenant2()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, "http://example.org/tenant2/MultiTenantTests");
            req.SetRequestContext(new HttpRequestContext
            {
                VirtualPathRoot = "/"
            });
            req.SetRouteData(_config.Routes.GetRouteData(req));
            req.SetConfiguration(_config);
            var uriMaker = _factory.NewUriMakerFor<MultipleTenantController>(req);
            var uri = uriMaker.UriFor(c => c.GetById(123));
            Assert.Equal("http://example.org/tenant2/MultiTenantTests/123", uri.ToString());
        }


        public RouteDataPropagationTests()
        {
            _config = new HttpConfiguration();
            _factory = _config.MapHttpAttributeRoutesAndUseUriMaker(new DefaultDirectRouteProvider());
            _config.EnsureInitialized();
        }

    }
}
