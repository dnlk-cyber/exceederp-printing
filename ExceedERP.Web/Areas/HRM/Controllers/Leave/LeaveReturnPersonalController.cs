using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveUser,
        HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class LeaveReturnPersonalController : Controller
    {
        // GET: HRM/LeaveReturn
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext dbApplication = new ApplicationDbContext();

      
        HRMInputs inputs = new HRMInputs();
        public static int EmployeeId { get; set; }
        public static bool IsForOther { get; set; }
        public static decimal Total = 0;
        public static List<string> reportsToEmployee = new List<string>();
        public ActionResult Index()
        {

            //GetStatusType();
            //GetPeriod();
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            //EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            ViewData["EmployeeList"] = db.Person_Employee.Select(emp => new
                                                          {
                                                              EmployeeId = emp.EmployeeId,
                                                              PersonId = emp.EmployeeId, // If this is correct, otherwise adjust if you meant PersonId column
                                                              EmployeeCode = emp.EmployeeCode,
                                                              FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName,
                                                              FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName,
                                                              IsActive = emp.IsActive
                                                          }).AsQueryable();

            //ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            //reportsToEmployee = list.Select(x => x.PersonId.ToString()).ToList();
            //reportsToEmployee.Add(EmployeeId.ToString());

            //var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = db.Person_Employee.AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

            ViewData["UserName"] = dbApplication.Users.AsQueryable();
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();
            ViewData["RequestStatus"] = Enum.GetValues(typeof(RequestStatus)).Cast<RequestStatus>().Select(s => new { Text = s.ToString(), Value = s.ToString() }).ToList();
            ViewData["PeriodList"] = db.GLFiscalYears.Select(x => new { x.GlFiscalYearId, x.Name }).AsQueryable();
        }
        public int SetEmployee(string x)
        {
            IsForOther = false;

            if (x == "true")
            {
                var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                //var list = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeId).ToList();
                var list = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId).ToList();
                list.Remove(employeeId);
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
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "LeaveReturn").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "LeaveReturn" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "LeaveReturn" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        #region Sendfor approval old
        //public ActionResult SendForApproval()
        //{
        //    var status =
        //              db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
        //    string messages = "";
        //    if (status.Any())
        //    {
        //        foreach (var s in status)
        //        {
        //            if (s.Status == RequestStatus.APPROVE)
        //            {
        //                messages = "The Request is already approved!!";
        //                //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //                ModelState.AddModelError("xx", messages);
        //                return null;
        //            }
        //            if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
        //            {
        //                messages = "The Request is already send for approval!!";
        //                // return  ModelState.AddModelError("xx", messages );
        //                return null;//new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //            }

        //        }
        //    }
        //    RequestesStatus requestesStatus;
        //    db = new ExceedDbContext();
        //    requestesStatus =
        //        db.RequestesStatus.Where(
        //            x =>
        //                x.RequestId == HRMInputs.RequestId &&
        //                x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
        //    db = new ExceedDbContext();
        //    if (requestesStatus == null)
        //    {
        //        var entityy = new RequestesStatus
        //        {
        //            RequestId = HRMInputs.RequestId,
        //            ReferenceType = HRMInputs.ReferenceType,
        //            Status = RequestStatus.WAITING_FOR_APPROVAL,
        //            Date = DateTime.Now,
        //            DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
        //        };

        //        db.RequestesStatus.Add(entityy);
        //        db.SaveChanges();
        //    }
        //    else
        //    {
        //        var entity = new RequestesStatus
        //        {
        //            RequestStatusId = requestesStatus.RequestStatusId,
        //            RequestId = requestesStatus.RequestId,
        //            ReferenceType = requestesStatus.ReferenceType,
        //            Status = requestesStatus.Status,
        //            Date = requestesStatus.Date,
        //            DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
        //            Remark = requestesStatus.Remark
        //        };

        //        db.RequestesStatus.Attach(entity);
        //        db.RequestesStatus.Remove(entity);
        //        db.SaveChanges();
        //        db = new ExceedDbContext();

        //        var entityy = new RequestesStatus
        //        {
        //            RequestId = HRMInputs.RequestId,
        //            ReferenceType = HRMInputs.ReferenceType,
        //            Status = RequestStatus.WAITING_FOR_APPROVAL,
        //            Date = DateTime.Now,
        //            DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
        //        };

        //        db.RequestesStatus.Add(entityy);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}
        #endregion
        public JsonResult SendForApproval(int id)
        {
            var leaveReturn = db.LeaveReturns.FirstOrDefault(x => x.LeaveReturnId == id);
            if (leaveReturn != null && leaveReturn.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The Request is already approved.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (leaveReturn != null && leaveReturn.IsDepartmentSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The Request is already send for approval.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (leaveReturn != null)
            {
                var previousRequest = db.LeaveReturns.Where(x => x.EmployeeId == leaveReturn.EmployeeId && x.IsDepartmentSendForApproval && x.LeaveReturnId != leaveReturn.LeaveReturnId &&
                                              ((x.From <= leaveReturn.From && x.To >= leaveReturn.From) ||
                                              x.To >= leaveReturn.From && x.To <= leaveReturn.To));
                if (previousRequest.Any())
                {
                    // ModelState.AddModelError("","you can't send this request it overlaps to your previous request");
                    //var messages = "you can't send this request it overlaps to your previous request";
                    //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    var formattedData = new
                    {
                        Message = "you can't send this request it overlaps to your previous request.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

            }
            if (ModelState.IsValid)
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
                            // messages = "The Request is already approved!!";
                            //  return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                            //ModelState.AddModelError("xx", messages);
                            // return null;
                            var formattedData = new
                            {
                                Message = "The Request is already approved!!",
                                Status = "OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                        if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                        {
                            //messages = "The Request is already send for approval!!";
                            // return  ModelState.AddModelError("xx", messages );
                            // return null;//new JavaScriptResult() { Script = "alert('" + messages + "');" };
                            var formattedData = new
                            {
                                Message = "The Request is already send for approval.",
                                Status = "OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
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

                    var updateLeaveRequest = db.LeaveRequest.Find(entityy.RequestId);
                    if (updateLeaveRequest != null)
                    {
                        updateLeaveRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                        updateLeaveRequest.DepartmentSendForApprovalTime = DateTime.Now;
                        updateLeaveRequest.IsDepartmentSendForApproval = true;

                        db.Entry(updateLeaveRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
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

                    var updateLeaveRequest = db.LeaveRequest.Find(entityy.RequestId);
                    if (updateLeaveRequest != null)
                    {
                        updateLeaveRequest.DepartmentSendForApprovalBy = User.Identity.Name;
                        updateLeaveRequest.DepartmentSendForApprovalTime = DateTime.Now;
                        updateLeaveRequest.IsDepartmentSendForApproval = true;

                        db.Entry(updateLeaveRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var headId = EmployeeHelper.GetHeadId(empId);
            if (headId != 0)
            {
                NotifModels notif = new NotifModels
                {
                    UserId = UserInformationHelper.GetEmployeeUserId(headId),
                    Title = "Leave Return Request",
                    Message = $"{User.Identity.Name} has Submitted and sent a Leave Return Request  that requires your approval.",
                    URL = "~/HRM/LeaveReturn"
                };

                Notify.NotifyUser(notif);
            }
            var formattedData1 = new
            {
                Message = "Request successfully sent.",
                Status = "Success"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        public bool CheckStatusForBackColor(int leaveId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == leaveId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "LeaveReturn").FirstOrDefault();
            if (reques != null)
            {
                return true;
            }
            return false;
        }
        public ActionResult RefreshPreviousLeaveData()
        {
            var leaveRequests = db.LeaveRequest.ToList();

            foreach (var leave in leaveRequests)
            {
                if (leave.Type != LeaveType.ANNUAL|| leave.Type != LeaveType.DayOff)
                {
                    var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == leave.LeaveRequestId && x.Status == RequestStatus.APPROVE);
                    if (requestStatus != null)
                    {
                        var leaveTaken = db.Leaves.FirstOrDefault(x => x.RequestId == leave.LeaveRequestId);

                        if (leaveTaken == null)
                        {

                            int count = 0;
                            double weekDays = 0;
                            var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == leave.GlFiscalYearId).ToList();
                            if (holidayList.Any())
                            {
                                foreach (var h in holidayList)
                                {
                                    if (h.Date >= leave.From && h.Date <= leave.To)
                                    {
                                        count++;
                                    }
                                }
                            }
                            var entity = new Core.Domain.HRM.LeaveManagment.Leave
                            {
                                EmployeeId = leave.EmployeeId,
                                LeaveType = leave.Type,
                                Amount = leave.LeaveDays,
                                GlFiscalYearId = leave.GlFiscalYearId,
                                From = leave.From,
                                To = leave.To,
                                FromAmharic = leave.FromAmharic,
                                ToAmharic = leave.ToAmharic,
                                OrgName = null,
                                Before = null,
                                After = null,
                                Remark = leave.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                EmployeeCode = leave.Employee.EmployeeCode,
                                ModifiedBy = null,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                RequestId = leave.LeaveRequestId
                            };

                            db.Leaves.Add(entity);
                            db.SaveChanges();

                        }
                    }
                }
            }

            return RedirectToAction("Index");
        }
        public decimal GetHolidays(DateTime from, DateTime to, int glFiscalYearId)
        {
            decimal count = 0;

            var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == glFiscalYearId).ToList();
            if (holidayList.Any())
            {
                foreach (var h in holidayList)
                {
                    if (h.Date.Date >= from && h.Date.Date <= to)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public double GetWeekDays(DateTime startDate, DateTime endDate, int empId)
        {
            double noOfWeek = 0;
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
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
                                var weekendExceptions = db.WeekendExceptions.ToList();
                                if (weekendExceptions.Any())
                                {
                                    var weekendException = db.WeekendExceptions
                                              .FirstOrDefault(x => x.EmployeeId == empId);
                                    if (weekendException == null && placement != null)
                                    {
                                        weekendException = db.WeekendExceptions.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);

                                    }
                                    // Skip because this is an exception for those simple employees that have Saturday as a working day, while other workers have it as an off day.

                                    if (weekendException != null)
                                    {
                                        noOfWeek += saturdayweekendSetting.IsHalfDay ? 0.5 : saturdayweekendSetting.IsFullDay ? 0 : 1;

                                    }
                                    else
                                    {
                                        // Skip because this is an exception for those simple employees that have Saturday as a working day, while other workers have it as an off day.
                                    }
                                }
                                else
                                {
                                    if (saturdayweekendSetting.IsHalfDay)
                                    {
                                        noOfWeek += 0.5;
                                    }
                                    else if (saturdayweekendSetting.IsFullDay)
                                    {
                                        noOfWeek += 0;

                                    }
                                    else
                                    {
                                        noOfWeek += 1;

                                    }
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
                                else if (sundayweekendSetting.IsFullDay)
                                {
                                    noOfWeek += 0;
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

        public ActionResult LeaveReturn_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeesId = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId).ToList();
            employeesId.Add(employeeId);
            //List<int> employeeids = EmployeeHelper.GetSubordinates(employeeid);
            //IQueryable<LeaveRequest> leaverequest = db.LeaveRequest.Where(x => employeeids.Contains(x.EmployeeId)).OrderBy(x => x.RequestDate);
            //IQueryable<LeaveRequest> leaverequest = db.LeaveRequest.Where(x => x.EmployeeId == employeeid || x.RequesterId == employeeid).OrderBy(x => x.RequestDate);
            IQueryable<LeaveReturn> leaveReturns = db.LeaveReturns.Where(x => employeesId.Contains(x.EmployeeId));

            DataSourceResult result = leaveReturns.ToDataSourceResult(request, leaveReturn => new
            {
                LeaveReturnId = leaveReturn.LeaveReturnId,
                LeaveRequestId = leaveReturn.LeaveRequestId,
                EmployeeId = leaveReturn.EmployeeId,
                GlFiscalYearId = leaveReturn.GlFiscalYearId,
                Amount = leaveReturn.Amount,
                IsApproved = CheckStatusForBackColor(leaveReturn.LeaveReturnId),
                From = leaveReturn.From,
                To = leaveReturn.To,
                FromAm = leaveReturn.FromAm,
                ToAm = leaveReturn.ToAm,
                Reason = leaveReturn.Reason,
                DepartmentSendForApprovalBy = leaveReturn.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leaveReturn.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leaveReturn.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = leaveReturn.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leaveReturn.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leaveReturn.IsDepartmentOnlineApproved,
                OnlineApprovedBy = leaveReturn.OnlineApprovedBy,
                OnlineApprovedTime = leaveReturn.OnlineApprovedTime,
                IsOnlineApproved = leaveReturn.IsOnlineApproved,
                IsSendForApproval = leaveReturn.IsSendForApproval,
                 IsForOther = leaveReturn.IsForOther

            });

            return Json(result);
        }
        public ActionResult LeaveReturnPersonal_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //List<int> employeeids = EmployeeHelper.GetSubordinates(employeeid);
            //IQueryable<LeaveRequest> leaverequest = db.LeaveRequest.Where(x => employeeids.Contains(x.EmployeeId)).OrderBy(x => x.RequestDate);
            //IQueryable<LeaveRequest> leaverequest = db.LeaveRequest.Where(x => x.EmployeeId == employeeid || x.RequesterId == employeeid).OrderBy(x => x.RequestDate);
            IQueryable<LeaveReturn> leaveReturns = db.LeaveReturns.Where(x => x.LeaveRequestId == id);

            DataSourceResult result = leaveReturns.ToDataSourceResult(request, leaveReturn => new
            {
                LeaveReturnId = leaveReturn.LeaveReturnId,
                LeaveRequestId = leaveReturn.LeaveRequestId,
                EmployeeId = leaveReturn.EmployeeId,
                GlFiscalYearId = leaveReturn.GlFiscalYearId,
                Amount = leaveReturn.Amount,
                IsApproved = CheckStatusForBackColor(leaveReturn.LeaveReturnId),
                From = leaveReturn.From,
                To = leaveReturn.To,
                FromAm = leaveReturn.FromAm,
                ToAm = leaveReturn.ToAm,
                DepartmentSendForApprovalBy = leaveReturn.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = leaveReturn.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = leaveReturn.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = leaveReturn.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = leaveReturn.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = leaveReturn.IsDepartmentOnlineApproved,
                OnlineApprovedBy = leaveReturn.OnlineApprovedBy,
                OnlineApprovedTime = leaveReturn.OnlineApprovedTime,
                IsOnlineApproved = leaveReturn.IsOnlineApproved,
                // IsForOther = leaveReturn.IsForOther

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveReturnPersonal_Create([DataSourceRequest]DataSourceRequest request, LeaveReturn leaveReturn, int id)
        {
          

            
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(leaveReturn.FromAm)
                || string.IsNullOrEmpty(leaveReturn.ToAm))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                    return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    leaveReturn.From = HrHelper.ParseEthiopicDate(leaveReturn.FromAm);
                    leaveReturn.To = HrHelper.ParseEthiopicDate(leaveReturn.ToAm);
                }

            }
            else
            {

                leaveReturn.FromAm = XAPI.EthiopicDateTime.GetEthiopicDate(leaveReturn.From.Day, leaveReturn.From.Month, leaveReturn.From.Year);
                leaveReturn.ToAm = XAPI.EthiopicDateTime.GetEthiopicDate(leaveReturn.To.Day, leaveReturn.To.Month, leaveReturn.To.Year);

            }
            var leaveRequest = db.LeaveRequest
             .Where(x => x.LeaveRequestId == id
             && x.Type == LeaveType.ANNUAL
             && x.IsOnlineApproved
             && !x.IsVoid).FirstOrDefault();
            if (leaveRequest == null)
            {
                ModelState.AddModelError("", "No such annual  Leave Request which is approved ");
                return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
            }
           
            if (leaveRequest.From.Date>leaveReturn.From
                &&leaveRequest.To.Date<leaveReturn.From)
            {

                ModelState.AddModelError("", "Date Range is out Of request ");
                return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));


            }
            leaveReturn.EmployeeId = leaveRequest.EmployeeId;

            leaveReturn.LeaveRequestId = leaveRequest.LeaveRequestId;
            
            
           
            var per = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveReturn.From.Date && x.DateTo >= leaveReturn.From.Date);
            if (per == null)
            {
                ModelState.AddModelError("", "Check your Period if it defined");
                return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
            }
            
            // Calculate leave days if the amount is zero or exceeds the leave days available
            if (leaveReturn.Amount == 0 || leaveReturn.Amount > leaveRequest.LeaveDays ||
                leaveRequest.From.Date > leaveReturn.From.Date && leaveReturn.To.Date < leaveRequest.To.Date ||
                leaveRequest.From.Date >= leaveReturn.From.Date && leaveReturn.To.Date < leaveRequest.To.Date)
            {
                leaveReturn.Amount = HrHelper.GetLeaveDays(
                    leaveReturn.From, leaveReturn.To, leaveReturn.GlFiscalYearId,
                    leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
            }
            // If the requested leave period matches exactly
            else if (leaveRequest.From.Date == leaveReturn.From.Date 
                   && leaveReturn.To.Date == leaveRequest.To.Date)
            {
                leaveReturn.Amount = leaveRequest.LeaveDays;
            }

            // Handle partial leave cases
            else
            {
                leaveReturn.Amount = HrHelper.GetLeaveDays(
                    leaveReturn.From, leaveReturn.To, leaveReturn.GlFiscalYearId,
                    leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
            }


            
            if (ModelState.IsValid)
            {
                var entity = new LeaveReturn
                {
                    LeaveRequestId = id,
                    EmployeeId = leaveReturn.EmployeeId,
                    GlFiscalYearId = leaveReturn.GlFiscalYearId,
                    Amount = leaveReturn.Amount,
                    From = leaveReturn.From,
                    To = leaveReturn.To,
                    FromAm = leaveReturn.FromAm,
                    ToAm = leaveReturn.ToAm,
                    Reason = leaveReturn.Reason,
                    Remark = leaveReturn.Remark,
                    Status = RequestStatus.WAITING_FOR_APPROVAL.ToString(),
                    IsSendForApproval = true
                };
                db.LeaveReturns.Add(entity);
                db.SaveChanges();
                var reqeustStatus = new RequestesStatus
                {
                    RequestId = entity.LeaveReturnId,
                    ReferenceType = "LeaveReturn",
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    ChangedBy = User.Identity.Name,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Remark = "",
                    ApproverId = 0
                };
                db.RequestesStatus.Add(reqeustStatus);
                db.SaveChanges();
            }

            return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveReturnPersonal_Update([DataSourceRequest]DataSourceRequest request, LeaveReturn leaveReturn)
        {
            var leaveReturnexisted = db.LeaveReturns.Find(leaveReturn.LeaveReturnId);
            if(leaveReturnexisted != null&& leaveReturnexisted.IsOnlineApproved)
            {
                ModelState.AddModelError("", "you can't modify returned leave ");
                return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));

            }
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(leaveReturn.FromAm)
                || string.IsNullOrEmpty(leaveReturn.ToAm))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                    return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    leaveReturn.From = HrHelper.ParseEthiopicDate(leaveReturn.FromAm);
                    leaveReturn.To = HrHelper.ParseEthiopicDate(leaveReturn.ToAm);
                }

            }
            else
            {

                leaveReturn.FromAm = XAPI.EthiopicDateTime.GetEthiopicDate(leaveReturn.From.Day, leaveReturn.From.Month, leaveReturn.From.Year);
                leaveReturn.ToAm = XAPI.EthiopicDateTime.GetEthiopicDate(leaveReturn.To.Day, leaveReturn.To.Month, leaveReturn.To.Year);

            }
            var per = db.GLFiscalYears
                .FirstOrDefault(x => x.DateFrom <= leaveReturn.From.Date
                && x.DateTo >= leaveReturn.From.Date);
            if (per == null)
            {
                ModelState.AddModelError("", "Check your Period if it defined");
                return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
            }
            var leaveRequest = db.LeaveRequest.FirstOrDefault(x => x.LeaveRequestId == leaveReturn.LeaveRequestId);
            if (leaveRequest == null)
            {
                ModelState.AddModelError("", "you have no annual leave taken in this date rage");
                return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));

            }
           
            if (leaveReturn.From.Date == leaveRequest.From.Date
                 && leaveReturn.To.Date == leaveRequest.To.Date)
            {
                leaveReturn.Amount = leaveRequest.LeaveDays;
            }
            // Calculate leave days if the amount is zero or exceeds the leave days available
            if (leaveReturn.Amount == 0 || leaveReturn.Amount > leaveRequest.LeaveDays ||
                leaveRequest.From.Date > leaveReturn.From.Date && leaveReturn.To.Date < leaveRequest.To.Date ||
                leaveRequest.From.Date >= leaveReturn.From.Date && leaveReturn.To.Date < leaveRequest.To.Date)
            {
                leaveReturn.Amount = HrHelper.GetLeaveDays(
                    leaveReturn.From, leaveReturn.To, leaveReturn.GlFiscalYearId,
                    leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
            }

            if (ModelState.IsValid)
            {

                var entity = new LeaveReturn
                {
                    LeaveReturnId = leaveReturn.LeaveReturnId,
                    EmployeeId = leaveReturn.EmployeeId,
                    GlFiscalYearId = per.GlFiscalYearId,
                    LeaveRequestId = leaveReturn.LeaveRequestId,
                    Amount = leaveReturn.Amount,
                    From = leaveReturn.From,
                    To = leaveReturn.To,
                    FromAm = leaveReturn.FromAm,
                    ToAm = leaveReturn.ToAm,
                    Reason = leaveReturn.Reason,
                    Remark = leaveReturn.Remark,
                    IsSendForApproval = leaveReturn.IsSendForApproval
                };

                db.LeaveReturns.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveReturnPersonal_Destroy([DataSourceRequest]DataSourceRequest request, LeaveReturn leaveReturn)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LeaveReturn
                {
                    LeaveReturnId = leaveReturn.LeaveReturnId,
                    EmployeeId = leaveReturn.EmployeeId,
                    GlFiscalYearId = leaveReturn.GlFiscalYearId,
                    Amount = leaveReturn.Amount,
                    From = leaveReturn.From,
                    To = leaveReturn.To,
                    FromAm = leaveReturn.FromAm,
                    ToAm = leaveReturn.ToAm,
                    Reason = leaveReturn.Reason,
                    Remark = leaveReturn.Remark,
                    IsForOther = leaveReturn.IsForOther
                };
                #region delete log

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveReturn.EmployeeId);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Returened Leave of employee code  " + person.FullNameaEnglish);
                #endregion
                db.LeaveReturns.Attach(entity);
                db.LeaveReturns.Remove(entity);
                db.SaveChanges();

                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == entity.LeaveReturnId && x.ReferenceType == "LeaveReturn" && x.Status == RequestStatus.WAITING_FOR_APPROVAL);
                if (requestStatus != null)
                {
                    db.RequestesStatus.Attach(requestStatus);
                    db.RequestesStatus.Remove(requestStatus);
                    db.SaveChanges();
                }
            }

            return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
        }
        //public double GetWeekDays(DateTime startDate, DateTime endDate)
        //{
        //    double noOfWeek = 0;
        //    var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
        //    var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
        //    List<DateTime> dayList = new List<DateTime>();
        //    TimeSpan diff = endDate - startDate;
        //    int days = diff.Days;
        //    for (var i = 0; i <= days; i++)
        //    {
        //        var testDate = startDate.AddDays(i);
        //        switch (testDate.DayOfWeek)
        //        {
        //            case DayOfWeek.Saturday:

        //                if (saturdayweekendSetting != null)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        if (saturdayweekendSetting.IsHalfDay)
        //                        {
        //                            noOfWeek += 0.5;
        //                        }
        //                        else
        //                        {
        //                            noOfWeek += 1;

        //                        }
        //                    }
        //                }
        //                break;
        //            case DayOfWeek.Sunday:

        //                if (sundayweekendSetting != null)
        //                {
        //                    var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
        //                    if (holiday == null)
        //                    {
        //                        if (sundayweekendSetting.IsHalfDay)
        //                        {
        //                            noOfWeek += 0.5;
        //                        }
        //                        else
        //                        {
        //                            noOfWeek += 1;

        //                        }
        //                    }
        //                }
        //                //dayList.Add(testDate);
        //                break;
        //        }
        //    }
        //    return noOfWeek;
        //}
     
        public decimal GetLeaveDays(DateTime from, DateTime to, int glFiscalYearId, LeaveDay day, LeaveType leaveType)
        {
            decimal leaveDays = 0;

            decimal holiday = GetHolidays(from, to, glFiscalYearId);
            var weekDays = HrHelper.GetWeekDays(from, to,0);
            if (leaveType == LeaveType.ANNUAL)
            {
                leaveDays = to.Subtract(from).Days + 1 - holiday;
                leaveDays = leaveDays - Convert.ToDecimal(weekDays);
                if (day == LeaveDay.Half_Day)
                {
                    decimal checkWeekEnd = to.Subtract(from).Days + 1 - holiday;
                    double amt = HrHelper.GetHalf(from, to);
                    double notHalf = HrHelper.GetNotHalf(from, to);
                    bool isHalf = HrHelper.IsHalf(from, to);
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


        public decimal GetLeaveDays(string from, string to, string half, int hour)
        {

            decimal days = 0;

            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty( to))
            {


                var splitDatefrom = from.Split('/');
                DateTime f = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDatefrom[0]), Convert.ToInt32(splitDatefrom[1]), Convert.ToInt32(splitDatefrom[2]));

                var splitDateto = to.Split('/');
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
                    weekDays = HrHelper.GetWeekDays(f, t,0);
                }




                days = t.Subtract(f).Days + 1 - count;
                days = days - Convert.ToDecimal(weekDays);
                if (half == "1")
                {
                    decimal checkWeekEnd = t.Subtract(f).Days + 1 - count;
                    double amt = HrHelper.GetHalf(f, t);
                    double notHalf = HrHelper.GetNotHalf(f, t);
                    bool isHalf = HrHelper.IsHalf(f, t);
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
        public decimal GetBalance(/*int period,*/ int empId)
        {
            Total = 0;
            if (empId == 0)
            {
                empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            }


            var entitles = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired/*&& x.GlFiscalYearId == period*/);
            if (entitles != null)
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
        public string GetEmpId()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);

            if (entitle.Any())
            {
                var balance = entitle.Sum(x => x.Amount).ToString();

                return balance;
            }

            return "0";
        }
        public decimal GetAmount(/*int period,*/ int empId)
        {
            var balance = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);
            decimal totalBalance = 0;
            if (balance.Any())
            {
                //if (lt == LeaveType.ANNUAL.ToString())
                //{
                totalBalance = balance.Sum(x => x.Balance);

                //}
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
                totalBalance = balance.Sum(x => x.Balance);
            }
            return totalBalance;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
