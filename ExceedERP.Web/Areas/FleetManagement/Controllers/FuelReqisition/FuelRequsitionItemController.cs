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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Requisitions
{
 [Authorize]

    public class FuelRequsitionItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        [AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult Index()
        {
            return View();
        }


     [AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult FuelStoreRequisitionItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StoreRequisitionItem> storerequisitionItems = db.StoreRequisitionItems.Where(p => p.StoreRequisitionID == id);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new
            {

                StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                ItemCode = storeRequisitionItem.ItemCode,
                PartNo = storeRequisitionItem.PartNo,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                IsApproved = storeRequisitionItem.IsApproved,
                ItemSpecification = storeRequisitionItem.ItemSpecification,
                Unit = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemUom(storeRequisitionItem.ItemCode),
                CurrentBalance = 0
            });

            return Json(result);
        }


     [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelStoreRequisitionItems_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem, int id)
        {
            
            var exist =
               db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == id)
                   .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                   .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + storeRequisitionItem.ItemCode + " already exists");
            }

            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {
                    StoreRequisitionID = id,
                    ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                    ItemCode = storeRequisitionItem.ItemCode,
                    PartNo = storeRequisitionItem.PartNo,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                    IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                    PurchaseQuantity = 0,//assuming all will be given
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,

                    ModifiedBy = storeRequisitionItem.ModifiedBy,
                    LastModified = storeRequisitionItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                };

                db.StoreRequisitionItems.Add(entity);
                db.SaveChanges();
                storeRequisitionItem.StoreRequisitionItemID = entity.StoreRequisitionItemID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "fuel requisition item", "Success", User.Identity.Name, $"created fuel requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelStoreRequisitionItems_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
        {

            var exist = db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID != storeRequisitionItem.StoreRequisitionItemID).Where(x => x.StoreRequisitionID == storeRequisitionItem.StoreRequisitionID)
                    .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                    .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + storeRequisitionItem.ItemCode + " already exists");
            }
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {

                    StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                    StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                    ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                    ItemCode = storeRequisitionItem.ItemCode,
                    PartNo = storeRequisitionItem.PartNo,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.ApprovedQuantity,//default issue is equal to approved
                    PurchaseQuantity = 0,//assuming all will be given
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,

                    DateCreated = storeRequisitionItem.DateCreated,
                    CreatedBy = storeRequisitionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                db.StoreRequisitionItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "fuel requisition item", "Success", User.Identity.Name, $"updated fuel requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


       [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelStoreRequisitionItems_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
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
                    ItemSpecification = storeRequisitionItem.ItemSpecification
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
