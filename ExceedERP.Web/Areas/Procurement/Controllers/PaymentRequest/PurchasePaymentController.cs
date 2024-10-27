using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.Procurement.Controllers.p
{
    public class PurchasePaymentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser, ProcurementRoles.ProcurementPaymentApprover)]
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

            var purchaserList=from emp in employees
            join pur in purchasers on emp.EmployeeId equals pur.EmployeeId            
           select new { EmployeeId = emp.EmployeeId, FullNameEnglish = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish };

            ViewData["Purchasers"] = purchaserList.ToList();      

            var suppliers = _db.ProcurementSuppliers.ToList();

            ViewData["suppliers"] = suppliers.Select(c => new ProcurementSupplier
            {
                Name = c.Name

            });

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser, FinanceConstants.AccountPayableView, ProcurementRoles.ProcurementPaymentApprover)]
        public ActionResult PurchasePayments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchasePayment> PurchasePayments = _db.PurchasePayments;
            DataSourceResult result = PurchasePayments.ToDataSourceResult(request, purchasePayment => new {
               
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

                ForwardedFrom = purchasePayment.ForwardedFrom,
                ForwardedTo = purchasePayment.ForwardedTo,
                IsFinalApproval = purchasePayment.IsFinalApproval,
                CurrentApprovalStep = purchasePayment.CurrentApprovalStep,

                PurposeOfPayment = purchasePayment.PurposeOfPayment,
                ProcurementOfficer = purchasePayment.ProcurementOfficer,
                ProcurementAnalyzer = purchasePayment.ProcurementAnalyzer,

                BranchId = purchasePayment.BranchId,
                BusinessUnitId = purchasePayment.BusinessUnitId,

                Paid = purchasePayment.Paid,

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

                OrgBranchName = purchasePayment.OrgBranchName,
                DateCreated = purchasePayment.DateCreated,
                LastModified = purchasePayment.LastModified,
                CreatedBy = purchasePayment.CreatedBy,
                ModifiedBy = purchasePayment.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePayments_Create([DataSourceRequest]DataSourceRequest request, PurchasePayment purchasePayment)
        {
            #region validation

            if(String.IsNullOrEmpty(purchasePayment.PoOrContractNumber))
                ModelState.AddModelError("required", "Please select purchase order number");

            //var exist = _db.PurchasePayments
            //    .Where(x => x.Type == purchasePayment.Type)//po
            //    .Where(x => x.PaymentType != PaymentType.Credit) //credit has its own grn,can request on deliver..later will allow all
            //    .Where(x => x.PoOrContractNumber == purchasePayment.PoOrContractNumber).FirstOrDefault();
            //if (exist != null)
            //    ModelState.AddModelError("exisit","Payment request for this source ia already added");

            if(!IsItLegibleToAdd(purchasePayment))
                ModelState.AddModelError("ilegitimate", "Please check if there  is correct payment schedule defined");

            if (purchasePayment.PaymentType==PaymentType.Credit && String.IsNullOrEmpty(purchasePayment.SupplierInvoice))
                ModelState.AddModelError("exisit", "Please fill supplier invoice number/reference");


            #endregion
            #region data
            if (purchasePayment.ApplyPenality)
            {
                purchasePayment.NetPayable = purchasePayment.PaymentAmount - purchasePayment.PenalityAmount;
                purchasePayment.AmountDue = purchasePayment.NetPayable;
            }
            else
            {
                purchasePayment.NetPayable = purchasePayment.PaymentAmount;
                purchasePayment.AmountDue = purchasePayment.NetPayable;
            }

            if(purchasePayment.Type=="Purchase Order")
            {
                int poid = int.Parse(purchasePayment.PoOrContractNumber);
                var po = _db.PurchaseOrders.Find(poid);
                purchasePayment.PurchasePaymentType = po.PurchasePaymentType;
                purchasePayment.ProformaRef = po.ProformaPurchaseID.ToString();
                purchasePayment.TenderRef = po.BidAwardedSupplierID.ToString();
            }
            else if (purchasePayment.Type == "Tender Award")
            {

            }

                #endregion

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
                    PurchasePaymentType = purchasePayment.PurchasePaymentType,
                    PoOrContractNumber = purchasePayment.PoOrContractNumber,
                    TenderRef = purchasePayment.TenderRef,
                    ProformaRef = purchasePayment.ProformaRef,
                    NextApprover = purchasePayment.NextApprover,

                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = purchasePayment.IsFinalApproval,
                    CurrentApprovalStep = purchasePayment.CurrentApprovalStep,

                    PurposeOfPayment = purchasePayment.PurposeOfPayment,
                    ProcurementOfficer = purchasePayment.ProcurementOfficer,
                    ProcurementAnalyzer = purchasePayment.ProcurementAnalyzer,

                    BranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name),
                    BusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name),

                    Paid = purchasePayment.Paid,

                    IsBudgetApproved = purchasePayment.IsBudgetApproved,
                    BudgetApprovedBy = purchasePayment.BudgetApprovedBy,
                    BudgetApprovedTime = purchasePayment.BudgetApprovedTime,

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

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePayments_Update([DataSourceRequest]DataSourceRequest request, PurchasePayment purchasePayment)
        {

            #region validation

            if (String.IsNullOrEmpty(purchasePayment.PoOrContractNumber))
                ModelState.AddModelError("required", "Please select purchase order number");

            //var exist = _db.PurchasePayments
            //    .Where(x=>x.PurchasePaymentId!=purchasePayment.PurchasePaymentId)
            //    .Where(x => x.PaymentType != PaymentType.Credit) //credit has its own grn,can request on deliver..later will allow all
            //    .Where(x => x.Type == purchasePayment.Type) //po
            //    .Where(x => x.PoOrContractNumber == purchasePayment.PoOrContractNumber).FirstOrDefault();
            //if (exist != null)
            //    ModelState.AddModelError("exisit", "Payment request for this source ia already added");

            if (!IsItLegibleToAdd(purchasePayment))
                ModelState.AddModelError("ilegitimate", "Please check if there  is correct payment schedule defined");

            if (purchasePayment.PaymentType == PaymentType.Credit && String.IsNullOrEmpty(purchasePayment.SupplierInvoice))
                ModelState.AddModelError("exisit", "Please fill supplier invoice number/reference");


            #endregion
            #region data
            if (purchasePayment.ApplyPenality)
            {
                purchasePayment.NetPayable = purchasePayment.PaymentAmount - purchasePayment.PenalityAmount;
                purchasePayment.AmountDue = purchasePayment.NetPayable;
            }
            else
            {
                purchasePayment.NetPayable = purchasePayment.PaymentAmount;
                purchasePayment.AmountDue = purchasePayment.NetPayable;
            }


            if (purchasePayment.Type == "Purchase Order")
            {
                int poid = int.Parse(purchasePayment.PoOrContractNumber);
                var po = _db.PurchaseOrders.Find(poid);
                purchasePayment.PurchasePaymentType = po.PurchasePaymentType;
                purchasePayment.ProformaRef = po.ProformaPurchaseID.ToString();
                purchasePayment.TenderRef = po.BidAwardedSupplierID.ToString();
            }
            else if (purchasePayment.Type == "Tender Award")
            {

            }
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
                    PurchasePaymentType = purchasePayment.PurchasePaymentType,
                    PoOrContractNumber = purchasePayment.PoOrContractNumber,
                    TenderRef = purchasePayment.TenderRef,
                    ProformaRef = purchasePayment.ProformaRef,
                    NextApprover = purchasePayment.NextApprover,

                    ForwardedFrom = purchasePayment.ForwardedFrom,
                    ForwardedTo = purchasePayment.ForwardedTo,
                    IsFinalApproval = purchasePayment.IsFinalApproval,
                    CurrentApprovalStep = purchasePayment.CurrentApprovalStep,

                    PurposeOfPayment = purchasePayment.PurposeOfPayment,
                    ProcurementOfficer = purchasePayment.ProcurementOfficer,
                    ProcurementAnalyzer = purchasePayment.ProcurementAnalyzer,

                    BranchId = purchasePayment.BranchId,
                    BusinessUnitId = purchasePayment.BusinessUnitId,

                    IsBudgetApproved = purchasePayment.IsBudgetApproved,
                    BudgetApprovedBy = purchasePayment.BudgetApprovedBy,
                    BudgetApprovedTime = purchasePayment.BudgetApprovedTime,

                    Paid = purchasePayment.Paid,

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

        [AuthorizeRoles(ProcurementRoles.ProcurementPaymentUser)]
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
                    PurchasePaymentType = purchasePayment.PurchasePaymentType,
                    PoOrContractNumber = purchasePayment.PoOrContractNumber,
                    NextApprover = purchasePayment.NextApprover,

                    PurposeOfPayment = purchasePayment.PurposeOfPayment,
                    ProcurementOfficer = purchasePayment.ProcurementOfficer,
                    ProcurementAnalyzer = purchasePayment.ProcurementAnalyzer,

                    BranchId = purchasePayment.BranchId,
                    BusinessUnitId = purchasePayment.BusinessUnitId,

                    IsBudgetApproved = purchasePayment.IsBudgetApproved,
                    BudgetApprovedBy = purchasePayment.BudgetApprovedBy,
                    BudgetApprovedTime = purchasePayment.BudgetApprovedTime,

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

        public JsonResult SendForApproval(int id)
        {
            var record = _db.PurchasePayments.Find(id);
            //except advance all should have grn
            if (record.PaymentType == PaymentType.Credit)  //PaymentType.Payment && record.PurchasePaymentType == PurchasePaymentType.Credit)
            {
                //check if there is grn
                var grns = _db.PurchasePaymentGrns
                    .Where(x => x.PurchasePaymentId == record.PurchasePaymentId);
                if(!grns.Any())
                {
                    var formattedData = new
                    {
                        Message = "Please add GRNs first.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }

            if (record.NetPayable<=0)
            {
                var formattedData = new
                {
                    Message = "Net payable should be more than 0.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.PurchasePayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow

                bool isNewRecord = true;
                var validations = record.PurchasetPaymentValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.NetPayable;
                int docid = record.PurchasePaymentId;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.PurchasePaymentRequest, isNewRecord,step);
                #endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
           
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
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

        private bool IsItLegibleToAdd(PurchasePayment record)
        {
            if(record.Type=="Purchase Order")
            {
                int id = int.Parse(record.PoOrContractNumber);
               
                var paymentSchedule = _db.PoPaymentSchedules.Where(x => x.PurchaseOrderID == id).ToList();
                if (paymentSchedule == null)
                    return false;

                if(record.PaymentType==PaymentType.Advance)
                {
                    var advances = paymentSchedule.Where(x => x.PaymentType == PaymentType.Advance).ToList();
                    decimal sumAdvancePercent = advances.Sum(x => x.Percentage);
                    if (sumAdvancePercent <= 0)
                        return false;
                    else
                        return true;
                }
                else if (record.PaymentType == PaymentType.Credit)
                {
                    var payments = paymentSchedule.Where(x => x.PaymentType == PaymentType.Credit).ToList();
                    decimal sumPaymentPercent = payments.Sum(x => x.Percentage);
                    if (sumPaymentPercent <= 0)
                        return false;
                    else
                        return true;
                }
                else
                {
                    var payments = paymentSchedule.Where(x => x.PaymentType == PaymentType.Payment).ToList();
                    decimal sumPaymentPercent = payments.Sum(x => x.Percentage);
                    if (sumPaymentPercent <= 0)
                        return false;
                    else
                        return true;
                }
            }

            else if (record.Type == "Tender Award")
            {
                int id = int.Parse(record.PoOrContractNumber);
                var paymentSchedule = _db.ContractPaymentSchedules.Where(x => x.ContractManagementID == id).ToList();
                if (paymentSchedule == null)
                    return false;

                if (record.PaymentType == PaymentType.Advance)
                {
                    var advances = paymentSchedule.Where(x => x.PaymentType == PaymentType.Advance).ToList();
                    decimal sumAdvancePercent = advances.Sum(x => x.Percentage);
                    if (sumAdvancePercent <= 0)
                        return false;
                    else
                        return true;
                }
                else if (record.PaymentType == PaymentType.Credit)
                {
                    var advances = paymentSchedule.Where(x => x.PaymentType == PaymentType.Credit).ToList();
                    decimal sumAdvancePercent = advances.Sum(x => x.Percentage);
                    if (sumAdvancePercent <= 0)
                        return false;
                    else
                        return true;
                }
                else
                {
                    var payments = paymentSchedule.Where(x => x.PaymentType == PaymentType.Payment).ToList();
                    decimal sumPaymentPercent = payments.Sum(x => x.Percentage);
                    if (sumPaymentPercent <= 0)
                        return false;
                    else
                        return true;
                }
            }
            return false;
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
        [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.PurchasePayments.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                
                #region validate payment

                var payment = _db.APPayments
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.PoType == "Purchase Payment")
                    .FirstOrDefault(x => x.PurchaseReference == record.PurchasePaymentId.ToString());

                if (payment != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This payment request can not be void since it is referred in payment {payment.ApPaymentId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate invoice

                var invoice = _db.APPaybleInvoices
                    .Where(x => x.IsVoid == false)
                    .FirstOrDefault(x => x.SourceId == record.PurchasePaymentId.ToString());

                if (invoice != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This payment request can not be void since it is referred in invoice {invoice.ApPaybleInvoicesId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion


                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsFinanceApproved = false;
                record.IsFinanceSendForApproval = false;
                record.IsBudgetApproved = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.ForwardedFrom = User.Identity.Name;
                record.ForwardedTo = User.Identity.Name;
                _db.PurchasePayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;

                var approvals = _db.PurchasePaymentValidations
                   .Where(x => x.PurchasePaymentId == id).ToList();

                //remove to allow approval again
                _db.PurchasePaymentValidations.RemoveRange(approvals);
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
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
