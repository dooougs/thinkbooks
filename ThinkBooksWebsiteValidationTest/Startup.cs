using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ThinkBooksWebsiteValidationTest.Startup))]
namespace ThinkBooksWebsiteValidationTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
