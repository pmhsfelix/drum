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
        [RoutePrefix("api/resources")]
        public class ResourceController : ApiController
        {
            [Route("", Name = "GetAll")]
            public void GetAll() { }

            [Route("", Name = "GetAllPaged")]
            public void GetPaged(int page, int count) { }

            [Route("{id}", Name = "GetById")]
            public void GetById(int id) { }

            [Route("", Name = "GetByProp1")]
            public void GetByProp1(string prop1) { }

            [Route("", Name = "GetByProp2")]
            public void GetByProp2(string prop2) { }

        }
        
        [Fact]
        public void Fact()
        {
            var uri = _uriMaker.UriFor(c => c.GetAll());
            Assert.Equal("http://example.org/api/resources", uri.ToString());
        }

        public UriMakerTests()
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
            var req = new HttpRequestMessage(HttpMethod.Get,"http://example.org");
            req.SetConfiguration(config);
            _uriMaker = new UriMaker<ResourceController>(new UrlHelper(req));
        }

        private readonly UriMaker<ResourceController> _uriMaker;
    }
}
