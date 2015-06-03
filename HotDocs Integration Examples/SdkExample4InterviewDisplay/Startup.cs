using Microsoft.Owin;
using Owin;
using SdkExample4InterviewDisplay;

[assembly: OwinStartup(typeof (Startup))]

namespace SdkExample4InterviewDisplay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}