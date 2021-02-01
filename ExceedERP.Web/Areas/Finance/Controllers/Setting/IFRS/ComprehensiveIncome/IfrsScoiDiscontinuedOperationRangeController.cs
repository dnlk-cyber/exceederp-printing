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
    public class IfrsScoiDiscontinuedOperationRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsScoiDiscontinuedOperationRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsScoiDiscontinuedOperationRange> ifrsscoidiscontinuedoperationranges = _db.IfrsScoiDiscontinuedOperationRanges.Where(x=>x.IfrsScoiDiscontinuedOperationId==id);
            DataSourceResult result = ifrsscoidiscontinuedoperationranges.ToDataSourceResult(request, ifrsScoiDiscontinuedOperationRange => new {
                IfrsScoiDiscontinuedOperationId=id,
                IfrsScoiDiscontinuedOperationRangeId = ifrsScoiDiscontinuedOperationRange.IfrsScoiDiscontinuedOperationRangeId,
                AccountFrom = ifrsScoiDiscontinuedOperationRange.AccountFrom,
                AccountTo = ifrsScoiDiscontinuedOperationRange.AccountTo,
                DateCreated = ifrsScoiDiscontinuedOperationRange.DateCreated,
                CreatedBy = ifrsScoiDiscontinuedOperationRange.CreatedBy,
                LastModified = ifrsScoiDiscontinuedOperationRange.LastModified,
                ModifiedBy = ifrsScoiDiscontinuedOperationRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiDiscontinuedOperationRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsScoiDiscontinuedOperationRange ifrsScoiDiscontinuedOperationRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiDiscontinuedOperationRange
                {
                    IfrsScoiDiscontinuedOperationId = id,
                    AccountFrom = ifrsScoiDiscontinuedOperationRange.AccountFrom,
                    AccountTo = ifrsScoiDiscontinuedOperationRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsScoiDiscontinuedOperationRange.LastModified,
                    ModifiedBy = ifrsScoiDiscontinuedOperationRange.ModifiedBy,
                };

                _db.IfrsScoiDiscontinuedOperationRanges.Add(entity);
                _db.SaveChanges();
                ifrsScoiDiscontinuedOperationRange.IfrsScoiDiscontinuedOperationRangeId = entity.IfrsScoiDiscontinuedOperationRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Discontinued operation range", "Success", User.Identity.Name, $"created range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiDiscontinuedOperationRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiDiscontinuedOperationRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsScoiDiscontinuedOperationRange ifrsScoiDiscontinuedOperationRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiDiscontinuedOperationRange
                {
                    IfrsScoiDiscontinuedOperationId = id,
                    IfrsScoiDiscontinuedOperationRangeId = ifrsScoiDiscontinuedOperationRange.IfrsScoiDiscontinuedOperationRangeId,
                    AccountFrom = ifrsScoiDiscontinuedOperationRange.AccountFrom,
                    AccountTo = ifrsScoiDiscontinuedOperationRange.AccountTo,
                    DateCreated = ifrsScoiDiscontinuedOperationRange.DateCreated,
                    CreatedBy = ifrsScoiDiscontinuedOperationRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsScoiDiscontinuedOperationRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Discontinued operation range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiDiscontinuedOperationRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiDiscontinuedOperationRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsScoiDiscontinuedOperationRange ifrsScoiDiscontinuedOperationRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiDiscontinuedOperationRange
                {
                    IfrsScoiDiscontinuedOperationRangeId = ifrsScoiDiscontinuedOperationRange.IfrsScoiDiscontinuedOperationRangeId,
                    AccountFrom = ifrsScoiDiscontinuedOperationRange.AccountFrom,
                    AccountTo = ifrsScoiDiscontinuedOperationRange.AccountTo
                };

                _db.IfrsScoiDiscontinuedOperationRanges.Attach(entity);
                _db.IfrsScoiDiscontinuedOperationRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted discontinuing operations account range from {ifrsScoiDiscontinuedOperationRange.AccountFrom} to {ifrsScoiDiscontinuedOperationRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsScoiDiscontinuedOperationRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
