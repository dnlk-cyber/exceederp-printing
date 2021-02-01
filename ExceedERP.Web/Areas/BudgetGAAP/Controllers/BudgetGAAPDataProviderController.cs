using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.BudgetGAAP.Controllers
{
    public class BudgetGAAPDataProviderController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        [AllowAnonymous]
        public ActionResult BalanceSheetCategoryDDBind()
        {

            var BalanceSheetCategory = db.BalanceSheetCategories.Distinct().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BalanceSheetCategoryId.ToString()
            });

            return Json(BalanceSheetCategory, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProfitLossStatementCategoryDDBind()
        {

            var ProfitLossStatementCategory = db.ProfitLossStatementCategories.Distinct().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.ProfitLossStatementCategoryId.ToString()
            });

            return Json(ProfitLossStatementCategory, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult FiscalYearDDBind()
        {

            var FiscalYear = db.GLFiscalYears.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.GlFiscalYearId.ToString()
            });

            return Json(FiscalYear, JsonRequestBehavior.AllowGet);
        }
    }
}