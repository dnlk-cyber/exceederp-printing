using System.Web.Mvc;

namespace ExceedERP.Web.Areas.ProductionFollowUp
{
    public class ProductionFollowUpAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ProductionFollowUp";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ProductionFollowUp_default",
                "ProductionFollowUp/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}