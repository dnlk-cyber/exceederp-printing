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
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Termination
{
    public class LeaveTerminationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/LeaveTermination
        public ActionResult Index()
        {
            var periodList = db.GLFiscalYears.ToList();
            ViewData["PeriodList"] = periodList;
            return View();
        }
        public ActionResult LeaveTermination_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Core.Domain.HRM.LeaveManagment.LeaveEntitle> leaves = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            DataSourceResult result = leaves.ToDataSourceResult(request, leaveEntitle => new
            {
                LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                EmployeeId = leaveEntitle.EmployeeId,
                Balance = leaveEntitle.Balance,
                GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                Amount = leaveEntitle.Amount,
                Remark = leaveEntitle.Remark,
                DateCreated = leaveEntitle.DateCreated,
                LastModified = leaveEntitle.LastModified,
                CreatedBy = leaveEntitle.CreatedBy,
                ModifiedBy = leaveEntitle.ModifiedBy,
                EmployeeCode = leaveEntitle.EmployeeCode,
                TotlalAmount = leaveEntitle.TotlalAmount,
                OrgBranchName = leaveEntitle.OrgBranchName,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult LeaveTermination_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.LeaveManagment.Leave leave,int id)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {


                //var entity = new Core.Domain.HRM.LeaveManagment.Leave
                //{
                //    EmployeeId = leave.EmployeeId,
                //    LeaveType = leave.LeaveType,
                //    Amount = leave.Amount,
                //    GlFiscalYearId = leave.GlFiscalYearId,
                //    From = From,
                //    To = To,
                //    OrgName = leave.OrgName,
                //    Before = leave.Before,
                //    After = leave.After,
                //    Remark = leave.Remark,
                //    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leave.EmployeeId).EmployeeCode,
                //    FromAmharic = FromAmharic,
                //    ToAmharic = ToAmharic,
                //    DateCreated = leave.DateCreated,
                //    LastModified = leave.LastModified,
                //    CreatedBy = leave.CreatedBy,
                //    ModifiedBy = leave.ModifiedBy,
                //    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                //    Hours = leave.Hours,
                //    Day = leave.Day,
                //    MorningOrAfternoon = leave.MorningOrAfternoon,
                //};




            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult LeaveTermination_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.LeaveManagment.Leave leave)
        {
           
            ModelState.Clear();
            if (ModelState.IsValid)
            {
               
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

        public ActionResult LeaveTermination_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.HRM.LeaveManagment.Leave leave)
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

        public bool Paid(int id, int gid)
        {
            var leave = db.LeavesEntitle.FirstOrDefault(x=>x.EmployeeId == id && x.GlFiscalYearId == gid);
            if(leave != null)
            {
                leave.IsExpired = true;

                //db.LeavesEntitle.Attach(leave);
                db.Entry(leave).State = EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            return false;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
