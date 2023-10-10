using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
            var exisit = db.FixedAssetSubCategoryNames.Where(x => x.Name == fixedAssetSubCategoryName.Name).FirstOrDefault();
            if (exisit != null)
                ModelState.AddModelError("name","Name already Exisits");
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetSubCategoryName", "Success", User.Identity.Name, $"created FixedAssetSubCategoryName {entity.FixedAssetSubCategoryNameId} subCatName {entity.FixedAssetSubCategoryNameId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetSubCategoryName }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetSubCategoryNames_Update([DataSourceRequest]DataSourceRequest request, FixedAssetSubCategoryName fixedAssetSubCategoryName)
        {
            var exisit = db.FixedAssetSubCategoryNames
                 .Where(x => x.FixedAssetSubCategoryNameId != fixedAssetSubCategoryName.FixedAssetSubCategoryNameId)
                .Where(x => x.Name == fixedAssetSubCategoryName.Name).FirstOrDefault();
            if (exisit != null)
                ModelState.AddModelError("name", "Name already Exisits");
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetSubCategoryName", "Success", User.Identity.Name, $"Update FixedAssetSubCategoryName {entity.FixedAssetSubCategoryNameId} SubCatName {entity.FixedAssetSubCategoryNameId}", Modules.FixedAsset);
                #endregion
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete FixedAssetSubCategoryName {fixedAssetSubCategoryName.FixedAssetSubCategoryNameId} subCatName {fixedAssetSubCategoryName.FixedAssetSubCategoryNameId}", Modules.FixedAsset);
                #endregion
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
