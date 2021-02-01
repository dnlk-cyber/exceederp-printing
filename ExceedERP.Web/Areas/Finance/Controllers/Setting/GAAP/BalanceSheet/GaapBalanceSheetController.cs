using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.GAAP
{
     [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class GaapBalanceSheetController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.GapBalanceSheets.Count();
            return View();
        }

        public ActionResult GaapBalanceSheets_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapBalanceSheet> gaapBalanceSheets = _db.GapBalanceSheets;
            DataSourceResult result = gaapBalanceSheets.ToDataSourceResult(request, gaapBalanceSheet => new {
                GaapBalanceSheetId = gaapBalanceSheet.GaapBalanceSheetId,
                ReportType = gaapBalanceSheet.ReportType,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheets_Create([DataSourceRequest]DataSourceRequest request, GaapBalanceSheet gaapBalanceSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheet
                {
                    ReportType = gaapBalanceSheet.ReportType,
                };

                _db.GapBalanceSheets.Add(entity);
                _db.SaveChanges();
                gaapBalanceSheet.GaapBalanceSheetId = entity.GaapBalanceSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet ", "Success", User.Identity.Name, $"created rule {entity.GaapBalanceSheetId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheets_Update([DataSourceRequest]DataSourceRequest request, GaapBalanceSheet gaapBalanceSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheet
                {
                    GaapBalanceSheetId = gaapBalanceSheet.GaapBalanceSheetId,
                    ReportType = gaapBalanceSheet.ReportType,
                };

                _db.GapBalanceSheets.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet ", "Success", User.Identity.Name, $"updated rule {entity.GaapBalanceSheetId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheets_Destroy([DataSourceRequest]DataSourceRequest request, GaapBalanceSheet gaapBalanceSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheet
                {
                    GaapBalanceSheetId = gaapBalanceSheet.GaapBalanceSheetId
                };

                _db.GapBalanceSheets.Attach(entity);
                _db.GapBalanceSheets.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted balance sheet {gaapBalanceSheet.GaapBalanceSheetId}");
                #endregion
            }

            return Json(new[] { gaapBalanceSheet }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
