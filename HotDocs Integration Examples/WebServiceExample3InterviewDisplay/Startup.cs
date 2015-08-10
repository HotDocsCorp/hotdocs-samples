using Microsoft.Owin;
using Owin;
using WebServiceExample3InterviewDisplay;

[assembly: OwinStartup(typeof(Startup))]

namespace WebServiceExample3InterviewDisplay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}