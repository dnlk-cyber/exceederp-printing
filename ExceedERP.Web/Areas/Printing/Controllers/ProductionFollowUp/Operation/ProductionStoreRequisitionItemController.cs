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

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{

    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class ProductionStoreRequisitionItemController : Controller

    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {  
            return View();
        }


        //[AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult ProductionStoreRequisitionItems_Read([DataSourceRequest]DataSourceRequest request , string jobNO )
        {
            IQueryable<IssueItem> storerequisitionItems = db.IssueItems.Where(p => p.ProductionJobNo == jobNO);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new {

                StoreRequisitionItemID = storeRequisitionItem.IssueItemID,
                StoreRequisitionID = storeRequisitionItem.IssueID,
                ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                ItemCode = storeRequisitionItem.ItemCode,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                Unit= ExceedERP.Helpers.Inventory.InventoryHelper.GetItemUom(storeRequisitionItem.ItemCode),
                CurrentBalance = 0
            });

            return Json(result);
        }


       // [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionStoreRequisitionItems_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem , int id)
        {
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

                   
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
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
        public ActionResult ProductionStoreRequisitionItems_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
        {
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
        public ActionResult ProductionStoreRequisitionItems_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
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
