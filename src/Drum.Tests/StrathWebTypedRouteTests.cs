using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Strathweb.TypedRouting;
using Xunit;

namespace Drum.Tests
{
    public class StrathWebTypedRouteTests
    {
        [Fact]
        public async Task UriMaker_is_ctor_injected_using_Autofac()
        {
            var config = new HttpConfiguration();
           
            // Web API routes
            config.MapHttpAttributeRoutesAndUseUriMaker(new TypedDirectRouteProvider());
            config.TypedRoute("api/typedroutes/resources", r => r.Action<TypedRoutesController>(c => c.GetAll()));
            config.TypedRoute("api/typedroutes/resources", r => r.Action<TypedRoutesController>(c => c.GetPaged(Param.Any<int>(), Param.Any<int>())));
            config.TypedRoute("api/typedroutes/resources/{id:int}", r => r.Action<TypedRoutesController>(c => c.GetById(Param.Any<int>())));
            config.TypedRoute("api/typedroutes/resources/{id:int}", r => r.Action<TypedRoutesController>(c => c.GetByIdDetailed(Param.Any<int>(), Param.Any<bool>())));

            var client = new HttpClient(new HttpServer(config));
            var res = await client.GetAsync("http://www.example.net/api/typedroutes/resources");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var body = await res.Content.ReadAsAsync<ResourceState>();
            Assert.Equal("http://www.example.net/api/typedroutes/resources", body.self.ToString());
            Assert.Equal("http://www.example.net/api/typedroutes/resources?page=1&count=10", body.next.ToString());
            Assert.Equal("http://www.example.net/api/typedroutes/resources/0", body.first.ToString());
            Assert.Equal("http://www.example.net/api/typedroutes/resources/0?detailed=True", body.first_alternative.ToString());

        }
    }

    public class TypedRoutesController : ApiController
    {
        public ResourceState GetAll()
        {
            var maker = Request.TryGetUriMakerFor<TypedRoutesController>();
            return new ResourceState
            {
                self = maker.UriFor(c => c.GetAll()),
                next = maker.UriFor(c => c.GetPaged(1, 10)),
                first = maker.UriFor(c => c.GetById(0)),
                first_alternative = maker.UriFor(c => c.GetByIdDetailed(0, true))
            };
        }

        public void GetPaged(int page, int count)
        {
            throw new NotImplementedException();
        }

        public void GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void GetByIdDetailed(int id, bool detailed)
        {
            throw new NotImplementedException();
        }
    }
}
