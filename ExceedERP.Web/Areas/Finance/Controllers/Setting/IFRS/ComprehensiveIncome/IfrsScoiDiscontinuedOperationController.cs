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
    public class IfrsScoiDiscontinuedOperationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsScoiDiscontinuedOperations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IfrsScoiDiscontinuedOperation> ifrsscoidiscontinuedoperations = _db.IfrsScoiDiscontinuedOperations.Where(x=>x.IfrsStatementOfComprehensiveIncomeId==id);
            DataSourceResult result = ifrsscoidiscontinuedoperations.ToDataSourceResult(request, ifrsScoiDiscontinuedOperation => new {
                IfrsScoiDiscontinuedOperationId = ifrsScoiDiscontinuedOperation.IfrsScoiDiscontinuedOperationId,
                IfrsStatementOfComprehensiveIncomeId = id,
                Description = ifrsScoiDiscontinuedOperation.Description,
                NoteNumber = ifrsScoiDiscontinuedOperation.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiDiscontinuedOperations_Create(int id, [DataSourceRequest]DataSourceRequest request, IfrsScoiDiscontinuedOperation ifrsScoiDiscontinuedOperation)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.IfrsScoiContinuingOperations.Where(x => x.NoteNumber == ifrsScoiDiscontinuedOperation.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Continuing Operations.");
            }

            var exists1 = _db.IfrsScoiDiscontinuedOperations.Where(x => x.NoteNumber == ifrsScoiDiscontinuedOperation.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Discontinued activities.");
            }
            var exists2 = _db.IfrsScoiOtherComprehensiveIncomes.Where(x => x.NoteNumber == ifrsScoiDiscontinuedOperation.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under other comprehensive income.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiDiscontinuedOperation
                {
                    IfrsStatementOfComprehensiveIncomeId = id,
                    Description = ifrsScoiDiscontinuedOperation.Description,
                    NoteNumber = ifrsScoiDiscontinuedOperation.NoteNumber
                };

                _db.IfrsScoiDiscontinuedOperations.Add(entity);
                _db.SaveChanges();
                ifrsScoiDiscontinuedOperation.IfrsScoiDiscontinuedOperationId = entity.IfrsScoiDiscontinuedOperationId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Disontinued operation", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiDiscontinuedOperation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiDiscontinuedOperations_Update(int id, [DataSourceRequest]DataSourceRequest request, IfrsScoiDiscontinuedOperation ifrsScoiDiscontinuedOperation)
        {
            #region validation

            var exists = _db.IfrsScoiContinuingOperations.Where(x => x.NoteNumber == ifrsScoiDiscontinuedOperation.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Continuing Operations.");
            }

            var exists1 = _db.IfrsScoiDiscontinuedOperations.Where(x => x.IfrsScoiDiscontinuedOperationId != ifrsScoiDiscontinuedOperation.IfrsScoiDiscontinuedOperationId)
                                                            .Where(x => x.NoteNumber == ifrsScoiDiscontinuedOperation.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under Discontinued activities.");
            }
            var exists2 = _db.IfrsScoiOtherComprehensiveIncomes.Where(x => x.NoteNumber == ifrsScoiDiscontinuedOperation.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under other comprehensive income.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiDiscontinuedOperation
                {
                    IfrsStatementOfComprehensiveIncomeId = id,
                    IfrsScoiDiscontinuedOperationId = ifrsScoiDiscontinuedOperation.IfrsScoiDiscontinuedOperationId,
                    Description = ifrsScoiDiscontinuedOperation.Description,
                    NoteNumber = ifrsScoiDiscontinuedOperation.NoteNumber
                };

                _db.IfrsScoiDiscontinuedOperations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup Disontinued operation", "Success", User.Identity.Name, $"updated rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsScoiDiscontinuedOperation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsScoiDiscontinuedOperations_Destroy([DataSourceRequest]DataSourceRequest request, IfrsScoiDiscontinuedOperation ifrsScoiDiscontinuedOperation)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsScoiDiscontinuedOperation
                {
                    IfrsScoiDiscontinuedOperationId = ifrsScoiDiscontinuedOperation.IfrsScoiDiscontinuedOperationId,
                    Description = ifrsScoiDiscontinuedOperation.Description,
                    NoteNumber = ifrsScoiDiscontinuedOperation.NoteNumber
                };

                _db.IfrsScoiDiscontinuedOperations.Attach(entity);
                _db.IfrsScoiDiscontinuedOperations.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted discontinuing operations account range {ifrsScoiDiscontinuedOperation.Description}");
                #endregion
            }

            return Json(new[] { ifrsScoiDiscontinuedOperation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
