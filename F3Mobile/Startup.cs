using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(F3Mobile.Startup))]
namespace F3Mobile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
