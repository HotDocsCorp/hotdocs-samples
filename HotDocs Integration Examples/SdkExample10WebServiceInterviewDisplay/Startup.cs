using Microsoft.Owin;
using Owin;
using SdkExample10WebServiceInterviewDisplay;

[assembly: OwinStartup(typeof(Startup))]

namespace SdkExample10WebServiceInterviewDisplay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}