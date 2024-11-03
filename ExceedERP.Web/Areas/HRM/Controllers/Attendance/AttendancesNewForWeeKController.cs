using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
    [AuthorizeRoles(HrmConstants.HRAttendanceUser,
          HrmConstants.HRDepartmentHead,
          HrmConstants.HREmployeeSelfService)]
    public class AttendancesNewForWeeKController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["EmployeeList"] = db.Person_Employee.AsQueryable();
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();
            // GenerateEmployeesForAttendance(DateTime.Today);

            return View();
        }

        public void GenerateEmployeesForAttendance(DateTime date)
        {
            // Calculate the start date (Monday) based on the selected date
            DateTime startDate = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);

            // Create a list to store the dates and day names for the week
            List<(DateTime, string)> weekDates = new List<(DateTime, string)>();

            // Generate the dates and day names for Monday to Saturday
            for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
            {
                DateTime date1 = startDate.AddDays(i);
                string dayName = date1.ToString("dddd"); // Get the full day name

                weekDates.Add((date1, dayName));
            }
            ApplicationDbContext adb = new ApplicationDbContext();
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            foreach (var (date1, dayName) in weekDates)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent && x.IsHead && x.IsApprove);
                if (placement != null)
                {
                    List<int> employeesId = GetEmployeesUnderBoss(empId);

                    foreach (var emp in employeesId)
                    {
                        var check = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == emp && x.Date == date1);
                        if (check == null)
                        {
                            var attendance = new AttendanceNew
                            {
                                EmployeeId = emp,
                                IsPresentMorning = true,
                                IsPresentAfternoon = true,
                                Date = date1,
                                IsDepartmentApproved = false,
                                IsApproved = false
                            };

                            attendance.DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(date1.Day, date1.Month, date1.Year);
                            var period = db.GLPeriods.FirstOrDefault(x => x.DateFrom <= date && x.DateTo >= date);
                            if (period != null)
                            {
                                attendance.GlFisicalYearId = period.GlFiscalYearId;
                                attendance.GlPeriodId = period.GLPeriodId;
                                db.AttendanceNews.Add(attendance);
                                db.SaveChanges();
                            }
                        }
                    }

                }
                else
                {
                }
            }
           
        }
        public List<int> GetEmployeesUnderBoss(int id)
        {
            List<int> employeesIdList = EmployeeHelper.GetSubordinates_FilteredByHead(id);

            //var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            //if (placement != null)
            //{
            //    var placements = db.Placements.Where(x => x.OrgStructureId == placement.OrgStructureId && x.isCurrent);
            //    if (placements.Any())
            //    {
            //        var empIdList = placements.Select(x => x.EmployeeId).ToList();
            //        employeesIdList.AddRange(empIdList);
            //    }

            //    var employeesUnderThisParent = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId).ToList();
            //    foreach (var und in employeesUnderThisParent)
            //    {
            //        var employeesPlacedInThisParent = db.Placements.Where(x => x.OrgStructureId == und.OrgStructureId).ToList();
            //        if (employeesPlacedInThisParent.Any())
            //        {
            //            var empIds = employeesPlacedInThisParent.Select(x => x.EmployeeId).ToList();

            //            employeesIdList.AddRange(empIds);
            //        }

            //    }

            //}

            return employeesIdList;
        }
        public int GetDepartment(int id)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                return placement.OrgStructureId;
            }
            return 0;
        }
        public ActionResult AttendancesNewForWeeK_Read([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime attendanceDate = DateTime.Today;
            int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var employees = GetEmployeesUnderBoss(empId);
            var employees = EmployeeHelper.GetSubordinates_FilteredByHead(empId);

            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
            }
            if (attendanceDate <= DateTime.Today)
            {
                var checkExit = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId) && x.Date == attendanceDate).ToList();
                if (checkExit.Count() < employees.Count())
                {
                    GenerateEmployeesForAttendance(attendanceDate);
                }
            }
            else
            {
                
            }
            DateTime startDate = attendanceDate.AddDays(-(int)attendanceDate.DayOfWeek + (int)DayOfWeek.Monday);
            // Create a list to store the dates  for the week
            List<DateTime> weekDates = new List<DateTime>();
            // Generate the dates and day names for Monday to Saturday
            for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
            {
                DateTime date1 = startDate.AddDays(i);

                weekDates.Add(date1);
            }
            var attendancenewRecords = db.AttendanceNews.ToList().Where(x => employees.Contains(x.EmployeeId)).Where(x => weekDates.Contains(x.Date) /*x.Date == attendanceDate*/);
            #region COMMENTED OUT  EXISTEDCODE
            //var attendancenewLists= attendancenewRecords.GroupBy(x => x.EmployeeId)
            //.Select(group => new AttendanceNewVM
            //{
            //    EmployeeId = group.Key,
            //    OrgStructureId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).OrgStructureId
            //        : 0,
            //    AttendanceNewId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AttendanceNewId
            //        : 0,
            //    DayOneDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).Date
            //        : DateTime.MinValue,
            //    IsPresentMorning1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentMorning
            //        :false,
            //    IsPresentAfternoon1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentAfternoon
            //        :false,
            //    DayTwoDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).Date
            //        : DateTime.MinValue,
            //    IsPresentMorning2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentAfternoon
            //        : false,
            //    DayThreeDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).Date
            //        : DateTime.MinValue,
            //    IsPresentMorning3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentAfternoon
            //        : false,
            //    DayFourDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).Date
            //        : DateTime.MinValue,
            //    IsPresentMorning4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentAfternoon
            //        : false,
            //    DayFiveDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).Date
            //        : DateTime.MinValue,
            //    IsPresentMorning5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentAfternoon
            //        : false,
            //    DaySixDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).Date
            //        : DateTime.MinValue,
            //    IsPresentMorning6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentAfternoon
            //        : false,
            //    IsDepartmentApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday|| ar.Date.DayOfWeek == DayOfWeek.Tuesday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Wednesday|| ar.Date.DayOfWeek == DayOfWeek.Thursday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Friday|| ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsDepartmentApproved
            //        : false,
            //    IsApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday|| ar.Date.DayOfWeek == DayOfWeek.Tuesday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Wednesday|| ar.Date.DayOfWeek == DayOfWeek.Thursday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Friday|| ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsApproved
            //        : false,
            //    IsPresentAfternoon = true,
            //    IsPresentMorning = true,
            //    // Add other properties as needed
            //})
            //.ToList();
            #endregion
            var attendancenewLists = new List<AttendanceNewVM>();
            if (attendancenewRecords.Any())
            {
                attendancenewLists = attendancenewRecords.GroupBy(x => x.EmployeeId)
                                   .Select(group => new AttendanceNewVM
                                   {
                                       EmployeeId = group.Key,
                                       OrgStructureId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).OrgStructureId
                                           : 0,
                                       AttendanceNewId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AttendanceNewId
                                           : 0,
                                       DayOneDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).Date
                                           : DateTime.MinValue,
                                       IsPresentMorning1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentMorning
                                           : false,
                                       MorningAttendance1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
                                           : TakeAttendance.Present,
                                       AfternoonAttendance1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
                                           : TakeAttendance.Present,
                                       IsPresentAfternoon1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentAfternoon
                                           : false,
                                       DayTwoDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).Date
                                           : DateTime.MinValue,
                                       MorningAttendance2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
                                           : TakeAttendance.Present,
                                       AfternoonAttendance2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
                                           : TakeAttendance.Present,
                                       IsPresentMorning2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentMorning
                                           : false,
                                       IsPresentAfternoon2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentAfternoon
                                           : false,
                                       DayThreeDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).Date
                                           : DateTime.MinValue,
                                       MorningAttendance3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
                                           : TakeAttendance.Present,
                                       AfternoonAttendance3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
                                           : TakeAttendance.Present,
                                       IsPresentMorning3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentMorning
                                           : false,
                                       IsPresentAfternoon3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentAfternoon
                                           : false,
                                       DayFourDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).Date
                                           : DateTime.MinValue,
                                       MorningAttendance4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
                                           : TakeAttendance.Present,
                                       AfternoonAttendance4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
                                           : TakeAttendance.Present,
                                       IsPresentMorning4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentMorning
                                           : false,
                                       IsPresentAfternoon4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentAfternoon
                                           : false,
                                       DayFiveDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).Date
                                           : DateTime.MinValue,
                                       MorningAttendance5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
                                           : TakeAttendance.Present,
                                       AfternoonAttendance5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
                                           : TakeAttendance.Present,
                                       IsPresentMorning5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentMorning
                                           : false,
                                       IsPresentAfternoon5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentAfternoon
                                           : false,
                                       DaySixDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).Date
                                           : DateTime.MinValue,
                                       MorningAttendance6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
                                           : TakeAttendance.Present,
                                       AfternoonAttendance6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
                                           : TakeAttendance.Present,
                                       IsPresentMorning6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentMorning
                                           : false,
                                       IsPresentAfternoon6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentAfternoon
                                           : false,
                                       IsDepartmentApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday || ar.Date.DayOfWeek == DayOfWeek.Tuesday
                                                                           || ar.Date.DayOfWeek == DayOfWeek.Wednesday || ar.Date.DayOfWeek == DayOfWeek.Thursday
                                                                           || ar.Date.DayOfWeek == DayOfWeek.Friday || ar.Date.DayOfWeek == DayOfWeek.Saturday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsDepartmentApproved
                                           : false,
                                       IsApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday || ar.Date.DayOfWeek == DayOfWeek.Tuesday
                                                                           || ar.Date.DayOfWeek == DayOfWeek.Wednesday || ar.Date.DayOfWeek == DayOfWeek.Thursday
                                                                           || ar.Date.DayOfWeek == DayOfWeek.Friday || ar.Date.DayOfWeek == DayOfWeek.Saturday)
                                           ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsApproved
                                           : false,
                                       IsPresentAfternoon = true,
                                       IsPresentMorning = true,
                                       // Add other properties as needed
                                   })
                                   .ToList();

            }
            #region comments
            //var attendancenewLists = attendancenewRecords.GroupBy(x => x.EmployeeId)
            //.Select(group => new AttendanceNewVM
            //{
            //    EmployeeId = group.Key,
            //    OrgStructureId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).OrgStructureId
            //        : 0,
            //    AttendanceNewId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AttendanceNewId
            //        : 0,
            //    DayOneDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).Date
            //        : DateTime.MinValue,
            //    IsPresentMorning1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentMorning
            //        :false,
            //    MorningAttendance1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
            //        :TakeAttendance.Present,
            //    AfternoonAttendance1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
            //        :TakeAttendance.Present,
            //    IsPresentAfternoon1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentAfternoon
            //        :false,
            //    DayTwoDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).Date
            //        : DateTime.MinValue,
            //    MorningAttendance2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
            //        : TakeAttendance.Present,
            //    AfternoonAttendance2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
            //        : TakeAttendance.Present,
            //    IsPresentMorning2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentAfternoon
            //        : false,
            //    DayThreeDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).Date
            //        : DateTime.MinValue,
            //    MorningAttendance3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
            //        : TakeAttendance.Present,
            //    AfternoonAttendance3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
            //        : TakeAttendance.Present,
            //    IsPresentMorning3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentAfternoon
            //        : false,
            //    DayFourDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).Date
            //        : DateTime.MinValue,
            //    MorningAttendance4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
            //        : TakeAttendance.Present,
            //    AfternoonAttendance4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
            //        : TakeAttendance.Present,
            //    IsPresentMorning4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentAfternoon
            //        : false,
            //    DayFiveDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).Date
            //        : DateTime.MinValue,
            //    MorningAttendance5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
            //        : TakeAttendance.Present,
            //    AfternoonAttendance5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
            //        : TakeAttendance.Present,
            //    IsPresentMorning5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentAfternoon
            //        : false,
            //    DaySixDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).Date
            //        : DateTime.MinValue,
            //    MorningAttendance6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).MorningAttendance
            //        : TakeAttendance.Present,
            //    AfternoonAttendance6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AfternoonAttendance
            //        : TakeAttendance.Present,
            //    IsPresentMorning6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentMorning
            //        : false,
            //    IsPresentAfternoon6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentAfternoon
            //        : false,
            //    IsDepartmentApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday|| ar.Date.DayOfWeek == DayOfWeek.Tuesday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Wednesday|| ar.Date.DayOfWeek == DayOfWeek.Thursday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Friday|| ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsDepartmentApproved
            //        : false,
            //    IsApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday|| ar.Date.DayOfWeek == DayOfWeek.Tuesday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Wednesday|| ar.Date.DayOfWeek == DayOfWeek.Thursday
            //                                        || ar.Date.DayOfWeek == DayOfWeek.Friday|| ar.Date.DayOfWeek == DayOfWeek.Saturday)
            //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsApproved
            //        : false,
            //    IsPresentAfternoon = true,
            //    IsPresentMorning = true,
            //    // Add other properties as needed
            //})
            //.ToList();
            //IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => weekDates.Contains(x.Date) /*x.Date == attendanceDate*/);
            #endregion
            DataSourceResult result = attendancenewLists.ToDataSourceResult(request, attendanceNew => new
            {
                AttendanceNewId = attendanceNew.AttendanceNewId,
                EmployeeId = attendanceNew.EmployeeId,
                //OrgStructureId = GetDepartment(attendanceNew.EmployeeId),
                OrgStructureId = attendanceNew.OrgStructureId,
                Date = attendanceNew.Date,
                DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.Date.Day, attendanceNew.Date.Month, attendanceNew.Date.Year)+"("+ attendanceNew.Date.ToString("dddd")+")",
                IsPresentMorning = attendanceNew.IsPresentMorning,
                IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
                DayOneDate = attendanceNew.DayOneDate,
                MorningAttendance1 = attendanceNew.MorningAttendance1,
                AfternoonAttendance1 = attendanceNew.AfternoonAttendance1,
                DayOneAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayOneDate.Day, attendanceNew.DayOneDate.Month, attendanceNew.DayOneDate.Year) + "(" + attendanceNew.DayOneDate.ToString("dddd") + ")",
                IsPresentMorning1 = attendanceNew.IsPresentMorning1,
                IsPresentAfternoon1 = attendanceNew.IsPresentAfternoon1,
                DayTwoDate = attendanceNew.DayTwoDate,
                DayTwoAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayTwoDate.Day, attendanceNew.DayTwoDate.Month, attendanceNew.DayTwoDate.Year) + "(" + attendanceNew.DayTwoDate.ToString("dddd") + ")",
                MorningAttendance2 = attendanceNew.MorningAttendance2,
                AfternoonAttendance2 = attendanceNew.AfternoonAttendance2,
                IsPresentMorning2 = attendanceNew.IsPresentMorning2,
                IsPresentAfternoon2 = attendanceNew.IsPresentAfternoon2,
                DayThreeDate = attendanceNew.DayThreeDate,
                DayThreeAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayThreeDate.Day, attendanceNew.DayThreeDate.Month, attendanceNew.DayThreeDate.Year) + "(" + attendanceNew.DayThreeDate.ToString("dddd") + ")",
                MorningAttendance3 = attendanceNew.MorningAttendance3,
                AfternoonAttendance3 = attendanceNew.AfternoonAttendance3,
                IsPresentMorning3 = attendanceNew.IsPresentMorning3,
                IsPresentAfternoon3 = attendanceNew.IsPresentAfternoon3,
                DayFourDate = attendanceNew.DayFourDate,
                DayFourAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFourDate.Day, attendanceNew.DayFourDate.Month, attendanceNew.DayFourDate.Year) + "(" + attendanceNew.DayFourDate.ToString("dddd") + ")",
                MorningAttendance4 = attendanceNew.MorningAttendance4,
                AfternoonAttendance4 = attendanceNew.AfternoonAttendance4,
                IsPresentMorning4 = attendanceNew.IsPresentMorning4,
                IsPresentAfternoon4 = attendanceNew.IsPresentAfternoon4,
                DayFiveDate = attendanceNew.DayFiveDate,
                DayFiveAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFiveDate.Day, attendanceNew.DayFiveDate.Month, attendanceNew.DayFiveDate.Year) + "(" + attendanceNew.DayFiveDate.ToString("dddd") + ")",
                MorningAttendance5 = attendanceNew.MorningAttendance5,
                AfternoonAttendance5 = attendanceNew.AfternoonAttendance5,
                IsPresentMorning5 = attendanceNew.IsPresentMorning5,
                IsPresentAfternoon5 = attendanceNew.IsPresentAfternoon5,
                DaySixDate = attendanceNew.DaySixDate,
                DaySixAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DaySixDate.Day, attendanceNew.DaySixDate.Month, attendanceNew.DaySixDate.Year) + "(" + attendanceNew.DaySixDate.ToString("dddd") + ")",
                MorningAttendance6 = attendanceNew.MorningAttendance6,
                AfternoonAttendance6 = attendanceNew.AfternoonAttendance6,
                IsPresentMorning6 = attendanceNew.IsPresentMorning6,
                IsPresentAfternoon6= attendanceNew.IsPresentAfternoon6,
                IsApproved = attendanceNew.IsApproved,
                ApproverEmpId = attendanceNew.ApproverEmpId,
                IsDepartmentApproved = attendanceNew.IsDepartmentApproved,
                DepartmentApproverId = attendanceNew.DepartmentApproverId,
            });

            return Json(result);
        }
        public ActionResult AttendancesNewForWeeKForOneEmployee_Read([DataSourceRequest]DataSourceRequest request, string date,int employeeId)
        {
            DateTime attendanceDate = DateTime.Today;

            int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employees = GetEmployeesUnderBoss(empId);

            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
            }
            DateTime startDate = attendanceDate.AddDays(-(int)attendanceDate.DayOfWeek + (int)DayOfWeek.Monday);

            // Create a list to store the dates  for the week
            List<DateTime> weekDates = new List<DateTime>();

            // Generate the dates and day names for Monday to Saturday
            for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
            {
                DateTime date1 = startDate.AddDays(i);

                weekDates.Add(date1);
            }

            IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => weekDates.Contains(x.Date));

            DataSourceResult result = attendancenews.ToDataSourceResult(request, attendanceNew => new
            {
                AttendanceNewId = attendanceNew.AttendanceNewId,
                EmployeeId = attendanceNew.EmployeeId,
                // OrgStructureId = GetDepartment(attendanceNew.EmployeeId),
                OrgStructureId = attendanceNew.OrgStructureId,
                Date = attendanceNew.Date,
                DateAm = attendanceNew.DateAm,
                IsPresentMorning = attendanceNew.IsPresentMorning,
                IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
                IsApproved = attendanceNew.IsApproved,
                ApproverEmpId = attendanceNew.ApproverEmpId,
                IsDepartmentApproved = attendanceNew.IsDepartmentApproved,
                DepartmentApproverId = attendanceNew.DepartmentApproverId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttendancesNewForWeeK_Create([DataSourceRequest]DataSourceRequest request, AttendanceNew attendanceNew)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == attendanceNew.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                attendanceNew.OrgStructureId = placement.OrgStructureId;
            }
            if (ModelState.IsValid)
            {
                var entity = new AttendanceNew
                {
                    EmployeeId = attendanceNew.EmployeeId,
                    OrgStructureId = attendanceNew.OrgStructureId,
                    Date = attendanceNew.Date,
                    DateAm = attendanceNew.DateAm,
                    IsPresentMorning = attendanceNew.IsPresentMorning,
                    IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
                    IsApproved = attendanceNew.IsApproved,
                };

                db.AttendanceNews.Add(entity);
                db.SaveChanges();
                attendanceNew.AttendanceNewId = entity.AttendanceNewId;
            }

            return Json(new[] { attendanceNew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttendancesNewForWeeK_Update([DataSourceRequest]DataSourceRequest request, AttendanceNew attendanceNew)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == attendanceNew.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                attendanceNew.OrgStructureId = placement.OrgStructureId;
            }
            if (ModelState.IsValid)
            {
                var entity = new AttendanceNew
                {
                    AttendanceNewId = attendanceNew.AttendanceNewId,
                    EmployeeId = attendanceNew.EmployeeId,
                    OrgStructureId = attendanceNew.OrgStructureId,
                    Date = attendanceNew.Date,
                    DateAm = attendanceNew.DateAm,
                    IsPresentMorning = attendanceNew.IsPresentMorning,
                    IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
                    IsApproved = attendanceNew.IsApproved,
                };

                db.AttendanceNews.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attendanceNew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttendancesNewForWeeK_Destroy([DataSourceRequest]DataSourceRequest request, AttendanceNew attendanceNew)
        {
            if (ModelState.IsValid)
            {
                var entity = new AttendanceNew
                {
                    AttendanceNewId = attendanceNew.AttendanceNewId,
                    EmployeeId = attendanceNew.EmployeeId,
                    OrgStructureId = attendanceNew.OrgStructureId,
                    Date = attendanceNew.Date,
                    DateAm = attendanceNew.DateAm,
                    IsPresentMorning = attendanceNew.IsPresentMorning,
                    IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
                    IsApproved = attendanceNew.IsApproved,
                };

                db.AttendanceNews.Attach(entity);
                db.AttendanceNews.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attendanceNew }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AttendanceNewVM> attendances)
        {
            ModelState.Clear();
            var results = new List<WeeklyAttendance>();
            if (attendances.Any() && ModelState.IsValid)
            {
                foreach (var att in attendances)
                {
                    if (att.DayOneDate.DayOfWeek == DayOfWeek.Monday)
                    {
                        var attendance = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == att.EmployeeId&&x.Date==att.DayOneDate);
                        if (attendance != null)
                        {
                            attendance.MorningAttendance = att.MorningAttendance1;
                            attendance.AfternoonAttendance = att.AfternoonAttendance1;
                           
                            if (att.MorningAttendance1 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = false;
                                attendance.MorningAttendance = TakeAttendance.Absent;
                            }
                            if (att.AfternoonAttendance1 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = false;
                                attendance.AfternoonAttendance = TakeAttendance.Absent;
                            }
                            if (att.MorningAttendance1 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = true;
                            }
                            if (att.AfternoonAttendance1 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = true;
                            }


                            var leaveTaken = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId &&  att.DayOneDate>= x.From && att.DayOneDate<=x.To);

                            if (leaveTaken != null)
                            {
                                if (leaveTaken.Day == LeaveDay.Half_Day)
                                {
                                    if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Morning)
                                    {
                                        attendance.IsPresentMorning = true;
                                        attendance.MorningAttendance = TakeAttendance.Leave;

                                    }
                                    else if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
                                    {
                                        attendance.IsPresentAfternoon = true;
                                        attendance.AfternoonAttendance = TakeAttendance.Leave;

                                    }
                                }
                                else
                                {
                                    attendance.IsPresentMorning = true;
                                    attendance.IsPresentAfternoon = true;
                                    attendance.MorningAttendance = TakeAttendance.Leave;
                                    attendance.AfternoonAttendance = TakeAttendance.Leave;
                                }
                            }

                            db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (att.DayTwoDate.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        var attendance = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == att.EmployeeId&&x.Date==att.DayTwoDate);
                        if (attendance != null)
                        {
                            //attendance.IsPresentMorning = att.IsPresentMorning2;
                            //attendance.IsPresentAfternoon = att.IsPresentAfternoon2;
                            attendance.MorningAttendance = att.MorningAttendance2;
                            attendance.AfternoonAttendance = att.AfternoonAttendance2;
                            if (att.MorningAttendance2 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = false;
                                attendance.MorningAttendance = TakeAttendance.Absent;
                            }
                            if (att.AfternoonAttendance2 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = false;
                                attendance.AfternoonAttendance = TakeAttendance.Absent;
                            }
                            if (att.MorningAttendance2!= TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = true;
                            }
                            if (att.AfternoonAttendance2 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = true;
                            }
                            var leaveTaken = db.Leaves .Where(x => x.EmployeeId == att.EmployeeId)
                                                        .Where(x => att.DayTwoDate >= x.From && att.DayTwoDate <= x.To)
                                                        .FirstOrDefault();
                            if (leaveTaken != null)
                            {
                                if (leaveTaken.Day == LeaveDay.Half_Day)
                                {
                                    if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Morning)
                                    {
                                        attendance.IsPresentMorning = true;
                                        attendance.MorningAttendance = TakeAttendance.Leave;

                                    }
                                    else if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
                                    {
                                        attendance.IsPresentAfternoon = true;
                                        attendance.AfternoonAttendance = TakeAttendance.Leave;

                                    }
                                }
                                else
                                {
                                    //attendance.IsPresentMorning = att.IsPresentMorning2;
                                    //attendance.IsPresentAfternoon = att.IsPresentAfternoon2;
                                    attendance.IsPresentMorning = true;
                                    attendance.IsPresentAfternoon = true;
                                    attendance.MorningAttendance = TakeAttendance.Leave;
                                    attendance.AfternoonAttendance = TakeAttendance.Leave;
                                }
                            }

                            db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (att.DayThreeDate.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        var attendance = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == att.EmployeeId&&x.Date==att.DayThreeDate);
                        if (attendance != null)
                        {
                            //attendance.IsPresentMorning = att.IsPresentMorning3;
                            //attendance.IsPresentAfternoon = att.IsPresentAfternoon3;
                            attendance.MorningAttendance = att.MorningAttendance3;
                            attendance.AfternoonAttendance = att.AfternoonAttendance3;
                            if (att.MorningAttendance3 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = false;
                                attendance.MorningAttendance = TakeAttendance.Absent;
                            }
                            if (att.AfternoonAttendance3 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = false;
                                attendance.AfternoonAttendance = TakeAttendance.Absent;
                            }
                            if (att.MorningAttendance3 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = true;
                            }
                            if (att.AfternoonAttendance3 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = true;
                            }

                            var leaveTaken = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.DayThreeDate && x.To >= att.DayThreeDate);

                            if (leaveTaken != null)
                            {
                                if (leaveTaken.Day == LeaveDay.Half_Day)
                                {
                                    if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Morning)
                                    {
                                        attendance.IsPresentMorning = true;
                                    }
                                    else if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
                                    {
                                        attendance.IsPresentAfternoon = true;
                                    }
                                }
                                else
                                {
                                    attendance.IsPresentMorning = true;
                                    attendance.IsPresentAfternoon = true;
                                }
                            }

                            db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (att.DayFourDate.DayOfWeek == DayOfWeek.Thursday)
                    {
                        var attendance = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == att.EmployeeId&&x.Date==att.DayFourDate);
                        if (attendance != null)
                        {
                            attendance.MorningAttendance = att.MorningAttendance4;
                            attendance.AfternoonAttendance = att.AfternoonAttendance4;
                            if (att.MorningAttendance4 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = false;
                                attendance.MorningAttendance = TakeAttendance.Absent;
                            }
                            if (att.AfternoonAttendance4 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = false;
                                attendance.AfternoonAttendance = TakeAttendance.Absent;
                            }
                            if (att.MorningAttendance4 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = true;
                            }
                            if (att.AfternoonAttendance4 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = true;
                            }
                            var leaveTaken = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.DayFourDate && x.To >= att.DayFourDate);

                            if (leaveTaken != null)
                            {
                                if (leaveTaken.Day == LeaveDay.Half_Day)
                                {
                                    if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Morning)
                                    {
                                        attendance.IsPresentMorning = true;
                                        attendance.MorningAttendance = TakeAttendance.Leave;

                                    }
                                    else if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
                                    {
                                        attendance.IsPresentAfternoon = true;
                                        attendance.AfternoonAttendance = TakeAttendance.Leave;

                                    }
                                }
                                else
                                {
                                    //attendance.IsPresentMorning = att.IsPresentMorning2;
                                    //attendance.IsPresentAfternoon = att.IsPresentAfternoon2;
                                    attendance.IsPresentMorning = true;
                                    attendance.IsPresentAfternoon = true;
                                    attendance.MorningAttendance = TakeAttendance.Leave;
                                    attendance.AfternoonAttendance = TakeAttendance.Leave;
                                }
                            }

                            db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (att.DayFiveDate.DayOfWeek == DayOfWeek.Friday)
                    {
                        var attendance = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == att.EmployeeId&&x.Date==att.DayFiveDate);
                        if (attendance != null)
                        {
                            //attendance.IsPresentMorning = att.IsPresentMorning5;
                            //attendance.IsPresentAfternoon = att.IsPresentAfternoon5;
                            attendance.MorningAttendance = att.MorningAttendance5;
                            attendance.AfternoonAttendance = att.AfternoonAttendance5;
                            if (att.MorningAttendance5 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = false;
                                attendance.MorningAttendance = TakeAttendance.Absent;
                            }
                            if (att.AfternoonAttendance5== TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = false;
                                attendance.AfternoonAttendance = TakeAttendance.Absent;
                            }
                            if (att.MorningAttendance5 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = true;
                            }
                            if (att.AfternoonAttendance5 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = true;
                            }
                            var leaveTaken = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.DayFiveDate && x.To >= att.DayFiveDate);

                            if (leaveTaken != null)
                            {
                                if (leaveTaken.Day == LeaveDay.Half_Day)
                                {
                                    if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Morning)
                                    {
                                        attendance.IsPresentMorning = true;
                                        attendance.MorningAttendance = TakeAttendance.Leave;

                                    }
                                    else if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
                                    {
                                        attendance.IsPresentAfternoon = true;
                                        attendance.AfternoonAttendance = TakeAttendance.Leave;

                                    }
                                }
                                else
                                {
                                    //attendance.IsPresentMorning = att.IsPresentMorning2;
                                    //attendance.IsPresentAfternoon = att.IsPresentAfternoon2;
                                    attendance.IsPresentMorning = true;
                                    attendance.IsPresentAfternoon = true;
                                    attendance.MorningAttendance = TakeAttendance.Leave;
                                    attendance.AfternoonAttendance = TakeAttendance.Leave;
                                }
                            }

                            db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (att.DaySixDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        var attendance = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == att.EmployeeId&&x.Date==att.DaySixDate);
                        if (attendance != null)
                        {
                            //attendance.IsPresentMorning = att.IsPresentMorning6;
                            //attendance.IsPresentAfternoon = att.IsPresentAfternoon6;
                            attendance.MorningAttendance = att.MorningAttendance6;
                            attendance.AfternoonAttendance = att.AfternoonAttendance6;
                            if (att.MorningAttendance6 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = false;
                                attendance.MorningAttendance = TakeAttendance.Absent;
                            }
                            if (att.AfternoonAttendance6 == TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = false;
                                attendance.AfternoonAttendance = TakeAttendance.Absent;
                            }
                            if (att.MorningAttendance6 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentMorning = true;
                            }
                            if (att.AfternoonAttendance6 != TakeAttendance.Absent)
                            {
                                attendance.IsPresentAfternoon = true;
                            }
                            var leaveTaken = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.DaySixDate && x.To >= att.DaySixDate);

                            if (leaveTaken != null)
                            {
                                if (leaveTaken.Day == LeaveDay.Half_Day)
                                {
                                    if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Morning)
                                    {
                                        attendance.IsPresentMorning = true;
                                        attendance.MorningAttendance = TakeAttendance.Leave;

                                    }
                                    else if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
                                    {
                                        attendance.IsPresentAfternoon = true;
                                        attendance.AfternoonAttendance = TakeAttendance.Leave;

                                    }
                                }
                                else
                                {
                                    //attendance.IsPresentMorning = att.IsPresentMorning2;
                                    //attendance.IsPresentAfternoon = att.IsPresentAfternoon2;
                                    attendance.IsPresentMorning = true;
                                    attendance.IsPresentAfternoon = true;
                                    attendance.MorningAttendance = TakeAttendance.Leave;
                                    attendance.AfternoonAttendance = TakeAttendance.Leave;
                                }
                            }
                            db.Entry(attendance).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                 
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        public JsonResult DepartmentApproveAttendance(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                DateTime attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
                DateTime startDate = attendanceDate.AddDays(-(int)attendanceDate.DayOfWeek + (int)DayOfWeek.Monday);

                // Create a list to store the dates  for the week
                List<DateTime> weekDates = new List<DateTime>();

                // Generate the dates and day names for Monday to Saturday
                for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
                {
                    DateTime date1 = startDate.AddDays(i);

                    weekDates.Add(date1);
                }
                var attendances = db.AttendanceNews.Where(x => weekDates.Contains( x.Date)).ToList();
                foreach (var att in attendances)
                {
                    att.IsDepartmentApproved = true;
                    int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    int orgId = UserInformationHelper.GetEmployeeDepartmentIdByEmpId(empId);
                    att.ApproverEmpId = empId;
                    att.DepartmentApproverId = orgId;

                    db.Entry(att).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                var formattedData = new
                {
                    Message = "Successfully Approved.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "Not Approved.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReturnAttendance(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                DateTime attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
                DateTime startDate = attendanceDate.AddDays(-(int)attendanceDate.DayOfWeek + (int)DayOfWeek.Monday);

                // Create a list to store the dates  for the week
                List<DateTime> weekDates = new List<DateTime>();

                // Generate the dates and day names for Monday to Saturday
                for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
                {
                    DateTime date1 = startDate.AddDays(i);

                    weekDates.Add(date1);
                }
                var attendances = db.AttendanceNews.Where(x => weekDates.Contains(x.Date) && !x.IsApproved).ToList();

                if (!attendances.Any())
                {
                    var formattedData = new
                    {
                        Message = "This attendance can't be returend, it is approved by HR.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    foreach (var att in attendances)
                    {
                        att.IsDepartmentApproved = false;
                        int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        int orgId = UserInformationHelper.GetEmployeeDepartmentIdByEmpId(empId);
                        att.ApproverEmpId = empId;
                        att.DepartmentApproverId = orgId;

                        db.Entry(att).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                var formattedData2 = new
                {
                    Message = "Successfully Returned.",
                    Status = "OK"
                };
                return Json(formattedData2, JsonRequestBehavior.AllowGet);
            }

            var formattedData3 = new
            {
                Message = "Not Approved.",
                Status = "OK"
            };
            return Json(formattedData3, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentDateInAmharic()
        {
            var now = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

            var formattedData = new
            {
                Message = now + " (" + DateTime.Now.DayOfWeek + ")"
            };

            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDayOfWeek(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                var dayOfWeek = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

                var formattedData = new
                {
                    Message = dayOfWeek.DayOfWeek.ToString()
                };

                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = ""
            };

            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        public string GetEmployeeAndDepartment()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            return id.ToString();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
