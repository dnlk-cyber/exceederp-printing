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
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRProfileAdmin, HrmConstants.HRDepartmentHead)]
    public class ExpirianceRequestStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            ViewData["Department"] = db.MainCompanyStructures.ToList();
        }

        public ActionResult SendForApproval()
        {
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
                    Date = DateTime.Now
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
                    Remark = requestesStatus.Remark
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
                    Date = DateTime.Now
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin, HrmConstants.HRProfileAdmin, HrmConstants.HRDepartmentHead)]
        public ActionResult ExprianceRequests_Read([DataSourceRequest]DataSourceRequest request)
        {
           // List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ExperienceRequest> expriancerequests = db.ExprianceRequests;//.Where(x => mybraches.Contains(x.OrgBranchName));
            List<ExperienceRequest> Requstes = new List<ExperienceRequest>();
            foreach (var m in expriancerequests)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.RequestId == m.ExperienceRequestId && x.ReferenceType == "Experience").FirstOrDefault();
                if (status != null)
                {
                    Requstes.Add(m);
                }

            }

            expriancerequests = Requstes.AsQueryable().OrderBy(x => x.ApplicationDate);
            DataSourceResult result = expriancerequests.ToDataSourceResult(request, exprianceRequest => new
            {
                ExperienceRequestId = exprianceRequest.ExperienceRequestId,
                EmployeeId = exprianceRequest.EmployeeId,
                ApplicationDate = exprianceRequest.ApplicationDate,
                Remark = exprianceRequest.Remark,
                EmployeeCode = exprianceRequest.EmployeeCode,
                ApplicationDateAm = exprianceRequest.ApplicationDateAm,
                DateCreated = exprianceRequest.DateCreated,
                LastModified = exprianceRequest.LastModified,
                CreatedBy = exprianceRequest.CreatedBy,
                ModifiedBy = exprianceRequest.ModifiedBy,
                RequestType = exprianceRequest.RequestType,
                OrgBranchName = exprianceRequest.OrgBranchName,
                DepartmentSendForApprovalBy = exprianceRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = exprianceRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = exprianceRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = exprianceRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = exprianceRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = exprianceRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = exprianceRequest.OnlineApprovedBy,
                OnlineApprovedTime = exprianceRequest.OnlineApprovedTime,
                IsOnlineApproved = exprianceRequest.IsOnlineApproved,
                IsDirect = exprianceRequest.IsDirect
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Create([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == experienceRequest.EmployeeId);
            if(person != null)
            {
                experienceRequest.EmployeeCode = person.EmployeeCode;
            }
            experienceRequest.ApplicationDate = DateTime.Today;
            experienceRequest.ApplicationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
            if (ModelState.IsValid)
            {
                var entity = new ExperienceRequest
                {
                    EmployeeId = experienceRequest.EmployeeId,
                    EmployeeCode = experienceRequest.EmployeeCode,
                    ApplicationDate = experienceRequest.ApplicationDate,
                    ApplicationDateAm = experienceRequest.ApplicationDateAm,
                    Remark = experienceRequest.Remark,
                    DateCreated = experienceRequest.DateCreated,
                    LastModified = experienceRequest.LastModified,
                    CreatedBy = experienceRequest.CreatedBy,
                    ModifiedBy = experienceRequest.ModifiedBy,
                    OrgBranchName = experienceRequest.OrgBranchName,
                    IsDirect = true,
                };

                db.ExprianceRequests.Add(entity);
                db.SaveChanges();
                experienceRequest.ExperienceRequestId = entity.ExperienceRequestId;

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                var requestStatus1 = new RequestesStatus
                {
                    RequestId = entity.ExperienceRequestId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Experience",
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                };

                db.RequestesStatus.Add(requestStatus1);
                db.SaveChanges();

                var requestStatus2 = new RequestesStatus
                {
                    RequestId = entity.ExperienceRequestId,
                    Date = DateTime.Now.Date,
                    ReferenceType = "Experience",
                    Status = RequestStatus.DEPARTMENT_APPROVAL,
                };

                db.RequestesStatus.Add(requestStatus2);
                db.SaveChanges();
            }

            return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Update([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExperienceRequest
                {
                    ExperienceRequestId = experienceRequest.ExperienceRequestId,
                    EmployeeId = experienceRequest.EmployeeId,
                    ApplicationDate = experienceRequest.ApplicationDate,
                    Remark = experienceRequest.Remark,
                    IsDirect = experienceRequest.IsDirect,
                    IsDepartmentOnlineApproved = experienceRequest.IsDepartmentOnlineApproved,
                    IsOnlineApproved = experienceRequest.IsOnlineApproved,
                    IsSendForApproval = experienceRequest.IsSendForApproval,
                };

                db.ExprianceRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Destroy([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExperienceRequest
                {
                    ExperienceRequestId = experienceRequest.ExperienceRequestId,
                    EmployeeId = experienceRequest.EmployeeId,
                    ApplicationDate = experienceRequest.ApplicationDate,
                    Remark = experienceRequest.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Experience Requests information of Employee " + experienceRequest.EmployeeId);
                #endregion
                db.ExprianceRequests.Attach(entity);
                db.ExprianceRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult UpdateExperience()
        {
            var experiences = db.ExprianceRequests.ToList();

            foreach (var exp in experiences)
            {
                var requestStatuses = db.RequestesStatus.Where(x => x.RequestId == exp.ExperienceRequestId).ToList();

                foreach (var req in requestStatuses)
                {
                    if (req.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        exp.IsSendForApproval = true;
                    }
                    if (req.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        exp.IsDepartmentOnlineApproved = true;
                    }
                    if (req.Status == RequestStatus.APPROVE)
                    {
                        exp.IsOnlineApproved = true;
                    }

                    db.Entry(exp).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        public JsonResult ApproveExperienceRequest(int id)
        {
            var experience = db.ExprianceRequests.FirstOrDefault(x => x.ExperienceRequestId == id);
            if (experience != null)
            {
                if (experience.IsOnlineApproved)
                {
                    var formatedData = new
                    {
                        Message = "This request is already approved",
                        Status = "FAIL"
                    };
                    return Json(formatedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    experience.IsOnlineApproved = true;

                    db.Entry(experience).State = EntityState.Modified;
                    //db.SaveChanges();
                    #region change resquest status 

                    var status = new RequestesStatus
                    {
                        RequestId = experience.ExperienceRequestId,
                        Date = DateTime.Now.Date,
                        ReferenceType = "Experience",
                        Status = RequestStatus.APPROVE

                    };
                    db.RequestesStatus.Add(status);

                    #endregion

                    db.SaveChanges();

                    var formatedData = new
                    {
                        Message = "This request is successfully Approved",
                        Status = "OK"
                    };
                    return Json(formatedData, JsonRequestBehavior.AllowGet);
                }
            }
            var formatedData1 = new
            {
                Message = "Please select employee",
                Status = "Fail"
            };
            return Json(formatedData1, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
