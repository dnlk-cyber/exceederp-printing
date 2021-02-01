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
    public class IfrsSocfOperatingActivityRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSocfOperatingActivityRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSocfOperatingActivityRange> ifrssocfoperatingactivityranges = _db.IfrsSocfOperatingActivityRanges.Where(x=>x.IfrsSocfOperatingActivityId==id);
            DataSourceResult result = ifrssocfoperatingactivityranges.ToDataSourceResult(request, ifrsSocfOperatingActivityRange => new {
                IfrsSocfOperatingActivityId=id,
                IfrsSocfOperatingActivityRangeId = ifrsSocfOperatingActivityRange.IfrsSocfOperatingActivityRangeId,
                AccountFrom = ifrsSocfOperatingActivityRange.AccountFrom,
                AccountTo = ifrsSocfOperatingActivityRange.AccountTo,
                DateCreated = ifrsSocfOperatingActivityRange.DateCreated,
                CreatedBy = ifrsSocfOperatingActivityRange.CreatedBy,
                LastModified = ifrsSocfOperatingActivityRange.LastModified,
                ModifiedBy = ifrsSocfOperatingActivityRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfOperatingActivityRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfOperatingActivityRange ifrsSocfOperatingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfOperatingActivityRange
                {
                    IfrsSocfOperatingActivityId = id,
                    AccountFrom = ifrsSocfOperatingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfOperatingActivityRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsSocfOperatingActivityRange.LastModified,
                    ModifiedBy = ifrsSocfOperatingActivityRange.ModifiedBy,
                };

                _db.IfrsSocfOperatingActivityRanges.Add(entity);
                _db.SaveChanges();
                ifrsSocfOperatingActivityRange.IfrsSocfOperatingActivityRangeId = entity.IfrsSocfOperatingActivityRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup operating activity range", "Success", User.Identity.Name, $"created range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfOperatingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfOperatingActivityRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfOperatingActivityRange ifrsSocfOperatingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfOperatingActivityRange
                {
                    IfrsSocfOperatingActivityId = id,
                    IfrsSocfOperatingActivityRangeId = ifrsSocfOperatingActivityRange.IfrsSocfOperatingActivityRangeId,
                    AccountFrom = ifrsSocfOperatingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfOperatingActivityRange.AccountTo,
                    DateCreated = ifrsSocfOperatingActivityRange.DateCreated,
                    CreatedBy = ifrsSocfOperatingActivityRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsSocfOperatingActivityRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup operating activity range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfOperatingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfOperatingActivityRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSocfOperatingActivityRange ifrsSocfOperatingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfOperatingActivityRange
                {
                    IfrsSocfOperatingActivityRangeId = ifrsSocfOperatingActivityRange.IfrsSocfOperatingActivityRangeId,
                    AccountFrom = ifrsSocfOperatingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfOperatingActivityRange.AccountTo
                };

                _db.IfrsSocfOperatingActivityRanges.Attach(entity);
                _db.IfrsSocfOperatingActivityRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted operating activity account range from {ifrsSocfOperatingActivityRange.AccountFrom} to {ifrsSocfOperatingActivityRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsSocfOperatingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
