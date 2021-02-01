using ExceedERP.Core.Domain.Budget;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     


namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetReport)]
    public class BudgetReportController : Controller
    {
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

    }
}