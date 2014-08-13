using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Xunit;

namespace Drum.Tests
{
    public class InjectionUsingAutofacTests
    {
        [Fact]
        public async Task UriMaker_is_ctor_injected_using_Autofac()
        {
            var config = new HttpConfiguration();
            // Autofac
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Web API routes
            var factory = config.MapHttpAttributeRoutesAndUseUriMaker();
            builder.RegisterInstance(factory).ExternallyOwned();
            builder.RegisterHttpRequestMessage(config);
            builder.RegisterGeneric(typeof(UriMaker<>)).AsSelf().InstancePerRequest();

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            
            var client = new HttpClient(new HttpServer(config));
            var res = await client.GetAsync("http://www.example.net/api/autofac/resources");
            Assert.Equal(HttpStatusCode.OK,res.StatusCode);
            var body = await res.Content.ReadAsAsync<ResourceState>();
            Assert.Equal("http://www.example.net/api/autofac/resources", body.self.ToString());
            Assert.Equal("http://www.example.net/api/autofac/resources?page=1&count=10", body.next.ToString());
            Assert.Equal("http://www.example.net/api/autofac/resources/0", body.first.ToString());
            Assert.Equal("http://www.example.net/api/autofac/resources/0?detailed=True", body.first_alternative.ToString());

        }

        [RoutePrefix("api/autofac/resources")]
        public class InjectionController : ApiController
        {
            [Route("")]
            public ResourceState GetAll()
            {
                var maker = Request.TryGetUriMakerFor<InjectionController>();
                var repr = new ResourceState
                {
                    self = maker.UriFor(c => c.GetAll()),
                    next = maker.UriFor(c => c.GetPaged(1, 10)),
                    first = maker.UriFor(c => c.GetById(0)),
                    first_alternative = maker.UriFor(c => c.GetById(0, true))
                };
                return repr;
            }

            [Route("")]
            public void GetPaged(int page, int count)
            {
                throw new NotImplementedException();
            }

            [Route("{id}")]
            public void GetById(int id)
            {
                throw new NotImplementedException();
            }

            [Route("{id}")]
            public void GetById(int id, bool detailed)
            {
                throw new NotImplementedException();
            }
        }
    }
}
