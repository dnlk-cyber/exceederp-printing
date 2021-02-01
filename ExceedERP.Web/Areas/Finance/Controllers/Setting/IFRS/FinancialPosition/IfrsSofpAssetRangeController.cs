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
    public class IfrsSofpAssetRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSofpAssetRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSofpAssetRange> ifrssofpassetranges = _db.IfrsSofpAssetRanges.Where(x=>x.IfrsSofpAssetId==id);
            DataSourceResult result = ifrssofpassetranges.ToDataSourceResult(request, ifrsSofpAssetRange => new {
                IfrsSofpAssetId=id,
                IfrsSofpAssetRangeId = ifrsSofpAssetRange.IfrsSofpAssetRangeId,
                AccountFrom = ifrsSofpAssetRange.AccountFrom,
                AccountTo = ifrsSofpAssetRange.AccountTo,
                DateCreated = ifrsSofpAssetRange.DateCreated,
                CreatedBy = ifrsSofpAssetRange.CreatedBy,
                LastModified = ifrsSofpAssetRange.LastModified,
                ModifiedBy = ifrsSofpAssetRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpAssetRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSofpAssetRange ifrsSofpAssetRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpAssetRange
                {
                    IfrsSofpAssetId = id,
                    AccountFrom = ifrsSofpAssetRange.AccountFrom,
                    AccountTo = ifrsSofpAssetRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsSofpAssetRange.LastModified,
                    ModifiedBy = ifrsSofpAssetRange.ModifiedBy,
                };

                _db.IfrsSofpAssetRanges.Add(entity);
                _db.SaveChanges();
                ifrsSofpAssetRange.IfrsSofpAssetRangeId = entity.IfrsSofpAssetRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position asset range", "Success", User.Identity.Name, $"create range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpAssetRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpAssetRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSofpAssetRange ifrsSofpAssetRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpAssetRange
                {
                    IfrsSofpAssetId = id,
                    IfrsSofpAssetRangeId = ifrsSofpAssetRange.IfrsSofpAssetRangeId,
                    AccountFrom = ifrsSofpAssetRange.AccountFrom,
                    AccountTo = ifrsSofpAssetRange.AccountTo,
                    DateCreated = ifrsSofpAssetRange.DateCreated,
                    CreatedBy = ifrsSofpAssetRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy =User.Identity.Name
                };

                _db.IfrsSofpAssetRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position asset range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpAssetRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpAssetRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSofpAssetRange ifrsSofpAssetRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpAssetRange
                {
                    IfrsSofpAssetRangeId = ifrsSofpAssetRange.IfrsSofpAssetRangeId,
                    AccountFrom = ifrsSofpAssetRange.AccountFrom,
                    AccountTo = ifrsSofpAssetRange.AccountTo
                };

                _db.IfrsSofpAssetRanges.Attach(entity);
                _db.IfrsSofpAssetRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted financial position asset account range from {ifrsSofpAssetRange.AccountFrom} to {ifrsSofpAssetRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsSofpAssetRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
