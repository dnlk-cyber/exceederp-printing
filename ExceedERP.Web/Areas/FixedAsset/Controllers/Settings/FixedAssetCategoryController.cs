using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{

    [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin)]
    public class FixedAssetCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateData();
            return View();
        }

        public void PopulateData()
        {
            var fixedAssetGroup = db.FixedAssetGroups;
            ViewData["fixedAssetGroupsa"] = fixedAssetGroup.ToList();
        }

          public ActionResult FixedAssetCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetCategory> fixedassetcategories = db.FixedAssetCategories;
            DataSourceResult result = fixedassetcategories.ToDataSourceResult(request, fixedAssetCategory => new
            {
                FixedAssetCategoryID = fixedAssetCategory.FixedAssetCategoryID,
                CategoryName = fixedAssetCategory.CategoryName,
                CategoryCode = fixedAssetCategory.CategoryCode,
                FixedAssetGroupID = fixedAssetCategory.FixedAssetGroupID,
                DepreciationCategory = fixedAssetCategory.DepreciationCategory,
                Abbreviation = fixedAssetCategory.Abbreviation,
                FixedAssetClass = fixedAssetCategory.FixedAssetClass,

                DateCreated = fixedAssetCategory.DateCreated,
                LastModified = fixedAssetCategory.LastModified,
                CreatedBy = fixedAssetCategory.CreatedBy,
                ModifiedBy = fixedAssetCategory.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCategories_Create([DataSourceRequest]DataSourceRequest request, FixedAssetCategory fixedAssetCategory)
        {
            #region validation

            bool valid = true;

            var exists = db.FixedAssetCategories.Where(x => x.CategoryCode == fixedAssetCategory.CategoryCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Category code", fixedAssetCategory.CategoryCode + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetCategory
                    {
                        CategoryName = fixedAssetCategory.CategoryName,
                        CategoryCode = fixedAssetCategory.CategoryCode,
                        FixedAssetGroupID = fixedAssetCategory.FixedAssetGroupID,
                        DepreciationCategory = fixedAssetCategory.DepreciationCategory,
                        Abbreviation = fixedAssetCategory.Abbreviation,
                        FixedAssetClass = fixedAssetCategory.FixedAssetClass,

                        DateCreated = fixedAssetCategory.DateCreated,
                        LastModified = fixedAssetCategory.LastModified,
                        CreatedBy = fixedAssetCategory.CreatedBy,
                        ModifiedBy = fixedAssetCategory.ModifiedBy
                    };

                    db.FixedAssetCategories.Add(entity);
                    db.SaveChanges();
                    fixedAssetCategory.FixedAssetCategoryID = entity.FixedAssetCategoryID;
                }
            }

            return Json(new[] { fixedAssetCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCategories_Update([DataSourceRequest]DataSourceRequest request, FixedAssetCategory fixedAssetCategory)
        {
            #region validation

            bool valid = true;

            var exists = db.FixedAssetCategories.Where(x => x.FixedAssetCategoryID != fixedAssetCategory.FixedAssetCategoryID).Where(x => x.CategoryCode == fixedAssetCategory.CategoryCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Category code", fixedAssetCategory.CategoryCode + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetCategory
                    {
                        FixedAssetCategoryID = fixedAssetCategory.FixedAssetCategoryID,
                        CategoryName = fixedAssetCategory.CategoryName,
                        CategoryCode = fixedAssetCategory.CategoryCode,
                        FixedAssetGroupID = fixedAssetCategory.FixedAssetGroupID,
                        DepreciationCategory = fixedAssetCategory.DepreciationCategory,
                        Abbreviation = fixedAssetCategory.Abbreviation,
                        FixedAssetClass = fixedAssetCategory.FixedAssetClass,

                        DateCreated = fixedAssetCategory.DateCreated,
                        LastModified = fixedAssetCategory.LastModified,
                        CreatedBy = fixedAssetCategory.CreatedBy,
                        ModifiedBy = fixedAssetCategory.ModifiedBy
                    };

                    db.FixedAssetCategories.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { fixedAssetCategory }.ToDataSourceResult(request, ModelState));
        }

        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetCategories_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetCategory fixedAssetCategory)
        {
            #region validation

            bool valid = true;
            var categories = db.FixedAssetSubCategories.Where(x => x.CategoryCode == fixedAssetCategory.CategoryCode).ToList();

            if (categories.Any())
            {
                ModelState.AddModelError("Category code", fixedAssetCategory.CategoryCode + " Cannot be deleted ,it has sub categories under it.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetCategory
                    {
                        FixedAssetCategoryID = fixedAssetCategory.FixedAssetCategoryID,
                        CategoryName = fixedAssetCategory.CategoryName,
                        CategoryCode = fixedAssetCategory.CategoryCode,
                        FixedAssetGroupID = fixedAssetCategory.FixedAssetGroupID,
                        DepreciationCategory = fixedAssetCategory.DepreciationCategory,
                        Abbreviation = fixedAssetCategory.Abbreviation,
                        FixedAssetClass = fixedAssetCategory.FixedAssetClass,
                        DateCreated = fixedAssetCategory.DateCreated,
                        LastModified = fixedAssetCategory.LastModified,
                        CreatedBy = fixedAssetCategory.CreatedBy,
                        ModifiedBy = fixedAssetCategory.ModifiedBy
                    };

                    db.FixedAssetCategories.Attach(entity);
                    db.FixedAssetCategories.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { fixedAssetCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
