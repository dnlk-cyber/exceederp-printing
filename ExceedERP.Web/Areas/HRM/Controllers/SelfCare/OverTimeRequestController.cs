using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
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
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class OverTimeRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
        public static List<string> reportsToEmployee = new List<string>();
        public static string FromDateAm { get; set; }
        public static DateTime FromDate { get; set; }
        public static string ToDateAm { get; set; }
        public static DateTime ToDate { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

            reportsToEmployee = list.Select(x => x.PersonId.ToString()).ToList();
            reportsToEmployee.Add(EmployeeId.ToString());
            var structres = db.MainCompanyStructures;
            ViewData["organization"] = structres;
            var employees = db.Person_Employee.ToList();
            
            ViewData["EmployeeList"] = employees.Select(x => new { x.EmployeeId, x.FirstName, x.MiddleName, x.LastName, x.FirstNameEnglish, x.MiddleNameEnglish, x.LastNameEnglish, x.FullNameAmharic, x.FullNameaEnglish });
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromDateAm")
            {
                FromDateAm = value;
                var splitDate = FromDateAm.Split('/');
                FromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            if (name == "ToDateAm")
            {
                ToDateAm = value;
                var splitDate = ToDateAm.Split('/');
                ToDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult SendForApproval()
        {
            string messages = "";
            var checkList = db.OverTimeRequestEmployee.Where(x => x.OverTimeRequestId == HRMInputs.RequestId).ToList();
            if (!checkList.Any())
            {
                messages = "You should to add one or more employee!!";
                // return  ModelState.AddModelError("xx", messages );
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                // ModelState.AddModelError("", "You should to add one or more employee!!");
            }

            var status =
                  db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();

            if (status.Any())
            {
                foreach (var s in status)
                {
                    if (s.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        messages = "The Request is already approved!!";
                        //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        ModelState.AddModelError("xx", messages);
                        return null;
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        // return  ModelState.AddModelError("xx", messages );
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

        public ActionResult DepartmentForApproval()
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
                        //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        ModelState.AddModelError("xx", messages);
                        return null;
                    }
                    if (s.Status == RequestStatus.DEPARTMENT_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        // return  ModelState.AddModelError("xx", messages );
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
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.DEPARTMENT_APPROVAL,
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
                    Status = RequestStatus.DEPARTMENT_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "OverTime").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "OverTime" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "OverTime" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        public ActionResult OverTimeRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            int orgId = 0;
            int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if(placement != null)
            {
                orgId = placement.OrgStructureId;
            }

            IQueryable<OverTimeRequest> overtimerequest = db.OverTimeRequest.Where(x => x.OrgStructureId == orgId);

            DataSourceResult result = overtimerequest.ToDataSourceResult(request, overTimeRequest => new
            {
                OverTimeRequestId = overTimeRequest.OverTimeRequestId,
                FromDate = overTimeRequest.FromDate,
                ToDate = overTimeRequest.ToDate,
                FromTime = overTimeRequest.FromTime,
                ToTime = overTimeRequest.ToTime,
                OrgStructureId = overTimeRequest.OrgStructureId,
                Description = overTimeRequest.Description,
                FromDateAm = overTimeRequest.FromDateAm,
                ToDateAm = overTimeRequest.ToDateAm,
                Type = overTimeRequest.Type
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequest_Create([DataSourceRequest]DataSourceRequest request, OverTimeRequest overTimeRequest)
        {
            ModelState.Clear();
            if (overTimeRequest.ToTime < overTimeRequest.FromTime)
            {
                ModelState.AddModelError("", "Starting time cann't exceed End time");
            }

            if (ModelState.IsValid)
            {

                var splitDateFrom = overTimeRequest.FromDateAm.Split('/');
                FromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]), Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));

                var splitDateTo = overTimeRequest.ToDateAm.Split('/');
                ToDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));



                if (overTimeRequest.FromDate.Date > overTimeRequest.ToDate.Date)
                {
                    ModelState.AddModelError("", "Please check the date range!!");
                }


                else
                {
                    var entity = new OverTimeRequest
                    {
                        FromDate = FromDate,//overTimeRequest.FromDate,
                        ToDate = ToDate,//overTimeRequest.ToDate,
                        FromTime = overTimeRequest.FromTime,
                        ToTime = overTimeRequest.ToTime,
                        OrgStructureId = overTimeRequest.OrgStructureId,
                        Description = overTimeRequest.Description,
                        FromDateAm = overTimeRequest.FromDateAm,
                        ToDateAm = overTimeRequest.ToDateAm,
                        Type = overTimeRequest.Type
                    };

                    db.OverTimeRequest.Add(entity);
                    db.SaveChanges();
                    overTimeRequest.OverTimeRequestId = entity.OverTimeRequestId;

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "OverTimeRequest ", "Success", User.Identity.Name, User.Identity.Name + " add new Overtime Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                    #endregion

                    #region notification


                    var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var superviorId = EmployeeHelper.GetSupervisor(empid);

                    NotifModels notif = new NotifModels
                    {

                        UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                        Title = "Medical Request",
                        Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Medical",
                        URL = "~/HRM/OverTimeRequest"
                    };

                    Notify.NotifyUser(notif);

                    #endregion
                }
            }

            return Json(new[] { overTimeRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequest_Update([DataSourceRequest]DataSourceRequest request, OverTimeRequest overTimeRequest)
        {
            ModelState.Clear();
            if (FromDateAm == null)
            {
                FromDateAm = overTimeRequest.FromDateAm;
                var splitDate = FromDateAm.Split('/');
                FromDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (ToDateAm == null)
            {
                ToDateAm = overTimeRequest.ToDateAm;
                var splitDate = FromDateAm.Split('/');
                ToDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (ModelState.IsValid)
            {
                if (overTimeRequest.FromDate.Date > overTimeRequest.ToDate.Date)
                {
                    ModelState.AddModelError("", "Please check the date range!!");
                }
                else
                {

                    var entity = new OverTimeRequest
                    {
                        OverTimeRequestId = overTimeRequest.OverTimeRequestId,
                        FromDate = FromDate,
                        ToDate = ToDate,
                        FromTime = overTimeRequest.FromTime,
                        ToTime = overTimeRequest.ToTime,
                        OrgStructureId = overTimeRequest.OrgStructureId,
                        Description = overTimeRequest.Description,
                        FromDateAm = FromDateAm,
                        ToDateAm = ToDateAm,
                        Type = overTimeRequest.Type
                    };

                    db.OverTimeRequest.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "OverTimeRequest ", "Success", User.Identity.Name, User.Identity.Name + " update Overtime Request at date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { overTimeRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverTimeRequest_Destroy([DataSourceRequest]DataSourceRequest request, OverTimeRequest overTimeRequest)
        {
            ModelState.Clear();

            if (FromDateAm == null)
            {
                ModelState.AddModelError("", "Fom date value  is mandatory!!");
            }
            if (ToDateAm == null)
            {
                ModelState.AddModelError("", "To date value  is mandatory!!");
            }
            if (ModelState.IsValid)
            {
                var entity = new OverTimeRequest
                {
                    OverTimeRequestId = overTimeRequest.OverTimeRequestId,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    FromTime = overTimeRequest.FromTime,
                    ToTime = overTimeRequest.ToTime,
                    OrgStructureId = overTimeRequest.OrgStructureId,
                    Description = overTimeRequest.Description,
                    FromDateAm = FromDateAm,
                    ToDateAm = ToDateAm,
                    Type = overTimeRequest.Type
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted over time request of work unit" + overTimeRequest.OrgStructureId);
                #endregion
                db.OverTimeRequest.Attach(entity);
                db.OverTimeRequest.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { overTimeRequest }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult OverTimeRequestApproval_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<OverTimeRequest> overtimerequests = db.OverTimeRequest;

            List<OverTimeRequest> overTime = new List<OverTimeRequest>();
            foreach (var m in overtimerequests)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.DEPARTMENT_APPROVAL && x.RequestId == m.OverTimeRequestId && x.ReferenceType == "OverTime").FirstOrDefault();
                if (status != null)
                {
                    overTime.Add(m);
                }

            }



            DataSourceResult result = overTime.ToDataSourceResult(request, overTimeRequest => new
            {
                OverTimeRequestId = overTimeRequest.OverTimeRequestId,
                FromDate = overTimeRequest.FromDate,
                ToDate = overTimeRequest.ToDate,
                FromTime = overTimeRequest.FromTime,
                ToTime = overTimeRequest.ToTime,
                OrgStructureId = overTimeRequest.OrgStructureId,
                Description = overTimeRequest.Description,
                FromDateAm = overTimeRequest.FromDateAm,
                ToDateAm = overTimeRequest.ToDateAm,
                Type = overTimeRequest.Type
            });

            return Json(result);
        }
        public JsonResult GetEmployeesUnderLogedInUserDepartment()
        {
            int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var empIds = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(empId);
            var empIds = EmployeeHelper.GetSubordinates_FilteredByHead(empId);

            var employees = db.Person_Employee.Where(x => empIds.Contains(x.EmployeeId) && x.IsActive).ToList();

            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
