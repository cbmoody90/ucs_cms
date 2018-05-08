using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(United_Caring_Services.Startup))]
namespace United_Caring_Services
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
