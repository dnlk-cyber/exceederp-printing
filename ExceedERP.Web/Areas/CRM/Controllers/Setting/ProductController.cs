using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

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
            //var ProductCategories = .AsQueryable();
            var ItemCategories = db.ItemCategorys.AsQueryable();
            var Items = db.InventoryItems.AsQueryable();

            var uom = db.Lookups.Where(u => u.LookupType == LookUpTypes.UOM).AsQueryable();
            ViewData["ProductCategories"] = db.ProductCategories.Select(c => new { c.ProductCategoryId, c.Description }).ToList();
            ViewData["ProductMainCategories"] = db.ProductCategories.Where(c => c.Parent == null).Select(c => new { c.ProductCategoryId, c.Description }).ToList();
            ViewData["ProductSubCategories"] = db.ProductCategories.Where(c => c.Parent != null).Select(c => new { c.ProductCategoryId, c.Description }).ToList();

            ViewData["ItemCategory"] = ItemCategories.Select(tcategory => new Reference() { Code = tcategory.ItemCategoryCode.ToString(), Description = tcategory.Name }).ToList();
            ViewData["Item"] = Items.Select(tcategory => new Reference() { Code = tcategory.ItemCode.ToString(), Description = tcategory.Name + " " + tcategory.ItemCode }).ToList();

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
        public ActionResult UpdateSubCategory()
        {
            //var textileCat = db.ItemCategorys.Where(c => c.ItemCategoryCode == )
            // textile category only
            var emptySubs = db.Products.Where(p => string.IsNullOrEmpty(p.SubCategoryId)).ToList();
            if (emptySubs.Any())
            {
                foreach(var prod in emptySubs)
                {
                    var inventoryItem = db.InventoryItems.FirstOrDefault(i => i.ItemCode == prod.ItemCode);

                    if(inventoryItem != null && inventoryItem.SubCategoryCode != null)
                    {
                        var subCategory = db.ProductCategories.FirstOrDefault(p => p.InventoryCategoryCode == inventoryItem.SubCategoryCode);
                        if(subCategory != null)
                        {

                            prod.SubCategoryId = subCategory.ProductCategoryId.ToString();

                            db.Products.Attach(prod);
                            db.Entry(prod).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }

            return Redirect(nameof(Index));
        }
        public ActionResult GenerateProducts()
        {
            var allCategories = db.ProductCategories.ToList();
            foreach (ProductCategory productCategory in allCategories)
            {
                var items = db.InventoryItems.Where(i => i.ItemCategoryCode == productCategory.InventoryCategoryCode).ToList();
                foreach (var item in items)
                {
                    var existingItem = db.Products.FirstOrDefault(i => i.ItemCode == item.ItemCode);
                    if(existingItem == null)
                    {
                        var product = new Product
                        {
                            ItemCode = item.ItemCode,
                            Name = item.Name,
                            CategoryID = productCategory.ProductCategoryId.ToString(),
                            IsActive = true,
                            CreatedBy = null,
                            DateCreated = null,
                            LastModified = null,
                            ModifiedBy = null,
                            ItemCategoryCode = item.SubCategoryCode,
                            OrgBranchName = null,
                            Price = item.AvgPrice,
                            UOM = item.UoMName,
                        };
                        db.Products.Add(product);
                        db.SaveChanges();
                    }
                }
            }
            return Redirect(nameof(Index));
        }
        public ActionResult CompareNamesWithInventory()
        {

            var products = db.Products.ToList();
            foreach (Product product in products)
            {
                var inventoryItem = db.InventoryItems.Find(product.ItemCode);
                if(inventoryItem != null && product.Name != inventoryItem.Name)
                {
                    product.Name = inventoryItem.Name;

                    db.Products.Attach(product);
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Redirect(nameof(Index));
        }
        public ActionResult Products_Read([DataSourceRequest]DataSourceRequest request)
        {


            List<Product> products = db.Products.ToList();
            List<Product> listToReturn = new List<Product>();
            foreach (Product product in products)
            {
                product.HasTransaction = this.ProductHasTransaction(product);
                listToReturn.Add(product);
            }
            DataSourceResult result = listToReturn.AsQueryable().ToDataSourceResult(request, product => new Product
            {
                ProductID = product.ProductID,
                Name = product.Name,
                CategoryID = product.CategoryID,
                SubCategoryId = product.SubCategoryId,
                UOM = product.UOM,
                HasTransaction = product.HasTransaction,
                Price = product.Price,
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
            List<Product> InventoryItem = db.Products.Where(p => p.ItemCode == product.ItemCode).ToList();
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
                        SubCategoryId = product.SubCategoryId,
                        UOM = product.UOM,
                        Price = product.Price,
                        IsActive = product.IsActive,
                        Remark = product.Remark,
                        ItemCategoryCode = product.ItemCategoryCode,
                        ItemCode = product.ItemCode,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.Products.Add(entity);
                    db.SaveChanges();
                    product.ProductID = entity.ProductID;
                    var ProductCategory = db.ProductCategories.Where(x => x.ProductCategoryId.ToString() == product.CategoryID).FirstOrDefault();

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
                    SubCategoryId = product.SubCategoryId,
                    UOM = product.UOM,
                    Price = product.Price,
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

            if (entity != null && ProductHasTransaction(product))
            {
                ModelState.AddModelError("Product_transaction", "Product is Used in Transactions. It Cannot be deleted!");
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Fail", User.Identity.Name, "An Attempt to delete Product with Id " + product.ProductID + "failed because it is used in active transactions.", Modules.CRM);

                return Json(new[] { product }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (entity != null)
                {
                    db.Products.Attach(entity);
                    db.Products.Remove(entity);
                    db.SaveChanges();

                    UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Product number " + product.ProductID, Modules.CRM);
                    return Json(new[] { product }.ToDataSourceResult(request, ModelState));
                }
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
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
        private bool ProductHasTransaction(Product product)
        {
            return db.LineItems.Any(l => l.ThingID == product.ProductID);

        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
