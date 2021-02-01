using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;
using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Areas.HRM.Controllers.Leave;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class RequestesStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRDepartmentHead, HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveApprover,
            HrmConstants.HRRecruitmentApprover, HrmConstants.HRSelfServiceManager, HrmConstants.HREmployeeSelfService, HrmConstants.HROverTimeRequestApprover,
           HrmConstants.HRHealthApprover, HrmConstants.HRRecruitmentRequester, HrmConstants.HRTerminationUser, HrmConstants.HRTrainingRequester, HrmConstants.HRTrainingApprover)]
        public ActionResult Index()
        {
            return View();
        }
        public int GetApproverDept(int? id)
        {
            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == id);
            if (place != null)
            {
                return place.OrgStructureId;
            }
            return 0;
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRDepartmentHead, HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveApprover,
            HrmConstants.HRRecruitmentApprover, HrmConstants.HRSelfServiceManager, HrmConstants.HREmployeeSelfService, HrmConstants.HROverTimeRequestApprover,
            HrmConstants.HRHealthApprover, HrmConstants.HRRecruitmentRequester, HrmConstants.HRTerminationUser, HrmConstants.HRTrainingRequester, HrmConstants.HRTrainingApprover)]

        public ActionResult RequestesStatus_Read([DataSourceRequest]DataSourceRequest request, int id, string referenceType)
        {
            GetStatusType();

            HRMInputs.RequestId = id;
            HRMInputs.ReferenceType = referenceType;
            IQueryable<RequestesStatus> requestesstatus = db.RequestesStatus.Where(x => x.RequestId == id && x.ReferenceType == referenceType);
            DataSourceResult result = requestesstatus.ToDataSourceResult(request, requestesStatus => new
            {
                RequestStatusId = requestesStatus.RequestStatusId,
                RequestId = requestesStatus.RequestId,
                ReferenceType = requestesStatus.ReferenceType,
                Status = requestesStatus.Status,
                Date = requestesStatus.Date,
                Remark = requestesStatus.Remark,
                DateReqStatAm = requestesStatus.DateReqStatAm,
                ApproverId = requestesStatus.ApproverId,
                ApproverDepartment = GetApproverDept(requestesStatus.ApproverId)
            });
            return Json(result);


        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                 .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }


        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRLeaveApprover,
            HrmConstants.HRRecruitmentApprover, HrmConstants.HRSelfServiceManager,
            HrmConstants.HRHealthApprover, HrmConstants.HRTrainingApprover, HrmConstants.HROverTimeRequestApprover)]


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestesStatus_Create([DataSourceRequest]DataSourceRequest request, RequestesStatus requestesStatus, int id, string referenceType2)
        {

            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new RequestesStatus
                {
                    RequestId = id,
                    ReferenceType = referenceType2,
                    Status = requestesStatus.Status,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Remark = requestesStatus.Remark
                };

                db.RequestesStatus.Add(entity);
                db.SaveChanges();
                requestesStatus.RequestStatusId = entity.RequestStatusId;
                //
                if (referenceType2 == "Leave" && requestesStatus.Status == RequestStatus.APPROVE)
                {
                    var leavere = db.LeaveRequest.Find(id);
                    #region update balance


                    var entiteledEmployee = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leavere.EmployeeId && x.GlFiscalYearId == leavere.GlFiscalYearId);
                    if (entiteledEmployee != null)
                    {
                        decimal totalAmountField = entiteledEmployee.TotlalAmount;
                        if (leavere != null)
                        {
                            decimal totalEntiteleft = totalAmountField - leavere.LeaveDays;
                            entiteledEmployee.TotlalAmount = totalEntiteleft;
                        }

                        db.Entry(entiteledEmployee).State = EntityState.Modified;
                        db.SaveChanges();

                    }



                    #endregion
                }


                if (referenceType2 == "Termination")
                {
                    if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {

                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRLeaveApprover)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);
                        var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        var userDept =
                            db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);


                        var deptName = db.MainCompanyStructures.FirstOrDefault(x =>
                            x.OrgStructureId == userDept.OrgStructureId);

                        NotifModels notif = new NotifModels
                        {
                            UserId = UserInformationHelper.GetEmployeeUserId(hrEmployeeId),
                            Title = "Termination Request",
                            Message =
                                $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has requested Terminaiton & he is from {deptName?.Name} Department"
                        };

                        Notify.NotifyUser(notif);


                    }
                }
            }

            return Json(new[] { requestesStatus }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRLeaveApprover,
            HrmConstants.HRRecruitmentApprover,
            HrmConstants.HRHealthApprover, HrmConstants.HRTrainingApprover, HrmConstants.HROverTimeRequestApprover, HrmConstants.HRDepartmentHead)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestesStatus_Update([DataSourceRequest]DataSourceRequest request, RequestesStatus requestesStatus)
        {

            ModelState.Clear();

            #region Validation 

            if (requestesStatus.ReferenceType == "Leave")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var leaveReq = db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);
                if (leaveReq != null)
                {
                    var supervisorId = EmployeeHelper.GetSupervisor(leaveReq.EmployeeId);
                    var roleCheck = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRDepartmentHead);
                    var roleCheckHR = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRLeaveApprover);
                    var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                        x.RequestId == leaveReq.LeaveRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Leave");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == leaveReq.LeaveRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Leave");

                    if (deptapp == null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "This request is not approved by department so You can not approve");
                    }
                    if (deptapp != null && requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        ModelState.AddModelError("", "You Approved This Request before so Cannot approve again");
                    }
                    if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "You Approved This Request before so Cannot approve again");
                    }
                    if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheckHR)
                    {
                        ModelState.AddModelError("", "you cannot approve as HR");
                    }

                    if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL && empId != supervisorId)
                    {
                        ModelState.AddModelError("", "You are not Head of This department you can not Approve as Head");
                    }
                    if (hrapprove == null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        //var entity = new Core.Domain.HRM.LeaveManagment.Leave
                        //{
                        //    EmployeeId = leaveReq.EmployeeId,
                        //    LeaveType = leaveReq.Type,
                        //    Amount = leaveReq.LeaveDays,
                        //    GlFiscalYearId = leaveReq.GlFiscalYearId,
                        //    From = leaveReq.From,
                        //    To = leaveReq.To,
                        //    Remark = leaveReq.Remark,
                        //    EmployeeCode = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == leaveReq.EmployeeId)?.EmployeeCode,
                        //    FromAmharic = leaveReq.FromAmharic,
                        //    ToAmharic = leaveReq.ToAmharic,
                        //    DateCreated = leaveReq.DateCreated,
                        //    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        //};
                        //db.Leaves.Add(entity);
                        //db.SaveChanges();
                    }
                }
            }
            if (requestesStatus.ReferenceType == "Termination")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var termReq = db.Terminations.FirstOrDefault(x => x.TerminationId == requestesStatus.RequestId);
                if (termReq != null)
                {
                    var supervisorId = EmployeeHelper.GetSupervisor(termReq.EmployeeId);
                    var roleCheck = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRAdmin);
                    var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                        x.RequestId == termReq.TerminationId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Termination");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == termReq.TerminationId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Termination");
                    //if (deptapp == null && requestesStatus.Status == RequestStatus.APPROVE)
                    //{
                    //    ModelState.AddModelError("", "This request is not approved by department so You can not approve");
                    //}
                    if (deptapp != null && requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        ModelState.AddModelError("", "You approve this request before so cannot approve again");
                    }
                    if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "You approve this request before so cannot approve again");
                    }
                    if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL && empId != supervisorId)
                    {
                        ModelState.AddModelError("", "You are not Head of This department you can not Approve as Head");
                    }
                    if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheck)
                    {
                        ModelState.AddModelError("", "you cannot approve as HR");
                    }
                }
            }
            if (requestesStatus.ReferenceType == "Experience")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var expReq = db.ExprianceRequests.FirstOrDefault(x => x.ExperienceRequestId == requestesStatus.RequestId);
                if (expReq != null)
                {
                    var supervisorId = EmployeeHelper.GetSupervisor(expReq.EmployeeId);
                    var roleCheck = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRAdmin);
                    var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                        x.RequestId == expReq.ExperienceRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Experience");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == expReq.ExperienceRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Experience");


                    if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "You approved this request before so cannot approve again");
                    }
                    if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL && empId != supervisorId)
                    {
                        ModelState.AddModelError("", "Department cann't approve Experience Letter Request");
                    }
                    if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheck)
                    {
                        ModelState.AddModelError("", "you cannot approve as HR");
                    }
                }
            }
            if (requestesStatus.ReferenceType == "Medical")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var medReq = db.MedicalRequest.FirstOrDefault(x => x.MedicalRequestId == requestesStatus.RequestId);
                if (medReq != null)
                {
                    var supervisorId = EmployeeHelper.GetSupervisor(medReq.EmployeeId);
                    var roleCheck = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRHealthAdmin);
                    var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                        x.RequestId == medReq.MedicalRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Medical");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == medReq.MedicalRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Medical");
                    //if (deptapp == null && requestesStatus.Status == RequestStatus.APPROVE)
                    //{
                    //    ModelState.AddModelError("", "This request is not approved by department so You can not approve");
                    //}
                    if (deptapp != null && requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        ModelState.AddModelError("", "You approved this request before so cannot approve again");
                    }
                    if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "You approved this request before so cannot approve again");
                    }
                    if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL && empId != supervisorId)
                    {
                        ModelState.AddModelError("", "You are not Head of This department you can not Approve as Head");
                    }
                    if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheck)
                    {
                        ModelState.AddModelError("", "you cannot approve as HR");
                    }
                }
            }
            if (requestesStatus.ReferenceType == "Recruitment")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var recruitReq = db.RecruitmentRequest.FirstOrDefault(x => x.RecruitmentRequestId == requestesStatus.RequestId);

                if (recruitReq != null)
                {

                    var recHead = db.Placements.FirstOrDefault(x => x.OrgStructureId == recruitReq.OrgStructureId && x.isCurrent && x.IsHead)?.EmployeeId;
                    if (recHead != null)
                    {
                        var supervisorId = EmployeeHelper.GetSupervisor(Convert.ToInt32(recHead));
                        var roleCheck =
                            UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRRecruitmentApprover);


                        var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                     x.RequestId == recruitReq.RecruitmentRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Recruitment");

                        var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                     x.RequestId == recruitReq.RecruitmentRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Recruitment");

                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
                        if (placement != null && recruitReq.LookupId == placement.LookupId)
                        {
                            ModelState.AddModelError("",
                               "you can't approve requests for your own jobtitle");
                        }
                        //if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL && empId != supervisorId)
                        //{
                        //    ModelState.AddModelError("",
                        //        "You are not Head of This department you can not Approve as Head");
                        //}
                        if (deptapp != null && requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            ModelState.AddModelError("", "You approved this request before so cannot approve again");
                        }
                        if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            ModelState.AddModelError("", "You approved this request before so cannot approve again");
                        }
                        if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheck)
                        {
                            ModelState.AddModelError("", "you cannot approve as HR");
                        }
                    }
                }
            }
            if (requestesStatus.ReferenceType == "Training")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var trainingReq = db.TrainingApplicants.FirstOrDefault(x => x.TrainingApplicantId == requestesStatus.RequestId);
                if (trainingReq != null)
                {
                    var recHead = db.Placements.FirstOrDefault(x => x.OrgStructureId == trainingReq.OrgStructureId && x.isCurrent && x.IsHead)?.EmployeeId;
                    if (recHead != null)
                    {
                        var supervisorId = EmployeeHelper.GetSupervisor(Convert.ToInt32(recHead));
                        var roleCheck =
                            UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRTrainingApprover);
                        var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                            x.RequestId == trainingReq.TrainingApplicantId &&
                            x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Training");
                        var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                           x.RequestId == trainingReq.TrainingApplicantId &&
                           x.Status == RequestStatus.APPROVE && x.ReferenceType == "Training");



                        if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            ModelState.AddModelError("", "You approved This request before so cannot approve again");
                        }
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            ModelState.AddModelError("",
                                "Department cann't approve Training Request");
                        }

                        if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheck)
                        {
                            ModelState.AddModelError("", "you cannot approve as HR");
                        }
                    }
                }
            }
            if (requestesStatus.ReferenceType == "Education")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var educationReq = db.EducationRequests.FirstOrDefault(x => x.EducationRequestId == requestesStatus.RequestId);
                if (educationReq != null)
                {
                    var recHead = db.Placements.FirstOrDefault(x => x.OrgStructureId == educationReq.OrgStructureId && x.isCurrent && x.IsHead)?.EmployeeId;
                    if (recHead != null)
                    {
                        var supervisorId = EmployeeHelper.GetSupervisor(Convert.ToInt32(recHead));
                        var roleCheck =
                            UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRAdmin);
                        var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                            x.RequestId == educationReq.EducationRequestId &&
                            x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Education");

                        var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                           x.RequestId == educationReq.EducationRequestId &&
                           x.Status == RequestStatus.APPROVE && x.ReferenceType == "Education");


                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            ModelState.AddModelError("", "Department Cann't approve Education Request");
                        }
                        if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            ModelState.AddModelError("", "You approved This request before so cannot approve again");
                        }

                        if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheck)
                        {
                            ModelState.AddModelError("", "you cannot approve as HR");
                        }
                    }
                }
            }
            if (requestesStatus.ReferenceType == "LeaveReturn")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var requesteEmpId = requestesStatus.RequestId;
                var leaveReturn = db.LeaveReturns.FirstOrDefault(x => x.LeaveReturnId == requestesStatus.RequestId);
                if (leaveReturn != null)
                {
                    var supervisorId = EmployeeHelper.GetSupervisor(leaveReturn.EmployeeId);
                    var roleCheck = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRDepartmentHead);
                    var roleCheckHR = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRLeaveApprover);
                    var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                        x.RequestId == leaveReturn.LeaveReturnId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "LeaveReturn");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == leaveReturn.LeaveReturnId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "LeaveReturn");

                    if (deptapp == null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "This request is not approved by department so You can not approve");
                    }
                    if (deptapp != null && requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        ModelState.AddModelError("", "You Approved This Request before so Cannot approve again");
                    }
                    if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "You Approved This Request before so Cannot approve again");
                    }
                    if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheckHR)
                    {
                        ModelState.AddModelError("", "you cannot approve as HR");
                    }

                    if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL && empId != supervisorId)
                    {
                        ModelState.AddModelError("", "You are not Head of This department you can not Approve as Head");
                    }
                    if (hrapprove == null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        //var entity = new Core.Domain.HRM.LeaveManagment.Leave
                        //{
                        //    EmployeeId = leaveReq.EmployeeId,
                        //    LeaveType = leaveReq.Type,
                        //    Amount = leaveReq.LeaveDays,
                        //    GlFiscalYearId = leaveReq.GlFiscalYearId,
                        //    From = leaveReq.From,
                        //    To = leaveReq.To,
                        //    Remark = leaveReq.Remark,
                        //    EmployeeCode = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == leaveReq.EmployeeId)?.EmployeeCode,
                        //    FromAmharic = leaveReq.FromAmharic,
                        //    ToAmharic = leaveReq.ToAmharic,
                        //    DateCreated = leaveReq.DateCreated,
                        //    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        //};
                        //db.Leaves.Add(entity);
                        //db.SaveChanges();
                    }
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    ChangedBy = User.Identity.Name,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Remark = requestesStatus.Remark,
                    ApproverId = empId
                };

                db.RequestesStatus.Add(entity);

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                //////// if the request is LEAVE REQUEST it does not save here it saves ///////////////////////
                ////the approve action in the below 'if' fragmet after checking the balance////////////////
                //////////////////////////////////////////////////////////////////////////////////////

                if (requestesStatus.ReferenceType == "Leave" && requestesStatus.Status == RequestStatus.APPROVE)
                {

                }
                else
                {
                    db.SaveChanges();
                }
                requestesStatus.RequestStatusId = entity.RequestStatusId;


                #region update parent
                if (requestesStatus.ReferenceType == "Payment")
                {
                    var paymentRequest = db.PaymentRequests.Find(requestesStatus.RequestId);
                    if (paymentRequest != null)
                    {
                        paymentRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            paymentRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            paymentRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            paymentRequest.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            paymentRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            paymentRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            paymentRequest.IsDepartmentSendForApproval = true;
                        }

                        db.PaymentRequests.Attach(paymentRequest);
                        db.Entry(paymentRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (requestesStatus.ReferenceType == "Leave")
                {
                    var leaveRequest = db.LeaveRequest.Find(requestesStatus.RequestId);
                    if (leaveRequest != null)
                    {
                        leaveRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            leaveRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            leaveRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            leaveRequest.IsDepartmentOnlineApproved = true;

                            db.Entry(leaveRequest).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            leaveRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            leaveRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            leaveRequest.IsDepartmentSendForApproval = true;

                            db.Entry(leaveRequest).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                                               
                    }
                }
                if (requestesStatus.ReferenceType == "Termination")
                {
                    var terminationRequest = db.Terminations.Find(requestesStatus.RequestId);
                    if (terminationRequest != null)
                    {
                        terminationRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            terminationRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            terminationRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            terminationRequest.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            terminationRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            terminationRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            terminationRequest.IsDepartmentSendForApproval = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            terminationRequest.OnlineApprovedBy = User.Identity.Name;
                            terminationRequest.OnlineApprovedTime = DateTime.Now;
                            terminationRequest.IsOnlineApproved = true;
                        }

                        db.Entry(terminationRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (requestesStatus.ReferenceType == "Experience")
                {
                    var experienceRequest = db.ExprianceRequests.Find(requestesStatus.RequestId);
                    if (experienceRequest != null)
                    {
                        experienceRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            experienceRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            experienceRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            experienceRequest.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            experienceRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            experienceRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            experienceRequest.IsDepartmentSendForApproval = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            experienceRequest.OnlineApprovedBy = User.Identity.Name;
                            experienceRequest.OnlineApprovedTime = DateTime.Now;
                            experienceRequest.IsOnlineApproved = true;
                        }

                        db.Entry(experienceRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (requestesStatus.ReferenceType == "Medical")
                {
                    var medicalRequest = db.MedicalRequest.Find(requestesStatus.RequestId);
                    if (medicalRequest != null)
                    {
                        medicalRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            medicalRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            medicalRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            medicalRequest.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            medicalRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            medicalRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            medicalRequest.IsDepartmentSendForApproval = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            medicalRequest.OnlineApprovedBy = User.Identity.Name;
                            medicalRequest.OnlineApprovedTime = DateTime.Now;
                            medicalRequest.IsOnlineApproved = true;
                        }

                        db.Entry(medicalRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (requestesStatus.ReferenceType == "Recruitment")
                {
                    var recruitmentRequest = db.RecruitmentRequest.Find(requestesStatus.RequestId);
                    if (recruitmentRequest != null)
                    {
                        recruitmentRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            recruitmentRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            recruitmentRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            recruitmentRequest.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            recruitmentRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            recruitmentRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            recruitmentRequest.IsDepartmentSendForApproval = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            recruitmentRequest.OnlineApprovedBy = User.Identity.Name;
                            recruitmentRequest.OnlineApprovedTime = DateTime.Now;
                            recruitmentRequest.IsOnlineApproved = true;
                        }

                        db.Entry(recruitmentRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (requestesStatus.ReferenceType == "Training")
                {
                    var trainingRequest = db.TrainingApplicants.Find(requestesStatus.RequestId);
                    if (trainingRequest != null)
                    {
                        trainingRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            trainingRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            trainingRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            trainingRequest.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            trainingRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            trainingRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            trainingRequest.IsDepartmentSendForApproval = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            trainingRequest.OnlineApprovedBy = User.Identity.Name;
                            trainingRequest.OnlineApprovedTime = DateTime.Now;
                            trainingRequest.IsOnlineApproved = true;
                        }

                        db.Entry(trainingRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (requestesStatus.ReferenceType == "Education")
                {
                    var educationRequest = db.EducationRequests.Find(requestesStatus.RequestId);
                    if (educationRequest != null)
                    {
                        educationRequest.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            educationRequest.DepartmentOnlineApprovedBy = User.Identity.Name;
                            educationRequest.DepartmentOnlineApprovedTime = DateTime.Now;
                            educationRequest.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            educationRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                            educationRequest.DepartmentSendForApprovalTime = DateTime.Now;
                            educationRequest.IsDepartmentSendForApproval = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            educationRequest.OnlineApprovedBy = User.Identity.Name;
                            educationRequest.OnlineApprovedTime = DateTime.Now;
                            educationRequest.IsOnlineApproved = true;
                        }

                        db.Entry(educationRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (requestesStatus.ReferenceType == "LeaveReturn")
                {
                    var leaveVoid = db.LeaveReturns.Find(requestesStatus.RequestId);
                    if (leaveVoid != null)
                    {
                        leaveVoid.Status = requestesStatus.Status.ToString();
                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                        {
                            leaveVoid.DepartmentOnlineApprovedBy = User.Identity.Name;
                            leaveVoid.DepartmentOnlineApprovedTime = DateTime.Now;
                            leaveVoid.IsDepartmentOnlineApproved = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            leaveVoid.DepartmentSendForApprovalBy = User.Identity.Name;
                            leaveVoid.DepartmentSendForApprovalTime = DateTime.Now;
                            leaveVoid.IsDepartmentSendForApproval = true;
                        }
                        else if (requestesStatus.Status == RequestStatus.APPROVE)
                        {
                            leaveVoid.OnlineApprovedBy = User.Identity.Name;
                            leaveVoid.OnlineApprovedTime = DateTime.Now;
                            leaveVoid.IsOnlineApproved = true;
                        }

                        db.Entry(leaveVoid).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                #endregion


                #region Notifications

                if (requestesStatus.ReferenceType == "Leave")
                {
                    if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {

                        var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        var superviorId = EmployeeHelper.GetSupervisor(empid);

                        NotifModels notif = new NotifModels
                        {

                            UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                            Title = "Leave Request",
                            Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Leave",
                            URL = "~/HRM/LeaveRequest"
                        };

                        Notify.NotifyUser(notif);

                    }
                    else if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {

                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRLeaveApprover)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);

                        var requesterEmpId =
                            db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);
                        if (requesterEmpId != null)
                        {
                            var userDept =
                                db.Placements.FirstOrDefault(x => x.EmployeeId == requesterEmpId.EmployeeId && x.isCurrent);

                            var deptName = db.MainCompanyStructures.FirstOrDefault(x =>
                                x.OrgStructureId == userDept.OrgStructureId);
                            var requesterEmpName =
                                db.Person_Employee.FirstOrDefault(x => x.EmployeeId == userDept.EmployeeId && x.IsActive);

                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(hrEmployeeId),
                                Title = "Leave Request",
                                Message =
                                    $" {requesterEmpName?.FullNameAmharic}  has requested Leave & he is from {deptName?.Name} Department"
                            };

                            Notify.NotifyUser(notif);
                        }



                    }
                    else if (requestesStatus.Status == RequestStatus.REJECT)
                    {
                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRLeaveApprover)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);

                        var requesterEmpId =
                            db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Leave Request",
                                Message = "Your Leave Request are Not Accepted"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }

                }

                if (requestesStatus.ReferenceType == "LeaveReturn")
                {

                    if (requestesStatus.Status == RequestStatus.REJECT)
                    {
                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRLeaveApprover)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);

                        var requesterEmpId =
                            db.LeaveReturns.FirstOrDefault(x => x.LeaveReturnId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Leave Return",
                                Message = "Your Leave Return are Rejected"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }
                    else if (requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRLeaveApprover)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);

                        var requesterEmpId =
                            db.LeaveReturns.FirstOrDefault(x => x.LeaveReturnId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Leave Return",
                                Message = "Your Leave Return are Approved"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }

                }

                if (requestesStatus.ReferenceType == "Termination")
                {

                    if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {

                        var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        var superviorId = EmployeeHelper.GetSupervisor(empid);

                        NotifModels notif = new NotifModels
                        {

                            UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                            Title = "Termination Request",
                            Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Termination",
                            URL = "~/HRM/TerminationRequest"
                        };

                        Notify.NotifyUser(notif);


                    }
                    else if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {

                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);
                        //var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                        var requesterEmpId =
                            db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);
                        if (requesterEmpId != null)
                        {

                            var userDept =
                                db.Placements.FirstOrDefault(x =>
                                    x.EmployeeId == requesterEmpId.EmployeeId && x.isCurrent);

                            var deptName = db.MainCompanyStructures.FirstOrDefault(x =>
                                x.OrgStructureId == userDept.OrgStructureId);
                            var requesterEmpName =
                                db.Person_Employee.FirstOrDefault(
                                    x => x.EmployeeId == userDept.EmployeeId && x.IsActive);

                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(hrEmployeeId),
                                Title = "Termination Request",
                                Message =
                                    $" {requesterEmpName?.FullNameAmharic}  has requested Termination & he is from {deptName?.Name} Department"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }

                    else if (requestesStatus.Status == RequestStatus.REJECT)
                    {
                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);

                        var requesterEmpId =
                            db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Terminaiton Request",
                                Message = "Your Terminaiton Request are Not Accepted"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }
                    else if (requestesStatus.Status == RequestStatus.APPROVE)
                    {

                        #region Update Employement Status and Placement
                        var termination = db.Terminations.FirstOrDefault(x => x.TerminationId == requestesStatus.RequestId);
                        if (termination != null)
                        {

                            var employee = db.Person_Employee.Find(termination.EmployeeId);
                            if (employee != null)
                            {
                                employee.IsActive = false;
                                db.Entry(employee).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId && x.isCurrent);
                            if (placement != null)
                            {
                                placement.isCurrent = false;
                                db.Entry(placement).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            #endregion

                            var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin)
                                .FirstOrDefault();

                            var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);

                            var requesterEmpId =
                                db.Terminations.FirstOrDefault(x => x.TerminationId == requestesStatus.RequestId);


                            if (requesterEmpId != null)
                            {
                                NotifModels notif = new NotifModels
                                {
                                    UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                    Title = "Terminaiton Request",
                                    Message = "Your Terminaiton Request are Approved"
                                };

                                Notify.NotifyUser(notif);

                                var commitment = db.Commitments.FirstOrDefault(x => x.EmployeeId == requesterEmpId.EmployeeId);
                                if (commitment != null)
                                {
                                    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == requesterEmpId.EmployeeId);
                                    if (emp != null)
                                    {
                                        var superviorId = EmployeeHelper.GetSupervisor(emp.EmployeeId);

                                        NotifModels notiff = new NotifModels
                                        {

                                            UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                                            Title = "Terminaiton Request",
                                            Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has Terminate remove his Commitment",
                                            URL = "~/HRM/Termination"
                                        };

                                        Notify.NotifyUser(notiff);

                                    }

                                }
                            }
                        }
                    }
                }

                if (requestesStatus.ReferenceType == "Experience")
                {
                    if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {


                        var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        var superviorId = EmployeeHelper.GetSupervisor(empid);

                        NotifModels notif = new NotifModels
                        {

                            UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                            Title = "Experience Request",
                            Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Experience",
                            URL = "~/HRM/ExpirianceRequest"
                        };

                        Notify.NotifyUser(notif);


                    }
                    else if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {

                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);
                        //var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                        var requesterEmpId =
                            db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);
                        if (requesterEmpId != null)
                        {

                            var userDept =
                                db.Placements.FirstOrDefault(x =>
                                    x.EmployeeId == requesterEmpId.EmployeeId && x.isCurrent);

                            var deptName = db.MainCompanyStructures.FirstOrDefault(x =>
                                x.OrgStructureId == userDept.OrgStructureId);
                            var requesterEmpName =
                                db.Person_Employee.FirstOrDefault(
                                    x => x.EmployeeId == userDept.EmployeeId && x.IsActive);

                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(hrEmployeeId),
                                Title = "Experience Request",
                                Message =
                                    $" {requesterEmpName?.FullNameAmharic}  has requested Experience Letter & he is from {deptName?.Name} Department"
                            };

                            Notify.NotifyUser(notif);

                        }

                    }
                    else if (requestesStatus.Status == RequestStatus.REJECT)
                    {
                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin)
                            .FirstOrDefault();


                        var requesterEmpId =
                            db.ExprianceRequests.FirstOrDefault(x => x.ExperienceRequestId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Experience Request",
                                Message = "Your Experience Request are Not Accepted"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }
                    else if (requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin)
                            .FirstOrDefault();


                        var requesterEmpId =
                            db.ExprianceRequests.FirstOrDefault(x => x.ExperienceRequestId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Experience Request",
                                Message = "Your Experience Request are Approved"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }

                }

                if (requestesStatus.ReferenceType == "Medical")
                {
                    if (requestesStatus.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        var superviorId = EmployeeHelper.GetSupervisor(empid);

                        NotifModels notif = new NotifModels
                        {

                            UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                            Title = "Medical Request",
                            Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Medical",
                            URL = "~/HRM/MedicalRequest"
                        };

                        Notify.NotifyUser(notif);

                    }
                    else if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {

                        var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRHealthApprover)
                            .FirstOrDefault();

                        var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);
                        //var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                        var requesterEmpId =
                            db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);
                        if (requesterEmpId != null)
                        {
                            var userDept =
                                db.Placements.FirstOrDefault(x => x.EmployeeId == requesterEmpId.EmployeeId && x.isCurrent);

                            var deptName = db.MainCompanyStructures.FirstOrDefault(x =>
                                x.OrgStructureId == userDept.OrgStructureId);
                            var requesterEmpName =
                                db.Person_Employee.FirstOrDefault(x => x.EmployeeId == userDept.EmployeeId && x.IsActive);

                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(hrEmployeeId),
                                Title = "Medical Request",
                                Message =
                                    $" {requesterEmpName?.FullNameAmharic}  has requested for Medical Problem & he is from {deptName?.Name} Department"
                            };

                            Notify.NotifyUser(notif);
                        }


                    }
                    else if (requestesStatus.Status == RequestStatus.REJECT)
                    {

                        var requesterEmpId =
                            db.MedicalRequest.FirstOrDefault(x => x.MedicalRequestId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Medical Request",
                                Message = "Your Medical Request are Not Accepted"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }
                    else if (requestesStatus.Status == RequestStatus.REJECT)
                    {

                        var requesterEmpId =
                            db.MedicalRequest.FirstOrDefault(x => x.MedicalRequestId == requestesStatus.RequestId);


                        if (requesterEmpId != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                Title = "Medical Request",
                                Message = "Your Medical Request are Approved"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }

                }
                #endregion

                #region Update Balance
                if (requestesStatus.ReferenceType == "Leave" && requestesStatus.Status == RequestStatus.APPROVE)
                {

                    var leaves = db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);

                    if (leaves != null /*&& leaves.Type == LeaveType.ANNUAL*/)
                    {
                        var leaveEntitle = db.LeavesEntitle.Where(x => x.EmployeeId == leaves.EmployeeId && !x.IsExpired);
                        var totalBalace = leaveEntitle.Sum(x => x.Balance);
                        if (leaves.LeaveDays > totalBalace && leaves.Type == LeaveType.ANNUAL)
                        {
                            ModelState.AddModelError("", "you requested more than your balance");
                            //var messages = "you requested morethan your balance";
                            //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        }
                        else
                        {
                            #region Leave Final Approve Notification 
                            var userRole = UserInformationHelper.GetUsersInRole(HrmConstants.HRLeaveApprover)
                                    .FirstOrDefault();

                            var hrEmployeeId = UserInformationHelper.GetEmployeeId(userRole?.UserName);

                            var requesterEmpId =
                                db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);


                            if (requesterEmpId != null)
                            {
                                NotifModels notif = new NotifModels
                                {
                                    UserId = UserInformationHelper.GetEmployeeUserId(requesterEmpId.EmployeeId),
                                    Title = "Leave Request",
                                    Message = "Your Leave Request are Approved"
                                };

                                Notify.NotifyUser(notif);
                            }
                            #endregion


                            leaves.OnlineApprovedBy = User.Identity.Name;
                            leaves.OnlineApprovedTime = DateTime.Now;
                            leaves.IsOnlineApproved = true;

                            db.Entry(leaves).State = EntityState.Modified;

                            int count = 0;
                            double weekDays = 0;
                            var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == leaves.GlFiscalYearId).ToList();
                            if (holidayList.Any())
                            {
                                foreach (var h in holidayList)
                                {
                                    if (h.Date >= leaves.From && h.Date <= leaves.To)
                                    {
                                        count++;
                                    }
                                }
                            }
                            weekDays = GetWeekDays(leaves.From, leaves.To);
                            var entityy = new Core.Domain.HRM.LeaveManagment.Leave
                            {
                                EmployeeId = leaves.EmployeeId,
                                LeaveType = leaves.Type,
                                Amount = leaves.LeaveDays,
                                GlFiscalYearId = leaves.GlFiscalYearId,
                                From = leaves.From,
                                To = leaves.To,
                                FromAmharic = leaves.FromAmharic,
                                ToAmharic = leaves.ToAmharic,
                                OrgName = null,
                                Before = null,
                                After = null,
                                Remark = leaves.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                EmployeeCode = leaves.Employee.EmployeeCode,
                                ModifiedBy = null,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                RequestId = leaves.LeaveRequestId
                            };
                            if (entityy.LeaveType == LeaveType.ANNUAL)
                            {

                                entityy.Amount = entityy.To.Subtract(entityy.From).Days + 1 - count;
                                entityy.Amount = entityy.Amount - Convert.ToDecimal(weekDays);
                                if (leaves.Day == LeaveDay.ግማሽ_ቀን)
                                {

                                    decimal checkWeekEnd = entityy.To.Subtract(entityy.From).Days + 1 - count;
                                    double amt = GetHalf(entityy.From, entityy.To);
                                    double notHalf = GetNotHalf(entityy.From, entityy.To);
                                    bool isHalf = IsHalf(entityy.From, entityy.To);
                                    if (isHalf)
                                    {
                                        entityy.Amount = (checkWeekEnd - Convert.ToDecimal(notHalf)) / 2;
                                    }
                                    else
                                    {
                                        entityy.Amount = entityy.Amount / 2;
                                    }

                                }

                                decimal days = entityy.Amount * 8;
                                decimal leaveDays = entityy.Amount * entityy.Hours;
                                entityy.Amount = (days - leaveDays) / 8;




                                //====================================================================
                                #region Update Balance

                                var totalSum = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == entityy.EmployeeId).ToList();

                                var totalEntitle = totalSum.Sum(x => x.Balance);
                                if (totalEntitle < entityy.Amount)
                                {
                                    ModelState.AddModelError("", "your request amount is greater than your balance");
                                }
                                else
                                {
                                    db.Leaves.Add(entityy);
                                    db.SaveChanges();



                                    decimal dateAsked = entityy.Amount;
                                    decimal dateLeft = 0;
                                    do
                                    {

                                        List<LeaveModel> leaveList = new List<LeaveModel>();
                                        var yearLists = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == entityy.EmployeeId).ToList();



                                        foreach (var year in yearLists)
                                        {
                                            var leavel = new LeaveModel();
                                            var gl = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == year.GlFiscalYearId);
                                            if (gl != null)
                                            {
                                                leavel.Year = gl.DateTo;
                                            }

                                            leaveList.Add(leavel);
                                        }
                                        var smallestDate = leaveList.Min(x => x.Year);
                                        if (smallestDate != null)
                                        {
                                            var id = db.GLFiscalYears.FirstOrDefault(x => x.DateTo.Year == smallestDate.Year);
                                            if (id != null)
                                            {
                                                var entiteledEmp = db.LeavesEntitle.FirstOrDefault
                                                    (x => x.EmployeeId == entityy.EmployeeId && x.GlFiscalYearId == id.GlFiscalYearId && !x.IsExpired);
                                                if (entiteledEmp != null)
                                                {
                                                    if (entiteledEmp.Balance < dateAsked)
                                                    {
                                                        dateLeft = dateAsked - entiteledEmp.Balance;
                                                        if (dateLeft >= 0)
                                                        {
                                                            //if(id.Status == Core.Domain.Finance.GL.FiscalYearStatus.Close)
                                                            //{
                                                            entiteledEmp.Balance = 0;
                                                            entiteledEmp.IsExpired = true;

                                                            db.Entry(entiteledEmp).State = EntityState.Modified;
                                                            db.SaveChanges();
                                                            //}


                                                            dateAsked = dateLeft;
                                                        }
                                                    }

                                                    else
                                                    {
                                                        dateLeft = entiteledEmp.Balance - dateAsked;
                                                        if (dateLeft >= 0)
                                                        {
                                                            entiteledEmp.Balance = dateLeft;

                                                            if (dateLeft == 0)
                                                            {
                                                                //if (id.Status == Core.Domain.Finance.GL.FiscalYearStatus.Close)
                                                                //{
                                                                entiteledEmp.IsExpired = true;
                                                                //}
                                                            }

                                                            db.Entry(entiteledEmp).State = EntityState.Modified;
                                                            db.SaveChanges();

                                                            dateLeft = 0;
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                    } while (dateLeft > 0);

                                    #endregion

                                    #region  update balance column in leave request

                                    var entitl = db.LeavesEntitle.Where(x => x.EmployeeId == entityy.EmployeeId && !x.IsExpired).ToList();
                                    if (entitl.Any())
                                    {
                                        decimal ttlB = entitl.Sum(x => x.Balance);

                                        var reqeust = db.LeaveRequest.Find(requestesStatus.RequestId);
                                        if (reqeust != null)
                                        {
                                            reqeust.LeaveBalance = ttlB;

                                            db.Entry(reqeust).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }

                                    }
                                    #endregion
                                    #region Add Datas to Leave Taken

                                    #endregion


                                }
                            }
                            else
                            {
                                db.Leaves.Add(entityy);
                                db.SaveChanges();
                            }
                        }
                    }


                }

                if (requestesStatus.ReferenceType == "LeaveReturn" && requestesStatus.Status == RequestStatus.APPROVE)
                {
                    var leave = db.LeaveReturns.FirstOrDefault(x => x.LeaveReturnId == requestesStatus.RequestId);
                    if (leave != null)
                    {
                        //var entitle = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leave.EmployeeId && x.GlFiscalYearId == leave.GlFiscalYearId);
                        //if (entitle != null)
                        //{
                        //    //entitle.Balance = entitle.Balance + leave.Amount;
                        //    //if (entitle.Void != null)
                        //    //{
                        //    //    entitle.Void = entitle.Void + leave.Amount;
                        //    //}
                        //    //else
                        //    //{
                        //    //    entitle.Void = leave.Amount;
                        //    //}


                        //    //db.Entry(entitle).State = EntityState.Modified;
                        //    //db.SaveChanges();
                        //}
                        var branchId = 0;
                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == leave.EmployeeId);
                        if (placement != null)
                        {
                            branchId = placement.BranchId;
                        }
                        var voidEntitle = new LeaveEntitle
                        {
                            EmployeeId = leave.EmployeeId,
                            BranchId = branchId,
                            TotlalAmount = leave.Amount,
                            GlFiscalYearId = leave.GlFiscalYearId,
                            Void = "Void",
                            Amount = leave.Amount,
                            Balance = leave.Amount,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Today,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = User.Identity.Name,
                            EmployeeCode = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leave.EmployeeId)?.EmployeeCode,

                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        };

                        db.LeavesEntitle.Add(voidEntitle);
                        db.SaveChanges();
                    }
                }

                #endregion

                #region Generate Patient Card
                if (requestesStatus.ReferenceType == "Medical" && requestesStatus.Status == RequestStatus.APPROVE)
                {
                    var medicalRequest = db.MedicalRequest.FirstOrDefault(x => x.MedicalRequestId == requestesStatus.RequestId);
                    if (medicalRequest != null)
                    {
                        var patientCard = db.PatientCard.FirstOrDefault(x => x.EmployeeId == medicalRequest.EmployeeId);
                        if (patientCard == null)
                        {
                            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == medicalRequest.EmployeeId);
                            if (person != null)
                            {
                                var age = DateTime.Today.Year - person.DateOfBirth.Year;
                                //var year = age / 365;
                                var dateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
                                if (ModelState.IsValid)
                                {
                                    var medical = new PatientCard
                                    {
                                        PatientCardId = person.EmployeeCode,
                                        EmployeeId = person.EmployeeId,
                                        //WorkSpace = patientCard.WorkSpace,
                                        Date = DateTime.Today,
                                        Age = age,
                                        Sex = person.Gender.ToString(),
                                        DateAmharic = dateAm,
                                        IsNew = true,
                                        IsSent = false,
                                        IsTested = false,
                                        LastModifiedDate = DateTime.Now
                                    };

                                    db.PatientCard.Add(medical);
                                    db.SaveChanges();

                                    #region operation log                

                                    UserHelper.OperationLog(Request.UserHostAddress, "Patient Card", "Success", User.Identity.Name, User.Identity.Name + " add Patient Card at date " + DateTime.Today + " card no " + medical.PatientCardId, Modules.HealthAndInsurance);

                                    #endregion
                                }

                            }
                        }
                    }
                }

                #endregion
            }
            return Json(new[] { requestesStatus }.ToDataSourceResult(request, ModelState));
        }
        public double GetWeekDays(DateTime startDate, DateTime endDate)
        {
            double noOfWeek = 0;
            var sundayweekendSetting = db.Weekends.Where(x => x.Name == WeekendName.SUNDAY).FirstOrDefault();
            var saturdayweekendSetting = db.Weekends.Where(x => x.Name == WeekendName.SATURDAY).FirstOrDefault();
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {

                                if (saturdayweekendSetting.IsHalfDay)
                                {
                                    noOfWeek += 0.5;
                                }
                                else
                                {
                                    noOfWeek += 1;

                                }

                            }
                            else
                            {
                                noOfWeek = 0;
                            }

                        }
                        //if (saturdayweekendSetting != null)
                        //{
                        //    if (saturdayweekendSetting.IsHalfDay)
                        //    {
                        //        noOfWeek += 0.5;
                        //    }
                        //    else
                        //    {
                        //        noOfWeek += 1;

                        //    }
                        //}
                        break;
                    case DayOfWeek.Sunday:

                        if (sundayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                if (sundayweekendSetting.IsHalfDay)
                                {
                                    noOfWeek += 0.5;
                                }
                                else
                                {
                                    noOfWeek += 1;

                                }
                            }

                        }
                        //if (sundayweekendSetting != null)
                        //{
                        //    if (sundayweekendSetting.IsHalfDay)
                        //    {
                        //        noOfWeek += 0.5;
                        //    }
                        //    else
                        //    {
                        //        noOfWeek += 1;

                        //    }
                        //}
                        //dayList.Add(testDate);
                        break;
                }
            }
            return noOfWeek;
        }
        public double GetHalf(DateTime startDate, DateTime endDate)
        {
            double noOfWeek = 0;
            var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {

                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null && saturdayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }
                        }
                        break;
                    case DayOfWeek.Sunday:


                        if (sundayweekendSetting != null && sundayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }
                        }

                        break;
                }
            }
            return noOfWeek;
        }
        public double GetNotHalf(DateTime startDate, DateTime endDate)
        {
            double noOfWeek = 0;
            var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {

                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null && !saturdayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }

                        }

                        break;
                    case DayOfWeek.Sunday:


                        if (sundayweekendSetting != null && !sundayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }
                        }


                        break;
                }
            }
            return noOfWeek;
        }
        public bool IsHalf(DateTime startDate, DateTime endDate)
        {
            bool isHalf = false;
            var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {

                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null && saturdayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                isHalf = true;
                            }

                        }

                        break;
                    case DayOfWeek.Sunday:


                        if (sundayweekendSetting != null && sundayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                isHalf = true;
                            }
                        }


                        break;
                }
            }
            return isHalf;
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRLeaveApprover,
            HrmConstants.HRRecruitmentApprover, HrmConstants.HRSelfServiceManager,
            HrmConstants.HRHealthApprover, HrmConstants.HRTrainingApprover, HrmConstants.HROverTimeRequestApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestesStatus_Destroy([DataSourceRequest]DataSourceRequest request, RequestesStatus requestesStatus)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    DateReqStatAm = requestesStatus.DateReqStatAm,
                    Remark = requestesStatus.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted recruitment request of " + requestesStatus.ReferenceType + " and status" + requestesStatus.Status.ToString());
                #endregion
                db.RequestesStatus.Attach(entity);
                db.RequestesStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { requestesStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
