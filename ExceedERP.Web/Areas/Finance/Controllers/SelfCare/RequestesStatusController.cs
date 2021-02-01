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
using ExceedERP.Web.Areas.HRM.Controllers;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    public class RequestesStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveApprover,
            HrmConstants.HRRecruitmentApprover, HrmConstants.HRSelfServiceManager, HrmConstants.HREmployeeSelfService, HrmConstants.HROverTimeRequestApprover,
           HrmConstants.HRHealthApprover, HrmConstants.HRRecruitmentRequester, HrmConstants.HRTerminationUser, HrmConstants.HRTrainingRequester, HrmConstants.HRTrainingApprover)]
        public ActionResult Index()
        {
            return View();
        }

       
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveApprover,
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
                DateReqStatAm = requestesStatus.DateReqStatAm
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

        //[Authorize(Roles = HR_Recruitment_Admin + "," + HR_Recruitment_Approver + "," + HR_Manager_SelfService + "," + HR_Leave_Approver + ","
        //    + OverTime_Request_Approver + "," + Health_Approver + "," + HR_Training_Approver)]

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
            HrmConstants.HRRecruitmentApprover, HrmConstants.HRSelfServiceManager,
            HrmConstants.HRHealthApprover, HrmConstants.HRTrainingApprover, HrmConstants.HROverTimeRequestApprover)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RequestesStatus_Update([DataSourceRequest]DataSourceRequest request, RequestesStatus requestesStatus)
        {

            ModelState.Clear();
            if (requestesStatus.ReferenceType == "Leave")
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               var requesteEmpId=requestesStatus.RequestId;
                var leaveReq = db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);
                if (leaveReq != null)
                {
                    var supervisorId = EmployeeHelper.GetSupervisor(leaveReq.EmployeeId);
                    var roleCheck = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRLeaveApprover);
                    var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                        x.RequestId == leaveReq.LeaveRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType== "Leave");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == leaveReq.LeaveRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Leave");

                    if (deptapp==null && requestesStatus.Status==RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("","This request is not approved by department so You can not approve");
                    }
                    if (deptapp != null && requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        ModelState.AddModelError("", "You Approved This Request before so Cannot approve again");
                    }
                    if (hrapprove != null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "You Approved This Request before so Cannot approve again");
                    }
                    if (requestesStatus.Status == RequestStatus.APPROVE && !roleCheck)
                    {
                        ModelState.AddModelError("","you cannot approve as HR");
                    }

                    if (requestesStatus.Status==RequestStatus.DEPARTMENT_APPROVAL && empId!=supervisorId)
                    {
                        ModelState.AddModelError("","You are not Head of This department you can not Approve as Head");
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
                        x.RequestId == termReq.TerminationId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType== "Termination");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == termReq.TerminationId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Termination");
                    if (deptapp == null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "This request is not approved by department so You can not approve");
                    }
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
                        x.RequestId == expReq.ExperienceRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType== "Experience");
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
                    var roleCheck = UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRHealthApprover);
                    var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                        x.RequestId == medReq.MedicalRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType== "Medical");
                    var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                       x.RequestId == medReq.MedicalRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Medical");
                    if (deptapp == null && requestesStatus.Status == RequestStatus.APPROVE)
                    {
                        ModelState.AddModelError("", "This request is not approved by department so You can not approve");
                    }
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
                    if (recHead!=null)
                    {
                        var supervisorId = EmployeeHelper.GetSupervisor(Convert.ToInt32(recHead));
                        var roleCheck =
                            UserInformationHelper.CheckUserRole(User.Identity.Name, HrmConstants.HRRecruitmentApprover);
                        

                        var hrapprove = db.RequestesStatus.FirstOrDefault(x =>
                     x.RequestId == recruitReq.RecruitmentRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Recruitment");

                        var deptapp = db.RequestesStatus.FirstOrDefault(x =>
                     x.RequestId == recruitReq.RecruitmentRequestId && x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.ReferenceType == "Recruitment");

                        if (requestesStatus.Status == RequestStatus.DEPARTMENT_APPROVAL && empId != supervisorId)
                        {
                            ModelState.AddModelError("",
                                "You are not Head of This department you can not Approve as Head");
                        }
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
                            x.Status == RequestStatus.DEPARTMENT_APPROVAL &&x.ReferenceType== "Training");
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

            if (ModelState.IsValid)
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    ChangedBy=User.Identity.Name,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Remark = requestesStatus.Remark
                };

                db.RequestesStatus.Add(entity);
                db.SaveChanges();
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
                    else if (requestesStatus.Status == RequestStatus.APPROVE)
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
                                Message = "Your Leave Request are Approved"
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
                                Message = "Your Terminaiton Request are Approved"
                            };

                            Notify.NotifyUser(notif);
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

                if (requestesStatus.ReferenceType == "Leave" && requestesStatus.Status == RequestStatus.APPROVE)
                {


                    var leaves = db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == requestesStatus.RequestId);
                    if (leaves != null)
                    {
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
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        };
                        if (entityy.LeaveType == LeaveType.ANNUAL)
                        {
                            entityy.Amount = entityy.To.Subtract(entityy.From).Days + 1 - count;
                            entityy.Amount = entityy.Amount - Convert.ToDecimal(weekDays);
                            if (entityy.Day == LeaveDay.ግማሽ_ቀን)
                            {
                                entityy.Amount = entityy.Amount / 2;
                            }

                            decimal days = entityy.Amount * 8;
                            decimal leaveDays = entityy.Amount * entityy.Hours;
                            entityy.Amount = (days - leaveDays) / 8;

                            #region update balance


                            var entiteledEmployee = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == entityy.EmployeeId && x.GlFiscalYearId == entityy.GlFiscalYearId);
                            if (entiteledEmployee != null)
                            {
                                decimal totalAmountField = entiteledEmployee.TotlalAmount;
                                decimal totalEntiteleft = totalAmountField - entityy.Amount;
                                entiteledEmployee.TotlalAmount = totalEntiteleft;

                                // db.LeavesEntitle.Attach(entiteledEmployee);
                                db.Entry(entiteledEmployee).State = EntityState.Modified;
                                db.SaveChanges();

                            }



                            #endregion

                        }
                        else
                        {


                            entityy.Amount = entityy.To.Subtract(entityy.From).Days;
                        }

                        db.Leaves.Add(entityy);
                        db.SaveChanges();
                        //leave.TakenLeaveId = entityy.TakenLeaveId;





                    }
                }
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
                            if (saturdayweekendSetting.IsHalfDay)
                            {
                                noOfWeek += 0.5;
                            }
                            else
                            {
                                noOfWeek += 1;

                            }
                        }
                        break;
                    case DayOfWeek.Sunday:

                        if (sundayweekendSetting != null)
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
                        //dayList.Add(testDate);
                        break;
                }
            }
            return noOfWeek;
        }

        //[Authorize(Roles = HR_Recruitment_Admin + "," + HR_Recruitment_Approver + "," + HR_Manager_SelfService + "," + HR_Leave_Approver + ","
        //    + OverTime_Request_Approver + "," + Health_Approver + "," + HR_Training_Approver)]


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
