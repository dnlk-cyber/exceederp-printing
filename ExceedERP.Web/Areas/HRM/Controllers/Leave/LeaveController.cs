using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
    public class LeaveController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        public static string FromAmharic { get; set; }
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();
            db.Configuration.ProxyCreationEnabled = false;
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            ViewData["AllEmployeeList"] =db.Person_Employee.AsQueryable();
        }
        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            db = new ExceedDbContext();
            var employees = inputs.GetEmployees(User.Identity.Name);

            return Json(employees.ToDataSourceResult(request));
        }
        public ActionResult Employee_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = inputs.GetEmployees(User.Identity.Name);
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
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
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
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin, HrmConstants.HRLeaveUser)]
        public ActionResult Leaves_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<int> employees = inputs.GetUserBranchAllEmployees(User.Identity.Name);
            IQueryable<Core.Domain.HRM.LeaveManagment.Leave> leaves = db.Leaves.Where(x => employees.Contains(x.EmployeeId));
            DataSourceResult result = leaves.ToDataSourceResult(request, leave => new
            {
                TakenLeaveId = leave.TakenLeaveId,
                EmployeeId = leave.EmployeeId,
                LeaveType = leave.LeaveType,
                Amount = leave.Amount,
                GlFiscalYearId = leave.GlFiscalYearId,
                From = leave.From,
                To = leave.To,
                OrgName = leave.OrgName,
                Before = leave.Before,
                After = leave.After,
                Remark = leave.Remark,
                EmployeeCode = leave.EmployeeCode,
                FromAmharic = leave.FromAmharic,
                ToAmharic = leave.ToAmharic,
                DateCreated = leave.DateCreated,
                LastModified = leave.LastModified,
                CreatedBy = leave.CreatedBy,
                ModifiedBy = leave.ModifiedBy,
                OrgBranchName = leave.OrgBranchName,
                Hours = leave.Hours,
                Day = leave.Day,
                MorningOrAfternoon = leave.MorningOrAfternoon,
                LeaveActionTypes = leave.LeaveActionTypes
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult Leaves_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.LeaveManagment.Leave leave)
        {
            ModelState.Clear();
            //int totalLeaveEntitle = Convert.ToInt32(db.LeavesEntitle.Where(x => x.EmployeeId == leave.EmployeeId).Select(x => x.TotlalAmount));

            int count = 0;
            double weekDays = 0;
            int holidays = 0;
            int currentBalance = 0;
            int askedBalance = 0;
           
            var leaveEntitleBalance = db.LeavesEntitle.Where(x=>x.EmployeeId == leave.EmployeeId && !x.IsExpired).Sum(x=>x.Balance);

            currentBalance = Convert.ToInt32((leaveEntitleBalance * leave.From.Month) / 12);

            askedBalance = (leave.To - leave.From).Days;

            if(askedBalance > currentBalance )
            {
                ModelState.AddModelError("","you can't take this amount of leave at this month");
            }

            #region validation
            if (leave.LeaveType == LeaveType.ANNUAL)
            {
                string messages = "";
                if (leave.EmployeeId == 0)
                {
                    messages = "Please select employee First!!";
                    ModelState.AddModelError("", messages);
                }


                var leavePlans =
                    db.LeavePlans
                        .FirstOrDefault(x => x.EmployeeId == leave.EmployeeId && x.GlFiscalYearId == leave.GlFiscalYearId);
                if (leavePlans != null)
                {
                    if (From.Date >= leavePlans.From.Date && To.Date <= leavePlans.To.Date)
                    {
                        leave.Amount = Convert.ToDecimal((To.Date - From.Date).TotalDays);
                        if (leave.Amount > leavePlans.LeaveAmount)
                        {
                            messages = "The requested amount is greater than your plan!!";
                            ModelState.AddModelError("", messages);

                        }

                    }
                    else if (From.Date < leavePlans.From.Date || To.Date > leavePlans.To.Date)
                    {
                        ModelState.AddModelError("", "Date you select is out of your Plan date range!!");
                        //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }


                }
                else
                {

                    messages = "You should set leave plan for this period!!";
                    ModelState.AddModelError("", messages);
                    //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }

            }
            #endregion

            var leavetaken = db.Leaves.FirstOrDefault(x => x.EmployeeId == leave.EmployeeId && x.GlFiscalYearId == leave.GlFiscalYearId);
            if (leave != null)
            {
                if (From.Date >= leave.From && From.Date <= leave.To)
                {
                    ModelState.AddModelError("", "Intial date over lap other leave taken date");
                }
                if (To.Date >= leave.From && To.Date <= leave.To)
                {
                    ModelState.AddModelError("", "End date over lap other leave taken date");
                }
            }

            if (ModelState.IsValid)
            {
                if (FromAmharic == null || ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date form and to values are mandatory!!");
                }
                else if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else
                {
                    var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == leave.GlFiscalYearId).ToList();
                    if (holidayList.Any())
                    {
                        foreach (var h in holidayList)
                        {
                            if (h.Date >= From && h.Date <= To)
                            {
                                count++;
                            }
                        }
                    }
                    weekDays = GetWeekDays(From, To);

                    var entity = new Core.Domain.HRM.LeaveManagment.Leave
                    {
                        EmployeeId = leave.EmployeeId,
                        LeaveType = leave.LeaveType,
                        Amount = leave.Amount,
                        GlFiscalYearId = leave.GlFiscalYearId,
                        From = From,
                        To = To,
                        OrgName = leave.OrgName,
                        Before = leave.Before,
                        After = leave.After,
                        Remark = leave.Remark,
                        EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leave.EmployeeId).EmployeeCode,
                        FromAmharic = FromAmharic,
                        ToAmharic = ToAmharic,
                        DateCreated = leave.DateCreated,
                        LastModified = leave.LastModified,
                        CreatedBy = leave.CreatedBy,
                        ModifiedBy = leave.ModifiedBy,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        Hours = leave.Hours,
                        Day = leave.Day,
                        MorningOrAfternoon = leave.MorningOrAfternoon,
                    };
                    entity.From = From;
                    entity.To = To;

                    if (entity.LeaveType != LeaveType.ANNUAL)
                    {
                        entity.LeaveActionTypes = LeaveActionTypes.Without_Plan;
                    }
                    else
                    {
                        entity.LeaveActionTypes = leave.LeaveActionTypes;
                    }

                    if (entity.LeaveType == LeaveType.ANNUAL)
                    {
                        entity.Amount = To.Subtract(From).Days + 1 - count;
                        entity.Amount = entity.Amount - Convert.ToDecimal(weekDays);
                        if (leave.Day == LeaveDay.ግማሽ_ቀን)
                        {
                            decimal checkWeekEnd = To.Subtract(From).Days + 1 - count;
                            double amt = GetHalf(From, To);
                            double notHalf = GetNotHalf(From, To);
                            bool isHalf = IsHalf(From, To);
                            if (isHalf)
                            {
                                entity.Amount = (checkWeekEnd - Convert.ToDecimal(notHalf)) / 2;
                            }
                            else
                            {
                                entity.Amount = entity.Amount / 2;
                            }

                        }

                        decimal days = entity.Amount * 8;
                        decimal leaveDays = entity.Amount * entity.Hours;
                        entity.Amount = (days - leaveDays) / 8;



                    }
                    else
                    {
                        // entity.Amount = To.Subtract(From).Days;

                        entity.Amount = To.Subtract(From).Days + 1;

                        if (entity.Day == LeaveDay.ግማሽ_ቀን)
                        {
                          
                            entity.Amount = entity.Amount / 2;

                        }

                        db.Leaves.Add(entity);
                        db.SaveChanges();

                        leave.TakenLeaveId = entity.TakenLeaveId;

                    }
                    if(entity.LeaveType == LeaveType.SICK_LEAVE)
                    {
                        var sickLeave = db.SickLeaveSettings.FirstOrDefault();
                        if(sickLeave != null)
                        {
                            if(Convert.ToDouble(entity.Amount) >= sickLeave.CurrentValue)
                            {
                                var employee = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == entity.EmployeeId);
                                if(employee != null)
                                {
                                    employee.IsActive = false;

                                    db.Entry(employee).State = EntityState.Modified;
                                    db.SaveChanges();

                                    var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == employee.EmployeeId && x.isCurrent);
                                    if(placement != null)
                                    {
                                        placement.isCurrent = false;

                                        db.Entry(placement).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    if (entity.LeaveType == LeaveType.ANNUAL)
                    {

                        #region update balance


                        //=========================================================================

                        var totalSum = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leave.EmployeeId).ToList();

                        var totalEntitle = totalSum.Sum(x => x.Balance);
                        if (totalEntitle < entity.Amount)
                        {
                            ModelState.AddModelError("", "your request amount is greater than your balance");
                        }
                        else
                        {
                            db.Leaves.Add(entity);
                            db.SaveChanges();

                            leave.TakenLeaveId = entity.TakenLeaveId;


                            decimal dateAsked = entity.Amount;
                            decimal dateLeft = 0;
                            do
                            {

                                List<LeaveModel> leaveList = new List<LeaveModel>();
                                var yearLists = db.LeavesEntitle.Where(x => !x.IsExpired && x.EmployeeId == leave.EmployeeId).ToList();



                                foreach (var year in yearLists)
                                {
                                    var leavel = new LeaveModel();
                                    var gl = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == year.GlFiscalYearId);
                                    if (gl != null)
                                    {
                                        leavel.Year = gl.DateTo;
                                    }

                                    leaveList.Add(leavel);
                                }
                                var smallestDate = leaveList.Min(x => x.Year);
                                if (smallestDate != null)
                                {
                                    var id = db.GLFiscalYears.FirstOrDefault(x => x.DateTo.Year == smallestDate.Year);
                                    if (id != null)
                                    {
                                        var entiteledEmployee = db.LeavesEntitle.FirstOrDefault
                                            (x => x.EmployeeId == leave.EmployeeId && x.GlFiscalYearId == id.GlFiscalYearId && !x.IsExpired);
                                        if (entiteledEmployee != null)
                                        {
                                            if (entiteledEmployee.Balance < dateAsked)
                                            {
                                                dateLeft = dateAsked - entiteledEmployee.Balance;
                                                if (dateLeft >= 0)
                                                {
                                                    //if (id.Status == Core.Domain.Finance.GL.FiscalYearStatus.Close)
                                                    //{
                                                        entiteledEmployee.Balance = 0;
                                                        entiteledEmployee.IsExpired = true;

                                                        db.Entry(entiteledEmployee).State = EntityState.Modified;
                                                        db.SaveChanges();
                                                    //}
                                                    dateAsked = dateLeft;
                                                }
                                            }

                                            else
                                            {
                                                dateLeft = entiteledEmployee.Balance - dateAsked;
                                                if (dateLeft >= 0)
                                                {
                                                    entiteledEmployee.Balance = dateLeft;

                                                    if (dateLeft == 0)
                                                    {
                                                        //if (id.Status == Core.Domain.Finance.GL.FiscalYearStatus.Close)
                                                        //{
                                                            entiteledEmployee.IsExpired = true;
                                                        //}
                                                    }

                                                    db.Entry(entiteledEmployee).State = EntityState.Modified;
                                                    db.SaveChanges();

                                                    dateLeft = 0;
                                                }
                                            }
                                        }
                                    }
                                }


                            } while (dateLeft > 0);
                        }
                    }

                    #endregion

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if(person != null)
                    {
                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                      
                            UserHelper.OperationLog(Request.UserHostAddress, "Leave Taken", "Success", User.Identity.Name, "Leave Taken for "+ person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);

                        
                    }
                    
                    #endregion
                }

            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        public ActionResult Leaves_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.LeaveManagment.Leave leave)
        {
            int count = 0;
            double weekDays = 0;
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = leave.FromAmharic;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]),
                        Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = leave.ToAmharic;
                    var splitDate = FromAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]),
                        Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else
                {
                    var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == leave.GlFiscalYearId).ToList();
                    if (holidayList.Any())
                    {
                        foreach (var h in holidayList)
                        {
                            if (h.Date >= From && h.Date <= To)
                            {
                                count++;
                            }
                        }
                    }
                    weekDays = GetWeekDays(From, To);
                    var entity = new Core.Domain.HRM.LeaveManagment.Leave
                    {
                        TakenLeaveId = leave.TakenLeaveId,
                        EmployeeId = leave.EmployeeId,
                        LeaveType = leave.LeaveType,
                        Amount = leave.Amount,
                        GlFiscalYearId = leave.GlFiscalYearId,
                        From = From,
                        To = To,
                        OrgName = leave.OrgName,
                        Before = leave.Before,
                        After = leave.After,
                        Remark = leave.Remark,
                        EmployeeCode =
                            inputs
                                .GetEmployees()
                                .FirstOrDefault(x => x.PersonId == leave.EmployeeId)
                                .EmployeeCode,
                        FromAmharic = FromAmharic,
                        ToAmharic = ToAmharic,
                        DateCreated = leave.DateCreated,
                        LastModified = leave.LastModified,
                        CreatedBy = leave.CreatedBy,
                        ModifiedBy = leave.ModifiedBy,
                        OrgBranchName = leave.OrgBranchName,
                        Day = leave.Day,
                        MorningOrAfternoon = leave.MorningOrAfternoon,
                    };


                    if (entity.LeaveType != LeaveType.ANNUAL)
                    {
                        entity.LeaveActionTypes = LeaveActionTypes.Without_Plan;
                    }
                    else
                    {
                        entity.LeaveActionTypes = leave.LeaveActionTypes;
                    }

                    if (entity.LeaveType == LeaveType.SICK_LEAVE)
                    {
                        var sickLeave = db.SickLeaveSettings.FirstOrDefault();
                        if (sickLeave != null)
                        {
                            if (Convert.ToDouble(entity.Amount) >= sickLeave.CurrentValue)
                            {
                                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                                if (employee != null)
                                {
                                    employee.IsActive = false;

                                    db.Entry(employee).State = EntityState.Modified;
                                    db.SaveChanges();

                                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.isCurrent);
                                    if (placement != null)
                                    {
                                        placement.isCurrent = false;

                                        db.Entry(placement).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    if (entity.LeaveType == LeaveType.ANNUAL)
                    {
                        entity.Amount = To.Subtract(From).Days + 1 - count;
                        entity.Amount = entity.Amount - Convert.ToDecimal(weekDays);
                        if (leave.Day == LeaveDay.ግማሽ_ቀን)
                        {
                            entity.Amount = entity.Amount / 2;
                        }

                        decimal days = entity.Amount * 8;
                        decimal leaveDays = entity.Amount * entity.Hours;
                        entity.Amount = (days - leaveDays) / 8;
                    }
                    else
                    {
                        entity.Amount = To.Subtract(From).Days;
                    }


                    db.Leaves.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();


                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Taken", "Success", User.Identity.Name, "Leave Taken of " +person.EmployeeCode+" is Updated By  " + User.Identity.Name,Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
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
                            else
                            {
                                noOfWeek = 0;
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
        public ActionResult UpdateLeaveTaken()
        {
            var requests = db.LeaveRequest.Where(x=>x.IsOnlineApproved).ToList();
            foreach (var request in requests)
            {
                var leaves = db.Leaves.Where(x => x.From == request.From && x.To == request.To);
                if (!leaves.Any())
                {
                    int count = 0;
                    double weekDays = 0;
                    var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == request.GlFiscalYearId).ToList();
                    if (holidayList.Any())
                    {
                        foreach (var h in holidayList)
                        {
                            if (h.Date >= request.From && h.Date <= request.To)
                            {
                                count++;
                            }
                        }
                    }
                    weekDays = GetWeekDays(request.From, request.To);
                    var entity = new Core.Domain.HRM.LeaveManagment.Leave
                    {
                        EmployeeId = request.EmployeeId,
                        LeaveType = request.Type,
                        Amount = request.LeaveDays,
                        GlFiscalYearId = request.GlFiscalYearId,
                        From = request.From,
                        To = request.To,
                        FromAmharic = request.FromAmharic,
                        ToAmharic = request.ToAmharic,
                        OrgName = null,
                        Before = null,
                        After = null,
                        Remark = request.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = null,
                        CreatedBy = User.Identity.Name,
                        EmployeeCode = request.EmployeeCode,
                        ModifiedBy = null,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        RequestId = request.LeaveRequestId
                    };

                    db.Leaves.Add(entity);
                    db.SaveChanges();

                }
            }
            return View("Success");
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
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult Leaves_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.LeaveManagment.Leave leave)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.HRM.LeaveManagment.Leave
                {
                    TakenLeaveId = leave.TakenLeaveId,
                    EmployeeId = leave.EmployeeId,
                    LeaveType = leave.LeaveType,
                    Amount = leave.Amount,
                    GlFiscalYearId = leave.GlFiscalYearId,
                    From = leave.From,
                    To = leave.To,
                    OrgName = leave.OrgName,
                    Before = leave.Before,
                    After = leave.After,
                    Remark = leave.Remark,
                    EmployeeCode = leave.EmployeeCode,
                    FromAmharic = leave.FromAmharic,
                    ToAmharic = leave.ToAmharic,
                    DateCreated = leave.DateCreated,
                    LastModified = leave.LastModified,
                    CreatedBy = leave.CreatedBy,
                    ModifiedBy = leave.ModifiedBy,
                    OrgBranchName = leave.OrgBranchName,
                    LeaveActionTypes = leave.LeaveActionTypes
            };
                #region update balance

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Leave Taken " + leave.EmployeeCode);
                #endregion
                var entiteledEmployee = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == leave.EmployeeId && x.GlFiscalYearId == leave.GlFiscalYearId);
                if (entiteledEmployee != null)
                {
                    decimal totalAmountField = entiteledEmployee.TotlalAmount;
                    decimal totalEntiteleft = totalAmountField + leave.Amount;
                    entiteledEmployee.TotlalAmount = totalEntiteleft;

                    // db.LeavesEntitle.Attach(entiteledEmployee);
                    db.Entry(entiteledEmployee).State = EntityState.Modified;
                    db.SaveChanges();

                }



                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave of employee" + leave.EmployeeCode);
                #endregion
                db.Leaves.Attach(entity);
                db.Leaves.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
