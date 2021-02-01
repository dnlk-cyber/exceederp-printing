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
    public class GaapBalanceSheetAssetController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GaapBalanceSheetAssets_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GaapBalanceSheetAsset> gaapBalanceSheetAssets = _db.GaapBalanceSheetAssets.Where(x=>x.GaapBalanceSheetId==id);
            DataSourceResult result = gaapBalanceSheetAssets.ToDataSourceResult(request, gaapBalanceSheetAsset => new {
                GaapBalanceSheetAssetId = gaapBalanceSheetAsset.GaapBalanceSheetAssetId,
                GaapBalanceSheetId = id,
                Category = gaapBalanceSheetAsset.Category,
                Description = gaapBalanceSheetAsset.Description,
                NoteNumber = gaapBalanceSheetAsset.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetAssets_Create(int id, [DataSourceRequest]DataSourceRequest request, GaapBalanceSheetAsset gaapBalanceSheetAsset)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.GaapBalanceSheetLiabilities.Where(x => x.NoteNumber == gaapBalanceSheetAsset.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under liability.");
            }

            var exists1 = _db.GaapBalanceSheetAssets.Where(x => x.NoteNumber == gaapBalanceSheetAsset.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }

            var exists2 = _db.GaapBalanceSheetCapitals.Where(x => x.NoteNumber == gaapBalanceSheetAsset.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under capital.");
            }



            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetAsset
                {
                    GaapBalanceSheetId = id,
                    Category = gaapBalanceSheetAsset.Category,
                    Description = gaapBalanceSheetAsset.Description,
                    NoteNumber = gaapBalanceSheetAsset.NoteNumber
                };

                _db.GaapBalanceSheetAssets.Add(entity);
                _db.SaveChanges();
                gaapBalanceSheetAsset.GaapBalanceSheetAssetId = entity.GaapBalanceSheetAssetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet asset", "Success", User.Identity.Name, $"created rule {entity.Description} under {entity.GaapBalanceSheetId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetAsset }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetAssets_Update(int id, [DataSourceRequest]DataSourceRequest request, GaapBalanceSheetAsset gaapBalanceSheetAsset)
        {
            #region validation

            var exists = _db.IfrsSofpEquityAndLiablities.Where(x => x.NoteNumber == gaapBalanceSheetAsset.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under equity and liability.");
            }

            var exists1 = _db.GaapBalanceSheetAssets.Where(x => x.GaapBalanceSheetAssetId != gaapBalanceSheetAsset.GaapBalanceSheetAssetId)
                                            .Where(x => x.NoteNumber == gaapBalanceSheetAsset.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }

            var exists2 = _db.GaapBalanceSheetCapitals.Where(x => x.NoteNumber == gaapBalanceSheetAsset.NoteNumber).AsQueryable();
            if (exists2.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under capital.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetAsset
                {
                    GaapBalanceSheetId = id,
                    GaapBalanceSheetAssetId = gaapBalanceSheetAsset.GaapBalanceSheetAssetId,
                    Category = gaapBalanceSheetAsset.Category,
                    Description = gaapBalanceSheetAsset.Description,
                    NoteNumber = gaapBalanceSheetAsset.NoteNumber
                };

                _db.GaapBalanceSheetAssets.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GAAP report setup balance sheet asset", "Success", User.Identity.Name, $"updated rule {entity.Description} under {entity.GaapBalanceSheetId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gaapBalanceSheetAsset }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GaapBalanceSheetAssets_Destroy([DataSourceRequest]DataSourceRequest request, GaapBalanceSheetAsset gaapBalanceSheetAsset)
        {
            if (ModelState.IsValid)
            {
                var entity = new GaapBalanceSheetAsset
                {
                    GaapBalanceSheetAssetId = gaapBalanceSheetAsset.GaapBalanceSheetAssetId,
                    Category = gaapBalanceSheetAsset.Category,
                    Description = gaapBalanceSheetAsset.Description,
                    NoteNumber = gaapBalanceSheetAsset.NoteNumber
                };

                _db.GaapBalanceSheetAssets.Attach(entity);
                _db.GaapBalanceSheetAssets.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted balance sheet asset {gaapBalanceSheetAsset.Description}");
                #endregion
            }

            return Json(new[] { gaapBalanceSheetAsset }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
