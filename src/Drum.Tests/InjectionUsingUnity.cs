using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Practices.Unity;
using Unity.WebApi;
using Xunit;

namespace Drum.Tests
{
    public class InjectionUsingUnityTests
    {
        [Fact]
        public async Task UriMaker_is_ctor_injected_using_Unity()
        {
            var config = new HttpConfiguration();
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // Web API routes
            var uriMakerContext = config.MapHttpAttributeRoutesAndUseUriMaker();
            
            // Unity
            var container = new UnityContainer();
            container.RegisterType(typeof(UriMaker<>), typeof(UriMaker<>), new InjectionConstructor(typeof(UriMakerContext), typeof(HttpRequestMessage)));
            container.RegisterType<CurrentRequest, CurrentRequest>(new HierarchicalLifetimeManager()); ;
            container.RegisterInstance(uriMakerContext);
            container.RegisterType<HttpRequestMessage>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(
                    c => c.Resolve<CurrentRequest>().Value));
            config.DependencyResolver = new UnityDependencyResolver(container);
            config.MessageHandlers.Add(new CurrentRequestHandler());

            //builder.RegisterInstance(uriMakerContext).ExternallyOwned();
            //builder.RegisterHttpRequestMessage(config);
            //builder.RegisterGeneric(typeof(UriMaker<>)).AsSelf().InstancePerRequest();
            
            var client = new HttpClient(new HttpServer(config));
            var res = await client.GetAsync("http://www.example.net/api/unity/resources");
            if (res.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(await res.Content.ReadAsStringAsync());
            }
            Assert.Equal(HttpStatusCode.OK,res.StatusCode);
            var body = await res.Content.ReadAsAsync<ResourceState>();
            Assert.Equal("http://www.example.net/api/unity/resources", body.self.ToString());
            Assert.Equal("http://www.example.net/api/unity/resources?page=1&count=10", body.next.ToString());
            Assert.Equal("http://www.example.net/api/unity/resources/0", body.first.ToString());
            Assert.Equal("http://www.example.net/api/unity/resources/0?detailed=True", body.first_alternative.ToString());

        }

        public class CurrentRequest
        {
            public HttpRequestMessage Value { get; set; }
        }

        public class CurrentRequestHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var scope = request.GetDependencyScope();
                var currentRequest = (CurrentRequest)scope.GetService(typeof(CurrentRequest));
                currentRequest.Value = request;
                return base.SendAsync(request, cancellationToken);
            }
        }

        [RoutePrefix("api/unity/resources")]
        public class UnityInjectionController : ApiController
        {
            private readonly UriMaker<UnityInjectionController> _uriMaker;

            [Route("")]
            public ResourceState GetAll()
            {
                var repr = new ResourceState
                {
                    self = _uriMaker.UriFor(c => c.GetAll()),
                    next = _uriMaker.UriFor(c => c.GetPaged(1, 10)),
                    first = _uriMaker.UriFor(c => c.GetById(0)),
                    first_alternative = _uriMaker.UriFor(c => c.GetById(0, true))
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

            public UnityInjectionController(UriMaker<UnityInjectionController> uriMaker)
            {
                _uriMaker = uriMaker;
            }
        }
    }
}
