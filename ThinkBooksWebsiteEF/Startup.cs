using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ThinkBooksWebsiteEF.Startup))]
namespace ThinkBooksWebsiteEF
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
