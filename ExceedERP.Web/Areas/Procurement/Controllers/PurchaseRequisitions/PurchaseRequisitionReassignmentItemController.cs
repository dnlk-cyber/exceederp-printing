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
    public class PurchaseRequisitionReassignmentItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementUser, ProcurementConstants.ProcurementApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUser, ProcurementConstants.ProcurementApprover)]
        public ActionResult PurchaseRequisitionReassignmentItems_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<PurchaseRequisitionReassignmentItem> purchaseRequisitionReassignmentItems = _db.PurchaseRequisitionReassignmentItems.Where(pri => pri.PurchaseRequisitionAssignmentId == id);
            DataSourceResult result = purchaseRequisitionReassignmentItems.ToDataSourceResult(request, purchaseRequisitionReassignmentItem => new {
                PurchaseRequisitionReassignmentItemId = purchaseRequisitionReassignmentItem.PurchaseRequisitionReassignmentItemId,
                PurchaseRequisitionAssignmentId = purchaseRequisitionReassignmentItem.PurchaseRequisitionAssignmentId,
                CatCode = purchaseRequisitionReassignmentItem.CatCode,
                CategoryName = purchaseRequisitionReassignmentItem.CategoryName,
                ItmCode = purchaseRequisitionReassignmentItem.ItmCode,
                ItemName = purchaseRequisitionReassignmentItem.ItemName,
                PartNo = purchaseRequisitionReassignmentItem.PartNo,
                ModelNo = purchaseRequisitionReassignmentItem.ModelNo,
                SerialNo = purchaseRequisitionReassignmentItem.SerialNo,
                ChasisNo = purchaseRequisitionReassignmentItem.ChasisNo,
                UoM = purchaseRequisitionReassignmentItem.UoM,
                ItemSpecification = purchaseRequisitionReassignmentItem.ItemSpecification,
                Name = purchaseRequisitionReassignmentItem.Name,
                Quantity = purchaseRequisitionReassignmentItem.Quantity,
                ItemType = purchaseRequisitionReassignmentItem.ItemType
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentItems_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentItem purchaseRequisitionReassignmentItem , int id )
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionReassignmentItem
                {
                    PurchaseRequisitionAssignmentId = id,
                    CatCode = purchaseRequisitionReassignmentItem.CatCode,
                    CategoryName = purchaseRequisitionReassignmentItem.CategoryName,
                    ItmCode = purchaseRequisitionReassignmentItem.ItmCode,
                    ItemName = purchaseRequisitionReassignmentItem.ItemName,
                    PartNo = purchaseRequisitionReassignmentItem.PartNo,
                    ModelNo = purchaseRequisitionReassignmentItem.ModelNo,
                    SerialNo = purchaseRequisitionReassignmentItem.SerialNo,
                    ChasisNo = purchaseRequisitionReassignmentItem.ChasisNo,
                    UoM = purchaseRequisitionReassignmentItem.UoM,
                    ItemSpecification = purchaseRequisitionReassignmentItem.ItemSpecification,
                    Name = purchaseRequisitionReassignmentItem.Name,
                    Quantity = purchaseRequisitionReassignmentItem.Quantity,
                    ItemType = purchaseRequisitionReassignmentItem.ItemType
                };

                _db.PurchaseRequisitionReassignmentItems.Add(entity);
                _db.SaveChanges();
                purchaseRequisitionReassignmentItem.PurchaseRequisitionReassignmentItemId = entity.PurchaseRequisitionReassignmentItemId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition reassignment item", "Success", User.Identity.Name, $"created reassignemnt item {entity.ItemName} under record {entity.PurchaseRequisitionAssignmentId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionReassignmentItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentItems_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentItem purchaseRequisitionReassignmentItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionReassignmentItem
                {
                    PurchaseRequisitionAssignmentId = purchaseRequisitionReassignmentItem.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionReassignmentItemId = purchaseRequisitionReassignmentItem.PurchaseRequisitionReassignmentItemId,
                    CatCode = purchaseRequisitionReassignmentItem.CatCode,
                    CategoryName = purchaseRequisitionReassignmentItem.CategoryName,
                    ItmCode = purchaseRequisitionReassignmentItem.ItmCode,
                    ItemName = purchaseRequisitionReassignmentItem.ItemName,
                    PartNo = purchaseRequisitionReassignmentItem.PartNo,
                    ModelNo = purchaseRequisitionReassignmentItem.ModelNo,
                    SerialNo = purchaseRequisitionReassignmentItem.SerialNo,
                    ChasisNo = purchaseRequisitionReassignmentItem.ChasisNo,
                    UoM = purchaseRequisitionReassignmentItem.UoM,
                    ItemSpecification = purchaseRequisitionReassignmentItem.ItemSpecification,
                    Name = purchaseRequisitionReassignmentItem.Name,
                    Quantity = purchaseRequisitionReassignmentItem.Quantity,
                    ItemType = purchaseRequisitionReassignmentItem.ItemType,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                _db.PurchaseRequisitionReassignmentItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition reassignment item", "Success", User.Identity.Name, $"updated reassignemnt item {entity.ItemName} under record {entity.PurchaseRequisitionAssignmentId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionReassignmentItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentItems_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentItem purchaseRequisitionReassignmentItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionReassignmentItem
                {
                    PurchaseRequisitionReassignmentItemId = purchaseRequisitionReassignmentItem.PurchaseRequisitionReassignmentItemId,
                    CatCode = purchaseRequisitionReassignmentItem.CatCode,
                    CategoryName = purchaseRequisitionReassignmentItem.CategoryName,
                    ItmCode = purchaseRequisitionReassignmentItem.ItmCode,
                    ItemName = purchaseRequisitionReassignmentItem.ItemName,
                    PartNo = purchaseRequisitionReassignmentItem.PartNo,
                    ModelNo = purchaseRequisitionReassignmentItem.ModelNo,
                    Quantity = purchaseRequisitionReassignmentItem.Quantity,
                    ItemType = purchaseRequisitionReassignmentItem.ItemType
                };

                _db.PurchaseRequisitionReassignmentItems.Attach(entity);
                _db.PurchaseRequisitionReassignmentItems.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { purchaseRequisitionReassignmentItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
