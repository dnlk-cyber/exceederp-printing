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
    public class IfrsSocfInvestingActivityRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSocfInvestingActivityRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSocfInvestingActivityRange> ifrssocfinvestingactivityranges = _db.IfrsSocfInvestingActivityRanges.Where(x=>x.IfrsSocfInvestingActivityId==id);
            DataSourceResult result = ifrssocfinvestingactivityranges.ToDataSourceResult(request, ifrsSocfInvestingActivityRange => new {
                IfrsSocfInvestingActivityId=id,
                IfrsSocfInvestingActivityRangeId = ifrsSocfInvestingActivityRange.IfrsSocfInvestingActivityRangeId,
                AccountFrom = ifrsSocfInvestingActivityRange.AccountFrom,
                AccountTo = ifrsSocfInvestingActivityRange.AccountTo,
                DateCreated = ifrsSocfInvestingActivityRange.DateCreated,
                CreatedBy = ifrsSocfInvestingActivityRange.CreatedBy,
                LastModified = ifrsSocfInvestingActivityRange.LastModified,
                ModifiedBy = ifrsSocfInvestingActivityRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfInvestingActivityRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfInvestingActivityRange ifrsSocfInvestingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfInvestingActivityRange
                {
                    IfrsSocfInvestingActivityId = id,
                    AccountFrom = ifrsSocfInvestingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfInvestingActivityRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsSocfInvestingActivityRange.LastModified,
                    ModifiedBy = ifrsSocfInvestingActivityRange.ModifiedBy,
                };

                _db.IfrsSocfInvestingActivityRanges.Add(entity);
                _db.SaveChanges();
                ifrsSocfInvestingActivityRange.IfrsSocfInvestingActivityRangeId = entity.IfrsSocfInvestingActivityRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup investing activity range", "Success", User.Identity.Name, $"created range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfInvestingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfInvestingActivityRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsSocfInvestingActivityRange ifrsSocfInvestingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfInvestingActivityRange
                {
                    IfrsSocfInvestingActivityId = id,
                    IfrsSocfInvestingActivityRangeId = ifrsSocfInvestingActivityRange.IfrsSocfInvestingActivityRangeId,
                    AccountFrom = ifrsSocfInvestingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfInvestingActivityRange.AccountTo,
                    DateCreated = ifrsSocfInvestingActivityRange.DateCreated,
                    CreatedBy = ifrsSocfInvestingActivityRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsSocfInvestingActivityRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup investing activity range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSocfInvestingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSocfInvestingActivityRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSocfInvestingActivityRange ifrsSocfInvestingActivityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSocfInvestingActivityRange
                {
                    IfrsSocfInvestingActivityRangeId = ifrsSocfInvestingActivityRange.IfrsSocfInvestingActivityRangeId,
                    AccountFrom = ifrsSocfInvestingActivityRange.AccountFrom,
                    AccountTo = ifrsSocfInvestingActivityRange.AccountTo
                };

                _db.IfrsSocfInvestingActivityRanges.Attach(entity);
                _db.IfrsSocfInvestingActivityRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted investing activity account range from {ifrsSocfInvestingActivityRange.AccountFrom} to {ifrsSocfInvestingActivityRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsSocfInvestingActivityRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
