using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CloudServicesEmbeddedExampleInterview.Startup))]
namespace CloudServicesEmbeddedExampleInterview
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
