using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SciEvidenceBank.Startup))]
namespace SciEvidenceBank
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
