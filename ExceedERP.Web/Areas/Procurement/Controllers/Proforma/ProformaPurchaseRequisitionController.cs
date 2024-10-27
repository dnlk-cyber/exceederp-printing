using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Proforma
{
    public class ProformaPurchaseRequisitionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult ProformaPurchaseRequisitions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProformaPurchaseRequisition> proformaPurchaserequisitions = _db.ProformaPurchaseRequisitions.Where(x=>x.ProformaPurchaseID==id);
            DataSourceResult result = proformaPurchaserequisitions.ToDataSourceResult(request, proformaPurchaseRequisition => new {
                ProformaPurchaseRequisitionId = proformaPurchaseRequisition.ProformaPurchaseRequisitionId,
                StoreRequisitionID = proformaPurchaseRequisition.StoreRequisitionID,
                PurchaseRequisitionAssignmentId = proformaPurchaseRequisition.PurchaseRequisitionAssignmentId,
                ProformaPurchaseID = proformaPurchaseRequisition.ProformaPurchaseID,
                PurchaseRequisitionID = proformaPurchaseRequisition.PurchaseRequisitionID,
                OrgBranchName = proformaPurchaseRequisition.OrgBranchName,
                DateCreated = proformaPurchaseRequisition.DateCreated,
                LastModified = proformaPurchaseRequisition.LastModified,
                CreatedBy = proformaPurchaseRequisition.CreatedBy,
                ModifiedBy = proformaPurchaseRequisition.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaPurchaseRequisitions_Create(int id,[DataSourceRequest]DataSourceRequest request, ProformaPurchaseRequisition proformaPurchaseRequisition)
        {


            var assignment =
                _db.PurchaseRequisitionAssignments.Find(proformaPurchaseRequisition.PurchaseRequisitionAssignmentId);


            if (assignment != null)
            {
                proformaPurchaseRequisition.PurchaseRequisitionID = assignment.PurchaseRequisitionID;

               var purchaseRequisition = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (purchaseRequisition != null)
                    proformaPurchaseRequisition.StoreRequisitionID = purchaseRequisition.StoreRequisitionID;
            }

            if (ModelState.IsValid)
            {
                var entity = new ProformaPurchaseRequisition
                {
                    ProformaPurchaseID = id,
                    PurchaseRequisitionAssignmentId=proformaPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    StoreRequisitionID = proformaPurchaseRequisition.StoreRequisitionID,
                    PurchaseRequisitionID = proformaPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = proformaPurchaseRequisition.OrgBranchName,
                    DateCreated = proformaPurchaseRequisition.DateCreated,
                    LastModified = proformaPurchaseRequisition.LastModified,
                    CreatedBy = proformaPurchaseRequisition.CreatedBy,
                    ModifiedBy = proformaPurchaseRequisition.ModifiedBy
                };

                _db.ProformaPurchaseRequisitions.Add(entity);
                _db.SaveChanges();
                proformaPurchaseRequisition.ProformaPurchaseRequisitionId = entity.ProformaPurchaseRequisitionId;

                AddPrItems(proformaPurchaseRequisition.PurchaseRequisitionAssignmentId, id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma purchase requisition", "Success", User.Identity.Name, $"created pr {entity.ProformaPurchaseRequisitionId} under record {entity.PurchaseRequisitionID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaPurchaseRequisitions_Update([DataSourceRequest]DataSourceRequest request, ProformaPurchaseRequisition proformaPurchaseRequisition)
        {
            //var exist = _db.ProformaPurchaseRequisitions.Where(x=>x.ProformaPurchaseRequisitionId!=proformaPurchaseRequisition.ProformaPurchaseRequisitionId).Where(x => x.PurchaseRequisitionID == proformaPurchaseRequisition.PurchaseRequisitionID).ToList();
            //if (exist.Any())
            //    ModelState.AddModelError("exist", proformaPurchaseRequisition.PurchaseRequisitionID + " already exists in the system.");

            var assignment =
                _db.PurchaseRequisitionAssignments.Find(proformaPurchaseRequisition.PurchaseRequisitionAssignmentId);


            if (assignment != null)
            {
                proformaPurchaseRequisition.PurchaseRequisitionID = assignment.PurchaseRequisitionID;

                var purchaseRequisition = _db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (purchaseRequisition != null)
                    proformaPurchaseRequisition.StoreRequisitionID = purchaseRequisition.StoreRequisitionID;
            }

            if (ModelState.IsValid)
            {
                var entity = new ProformaPurchaseRequisition
                {
                    ProformaPurchaseID = proformaPurchaseRequisition.ProformaPurchaseID,
                    ProformaPurchaseRequisitionId = proformaPurchaseRequisition.ProformaPurchaseRequisitionId,
                    StoreRequisitionID = proformaPurchaseRequisition.StoreRequisitionID,
                    PurchaseRequisitionAssignmentId = proformaPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = proformaPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = proformaPurchaseRequisition.OrgBranchName,
                    DateCreated = proformaPurchaseRequisition.DateCreated,
                    LastModified = proformaPurchaseRequisition.LastModified,
                    CreatedBy = proformaPurchaseRequisition.CreatedBy,
                    ModifiedBy = proformaPurchaseRequisition.ModifiedBy
                };

                _db.ProformaPurchaseRequisitions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "proforma purchase requisition", "Success", User.Identity.Name, $"updated pr {entity.ProformaPurchaseRequisitionId} under record {entity.PurchaseRequisitionID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { proformaPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaPurchaseRequisitions_Destroy([DataSourceRequest]DataSourceRequest request, ProformaPurchaseRequisition proformaPurchaseRequisition)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProformaPurchaseRequisition
                {
                    StoreRequisitionID = proformaPurchaseRequisition.StoreRequisitionID,
                    ProformaPurchaseID = proformaPurchaseRequisition.ProformaPurchaseID,
                    ProformaPurchaseRequisitionId = proformaPurchaseRequisition.ProformaPurchaseRequisitionId,
                    PurchaseRequisitionAssignmentId = proformaPurchaseRequisition.PurchaseRequisitionAssignmentId,
                    PurchaseRequisitionID = proformaPurchaseRequisition.PurchaseRequisitionID,
                    OrgBranchName = proformaPurchaseRequisition.OrgBranchName,
                    DateCreated = proformaPurchaseRequisition.DateCreated,
                    LastModified = proformaPurchaseRequisition.LastModified,
                    CreatedBy = proformaPurchaseRequisition.CreatedBy,
                    ModifiedBy = proformaPurchaseRequisition.ModifiedBy
                };

                _db.ProformaPurchaseRequisitions.Attach(entity);
                _db.ProformaPurchaseRequisitions.Remove(entity);
                _db.SaveChanges();
                RemovePrItems(proformaPurchaseRequisition.PurchaseRequisitionAssignmentId, proformaPurchaseRequisition.ProformaPurchaseID);
            }

            return Json(new[] { proformaPurchaseRequisition }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        private void AddPrItems(int assignmentid, int proformaid)
        {
            var record = _db.PurchaseRequisitionAssignments.Find(assignmentid);
            if (record != null)
            {
                foreach (var item in record.PurchaseRequisitionReassignmentItems)
                {
                    if (item.ItemType == PurchaseItemType.FixedAsset)
                    {
                        #region  fixed asset

                        var exist = _db.ProformaItems.Where(x => x.ProformaPurchaseID == proformaid)
                            .Where(x => x.ItmCode == item.ItmCode).Where(x=>x.Name==item.Name).FirstOrDefault(x=>x.ModelNo==item.ModelNo);
                        if (exist == null)
                        {

                            ProformaItem proformaItem = new ProformaItem
                            {
                                ProformaPurchaseID = proformaid,
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

                            proformaItem.Quantity = item.Quantity;
                            _db.ProformaItems.Add(proformaItem);
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

                        var exist = _db.ProformaItems
                            .Where(x => x.ProformaPurchaseID == proformaid)
                            .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (exist == null)
                        {

                            ProformaItem proformaItem = new ProformaItem
                            {
                                ProformaPurchaseID = proformaid,
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

                            proformaItem.Quantity = item.Quantity;
                            _db.ProformaItems.Add(proformaItem);
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

                        var exist = _db.ProformaItems.Where(x => x.ProformaPurchaseID == proformaid)
                            .Where(x => x.ItmCode == item.ItmCode).FirstOrDefault(x => x.Name == item.Name);
                        if (exist == null)
                        {

                        }
                        else
                        {
                            decimal qty = exist.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.ProformaItems.Remove(exist);
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

                        var exist = _db.ProformaItems
                            .Where(x => x.ProformaPurchaseID == proformaid)
                            .Where(x => x.ItemSpecification == item.ItemSpecification)//added
                            .FirstOrDefault(x => x.ItmCode == item.ItmCode);
                        if (exist == null)
                        {

                        }
                        else
                        {
                            decimal qty = exist.Quantity - item.Quantity;
                            if (qty <= 0)
                            {
                                _db.ProformaItems.Remove(exist);
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
