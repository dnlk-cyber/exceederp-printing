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
    public class IfrsStatementOfFinancialPositionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.IfrsStatementOfFinancialPositions.Count();
            return View();
        }

        public ActionResult IfrsStatementOfFinancialPositions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsStatementOfFinancialPosition> ifrsstatementoffinancialpositions = _db.IfrsStatementOfFinancialPositions;
            DataSourceResult result = ifrsstatementoffinancialpositions.ToDataSourceResult(request, ifrsStatementOfFinancialPosition => new {
                IfrsStatementOfFinancialPositionId = ifrsStatementOfFinancialPosition.IfrsStatementOfFinancialPositionId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfFinancialPositions_Create([DataSourceRequest]DataSourceRequest request, IfrsStatementOfFinancialPosition ifrsStatementOfFinancialPosition)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfFinancialPosition
                {
                };

                _db.IfrsStatementOfFinancialPositions.Add(entity);
                _db.SaveChanges();
                ifrsStatementOfFinancialPosition.IfrsStatementOfFinancialPositionId = entity.IfrsStatementOfFinancialPositionId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup cash flow", "Success", User.Identity.Name, $"created rule  {entity.IfrsStatementOfFinancialPositionId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfFinancialPosition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfFinancialPositions_Update([DataSourceRequest]DataSourceRequest request, IfrsStatementOfFinancialPosition ifrsStatementOfFinancialPosition)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfFinancialPosition
                {
                    IfrsStatementOfFinancialPositionId = ifrsStatementOfFinancialPosition.IfrsStatementOfFinancialPositionId,
                };

                _db.IfrsStatementOfFinancialPositions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position", "Success", User.Identity.Name, $"updated rule  {entity.IfrsStatementOfFinancialPositionId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfFinancialPosition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfFinancialPositions_Destroy([DataSourceRequest]DataSourceRequest request, IfrsStatementOfFinancialPosition ifrsStatementOfFinancialPosition)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfFinancialPosition
                {
                    IfrsStatementOfFinancialPositionId = ifrsStatementOfFinancialPosition.IfrsStatementOfFinancialPositionId,
                };

                _db.IfrsStatementOfFinancialPositions.Attach(entity);
                _db.IfrsStatementOfFinancialPositions.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted financial position {ifrsStatementOfFinancialPosition.IfrsStatementOfFinancialPositionId}");
                #endregion
            }

            return Json(new[] { ifrsStatementOfFinancialPosition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
