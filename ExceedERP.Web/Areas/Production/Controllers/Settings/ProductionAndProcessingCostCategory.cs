using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Settings
{
    public class ProductionAndProcessingCostCategoryController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public ProductionAndProcessingCostCategoryController()
        {
            db = new ExceedDbContext();
        }
       
        public ActionResult Categories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionAndProcessingCostCategory> categories = db.ProductionAndProcessingCostCategories.Where(cc => cc.ParentId <= 0 || cc.ParentId == null);
            DataSourceResult result = categories.ToDataSourceResult(request, category => new
            {
                ProductionAndProcessingCostCategoryId = category.ProductionAndProcessingCostCategoryId,
                Title = category.Title,
                HasSubCategory = category.HasSubCategory,
                ParentId = category.ParentId,
                IsDeductive = category.IsDeductive,
                Remark = category.Remark,

                OrgBranchName = category.OrgBranchName,
                DateCreated = category.DateCreated,
                LastModified = category.LastModified,
                CreatedBy = category.CreatedBy,
                ModifiedBy = category.ModifiedBy,
            });

            return Json(result);
        }
        public ActionResult SubCategories_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionAndProcessingCostCategory> categories = db.ProductionAndProcessingCostCategories.Where(c => c.ParentId == id);
            DataSourceResult result = categories.ToDataSourceResult(request, category => new
            {
                ProductionAndProcessingCostCategoryId = category.ProductionAndProcessingCostCategoryId,
                Title = category.Title,
                HasSubCategory = category.HasSubCategory,
                ParentId = category.ParentId,
                IsDeductive = category.IsDeductive,
                Remark = category.Remark,

                OrgBranchName = category.OrgBranchName,
                DateCreated = category.DateCreated,
                LastModified = category.LastModified,
                CreatedBy = category.CreatedBy,
                ModifiedBy = category.ModifiedBy,
            });

            return Json(result);
        }
        private bool CategoryExists(ProductionAndProcessingCostCategory category) {
            return 
                db.ProductionAndProcessingCostCategories
                .FirstOrDefault(c => 
                    c.Title == category.Title && 
                    c.ProductionAndProcessingCostCategoryId != category.ProductionAndProcessingCostCategoryId
                    ) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Category_Create([DataSourceRequest]DataSourceRequest request, ProductionAndProcessingCostCategory category)
        {
            if (ModelState.IsValid)
            {
                if (CategoryExists(category))
                {
                    ModelState.AddModelError("", "Duplicate values not allowed!");
                }
                else { 
                    var entity = new ProductionAndProcessingCostCategory
                    {
                        //ProductionAndProcessingCostCategoryId = category.ProductionAndProcessingCostCategoryId,
                        Title = category.Title,
                        HasSubCategory = category.HasSubCategory,
                        ParentId = category.ParentId,
                        IsDeductive = category.IsDeductive,
                        Remark = category.Remark,

                        OrgBranchName = category.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = category.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = category.ModifiedBy,

                    };

                    db.ProductionAndProcessingCostCategories.Add(entity);
                    db.SaveChanges();

                    category.ProductionAndProcessingCostCategoryId = entity.ProductionAndProcessingCostCategoryId;

                    if(category.ParentId > 0)
                    {
                        this.ChangeAffectedCategories(category.ParentId);
                    }
                }
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubCategory_Create([DataSourceRequest]DataSourceRequest request, ProductionAndProcessingCostCategory category, int id)
        {
            if (ModelState.IsValid)
            {
                if (CategoryExists(category))
                {
                    ModelState.AddModelError("", "Duplicate values not allowed!");
                }
                else { 
                    var entity = new ProductionAndProcessingCostCategory
                    {
                        //ProductionAndProcessingCostCategoryId = category.ProductionAndProcessingCostCategoryId,
                        Title = category.Title,
                        HasSubCategory = category.HasSubCategory,
                        ParentId = id,
                        IsDeductive = ParentIsDeductive(id) ? true :  category.IsDeductive,
                        Remark = category.Remark,

                        OrgBranchName = category.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = category.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = category.ModifiedBy,

                    };

                    db.ProductionAndProcessingCostCategories.Add(entity);
                    db.SaveChanges();

                    category.ProductionAndProcessingCostCategoryId = entity.ProductionAndProcessingCostCategoryId;
                }
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Category_Update([DataSourceRequest]DataSourceRequest request, ProductionAndProcessingCostCategory category)
        {
            if (ModelState.IsValid)
            {
                if (CategoryExists(category))
                {
                    ModelState.AddModelError("", "Duplicate values not allowed!");
                }
                else
                {
                    var entity = new ProductionAndProcessingCostCategory
                    {
                        ProductionAndProcessingCostCategoryId = category.ProductionAndProcessingCostCategoryId,
                        Title = category.Title,
                        HasSubCategory = category.HasSubCategory,
                        ParentId = category.ParentId,
                        IsDeductive = ParentIsDeductive(category.ParentId) ? true : category.IsDeductive,
                        Remark = category.Remark,

                        OrgBranchName = category.OrgBranchName,
                        DateCreated = category.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = category.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.ProductionAndProcessingCostCategories.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    if (category.ParentId > 0)
                    {
                        this.ChangeAffectedCategories(category.ParentId);
                    }
                }
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Category_Destroy([DataSourceRequest]DataSourceRequest request, ProductionAndProcessingCostCategory category)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionAndProcessingCostCategory
                {
                    ProductionAndProcessingCostCategoryId = category.ProductionAndProcessingCostCategoryId,
                    Title = category.Title,
                    HasSubCategory = category.HasSubCategory,
                    ParentId = category.ParentId,
                    IsDeductive = category.IsDeductive,
                    Remark = category.Remark,

                    OrgBranchName = category.OrgBranchName,
                    DateCreated = category.DateCreated,
                    LastModified = category.LastModified,
                    CreatedBy = category.CreatedBy,
                    ModifiedBy = category.ModifiedBy,

                };

                db.ProductionAndProcessingCostCategories.Attach(entity);
                db.ProductionAndProcessingCostCategories.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        private void ChangeAffectedCategories(int? parentId)
        {
            var parentCategory = db.ProductionAndProcessingCostCategories.Find(parentId ?? 0);
            if (parentCategory != null && !parentCategory.HasSubCategory)
            {
                parentCategory.HasSubCategory = true;

                db.ProductionAndProcessingCostCategories.Attach(parentCategory);
                db.Entry(parentCategory).State = EntityState.Modified;
                db.SaveChanges();

                // if parent is dedudctive, make all the sub-category under this also deductive
                if (parentCategory.IsDeductive)
                {
                    var subList = db.ProductionAndProcessingCostCategories.Where(sc => sc.ParentId == parentCategory.ProductionAndProcessingCostCategoryId).ToList();
                    if (subList.Any())
                    {
                        foreach(var subCategory in subList)
                        {
                            subCategory.IsDeductive = true;

                            db.ProductionAndProcessingCostCategories.Attach(subCategory);
                            db.Entry(subCategory).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
        private bool ParentIsDeductive(int? parentId)
        {
            var category = db.ProductionAndProcessingCostCategories.Find(parentId);
            if(category != null)
            {
                return category.IsDeductive;
            }
            else
            {
                return false;
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}