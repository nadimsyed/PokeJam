using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PokeJam.Startup))]
namespace PokeJam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
