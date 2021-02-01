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
    public class GaapCashFlowReceiptRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapCashFlowReceiptRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapCashFlowReceiptRange> gaapCashFlowReceiptRanges = _db.GaapCashFlowReceiptRanges.Where(x=>x.GaapCashFlowReceiptId==id);
            DataSourceResult result = gaapCashFlowReceiptRanges.ToDataSourceResult(request, gaapCashFlowReceiptRange => new {
                GaapCashFlowReceiptRangeId = gaapCashFlowReceiptRange.GaapCashFlowReceiptRangeId,
                GaapCashFlowReceiptId = gaapCashFlowReceiptRange.GaapCashFlowReceiptId,
                AccountFrom = gaapCashFlowReceiptRange.AccountFrom,
                AccountTo = gaapCashFlowReceiptRange.AccountTo,
                DateCreated = gaapCashFlowReceiptRange.DateCreated,
                CreatedBy = gaapCashFlowReceiptRange.CreatedBy,
                LastModified = gaapCashFlowReceiptRange.LastModified,
                ModifiedBy = gaapCashFlowReceiptRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowReceiptRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapCashFlowReceiptRange gaapCashFlowReceiptRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowReceiptRange
                {
                    GaapCashFlowReceiptId= id,
                    AccountFrom = gaapCashFlowReceiptRange.AccountFrom,
                    AccountTo = gaapCashFlowReceiptRange.AccountTo,
                    DateCreated =DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = gaapCashFlowReceiptRange.LastModified,
                    ModifiedBy = gaapCashFlowReceiptRange.ModifiedBy,
                };

                _db.GaapCashFlowReceiptRanges.Add(entity);
                _db.SaveChanges();
                gaapCashFlowReceiptRange.GaapCashFlowReceiptRangeId = entity.GaapCashFlowReceiptRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow receipt range", "Success", User.Identity.Name, $"created range {entity.AccountFrom} to {entity.AccountTo} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlowReceiptRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowReceiptRanges_Update([DataSourceRequest]DataSourceRequest request, GaapCashFlowReceiptRange gaapCashFlowReceiptRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowReceiptRange
                {
                    GaapCashFlowReceiptRangeId = gaapCashFlowReceiptRange.GaapCashFlowReceiptRangeId,
                    GaapCashFlowReceiptId = gaapCashFlowReceiptRange.GaapCashFlowReceiptId,
                    AccountFrom = gaapCashFlowReceiptRange.AccountFrom,
                    AccountTo = gaapCashFlowReceiptRange.AccountTo,
                    DateCreated = gaapCashFlowReceiptRange.DateCreated,
                    CreatedBy = gaapCashFlowReceiptRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.GaapCashFlowReceiptRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup cash flow receipt range", "Success", User.Identity.Name, $"updated range {entity.AccountFrom} to {entity.AccountTo} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapCashFlowReceiptRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapCashFlowReceiptRanges_Destroy([DataSourceRequest]DataSourceRequest request, GaapCashFlowReceiptRange gaapCashFlowReceiptRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapCashFlowReceiptRange
                {
                    GaapCashFlowReceiptRangeId = gaapCashFlowReceiptRange.GaapCashFlowReceiptRangeId,
                    GaapCashFlowReceiptId = gaapCashFlowReceiptRange.GaapCashFlowReceiptId,
                    AccountFrom = gaapCashFlowReceiptRange.AccountFrom,
                    AccountTo = gaapCashFlowReceiptRange.AccountTo,
                    DateCreated = gaapCashFlowReceiptRange.DateCreated,
                    CreatedBy = gaapCashFlowReceiptRange.CreatedBy,
                    LastModified = gaapCashFlowReceiptRange.LastModified,
                    ModifiedBy = gaapCashFlowReceiptRange.ModifiedBy,
                };

                _db.GaapCashFlowReceiptRanges.Attach(entity);
                _db.GaapCashFlowReceiptRanges.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap cash flow receipt account range from {gaapCashFlowReceiptRange.AccountFrom} to {gaapCashFlowReceiptRange.AccountTo}");
                #endregion
            }

            return Json(new[] { gaapCashFlowReceiptRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
