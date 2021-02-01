using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PurchaseRequisitions
{
    public class PurchaseRequisitionPurchaseDepartmentValidationReviewController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

        [AuthorizeRoles(ProcurementConstants.ProcurementUser, ProcurementConstants.ProcurementApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUser, ProcurementConstants.ProcurementApprover)]
        public ActionResult PurchaseRequisitionPurchaseDepartmentValidations_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var purchaserequisitionpurchasedepartmentvalidations =
                db.PurchaseRequisitionPurchaseDepartmentValidations.Where(pri => pri.PurchaseRequisitionID == id);
            var result = purchaserequisitionpurchasedepartmentvalidations.ToDataSourceResult(request,
                purchaseRequisitionPurchaseDepartmentValidation => new
                    {
                        purchaseRequisitionPurchaseDepartmentValidation
                            .PurchaseRequisitionPurchaseDepartmentValidationID,
                        purchaseRequisitionPurchaseDepartmentValidation.Status,
                        purchaseRequisitionPurchaseDepartmentValidation.Approver,
                        purchaseRequisitionPurchaseDepartmentValidation.Remark
                    });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementConstants.ProcurementApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionPurchaseDepartmentValidations_Create([DataSourceRequest] DataSourceRequest request, PurchaseRequisitionPurchaseDepartmentValidation purchaseRequisitionPurchaseDepartmentValidation, int id)
        {
            var purchaseRequisiton = db.PurchaseRequisitions.Find(id);
            if (purchaseRequisitionPurchaseDepartmentValidation.Status == ApprovalStatuses.Approve)
            {
                if (purchaseRequisiton != null && purchaseRequisiton.ProcurementOfficer == 0)
                {
                    ModelState.AddModelError("purchaser", " Please assign procurement officer.");

                }

                if (purchaseRequisiton != null && (purchaseRequisiton.PurchaseRequisitionItems.Count == 0 &&
                                                   purchaseRequisiton.PurchaseRequisitionItems.Count == 0))
                {
                    ModelState.AddModelError("noItem",
                        "No Purchase Requisition Item found Please Check purchase Requisition " +
                        purchaseRequisiton.PurchaseRequisitionID);
                }
                else if (purchaseRequisiton != null && purchaseRequisiton.IsSecondOnlineApproved)
                {
                    ModelState.AddModelError("alreadyApproved",
                        "You Can not Manipulate This Record : It is Already Approved");
                }
            }
            if (ModelState.IsValid)
            {
                if (purchaseRequisitionPurchaseDepartmentValidation.Status == ApprovalStatuses.Approve)
                {
                    if (purchaseRequisiton != null)
                    {
                        purchaseRequisiton.Status = "Purchase Approved";
                        purchaseRequisiton.IsSecondOnlineApproved = true;
                        purchaseRequisiton.SecondOnlineApprovedBy = User.Identity.Name;
                        purchaseRequisiton.SecondOnlineApprovedTime = DateTime.Now;
                        db.PurchaseRequisitions.Attach(purchaseRequisiton);
                        db.Entry(purchaseRequisiton).State = EntityState.Modified;
                        db.SaveChanges();

                        #region transfer to proforma

                        if (purchaseRequisiton.ProcurementMethod == ProcurementMethod.Proforma)
                        {
                            TransferToProforma(purchaseRequisiton);
                        }

                        #endregion
                        #region transfer to direct purchase

                        else if (purchaseRequisiton.ProcurementMethod == ProcurementMethod.DirectPurchase)
                        {
                            TransferToDirectPurchase(purchaseRequisiton);
                        }

                        #endregion
                        #region follow up status
                        if (purchaseRequisiton.PurchaseCategory == PurchaseCategory.LocalPurchase)
                        {
                           #region create a follow up
                            var followup = new PurchaseFollowUp
                            {
                                PurchaseRequisitionID = purchaseRequisiton.PurchaseRequisitionID,
                                BranchId = purchaseRequisiton.BranchId,
                                OrgStructureId = purchaseRequisiton.OrgStructureId,
                                ReferenceNo = purchaseRequisiton.ReferenceNo,
                                EmployeeId = purchaseRequisiton.EmployeeId,
                                ProcurementOfficer = purchaseRequisiton.ProcurementOfficer,
                                PurchaseCategory = purchaseRequisiton.PurchaseCategory,
                                ProcurementMethod = purchaseRequisiton.ProcurementMethod,
                                RequestDate = purchaseRequisiton.RequestDate,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };

                            db.PurchaseFollowUps.Add(followup);
                            db.SaveChanges();
                            #endregion
                           #region system status

                            var followupsystem = new PurchaseFollowUpSystemStatus()
                            {
                                PurchaseFollowUpID = followup.PurchaseFollowUpID,
                                PurchaseFollowUp = followup,
                                Date = DateTime.Now,
                                Description = "User Department approved",
                                User = User.Identity.Name
                            };

                            db.PurchaseFollowUpSystemStatus.Add(followupsystem);
                            db.SaveChanges();

                            #endregion
                           #region add items

                            foreach (var item in purchaseRequisiton.PurchaseRequisitionItems)
                            {
                                var items = new PurchaseFollowUpItems
                                {
                                    PurchaseFollowUpID = followup.PurchaseFollowUpID,
                                    CatCode = item.CatCode,
                                    CategoryName = item.CategoryName,
                                    ItmCode = item.ItmCode,
                                    ItemName = item.ItemName,
                                    PartNo = item.PartNo,
                                    Quantity = item.Quantity,
                                    ItemSpecification = item.ItemSpecification,
                                };

                                db.PurchaseFollowUpItems.Add(items);
                                db.SaveChanges();

                            }


                            #endregion 
                        }
                        else
                        {
                           #region create a follow up
                            var followup = new ForeignPurchase
                            {
                                OrgStructureId = purchaseRequisiton.OrgStructureId,
                                ProcurementMethod = purchaseRequisiton.ProcurementMethod.ToString(),
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };

                            db.ForeignPurchases.Add(followup);
                            db.SaveChanges();
                            #endregion
                           #region system status

                            var followupsystem = new ForeignPurchaseFollowUpSystemStatus()
                            {
                                ForeignPurchaseId = followup.ForeignPurchaseId,
                                ForeignPurchase = followup,
                                Date = DateTime.Now,
                                Description = "User Department approved",
                                User = User.Identity.Name
                            };

                            db.ForeignPurchaseFollowUpSystemStatus.Add(followupsystem);
                            db.SaveChanges();

                            #endregion
                           #region add items

                            foreach (var item in purchaseRequisiton.PurchaseRequisitionItems)
                            {
                                var items = new ForeignPurchaseFollowUpItems
                                {
                                    ForeignPurchaseId = followup.ForeignPurchaseId,
                                    CatCode = item.CatCode,
                                    CategoryName = item.CategoryName,
                                    ItmCode = item.ItmCode,
                                    ItemName = item.ItemName,
                                    PartNo = item.PartNo,
                                    Quantity = item.Quantity,
                                    ItemSpecification = item.ItemSpecification,
                                };

                                db.ForeignPurchaseFollowUpItems.Add(items);
                                db.SaveChanges();

                            }


                            #endregion 
                        }
                       

                        _dashboardHelper.AddNotofication(Request.UserHostAddress, "", "", DateTime.Now, "", "Purchase requisition no : " + purchaseRequisiton.PurchaseRequisitionID + " Procurement approved by user : " + User.Identity.Name + " at " + DateTime.Now);
                        #endregion
                        #region notification

                        NotifModels notif = new NotifModels
                        {
                            UserId =UserInformationHelper.GetEmployeeUserId(purchaseRequisiton.ProcurementOfficer),
                            Title = "Purchase requisiton assignement",
                            Message =$" {UserInformationHelper.GetUserFullName(User.Identity.Name)} has assigned you as a team leader for purchase requisition with ID {purchaseRequisiton.PurchaseRequisitionID}  with method  {purchaseRequisiton.ProcurementMethod.ToString()} "
                        };

                        Notify.NotifyUser(notif);

                        #endregion
                    }
                }
                else if (purchaseRequisitionPurchaseDepartmentValidation.Status == ApprovalStatuses.Reject)
                {
                    if (purchaseRequisiton != null)
                    {
                        purchaseRequisiton.Status = "Purchase Rejected";
                        _dashboardHelper.AddNotofication(Request.UserHostAddress, "", "", DateTime.Now, "", "Purchase requisition no : " + purchaseRequisiton.PurchaseRequisitionID + " Procurement rejected by user : " + User.Identity.Name + " at " + DateTime.Now);
                        db.PurchaseRequisitions.Attach(purchaseRequisiton);
                        db.Entry(purchaseRequisiton).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                var entity = new PurchaseRequisitionPurchaseDepartmentValidation
                    {
                        PurchaseRequisitionID = id,
                        Status = purchaseRequisitionPurchaseDepartmentValidation.Status,
                        Approver = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = purchaseRequisitionPurchaseDepartmentValidation.Remark
                    };

                db.PurchaseRequisitionPurchaseDepartmentValidations.Add(entity);
                db.SaveChanges();
                purchaseRequisitionPurchaseDepartmentValidation.PurchaseRequisitionPurchaseDepartmentValidationID = entity.PurchaseRequisitionPurchaseDepartmentValidationID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition department approval", "Success", User.Identity.Name, $"approved purchase requisition {entity.PurchaseRequisitionID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { purchaseRequisitionPurchaseDepartmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
    

      
        private void TransferToProforma(PurchaseRequisition purchaseRequisition)
        {
            bool transferrable = false;
            #region check if there is anything to send to proforma

            foreach (var requestedItem in purchaseRequisition.PurchaseRequisitionItems)
            {
                if (requestedItem.Quantity > 0)
                {
                    transferrable = true;
                    break;
                }
            }

            #endregion

            if (transferrable)
            {
                #region transfer to proforma

                var assignment = db.PurchaseRequisitionAssignments
                    .Where(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID)
                    .Where(x => x.ProcurementMethod == ProcurementMethod.Proforma).ToList();
                foreach (var assign in assignment)
                {
                    var exist = db.ProformaPurchaseRequisitions
                        .Where(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID).ToList();
                    if (!exist.Any())
                    {
                        #region create proforma

                        var proforma = new ProformaPurchase
                        {
                            PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                            ProformaReference = (String.IsNullOrEmpty(purchaseRequisition.ReferenceNo))? purchaseRequisition.ReferenceNo: "NA",
                            Description = "NA",
                            ProcurementOfficer = assign.ProcurementOfficer,
                            FloatDate = DateTime.Today,
                            ClosingDate = DateTime.Today,
                            EvaluationDate = DateTime.Today,


                            Status = "Editing",
                            Remark = purchaseRequisition.Remark,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                        };
                        db.ProformaPurchases.Add(proforma);
                        db.SaveChanges();

                        #endregion

                        #region pr
                        var proformaPurchaseRequisition = new ProformaPurchaseRequisition
                        {
                            ProformaPurchase = proforma,
                            StoreRequisitionID = purchaseRequisition.StoreRequisitionID,
                            PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID
                        };
                        db.ProformaPurchaseRequisitions.Add(proformaPurchaseRequisition);
                        db.SaveChanges();
                        #endregion

                        foreach (var requestedItem in purchaseRequisition.PurchaseRequisitionItems)
                        {
                            if (requestedItem.Quantity > 0)
                            {
                                #region stock

                                var proformaItem = new ProformaItem
                                {
                                    ProformaPurchase = proforma,
                                    CatCode = requestedItem.CatCode,
                                    CategoryName = requestedItem.CategoryName,
                                    ItmCode = requestedItem.ItmCode,
                                    ItemName = requestedItem.ItemName,
                                    Name=requestedItem.Name,
                                    ModelNo=requestedItem.ModelNo,
                                    PartNo = requestedItem.PartNo,
                                    Quantity = requestedItem.Quantity,
                                    IsApproved = requestedItem.IsApproved,
                                    ItemSpecification = requestedItem.ItemSpecification,
                                    UoM = requestedItem.UoM,
                                    ItemType = requestedItem.ItemType,

                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,
                                };
                                db.ProformaItems.Add(proformaItem);
                                db.SaveChanges();

                                #endregion
                            }

                        }
                    }
                }
               

                #endregion
            }

        }
        private void TransferToDirectPurchase(PurchaseRequisition purchaseRequisition)
        {
            bool transferrable = false;
            #region check if there is anything to transfer

            foreach (var requestedItem in purchaseRequisition.PurchaseRequisitionItems)
            {
                if (requestedItem.Quantity > 0)
                {
                    transferrable = true;
                    break;
                }
            }

            #endregion

            if (transferrable)
            {
                #region transfer to purchase order

                string sr = "";
                var pr = db.PurchaseRequisitions.Find(purchaseRequisition.PurchaseRequisitionID);
                if (pr != null)
                    sr = pr.StoreRequisitionID.ToString();

                var assignment = db.PurchaseRequisitionAssignments
                    .Where(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID)
                    .Where(x => x.ProcurementMethod == ProcurementMethod.DirectPurchase).ToList();
                foreach (var assign in assignment)
                {
                    var exist = db.ProformaPurchaseRequisitions
                        .Where(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID).ToList();
                    if (!exist.Any())
                    {
                        #region create po

                        var purchaseOrder = new PurchaseOrder
                        {

                            PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                            SroreRequisition = sr,
                            ProcurementOfficer = assign.ProcurementOfficer,
                            ForwardedTo = UserInformationHelper.GetEmployeeUserId(assign.ProcurementOfficer),
                            IsManual = true,
                            SupplierName = "",
                            SupplierId = "",
                            POType = POType.CashBase,
                            ProcurementMethod = ProcurementMethod.DirectPurchase,
                            ProformaPurchaseID = 0,
                            OrderDate = DateTime.Today,
                            DeliveryPlace = "",
                            DeliveryDate = DateTime.Today,
                            OrgStructureId = purchaseRequisition.OrgStructureId,


                            Status = "Editing",
                            Remark = purchaseRequisition.Remark,

                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                        };
                        db.PurchaseOrders.Add(purchaseOrder);
                        db.SaveChanges();

                        #endregion

                        foreach (var requisitionItem in purchaseRequisition.PurchaseRequisitionItems)
                        {
                            if (requisitionItem.Quantity > 0)
                            {
                                #region item

                                var purchaseOrderItem = new PurchaseOrderItem
                                {
                                    PurchaseOrder = purchaseOrder,
                                    CatCode = requisitionItem.CatCode,
                                    CategoryName = requisitionItem.CategoryName,
                                    ItmCode = requisitionItem.ItmCode,
                                    ItemName = requisitionItem.ItemName,
                                    Name=requisitionItem.Name,
                                    ModelNo=requisitionItem.ModelNo,
                                    PartNo = requisitionItem.PartNo,
                                    Quantity = requisitionItem.Quantity,
                                    IsApproved = requisitionItem.IsApproved,
                                    ItemSpecification = requisitionItem.ItemSpecification,
                                    UoM = requisitionItem.UoM,
                                    ItemType = requisitionItem.ItemType,

                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Today,
                                    ModifiedBy = User.Identity.Name,
                                    LastModified = DateTime.Today
                                };
                                db.PurchaseOrderItems.Add(purchaseOrderItem);
                                db.SaveChanges();

                                #endregion
                            }

                        }
                    }
                }

                #endregion
            }

        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}