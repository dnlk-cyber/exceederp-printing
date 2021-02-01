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
    public class IfrsScoiContinuingOperationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsScoiContinuingOperations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IfrsScoiContinuingOperation> ifrsscoicontinuingoperations = _db.IfrsScoiContinuingOperations.Where(x=>x.IfrsStatementOfComprehensiveIncomeId==id);
            DataSourceResult result = ifrsscoicontinuingoperations.ToDataSourceResult(request, ifrsScoiContinuingOperation => new {
                IfrsScoiContinuingOperationId = ifrsScoiContinuingOperation.IfrsScoiContinuingOperationId,
                IfrsStatementOfComprehensiveIncomeId = id,
                Category = ifrsScoiContinuingOperation.Category,
                Description = ifrsScoiContinuingOperation.Description,
                NoteNumber = ifrsScoiContinuingOperation.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiContinuingOperations_Create(int id, [DataSourceRequest]DataSourceRequest request, IfrsScoiContinuingOperation ifrsScoiContinuingOperation)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.IfrsScoiContinuingOperations.Where(x => x.NoteNumber == ifrsScoiContinuingOperation.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Continuing Operations.");
            }

            var exists1 = _db.IfrsScoiDiscontinuedOperations.Where(x => x.NoteNumber == ifrsScoiContinuingOperation.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Discontinued activities.");
            }
            var exists2 = _db.IfrsScoiOtherComprehensiveIncomes.Where(x => x.NoteNumber == ifrsScoiContinuingOperation.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under other comprehensive income.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiContinuingOperation
                {
                    IfrsStatementOfComprehensiveIncomeId = id,
                    Category = ifrsScoiContinuingOperation.Category,
                    Description = ifrsScoiContinuingOperation.Description,
                    NoteNumber = ifrsScoiContinuingOperation.NoteNumber
                };

                _db.IfrsScoiContinuingOperations.Add(entity);
                _db.SaveChanges();
                ifrsScoiContinuingOperation.IfrsScoiContinuingOperationId = entity.IfrsScoiContinuingOperationId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Continuing operation", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiContinuingOperation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiContinuingOperations_Update(int id, [DataSourceRequest]DataSourceRequest request, IfrsScoiContinuingOperation ifrsScoiContinuingOperation)
        {
            #region validation

            var exists = _db.IfrsScoiContinuingOperations.Where(x => x.IfrsScoiContinuingOperationId != ifrsScoiContinuingOperation.IfrsScoiContinuingOperationId)
                                                         .Where(x => x.NoteNumber == ifrsScoiContinuingOperation.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Continuing Operations.");
            }

            var exists1 = _db.IfrsScoiDiscontinuedOperations.Where(x => x.NoteNumber == ifrsScoiContinuingOperation.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Discontinued activities.");
            }
            var exists2 = _db.IfrsScoiOtherComprehensiveIncomes.Where(x => x.NoteNumber == ifrsScoiContinuingOperation.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under other comprehensive income.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiContinuingOperation
                {
                    IfrsStatementOfComprehensiveIncomeId = id,
                    IfrsScoiContinuingOperationId = ifrsScoiContinuingOperation.IfrsScoiContinuingOperationId,
                    Category = ifrsScoiContinuingOperation.Category,
                    Description = ifrsScoiContinuingOperation.Description,
                    NoteNumber = ifrsScoiContinuingOperation.NoteNumber
                };

                _db.IfrsScoiContinuingOperations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Continuing operation", "Success", User.Identity.Name, $"updated rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiContinuingOperation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiContinuingOperations_Destroy([DataSourceRequest]DataSourceRequest request, IfrsScoiContinuingOperation ifrsScoiContinuingOperation)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiContinuingOperation
                {
                    IfrsScoiContinuingOperationId = ifrsScoiContinuingOperation.IfrsScoiContinuingOperationId,
                    Category = ifrsScoiContinuingOperation.Category,
                    Description = ifrsScoiContinuingOperation.Description,
                    NoteNumber = ifrsScoiContinuingOperation.NoteNumber
                };

                _db.IfrsScoiContinuingOperations.Attach(entity);
                _db.IfrsScoiContinuingOperations.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted continuiing operation {ifrsScoiContinuingOperation.Description}");
                #endregion
            }

            return Json(new[] { ifrsScoiContinuingOperation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
