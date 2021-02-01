using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.BudgetGAAP
{
    public class BudgetGAAPGAAPAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BudgetGAAP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BudgetGAAP_default",
                "BudgetGAAP/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}