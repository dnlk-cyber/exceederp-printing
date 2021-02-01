using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class TrainingApplicantController : BaseController
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
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);

            ViewData["Department"] = db.MainCompanyStructures.ToList();

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
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
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Training").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Training" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Training" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
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
            var courseList = db.Lookups.ToList();



            if (courseList.Count() > 0)
            {
                courseList = courseList.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).ToList();
            }
            ViewData["CourseList"] = courseList != null ? courseList.ToList() : new List<Lookup>();
            return Json(courseList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TrainingApplicants_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TrainingApplicant> trainingapplicants = db.TrainingApplicants.OrderBy(x => x.Date);

            //var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //List<int> employeeids = EmployeeHelper.GetSubordinates(employeeid);
            //IQueryable<LeaveRequest> leaverequest = db.TrainingApplicants.Where(x => employeeids.Contains(x.EmployeeId)).OrderBy(x => x.RequestDate);

            DataSourceResult result = trainingapplicants.ToDataSourceResult(request, trainingApplicant => new
            {
                TrainingApplicantId = trainingApplicant.TrainingApplicantId,
                OrgStructureId = trainingApplicant.OrgStructureId,
                LookupId = trainingApplicant.LookupId,
                NoofTrainee = trainingApplicant.NoofTrainee,
                Remark = trainingApplicant.Remark,
                Date = trainingApplicant.Date,
                DateAmharic = trainingApplicant.DateAmharic,
                DepartmentSendForApprovalBy = trainingApplicant.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = trainingApplicant.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = trainingApplicant.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = trainingApplicant.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = trainingApplicant.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = trainingApplicant.IsDepartmentOnlineApproved,
                OnlineApprovedBy = trainingApplicant.OnlineApprovedBy,
                OnlineApprovedTime = trainingApplicant.OnlineApprovedTime,
                IsOnlineApproved = trainingApplicant.IsOnlineApproved,


            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicants_Create([DataSourceRequest]DataSourceRequest request, TrainingApplicant trainingApplicant)
        {
            string messages = "";
            ModelState.Clear();

            var plans =
                   db.TrainingPlans.FirstOrDefault(x => x.Course == trainingApplicant.LookupId && x.WorkUnit == trainingApplicant.OrgStructureId);
            if (plans != null)
            {
                var employee = db.TrainingApplicants.FirstOrDefault(x => x.TrainingApplicantId == trainingApplicant.TrainingApplicantId);

                if (trainingApplicant.NoofTrainee > plans.NoofTrainee)
                {
                    // messages = "The Request is denied B/c the requested number of trainee is greater that the plan!!";

                    //ModelState.AddModelError("Training Request", messages);
                    //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    ModelState.AddModelError("", "Maximum No of trainee are registered so cannot register more");
                }

            }
            if(plans==null)
            {
                messages = "Please set the plan first!!";
                ModelState.AddModelError("Training Request", messages);
                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
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
               
                    var entity = new TrainingApplicant
                    {
                        OrgStructureId = trainingApplicant.OrgStructureId,
                        LookupId = trainingApplicant.LookupId,
                        Remark = trainingApplicant.Remark,
                        NoofTrainee = trainingApplicant.NoofTrainee,
                        Date = Date,
                        DateAmharic = DateAmharic,

                    };

                    db.TrainingApplicants.Add(entity);
                    db.SaveChanges();
                    trainingApplicant.TrainingApplicantId = entity.TrainingApplicantId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "TrainingApplicant ", "Success", User.Identity.Name, User.Identity.Name + " add new Training Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

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
                            Title = "Training Request",
                            Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has requested Training for { deptName }"
                        };

                        Notify.NotifyUser(notif);
                    }
                }



                #endregion

            }

            return Json(new[] { trainingApplicant }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicants_Update([DataSourceRequest]DataSourceRequest request, TrainingApplicant trainingApplicant)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = trainingApplicant.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new TrainingApplicant
                    {
                        TrainingApplicantId = trainingApplicant.TrainingApplicantId,
                        OrgStructureId = trainingApplicant.OrgStructureId,
                        LookupId = trainingApplicant.LookupId,
                        Remark = trainingApplicant.Remark,
                        NoofTrainee = trainingApplicant.NoofTrainee,
                        Date = Date,
                        DateAmharic = DateAmharic,

                    };

                    db.TrainingApplicants.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "TrainingApplicant ", "Success", User.Identity.Name, User.Identity.Name + " update Training Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { trainingApplicant }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicants_Destroy([DataSourceRequest]DataSourceRequest request, TrainingApplicant trainingApplicant)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new TrainingApplicant
                {
                    TrainingApplicantId = trainingApplicant.TrainingApplicantId,
                    OrgStructureId = trainingApplicant.OrgStructureId,
                    LookupId = trainingApplicant.LookupId,
                    Remark = trainingApplicant.Remark,
                    NoofTrainee = trainingApplicant.NoofTrainee,
                    Date = trainingApplicant.Date,

                    DateAmharic = trainingApplicant.DateAmharic,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training applicant information of work unit " + trainingApplicant.OrgStructureId);
                #endregion
                db.TrainingApplicants.Attach(entity);
                db.TrainingApplicants.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingApplicant }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
