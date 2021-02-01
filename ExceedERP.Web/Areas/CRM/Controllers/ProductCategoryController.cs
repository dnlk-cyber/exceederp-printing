using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess;
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
            List<ProductCategory> thingCategoryList = db.ProductCategories.ToList();
            ViewData["CategoryList"] = thingCategoryList;
            return View();

        }

        public ActionResult ProductCategories_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<ProductCategory> ProductCategories = db.ProductCategories;
            DataSourceResult result = ProductCategories.ToDataSourceResult(request, thingCategory => new
            {
                ProductCategoryId = thingCategory.ProductCategoryId,
                Description = thingCategory.Description,
                Index = thingCategory.Index,
                Parent = thingCategory.Parent,
                      InventoryAccountCode =      thingCategory.InventoryAccountCode,
                   SalesAccountCode =   thingCategory.SalesAccountCode,
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
                ModelState.AddModelError("DuplicateIndexEntry","Please readjust index value!");
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
                    ProductCategory.SalesAccountCode = thingCategory.SalesAccountCode;
                    ProductCategory.IsActive = thingCategory.IsActive;
                    ProductCategory.Remark = thingCategory.Remark;

                    db.Entry(ProductCategory).State = EntityState.Modified;
                    db.ProductCategories.Attach(ProductCategory);
                  
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
                    SalesAccountCode = thingCategory.SalesAccountCode,
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
