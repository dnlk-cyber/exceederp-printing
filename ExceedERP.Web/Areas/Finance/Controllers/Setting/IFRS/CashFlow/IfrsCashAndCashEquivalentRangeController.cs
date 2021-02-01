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
    public class IfrsCashAndCashEquivalentRangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsCashAndCashEquivalentRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsCashAndCashEquivalentRange> IfrsCashAndCashEquivalentRanges = _db.IfrsCashAndCashEquivalentRanges.Where(x=>x.IfrsStatementOfCashFlowId==id);
            DataSourceResult result = IfrsCashAndCashEquivalentRanges.ToDataSourceResult(request, IfrsCashAndCashEquivalentRange => new {
                IfrsStatementOfCashFlowId=id,
                IfrsCashAndCashEquivalentRangeId = IfrsCashAndCashEquivalentRange.IfrsCashAndCashEquivalentRangeId,
                AccountFrom = IfrsCashAndCashEquivalentRange.AccountFrom,
                AccountTo = IfrsCashAndCashEquivalentRange.AccountTo,
                DateCreated = IfrsCashAndCashEquivalentRange.DateCreated,
                CreatedBy = IfrsCashAndCashEquivalentRange.CreatedBy,
                LastModified = IfrsCashAndCashEquivalentRange.LastModified,
                ModifiedBy = IfrsCashAndCashEquivalentRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsCashAndCashEquivalentRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsCashAndCashEquivalentRange IfrsCashAndCashEquivalentRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsCashAndCashEquivalentRange
                {
                    IfrsStatementOfCashFlowId = id,
                    AccountFrom = IfrsCashAndCashEquivalentRange.AccountFrom,
                    AccountTo = IfrsCashAndCashEquivalentRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = IfrsCashAndCashEquivalentRange.LastModified,
                    ModifiedBy = IfrsCashAndCashEquivalentRange.ModifiedBy,
                };

                _db.IfrsCashAndCashEquivalentRanges.Add(entity);
                _db.SaveChanges();
                IfrsCashAndCashEquivalentRange.IfrsCashAndCashEquivalentRangeId = entity.IfrsCashAndCashEquivalentRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup operating activity range", "Success", User.Identity.Name, $"created range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { IfrsCashAndCashEquivalentRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsCashAndCashEquivalentRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsCashAndCashEquivalentRange IfrsCashAndCashEquivalentRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsCashAndCashEquivalentRange
                {
                    IfrsStatementOfCashFlowId = id,
                    IfrsCashAndCashEquivalentRangeId = IfrsCashAndCashEquivalentRange.IfrsCashAndCashEquivalentRangeId,
                    AccountFrom = IfrsCashAndCashEquivalentRange.AccountFrom,
                    AccountTo = IfrsCashAndCashEquivalentRange.AccountTo,
                    DateCreated = IfrsCashAndCashEquivalentRange.DateCreated,
                    CreatedBy = IfrsCashAndCashEquivalentRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsCashAndCashEquivalentRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup operating activity range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { IfrsCashAndCashEquivalentRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsCashAndCashEquivalentRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsCashAndCashEquivalentRange IfrsCashAndCashEquivalentRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsCashAndCashEquivalentRange
                {
                    IfrsCashAndCashEquivalentRangeId = IfrsCashAndCashEquivalentRange.IfrsCashAndCashEquivalentRangeId,
                    AccountFrom = IfrsCashAndCashEquivalentRange.AccountFrom,
                    AccountTo = IfrsCashAndCashEquivalentRange.AccountTo
                };

                _db.IfrsCashAndCashEquivalentRanges.Attach(entity);
                _db.IfrsCashAndCashEquivalentRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted operating activity account range from {IfrsCashAndCashEquivalentRange.AccountFrom} to {IfrsCashAndCashEquivalentRange.AccountTo}");
                #endregion
            }

            return Json(new[] { IfrsCashAndCashEquivalentRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
