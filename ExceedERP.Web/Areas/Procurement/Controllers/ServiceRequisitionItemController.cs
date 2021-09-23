using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    public class ServiceRequisitionItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementUserDepartmentApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementUserDepartmentApprover)]
        public ActionResult ServiceRequisitionItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseRequisitionItem> purchaserequisitionItems = db.PurchaseRequisitionItems.Where(pri => pri.PurchaseRequisitionID == id);
            DataSourceResult result = purchaserequisitionItems.ToDataSourceResult(request, purchaseRequisitionItem => new
            {
                PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                PurchaseRequisitionID = purchaseRequisitionItem.PurchaseRequisitionID,
                CatCode = purchaseRequisitionItem.CatCode,
                CategoryName = purchaseRequisitionItem.CategoryName,
                ItmCode = purchaseRequisitionItem.ItmCode,
                ItemName = purchaseRequisitionItem.ItemName,
                PartNo = purchaseRequisitionItem.PartNo,
                RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                Quantity = purchaseRequisitionItem.Quantity,
                IsApproved = purchaseRequisitionItem.IsApproved,
                ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                UoM = purchaseRequisitionItem.UoM,
                ItemType = purchaseRequisitionItem.ItemType,
            });

            return Json(result);
        }

       [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceRequisitionItems_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem, int id)
        {
            var serviceCategory = db.ServiceCategories.Find(purchaseRequisitionItem.CatCode);
           // purchaseRequisitionItem.Quantity = purchaseRequisitionItem.RequestedQuantity;
            #region validation


            var exists = db.PurchaseRequisitionItems
                .Where(x => x.PurchaseRequisitionID ==id)
                .Where(x => x.ItmCode == purchaseRequisitionItem.ItmCode).ToList();

            if (exists.Any())
                ModelState.AddModelError("exist", "Item already exists in the system.");

            #endregion
            if (ModelState.IsValid)
            {
                if (serviceCategory != null)
                {
                    var entity = new PurchaseRequisitionItem
                    {
                        PurchaseRequisitionID = id,
                        CatCode = purchaseRequisitionItem.CatCode,
                        CategoryName = serviceCategory.Name,
                        ItmCode = purchaseRequisitionItem.ItmCode,
                        ItemName = purchaseRequisitionItem.ItmCode,
                        PartNo = purchaseRequisitionItem.PartNo,
                        RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                        Quantity = purchaseRequisitionItem.Quantity,
                        IsApproved = purchaseRequisitionItem.IsApproved,
                        ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                        //UoM =  serviceCategory.UoMName,
                        ItemType =PurchaseItemType.Service,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.PurchaseRequisitionItems.Add(entity);
                    db.SaveChanges();
                    purchaseRequisitionItem.PurchaseRequisitionItemID = entity.PurchaseRequisitionItemID;
                }
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceRequisitionItems_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem)
        {
            var serviceCategory = db.ServiceCategories.Find(purchaseRequisitionItem.CatCode);
            #region validation
            if (purchaseRequisitionItem.Quantity > purchaseRequisitionItem.RequestedQuantity)
                ModelState.AddModelError("qty","Final quantity can not be more than requested quanity");

            var exists = db.PurchaseRequisitionItems
                .Where(x => x.PurchaseRequisitionID == purchaseRequisitionItem.PurchaseRequisitionID)
                .Where(x => x.PurchaseRequisitionItemID != purchaseRequisitionItem.PurchaseRequisitionItemID)
                .Where(x => x.ItmCode == purchaseRequisitionItem.ItmCode).ToList();

            if (exists.Any())
                ModelState.AddModelError("exist", "Item already exists in the system.");

            #endregion
            if (ModelState.IsValid)
            {
                if (serviceCategory != null)
                {
                    var entity = new PurchaseRequisitionItem
                    {
                        PurchaseRequisitionID = purchaseRequisitionItem.PurchaseRequisitionID,
                        PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                        CatCode = purchaseRequisitionItem.CatCode,
                        CategoryName = serviceCategory.Name,
                        ItmCode = purchaseRequisitionItem.ItmCode,
                        ItemName = purchaseRequisitionItem.ItmCode,
                        PartNo = purchaseRequisitionItem.PartNo,
                        RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                        Quantity = purchaseRequisitionItem.Quantity,
                        IsApproved = purchaseRequisitionItem.IsApproved,
                        ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                        //UoM = item.UoMName,
                        ItemType = purchaseRequisitionItem.ItemType,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Today,
                    };

                    db.PurchaseRequisitionItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceRequisitionItems_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItem
                {
                    PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                    CatCode = purchaseRequisitionItem.CatCode,
                    CategoryName = purchaseRequisitionItem.CategoryName,
                    ItmCode = purchaseRequisitionItem.ItmCode,
                    ItemName = purchaseRequisitionItem.ItemName,
                    PartNo = purchaseRequisitionItem.PartNo,
                    RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                    Quantity = purchaseRequisitionItem.Quantity,
                    IsApproved = purchaseRequisitionItem.IsApproved,
                    ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                    UoM = purchaseRequisitionItem.UoM,
                    ItemType = purchaseRequisitionItem.ItemType,
                };

                db.PurchaseRequisitionItems.Attach(entity);
                db.PurchaseRequisitionItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted purchase requisition item " + purchaseRequisitionItem.PurchaseRequisitionID + " " + purchaseRequisitionItem.ItemName);
                #endregion
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
