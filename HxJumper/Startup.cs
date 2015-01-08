using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HxJumper.Startup))]
namespace HxJumper
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
