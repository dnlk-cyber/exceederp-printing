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
    public class GaapBalanceSheetCapitalRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapBalanceSheetCapitalRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapBalanceSheetCapitalRange> gaapBalanceSheetCapitalRanges = _db.GaapBalanceSheetCapitalRanges.Where(x=>x.GaapBalanceSheetCapitalId==id);
            DataSourceResult result = gaapBalanceSheetCapitalRanges.ToDataSourceResult(request, gaapBalanceSheetCapitalRange => new {
                GaapBalanceSheetCapitalId = id,
                GaapBalanceSheetCapitalRangeId = gaapBalanceSheetCapitalRange.GaapBalanceSheetCapitalRangeId,
                AccountFrom = gaapBalanceSheetCapitalRange.AccountFrom,
                AccountTo = gaapBalanceSheetCapitalRange.AccountTo,
                DateCreated = gaapBalanceSheetCapitalRange.DateCreated,
                CreatedBy = gaapBalanceSheetCapitalRange.CreatedBy,
                LastModified = gaapBalanceSheetCapitalRange.LastModified,
                ModifiedBy = gaapBalanceSheetCapitalRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetCapitalRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapBalanceSheetCapitalRange gaapBalanceSheetCapitalRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetCapitalRange
                {
                    GaapBalanceSheetCapitalId = id,
                    AccountFrom = gaapBalanceSheetCapitalRange.AccountFrom,
                    AccountTo = gaapBalanceSheetCapitalRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = gaapBalanceSheetCapitalRange.LastModified,
                    ModifiedBy = gaapBalanceSheetCapitalRange.ModifiedBy,
                };

                _db.GaapBalanceSheetCapitalRanges.Add(entity);
                _db.SaveChanges();
                gaapBalanceSheetCapitalRange.GaapBalanceSheetCapitalRangeId = entity.GaapBalanceSheetCapitalRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet capital range", "Success", User.Identity.Name, $"created range {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetCapitalRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetCapitalRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, GaapBalanceSheetCapitalRange gaapBalanceSheetCapitalRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetCapitalRange
                {
                    GaapBalanceSheetCapitalId = gaapBalanceSheetCapitalRange.GaapBalanceSheetCapitalId,
                    GaapBalanceSheetCapitalRangeId = gaapBalanceSheetCapitalRange.GaapBalanceSheetCapitalRangeId,
                    AccountFrom = gaapBalanceSheetCapitalRange.AccountFrom,
                    AccountTo = gaapBalanceSheetCapitalRange.AccountTo,
                    DateCreated = gaapBalanceSheetCapitalRange.DateCreated,
                    CreatedBy = gaapBalanceSheetCapitalRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.GaapBalanceSheetCapitalRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet capital range", "Success", User.Identity.Name, $"updated range {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetCapitalRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetCapitalRanges_Destroy([DataSourceRequest]DataSourceRequest request, GaapBalanceSheetCapitalRange gaapBalanceSheetCapitalRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetCapitalRange
                {
                    GaapBalanceSheetCapitalRangeId = gaapBalanceSheetCapitalRange.GaapBalanceSheetCapitalRangeId,
                    AccountFrom = gaapBalanceSheetCapitalRange.AccountFrom,
                    AccountTo = gaapBalanceSheetCapitalRange.AccountTo
                };

                _db.GaapBalanceSheetCapitalRanges.Attach(entity);
                _db.GaapBalanceSheetCapitalRanges.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted balance sheet capital account range from {gaapBalanceSheetCapitalRange.AccountFrom} to {gaapBalanceSheetCapitalRange.AccountTo}");
                #endregion
            }

            return Json(new[] { gaapBalanceSheetCapitalRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
