using System.Web.Mvc;

namespace ExceedERP.Web.Areas.PrintingEstimation
{
    public class PrintingEstimationAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PrintingEstimation";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PrintingEstimation_default",
                "PrintingEstimation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}