using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Observer.Startup))]
namespace Observer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
