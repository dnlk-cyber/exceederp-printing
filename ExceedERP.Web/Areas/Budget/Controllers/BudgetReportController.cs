using ExceedERP.Core.Domain.Budget;
using ExceedERP.Web.Helpers;
using System.Web.Mvc;


namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetReport)]
    public class BudgetReportController : Controller
    {
        public ActionResult BudgetPlanByQuarter()
        {
            return View();
        }
        public ActionResult BudgetSummary()
        {
            return View();
        }
        public ActionResult BudgetAllocationUsage()
        {
            return View();
        }
        

        public ActionResult BudgetTransferVsPlan()
        {
            return View();
        }
        public ActionResult ProfitLoss()
        {
            return View();
        }
        public ActionResult BudgetMonth()
        {
            return View();
        }
      
        public ActionResult BudgetYear()
        {
            return View();
        }
        public ActionResult PlanVsActualWorkUnit()
        {
            return View();
        }
        public ActionResult PlanVsActualWorkUnitSummary()
        {
            return View();
        }
        public ActionResult TaskPlan()
        {
            return View();
        }

        public ActionResult Expense()
        {
            return View();
        }
        public ActionResult BalanceSheet()
        {
            return View();
        }
        public ActionResult InvestmentPlan()
        {
            return View();
        }
        public ActionResult Revenue()
        {
            return View();
        }
        public ActionResult BudgetPlanReportView()
        {
            return View();
        }
        //remove these
        //public ActionResult ReportTest()
        //{
        //    return View();
        //}
        //public ActionResult BudgetPlanTestview()
        //{
        //    return View();
        //}
        ////add an action method that will call a view 
        //public ActionResult PlanTestViewR()
        //{
        //    return View();
        //}


    }
}