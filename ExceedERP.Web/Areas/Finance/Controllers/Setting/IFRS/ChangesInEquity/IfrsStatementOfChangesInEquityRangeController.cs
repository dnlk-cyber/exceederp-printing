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
    public class IfrsStatementOfChangesInEquityRangeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsStatementOfChangesInEquityRanges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsStatementOfChangesInEquityRange> ifrsstatementofchangesinequityranges = _db.IfrsStatementOfChangesInEquityRanges.Where(x=>x.IfrsStatementOfChangesInEquityId==id);
            DataSourceResult result = ifrsstatementofchangesinequityranges.ToDataSourceResult(request, ifrsStatementOfChangesInEquityRange => new {
                IfrsStatementOfChangesInEquityId=id,
                IfrsStatementOfChangesInEquityRangeId = ifrsStatementOfChangesInEquityRange.IfrsStatementOfChangesInEquityRangeId,
                AccountFrom = ifrsStatementOfChangesInEquityRange.AccountFrom,
                AccountTo = ifrsStatementOfChangesInEquityRange.AccountTo,
                DateCreated = ifrsStatementOfChangesInEquityRange.DateCreated,
                CreatedBy = ifrsStatementOfChangesInEquityRange.CreatedBy,
                LastModified = ifrsStatementOfChangesInEquityRange.LastModified,
                ModifiedBy = ifrsStatementOfChangesInEquityRange.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfChangesInEquityRanges_Create(int id,[DataSourceRequest]DataSourceRequest request, IfrsStatementOfChangesInEquityRange ifrsStatementOfChangesInEquityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfChangesInEquityRange
                {
                    IfrsStatementOfChangesInEquityId = id,
                    AccountFrom = ifrsStatementOfChangesInEquityRange.AccountFrom,
                    AccountTo = ifrsStatementOfChangesInEquityRange.AccountTo,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = ifrsStatementOfChangesInEquityRange.LastModified,
                    ModifiedBy = ifrsStatementOfChangesInEquityRange.ModifiedBy,
                };

                _db.IfrsStatementOfChangesInEquityRanges.Add(entity);
                _db.SaveChanges();
                ifrsStatementOfChangesInEquityRange.IfrsStatementOfChangesInEquityRangeId = entity.IfrsStatementOfChangesInEquityRangeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup changes in equity range", "Success", User.Identity.Name, $"created range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfChangesInEquityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfChangesInEquityRanges_Update(int id,[DataSourceRequest]DataSourceRequest request, IfrsStatementOfChangesInEquityRange ifrsStatementOfChangesInEquityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfChangesInEquityRange
                {
                    IfrsStatementOfChangesInEquityId = id,
                    IfrsStatementOfChangesInEquityRangeId = ifrsStatementOfChangesInEquityRange.IfrsStatementOfChangesInEquityRangeId,
                    AccountFrom = ifrsStatementOfChangesInEquityRange.AccountFrom,
                    AccountTo = ifrsStatementOfChangesInEquityRange.AccountTo,
                    DateCreated = ifrsStatementOfChangesInEquityRange.DateCreated,
                    CreatedBy = ifrsStatementOfChangesInEquityRange.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.IfrsStatementOfChangesInEquityRanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup changes in equity range", "Success", User.Identity.Name, $"updated range  {entity.AccountFrom} to {entity.AccountTo}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfChangesInEquityRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfChangesInEquityRanges_Destroy([DataSourceRequest]DataSourceRequest request, IfrsStatementOfChangesInEquityRange ifrsStatementOfChangesInEquityRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfChangesInEquityRange
                {
                    IfrsStatementOfChangesInEquityRangeId = ifrsStatementOfChangesInEquityRange.IfrsStatementOfChangesInEquityRangeId,
                    AccountFrom = ifrsStatementOfChangesInEquityRange.AccountFrom,
                    AccountTo = ifrsStatementOfChangesInEquityRange.AccountTo
                };

                _db.IfrsStatementOfChangesInEquityRanges.Attach(entity);
                _db.IfrsStatementOfChangesInEquityRanges.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted changes in equity account range from {ifrsStatementOfChangesInEquityRange.AccountFrom} to {ifrsStatementOfChangesInEquityRange.AccountTo}");
                #endregion
            }

            return Json(new[] { ifrsStatementOfChangesInEquityRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
