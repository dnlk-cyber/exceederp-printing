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
    public class GaapBalanceSheetLiabilityController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapBalanceSheetLiabilitys_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapBalanceSheetLiability> gaapBalanceSheetLiabilitys = _db.GaapBalanceSheetLiabilities.Where(x=>x.GaapBalanceSheetId==id);
            DataSourceResult result = gaapBalanceSheetLiabilitys.ToDataSourceResult(request, gaapBalanceSheetLiability => new {
                GaapBalanceSheetLiabilityId = gaapBalanceSheetLiability.GaapBalanceSheetLiabilityId,
                GaapBalanceSheetId = id,
                Category = gaapBalanceSheetLiability.Category,
                Description = gaapBalanceSheetLiability.Description,
                NoteNumber = gaapBalanceSheetLiability.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetLiabilitys_Create(int id, [DataSourceRequest]DataSourceRequest request, GaapBalanceSheetLiability gaapBalanceSheetLiability)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.GaapBalanceSheetCapitals.Where(x => x.NoteNumber == gaapBalanceSheetLiability.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under capital.");
            }

            var exists1 = _db.GaapBalanceSheetLiabilities.Where(x => x.NoteNumber == gaapBalanceSheetLiability.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under liability.");
            }

            var exists2 = _db.GaapBalanceSheetAssets.Where(x => x.NoteNumber == gaapBalanceSheetLiability.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetLiability
                {
                    GaapBalanceSheetId = id,
                    Category = gaapBalanceSheetLiability.Category,
                    Description = gaapBalanceSheetLiability.Description,
                    NoteNumber = gaapBalanceSheetLiability.NoteNumber
                };

                _db.GaapBalanceSheetLiabilities.Add(entity);
                _db.SaveChanges();
                gaapBalanceSheetLiability.GaapBalanceSheetLiabilityId = entity.GaapBalanceSheetLiabilityId;
               
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet liability", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapBalanceSheetId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetLiability }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetLiabilitys_Update(int id, [DataSourceRequest]DataSourceRequest request, GaapBalanceSheetLiability gaapBalanceSheetLiability)
        {
            #region validation

            var exists = _db.GaapBalanceSheetCapitals.Where(x => x.NoteNumber == gaapBalanceSheetLiability.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under capital.");
            }

            var exists1 = _db.GaapBalanceSheetLiabilities.Where(x => x.GaapBalanceSheetLiabilityId != gaapBalanceSheetLiability.GaapBalanceSheetLiabilityId)
                                            .Where(x => x.NoteNumber == gaapBalanceSheetLiability.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under liability.");
            }

            var exists2 = _db.GaapBalanceSheetAssets.Where(x => x.NoteNumber == gaapBalanceSheetLiability.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetLiability
                {
                    GaapBalanceSheetId = id,
                    GaapBalanceSheetLiabilityId = gaapBalanceSheetLiability.GaapBalanceSheetLiabilityId,
                    Category = gaapBalanceSheetLiability.Category,
                    Description = gaapBalanceSheetLiability.Description,
                    NoteNumber = gaapBalanceSheetLiability.NoteNumber
                };

                _db.GaapBalanceSheetLiabilities.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet liability", "Success", User.Identity.Name, $"updated rule {entity.Description} under {entity.GaapBalanceSheetId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetLiability }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetLiabilitys_Destroy([DataSourceRequest]DataSourceRequest request, GaapBalanceSheetLiability gaapBalanceSheetLiability)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetLiability
                {
                    GaapBalanceSheetLiabilityId = gaapBalanceSheetLiability.GaapBalanceSheetLiabilityId,
                    Category = gaapBalanceSheetLiability.Category,
                    Description = gaapBalanceSheetLiability.Description,
                    NoteNumber = gaapBalanceSheetLiability.NoteNumber
                };

                _db.GaapBalanceSheetLiabilities.Attach(entity);
                _db.GaapBalanceSheetLiabilities.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted balance sheet liability {gaapBalanceSheetLiability.Description}");
                #endregion
            }

            return Json(new[] { gaapBalanceSheetLiability }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
