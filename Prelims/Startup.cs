using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Prelims.Startup))]
namespace Prelims
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
