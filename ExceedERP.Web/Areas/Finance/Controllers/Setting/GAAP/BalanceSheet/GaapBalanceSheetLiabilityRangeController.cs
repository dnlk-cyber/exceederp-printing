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
    public class GaapBalanceSheetLiabilityRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapBalanceSheetLiabilityRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapBalanceSheetLiabilityRange> gaapBalanceSheetLiabilityRanges = _db.GaapBalanceSheetLiabilityRanges.Where(x=>x.GaapBalanceSheetLiabilityId==id);
            DataSourceResult result = gaapBalanceSheetLiabilityRanges.ToDataSourceResult(request, gaapBalanceSheetLiabilityRange => new {
                GaapBalanceSheetLiabilityId = id,
                GaapBalanceSheetLiabilityRangeId = gaapBalanceSheetLiabilityRange.GaapBalanceSheetLiabilityRangeId,
                AccountFrom = gaapBalanceSheetLiabilityRange.AccountFrom,
                AccountTo = gaapBalanceSheetLiabilityRange.AccountTo,
                DateCreated = gaapBalanceSheetLiabilityRange.DateCreated,
                CreatedBy = gaapBalanceSheetLiabilityRange.CreatedBy,
                LastModified = gaapBalanceSheetLiabilityRange.LastModified,
                ModifiedBy = gaapBalanceSheetLiabilityRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetLiabilityRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapBalanceSheetLiabilityRange gaapBalanceSheetLiabilityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetLiabilityRange
                {
                    GaapBalanceSheetLiabilityId = id,
                    AccountFrom = gaapBalanceSheetLiabilityRange.AccountFrom,
                    AccountTo = gaapBalanceSheetLiabilityRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = gaapBalanceSheetLiabilityRange.LastModified,
                    ModifiedBy = gaapBalanceSheetLiabilityRange.ModifiedBy,
                };

                _db.GaapBalanceSheetLiabilityRanges.Add(entity);
                _db.SaveChanges();
                gaapBalanceSheetLiabilityRange.GaapBalanceSheetLiabilityRangeId = entity.GaapBalanceSheetLiabilityRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet liability range", "Success", User.Identity.Name, $"created range {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetLiabilityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetLiabilityRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, GaapBalanceSheetLiabilityRange gaapBalanceSheetLiabilityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetLiabilityRange
                {
                    GaapBalanceSheetLiabilityId = gaapBalanceSheetLiabilityRange.GaapBalanceSheetLiabilityId,
                    GaapBalanceSheetLiabilityRangeId = gaapBalanceSheetLiabilityRange.GaapBalanceSheetLiabilityRangeId,
                    AccountFrom = gaapBalanceSheetLiabilityRange.AccountFrom,
                    AccountTo = gaapBalanceSheetLiabilityRange.AccountTo,
                    DateCreated = gaapBalanceSheetLiabilityRange.DateCreated,
                    CreatedBy = gaapBalanceSheetLiabilityRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.GaapBalanceSheetLiabilityRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet liability range", "Success", User.Identity.Name, $"updated range {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetLiabilityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetLiabilityRanges_Destroy([DataSourceRequest]DataSourceRequest request, GaapBalanceSheetLiabilityRange gaapBalanceSheetLiabilityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetLiabilityRange
                {
                    GaapBalanceSheetLiabilityRangeId = gaapBalanceSheetLiabilityRange.GaapBalanceSheetLiabilityRangeId,
                    AccountFrom = gaapBalanceSheetLiabilityRange.AccountFrom,
                    AccountTo = gaapBalanceSheetLiabilityRange.AccountTo
                };

                _db.GaapBalanceSheetLiabilityRanges.Attach(entity);
                _db.GaapBalanceSheetLiabilityRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted balance sheet liability account range from {gaapBalanceSheetLiabilityRange.AccountFrom} to {gaapBalanceSheetLiabilityRange.AccountTo}");
                #endregion
            }

            return Json(new[] { gaapBalanceSheetLiabilityRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
