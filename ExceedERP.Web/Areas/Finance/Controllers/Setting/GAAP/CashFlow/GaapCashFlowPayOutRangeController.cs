using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.GAAP
{
    public class GaapCashFlowPayOutRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapCashFlowPayOutRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapCashFlowPayOutRange> gaapCashFlowPayOutRanges = _db.GaapCashFlowPayOutRanges.Where(x=>x.GaapCashFlowPayOutId==id);
            DataSourceResult result = gaapCashFlowPayOutRanges.ToDataSourceResult(request, gaapCashFlowPayOutRange => new {
                GaapCashFlowPayOutRangeId = gaapCashFlowPayOutRange.GaapCashFlowPayOutRangeId,
                GaapCashFlowPayOutId = gaapCashFlowPayOutRange.GaapCashFlowPayOutId,
                AccountFrom = gaapCashFlowPayOutRange.AccountFrom,
                AccountTo = gaapCashFlowPayOutRange.AccountTo,
                DateCreated = gaapCashFlowPayOutRange.DateCreated,
                CreatedBy = gaapCashFlowPayOutRange.CreatedBy,
                LastModified = gaapCashFlowPayOutRange.LastModified,
                ModifiedBy = gaapCashFlowPayOutRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowPayOutRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapCashFlowPayOutRange gaapCashFlowPayOutRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowPayOutRange
                {
                    GaapCashFlowPayOutId=id,
                    AccountFrom = gaapCashFlowPayOutRange.AccountFrom,
                    AccountTo = gaapCashFlowPayOutRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = gaapCashFlowPayOutRange.LastModified,
                    ModifiedBy = gaapCashFlowPayOutRange.ModifiedBy,
                };

                _db.GaapCashFlowPayOutRanges.Add(entity);
                _db.SaveChanges();
                gaapCashFlowPayOutRange.GaapCashFlowPayOutRangeId = entity.GaapCashFlowPayOutRangeId;
            }

            return Json(new[] { gaapCashFlowPayOutRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowPayOutRanges_Update([DataSourceRequest]DataSourceRequest request, GaapCashFlowPayOutRange gaapCashFlowPayOutRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowPayOutRange
                {
                    GaapCashFlowPayOutId = gaapCashFlowPayOutRange.GaapCashFlowPayOutId,
                    GaapCashFlowPayOutRangeId = gaapCashFlowPayOutRange.GaapCashFlowPayOutRangeId,
                    AccountFrom = gaapCashFlowPayOutRange.AccountFrom,
                    AccountTo = gaapCashFlowPayOutRange.AccountTo,
                    DateCreated = gaapCashFlowPayOutRange.DateCreated,
                    CreatedBy = gaapCashFlowPayOutRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.GaapCashFlowPayOutRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { gaapCashFlowPayOutRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowPayOutRanges_Destroy([DataSourceRequest]DataSourceRequest request, GaapCashFlowPayOutRange gaapCashFlowPayOutRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowPayOutRange
                {
                    GaapCashFlowPayOutId = gaapCashFlowPayOutRange.GaapCashFlowPayOutId,
                    GaapCashFlowPayOutRangeId = gaapCashFlowPayOutRange.GaapCashFlowPayOutRangeId,
                    AccountFrom = gaapCashFlowPayOutRange.AccountFrom,
                    AccountTo = gaapCashFlowPayOutRange.AccountTo
                };

                _db.GaapCashFlowPayOutRanges.Attach(entity);
                _db.GaapCashFlowPayOutRanges.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap cash flow payout account range from {gaapCashFlowPayOutRange.AccountFrom} to {gaapCashFlowPayOutRange.AccountTo}");
                #endregion
            }

            return Json(new[] { gaapCashFlowPayOutRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
