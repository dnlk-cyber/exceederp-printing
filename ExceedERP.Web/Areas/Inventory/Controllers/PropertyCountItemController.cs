using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{

    public class PropertyCountItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //private List<PropertyCountItem> _propertyItems = new List<PropertyCountItem>();

         [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        public ActionResult Index()
        {
            return View();
        }

        

         [AuthorizeRoles(InventoryConstants.PropertyCount, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyCountApprover)]
        public ActionResult PropertyCountItems_Read([DataSourceRequest]DataSourceRequest request , int id)
            {
                IQueryable<PropertyCountItem> propertycountItems = db.PropertyCountItems
                .Where(x => x.PropertyCountId == id)
                .OrderBy(x => x.ItemCodeReadOnly);
                //.AsQueryable();
                DataSourceResult result = propertycountItems.ToDataSourceResult(request, propertyCountItem => new {
                 
                    PropertyCountItemId = propertyCountItem.PropertyCountItemId,
                    PropertyCountId = propertyCountItem.PropertyCountId,
                    ItemCategoryCode = propertyCountItem.ItemCategoryCode,
                    ItemCode = propertyCountItem.ItemCode,
                    ItemCodeReadOnly = propertyCountItem.ItemCodeReadOnly,
                    ItemName = propertyCountItem.ItemName,
                    Count = propertyCountItem.Count,
                    PreviousBalance = propertyCountItem.PreviousBalance,
                    Price = propertyCountItem.Price,
                    Location = propertyCountItem.Location,
                    Status = propertyCountItem.Status
                });

                return Json(result);
            }

         [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PropertyCountItems_Create([DataSourceRequest]DataSourceRequest request, PropertyCountItem propertyCountItem,int id)
         {
             var record = db.PropertyCounts.Find(id);
             var exist =
                 db.PropertyCountItems
                     .Where(x => x.ItemCode == propertyCountItem.ItemCode)
                     .FirstOrDefault(x => x.PropertyCountId == id);
             if (exist != null)
             {
                 ModelState.AddModelError("exist","Item: "+propertyCountItem.ItemCode+ " already exists in this count sheet.");
             }

             var item = db.InventoryItems.Find(propertyCountItem.ItemCode);

            if (ModelState.IsValid)
            {
                if (item != null)
                {
                    var entity = new PropertyCountItem
                    {
                        PropertyCountId = id,
                        ItemCategoryCode = propertyCountItem.ItemCategoryCode,
                        ItemCode = propertyCountItem.ItemCode,
                        ItemCodeReadOnly = propertyCountItem.ItemCode.Trim(),
                        ItemName = item.Name,
                        Count = propertyCountItem.Count,
                        Price = propertyCountItem.Price,
                        CreatedBy = User.Identity.Name,
                        Status = true,
                        Location = propertyCountItem.Location,
                        DateCreated = DateTime.Now
                    };

                    db.PropertyCountItems.Add(entity);
                    db.SaveChanges();
                    propertyCountItem.PropertyCountItemId = entity.PropertyCountItemId;
                }

                //if not in store add to store and branch
                var propertycount = db.PropertyCounts.Find(id);
                var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == propertycount.StoreCode && x.ItemCode == propertyCountItem.ItemCode);
                if (storeItem == null)
                {
                    #region add to store

                    if (record != null)
                    {
                        storeItem = new StoreItemAssignment
                        {
                            StoreCode = record.StoreCode,
                            ItemCategoryCode = propertyCountItem.ItemCategoryCode,
                            ItemCode = propertyCountItem.ItemCode,
                            Balance = propertyCountItem.Count,
                            AvgPrice = propertyCountItem.Price,
                            ItemLocation = propertyCountItem.Location,
                        };
                        db.StoreItemAssignments.Add(storeItem);
                        db.SaveChanges();
                    }

                    #endregion
                    #region add to branch

                    if (record != null)
                    {
                        var branchItem = new BranchItemAssignment();
                        branchItem.BranchId = record.BranchId;
                        branchItem.ItemCategoryCode = propertyCountItem.ItemCategoryCode;
                        branchItem.ItemCode = propertyCountItem.ItemCode;
                        branchItem.Balance = propertyCountItem.Count;
                        branchItem.AvgPrice = propertyCountItem.Price;
                        db.BranchItemAssignments.Add(branchItem);
                        db.SaveChanges();
                    }

                    #endregion
                }
            }

            return Json(new[] { propertyCountItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PropertyCountItems_Update([DataSourceRequest]DataSourceRequest request, PropertyCountItem propertyCountItem)
        {
            var count = db.PropertyCounts.Find(propertyCountItem.PropertyCountId);
            if (ModelState.IsValid)
            {
                var entity = new PropertyCountItem
                {
                    PropertyCountItemId = propertyCountItem.PropertyCountItemId,
                    ItemCategoryCode = propertyCountItem.ItemCategoryCode,
                    ItemCode = propertyCountItem.ItemCode,
                    ItemCodeReadOnly = propertyCountItem.ItemCode.Trim(),
                    ItemName = propertyCountItem.ItemName,
                    PreviousBalance = propertyCountItem.PreviousBalance,
                    Count = propertyCountItem.Count,
                    PropertyCountId = propertyCountItem.PropertyCountId,
                    Status = true,
                    Price= propertyCountItem.Price,
                    Location = propertyCountItem.Location,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };
              
                //var propertyCount = db.PropertyCounts.Find(entity.PropertyCountId);
                //var storeItem =db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == propertyCount.StoreCode && x.ItemCode == propertyCountItem.ItemCode);
                //if (storeItem != null) entity.PreviousBalance =storeItem.Balance;
                //if (count != null)
                //    entity.Price = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(propertyCountItem.ItemCode, count.StoreCode);


                db.PropertyCountItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { propertyCountItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PropertyCountItems_Destroy([DataSourceRequest]DataSourceRequest request, PropertyCountItem propertyCountItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PropertyCountItem
                {
                    PropertyCountItemId = propertyCountItem.PropertyCountItemId,
                    ItemCategoryCode = propertyCountItem.ItemCategoryCode,
                    ItemCode = propertyCountItem.ItemCode,
                    Count = propertyCountItem.Count,
                    Status = propertyCountItem.Status
                };

                db.PropertyCountItems.Attach(entity);
                db.PropertyCountItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted property count number " + propertyCountItem.PropertyCountId + " " + propertyCountItem.PropertyCount.Remark);
                #endregion
            }

            return Json(new[] { propertyCountItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(InventoryConstants.PropertyCount)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public ActionResult Change(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var propertyCountItem = db.PropertyCountItems.Find(id);
            if (propertyCountItem == null)
            {
                return HttpNotFound();
            }
            propertyCountItem.Status = true;
            db.Entry(propertyCountItem).State = EntityState.Modified;
            db.SaveChanges();
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
