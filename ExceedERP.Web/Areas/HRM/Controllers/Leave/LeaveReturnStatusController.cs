using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    public class LeaveReturnStatusController : BaseController
    {
        // GET: HRM/LeaveReturn
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
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();

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
                var list = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeId).ToList();

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
                if (leave.Type != LeaveType.ANNUAL)
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
                            weekDays = GetWeekDays(leave.From, leave.To);
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
        public ActionResult LeaveReturnStatus_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //List<int> employeeids = EmployeeHelper.GetSubordinates(employeeid);
            //employeeids.Remove(employeeid);
            //IQueryable<LeaveRequest> leaverequest = db.LeaveRequest.Where(x => employeeids.Contains(x.EmployeeId)).OrderBy(x => x.RequestDate);
            //IQueryable<LeaveRequest> leaverequest = db.LeaveRequest.Where(x => x.EmployeeId == employeeid || x.RequesterId == employeeid).OrderBy(x => x.RequestDate);
            IQueryable<LeaveReturn> leaveReturns = db.LeaveReturns.Where(x => x.IsDepartmentOnlineApproved||x.IsOnlineApproved );

            DataSourceResult result = leaveReturns.ToDataSourceResult(request, leaveReturn => new
            {
                LeaveReturnId = leaveReturn.LeaveReturnId,
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

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveReturnStatus_Create([DataSourceRequest]DataSourceRequest request, LeaveReturn leaveReturn)
        {
            ModelState.Clear();
            if (leaveReturn.FromAm != null)
            {
                var splitDateFrom = leaveReturn.FromAm.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]), Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));

            }
            if (leaveReturn.ToAm != null)
            {
                var splitDateTo = leaveReturn.ToAm.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

            }

            if (ModelState.IsValid)
            {

                var entity = new LeaveReturn
                {
                    EmployeeId = leaveReturn.EmployeeId,
                    GlFiscalYearId = leaveReturn.GlFiscalYearId,
                    Amount = leaveReturn.Amount,
                    From = From,
                    To = To,
                    FromAm = leaveReturn.FromAm,
                    ToAm = leaveReturn.ToAm
                };

                db.LeaveReturns.Add(entity);
                db.SaveChanges();

            }

            return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveReturnStatus_Update([DataSourceRequest]DataSourceRequest request, LeaveReturn leaveReturn)
        {

            ModelState.Clear();
            if (leaveReturn.FromAm != null)
            {
                var splitDateFrom = leaveReturn.FromAm.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]), Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));

            }
            if (leaveReturn.ToAm != null)
            {
                var splitDateTo = leaveReturn.ToAm.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

            }
            if (ModelState.IsValid)
            {


                var entity = new LeaveReturn
                {
                    LeaveReturnId = leaveReturn.LeaveReturnId,
                    EmployeeId = leaveReturn.EmployeeId,
                    GlFiscalYearId = leaveReturn.GlFiscalYearId,
                    Amount = leaveReturn.Amount,
                    From = From,
                    To = To,
                    FromAm = leaveReturn.FromAm,
                    ToAm = leaveReturn.ToAm
                };



                db.LeaveReturns.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveReturnStatus_Destroy([DataSourceRequest]DataSourceRequest request, LeaveReturn leaveReturn)
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
                    ToAm = leaveReturn.ToAm
                };
                #region delete log

                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveReturn.EmployeeId);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Returened Leave of employee code  " + person.FullNameaEnglish);
                #endregion
                db.LeaveReturns.Attach(entity);
                db.LeaveReturns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leaveReturn }.ToDataSourceResult(request, ModelState));
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
