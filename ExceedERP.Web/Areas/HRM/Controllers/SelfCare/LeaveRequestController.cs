﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.Common.Controllers;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
 using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Global;
using System.IO;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class LeaveRequestController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        static ApplicationDbContext dbApplication = new ApplicationDbContext();

        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        public static string FromAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
        public static decimal Total = 0;
        public static List<string> reportsToEmployee = new List<string>();
        public ActionResult Index()
        {

            GetStatusType();
            GetPeriod();
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            reportsToEmployee = list.Select(x => x.PersonId.ToString()).ToList();
            reportsToEmployee.Add(EmployeeId.ToString());

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            ViewData["UserName"] = dbApplication.Users.ToList();
            ViewData["Department"] = db.MainCompanyStructures.Where(x=>x.IsActive).ToList();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits;
            ViewData["businessUnits"] = businessUnits;
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        public int SetEmployee(string x)
        {
            IsForOther = false;

            if (x == "true")
            {
                  var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                  //var list = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeId).ToList();
                  var list = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId).ToList();
               
                IsForOther = true;
                return list.Count();

            }
            return 0;
        }
        private JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json((periodList ?? throw new InvalidOperationException()).Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
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
                        //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                        ModelState.AddModelError("xx", messages);
                        return null;
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        // return  ModelState.AddModelError("xx", messages );
                        return null;//new JavaScriptResult() { Script = "alert('" + messages + "');" };
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
        public bool CheckStatusForBackColor(int leaveId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == leaveId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Leave").FirstOrDefault();
            if (reques != null)
            {
                return true;
            }
            return false;
        }
        public bool ChangeInDepartmentForBackColor(int leaveId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == leaveId && x.Status != RequestStatus.APPROVE && (x.Status == RequestStatus.DEPARTMENT_APPROVAL || x.Status == RequestStatus.PEND || x.Status == RequestStatus.REJECT) && x.ReferenceType == "Leave").FirstOrDefault();
            if (reques != null)
            {
                return true;
            }
            return false;
        }
        public string GetEmployeeCode(int id)
        {
            string code = "";
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                code = person.EmployeeCode;
            }
            return code;
        }
        public ActionResult LeaveRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            var  employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByHead(employeeid);
            //employeeids.Remove(employeeid);
            //List<LeaveRequest> leaveRequestLists = new List<LeaveRequest>();
            List<LeaveRequest> leaverequests = db.LeaveRequest.Where(x => employeeids.Contains(x.EmployeeId) /*&& x.IsForOther*/).OrderBy(x => x.RequestDate).ToList();
           // List<LeaveRequest> leaverequestSelf = db.LeaveRequest.Where(x => employeeids.Contains(x.EmployeeId) && !x.IsForOther).OrderBy(x => x.RequestDate).ToList();
            //foreach (var lrs in leaverequestSelf)
            //{
            //    var sentRequest = db.RequestesStatus.FirstOrDefault(x => x.RequestId == lrs.LeaveRequestId);
            //    if (sentRequest != null)
            //    {
            //        leaverequests.Add(lrs);
            //    }
            //}

            DataSourceResult result = leaverequests.ToDataSourceResult(request, leaveRequest => new
            {
                LeaveRequestId = leaveRequest.LeaveRequestId,
                EmployeeId = leaveRequest.EmployeeId,
                BranchId = leaveRequest.BranchId,
                BusinessUnitId = leaveRequest.BusinessUnitId,
                From = leaveRequest.From,
                IsForOther = leaveRequest.IsForOther,
                To = leaveRequest.To,
                LeaveDays = leaveRequest.LeaveDays,
                Type = leaveRequest.Type,
                Reason = leaveRequest.Reason,
                GlFiscalYearId = leaveRequest.GlFiscalYearId,
                RequestDate = leaveRequest.RequestDate,
                Remark = leaveRequest.Remark,
                LeaveBalance = leaveRequest.LeaveBalance,
                FromAmharic = leaveRequest.FromAmharic,
                ToAmharic = leaveRequest.ToAmharic,
                Day = leaveRequest.Day,
                MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                Hours = leaveRequest.Hours,
                Status = leaveRequest.Status,
                EmployeeCode = GetEmployeeCode(leaveRequest.EmployeeId),
                IsApproved = CheckStatusForBackColor(leaveRequest.LeaveRequestId),
                IsChangeInDepartment = ChangeInDepartmentForBackColor(leaveRequest.LeaveRequestId),
                DepartmentSendForApprovalBy = leaveRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leaveRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leaveRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = leaveRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leaveRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leaveRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = leaveRequest.OnlineApprovedBy,
                OnlineApprovedTime = leaveRequest.OnlineApprovedTime,
                IsOnlineApproved = leaveRequest.IsOnlineApproved,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Create([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
            decimal entitleBal = 0;

            #region Pre Conditions for validation

            if (string.IsNullOrEmpty(leaveRequest.FromAmharic) || string.IsNullOrEmpty(leaveRequest.ToAmharic))
            {
                ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            leaveRequest.From = ParseEthiopicDate(leaveRequest.FromAmharic);
            leaveRequest.To = ParseEthiopicDate(leaveRequest.ToAmharic);

            if (leaveRequest.From > leaveRequest.To)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            //if (leaveRequest.From < DateTime.Now.Date)
            //{
            //    ModelState.AddModelError("", "From date must not be a past date!!");
            //    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            //}

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                return new JavaScriptResult { Script = "alert('Check your Period if it is defined');" };
            }

            leaveRequest.LeaveDays = GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type);

            leaveRequest.EmployeeId = leaveRequest.IsForOther ? leaveRequest.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            #endregion

            #region Validations

            entitleBal = db.LeavesEntitle.Where(x => x.EmployeeId == leaveRequest.EmployeeId && !x.IsExpired).Sum(x => x.Balance);

            if (leaveRequest.LeaveDays > entitleBal && leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You requested more than your balance");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            if (db.LeaveRequest.Any(x => x.EmployeeId == leaveRequest.EmployeeId && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To))))
            {
                ModelState.AddModelError("", "Your request overlaps with another request");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.IsActive);
            if (employee != null)
            {
                if (leaveRequest.Type == LeaveType.MATERNITY && employee.Gender == Gender.MALE)
                {
                    ModelState.AddModelError("", "Males can't take Maternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                if (leaveRequest.Type == LeaveType.PATERNITY && employee.Gender == Gender.FEMALE)
                {
                    ModelState.AddModelError("", "Females can't take Paternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                if (leaveRequest.Type == LeaveType.MATERNITY)
                {
                    var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.Maternitiy.ToString());
                    if (config != null)
                    {
                        var allowedDays = Convert.ToDecimal(config.CurrentValue);
                        if (leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", "You are requesting maternity leave above the setted Maximum value");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }

                        var maternityRequests = db.LeaveRequest.Where(x => x.Type == LeaveType.MATERNITY && x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId).ToList();
                        if (maternityRequests.Any() && maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", $"You can't take maternity leave more than {allowedDays} days in one year, check your previous maternity leave requests");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }

            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced)
            {
                if (leaveRequest.Type != LeaveType.ANNUAL && leaveRequest.Type != LeaveType.OTHER && leaveRequest.Type != LeaveType.Leave_With_Out_Pay)
                {
                    ModelState.AddModelError("", $"You can't take Forced Leave for {leaveRequest.Type}, it's allowed only for Annual, Leave without pay, and Other leave types");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                var leavePlan = db.LeavePlans.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (leavePlan != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    if (leaveRequest.From < leavePlan.From || leaveRequest.To > leavePlan.To || leaveRequest.LeaveDays > leavePlan.LeaveAmount)
                    {
                        ModelState.AddModelError("", "The requested leave amount or dates are outside of your plan");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    ModelState.AddModelError("", "You should set a leave plan for this period!!");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var existingLeave = db.Leaves.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (existingLeave != null)
            {
                if ((leaveRequest.From >= existingLeave.From && leaveRequest.From <= existingLeave.To) || (leaveRequest.To >= existingLeave.From && leaveRequest.To <= existingLeave.To))
                {
                    ModelState.AddModelError("", "Selected date range overlaps with other leave taken date. Please select another date range");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            var entitleCurrentYear = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (entitleCurrentYear != null && leaveRequest.Type == LeaveType.ANNUAL)
            {
                var totalEntitle = entitleBal;
                var exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalEntitle);
                var currentYearDeserve = GetCurrentYearDeserve(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount);

                if (exactDeserve == 0 || leaveRequest.LeaveDays > exactDeserve)
                {
                    ModelState.AddModelError("", "You can't take this amount of leave in this period");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId && x.Type == LeaveType.ANNUAL && !x.IsOnlineApproved);
                if (leaveRequests.Any())
                {
                    var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
                    var totalNeeded = leaveRequestedDays + leaveRequest.LeaveDays;
                    if (totalNeeded > exactDeserve)
                    {
                        ModelState.AddModelError("", "You have other waiting requests so the sum of days exceeds your deserved amount");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You have no entitlement for this budget year");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent == true).FirstOrDefault();

            if (placement != null)
            {
                leaveRequest.BranchId = placement.BranchId;
                leaveRequest.BusinessUnitId = placement.BusinessUnitId;
            }
            else
            {
                ModelState.AddModelError("placement", "Employee does not have a current placement");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            #endregion

            if (ModelState.IsValid)
            {
                var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId).Sum(x => x.Balance);
                leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;

                var entity = new LeaveRequest
                    {
                        EmployeeId = EmployeeId,
                        From = leaveRequest.From,
                        To = leaveRequest.To,
                        LeaveDays = leaveRequest.LeaveDays,
                        Type = leaveRequest.Type,
                        Reason = leaveRequest.Reason,
                        Remark = leaveRequest.Remark,
                        GlFiscalYearId = leaveRequest.GlFiscalYearId,
                        RequestDate = DateTime.Now,
                        LeaveBalance = totalBalance,
                        FromAmharic = leaveRequest.FromAmharic,
                        ToAmharic = leaveRequest.ToAmharic,
                        Day = leaveRequest.Day,
                        MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                        Hours = leaveRequest.Hours,
                        IsForOther = leaveRequest.IsForOther,
                        BranchId = leaveRequest.BranchId,
                        BusinessUnitId = leaveRequest.BusinessUnitId,
                        EmployeeCode = inputs.GetEmployeeCode(EmployeeId)
                    };

                    db.LeaveRequest.Add(entity);
                    db.SaveChanges();
                    leaveRequest.LeaveRequestId = entity.LeaveRequestId;
                    IsForOther = false;

            }
            

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeaveRequest_Create([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        var period = db.GLFiscalYears.FirstOrDefault(x => x.Status == FiscalYearStatus.Open);
        //        leaveRequest.LeaveDays = Convert.ToDecimal((To.Date - From.Date).TotalDays);
        //        if (!IsForOther)
        //        {
        //            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //        }
        //        else
        //        {
        //            EmployeeId = leaveRequest.EmployeeId;
        //        }

        //        if (leaveRequest.Type == LeaveType.ANNUAL)
        //        {
        //            string messages = "";
        //            if (EmployeeId == 0)
        //            {
        //                ModelState.AddModelError("", "Please select employee First!!");
        //                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }


        //            var leavePlans =
        //                db.LeavePlans
        //                    .FirstOrDefault(x => x.EmployeeId == EmployeeId && x.GlFiscalYearId == period.GlFiscalYearId);
        //            if (leavePlans != null)
        //            {
        //                if (From.Date >= leavePlans.From.Date && To.Date <= leavePlans.To.Date)
        //                {
        //                    leaveRequest.LeaveDays = Convert.ToDecimal((To.Date - From.Date).TotalDays);
        //                    if (leaveRequest.LeaveDays > leavePlans.LeaveAmount)
        //                    {
        //                       ModelState.AddModelError("", "The requested amount is greater than your plan!!");
        //                        //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //                    }

        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("", "Please check selected period leave plan date range!!");
        //                    //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //                }

        //                //RedirectToAction("Index");

        //            }
        //            else
        //            {
        //                //RedirectToAction("Index");
        //               ModelState.AddModelError("", "You should set leave plan for this period!!");
        //                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }

        //        }
        //        if (ModelState.IsValid)
        //        {
        //            if (From.Date > To.Date)
        //            {
        //                ModelState.AddModelError("", "Please Check the date range!!");
        //            }
        //            else if (FromAmharic == null || ToAmharic == null)
        //            {
        //                ModelState.AddModelError("", "Form and To date value and is mandatory!!");
        //            }


        //            else if (EmployeeId == 0)
        //            {
        //                ModelState.AddModelError("", "Please select employee First!!");
        //                // return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }
        //            else if (leaveRequest.LeaveDays == 0)
        //            {
        //                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
        //                // return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }
        //            if (leaveRequest.EmployeeId != 0)
        //            {
        //                var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
        //                if (placemnt != null)
        //                {
        //                    leaveRequest.BranchId = placemnt.BranchId;
        //                    leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("placement", "Employee has not current placemnt");
        //                }
        //            }
        //            if (ModelState.IsValid)
        //            {
                       

        //                var entity = new LeaveRequest
        //                {
        //                    EmployeeId = EmployeeId,
        //                    From = From,
        //                    To = To,
        //                    LeaveDays = leaveRequest.LeaveDays,
        //                    Type = leaveRequest.Type,
        //                    Reason = leaveRequest.Reason,
        //                    Remark = leaveRequest.Remark,
        //                    GlFiscalYearId = period.GlFiscalYearId,//leaveRequest.GlFiscalYearId,
        //                    RequestDate = DateTime.Now,
        //                    LeaveBalance = Total,
        //                    FromAmharic = FromAmharic,
        //                    ToAmharic = ToAmharic,
        //                    Day = leaveRequest.Day,
        //                    MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
        //                    Hours = leaveRequest.Hours,
        //                    IsForOther = leaveRequest.IsForOther,
        //                    BranchId =leaveRequest.BranchId,
        //                    BusinessUnitId = leaveRequest.BusinessUnitId,
        //                    EmployeeCode =
        //                    inputs.GetEmployeeCode(EmployeeId)
        //                };

        //                db.LeaveRequest.Add(entity);
        //                db.SaveChanges();
        //                leaveRequest.LeaveRequestId = entity.LeaveRequestId;
        //                IsForOther = false;
        //                //#region notification


        //                //var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //                //var superviorId = EmployeeHelper.GetSupervisor(empid);

        //                //NotifModels notif = new NotifModels
        //                //{

        //                //    UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
        //                //    Title = "Leave Request",
        //                //    Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Leave",
        //                //    URL = "~/HRM/LeaveRequest"
        //                //};

        //                //Notify.NotifyUser(notif);

        //                //#endregion
        //            }
        //        }
        //    }
         
        //    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Update([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
            decimal entitleBal = 0;

            #region Pre Conditions for validation

            if (string.IsNullOrEmpty(leaveRequest.FromAmharic) || string.IsNullOrEmpty(leaveRequest.ToAmharic))
            {
                ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            leaveRequest.From = ParseEthiopicDate(leaveRequest.FromAmharic);
            leaveRequest.To = ParseEthiopicDate(leaveRequest.ToAmharic);

            if (leaveRequest.From > leaveRequest.To)
            {
                ModelState.AddModelError("", "From Date field should be less or equal to To date field!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            //if (leaveRequest.From < DateTime.Now.Date)
            //{
            //    ModelState.AddModelError("", "From date must not be a past date!!");
            //    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            //}

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
            {
                return new JavaScriptResult { Script = "alert('Check your Period if it is defined');" };
            }

            leaveRequest.LeaveDays = GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYear.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type);

            leaveRequest.EmployeeId = leaveRequest.IsForOther ? leaveRequest.EmployeeId : UserInformationHelper.GetEmployeeId(User.Identity.Name);

            if (leaveRequest.EmployeeId == 0)
            {
                ModelState.AddModelError("", "Please select an employee first!! Or refresh the page to clear the cache");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            #endregion

            #region Validations

            entitleBal = db.LeavesEntitle.Where(x => x.EmployeeId == leaveRequest.EmployeeId && !x.IsExpired).Sum(x => x.Balance);

            if (leaveRequest.LeaveDays > entitleBal && leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You requested more than your balance");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            if (db.LeaveRequest.Any(x => x.EmployeeId == leaveRequest.EmployeeId && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To))))
            {
                ModelState.AddModelError("", "Your request overlaps with another request");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.IsActive);
            if (employee != null)
            {
                if (leaveRequest.Type == LeaveType.MATERNITY && employee.Gender == Gender.MALE)
                {
                    ModelState.AddModelError("", "Males can't take Maternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                if (leaveRequest.Type == LeaveType.PATERNITY && employee.Gender == Gender.FEMALE)
                {
                    ModelState.AddModelError("", "Females can't take Paternity Leave");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                if (leaveRequest.Type == LeaveType.MATERNITY)
                {
                    var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.Maternitiy.ToString());
                    if (config != null)
                    {
                        var allowedDays = Convert.ToDecimal(config.CurrentValue);
                        if (leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", "You are requesting maternity leave above the setted Maximum value");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }

                        var maternityRequests = db.LeaveRequest.Where(x => x.Type == LeaveType.MATERNITY && x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId).ToList();
                        if (maternityRequests.Any() && maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays > allowedDays)
                        {
                            ModelState.AddModelError("", $"You can't take maternity leave more than {allowedDays} days in one year, check your previous maternity leave requests");
                            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                        }
                    }
                }
            }

            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced)
            {
                if (leaveRequest.Type != LeaveType.ANNUAL && leaveRequest.Type != LeaveType.OTHER && leaveRequest.Type != LeaveType.Leave_With_Out_Pay)
                {
                    ModelState.AddModelError("", $"You can't take Forced Leave for {leaveRequest.Type}, it's allowed only for Annual, Leave without pay, and Other leave types");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                var leavePlan = db.LeavePlans.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (leavePlan != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    if (leaveRequest.From < leavePlan.From || leaveRequest.To > leavePlan.To || leaveRequest.LeaveDays > leavePlan.LeaveAmount)
                    {
                        ModelState.AddModelError("", "The requested leave amount or dates are outside of your plan");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
                else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
                {
                    ModelState.AddModelError("", "You should set a leave plan for this period!!");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            if (leaveRequest.LeaveDays == 0)
            {
                ModelState.AddModelError("", "Leave Days must be greater than 0!!");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }

            var existingLeave = db.Leaves.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (existingLeave != null)
            {
                if ((leaveRequest.From >= existingLeave.From && leaveRequest.From <= existingLeave.To) || (leaveRequest.To >= existingLeave.From && leaveRequest.To <= existingLeave.To))
                {
                    ModelState.AddModelError("", "Selected date range overlaps with other leave taken date. Please select another date range");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }
            }

            var entitleCurrentYear = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
            if (entitleCurrentYear != null && leaveRequest.Type == LeaveType.ANNUAL)
            {
                var totalEntitle = entitleBal;
                var exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalEntitle);
                var currentYearDeserve = GetCurrentYearDeserve(fiscalYear.DateFrom, leaveRequest.To, entitleCurrentYear.Amount);

                if (exactDeserve == 0 || leaveRequest.LeaveDays > exactDeserve)
                {
                    ModelState.AddModelError("", "You can't take this amount of leave in this period");
                    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                }

                var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == fiscalYear.GlFiscalYearId && x.Type == LeaveType.ANNUAL && !x.IsOnlineApproved);
                if (leaveRequests.Any())
                {
                    var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
                    var totalNeeded = leaveRequestedDays + leaveRequest.LeaveDays;
                    if (totalNeeded > exactDeserve)
                    {
                        ModelState.AddModelError("", "You have other waiting requests so the sum of days exceeds your deserved amount");
                        return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                ModelState.AddModelError("", "You have no entitlement for this budget year");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent == true).FirstOrDefault();

            if (placement != null)
            {
                leaveRequest.BranchId = placement.BranchId;
                leaveRequest.BusinessUnitId = placement.BusinessUnitId;
            }
            else
            {
                ModelState.AddModelError("placement", "Employee does not have a current placement");
                return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));

            }
            #endregion
            if (ModelState.IsValid)
            {
                var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId).Sum(x => x.Balance);
                leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;


                var entity = new LeaveRequest
                {
                    LeaveRequestId = leaveRequest.LeaveRequestId,
                    EmployeeId = leaveRequest.EmployeeId,
                    BranchId = leaveRequest.BranchId,
                    BusinessUnitId = leaveRequest.BusinessUnitId,
                    From = leaveRequest.From,
                    To = leaveRequest.To,
                    LeaveDays = leaveRequest.LeaveDays,
                    Type = leaveRequest.Type,
                    Reason = leaveRequest.Reason,
                    GlFiscalYearId = leaveRequest.GlFiscalYearId,
                    Remark = leaveRequest.Remark,
                    RequestDate = DateTime.Now,
                    LeaveBalance = totalBalance,
                    Day = leaveRequest.Day,
                    MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                    Hours = leaveRequest.Hours,
                    FromAmharic = leaveRequest.FromAmharic,
                    ToAmharic = leaveRequest.ToAmharic,
                    IsForOther = leaveRequest.IsForOther,
                    LeaveActionTypes = leaveRequest.LeaveActionTypes,
                    EmployeeCode = inputs.GetEmployeeCode(leaveRequest.EmployeeId),
                    RequesterId = UserInformationHelper.GetEmployeeId(User.Identity.Name)
                };


                db.LeaveRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                IsForOther = false;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequestPersonal ", "Success", User.Identity.Name, User.Identity.Name + " update leave request of id" + entity.LeaveRequestId, Modules.HRM);

                #endregion
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LeaveRequest_Update([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        //{
        //    int count = 0;
        //    double weekDays = 0;
        //    int holidays = 0;
        //    ModelState.Clear();
        //    if (leaveRequest.IsOnlineApproved)
        //    {
        //        ModelState.AddModelError("", "you can't modify approved requestes");
        //    }


        //    #region Pre Requestes 
        //    if (leaveRequest.FromAmharic != null)
        //    {
        //        var splitDate = leaveRequest.FromAmharic.Split('/');
        //        leaveRequest.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "From Date is mandatory");
        //    }
        //    if (leaveRequest.ToAmharic != null)
        //    {
        //        var splitDate = leaveRequest.ToAmharic.Split('/');
        //        leaveRequest.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "To Date is mandatory");
        //    }

        //    var per = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From.Date && x.DateTo >= leaveRequest.From.Date);
        //    if (per == null)
        //    {
        //        var messages = "Check your Period if it defined";
        //        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //    }

        //    leaveRequest.LeaveDays = GetLeaveDays(leaveRequest.From, leaveRequest.To, per.GlFiscalYearId, leaveRequest.Day, leaveRequest.Type);

        //    #endregion

        //    decimal entitleBal = 0;
        //    var entitleBalan = db.LeavesEntitle.Where(x => x.EmployeeId == leaveRequest.EmployeeId && !x.IsExpired);

        //    if (entitleBalan.Any())
        //    {
        //        entitleBal = entitleBalan.Sum(x => x.Balance);
        //    }
        //    if (leaveRequest.LeaveDays > entitleBal && leaveRequest.Type == LeaveType.ANNUAL)
        //    {
        //        ModelState.AddModelError("", "you requested morethan your balance");
        //    }
        //    var leaveReqPrev = db.LeaveRequest.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.LeaveRequestId != leaveRequest.LeaveRequestId && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To)));
        //    if (leaveReqPrev != null)
        //    {
        //        ModelState.AddModelError("", "Your request is overlap with other request");
        //    }
        //    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId);
        //    if (emp != null && leaveRequest.Type == LeaveType.MATERNITY && emp.Gender == Gender.MALE)
        //    {
        //        ModelState.AddModelError("", "Male can't take Maternity Leave ");
        //    }
        //    if (leaveRequest.Type == LeaveType.MATERNITY)
        //    {
        //        var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.Maternitiy.ToString());
        //        if (config != null)
        //        {
        //            var allowedDays = Convert.ToDecimal(config.CurrentValue);
        //            if (leaveRequest.LeaveDays > allowedDays)
        //            {
        //                ModelState.AddModelError("", "You are requesting maternity leave above the setting value");
        //            }
        //            var maternityRequests = db.LeaveRequest.Where(x => x.Type == LeaveType.MATERNITY && x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId && x.LeaveRequestId != leaveRequest.LeaveRequestId).ToList();
        //            if (maternityRequests.Any())
        //            {
        //                var totalAskedMaternity = maternityRequests.Sum(x => x.LeaveDays) + leaveRequest.LeaveDays;
        //                if (totalAskedMaternity > allowedDays)
        //                {
        //                    ModelState.AddModelError("", "You can't take materinit leave more than " + allowedDays + " in one year, check your previous maternity leave request");
        //                }
        //            }
        //        }
        //    }
        //    if (emp != null && leaveRequest.Type == LeaveType.PATERNITY && emp.Gender == Gender.FEMALE)
        //    {
        //        ModelState.AddModelError("", "Female can't take Paternity Leave ");
        //    }
        //    if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced)
        //    {
        //        if (leaveRequest.Type != LeaveType.ANNUAL && leaveRequest.Type != LeaveType.OTHER && leaveRequest.Type != LeaveType.Leave_With_Out_Pay)
        //        {
        //            ModelState.AddModelError("", "You can't take Forced Leave for " + leaveRequest.Type + " it's allowed for only Annual,Leave without pay and Other leave types");
        //        }
        //    }


        //    if (leaveRequest.Type == LeaveType.ANNUAL)
        //    {

        //        var leavePlans = db.LeavePlans.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId);
        //        if (leavePlans != null && leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
        //        {
        //            if (leaveRequest.From.Date >= leavePlans.From.Date && leaveRequest.To.Date <= leavePlans.To.Date)
        //            {

        //                if (leaveRequest.LeaveDays > leavePlans.LeaveAmount)
        //                {
        //                    ModelState.AddModelError("", "The requested amount is greater than your plan!!");
        //                }


        //            }
        //            else if (leaveRequest.From.Date < leavePlans.From.Date || leaveRequest.To.Date > leavePlans.To.Date)
        //            {
        //                ModelState.AddModelError("", "Date you select is out of your Plan date range!!");
        //            }

        //        }
        //        else if (leaveRequest.LeaveActionTypes == LeaveActionTypes.With_Plan)
        //        {
        //            ModelState.AddModelError("", "You should set leave plan for this period!!");
        //        }

        //    }


        //    if (leaveRequest.From.Date > leaveRequest.To.Date)
        //    {
        //        ModelState.AddModelError("", "Date from should less or equal to date to!!");
        //    }
        //    else if (leaveRequest.LeaveDays == 0)
        //    {
        //        ModelState.AddModelError("", "Leave Days must be greater than 0!!");
        //    }


        //    var leave = db.Leaves.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId);
        //    if (leave != null)
        //    {
        //        if (leaveRequest.From.Date >= leave.From && leaveRequest.From.Date <= leave.To)
        //        {
        //            ModelState.AddModelError("", "Intial date overlap other leave taken date. please select other date range");
        //        }
        //        if (leaveRequest.To.Date >= leave.From && leaveRequest.To.Date <= leave.To)
        //        {
        //            ModelState.AddModelError("", "End date over lapother leave taken date. please select other date range");
        //        }
        //    }

        //    var entitelCurrentYear = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId);

        //    if (entitelCurrentYear != null && leaveRequest.Type == LeaveType.ANNUAL)
        //    {
        //        var workedDays = (leaveRequest.To - per.DateFrom.Date).Days;

        //        var totalEntitle = entitleBalan.Sum(x => x.Balance);

        //        var exactDeserve = GetEmployeeDeserveLeaveDays(per.DateFrom, leaveRequest.To, entitelCurrentYear.Amount, entitelCurrentYear.Balance, totalEntitle);

        //        var currentYearDeserve = GetCurrentYearDeserve(per.DateFrom, leaveRequest.To, entitelCurrentYear.Amount);
        //        var currentYearBalance = entitelCurrentYear.Balance;
        //        var currentYearEntitle = entitelCurrentYear.Amount;

        //        var previousLeftBalance = GetPreviosYearLeftBalance(entitelCurrentYear.Balance, totalEntitle);

        //        if (previousLeftBalance == 0 && leaveRequest.LeaveDays > currentYearDeserve && leaveRequest.Type == LeaveType.ANNUAL)
        //        {
        //            ModelState.AddModelError("", "you can't take this amount of leave  in this period ");
        //        }

        //        if (leaveRequest.LeaveDays > previousLeftBalance && leaveRequest.Type == LeaveType.ANNUAL)
        //        {
        //            var neededFromCurrentYear = leaveRequest.LeaveDays - previousLeftBalance;
        //            var takenFromCurrentYear = currentYearEntitle - currentYearBalance;
        //            var needed = takenFromCurrentYear + neededFromCurrentYear;
        //            if (neededFromCurrentYear > currentYearDeserve)
        //            {
        //                ModelState.AddModelError("", "you request exceed your deserve amount");
        //            }
        //            if (needed > currentYearDeserve)
        //            {
        //                ModelState.AddModelError("", "you request exceed your left deserve amount");
        //            }
        //            var leaveRequests = db.LeaveRequest.Where(x => x.EmployeeId == leaveRequest.EmployeeId && x.GlFiscalYearId == per.GlFiscalYearId && x.Type == LeaveType.ANNUAL && !x.IsOnlineApproved && x.LeaveRequestId != leaveRequest.LeaveRequestId);
        //            if (leaveRequests.Any())
        //            {
        //                var leaveRequestedDays = leaveRequests.Sum(x => x.LeaveDays);
        //                var sumofRequest = leaveRequestedDays + leaveRequest.LeaveDays;

        //                if (sumofRequest > previousLeftBalance)
        //                {
        //                    var neededFromNow = sumofRequest - previousLeftBalance;
        //                    var totalNeeded = neededFromNow + takenFromCurrentYear;
        //                    if (neededFromNow > currentYearDeserve)
        //                    {
        //                        ModelState.AddModelError("", "you have other waiting requests so sum of days exceed your deserve amount");
        //                    }
        //                    if (totalNeeded > currentYearDeserve)
        //                    {
        //                        ModelState.AddModelError("", "you have other waiting requests so sum of days exceed your left deserve amount");
        //                    }
        //                }
        //            }
        //        }

        //        //}               

        //    }
        //    else if (leaveRequest.Type == LeaveType.ANNUAL)
        //    {
        //        ModelState.AddModelError("", "You have no entitlement for this budget year");
        //    }
        //    if (leaveRequest.EmployeeId != 0)
        //    {
        //        var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
        //        if (placemnt != null)
        //        {
        //            leaveRequest.BranchId = placemnt.BranchId;
        //            leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("placement", "Employee has not current placemnt");
        //        }
        //    }
        //    if (leaveRequest.EmployeeId != 0)
        //    {
        //        var placemnt = db.Placements.Where(x => x.EmployeeId == leaveRequest.EmployeeId).Where(x => x.isCurrent == true).FirstOrDefault();
        //        if (placemnt != null)
        //        {
        //            leaveRequest.BranchId = placemnt.BranchId;
        //            leaveRequest.BusinessUnitId = placemnt.BusinessUnitId;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("placement", "Employee has not current placemnt");
        //        }
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var totalBalance = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId).Sum(x => x.Balance);
        //        leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;


        //        var entity = new LeaveRequest
        //        {
        //            LeaveRequestId = leaveRequest.LeaveRequestId,
        //            EmployeeId = leaveRequest.EmployeeId,
        //            BranchId = leaveRequest.BranchId,
        //            BusinessUnitId = leaveRequest.BusinessUnitId,
        //            From = leaveRequest.From,
        //            To = leaveRequest.To,
        //            LeaveDays = leaveRequest.LeaveDays,
        //            Type = leaveRequest.Type,
        //            Reason = leaveRequest.Reason,
        //            GlFiscalYearId = leaveRequest.GlFiscalYearId,
        //            Remark = leaveRequest.Remark,
        //            RequestDate = DateTime.Now,
        //            LeaveBalance = totalBalance,
        //            Day = leaveRequest.Day,
        //            MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
        //            Hours = leaveRequest.Hours,
        //            FromAmharic = leaveRequest.FromAmharic,
        //            ToAmharic = leaveRequest.ToAmharic,
        //            IsForOther = leaveRequest.IsForOther,
        //            LeaveActionTypes = leaveRequest.LeaveActionTypes,
        //            EmployeeCode = inputs.GetEmployeeCode(leaveRequest.EmployeeId),
        //            RequesterId = UserInformationHelper.GetEmployeeId(User.Identity.Name)
        //        };


        //        db.LeaveRequest.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        IsForOther = false;

        //        #region operation log

        //        UserHelper.OperationLog(Request.UserHostAddress, "LeaveRequestPersonal ", "Success", User.Identity.Name, User.Identity.Name + " update leave request of id" + entity.LeaveRequestId, Modules.HRM);

        //        #endregion
        //    }

        //    return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequest_Destroy([DataSourceRequest]DataSourceRequest request, LeaveRequest leaveRequest)
        {
            ModelState.Clear();
            if (leaveRequest.IsOnlineApproved)
            {
                ModelState.AddModelError("", "you can't delete approved requestes");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new LeaveRequest
                    {
                        LeaveRequestId = leaveRequest.LeaveRequestId,
                        EmployeeId = leaveRequest.EmployeeId,
                        BranchId = leaveRequest.BranchId,
                        BusinessUnitId = leaveRequest.BusinessUnitId,
                        From = leaveRequest.From,
                        To = leaveRequest.To,
                        LeaveDays = leaveRequest.LeaveDays,
                        Type = leaveRequest.Type,
                        Reason = leaveRequest.Reason,
                        GlFiscalYearId = leaveRequest.GlFiscalYearId,
                        Remark = leaveRequest.Remark,
                        LeaveBalance = leaveRequest.LeaveBalance,
                        FromAmharic = leaveRequest.FromAmharic,
                        ToAmharic = leaveRequest.ToAmharic,
                        EmployeeCode = leaveRequest.EmployeeCode,
                        IsForOther = leaveRequest.IsForOther,
                        Day = leaveRequest.Day,
                        MorningOrAfternoon = leaveRequest.MorningOrAfternoon,
                        Hours = leaveRequest.Hours
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted leave request of employee code  " + leaveRequest.EmployeeCode);
                    #endregion
                    db.LeaveRequest.Attach(entity);
                    db.LeaveRequest.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { leaveRequest }.ToDataSourceResult(request, ModelState));
        }
        private DateTime ParseEthiopicDate(string ethiopicDate)
        {
            var parts = ethiopicDate.Split('/');
            return XAPI.EthiopicDateTime.GetGregorianDate(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Leave" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "false";
            }
            return "false";
        }
        public double GetWeekDays(DateTime startDate, DateTime endDate)
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
                        }
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
                        //dayList.Add(testDate);
                        break;
                }
            }
            return noOfWeek;
        }
        public decimal GetLeaveDays(string from, string to, string half, int hour)
        {

            decimal days = 0;

            if (from != "" && to != "")
            {


                FromAmharic = from;
                var splitDatefrom = FromAmharic.Split('/');
                DateTime f = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDatefrom[0]), Convert.ToInt32(splitDatefrom[1]), Convert.ToInt32(splitDatefrom[2]));

                ToAmharic = to;
                var splitDateto = ToAmharic.Split('/');
                DateTime t = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateto[0]), Convert.ToInt32(splitDateto[1]), Convert.ToInt32(splitDateto[2]));
                var count = 0;
                double weekDays = 0;
                var year = db.GLFiscalYears.FirstOrDefault(x => x.Status == FiscalYearStatus.Open);
                if (year != null)
                {
                    var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == year.GlFiscalYearId).ToList();
                    if (holidayList.Any())
                    {
                        foreach (var h in holidayList)
                        {
                            if (h.Date >= f && h.Date <= t)
                            {
                                count++;
                            }
                        }
                    }
                    weekDays = GetWeekDays(f, t);
                }




                days = t.Subtract(f).Days + 1 - count;
                days = days - Convert.ToDecimal(weekDays);
                if (half == "1")
                {
                    decimal checkWeekEnd = t.Subtract(f).Days + 1 - count;
                    double amt = GetHalf(f, t);
                    double notHalf = GetNotHalf(f, t);
                    bool isHalf = IsHalf(f, t);
                    if (isHalf)
                    {
                        days = (checkWeekEnd - Convert.ToDecimal(notHalf)) / 2;
                    }
                    else
                    {
                        days = days / 2;
                    }

                }

                decimal totalhours = days * 8;
                decimal somehours = days * hour;
                days = (totalhours - somehours) / 8;
            }
            return days;
        }
        public decimal GetBalance(/*int period,*/ /*int empId*/)
        {
            Total = 0;
            //if (empId == 0)
            //{
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //}


            var entitles = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired/*&& x.GlFiscalYearId == period*/);
            if (entitles.Any())
            {
                //var takenLeave = db.Leaves.Where(x => x.EmployeeId == empId && x.GlFiscalYearId == period).ToList();
                //var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.EmployeeId == empId && x.Check).ToList();
                //var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period && x.EmployeeId == empId).ToList();
                //decimal sumoftaken = takenLeave.Sum(x => x.Amount);
                //decimal sumoftransfer = tranferedList.Sum(x => x.Amount);
                //decimal sumofEntitle = entitles.Amount;
                //decimal sumofpaid = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));
                //decimal subtract = sumoftaken + sumofpaid;

                //Total = (sumofEntitle + sumoftransfer) - subtract;

                Total = entitles.Sum(x => x.Balance);
            }
            return Total;
        }
        public decimal GetLeaveDays(DateTime from, DateTime to, int glFiscalYearId, LeaveDay day, LeaveType leaveType)
        {
            decimal leaveDays = 0;

            decimal holiday = GetHolidays(from, to, glFiscalYearId);
            var weekDays = GetWeekDays(from, to);
            if (leaveType == LeaveType.ANNUAL)
            {
                leaveDays = to.Subtract(from).Days + 1 - holiday;
                leaveDays = leaveDays - Convert.ToDecimal(weekDays);
                if (day == LeaveDay.Half_Day)
                {
                    decimal checkWeekEnd = to.Subtract(from).Days + 1 - holiday;
                    double amt = GetHalf(from, to);
                    double notHalf = GetNotHalf(from, to);
                    bool isHalf = IsHalf(from, to);
                    if (isHalf)
                    {
                        leaveDays = (checkWeekEnd - Convert.ToDecimal(notHalf)) / 2;
                    }
                    else
                    {
                        leaveDays = leaveDays / 2;
                    }
                }

            }
            else
            {
                leaveDays = to.Subtract(from).Days + 1;
                if (day == LeaveDay.Half_Day)
                {
                    leaveDays = leaveDays / 2;
                }
            }

            return leaveDays;
        }
        public decimal GetEmployeeDeserveLeaveDays(DateTime from, DateTime to, decimal amount, decimal balance, decimal totalbalance)
        {
            decimal exactDeserve = 0;

            var workedDays = (to.Date - from.Date).Days;

            var deserveAmountCurrentYear = (amount * workedDays) / 365;

            var previousEntitle = totalbalance - balance;

            exactDeserve = previousEntitle + deserveAmountCurrentYear;

            var modul = exactDeserve % 1;
            decimal added = 0;

            if (modul >= Convert.ToDecimal(0.7))
            {
                added = 1;
            }
            else if (modul <= Convert.ToDecimal(0.3))
            {
                added = 0;
            }
            else
            {
                added = Convert.ToDecimal(0.5);
            }

            exactDeserve = (int)exactDeserve + added;

            return exactDeserve;
        }
        public decimal GetCurrentYearDeserve(DateTime from, DateTime to, decimal amount)
        {

            var workedDays = (to.Date - from.Date).Days;

            var deserveAmountCurrentYear = (amount * workedDays) / 365;

            return deserveAmountCurrentYear;
        }
        public decimal GetPreviosYearLeftBalance(decimal balance, decimal totalbalance)
        {
            var leftBalanceFromPrevious = totalbalance - balance;

            return leftBalanceFromPrevious;
        }
        public string GetLeaveBalance(int empId)
        {
            //SalaryJobTitle salary = new SalaryJobTitle();

            if (empId == 0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }
            var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);

            if (entitle.Any())
            {
                var balance = entitle.Sum(x => x.Amount).ToString();

                return balance;
            }

            return "0";
        }
        public decimal GetBalance(int period, int empId)
        {
            Total = 0;
            if (empId == 0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }


            var entitles = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == empId && x.GlFiscalYearId == period);
            //if (entitles != null)
            //{
            //    var takenLeave = db.Leaves.Where(x => x.EmployeeId == empId && x.GlFiscalYearId == period).ToList();
            //    var tranferedList = db.LeaveTransfer.Where(x => x.To == period && x.EmployeeId == empId && x.Check).ToList();
            //    var paidIncash = db.LeavePaid.Where(x => x.GlFiscalYearId == period && x.EmployeeId == empId).ToList();
            //    decimal sumoftaken = takenLeave.Sum(x => x.Amount);
            //    decimal sumoftransfer = tranferedList.Sum(x => x.Amount);
            //    decimal sumofEntitle = entitles.Amount;
            //    decimal sumofpaid = Convert.ToDecimal(paidIncash.Sum(x => x.Amount));
            //    decimal subtract = sumoftaken + sumofpaid;

            //    Total = (sumofEntitle + sumoftransfer) - subtract;
            //}
            return Total;
        }
        public decimal GetAmount(int period, int empId)
        {
            var balance = db.LeavesEntitle.Where(x=>x.EmployeeId == empId && !x.IsExpired);
            decimal totalBalance = 0;
            if(balance.Any())
            {
              
                totalBalance = balance.Sum(x => x.Amount);
            }
            return totalBalance;
        }
        public decimal GetCurrentEmpBalance(int period)
        {

            var user = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var balance = db.LeavesEntitle.Where(x => x.EmployeeId == user && !x.IsExpired);
            decimal totalBalance = 0;
            if (balance != null)
            {
                totalBalance = balance.Sum(x => x.Amount);
            }
            return totalBalance;

        }
        public decimal GetHolidays(DateTime from, DateTime to, int glFiscalYearId)
        {
            decimal count = 0;

            var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == glFiscalYearId).ToList();
            if (holidayList.Any())
            {
                foreach (var h in holidayList)
                {
                    if (h.Date >= from && h.Date <= to)
                    {
                        count++;
                    }
                }
            }
            return count;
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
        public ActionResult UploadDocument(int leaveRequestId, HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + leaveRequestId.ToString() + "-" + AttachmentType.DOCUMENT), leaveRequestId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + leaveRequestId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = leaveRequestId.ToString(),
                            Description = leaveRequestId.ToString() + "-" + txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = leaveRequestId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult DownloadDoument(string filename, string attachId)
        {
            int.TryParse(attachId, out int attachmentId);
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
