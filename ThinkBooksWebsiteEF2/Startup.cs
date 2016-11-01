using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ThinkBooksWebsiteEF2.Startup))]
namespace ThinkBooksWebsiteEF2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
