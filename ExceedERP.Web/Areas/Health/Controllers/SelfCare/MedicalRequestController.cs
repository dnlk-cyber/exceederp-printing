using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Core.Domain.Health;

namespace ExceedERP.Web.Areas.Health.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService,  HrmConstants.HRDepartmentHead)]
    public class MedicalRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
        public static List<string> reportsToEmployee = new List<string>();
        public ActionResult Index()
        { 
            PopulateViewData();
            GetDepartments();
            GetStatusType();

            return View();
        }
        private void PopulateViewData()
        {
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            reportsToEmployee = list.Select(x => x.PersonId.ToString()).ToList();
            reportsToEmployee.Add(EmployeeId.ToString());
            var employeesName = db.Person_Employee.ToList();
            List<Person_Employee> person = new List<Person_Employee>();
        
            ViewData["EmployeeName"] = employeesName;
            ViewData["Department"] = db.MainCompanyStructures.ToList();
        }
        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            db = new ExceedDbContext();
            var employees = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

            return Json(employees.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Employee_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.EmployeeCode == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
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
            ViewData["OrgStructureId"] = allDepartment;
        }
        public JsonResult GetEmployeesList(int OrgStructureId)
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var palcment = db.Placements.Where(x => x.Department == OrgStructureId).ToList();
            if (palcment.Any())
            {
                foreach (var p in palcment)
                {
                    var employeeList = db.Person_Employee.Where(x => x.EmployeeId == p.EmployeeId == x.IsActive).AsQueryable();




                    foreach (var emp in employeeList)
                    {
                        EmployeeIdView = new EmployeeIdView();
                        EmployeeIdView.PersonId = emp.EmployeeId;
                        EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                        EmployeeIdViews.Add(EmployeeIdView);
                    }
                }
            }
            ViewData["EmployeeList"] = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return Json(EmployeeIdViews.Select(p => new { PersonId = p.PersonId, FullName = p.FullName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MedicalRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeid);
            IQueryable<MedicalRequest> medicalrequest = db.MedicalRequest.Where(x => employeeids.Contains(x.EmployeeId)).OrderBy(x => x.Date);
            DataSourceResult result = medicalrequest.ToDataSourceResult(request, medicalRequest => new
            {
                MedicalRequestId = medicalRequest.MedicalRequestId,
                EmployeeId = medicalRequest.EmployeeId,
                Date = medicalRequest.Date,
                OrgStructureId = medicalRequest.OrgStructureId,
                Remark = medicalRequest.Remark,
                EmployeeCode = medicalRequest.EmployeeCode,
                DateAmharic = medicalRequest.DateAmharic,
                IsForOther = medicalRequest.IsForOther,
                DepartmentSendForApprovalBy = medicalRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = medicalRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = medicalRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = medicalRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = medicalRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = medicalRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = medicalRequest.OnlineApprovedBy,
                OnlineApprovedTime = medicalRequest.OnlineApprovedTime,
                IsOnlineApproved = medicalRequest.IsOnlineApproved,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalRequest_Create([DataSourceRequest]DataSourceRequest request, MedicalRequest medicalRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!medicalRequest.IsForOther)
                {
                    EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                }
                else
                {
                    EmployeeId = medicalRequest.EmployeeId;
                }
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date value and is mandatory!!");
                }
                else if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }

                else
                {
                    var entity = new MedicalRequest
                    {
                        EmployeeId = EmployeeId,
                        Date = Date,
                        OrgStructureId = medicalRequest.OrgStructureId,
                        Remark = medicalRequest.Remark,
                        EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == EmployeeId)?.EmployeeCode,
                        DateAmharic = DateAmharic,
                        IsForOther = medicalRequest.IsForOther
                    };

                    db.MedicalRequest.Add(entity);
                    db.SaveChanges();
                    medicalRequest.MedicalRequestId = entity.MedicalRequestId;

                    //#region notification


                    //var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    //var superviorId = EmployeeHelper.GetSupervisor(empid);

                    //NotifModels notif = new NotifModels
                    //{

                    //    UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                    //    Title = "Medical Request",
                    //    Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Medical",
                    //    URL = "~/HRM/MedicalRequest"
                    //};

                    //Notify.NotifyUser(notif);

                    //#endregion
                }
            }
            IsForOther = false;
            return Json(new[] { medicalRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalRequest_Update([DataSourceRequest]DataSourceRequest request, MedicalRequest medicalRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = medicalRequest.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }

                else
                {
                    var entity = new MedicalRequest
                    {
                        MedicalRequestId = medicalRequest.MedicalRequestId,
                        EmployeeId = medicalRequest.EmployeeId,
                        Date = Date,
                        OrgStructureId = medicalRequest.OrgStructureId,
                        Remark = medicalRequest.Remark,
                        DateAmharic = DateAmharic,
                        IsForOther = medicalRequest.IsForOther,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == medicalRequest.EmployeeId).FirstOrDefault().EmployeeCode,
                    };

                    db.MedicalRequest.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            IsForOther = false;

            return Json(new[] { medicalRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalRequest_Destroy([DataSourceRequest]DataSourceRequest request, MedicalRequest medicalRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new MedicalRequest
                {
                    MedicalRequestId = medicalRequest.MedicalRequestId,
                    EmployeeId = medicalRequest.EmployeeId,
                    Date = medicalRequest.Date,
                    OrgStructureId = medicalRequest.OrgStructureId,
                    Remark = medicalRequest.Remark,
                    DateAmharic = medicalRequest.DateAmharic,
                    EmployeeCode = medicalRequest.EmployeeCode,
                    IsForOther = medicalRequest.IsForOther
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted medical request of employee code  " + medicalRequest.EmployeeCode);
                #endregion

                db.MedicalRequest.Attach(entity);
                db.MedicalRequest.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { medicalRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
