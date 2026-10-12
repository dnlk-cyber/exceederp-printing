﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
 using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using System.IO;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRDepartmentHead)]
    public class LeaveRequestStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {

            ViewData["PeriodList"] = CacheData.GetFiscalYearCaches().Select(
                             i => new
                             {
                                 Name = i.Name,
                                 GlFiscalYearId = i.Id,
                             }).AsQueryable();



            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
                  i => new
                  {
                      FullNameAmharic = i.AmharicName,
                      FullNameaEnglish = i.Name,
                      FullName = i.Name,
                      FullNameCode = i.Code + " " + i.Name,
                      EmployeeCode = i.Code,
                      EmployeeId = i.Id,

                  }).AsQueryable();

            ViewData["branches"] = CacheData.GetBranchCaches().Select(
              i => new
              {
                  Name = i.Name,
                  BranchId = i.Id,
              }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
               }).AsQueryable();
            ViewData["Department"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
              i => new
              {
                  Description = i.Name,
                  LookupId = i.Id,
              }).AsQueryable();

        }
        public JsonResult UpdateLeaveRequestinfo()
        {
            try
            {
                // Load current placements
                var currentPlacements = db.Placements
                    .Where(x => x.isCurrent)
                    .ToList();

                if (!currentPlacements.Any())
                {
                    return Json(new
                    {
                        success = true,
                        message = "No current placements found."
                    }, JsonRequestBehavior.AllowGet);
                }

                // Load employees who have current placements
                var employeeIds = currentPlacements
                    .Select(p => p.EmployeeId)
                    .Distinct()
                    .ToList();

                // Find all leave requests missing LookupId
                var leaveRequests = db.LeaveRequest
                    .Where(e => employeeIds.Contains(e.EmployeeId) &&
                                (!e.LookupId.HasValue || e.LookupId == 0))
                    .ToList();

                // Dictionary for quick lookup
                var currentPlacementDic = currentPlacements
                    .ToDictionary(p => p.EmployeeId, p => p);

                // Update each leave request
                foreach (var leaveRequest in leaveRequests)
                {
                    if (currentPlacementDic.TryGetValue(leaveRequest.EmployeeId, out var placement))
                    {
                        leaveRequest.LookupId = placement.LookupId;
                        leaveRequest.BusinessUnitId = placement.BusinessUnitId;
                        leaveRequest.BranchId = placement.BranchId;
                        db.Entry(leaveRequest).State = EntityState.Modified;
                    }
                }

                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = $"Successfully updated {leaveRequests.Count} leave requests."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Unexpected error occurred.",
                    details = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "false";
            }
            return "false";
        }

        public ActionResult LeaveRequest_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            // company workflow setting
            var workflowSetting = db.CompanyLeaveWorkflowSettings.FirstOrDefault();

            bool useProjectManagerApproval = workflowSetting != null && workflowSetting.UseProjectManagerApproval;

            // base query
            var leaveRequests = db.LeaveRequest.Where(lr =>
                myBranches.Contains(lr.BranchId) &&
                lr.IsDepartmentOnlineApproved);

            // if company uses PM approval,
            // only show PM approved requests for final HR approval
            if (useProjectManagerApproval)
            {
                leaveRequests = leaveRequests.Where(lr =>
                    lr.IsProjectManagerApproved == true);
            }
            var hrManagerSetting = db.CompanyApprovalSettings.FirstOrDefault(x => x.IsEnabled &&x.Module==Core.Domain.HRM.Company.ApprovalModule.Leave&& x.Role == Core.Domain.HRM.Company.ApprovalRole.HRManager);

            bool hrManagerEabled = hrManagerSetting != null;
            if (hrManagerEabled)
            {
                leaveRequests = leaveRequests.Where(lr =>
                    lr.IsHRManagerApproved == true);
            }

            // targeted correlated subquery instead of grouping the whole RequestesStatus table (was very slow on first load)
            var query =
                from lr in leaveRequests
                let latestStatus = db.RequestesStatus
                    .Where(r => r.ReferenceType == "Leave" && r.RequestId == lr.LeaveRequestId)
                    .OrderByDescending(r => r.RequestStatusId)
                    .Select(r => (RequestStatus?)r.Status)
                    .FirstOrDefault()
                orderby lr.RequestDate descending
                select new
                {
                    LeaveRequestId = lr.LeaveRequestId,
                    EmployeeId = lr.EmployeeId,
                    BranchId = lr.BranchId,
                    BusinessUnitId = lr.BusinessUnitId,
                    LookupId = lr.LookupId,
                    From = lr.From,
                    GlFiscalYearId = lr.GlFiscalYearId,
                    To = lr.To,
                    LeaveDays = lr.LeaveDays,
                    LeaveBalance = lr.LeaveBalance,
                    Type = lr.Type,
                    Reason = lr.Reason,
                    Remark = lr.Remark,
                    RequestDate = lr.RequestDate,
                    FromAmharic = lr.FromAmharic,
                    ToAmharic = lr.ToAmharic,

                    // ✅ compute all 3 statuses in SQL
                    IsApproved = latestStatus == RequestStatus.DEPARTMENT_APPROVAL,
                    IsPending = latestStatus == RequestStatus.PEND,
                    IsRejected = latestStatus == RequestStatus.REJECT,

                    EmployeeCode = lr.Employee.EmployeeCode, // use navigation property instead of separate query
                   Day = lr.Day,
                    Status = lr.Status,
                    DepartmentSendForApprovalBy = lr.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = lr.DepartmentSendForApprovalTime,
                    IsDepartmentSendForApproval = lr.IsDepartmentSendForApproval,
                    DepartmentOnlineApprovedBy = lr.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = lr.DepartmentOnlineApprovedTime,
                    IsDepartmentOnlineApproved = lr.IsDepartmentOnlineApproved,
                    IsProjectManagerApproved = lr.IsProjectManagerApproved,
                    ProjectManagerApprovedBy = lr.ProjectManagerApprovedBy,
                    ProjectManagerApprovedDate = lr.ProjectManagerApprovedDate,
                    OnlineApprovedBy = lr.OnlineApprovedBy,
                    OnlineApprovedTime = lr.OnlineApprovedTime,
                    IsOnlineApproved = lr.IsOnlineApproved,
                    IsVoid = lr.IsVoid,
                    VoidBy = lr.VoidBy,
                    VoidTime = lr.VoidTime
                };

            DataSourceResult result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadDocument(int leaveRequestId, HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + leaveRequestId.ToString() + "-" + AttachmentType.DOCUMENT), leaveRequestId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + leaveRequestId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = leaveRequestId.ToString(),
                            Description = leaveRequestId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = leaveRequestId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }

        public ActionResult DownloadDoument(string filename, string attachId)
        {
            int.TryParse(attachId, out int attachmentId);
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }

        public ActionResult RefreshBussinessUnitAndBranch()
        {
            var leaveRequests = db.LeaveRequest.ToList();
            foreach (var employeeLeave in leaveRequests)
            {
                var placement = db.Placements.Where(x => x.EmployeeId == employeeLeave.EmployeeId && x.isCurrent).FirstOrDefault();
                if(placement != null)
                {
                    employeeLeave.BranchId = placement.BranchId;
                    employeeLeave.BusinessUnitId = placement.BusinessUnitId;
                    db.LeaveRequest.Attach(employeeLeave);
                    db.Entry(employeeLeave).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
