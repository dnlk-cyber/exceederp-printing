using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
    public class EmployeeTransferRequestHRsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var employeeIds = db.Placements.Select(x => x.EmployeeId).ToList();
            ViewData["EmployeeList"] = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).Select(x => new { EmployeeId = x.EmployeeId, EmployeeCode = x.EmployeeCode, FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();           
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;

        }
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult EmployeeTransferRequests_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeTransferRequest> employeeTransfers = db.EmployeeTransferRequests.Where(x=>x.IsDepartmentOnlineApproved);
            DataSourceResult result = employeeTransfers.ToDataSourceResult(request, employeeTransfer => new
            {

                EmployeeTransferRequestId = employeeTransfer.EmployeeTransferRequestId,
                EmployeeId = employeeTransfer.EmployeeId,
                OrgStructureId = employeeTransfer.OrgStructureId,
                BranchId = employeeTransfer.BranchId,
                BusinessUnitId = employeeTransfer.BusinessUnitId,
                Reason = employeeTransfer.Reason,
                RequestDate = employeeTransfer.RequestDate,
                Remark = employeeTransfer.Remark,
                Status = employeeTransfer.Status,
                ForwardedFrom = employeeTransfer.ForwardedFrom,
                ForwardedTo = employeeTransfer.ForwardedTo,
                FromBranchId = employeeTransfer.FromBranchId,
                FromBusinessUnitId = employeeTransfer.FromBusinessUnitId,
                CurrentApprovalStep = employeeTransfer.CurrentApprovalStep,
                CreatedBy = employeeTransfer.CreatedBy,
                DateCreated = employeeTransfer.DateCreated,
                DepartmentOnlineApprovedBy = employeeTransfer.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = employeeTransfer.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalBy = employeeTransfer.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = employeeTransfer.DepartmentSendForApprovalTime,
                IsDepartmentOnlineApproved = employeeTransfer.IsDepartmentOnlineApproved,
                IsDepartmentSendForApproval = employeeTransfer.IsDepartmentSendForApproval,
                IsFinalApproval = employeeTransfer.IsFinalApproval,
                IsOnlineApproved = employeeTransfer.IsOnlineApproved,
                IsOnlineTransferred = employeeTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy = employeeTransfer.IsOnlineTransferredBy,
                IsSendForApproval = employeeTransfer.IsSendForApproval,
                LastModified = employeeTransfer.LastModified,
                ModifiedBy = employeeTransfer.ModifiedBy,
                OnlineApprovedBy = employeeTransfer.OnlineApprovedBy,
                OnlineApprovedTime = employeeTransfer.OnlineApprovedTime,
                OnlineTransferredTime = employeeTransfer.OnlineTransferredTime,
                SendForApprovalBy = employeeTransfer.SendForApprovalBy,
                SendForApprovalTime = employeeTransfer.SendForApprovalTime,
                IsVoid = employeeTransfer.IsVoid,
                VoidTime = employeeTransfer.VoidTime,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Create([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                employeeTransfer.DateCreated = DateTime.Now;
                employeeTransfer.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(employeeTransfer);
                db.EmployeeTransferRequests.Add(entity);
                db.SaveChanges();
                employeeTransfer.EmployeeTransferRequestId = entity.EmployeeTransferRequestId;
            }

            return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Update([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                employeeTransfer.LastModified = DateTime.Now;
                employeeTransfer.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(employeeTransfer);
                db.EmployeeTransferRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeTransferRequests_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeTransferRequest employeeTransfer)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(employeeTransfer);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee Transfer   information of applicant" + employeeTransfer.EmployeeTransferRequestId);
                #endregion
                db.EmployeeTransferRequests.Attach(entity);
                db.EmployeeTransferRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeTransfer }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeTransferRequest GetMappedObject(EmployeeTransferRequest employeeTransfer)
        {
            var entity = new EmployeeTransferRequest
            {
                EmployeeTransferRequestId=employeeTransfer.EmployeeTransferRequestId,
                EmployeeId =employeeTransfer.EmployeeId,
                OrgStructureId =employeeTransfer.OrgStructureId,
                BranchId=employeeTransfer.BranchId,
                BusinessUnitId=employeeTransfer.BusinessUnitId,
                Reason=employeeTransfer.Reason,
                RequestDate=employeeTransfer.RequestDate,
                Remark=employeeTransfer.Remark,
                Status=employeeTransfer.Status,
                ForwardedFrom=employeeTransfer.ForwardedFrom,
                ForwardedTo=employeeTransfer.ForwardedTo,
                FromBranchId=employeeTransfer.FromBranchId,
                FromBusinessUnitId=employeeTransfer.FromBusinessUnitId,
                CurrentApprovalStep=employeeTransfer.CurrentApprovalStep,
                CreatedBy=employeeTransfer.CreatedBy,
                DateCreated=employeeTransfer.DateCreated,
                DepartmentOnlineApprovedBy=employeeTransfer.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime=employeeTransfer.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalBy=employeeTransfer.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime=employeeTransfer.DepartmentSendForApprovalTime,
                IsDepartmentOnlineApproved=employeeTransfer.IsDepartmentOnlineApproved,
                IsDepartmentSendForApproval=employeeTransfer.IsDepartmentSendForApproval,
                IsFinalApproval=employeeTransfer.IsFinalApproval,
                IsOnlineApproved=employeeTransfer.IsOnlineApproved,
                IsOnlineTransferred=employeeTransfer.IsOnlineTransferred,
                IsOnlineTransferredBy=employeeTransfer.IsOnlineTransferredBy,
                IsSendForApproval=employeeTransfer.IsSendForApproval,
                LastModified=employeeTransfer.LastModified,
                ModifiedBy=employeeTransfer.ModifiedBy,
                OnlineApprovedBy = employeeTransfer.OnlineApprovedBy,
                OnlineApprovedTime=employeeTransfer.OnlineApprovedTime,
                OnlineTransferredTime=employeeTransfer.OnlineTransferredTime,
                SendForApprovalBy=employeeTransfer.SendForApprovalBy,
                SendForApprovalTime=employeeTransfer.SendForApprovalTime,
                IsVoid = employeeTransfer.IsVoid,
                VoidTime = employeeTransfer.VoidTime,

            };
            return entity;
        }
        public JsonResult SendforDepartmentApproval(int id)
        {
            var record = db.EmployeeTransferRequests.Find(id);
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

            var lines = db.TransferRequestDetails.Where(x => x.EmployeeTransferRequestId == id);
            if (!lines.Any())
            {
                errMsg = "Please add job title .";
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
                //var validations = record.ApTravelOrderDepartmentValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                //if (validations.Any())
                //    isNewRecord = false;
                //decimal docamount = record.AdvanceAmount;
                //int docid = record.ApTravelOrderId;
                //int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                //DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.ApTravelOrderSelfCare, isNewRecord, step);
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
