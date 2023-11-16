using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PurchaseRequisitions
{
    public class PurchaseRequisitionReassignmentValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchaseRequisitionReassignmentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchaseRequisitionReassignmentValidation> purchaserequisitionreassignmentvalidations = db.PurchaseRequisitionReassignmentValidations.Where(x=>x.PurchaseRequisitionAssignmentId==id);
            DataSourceResult result = purchaserequisitionreassignmentvalidations.ToDataSourceResult(request, purchaseRequisitionReassignmentValidation => new {
                PurchaseRequisitionReassignmentValidationId = purchaseRequisitionReassignmentValidation.PurchaseRequisitionReassignmentValidationId,
                Status = purchaseRequisitionReassignmentValidation.Status,
                Remark = purchaseRequisitionReassignmentValidation.Remark,
                OrgBranchName = purchaseRequisitionReassignmentValidation.OrgBranchName,
                DateCreated = purchaseRequisitionReassignmentValidation.DateCreated,
                LastModified = purchaseRequisitionReassignmentValidation.LastModified,
                CreatedBy = purchaseRequisitionReassignmentValidation.CreatedBy,
                ModifiedBy = purchaseRequisitionReassignmentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentValidation purchaseRequisitionReassignmentValidation)
        {
            var purchaseRequisitionAssignment = db.PurchaseRequisitionAssignments.Find(id);
            if (purchaseRequisitionReassignmentValidation.Status == ApprovalStatuses.Approve)
            {
                 if (purchaseRequisitionAssignment != null && purchaseRequisitionAssignment.IsOnlineApproved)
                    ModelState.AddModelError("alreadyApproved", "You Can not Manipulate This Record : It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (purchaseRequisitionReassignmentValidation.Status == ApprovalStatuses.Approve)
                {
                    if (purchaseRequisitionAssignment != null)
                    {
                        purchaseRequisitionAssignment.Status = "Approved";
                        purchaseRequisitionAssignment.IsOnlineApproved = true;
                        purchaseRequisitionAssignment.OnlineApprovedBy = User.Identity.Name;
                        purchaseRequisitionAssignment.OnlineApprovedTime = DateTime.Now;

                        db.PurchaseRequisitionAssignments.Attach(purchaseRequisitionAssignment);
                        db.Entry(purchaseRequisitionAssignment).State = EntityState.Modified;
                        db.SaveChanges();

                        //
                        var purchaseReq =
                            db.PurchaseRequisitions.Find(purchaseRequisitionAssignment.PurchaseRequisitionID);

                        if (purchaseReq != null && purchaseReq.FromPreviousPurchase  && purchaseRequisitionAssignment.ProcurementMethod==ProcurementMethod.DirectPurchase)
                        {
                            TransferPrevPurchaseToDirectPurchase(purchaseRequisitionAssignment);
                        }
                        else if (purchaseRequisitionAssignment.ProcurementMethod == ProcurementMethod.DirectPurchase)
                        {
                            TransferPurchaseToDirectPurchase(purchaseRequisitionAssignment);
                        }
                        else if (purchaseRequisitionAssignment.ProcurementMethod == ProcurementMethod.Proforma)
                        {
                            TransferPurchaseToProforma(purchaseRequisitionAssignment);
                        }
                        else if (purchaseRequisitionAssignment.ProcurementMethod == ProcurementMethod.Tender)
                        {
                            TransferPurchaseToTender(purchaseRequisitionAssignment);
                        }
                    }
                }
                else if (purchaseRequisitionReassignmentValidation.Status == ApprovalStatuses.Reject)
                {
                    if (purchaseRequisitionAssignment != null)
                    {
                        purchaseRequisitionAssignment.Status = "Rejected";
                        db.PurchaseRequisitionAssignments.Attach(purchaseRequisitionAssignment);
                        db.Entry(purchaseRequisitionAssignment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                var entity = new PurchaseRequisitionReassignmentValidation
                {
                    PurchaseRequisitionAssignmentId =id,
                    Status = purchaseRequisitionReassignmentValidation.Status,
                    Remark = purchaseRequisitionReassignmentValidation.Remark,
                    OrgBranchName = purchaseRequisitionReassignmentValidation.OrgBranchName,
                    DateCreated = purchaseRequisitionReassignmentValidation.DateCreated,
                    LastModified = purchaseRequisitionReassignmentValidation.LastModified,
                    CreatedBy = purchaseRequisitionReassignmentValidation.CreatedBy,
                    ModifiedBy = purchaseRequisitionReassignmentValidation.ModifiedBy
                };

                db.PurchaseRequisitionReassignmentValidations.Add(entity);
                db.SaveChanges();
                purchaseRequisitionReassignmentValidation.PurchaseRequisitionReassignmentValidationId = entity.PurchaseRequisitionReassignmentValidationId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchaserequisition reassignment approval", "Success", User.Identity.Name, $"approved po {entity.PurchaseRequisitionAssignmentId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaseRequisitionReassignmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentValidations_Update([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentValidation purchaseRequisitionReassignmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionReassignmentValidation
                {
                    PurchaseRequisitionReassignmentValidationId = purchaseRequisitionReassignmentValidation.PurchaseRequisitionReassignmentValidationId,
                    Status = purchaseRequisitionReassignmentValidation.Status,
                    Remark = purchaseRequisitionReassignmentValidation.Remark,
                    OrgBranchName = purchaseRequisitionReassignmentValidation.OrgBranchName,
                    DateCreated = purchaseRequisitionReassignmentValidation.DateCreated,
                    LastModified = purchaseRequisitionReassignmentValidation.LastModified,
                    CreatedBy = purchaseRequisitionReassignmentValidation.CreatedBy,
                    ModifiedBy = purchaseRequisitionReassignmentValidation.ModifiedBy
                };

                db.PurchaseRequisitionReassignmentValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { purchaseRequisitionReassignmentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionReassignmentValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseRequisitionReassignmentValidation purchaseRequisitionReassignmentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseRequisitionReassignmentValidation
                {
                    PurchaseRequisitionReassignmentValidationId = purchaseRequisitionReassignmentValidation.PurchaseRequisitionReassignmentValidationId,
                    Status = purchaseRequisitionReassignmentValidation.Status,
                    Remark = purchaseRequisitionReassignmentValidation.Remark,
                    OrgBranchName = purchaseRequisitionReassignmentValidation.OrgBranchName,
                    DateCreated = purchaseRequisitionReassignmentValidation.DateCreated,
                    LastModified = purchaseRequisitionReassignmentValidation.LastModified,
                    CreatedBy = purchaseRequisitionReassignmentValidation.CreatedBy,
                    ModifiedBy = purchaseRequisitionReassignmentValidation.ModifiedBy
                };

                db.PurchaseRequisitionReassignmentValidations.Attach(entity);
                db.PurchaseRequisitionReassignmentValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { purchaseRequisitionReassignmentValidation }.ToDataSourceResult(request, ModelState));
        }


        private void TransferPrevPurchaseToDirectPurchase(PurchaseRequisitionAssignment assignment)
        {
            var purchaseReq =
                db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);

            bool transferrable = false;
           

            if (purchaseReq != null)
            {
                #region check if there is anything to transfer
                foreach (var requestedItem in assignment.PurchaseRequisitionReassignmentItems)
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
                    string sr = "";
                    var pr = db.PurchaseRequisitions.Find(purchaseReq.PurchaseRequisitionID);
                    if (pr != null)
                        sr = pr.StoreRequisitionID.ToString();


                    #region from po

                    if (purchaseReq.PreviousProcurementMethod == ProcurementMethod.DirectPurchase)
                    {
                        #region transfer from purchase order

                        var po = db.PurchaseOrders.Find(int.Parse(purchaseReq.PreviousPurchaseReference));
                        var poitems = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == po.PurchaseOrderID).ToList();

                        #region create po

                        var purchaseOrder = new PurchaseOrder
                        {
                            PurchaseRequisitionID = purchaseReq.PurchaseRequisitionID,
                            SroreRequisition = sr,
                            ProcurementOfficer = assignment.ProcurementOfficer,
                            BranchId= EmployeeHelper.GetEmployeePlacementBranch(assignment.ProcurementOfficer),
                            BusinessUnitId=pr.BusinessUnitId,
                            IsManual = false,
                            SupplierName =po.SupplierName,
                            SupplierId = po.SupplierId,
                            POType = POType.CashBase,
                            ProcurementMethod = ProcurementMethod.DirectPurchase,
                            ProformaPurchaseID = 0,
                            OrderDate = DateTime.Today,
                            DeliveryPlace = po.DeliveryPlace,
                            DeliveryDate = DateTime.Today,
                            OrgStructureId = purchaseReq.OrgStructureId,

                            ForwardedFrom = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                            ForwardedTo =UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                            IsFinalApproval = false,

                            Status = "Editing",
                            Remark = purchaseReq.Remark,

                            DateCreated = DateTime.Now,
                            CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                        };
                        db.PurchaseOrders.Add(purchaseOrder);
                        db.SaveChanges();

                        #endregion

                        foreach (var requisitionItem in assignment.PurchaseRequisitionReassignmentItems)
                        {
                            if (requisitionItem.Quantity > 0)
                            {
                                var previtem = poitems
                                    .Where(x=>x.ItemSpecification==requisitionItem.ItemSpecification)//added
                                    .FirstOrDefault(x => x.ItmCode == requisitionItem.ItmCode);
                                #region item

                                if (previtem != null)
                                {
                                    decimal qty = requisitionItem.Quantity < previtem.Quantity
                                        ? requisitionItem.Quantity
                                        : previtem.Quantity;
                                    var tax = db.GLTaxRates.Find(previtem.GLTaxRateID);

                                    if (tax != null)
                                    {
                                        var orderItem = new PurchaseOrderItem
                                        {
                                            FromPreviousPurchase = true,
                                            PurchaseOrder = purchaseOrder,
                                            CatCode = requisitionItem.CatCode,
                                            CategoryName = requisitionItem.CategoryName,
                                            ItmCode = requisitionItem.ItmCode,
                                            ItemName = requisitionItem.ItemName,
                                            Name = requisitionItem.Name,
                                            ModelNo = requisitionItem.ModelNo,
                                            PartNo = requisitionItem.PartNo,
                                            Quantity = qty,
                                            UnitPrice = previtem.UnitPrice,
                                            SubTotal = qty * previtem.UnitPrice,
                                            TaxAmount = qty * previtem.UnitPrice * tax.CalculatedRate,
                                            Total =
                                                qty * previtem.UnitPrice + qty * previtem.UnitPrice * tax.CalculatedRate,
                                            
                                            ItemSpecification = requisitionItem.ItemSpecification,
                                            UoM = requisitionItem.UoM,
                                            ItemType = requisitionItem.ItemType,

                                            CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                                            DateCreated = DateTime.Today
                                        };
                                        db.PurchaseOrderItems.Add(orderItem);
                                    }

                                    db.SaveChanges();
                                }

                                #endregion
                            }
                        }

                        #endregion
                    }

                    #endregion
                    #region from proforma

                    if (purchaseReq.PreviousProcurementMethod == ProcurementMethod.Proforma)
                    {
                        #region transfer from proforma

                        var winningSupplier = db.WinningSuppliers.Find(int.Parse(purchaseReq.PreviousPurchaseReference));
                        var winningitems = db.WinningSuppliers.Where(x => x.ProformaPurchaseID == winningSupplier.ProformaPurchaseID)
                            .Where(x => x.SupplierName == winningSupplier.SupplierName)
                            .ToList();

                        #region create po

                        var purchaseOrder = new PurchaseOrder
                        {
                            PurchaseRequisitionID = purchaseReq.PurchaseRequisitionID,
                            BranchId=purchaseReq.BranchId,
                            BusinessUnitId=purchaseReq.BusinessUnitId,
                            SroreRequisition = sr,
                            ProcurementOfficer = assignment.ProcurementOfficer,
                            IsManual = false,
                            SupplierName = winningSupplier.SupplierName,
                            SupplierId = winningSupplier.SupplierId,
                            POType = POType.CashBase,
                            ProcurementMethod = ProcurementMethod.DirectPurchase,
                            ProformaPurchaseID = 0,
                            OrderDate = DateTime.Today,
                            DeliveryPlace = "NA",
                            DeliveryDate = DateTime.Today,
                            OrgStructureId = purchaseReq.OrgStructureId,


                            Status = "Editing",
                            Remark = purchaseReq.Remark,

                            DateCreated = DateTime.Now,
                            CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                        };
                        db.PurchaseOrders.Add(purchaseOrder);
                        db.SaveChanges();

                        #endregion

                        foreach (var requisitionItem in purchaseReq.PurchaseRequisitionItems)
                        {
                            if (requisitionItem.Quantity > 0)
                            {
                                var previtem = winningitems.SelectMany(x=>x.WinningItems)
                                    .Where(x => x.ItemSpecification == requisitionItem.ItemSpecification)//added
                                    .FirstOrDefault(x => x.ItmCode == requisitionItem.ItmCode);
                                #region item

                                if (previtem != null)
                                {
                                    decimal qty = requisitionItem.Quantity < previtem.Quanity
                                        ? requisitionItem.Quantity
                                        : previtem.Quanity;
                                    var tax = db.GLTaxRates.Find(previtem.GLTaxRateID);

                                    if (tax != null)
                                    {
                                        var orderItem = new PurchaseOrderItem
                                        {
                                            FromPreviousPurchase = true,
                                            PurchaseOrder = purchaseOrder,
                                            CatCode = requisitionItem.CatCode,
                                            CategoryName = requisitionItem.CategoryName,
                                            ItmCode = requisitionItem.ItmCode,
                                            ItemName = requisitionItem.ItemName,
                                            Name = requisitionItem.Name,
                                            ModelNo = requisitionItem.ModelNo,
                                            PartNo = requisitionItem.PartNo,
                                            Quantity = qty,
                                            UnitPrice = previtem.UPrice,
                                            SubTotal = qty * previtem.UPrice,
                                            TaxAmount = qty * previtem.UPrice * tax.CalculatedRate,
                                            Total =qty * previtem.UPrice + qty * previtem.UPrice * tax.CalculatedRate,
                                            IsApproved = requisitionItem.IsApproved,
                                            ItemSpecification = requisitionItem.ItemSpecification,
                                            UoM = requisitionItem.UoM,
                                            ItemType = requisitionItem.ItemType,

                                            CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                                            DateCreated = DateTime.Today
                                           
                                        };
                                        db.PurchaseOrderItems.Add(orderItem);
                                    }

                                    db.SaveChanges();
                                }

                                #endregion
                            }
                        }

                        #endregion
                    }

                    #endregion
                    #region from tender 

                    if (purchaseReq.PreviousProcurementMethod == ProcurementMethod.Tender)
                    {
                        #region transfer from tender

                        var bidAwardedSupplier = db.BidAwardedSuppliers.Find(int.Parse(purchaseReq.PreviousPurchaseReference));
                        var awardedSuppliers = db.BidAwardedSuppliers.Where(x => x.LotID == bidAwardedSupplier.LotID)
                            .Where(x => x.SupplierName == bidAwardedSupplier.SupplierName)
                            .ToList();

                        #region create po

                        var purchaseOrder = new PurchaseOrder
                        {
                            PurchaseRequisitionID = purchaseReq.PurchaseRequisitionID,
                            BranchId=purchaseReq.BranchId,
                            BusinessUnitId=purchaseReq.BusinessUnitId,
                            SroreRequisition = sr,
                            ProcurementOfficer = assignment.ProcurementOfficer,
                            IsManual = false,
                            SupplierName = bidAwardedSupplier.SupplierName,
                            SupplierId = bidAwardedSupplier.SupplierId,
                            POType = POType.CashBase,
                            ProcurementMethod = ProcurementMethod.DirectPurchase,
                            ProformaPurchaseID = 0,
                            OrderDate = DateTime.Today,
                            DeliveryPlace = "NA",
                            DeliveryDate = DateTime.Today,
                            OrgStructureId = purchaseReq.OrgStructureId,


                            Status = "Editing",
                            Remark = purchaseReq.Remark,

                            DateCreated = DateTime.Now,
                             CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                        };
                        db.PurchaseOrders.Add(purchaseOrder);
                        db.SaveChanges();

                        #endregion

                        foreach (var requisitionItem in purchaseReq.PurchaseRequisitionItems)
                        {
                            if (requisitionItem.Quantity > 0)
                            {
                                var previtem = awardedSuppliers.SelectMany(x=>x.BidWinningItems)
                                    .Where(x => x.ItemSpecification == requisitionItem.ItemSpecification)//added
                                    .FirstOrDefault(x => x.ItmCode == requisitionItem.ItmCode);
                                #region item

                                if (previtem != null)
                                {
                                    decimal qty = requisitionItem.Quantity < previtem.Quanity
                                        ? requisitionItem.Quantity
                                        : previtem.Quanity;
                                    var tax = db.GLTaxRates.Find(previtem.GLTaxRateID);

                                    if (tax != null)
                                    {
                                        var orderItem = new PurchaseOrderItem
                                        {
                                            FromPreviousPurchase = true,
                                            PurchaseOrder = purchaseOrder,
                                            CatCode = requisitionItem.CatCode,
                                            CategoryName = requisitionItem.CategoryName,
                                            ItmCode = requisitionItem.ItmCode,
                                            ItemName = requisitionItem.ItemName,
                                            Name = requisitionItem.Name,
                                            ModelNo = requisitionItem.ModelNo,
                                            PartNo = requisitionItem.PartNo,
                                            Quantity = qty,
                                            UnitPrice = previtem.UPrice,
                                            SubTotal = qty * previtem.UPrice,
                                            TaxAmount = qty * previtem.UPrice * tax.CalculatedRate,
                                            Total = qty * previtem.UPrice + qty * previtem.UPrice * tax.CalculatedRate,
                                            IsApproved = requisitionItem.IsApproved,
                                            ItemSpecification = requisitionItem.ItemSpecification,
                                            UoM = requisitionItem.UoM,
                                            ItemType = requisitionItem.ItemType,

                                            CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                                            DateCreated = DateTime.Today
                                        };
                                        db.PurchaseOrderItems.Add(orderItem);
                                    }

                                    db.SaveChanges();
                                }

                                #endregion
                            }
                        }

                        #endregion
                    }

                    #endregion
                }
            }
        }

        private void TransferPurchaseToDirectPurchase(PurchaseRequisitionAssignment assignment)
        {
            var purchaseReq =db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);

            bool transferrable = false;


            if (purchaseReq != null)
            {
                #region check if there is anything to transfer
                foreach (var requestedItem in assignment.PurchaseRequisitionReassignmentItems)
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
                    string sr = "";
                    var pr = db.PurchaseRequisitions.Find(purchaseReq.PurchaseRequisitionID);
                    if (pr != null)
                    {
                        sr = pr.StoreRequisitionID.ToString();


                        #region create po

                        #region transfer to purchase order

                        #region create po

                        var purchaseOrder = new PurchaseOrder
                        {
                            PurchaseRequisitionID = purchaseReq.PurchaseRequisitionID,
                            BusinessUnitId = purchaseReq.BusinessUnitId,
                            SroreRequisition = sr,
                            ProcurementOfficer = assignment.ProcurementOfficer,
                            BranchId = EmployeeHelper.GetEmployeePlacementBranch(assignment.ProcurementOfficer),
                            IsManual = true,
                            SupplierName = "",
                            SupplierId = "",
                            POType = POType.CashBase,
                            ProcurementMethod = ProcurementMethod.DirectPurchase,
                            ProformaPurchaseID = 0,
                            OrderDate = DateTime.Today,
                            DeliveryPlace = "",
                            DeliveryDate = DateTime.Today,
                            OrgStructureId = purchaseReq.OrgStructureId,

                            ForwardedFrom = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                            ForwardedTo = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                            IsFinalApproval = false,

                            Status = "Editing",
                            Remark = purchaseReq.Remark,

                            DateCreated = DateTime.Now,
                            CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                        };
                        db.PurchaseOrders.Add(purchaseOrder);
                        db.SaveChanges();

                        #endregion

                        foreach (var requisitionItem in assignment.PurchaseRequisitionReassignmentItems)
                        {
                            if (requisitionItem.Quantity > 0)
                            {
                                #region item

                                var orderItem = new PurchaseOrderItem
                                {
                                    FromPreviousPurchase = true,
                                    PurchaseOrder = purchaseOrder,
                                    CatCode = requisitionItem.CatCode,
                                    CategoryName = requisitionItem.CategoryName,
                                    ItmCode = requisitionItem.ItmCode,
                                    ItemName = requisitionItem.ItemName,
                                    Name = requisitionItem.Name,
                                    ModelNo = requisitionItem.ModelNo,
                                    PartNo = requisitionItem.PartNo,
                                    Quantity = requisitionItem.Quantity,
                                    UnitPrice = 0,
                                    SubTotal = 0,
                                    TaxAmount = 0,
                                    Total = 0,

                                    ItemSpecification = requisitionItem.ItemSpecification,
                                    UoM = requisitionItem.UoM,
                                    ItemType = requisitionItem.ItemType,

                                    CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                                    DateCreated = DateTime.Today,
                                    ModifiedBy = User.Identity.Name,
                                    LastModified = DateTime.Today
                                };
                                if (pr.FromPreviousPurchase)
                                {
                                    #region get price from previous
                                    int did = int.Parse(pr.PreviousPurchaseReference);
                                    if (pr.PreviousProcurementMethod == ProcurementMethod.Tender)
                                    {
                                        var awardItem = db.BidAwardedSupplierItems
                                            .Where(x => x.ItmCode == requisitionItem.ItmCode)
                                            .Where(x => x.ItemType == requisitionItem.ItemType);

                                        if (requisitionItem.ItemType == PurchaseItemType.FixedAsset)
                                            awardItem = awardItem.Where(x => x.Name == requisitionItem.Name);

                                        var previtem = awardItem.FirstOrDefault();
                                        if (previtem != null)
                                        {
                                            orderItem.UnitPrice = previtem.UPrice;
                                            orderItem.SubTotal = orderItem.Quantity * previtem.UPrice;
                                            var tax = db.GLTaxRates.Find(previtem.GLTaxRateID);
                                            if (tax != null)
                                                orderItem.TaxAmount = orderItem.SubTotal * tax.CalculatedRate/100;

                                            orderItem.Total = orderItem.SubTotal + orderItem.TaxAmount;
                                        }
                                    }
                                    else if (pr.PreviousProcurementMethod == ProcurementMethod.Proforma)
                                    {
                                        var awardItem = db.WinningSupplierItems
                                            .Where(x => x.ItmCode == requisitionItem.ItmCode)
                                            .Where(x => x.ItemType == requisitionItem.ItemType);

                                        if (requisitionItem.ItemType == PurchaseItemType.FixedAsset)
                                            awardItem = awardItem.Where(x => x.Name == requisitionItem.Name);

                                        var previtem = awardItem.FirstOrDefault();
                                        if (previtem != null)
                                        {
                                            orderItem.UnitPrice = previtem.UPrice;
                                            orderItem.SubTotal = orderItem.Quantity * previtem.UPrice;
                                            var tax = db.GLTaxRates.Find(previtem.GLTaxRateID);
                                            if (tax != null)
                                                orderItem.TaxAmount = orderItem.SubTotal * tax.CalculatedRate / 100;

                                            orderItem.Total = orderItem.SubTotal + orderItem.TaxAmount;
                                        }
                                    }

                                    else if (pr.PreviousProcurementMethod == ProcurementMethod.DirectPurchase)
                                    {
                                        var awardItem = db.PurchaseOrderItems
                                            .Where(x => x.ItmCode == requisitionItem.ItmCode)
                                            .Where(x => x.ItemType == requisitionItem.ItemType);

                                        if (requisitionItem.ItemType == PurchaseItemType.FixedAsset)
                                            awardItem = awardItem.Where(x => x.Name == requisitionItem.Name);

                                        var previtem = awardItem.FirstOrDefault();
                                        if (previtem != null)
                                        {
                                            orderItem.UnitPrice = previtem.UnitPrice;
                                            orderItem.SubTotal = orderItem.Quantity * previtem.UnitPrice;
                                            var tax = db.GLTaxRates.Find(previtem.GLTaxRateID);
                                            if (tax != null)
                                                orderItem.TaxAmount = orderItem.SubTotal * tax.CalculatedRate / 100;

                                            orderItem.Total = orderItem.SubTotal + orderItem.TaxAmount;
                                        }
                                    }


                                    #endregion
                                }

                                db.PurchaseOrderItems.Add(orderItem);
                                db.SaveChanges();


                                #endregion
                            }
                        }

                        #endregion

                        #endregion
                    }
                }
            }
        }

        private void TransferPurchaseToProforma(PurchaseRequisitionAssignment assignment)
        {
            var purchaseReq = db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);

            bool transferrable = false;


            if (purchaseReq != null)
            {
                #region check if there is anything to transfer
                foreach (var requestedItem in assignment.PurchaseRequisitionReassignmentItems)
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
                    int sr = 0;
                    var pr = db.PurchaseRequisitions.Find(purchaseReq.PurchaseRequisitionID);
                    if (pr != null)
                        sr = pr.StoreRequisitionID;


                    #region create proforma

                    #region transfer to proforma

                    #region create proforma

                    var proforma = new ProformaPurchase
                    {

                        StoreRequisitionID = sr,
                        BusinessUnitId= purchaseReq.BusinessUnitId,
                        Description="NA",
                        ProformaReference = String.IsNullOrEmpty(purchaseReq.ReferenceNo)?"NA": purchaseReq.ReferenceNo,
                        ProcurementOfficer = assignment.ProcurementOfficer,
                        BranchId = EmployeeHelper.GetEmployeePlacementBranch(assignment.ProcurementOfficer),
                        FloatDate = DateTime.Today,
                        PurchaseRequisitionID = purchaseReq.PurchaseRequisitionID,
                        ClosingDate = DateTime.Today,
                        EvaluationDate =DateTime.Today,
                        ForwardedFrom = User.Identity.Name,
                        ForwardedTo = User.Identity.Name,
                        IsFinalApproval = false,


                        Status = "Editing",
                        Remark = purchaseReq.Remark,

                        DateCreated = DateTime.Now,
                         CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                    };
                    db.ProformaPurchases.Add(proforma);
                    db.SaveChanges();

                    #endregion

                    #region create proforma pr

                    var proformapr = new ProformaPurchaseRequisition
                    {
                        ProformaPurchase = proforma,
                        StoreRequisitionID = sr,
                        PurchaseRequisitionID = purchaseReq.PurchaseRequisitionID,
                        DateCreated = DateTime.Now,
                        CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                        PurchaseRequisitionAssignmentId=assignment.PurchaseRequisitionAssignmentId
                    };
                    db.ProformaPurchaseRequisitions.Add(proformapr);
                    db.SaveChanges();

                    #endregion

                    foreach (var requisitionItem in assignment.PurchaseRequisitionReassignmentItems)
                    {
                        if (requisitionItem.Quantity > 0)
                        {
                            #region item
                            var proformaItem = new ProformaItem
                            {
                                ProformaPurchase = proforma,
                                CatCode = requisitionItem.CatCode,
                                CategoryName = requisitionItem.CategoryName,
                                ItmCode = requisitionItem.ItmCode,
                                ItemName = requisitionItem.ItemName,
                                Name = requisitionItem.Name,
                                ModelNo = requisitionItem.ModelNo,
                                PartNo = requisitionItem.PartNo,
                                Quantity = requisitionItem.Quantity,

                                ItemSpecification = requisitionItem.ItemSpecification,
                                UoM = requisitionItem.UoM,
                                ItemType = requisitionItem.ItemType,

                                CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                                DateCreated = DateTime.Today,
                                ModifiedBy = User.Identity.Name,
                                LastModified = DateTime.Today
                            };
                            db.ProformaItems.Add(proformaItem);
                            db.SaveChanges();


                            #endregion
                        }
                    }

                    #endregion

                    #endregion
                }
            }
        }

        private void TransferPurchaseToTender(PurchaseRequisitionAssignment assignment)
        {
            var purchaseReq = db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);

            bool transferrable = false;


            if (purchaseReq != null)
            {
                #region check if there is anything to transfer
                foreach (var requestedItem in assignment.PurchaseRequisitionReassignmentItems)
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
                    int sr = 0;
                    var pr = db.PurchaseRequisitions.Find(purchaseReq.PurchaseRequisitionID);
                    if (pr != null)
                        sr = pr.StoreRequisitionID;


                    #region create tender

                    #region transfer to tender

                    #region create tender

                    var tender = new Tender
                    {

                        ProcurementOfficer =purchaseReq.ProcurementOfficer,
                        BusinessUnitId=purchaseReq.BusinessUnitId,
                        BranchId = EmployeeHelper.GetEmployeePlacementBranch(assignment.ProcurementOfficer),
                        TenderReference = "NA",
                        Description = "NA",
                        FloatDate = DateTime.Today,
                        ClosingDate = DateTime.Today,
                        OpenigDate = DateTime.Today,
                        TechnicalOpenigDate =DateTime.Today,
                        FinancialOpenigDate = DateTime.Today,

                        ProcurementCategory =ProcurementCategory.Goods,
                        PurchaseCategory = assignment.PurchaseCategory,
                        TenderType = assignment.TenderType,


                        Status = "Editing",
                        Remark = purchaseReq.Remark,

                        DateCreated = DateTime.Now,
                         CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                    };
                    db.Tenders.Add(tender);
                    db.SaveChanges();

                    #endregion

                    #region create lot

                    var lot = new Lot
                    {
                        Tender = tender,
                        LotNumber = 1,
                        Description = "NA",
                        DateCreated = DateTime.Now,
                         CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                    };
                    db.Lots.Add(lot);
                    db.SaveChanges();

                    #endregion

                    #region create lot purchase requisition

                    var lotpr = new LotPurchaseRequisition
                    {
                        Lot = lot,
                        StoreRequisitionID = sr,
                        PurchaseRequisitionID = purchaseReq.PurchaseRequisitionID,
                        DateCreated = DateTime.Now,
                         CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                    };
                    db.LotPurchaseRequisitions.Add(lotpr);
                    db.SaveChanges();

                    #endregion

                    foreach (var requisitionItem in assignment.PurchaseRequisitionReassignmentItems)
                    {
                        if (requisitionItem.Quantity > 0)
                        {
                            #region item
                            var lotitem = new LotItems
                            {
                                Lot = lot,
                                CatCode = requisitionItem.CatCode,
                                CategoryName = requisitionItem.CategoryName,
                                ItmCode = requisitionItem.ItmCode,
                                ItemName = requisitionItem.ItemName,
                                Name = requisitionItem.Name,
                                ModelNo = requisitionItem.ModelNo,
                                PartNo = requisitionItem.PartNo,
                                Quantity = requisitionItem.Quantity,

                                ItemSpecification = requisitionItem.ItemSpecification,
                                UoM = requisitionItem.UoM,
                                ItemType = requisitionItem.ItemType,

                                 CreatedBy = UserInformationHelper.GetEmployeeUserId(assignment.ProcurementOfficer),
                                DateCreated = DateTime.Today,
                               
                            };
                            db.LotItems.Add(lotitem);
                            db.SaveChanges();
                            

                            #endregion
                        }
                    }

                    #endregion

                    #endregion
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
