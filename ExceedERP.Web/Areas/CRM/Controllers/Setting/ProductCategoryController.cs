using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class ProductCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var thingCategoryList = db.ProductCategories.Select(c => new { c.ProductCategoryId, c.Description}).ToList();
            ViewData["CategoryList"] = thingCategoryList;
            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name}).ToList();

            CheckAndUpdateEmptyCategoryCodes();
            return View();

        }
        private void CheckAndUpdateEmptyCategoryCodes()
        {
            var emptyCode = db.ProductCategories.Where(c => c.InventoryCategoryCode == null).ToList();
            foreach (var category in emptyCode)
            {
                var catMapping = db.InventoryCateoryMaps.FirstOrDefault(cm => cm.Category == category.Description);
                if(catMapping != null)
                {
                    category.InventoryCategoryCode = catMapping.InventoryCategoryId;

                    db.ProductCategories.Attach(category);
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public ActionResult ProductCategories_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<ProductCategory> productCategories = db.ProductCategories;
            List<ProductCategory> listToReturn = new List<ProductCategory>();
            foreach (var category in productCategories)
            {
                category.HasTransaction = this.ProductCategoryHasTransaction(category);

                listToReturn.Add(category);
            }
            DataSourceResult result = listToReturn.ToDataSourceResult(request, thingCategory => new
            {
                ProductCategoryId = thingCategory.ProductCategoryId,
                Description = thingCategory.Description,
                Index = thingCategory.Index,
                Parent = thingCategory.Parent,
                InventoryCategoryCode = thingCategory.InventoryCategoryCode,
                HasTransaction = thingCategory.HasTransaction,
                UnEarnedAccountCode = thingCategory.UnEarnedAccountCode,
                InventoryAccountCode = thingCategory.InventoryAccountCode,
                SalesAccountCode = thingCategory.SalesAccountCode,
                IsActive = thingCategory.IsActive,
                CreatedBy = thingCategory.CreatedBy,
                DateCreated = thingCategory.DateCreated,
                LastModified = thingCategory.LastModified,
                ModifiedBy = thingCategory.ModifiedBy,
                OrgBranchName = thingCategory.OrgBranchName,
                Remark = thingCategory.Remark
            });
            ViewData["CategoryList"] = db.ProductCategories;
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductCategories_Create([DataSourceRequest]DataSourceRequest request, ProductCategory thingCategory)
        {
            List<ProductCategory> thingCategoryList = db.ProductCategories.Where(tc => tc.Index == thingCategory.Index).ToList();
            List<ProductCategory> thingCategoryLists = db.ProductCategories.Where(tc => tc.Description == thingCategory.Description).ToList();
            if (thingCategoryList.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Please readjust index value!");
            }
            else if (thingCategoryLists.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Category already exist!");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new ProductCategory
                    {
                        Description = thingCategory.Description,

                        Index = thingCategory.Index,
                        Parent = thingCategory.Parent,
                        IsActive = thingCategory.IsActive,
                        InventoryAccountCode = thingCategory.InventoryAccountCode,
                        InventoryCategoryCode = thingCategory.InventoryCategoryCode,
                        UnEarnedAccountCode = thingCategory.UnEarnedAccountCode,
                        SalesAccountCode = thingCategory.SalesAccountCode,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = thingCategory.Remark
                    };

                    db.ProductCategories.Add(entity);
                    db.SaveChanges();
                    thingCategory.ProductCategoryId = entity.ProductCategoryId;
                }
            }

            return Json(new[] { thingCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductCategories_Update([DataSourceRequest]DataSourceRequest request, ProductCategory thingCategory)
        {
            List<ProductCategory> thingCategoryList = db.ProductCategories.Where(tc => tc.Index == thingCategory.Index).ToList();
            List<ProductCategory> thingCategoryLists = db.ProductCategories.Where(tc => tc.Description == thingCategory.Description && tc.ProductCategoryId != thingCategory.ProductCategoryId).ToList();
            if (thingCategoryList.Count > 1)
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Please readjust index value!");
            }
            else if (thingCategoryLists.Any())
            {
                ModelState.AddModelError("DuplicateIndexEntry", "Category already exist!");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var ProductCategory = db.ProductCategories.Where(x => x.ProductCategoryId == thingCategory.ProductCategoryId).FirstOrDefault();

                    ProductCategory.Description = thingCategory.Description;
                    ProductCategory.Index = thingCategory.Index;
                    ProductCategory.Parent = thingCategory.Parent;
                    ProductCategory.InventoryAccountCode = thingCategory.InventoryAccountCode;
                    ProductCategory.InventoryCategoryCode = thingCategory.InventoryCategoryCode;
                    ProductCategory.SalesAccountCode = thingCategory.SalesAccountCode;
                    ProductCategory.UnEarnedAccountCode = thingCategory.UnEarnedAccountCode;
                    ProductCategory.IsActive = thingCategory.IsActive;
                    ProductCategory.Remark = thingCategory.Remark;

                    db.ProductCategories.Attach(ProductCategory);
                    db.Entry(ProductCategory).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }

            return Json(new[] { thingCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductCategories_Destroy([DataSourceRequest]DataSourceRequest request, ProductCategory thingCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductCategory
                {
                    ProductCategoryId = thingCategory.ProductCategoryId,
                    Description = thingCategory.Description,
                    Index = thingCategory.Index,
                    Parent = thingCategory.Parent,
                    InventoryAccountCode = thingCategory.InventoryAccountCode,
                    InventoryCategoryCode = thingCategory.InventoryCategoryCode,
                    SalesAccountCode = thingCategory.SalesAccountCode,
                    UnEarnedAccountCode = thingCategory.UnEarnedAccountCode,
                    IsActive = thingCategory.IsActive,
                    Remark = thingCategory.Remark
                };

                db.ProductCategories.Attach(entity);
                db.ProductCategories.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Product Category number " + thingCategory.ProductCategoryId);

            }

            return Json(new[] { thingCategory }.ToDataSourceResult(request, ModelState));
        }

        private bool ProductCategoryHasTransaction(ProductCategory category)
        {
            return db.Products.Any(c => c.CategoryID == category.ProductCategoryId.ToString()) ||
                db.InventoryCateoryMaps.Any(c => c.Category == category.Description);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
