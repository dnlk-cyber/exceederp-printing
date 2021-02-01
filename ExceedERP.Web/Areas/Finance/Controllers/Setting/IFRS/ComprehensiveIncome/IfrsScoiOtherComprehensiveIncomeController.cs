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
    public class IfrsScoiOtherComprehensiveIncomeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsScoiOtherComprehensiveIncomes_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IfrsScoiOtherComprehensiveIncome> ifrsscoiothercomprehensiveincomes = _db.IfrsScoiOtherComprehensiveIncomes.Where(x=>x.IfrsStatementOfComprehensiveIncomeId==id);
            DataSourceResult result = ifrsscoiothercomprehensiveincomes.ToDataSourceResult(request, ifrsScoiOtherComprehensiveIncome => new {
                IfrsScoiOtherComprehensiveIncomeId = ifrsScoiOtherComprehensiveIncome.IfrsScoiOtherComprehensiveIncomeId,
                IfrsStatementOfComprehensiveIncomeId=id,
                Description = ifrsScoiOtherComprehensiveIncome.Description,
                NoteNumber = ifrsScoiOtherComprehensiveIncome.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiOtherComprehensiveIncomes_Create(int id, [DataSourceRequest]DataSourceRequest request, IfrsScoiOtherComprehensiveIncome ifrsScoiOtherComprehensiveIncome)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.IfrsScoiContinuingOperations.Where(x => x.NoteNumber == ifrsScoiOtherComprehensiveIncome.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Continuing Operations.");
            }

            var exists1 = _db.IfrsScoiDiscontinuedOperations.Where(x => x.NoteNumber == ifrsScoiOtherComprehensiveIncome.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Discontinued activities.");
            }
            var exists2 = _db.IfrsScoiOtherComprehensiveIncomes.Where(x => x.NoteNumber == ifrsScoiOtherComprehensiveIncome.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under other comprehensive income.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiOtherComprehensiveIncome
                {
                    IfrsStatementOfComprehensiveIncomeId = id,
                    Description = ifrsScoiOtherComprehensiveIncome.Description,
                    NoteNumber = ifrsScoiOtherComprehensiveIncome.NoteNumber
                };

                _db.IfrsScoiOtherComprehensiveIncomes.Add(entity);
                _db.SaveChanges();
                ifrsScoiOtherComprehensiveIncome.IfrsScoiOtherComprehensiveIncomeId = entity.IfrsScoiOtherComprehensiveIncomeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup other comprehensive income", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiOtherComprehensiveIncome }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiOtherComprehensiveIncomes_Update(int id, [DataSourceRequest]DataSourceRequest request, IfrsScoiOtherComprehensiveIncome ifrsScoiOtherComprehensiveIncome)
        {
            #region validation

            var exists = _db.IfrsScoiContinuingOperations.Where(x => x.NoteNumber == ifrsScoiOtherComprehensiveIncome.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Continuing Operations.");
            }

            var exists1 = _db.IfrsScoiDiscontinuedOperations.Where(x => x.NoteNumber == ifrsScoiOtherComprehensiveIncome.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Discontinued activities.");
            }
            var exists2 = _db.IfrsScoiOtherComprehensiveIncomes.Where(x => x.IfrsScoiOtherComprehensiveIncomeId != ifrsScoiOtherComprehensiveIncome.IfrsScoiOtherComprehensiveIncomeId)
                                                               .Where(x => x.NoteNumber == ifrsScoiOtherComprehensiveIncome.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under other comprehensive income.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiOtherComprehensiveIncome
                {
                    IfrsStatementOfComprehensiveIncomeId = id,
                    IfrsScoiOtherComprehensiveIncomeId = ifrsScoiOtherComprehensiveIncome.IfrsScoiOtherComprehensiveIncomeId,
                    Description = ifrsScoiOtherComprehensiveIncome.Description,
                    NoteNumber = ifrsScoiOtherComprehensiveIncome.NoteNumber
                };

                _db.IfrsScoiOtherComprehensiveIncomes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup other comprehensive income", "Success", User.Identity.Name, $"updated rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiOtherComprehensiveIncome }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiOtherComprehensiveIncomes_Destroy([DataSourceRequest]DataSourceRequest request, IfrsScoiOtherComprehensiveIncome ifrsScoiOtherComprehensiveIncome)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiOtherComprehensiveIncome
                {
                    IfrsScoiOtherComprehensiveIncomeId = ifrsScoiOtherComprehensiveIncome.IfrsScoiOtherComprehensiveIncomeId,
                    Description = ifrsScoiOtherComprehensiveIncome.Description,
                    NoteNumber = ifrsScoiOtherComprehensiveIncome.NoteNumber
                };

                _db.IfrsScoiOtherComprehensiveIncomes.Attach(entity);
                _db.IfrsScoiOtherComprehensiveIncomes.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted other comprehensive income {ifrsScoiOtherComprehensiveIncome.Description}");
                #endregion
            }

            return Json(new[] { ifrsScoiOtherComprehensiveIncome }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
