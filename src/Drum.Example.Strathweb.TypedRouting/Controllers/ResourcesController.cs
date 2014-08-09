using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Drum.Example.Strathweb.TypedRouting.Controllers
{
    public class ResourcesController : ApiController
    {
        public ResourceState GetAll()
        {
            var maker = Request.TryGetUriMakerFor<ResourcesController>();
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

    public class ResourceState
    {
        public Uri self { get; set; }
        public Uri next { get; set; }
        public Uri first { get; set; }
        public Uri first_alternative { get; set; }
    }
}
