using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    public class PoPurchaseRequisitionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PoPurchaseRequisitions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PoPurchaseRequisition> poPurchaseRequisitions = _db.PoPurchaseRequisitions.Where(x=>x.PurchaseOrderID==id);
            DataSourceResult result = poPurchaseRequisitions.ToDataSourceResult(request, poPurchaseRequisition => new {
                PoPurchaseRequisitionId = poPurchaseRequisition.PoPurchaseRequisitionId,
                StoreRequisitionID = poPurchaseRequisition.StoreRequisitionID,
                PurchaseRequisitionAssignmentId = poPurchaseRequisition.PurchaseRequisitionAssignmentId,
                PurchaseOrderID = poPurchaseRequisition.PurchaseOrderID,
                PurchaseRequisitionID = poPurchaseRequisition.PurchaseRequisitionID,
                OrgBranchName = poPurchaseRequisition.OrgBranchName,
                DateCreated = poPurchaseRequisition.DateCreated,
                LastModified = poPurchaseRequisition.LastModified,
                CreatedBy = poPurchaseRequisition.CreatedBy,
                ModifiedBy = poPurchaseRequisition.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoPurchaseRequisitions_Create(int id,[DataSourceRequest]DataSourceRequest request, PoPurchaseRequisition poPurchaseRequisition)
        {

            var exist = _db.PoPurchaseRequisitions
               .Where(x => x.PurchaseRequisitionAssignmentId == poPurchaseRequisition.PurchaseRequisitionAssignmentId).FirstOrDefault();
            if (exist != null)
            {
                ModelState.AddModelError("exist", $" The assignement is already added in po {exist.PurchaseOrderID}  ");
            }

            var assignment =
                _db.PurchaseRequisitionAssignments.Find(poPurchaseRequisition.PurchaseRequisitionAssignmentId);


            if (assignment != null)
            {
                poPurchaseRequisition.PurchaseRequisitionID = assignment.PurchaseRequisitionID;

               var purchaseRequisition = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (purchaseRequisition != null)
                    poPurchaseRequisition.StoreRequisitionID = purchaseRequisition.StoreRequisitionID;
            }

            if (ModelState.IsValid)
            {
                var entity = new PoPurchaseRequisition
                {
                    PurchaseOrderID = id,
                    PurchaseRequisitionAssignmentId=poPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    StoreRequisitionID = poPurchaseRequisition.StoreRequisitionID,
                    PurchaseRequisitionID = poPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = poPurchaseRequisition.OrgBranchName,
                    DateCreated = poPurchaseRequisition.DateCreated,
                    LastModified = poPurchaseRequisition.LastModified,
                    CreatedBy = poPurchaseRequisition.CreatedBy,
                    ModifiedBy = poPurchaseRequisition.ModifiedBy
                };

                _db.PoPurchaseRequisitions.Add(entity);
                _db.SaveChanges();
                poPurchaseRequisition.PoPurchaseRequisitionId = entity.PoPurchaseRequisitionId;

                AddPrItems(poPurchaseRequisition.PurchaseRequisitionAssignmentId, id);

                #region update pr to parent po

                var po = _db.PurchaseOrders.Find(id);
                if (po != null)
                {
                    po.PurchaseRequisitionID = entity.PurchaseRequisitionID;
                    _db.PurchaseOrders.Attach(po);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order purchase requisitions", "Success", User.Identity.Name, $"created pr {entity.PurchaseRequisitionID} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { poPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoPurchaseRequisitions_Update([DataSourceRequest]DataSourceRequest request, PoPurchaseRequisition poPurchaseRequisition)
        {
            var exist = _db.PoPurchaseRequisitions
                .Where(x => x.PoPurchaseRequisitionId != poPurchaseRequisition.PoPurchaseRequisitionId)
              .Where(x => x.PurchaseRequisitionAssignmentId == poPurchaseRequisition.PurchaseRequisitionAssignmentId).FirstOrDefault();
            if (exist != null)
            {
                ModelState.AddModelError("exist", $" The assignement is already added in po {exist.PurchaseOrderID}  ");
            }

            var assignment =
                _db.PurchaseRequisitionAssignments.Find(poPurchaseRequisition.PurchaseRequisitionAssignmentId);


            if (assignment != null)
            {
                poPurchaseRequisition.PurchaseRequisitionID = assignment.PurchaseRequisitionID;

                var purchaseRequisition = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (purchaseRequisition != null)
                    poPurchaseRequisition.StoreRequisitionID = purchaseRequisition.StoreRequisitionID;
            }

            if (ModelState.IsValid)
            {
                var entity = new PoPurchaseRequisition
                {
                    PurchaseOrderID = poPurchaseRequisition.PurchaseOrderID,
                    PoPurchaseRequisitionId = poPurchaseRequisition.PoPurchaseRequisitionId,
                    StoreRequisitionID = poPurchaseRequisition.StoreRequisitionID,
                    PurchaseRequisitionAssignmentId = poPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = poPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = poPurchaseRequisition.OrgBranchName,
                    DateCreated = poPurchaseRequisition.DateCreated,
                    LastModified = poPurchaseRequisition.LastModified,
                    CreatedBy = poPurchaseRequisition.CreatedBy,
                    ModifiedBy = poPurchaseRequisition.ModifiedBy
                };

                _db.PoPurchaseRequisitions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region update pr to parent po

                var po = _db.PurchaseOrders.Find(poPurchaseRequisition.PurchaseOrderID);
                if (po != null)
                {
                    po.PurchaseRequisitionID = entity.PurchaseRequisitionID;
                    _db.PurchaseOrders.Attach(po);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order purchase requisitions", "Success", User.Identity.Name, $"updated pr {entity.PurchaseRequisitionID} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { poPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoPurchaseRequisitions_Destroy([DataSourceRequest]DataSourceRequest request, PoPurchaseRequisition poPurchaseRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoPurchaseRequisition
                {
                    StoreRequisitionID = poPurchaseRequisition.StoreRequisitionID,
                    PurchaseOrderID = poPurchaseRequisition.PurchaseOrderID,
                    PoPurchaseRequisitionId = poPurchaseRequisition.PoPurchaseRequisitionId,
                    PurchaseRequisitionAssignmentId = poPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = poPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = poPurchaseRequisition.OrgBranchName,
                    DateCreated = poPurchaseRequisition.DateCreated,
                    LastModified = poPurchaseRequisition.LastModified,
                    CreatedBy = poPurchaseRequisition.CreatedBy,
                    ModifiedBy = poPurchaseRequisition.ModifiedBy
                };

                _db.PoPurchaseRequisitions.Attach(entity);
                _db.PoPurchaseRequisitions.Remove(entity);
                _db.SaveChanges();
                RemovePrItems(poPurchaseRequisition.PurchaseRequisitionAssignmentId, poPurchaseRequisition.PurchaseOrderID);
            }

            return Json(new[] { poPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        private void AddPrItems(int assignmentid, int poid)
        {
            var record = _db.PurchaseRequisitionAssignments.Find(assignmentid);
            if (record != null)
            {
                foreach (var item in record.PurchaseRequisitionReassignmentItems)
                {
                    if (item.ItemType == PurchaseItemType.FixedAsset)
                    {
                        #region  fixed asset

                        var exist = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == poid)
                            .Where(x => x.ItmCode == item.ItmCode).Where(x=>x.Name==item.Name).FirstOrDefault(x => x.ModelNo == item.ModelNo);
                        if (exist == null)
                        {

                            PurchaseOrderItem purchaseOrderItem = new PurchaseOrderItem
                            {
                                PurchaseOrderID = poid,
                                CatCode = item.CatCode,
                                CategoryName = item.CategoryName,
                                ItmCode = item.ItmCode,
                                ItemName = item.ItemName,
                                PartNo = item.PartNo,
                                Quantity = item.Quantity,
                                Name = item.Name,
                                ModelNo = item.ModelNo,
                                ItemSpecification = item.ItemSpecification,
                                UoM = item.UoM,
                                ItemType = item.ItemType,
                            };

                            purchaseOrderItem.Quantity = item.Quantity;
                            _db.PurchaseOrderItems.Add(purchaseOrderItem);
                            _db.SaveChanges();
                        }
                        else
                        {
                            exist.Quantity = exist.Quantity + item.Quantity;
                            _db.Entry(exist).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        #endregion
                    }
                    else
                    {
                        #region non fixed asset

                        var exist = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == poid).FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (exist == null)
                        {

                            PurchaseOrderItem purchaseOrderItem = new PurchaseOrderItem
                            {
                                PurchaseOrderID = poid,
                                CatCode = item.CatCode,
                                CategoryName = item.CategoryName,
                                ItmCode = item.ItmCode,
                                ItemName = item.ItemName,
                                PartNo = item.PartNo,
                                Quantity = item.Quantity,
                                ItemSpecification = item.ItemSpecification,
                                UoM = item.UoM,
                                ItemType = item.ItemType,
                            };

                            purchaseOrderItem.Quantity = item.Quantity;
                            _db.PurchaseOrderItems.Add(purchaseOrderItem);
                            _db.SaveChanges();
                        }
                        else
                        {
                            exist.Quantity = exist.Quantity + item.Quantity;
                            _db.Entry(exist).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        #endregion
                    }

                }
            }
        }
        private void RemovePrItems(int assignmentid, int proformaid)
        {
            var record = _db.PurchaseRequisitionAssignments.Find(assignmentid);
            if (record != null)
            {
                foreach (var item in record.PurchaseRequisitionReassignmentItems)
                {
                    if (item.ItemType == PurchaseItemType.FixedAsset)
                    {
                        #region  fixed asset

                        var exist = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == proformaid)
                            .Where(x => x.ItmCode == item.ItmCode).FirstOrDefault(x => x.Name == item.Name);
                        if (exist == null)
                        {

                        }
                        else
                        {
                            decimal qty = exist.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.PurchaseOrderItems.Remove(exist);
                                _db.SaveChanges();
                            }
                            else
                            {
                                exist.Quantity = qty;
                                _db.Entry(exist).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                        }

                        #endregion
                    }
                    else
                    {
                        #region non fixed asset

                        var exist = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == proformaid).FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (exist == null)
                        {

                        }
                        else
                        {
                            decimal qty = exist.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.PurchaseOrderItems.Remove(exist);
                                _db.SaveChanges();
                            }
                            else
                            {
                                exist.Quantity = qty;
                                _db.Entry(exist).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                        }

                        #endregion
                    }

                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
