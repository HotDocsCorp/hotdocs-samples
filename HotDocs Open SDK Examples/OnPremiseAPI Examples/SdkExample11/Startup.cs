using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SdkExample10.Startup))]

namespace SdkExample10
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}