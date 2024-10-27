using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Report;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    public class RevenueByTypeListRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RevenueListRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RevenueListRange> revenueListRanges = _db.RevenueListRanges.Where(x=>x.RevenueListId==id);
            DataSourceResult result = revenueListRanges.ToDataSourceResult(request, revenueListRange => new {
                RevenueListId = revenueListRange.RevenueListId,
                RevenueListRangeId = revenueListRange.RevenueListRangeId,
                AccountFrom = revenueListRange.AccountFrom,
                AccountTo = revenueListRange.AccountTo,
                DateCreated = revenueListRange.DateCreated,
                CreatedBy = revenueListRange.CreatedBy,
                LastModified = revenueListRange.LastModified,
                ModifiedBy = revenueListRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueListRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, RevenueListRange revenueListRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueListRange
                {
                    RevenueListId=id,
                    AccountFrom = revenueListRange.AccountFrom,
                    AccountTo = revenueListRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = revenueListRange.LastModified,
                    ModifiedBy = revenueListRange.ModifiedBy,
                };

                _db.RevenueListRanges.Add(entity);
                _db.SaveChanges();
                revenueListRange.RevenueListRangeId = entity.RevenueListRangeId;


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue by type list range", "Success", User.Identity.Name, $"created revenue by sales type range {entity.AccountFrom} to {entity.AccountTo} under type list{entity.RevenueListId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueListRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueListRanges_Update([DataSourceRequest]DataSourceRequest request, RevenueListRange revenueListRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueListRange
                {
                    RevenueListId = revenueListRange.RevenueListId,
                    RevenueListRangeId = revenueListRange.RevenueListRangeId,
                    AccountFrom = revenueListRange.AccountFrom,
                    AccountTo = revenueListRange.AccountTo,
                    DateCreated = revenueListRange.DateCreated,
                    CreatedBy = revenueListRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.RevenueListRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue by type list range", "Success", User.Identity.Name, $"updated revenue by sales type range {entity.AccountFrom} to {entity.AccountTo} under type list{entity.RevenueListId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueListRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueListRanges_Destroy([DataSourceRequest]DataSourceRequest request, RevenueListRange revenueListRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueListRange
                {
                    RevenueListRangeId = revenueListRange.RevenueListRangeId,
                    AccountFrom = revenueListRange.AccountFrom,
                    AccountTo = revenueListRange.AccountTo
                };

                _db.RevenueListRanges.Attach(entity);
                _db.RevenueListRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted revenue type list account range from {revenueListRange.AccountFrom} to {revenueListRange.AccountTo}");
                #endregion
            }

            return Json(new[] { revenueListRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
