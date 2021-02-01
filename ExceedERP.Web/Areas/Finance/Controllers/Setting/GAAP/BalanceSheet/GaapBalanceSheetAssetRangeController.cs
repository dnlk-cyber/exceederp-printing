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
    public class GaapBalanceSheetAssetRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapBalanceSheetAssetRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapBalanceSheetAssetRange> gaapBalanceSheetAssetRanges = _db.GaapBalanceSheetAssetRanges.Where(x=>x.GaapBalanceSheetAssetId==id);
            DataSourceResult result = gaapBalanceSheetAssetRanges.ToDataSourceResult(request, gaapBalanceSheetAssetRange => new {
                GaapBalanceSheetAssetId = id,
                GaapBalanceSheetAssetRangeId = gaapBalanceSheetAssetRange.GaapBalanceSheetAssetRangeId,
                AccountFrom = gaapBalanceSheetAssetRange.AccountFrom,
                AccountTo = gaapBalanceSheetAssetRange.AccountTo,
                DateCreated = gaapBalanceSheetAssetRange.DateCreated,
                CreatedBy = gaapBalanceSheetAssetRange.CreatedBy,
                LastModified = gaapBalanceSheetAssetRange.LastModified,
                ModifiedBy = gaapBalanceSheetAssetRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetAssetRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapBalanceSheetAssetRange gaapBalanceSheetAssetRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetAssetRange
                {
                    GaapBalanceSheetAssetId = id,
                    AccountFrom = gaapBalanceSheetAssetRange.AccountFrom,
                    AccountTo = gaapBalanceSheetAssetRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy =User.Identity.Name,
                    LastModified = gaapBalanceSheetAssetRange.LastModified,
                    ModifiedBy = gaapBalanceSheetAssetRange.ModifiedBy,
                };

                _db.GaapBalanceSheetAssetRanges.Add(entity);
                _db.SaveChanges();
                gaapBalanceSheetAssetRange.GaapBalanceSheetAssetRangeId = entity.GaapBalanceSheetAssetRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet asset range", "Success", User.Identity.Name, $"created range {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetAssetRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetAssetRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, GaapBalanceSheetAssetRange gaapBalanceSheetAssetRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetAssetRange
                {
                    GaapBalanceSheetAssetId = gaapBalanceSheetAssetRange.GaapBalanceSheetAssetId,
                    GaapBalanceSheetAssetRangeId = gaapBalanceSheetAssetRange.GaapBalanceSheetAssetRangeId,
                    AccountFrom = gaapBalanceSheetAssetRange.AccountFrom,
                    AccountTo = gaapBalanceSheetAssetRange.AccountTo,
                    DateCreated = gaapBalanceSheetAssetRange.DateCreated,
                    CreatedBy = gaapBalanceSheetAssetRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.GaapBalanceSheetAssetRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet asset range", "Success", User.Identity.Name, $"updated range {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetAssetRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetAssetRanges_Destroy([DataSourceRequest]DataSourceRequest request, GaapBalanceSheetAssetRange gaapBalanceSheetAssetRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetAssetRange
                {
                    GaapBalanceSheetAssetRangeId = gaapBalanceSheetAssetRange.GaapBalanceSheetAssetRangeId,
                    AccountFrom = gaapBalanceSheetAssetRange.AccountFrom,
                    AccountTo = gaapBalanceSheetAssetRange.AccountTo
                };

                _db.GaapBalanceSheetAssetRanges.Attach(entity);
                _db.GaapBalanceSheetAssetRanges.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted balance sheet asset account range from {gaapBalanceSheetAssetRange.AccountFrom} to {gaapBalanceSheetAssetRange.AccountTo}");
                #endregion
            }

            return Json(new[] { gaapBalanceSheetAssetRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
