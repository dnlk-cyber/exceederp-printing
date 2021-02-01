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
    public class IfrsStatementOfCashFlowController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.IfrsStatementOfCashFlows.Count();
            return View();
        }

        public ActionResult IfrsStatementOfCashFlows_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsStatementOfCashFlow> ifrsstatementofcashflows = _db.IfrsStatementOfCashFlows;
            DataSourceResult result = ifrsstatementofcashflows.ToDataSourceResult(request, ifrsStatementOfCashFlow => new {
                IfrsStatementOfCashFlowId = ifrsStatementOfCashFlow.IfrsStatementOfCashFlowId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfCashFlows_Create([DataSourceRequest]DataSourceRequest request, IfrsStatementOfCashFlow ifrsStatementOfCashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfCashFlow
                {
                };

                _db.IfrsStatementOfCashFlows.Add(entity);
                _db.SaveChanges();
                ifrsStatementOfCashFlow.IfrsStatementOfCashFlowId = entity.IfrsStatementOfCashFlowId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup cash flow", "Success", User.Identity.Name, $"created rule  {entity.IfrsStatementOfCashFlowId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfCashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfCashFlows_Update([DataSourceRequest]DataSourceRequest request, IfrsStatementOfCashFlow ifrsStatementOfCashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfCashFlow
                {
                    IfrsStatementOfCashFlowId = ifrsStatementOfCashFlow.IfrsStatementOfCashFlowId,
                };

                _db.IfrsStatementOfCashFlows.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup cash flow", "Success", User.Identity.Name, $"updated rule  {entity.IfrsStatementOfCashFlowId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfCashFlow }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfCashFlows_Destroy([DataSourceRequest]DataSourceRequest request, IfrsStatementOfCashFlow ifrsStatementOfCashFlow)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfCashFlow
                {
                    IfrsStatementOfCashFlowId = ifrsStatementOfCashFlow.IfrsStatementOfCashFlowId,
                };

                _db.IfrsStatementOfCashFlows.Attach(entity);
                _db.IfrsStatementOfCashFlows.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted cashflow {ifrsStatementOfCashFlow.IfrsStatementOfCashFlowId}");
                #endregion
            }

            return Json(new[] { ifrsStatementOfCashFlow }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
