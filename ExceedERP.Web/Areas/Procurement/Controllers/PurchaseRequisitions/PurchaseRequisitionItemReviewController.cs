using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PurchaseRequisitions
{
    public class PurchaseRequisitionItemReviewController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult PurchaseRequisitionItemReviews_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<PurchaseRequisitionItem> purchaserequisitionItems = db.PurchaseRequisitionItems.Where(pri => pri.PurchaseRequisitionID == id);
            DataSourceResult result = purchaserequisitionItems.ToDataSourceResult(request, purchaseRequisitionItem => new {
                PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                PurchaseRequisitionID = purchaseRequisitionItem.PurchaseRequisitionID,
                CatCode = purchaseRequisitionItem.CatCode,// + "-" + purchaseRequisitionItem.CategoryName,
                CategoryName = purchaseRequisitionItem.CategoryName,
                ItmCode = purchaseRequisitionItem.ItmCode,// + "-" + purchaseRequisitionItem.ItemName,
                ItemName = purchaseRequisitionItem.ItemName,
                PartNo = purchaseRequisitionItem.PartNo,
                ModelNo = purchaseRequisitionItem.ModelNo,
                SerialNo = purchaseRequisitionItem.SerialNo,
                ChasisNo = purchaseRequisitionItem.ChasisNo,
                RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                Quantity = purchaseRequisitionItem.Quantity,
                IsApproved = purchaseRequisitionItem.IsApproved,
                ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                Name = purchaseRequisitionItem.Name,
                UoM = purchaseRequisitionItem.UoM,
                ItemType = purchaseRequisitionItem.ItemType,
                BudgetCode = purchaseRequisitionItem.BudgetCode,
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItemReviews_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem , int id )
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItem
                {
                    PurchaseRequisitionID = id,
                    CatCode = purchaseRequisitionItem.CatCode,
                    CategoryName = purchaseRequisitionItem.CategoryName,
                    ItmCode = purchaseRequisitionItem.ItmCode,
                    ItemName = purchaseRequisitionItem.ItemName,
                    PartNo = purchaseRequisitionItem.PartNo,
                    ModelNo = purchaseRequisitionItem.ModelNo,
                    SerialNo = purchaseRequisitionItem.SerialNo,
                    ChasisNo = purchaseRequisitionItem.ChasisNo,
                    RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                    Quantity = purchaseRequisitionItem.Quantity,
                    IsApproved = purchaseRequisitionItem.IsApproved,
                    ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                    Name = purchaseRequisitionItem.Name,
                    UoM = purchaseRequisitionItem.UoM,
                    ItemType = purchaseRequisitionItem.ItemType,
                    BudgetCode = purchaseRequisitionItem.BudgetCode,
                    CreatedBy =User.Identity.Name,
                    DateCreated=DateTime.Now
                };

                db.PurchaseRequisitionItems.Add(entity);
                db.SaveChanges();
                purchaseRequisitionItem.PurchaseRequisitionItemID = entity.PurchaseRequisitionItemID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition item review", "Success", User.Identity.Name, $"created item {entity.ItemName} under record {entity.PurchaseRequisitionID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItemReviews_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItem
                {
                    PurchaseRequisitionID = purchaseRequisitionItem.PurchaseRequisitionID,
                    PurchaseRequisitionItemID = purchaseRequisitionItem.PurchaseRequisitionItemID,
                    CatCode = purchaseRequisitionItem.CatCode,
                    CategoryName = purchaseRequisitionItem.CategoryName,
                    ItmCode = purchaseRequisitionItem.ItmCode,
                    ItemName = purchaseRequisitionItem.ItemName,
                    PartNo = purchaseRequisitionItem.PartNo,
                    ModelNo = purchaseRequisitionItem.ModelNo,
                    SerialNo = purchaseRequisitionItem.SerialNo,
                    ChasisNo = purchaseRequisitionItem.ChasisNo,
                    RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                    Quantity = purchaseRequisitionItem.Quantity,
                    IsApproved = purchaseRequisitionItem.IsApproved,
                    ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                    Name = purchaseRequisitionItem.Name,
                    UoM = purchaseRequisitionItem.UoM,
                    ItemType = purchaseRequisitionItem.ItemType,
                    BudgetCode = purchaseRequisitionItem.BudgetCode,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                db.PurchaseRequisitionItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition item review", "Success", User.Identity.Name, $"updated item {entity.ItemName} under record {entity.PurchaseRequisitionID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItemReviews_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItem purchaseRequisitionItem)
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
                    ModelNo = purchaseRequisitionItem.ModelNo,
                    SerialNo = purchaseRequisitionItem.SerialNo,
                    ChasisNo = purchaseRequisitionItem.ChasisNo,
                    RequestedQuantity = purchaseRequisitionItem.RequestedQuantity,
                    Quantity = purchaseRequisitionItem.Quantity,
                    IsApproved = purchaseRequisitionItem.IsApproved,
                    ItemSpecification = purchaseRequisitionItem.ItemSpecification,
                    Name = purchaseRequisitionItem.Name,
                    UoM = purchaseRequisitionItem.UoM,
                    ItemType = purchaseRequisitionItem.ItemType,
                    BudgetCode = purchaseRequisitionItem.BudgetCode,
                };

                db.PurchaseRequisitionItems.Attach(entity);
                db.PurchaseRequisitionItems.Remove(entity);
                db.SaveChanges();
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
