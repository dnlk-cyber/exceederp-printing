using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.Budget.Controllers.ProcurementPaymentRequest
{
    public class PurchasePaymentBudgetController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var branches = _db.Branch;
            ViewData["Branches"] = branches;

            var employees = _db.Person_Employee;
            var purchasers = _db.Purchasers;

            var purchaserList = from emp in employees
                                join pur in purchasers on emp.EmployeeId equals pur.EmployeeId
                                select new { EmployeeId = emp.EmployeeId, FullNameEnglish = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish };

            ViewData["Purchasers"] = purchaserList.ToList();

            var suppliers = _db.ProcurementSuppliers.ToList();

            ViewData["suppliers"] = suppliers.Select(c => new ProcurementSupplier
            {
                Name = c.Name

            });

        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult PurchasePayments_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<PurchasePayment> purchasePayments = _db.PurchasePayments
                .Where(x => mybraches.Contains(x.BranchId) && x.IsOnlineApproved);

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                purchasePayments = purchasePayments.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

            DataSourceResult result = purchasePayments.ToDataSourceResult(request, purchasePayment => new {
               
                PurchasePaymentId = purchasePayment.PurchasePaymentId,
                Date = purchasePayment.Date,
                //WithholdingType = purchasePayment.WithholdingType,
                //WithholdingAmount = purchasePayment.WithholdingAmount,
                ForBranchId = purchasePayment.ForBranchId,
                SupplierId = purchasePayment.SupplierId,
                SupplierName = purchasePayment.SupplierName,
                SupplierInvoice = purchasePayment.SupplierInvoice,
                WithOutGRN = purchasePayment.WithOutGRN,
                PaymentAmount = purchasePayment.PaymentAmount,
                ApplyPenality = purchasePayment.ApplyPenality,
                PenalityAmount = purchasePayment.PenalityAmount,
                AmountDue = purchasePayment.AmountDue,
                ServiceCharge = purchasePayment.ServiceCharge,
                OtherPayment = purchasePayment.OtherPayment,
                NetPayable = purchasePayment.NetPayable,
                Remark = purchasePayment.Remark,
                Status = purchasePayment.Status,
                IsOnlineApproved = purchasePayment.IsOnlineApproved,
                OnlineApprovedBy = purchasePayment.OnlineApprovedBy,
                OnlineApprovedTime = purchasePayment.OnlineApprovedTime,
                IsSendForApproval = purchasePayment.IsSendForApproval,
                SendForApprovalBy = purchasePayment.SendForApprovalBy,
                SendForApprovalTime = purchasePayment.SendForApprovalTime,
                IsVoid = purchasePayment.IsVoid,
                VoidBy = purchasePayment.VoidBy,
                VoidTime = purchasePayment.VoidTime,

                Type = purchasePayment.Type,
                PaymentType = purchasePayment.PaymentType,
                PurchasePaymentType = purchasePayment.PurchasePaymentType,
                PoOrContractNumber = purchasePayment.PoOrContractNumber,
                TenderRef = purchasePayment.TenderRef,
                ProformaRef = purchasePayment.ProformaRef,
                NextApprover = purchasePayment.NextApprover,

                PurposeOfPayment = purchasePayment.PurposeOfPayment,
                ProcurementOfficer = purchasePayment.ProcurementOfficer,

                BranchId = purchasePayment.BranchId,
                BusinessUnitId = purchasePayment.BusinessUnitId,

                IsBudgetApproved = purchasePayment.IsBudgetApproved,
                BudgetApprovedBy = purchasePayment.BudgetApprovedBy,
                BudgetApprovedTime = purchasePayment.BudgetApprovedTime,

                IsFinanceSendForApproval = purchasePayment.IsFinanceSendForApproval,
                FinanceSendForApprovalBy = purchasePayment.FinanceSendForApprovalBy,
                FinanceSendForApprovalTime = purchasePayment.FinanceSendForApprovalTime,

                IsFinanceApproved = purchasePayment.IsFinanceApproved,
                FinanceApprovedBy = purchasePayment.FinanceApprovedBy,
                FinanceApprovedTime = purchasePayment.FinanceApprovedTime,

                IsAuthorized = purchasePayment.IsAuthorized,
                AuthorizedBy = purchasePayment.AuthorizedBy,
                AuthorizedTime = purchasePayment.AuthorizedTime,

                Paid = purchasePayment.Paid,

                OrgBranchName = purchasePayment.OrgBranchName,
                DateCreated = purchasePayment.DateCreated,
                LastModified = purchasePayment.LastModified,
                CreatedBy = purchasePayment.CreatedBy,
                ModifiedBy = purchasePayment.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePayments_Create([DataSourceRequest]DataSourceRequest request, PurchasePayment purchasePayment)
        {
            

            if (purchasePayment.ApplyPenality)
                purchasePayment.NetPayable = purchasePayment.PaymentAmount - purchasePayment.PenalityAmount;
            else
                purchasePayment.NetPayable = purchasePayment.PaymentAmount;
            //#region validation 

            //var Purchase = _db.PurchasePayments.Find(id);

            //if (Purchase != null && Purchase.PurchaseStatus==PurchaseStatus.Terminated)
            //    ModelState.AddModelError("terminated", "The Purchase is terminated,hence can't request payment.");
            //if (PurchasePayment.PenalityAmount >= PurchasePayment.PaymentAmount)
            //    ModelState.AddModelError("penaltyoverpayment", "The penalty amount is more than the payment.");
            //if (PurchasePayment.ApplyPenality)
            //    PurchasePayment.NetPayable = PurchasePayment.PaymentAmount - PurchasePayment.PenalityAmount;

            //#endregion

            if (ModelState.IsValid)
            {
                var entity = new PurchasePayment
                {
                    Date = purchasePayment.Date.Date,
                    ForBranchId = purchasePayment.ForBranchId,
                    SupplierId = purchasePayment.SupplierId,
                    SupplierName = purchasePayment.SupplierName,
                    SupplierInvoice = purchasePayment.SupplierInvoice,
                    WithOutGRN = purchasePayment.WithOutGRN,
                    PaymentAmount = purchasePayment.PaymentAmount,
                    ApplyPenality = purchasePayment.ApplyPenality,
                    PenalityAmount = purchasePayment.PenalityAmount,
                    ServiceCharge = purchasePayment.ServiceCharge,
                    OtherPayment = purchasePayment.OtherPayment,
                    NetPayable = purchasePayment.NetPayable,
                    AmountDue = purchasePayment.AmountDue,

                    Remark = purchasePayment.Remark,
                    IsOnlineApproved = purchasePayment.IsOnlineApproved,
                    OnlineApprovedBy = purchasePayment.OnlineApprovedBy,
                    OnlineApprovedTime = purchasePayment.OnlineApprovedTime,
                    IsVoid = purchasePayment.IsVoid,
                    VoidBy = purchasePayment.VoidBy,
                    VoidTime = purchasePayment.VoidTime,

                    Type = purchasePayment.Type,
                    PaymentType = purchasePayment.PaymentType,
                    PoOrContractNumber = purchasePayment.PoOrContractNumber,
                    TenderRef = purchasePayment.TenderRef,
                    ProformaRef = purchasePayment.ProformaRef,
                    NextApprover = purchasePayment.NextApprover,
                    PurposeOfPayment = purchasePayment.PurposeOfPayment,
                    ProcurementOfficer = purchasePayment.ProcurementOfficer,
                    BranchId = purchasePayment.BranchId,
                    BusinessUnitId = purchasePayment.BusinessUnitId,

                    Paid = purchasePayment.Paid,

                    IsBudgetApproved = purchasePayment.IsBudgetApproved,
                    BudgetApprovedBy = purchasePayment.BudgetApprovedBy,
                    BudgetApprovedTime = purchasePayment.BudgetApprovedTime,

                    IsAuthorized = purchasePayment.IsAuthorized,
                    AuthorizedBy = purchasePayment.AuthorizedBy,
                    AuthorizedTime = purchasePayment.AuthorizedTime,

                    OrgBranchName = purchasePayment.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = purchasePayment.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = purchasePayment.ModifiedBy
                };

                _db.PurchasePayments.Add(entity);
                _db.SaveChanges();
                purchasePayment.PurchasePaymentId = entity.PurchasePaymentId;

               // GetGrnList(id, entity.PurchasePaymentId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment", "Success", User.Identity.Name, $"created Purchase payment {entity.PaymentAmount} under record {entity.PurchasePaymentId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchasePayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePayments_Update([DataSourceRequest]DataSourceRequest request, PurchasePayment purchasePayment)
        {


            if (purchasePayment.ApplyPenality)
                purchasePayment.NetPayable = purchasePayment.PaymentAmount - purchasePayment.PenalityAmount;
            else
                purchasePayment.NetPayable = purchasePayment.PaymentAmount;
            #region validation 
            //var Purchase = _db.PurchaseManagements.Find(PurchasePayment.PurchaseManagementID);

            //if (Purchase != null && Purchase.PurchaseStatus == PurchaseStatus.Terminated)
            //    ModelState.AddModelError("terminated", "The Purchase is terminated,hence can't request payment.");

            //if (PurchasePayment.PenalityAmount>= PurchasePayment.PaymentAmount)
            //    ModelState.AddModelError("penaltyoverpayment","The penalty amount is more than the payment.");
            //if (PurchasePayment.ApplyPenality)
            //    PurchasePayment.NetPayable = PurchasePayment.PaymentAmount - PurchasePayment.PenalityAmount;

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PurchasePayment
                {
                    
                    PurchasePaymentId = purchasePayment.PurchasePaymentId,
                    Date = purchasePayment.Date,
                    ForBranchId = purchasePayment.ForBranchId,
                    SupplierId = purchasePayment.SupplierId,
                    SupplierName = purchasePayment.SupplierName,
                    SupplierInvoice = purchasePayment.SupplierInvoice,
                    WithOutGRN = purchasePayment.WithOutGRN,
                    PaymentAmount = purchasePayment.PaymentAmount,
                    ApplyPenality = purchasePayment.ApplyPenality,
                    PenalityAmount = purchasePayment.PenalityAmount,
                    ServiceCharge = purchasePayment.ServiceCharge,
                    OtherPayment = purchasePayment.OtherPayment,
                    NetPayable = purchasePayment.NetPayable,
                    AmountDue = purchasePayment.AmountDue,
                    Remark = purchasePayment.Remark,
                    IsOnlineApproved = purchasePayment.IsOnlineApproved,
                    OnlineApprovedBy = purchasePayment.OnlineApprovedBy,
                    OnlineApprovedTime = purchasePayment.OnlineApprovedTime,
                    IsVoid = purchasePayment.IsVoid,
                    VoidBy = purchasePayment.VoidBy,
                    VoidTime = purchasePayment.VoidTime,

                    Type = purchasePayment.Type,
                    PaymentType = purchasePayment.PaymentType,
                    PoOrContractNumber = purchasePayment.PoOrContractNumber,
                    TenderRef = purchasePayment.TenderRef,
                    ProformaRef = purchasePayment.ProformaRef,
                    NextApprover = purchasePayment.NextApprover,
                    PurposeOfPayment = purchasePayment.PurposeOfPayment,
                    ProcurementOfficer = purchasePayment.ProcurementOfficer,
                    BranchId = purchasePayment.BranchId,
                    BusinessUnitId = purchasePayment.BusinessUnitId,

                    Paid = purchasePayment.Paid,

                    IsBudgetApproved = purchasePayment.IsBudgetApproved,
                    BudgetApprovedBy = purchasePayment.BudgetApprovedBy,
                    BudgetApprovedTime = purchasePayment.BudgetApprovedTime,

                    IsAuthorized = purchasePayment.IsAuthorized,
                    AuthorizedBy = purchasePayment.AuthorizedBy,
                    AuthorizedTime = purchasePayment.AuthorizedTime,

                    OrgBranchName = purchasePayment.OrgBranchName,
                    DateCreated = purchasePayment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = purchasePayment.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.PurchasePayments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment", "Success", User.Identity.Name, $"updated Purchase payment {entity.PaymentAmount} under record {entity.PurchasePaymentId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchasePayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePayments_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePayment purchasePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePayment
                {
                    PurchasePaymentId = purchasePayment.PurchasePaymentId,
                    Date = purchasePayment.Date,
                    ForBranchId = purchasePayment.ForBranchId,
                    SupplierId = purchasePayment.SupplierId,                   
                    SupplierName = purchasePayment.SupplierName,
                    SupplierInvoice = purchasePayment.SupplierInvoice,
                    WithOutGRN = purchasePayment.WithOutGRN,
                    PaymentAmount = purchasePayment.PaymentAmount,
                    ApplyPenality = purchasePayment.ApplyPenality,
                    PenalityAmount = purchasePayment.PenalityAmount,
                    ServiceCharge = purchasePayment.ServiceCharge,
                    OtherPayment = purchasePayment.OtherPayment,
                    NetPayable = purchasePayment.NetPayable,
                    AmountDue = purchasePayment.AmountDue,
                    Remark = purchasePayment.Remark,
                    IsOnlineApproved = purchasePayment.IsOnlineApproved,
                    OnlineApprovedBy = purchasePayment.OnlineApprovedBy,
                    OnlineApprovedTime = purchasePayment.OnlineApprovedTime,
                    IsVoid = purchasePayment.IsVoid,
                    VoidBy = purchasePayment.VoidBy,
                    VoidTime = purchasePayment.VoidTime,

                    Type = purchasePayment.Type,
                    PaymentType = purchasePayment.PaymentType,
                    PoOrContractNumber = purchasePayment.PoOrContractNumber,
                    NextApprover = purchasePayment.NextApprover,
                    PurposeOfPayment = purchasePayment.PurposeOfPayment,
                    ProcurementOfficer = purchasePayment.ProcurementOfficer,
                    BranchId = purchasePayment.BranchId,
                    BusinessUnitId = purchasePayment.BusinessUnitId,

                    IsBudgetApproved = purchasePayment.IsBudgetApproved,
                    BudgetApprovedBy = purchasePayment.BudgetApprovedBy,
                    BudgetApprovedTime = purchasePayment.BudgetApprovedTime,

                    Paid = purchasePayment.Paid,

                    OrgBranchName = purchasePayment.OrgBranchName,
                    DateCreated = purchasePayment.DateCreated,
                    LastModified = purchasePayment.LastModified,
                    CreatedBy = purchasePayment.CreatedBy,
                    ModifiedBy = purchasePayment.ModifiedBy
                };

                _db.PurchasePayments.Attach(entity);
                _db.PurchasePayments.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { purchasePayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        public ActionResult ApproveBudget(int id)
        {
            var record = _db.PurchasePayments.Find(id);
            if (record != null && !record.IsBudgetApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 17);
                record.IsBudgetApproved = true;
                record.BudgetApprovedBy = User.Identity.Name;
                record.BudgetApprovedTime = DateTime.Now;
                _db.PurchasePayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                //_db.SaveChanges();
                var entity = new PurchasePaymentBudgetValidation
                {
                    PurchasePaymentId = record.PurchasePaymentId,
                    Status = Core.Domain.Common.ApprovalStatuses.Approve,
                    DateCreated =DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                _db.PurchasePaymentBudgetValidations.Add(entity);
                _db.SaveChanges();
               
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment approval", "Success", User.Identity.Name, $"approved payment {entity.PurchasePaymentId} ", Modules.Budget);
                #endregion
            
                var formattedData = new
                {
                    Message = "Budget Approved",
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

            if (record.IsBudgetApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved, you cannot approve again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        [AuthorizeRoles(BudgetRoles.BudgetVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.PurchasePayments.Find(id);
            if (record != null && record.IsBudgetApproved && !record.IsOnlineApproved)
            {

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsBudgetApproved = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.PurchasePayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase payment budget Void", "Success", User.Identity.Name, $"void Purchase Payment {id} ", Modules.Budget);
                #endregion

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

            if (!record.IsBudgetApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is approved at finance, it should be void there first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }

        private void GetGrnList(int Purchaseid,int paymentid)
        {
           
            //var Purchase = _db.PurchasePayments.Find(Purchaseid);
            //var receiveType = _db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.Tender);
            //if (Purchase != null && Purchase.ProcurementRefNo != 0)
            //{
            //    #region grn stock 

            //    var stockReceives = _db.GoodReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
            //        .Where(x => x.ReceiveRef == Purchase.ProcurementRefNo.ToString()).ToList();

            //    foreach (var goodReceive in stockReceives)
            //    {
            //        #region grn 

            //        var grn = new PurchasePaymentGrn
            //        {
            //            PurchasePaymentId = paymentid,
            //            GrnNo = goodReceive.GoodReceiveId,
            //            ItemType = PurchaseItemType.Stock,
            //            DateCreated = DateTime.Now,
            //            LastModified = goodReceive.LastModified,
            //            CreatedBy = User.Identity.Name,
            //            ModifiedBy = goodReceive.ModifiedBy
            //        };
            //        _db.PurchasePaymentGrns.Add(grn);
            //        _db.SaveChanges();

            //        #endregion
            //    }

            //    #endregion

            //    #region grn service 

            //    var serviceReceives = _db.ServiceReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
            //        .Where(x => x.ReceiveRef == Purchase.ProcurementRefNo.ToString()).ToList();

            //    foreach (var serviceReceive in serviceReceives)
            //    {
            //        #region grn service header

            //        var grn = new PurchasePaymentGrn
            //        {
            //            PurchasePaymentId = paymentid,
            //            ItemType = PurchaseItemType.Service,
            //            DateCreated = DateTime.Now,
            //            LastModified = serviceReceive.LastModified,
            //            CreatedBy = User.Identity.Name,
            //        };
            //        _db.PurchasePaymentGrns.Add(grn);
            //        _db.SaveChanges();

            //        #endregion
            //    }

            //    #endregion

            //    #region grn fixed asset 

            //    var fixedAssetReceives = _db.FixedAssetReceives.Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
            //        .Where(x => x.ReceiveRef == Purchase.ProcurementRefNo.ToString()).ToList();

            //    foreach (var fixedAssetReceive in fixedAssetReceives)
            //    {
            //        #region grn fixed asset 

            //        var grn = new PurchasePaymentGrn
            //        {
            //            PurchasePaymentId = paymentid,
            //            ItemType = PurchaseItemType.FixedAsset,
            //            DateCreated = DateTime.Now,
            //            LastModified = fixedAssetReceive.LastModified,
            //            CreatedBy = User.Identity.Name,
            //        };
            //        _db.PurchasePaymentGrns.Add(grn);
            //        _db.SaveChanges();

            //        #endregion
            //    }

            //    #endregion
            //}

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
