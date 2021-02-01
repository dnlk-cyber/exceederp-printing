using System.Web.Http;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ExceedERP.Web.Startup))]
namespace ExceedERP.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //added
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("ExceedDbContext");

            app.UseHangfireDashboard("/nexterp");
            app.UseHangfireServer();

            //new
            //app.MapSignalR();
        }
    }
}
