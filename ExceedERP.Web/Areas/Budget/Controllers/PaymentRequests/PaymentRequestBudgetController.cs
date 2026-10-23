using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Budget.Controllers.PaymentRequests
{
    [Authorize]
    public class PaymentRequestBudgetController : Controller
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
            var employeName = _db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            var tax = _db.GLTaxRates.ToList();
            ViewData["GLTaxRate"] = tax;

            var branch = _db.Branch.ToList();
            ViewData["Branch"] = branch;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = _db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }

            var structure = _db.MainCompanyStructures.ToList();
            ViewData["OrgStructure"] = structure;

            var items = _db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }
        private JsonResult GetPeriod()
        {
            var periodList = _db.GLFiscalYears;
            ViewData["PeriodList"] = periodList?.ToList() ?? new List<GlFiscalYear>();

            return Json((periodList ?? throw new InvalidOperationException()).Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment, BudgetRoles.BudgetPaymentApproval)]
        public ActionResult PaymentRequest_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<PaymentRequest> paymentRequests = _db.PaymentRequests
                .Where(x => mybraches.Contains(x.BranchId))
                .Where(x => x.IsDepartmentOnlineApproved);

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                paymentRequests = paymentRequests.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

            DataSourceResult result = paymentRequests.ToDataSourceResult(request, paymentRequest => new
            {
                EmployeeId = paymentRequest.EmployeeId,
                OrgStuructureId = paymentRequest.OrgStuructureId,
                BranchId = paymentRequest.BranchId,
                BusinessUnitId = paymentRequest.BusinessUnitId,
                FromBranchId = paymentRequest.FromBranchId,
                FromBusinessUnitId = paymentRequest.FromBusinessUnitId,
                CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                CreatedAtOrgStructureId = paymentRequest.CreatedAtOrgStructureId,
                PaymentRequestId = paymentRequest.PaymentRequestId,
                SupplierId = paymentRequest.SupplierId,
                IsNewSupplier = paymentRequest.IsNewSupplier,
                Supplier = paymentRequest.Supplier,
                Purchaser = paymentRequest.Purchaser,
                ReasonOfPayment = paymentRequest.ReasonOfPayment,
                RequestDate = paymentRequest.RequestDate,
                RequestDateAm = paymentRequest.RequestDateAm,
                InvoiceNo = paymentRequest.InvoiceNo,
                PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                PONo = paymentRequest.PONo,
                GRN = paymentRequest.GRN,
                Status = paymentRequest.Status,
                Remark = paymentRequest.Remark,
                IsVoid = paymentRequest.IsVoid,
                VoidBy = paymentRequest.VoidBy,
                VoidTime = paymentRequest.VoidTime,

                IsBudgetApproved = paymentRequest.IsBudgetApproved,
                BudgetApprovedBy = paymentRequest.BudgetApprovedBy,
                BudgetApprovedTime = paymentRequest.BudgetApprovedTime,

                IsAuthorized = paymentRequest.IsAuthorized,
                AuthorizedBy = paymentRequest.AuthorizedBy,
                AuthorizedTime = paymentRequest.AuthorizedTime,

                IsDepartmentOnlineApproved = paymentRequest.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = paymentRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = paymentRequest.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = paymentRequest.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = paymentRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = paymentRequest.DepartmentSendForApprovalTime,

                ForwardedFrom = paymentRequest.ForwardedFrom,
                ForwardedTo = paymentRequest.ForwardedTo,
                IsFinalApproval = paymentRequest.IsFinalApproval,
                CurrentApprovalStep = paymentRequest.CurrentApprovalStep,

                IsOnlineApproved = paymentRequest.IsOnlineApproved,
                OnlineApprovedBy = paymentRequest.OnlineApprovedBy,
                OnlineApprovedTime = paymentRequest.OnlineApprovedTime,
                IsSendForApproval = paymentRequest.IsSendForApproval,
                SendForApprovalBy = paymentRequest.SendForApprovalBy,
                SendForApprovalTime = paymentRequest.SendForApprovalTime,
                IsOnlineTransferred = paymentRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = paymentRequest.IsOnlineTransferredBy,
                OnlineTransferredTime = paymentRequest.OnlineTransferredTime,
                OrgBranchName = paymentRequest.OrgBranchName,

                DateCreated = paymentRequest.DateCreated,
                CreatedBy = paymentRequest.CreatedBy,
                LastModified = paymentRequest.LastModified,
                ModifiedBy = paymentRequest.ModifiedBy,
            });

            return Json(result);
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Create([DataSourceRequest] DataSourceRequest request, PaymentRequest paymentRequest)
        {



            if (ModelState.IsValid)
            {

                var entity = new PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    BranchId = paymentRequest.BranchId,
                    BusinessUnitId = paymentRequest.BusinessUnitId,
                    FromBranchId = paymentRequest.FromBranchId,
                    FromBusinessUnitId = paymentRequest.FromBusinessUnitId,
                    OrgStuructureId = paymentRequest.OrgStuructureId,
                    CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                    SupplierId = paymentRequest.SupplierId,
                    IsNewSupplier = paymentRequest.IsNewSupplier,
                    Supplier = paymentRequest.Supplier,
                    Purchaser = paymentRequest.Purchaser,
                    ReasonOfPayment = paymentRequest.ReasonOfPayment,
                    RequestDateAm = paymentRequest.RequestDateAm,
                    InvoiceNo = paymentRequest.InvoiceNo,
                    RequestDate = paymentRequest.RequestDate,
                    PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                    PONo = paymentRequest.PONo,
                    GRN = paymentRequest.GRN,
                    Remark = paymentRequest.Remark,
                    IsVoid = paymentRequest.IsVoid,
                    VoidBy = paymentRequest.VoidBy,
                    VoidTime = paymentRequest.VoidTime,
                    IsOnlineApproved = paymentRequest.IsOnlineApproved,
                    OnlineApprovedBy = paymentRequest.OnlineApprovedBy,
                    OnlineApprovedTime = paymentRequest.OnlineApprovedTime,
                    IsSendForApproval = paymentRequest.IsSendForApproval,
                    SendForApprovalBy = paymentRequest.SendForApprovalBy,
                    SendForApprovalTime = paymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = paymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = paymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = paymentRequest.OnlineTransferredTime,

                    IsBudgetApproved = paymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = paymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = paymentRequest.BudgetApprovedTime,

                    ForwardedFrom = paymentRequest.ForwardedFrom,
                    ForwardedTo = paymentRequest.ForwardedTo,
                    IsFinalApproval = paymentRequest.IsFinalApproval,
                    CurrentApprovalStep = paymentRequest.CurrentApprovalStep,

                    IsDepartmentOnlineApproved = paymentRequest.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = paymentRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = paymentRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = paymentRequest.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = paymentRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = paymentRequest.DepartmentSendForApprovalTime,

                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = paymentRequest.LastModified,
                    ModifiedBy = paymentRequest.ModifiedBy,
                };

                _db.PaymentRequests.Add(entity);
                _db.SaveChanges();
                paymentRequest.PaymentRequestId = entity.PaymentRequestId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request", "Success", User.Identity.Name, $"created payment request {entity.PaymentRequestId} ", Modules.Finance);
                #endregion
            }



            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Update([DataSourceRequest] DataSourceRequest request, PaymentRequest paymentRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    OrgStuructureId = paymentRequest.OrgStuructureId,
                    BranchId = paymentRequest.BranchId,
                    BusinessUnitId = paymentRequest.BusinessUnitId,
                    FromBranchId = paymentRequest.FromBranchId,
                    FromBusinessUnitId = paymentRequest.FromBusinessUnitId,
                    CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                    PaymentRequestId = paymentRequest.PaymentRequestId,
                    SupplierId = paymentRequest.SupplierId,
                    IsNewSupplier = paymentRequest.IsNewSupplier,
                    Supplier = paymentRequest.Supplier,
                    Purchaser = paymentRequest.Purchaser,
                    ReasonOfPayment = paymentRequest.ReasonOfPayment,
                    RequestDateAm = paymentRequest.RequestDateAm,
                    InvoiceNo = paymentRequest.InvoiceNo,
                    RequestDate = paymentRequest.RequestDate,
                    PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                    PONo = paymentRequest.PONo,
                    GRN = paymentRequest.GRN,

                    IsBudgetApproved = paymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = paymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = paymentRequest.BudgetApprovedTime,

                    ForwardedFrom = paymentRequest.ForwardedFrom,
                    ForwardedTo = paymentRequest.ForwardedTo,
                    IsFinalApproval = paymentRequest.IsFinalApproval,
                    CurrentApprovalStep = paymentRequest.CurrentApprovalStep,


                    IsDepartmentOnlineApproved = paymentRequest.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = paymentRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = paymentRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = paymentRequest.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = paymentRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = paymentRequest.DepartmentSendForApprovalTime,

                    IsOnlineApproved = paymentRequest.IsOnlineApproved,
                    OnlineApprovedBy = paymentRequest.OnlineApprovedBy,
                    OnlineApprovedTime = paymentRequest.OnlineApprovedTime,
                    IsSendForApproval = paymentRequest.IsSendForApproval,
                    SendForApprovalBy = paymentRequest.SendForApprovalBy,
                    SendForApprovalTime = paymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = paymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = paymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = paymentRequest.OnlineTransferredTime,
                    OrgBranchName = paymentRequest.OrgBranchName,

                    Remark = paymentRequest.Remark,
                    IsVoid = paymentRequest.IsVoid,
                    VoidBy = paymentRequest.VoidBy,
                    VoidTime = paymentRequest.VoidTime,


                    DateCreated = paymentRequest.DateCreated,
                    CreatedBy = paymentRequest.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                };

                _db.PaymentRequests.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request", "Success", User.Identity.Name, $"updated payment request {entity.PaymentRequestId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetPayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequest_Destroy([DataSourceRequest] DataSourceRequest request, PaymentRequest paymentRequest)
        {

            if (ModelState.IsValid)
            {
                var entity = new PaymentRequest
                {
                    EmployeeId = paymentRequest.EmployeeId,
                    OrgStuructureId = paymentRequest.OrgStuructureId,
                    BranchId = paymentRequest.BranchId,
                    BusinessUnitId = paymentRequest.BusinessUnitId,
                    FromBranchId = paymentRequest.FromBranchId,
                    FromBusinessUnitId = paymentRequest.FromBusinessUnitId,
                    CreatedAtBranchId = paymentRequest.CreatedAtBranchId,
                    CreatedAtBusinessUnitId = paymentRequest.CreatedAtBusinessUnitId,
                    PaymentRequestId = paymentRequest.PaymentRequestId,
                    SupplierId = paymentRequest.SupplierId,
                    IsNewSupplier = paymentRequest.IsNewSupplier,
                    Supplier = paymentRequest.Supplier,
                    Purchaser = paymentRequest.Purchaser,
                    ReasonOfPayment = paymentRequest.ReasonOfPayment,
                    RequestDateAm = paymentRequest.RequestDateAm,
                    InvoiceNo = paymentRequest.InvoiceNo,
                    RequestDate = paymentRequest.RequestDate,
                    PurchaseRequestNo = paymentRequest.PurchaseRequestNo,
                    PONo = paymentRequest.PONo,
                    GRN = paymentRequest.GRN,
                    Remark = paymentRequest.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Payment request ID  " + paymentRequest.PaymentRequestId);
                #endregion
                _db.PaymentRequests.Attach(entity);
                _db.PaymentRequests.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        //[AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        //public ActionResult ApproveBudget(int id)
        //{
        //    var record = _db.PaymentRequests.Find(id);
        //    if (record != null && !record.IsBudgetApproved)
        //    {
        //        record.Status = Enum.GetName(typeof(OperationStatuses), 17);
        //        record.IsBudgetApproved = true;
        //        record.BudgetApprovedBy = User.Identity.Name;
        //        record.BudgetApprovedTime = DateTime.Now;
        //        _db.PaymentRequests.Attach(record);
        //        _db.Entry(record).State = EntityState.Modified;
        //        _db.SaveChanges();
        //        //var entity = new PaymentRequestBudgetValidation
        //        //{
        //        //    PurchasePaymentId = record.PurchasePaymentId,
        //        //    Status = Core.Domain.Common.ApprovalStatuses.Approve,
        //        //    DateCreated = DateTime.Now,
        //        //    CreatedBy = User.Identity.Name,
        //        //};

        //        //_db.PurchasePaymentBudgetValidations.Add(entity);
        //        //_db.SaveChanges();

        //        #region create budget usage
        //        FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
        //        decimal amount = 0;
        //        var lines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == id).ToList();
        //        if(lines.Any())
        //            amount=lines.Sum(x => x.TotalAmount);
        //        var budgetUsage = new BudgetUsage
        //        {
        //            GlFiscalYearId = fiscalYearHelper.GetFiscalYearOfDate(record.RequestDate),
        //            GLPeriodId = fiscalYearHelper.GetPeriodOfDate(record.RequestDate),
        //            BranchId = record.BranchId,
        //            BusinessUnitId = record.BusinessUnitId,
        //            EmployeeId = record.EmployeeId,
        //            ReasonOfPayment = record.ReasonOfPayment,
        //            RequestDate = record.RequestDate,
        //            Amount = amount,
        //            NetAmount = amount,                   

        //            DateCreated = DateTime.Now,
        //            CreatedBy = User.Identity.Name

        //        };

        //        _db.BudgetUsages.Add(budgetUsage);
        //        _db.SaveChanges();

        //        var usageDetail = new BudgetUsageDetail
        //        {
        //            BudgetUsageID = budgetUsage.BudgetUsageID,
        //            PlanCategoryId = 0,
        //            BranchId = budgetUsage.BranchId,
        //            BusinessUnitId = budgetUsage.BusinessUnitId,
        //            OrgStructureId = record.OrgStuructureId,
        //            BudgetCode = "",
        //            BudgetItem = "NA",
        //            AccountCode = "",
        //            AccountDesc = "",
        //            UsageAmount = amount,
        //            CostCenter = "",

        //            DateCreated = DateTime.Now,
        //            CreatedBy = User.Identity.Name

        //        };

        //        _db.BudgetUsageDetails.Add(usageDetail);
        //        _db.SaveChanges();

        //        #endregion

        //        #region operation log
        //        UserHelper.OperationLog(Request.UserHostAddress, "Payment request budget approval", "Success", User.Identity.Name, $"approved payment {id} ", Modules.Budget);
        //        #endregion

        //        var formattedData = new
        //        {
        //            Message = "Budget Approved",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record == null)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is not found.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }

        //    if (record.IsBudgetApproved)
        //    {
        //        var formattedData = new
        //        {
        //            Message = "The record is already approved, you cannot approve again.",
        //            Status = "NOT_OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);
        //    }


        //    return null;
        //}

        [AuthorizeRoles(BudgetRoles.BudgetVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.PaymentRequests.Find(id);
            if (record != null && record.IsBudgetApproved && !record.IsOnlineApproved)
            {

                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsDepartmentOnlineApproved = false;
                record.IsDepartmentSendForApproval = false;
                record.IsBudgetApproved = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.PaymentRequests.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                #region remove related validations

                _db.PaymentRequestDepartmentValidations.RemoveRange(record.PaymentRequestDepartmentValidations);
                _db.SaveChanges();

                #endregion

                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Payment requst budget Void", "Success", User.Identity.Name, $"void Payment Request {id} ", Modules.Budget);
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
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
