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
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
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
        HRMInputs inputs = new HRMInputs();

        public ActionResult Index()
        {
            PopulateViewData();
            GetStatusType();
            GetPeriod();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = db.Person_Employee.AsQueryable();// list.Where(x => x.IsActive);
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            ViewData["Department"] = db.MainCompanyStructures.ToList();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits;
            ViewData["businessUnits"] = businessUnits;
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
            var requestStatus = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave").OrderByDescending(x => x.RequestStatusId).FirstOrDefault();
            if (requestStatus != null)
            {
                if (requestStatus.Status == RequestStatus.APPROVE)
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
        public bool CheckStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId
                                                    && x.ReferenceType == "Leave")
                                                    .OrderByDescending(x => x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null && status.Status == RequestStatus.DEPARTMENT_APPROVAL)
            {


                return true;
            }
            return false;
        }
        public bool CheckPendStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId
                                                    && x.ReferenceType == "Leave")
                                                    .OrderByDescending(x => x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null && status.Status == RequestStatus.PEND)
            {


                return true;
            }
            return false;
        }
        public bool CheckRejectStatusForBackColor(int leaveId)
        {
            var status = db.RequestesStatus.Where(x => x.RequestId == leaveId
                                                    && x.ReferenceType == "Leave")
                                                    .OrderByDescending(x => x.RequestStatusId)
                                                    .FirstOrDefault();
            if (status != null && status.Status == RequestStatus.REJECT)
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

        public ActionResult LeaveRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            var leaveRequestIds = db.RequestesStatus.Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Leave").Select(x => x.RequestId);
            var leaveRequests = db.LeaveRequest.Where(x=> leaveRequestIds.Contains(x.LeaveRequestId)).ToList();
            var branches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var placements = db.Placements.Where(x => branches.Contains(x.BranchId.ToString())).Select(x => x.EmployeeId).ToList();
            leaveRequests = leaveRequests.Where(x => placements.Contains(x.EmployeeId)).OrderByDescending(x=>x.RequestDate).ToList();
            DataSourceResult result = leaveRequests.ToDataSourceResult(request, leaveRequest => new
            {
                LeaveRequestId = leaveRequest.LeaveRequestId,
                EmployeeId = leaveRequest.EmployeeId,
                BranchId = leaveRequest.BranchId,
                BusinessUnitId = leaveRequest.BusinessUnitId,
                From = leaveRequest.From,
                GlFiscalYearId = leaveRequest.GlFiscalYearId,
                To = leaveRequest.To,
                LeaveDays = leaveRequest.LeaveDays,
                //LeaveBalance = GetTotalLeaveBalance(leaveRequest.EmployeeId),
                LeaveBalance = leaveRequest.LeaveBalance,
                Type = leaveRequest.Type,
                Reason = leaveRequest.Reason,
                Remark = leaveRequest.Remark,
                RequestDate = leaveRequest.RequestDate,
                FromAmharic = leaveRequest.FromAmharic,
                ToAmharic = leaveRequest.ToAmharic,
                //LeaveBalance = leaveRequest.LeaveBalance,
                IsApproved = CheckStatusForBackColor(leaveRequest.LeaveRequestId),
                IsPending = CheckPendStatusForBackColor(leaveRequest.LeaveRequestId),
                IsRejected = CheckRejectStatusForBackColor(leaveRequest.LeaveRequestId),
                EmployeeCode = GetEmployeeCode(leaveRequest.EmployeeId),
                Day = leaveRequest.Day,
                Status = leaveRequest.Status,
                DepartmentSendForApprovalBy = leaveRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leaveRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leaveRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = leaveRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leaveRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leaveRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = leaveRequest.OnlineApprovedBy,
                OnlineApprovedTime = leaveRequest.OnlineApprovedTime,
                IsOnlineApproved = leaveRequest.IsOnlineApproved,
                IsVoid = leaveRequest.IsVoid,
                VoidBy = leaveRequest.VoidBy,
                VoidTime = leaveRequest.VoidTime,
            });

            return Json(result);
        }

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
