using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{
    [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin)]

    public class FixedAssetSubCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateData();
            return View();
        }

        public void PopulateData()
        {
            var categories = db.FixedAssetCategories;
            ViewData["categories"] = categories.ToList();
        }

       
        public ActionResult FixedAssetSubCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetSubCategory> fixedassetsubcategories = db.FixedAssetSubCategories;
            DataSourceResult result = fixedassetsubcategories.ToDataSourceResult(request, fixedAssetSubCategory => new
            {
                FixedAssetSubCategoryID = fixedAssetSubCategory.FixedAssetSubCategoryID,
                CategoryCode = fixedAssetSubCategory.CategoryCode,
                SubCategoryName = fixedAssetSubCategory.SubCategoryName,
                SubCategoryCode = fixedAssetSubCategory.SubCategoryCode,
                Abbreviation = fixedAssetSubCategory.Abbreviation,
                IsGroup = fixedAssetSubCategory.IsGroup,

                DateCreated = fixedAssetSubCategory.DateCreated,
                LastModified = fixedAssetSubCategory.LastModified,
                CreatedBy = fixedAssetSubCategory.CreatedBy,
                ModifiedBy = fixedAssetSubCategory.ModifiedBy
            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetSubCategories_Create([DataSourceRequest]DataSourceRequest request, FixedAssetSubCategory fixedAssetSubCategory)
        {  
            
            #region validation

            bool valid = true;
           
            var exists = db.FixedAssetSubCategories.Where(x => x.SubCategoryCode == fixedAssetSubCategory.SubCategoryCode).Where(x => x.CategoryCode == fixedAssetSubCategory.CategoryCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Sub Category code", fixedAssetSubCategory.SubCategoryCode + " already exists in the system with in category "+fixedAssetSubCategory.CategoryCode);
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetSubCategory
                    {
                        CategoryCode = fixedAssetSubCategory.CategoryCode,
                        SubCategoryName = fixedAssetSubCategory.SubCategoryName,
                        SubCategoryCode = fixedAssetSubCategory.SubCategoryCode,
                        Abbreviation = fixedAssetSubCategory.Abbreviation,
                        IsGroup = fixedAssetSubCategory.IsGroup,

                        DateCreated = fixedAssetSubCategory.DateCreated,
                        LastModified = fixedAssetSubCategory.LastModified,
                        CreatedBy = fixedAssetSubCategory.CreatedBy,
                        ModifiedBy = fixedAssetSubCategory.ModifiedBy
                    };

                    db.FixedAssetSubCategories.Add(entity);
                    db.SaveChanges();
                    fixedAssetSubCategory.FixedAssetSubCategoryID = entity.FixedAssetSubCategoryID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetSubCategory", "Success", User.Identity.Name, $"created FixedAssetSubCategory {entity.FixedAssetSubCategoryID} Tag {entity.CategoryCode}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { fixedAssetSubCategory }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetSubCategories_Update([DataSourceRequest]DataSourceRequest request, FixedAssetSubCategory fixedAssetSubCategory)
        {
            #region validation

            bool valid = true;

            var exists = db.FixedAssetSubCategories
                .Where(x => x.FixedAssetSubCategoryID != fixedAssetSubCategory.FixedAssetSubCategoryID)
                .Where(x => x.SubCategoryCode == fixedAssetSubCategory.SubCategoryCode)
                .Where(x => x.CategoryCode == fixedAssetSubCategory.CategoryCode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Sub Category code", fixedAssetSubCategory.SubCategoryCode + " already exists in the system with in category " + fixedAssetSubCategory.CategoryCode);
                valid = false;
            }
            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetSubCategory
                    {
                        FixedAssetSubCategoryID = fixedAssetSubCategory.FixedAssetSubCategoryID,
                        CategoryCode = fixedAssetSubCategory.CategoryCode,
                        SubCategoryName = fixedAssetSubCategory.SubCategoryName,
                        SubCategoryCode = fixedAssetSubCategory.SubCategoryCode,
                        Abbreviation = fixedAssetSubCategory.Abbreviation,
                        IsGroup = fixedAssetSubCategory.IsGroup,

                        DateCreated = fixedAssetSubCategory.DateCreated,
                        LastModified = fixedAssetSubCategory.LastModified,
                        CreatedBy = fixedAssetSubCategory.CreatedBy,
                        ModifiedBy = fixedAssetSubCategory.ModifiedBy
                    };

                    db.FixedAssetSubCategories.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetSubCategory", "Success", User.Identity.Name, $"update FixedAssetSubCategory {entity.FixedAssetSubCategoryID} Tag {entity.CategoryCode}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { fixedAssetSubCategory }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetSubCategories_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetSubCategory fixedAssetSubCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetSubCategory
                {
                    FixedAssetSubCategoryID = fixedAssetSubCategory.FixedAssetSubCategoryID,
                    CategoryCode = fixedAssetSubCategory.CategoryCode,
                    SubCategoryName = fixedAssetSubCategory.SubCategoryName,
                    SubCategoryCode = fixedAssetSubCategory.SubCategoryCode,
                    Abbreviation = fixedAssetSubCategory.Abbreviation,
                    IsGroup = fixedAssetSubCategory.IsGroup,

                    DateCreated = fixedAssetSubCategory.DateCreated,
                    LastModified = fixedAssetSubCategory.LastModified,
                    CreatedBy = fixedAssetSubCategory.CreatedBy,
                    ModifiedBy = fixedAssetSubCategory.ModifiedBy
                };

                db.FixedAssetSubCategories.Attach(entity);
                db.FixedAssetSubCategories.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete FixedAssetSubCategory {fixedAssetSubCategory.FixedAssetSubCategoryID} Tag {fixedAssetSubCategory.CategoryCode}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetSubCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
