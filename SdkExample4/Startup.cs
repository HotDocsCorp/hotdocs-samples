using Microsoft.Owin;
using Owin;
using SdkExample4;

[assembly: OwinStartup(typeof (Startup))]

namespace SdkExample4
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}