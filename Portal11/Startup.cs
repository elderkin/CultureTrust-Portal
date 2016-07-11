using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Portal11.Startup))]
namespace Portal11
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
