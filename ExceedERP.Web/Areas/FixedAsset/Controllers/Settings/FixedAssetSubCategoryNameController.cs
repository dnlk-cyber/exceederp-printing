using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{
    public class FixedAssetSubCategoryNameController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FixedAssetSubCategoryNames_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetSubCategoryName> fixedassetsubcategorynames = db.FixedAssetSubCategoryNames.Where(x=>x.FixedAssetSubCategoryID==id);
            DataSourceResult result = fixedassetsubcategorynames.ToDataSourceResult(request, fixedAssetSubCategoryName => new {
                FixedAssetSubCategoryNameId = fixedAssetSubCategoryName.FixedAssetSubCategoryNameId,
                FixedAssetSubCategoryID = fixedAssetSubCategoryName.FixedAssetSubCategoryID,
                Name = fixedAssetSubCategoryName.Name,
                DateCreated = fixedAssetSubCategoryName.DateCreated,
                LastModified = fixedAssetSubCategoryName.LastModified,
                CreatedBy = fixedAssetSubCategoryName.CreatedBy,
                ModifiedBy = fixedAssetSubCategoryName.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetSubCategoryNames_Create(int id,[DataSourceRequest]DataSourceRequest request, FixedAssetSubCategoryName fixedAssetSubCategoryName)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetSubCategoryName
                {
                    FixedAssetSubCategoryID = id,
                    Name = fixedAssetSubCategoryName.Name,
                    DateCreated = fixedAssetSubCategoryName.DateCreated,
                    LastModified = fixedAssetSubCategoryName.LastModified,
                    CreatedBy = fixedAssetSubCategoryName.CreatedBy,
                    ModifiedBy = fixedAssetSubCategoryName.ModifiedBy
                };

                db.FixedAssetSubCategoryNames.Add(entity);
                db.SaveChanges();
                fixedAssetSubCategoryName.FixedAssetSubCategoryNameId = entity.FixedAssetSubCategoryNameId;
            }

            return Json(new[] { fixedAssetSubCategoryName }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetSubCategoryNames_Update([DataSourceRequest]DataSourceRequest request, FixedAssetSubCategoryName fixedAssetSubCategoryName)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetSubCategoryName
                {
                    FixedAssetSubCategoryID = fixedAssetSubCategoryName.FixedAssetSubCategoryID,
                    FixedAssetSubCategoryNameId = fixedAssetSubCategoryName.FixedAssetSubCategoryNameId,
                    Name = fixedAssetSubCategoryName.Name,
                    DateCreated = fixedAssetSubCategoryName.DateCreated,
                    LastModified = fixedAssetSubCategoryName.LastModified,
                    CreatedBy = fixedAssetSubCategoryName.CreatedBy,
                    ModifiedBy = fixedAssetSubCategoryName.ModifiedBy
                };

                db.FixedAssetSubCategoryNames.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { fixedAssetSubCategoryName }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetSubCategoryNames_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetSubCategoryName fixedAssetSubCategoryName)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetSubCategoryName
                {
                    FixedAssetSubCategoryNameId = fixedAssetSubCategoryName.FixedAssetSubCategoryNameId,
                    Name = fixedAssetSubCategoryName.Name,
                    DateCreated = fixedAssetSubCategoryName.DateCreated,
                    LastModified = fixedAssetSubCategoryName.LastModified,
                    CreatedBy = fixedAssetSubCategoryName.CreatedBy,
                    ModifiedBy = fixedAssetSubCategoryName.ModifiedBy
                };

                db.FixedAssetSubCategoryNames.Attach(entity);
                db.FixedAssetSubCategoryNames.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { fixedAssetSubCategoryName }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
