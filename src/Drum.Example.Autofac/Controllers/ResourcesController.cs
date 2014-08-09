using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Drum.Example.Autofac.Controllers
{
    [RoutePrefix("api/resources")]
    public class ResourcesController : ApiController
    {
        private readonly UriMaker<ResourcesController> _maker;

        public ResourcesController(UriMaker<ResourcesController> maker)
        {
            _maker = maker;
        }

        [Route("")]
        public ResourceState GetAll()
        {
            return new ResourceState
            {
                self = _maker.UriFor(c => c.GetAll()),
                next = _maker.UriFor(c => c.GetPaged(1, 10)),
                first = _maker.UriFor(c => c.GetById(0)),
                first_alternative = _maker.UriFor(c => c.GetById(0, true))
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
