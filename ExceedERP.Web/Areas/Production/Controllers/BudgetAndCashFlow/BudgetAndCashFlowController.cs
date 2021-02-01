using ExceedERP.DataAccess.Context;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.BudgetAndCashFlow
{
    public class BudgetAndCashFlowController : Controller
    {
        private ExceedDbContext db;

        public BudgetAndCashFlowController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult ProjectedCashFlow()
        {

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();

            ViewData["FilscalYears"] = db.GLFiscalYears.Select(y => new { y.GlFiscalYearId, y.Name }).ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(pp => new { pp.ProductionPeriodId, pp.Name }).ToList();
            return View();
        }
        public ActionResult CleanedSale()
        {
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();

            ViewData["FilscalYears"] = db.GLFiscalYears.Select(y => new { y.GlFiscalYearId, y.Name }).ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(pp => new { pp.ProductionPeriodId, pp.Name }).ToList();
            return View();
        }
        public ActionResult RawPurchase()
        {
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();

            ViewData["FilscalYears"] = db.GLFiscalYears.Select(y => new { y.GlFiscalYearId, y.Name }).ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(pp => new { pp.ProductionPeriodId, pp.Name }).ToList();
            return View();
        }
        public ActionResult CustomClean()
        {
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();

            ViewData["FilscalYears"] = db.GLFiscalYears.Select(y => new { y.GlFiscalYearId, y.Name }).ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            ViewData["ProductionPeriods"] = db.ProductionPeriods.Select(pp => new { pp.ProductionPeriodId, pp.Name }).ToList();
            return View();
        }
        public ActionResult Overhead()
        {
            return View();
        }
    }
}