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
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    public class FinancePaymentRequestController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["EquipmentType"] = db.EquipmentType.Select(x => new { x.EquipmentTypeID, x.EquipmentTypeName }).ToList();
            ViewData["EquipmentName"] = db.EquipmentName.Select(x => new { x.EquipmentNameID, x.Name }).ToList();
            ViewData["Employee"] = db.Person_Employee.Select(x => new { x.EmployeeId, FullName = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();
            ViewData["Branch"] = db.Branch.Select(x => new { x.BranchId, x.Name }).ToList();
            ViewData["Department"] = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();
            ViewData["GLPeriod"] = db.GLPeriods.Select(x => new { x.GLPeriodId, x.Name }).ToList();
            ViewData["GlFiscalYear"] = db.GLFiscalYears.Select(x => new { x.GlFiscalYearId, x.Name }).ToList();
            return View();
        }
        public ActionResult FinaceRentalPaymentRequests_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rentalPaymentRequests = db.RentalPaymentRequests.Where(x=>x.IsBudgetApproved);
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

            });
            return Json(result);
        }
        public ActionResult RentalPaymentRequestFinanceValidations_Read(int rId, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RentalPaymentRequestValidation> rentalPaymentRequestValidations = db.RentalPaymentRequestValidations.Where(x => x.RentalPaymentRequestId == rId);
            DataSourceResult result = rentalPaymentRequestValidations.ToDataSourceResult(request, rentalPaymentRequestValidation => new {
                RentalPaymentRequestBudgetValidationId = rentalPaymentRequestValidation.RentalPaymentRequestValidationId,
                PaymentRequestId = rentalPaymentRequestValidation.RentalPaymentRequestId,
                Status = rentalPaymentRequestValidation.Status,
                Remark = rentalPaymentRequestValidation.Remark,
                OrgBranchName = rentalPaymentRequestValidation.OrgBranchName,
                DateCreated = rentalPaymentRequestValidation.DateCreated,
                LastModified = rentalPaymentRequestValidation.LastModified,
                CreatedBy = rentalPaymentRequestValidation.CreatedBy,
                ModifiedBy = rentalPaymentRequestValidation.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult RentalPaymentRequestLine_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var rentalPaymentRequestLines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id).ToList();
            DataSourceResult result = rentalPaymentRequestLines.ToDataSourceResult(request, rentalPaymentRequestLine => new
            {
                RentalPaymentRequestLineId = rentalPaymentRequestLine.RentalPaymentRequestLineId,
                RentalPaymentRequestId = rentalPaymentRequestLine.RentalPaymentRequestId,
                EquipmnetTypeId = rentalPaymentRequestLine.EquipmnetTypeId,
                EquipmentNameId = rentalPaymentRequestLine.EquipmentNameId,
                Amount = rentalPaymentRequestLine.Amount,
                TaxAmount = rentalPaymentRequestLine.TaxAmount,
                SubTotalAmount = rentalPaymentRequestLine.SubTotalAmount,
                FuelAmount = rentalPaymentRequestLine.FuelAmount,
                Commission = rentalPaymentRequestLine.Commission,
                AdvancedPayment = rentalPaymentRequestLine.AdvancedPayment,
                TotalAmount = rentalPaymentRequestLine.TotalAmount,
            });
            return Json(result);
        }
        public ActionResult FinanceRentalPaymentRequests_Create([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
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
                    IsOnlineApproved = true,
                    OnlineApprovedBy = User.Identity.Name,
                    OnlineApprovedTime = DateTime.Now,
                    IsSendForApproval = rentalPaymentRequest.IsSendForApproval,
                    SendForApprovalBy = rentalPaymentRequest.SendForApprovalBy,
                    SendForApprovalTime = rentalPaymentRequest.SendForApprovalTime,
                    IsOnlineTransferred = rentalPaymentRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = rentalPaymentRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = rentalPaymentRequest.OnlineTransferredTime,

                    IsBudgetApproved = rentalPaymentRequest.IsBudgetApproved,
                    BudgetApprovedBy = rentalPaymentRequest.BudgetApprovedBy,
                    BudgetApprovedTime = rentalPaymentRequest.BudgetApprovedTime,

                    //ForwardedFrom = User.Identity.Name,
                    //ForwardedTo = User.Identity.Name,
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
        public ActionResult RentalPaymentRequestFinanceValidations_Create([DataSourceRequest]DataSourceRequest request, RentalPaymentRequestValidation rentalPaymentRequestValidation, int id)
        {
            var record = db.RentalPaymentRequests.Find(id);
            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "cannot manipulate this record,it is already approved");

            //var lines = db.RentalPaymentRequestLines.Where(x => x.RentalPaymentRequestId == id);
            //if (!lines.Any())
            //    ModelState.AddModelError("lines", "Please add line items");

            //if (record != null && record.IsOnlineApproved)
            //    ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = db.RentalPaymentRequestValidations
                    .Where(x => x.RentalPaymentRequestId == id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (rentalPaymentRequestValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsOnlineApproved)
                        {
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                            //#region approval notification

                            //NotifModels notif = new NotifModels
                            //{
                            //    UserId = record.ForwardedFrom,
                            //    URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApPaymentRequestSelfCare, record.PaymentRequestId),
                            //    Message =
                            //        $"Payment request document {record.PaymentRequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            //};

                            //Notify.NotifyUser(notif);

                            //#endregion
                        }
                        //else
                        //{
                        //    record.Status = "Partially Approved";
                        //    record.IsDepartmentOnlineApproved = false;
                        //    record.IsVoid = false;
                        //    record.OnlineApprovedBy = User.Identity.Name;
                        //    record.OnlineApprovedTime = DateTime.Now;
                        //    db.Entry(record).State = EntityState.Modified;
                        //    db.SaveChanges();

                        //    //#region approval notification

                        //    //NotifModels notif = new NotifModels
                        //    //{
                        //    //    UserId = record.ForwardedFrom,
                        //    //    URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.ApPaymentRequestSelfCare, record.PaymentRequestId),
                        //    //    Message =
                        //    //        $"Payment request document {record.PaymentRequestId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                        //    //};

                        //    //Notify.NotifyUser(notif);

                        //    //#endregion

                        //    //#region forward to next approver

                        //    //decimal docamount = record.PaymentRequestLines.Where(x => x.PaymentRequestId == record.PaymentRequestId).Sum(x => x.TotalAmount);
                        //    //DocApprovalWorkflowHelper.ForwardToApprover(record.PaymentRequestId, docamount, "",
                        //    //    record.ForwardedTo, ApprovalDocumentType.ApPaymentRequestSelfCare, false,(int)record.CurrentApprovalStep);

                        //    //#endregion
                        //}
                    }

                    if (rentalPaymentRequestValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsDepartmentSendForApproval = false;
                        record.Status = "Rejected";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new RentalPaymentRequestValidation
                    {
                        RentalPaymentRequestId = id,
                        Status = rentalPaymentRequestValidation.Status,
                        Remark = rentalPaymentRequestValidation.Remark,
                        OrgBranchName = rentalPaymentRequestValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = rentalPaymentRequestValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = rentalPaymentRequestValidation.ModifiedBy
                    };

                    db.RentalPaymentRequestValidations.Add(entity);
                    db.SaveChanges();
                    rentalPaymentRequestValidation.RentalPaymentRequestValidationId = entity.RentalPaymentRequestValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finace payment request approval", "Success", User.Identity.Name, $"approved request {entity.RentalPaymentRequestValidationId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { rentalPaymentRequestValidation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceRentalPaymentRequests_Update([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
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
                };
                db.RentalPaymentRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance rental payment request update", "Success", User.Identity.Name, $"updated finance rental payment request {entity.RentalPaymentRequestId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalPaymentRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FinanceRentalPaymentRequests_Destroy([DataSourceRequest] DataSourceRequest request, RentalPaymentRequest rentalPaymentRequest)
        {
            if (ModelState.IsValid)
            {
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
                    DurationFrom = rentalPaymentRequest.DurationFrom,
                    DurationTo = rentalPaymentRequest.DurationTo,
                    OrgStructureId = rentalPaymentRequest.OrgStructureId,
                    ProcurementSupplierID = rentalPaymentRequest.ProcurementSupplierID
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}