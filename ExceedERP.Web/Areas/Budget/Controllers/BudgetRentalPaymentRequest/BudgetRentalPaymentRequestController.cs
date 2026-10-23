using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.FleetManagement.RentalPayment;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Budget;

namespace ExceedERP.Web.Areas.Budget.Controllers.BudgetRentalPaymentRequest
{
    public class BudgetRentalPaymentRequestController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();      
        public ActionResult Index()
        {
            ViewData["EquipmentType"] = db.EquipmentType.Select(x => new { x.EquipmentTypeID, x.EquipmentTypeName }).ToList();
            ViewData["EquipmentName"] = db.EquipmentName.Select(x => new { x.EquipmentNameID, x.Name }).ToList();           
            ViewData["Employee"] = db.Person_Employee.Select(x => new { x.EmployeeId, FullName = x.FirstNameEnglish + " "+ x.MiddleNameEnglish+" " + x.LastNameEnglish }).ToList();
            ViewData["Branch"] = db.Branch.Select(x => new { x.BranchId, x.Name }).ToList();
            ViewData["Department"] = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            ViewData["GLPeriod"] = db.GLPeriods.Select(x => new { x.GLPeriodId, x.Name }).ToList();
            ViewData["GlFiscalYear"] = db.GLFiscalYears.Select(x => new { x.GlFiscalYearId, x.Name }).ToList();
            return View();
        }
        public ActionResult RentalPaymentRequests_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rentalPaymentRequests = db.RentalPaymentRequests;
            DataSourceResult result = rentalPaymentRequests.ToDataSourceResult(request, rentalPaymentRequest => new
            {
                RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId,
                EmployeeId = rentalPaymentRequest.EmployeeId,
                BranchId = rentalPaymentRequest.BranchId,
                ReasonofPayment = rentalPaymentRequest.ReasonofPayment,
                RequestDate = rentalPaymentRequest.RequestDate,
                Refference = rentalPaymentRequest.Refference,
                Remark = rentalPaymentRequest.Remark,
                GLPeriodId = rentalPaymentRequest.GLPeriodId,
                GLFiscalYearId = rentalPaymentRequest.GLFiscalYearId,
                DurationFrom = rentalPaymentRequest.DurationFrom,
                DurationTo = rentalPaymentRequest.DurationTo,
                OrgStructureId = rentalPaymentRequest.OrgStructureId,
                ProcurementSupplierID = rentalPaymentRequest.ProcurementSupplierID,
                Status = rentalPaymentRequest.Status,
                IsBudgetApproved = rentalPaymentRequest.IsBudgetApproved,
                BudgetApprovedBy = rentalPaymentRequest.BudgetApprovedBy,
                BudgetApprovedTime = rentalPaymentRequest.BudgetApprovedTime,
                IsDepartmentOnlineApproved = rentalPaymentRequest.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = rentalPaymentRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = rentalPaymentRequest.DepartmentOnlineApprovedTime,
                IsDepartmentSendForApproval = rentalPaymentRequest.IsDepartmentSendForApproval,
                DepartmentSendForApprovalBy = rentalPaymentRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = rentalPaymentRequest.DepartmentSendForApprovalTime,
                BusinessUnitId = rentalPaymentRequest.BusinessUnitId,
                ForwardedFrom = rentalPaymentRequest.ForwardedFrom,
                ForwardedTo = rentalPaymentRequest.ForwardedTo,
                IsFinalApproval = rentalPaymentRequest.IsFinalApproval,
                CurrentApprovalStep = rentalPaymentRequest.CurrentApprovalStep,
                IsOnlineApproved = rentalPaymentRequest.IsOnlineApproved,
                OnlineApprovedBy = rentalPaymentRequest.OnlineApprovedBy,
                OnlineApprovedTime = rentalPaymentRequest.OnlineApprovedTime,
                IsSendForApproval = rentalPaymentRequest.IsSendForApproval,
                SendForApprovalBy = rentalPaymentRequest.SendForApprovalBy,
                SendForApprovalTime = rentalPaymentRequest.SendForApprovalTime,
                IsOnlineTransferred = rentalPaymentRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = rentalPaymentRequest.IsOnlineTransferredBy,
                OnlineTransferredTime = rentalPaymentRequest.OnlineTransferredTime,
                OrgBranchName = rentalPaymentRequest.OrgBranchName,
                DateCreated = rentalPaymentRequest.DateCreated,
                CreatedBy = rentalPaymentRequest.CreatedBy,
                LastModified = rentalPaymentRequest.LastModified,
                ModifiedBy = rentalPaymentRequest.ModifiedBy,
                IsFinanceSendForApproval = rentalPaymentRequest.IsFinanceSendForApproval,
                FinanceSendForApprovalBy = rentalPaymentRequest.FinanceSendForApprovalBy,
                FinanceSendForApprovalTime = rentalPaymentRequest.FinanceSendForApprovalTime,
                IsFinanceApproved = rentalPaymentRequest.IsFinanceApproved,
                FinanceApprovedBy= rentalPaymentRequest.FinanceApprovedBy,
                FinanceApprovedTime = rentalPaymentRequest.FinanceApprovedTime,
            });
            return Json(result);
        }
        public ActionResult RentalPaymentRequests_Create([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
        {
            if (ModelState.IsValid)
                
            {
                var durations = db.GLPeriods.Where(x => x.GLPeriodId == rentalPaymentRequest.GLPeriodId)
                 .Select(x => new { x.DateFrom, x.DateTo })
                 .FirstOrDefault();
                var entity = new RentalPaymentRequest
                {
                    RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId,
                    EmployeeId = rentalPaymentRequest.EmployeeId,
                    BranchId = rentalPaymentRequest.BranchId,
                    ReasonofPayment = rentalPaymentRequest.ReasonofPayment,
                    RequestDate = rentalPaymentRequest.RequestDate,
                    Refference = rentalPaymentRequest.Refference,
                    Remark = rentalPaymentRequest.Remark,
                    GLPeriodId = rentalPaymentRequest.GLPeriodId,
                    GLFiscalYearId = rentalPaymentRequest.GLFiscalYearId,
                    DurationFrom = durations.DateFrom,
                    DurationTo = durations.DateTo,
                    OrgStructureId = rentalPaymentRequest.OrgStructureId,
                    ProcurementSupplierID = rentalPaymentRequest.ProcurementSupplierID,
                    IsOnlineApproved = rentalPaymentRequest.IsOnlineApproved,
                    OnlineApprovedBy = rentalPaymentRequest.OnlineApprovedBy,
                    OnlineApprovedTime = rentalPaymentRequest.OnlineApprovedTime,
                    IsSendForApproval = rentalPaymentRequest.IsSendForApproval,
                    SendForApprovalBy = rentalPaymentRequest.SendForApprovalBy,
                    SendForApprovalTime = rentalPaymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = rentalPaymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = rentalPaymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = rentalPaymentRequest.OnlineTransferredTime,
                    BusinessUnitId = rentalPaymentRequest.BusinessUnitId,
                    IsBudgetApproved = rentalPaymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = rentalPaymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = rentalPaymentRequest.BudgetApprovedTime,
                    ForwardedFrom = User.Identity.Name,
                    ForwardedTo = User.Identity.Name,
                    IsFinalApproval = rentalPaymentRequest.IsFinalApproval,
                    CurrentApprovalStep = rentalPaymentRequest.CurrentApprovalStep,
                    IsDepartmentOnlineApproved = rentalPaymentRequest.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = rentalPaymentRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = rentalPaymentRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = rentalPaymentRequest.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = rentalPaymentRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = rentalPaymentRequest.DepartmentSendForApprovalTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = rentalPaymentRequest.LastModified,
                    ModifiedBy = rentalPaymentRequest.ModifiedBy,
                    IsFinanceSendForApproval = rentalPaymentRequest.IsFinanceSendForApproval,
                    FinanceSendForApprovalBy = rentalPaymentRequest.FinanceSendForApprovalBy,
                    FinanceSendForApprovalTime = rentalPaymentRequest.FinanceSendForApprovalTime,
                    IsFinanceApproved = rentalPaymentRequest.IsFinanceApproved,
                    FinanceApprovedBy = rentalPaymentRequest.FinanceApprovedBy,
                    FinanceApprovedTime = rentalPaymentRequest.FinanceApprovedTime,
                };
                db.RentalPaymentRequests.Add(entity);
                db.SaveChanges();
                rentalPaymentRequest.RentalPaymentRequestId = entity.RentalPaymentRequestId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request registration", "Success", User.Identity.Name, $"created rental payment request number {entity.RentalPaymentRequestId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequest }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalPaymentRequests_Update([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
        {
          if (ModelState.IsValid)
            {
                var durations = db.GLPeriods.Where(x => x.GLPeriodId == rentalPaymentRequest.GLPeriodId)
                 .Select(x => new { x.DateFrom, x.DateTo })
                 .FirstOrDefault();
                var entity = new RentalPaymentRequest
                {
                    RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId,
                    EmployeeId = rentalPaymentRequest.EmployeeId,
                    BranchId = rentalPaymentRequest.BranchId,
                    ReasonofPayment = rentalPaymentRequest.ReasonofPayment,
                    RequestDate = rentalPaymentRequest.RequestDate,
                    Refference = rentalPaymentRequest.Refference,
                    Remark = rentalPaymentRequest.Remark,
                    GLPeriodId = rentalPaymentRequest.GLPeriodId,
                    GLFiscalYearId = rentalPaymentRequest.GLFiscalYearId,
                    DurationFrom = durations.DateFrom,
                    DurationTo = durations.DateTo,
                    OrgStructureId = rentalPaymentRequest.OrgStructureId,
                    ProcurementSupplierID = rentalPaymentRequest.ProcurementSupplierID,
                    BusinessUnitId = rentalPaymentRequest.BusinessUnitId,
                    IsBudgetApproved = rentalPaymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = rentalPaymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = rentalPaymentRequest.BudgetApprovedTime,
                    ForwardedFrom = rentalPaymentRequest.ForwardedFrom,
                    ForwardedTo = rentalPaymentRequest.ForwardedTo,
                    IsFinalApproval = rentalPaymentRequest.IsFinalApproval,
                    CurrentApprovalStep = rentalPaymentRequest.CurrentApprovalStep,
                    IsDepartmentOnlineApproved = rentalPaymentRequest.IsDepartmentOnlineApproved,
                    DepartmentOnlineApprovedBy = rentalPaymentRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = rentalPaymentRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentSendForApproval = rentalPaymentRequest.IsDepartmentSendForApproval,
                    DepartmentSendForApprovalBy = rentalPaymentRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = rentalPaymentRequest.DepartmentSendForApprovalTime,
                    IsOnlineApproved = rentalPaymentRequest.IsOnlineApproved,
                    OnlineApprovedBy = rentalPaymentRequest.OnlineApprovedBy,
                    OnlineApprovedTime = rentalPaymentRequest.OnlineApprovedTime,
                    IsSendForApproval = rentalPaymentRequest.IsSendForApproval,
                    SendForApprovalBy = rentalPaymentRequest.SendForApprovalBy,
                    SendForApprovalTime = rentalPaymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = rentalPaymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = rentalPaymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = rentalPaymentRequest.OnlineTransferredTime,
                    OrgBranchName = rentalPaymentRequest.OrgBranchName,
                    IsVoid = rentalPaymentRequest.IsVoid,
                    VoidBy = rentalPaymentRequest.VoidBy,
                    VoidTime = rentalPaymentRequest.VoidTime,
                    DateCreated = rentalPaymentRequest.DateCreated,
                    CreatedBy = rentalPaymentRequest.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    IsFinanceSendForApproval = rentalPaymentRequest.IsFinanceSendForApproval,
                    FinanceSendForApprovalBy = rentalPaymentRequest.FinanceSendForApprovalBy,
                    FinanceSendForApprovalTime = rentalPaymentRequest.FinanceSendForApprovalTime,
                    IsFinanceApproved = rentalPaymentRequest.IsFinanceApproved,
                    FinanceApprovedBy = rentalPaymentRequest.FinanceApprovedBy,
                    FinanceApprovedTime = rentalPaymentRequest.FinanceApprovedTime,
                };
                db.RentalPaymentRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental payment request update", "Success", User.Identity.Name, $"updated rental payment request {entity.RentalPaymentRequestId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalPaymentRequests_Destroy([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalPaymentRequest
                {
                    BusinessUnitId = rentalPaymentRequest.BusinessUnitId,
                    RentalPaymentRequestId = rentalPaymentRequest.RentalPaymentRequestId,
                    EmployeeId = rentalPaymentRequest.EmployeeId,
                    BranchId = rentalPaymentRequest.BranchId,
                    ReasonofPayment = rentalPaymentRequest.ReasonofPayment,
                    RequestDate = rentalPaymentRequest.RequestDate,
                    Refference = rentalPaymentRequest.Refference,
                    Remark = rentalPaymentRequest.Remark,
                    GLPeriodId = rentalPaymentRequest.GLPeriodId,
                    GLFiscalYearId = rentalPaymentRequest.GLFiscalYearId,
                    DurationFrom = rentalPaymentRequest.DurationFrom,
                    DurationTo = rentalPaymentRequest.DurationTo,
                    OrgStructureId = rentalPaymentRequest.OrgStructureId,
                    ProcurementSupplierID = rentalPaymentRequest.ProcurementSupplierID,
                    IsFinanceSendForApproval = rentalPaymentRequest.IsFinanceSendForApproval,
                    FinanceSendForApprovalBy = rentalPaymentRequest.FinanceSendForApprovalBy,
                    FinanceSendForApprovalTime = rentalPaymentRequest.FinanceSendForApprovalTime,
                    IsFinanceApproved = rentalPaymentRequest.IsFinanceApproved,
                    FinanceApprovedBy = rentalPaymentRequest.FinanceApprovedBy,
                    FinanceApprovedTime = rentalPaymentRequest.FinanceApprovedTime,
                };
                db.RentalPaymentRequests.Attach(entity);
                db.RentalPaymentRequests.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted rental inspection request {entity.RentalPaymentRequestId }");
                #endregion
            }
            return Json(new[] { rentalPaymentRequest }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = db.RentalPaymentRequests.Find(id);
            string errMsg = "";
            if (record != null && record.IsDepartmentOnlineApproved)
            {
                errMsg = "You cannot manipulate this record,it is already approved";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && record.IsDepartmentSendForApproval)
            {
                errMsg = "This record is already sent for department approval.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var lines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id);
            if (!lines.Any())
            {
                errMsg = "Please add line items.";
                var formattedData = new
                {
                    Message = errMsg,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record != null && !record.IsDepartmentOnlineApproved && !record.IsDepartmentSendForApproval)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                record.IsDepartmentSendForApproval = true;
                record.DepartmentSendForApprovalBy = User.Identity.Name;
                record.DepartmentSendForApprovalTime = DateTime.Now;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                //#region approval workflow
                //bool isNewRecord = true;
                //var validations = record.RentalPaymentRequestDepartmentValidation.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                //if (validations.Any())
                //    isNewRecord = false;
                //decimal docamount = record.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestLineId == id).Sum(x => x.TotalAmount);
                //int docid = record.RentalPaymentRequestId;
                ////DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApPaymentRequestSelfCare, isNewRecord);
                //#endregion
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(BudgetRoles.BudgetPRApproval)]
        public ActionResult ApproveBudget(int id)
        {
            var record = db.RentalPaymentRequests.Find(id);
            if (record != null && !record.IsBudgetApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 17);
                record.IsBudgetApproved = true;
                record.BudgetApprovedBy = User.Identity.Name;
                record.BudgetApprovedTime = DateTime.Now;
                db.RentalPaymentRequests.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
               
                //#region create budget usage
                //FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
                //decimal amount = 0;
                //var lines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id).ToList();
                //if (lines.Any())
                //    amount = lines.Sum(x => x.TotalAmount);
                //var budgetUsage = new BudgetUsage
                //{
                //    GlFiscalYearId = fiscalYearHelper.GetFiscalYearOfDate(record.RequestDate),
                //    GLPeriodId = fiscalYearHelper.GetPeriodOfDate(record.RequestDate),
                //    BranchId = record.BranchId,
                //    BusinessUnitId = record.BusinessUnitId,
                //    EmployeeId = record.EmployeeId,
                //    ReasonOfPayment = record.ReasonOfPayment,
                //    RequestDate = record.RequestDate,
                //    Amount = amount,
                //    NetAmount = amount,

                //    DateCreated = DateTime.Now,
                //    CreatedBy = User.Identity.Name

                //};

                //_db.BudgetUsages.Add(budgetUsage);
                //_db.SaveChanges();

                //var usageDetail = new BudgetUsageDetail
                //{
                //    BudgetUsageID = budgetUsage.BudgetUsageID,
                //    PlanCategoryId = 0,
                //    BranchId = budgetUsage.BranchId,
                //    BusinessUnitId = budgetUsage.BusinessUnitId,
                //    OrgStructureId = record.OrgStuructureId,
                //    BudgetCode = "",
                //    BudgetItem = "NA",
                //    AccountCode = "",
                //    AccountDesc = "",
                //    UsageAmount = amount,
                //    CostCenter = "",

                //    DateCreated = DateTime.Now,
                //    CreatedBy = User.Identity.Name

                //};

                //_db.BudgetUsageDetails.Add(usageDetail);
                //_db.SaveChanges();

                //#endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Payment request budget approval", "Success", User.Identity.Name, $"approved payment {id} ", Modules.Budget);
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}