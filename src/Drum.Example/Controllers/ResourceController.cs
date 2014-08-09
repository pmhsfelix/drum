using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Drum.Example.Controllers
{
    [RoutePrefix("api/resources")]
    public class ResourceController : ApiController
    {
        [Route("")]
        public ResourceState GetAll()
        {
            var maker = Request.TryGetUriMakerFor<ResourceController>();
            return new ResourceState
            {
                self = maker.UriFor(c => c.GetAll()),
                next = maker.UriFor(c => c.GetPaged(1, 10)),
                first = maker.UriFor(c => c.GetById(0)),
                first_alternative = maker.UriFor(c => c.GetById(0,true))
            };
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

    public class ResourceState
    {
        public Uri self { get; set; }
        public Uri next { get; set; }
        public Uri first { get; set; }
        public Uri first_alternative { get; set; }
    }
}
