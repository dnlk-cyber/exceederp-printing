using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.GAAP
{
    public class GaapIncomeStatementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = db.GaapIncomeStatements.Count();
            return View();
        }

        public ActionResult GaapIncomeStatements_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapIncomeStatement> gaapincomestatements = db.GaapIncomeStatements;
            DataSourceResult result = gaapincomestatements.ToDataSourceResult(request, gaapIncomeStatement => new {
                GaapIncomeStatementId = gaapIncomeStatement.GaapIncomeStatementId,
                ReportType = gaapIncomeStatement.ReportType,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatements_Create([DataSourceRequest]DataSourceRequest request, GaapIncomeStatement gaapIncomeStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatement
                {
                    ReportType = gaapIncomeStatement.ReportType,
                };

                db.GaapIncomeStatements.Add(entity);
                db.SaveChanges();
                gaapIncomeStatement.GaapIncomeStatementId = entity.GaapIncomeStatementId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement ", "Success", User.Identity.Name, $"created rule {entity.GaapIncomeStatementId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatements_Update([DataSourceRequest]DataSourceRequest request, GaapIncomeStatement gaapIncomeStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatement
                {
                    GaapIncomeStatementId = gaapIncomeStatement.GaapIncomeStatementId,
                    ReportType = gaapIncomeStatement.ReportType,
                };

                db.GaapIncomeStatements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement ", "Success", User.Identity.Name, $"updated rule {entity.GaapIncomeStatementId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatements_Destroy([DataSourceRequest]DataSourceRequest request, GaapIncomeStatement gaapIncomeStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatement
                {
                    GaapIncomeStatementId = gaapIncomeStatement.GaapIncomeStatementId,
                };

                db.GaapIncomeStatements.Attach(entity);
                db.GaapIncomeStatements.Remove(entity);
                db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap income statement {gaapIncomeStatement.GaapIncomeStatementId}");
                #endregion
            }

            return Json(new[] { gaapIncomeStatement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
