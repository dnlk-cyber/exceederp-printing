using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS
{
    public class APTSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "APTS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "APTS_default",
                "APTS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}