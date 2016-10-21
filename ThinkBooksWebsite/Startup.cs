using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ThinkBooksWebsite.Startup))]
namespace ThinkBooksWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
