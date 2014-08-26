using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Drum.Example.Controllers
{
    public class ResourceDto
    {
        public string Text { get; set; }
    }

    [RoutePrefix("api/resources")]
    public class ResourcesController : ApiController
    {
        [Route("")]
        public ResourceState GetAll()
        {
            var maker = Request.TryGetUriMakerFor<ResourcesController>();
            return new ResourceState
            {
                self = maker.UriFor(c => c.GetAll()),
                next = maker.UriFor(c => c.GetPaged(1, 10)),
                first = maker.UriFor(c => c.GetById(0)),
                first_alternative = maker.UriFor(c => c.GetById(0,true)),
                add = maker.UriFor(c => c.Post(Param<ResourceDto>.Any)),
                edit = maker.UriFor(c => c.Put(0, Param<ResourceDto>.Any))
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

        [Route("")]
        public void Post(ResourceDto dto)
        {
        }

        [Route("{id}")]
        public void Put(int id, ResourceDto dto)
        {
        }
    }

    public class ResourceState
    {
        public Uri self { get; set; }
        public Uri next { get; set; }
        public Uri first { get; set; }
        public Uri first_alternative { get; set; }
        public Uri add { get; set; }
        public Uri edit { get; set; }
    }
}
