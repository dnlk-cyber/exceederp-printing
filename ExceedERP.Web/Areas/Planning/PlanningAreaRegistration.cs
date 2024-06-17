using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Planning
{
    public class PlanningAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Planning";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Planning_default",
                "Planning/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}