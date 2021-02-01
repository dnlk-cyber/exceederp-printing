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
    public class IfrsStatementOfComprehensiveIncomeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.IfrsStatementOfComprehensiveIncomes.Count();
            return View();
        }

        public ActionResult IfrsStatementOfComprehensiveIncomes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsStatementOfComprehensiveIncome> ifrsstatementofcomprehensiveincomes = _db.IfrsStatementOfComprehensiveIncomes;
            DataSourceResult result = ifrsstatementofcomprehensiveincomes.ToDataSourceResult(request, ifrsStatementOfComprehensiveIncome => new
            {
                IfrsStatementOfComprehensiveIncomeId = ifrsStatementOfComprehensiveIncome.IfrsStatementOfComprehensiveIncomeId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfComprehensiveIncomes_Create([DataSourceRequest]DataSourceRequest request, IfrsStatementOfComprehensiveIncome ifrsStatementOfComprehensiveIncome)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfComprehensiveIncome
                {
                };

                _db.IfrsStatementOfComprehensiveIncomes.Add(entity);
                _db.SaveChanges();
                ifrsStatementOfComprehensiveIncome.IfrsStatementOfComprehensiveIncomeId = entity.IfrsStatementOfComprehensiveIncomeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup comprehensive income", "Success", User.Identity.Name, $"created rule  {entity.IfrsStatementOfComprehensiveIncomeId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfComprehensiveIncome }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfComprehensiveIncomes_Update([DataSourceRequest]DataSourceRequest request, IfrsStatementOfComprehensiveIncome ifrsStatementOfComprehensiveIncome)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfComprehensiveIncome
                {
                    IfrsStatementOfComprehensiveIncomeId = ifrsStatementOfComprehensiveIncome.IfrsStatementOfComprehensiveIncomeId,
                };

                _db.IfrsStatementOfComprehensiveIncomes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup comprehensive income", "Success", User.Identity.Name, $"updated rule  {entity.IfrsStatementOfComprehensiveIncomeId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsStatementOfComprehensiveIncome }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsStatementOfComprehensiveIncomes_Destroy([DataSourceRequest]DataSourceRequest request, IfrsStatementOfComprehensiveIncome ifrsStatementOfComprehensiveIncome)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsStatementOfComprehensiveIncome
                {
                    IfrsStatementOfComprehensiveIncomeId = ifrsStatementOfComprehensiveIncome.IfrsStatementOfComprehensiveIncomeId,
                };

                _db.IfrsStatementOfComprehensiveIncomes.Attach(entity);
                _db.IfrsStatementOfComprehensiveIncomes.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted statement of comprehensive income {ifrsStatementOfComprehensiveIncome.IfrsStatementOfComprehensiveIncomeId}");
                #endregion
            }

            return Json(new[] { ifrsStatementOfComprehensiveIncome }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
