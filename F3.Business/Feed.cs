using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using F3.ViewModels;

namespace F3.Business
{
    public interface IFeed
    {
        IEnumerable<Post> GetPosts();
    }

    public class Feed : IFeed
    {
        private const string FeedUri = "http://f3nation.com/locations/charlotte-south-nc/feed/";
        public IEnumerable<Post> GetPosts()
        {
            IEnumerable<Post> posts = new List<Post>();
            var feed = nJupiter.Web.Syndication.FeedReader.GetFeed(new Uri(FeedUri));
            if (feed != null && feed.Items != null)
            {
                posts = feed.Items.Select(i => new Post
                {
                    Url = i.Uri.ToString(),
                    Author = i.Author.Name,
                    Content = i.Description,
                    Title = i.Title,
                    Date = i.PublishDate.ToString("G"),
                    
                });
            }

            return posts;
        }
    }
}
