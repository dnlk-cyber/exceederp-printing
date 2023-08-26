using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Finance.Models;
using ExceedERP.Web.Areas.Procurement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    
    public class PurchaseOrderController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public ActionResult UpdatePurchaseOrderStatus()
        {
            var purchaseOrders = _db.PurchaseOrders.Where(po => po.IsOnlineApproved && po.IsVoid == false).ToList();
            foreach (var item in purchaseOrders)
            {
                InventoryValidationHelper.UpdatePurchaseOrderFullFilledStatus(item);
            }

            return RedirectToAction(nameof(Index));
        }
        private void PopulateViewData()
        {
            
            var stores = _db.InventoryStores;
            var receivetype = _db.ReceiveTypes;
            var items = _db.PurchaseOrderItems;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "-" + i.ItemName,
                    ItmCode = i.ItmCode
                }).AsQueryable();

            ViewData["Items"] = selecteditems;

            var selectedcatedories = items.Select(
                i => new
                {
                    Name =i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;

            var suppliers = _db.ProcurementSuppliers;
           
            var branches = _db.Branch;
            var costCenters = _db.BranchCostCenter;
            var tax = _db.GLTaxRates;
            var purchaseRequisition = _db.PurchaseRequisitions;
            var proformaPurchases = _db.ProformaPurchases;

            var selectedproformaPurchases = proformaPurchases.Select(
              i => new
              {
                  ProformaPurchaseID = i.ProformaPurchaseID,
                  Remark = i.ProformaPurchaseID + "-" + i.ProformaReference,
              })
              .AsQueryable();

            ViewData["proformaPurchases"] = selectedproformaPurchases;
            ViewData["purchaseRequisition"] = purchaseRequisition;
            ViewData["tax"] = tax;
            ViewData["suppliers"] = suppliers.Select(c => new
            {
                SupplierId = c.SupplierId,
                Name = c.Name
            }).AsQueryable();
            ViewData["costCenters"] = costCenters;
            ViewData["Branches"] = branches;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;


            ViewData["stores"] = stores;
            ViewData["ReeceiveType"] = receivetype;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

        }
    
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PurchaseOrders_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<PurchaseOrder> purchaseorders = _db.PurchaseOrders.Where(x => x.IsManual == false); 

            if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementTenderApprover))
            {

            }
            else if (System.Web.HttpContext.Current.User.IsInRole(ProcurementRoles.ProcurementTenderUser))
            {
                int empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
                purchaseorders = purchaseorders.Where(x => x.ProcurementOfficer == empid && empid != 0);
            }

            DataSourceResult result = purchaseorders.ToDataSourceResult(request, purchaseOrder => new {
                PurchaseOrderID = purchaseOrder.PurchaseOrderID,
                ProcurementOfficer = purchaseOrder.ProcurementOfficer,
                BranchId = purchaseOrder.BranchId,
                BusinessUnitId = purchaseOrder.BusinessUnitId,
                SupplierId = purchaseOrder.SupplierId,
                IsNewSupplier = purchaseOrder.IsNewSupplier,
                SupplierName = purchaseOrder.SupplierName,
                POType = purchaseOrder.POType,
                ProcurementMethod = purchaseOrder.ProcurementMethod,
                PurchaseRequisitionID = purchaseOrder.PurchaseRequisitionID,
                ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                TenderID = purchaseOrder.TenderID,
                BidAwardedSupplierID = purchaseOrder.BidAwardedSupplierID,
                OrgStructureId = purchaseOrder.OrgStructureId,
                OrderDate = purchaseOrder.OrderDate,
                DeliveryDate = purchaseOrder.DeliveryDate,
                DeliveryPlace = purchaseOrder.DeliveryPlace,
                PurchasePaymentType = purchaseOrder.PurchasePaymentType,

                Status = purchaseOrder.Status,
                IsTransferredToPayment = purchaseOrder.IsTransferredToPayment,
                IsTransferredToFollowUp = purchaseOrder.IsTransferredToFollowUp,
                Remark = purchaseOrder.Remark,
                PurchaseStatus = purchaseOrder.PurchaseStatus,

                IsFullfilled = purchaseOrder.IsFullfilled,
                IsPaid = purchaseOrder.IsPaid,

              
                ForwardedFrom = purchaseOrder.ForwardedFrom,
                ForwardedTo = purchaseOrder.ForwardedTo,
                IsFinalApproval = purchaseOrder.IsFinalApproval,
                CurrentApprovalStep = purchaseOrder.CurrentApprovalStep,

                IsSendForApproval = purchaseOrder.IsSendForApproval,
                SendForApprovalBy = purchaseOrder.SendForApprovalBy,
                SendForApprovalTime = purchaseOrder.SendForApprovalTime,

                IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,

                IsVoid = purchaseOrder.IsVoid,
                VoidBy = purchaseOrder.VoidBy,
                VoidTime = purchaseOrder.VoidTime,
                 
                DateCreated = purchaseOrder.DateCreated,
                LastModified = purchaseOrder.LastModified,
                CreatedBy = purchaseOrder.CreatedBy,
                ModifiedBy = purchaseOrder.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrders_Create([DataSourceRequest] DataSourceRequest request,PurchaseOrder purchaseOrder)
        {


            #region  validation

            if (purchaseOrder.OrderDate > purchaseOrder.DeliveryDate)
                ModelState.AddModelError("orderdate", "Order Date shoul be before delivery date.");

            if (purchaseOrder.ProformaPurchaseID == 0 || purchaseOrder.ProformaPurchaseID == null)
                ModelState.AddModelError("choose", "Please select proforma number.");

            if (!purchaseOrder.IsNewSupplier)
            {
                if (String.IsNullOrEmpty(purchaseOrder.SupplierId))
                {
                    ModelState.AddModelError("supplierid", "Please select supplier.");
                }
                else
                {
                    var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == purchaseOrder.SupplierId);
                    if (supplier != null) purchaseOrder.SupplierName = supplier.Name;
                }
            }

            else
            {
                if (String.IsNullOrEmpty(purchaseOrder.SupplierName))
                {
                    ModelState.AddModelError("suppliername", "Please enter supplier name.");
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                var proforma = _db.ProformaPurchases.Find(purchaseOrder.ProformaPurchaseID);
                if (proforma != null)
                {
                    int empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
                    var entity = new PurchaseOrder
                    {
                        BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                        BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),
                        ProcurementOfficer = UserInformationHelper.GetUserEmployeeId(User.Identity.Name),
                        SupplierId = purchaseOrder.SupplierId,
                        IsNewSupplier = purchaseOrder.IsNewSupplier,
                        SupplierName = purchaseOrder.SupplierName,
                        POType = purchaseOrder.POType,
                        ProcurementMethod = purchaseOrder.ProcurementMethod,
                        PurchaseRequisitionID =proforma.PurchaseRequisitionID,
                        ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                        TenderID = purchaseOrder.TenderID,
                        BidAwardedSupplierID = purchaseOrder.BidAwardedSupplierID,
                        OrderDate = purchaseOrder.OrderDate.Date,
                        DeliveryDate = purchaseOrder.DeliveryDate.Date,
                        DeliveryPlace = purchaseOrder.DeliveryPlace,
                        PurchasePaymentType = purchaseOrder.PurchasePaymentType,
                        //OrgStructureId = proforma.PurchaseRequisition.OrgStructureId,
                        IsTransferredToPayment = purchaseOrder.IsTransferredToPayment,
                        IsTransferredToFollowUp = purchaseOrder.IsTransferredToFollowUp,
                        Status = purchaseOrder.Status,
                        Remark = purchaseOrder.Remark,
                        PurchaseStatus = purchaseOrder.PurchaseStatus,

                        IsFullfilled = purchaseOrder.IsFullfilled,
                        IsPaid = purchaseOrder.IsPaid,

                        ForwardedFrom = User.Identity.Name,
                        ForwardedTo = User.Identity.Name,
                        IsFinalApproval = purchaseOrder.IsFinalApproval,
                        CurrentApprovalStep = purchaseOrder.CurrentApprovalStep,

                        IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                        OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                        OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,
                        IsVoid = purchaseOrder.IsVoid,
                        VoidBy = purchaseOrder.VoidBy,
                        VoidTime = purchaseOrder.VoidTime,

                        DateCreated = purchaseOrder.DateCreated,
                        LastModified = purchaseOrder.LastModified,
                        CreatedBy = purchaseOrder.CreatedBy,
                        ModifiedBy = purchaseOrder.ModifiedBy
                    };

                    _db.PurchaseOrders.Add(entity);
                    _db.SaveChanges();
                    purchaseOrder.PurchaseOrderID = entity.PurchaseOrderID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "purchase order", "Success", User.Identity.Name, $"created po {entity.PurchaseOrderID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] {purchaseOrder}.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrders_Update([DataSourceRequest]DataSourceRequest request, PurchaseOrder purchaseOrder)
        {
            #region  validation

            if (purchaseOrder.OrderDate > purchaseOrder.DeliveryDate)
                ModelState.AddModelError("orderdate", "Order Date shoul be before delivery date.");

            if (purchaseOrder.ProformaPurchaseID == 0 || purchaseOrder.ProformaPurchaseID == null)
                ModelState.AddModelError("choose", "Please select proforma number.");

            if (!purchaseOrder.IsNewSupplier)
            {
                if (String.IsNullOrEmpty(purchaseOrder.SupplierId))
                {
                    ModelState.AddModelError("supplierid", "Please select supplier.");
                }
                else
                {
                    var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == purchaseOrder.SupplierId);
                    if (supplier != null) purchaseOrder.SupplierName = supplier.Name;
                }
            }

            else
            {
                if (String.IsNullOrEmpty(purchaseOrder.SupplierName))
                {
                    ModelState.AddModelError("suppliername", "Please enter supplier name.");
                }
            }

            #endregion
            if (ModelState.IsValid)
            {

              
                var entity = new PurchaseOrder
                    {
                        ProcurementOfficer = purchaseOrder.ProcurementOfficer,
                        BranchId = purchaseOrder.BranchId,
                        BusinessUnitId = purchaseOrder.BusinessUnitId,
                        PurchaseOrderID = purchaseOrder.PurchaseOrderID,
                        SupplierId = purchaseOrder.SupplierId,
                        IsNewSupplier = purchaseOrder.IsNewSupplier,
                        SupplierName = purchaseOrder.SupplierName,
                        POType = purchaseOrder.POType,
                        ProcurementMethod = purchaseOrder.ProcurementMethod,
                        PurchaseRequisitionID = purchaseOrder.PurchaseRequisitionID,
                        ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                    TenderID = purchaseOrder.TenderID,
                    BidAwardedSupplierID = purchaseOrder.BidAwardedSupplierID,
                    PurchasePaymentType = purchaseOrder.PurchasePaymentType,
                        OrderDate = purchaseOrder.OrderDate,
                        DeliveryDate = purchaseOrder.DeliveryDate,
                        DeliveryPlace = purchaseOrder.DeliveryPlace,
                        OrgStructureId = purchaseOrder.OrgStructureId,
                        Status = purchaseOrder.Status,
                        IsTransferredToPayment = purchaseOrder.IsTransferredToPayment,
                        IsTransferredToFollowUp = purchaseOrder.IsTransferredToFollowUp,
                        Remark = purchaseOrder.Remark,
                        PurchaseStatus = purchaseOrder.PurchaseStatus,

                        IsFullfilled = purchaseOrder.IsFullfilled,
                        IsPaid = purchaseOrder.IsPaid,

                        ForwardedFrom = purchaseOrder.ForwardedFrom,
                        ForwardedTo = purchaseOrder.ForwardedTo,
                        IsFinalApproval = purchaseOrder.IsFinalApproval,
                        CurrentApprovalStep = purchaseOrder.CurrentApprovalStep,


                    IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                        OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                        OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,
                        IsVoid = purchaseOrder.IsVoid,
                        VoidBy = purchaseOrder.VoidBy,
                        VoidTime = purchaseOrder.VoidTime,

                        DateCreated = purchaseOrder.DateCreated,
                        LastModified = purchaseOrder.LastModified,
                        CreatedBy = purchaseOrder.CreatedBy,
                        ModifiedBy = purchaseOrder.ModifiedBy
                    };

                    _db.PurchaseOrders.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order", "Success", User.Identity.Name, $"updated po {entity.PurchaseOrderID} ", Modules.Procurement);
                #endregion
            }




            return Json(new[] { purchaseOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrders_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseOrder
                {
                    ProcurementOfficer = purchaseOrder.ProcurementOfficer,
                    BranchId = purchaseOrder.BranchId,
                    BusinessUnitId = purchaseOrder.BusinessUnitId,
                    PurchaseOrderID = purchaseOrder.PurchaseOrderID,
                    SupplierId = purchaseOrder.SupplierId,
                    IsNewSupplier = purchaseOrder.IsNewSupplier,
                    SupplierName = purchaseOrder.SupplierName,
                    POType = purchaseOrder.POType,
                    ProcurementMethod = purchaseOrder.ProcurementMethod,
                    PurchaseRequisitionID = purchaseOrder.PurchaseRequisitionID,
                    ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                    TenderID = purchaseOrder.TenderID,
                    BidAwardedSupplierID = purchaseOrder.BidAwardedSupplierID,
                    OrderDate = purchaseOrder.OrderDate,
                    DeliveryDate = purchaseOrder.DeliveryDate,
                    DeliveryPlace = purchaseOrder.DeliveryPlace,
                    OrgStructureId=purchaseOrder.OrgStructureId,
                    Status = purchaseOrder.Status,
                    Remark = purchaseOrder.Remark,

                    ForwardedFrom = purchaseOrder.ForwardedFrom,
                    ForwardedTo = purchaseOrder.ForwardedTo,
                    IsFinalApproval = purchaseOrder.IsFinalApproval,

                    IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                    OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                    OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,
                    IsVoid = purchaseOrder.IsVoid,
                    VoidBy = purchaseOrder.VoidBy,
                    VoidTime = purchaseOrder.VoidTime,
                   
                    DateCreated = purchaseOrder.DateCreated,
                    LastModified = purchaseOrder.LastModified,
                    CreatedBy = purchaseOrder.CreatedBy,
                    ModifiedBy = purchaseOrder.ModifiedBy
                };

                _db.PurchaseOrders.Attach(entity);
                _db.PurchaseOrders.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { purchaseOrder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.PurchaseOrders.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.PurchaseOrderItem.Count > 0)
            {
                #region validate price and tax

                foreach (var item in record.PurchaseOrderItem)
                {
                    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill unit price and tax information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.PurchaseOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow

                bool isNewRecord = true;
                var validations = record.PurchaseOrderValidation.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.PurchaseOrderItem.Sum(x => x.Total);
                int docid = record.PurchaseOrderID;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.PurchaseOrder, isNewRecord,step);
                #endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.PurchaseOrderItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult TransferToPayment(int id)
        {
            bool transferred = false;
            var record = _db.PurchaseOrders.Find(id);
            var paymentSchedule = _db.PoPaymentSchedules.Where(x => x.PurchaseOrderID == id);
            decimal amount = 0;
            var items = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id).ToList();
            if (items.Any())
                amount = items.Sum(x => x.Total);

            if (paymentSchedule == null)
            {
                var formattedData = new
                {
                    Message = "There is no payment schedule defined.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (paymentSchedule.Sum(x => x.Percentage) <= 0)
            {
                var formattedData = new
                {
                    Message = "Advance percentage should be more than zero.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsTransferredToPayment && record.IsOnlineApproved && !record.IsVoid && paymentSchedule.Any() && amount > 0)
            {
                decimal sumAdvancePercent = 0;
                decimal sumPaymentPercent = 0;
                var advances = paymentSchedule.Where(x => x.PaymentType == PaymentType.Advance).ToList();
                var payments = paymentSchedule.Where(x => x.PaymentType == PaymentType.Payment).ToList();
                if (advances.Any())
                {
                    sumAdvancePercent = advances.Sum(x => x.Percentage);
                    if (sumAdvancePercent > 0)
                    {
                        #region create advance payment
                        var entity = new ApPayment
                        {
                            IsPrepayment = true,
                            SupplierName = record.SupplierName,
                            SupplierId = record.SupplierId,
                            InvoiceNumber = "",
                            InvoiceAmount = 0,
                            PaymentAmount = 0,
                            RemainingBalance = 0,
                            HasPrepayment = false,
                            PrepaymentAmount = amount * (sumAdvancePercent / 100),
                            NetAmount = amount * (sumAdvancePercent / 100),
                            PaymentMethod = "Na",
                            PaymentReference = "NA",
                            GLTaxRateID = 0,
                            PaymentDate = DateTime.Today,
                            PoTypeEnum = PurchaseType.PurchaseOrder,
                            PoType = "Purchase Order",
                            PurchaseReference = record.PurchaseOrderID.ToString(),
                            CollectedByPurchaser = true,
                            EmployeeId = record.ProcurementOfficer,
                            Remark = record.Remark,
                            OrgBranchName = record.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = record.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = record.ModifiedBy
                        };
                        _db.APPayments.Add(entity);
                        //_db.SaveChanges();


                        #endregion
                        //record.Status = Enum.GetName(typeof(OperationStatuses), 14);
                        //record.IsTransferredToPayment = true;
                        //_db.PurchaseOrders.Attach(record);
                        //_db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();
                        //var formattedData1 = new
                        //{
                        //    Message = "Successfully Sent for Payment.",
                        //    Status = "OK"
                        //};
                        //return Json(formattedData1, JsonRequestBehavior.AllowGet);
                        transferred = true;
                    }
                }
                if (payments.Any())
                {
                    sumPaymentPercent = payments.Sum(x => x.Percentage);
                    if (sumPaymentPercent > 0)
                    {
                        #region create payment request
                        var entity = new PurchasePayment
                        {
                            Date = DateTime.Today,
                            SupplierId = record.SupplierId,
                            SupplierName = record.SupplierName,
                            SupplierInvoice = "",
                            WithOutGRN = false,
                            PaymentAmount = amount,
                            ApplyPenality =false,
                            PenalityAmount = 0,
                            NetPayable = amount,
                            
                            Type = "Purchase Order",
                            PaymentType = PaymentType.Payment,                            
                            PoOrContractNumber =record.PurchaseOrderID.ToString(),
                            PurposeOfPayment = "Supplier Payment",
                            ProcurementOfficer = record.ProcurementOfficer,
                            BranchId = record.BranchId,
                            BusinessUnitId = record.BusinessUnitId,                      

                          
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        };

                        _db.PurchasePayments.Add(entity);
                        //_db.SaveChanges();
                        #endregion
                        //#region create payment
                        //var entity = new ApPayment
                        //{
                        //    IsPrepayment = false,
                        //    SupplierName = record.SupplierName,
                        //    SupplierId = record.SupplierId,
                        //    InvoiceNumber = "",
                        //    InvoiceAmount = 0,
                        //    PaymentAmount = amount * (sumPaymentPercent / 100),
                        //    RemainingBalance = 0,
                        //    HasPrepayment = false,
                        //    PrepaymentAmount = 0,
                        //    NetAmount = amount * (sumPaymentPercent / 100),
                        //    PaymentMethod = "Na",
                        //    PaymentReference = "NA",
                        //    GLTaxRateID = 0,
                        //    PaymentDate = DateTime.Today,
                        //    PoTypeEnum = PurchaseType.PurchaseOrder,
                        //    PoType = "Purchase Order",
                        //    PurchaseReference = record.PurchaseOrderID.ToString(),
                        //    CollectedByPurchaser = true,
                        //    EmployeeId = record.ProcurementOfficer,
                        //    Remark = record.Remark,
                        //    OrgBranchName = record.OrgBranchName,
                        //    DateCreated = DateTime.Now,
                        //    LastModified = record.LastModified,
                        //    CreatedBy = User.Identity.Name,
                        //    ModifiedBy = record.ModifiedBy
                        //};
                        //_db.APPayments.Add(entity);
                        //// _db.SaveChanges();

                        //#endregion
                        //record.Status = Enum.GetName(typeof(OperationStatuses), 14);
                        //record.IsTransferredToPayment = true;
                        //_db.PurchaseOrders.Attach(record);
                        //_db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();

                        //var formattedData1 = new
                        //{
                        //    Message = "Successfully Sent for Payment.",
                        //    Status = "OK"
                        //};
                        //return Json(formattedData1, JsonRequestBehavior.AllowGet);
                        transferred = true;
                    }
                }

                if (transferred)
                {
                    record.Status = Enum.GetName(typeof(OperationStatuses), 14);
                    record.IsTransferredToPayment = true;
                    _db.PurchaseOrders.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();

                    var formattedData1 = new
                    {
                        Message = "Successfully Sent for Payment.",
                        Status = "OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There is no payment to be transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }

            if (amount == 0)
            {
                var formattedData = new
                {
                    Message = "Total amount is 0.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!paymentSchedule.Any())
            {
                var formattedData = new
                {
                    Message = "Please define payment shcedule.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is void.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsTransferredToPayment)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for payment you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult TransferToForeignFollowup(int id)
        {
            var record = _db.PurchaseOrders.Find(id);
            var items = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id).ToList();
            
            if (record != null)
            {
                

                if (!items.Any())
                {
                    var formattedData = new
                    {
                        Message = "There is no purchase order items defined.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                //if (record.PurchaseCategory != PurchaseCategory.ForeignPurchase)
                //{
                //    var formattedData = new
                //    {
                //        Message = "There tender is not foreign purchase.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData, JsonRequestBehavior.AllowGet);
                //}


                if (!record.IsTransferredToFollowUp && record.IsOnlineApproved && !record.IsVoid)
                {
                    #region create foreign followup header
                    var foreignPurchase = new ForeignPurchase
                    {
                        TenderRef = record.PurchaseOrderID.ToString(),
                        PurchaseRef = record.PurchaseOrderID,
                        Supplier = record.SupplierName,
                        ItemDesc = "",
                        OrgStructureId = 0,
                        Quantity = 0,
                        ProcurementMethod = "Purchase Order",
                        FOBMethod = FOBMethod.NationalBank,
                        ApprovalDate = null,
                        OpenigDate = record.OrderDate,
                        TechnicalDocSendDate = null,
                        FinancialOpenigDate = record.OrderDate,
                        MinuteApprovedDate = null,
                        AwardDate = record.OrderDate,
                        ContractDate = record.OrderDate,
                        PaymentMethod = ForeignPaymentMethod.LC,
                        PaymentOpenDate = null,
                        PaymentExtensionDate = null,
                        DocReceivedFromBankDate = null,
                        ClearanceStartDate = null,
                        TransitProcessDate = null,
                        DeliveryDate = record.DeliveryDate,
                        DeliveryNo = "",
                        UnitPrice = 0,
                        ForeignCurrencyProcessId = 0,
                        Remark = record.Remark,
                        OrgBranchName = record.OrgBranchName,
                        DateCreated = record.DateCreated,
                        LastModified = record.LastModified,
                        CreatedBy = record.CreatedBy,
                        ModifiedBy = record.ModifiedBy
                    };
                    _db.ForeignPurchases.Add(foreignPurchase);
                    _db.SaveChanges();
                    #endregion

                    foreach (var item in items)
                    {
                        #region create foreign followup items
                        var followupitem = new ForeignPurchaseFollowUpItems
                        {
                            ForeignPurchase = foreignPurchase,
                            CatCode = item.CatCode,
                            CategoryName = item.CategoryName,
                            ItmCode = item.ItmCode,
                            ItemName = item.ItemName,
                            ItemType = item.ItemType,
                            PartNo = item.PartNo,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            GLTaxRateID = item.GLTaxRateID,
                            TaxAmount = item.TaxAmount,
                            SubTotal = item.UnitPrice * item.Quantity,
                            AdditionalUnitCost = 0,
                            FinalUnitPrice = item.UnitPrice,
                            Total = item.Total,
                            ItemSpecification = item.ItemSpecification,
                            UoM = item.UoM
                        };

                        _db.ForeignPurchaseFollowUpItems.Add(followupitem);
                        _db.SaveChanges();
                        #endregion
                    }

                    record.Status = Enum.GetName(typeof(OperationStatuses), 14);
                    record.IsTransferredToFollowUp = true;
                    _db.PurchaseOrders.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully Sent for Followup.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is void.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsTransferredToPayment)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for payment you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.PurchaseOrders.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                var poreceivetypes = _db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.PurchaseOrder)
                    .Select(x => x.ReceiveTypeId).ToList();
                #region validate payment

                var payment = _db.PurchasePayments
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.Type == "Purchase Order")
                    .FirstOrDefault(x => x.PoOrContractNumber == record.PurchaseOrderID.ToString());

                if (payment != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This purchase order can not be void since it is referred in payment {payment.PurchasePaymentId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate grn

                var goodReceive = _db.GoodReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.PurchaseOrderID.ToString());

                if (goodReceive!=null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This purchase order can not be void since it is referred in good receive {goodReceive.GoodReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate service grn

                var serviceReceive = _db.ServiceReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.PurchaseOrderID.ToString());

                if (serviceReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This purchase order can not be void since it is referred in service receive {serviceReceive.ServiceReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate fixed grn

                var fixedAssetReceive = _db.FixedAssetReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.PurchaseOrderID.ToString());

                if (fixedAssetReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This purchase order can not be void since it is referred in fixed asset receive {fixedAssetReceive.FixedAssetReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region remove related validations

                _db.PurchaseOrderValidations.RemoveRange(record.PurchaseOrderValidation);
                _db.SaveChanges();

                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                //record.isVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.ForwardedFrom = UserInformationHelper.GetEmployeeUserId(record.ProcurementOfficer);
                record.ForwardedTo = UserInformationHelper.GetEmployeeUserId(record.ProcurementOfficer);
                _db.PurchaseOrders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        public ActionResult GrnHeader_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<GrnViewModel> purchases = new List<GrnViewModel>();

            var receiveType = _db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.PurchaseOrder);
            #region grn stock header
            var stockReceives = _db.GoodReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == id.ToString());

            foreach (var goodReceive in stockReceives)
            {
                #region grn header
                GrnViewModel record = new GrnViewModel
                {
                    GrnId = goodReceive.GoodReceiveId,
                    GrnType = GrnType.Stock,
                    IsManual = false,
                    ReceiveRef = goodReceive.ReceiveRef,
                    ReceiveTypeId = goodReceive.ReceiveTypeId,
                    StoreCode = goodReceive.StoreCode,
                    SupplierName = goodReceive.SupplierName,
                    SupplierId = goodReceive.SupplierId,
                    SupplierInvoiceNo = goodReceive.SupplierInvoiceNo,
                    TenderLot = goodReceive.TenderLot,
                    Reference = goodReceive.Reference,
                    ReceiveDate = goodReceive.ReceiveDate,
                    Status = goodReceive.Status,
                    Remark = goodReceive.Remark,
                    GLPeriodId = goodReceive.GLPeriodId,

                    IsSendForApproval = goodReceive.IsSendForApproval,
                    SendForApprovalBy = goodReceive.SendForApprovalBy,
                    SendForApprovalTime = goodReceive.SendForApprovalTime,

                    IsOnlineApproved = goodReceive.IsOnlineApproved,
                    OnlineApprovedBy = goodReceive.OnlineApprovedBy,
                    OnlineApprovedTime = goodReceive.OnlineApprovedTime,

                    IsVoid = goodReceive.IsVoid,
                    VoidBy = goodReceive.VoidBy,
                    VoidTime = goodReceive.VoidTime,

                    DateCreated = goodReceive.DateCreated,
                    LastModified = goodReceive.LastModified,
                    CreatedBy = goodReceive.CreatedBy,
                    ModifiedBy = goodReceive.ModifiedBy
                };
                purchases.Add(record);
                #endregion
            }
            #endregion
            #region grn service header
            var serviceReceives = _db.ServiceReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == id.ToString());

            foreach (var serviceReceive in serviceReceives)
            {
                #region grn service header
                GrnViewModel record = new GrnViewModel
                {
                    GrnId = serviceReceive.ServiceReceiveId,
                    GrnType = GrnType.Stock,
                    IsManual = false,
                    ReceiveRef = serviceReceive.ReceiveRef,
                    ReceiveTypeId = serviceReceive.ReceiveTypeId,
                    StoreCode = "NA",
                    SupplierName = serviceReceive.SupplierName,
                    SupplierId = serviceReceive.SupplierId,
                    SupplierInvoiceNo = serviceReceive.SupplierInvoiceNo,
                    TenderLot = serviceReceive.TenderLot,
                    Reference = serviceReceive.Reference,
                    ReceiveDate = serviceReceive.ReceiveDate,
                    Status = serviceReceive.Status,
                    Remark = serviceReceive.Remark,
                    GLPeriodId = serviceReceive.GLPeriodId,

                    IsSendForApproval = serviceReceive.IsSendForApproval,
                    SendForApprovalBy = serviceReceive.SendForApprovalBy,
                    SendForApprovalTime = serviceReceive.SendForApprovalTime,

                    IsOnlineApproved = serviceReceive.IsOnlineApproved,
                    OnlineApprovedBy = serviceReceive.OnlineApprovedBy,
                    OnlineApprovedTime = serviceReceive.OnlineApprovedTime,

                    IsVoid = serviceReceive.IsVoid,
                    VoidBy = serviceReceive.VoidBy,
                    VoidTime = serviceReceive.VoidTime,

                    DateCreated = serviceReceive.DateCreated,
                    LastModified = serviceReceive.LastModified,
                    CreatedBy = serviceReceive.CreatedBy,
                    ModifiedBy = serviceReceive.ModifiedBy
                };
                purchases.Add(record);
                #endregion
            }
            #endregion
            #region grn fixed asset header

            var fixedAssetReceives = _db.FixedAssetReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == id.ToString());

            foreach (var fixedAssetReceive in fixedAssetReceives)
            {
                #region grn fixed asset header
                GrnViewModel record = new GrnViewModel
                {
                    GrnId = fixedAssetReceive.FixedAssetReceiveId,
                    GrnType = GrnType.Stock,
                    IsManual = false,
                    ReceiveRef = fixedAssetReceive.ReceiveRef,
                    ReceiveTypeId = fixedAssetReceive.ReceiveTypeId,
                    StoreCode = fixedAssetReceive.StoreCode,
                    SupplierName = fixedAssetReceive.SupplierName,
                    SupplierId = fixedAssetReceive.SupplierId,
                    SupplierInvoiceNo = fixedAssetReceive.SupplierInvoiceNo,
                    TenderLot = fixedAssetReceive.TenderLot,
                    Reference = fixedAssetReceive.Reference,
                    ReceiveDate = fixedAssetReceive.ReceiveDate,
                    Status = fixedAssetReceive.Status,
                    Remark = fixedAssetReceive.Remark,
                    GLPeriodId = fixedAssetReceive.GLPeriodId,

                    IsSendForApproval = fixedAssetReceive.IsSendForApproval,
                    SendForApprovalBy = fixedAssetReceive.SendForApprovalBy,
                    SendForApprovalTime = fixedAssetReceive.SendForApprovalTime,

                    IsOnlineApproved = fixedAssetReceive.IsOnlineApproved,
                    OnlineApprovedBy = fixedAssetReceive.OnlineApprovedBy,
                    OnlineApprovedTime = fixedAssetReceive.OnlineApprovedTime,

                    IsVoid = fixedAssetReceive.IsVoid,
                    VoidBy = fixedAssetReceive.VoidBy,
                    VoidTime = fixedAssetReceive.VoidTime,

                    DateCreated = fixedAssetReceive.DateCreated,
                    LastModified = fixedAssetReceive.LastModified,
                    CreatedBy = fixedAssetReceive.CreatedBy,
                    ModifiedBy = fixedAssetReceive.ModifiedBy
                };
                purchases.Add(record);
                #endregion
            }
            #endregion

            IQueryable<GrnViewModel> grnViewModels = purchases.AsQueryable();
            DataSourceResult result = grnViewModels.ToDataSourceResult(request, grnViewModel => new
            {
                ReceiveRef = grnViewModel.ReceiveRef,
                GrnType = grnViewModel.GrnType,
                IsManual = grnViewModel.IsManual,
                BranchId = grnViewModel.BranchId,
                ReceiveTypeId = grnViewModel.ReceiveTypeId,
                GrnId = grnViewModel.GrnId,
                TenderLot = grnViewModel.TenderLot,
                StoreCode = grnViewModel.StoreCode,
                SupplierName = grnViewModel.SupplierName,
                SupplierInvoiceNo = grnViewModel.SupplierInvoiceNo,
                Reference = grnViewModel.Reference,
                ReceiveDate = grnViewModel.ReceiveDate,
                Status = grnViewModel.Status,
                Remark = grnViewModel.Remark,

                IsSendForApproval = grnViewModel.IsSendForApproval,
                SendForApprovalBy = grnViewModel.SendForApprovalBy,
                SendForApprovalTime = grnViewModel.SendForApprovalTime,

                IsOnlineApproved = grnViewModel.IsOnlineApproved,
                OnlineApprovedBy = grnViewModel.OnlineApprovedBy,
                OnlineApprovedTime = grnViewModel.OnlineApprovedTime,

                IsVoid = grnViewModel.IsVoid,
                VoidBy = grnViewModel.VoidBy,
                VoidTime = grnViewModel.VoidTime,

                DateCreated = grnViewModel.DateCreated,
                LastModified = grnViewModel.LastModified,
                CreatedBy = grnViewModel.CreatedBy,
                ModifiedBy = grnViewModel.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult GrnItem_Read([DataSourceRequest]DataSourceRequest request, int id, GrnType type)
        {
            List<GrnViewModelItem> viewModelItems = new List<GrnViewModelItem>();

            if (type == GrnType.Stock)
            {
                #region stock

                var goodReceiveItems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == id).ToList();
                foreach (var goodReceiveItem in goodReceiveItems)
                {

                    GrnViewModelItem itemvm = new GrnViewModelItem
                    {
                        GrnViewModelItemID = goodReceiveItem.GoodReceiveItemID,
                        Category = goodReceiveItem.ItemCategory.Name,
                        SubCategory ="",
                        Item=goodReceiveItem.InventoryItem.Name,
                        Quantity = goodReceiveItem.Quantity,
                        UnitPrice = goodReceiveItem.UnitPrice,
                        SubTotal = goodReceiveItem.SubTotal,
                        Total = goodReceiveItem.Total,
                        GLTaxRateID = goodReceiveItem.GLTaxRateID,
                        TaxAmount = goodReceiveItem.TaxAmount,
                        IsExtra = goodReceiveItem.IsExtra,
                        ExtraQuantity = goodReceiveItem.ExtraQuantity,
                        ExtraAmount = goodReceiveItem.ExtraAmount,
                        WarrantyInformation = goodReceiveItem.WarrantyInformation,
                        Specification = goodReceiveItem.Specification,
                        Expire = goodReceiveItem.Expire,
                        ExpireDate = goodReceiveItem.ExpireDate,
                        Balance = goodReceiveItem.Balance,
                        OrgBranchName = goodReceiveItem.OrgBranchName,
                    };
                    viewModelItems.Add(itemvm);
                }

                #endregion

            }
            else if (type == GrnType.Service)
            {
                #region service

                var serviceReceiveItems = _db.ServiceReceiveItems.Where(x => x.ServiceReceiveId == id).ToList();
                foreach (var item in serviceReceiveItems)
                {
                    var serviceCategory = _db.ServiceCategories.FirstOrDefault(x => x.ServiceCategoryCode == item.ServiceCategoryCode);
                    GrnViewModelItem itemvm = new GrnViewModelItem
                    {
                        GrnViewModelItemID = item.ServiceReceiveItemId,
                        Category = serviceCategory?.Name,
                        SubCategory = "",
                        Item = item.ServiceItemName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SubTotal = item.SubTotal,
                        Total = item.Total,
                        GLTaxRateID = item.GLTaxRateID,
                        TaxAmount = item.TaxAmount,
                       
                    };
                    viewModelItems.Add(itemvm);
                }

                #endregion

            }

            else if (type == GrnType.FixedAsset)
            {
                #region stock

                var fixedAssetReceiveItems = _db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveId == id).ToList();
                foreach (var fixedAssetReceiveItem in fixedAssetReceiveItems)
                {

                    GrnViewModelItem itemvm = new GrnViewModelItem
                    {
                        GrnViewModelItemID = fixedAssetReceiveItem.FixedAssetReceiveItemID,
                        Category = fixedAssetReceiveItem.FixedAssetCategory.CategoryName,
                        SubCategory = fixedAssetReceiveItem.FixedAssetSubCategory.SubCategoryName,
                        Item = fixedAssetReceiveItem.Name,
                        Quantity = fixedAssetReceiveItem.Quantity,
                        UnitPrice = fixedAssetReceiveItem.UnitPrice,
                        SubTotal = fixedAssetReceiveItem.SubTotal,
                        Total = fixedAssetReceiveItem.Total,
                        GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                        TaxAmount = fixedAssetReceiveItem.TaxAmount,
                        IsExtra = fixedAssetReceiveItem.IsExtra,
                        ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                        ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                        WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                        Specification = fixedAssetReceiveItem.Specification,
                        Expire = fixedAssetReceiveItem.Expire,
                        ExpireDate = fixedAssetReceiveItem.ExpireDate,
                        Balance = fixedAssetReceiveItem.Balance,
                        OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
                    };
                    viewModelItems.Add(itemvm);
                }

                #endregion

            }

            IQueryable<GrnViewModelItem> grnViewModelItems = viewModelItems.AsQueryable();
            DataSourceResult result = grnViewModelItems.ToDataSourceResult(request, fixedAssetReceiveItem => new
            {
                GrnViewModelItemID = fixedAssetReceiveItem.GrnViewModelItemID,
                Category = fixedAssetReceiveItem.Category,
                SubCategory = fixedAssetReceiveItem.SubCategory,
                Item = fixedAssetReceiveItem.Item,
                Quantity = fixedAssetReceiveItem.Quantity,
                UnitPrice = fixedAssetReceiveItem.UnitPrice,
                SubTotal = fixedAssetReceiveItem.SubTotal,
                Total = fixedAssetReceiveItem.Total,
                GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                TaxAmount = fixedAssetReceiveItem.TaxAmount,
                IsExtra = fixedAssetReceiveItem.IsExtra,
                ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                Specification = fixedAssetReceiveItem.Specification,
                Expire = fixedAssetReceiveItem.Expire,
                ExpireDate = fixedAssetReceiveItem.ExpireDate,
                Balance = fixedAssetReceiveItem.Balance,
                OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
            });

            return Json(result);
        }
        public ActionResult GrnValidation_Read([DataSourceRequest]DataSourceRequest request, int id, GrnType type)
        {
            List<GrnViewModelValidation> grnViewModelValidations = new List<GrnViewModelValidation>();

            if (type == GrnType.Stock)
            {
                #region stock

                var goodReceiveValidations = _db.GoodReceiveValidations.Where(x => x.GoodReceiveId == id).ToList();
                foreach (var validation in goodReceiveValidations)
                {

                    GrnViewModelValidation record = new GrnViewModelValidation
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    grnViewModelValidations.Add(record);
                }

                #endregion
            }
            else if (type == GrnType.Service)
            {
                #region service

                var serviceReceiveValidations = _db.ServiceReceiveValidations.Where(x => x.ServiceReceiveId == id).ToList();
                foreach (var validation in serviceReceiveValidations)
                {

                    GrnViewModelValidation record = new GrnViewModelValidation
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    grnViewModelValidations.Add(record);
                }

                #endregion
            }

            else if (type == GrnType.FixedAsset)
            {
                #region service

                var fixedAssetReceiveValidations = _db.FixedAssetReceiveValidations.Where(x => x.FixedAssetReceiveValidationID == id).ToList();
                foreach (var validation in fixedAssetReceiveValidations)
                {

                    GrnViewModelValidation record = new GrnViewModelValidation
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    grnViewModelValidations.Add(record);
                }

                #endregion
            }

            IQueryable<GrnViewModelValidation> paymentScheduleViewModels = grnViewModelValidations.AsQueryable();
            DataSourceResult result = paymentScheduleViewModels.ToDataSourceResult(request, paymentScheduleViewModel => new
            {
                Status = paymentScheduleViewModel.Status,
                Remark = paymentScheduleViewModel.Remark,
                OrgBranchName = paymentScheduleViewModel.OrgBranchName,
                DateCreated = paymentScheduleViewModel.DateCreated,
                LastModified = paymentScheduleViewModel.LastModified,
                CreatedBy = paymentScheduleViewModel.CreatedBy,
                ModifiedBy = paymentScheduleViewModel.ModifiedBy
            });

            return Json(result);
        }
        
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
