using ExceedERP.Core.Domain.Budget;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     


namespace ExceedERP.Web.Areas.BudgetGAAP.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetReport)]
    public class BudgetGAAPReportController : Controller
    {
        
        public ActionResult Expense()
        {
            return View();
        }
        public ActionResult BalanceSheet()
        {
            return View();
        }

        public ActionResult ProfitLoss()
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