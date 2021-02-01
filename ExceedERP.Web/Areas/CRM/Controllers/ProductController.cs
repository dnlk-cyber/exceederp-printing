﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
  using ExceedERP.Core.Domain.Common;
 using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;  



namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class ProductController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var storeList = db.InventoryStores.Select(p => new { StoreCode = p.StoreCode, Name = p.Name }).ToList();
            var accountList = db.Accounts;
            var periodList = db.Periods;
            var objectStateDefinitionList = db.ObjectStateDefinitions;
            var ProductCategories = db.ProductCategories.AsQueryable();
            var ItemCategories = db.ItemCategorys.AsQueryable();
            var Items = db.InventoryItems.AsQueryable();

            var uom = db.Lookups.Where(u => u.LookupType == LookUpTypes.UOM).AsQueryable();
            ViewData["ThingCategory"] = ProductCategories.Select(tcategory => new Reference() { Code = tcategory.ProductCategoryId.ToString(), Description = tcategory.Description }).ToList();

            ViewData["ItemCategory"] = ItemCategories.Select(tcategory => new Reference() { Code = tcategory.ItemCategoryCode.ToString(), Description = tcategory.Name }).ToList();
            ViewData["Item"] = Items.Select(tcategory => new Reference() { Code = tcategory.ItemCode.ToString(), Description = tcategory.Name }).ToList();

            //ViewData["defaultThingCategory"] = ProductCategories.FirstOrDefault();
            ViewData["PeriodList"] = periodList.Where(pl => pl.PeriodType == PeriodType.ACCOUNTING_PERIOD).AsQueryable();
            //ViewData["defaultPeriodList"] = periodList.FirstOrDefault();
            ViewData["ListOfAccounts"] = accountList;
            //ViewData["defaultListOfAccounts"] = accountList.FirstOrDefault();
            ViewData["StoreList"] = storeList;
            //ViewData["defaultStoreList"] = storeList.FirstOrDefault();
            ViewData["ObjectStateDefinitionList"] = objectStateDefinitionList;
            //ViewData["defaultObjectStateDefinitionList"] = objectStateDefinitionList.FirstOrDefault();
            ViewData["UOM"] = uom.AsQueryable();

            ViewData["SpecificationCategory"] = db.Lookups.Where(lup => lup.LookupType == LookUpTypes.PROJECT_TYPE).AsQueryable();
            return View();
        }

        public ActionResult Products_Read([DataSourceRequest]DataSourceRequest request)
        {


            List<Product> products = db.Products.ToList();
 
            DataSourceResult result = products.ToDataSourceResult(request, product => new Product
            {
                ProductID = product.ProductID,
                Name = product.Name,
                CategoryID = product.CategoryID,
                UOM = product.UOM,
               IsActive = product.IsActive,
                ItemCategoryCode = product.ItemCategoryCode,
                ItemCode = product.ItemCode,
                CreatedBy = product.CreatedBy,
                DateCreated = product.DateCreated,
                LastModified = product.LastModified,
                ModifiedBy = product.ModifiedBy
               
            

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Create([DataSourceRequest]DataSourceRequest request, Product product)
        {
            List<Product> products = db.Products.Where(p => p.Name == product.Name && p.ItemCode == product.ItemCode).ToList();
            List<Product> InventoryItem = db.Products.Where(p =>  p.ItemCode == product.ItemCode).ToList();
            if (products.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "Product Already exist");
            }
            else if (InventoryItem.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "Inventory Item Already Mapped !");
            }
            else
            {
                if (ModelState.IsValid)
                {
                   
                    var entity = new Product
                    {
                        Name = product.Name,
                        CategoryID = product.CategoryID,
                        UOM = product.UOM,
                        IsActive = product.IsActive,
                        Remark = product.Remark,
                        ItemCategoryCode = product.ItemCategoryCode,
                        ItemCode = product.ItemCode,
                        DateCreated=DateTime.Now,
                        CreatedBy=User.Identity.Name

                    };

                    db.Products.Add(entity);
                    db.SaveChanges();
                    product.ProductID = entity.ProductID;
                    var ProductCategory = db.ProductCategories.Where(x=>x.ProductCategoryId.ToString() == product.CategoryID).FirstOrDefault();
                  
                    var AccMap = new AccountMap
                    {
                     
                        AccountID = ProductCategory.SalesAccountCode,
                        ReferenceType = "Product",
                        ControlAccountID = ExceedERP.Core.Domain.CRM.AccountRequirementDefinition.SALES_ACCOUNT.ToString(),
                        Reference = product.ProductID.ToString(),
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                       
                    };

                    db.AccountMaps.Add(AccMap);
                    db.SaveChanges();
                }

            }
            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Update([DataSourceRequest]DataSourceRequest request, Product product)
        {
         
                if (ModelState.IsValid)
                {
                    var entity = new Product
                    {
                        ProductID = product.ProductID,
                        Name = product.Name,
                        CategoryID = product.CategoryID,
                        UOM = product.UOM,
                        IsActive = product.IsActive,
                        Remark = product.Remark,
                        ItemCategoryCode = product.ItemCategoryCode,
                        ItemCode = product.ItemCode,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.Products.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                
            }
            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Products_Destroy([DataSourceRequest]DataSourceRequest request, Product product)
        {
            Product entity = db.Products.Where(x => x.ProductID == product.ProductID).FirstOrDefault();

            if (entity != null)
            {
                db.Products.Attach(entity);
                db.Products.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Product number " + product.ProductID);
      
            }

            return Json(new[] { product }.ToDataSourceResult(request));
        }


        public ActionResult ProductDDBind()
        {

            var customers = db.InventoryItems.Select(s => new BindingModel
            {
                Name = s.Name,
                Id = s.ItemCode.ToString()
            });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
