﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Global;
using System.IO;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class LeaveRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext dbApplication = new ApplicationDbContext();
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
            ViewData["EmployeeName"] = CacheData.GetEmployeeCaches().Select(
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
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
               i => new
               {
                   Description = i.Name,
                   LookupId = i.Id,
               }).AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
        }

        private void GetNotiofied(LeaveRequest leaveRequest)
        {
            NotifModels notif = new NotifModels
            {
                UserId = UserInformationHelper.GetEmployeeUserId(leaveRequest.EmployeeId),
                Title = $"You have a leave Request {leaveRequest.FromAmharic} - {leaveRequest.ToAmharic} for {leaveRequest.LeaveDays}",
                Message = "Leave.",
                URL = "/HRM/LeaveRequest"
            };

            Notify.NotifyUser(notif);
        }

        public ActionResult LeaveRequest_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);

            if (employeeIds == null || !employeeIds.Any())
            {
                return Json(new DataSourceResult { Data = new List<object>(), Total = 0 }, JsonRequestBehavior.AllowGet);
            }

            // Direct projection with targeted subquery for the latest RequestStatus
            var query = from lr in db.LeaveRequest.AsNoTracking()
                        where employeeIds.Contains(lr.EmployeeId)
                              && lr.IsSendForApproval
                        let latestStatus = db.RequestesStatus
                            .Where(r => r.ReferenceType == "Leave" && r.RequestId == lr.LeaveRequestId)
                            .OrderByDescending(r => r.RequestStatusId)
                            .Select(r => (RequestStatus?)r.Status)
                            .FirstOrDefault()
                        orderby lr.RequestDate descending
                        select new
                        {
                            lr.LeaveRequestId,
                            lr.EmployeeId,
                            lr.BranchId,
                            lr.BusinessUnitId,
                            lr.LookupId,
                            lr.From,
                            lr.GlFiscalYearId,
                            lr.To,
                            lr.LeaveDays,
                            lr.LeaveActionTypes,
                            lr.Type,
                            lr.LeaveBalance,
                            lr.Hours,
                            lr.Day,
                            lr.ExactDeserveLeaveDays,
                            lr.Reason,
                            lr.Remark,
                            lr.RequestDate,
                            lr.FromAmharic,
                            lr.ToAmharic,

                            // Direct status checks from correlated subquery
                            IsApproved = latestStatus == RequestStatus.APPROVE,
                            IsChangeInDepartment = latestStatus == RequestStatus.DEPARTMENT_APPROVAL,
                            IsPending = latestStatus == RequestStatus.PEND,
                            IsRejected = latestStatus == RequestStatus.REJECT,

                            EmployeeCode = lr.Employee.EmployeeCode,
                            lr.DepartmentOnlineApprovedBy,
                            lr.Status,
                            lr.DepartmentSendForApprovalBy,
                            lr.DepartmentSendForApprovalTime,
                            lr.IsDepartmentSendForApproval,
                            lr.DepartmentOnlineApprovedTime,
                            lr.IsForOther,
                            lr.IsDepartmentOnlineApproved,
                            lr.OnlineApprovedBy,
                            lr.OnlineApprovedTime,
                            lr.IsOnlineApproved,
                            lr.MorningOrAfternoon,
                            lr.IsSendForApproval,
                            lr.ToTime,
                            lr.ModifiedBy,
                            lr.LastModified,
                            lr.CreatedBy,
                            lr.DateCreated,
                            lr.IsVoid,
                            lr.VoidBy,
                            lr.VoidTime
                        };

            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Update([DataSourceRequest] DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();

            var entity = new LeaveRequestEditService(db).Update(leaveRequest, User.Identity.Name, ModelState);
            if (entity != null)
            {
                UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequest", "Success", User.Identity.Name, $"{User.Identity.Name} updated leave request of id {leaveRequest.LeaveRequestId}", Modules.HRM);
                GetNotiofied(entity);
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public string GetTotalLeaveAmount(int empId, DateTime? from, DateTime? to, string fromAm, string toAm, MorningOrAfternoon morningOrAfternoon)
        {
            DateTime fromDate = !string.IsNullOrEmpty(fromAm) ? HrHelper.ParseEthiopicDate(fromAm) : from ?? DateTime.Now;
            DateTime toDate = !string.IsNullOrEmpty(toAm) ? HrHelper.ParseEthiopicDate(toAm) : to ?? DateTime.Now;
            return new LeaveRequestEditService(db).CalculateAmounts(empId, fromDate, toDate, morningOrAfternoon);
        }

        [HttpPost]
        public string GetTotalLeaveAmountGorg(int empId, DateTime? from, DateTime? to, MorningOrAfternoon morningOrAfternoon)
        {
            return new LeaveRequestEditService(db).CalculateAmounts(empId, from ?? DateTime.Now, to ?? DateTime.Now, morningOrAfternoon);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Destroy([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
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
                        LookupId = leaveRequest.LookupId,
                        From = leaveRequest.From,
                        To = leaveRequest.To,
                        LeaveDays = leaveRequest.LeaveDays,
                        Type = leaveRequest.Type,
                        Reason = leaveRequest.Reason,
                        GlFiscalYearId = leaveRequest.GlFiscalYearId,
                        Remark = leaveRequest.Remark,
                        LeaveBalance = leaveRequest.LeaveBalance,
                        FromAmharic = leaveRequest.FromAmharic,
                        ToAmharic = leaveRequest.ToAmharic,
                        EmployeeCode = leaveRequest.EmployeeCode,
                        IsForOther = leaveRequest.IsForOther,
                        Day = leaveRequest.Day,
                        MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                        Hours = leaveRequest.Hours
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
        public int SetEmployee(string x)
        {

            if (x == "true")
            {
                var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var list = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId).ToList();

                return list.Count();

            }
            return 0;
        }

        public string CheckStatus(int masterId)
        {
            var requestStatus = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave").OrderByDescending(x=>x.RequestStatusId).FirstOrDefault();
            if (requestStatus != null)
            {
                if (requestStatus.Status== RequestStatus.APPROVE)
                {
                    return "approved";
                }
                else if (requestStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                {
                    return "Dapproved";
                }
                else if (requestStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                {
                    return "waiting";
                }
                else if (requestStatus.Status == RequestStatus.PEND)
                {
                    return "pend";
                }
                else if (requestStatus.Status == RequestStatus.REJECT)
                {
                    return "reject";
                }

                return "false";
            }
            return "false";
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
