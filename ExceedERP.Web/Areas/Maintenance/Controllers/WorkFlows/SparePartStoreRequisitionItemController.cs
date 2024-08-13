using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Maintenance.SpareStoreRequisition;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser, MaintenanceRoles.MaintenanceWorkShopUser, MaintenanceRoles.ServiceUser)]

    public class SparePartStoreRequisitionItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        private bool IsSRReviewed(int id)
        {
            var sr = db.StoreRequisitions.Find(id);
            if (sr != null)
            {
                if (sr.IsSecondOnlineApproved)
                    return true;
            }
            return false;
        }
        private string GetStore(int id)
        {
            return db.StoreRequisitions.Find(id).StoreCode;
        }
        //[AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult SparePartStoreRequisitionItems_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<StoreRequisitionItem> storerequisitionItems = db.StoreRequisitionItems.Where(p => p.StoreRequisitionID == id);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new 
            {
                
                AgreementPrice = storeRequisitionItem.AgreementPrice,
                StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                ItemCode = storeRequisitionItem.ItemCode,
                PartNo = storeRequisitionItem.PartNo,
                ChasisNo = storeRequisitionItem.ChasisNo,
                SerialNo = storeRequisitionItem.SerialNo,
                ModelNo = storeRequisitionItem.ModelNo,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                IsApproved = storeRequisitionItem.IsApproved,
                ItemSpecification = storeRequisitionItem.ItemSpecification,
                Unit= ExceedERP.Helpers.Inventory.InventoryHelper.GetItemUom(storeRequisitionItem.ItemCode),
               // CurrentBalance = 0,
                CurrentAPrice = storeRequisitionItem.CurrentAPrice,
                CurrentBalance = IsSRReviewed(storeRequisitionItem.StoreRequisitionID) ? storeRequisitionItem.CurrentBalance : ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, GetStore(storeRequisitionItem.StoreRequisitionID)),
            });

            return Json(result);
        }


       // [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartStoreRequisitionItems_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem , int id)
        {
            var sr = db.StoreRequisitions.Find(id);
            storeRequisitionItem.IssuedQuantity = 0;
            storeRequisitionItem.PurchaseQuantity = 0;
            if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
            {
                ModelState.AddModelError("above","Approved quantity cannot be more than requested.");
            }
            var exist =
                db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == id)
                    .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                    .Where(x => x.ItemSpecification == storeRequisitionItem.ItemSpecification)//added
                    .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " +storeRequisitionItem.ItemCode+" already exists");
            }
            ModelState.Remove("AgreementPrice");
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
                    PurchaseQuantity = 0,//assuming all will be given
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    CurrentAPrice = storeRequisitionItem.CurrentAPrice,

                };

                db.StoreRequisitionItems.Add(entity);
                db.SaveChanges();
                storeRequisitionItem.StoreRequisitionItemID = entity.StoreRequisitionItemID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition item", "Success", User.Identity.Name, $"created store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


       //[AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartStoreRequisitionItems_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
        {
            var sr = db.StoreRequisitions.Find(storeRequisitionItem.StoreRequisitionID);
            storeRequisitionItem.IssuedQuantity = 0;
            storeRequisitionItem.PurchaseQuantity = 0;
            if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
            {
                ModelState.AddModelError("above", "Approved quantity cannot be more than requested.");
            }

            var exist = db.StoreRequisitionItems
                     .Where(x => x.StoreRequisitionItemID != storeRequisitionItem.StoreRequisitionItemID)
                     .Where(x => x.StoreRequisitionID == storeRequisitionItem.StoreRequisitionID)
                     .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                     .Where(x => x.ItemSpecification == storeRequisitionItem.ItemSpecification)//added
                     .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + storeRequisitionItem.ItemCode + " already exists");
            }
            ModelState.Remove("AgreementPrice");
            
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
                    IssuedQuantity = storeRequisitionItem.ApprovedQuantity,//default issue is equal to approved
                    PurchaseQuantity = 0,//assuming all will be given
                    ItemSpecification = storeRequisitionItem.ItemSpecification,

                    DateCreated = storeRequisitionItem.DateCreated,
                    CreatedBy = storeRequisitionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    CurrentAPrice = storeRequisitionItem.CurrentAPrice,
                    CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),
                };

                db.StoreRequisitionItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition item", "Success", User.Identity.Name, $"updated store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


         //[AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartStoreRequisitionItems_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {
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
                    CurrentAPrice = storeRequisitionItem.CurrentAPrice
                };

                db.StoreRequisitionItems.Attach(entity);
                db.StoreRequisitionItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store requisition item " + storeRequisitionItem.StoreRequisitionID + " " + storeRequisitionItem.InventoryItem.Name);
                #endregion
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
