using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NXYSOFT_RMS.Startup))]
namespace NXYSOFT_RMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
