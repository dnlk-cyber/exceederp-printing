using System;
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
    public class GaapIncomeStatementOperationRevenueRangeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapIncomeStatementOperationRevenueRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapIncomeStatementOperationRevenueRange> gaapincomestatementoperationrevenueranges = db.GaapIncomeStatementOperationRevenueRanges.Where(x=>x.GaapIncomeStatementOperationRevenueId==id);
            DataSourceResult result = gaapincomestatementoperationrevenueranges.ToDataSourceResult(request, gaapIncomeStatementOperationRevenueRange => new {
                GaapIncomeStatementOperationRevenueId = gaapIncomeStatementOperationRevenueRange.GaapIncomeStatementOperationRevenueId,
                GaapIncomeStatementOperationRevenueRangeId = gaapIncomeStatementOperationRevenueRange.GaapIncomeStatementOperationRevenueRangeId,
                AccountFrom = gaapIncomeStatementOperationRevenueRange.AccountFrom,
                AccountTo = gaapIncomeStatementOperationRevenueRange.AccountTo,
                DateCreated = gaapIncomeStatementOperationRevenueRange.DateCreated,
                CreatedBy = gaapIncomeStatementOperationRevenueRange.CreatedBy,
                LastModified = gaapIncomeStatementOperationRevenueRange.LastModified,
                ModifiedBy = gaapIncomeStatementOperationRevenueRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationRevenueRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationRevenueRange gaapIncomeStatementOperationRevenueRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationRevenueRange
                {
                    GaapIncomeStatementOperationRevenueId= id,
                    AccountFrom = gaapIncomeStatementOperationRevenueRange.AccountFrom,
                    AccountTo = gaapIncomeStatementOperationRevenueRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = gaapIncomeStatementOperationRevenueRange.LastModified,
                    ModifiedBy = gaapIncomeStatementOperationRevenueRange.ModifiedBy,
                };

                db.GaapIncomeStatementOperationRevenueRanges.Add(entity);
                db.SaveChanges();
                gaapIncomeStatementOperationRevenueRange.GaapIncomeStatementOperationRevenueRangeId = entity.GaapIncomeStatementOperationRevenueRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational revenue range", "Success", User.Identity.Name, $"created range {entity.AccountFrom} under {entity.AccountTo} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationRevenueRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationRevenueRanges_Update([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationRevenueRange gaapIncomeStatementOperationRevenueRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationRevenueRange
                {
                    GaapIncomeStatementOperationRevenueRangeId = gaapIncomeStatementOperationRevenueRange.GaapIncomeStatementOperationRevenueRangeId,
                    GaapIncomeStatementOperationRevenueId = gaapIncomeStatementOperationRevenueRange.GaapIncomeStatementOperationRevenueId,
                    AccountFrom = gaapIncomeStatementOperationRevenueRange.AccountFrom,
                    AccountTo = gaapIncomeStatementOperationRevenueRange.AccountTo,
                    DateCreated = gaapIncomeStatementOperationRevenueRange.DateCreated,
                    CreatedBy = gaapIncomeStatementOperationRevenueRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                db.GaapIncomeStatementOperationRevenueRanges.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational revenue range", "Success", User.Identity.Name, $"updated range {entity.AccountFrom} under {entity.AccountTo} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationRevenueRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationRevenueRanges_Destroy([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationRevenueRange gaapIncomeStatementOperationRevenueRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationRevenueRange
                {
                    GaapIncomeStatementOperationRevenueRangeId = gaapIncomeStatementOperationRevenueRange.GaapIncomeStatementOperationRevenueRangeId,
                    AccountFrom = gaapIncomeStatementOperationRevenueRange.AccountFrom,
                    AccountTo = gaapIncomeStatementOperationRevenueRange.AccountTo
                };

                db.GaapIncomeStatementOperationRevenueRanges.Attach(entity);
                db.GaapIncomeStatementOperationRevenueRanges.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap income statement operational revenue account range from {gaapIncomeStatementOperationRevenueRange.AccountFrom} to {gaapIncomeStatementOperationRevenueRange.AccountTo}");
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationRevenueRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
