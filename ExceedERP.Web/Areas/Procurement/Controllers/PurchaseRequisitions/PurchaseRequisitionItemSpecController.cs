using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PurchaseRequisitions
{
   
    public class PurchaseRequisitionItemSpecController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser, InventoryConstants.PropertyUser)]
        public ActionResult Index()
        {
            return View();
        }
 
        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser, InventoryConstants.PropertyUser)]
        public ActionResult PurchaseRequisitionItemSpecifications_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<PurchaseRequisitionItemSpecification> purchaseRequisitionItemSpecifications = _db.PurchaseRequisitionItemSpecifications.Where(pri => pri.PurchaseRequisitionItemID == id);
            DataSourceResult result = purchaseRequisitionItemSpecifications.ToDataSourceResult(request, purchaseRequisitionItemSpecification => new {
                PurchaseRequisitionItemSpecificationId = purchaseRequisitionItemSpecification.PurchaseRequisitionItemSpecificationId,
                PurchaseRequisitionItemID = purchaseRequisitionItemSpecification.PurchaseRequisitionItemID,
                Specification = purchaseRequisitionItemSpecification.Specification,
            });

            return Json(result);
        }
 
        [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItemSpecifications_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItemSpecification purchaseRequisitionItemSpecification , int id )
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItemSpecification
                {
                    PurchaseRequisitionItemID = id,
                    Specification = purchaseRequisitionItemSpecification.Specification,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                _db.PurchaseRequisitionItemSpecifications.Add(entity);
                _db.SaveChanges();
                purchaseRequisitionItemSpecification.PurchaseRequisitionItemSpecificationId = entity.PurchaseRequisitionItemSpecificationId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition item spec", "Success", User.Identity.Name, $"created item spec {entity.Specification} under record {entity.PurchaseRequisitionItemID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionItemSpecification }.ToDataSourceResult(request, ModelState));
        }

 
        [AuthorizeRoles(InventoryConstants.PropertyAdmin,InventoryConstants.PropertyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItemSpecifications_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItemSpecification purchaseRequisitionItemSpecification)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItemSpecification
                {
                    PurchaseRequisitionItemID = purchaseRequisitionItemSpecification.PurchaseRequisitionItemID,
                    PurchaseRequisitionItemSpecificationId = purchaseRequisitionItemSpecification.PurchaseRequisitionItemSpecificationId,
                    Specification = purchaseRequisitionItemSpecification.Specification,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                };

                _db.PurchaseRequisitionItemSpecifications.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition item spec", "Success", User.Identity.Name, $"updated item spec {entity.Specification} under record {entity.PurchaseRequisitionItemID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionItemSpecification }.ToDataSourceResult(request, ModelState));
        }

  
        [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionItemSpecifications_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionItemSpecification purchaseRequisitionItemSpecification)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionItemSpecification
                {
                    PurchaseRequisitionItemSpecificationId = purchaseRequisitionItemSpecification.PurchaseRequisitionItemSpecificationId,
                    Specification = purchaseRequisitionItemSpecification.Specification,
                };

                _db.PurchaseRequisitionItemSpecifications.Attach(entity);
                _db.PurchaseRequisitionItemSpecifications.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted purchase requisition item specification " + purchaseRequisitionItemSpecification.PurchaseRequisitionItemID + " " + purchaseRequisitionItemSpecification.PurchaseRequisitionItem.ItemName);
                #endregion
            }

            return Json(new[] { purchaseRequisitionItemSpecification }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
