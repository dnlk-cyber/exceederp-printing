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
    public class GaapBalanceSheetCapitalController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapBalanceSheetCapitals_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapBalanceSheetCapital> gaapBalanceSheetCapitals = _db.GaapBalanceSheetCapitals.Where(x=>x.GaapBalanceSheetId==id);
            DataSourceResult result = gaapBalanceSheetCapitals.ToDataSourceResult(request, gaapBalanceSheetCapital => new {
                GaapBalanceSheetCapitalId = gaapBalanceSheetCapital.GaapBalanceSheetCapitalId,
                GaapBalanceSheetId = id,
                Category = gaapBalanceSheetCapital.Category,
                Description = gaapBalanceSheetCapital.Description,
                NoteNumber = gaapBalanceSheetCapital.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetCapitals_Create(int id, [DataSourceRequest]DataSourceRequest request, GaapBalanceSheetCapital gaapBalanceSheetCapital)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.GaapBalanceSheetLiabilities.Where(x => x.NoteNumber == gaapBalanceSheetCapital.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under liability.");
            }

            var exists1 = _db.GaapBalanceSheetCapitals.Where(x => x.NoteNumber == gaapBalanceSheetCapital.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }

            var exists2 = _db.GaapBalanceSheetAssets.Where(x => x.NoteNumber == gaapBalanceSheetCapital.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetCapital
                {
                    GaapBalanceSheetId = id,
                    Category = gaapBalanceSheetCapital.Category,
                    Description = gaapBalanceSheetCapital.Description,
                    NoteNumber = gaapBalanceSheetCapital.NoteNumber
                };

                _db.GaapBalanceSheetCapitals.Add(entity);
                _db.SaveChanges();
                gaapBalanceSheetCapital.GaapBalanceSheetCapitalId = entity.GaapBalanceSheetCapitalId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet capital", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapBalanceSheetId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetCapital }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetCapitals_Update(int id, [DataSourceRequest]DataSourceRequest request, GaapBalanceSheetCapital gaapBalanceSheetCapital)
        {
            #region validation

            var exists = _db.IfrsSofpEquityAndLiablities.Where(x => x.NoteNumber == gaapBalanceSheetCapital.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under equity and liability.");
            }

            var exists1 = _db.GaapBalanceSheetCapitals.Where(x => x.GaapBalanceSheetCapitalId != gaapBalanceSheetCapital.GaapBalanceSheetCapitalId)
                                            .Where(x => x.NoteNumber == gaapBalanceSheetCapital.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }

            var exists2 = _db.GaapBalanceSheetAssets.Where(x => x.NoteNumber == gaapBalanceSheetCapital.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetCapital
                {
                    GaapBalanceSheetId = id,
                    GaapBalanceSheetCapitalId = gaapBalanceSheetCapital.GaapBalanceSheetCapitalId,
                    Category = gaapBalanceSheetCapital.Category,
                    Description = gaapBalanceSheetCapital.Description,
                    NoteNumber = gaapBalanceSheetCapital.NoteNumber
                };

                _db.GaapBalanceSheetCapitals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                gaapBalanceSheetCapital.GaapBalanceSheetCapitalId = entity.GaapBalanceSheetCapitalId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet capital", "Success", User.Identity.Name, $"updated rule {entity.Description} under {entity.GaapBalanceSheetId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetCapital }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetCapitals_Destroy([DataSourceRequest]DataSourceRequest request, GaapBalanceSheetCapital gaapBalanceSheetCapital)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetCapital
                {
                    GaapBalanceSheetCapitalId = gaapBalanceSheetCapital.GaapBalanceSheetCapitalId,
                    Category = gaapBalanceSheetCapital.Category,
                    Description = gaapBalanceSheetCapital.Description,
                    NoteNumber = gaapBalanceSheetCapital.NoteNumber
                };

                _db.GaapBalanceSheetCapitals.Attach(entity);
                _db.GaapBalanceSheetCapitals.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted balance sheet capital {gaapBalanceSheetCapital.Description}");
                #endregion
            }

            return Json(new[] { gaapBalanceSheetCapital }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
