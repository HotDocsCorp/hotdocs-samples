using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CloudServicesAPIExample3Interview.Startup))]
namespace CloudServicesAPIExample3Interview
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
