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
    public class IfrsSofpAssetController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IfrsSofpAssets_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IfrsSofpAsset> ifrssofpassets = _db.IfrsSofpAssets.Where(x=>x.IfrsStatementOfFinancialPositionId==id);
            DataSourceResult result = ifrssofpassets.ToDataSourceResult(request, ifrsSofpAsset => new {
                IfrsSofpAssetId = ifrsSofpAsset.IfrsSofpAssetId,
                IfrsStatementOfFinancialPositionId = id,
                Category = ifrsSofpAsset.Category,
                Description = ifrsSofpAsset.Description,
                NoteNumber = ifrsSofpAsset.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpAssets_Create(int id, [DataSourceRequest]DataSourceRequest request, IfrsSofpAsset ifrsSofpAsset)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Header First");
            }
            #region validation

            var exists = _db.IfrsSofpEquityAndLiablities.Where(x => x.NoteNumber == ifrsSofpAsset.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under equity and liability.");
            }

            var exists1 = _db.IfrsSofpAssets.Where(x => x.NoteNumber == ifrsSofpAsset.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpAsset
                {
                    IfrsStatementOfFinancialPositionId = id,
                    Category = ifrsSofpAsset.Category,
                    Description = ifrsSofpAsset.Description,
                    NoteNumber = ifrsSofpAsset.NoteNumber
                };

                _db.IfrsSofpAssets.Add(entity);
                _db.SaveChanges();
                ifrsSofpAsset.IfrsSofpAssetId = entity.IfrsSofpAssetId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position asset", "Success", User.Identity.Name, $"created rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpAsset }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpAssets_Update(int id, [DataSourceRequest]DataSourceRequest request, IfrsSofpAsset ifrsSofpAsset)
        {
            #region validation

            var exists = _db.IfrsSofpEquityAndLiablities.Where(x => x.NoteNumber == ifrsSofpAsset.NoteNumber).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under equity and liability.");
            }

            var exists1 = _db.IfrsSofpAssets.Where(x => x.IfrsSofpAssetId != ifrsSofpAsset.IfrsSofpAssetId)
                                            .Where(x => x.NoteNumber == ifrsSofpAsset.NoteNumber).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The note number already exists under assets.");
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpAsset
                {
                    IfrsStatementOfFinancialPositionId = id,
                    IfrsSofpAssetId = ifrsSofpAsset.IfrsSofpAssetId,
                    Category = ifrsSofpAsset.Category,
                    Description = ifrsSofpAsset.Description,
                    NoteNumber = ifrsSofpAsset.NoteNumber
                };

                _db.IfrsSofpAssets.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "IFRS report setup financial position asset", "Success", User.Identity.Name, $"updated rule  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { ifrsSofpAsset }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IfrsSofpAssets_Destroy([DataSourceRequest]DataSourceRequest request, IfrsSofpAsset ifrsSofpAsset)
        {
            if (ModelState.IsValid)
            {
                var entity = new IfrsSofpAsset
                {
                    IfrsSofpAssetId = ifrsSofpAsset.IfrsSofpAssetId,
                    Category = ifrsSofpAsset.Category,
                    Description = ifrsSofpAsset.Description,
                    NoteNumber = ifrsSofpAsset.NoteNumber
                };

                _db.IfrsSofpAssets.Attach(entity);
                _db.IfrsSofpAssets.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted financial position asset {ifrsSofpAsset.Description}");
                #endregion
            }

            return Json(new[] { ifrsSofpAsset }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
