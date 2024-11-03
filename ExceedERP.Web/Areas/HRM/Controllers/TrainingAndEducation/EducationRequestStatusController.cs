using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class EducationRequestStatusController : BaseController
    {
      
            private ExceedDbContext db = new ExceedDbContext();
            public static DateTime Date { get; set; }
            public static string DateAmharic { get; set; }
            public ActionResult Index()
            {

                GetCourse();
                GetDepartments();
                GetStatusType();
                PopulateViewData();
                return View();
            }
            private void PopulateViewData()
            {
                HRMInputs inputs = new HRMInputs();
                var list = inputs.GetEmployees(User.Identity.Name);
           var employees = db.Person_Employee.ToList();
            ViewData["EmployeeList"] = employees;

            var depo = db.MainCompanyStructures.ToList();
            ViewData["Depo"] = depo;

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
            private void GetStatusType()
            {
                var ststus = Enum.GetValues(typeof(RequestStatus))
                    .Cast<RequestStatus>().ToList();

                var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
                ViewData["RequestStatus"] = ststuses;
            }
            public void GetDepartments()
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
            public JsonResult GetCourse()
            {
                var eduLevel = db.Lookups.ToList();

                if (eduLevel.Any())
                {
                    eduLevel = eduLevel.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();
                }
                ViewData["EduLevel"] = eduLevel.ToList();
                return Json(eduLevel.Select(p => new
                {
                    LookupId = p.LookupId,
                    Description = p.Description
                }), JsonRequestBehavior.AllowGet);
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
                DataSourceResult result = education.ToDataSourceResult(request, EducationRequest => new
                {
                    EducationRequestId = EducationRequest.EducationRequestId,
                    OrgStructureId = EducationRequest.OrgStructureId,
                    EmployeeId = EducationRequest.EmployeeId,
                    LookupId = EducationRequest.LookupId,
                    Remark = EducationRequest.Remark,
                    Date = EducationRequest.Date,
                    EducationType = EducationRequest.EducationType,
                    DateAmharic = EducationRequest.DateAmharic,

                });

                return Json(result);
            }
            [AcceptVerbs(HttpVerbs.Post)]
            public ActionResult EducationRequests_Create([DataSourceRequest]DataSourceRequest request, EducationRequest EducationRequest)
            {
                string messages = "";
                ModelState.Clear();

                var plans =
                       db.TrainingPlans.FirstOrDefault(x => x.Course == EducationRequest.LookupId && x.WorkUnit == EducationRequest.OrgStructureId);
                if (plans != null)
                {

                }
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date is mandatory!!");
                }
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }


                if (ModelState.IsValid)
                {

                    var entity = new EducationRequest
                    {
                        OrgStructureId = EducationRequest.OrgStructureId,
                        LookupId = EducationRequest.LookupId,
                        Remark = EducationRequest.Remark,
                        Date = Date,
                        DateAmharic = DateAmharic,
                        EducationType = EducationRequest.EducationType

                    };

                    db.EducationRequests.Add(entity);
                    db.SaveChanges();
                    EducationRequest.EducationRequestId = entity.EducationRequestId;

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
                                Title = "Training Request",
                                Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has requested Training for { deptName }"
                            };

                            Notify.NotifyUser(notif);
                        }
                    }



                    #endregion

                }

                return Json(new[] { EducationRequest }.ToDataSourceResult(request, ModelState));
            }
            [AcceptVerbs(HttpVerbs.Post)]
            public ActionResult EducationRequests_Update([DataSourceRequest]DataSourceRequest request, EducationRequest EducationRequest)
            {
                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    if (DateAmharic == null)
                    {
                        DateAmharic = EducationRequest.DateAmharic;
                        var splitDate = DateAmharic.Split('/');
                        Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    }
                    if (Date.Date > DateTime.Now.Date)
                    {

                        ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                    }
                    else
                    {
                        var entity = new EducationRequest
                        {
                            EducationRequestId = EducationRequest.EducationRequestId,
                            OrgStructureId = EducationRequest.OrgStructureId,
                            LookupId = EducationRequest.LookupId,
                            Remark = EducationRequest.Remark,
                            Date = Date,
                            EducationType = EducationRequest.EducationType,
                            DateAmharic = DateAmharic,

                        };

                        db.EducationRequests.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return Json(new[] { EducationRequest }.ToDataSourceResult(request, ModelState));
            }
            [AcceptVerbs(HttpVerbs.Post)]
            public ActionResult EducationRequests_Destroy([DataSourceRequest]DataSourceRequest request, EducationRequest EducationRequest)
            {
                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    var entity = new EducationRequest
                    {
                        EducationRequestId = EducationRequest.EducationRequestId,
                        OrgStructureId = EducationRequest.OrgStructureId,
                        LookupId = EducationRequest.LookupId,
                        Remark = EducationRequest.Remark,
                        EducationType = EducationRequest.EducationType,
                        Date = EducationRequest.Date,

                        DateAmharic = EducationRequest.DateAmharic,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training applicant information of work unit " + EducationRequest.OrgStructureId);
                    #endregion
                    db.EducationRequests.Attach(entity);
                    db.EducationRequests.Remove(entity);
                    db.SaveChanges();
                }

                return Json(new[] { EducationRequest }.ToDataSourceResult(request, ModelState));
            }
            protected override void Dispose(bool disposing)
            {
                db.Dispose();
                base.Dispose(disposing);
            }
        }
    }