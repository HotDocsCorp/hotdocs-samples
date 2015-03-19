using Microsoft.Owin;
using Owin;
using SdkExample11;

[assembly: OwinStartup(typeof(Startup))]

namespace SdkExample11
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}