using Microsoft.Owin;
using Owin;
using SdkExample12;

[assembly: OwinStartup(typeof(Startup))]

namespace SdkExample12
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}