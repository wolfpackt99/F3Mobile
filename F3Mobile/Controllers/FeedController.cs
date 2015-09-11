using F3.Business;
using F3.ViewModels;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace F3Mobile.Controllers
{
    public class FeedController : ApiController
    {
        [Inject]
        public IFeed Feed { get; set; }
        
        // GET: api/Feed
        public IEnumerable<Post> Get()
        {
            var posts = Feed.GetPosts();
            return posts;
        }

        // GET: api/Feed/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Feed
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Feed/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Feed/5
        public void Delete(int id)
        {
        }
    }
}
