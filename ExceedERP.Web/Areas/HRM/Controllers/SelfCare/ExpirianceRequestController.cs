using System;
using System.Collections.Generic;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class ExpirianceRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public static List<string> reportsToEmployee = new List<string>();
        public ActionResult Index()
        {

            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
        
            // Employees for dropdown (used by ApproverId column)
            var employees = db.Person_Employee
                              .Where(e => e.IsActive)
                              .Select(e => new
                              {
                                  PersonId = e.EmployeeId,
                                  EmployeeId = e.EmployeeId,
                                  FullNameAmharic= e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                  FullNameWithOutCode= e.FirstNameEnglish+" "+e.MiddleNameEnglish+" "+e.LastNameEnglish
                              }).AsQueryable();
            // Assign to ViewData for Kendo Grid ForeignKey binding
            ViewData["EmployeeList"] = employees; // For EmployeeId column
            ViewData["EmployeeName"] = employees;    // For ApproverId column
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

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

            if (x == "true")
            {
                var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

                return list.Count();

            }
            return 0;
        }
        public void SetDate(string name, string value)
        {
            if (name == "ApplicationDateAm")
            {
                var AmharicDate = value;
                var splitDate = AmharicDate.Split('/');
                var Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
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
        public JsonResult SendForApprovalExperienceRequest(int id)
        {
            HRMInputs.RequestId = id;
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
                var formatedData = new  {
                    Message ="Successfully Sent",
                    status ="Okay"
                };
                return Json(formatedData, JsonRequestBehavior.AllowGet);
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
                var formatedData = new
                {
                    Message = "Successfully Sent",
                    status = "Okay"
                };
                return Json(formatedData, JsonRequestBehavior.AllowGet);

            }
            
        }
        //public bool CheckStatusForBackColor(int exId)
        //{
        //    var reques = db.RequestesStatus.Where(x => x.RequestId == exId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Experience").FirstOrDefault();
        //    if (reques != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public ActionResult ExprianceRequests_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //    List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
        //    IQueryable<ExperienceRequest> expriancerequests = db.ExprianceRequests.Where(x => employeeIds.Contains(x.EmployeeId)||x.EmployeeId==employeeId);
        //    DataSourceResult result = expriancerequests.ToDataSourceResult(request, exprianceRequest => new
        //    {
        //        ExperienceRequestId = exprianceRequest.ExperienceRequestId,
        //        EmployeeId = exprianceRequest.EmployeeId,
        //        ApplicationDate = exprianceRequest.ApplicationDate,
        //        Remark = exprianceRequest.Remark,
        //        ApplicationDateAm = exprianceRequest.ApplicationDateAm,
        //        EmployeeCode = exprianceRequest.EmployeeCode,
        //        RequestType = exprianceRequest.RequestType,
        //        DateCreated = exprianceRequest.DateCreated,
        //        LastModified = exprianceRequest.LastModified,
        //        CreatedBy = exprianceRequest.CreatedBy,
        //        ModifiedBy = exprianceRequest.ModifiedBy,
        //        OrgBranchName = exprianceRequest.OrgBranchName,
        //        IsForOther = exprianceRequest.IsForOther,
        //        IsApproved = CheckStatusForBackColor(exprianceRequest.ExperienceRequestId),
        //        DepartmentSendForApprovalBy = exprianceRequest.DepartmentSendForApprovalBy,
        //        DepartmentSendForApprovalTime = exprianceRequest.DepartmentSendForApprovalTime,
        //        IsDepartmentSendForApproval = exprianceRequest.IsDepartmentSendForApproval,
        //        DepartmentOnlineApprovedBy = exprianceRequest.DepartmentOnlineApprovedBy,
        //        DepartmentOnlineApprovedTime = exprianceRequest.DepartmentOnlineApprovedTime,
        //        IsDepartmentOnlineApproved = exprianceRequest.IsDepartmentOnlineApproved,
        //        OnlineApprovedBy = exprianceRequest.OnlineApprovedBy,
        //        OnlineApprovedTime = exprianceRequest.OnlineApprovedTime,
        //        IsOnlineApproved = exprianceRequest.IsOnlineApproved,
        //    });

        //    return Json(result);
        //}

        public ActionResult ExprianceRequests_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);

            var expriancerequests = db.ExprianceRequests
                .Where(x => employeeIds.Contains(x.EmployeeId) || x.EmployeeId == employeeId);

            // Fetch all approved experience request IDs in one query
            var approvedRequestIds = db.RequestesStatus
                .Where(x => x.Status == RequestStatus.APPROVE && x.ReferenceType == "Experience")
                .Select(x => x.RequestId)
                .ToList();

            var result = expriancerequests.ToDataSourceResult(request, exprianceRequest => new
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
                // Computed field without DB hit
                IsApproved = approvedRequestIds.Contains(exprianceRequest.ExperienceRequestId),
                DepartmentSendForApprovalBy = exprianceRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = exprianceRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = exprianceRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = exprianceRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = exprianceRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = exprianceRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = exprianceRequest.OnlineApprovedBy,
                OnlineApprovedTime = exprianceRequest.OnlineApprovedTime,
                IsOnlineApproved = exprianceRequest.IsOnlineApproved
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Create([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            ModelState.Clear();

            if (!experienceRequest.IsForOther)
            {
                experienceRequest.EmployeeId  = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }
           
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrWhiteSpace(experienceRequest.ApplicationDateAm))
                {
                    ModelState.AddModelError("", "Application date (Ethiopian) is required.");
                    return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
                }

                // Convert from Ethiopian to Gregorian
                experienceRequest.ApplicationDate = HrHelper.ParseEthiopicDate(experienceRequest.ApplicationDateAm);
            }
            else
            {
                if (experienceRequest.ApplicationDate == default(DateTime))
                {
                    ModelState.AddModelError("", "Application date (Gregorian) is required.");
                    return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
                }

                // Convert from Gregorian to Ethiopian
                experienceRequest.ApplicationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    experienceRequest.ApplicationDate.Day,
                    experienceRequest.ApplicationDate.Month,
                    experienceRequest.ApplicationDate.Year
                );
            }
            if (experienceRequest.ApplicationDate > DateTime.Now)
            {

                ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
                return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));

            }
            var lastApproved = (from r in db.ExprianceRequests
                                join s in db.RequestesStatus on r.ExperienceRequestId equals s.RequestId
                                where r.EmployeeId == experienceRequest.EmployeeId
                                      && s.Status == RequestStatus.APPROVE
                                      && s.ReferenceType == "Experience"
                                orderby s.Date descending
                                select s.Date)
                    .FirstOrDefault();

            if (lastApproved != default(DateTime))
            {
                var daysSinceApproval = (DateTime.Now.Date - lastApproved.Date).TotalDays;

                if (daysSinceApproval < 180)
                {
                    ModelState.AddModelError("", "You cannot submit a new request within six (6) months of the last approval.");
                    return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new ExperienceRequest
                {
                    EmployeeId = experienceRequest.EmployeeId,
                    ApplicationDate = experienceRequest.ApplicationDate,
                    Remark = experienceRequest.Remark,
                    ApplicationDateAm = experienceRequest.ApplicationDateAm,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == experienceRequest.EmployeeId)?.EmployeeCode,
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

            }
            return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExprianceRequests_Update([DataSourceRequest]DataSourceRequest request, ExperienceRequest experienceRequest)
        {
            ModelState.Clear();

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(experienceRequest.ApplicationDateAm))
                {
                    ModelState.AddModelError("", "Application  date fields is mandatory");
                    return Json(new[] { experienceRequest }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    experienceRequest.ApplicationDate = HrHelper.ParseEthiopicDate(experienceRequest.ApplicationDateAm);
                }

            }
            else
            {

                experienceRequest.ApplicationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(experienceRequest.ApplicationDate.Day, experienceRequest.ApplicationDate.Month, experienceRequest.ApplicationDate.Year);

            }

            if (experienceRequest.ApplicationDate > DateTime.Now)
            {

                ModelState.AddModelError("", "Application date should be less or equal to the current date!!");
            }
            if (ModelState.IsValid)
            {
                var entity = new ExperienceRequest
                {
                    ExperienceRequestId = experienceRequest.ExperienceRequestId,
                    EmployeeId = experienceRequest.EmployeeId,
                    ApplicationDate = experienceRequest.ApplicationDate,
                    Remark = experienceRequest.Remark,
                    ApplicationDateAm = experienceRequest.ApplicationDateAm,
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
