using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.Helpers.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using ExceedERP.Core.Domain.HRM.Attendance.VM;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web.Configuration;
using Newtonsoft.Json;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = HR_Attendance_User + "," + HR_Admin)]
    public class EmpPayrollNoDaysWorkedController : BaseController
    {
        readonly private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        private const string HR_Attendance_User = "HR_Attendance_User";
        private const string HR_Admin = "HR_Admin";
        readonly HRMInputs inputs = new HRMInputs();
        private static List<EmployeeIdView> List { get; set; }
        public ActionResult Index()
        {
            List = new List<EmployeeIdView>();
            GetPeriod();

            List = inputs.GetPlacmentEmployees(User.Identity.Name);
            //ViewData["EmployeeList"] = List.Where(x => x.IsActive);
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["CurrentYearPeriods"] = db.GLPeriods.ToList();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(i => new
            {
                FullNameAmharic = i.Code + "-" + i.AmharicName,
                FullNameaEnglish = i.Code + "-" + i.Name,
                FullName = i.Code + "-" + i.Name,
                EmployeeId = i.Id,
                ReceiverEmployeeId = i.Id,
                SenderEmployee = i.Id,
                ReciverEmployeeId = i.Id,
                HeadId = i.Id
            }).AsQueryable();
            //var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            //var listt = from p in periodList
            //            join g in db.GLPeriods
            //                on p.Name equals g.Name
            //            select (new
            //            {
            //                g
            //            });
            //ViewData["PeriodList"] = listt.Select(x => x.g).ToList(); ;
        }

        public JsonResult GetPeriod()
        {
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var listt = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            var sortedPeriods = listt.OrderByDescending(x => x.g.DateFrom);
            ViewData["PeriodList"] = sortedPeriods.Select(x => x.g).ToList(); ;

            return Json(sortedPeriods.Select(p => new { GLPeriodId = p.g.GLPeriodId, Name = p.g.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentYearPeriods()
        {
            List<GLPeriod> gLPeriods = new List<GLPeriod>();

            var periodList = db.GLPeriods.Where(x => x.Status == FiscalYearStatus.Open).AsNoTracking().ToList();

            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });



            return Json(list.OrderByDescending(x => x.g.DateFrom).Select(p => new
            {
                GLPeriodId = p.g.GLPeriodId,
                Name = p.g.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartments()
        {

            var structures = db.MainCompanyStructures.Where(x => x.IsActive).ToList();

            return Json(structures.Select(p => new
            {
                p.OrgStructureId,
                p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Summary(int period)
        //{
        //    double count = 0;
        //    var perioddetail = db.GLPeriods.Where(x => x.GLPeriodId == period).FirstOrDefault();
        //    if (perioddetail != null)
        //    {
        //        var checkRegistredAttendance =
        //            db.WeeklyAttendances.Where(x => x.Month == perioddetail.DateFrom.Month).FirstOrDefault();
        //        if (checkRegistredAttendance == null)
        //        {
        //            return new JavaScriptResult() { Script = "alert('" + "There is no registered Attendance!!" + "');" };
        //        }
        //        var checkexistancep = db.PayrollProcessViewModel.FirstOrDefault(x => x.GLPeriodId == period);
        //        if (checkexistancep != null)
        //        {

        //            // ModelState.AddModelError("", "You cannot create this data. It already processed!!");
        //            return new JavaScriptResult() { Script = "alert('" + "You cannot create this data. It already processed!!" + "');" };
        //        }
        //        foreach (var l in List)
        //        {
        //            var check = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.EmployeeId == l.PersonId && x.GLPeriodId == period);
        //            if (check != null)
        //            {
        //                db.EmpPayrollNoDaysWorkeds.Attach(check);
        //                db.EmpPayrollNoDaysWorkeds.Remove(check);
        //                db.SaveChanges();
        //            }
        //            count = GetAbscentDays(l.PersonId, perioddetail.DateFrom, perioddetail.DateTo);
        //            if (count > 0)
        //            {


        //                var entity = new EmpPayrollNoDaysWorked
        //                {
        //                    EmployeeId = l.PersonId,
        //                    GLPeriodId = period,
        //                    Days = Convert.ToDecimal(count),
        //                    Remark = null,
        //                    DateCreated = DateTime.Now,
        //                    LastModified = null,
        //                    CreatedBy = User.Identity.Name,
        //                    ModifiedBy = null,
        //                    Reason = null,
        //                    MeasureTaken = null,
        //                };
        //                db = new ExceedDbContext();
        //                db.EmpPayrollNoDaysWorkeds.Add(entity);
        //                db.SaveChanges();
        //            }

        //        }
        //    }
        //    return null;
        //}
        public JsonResult Summary(int period)
        {
            try
            {
                double count = 0;

                // Retrieve period details
                var periodDetail = db.GLPeriods.Find(period);
                if (periodDetail == null)
                {
                    return Json(new { success = false, message = "Period not found!" }, JsonRequestBehavior.AllowGet);
                }

                // Check for approved attendance
                var hasApprovedAttendance = db.AttendanceNews
                    .Any(x => x.Date >= periodDetail.DateFrom && x.Date <= periodDetail.DateTo && x.IsApproved);
                if (!hasApprovedAttendance)
                {
                    return Json(new { success = false, message = "There is no approved registered Attendance in this period!" }, JsonRequestBehavior.AllowGet);
                }

                // Check if payroll data already exists
                //if (db.PayrollProcessViewModel.Any(x => x.GLPeriodId == period))
                //{
                //    return Json(new { success = false, message = "You cannot create this data. The payroll is already processed!" }, JsonRequestBehavior.AllowGet);
                //}

                // Prepare changes for employees
                foreach (var employee in List)
                {
                    count = GetAbscentDays(employee.PersonId, periodDetail.DateFrom, periodDetail.DateTo);

                    if (count > 0)
                    {
                        var existingRecord = db.EmpPayrollNoDaysWorkeds
                            .FirstOrDefault(x => x.EmployeeId == employee.PersonId && x.GLPeriodId == period);

                        if (existingRecord != null)
                        {
                            existingRecord.Days = Convert.ToDecimal(count);
                            existingRecord.LastModified = DateTime.Now;
                            existingRecord.ModifiedBy = User.Identity.Name;
                            db.Entry(existingRecord).State = EntityState.Modified;
                        }
                        else
                        {
                            var newRecord = new EmpPayrollNoDaysWorked
                            {
                                EmployeeId = employee.PersonId,
                                GLPeriodId = period,
                                Days = Convert.ToDecimal(count),
                                Remark = null,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                Reason = null,
                                MeasureTaken = null
                            };
                            db.EmpPayrollNoDaysWorkeds.Add(newRecord);
                        }
                    }
                }

                // Save changes to database
                db.SaveChanges();

                return Json(new { success = true, message = "day absent generated successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is implemented
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        //private double GetAbscentDays(int empId, DateTime startDate, DateTime endDate)
        //{
        //    return db.AttendanceNews
        //        .Where(x => x.EmployeeId == empId && x.Date >= startDate && x.Date <= endDate && x.IsApproved)
        //        .Sum(x => (!x.IsPresentMorning && !x.IsPresentAfternoon) ? 1 :
        //                  (!x.IsPresentMorning || !x.IsPresentAfternoon) ? 0.5 : 0);
        //}

        public JsonResult RefreshNew(string period, string from, string to,DateTime?fromGorg, DateTime?toGorg)
        {
            try
            {
                if (string.IsNullOrEmpty(period) || !int.TryParse(period, out int periodId))
                {
                    return Json(new { success = false, message = "Please select a valid period!" }, JsonRequestBehavior.AllowGet);
                }

                var periodDetail = db.GLPeriods.Find(periodId);
                if (periodDetail == null)
                {
                    return Json(new { success = false, message = "Selected period not found!" }, JsonRequestBehavior.AllowGet);
                }
                // ✅ Validation: Ensure fromGorg and toGorg are within the period and valid
                if (fromGorg.HasValue && (fromGorg.Value < periodDetail.DateFrom || fromGorg.Value > periodDetail.DateTo))
                {
                    return Json(new { success = false, message = $"The 'From' date must be within the selected period ({periodDetail.DateFrom:dd.MM.yyyy} - {periodDetail.DateTo:dd.MM.yyyy})." }, JsonRequestBehavior.AllowGet);
                }

                if (toGorg.HasValue && (toGorg.Value < periodDetail.DateFrom || toGorg.Value > periodDetail.DateTo))
                {
                    return Json(new { success = false, message = $"The 'To' date must be within the selected period ({periodDetail.DateFrom:dd.MM.yyyy} - {periodDetail.DateTo:dd.MM.yyyy})." }, JsonRequestBehavior.AllowGet);
                }

                if (fromGorg.HasValue && toGorg.HasValue && fromGorg.Value > toGorg.Value)
                {
                    return Json(new { success = false, message = "The 'From' date cannot be later than the 'To' date." }, JsonRequestBehavior.AllowGet);
                }
                DateTime fromDate = fromGorg.HasValue && periodDetail.DateFrom <= fromGorg.Value? fromGorg.Value : periodDetail.DateFrom;
                DateTime toDate = toGorg.HasValue ? toGorg.Value : periodDetail.DateTo;

                // If both from and to dates are supplied, use them instead of period's default
                if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
                {
                    fromDate = HrHelper.ParseEthiopicDate(from);
                    toDate = HrHelper.ParseEthiopicDate(to);
                }

                // Make sure fromDate <= toDate
                if (fromDate > toDate)
                {
                    return Json(new { success = false, message = "From date cannot be after To date." }, JsonRequestBehavior.AllowGet);
                }

                if ((toDate - fromDate).Days > 31)
                {
                    return Json(new { success = false, message = "The selected date range must not exceed 31 days." }, JsonRequestBehavior.AllowGet);
                }

                // Check attendance
                bool hasApprovedAttendance = db.AttendanceNews.Any(x =>
                    x.Date >= fromDate && x.Date <= toDate && x.IsApproved);

                if (!hasApprovedAttendance)
                {
                    return Json(new { success = false, message = "There is no approved registered attendance in this period!" }, JsonRequestBehavior.AllowGet);
                }

                // Process attendance for each employee
                foreach (var employee in List) // Ensure `List` is defined and populated
                {
                    double count = GetAbscentDays(employee.PersonId, fromDate, toDate);
                    if (count <= 0) continue;

                    var existingRecord = db.EmpPayrollNoDaysWorkeds
                        .FirstOrDefault(x => x.EmployeeId == employee.PersonId && x.GLPeriodId == periodId);

                    if (existingRecord != null)
                    {
                        existingRecord.Days = Convert.ToDecimal(count);
                        existingRecord.LastModified = DateTime.Now;
                        existingRecord.ModifiedBy = User.Identity.Name;
                        db.Entry(existingRecord).State = EntityState.Modified;
                    }
                    else
                    {
                        db.EmpPayrollNoDaysWorkeds.Add(new EmpPayrollNoDaysWorked
                        {
                            EmployeeId = employee.PersonId,
                            GLPeriodId = periodId,
                            Days = Convert.ToDecimal(count),
                            Remark = null,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            Reason = null,
                            MeasureTaken = null
                        });
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Day absent generated successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RefreshNewCoppy(string period, string from, string to)
        {
            try
            {
                if (string.IsNullOrEmpty(period))
                {
                    return Json(new { success = false, message = "Please select a period !" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(period) && (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to)))
                {
                    return Json(new { success = false, message = "Please select a period or enter from and to dates!" }, JsonRequestBehavior.AllowGet);
                }

                DateTime fromDate, toDate;
                int periodId = 0;
                if (!string.IsNullOrEmpty(period))
                    int.TryParse(period, out periodId);

                var periodDetail = db.GLPeriods.Find(periodId);
                if (periodDetail != null)
                {
                    fromDate = periodDetail.DateFrom;
                    toDate = periodDetail.DateTo;
                }
                if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
                {
                    fromDate = HrHelper.ParseEthiopicDate(from);
                    toDate = HrHelper.ParseEthiopicDate(to);
                }
                else
                {
                    return Json(new { success = false, message = "Please select a period and  enter from and to dates!" }, JsonRequestBehavior.AllowGet);
                }

                bool hasApprovedAttendance = db.AttendanceNews.Any(x =>
                    x.Date >= fromDate && x.Date <= toDate && x.IsApproved);

                if (!hasApprovedAttendance)
                {
                    return Json(new { success = false, message = "There is no approved registered attendance in this period!" }, JsonRequestBehavior.AllowGet);
                }

                foreach (var employee in List) // Make sure List is declared and populated properly
                {
                    double count = GetAbscentDays(employee.PersonId, fromDate, toDate);
                    if (count <= 0) continue;

                    var existingRecord = db.EmpPayrollNoDaysWorkeds
                        .FirstOrDefault(x => x.EmployeeId == employee.PersonId && x.GLPeriodId == periodId);

                    if (existingRecord != null)
                    {
                        existingRecord.Days = Convert.ToDecimal(count);
                        existingRecord.LastModified = DateTime.Now;
                        existingRecord.ModifiedBy = User.Identity.Name;
                        db.Entry(existingRecord).State = EntityState.Modified;
                    }
                    else
                    {
                        db.EmpPayrollNoDaysWorkeds.Add(new EmpPayrollNoDaysWorked
                        {
                            EmployeeId = employee.PersonId,
                            GLPeriodId = periodId,
                            Days = Convert.ToDecimal(count),
                            Remark = null,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            Reason = null,
                            MeasureTaken = null
                        });
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Day absent generated successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RefreshNewExis(int period, string from, string to)
        {
            try
            {
                double count = 0;
                DateTime fromDate = DateTime.Now;
                DateTime toDate = DateTime.Now;
                if (period == 0
                    && (string.IsNullOrEmpty(from)
                    || string.IsNullOrEmpty(to)))
                {
                    return Json(new { success = false, message = " please select Period or from and to dates!" }, JsonRequestBehavior.AllowGet);

                }
                // Retrieve period details
                var periodDetail = db.GLPeriods.Find(period);
                if (periodDetail == null
                    && (string.IsNullOrEmpty(from)
                    || string.IsNullOrEmpty(to)))
                {
                    return Json(new { success = false, message = "Period not found!" }, JsonRequestBehavior.AllowGet);
                }
                if (periodDetail == null
                    && !string.IsNullOrEmpty(from)
                   && !string.IsNullOrEmpty(to))
                {
                    fromDate = HrHelper.ParseEthiopicDate(from);
                    toDate = HrHelper.ParseEthiopicDate(to);
                    return Json(new { success = false, message = "Period not found!" }, JsonRequestBehavior.AllowGet);
                }
                else if (periodDetail != null)
                {
                    fromDate = periodDetail.DateFrom;
                    toDate = periodDetail.DateTo;
                }

                // Check for approved attendance
                var hasApprovedAttendance = db.AttendanceNews
                    .Any(x => x.Date >= fromDate && x.Date <= toDate && x.IsApproved);
                if (!hasApprovedAttendance)
                {
                    return Json(new { success = false, message = "There is no approved registered Attendance in this period!" }, JsonRequestBehavior.AllowGet);
                }


                foreach (var employee in List)
                {
                    count = GetAbscentDays(employee.PersonId, periodDetail.DateFrom, periodDetail.DateTo);

                    if (count > 0)
                    {
                        var existingRecord = db.EmpPayrollNoDaysWorkeds
                            .FirstOrDefault(x => x.EmployeeId == employee.PersonId && x.GLPeriodId == period);

                        if (existingRecord != null)
                        {
                            existingRecord.Days = Convert.ToDecimal(count);
                            existingRecord.LastModified = DateTime.Now;
                            existingRecord.ModifiedBy = User.Identity.Name;
                            db.Entry(existingRecord).State = EntityState.Modified;
                        }
                        else
                        {
                            var newRecord = new EmpPayrollNoDaysWorked
                            {
                                EmployeeId = employee.PersonId,
                                GLPeriodId = period,
                                Days = Convert.ToDecimal(count),
                                Remark = null,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                Reason = null,
                                MeasureTaken = null
                            };
                            db.EmpPayrollNoDaysWorkeds.Add(newRecord);
                        }
                    }
                }

                // Save changes to database
                db.SaveChanges();

                return Json(new { success = true, message = "Day absent generated successfully!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is implemented
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult RefreshNewOld(int period)
        {
            var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == period).FirstOrDefault();
            if (checkexistancep != null)
            {
                return Json(new
                {
                    success = false,
                    message = "You cannot create this data. It already processed!!"
                }, JsonRequestBehavior.AllowGet);
            }

            var attendances = db.AttendanceNews.Where(x => x.GlPeriodId == period).Where(x => !x.IsPresentMorning || !x.IsPresentAfternoon).ToList();
            foreach (var att in attendances)
            {
                double day = 0;
                if ((!att.IsPresentMorning && !att.IsPresentAfternoon) || (att.MorningAttendance == TakeAttendance.Absent && att.AfternoonAttendance == TakeAttendance.Absent))
                {
                    day = 1;
                }
                else if (!att.IsPresentMorning
                    || !att.IsPresentAfternoon
                    || att.MorningAttendance == TakeAttendance.Absent
                    || att.AfternoonAttendance == TakeAttendance.Absent)
                {
                    day = 0.5;
                }

                var leave = db.LeaveRequest.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.Date && x.To >= att.Date && x.IsOnlineApproved);
                if (leave != null)
                {
                    if (leave.Day == LeaveDay.Full_Day)
                    {
                        day = day - 1;
                    }
                    else if (leave.Day == LeaveDay.Half_Day)
                    {
                        day = day - 0.5;
                    }
                }
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == att.EmployeeId);
                if (person != null)
                {
                    var absent = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.EmployeeId == person.EmployeeId && x.GLPeriodId == att.GlPeriodId);
                    if (absent != null)
                    {
                        absent.Days = Convert.ToDecimal(day);
                        absent.ModifiedBy = User.Identity.Name;
                        absent.LastModified = DateTime.Now;
                        db.Entry(absent).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var entity = new EmpPayrollNoDaysWorked
                        {
                            EmployeeId = person.EmployeeId,
                            GLPeriodId = att.GlPeriodId,
                            Days = Convert.ToDecimal(day),
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                        };
                        db.EmpPayrollNoDaysWorkeds.Add(entity);
                        db.SaveChanges();
                    }
                }
            }
            return Json(new
            {
                success = false,
                message = "Employee Absent days are successfully generated!!"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Refresh(int period)
        {
            var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == period).FirstOrDefault();
            if (checkexistancep != null)
            {
                //ModelState.AddModelError("", "You cannot create this data. It already processed!!");
                var error = "You cannot create this data. It already processed!!";
                return new JavaScriptResult() { Script = "alert('" + error + "');" };
            }
            string late = "";
            double lateNo = 0;
            double absentNo = 0;
            decimal totalNo = 0;

            var weekeds = db.Weekends.Select(x => x.Name.ToString()).ToList();
            var weekAttendances = db.WeeklyAttendances.ToList();
            var employees = db.Person_Employee.ToList();
            int month = 0;
            //////////////////////////////////////////////////////////////////////////////////

            var employeeListss = db.Placements.Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
            List<string> emloyeeList = employeeListss.Select(x => x.EmployeeId.ToString()).ToList();//GetUserBranchEmployees(Convert.ToInt32(dept), branhList);

            var payroll = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == period);
            month = payroll.DateFrom.Month;
            var weeklyAttendance = db.WeeklyAttendances.Where(x => x.Month == month);


            foreach (var e in emloyeeList)
            {
                int id = 0;
                var code = "";
                var name = "";
                //EmployeeInfo = new AttendanceList();
                //EmployeeInfo.AttendanceDetails = new List<AttendanceDetail>();
                var employee = employees.FirstOrDefault(x => x.EmployeeId.ToString() == e);
                if (employee != null)
                {
                    id = employee.EmployeeId;
                    code = employee.EmployeeCode;
                    name = employee.FirstName + " " + employee.MiddleName + " " +
                                           employee.LastName;

                    /// don't forget filter by period
                    var lateDays = db.LateComingSummaries.Where(x => x.EmployeeId == employee.EmployeeId && x.GLPeriodId == period);

                    if (lateDays.Any())
                    {
                        var total = lateDays.Sum(x => x.TotalDays);

                        lateNo = Convert.ToDouble(total);
                        late = total.ToString();
                    }
                }

                var employeeAttendance = weeklyAttendance.Where(x => x.EmployeeId.ToString() == e).ToList();
                for (int i = 1; i < 5; i++)
                {

                    {
                        var week = i;
                        if (i >= 4)
                        {
                            week = 4;

                            if (employeeAttendance.Count(x => (!x.DayFive) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayFiveAN) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (employeeAttendance.Count(x => (!x.DayFour) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayFourAN) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (employeeAttendance.Count(x => (!x.DayThree) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayThreeAN) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (employeeAttendance.Count(x => (!x.DayTwo) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayTwoAN) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (!weekeds.Contains("SUNDAY"))
                            {
                                if (employeeAttendance.Count(x => (!x.DayOne) && x.Week >= 4) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                                if (employeeAttendance.Count(x => (!x.DayOneAN) && x.Week >= 4) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                            }
                            if (employeeAttendance.Count(x => (!x.DaySix) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DaySixAN) && x.Week >= 4) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (!weekeds.Contains("SATURDAY"))
                            {
                                if (employeeAttendance.Count(x => (!x.DaySeven) && x.Week >= 4) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                                if (employeeAttendance.Count(x => (!x.DaySevenAN) && x.Week >= 4) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                            }
                        }
                        else
                        {
                            week = i;

                            if (employeeAttendance.Count(x => (!x.DayFive) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayFiveAN) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (employeeAttendance.Count(x => (!x.DayFour) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayFourAN) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (employeeAttendance.Count(x => (!x.DayThree) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayThreeAN) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (employeeAttendance.Count(x => (!x.DayTwo) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DayTwoAN) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }

                            if (!weekeds.Contains("SUNDAY"))
                            {
                                if (employeeAttendance.Count(x => (!x.DayOne) && x.Week == i) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                                if (employeeAttendance.Count(x => (!x.DayOneAN) && x.Week == i) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                            }
                            if (employeeAttendance.Count(x => (!x.DaySix) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (employeeAttendance.Count(x => (!x.DaySixAN) && x.Week == i) == 1)
                            {
                                absentNo = absentNo + 0.5;
                            }
                            if (!weekeds.Contains("SATURDAY"))
                            {
                                if (employeeAttendance.Count(x => (!x.DaySeven) && x.Week == i) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                                if (employeeAttendance.Count(x => (!x.DaySevenAN) && x.Week == i) == 1)
                                {
                                    absentNo = absentNo + 0.5;
                                }
                            }
                        }
                    }
                }

                totalNo = Convert.ToDecimal(lateNo + absentNo);


                var entity = new EmpPayrollNoDaysWorked
                {
                    EmployeeId = id,
                    GLPeriodId = period,
                    Days = totalNo,//late.Days,
                                   //Remark = lat.Remark,
                    DateCreated = DateTime.Now,
                    //LastModified = late.LastModified,
                    CreatedBy = User.Identity.Name,
                    // ModifiedBy = late.ModifiedBy,
                    // Reason = late.Reason,
                    // MeasureTaken = late.MeasureTaken,
                };

                var checkExistance = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId && x.GLPeriodId == entity.GLPeriodId);
                if (checkExistance != null)
                {
                    checkExistance.Days = totalNo;
                    checkExistance.ModifiedBy = User.Identity.Name;
                    checkExistance.LastModified = DateTime.Now;

                    // db.EmpPayrollNoDaysWorkeds.Attach(entity);
                    db.Entry(checkExistance).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (entity.Days > 0)
                {
                    db.EmpPayrollNoDaysWorkeds.Add(entity);
                    db.SaveChanges();
                }

                late = "";
                absentNo = 0;
                lateNo = 0;

            }

            ////////////////////////////////////////////////////////////////

            //var lates = db.LateComingSummaries.ToList();

            //foreach (var lat in lates)
            //{
            //    var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == lat.GLPeriodId).FirstOrDefault();
            //    if (checkexistancep == null)
            //    {

            //        // ModelState.AddModelError("", "You cannot create this data. It already processed!!");

            //        var total = lates.Where(x => x.EmployeeId == lat.EmployeeId).Sum(x => x.TotalDays);
            //        // EmployeeInfo.LateDays = total.ToString();//lateDays.ToString();
            //        var PP = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.EmployeeId == lat.EmployeeId && x.GLPeriodId == lat.GLPeriodId);
            //        if (PP != null)
            //        {
            //            var entity = new EmpPayrollNoDaysWorked
            //            {
            //                EmployeeId = lat.EmployeeId,
            //                GLPeriodId = lat.GLPeriodId,
            //                Days = total,//late.Days,
            //                Remark = lat.Remark,
            //                DateCreated = DateTime.Now,
            //                //LastModified = late.LastModified,
            //                CreatedBy = User.Identity.Name,
            //                // ModifiedBy = late.ModifiedBy,
            //                // Reason = late.Reason,
            //                // MeasureTaken = late.MeasureTaken,
            //            };

            //            db.EmpPayrollNoDaysWorkeds.Add(entity);
            //            db.SaveChanges();

            //        }

            //    }

            //}

            var message = "Successfull";
            return new JavaScriptResult() { Script = "alert('" + message + "');" };
            //return RedirectToAction("Index");

        }
        //private double GetAbscentDays(int empId, DateTime startDate, DateTime endDate)
        //{
        //    List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
        //    double count = 0;
        //    var weeklyAttendances = db.WeeklyAttendances.Where(x => x.EmployeeId == empId && x.Year == DateTime.Now.Year && employees.Contains(x.EmployeeId.ToString())).ToList();
        //    foreach (var w in weeklyAttendances)
        //    {
        //        if (startDate <= w.DayOneDate && endDate >= w.DayOneDate)
        //        {
        //            if (!w.DayOne)
        //            {
        //                count = count + 0.5;
        //            }
        //            if (!w.DayOneAN)
        //            {
        //                count = count + 0.5;
        //            }
        //        }
        //        if (startDate <= w.DayTwoDate && endDate >= w.DayTwoDate)
        //        {
        //            if (!w.DayTwo)
        //            {
        //                count = count + 0.5;
        //            }
        //            if (!w.DayTwoAN)
        //            {
        //                count = count + 0.5;
        //            }
        //        }
        //        if (startDate <= w.DayThreeDate && endDate >= w.DayThreeDate)
        //        {
        //            if (!w.DayThree)
        //            {
        //                count = count + 0.5;
        //            }
        //            if (!w.DayThreeAN)
        //            {
        //                count = count + 0.5;
        //            }
        //        }
        //        if (startDate <= w.DayFourDate && endDate >= w.DayFourDate)
        //        {
        //            if (!w.DayFour)
        //            {
        //                count = count + 0.5;
        //            }
        //            if (!w.DayFourAN)
        //            {
        //                count = count + 0.5;
        //            }
        //        }
        //        if (startDate <= w.DayFiveDate && endDate >= w.DayFiveDate)
        //        {
        //            if (!w.DayFive)
        //            {
        //                count = count + 0.5;
        //            }
        //            if (!w.DayFiveAN)
        //            {
        //                count = count + 0.5;
        //            }
        //        }
        //        if (startDate <= w.DaySixDate && endDate >= w.DaySixDate)
        //        {
        //            if (!w.DaySix)
        //            {
        //                count = count + 0.5;
        //            }
        //            if (!w.DaySixAN)
        //            {
        //                count = count + 0.5;
        //            }
        //        }
        //        if (startDate <= w.DaySevenDate && endDate >= w.DaySevenDate)
        //        {
        //            if (!w.DaySeven)
        //            {
        //                count = count + 0.5;
        //            }
        //            if (!w.DaySevenAN)
        //            {
        //                count = count + 0.5;
        //            }
        //        }
        //    }
        //    return count;
        //}
        private double GetAbscentDays(int empId, DateTime startDate, DateTime endDate)
        {
            double count = 0;
            var weeklyAttendances = db.AttendanceNews.Where(x => x.EmployeeId == empId && x.Date >= startDate && x.Date <= endDate && x.IsApproved).ToList();
            foreach (var w in weeklyAttendances)
            {
                if (w.MorningAttendance == TakeAttendance.Absent && w.AfternoonAttendance == TakeAttendance.Absent)
                {


                    count += 1;


                }
                else if (w.MorningAttendance == TakeAttendance.Absent || w.AfternoonAttendance == TakeAttendance.Absent)
                {


                    count += 0.5;


                }

            }
            return count;
        }


        [Authorize(Roles = HR_Attendance_User + "," + HR_Admin)]
        public ActionResult EmpPayrollNoDaysWorkeds_Read([DataSourceRequest] DataSourceRequest request)
        {
            db.Database.CommandTimeout = 7200;//added
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<EmpPayrollNoDaysWorked> emppayrollnodaysworkeds = db.EmpPayrollNoDaysWorkeds.Where(x => employees.Contains(x.EmployeeId) /*&& x.Days > 0*/);
            DataSourceResult result = emppayrollnodaysworkeds.ToDataSourceResult(request, empPayrollNoDaysWorked => new
            {
                EmpPayrollNoDaysWorkedId = empPayrollNoDaysWorked.EmpPayrollNoDaysWorkedId,
                EmployeeId = empPayrollNoDaysWorked.EmployeeId,
                GLPeriodId = empPayrollNoDaysWorked.GLPeriodId,
                Days = empPayrollNoDaysWorked.Days,
                Remark = empPayrollNoDaysWorked.Remark,
                Reason = empPayrollNoDaysWorked.Reason,
                MeasureTaken = empPayrollNoDaysWorked.MeasureTaken,
                DateCreated = empPayrollNoDaysWorked.DateCreated,
                LastModified = empPayrollNoDaysWorked.LastModified,
                CreatedBy = empPayrollNoDaysWorked.CreatedBy,

                ModifiedBy = empPayrollNoDaysWorked.ModifiedBy,
            });

            return Json(result);
        }
        [Authorize(Roles = HR_Attendance_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpPayrollNoDaysWorkeds_Create([DataSourceRequest] DataSourceRequest request, EmpPayrollNoDaysWorked empPayrollNoDaysWorked)
        {
            ModelState.Clear();
            var checkEmp = db.EmpPayrollNoDaysWorkeds
                .Where(x => x.EmployeeId == empPayrollNoDaysWorked.EmployeeId && x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId).FirstOrDefault();
            if (checkEmp != null)
            {
                ModelState.AddModelError("", "this employee is already recoreded");
                return Json(new[] { empPayrollNoDaysWorked }.ToDataSourceResult(request, ModelState));

            }


            var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId);

            decimal totalDays = 30; // Default

            if (period != null)
            {
                totalDays = (decimal)(period.DateTo - period.DateFrom).TotalDays + 1;
            }

            if (empPayrollNoDaysWorked.Days > totalDays)
            {
                ModelState.AddModelError("", $"Days absent should be ≤ {totalDays}");
                return Json(new[] { empPayrollNoDaysWorked }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new EmpPayrollNoDaysWorked
                {
                    EmployeeId = empPayrollNoDaysWorked.EmployeeId,
                    GLPeriodId = empPayrollNoDaysWorked.GLPeriodId,
                    Days = empPayrollNoDaysWorked.Days,
                    Remark = empPayrollNoDaysWorked.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = empPayrollNoDaysWorked.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = empPayrollNoDaysWorked.ModifiedBy,
                    Reason = empPayrollNoDaysWorked.Reason,
                    MeasureTaken = empPayrollNoDaysWorked.MeasureTaken,
                };

                db.EmpPayrollNoDaysWorkeds.Add(entity);
                db.SaveChanges();
                empPayrollNoDaysWorked.EmpPayrollNoDaysWorkedId = entity.EmpPayrollNoDaysWorkedId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll employee work days", "Success", User.Identity.Name, $"added employee work days for employee  {entity.EmployeeId} of period {entity.GLPeriodId} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { empPayrollNoDaysWorked }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Attendance_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpPayrollNoDaysWorkeds_Update([DataSourceRequest] DataSourceRequest request, EmpPayrollNoDaysWorked empPayrollNoDaysWorked)
        {
            ModelState.Clear();

            var checkEmp = db.EmpPayrollNoDaysWorkeds
                .Where(x => x.EmpPayrollNoDaysWorkedId != empPayrollNoDaysWorked.EmpPayrollNoDaysWorkedId)
              .Where(x => x.EmployeeId == empPayrollNoDaysWorked.EmployeeId && x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId).FirstOrDefault();
            if (checkEmp != null)
            {
                ModelState.AddModelError("", "this employee is already recoreded");
                return Json(new[] { empPayrollNoDaysWorked }.ToDataSourceResult(request, ModelState));

            }

            var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId);

            decimal totalDays = 30; // Default
            if (period != null)
            {
                totalDays = (decimal)(period.DateTo - period.DateFrom).TotalDays + 1;
            }

            if (empPayrollNoDaysWorked.Days > totalDays)
            {
                ModelState.AddModelError("", $"Days absent should be ≤ {totalDays}");
                return Json(new[] { empPayrollNoDaysWorked }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                var entity = new EmpPayrollNoDaysWorked
                {
                    EmpPayrollNoDaysWorkedId = empPayrollNoDaysWorked.EmpPayrollNoDaysWorkedId,
                    EmployeeId = empPayrollNoDaysWorked.EmployeeId,
                    GLPeriodId = empPayrollNoDaysWorked.GLPeriodId,
                    Days = empPayrollNoDaysWorked.Days,
                    Remark = empPayrollNoDaysWorked.Remark,
                    DateCreated = empPayrollNoDaysWorked.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = empPayrollNoDaysWorked.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    Reason = empPayrollNoDaysWorked.Reason,
                    MeasureTaken = empPayrollNoDaysWorked.MeasureTaken,
                };

                db.EmpPayrollNoDaysWorkeds.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll employee work days", "Success", User.Identity.Name, $"updated employee work days for employee  {entity.EmployeeCode} of period {entity.GLPeriod.Name} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { empPayrollNoDaysWorked }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmpPayrollNoDaysWorkeds_Destroy([DataSourceRequest] DataSourceRequest request, EmpPayrollNoDaysWorked empPayrollNoDaysWorked)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == empPayrollNoDaysWorked.EmployeeId && x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId).FirstOrDefault();
                if (checkexistance != null)
                {

                    ModelState.AddModelError("", "You cannot delete this data. It already processed!!");
                }
                else
                {
                    var entity = new EmpPayrollNoDaysWorked
                    {
                        EmpPayrollNoDaysWorkedId = empPayrollNoDaysWorked.EmpPayrollNoDaysWorkedId,
                        EmployeeId = empPayrollNoDaysWorked.EmployeeId,
                        GLPeriodId = empPayrollNoDaysWorked.GLPeriodId,
                        Days = empPayrollNoDaysWorked.Days,
                        Remark = empPayrollNoDaysWorked.Remark,
                        DateCreated = empPayrollNoDaysWorked.DateCreated,
                        LastModified = empPayrollNoDaysWorked.LastModified,
                        CreatedBy = empPayrollNoDaysWorked.CreatedBy,
                        ModifiedBy = empPayrollNoDaysWorked.ModifiedBy,
                        Reason = empPayrollNoDaysWorked.Reason,
                        MeasureTaken = empPayrollNoDaysWorked.MeasureTaken,
                    };
                    #region delete log
                    UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee day abcent information of employee " + empPayrollNoDaysWorked.EmployeeCode + " and period " + empPayrollNoDaysWorked.GLPeriodId, Modules.Payroll);
                    #endregion
                    db.EmpPayrollNoDaysWorkeds.Attach(entity);
                    db.EmpPayrollNoDaysWorkeds.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { empPayrollNoDaysWorked }.ToDataSourceResult(request, ModelState));
        }
        public List<string> GetUserBranchEmployees(int dept, List<string> branchId = null)
        {
            var placments = from p in db.Placements
                            join
                                e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where p.isCurrent && e.IsActive
                            select (new
                            {
                                EmployeeId = p.EmployeeId,
                                p.BranchId,
                                p.OrgStructureId
                            });
            if (branchId != null)
            {
                placments = placments.Where(x => branchId.Contains(x.BranchId.ToString()));
            }
            if (dept != 0)
            {

                placments = placments.Where(x => x.OrgStructureId == dept);
            }
            if (placments.Any())
            {
                var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
                return listOfEmployees;
            }
            return new List<string>();
        }

        public async Task<JsonResult> ImportBiometricData(string period, string from, string to, DateTime? fromGorg, DateTime? toGorg)
        {

            if (string.IsNullOrEmpty(period) || !int.TryParse(period, out int periodId))
            {
                return Json(new { success = false, message = "Please select a valid period!" }, JsonRequestBehavior.AllowGet);
            }

            var periodDetail = db.GLPeriods.Find(periodId);
            if (periodDetail == null)
            {
                return Json(new { success = false, message = "Selected period not found!" }, JsonRequestBehavior.AllowGet);
            }
            
            if (fromGorg.HasValue && toGorg.HasValue && fromGorg.Value > toGorg.Value)
            {
                return Json(new { success = false, message = "The 'From' date cannot be later than the 'To' date." }, JsonRequestBehavior.AllowGet);
            }
            DateTime fromDate = fromGorg.HasValue ? fromGorg.Value : periodDetail.DateFrom;
            DateTime toDate = toGorg.HasValue ? toGorg.Value : periodDetail.DateTo;

            // If both from and to dates are supplied, use them instead of period's default
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                fromDate = HrHelper.ParseEthiopicDate(from);
                toDate = HrHelper.ParseEthiopicDate(to);
            }

            // Make sure fromDate <= toDate
            if (fromDate > toDate)
            {
                return Json(new { success = false, message = "From date cannot be after To date." }, JsonRequestBehavior.AllowGet);
            }

            if ((toDate - fromDate).Days > 31)
            {
                return Json(new { success = false, message = "The selected date range must not exceed 31 days." }, JsonRequestBehavior.AllowGet);
            }


            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("http://localhost:55587/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var query = new Dictionary<string, string>()
                {
                    ["start_date"] = $"{fromDate.Year}-{fromDate.Month}-{fromDate.Day}",
                    ["end_date"] = $"{toDate.Year}-{toDate.Month}-{toDate.Day}",

                };

                //GET Method
                var uri = QueryHelpers.AddQueryString("http://172.16.3.10/integration/get_attendance/", query);
                try
                {
                    HttpResponseMessage response = await client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                        List<BiometricDataVM> data = await response.Content.ReadAsAsync<List<BiometricDataVM>>();

                        //var formattedData = new
                        //{
                        //    Message = $"absent day for {data.FirstOrDefault().employee_id} -->" + data.FirstOrDefault().absent_day,
                        //    Status = "NOT_OK"
                        //};
                        //return Json(formattedData, JsonRequestBehavior.AllowGet);
                        foreach (var d in data)
                        {

                            var employee = db.Person_Employee.Where(x => x.EmployeeCode == d.employee_id).FirstOrDefault();
                            var existingAttendance = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == employee.EmployeeId).Where(x => x.GLPeriodId == periodId).FirstOrDefault();
                            if (existingAttendance != null)
                            {
                                existingAttendance.Days = decimal.Parse(d.absent_day);
                                db.EmpPayrollNoDaysWorkeds.Attach(existingAttendance);
                                db.Entry(existingAttendance).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                var entity = new EmpPayrollNoDaysWorked
                                {
                                    EmployeeId = employee.EmployeeId,
                                    GLPeriodId = periodId,
                                    Days = decimal.Parse(d.absent_day),
                                    Remark = "",
                                    DateCreated = DateTime.Now,
                                    CreatedBy = User.Identity.Name,
                                };

                                db.EmpPayrollNoDaysWorkeds.Add(entity);
                                db.SaveChanges();
                            }


                        }
                    }
                    else
                    {
                        var formattedData = new
                        {
                            Message = "Internal server Error",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    var formattedData = new
                    {
                        Message = "Ex: " + ex.Message + "-" + ex.InnerException.Message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }


            }

            var formattedData1 = new
            {
                Message = "",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        //occ 
        //with token
        public async Task<JsonResult> ImportBiometricData22_old(int period)
        {
            var periodObj = db.GLPeriods.Find(period);
            if (periodObj == null)
            {
                var formattedData = new
                {
                    Message = "The period is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            try

            {
                string token = await GetJwtToken();
                string username = WebConfigurationManager.AppSettings["JwtUsername"];
                string password = WebConfigurationManager.AppSettings["JwtPassword"];
                string attendanceApiUrl = WebConfigurationManager.AppSettings["AttendanceApiUrl"];
                string nextPageUrl = $"{attendanceApiUrl}?start_date={periodObj.DateFrom.Year}-{periodObj.DateFrom.Month}-{periodObj.DateFrom.Day}&end_date={periodObj.DateTo.Year}-{periodObj.DateTo.Month}-{periodObj.DateTo.Day}&departments={0}";

                do
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var response = await client.GetAsync(nextPageUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var jsonData = JObject.Parse(data);

                            var attendanceData = jsonData["data"].ToObject<List<AttendanceRecord>>();
                            SaveAttendanceDataToDb(attendanceData, period, null, null);

                            nextPageUrl = jsonData["next"]?.ToString(); // Update nextPageUrl for the next iteration
                        }
                        else
                        {
                            var formattedData1 = new
                            {
                                Message = "Failed to retrieve attendance data",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData1, JsonRequestBehavior.AllowGet);
                        }
                    }
                } while (!string.IsNullOrEmpty(nextPageUrl));

                var formattedData = new
                {
                    Message = "Attendance data fetching complete.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var formattedData1 = new
                {
                    Message = ex.Message,
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);

            }



        }
        //occ
        //basic auth, worked
        public async Task<JsonResult> ImportBiometricData222_old(string period, string from, string to, DateTime? fromGorg, DateTime? toGorg)
        {
            if (string.IsNullOrEmpty(period) || !int.TryParse(period, out int periodId))
            {
                return Json(new { success = false, message = "Please select a valid period!" }, JsonRequestBehavior.AllowGet);
            }
            var periodDetail = db.GLPeriods.Find(periodId);
            if (periodDetail == null)
            {
                return Json(new { success = false, message = "Selected period not found!" }, JsonRequestBehavior.AllowGet);
            }
           
            if (fromGorg.HasValue && toGorg.HasValue && fromGorg.Value > toGorg.Value)
            {
                return Json(new { success = false, message = "The 'From' date cannot be later than the 'To' date." }, JsonRequestBehavior.AllowGet);
            }

            DateTime fromDate = fromGorg.HasValue ? fromGorg.Value : periodDetail.DateFrom;
            DateTime toDate = toGorg.HasValue ? toGorg.Value : periodDetail.DateTo;
            // If both from and to dates are supplied, use them instead of period's default
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                fromDate = HrHelper.ParseEthiopicDate(from);
                toDate = HrHelper.ParseEthiopicDate(to);
            }
            // Make sure fromDate <= toDate
            if (fromDate > toDate)
            {
                return Json(new { success = false, message = "From date cannot be after To date." }, JsonRequestBehavior.AllowGet);
            }
            if ((toDate - fromDate).Days > 31)
            {
                return Json(new { success = false, message = "The selected date range must not exceed 31 days." }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                string username = WebConfigurationManager.AppSettings["JwtUsername"];
                string password = WebConfigurationManager.AppSettings["JwtPassword"];
                string attendanceApiUrl = WebConfigurationManager.AppSettings["AttendanceApiUrl"];
                string attendanceDepApiUrl = WebConfigurationManager.AppSettings["AttendanceDepApiUrl"];
                string allDepartments = WebConfigurationManager.AppSettings["AllDepartments"];
                string nextPageUrl = $"{attendanceApiUrl}?start_date={fromDate.Year}-{fromDate.Month}-{fromDate.Day}&end_date={toDate.Year}-{toDate.Month}-{toDate.Day}&departments={allDepartments}";

                // Encode username and password in base64
                var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
                var departments = GetDepartmentListAsync(attendanceDepApiUrl).Result;
                do
                {
                    using (var client = new HttpClient())
                    {
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        };
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                        var response = await client.GetAsync(nextPageUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var jsonData = JObject.Parse(data);

                            // var attendanceData = jsonData["data"].ToObject<List<AttendanceRecord>>();
                            var attendanceData = new List<AttendanceRecord>();

                            foreach (var item in jsonData["data"])
                            {
                                var record = new AttendanceRecord();
                                record.EmpId = item["emp_id"].Value<string>();
                                record.EmpCode = item["emp_code"].Value<string>();

                                // Uncomment these fields one by one to check for errors
                                // record.FirstName = item["first_name"].Value<string>();
                                // record.LastName = item["last_name"]?.Value<string>();
                                // record.NickName = item["nick_name"]?.Value<string>();
                                // record.Gender = item["gender"]?.Value<string>();
                                // record.DeptName = item["dept_name"].Value<string>();
                                // record.DeptCode = item["dept_code"].Value<string>();
                                // record.PositionCode = item["position_code"]?.Value<string>();
                                // record.PositionName = item["position_name"]?.Value<string>();

                                record.NeedPresent = item["need_present"].Value<decimal?>();
                                record.Present = item["present"].Value<decimal?>();
                                record.Absence = item["absence"].Value<decimal?>();

                                // record.Holiday = item["holiday"]?.Value<decimal>(); // Adjust type if necessary
                                record.PresentRate = item["present_rate"]?.Value<string>() ?? "0";
                                record.AbsenceRate = item["absence_rate"]?.Value<string>() ?? "0";

                                attendanceData.Add(record);
                            }
                            SaveAttendanceDataToDb(attendanceData, periodId, fromDate, toDate);

                            nextPageUrl = jsonData["next"]?.ToString(); // Update nextPageUrl for the next iteration
                        }
                        else
                        {
                            var formattedData1 = new
                            {
                                Message = "Failed to retrieve attendance data",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData1, JsonRequestBehavior.AllowGet);
                        }
                    }
                } while (!string.IsNullOrEmpty(nextPageUrl));

                var formattedData = new
                {
                    Message = "Attendance data fetching complete.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var formattedData1 = new
                {
                    Message = ex.Message,
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }
        }
        //occ updated latest
        public async Task<JsonResult> ImportBiometricData2(string period, string from, string to, DateTime? fromGorg, DateTime? toGorg)
        {
            if (string.IsNullOrEmpty(period) || !int.TryParse(period, out int periodId))
            {
                return Json(new { success = false, message = "Please select a valid period!" }, JsonRequestBehavior.AllowGet);
            }

            var periodDetail = db.GLPeriods.Find(periodId);
            if (periodDetail == null)
            {
                return Json(new { success = false, message = "Selected period not found!" }, JsonRequestBehavior.AllowGet);
            }
          
            if (fromGorg.HasValue && toGorg.HasValue && fromGorg.Value > toGorg.Value)
            {
                return Json(new { success = false, message = "The 'From' date cannot be later than the 'To' date." }, JsonRequestBehavior.AllowGet);
            }
            DateTime fromDate = fromGorg.HasValue?fromGorg.Value: periodDetail.DateFrom;
            DateTime toDate = toGorg.HasValue?toGorg.Value: periodDetail.DateTo;

            // If both from and to dates are supplied, use them instead of period's default
            if (!string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(to))
            {
                fromDate = HrHelper.ParseEthiopicDate(from);
                toDate = HrHelper.ParseEthiopicDate(to);
            }

            // Make sure fromDate <= toDate
            if (fromDate > toDate)
            {
                return Json(new { success = false, message = "From date cannot be after To date." }, JsonRequestBehavior.AllowGet);
            }

            if ((toDate - fromDate).Days > 31)
            {
                return Json(new { success = false, message = "The selected date range must not exceed 31 days." }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                string username = WebConfigurationManager.AppSettings["JwtUsername"];
                string password = WebConfigurationManager.AppSettings["JwtPassword"];
                string attendanceApiUrl = WebConfigurationManager.AppSettings["AttendanceApiUrl"];
                string attendanceDepApiUrl = WebConfigurationManager.AppSettings["AttendanceDepApiUrl"];
                string allDepartments = WebConfigurationManager.AppSettings["AllDepartments"];
                //string nextPageUrl = $"{attendanceApiUrl}?start_date={fromDate.Year}-{fromDate.Month}-{fromDate.Day}&end_date={toDate.Year}-{toDate.Month}-{toDate.Day}&departments={allDepartments}";

                // Encode username and password in base64
                var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));

                var departments = await GetDepartmentListAsync(attendanceDepApiUrl);
                if (departments != null && departments.Any())
                {
                    var distinctDeps = departments.DistinctByField(x => x.dept_code).ToList();
                    foreach (var department in distinctDeps)
                    {
                        await FetchAndSaveAttendanceData(attendanceApiUrl, fromDate, toDate, authToken, periodId, department.dept_code);
                    }
                }
                else
                {
                    // Fallback to using allDepartments from config
                    await FetchAndSaveAttendanceData(attendanceApiUrl, fromDate, toDate, authToken, periodId, allDepartments);
                }

                var formattedData = new
                {
                    Message = "Attendance data fetching complete.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var formattedData1 = new
                {
                    Message = ex.Message + "\n" + ex.StackTrace,
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task FetchAndSaveAttendanceData(string attendanceApiUrl, DateTime fromDate, DateTime toDate, string authToken, int periodId, string department)
        {
            string nextPageUrl = $"{attendanceApiUrl}?start_date={fromDate.Year}-{fromDate.Month}-{fromDate.Day}&end_date={toDate.Year}-{toDate.Month}-{toDate.Day}&departments={department}";

            using (var client = new HttpClient { Timeout = TimeSpan.FromMinutes(2) })
            {
                //var settings = new JsonSerializerSettings
                //{
                //    NullValueHandling = NullValueHandling.Ignore
                //};
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);


                do
                {
                    const int maxRetries = 3;
                    for (int attempt = 1; attempt <= maxRetries; attempt++)
                    {
                        try
                        {
                            var response = await client.GetAsync(nextPageUrl);
                            response.EnsureSuccessStatusCode();

                            var data = await response.Content.ReadAsStringAsync();
                            var jsonData = JObject.Parse(data);

                            var attendanceData = new List<AttendanceRecord>();

                            foreach (var item in jsonData["data"])
                            {
                                try
                                {
                                    var record = new AttendanceRecord();
                                    record.EmpId = item["emp_id"].Value<string>();
                                    record.EmpCode = item["emp_code"].Value<string>();

                                    // Uncomment these fields one by one to check for errors
                                    // record.FirstName = item["first_name"].Value<string>();
                                    // record.LastName = item["last_name"]?.Value<string>();
                                    // record.NickName = item["nick_name"]?.Value<string>();
                                    // record.Gender = item["gender"]?.Value<string>();
                                    // record.DeptName = item["dept_name"].Value<string>();
                                    // record.DeptCode = item["dept_code"].Value<string>();
                                    // record.PositionCode = item["position_code"]?.Value<string>();
                                    // record.PositionName = item["position_name"]?.Value<string>();

                                    record.NeedPresent = item["need_present"].Value<decimal?>();
                                    record.Present = item["present"].Value<decimal?>();
                                    record.Absence = item["absence"].Value<decimal?>();

                                    // record.Holiday = item["holiday"]?.Value<decimal>(); // Adjust type if necessary
                                    record.PresentRate = item["present_rate"]?.Value<string>() ?? "0";
                                    record.AbsenceRate = item["absence_rate"]?.Value<string>() ?? "0";

                                    attendanceData.Add(record);
                                }
                                catch (Exception ex)
                                {
                                    //_logger.LogError(ex, "Error parsing attendance record: " + item?.ToString());
                                }
                            }
                            SaveAttendanceDataToDb(attendanceData, periodId, fromDate, toDate);
                            nextPageUrl = jsonData["next"]?.ToString();
                            break;
                        }
                        catch (HttpRequestException ex) when (attempt < maxRetries)
                        {
                            //_logger.LogWarning($"Retry {attempt} - Error: {ex.Message}");
                            await Task.Delay(2000 * attempt); // Exponential backoff
                        }
                    }

                } while (!string.IsNullOrEmpty(nextPageUrl));
            }

        }
        private void SaveAttendanceDataToDb(List<AttendanceRecord> attendanceData, int period, DateTime? fromDate, DateTime? toDate)
        {

            foreach (var data in attendanceData)
            {
                if (!string.IsNullOrEmpty(data.EmpCode)/*&& data.EmpCode == "OCC-002408"*/)
                {
                    var employee = db.Person_Employee.Where(x => x.EmployeeCode == data.EmpCode).FirstOrDefault();
                    if (employee != null)
                    {
                        decimal absenceDays = data.Absence != null ? (decimal)data.Absence : 0;
                        decimal fieldDays = HrHelper.GetFieldDays(employee.EmployeeId, period, fromDate, toDate);
                        decimal leaveDays = HrHelper.GetLeaveDays(employee.EmployeeId, period, fromDate, toDate);

                        decimal exceptionDays = 0;

                        var excepDays = db.AttendanceExceptions
                            .Where(x => x.EmployeeId == employee.EmployeeId)
                            .Where(x => x.GLPeriodId == period).AsNoTracking().ToList();

                        if (excepDays.Any())
                            exceptionDays = excepDays.Sum(g => (g.ToDate - g.Date).Days + 1);

                        absenceDays = absenceDays - (exceptionDays + leaveDays + fieldDays);
                        if (absenceDays < 0)
                            absenceDays = 0;

                        var existingAttendance = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == employee.EmployeeId).Where(x => x.GLPeriodId == period).FirstOrDefault();
                        if (existingAttendance != null)
                        {
                            existingAttendance.Days = absenceDays; /*/ 8;*/
                            db.EmpPayrollNoDaysWorkeds.Attach(existingAttendance);
                            db.Entry(existingAttendance).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            var entity = new EmpPayrollNoDaysWorked
                            {
                                EmployeeId = employee.EmployeeId,
                                GLPeriodId = period,
                                Days = absenceDays, /*/ 8,*/
                                Remark = "",
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                            };

                            db.EmpPayrollNoDaysWorkeds.Add(entity);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        //emp code does not exisit
                    }
                }

            }
        }

        private async Task<List<DepartmentDataVM>> GetDepartmentListAsync(string url)
        {
            var departments = new List<DepartmentDataVM>();
            using (var client = new HttpClient())
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                string username = WebConfigurationManager.AppSettings["JwtUsername"];
                string password = WebConfigurationManager.AppSettings["JwtPassword"];
                // Encode username and password in base64
                var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                try
                {
                    var response = await client.GetAsync(url).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var jsonData = JObject.Parse(data);


                        foreach (var item in jsonData["data"])
                        {
                            var record = new DepartmentDataVM();
                            record.id = item["id"].Value<string>();
                            record.dept_code = item["dept_code"].Value<string>();
                            //record.dept_name = item["dept_name"].Value<string>();
                            //record.parent_dept = item["parent_dept"].Value<string>();
                            departments.Add(record);
                        }

                    }
                    else
                    {
                        throw new Exception($"Failed to retrieve data department list.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error fetching department data: {ex.Message}");
                }

            }

            return departments;
        }
        private async Task<string> GetJwtToken()
        {
            string jwtAuthUrl = WebConfigurationManager.AppSettings["JwtAuthUrl"];
            string username = WebConfigurationManager.AppSettings["JwtUsername"];
            string password = WebConfigurationManager.AppSettings["JwtPassword"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(
                    $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}",
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(jwtAuthUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var token = JObject.Parse(json)["token"].ToString();
                    return token;
                }
                else
                {
                    throw new Exception("Failed to retrieve JWT token");
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
