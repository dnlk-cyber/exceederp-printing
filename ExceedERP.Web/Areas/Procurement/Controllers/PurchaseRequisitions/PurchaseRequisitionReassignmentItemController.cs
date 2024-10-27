using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
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

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
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


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentItems_Create([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentItem reassignmentItem , int id )
        {
            #region data validation

            var assignment = _db.PurchaseRequisitionAssignments.Find(id);
            if(assignment!=null)
            {
                decimal pramount = 0;
                decimal assignedamount = 0;
                var pr=_db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);

                List<int> assignmentIds = _db.PurchaseRequisitionAssignments
                    .Where(x=>x.PurchaseRequisitionID==pr.PurchaseRequisitionID)
                    .Select(x=>x.PurchaseRequisitionAssignmentId).ToList();

                var assignmentItems=_db.PurchaseRequisitionReassignmentItems.Where(x=> assignmentIds.Contains(x.PurchaseRequisitionAssignmentId)).ToList();
                var prItem=_db.PurchaseRequisitionItems
                    .Where(x=>x.PurchaseRequisitionID== assignment.PurchaseRequisitionID)
                    .Where(x=>x.CatCode== reassignmentItem.CatCode)
                    .Where(x=>x.ItmCode== reassignmentItem.ItmCode)
                    .Where(x=>x.ItemName== reassignmentItem.ItemName)
                    .Where(x=>x.Name== reassignmentItem.Name)
                    .Where(x=>x.ItemSpecification == reassignmentItem.ItemSpecification).FirstOrDefault();

                if(prItem!=null)
                    pramount=prItem.Quantity;

                var prevAssignmentItem = assignmentItems
                  .Where(x => x.CatCode == reassignmentItem.CatCode)
                  .Where(x => x.ItmCode == reassignmentItem.ItmCode)
                  .Where(x => x.ItemName == reassignmentItem.ItemName)
                  .Where(x => x.Name == reassignmentItem.Name)
                  .Where(x => x.ItemSpecification == reassignmentItem.ItemSpecification).ToList();

                if (prevAssignmentItem.Any())
                    assignedamount = prevAssignmentItem.Sum(x=>x.Quantity);

                assignedamount=assignedamount+ reassignmentItem.Quantity;

                if(assignedamount>pramount)
                    ModelState.AddModelError("above", "Total assigned for this item will be above the PR Quantity.");
            }
            else
            {
                ModelState.AddModelError("notfound","Assignment not found.");
            }
            #endregion 

            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionReassignmentItem
                {
                    PurchaseRequisitionAssignmentId = id,
                    CatCode = reassignmentItem.CatCode,
                    CategoryName = reassignmentItem.CategoryName,
                    ItmCode = reassignmentItem.ItmCode,
                    ItemName = reassignmentItem.ItemName,
                    PartNo = reassignmentItem.PartNo,
                    ModelNo = reassignmentItem.ModelNo,
                    SerialNo = reassignmentItem.SerialNo,
                    ChasisNo = reassignmentItem.ChasisNo,
                    UoM = reassignmentItem.UoM,
                    ItemSpecification = reassignmentItem.ItemSpecification,
                    Name = reassignmentItem.Name,
                    Quantity = reassignmentItem.Quantity,
                    ItemType = reassignmentItem.ItemType
                };

                _db.PurchaseRequisitionReassignmentItems.Add(entity);
                _db.SaveChanges();
                reassignmentItem.PurchaseRequisitionReassignmentItemId = entity.PurchaseRequisitionReassignmentItemId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition reassignment item", "Success", User.Identity.Name, $"created reassignemnt item {entity.ItemName} under record {entity.PurchaseRequisitionAssignmentId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { reassignmentItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentItems_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentItem reassignmentItem)
        {
            #region data validation

            var assignment = _db.PurchaseRequisitionAssignments.Find(reassignmentItem.PurchaseRequisitionAssignmentId);
            if (assignment != null)
            {
                decimal pramount = 0;
                decimal assignedamount = 0;
                var pr = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);

                List<int> assignmentIds = _db.PurchaseRequisitionAssignments
                    .Where(x => x.PurchaseRequisitionID == pr.PurchaseRequisitionID)
                    .Select(x => x.PurchaseRequisitionAssignmentId).ToList();

                var assignmentItems = _db.PurchaseRequisitionReassignmentItems.Where(x => assignmentIds.Contains(x.PurchaseRequisitionAssignmentId)).ToList();
                var prItem = _db.PurchaseRequisitionItems
                    .Where(x => x.PurchaseRequisitionID == assignment.PurchaseRequisitionID)
                    .Where(x => x.CatCode == reassignmentItem.CatCode)
                    .Where(x => x.ItmCode == reassignmentItem.ItmCode)
                    .Where(x => x.ItemName == reassignmentItem.ItemName)
                .Where(x => x.Name == reassignmentItem.Name)
                    .Where(x => x.ItemSpecification == reassignmentItem.ItemSpecification).FirstOrDefault();

                if (prItem != null)
                    pramount = prItem.Quantity;

                var prevAssignmentItem = assignmentItems
                  .Where(x => x.CatCode == reassignmentItem.CatCode)
                  .Where(x => x.ItmCode == reassignmentItem.ItmCode)
                  .Where(x => x.ItemName == reassignmentItem.ItemName)
                .Where(x => x.Name == reassignmentItem.Name)
                  .Where(x => x.ItemSpecification == reassignmentItem.ItemSpecification).ToList();

                if (prevAssignmentItem.Any())
                    assignedamount = prevAssignmentItem.Sum(x => x.Quantity);

                assignedamount = assignedamount + reassignmentItem.Quantity;

                if (assignedamount > pramount)
                    ModelState.AddModelError("above", "Total assigned for this item will be above the PR Quantity.");
            }
            else
            {
                ModelState.AddModelError("notfound", "Assignment not found.");
            }
            #endregion 
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionReassignmentItem
                {
                    PurchaseRequisitionAssignmentId = reassignmentItem.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionReassignmentItemId = reassignmentItem.PurchaseRequisitionReassignmentItemId,
                    CatCode = reassignmentItem.CatCode,
                    CategoryName = reassignmentItem.CategoryName,
                    ItmCode = reassignmentItem.ItmCode,
                    ItemName = reassignmentItem.ItemName,
                    PartNo = reassignmentItem.PartNo,
                    ModelNo = reassignmentItem.ModelNo,
                    SerialNo = reassignmentItem.SerialNo,
                    ChasisNo = reassignmentItem.ChasisNo,
                    UoM = reassignmentItem.UoM,
                    ItemSpecification = reassignmentItem.ItemSpecification,
                    Name = reassignmentItem.Name,
                    Quantity = reassignmentItem.Quantity,
                    ItemType = reassignmentItem.ItemType,

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

            return Json(new[] { reassignmentItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
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
