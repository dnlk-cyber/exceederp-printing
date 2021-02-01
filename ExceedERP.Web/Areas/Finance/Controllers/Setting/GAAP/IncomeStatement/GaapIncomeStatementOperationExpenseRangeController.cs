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
    public class GaapIncomeStatementOperationExpenseRangeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapIncomeStatementOperationExpenseRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapIncomeStatementOperationExpenseRange> gaapincomestatementoperationexpenseranges = db.GaapIncomeStatementOperationExpenseRanges.Where(x=>x.GaapIncomeStatementOperationExpenseId==id);
            DataSourceResult result = gaapincomestatementoperationexpenseranges.ToDataSourceResult(request, gaapIncomeStatementOperationExpenseRange => new {
                GaapIncomeStatementOperationExpenseId = gaapIncomeStatementOperationExpenseRange.GaapIncomeStatementOperationExpenseId,
                GaapIncomeStatementOperationExpenseRangeId = gaapIncomeStatementOperationExpenseRange.GaapIncomeStatementOperationExpenseRangeId,
                AccountFrom = gaapIncomeStatementOperationExpenseRange.AccountFrom,
                AccountTo = gaapIncomeStatementOperationExpenseRange.AccountTo,
                DateCreated = gaapIncomeStatementOperationExpenseRange.DateCreated,
                CreatedBy = gaapIncomeStatementOperationExpenseRange.CreatedBy,
                LastModified = gaapIncomeStatementOperationExpenseRange.LastModified,
                ModifiedBy = gaapIncomeStatementOperationExpenseRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationExpenseRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationExpenseRange gaapIncomeStatementOperationExpenseRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationExpenseRange
                {
                    GaapIncomeStatementOperationExpenseId=id,
                    AccountFrom = gaapIncomeStatementOperationExpenseRange.AccountFrom,
                    AccountTo = gaapIncomeStatementOperationExpenseRange.AccountTo,
                    DateCreated =DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = gaapIncomeStatementOperationExpenseRange.LastModified,
                    ModifiedBy = gaapIncomeStatementOperationExpenseRange.ModifiedBy,
                };

                db.GaapIncomeStatementOperationExpenseRanges.Add(entity);
                db.SaveChanges();
                gaapIncomeStatementOperationExpenseRange.GaapIncomeStatementOperationExpenseRangeId = entity.GaapIncomeStatementOperationExpenseRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational expense range", "Success", User.Identity.Name, $"created range {entity.AccountFrom} under {entity.AccountTo} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationExpenseRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationExpenseRanges_Update([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationExpenseRange gaapIncomeStatementOperationExpenseRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationExpenseRange
                {
                    GaapIncomeStatementOperationExpenseId = gaapIncomeStatementOperationExpenseRange.GaapIncomeStatementOperationExpenseId,
                    GaapIncomeStatementOperationExpenseRangeId = gaapIncomeStatementOperationExpenseRange.GaapIncomeStatementOperationExpenseRangeId,
                    AccountFrom = gaapIncomeStatementOperationExpenseRange.AccountFrom,
                    AccountTo = gaapIncomeStatementOperationExpenseRange.AccountTo,
                    DateCreated = gaapIncomeStatementOperationExpenseRange.DateCreated,
                    CreatedBy = gaapIncomeStatementOperationExpenseRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                db.GaapIncomeStatementOperationExpenseRanges.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational expense range", "Success", User.Identity.Name, $"updated range {entity.AccountFrom} under {entity.AccountTo} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationExpenseRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationExpenseRanges_Destroy([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationExpenseRange gaapIncomeStatementOperationExpenseRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationExpenseRange
                {
                    GaapIncomeStatementOperationExpenseRangeId = gaapIncomeStatementOperationExpenseRange.GaapIncomeStatementOperationExpenseRangeId,
                    AccountFrom = gaapIncomeStatementOperationExpenseRange.AccountFrom,
                    AccountTo = gaapIncomeStatementOperationExpenseRange.AccountTo
                };

                db.GaapIncomeStatementOperationExpenseRanges.Attach(entity);
                db.GaapIncomeStatementOperationExpenseRanges.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap income statement operational expense account range from {gaapIncomeStatementOperationExpenseRange.AccountFrom} to {gaapIncomeStatementOperationExpenseRange.AccountTo}");
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationExpenseRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
