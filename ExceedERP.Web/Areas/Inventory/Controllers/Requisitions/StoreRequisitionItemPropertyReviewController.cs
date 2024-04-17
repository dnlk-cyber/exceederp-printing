using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Requisitions
{
    [Authorize]
    
    public class StoreRequisitionItemPropertyReviewController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

          [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
        public ActionResult Index()
        {
            return View();
        }

        private bool IsSRReviewed(int id)
        {
            var sr = db.StoreRequisitions.Find(id);
            if(sr!=null)
            {
                if (sr.IsSecondOnlineApproved)
                    return true;
            }
            return false;
        }

        private string GetItemUom(string itemcode)
        {
            var item = db.InventoryItems.Find(itemcode);
            if (item != null)
            {
                return item.UoMName;
            }
            return "";
        }

        private string GetStore(int id)
        {
            return db.StoreRequisitions.Find(id).StoreCode;
        }

        [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
        public ActionResult StoreRequsitionItemPropertyReviews_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<StoreRequisitionItem> storerequisitionItems = db.StoreRequisitionItems.Where(p => p.StoreRequisitionID == id);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new {

                StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                ItemCode = storeRequisitionItem.ItemCode,
                PartNo = storeRequisitionItem.PartNo,
                ModelNo = storeRequisitionItem.ModelNo,
                SerialNo = storeRequisitionItem.SerialNo,
                ChasisNo = storeRequisitionItem.ChasisNo,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,
                IsApproved = storeRequisitionItem.IsApproved,
                AgreementPrice = storeRequisitionItem.AgreementPrice,
                ItemSpecification = storeRequisitionItem.ItemSpecification,
                Unit = GetItemUom(storeRequisitionItem.ItemCode),
                CurrentBalance = IsSRReviewed(storeRequisitionItem.StoreRequisitionID) ? storeRequisitionItem.CurrentBalance : ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, GetStore(storeRequisitionItem.StoreRequisitionID)),
                CurrentAPrice = IsSRReviewed(storeRequisitionItem.StoreRequisitionID) ? storeRequisitionItem.CurrentAPrice : ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(storeRequisitionItem.ItemCode, GetStore(storeRequisitionItem.StoreRequisitionID)),
            });

            return Json(result);
        }


         [AuthorizeRoles(InventoryConstants.PropertyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequsitionItemPropertyReviews_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem , int id)
         {
             var currentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode,
                 storeRequisitionItem.StoreRequisition.StoreCode);
             if (storeRequisitionItem.IssuedQuantity >currentBalance)
             {
                 ModelState.AddModelError("nobalance", "There is no enough balance in store for item: "+storeRequisitionItem.InventoryItem.Name+" current balance is: "+currentBalance);
             }

            if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity >storeRequisitionItem.ApprovedQuantity)
            {
                ModelState.AddModelError("above","The sum of issue and purchase quantity cannot be more than the approved quantity");
            }

            if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity <storeRequisitionItem.ApprovedQuantity)
            {
               // ModelState.AddModelError("below", "The sum of issue and purchase quantity are less than the approved quantity");
            }
            if (storeRequisitionItem.GeneratePurchaseReq && storeRequisitionItem.PurchaseQuantity==0)
            {
                ModelState.AddModelError("below", "Purchase quantity is not set");
            }

            var sr = db.StoreRequisitions.Find(id);
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {
                    StoreRequisitionID = id,
                    ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                    ItemCode = storeRequisitionItem.ItemCode,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                    IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,
                    CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),
                    CurrentAPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(storeRequisitionItem.ItemCode, sr.StoreCode),

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };

                db.StoreRequisitionItems.Add(entity);
                db.SaveChanges();
                storeRequisitionItem.StoreRequisitionItemID = entity.StoreRequisitionItemID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition review item", "Success", User.Identity.Name, $"created store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


         [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequsitionItemPropertyReviews_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
         {
             var storeRe = db.StoreRequisitions.Find(storeRequisitionItem.StoreRequisitionID);
             if (storeRe != null)
             {
                 var currentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, storeRe.StoreCode);
                 if (storeRequisitionItem.IssuedQuantity > currentBalance)
                 {
                     ModelState.AddModelError("nobalance", "There is no enough balance in store for item: " + storeRequisitionItem.ItemCode + " current balance is: " + currentBalance);
                 }
             }
            if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity > storeRequisitionItem.ApprovedQuantity)
            {
                ModelState.AddModelError("above", "The sum of issue and purchase quantity cannot be more than the approved quantity");
            }

            if (storeRequisitionItem.PurchaseQuantity + storeRequisitionItem.IssuedQuantity < storeRequisitionItem.ApprovedQuantity)
            {
                //ModelState.AddModelError("below", "The sum of issue and purchase quantity are less than the approved quantity");
            }
            if (storeRequisitionItem.GeneratePurchaseReq && storeRequisitionItem.PurchaseQuantity == 0)
            {
                ModelState.AddModelError("below", "Purchase quantity is not set");
            }

            var sr = db.StoreRequisitions.Find(storeRequisitionItem.StoreRequisitionID);
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {

                    StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                    StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                    ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                    ItemCode = storeRequisitionItem.ItemCode,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,
                    CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),
                    CurrentAPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(storeRequisitionItem.ItemCode, sr.StoreCode),

                    DateCreated = storeRequisitionItem.DateCreated,
                    CreatedBy = storeRequisitionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };

                db.StoreRequisitionItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition review item", "Success", User.Identity.Name, $"updated store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion

            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequsitionItemPropertyReviews_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {
                    StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                    ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                    ItemCode = storeRequisitionItem.ItemCode,
                    PartNo = storeRequisitionItem.PartNo,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    GeneratePurchaseReq = storeRequisitionItem.GeneratePurchaseReq,
                    CurrentBalance = storeRequisitionItem.CurrentBalance,
                    CurrentAPrice = storeRequisitionItem.CurrentAPrice,
                };

                db.StoreRequisitionItems.Attach(entity);
                db.StoreRequisitionItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }

      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
