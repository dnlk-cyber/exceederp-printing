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
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Procurement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{

   
    public class ContractManagementController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractManager, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var branches = _db.Branch;
            ViewData["Branches"] = branches;
            var stores = _db.InventoryStores;
            ViewData["stores"] = stores;
            var receivetype = _db.ReceiveTypes;
            ViewData["ReeceiveType"] = receivetype;
            var tax = _db.GLTaxRates;

            ViewData["tax"] = tax;

            var suppliers = _db.ProcurementSuppliers.ToList();

            ViewData["suppliers"] = suppliers.Select(c => new ProcurementSupplier
            {
                Name = c.Name

            });

            var items = _db.ContractManagementItems;
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
                    Name = i.CatCode + "-" + i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;

            var inventoryPeriods = _db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
                i => new
                {
                    Name = i.Name,
                    GLPeriodId = i.InventoryPeriodId
                }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractManager, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractManagements_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractManagement> contractmanagements = _db.ContractManagements;

            bool isContractAdmin =User.IsInRole(ProcurementRoles.ProcurementContractManager);
            bool isContractApprover =User.IsInRole(ProcurementRoles.ProcurementContractApprover);

            if (!(isContractApprover||isContractAdmin))
            {
                int empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
                contractmanagements = contractmanagements.Where(x => x.ProcurementOfficer == empid && empid != 0);
            }

            DataSourceResult result = contractmanagements.ToDataSourceResult(request, contractManagement => new {
                ContractManagementID = contractManagement.ContractManagementID,
                BranchId = contractManagement.BranchId,
                BusinessUnitId = contractManagement.BusinessUnitId,
                SupplierName = contractManagement.SupplierName,
                IsNewSupplier = contractManagement.IsNewSupplier,
                SupplierId = contractManagement.SupplierId,
                ContractRefNo = contractManagement.ContractRefNo,
                TenderRefNo = contractManagement.TenderRefNo,
                ProcurementRefNo = contractManagement.ProcurementRefNo,
                SigningDate = contractManagement.SigningDate,
                ContractType = contractManagement.ContractType,
                PurchaseCategory = contractManagement.PurchaseCategory,
                DeliveryPlace = contractManagement.DeliveryPlace,
                DeliveryDate = contractManagement.DeliveryDate,
                DeliveryType = contractManagement.DeliveryType,
                ContractAmount = contractManagement.ContractAmount,
                RetentionPercentage = contractManagement.RetentionPercentage,
                ProcurementOfficer = contractManagement.ProcurementOfficer,
                Advance = contractManagement.Advance,

                ForwardedFrom = contractManagement.ForwardedFrom,
                ForwardedTo = contractManagement.ForwardedTo,
                IsFinalApproval = contractManagement.IsFinalApproval,

                IsFullfilled = contractManagement.IsFullfilled,
                IsPaid = contractManagement.IsPaid,

                Status = contractManagement.Status,
                ContractStatus = contractManagement.ContractStatus,
                IsTransferredToPayment = contractManagement.IsTransferredToPayment,
                IsTransferredToFollowUp = contractManagement.IsTransferredToFollowUp,

                IsOnlineApproved = contractManagement.IsOnlineApproved,
                OnlineApprovedBy = contractManagement.OnlineApprovedBy,
                OnlineApprovedTime = contractManagement.OnlineApprovedTime,
                IsSendForApproval = contractManagement.IsSendForApproval,
                SendForApprovalBy = contractManagement.SendForApprovalBy,
                SendForApprovalTime = contractManagement.SendForApprovalTime,
                IsVoid = contractManagement.IsVoid,
                VoidBy = contractManagement.VoidBy,
                VoidTime = contractManagement.VoidTime,
                OrgBranchName = contractManagement.OrgBranchName,
                DateCreated = contractManagement.DateCreated,
                LastModified = contractManagement.LastModified,
                CreatedBy = contractManagement.CreatedBy,
                ModifiedBy = contractManagement.ModifiedBy
            });

            return Json(result);
        }
   
   
        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagements_Create([DataSourceRequest]DataSourceRequest request, ContractManagement contractManagement)
        {
            #region validation

            if (contractManagement.ProcurementRefNo == 0)
                ModelState.AddModelError("exist", $"Please choose procurement reference.");

            if (contractManagement.ProcurementRefNo != 0)
            {
                var exist = _db.ContractManagements.Where(x => x.ProcurementRefNo == contractManagement.ProcurementRefNo)
                    .ToList();
                if(exist.Any())
                    ModelState.AddModelError("exist",$"The award number already exisits in the system.");
            }

            #endregion
            #region data

            if (contractManagement.ProcurementRefNo!=0)
            {
                int did = contractManagement.ProcurementRefNo;
                var award = _db.BidAwardedSuppliers.Find(did);
                if (award != null)
                {
                    contractManagement.SupplierId = award.SupplierId;
                    contractManagement.IsNewSupplier = award.IsNewSupplier;
                    contractManagement.SupplierName = award.SupplierName;

                    var lot = _db.Lots.Find(award.LotID);
                    if (lot != null)
                    {
                        var tender = _db.Tenders.Find(lot.TenderID);
                        if (tender != null)
                            contractManagement.ProcurementOfficer = tender.ProcurementOfficer;
                    }
                }
            }

            #endregion
            

            if (ModelState.IsValid)
            {
                var entity = new ContractManagement
                {
                    BranchId = contractManagement.BranchId,
                    BusinessUnitId = contractManagement.BusinessUnitId,
                    SupplierName = contractManagement.SupplierName,
                    IsNewSupplier = contractManagement.IsNewSupplier,
                    SupplierId = contractManagement.SupplierId,
                    ContractRefNo = contractManagement.ContractRefNo,
                    TenderRefNo = contractManagement.TenderRefNo,
                    ProcurementRefNo = contractManagement.ProcurementRefNo,
                    SigningDate = contractManagement.SigningDate.Date,
                    ContractType = contractManagement.ContractType,
                    PurchaseCategory = contractManagement.PurchaseCategory,
                    DeliveryPlace = contractManagement.DeliveryPlace,
                    DeliveryDate = contractManagement.DeliveryDate.Date,
                    DeliveryType = contractManagement.DeliveryType,
                    ContractAmount = contractManagement.ContractAmount,
                    RetentionPercentage = contractManagement.RetentionPercentage,
                    ProcurementOfficer = contractManagement.ProcurementOfficer,
                    Advance = contractManagement.Advance,

                    IsFullfilled = contractManagement.IsFullfilled,
                    IsPaid = contractManagement.IsPaid,

                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = contractManagement.IsFinalApproval,

                    Status = contractManagement.Status,
                    ContractStatus = contractManagement.ContractStatus,
                    IsTransferredToPayment = contractManagement.IsTransferredToPayment,
                    IsTransferredToFollowUp = contractManagement.IsTransferredToFollowUp,

                    IsOnlineApproved = contractManagement.IsOnlineApproved,
                    OnlineApprovedBy = contractManagement.OnlineApprovedBy,
                    OnlineApprovedTime = contractManagement.OnlineApprovedTime,
                    IsVoid = contractManagement.IsVoid,
                    VoidBy = contractManagement.VoidBy,
                    VoidTime = contractManagement.VoidTime,
                    OrgBranchName = contractManagement.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = contractManagement.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = contractManagement.ModifiedBy
                };

                _db.ContractManagements.Add(entity);
                _db.SaveChanges();
                contractManagement.ContractManagementID = entity.ContractManagementID;
                AddAwardItems(entity);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract management", "Success", User.Identity.Name, $"created contract {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractManagement }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagements_Update([DataSourceRequest]DataSourceRequest request, ContractManagement contractManagement)
        {
            #region validation

            if (contractManagement.ProcurementRefNo==0)
                ModelState.AddModelError("exist", $"Please choose procurement reference.");

            if (contractManagement.ProcurementRefNo != 0)
            {
                var exist = _db.ContractManagements.Where(x => x.ContractManagementID != contractManagement.ContractManagementID).Where(x => x.ProcurementRefNo == contractManagement.ProcurementRefNo)
                    .ToList();
                if (exist.Any())
                    ModelState.AddModelError("exist", $"The award number already exisits in the system.");
            }

            #endregion

            #region data

            if (contractManagement.ProcurementRefNo != 0)
            {
                int did = contractManagement.ProcurementRefNo;
                var award = _db.BidAwardedSuppliers.Find(did);
                if (award != null)
                {
                    contractManagement.SupplierId = award.SupplierId;
                    contractManagement.IsNewSupplier = award.IsNewSupplier;
                    contractManagement.SupplierName = award.SupplierName;

                    var lot = _db.Lots.Find(award.LotID);
                    if (lot != null)
                    {
                        var tender = _db.Tenders.Find(lot.TenderID);
                        if (tender != null)
                            contractManagement.ProcurementOfficer = tender.ProcurementOfficer;
                    }
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ContractManagement
                {
                    ContractManagementID = contractManagement.ContractManagementID,
                    BranchId = contractManagement.BranchId,
                    BusinessUnitId = contractManagement.BusinessUnitId,
                    SupplierName = contractManagement.SupplierName,
                    IsNewSupplier = contractManagement.IsNewSupplier,
                    SupplierId = contractManagement.SupplierId,
                    ContractRefNo = contractManagement.ContractRefNo,
                    ProcurementRefNo = contractManagement.ProcurementRefNo,
                    TenderRefNo = contractManagement.TenderRefNo,
                    SigningDate = contractManagement.SigningDate,
                    ContractType = contractManagement.ContractType,
                    PurchaseCategory = contractManagement.PurchaseCategory,
                    DeliveryPlace = contractManagement.DeliveryPlace,
                    DeliveryDate = contractManagement.DeliveryDate,
                    DeliveryType = contractManagement.DeliveryType,
                    ContractAmount = contractManagement.ContractAmount,
                    RetentionPercentage = contractManagement.RetentionPercentage,
                    ProcurementOfficer = contractManagement.ProcurementOfficer,
                    Advance = contractManagement.Advance,

                    ForwardedFrom = contractManagement.ForwardedFrom,
                    ForwardedTo = contractManagement.ForwardedTo,
                    IsFinalApproval = contractManagement.IsFinalApproval,

                    IsFullfilled = contractManagement.IsFullfilled,
                    IsPaid = contractManagement.IsPaid,

                    Status = contractManagement.Status,
                    ContractStatus = contractManagement.ContractStatus,
                    IsTransferredToPayment = contractManagement.IsTransferredToPayment,
                    IsTransferredToFollowUp = contractManagement.IsTransferredToFollowUp,

                    IsOnlineApproved = contractManagement.IsOnlineApproved,
                    OnlineApprovedBy = contractManagement.OnlineApprovedBy,
                    OnlineApprovedTime = contractManagement.OnlineApprovedTime,
                    IsVoid = contractManagement.IsVoid,
                    VoidBy = contractManagement.VoidBy,
                    VoidTime = contractManagement.VoidTime,
                    OrgBranchName = contractManagement.OrgBranchName,
                    DateCreated = contractManagement.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = contractManagement.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ContractManagements.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                AddAwardItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract management", "Success", User.Identity.Name, $"created contract {entity.ContractManagementID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { contractManagement }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagements_Destroy([DataSourceRequest]DataSourceRequest request, ContractManagement contractManagement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractManagement
                {
                    ContractManagementID = contractManagement.ContractManagementID,
                    BranchId = contractManagement.BranchId,
                    BusinessUnitId = contractManagement.BusinessUnitId,
                    SupplierName = contractManagement.SupplierName,
                    IsNewSupplier = contractManagement.IsNewSupplier,
                    SupplierId = contractManagement.SupplierId,
                    ContractRefNo = contractManagement.ContractRefNo,
                    ProcurementRefNo = contractManagement.ProcurementRefNo,
                    SigningDate = contractManagement.SigningDate,
                    ContractType = contractManagement.ContractType,
                    PurchaseCategory = contractManagement.PurchaseCategory,
                    DeliveryPlace = contractManagement.DeliveryPlace,
                    DeliveryDate = contractManagement.DeliveryDate,
                    ContractAmount = contractManagement.ContractAmount,
                    RetentionPercentage = contractManagement.RetentionPercentage,
                    ProcurementOfficer = contractManagement.ProcurementOfficer,
                    Advance = contractManagement.Advance,

                    Status = contractManagement.Status,
                    IsOnlineApproved = contractManagement.IsOnlineApproved,
                    OnlineApprovedBy = contractManagement.OnlineApprovedBy,
                    OnlineApprovedTime = contractManagement.OnlineApprovedTime,
                    IsVoid = contractManagement.IsVoid,
                    VoidBy = contractManagement.VoidBy,
                    VoidTime = contractManagement.VoidTime,
                    OrgBranchName = contractManagement.OrgBranchName,
                    DateCreated = contractManagement.DateCreated,
                    LastModified = contractManagement.LastModified,
                    CreatedBy = contractManagement.CreatedBy,
                    ModifiedBy = contractManagement.ModifiedBy
                };

                _db.ContractManagements.Attach(entity);
                _db.ContractManagements.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractManagement }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ContractManagements.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ContractManagementItem.Count > 0)
            {
                #region validate price and tax

                foreach (var item in record.ContractManagementItem)
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
                _db.ContractManagements.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow

                bool isNewRecord = true;
                var validations = record.ContaContractManagementValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.ContractManagementItem.Sum(x => x.Total);
                int docid = record.ContractManagementID;

                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ContractManagement, isNewRecord);
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

            if (record.ContractManagementItem.Count == 0)
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

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        public JsonResult TransferToPayment(int id)
        {
            bool transferred = false;
            var record = _db.ContractManagements.Find(id);
            var items = _db.ContractManagementItems.Where(x => x.ContractManagementID == id).ToList();

            decimal amount = 0;
            //foreach (var item in items)
            //{
            //    amount = amount + (item.Quantity + item.Addition + item.Omission) * item.UnitPrice;

            //}
            if (items.Any())
                amount = items.Sum(x => x.Total);

            var paymentSchedule = _db.ContractPaymentSchedules.Where(x => x.ContractManagementID == id).ToList(); 

            if (paymentSchedule == null)
            {
                var formattedData = new
                {
                    Message = "There is no payment schedule defined.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }            
            

            if (paymentSchedule.Sum(x=>x.Percentage) <= 0)
            {
                var formattedData = new
                {
                    Message = "Advance percentage should be more than zero.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
                


            if (record != null && !record.IsTransferredToPayment && record.IsOnlineApproved && !record.IsVoid && amount>0)
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
                        #region create payment
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
                            PoTypeEnum = PurchaseType.TenderAward,
                            PoType = "Tender Award",
                            PurchaseReference = record.ContractManagementID.ToString(),
                            CollectedByPurchaser = false,
                            EmployeeId = record.ProcurementOfficer,
                            Remark = record.Remark,
                            OrgBranchName = record.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = record.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = record.ModifiedBy
                        };
                        _db.APPayments.Add(entity);
                        // _db.SaveChanges();

                        #endregion
                        //record.Status = Enum.GetName(typeof(OperationStatuses), 14);
                        //record.IsTransferredToPayment = true;
                        //_db.ContractManagements.Attach(record);
                        //_db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();
                        //var formattedData = new
                        //{
                        //    Message = "Successfully Sent for Payment.",
                        //    Status = "OK"
                        //};
                        //return Json(formattedData, JsonRequestBehavior.AllowGet);
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
                            ApplyPenality = false,
                            PenalityAmount = 0,
                            NetPayable = amount,

                            Type = "Tender Award",
                            PaymentType = PaymentType.Payment,
                            PoOrContractNumber = record.ContractManagementID.ToString(),
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
                        //    PrepaymentAmount =0,
                        //    NetAmount = amount * (sumAdvancePercent / 100),
                        //    PaymentMethod = "Na",
                        //    PaymentReference = "NA",
                        //    GLTaxRateID = 0,
                        //    PaymentDate = DateTime.Today,
                        //    PoTypeEnum = PurchaseType.TenderAward,
                        //    PoType = "Tender Award",
                        //    PurchaseReference = record.ContractManagementID.ToString(),
                        //    CollectedByPurchaser = false,
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
                        //_db.ContractManagements.Attach(record);
                        //_db.Entry(record).State = EntityState.Modified;
                        //_db.SaveChanges();
                        //var formattedData = new
                        //{
                        //    Message = "Successfully Sent for Payment.",
                        //    Status = "OK"
                        //};
                        //return Json(formattedData, JsonRequestBehavior.AllowGet);
                        transferred = true;
                    }
                }

                if (transferred)
                {
                    record.Status = Enum.GetName(typeof(OperationStatuses), 14);
                    record.IsTransferredToPayment = true;
                    _db.ContractManagements.Attach(record);
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully Sent for Payment.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
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

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        public JsonResult TransferToForeignFollowup(int id)
        {
            var record = _db.ContractManagements.Find(id);
            var items = _db.ContractManagementItems.Where(x => x.ContractManagementID == id).ToList();
            var award = _db.BidAwardedSuppliers
                .FirstOrDefault(x => x.BidAwardedSupplierID == record.ProcurementRefNo);
            if (award != null)
            {
                var lot = _db.Lots.Find(award.LotID);
                if (lot != null)
                {
                    var tender = _db.Tenders.Find(lot.TenderID);
            
                    if (!items.Any())
                    {
                        var formattedData = new
                        {
                            Message = "There is no contract items defined.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (tender != null && tender.PurchaseCategory!=PurchaseCategory.ForeignPurchase)
                    {
                        var formattedData = new
                        {
                            Message = "There tender is not foreign purchase.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }


                    if (record != null && !record.IsTransferredToFollowUp && record.IsOnlineApproved && !record.IsVoid)
                    {
                        if (tender != null)
                        {
                            #region create foreign followup header
                            var foreignPurchase = new ForeignPurchase
                            {
                                TenderRef = record.TenderRefNo,
                                PurchaseRef = record.ProcurementRefNo,
                                Supplier = record.SupplierName,
                                ItemDesc = "",
                                OrgStructureId = 0,
                                Quantity = 0,
                                ProcurementMethod = "Tender Award",
                                FOBMethod = FOBMethod.NationalBank,
                                ApprovalDate = null,
                                OpenigDate = tender.OpenigDate,
                                TechnicalDocSendDate = null,
                                FinancialOpenigDate = tender.FinancialOpenigDate,
                                MinuteApprovedDate = null,
                                AwardDate = award.AwardDate,
                                ContractDate = record.SigningDate,
                                PaymentMethod =ForeignPaymentMethod.LC,
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
                                    ForeignPurchase= foreignPurchase,
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
                                    SubTotal = item.UnitPrice* item.Quantity,
                                    AdditionalUnitCost =0,
                                    FinalUnitPrice = item.UnitPrice,
                                    Total = item.Total,
                                    ItemSpecification = item.ItemSpecification,
                                    UoM = item.UoM
                                };

                                _db.ForeignPurchaseFollowUpItems.Add(followupitem);
                                _db.SaveChanges();
                                #endregion
                            }
                        }

                        record.Status = Enum.GetName(typeof(OperationStatuses), 14);
                        record.IsTransferredToFollowUp = true;
                        _db.ContractManagements.Attach(record);
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
            }

            if (award == null && record != null)
            {
                var formattedData = new
                {
                    Message = $"The award no {record.ProcurementRefNo} related with this contract is not found.",
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

        [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ContractManagements.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                var poreceivetypes = _db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.Tender)
                    .Select(x => x.ReceiveTypeId).ToList();

                #region validate payment

                var payment = _db.APPayments
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.PoTypeEnum == PurchaseType.TenderAward)
                    .FirstOrDefault(x => x.PurchaseReference == record.ContractManagementID.ToString());

                if (payment != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This contract can not be void since it is referred in payment {payment.ApPaymentId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate grn

                var goodReceive = _db.GoodReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.ProcurementRefNo.ToString());

                if (goodReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This contract can not be void since it is referred in good receive {goodReceive.GoodReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate service grn

                var serviceReceive = _db.ServiceReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.ProcurementRefNo.ToString());

                if (serviceReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This contract can not be void since it is referred in service receive {serviceReceive.ServiceReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate fixed grn

                var fixedAssetReceive = _db.FixedAssetReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.ProcurementRefNo.ToString());

                if (fixedAssetReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This contract can not be void since it is referred in fixed asset receive {fixedAssetReceive.FixedAssetReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ContractManagements.Attach(record);
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

        private void AddAwardItems(ContractManagement contractManagement)
        {
            int did = contractManagement.ProcurementRefNo;
            var record = _db.BidAwardedSuppliers.Find(did);

            if (record != null && !record.IsTransferredToContract && record.BidWinningItems.Count > 0)
            {
                #region delete existing

                var existingItems = _db.ContractManagementItems
                    .Where(x => x.ContractManagementID == contractManagement.ContractManagementID).ToList();
                _db.ContractManagementItems.RemoveRange(existingItems);
                _db.SaveChanges();

                #endregion

                var awardedSupplierItems = _db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == record.BidAwardedSupplierID).ToList();
                foreach (var item in awardedSupplierItems)
                {
                    if (item.Quanity > 0)
                    {
                        #region item
                        var contractManagementItem = new ContractManagementItem
                        {
                            ContractManagement = contractManagement,
                            CatCode = item.CatCode,
                            CategoryName = item.CategoryName,
                            ItmCode = item.ItmCode,
                            ItemName = item.ItemName,
                            PartNo = item.PartNo,
                            Quantity = item.Quanity,
                            Omission = 0,
                            Addition = 0,
                            ItemSpecification = item.ItemSpecification,
                            UoM = item.UoM,
                            ItemType = item.ItemType,
                            Type = item.ItemType.ToString(),
                            UnitPrice = item.UPrice,
                            Total = item.TPrice,
                            GLTaxRateID = item.GLTaxRateID,
                            TaxAmount = item.TaxAmount,

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Today,
                            ModifiedBy = User.Identity.Name,
                            LastModified = DateTime.Today
                        };
                        _db.ContractManagementItems.Add(contractManagementItem);


                        _db.SaveChanges();
                        #endregion
                    }

                }

            }

        }

        public ActionResult GrnHeader_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<GrnViewModel> grnVms = new List<GrnViewModel>();

            var contract = _db.ContractManagements.Find(id);
            var receiveType = _db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.Tender);
            #region grn stock header
            var stockReceives = _db.GoodReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());

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
                grnVms.Add(record);
                #endregion
            }
            #endregion
            #region grn service header
            var serviceReceives = _db.ServiceReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());

            foreach (var serviceReceive in serviceReceives)
            {
                #region grn service header
                GrnViewModel record = new GrnViewModel
                {
                    GrnId = serviceReceive.ServiceReceiveId,
                    GrnType = GrnType.Service,
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
                grnVms.Add(record);
                #endregion
            }
            #endregion
            #region grn fixed asset header

            var fixedAssetReceives = _db.FixedAssetReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId).Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());

            foreach (var fixedAssetReceive in fixedAssetReceives)
            {
                #region grn fixed asset header
                GrnViewModel record = new GrnViewModel
                {
                    GrnId = fixedAssetReceive.FixedAssetReceiveId,
                    GrnType = GrnType.FixedAsset,
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
                grnVms.Add(record);
                #endregion
            }
            #endregion
            IQueryable<GrnViewModel> grnViewModels = grnVms.AsQueryable();
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
                        SubCategory = "",
                        Item = goodReceiveItem.InventoryItem.Name,
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
