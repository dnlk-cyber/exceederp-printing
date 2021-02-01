using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.Report.IFRS;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.IFRS
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class IfrsStatementOfChangesInEquityController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsStatementOfChangesInEquities_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsStatementOfChangesInEquity> ifrsstatementofchangesinequities = _db.IfrsStatementOfChangesInEquities;
            DataSourceResult result = ifrsstatementofchangesinequities.ToDataSourceResult(request, ifrsStatementOfChangesInEquity => new {
                IfrsStatementOfChangesInEquityId = ifrsStatementOfChangesInEquity.IfrsStatementOfChangesInEquityId,
                Category = ifrsStatementOfChangesInEquity.Category,
                NoteNumber = ifrsStatementOfChangesInEquity.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfChangesInEquities_Create([DataSourceRequest]DataSourceRequest request, IfrsStatementOfChangesInEquity ifrsStatementOfChangesInEquity)
        {
            #region validation

            var exists = _db.IfrsStatementOfChangesInEquities.Where(x => x.NoteNumber == ifrsStatementOfChangesInEquity.NoteNumber).AsQueryable();
            if (exists.Any())
                ModelState.AddModelError("exist", "The note number already exists.");

            var exists1 = _db.IfrsStatementOfChangesInEquities.Where(x => x.Category == ifrsStatementOfChangesInEquity.Category).AsQueryable();
            if (exists1.Any())
                ModelState.AddModelError("exist", $"The category {ifrsStatementOfChangesInEquity.Category.ToString()} already exists.");

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfChangesInEquity
                {
                    Category = ifrsStatementOfChangesInEquity.Category,
                    NoteNumber = ifrsStatementOfChangesInEquity.NoteNumber
                };

                _db.IfrsStatementOfChangesInEquities.Add(entity);
                _db.SaveChanges();
                ifrsStatementOfChangesInEquity.IfrsStatementOfChangesInEquityId = entity.IfrsStatementOfChangesInEquityId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup changes in equity", "Success", User.Identity.Name, $"created rule  {entity.Category}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfChangesInEquity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfChangesInEquities_Update([DataSourceRequest]DataSourceRequest request, IfrsStatementOfChangesInEquity ifrsStatementOfChangesInEquity)
        {
            #region validation

            var exists = _db.IfrsStatementOfChangesInEquities
                .Where(x => x.IfrsStatementOfChangesInEquityId == ifrsStatementOfChangesInEquity.IfrsStatementOfChangesInEquityId)
                .Where(x => x.NoteNumber == ifrsStatementOfChangesInEquity.NoteNumber).AsQueryable();
            if (exists.Any())
                ModelState.AddModelError("exist", "The note number already exists.");

            var exists1 = _db.IfrsStatementOfChangesInEquities
                .Where(x => x.IfrsStatementOfChangesInEquityId == ifrsStatementOfChangesInEquity.IfrsStatementOfChangesInEquityId)
                .Where(x => x.Category == ifrsStatementOfChangesInEquity.Category).AsQueryable();
            if (exists1.Any())
                ModelState.AddModelError("exist", $"The category {ifrsStatementOfChangesInEquity.Category.ToString()} already exists.");


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfChangesInEquity
                {
                    IfrsStatementOfChangesInEquityId = ifrsStatementOfChangesInEquity.IfrsStatementOfChangesInEquityId,
                    Category = ifrsStatementOfChangesInEquity.Category,
                    NoteNumber = ifrsStatementOfChangesInEquity.NoteNumber
                };

                _db.IfrsStatementOfChangesInEquities.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup changes in equity", "Success", User.Identity.Name, $"updated rule  {entity.Category}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfChangesInEquity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfChangesInEquities_Destroy([DataSourceRequest]DataSourceRequest request, IfrsStatementOfChangesInEquity ifrsStatementOfChangesInEquity)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfChangesInEquity
                {
                    IfrsStatementOfChangesInEquityId = ifrsStatementOfChangesInEquity.IfrsStatementOfChangesInEquityId,
                    Category = ifrsStatementOfChangesInEquity.Category,
                    NoteNumber = ifrsStatementOfChangesInEquity.NoteNumber
                };

                _db.IfrsStatementOfChangesInEquities.Attach(entity);
                _db.IfrsStatementOfChangesInEquities.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted changes in equity {ifrsStatementOfChangesInEquity.IfrsStatementOfChangesInEquityId}");
                #endregion
            }

            return Json(new[] { ifrsStatementOfChangesInEquity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
