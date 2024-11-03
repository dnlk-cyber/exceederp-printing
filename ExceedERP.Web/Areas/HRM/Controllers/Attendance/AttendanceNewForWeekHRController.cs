﻿using System;
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
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
    [AuthorizeRoles(HrmConstants.HRAttendanceUser,HrmConstants.HRDepartmentHead)]
    public class AttendanceNewForWeekHRController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["EmployeeList"] = db.Person_Employee.AsQueryable();
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();

            return View();
        }
        public ActionResult AttendanceNewForWeekHR_Read([DataSourceRequest] DataSourceRequest request, string date)
        {
            // Initialize attendance date to today's date
            DateTime attendanceDate = DateTime.Today;
            // If the date is provided, split and convert it to the Gregorian date
            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                if (splitDateTo.Length == 3 && int.TryParse(splitDateTo[0], out int year) && int.TryParse(splitDateTo[1], out int month) && int.TryParse(splitDateTo[2], out int day))
                {
                    attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(year, month, day);
                }
            }
            // Calculate start date for the week (Monday)
            DateTime startDate = attendanceDate.AddDays(-(int)attendanceDate.DayOfWeek + (int)DayOfWeek.Monday);
            // Create a list to store the dates for the week (Monday to Saturday)
            List<DateTime> weekDates = new List<DateTime>();
            for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
            {
                weekDates.Add(startDate.AddDays(i));
            }
            // Get the list of branches for the current user
            List<int> branchIds = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name) ?? new List<int>();
            // Get the list of employee IDs for the current branches
            List<int> employeeIds = db.Placements.Where(x => x.isCurrent && branchIds.Contains(x.BranchId))
                                                 .Select(x => x.EmployeeId)
                                                 .ToList();
            // Retrieve the attendance records for the given week
            var attendancenewRecords = db.AttendanceNews
                                         .Where(x => employeeIds.Contains(x.EmployeeId) && x.IsDepartmentApproved)
                                         .Where(x => weekDates.Contains(x.Date))
                                         .ToList();
            // Create the attendance view model list
            List<AttendanceNewVM> attendancenewLists = new List<AttendanceNewVM>();
            if (attendancenewRecords.Any())
            {
                attendancenewLists = attendancenewRecords.GroupBy(x => x.EmployeeId)
                                       .Select(group => new AttendanceNewVM
                                       {
                                           EmployeeId = group.Key,
                                           OrgStructureId = group.FirstOrDefault(ar => ar.EmployeeId == group.Key)?.OrgStructureId ?? 0,
                                           AttendanceNewId = group.FirstOrDefault(ar => ar.EmployeeId == group.Key)?.AttendanceNewId ?? 0,
                                           DayOneDate = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)?.Date ?? DateTime.MinValue,
                                           IsPresentMorning1 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)?.IsPresentMorning ?? false,
                                           MorningAttendance1 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)?.MorningAttendance ?? TakeAttendance.Present,
                                           AfternoonAttendance1 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)?.AfternoonAttendance ?? TakeAttendance.Present,
                                           IsPresentAfternoon1 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)?.IsPresentAfternoon ?? false,

                                           DayTwoDate = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)?.Date ?? DateTime.MinValue,
                                           MorningAttendance2 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)?.MorningAttendance ?? TakeAttendance.Present,
                                           AfternoonAttendance2 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)?.AfternoonAttendance ?? TakeAttendance.Present,
                                           IsPresentMorning2 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)?.IsPresentMorning ?? false,
                                           IsPresentAfternoon2 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)?.IsPresentAfternoon ?? false,

                                           DayThreeDate = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)?.Date ?? DateTime.MinValue,
                                           MorningAttendance3 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)?.MorningAttendance ?? TakeAttendance.Present,
                                           AfternoonAttendance3 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)?.AfternoonAttendance ?? TakeAttendance.Present,
                                           IsPresentMorning3 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)?.IsPresentMorning ?? false,
                                           IsPresentAfternoon3 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)?.IsPresentAfternoon ?? false,

                                           DayFourDate = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)?.Date ?? DateTime.MinValue,
                                           MorningAttendance4 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)?.MorningAttendance ?? TakeAttendance.Present,
                                           AfternoonAttendance4 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)?.AfternoonAttendance ?? TakeAttendance.Present,
                                           IsPresentMorning4 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)?.IsPresentMorning ?? false,
                                           IsPresentAfternoon4 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)?.IsPresentAfternoon ?? false,

                                           DayFiveDate = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)?.Date ?? DateTime.MinValue,
                                           MorningAttendance5 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)?.MorningAttendance ?? TakeAttendance.Present,
                                           AfternoonAttendance5 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)?.AfternoonAttendance ?? TakeAttendance.Present,
                                           IsPresentMorning5 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)?.IsPresentMorning ?? false,
                                           IsPresentAfternoon5 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)?.IsPresentAfternoon ?? false,

                                           DaySixDate = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)?.Date ?? DateTime.MinValue,
                                           MorningAttendance6 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)?.MorningAttendance ?? TakeAttendance.Present,
                                           AfternoonAttendance6 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)?.AfternoonAttendance ?? TakeAttendance.Present,
                                           IsPresentMorning6 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)?.IsPresentMorning ?? false,
                                           IsPresentAfternoon6 = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)?.IsPresentAfternoon ?? false,

                                           IsDepartmentApproved = group.FirstOrDefault(ar => weekDates.Contains(ar.Date))?.IsDepartmentApproved ?? false,
                                           IsApproved = group.FirstOrDefault(ar => weekDates.Contains(ar.Date))?.IsApproved ?? false,
                                       })
                                       .ToList();
            }

            // Convert the attendance data to a data source result
            DataSourceResult result = attendancenewLists.ToDataSourceResult(request, attendanceNew => new
            {
                AttendanceNewId = attendanceNew.AttendanceNewId,
                EmployeeId = attendanceNew.EmployeeId,
                OrgStructureId = attendanceNew.OrgStructureId,
                Date = attendanceNew.DayOneDate,
                DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayOneDate.Day, attendanceNew.DayOneDate.Month, attendanceNew.DayOneDate.Year) + "(" + attendanceNew.DayOneDate.ToString("dddd") + ")",
                // Day One Data
                IsPresentMorning1 = attendanceNew.IsPresentMorning1,
                IsPresentAfternoon1 = attendanceNew.IsPresentAfternoon1,
                MorningAttendance1 = attendanceNew.MorningAttendance1,
                AfternoonAttendance1 = attendanceNew.AfternoonAttendance1,
                DayOneAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayOneDate.Day, attendanceNew.DayOneDate.Month, attendanceNew.DayOneDate.Year) + "(" + attendanceNew.DayOneDate.ToString("dddd") + ")",
                // Day Two Data
                DayTwoDate = attendanceNew.DayTwoDate,
                DayTwoAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayTwoDate.Day, attendanceNew.DayTwoDate.Month, attendanceNew.DayTwoDate.Year) + "(" + attendanceNew.DayTwoDate.ToString("dddd") + ")",
                MorningAttendance2 = attendanceNew.MorningAttendance2,
                AfternoonAttendance2 = attendanceNew.AfternoonAttendance2,
                IsPresentMorning2 = attendanceNew.IsPresentMorning2,
                IsPresentAfternoon2 = attendanceNew.IsPresentAfternoon2,
                //Day Three  Data
                DayThreeDate = attendanceNew.DayThreeDate,
                DayThreeAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayThreeDate.Day, attendanceNew.DayThreeDate.Month, attendanceNew.DayThreeDate.Year) + "(" + attendanceNew.DayThreeDate.ToString("dddd") + ")",
                MorningAttendance3 = attendanceNew.MorningAttendance3,
                AfternoonAttendance3 = attendanceNew.AfternoonAttendance3,
                IsPresentMorning3 = attendanceNew.IsPresentMorning3,
                IsPresentAfternoon3 = attendanceNew.IsPresentAfternoon3,
                DayFourDate = attendanceNew.DayFourDate,
                //Day Four  Data
                DayFourAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFourDate.Day, attendanceNew.DayFourDate.Month, attendanceNew.DayFourDate.Year) + "(" + attendanceNew.DayFourDate.ToString("dddd") + ")",
                MorningAttendance4 = attendanceNew.MorningAttendance4,
                AfternoonAttendance4 = attendanceNew.AfternoonAttendance4,
                IsPresentMorning4 = attendanceNew.IsPresentMorning4,
                IsPresentAfternoon4 = attendanceNew.IsPresentAfternoon4,
                DayFiveDate = attendanceNew.DayFiveDate,
                //Day Five Data
                DayFiveAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFiveDate.Day, attendanceNew.DayFiveDate.Month, attendanceNew.DayFiveDate.Year) + "(" + attendanceNew.DayFiveDate.ToString("dddd") + ")",
                MorningAttendance5 = attendanceNew.MorningAttendance5,
                AfternoonAttendance5 = attendanceNew.AfternoonAttendance5,
                IsPresentMorning5 = attendanceNew.IsPresentMorning5,
                IsPresentAfternoon5 = attendanceNew.IsPresentAfternoon5,
                //Day Six Data
                DaySixDate = attendanceNew.DaySixDate,
                DaySixAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DaySixDate.Day, attendanceNew.DaySixDate.Month, attendanceNew.DaySixDate.Year) + "(" + attendanceNew.DaySixDate.ToString("dddd") + ")",
                MorningAttendance6 = attendanceNew.MorningAttendance6,
                AfternoonAttendance6 = attendanceNew.AfternoonAttendance6,
                IsPresentMorning6 = attendanceNew.IsPresentMorning6,
                IsPresentAfternoon6 = attendanceNew.IsPresentAfternoon6,
                IsApproved = attendanceNew.IsApproved,
                ApproverEmpId = attendanceNew.ApproverEmpId,
                IsDepartmentApproved = attendanceNew.IsDepartmentApproved,
                DepartmentApproverId = attendanceNew.DepartmentApproverId,

            });

            return Json(result);
        }

        //     public ActionResult AttendanceNewForWeekHR_Read([DataSourceRequest]DataSourceRequest request, string date)
        //     {
        //         #region comment
        //         //DateTime attendanceDate = DateTime.Today;
        //         //if (!string.IsNullOrEmpty(date))
        //         //{
        //         //    var splitDateTo = date.Split('/');
        //         //    attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
        //         //}
        //         //DateTime startDate = attendanceDate.AddDays(-(int)attendanceDate.DayOfWeek + (int)DayOfWeek.Monday);

        //         //// Create a list to store the dates  for the week
        //         //List<DateTime> weekDates = new List<DateTime>();

        //         //// Generate the dates and day names for Monday to Saturday
        //         //for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
        //         //{
        //         //    DateTime date1 = startDate.AddDays(i);

        //         //    weekDates.Add(date1);
        //         //}
        //         //List<int> branchIds = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
        //         //List<int> EmployeeIds = new List<int>(db.Placements.Where(x => x.isCurrent && branchIds.Contains(x.BranchId)).Select(x => x.EmployeeId));

        //         //var attendancenewRecords = db.AttendanceNews.Where(x=> EmployeeIds.Contains(x.EmployeeId)).ToList().Where(x => x.IsDepartmentApproved).Where(x => weekDates.Contains(x.Date));
        //         //var attendancenewLists = attendancenewRecords.GroupBy(x => x.EmployeeId)
        //         //.Select(group => new AttendanceNewVM
        //         //{
        //         //    EmployeeId = group.Key,
        //         //    OrgStructureId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).OrgStructureId
        //         //        : 0,
        //         //    AttendanceNewId = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).AttendanceNewId
        //         //        : 0,
        //         //    DayOneDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).Date
        //         //        : DateTime.MinValue,
        //         //    IsPresentMorning1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentMorning
        //         //        : false,
        //         //    IsPresentAfternoon1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentAfternoon
        //         //        : false,
        //         //    DayTwoDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).Date
        //         //        : DateTime.MinValue,
        //         //    IsPresentMorning2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentMorning
        //         //        : false,
        //         //    IsPresentAfternoon2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentAfternoon
        //         //        : false,
        //         //    DayThreeDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).Date
        //         //        : DateTime.MinValue,
        //         //    IsPresentMorning3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentMorning
        //         //        : false,
        //         //    IsPresentAfternoon3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentAfternoon
        //         //        : false,
        //         //    DayFourDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).Date
        //         //        : DateTime.MinValue,
        //         //    IsPresentMorning4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentMorning
        //         //        : false,
        //         //    IsPresentAfternoon4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentAfternoon
        //         //        : false,
        //         //    DayFiveDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).Date
        //         //        : DateTime.MinValue,
        //         //    IsPresentMorning5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentMorning
        //         //        : false,
        //         //    IsPresentAfternoon5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentAfternoon
        //         //        : false,
        //         //    DaySixDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).Date
        //         //        : DateTime.MinValue,
        //         //    IsPresentMorning6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentMorning
        //         //        : false,
        //         //    IsPresentAfternoon6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentAfternoon
        //         //        : false,
        //         //    IsDepartmentApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday || ar.Date.DayOfWeek == DayOfWeek.Tuesday
        //         //                                        || ar.Date.DayOfWeek == DayOfWeek.Wednesday || ar.Date.DayOfWeek == DayOfWeek.Thursday
        //         //                                        || ar.Date.DayOfWeek == DayOfWeek.Friday || ar.Date.DayOfWeek == DayOfWeek.Saturday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsDepartmentApproved: false,
        //         //    IsApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday || ar.Date.DayOfWeek == DayOfWeek.Tuesday
        //         //                                       || ar.Date.DayOfWeek == DayOfWeek.Wednesday || ar.Date.DayOfWeek == DayOfWeek.Thursday
        //         //                                       || ar.Date.DayOfWeek == DayOfWeek.Friday || ar.Date.DayOfWeek == DayOfWeek.Saturday)
        //         //        ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsApproved: false,
        //         //    IsPresentAfternoon = true,
        //         //    IsPresentMorning = true,
        //         //    // Add other properties as needed
        //         //})
        //         //.ToList();
        //         ////IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => weekDates.Contains(x.Date) /*x.Date == attendanceDate*/);

        //         //DataSourceResult result = attendancenewLists.ToDataSourceResult(request, attendanceNew => new
        //         //{
        //         //    AttendanceNewId = attendanceNew.AttendanceNewId,
        //         //    EmployeeId = attendanceNew.EmployeeId,
        //         //    OrgStructureId = GetDepartment(attendanceNew.EmployeeId),
        //         //    Date = attendanceNew.Date,
        //         //    DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.Date.Day, attendanceNew.Date.Month, attendanceNew.Date.Year) + "(" + attendanceNew.Date.ToString("dddd") + ")",
        //         //    IsPresentMorning = attendanceNew.IsPresentMorning,
        //         //    IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
        //         //    DayOneDate = attendanceNew.DayOneDate,
        //         //    DayOneAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayOneDate.Day, attendanceNew.DayOneDate.Month, attendanceNew.DayOneDate.Year) + "(" + attendanceNew.DayOneDate.ToString("dddd") + ")",
        //         //    IsPresentMorning1 = attendanceNew.IsPresentMorning1,
        //         //    IsPresentAfternoon1 = attendanceNew.IsPresentAfternoon1,
        //         //    DayTwoDate = attendanceNew.DayTwoDate,
        //         //    DayTwoAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayTwoDate.Day, attendanceNew.DayTwoDate.Month, attendanceNew.DayTwoDate.Year) + "(" + attendanceNew.DayTwoDate.ToString("dddd") + ")",
        //         //    IsPresentMorning2 = attendanceNew.IsPresentMorning2,
        //         //    IsPresentAfternoon2 = attendanceNew.IsPresentAfternoon2,
        //         //    DayThreeDate = attendanceNew.DayThreeDate,
        //         //    DayThreeAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayThreeDate.Day, attendanceNew.DayThreeDate.Month, attendanceNew.DayThreeDate.Year) + "(" + attendanceNew.DayThreeDate.ToString("dddd") + ")",
        //         //    IsPresentMorning3 = attendanceNew.IsPresentMorning3,
        //         //    IsPresentAfternoon3 = attendanceNew.IsPresentAfternoon3,
        //         //    DayFourDate = attendanceNew.DayFourDate,
        //         //    DayFourAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFourDate.Day, attendanceNew.DayFourDate.Month, attendanceNew.DayFourDate.Year) + "(" + attendanceNew.DayFourDate.ToString("dddd") + ")",
        //         //    IsPresentMorning4 = attendanceNew.IsPresentMorning4,
        //         //    IsPresentAfternoon4 = attendanceNew.IsPresentAfternoon4,
        //         //    DayFiveDate = attendanceNew.DayFiveDate,
        //         //    DayFiveAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFiveDate.Day, attendanceNew.DayFiveDate.Month, attendanceNew.DayFiveDate.Year) + "(" + attendanceNew.DayFiveDate.ToString("dddd") + ")",
        //         //    IsPresentMorning5 = attendanceNew.IsPresentMorning5,
        //         //    IsPresentAfternoon5 = attendanceNew.IsPresentAfternoon5,
        //         //    DaySixDate = attendanceNew.DaySixDate,
        //         //    DaySixAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DaySixDate.Day, attendanceNew.DaySixDate.Month, attendanceNew.DaySixDate.Year) + "(" + attendanceNew.DaySixDate.ToString("dddd") + ")",
        //         //    IsPresentMorning6 = attendanceNew.IsPresentMorning6,
        //         //    IsPresentAfternoon6 = attendanceNew.IsPresentAfternoon6,
        //         //    IsApproved = attendanceNew.IsApproved,
        //         //    ApproverEmpId = attendanceNew.ApproverEmpId,
        //         //    IsDepartmentApproved = attendanceNew.IsDepartmentApproved,
        //         //    DepartmentApproverId = attendanceNew.DepartmentApproverId,
        //         //});
        //         //return Json(result);
        //         #endregion
        //         DateTime attendanceDate = DateTime.Today;

        //         if (!string.IsNullOrEmpty(date))
        //         {
        //             var splitDateTo = date.Split('/');
        //             attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
        //         }

        //         DateTime startDate = attendanceDate.AddDays(-(int)attendanceDate.DayOfWeek + (int)DayOfWeek.Monday);
        //         // Create a list to store the dates  for the week
        //         List<DateTime> weekDates = new List<DateTime>();
        //         // Generate the dates and day names for Monday to Saturday
        //         for (int i = 0; i < 6; i++) // Generate dates for Monday to Saturday
        //         {
        //             DateTime date1 = startDate.AddDays(i);

        //             weekDates.Add(date1);
        //         }
        //         List<int> branchIds = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
        //         List<int> employeeIds = db.Placements.Where(x => x.isCurrent && branchIds.Contains(x.BranchId)).Select(x => x.EmployeeId).ToList();
        //         var attendancenewRecords = db.AttendanceNews.ToList().Where(x => employeeIds.Contains(x.EmployeeId) && x.IsDepartmentApproved).Where(x => weekDates.Contains(x.Date) /*x.Date == attendanceDate*/);
        //         var attendancenewLists = new List<AttendanceNewVM>();
        //if (attendancenewRecords.Any())
        //{
        //	attendancenewLists = attendancenewRecords.GroupBy(x => x.EmployeeId)
        //		.Select(group =>
        //		{
        //			// Fetch attendance for each day of the week
        //			var monday = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday);
        //			var tuesday = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday);
        //			var wednesday = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday);
        //			var thursday = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday);
        //			var friday = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday);
        //			var saturday = group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday);

        //			return new AttendanceNewVM
        //			{
        //				EmployeeId = group.Key,
        //				OrgStructureId = monday?.OrgStructureId ?? 0,
        //				AttendanceNewId = monday?.AttendanceNewId ?? 0,
        //				DayOneDate = monday?.Date ?? DateTime.MinValue,
        //				IsPresentMorning1 = monday?.IsPresentMorning ?? false,
        //				MorningAttendance1 = monday?.MorningAttendance ?? TakeAttendance.Present,
        //				AfternoonAttendance1 = monday?.AfternoonAttendance ?? TakeAttendance.Present,
        //				IsPresentAfternoon1 = monday?.IsPresentAfternoon ?? false,

        //				DayTwoDate = tuesday?.Date ?? DateTime.MinValue,
        //				MorningAttendance2 = tuesday?.MorningAttendance ?? TakeAttendance.Present,
        //				AfternoonAttendance2 = tuesday?.AfternoonAttendance ?? TakeAttendance.Present,
        //				IsPresentMorning2 = tuesday?.IsPresentMorning ?? false,
        //				IsPresentAfternoon2 = tuesday?.IsPresentAfternoon ?? false,

        //				DayThreeDate = wednesday?.Date ?? DateTime.MinValue,
        //				MorningAttendance3 = wednesday?.MorningAttendance ?? TakeAttendance.Present,
        //				AfternoonAttendance3 = wednesday?.AfternoonAttendance ?? TakeAttendance.Present,
        //				IsPresentMorning3 = wednesday?.IsPresentMorning ?? false,
        //				IsPresentAfternoon3 = wednesday?.IsPresentAfternoon ?? false,

        //				DayFourDate = thursday?.Date ?? DateTime.MinValue,
        //				MorningAttendance4 = thursday?.MorningAttendance ?? TakeAttendance.Present,
        //				AfternoonAttendance4 = thursday?.AfternoonAttendance ?? TakeAttendance.Present,
        //				IsPresentMorning4 = thursday?.IsPresentMorning ?? false,
        //				IsPresentAfternoon4 = thursday?.IsPresentAfternoon ?? false,

        //				DayFiveDate = friday?.Date ?? DateTime.MinValue,
        //				MorningAttendance5 = friday?.MorningAttendance ?? TakeAttendance.Present,
        //				AfternoonAttendance5 = friday?.AfternoonAttendance ?? TakeAttendance.Present,
        //				IsPresentMorning5 = friday?.IsPresentMorning ?? false,
        //				IsPresentAfternoon5 = friday?.IsPresentAfternoon ?? false,

        //				DaySixDate = saturday?.Date ?? DateTime.MinValue,
        //				MorningAttendance6 = saturday?.MorningAttendance ?? TakeAttendance.Present,
        //				AfternoonAttendance6 = saturday?.AfternoonAttendance ?? TakeAttendance.Present,
        //				IsPresentMorning6 = saturday?.IsPresentMorning ?? false,
        //				IsPresentAfternoon6 = saturday?.IsPresentAfternoon ?? false,

        //				IsDepartmentApproved = saturday?.IsDepartmentApproved ?? false,
        //				IsApproved = saturday?.IsApproved ?? false,
        //				IsPresentAfternoon = true,
        //				IsPresentMorning = true,
        //			};
        //		})
        //		.ToList();
        //}


        ////IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => weekDates.Contains(x.Date) /*x.Date == attendanceDate*/);

        //DataSourceResult result = attendancenewLists.ToDataSourceResult(request, attendanceNew => new
        //         {
        //             AttendanceNewId = attendanceNew.AttendanceNewId,
        //             EmployeeId = attendanceNew.EmployeeId,
        //             //OrgStructureId = GetDepartment(attendanceNew.EmployeeId),
        //             OrgStructureId = attendanceNew.OrgStructureId,
        //             Date = attendanceNew.Date,
        //             DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.Date.Day, attendanceNew.Date.Month, attendanceNew.Date.Year) + "(" + attendanceNew.Date.ToString("dddd") + ")",
        //             IsPresentMorning = attendanceNew.IsPresentMorning,
        //             IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
        //             DayOneDate = attendanceNew.DayOneDate,
        //             MorningAttendance1 = attendanceNew.MorningAttendance1,
        //             AfternoonAttendance1 = attendanceNew.AfternoonAttendance1,
        //             DayOneAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayOneDate.Day, attendanceNew.DayOneDate.Month, attendanceNew.DayOneDate.Year) + "(" + attendanceNew.DayOneDate.ToString("dddd") + ")",
        //             IsPresentMorning1 = attendanceNew.IsPresentMorning1,
        //             IsPresentAfternoon1 = attendanceNew.IsPresentAfternoon1,
        //             DayTwoDate = attendanceNew.DayTwoDate,
        //             DayTwoAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayTwoDate.Day, attendanceNew.DayTwoDate.Month, attendanceNew.DayTwoDate.Year) + "(" + attendanceNew.DayTwoDate.ToString("dddd") + ")",
        //             MorningAttendance2 = attendanceNew.MorningAttendance2,
        //             AfternoonAttendance2 = attendanceNew.AfternoonAttendance2,
        //             IsPresentMorning2 = attendanceNew.IsPresentMorning2,
        //             IsPresentAfternoon2 = attendanceNew.IsPresentAfternoon2,
        //             DayThreeDate = attendanceNew.DayThreeDate,
        //             DayThreeAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayThreeDate.Day, attendanceNew.DayThreeDate.Month, attendanceNew.DayThreeDate.Year) + "(" + attendanceNew.DayThreeDate.ToString("dddd") + ")",
        //             MorningAttendance3 = attendanceNew.MorningAttendance3,
        //             AfternoonAttendance3 = attendanceNew.AfternoonAttendance3,
        //             IsPresentMorning3 = attendanceNew.IsPresentMorning3,
        //             IsPresentAfternoon3 = attendanceNew.IsPresentAfternoon3,
        //             DayFourDate = attendanceNew.DayFourDate,
        //             DayFourAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFourDate.Day, attendanceNew.DayFourDate.Month, attendanceNew.DayFourDate.Year) + "(" + attendanceNew.DayFourDate.ToString("dddd") + ")",
        //             MorningAttendance4 = attendanceNew.MorningAttendance4,
        //             AfternoonAttendance4 = attendanceNew.AfternoonAttendance4,
        //             IsPresentMorning4 = attendanceNew.IsPresentMorning4,
        //             IsPresentAfternoon4 = attendanceNew.IsPresentAfternoon4,
        //             DayFiveDate = attendanceNew.DayFiveDate,
        //             DayFiveAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFiveDate.Day, attendanceNew.DayFiveDate.Month, attendanceNew.DayFiveDate.Year) + "(" + attendanceNew.DayFiveDate.ToString("dddd") + ")",
        //             MorningAttendance5 = attendanceNew.MorningAttendance5,
        //             AfternoonAttendance5 = attendanceNew.AfternoonAttendance5,
        //             IsPresentMorning5 = attendanceNew.IsPresentMorning5,
        //             IsPresentAfternoon5 = attendanceNew.IsPresentAfternoon5,
        //             DaySixDate = attendanceNew.DaySixDate,
        //             DaySixAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DaySixDate.Day, attendanceNew.DaySixDate.Month, attendanceNew.DaySixDate.Year) + "(" + attendanceNew.DaySixDate.ToString("dddd") + ")",
        //             MorningAttendance6 = attendanceNew.MorningAttendance6,
        //             AfternoonAttendance6 = attendanceNew.AfternoonAttendance6,
        //             IsPresentMorning6 = attendanceNew.IsPresentMorning6,
        //             IsPresentAfternoon6 = attendanceNew.IsPresentAfternoon6,
        //             IsApproved = attendanceNew.IsApproved,
        //             ApproverEmpId = attendanceNew.ApproverEmpId,
        //             IsDepartmentApproved = attendanceNew.IsDepartmentApproved,
        //             DepartmentApproverId = attendanceNew.DepartmentApproverId,
        //         });

        //         return Json(result);
        //     }

        public ActionResult AttendanceNewHR_Read([DataSourceRequest]DataSourceRequest request, string date)
        {

            DateTime attendanceDate = DateTime.Today;

            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
            }

            IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x => x.Date == attendanceDate && x.IsDepartmentApproved);

            DataSourceResult result = attendancenews.ToDataSourceResult(request, attendanceNew => new
            {
                AttendanceNewId = attendanceNew.AttendanceNewId,
                EmployeeId = attendanceNew.EmployeeId,
                OrgStructureId = GetDepartment(attendanceNew.EmployeeId),
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
        public int GetDepartment(int id)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                return placement.OrgStructureId;
            }
            return 0;
        }
        public JsonResult ApproveAttendance(string date)
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
                var attendances = db.AttendanceNews.Where(x => weekDates.Contains(attendanceDate) && x.IsDepartmentApproved).ToList();
                foreach (var att in attendances)
                {
                    att.IsApproved = true;
                    int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    att.ApproverEmpId = empId;

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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
