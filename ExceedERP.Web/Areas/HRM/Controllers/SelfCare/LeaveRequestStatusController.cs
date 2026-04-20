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

        private JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
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
        public ActionResult UpdateLeaveReturnStatus()
        {
            var leaveReturns = db.LeaveReturns.Where(x=>x.IsOnlineApproved);
            foreach (var leaveReturn in leaveReturns)
            {
                var leaveRequests = db.LeaveRequest
                    .Where(x=>x.IsOnlineApproved
                    &&!x.IsVoid).ToList()
                    .Where(x => x.EmployeeId==leaveReturn.EmployeeId)
                    .Where(x => x.LeaveRequestId == leaveReturn.LeaveRequestId
                    ||(x.From<=leaveReturn.From&&x.To>=leaveReturn.To));
                foreach (var leaveRequest in leaveRequests)
                {

                    leaveRequest.IsVoid = true;
                    leaveRequest.VoidBy = User.Identity.Name;
                    leaveRequest.VoidTime = DateTime.Now;
                    leaveReturn.LeaveRequestId= leaveRequest.LeaveRequestId;
                    db.Entry(leaveReturn).State = EntityState.Modified;
                    db.Entry(leaveRequest).State = EntityState.Modified;
                   
                }
                db.SaveChanges();
            }
            return View("Success");
        }

        public bool CheckStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId 
                                                    &&x.ReferenceType == "Leave")
                                                    .OrderByDescending(x=>x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null&& status.Status == RequestStatus.DEPARTMENT_APPROVAL)
            {


                return true;
            }
            return false;
        }
        public bool CheckPendStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId 
                                                    &&x.ReferenceType == "Leave")
                                                    .OrderByDescending(x=>x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null&& status.Status == RequestStatus.PEND)
            {


                return true;
            }
            return false;
        }
        public bool CheckRejectStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId 
                                                    &&x.ReferenceType == "Leave")
                                                    .OrderByDescending(x=>x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null&& status.Status == RequestStatus.REJECT)
            {


                return true;
            }
            return false;
        }
        public string GetEmployeeCode(int id)
        {
            string code = "";
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                code = person.EmployeeCode;
            }
            return code;
        }
        private decimal GetTotalLeaveBalance(int id)
        {
            var leaveEntitlements = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            return leaveEntitlements.Any() ? leaveEntitlements.Sum(x => x.Balance) : 0;
        }
        public ActionResult LeaveRequest_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            // join LeaveRequest with the latest RequestStatus for each Leave
            var query =
                from lr in db.LeaveRequest
                where myBranches.Contains(lr.BranchId) && lr.IsDepartmentOnlineApproved
                join rs in
                    (
                        from r in db.RequestesStatus
                        where r.ReferenceType == "Leave"
                        group r by r.RequestId into g
                        select new
                        {
                            RequestId = g.Key,
                            LatestStatus = g.OrderByDescending(x => x.RequestStatusId).FirstOrDefault()
                        }
                    )
                    on lr.LeaveRequestId equals rs.RequestId into rsGroup
                from rs in rsGroup.DefaultIfEmpty()
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
                    IsApproved = rs.LatestStatus != null && rs.LatestStatus.Status == RequestStatus.DEPARTMENT_APPROVAL,
                    IsPending = rs.LatestStatus != null && rs.LatestStatus.Status == RequestStatus.PEND,
                    IsRejected = rs.LatestStatus != null && rs.LatestStatus.Status == RequestStatus.REJECT,

                    EmployeeCode = lr.Employee.EmployeeCode, // use navigation property instead of separate query
                   Day = lr.Day,
                    Status = lr.Status,
                    DepartmentSendForApprovalBy = lr.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = lr.DepartmentSendForApprovalTime,
                    IsDepartmentSendForApproval = lr.IsDepartmentSendForApproval,
                    DepartmentOnlineApprovedBy = lr.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = lr.DepartmentOnlineApprovedTime,
                    IsDepartmentOnlineApproved = lr.IsDepartmentOnlineApproved,
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

        //public ActionResult LeaveRequest_Read([DataSourceRequest]DataSourceRequest request)
        //{

        //    // var leaverequest = db.LeaveRequest.ToList();
        //    //List<LeaveRequest> leaves = new List<LeaveRequest>();
        //    //foreach (var m in leaverequest)
        //    //{
        //    //    db = new ExceedDbContext();
        //    //    var status = db.RequestesStatus.FirstOrDefault(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.RequestId == m.LeaveRequestId && x.ReferenceType == "Leave");
        //    //    if (status != null)
        //    //    {
        //    //        leaves.Add(m);
        //    //    }

        //    //}

        //    //var leaveRequestIds = db.RequestesStatus.Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Leave").Select(x => x.RequestId);
        //    //var leaveRequests = db.LeaveRequest.Where(x=> leaveRequestIds.Contains(x.LeaveRequestId)).ToList();

        //    List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

        //    //var placements = db.Placements.Where(x => branches.Contains(x.BranchId.ToString())).Select(x => x.EmployeeId).ToList();

        //    var leaveRequests = db.LeaveRequest.Where(x =>  myBranches.Contains(x.BranchId)&&x.IsDepartmentOnlineApproved).OrderByDescending(x=>x.RequestDate).AsQueryable();

        //    DataSourceResult result = leaveRequests.ToDataSourceResult(request, leaveRequest => new
        //    {
        //        LeaveRequestId = leaveRequest.LeaveRequestId,
        //        EmployeeId = leaveRequest.EmployeeId,
        //        BranchId = leaveRequest.BranchId,
        //        BusinessUnitId = leaveRequest.BusinessUnitId,
        //        From = leaveRequest.From,
        //        GlFiscalYearId = leaveRequest.GlFiscalYearId,
        //        To = leaveRequest.To,
        //        LeaveDays = leaveRequest.LeaveDays,
        //        //LeaveBalance = GetTotalLeaveBalance(leaveRequest.EmployeeId),
        //        LeaveBalance = leaveRequest.LeaveBalance,
        //        Type = leaveRequest.Type,
        //        Reason = leaveRequest.Reason,
        //        Remark = leaveRequest.Remark,
        //        RequestDate = leaveRequest.RequestDate,
        //        FromAmharic = leaveRequest.FromAmharic,
        //        ToAmharic = leaveRequest.ToAmharic,
        //        //LeaveBalance = leaveRequest.LeaveBalance,
        //        IsApproved = CheckStatusForBackColor(leaveRequest.LeaveRequestId),
        //        IsPending = CheckPendStatusForBackColor(leaveRequest.LeaveRequestId),
        //        IsRejected = CheckRejectStatusForBackColor(leaveRequest.LeaveRequestId),
        //        EmployeeCode = GetEmployeeCode(leaveRequest.EmployeeId),
        //        Day = leaveRequest.Day,
        //        Status = leaveRequest.Status,
        //        DepartmentSendForApprovalBy = leaveRequest.DepartmentSendForApprovalBy,
        //        DepartmentSendForApprovalTime = leaveRequest.DepartmentSendForApprovalTime,
        //        IsDepartmentSendForApproval = leaveRequest.IsDepartmentSendForApproval,
        //        DepartmentOnlineApprovedBy = leaveRequest.DepartmentOnlineApprovedBy,
        //        DepartmentOnlineApprovedTime = leaveRequest.DepartmentOnlineApprovedTime,
        //        IsDepartmentOnlineApproved = leaveRequest.IsDepartmentOnlineApproved,
        //        OnlineApprovedBy = leaveRequest.OnlineApprovedBy,
        //        OnlineApprovedTime = leaveRequest.OnlineApprovedTime,
        //        IsOnlineApproved = leaveRequest.IsOnlineApproved,
        //        IsVoid = leaveRequest.IsVoid,
        //        VoidBy = leaveRequest.VoidBy,
        //        VoidTime = leaveRequest.VoidTime,
        //    });

        //    return Json(result);
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Create([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            if (leaveRequest.EmployeeId != 0)
            {
                var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
                if (placemnt != null)
                {
                    leaveRequest.BranchId = placemnt.BranchId;
                    leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
            if (ModelState.IsValid)
            {

                var entity = new LeaveRequest
                {
                    EmployeeId = leaveRequest.EmployeeId,
                    BranchId = leaveRequest.BranchId,
                    BusinessUnitId = leaveRequest.BusinessUnitId,
                    From = leaveRequest.From,
                    To = leaveRequest.To,
                    LeaveDays = leaveRequest.LeaveDays,
                    Type = leaveRequest.Type,
                    Reason = leaveRequest.Reason,
                    Remark = leaveRequest.Remark,
                    GlFiscalYearId = leaveRequest.GlFiscalYearId,
                    RequestDate = DateTime.Now,
                };
                          
                db.LeaveRequest.Add(entity);
                db.SaveChanges();
                leaveRequest.LeaveRequestId = entity.LeaveRequestId;
               
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Update([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            if (leaveRequest.EmployeeId != 0)
            {
                var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
                if (placemnt != null)
                {
                    leaveRequest.BranchId = placemnt.BranchId;
                    leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
            if (ModelState.IsValid)
            {

                var entity = new LeaveRequest
                {
                    LeaveRequestId = leaveRequest.LeaveRequestId,
                    EmployeeId = leaveRequest.EmployeeId,
                    BranchId = leaveRequest.BranchId,
                    BusinessUnitId = leaveRequest.BusinessUnitId,
                    From = leaveRequest.From,
                    To = leaveRequest.To,
                    LeaveDays = UpdateLeaveDays(leaveRequest.LeaveRequestId, leaveRequest.Type, leaveRequest.LeaveDays),
                    Type = leaveRequest.Type,
                    Reason = leaveRequest.Reason,
                    Remark = leaveRequest.Remark,
                    GlFiscalYearId = leaveRequest.GlFiscalYearId,
                    RequestDate = DateTime.Now,
                };

                db.LeaveRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }
        public decimal UpdateLeaveDays(int id, LeaveType type, decimal Amount)
        {
            if (type == LeaveType.ANNUAL)
            {
                var leaverequest = db.LeaveRequest.Where(x => x.LeaveRequestId == id).FirstOrDefault();
                if (leaverequest != null)
                {
                    return leaverequest.LeaveDays;
                }

            }
            else
            {
                return Amount;
            }
            return Amount;

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Destroy([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            if (leaveRequest.IsOnlineApproved)
            {
                ModelState.AddModelError("", "you can't delete approved requestes");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new LeaveRequest
                    {
                        LeaveRequestId = leaveRequest.LeaveRequestId,
                        EmployeeId = leaveRequest.EmployeeId,
                        BranchId = leaveRequest.BranchId,
                        BusinessUnitId = leaveRequest.BusinessUnitId,
                        From = leaveRequest.From,
                        To = leaveRequest.To,
                        LeaveDays = leaveRequest.LeaveDays,
                        Type = leaveRequest.Type,
                        Reason = leaveRequest.Reason,
                        Remark = leaveRequest.Remark,
                        GlFiscalYearId = leaveRequest.GlFiscalYearId,
                        RequestDate = leaveRequest.RequestDate,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted leave request of employee code  " + leaveRequest.EmployeeCode);
                    #endregion
                    db.LeaveRequest.Attach(entity);
                    db.LeaveRequest.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
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
