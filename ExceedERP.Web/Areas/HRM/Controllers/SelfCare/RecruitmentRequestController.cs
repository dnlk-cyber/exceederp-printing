using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Lookup = ExceedERP.Core.Domain.Common.HRM.Lookup;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class RecruitmentRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int current { get; set; }
        public static int tobe { get; set; }
        public static JobTitlePlan planType { get; set; }
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var structres = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).AsQueryable();
            ViewData["jobTitles"] = structres;
            ViewData["branches"] = db.Branch;//.Where(x => mybraches.Contains(x.BranchId.ToString()));

            GetStatusType();
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);

            //List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.ToList();
            //allDepartment.AddRange(departments);
            ViewData["Departments"] = departments;
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            ViewData["Department"] = db.MainCompanyStructures.ToList();

        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }

        //previous
        public void GetDepartments1()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
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
            int orgStructureId = 0;

            var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeid&&x.isCurrent);
            if (place != null)
            {
                orgStructureId = place.OrgStructureId;
            }

            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            IQueryable<RecruitmentRequest> recruitmentrequest = db.RecruitmentRequest.Where(x => mybraches.Contains(x.OrgBranchName) && x.OrgStructureId == orgStructureId).OrderBy(x => x.Date);

            //IQueryable<LeaveRequest> leaverequest = db.RecruitmentRequest.Where(x => employeeids.Contains(x.EmployeeId)).OrderBy(x => x.RequestDate);

            DataSourceResult result = recruitmentrequest.ToDataSourceResult(request, recruitmentRequest => new
            {
                RecruitmentRequestId = recruitmentRequest.RecruitmentRequestId,
                OrgStructureId = recruitmentRequest.OrgStructureId,
                GlFiscalYearId = recruitmentRequest.GlFiscalYearId,
                EmploymentType = recruitmentRequest.EmploymentType,
                LookupId = recruitmentRequest.LookupId,
                NoOfCurrentStaff = recruitmentRequest.NoOfCurrentStaff,
                NoOfShouldBeStaff = recruitmentRequest.NoOfShouldBeStaff,
                NoOfVaccantPostion = recruitmentRequest.NoOfVaccantPostion,
                PlanType = recruitmentRequest.PlanType,
                Date = recruitmentRequest.Date,
                CreatedBy = recruitmentRequest.CreatedBy,
                DateCreated = recruitmentRequest.DateCreated,
                ModifiedBy = recruitmentRequest.ModifiedBy,
                LastModified = recruitmentRequest.LastModified,
                OrgBranchName = recruitmentRequest.OrgBranchName,
                BranchId = recruitmentRequest.BranchId,
                DepartmentSendForApprovalBy = recruitmentRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = recruitmentRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = recruitmentRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = recruitmentRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = recruitmentRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = recruitmentRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = recruitmentRequest.OnlineApprovedBy,
                OnlineApprovedTime = recruitmentRequest.OnlineApprovedTime,
                IsOnlineApproved = recruitmentRequest.IsOnlineApproved,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentRequest_Create([DataSourceRequest]DataSourceRequest request, RecruitmentRequest recruitmentRequest)
        {
            db = new ExceedDbContext();
            ModelState.Clear();

            int jobTitleQuantity = 0;
            var jobTitle = db.JobTitleMapping.Where(x => x.LookupId == recruitmentRequest.LookupId && x.OrgStructureId == recruitmentRequest.OrgStructureId).FirstOrDefault();
            if (jobTitle != null)
            {
                jobTitleQuantity = jobTitle.Quntity;
            }
            var plan = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == recruitmentRequest.BranchId && x.OrgStructureId == recruitmentRequest.OrgStructureId && x.LookupId == recruitmentRequest.LookupId);

            if (recruitmentRequest.NoOfVaccantPostion == 0 && plan.PlanType == JobTitlePlan.BOUNDED)
            {
                //ModelState.AddModelError("", "No of Vacant Position  should be greater than 0!!");
                var entity = new ManPowerPlanningRequest
                {
                    OrgStructureId = recruitmentRequest.OrgStructureId,
                    EmploymentType = recruitmentRequest.EmploymentType,
                    GlFiscalYearId = recruitmentRequest.GlFiscalYearId,
                    LookupId = recruitmentRequest.LookupId,
                    NoOfCurrentStaff = current,
                    NoOfShouldBeStaff = jobTitleQuantity,
                    PlanType = planType,
                    NumberOfStaff = recruitmentRequest.NoOfVaccantPostion,
                    Date = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = recruitmentRequest.ModifiedBy,
                    LastModified = recruitmentRequest.LastModified,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    BranchId = recruitmentRequest.BranchId,
                };

                db.ManPowerPlanningRequests.Add(entity);
                db.SaveChanges();

            }
            else
            {
                int no = tobe - current;
                if (recruitmentRequest.NoOfVaccantPostion > no)
                {
                    ModelState.AddModelError("", "No of Vacant Position  cannot exceed no of available vacant position");
                }
                var duplicate = db.RecruitmentRequest.FirstOrDefault(x => x.LookupId == recruitmentRequest.LookupId && x.OrgStructureId == recruitmentRequest.OrgStructureId && x.GlFiscalYearId == recruitmentRequest.GlFiscalYearId && x.BranchId == recruitmentRequest.BranchId);
                if (duplicate != null)
                {
                    ModelState.AddModelError("", "This Job is requested before");
                }
                //var recruitmentPlan = db.RecruitmentPlans.FirstOrDefault(x=>x.OrgStructureId==recruitmentRequest.OrgStructureId && x.LookupId==recruitmentRequest.LookupId && x.GlFiscalYearId==recruitmentRequest.GlFiscalYearId);
                //if(recruitmentPlan==null)
                //{
                //    ModelState.AddModelError("", "No such Plan Pleace Plan first");
                //}
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                var place = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
                if (place != null && recruitmentRequest.LookupId == place.LookupId)
                {
                    ModelState.AddModelError("", "you can't request for your jobtitle");
                }

                if (ModelState.IsValid)
                {
                    if (recruitmentRequest.NoOfVaccantPostion == 0)
                    {
                        recruitmentRequest.NoOfVaccantPostion = tobe - current;
                    }

                    var entity = new RecruitmentRequest
                    {
                        OrgStructureId = recruitmentRequest.OrgStructureId,
                        EmploymentType = recruitmentRequest.EmploymentType,
                        GlFiscalYearId = recruitmentRequest.GlFiscalYearId,
                        LookupId = recruitmentRequest.LookupId,
                        NoOfCurrentStaff = current,
                        NoOfShouldBeStaff = jobTitleQuantity,
                        PlanType = planType,
                        NoOfVaccantPostion = recruitmentRequest.NoOfVaccantPostion,
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

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "RecruitmentRequest ", "Success", User.Identity.Name, User.Identity.Name + " add new Recruitment Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                    #endregion

                    #region notification

                    var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var superviorId = EmployeeHelper.GetSupervisor(empid);

                    var deptOfHead = db.Placements.FirstOrDefault(x => x.EmployeeId == superviorId && x.isCurrent);
                    if (deptOfHead != null)
                    {
                        var dept = db.MainCompanyStructures.FirstOrDefault(x =>
                            x.OrgStructureId == deptOfHead.OrgStructureId);
                        if (dept != null)
                        {
                            var deptName = dept.Name;
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                                Title = "Recruitment Request",
                                Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has requested Recruitment for { deptName }"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }

                    #endregion
                }
            }
            return Json(new[] { recruitmentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentRequest_Update([DataSourceRequest]DataSourceRequest request, RecruitmentRequest recruitmentRequest)
        {
            ModelState.Clear();
            if (current != 0)
            {
                recruitmentRequest.NoOfCurrentStaff = current;
            }
            if (tobe != 0)
            {
                recruitmentRequest.NoOfShouldBeStaff = tobe;
            }
            if (recruitmentRequest.NoOfVaccantPostion == 0) recruitmentRequest.NoOfVaccantPostion = tobe - current;
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentRequest
                {
                    RecruitmentRequestId = recruitmentRequest.RecruitmentRequestId,
                    OrgStructureId = recruitmentRequest.OrgStructureId,
                    GlFiscalYearId = recruitmentRequest.GlFiscalYearId,
                    EmploymentType = recruitmentRequest.EmploymentType,
                    LookupId = recruitmentRequest.LookupId,
                    NoOfVaccantPostion = recruitmentRequest.NoOfVaccantPostion,
                    NoOfCurrentStaff = recruitmentRequest.NoOfCurrentStaff,
                    NoOfShouldBeStaff = recruitmentRequest.NoOfShouldBeStaff,
                    PlanType = recruitmentRequest.PlanType,
                    Date = DateTime.Now,
                    CreatedBy = recruitmentRequest.CreatedBy,
                    DateCreated = recruitmentRequest.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    OrgBranchName = recruitmentRequest.OrgBranchName,
                    BranchId = recruitmentRequest.BranchId,
                };

                db.RecruitmentRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "RecruitmentRequest ", "Success", User.Identity.Name, User.Identity.Name + " update Recruitment Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion
            }

            return Json(new[] { recruitmentRequest }.ToDataSourceResult(request, ModelState));
        }
        public string CheckPostions(int OrgId, int title)
        {
            tobe = 0;
            current = 0;

            var toBepostions = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgId && x.LookupId == title).FirstOrDefault();
            var currentStaff = db.Placements.Where(x => x.OrgStructureId == OrgId && x.LookupId == title).ToList().Count;
            int quntity = 0;
            if (toBepostions != null)
            {
                quntity = toBepostions.Quntity;
                tobe = quntity;
                planType = toBepostions.PlanType;
            }
            current = currentStaff;
            return quntity.ToString() + "-" + currentStaff.ToString() + "-" +
                   (quntity - currentStaff).ToString();
        }
        public ActionResult SendForApproval()
        {
            var status =
                     db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
            string messages = "";
            if (status.Any())
            {
                foreach (var s in status)
                {
                    if (s.Status == RequestStatus.APPROVE)
                    {
                        messages = "The Request is already approved!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }

                }
            }
            RequestesStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.RequestesStatus.Where(
                    x =>
                        x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }
            else
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    Remark = requestesStatus.Remark,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
                };

                db.RequestesStatus.Attach(entity);
                db.RequestesStatus.Remove(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentRequest_Destroy([DataSourceRequest]DataSourceRequest request, RecruitmentRequest recruitmentRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentRequest
                {
                    RecruitmentRequestId = recruitmentRequest.RecruitmentRequestId,
                    OrgStructureId = recruitmentRequest.OrgStructureId,
                    GlFiscalYearId = recruitmentRequest.GlFiscalYearId,
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
                    BranchId = recruitmentRequest.BranchId,
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
        public JsonResult GetJobTitle(int OrgStructureId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (!titleList.Contains(lookup))
                    {
                        titleList.Add(lookup);
                    }

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public string GetNoofShouldBe(int workunit, int jobtitle, int branch)
        {
            //SalaryJobTitle salary = new SalaryJobTitle();

            var jobTitle = db.JobTitleMapping.FirstOrDefault(x => x.LookupId == jobtitle && x.OrgStructureId == workunit && x.BranchId == branch);
            var placement = db.Placements.Where(s => s.OrgStructureId == workunit && s.LookupId == jobtitle && s.BranchId == branch && s.isCurrent).ToList();
            if (jobTitle != null)
            {

                current = (placement.Count);

                tobe = jobTitle.Quntity;
                int noofvacanct = tobe - current;
                string concateNumbers = tobe + "-" + current + "-" + noofvacanct;

                return concateNumbers;

            }

            return "0";
        }
        [HttpPost]
        public decimal GetNoOfCurrentStaff(int workunit, int jobtitle)
        {
            var placement = db.Placements.Where(s => s.OrgStructureId == workunit && s.LookupId == jobtitle).ToList();

            if (placement != null)
            {

                // int current = placement.Count;
                current = (placement.Count);
                return Convert.ToDecimal(placement.Count);

            }
            return 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
