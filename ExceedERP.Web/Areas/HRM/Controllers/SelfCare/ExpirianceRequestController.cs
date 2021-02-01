﻿using System;
using System.Collections.Generic;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.Common.Controllers;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService,  HrmConstants.HRDepartmentHead)]
    public class ExpirianceRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int EmployeeCode { get; set; }
        public static DateTime Date { get; set; }
        public static string AmharicDate { get; set; }
        HRMInputs inputs = new HRMInputs();
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
        public static List<string> reportsToEmployee = new List<string>();
        public ActionResult Index()
        {

            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            var employeesName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeesName;
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            reportsToEmployee = list.Select(x => x.PersonId.ToString()).ToList();
            reportsToEmployee.Add(EmployeeId.ToString());
            var context = new ApplicationDbContext();

            var allUsers = context.Users.ToList();

            var name = allUsers.FirstOrDefault(X => X.UserName == User.Identity.Name);
            if (name?.EmployeeId != null)
            {
                EmployeeCode = Convert.ToInt32(name.EmployeeId);
            }
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            ViewData["Department"] = db.MainCompanyStructures.ToList();
        }
        public string CheckStatuses(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Experience").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Experience" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Experience" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        public int SetEmployee(string x)
        {
            IsForOther = false;

            if (x == "true")
            {
                var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

                IsForOther = true;
                return list.Count();

            }
            return 0;
        }
        public void SetDate(string name, string value)
        {
            if (name == "ApplicationDateAm")
            {
                AmharicDate = value;
                var splitDate = AmharicDate.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult SendForApproval()
        {
            RequestesStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.RequestesStatus.FirstOrDefault(x => x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
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
        public ActionResult ExprianceRequests_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeids = EmployeeHelper.GetSubordinates(employeeid);
            IQueryable<ExperienceRequest> expriancerequests = db.ExprianceRequests.Where(x => employeeids.Contains(x.EmployeeId));

            DataSourceResult result = expriancerequests.ToDataSourceResult(request, exprianceRequest => new
            {
                ExperienceRequestId = exprianceRequest.ExperienceRequestId,
                EmployeeId = exprianceRequest.EmployeeId,
                ApplicationDate = exprianceRequest.ApplicationDate,
                Remark = exprianceRequest.Remark,
                ApplicationDateAm = exprianceRequest.ApplicationDateAm,
                EmployeeCode = exprianceRequest.EmployeeCode,
                RequestType = exprianceRequest.RequestType,
                DateCreated = exprianceRequest.DateCreated,
                LastModified = exprianceRequest.LastModified,
                CreatedBy = exprianceRequest.CreatedBy,
                ModifiedBy = exprianceRequest.ModifiedBy,
                OrgBranchName = exprianceRequest.OrgBranchName,
                IsForOther = exprianceRequest.IsForOther,
                DepartmentSendForApprovalBy = exprianceRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = exprianceRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = exprianceRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = exprianceRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = exprianceRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = exprianceRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = exprianceRequest.OnlineApprovedBy,
                OnlineApprovedTime = exprianceRequest.OnlineApprovedTime,
                IsOnlineApproved = exprianceRequest.IsOnlineApproved,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Create([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!IsForOther)
                {
                    EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                }


                else
                {
                    EmployeeId = experienceRequest.EmployeeId;
                }
                if (AmharicDate == null)
                {
                    ModelState.AddModelError("", "Application date value is mandatory!!");
                }
                else if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
                }

                else
                {
                    var entity = new ExperienceRequest
                    {
                        EmployeeId = EmployeeId,
                        ApplicationDate = Date,
                        Remark = experienceRequest.Remark,
                        ApplicationDateAm = AmharicDate,
                        EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == EmployeeId)?.EmployeeCode,
                        RequestType = experienceRequest.RequestType,
                        DateCreated = DateTime.Now,
                        LastModified = experienceRequest.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = experienceRequest.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        IsForOther = experienceRequest.IsForOther,
                    };

                    db.ExprianceRequests.Add(entity);
                    db.SaveChanges();
                    experienceRequest.ExperienceRequestId = entity.ExperienceRequestId;

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "ExpirianceRequest ", "Success", User.Identity.Name, User.Identity.Name + " add new Experiance Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                    #endregion

                    //#region notification



                    //var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    //var superviorId = EmployeeHelper.GetSupervisor(empid);

                    //NotifModels notif = new NotifModels
                    //{

                    //    UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                    //    Title = "Experience Request",
                    //    Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Experience",
                    //    URL = "~/HRM/ExpirianceRequest"
                    //};

                    //Notify.NotifyUser(notif);

                    //#endregion
                }
            }
            IsForOther = false;
            return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Update([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AmharicDate == null)
                {
                    AmharicDate = experienceRequest.ApplicationDateAm;
                    var splitDate = AmharicDate.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new ExperienceRequest
                    {
                        ExperienceRequestId = experienceRequest.ExperienceRequestId,
                        EmployeeId = experienceRequest.EmployeeId,
                        ApplicationDate = Date,
                        Remark = experienceRequest.Remark,
                        ApplicationDateAm = AmharicDate,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == experienceRequest.EmployeeId).FirstOrDefault().EmployeeCode,
                        RequestType = experienceRequest.RequestType,
                        DateCreated = experienceRequest.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = experienceRequest.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        OrgBranchName = experienceRequest.OrgBranchName,
                        IsForOther = experienceRequest.IsForOther,
                    };

                    db.ExprianceRequests.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "ExpirianceRequest ", "Success", User.Identity.Name, User.Identity.Name + " update Experiance Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                    #endregion

                }
            }
            IsForOther = false;
            return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Destroy([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ExperienceRequest
                {
                    ExperienceRequestId = experienceRequest.ExperienceRequestId,
                    EmployeeId = experienceRequest.EmployeeId,
                    ApplicationDate = experienceRequest.ApplicationDate,
                    Remark = experienceRequest.Remark,
                    ApplicationDateAm = experienceRequest.ApplicationDateAm,
                    EmployeeCode = experienceRequest.EmployeeCode,
                    RequestType = experienceRequest.RequestType,
                    DateCreated = experienceRequest.DateCreated,
                    LastModified = experienceRequest.LastModified,
                    CreatedBy = experienceRequest.CreatedBy,
                    ModifiedBy = experienceRequest.ModifiedBy,
                    OrgBranchName = experienceRequest.OrgBranchName,
                    IsForOther = experienceRequest.IsForOther,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Experience request of employee code  " + experienceRequest.EmployeeCode);
                #endregion
                db.ExprianceRequests.Attach(entity);
                db.ExprianceRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
