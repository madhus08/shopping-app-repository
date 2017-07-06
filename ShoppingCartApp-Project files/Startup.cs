using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShoppingAppFB.Startup))]
namespace ShoppingAppFB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
