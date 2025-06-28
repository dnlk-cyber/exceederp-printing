using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM.TenderManagement;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class SalesLotItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        
        public ActionResult SalesLotItems_Read([DataSourceRequest]DataSourceRequest request, int saleLotId)
        {
            IQueryable<SalesLotItem> salesLots= db.SalesLotItems.Where(x=>x.SalesLotId== saleLotId);
            DataSourceResult result = salesLots.ToDataSourceResult(request, salesLot => new SalesLotItem {
                SalesLotItemId = salesLot.SalesLotItemId,
                SalesLotId = salesLot.SalesLotId,               
                ProductCategoryDesc=salesLot.ProductCategoryDesc,
             // CategoryID=salesLot.ProductCategoryId,
                ProductCategoryId=salesLot.ProductCategoryId,
                ProductName = salesLot.ProductName,
                ProductID =salesLot.ProductID,               
                Quantity = salesLot.Quantity,
                UOM=salesLot.UOM,
                ProductArea=salesLot.ProductArea,
                ProductAreaNumber=salesLot.ProductAreaNumber,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified
               
            });

            return Json(result);
        }

        public ActionResult SalesLotItems_Create([DataSourceRequest]DataSourceRequest request, SalesLotItem salesLot, int saleLotId)
        {
            salesLot.SalesLotId = saleLotId;
            if (salesLot.ProductID == null)
            {
                
                var fetchAllProducts = db.Products.Where(Pro => Pro.CategoryID == salesLot.ProductCategoryId.ToString()).ToList();
                if (fetchAllProducts.Any())
                {
                    foreach (var product in fetchAllProducts)
                    {
                        var fetchStore = db.SalesLots.SingleOrDefault(sl => sl.SalesLotId == saleLotId);
                           
                            
                            var fetchquantity = db.StoreItemAssignments.FirstOrDefault(itm => itm.ItemCode == product.ItemCode&&itm.StoreCode==fetchStore.StoreCode);
                            if (fetchquantity != null)
                            {
                                salesLot.Quantity = fetchquantity.Balance;
                            }
                            else
                                salesLot.Quantity = 0;
                            salesLot.ProductID = product.ProductID;
                            salesLot.ProductName = product.Name;
                            salesLot.UOM = product.UOM;                        
                            salesLot.ProductArea = fetchStore.StoreCode;
                            salesLot.ProductAreaNumber = 0;
                            var xx = db.ProductCategories.SingleOrDefault(p => p.ProductCategoryId == salesLot.ProductCategoryId);
                            //  var yy = db.Products.SingleOrDefault(p => p.ProductID == salesLot.ProductID);

                            //  salesLot.ProductName = yy.Name;
                            salesLot.ProductCategoryDesc = xx.Description;
                            // salesLot.UOM = yy.UOM;

                            var registeredPrductt = db.SalesLotItems.SingleOrDefault(r => r.SalesLotId == saleLotId && r.ProductID == product.ProductID);

                            if (registeredPrductt == null)
                            {
                                if (ModelState.IsValid)
                                {
                                    salesLot.DateCreated = DateTime.Now;
                                    salesLot.CreatedBy = User.Identity.Name;
                                    var entity = GetMappedObject(salesLot);
                                    db.SalesLotItems.Add(entity);
                                    db.SaveChanges();
                                    salesLot.SalesLotItemId = entity.SalesLotItemId;
                                }
                            }
                        

                    }
                }
            }

            var x = db.ProductCategories.SingleOrDefault(p => p.ProductCategoryId == salesLot.ProductCategoryId);
            var y = db.Products.SingleOrDefault(p => p.ProductID == salesLot.ProductID);

            salesLot.ProductName = y.Name;
            salesLot.ProductCategoryDesc = x.Description;
            salesLot.UOM = y.UOM;
            var registeredPrduct = db.SalesLotItems.SingleOrDefault(r => r.SalesLotId == saleLotId && r.ProductID == salesLot.ProductID);
            if (registeredPrduct != null)
                ModelState.AddModelError(" item existed", "This item is registered under this lot");
                  

                if (ModelState.IsValid)
                {
                    salesLot.DateCreated = DateTime.Now;
                    salesLot.CreatedBy = User.Identity.Name;
                    var entity = GetMappedObject(salesLot);
                    db.SalesLotItems.Add(entity);
                    db.SaveChanges();
                    salesLot.SalesLotItemId = entity.SalesLotItemId;
                }
            
            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotItems_Update([DataSourceRequest]DataSourceRequest request, SalesLotItem salesLot)
        {
            var x = db.ProductCategories.SingleOrDefault(p => p.ProductCategoryId == salesLot.ProductCategoryId);
            var y = db.Products.SingleOrDefault(p => p.ProductID == salesLot.ProductID);

            salesLot.ProductName = y.Name;
            salesLot.ProductCategoryDesc = x.Description;
     

            if (ModelState.IsValid)
            {
                salesLot.LastModified = DateTime.Now;
                salesLot.ModifiedBy = User.Identity.Name;

                var registeredProduct = db.SalesLotItems.SingleOrDefault(r => r.SalesLotItemId == salesLot.SalesLotItemId);
                registeredProduct.Quantity = salesLot.Quantity;
               // registeredProduct.ProductCategory = salesLot.ProductCategory;                
                registeredProduct.ModifiedBy = salesLot.ModifiedBy;
                registeredProduct.UOM = y.UOM;
                registeredProduct.ProductName = y.Name;
                registeredProduct.ProductCategoryDesc = x.Description;
                registeredProduct.LastModified = salesLot.LastModified;
                registeredProduct.ProductID = salesLot.ProductID;
                registeredProduct.ProductCategoryId = salesLot.ProductCategoryId;
                db.Entry(registeredProduct).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotItems_Destroy([DataSourceRequest]DataSourceRequest request, SalesLotItem salesLot)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(salesLot);
                db.SalesLotItems.Attach(entity);
                db.SalesLotItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }

        private SalesLotItem GetMappedObject(SalesLotItem salesLot)
        {
            var entity = new SalesLotItem
            {
                SalesLotItemId = salesLot.SalesLotItemId,
                SalesLotId = salesLot.SalesLotId,
                ProductCategoryId = salesLot.ProductCategoryId,
                ProductName = salesLot.ProductName,
                ProductCategoryDesc=salesLot.ProductCategoryDesc,
                ProductID = salesLot.ProductID,
                Quantity = salesLot.Quantity,
                UOM = salesLot.UOM,
                ProductArea = salesLot.ProductArea,
                ProductAreaNumber = salesLot.ProductAreaNumber,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
