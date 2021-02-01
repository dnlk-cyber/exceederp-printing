using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Report.IFRS;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.IFRS
{
    public class IfrsScoiContinuingOperationRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsScoiContinuingOperationRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsScoiContinuingOperationRange> ifrsscoicontinuingoperationranges = _db.IfrsScoiContinuingOperationRanges.Where(x=>x.IfrsScoiContinuingOperationId==id);
            DataSourceResult result = ifrsscoicontinuingoperationranges.ToDataSourceResult(request, ifrsScoiContinuingOperationRange => new {
                IfrsScoiContinuingOperationId=id,
                IfrsScoiContinuingOperationRangeId = ifrsScoiContinuingOperationRange.IfrsScoiContinuingOperationRangeId,
                AccountFrom = ifrsScoiContinuingOperationRange.AccountFrom,
                AccountTo = ifrsScoiContinuingOperationRange.AccountTo,
                DateCreated = ifrsScoiContinuingOperationRange.DateCreated,
                CreatedBy = ifrsScoiContinuingOperationRange.CreatedBy,
                LastModified = ifrsScoiContinuingOperationRange.LastModified,
                ModifiedBy = ifrsScoiContinuingOperationRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiContinuingOperationRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsScoiContinuingOperationRange ifrsScoiContinuingOperationRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiContinuingOperationRange
                {
                    IfrsScoiContinuingOperationId = id,
                    AccountFrom = ifrsScoiContinuingOperationRange.AccountFrom,
                    AccountTo = ifrsScoiContinuingOperationRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsScoiContinuingOperationRange.LastModified,
                    ModifiedBy = ifrsScoiContinuingOperationRange.ModifiedBy,
                };

                _db.IfrsScoiContinuingOperationRanges.Add(entity);
                _db.SaveChanges();
                ifrsScoiContinuingOperationRange.IfrsScoiContinuingOperationRangeId = entity.IfrsScoiContinuingOperationRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Continuing operation range", "Success", User.Identity.Name, $"created range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiContinuingOperationRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiContinuingOperationRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsScoiContinuingOperationRange ifrsScoiContinuingOperationRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiContinuingOperationRange
                {
                    IfrsScoiContinuingOperationId = id,
                    IfrsScoiContinuingOperationRangeId = ifrsScoiContinuingOperationRange.IfrsScoiContinuingOperationRangeId,
                    AccountFrom = ifrsScoiContinuingOperationRange.AccountFrom,
                    AccountTo = ifrsScoiContinuingOperationRange.AccountTo,
                    DateCreated = ifrsScoiContinuingOperationRange.DateCreated,
                    CreatedBy = ifrsScoiContinuingOperationRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsScoiContinuingOperationRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Continuing operation range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiContinuingOperationRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiContinuingOperationRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsScoiContinuingOperationRange ifrsScoiContinuingOperationRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiContinuingOperationRange
                {
                    IfrsScoiContinuingOperationRangeId = ifrsScoiContinuingOperationRange.IfrsScoiContinuingOperationRangeId,
                    AccountFrom = ifrsScoiContinuingOperationRange.AccountFrom,
                    AccountTo = ifrsScoiContinuingOperationRange.AccountTo
                };

                _db.IfrsScoiContinuingOperationRanges.Attach(entity);
                _db.IfrsScoiContinuingOperationRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted continuing operations account range from {ifrsScoiContinuingOperationRange.AccountFrom} to {ifrsScoiContinuingOperationRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsScoiContinuingOperationRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
