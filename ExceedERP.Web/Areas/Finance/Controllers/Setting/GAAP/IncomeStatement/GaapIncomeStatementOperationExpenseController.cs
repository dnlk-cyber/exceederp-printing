using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting.GAAP
{
    public class GaapIncomeStatementOperationExpenseController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapIncomeStatementOperationExpenses_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapIncomeStatementOperationExpense> gaapincomestatementoperationexpenses = db.GaapIncomeStatementOperationExpenses.Where(x=>x.GaapIncomeStatementId==id);
            DataSourceResult result = gaapincomestatementoperationexpenses.ToDataSourceResult(request, gaapIncomeStatementOperationExpense => new {
                GaapIncomeStatementId = gaapIncomeStatementOperationExpense.GaapIncomeStatementId,
                GaapIncomeStatementOperationExpenseId = gaapIncomeStatementOperationExpense.GaapIncomeStatementOperationExpenseId,
                Category = gaapIncomeStatementOperationExpense.Category,
                Description = gaapIncomeStatementOperationExpense.Description,
                NoteNumber = gaapIncomeStatementOperationExpense.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationExpenses_Create(int id,[DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationExpense gaapIncomeStatementOperationExpense)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationExpense
                {
                    GaapIncomeStatementId =id,
                    Category = gaapIncomeStatementOperationExpense.Category,
                    Description = gaapIncomeStatementOperationExpense.Description,
                    NoteNumber = gaapIncomeStatementOperationExpense.NoteNumber
                };

                db.GaapIncomeStatementOperationExpenses.Add(entity);
                db.SaveChanges();
                gaapIncomeStatementOperationExpense.GaapIncomeStatementOperationExpenseId = entity.GaapIncomeStatementOperationExpenseId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational expense", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapIncomeStatementId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationExpense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationExpenses_Update([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationExpense gaapIncomeStatementOperationExpense)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationExpense
                {
                    GaapIncomeStatementOperationExpenseId = gaapIncomeStatementOperationExpense.GaapIncomeStatementOperationExpenseId,
                    GaapIncomeStatementId = gaapIncomeStatementOperationExpense.GaapIncomeStatementId,
                    Category = gaapIncomeStatementOperationExpense.Category,
                    Description = gaapIncomeStatementOperationExpense.Description,
                    NoteNumber = gaapIncomeStatementOperationExpense.NoteNumber
                };

                db.GaapIncomeStatementOperationExpenses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup income statement operational expense", "Success", User.Identity.Name, $"updated rule {entity.Description} under {entity.GaapIncomeStatementId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationExpense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapIncomeStatementOperationExpenses_Destroy([DataSourceRequest]DataSourceRequest request, GaapIncomeStatementOperationExpense gaapIncomeStatementOperationExpense)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapIncomeStatementOperationExpense
                {
                    GaapIncomeStatementOperationExpenseId = gaapIncomeStatementOperationExpense.GaapIncomeStatementOperationExpenseId,
                    Category = gaapIncomeStatementOperationExpense.Category,
                    Description = gaapIncomeStatementOperationExpense.Description,
                    NoteNumber = gaapIncomeStatementOperationExpense.NoteNumber
                };

                db.GaapIncomeStatementOperationExpenses.Attach(entity);
                db.GaapIncomeStatementOperationExpenses.Remove(entity);
                db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted gaap income statement operational expense {gaapIncomeStatementOperationExpense.Description}");
                #endregion
            }

            return Json(new[] { gaapIncomeStatementOperationExpense }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
