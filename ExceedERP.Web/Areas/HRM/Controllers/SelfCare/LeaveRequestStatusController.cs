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
                return "true";
            }
            return "false";
        }
        public bool CheckStatusForBackColor(int leaveId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == leaveId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Leave").FirstOrDefault();
            if (reques != null)
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
            IQueryable<LeaveRequest> leaverequest = db.LeaveRequest;

            List<LeaveRequest> leaves = new List<LeaveRequest>();
            foreach (var m in leaverequest)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.RequestId == m.LeaveRequestId && x.ReferenceType == "Leave").FirstOrDefault();
                if (status != null)
                {
                    leaves.Add(m);
                }

            }

            //var brance = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            //var placements = db.Placements.Where(x => brance.Contains(x.BranchId.ToString())).Select(x=>x.EmployeeId).ToList();

            //leaves = leaves.Where(x=>placements.Contains(x.EmployeeId)).ToList();

            List<int> employees = inputs.GetUserBranchAllEmployees(User.Identity.Name);
            leaves = leaves.Where(x => employees.Contains(x.EmployeeId)).ToList();


            leaverequest = leaves.AsQueryable().OrderBy(x => x.RequestDate);
            DataSourceResult result = leaverequest.ToDataSourceResult(request, leaveRequest => new
            {
                LeaveRequestId = leaveRequest.LeaveRequestId,
                EmployeeId = leaveRequest.EmployeeId,
                From = leaveRequest.From,
                GlFiscalYearId = leaveRequest.GlFiscalYearId,
                To = leaveRequest.To,
                LeaveDays = leaveRequest.LeaveDays,
                Type = leaveRequest.Type,
                Reason = leaveRequest.Reason,
                Remark = leaveRequest.Remark,
                RequestDate = leaveRequest.RequestDate,
                FromAmharic = leaveRequest.FromAmharic,
                ToAmharic = leaveRequest.ToAmharic,
                LeaveBalance = leaveRequest.LeaveBalance,
                IsApproved = CheckStatusForBackColor(leaveRequest.LeaveRequestId),
                EmployeeCode = GetEmployeeCode(leaveRequest.EmployeeId),
                Day = leaveRequest.Day,
                DepartmentSendForApprovalBy = leaveRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leaveRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leaveRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = leaveRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leaveRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leaveRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = leaveRequest.OnlineApprovedBy,
                OnlineApprovedTime = leaveRequest.OnlineApprovedTime,
                IsOnlineApproved = leaveRequest.IsOnlineApproved,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Create([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = new LeaveRequest
                {
                    EmployeeId = leaveRequest.EmployeeId,
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
            if (ModelState.IsValid)
            {
                var entity = new LeaveRequest
                {
                    LeaveRequestId = leaveRequest.LeaveRequestId,
                    EmployeeId = leaveRequest.EmployeeId,
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
