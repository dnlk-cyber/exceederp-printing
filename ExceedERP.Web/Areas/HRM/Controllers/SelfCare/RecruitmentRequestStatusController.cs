﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentApprover, HrmConstants.HRSelfServiceManager)]
    public class RecruitmentRequestStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["jobTitles"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).AsQueryable();
            GetDepartments();
            GetStatusType();
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            ViewData["Department"] = db.MainCompanyStructures.ToList();

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            ViewData["Branch"] = db.Branch.ToList();
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
            var reques = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Recruitment");
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.APPROVE);
                if (checkApprove != null)
                {
                    return "Approved";
                }
                var reject = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.REJECT);
                if (reject != null)
                {
                    return "Reject";
                }
                var deptApproval = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                if (deptApproval != null)
                {
                    return "De-Approved";
                }
                var pend = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.PEND);
                if (pend != null)
                {
                    return "Pend";
                }
                var waitingForApproval = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Recruitment" && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
                if (waitingForApproval != null)
                {
                    return "WaitingForApproval";
                }
                return "true";
            }
            return "false";
        }
     
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo =
                            db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        public ActionResult RecruitmentRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<RecruitmentRequest> recruitmentrequest = db.RecruitmentRequest.Where(x => mybraches.Contains(x.OrgBranchName)).OrderBy(x => x.Date);
            List<RecruitmentRequest> requests = new List<RecruitmentRequest>();
            foreach (var m in recruitmentrequest)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.RequestId == m.RecruitmentRequestId && x.ReferenceType == "Recruitment").FirstOrDefault();
                if (status != null)
                {
                    requests.Add(m);
                }
            }
            recruitmentrequest = requests.AsQueryable().OrderBy(x => x.Date);
            DataSourceResult result = recruitmentrequest.ToDataSourceResult(request, recruitmentRequest => new
            {
                RecruitmentRequestId = recruitmentRequest.RecruitmentRequestId,
                OrgStructureId = recruitmentRequest.OrgStructureId,
                BranchId = recruitmentRequest.BranchId,
                EmploymentType = recruitmentRequest.EmploymentType,
                LookupId = recruitmentRequest.LookupId,
                NoOfVaccantPostion = recruitmentRequest.NoOfVaccantPostion,
                NoOfCurrentStaff = recruitmentRequest.NoOfCurrentStaff,
                NoOfShouldBeStaff = recruitmentRequest.NoOfShouldBeStaff,
                PlanType = recruitmentRequest.PlanType,
                CreatedBy = recruitmentRequest.CreatedBy,
                DateCreated = recruitmentRequest.DateCreated,
                ModifiedBy = recruitmentRequest.ModifiedBy,
                LastModified = recruitmentRequest.LastModified,
                OrgBranchName = recruitmentRequest.OrgBranchName,
                //DepartmentSendForApprovalBy = recruitmentRequest.DepartmentSendForApprovalBy,
                //DepartmentSendForApprovalTime = recruitmentRequest.DepartmentSendForApprovalTime,
                //IsDepartmentSendForApproval = recruitmentRequest.IsDepartmentSendForApproval,
                //DepartmentOnlineApprovedBy = recruitmentRequest.DepartmentOnlineApprovedBy,
                //DepartmentOnlineApprovedTime = recruitmentRequest.DepartmentOnlineApprovedTime,
                //IsDepartmentOnlineApproved = recruitmentRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = recruitmentRequest.OnlineApprovedBy,
                OnlineApprovedTime = recruitmentRequest.OnlineApprovedTime,
                IsOnlineApproved = recruitmentRequest.IsOnlineApproved,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentRequest_Create([DataSourceRequest]DataSourceRequest request, RecruitmentRequest recruitmentRequest)
        {
            int no = recruitmentRequest.NoOfShouldBeStaff - recruitmentRequest.NoOfCurrentStaff;
            if (recruitmentRequest.NoOfVaccantPostion > no)
            {
                ModelState.AddModelError("", "No of Vacant Position  cannot exceed no of available vacant position");
            }
            var duplicate = db.RecruitmentRequest.FirstOrDefault(x => x.LookupId == recruitmentRequest.LookupId && x.OrgStructureId == recruitmentRequest.OrgStructureId && x.GlFiscalYearId == recruitmentRequest.GlFiscalYearId && x.BranchId == recruitmentRequest.BranchId);
            if (duplicate != null)
            {
                ModelState.AddModelError("", "This Job is requested before");
            }
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentRequest
                {
                    OrgStructureId = recruitmentRequest.OrgStructureId,
                    EmploymentType = recruitmentRequest.EmploymentType,
                    LookupId = recruitmentRequest.LookupId,
                    NoOfVaccantPostion = recruitmentRequest.NoOfVaccantPostion,
                    NoOfCurrentStaff = recruitmentRequest.NoOfCurrentStaff,
                    PlanType = recruitmentRequest.PlanType,
                    NoOfShouldBeStaff = recruitmentRequest.NoOfShouldBeStaff,
                    Date = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = recruitmentRequest.ModifiedBy,
                    LastModified = recruitmentRequest.LastModified,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    BranchId = recruitmentRequest.BranchId,
                };

                db.RecruitmentRequest.Add(entity);
                db.SaveChanges();
                recruitmentRequest.RecruitmentRequestId = entity.RecruitmentRequestId;

                var reqWaiting = new RequestesStatus
                {
                    RequestId = recruitmentRequest.RecruitmentRequestId,
                    ReferenceType = "Recruitment",
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    ChangedBy = User.Identity.Name,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),                   
                    ApproverId = 0,
                    Remark = "Asked by HR"
                };

                db.RequestesStatus.Add(reqWaiting);
                db.SaveChanges();

                var reqDep = new RequestesStatus
                {
                    RequestId = recruitmentRequest.RecruitmentRequestId,
                    ReferenceType = "Recruitment",
                    Status = RequestStatus.DEPARTMENT_APPROVAL,
                    ChangedBy = User.Identity.Name,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    ApproverId = 0,
                    Remark = "Asked by HR"
                };

                db.RequestesStatus.Add(reqDep);
                db.SaveChanges();
            }

            return Json(new[] { recruitmentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentRequest_Update([DataSourceRequest]DataSourceRequest request, RecruitmentRequest recruitmentRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentRequest
                {
                    RecruitmentRequestId = recruitmentRequest.RecruitmentRequestId,
                    OrgStructureId = recruitmentRequest.OrgStructureId,
                    EmploymentType = recruitmentRequest.EmploymentType,
                    LookupId = recruitmentRequest.LookupId,
                    NoOfVaccantPostion = recruitmentRequest.NoOfVaccantPostion,
                    NoOfCurrentStaff = recruitmentRequest.NoOfCurrentStaff,
                    PlanType = recruitmentRequest.PlanType,
                    NoOfShouldBeStaff = recruitmentRequest.NoOfShouldBeStaff
                };

                db.RecruitmentRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { recruitmentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentRequest_Destroy([DataSourceRequest]DataSourceRequest request, RecruitmentRequest recruitmentRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentRequest
                {
                    RecruitmentRequestId = recruitmentRequest.RecruitmentRequestId,
                    OrgStructureId = recruitmentRequest.OrgStructureId,
                    EmploymentType = recruitmentRequest.EmploymentType,
                    LookupId = recruitmentRequest.LookupId,
                    NoOfVaccantPostion = recruitmentRequest.NoOfVaccantPostion,
                    NoOfCurrentStaff = recruitmentRequest.NoOfCurrentStaff,
                    PlanType = recruitmentRequest.PlanType,
                    NoOfShouldBeStaff = recruitmentRequest.NoOfShouldBeStaff
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted recruitment request of work unit" + recruitmentRequest.OrgBranchName + " and job title" + recruitmentRequest.LookupId);
                #endregion
                db.RecruitmentRequest.Attach(entity);
                db.RecruitmentRequest.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { recruitmentRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
