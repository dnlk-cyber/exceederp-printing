using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    [Authorize]
    public class EducationRequestStatusController : BaseController
    {
      
            private ExceedDbContext db = new ExceedDbContext();
            public static DateTime Date { get; set; }
            public static string DateAmharic { get; set; }
            public ActionResult Index()
            {

                GetStatusType();
                PopulateViewData();
                return View();
            }
        private void PopulateViewData()
        {

            ViewData["EmployeeList"] = db.Person_Employee
                .Select(x => new
                {
                    EmployeeId = x.EmployeeId,
                    HeadId = x.EmployeeId,
                    EmployeeCode = x.EmployeeCode,
                    FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName,
                    FullNameWithOutCode = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish

                }).ToList();
            ViewData["jobTitle"] = db.Lookups
                                    .Where(x => x.LookupType == LookUpTypes.JOB_TITLE)
                                    .Select(x => new
                                    {
                                        x.LookupId,
                                        JobTitleId = x.LookupId,
                                        x.Description

                                    }).ToList();
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;
            ViewData["EduLevel"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }).ToList();

        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }
        public void SetDate(string name, string value)
            {
                if (name == "DateAmharic")
                {
                    DateAmharic = value;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }



            }
            public string CheckStatus(int masterId)
            {
                var reques = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Education");
                if (reques != null)
                {
                    var checkApprove = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Education" && x.Status == RequestStatus.APPROVE);
                    if (checkApprove != null)
                    {
                        return "approved";
                    }
                    var deptApproval = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "Education" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                    if (deptApproval != null)
                    {
                        return "Dapproved";
                    }
                    return "true";
                }
                return "false";
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
                        DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
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
                        Date = DateTime.Now,
                        DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                    };

                    db.RequestesStatus.Add(entityy);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            
            public ActionResult EducationRequests_Read([DataSourceRequest]DataSourceRequest request)
            {
            List<EducationRequest> education = new List<EducationRequest>();
            IQueryable<EducationRequest> EducationRequests = db.EducationRequests.OrderBy(x => x.Date);

            foreach (var m in EducationRequests)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.WAITING_FOR_APPROVAL && x.RequestId == m.EducationRequestId && x.ReferenceType == "Education").FirstOrDefault();
                if (status != null)
                {
                    education.Add(m);
                }
            }
                DataSourceResult result = education.ToDataSourceResult(request, educationRequest => new
                {
                    EducationRequestId = educationRequest.EducationRequestId,
                    OrgStructureId = educationRequest.OrgStructureId,
                    EmployeeId = educationRequest.EmployeeId,
                    LookupId = educationRequest.LookupId,
                    JobTitleId = educationRequest.JobTitleId,
                    Remark = educationRequest.Remark,
                    Date = educationRequest.Date,
                    DateAmharic = educationRequest.DateAmharic,
                    DesiredTrainingDate = educationRequest.DesiredTrainingDate,
                    DesiredTrainingDateAm = educationRequest.DesiredTrainingDateAm,
                    DurationofTraining = educationRequest.DurationofTraining,
                    TrainingProvider = educationRequest.TrainingProvider,
                    Reason = educationRequest.Reason,
                    Status = educationRequest.Status,
                    EducationType = educationRequest.EducationType,
                    DepartmentSendForApprovalBy = educationRequest.DepartmentSendForApprovalBy,
                    DepartmentSendForApprovalTime = educationRequest.DepartmentSendForApprovalTime,
                    IsDepartmentSendForApproval = educationRequest.IsDepartmentSendForApproval,
                    DepartmentOnlineApprovedBy = educationRequest.DepartmentOnlineApprovedBy,
                    DepartmentOnlineApprovedTime = educationRequest.DepartmentOnlineApprovedTime,
                    IsDepartmentOnlineApproved = educationRequest.IsDepartmentOnlineApproved,
                    OnlineApprovedBy = educationRequest.OnlineApprovedBy,
                    OnlineApprovedTime = educationRequest.OnlineApprovedTime,
                    IsOnlineApproved = educationRequest.IsOnlineApproved,
                    IsOnlineTransferred = educationRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = educationRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = educationRequest.OnlineTransferredTime,
                    IsSendForApproval = educationRequest.IsSendForApproval,
                    SendForApprovalBy = educationRequest.SendForApprovalBy,
                    SendForApprovalTime = educationRequest.SendForApprovalTime,
                    IsVoid = educationRequest.IsVoid,
                    VoidBy = educationRequest.VoidBy,
                    VoidTime = educationRequest.VoidTime,
                    OrgBranchName = educationRequest.OrgBranchName,
                    CreatedBy = educationRequest.CreatedBy,
                    DateCreated = educationRequest.DateCreated,
                    ModifiedBy = educationRequest.ModifiedBy,
                    LastModified = educationRequest.LastModified,

                });

                return Json(result);
            }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationRequests_Create([DataSourceRequest]DataSourceRequest request, EducationRequest educationRequest)
        {

            if (string.IsNullOrEmpty(educationRequest.DateAmharic))
                ModelState.AddModelError("", "Date is required");

            if (!string.IsNullOrEmpty(educationRequest.DateAmharic))
            {
                var splitDate = educationRequest.DateAmharic.Split('/');
                educationRequest.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (!string.IsNullOrEmpty(educationRequest.DesiredTrainingDateAm))
            {
                var splitDate = educationRequest.DesiredTrainingDateAm.Split('/');
                educationRequest.DesiredTrainingDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (educationRequest.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                return Json(new[] { educationRequest }.ToDataSourceResult(request, ModelState));

            }
            if (educationRequest.EmployeeId == 0)
            {
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                educationRequest.EmployeeId = empId;
            }

            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == educationRequest.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                educationRequest.EmployeeId = placement.EmployeeId;
                educationRequest.JobTitleId = placement.LookupId;
                educationRequest.OrgStructureId = placement.OrgStructureId;

            }
            var plans = db.EducationRequests
                  .FirstOrDefault(x => x.LookupId == educationRequest.LookupId
                                      && x.EmployeeId == educationRequest.EmployeeId
                                      && x.OrgStructureId == educationRequest.OrgStructureId
                                      && x.JobTitleId == educationRequest.JobTitleId
                                      && x.OrgStructureId == educationRequest.OrgStructureId
                                      && x.LookupId == educationRequest.LookupId);
            if (plans != null)
            {
                ModelState.AddModelError("", "Record Exist");
            }


            if (ModelState.IsValid)
            {
                //var userDepo = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name);
                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var entity = GetMappedObject(educationRequest);

                db.EducationRequests.Add(entity);
                db.SaveChanges();
                educationRequest.EducationRequestId = entity.EducationRequestId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "EducationRequest ", "Success", User.Identity.Name, User.Identity.Name + " add new Education Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

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
                            Title = "Education Request",
                            Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has requested Education for { deptName }"
                        };

                        Notify.NotifyUser(notif);
                    }
                }



                #endregion

            }

            return Json(new[] { educationRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationRequests_Update([DataSourceRequest]DataSourceRequest request, EducationRequest educationRequest)
        {
            if (string.IsNullOrEmpty(educationRequest.DateAmharic))
                ModelState.AddModelError("", "Date Is required");

            if (!string.IsNullOrEmpty(educationRequest.DateAmharic))
            {
                var splitDate = educationRequest.DateAmharic.Split('/');
                educationRequest.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (!string.IsNullOrEmpty(educationRequest.DesiredTrainingDateAm))
            {
                var splitDate = educationRequest.DesiredTrainingDateAm.Split('/');
                educationRequest.DesiredTrainingDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (educationRequest.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                return Json(new[] { educationRequest }.ToDataSourceResult(request, ModelState));

            }
           

            
            var plans = db.EducationRequests
                  .FirstOrDefault(x => x.LookupId == educationRequest.LookupId
                                      && x.EmployeeId == educationRequest.EmployeeId
                                      && x.OrgStructureId == educationRequest.OrgStructureId
                                      && x.JobTitleId == educationRequest.JobTitleId
                                      && x.OrgStructureId == educationRequest.OrgStructureId
                                      && x.EducationRequestId != educationRequest.EducationRequestId
                                      && x.LookupId == educationRequest.LookupId);
            if (plans != null)
            {
                ModelState.AddModelError("", "Record Exist");
            }

            if (ModelState.IsValid)
            {


                var entity = GetMappedObject(educationRequest);
                db.EducationRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "EducationRequest ", "Success", User.Identity.Name, User.Identity.Name + " update Education Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                #endregion


            }

            return Json(new[] { educationRequest }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EducationRequests_Destroy([DataSourceRequest]DataSourceRequest request, EducationRequest educationRequest)
        {
                ModelState.Clear();

            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(educationRequest);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training applicant information of work unit " + educationRequest.OrgStructureId);
                #endregion
                db.EducationRequests.Attach(entity);
                db.EducationRequests.Remove(entity);
                db.SaveChanges();

            }


            return Json(new[] { educationRequest }.ToDataSourceResult(request, ModelState));

        }
        private EducationRequest GetMappedObject(EducationRequest educationRequest)
        {

            var entity = new EducationRequest
            {
                EducationRequestId = educationRequest.EducationRequestId,
                OrgStructureId = educationRequest.OrgStructureId,
                EmployeeId = educationRequest.EmployeeId,
                LookupId = educationRequest.LookupId,
                JobTitleId = educationRequest.JobTitleId,
                Remark = educationRequest.Remark,
                Date = educationRequest.Date,
                DateAmharic = educationRequest.DateAmharic,
                DesiredTrainingDate = educationRequest.DesiredTrainingDate,
                DesiredTrainingDateAm = educationRequest.DesiredTrainingDateAm,
                DurationofTraining = educationRequest.DurationofTraining,
                TrainingProvider = educationRequest.TrainingProvider,
                Reason = educationRequest.Reason,
                Status = educationRequest.Status,
                EducationType = educationRequest.EducationType,
                DepartmentSendForApprovalBy = educationRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = educationRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = educationRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = educationRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = educationRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = educationRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = educationRequest.OnlineApprovedBy,
                OnlineApprovedTime = educationRequest.OnlineApprovedTime,
                IsOnlineApproved = educationRequest.IsOnlineApproved,
                IsOnlineTransferred = educationRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = educationRequest.IsOnlineTransferredBy,
                OnlineTransferredTime = educationRequest.OnlineTransferredTime,
                IsSendForApproval = educationRequest.IsSendForApproval,
                SendForApprovalBy = educationRequest.SendForApprovalBy,
                SendForApprovalTime = educationRequest.SendForApprovalTime,
                IsVoid = educationRequest.IsVoid,
                VoidBy = educationRequest.VoidBy,
                VoidTime = educationRequest.VoidTime,
                OrgBranchName = educationRequest.OrgBranchName,
                CreatedBy = educationRequest.CreatedBy,
                DateCreated = educationRequest.DateCreated,
                ModifiedBy = educationRequest.ModifiedBy,
                LastModified = educationRequest.LastModified,
            };
            return entity;
        }

        protected override void Dispose(bool disposing)
            {
                db.Dispose();
                base.Dispose(disposing);
            }
        }
    }