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
    public class IfrsSocfFinancingActivityRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSocfFinancingActivityRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSocfFinancingActivityRange> ifrssocffinancingactivityranges = _db.IfrsSocfFinancingActivityRanges.Where(x=>x.IfrsSocfFinancingActivityId==id);
            DataSourceResult result = ifrssocffinancingactivityranges.ToDataSourceResult(request, ifrsSocfFinancingActivityRange => new {
                IfrsSocfFinancingActivityId=id,
                IfrsSocfFinancingActivityRangeId = ifrsSocfFinancingActivityRange.IfrsSocfFinancingActivityRangeId,
                AccountFrom = ifrsSocfFinancingActivityRange.AccountFrom,
                AccountTo = ifrsSocfFinancingActivityRange.AccountTo,
                DateCreated = ifrsSocfFinancingActivityRange.DateCreated,
                CreatedBy = ifrsSocfFinancingActivityRange.CreatedBy,
                LastModified = ifrsSocfFinancingActivityRange.LastModified,
                ModifiedBy = ifrsSocfFinancingActivityRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfFinancingActivityRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfFinancingActivityRange ifrsSocfFinancingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfFinancingActivityRange
                {
                    IfrsSocfFinancingActivityId = id,
                    AccountFrom = ifrsSocfFinancingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfFinancingActivityRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsSocfFinancingActivityRange.LastModified,
                    ModifiedBy = ifrsSocfFinancingActivityRange.ModifiedBy,
                };

                _db.IfrsSocfFinancingActivityRanges.Add(entity);
                _db.SaveChanges();
                ifrsSocfFinancingActivityRange.IfrsSocfFinancingActivityRangeId = entity.IfrsSocfFinancingActivityRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financing activity range", "Success", User.Identity.Name, $"created range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfFinancingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfFinancingActivityRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfFinancingActivityRange ifrsSocfFinancingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfFinancingActivityRange
                {
                    IfrsSocfFinancingActivityId = id,
                    IfrsSocfFinancingActivityRangeId = ifrsSocfFinancingActivityRange.IfrsSocfFinancingActivityRangeId,
                    AccountFrom = ifrsSocfFinancingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfFinancingActivityRange.AccountTo,
                    DateCreated = ifrsSocfFinancingActivityRange.DateCreated,
                    CreatedBy = ifrsSocfFinancingActivityRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsSocfFinancingActivityRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financing activity range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfFinancingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfFinancingActivityRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSocfFinancingActivityRange ifrsSocfFinancingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfFinancingActivityRange
                {
                    IfrsSocfFinancingActivityRangeId = ifrsSocfFinancingActivityRange.IfrsSocfFinancingActivityRangeId,
                    AccountFrom = ifrsSocfFinancingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfFinancingActivityRange.AccountTo
                };

                _db.IfrsSocfFinancingActivityRanges.Attach(entity);
                _db.IfrsSocfFinancingActivityRanges.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted financial activity account range from {ifrsSocfFinancingActivityRange.AccountFrom} to {ifrsSocfFinancingActivityRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsSocfFinancingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
