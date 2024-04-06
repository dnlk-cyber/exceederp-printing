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

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
    public class AttendanceNewForWeekHRController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["EmployeeList"] = db.Person_Employee.AsQueryable();
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();

            return View();
        }

        public ActionResult AttendanceNewForWeekHR_Read([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime attendanceDate = DateTime.Today;
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
            var attendancenewRecords = db.AttendanceNews.ToList().Where(x => x.IsDepartmentApproved).Where(x => weekDates.Contains(x.Date) /*x.Date == attendanceDate*/);
            var attendancenewLists = attendancenewRecords.GroupBy(x => x.EmployeeId)
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
                IsPresentAfternoon1 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Monday).IsPresentAfternoon
                    : false,
                DayTwoDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).Date
                    : DateTime.MinValue,
                IsPresentMorning2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentMorning
                    : false,
                IsPresentAfternoon2 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Tuesday).IsPresentAfternoon
                    : false,
                DayThreeDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).Date
                    : DateTime.MinValue,
                IsPresentMorning3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentMorning
                    : false,
                IsPresentAfternoon3 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Wednesday).IsPresentAfternoon
                    : false,
                DayFourDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).Date
                    : DateTime.MinValue,
                IsPresentMorning4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentMorning
                    : false,
                IsPresentAfternoon4 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Thursday).IsPresentAfternoon
                    : false,
                DayFiveDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).Date
                    : DateTime.MinValue,
                IsPresentMorning5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentMorning
                    : false,
                IsPresentAfternoon5 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Friday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Friday).IsPresentAfternoon
                    : false,
                DaySixDate = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).Date
                    : DateTime.MinValue,
                IsPresentMorning6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentMorning
                    : false,
                IsPresentAfternoon6 = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsPresentAfternoon
                    : false,
                IsDepartmentApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday || ar.Date.DayOfWeek == DayOfWeek.Tuesday
                                                    || ar.Date.DayOfWeek == DayOfWeek.Wednesday || ar.Date.DayOfWeek == DayOfWeek.Thursday
                                                    || ar.Date.DayOfWeek == DayOfWeek.Friday || ar.Date.DayOfWeek == DayOfWeek.Saturday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsDepartmentApproved: false,
                IsApproved = group.Any(ar => ar.Date.DayOfWeek == DayOfWeek.Monday || ar.Date.DayOfWeek == DayOfWeek.Tuesday
                                                   || ar.Date.DayOfWeek == DayOfWeek.Wednesday || ar.Date.DayOfWeek == DayOfWeek.Thursday
                                                   || ar.Date.DayOfWeek == DayOfWeek.Friday || ar.Date.DayOfWeek == DayOfWeek.Saturday)
                    ? group.FirstOrDefault(ar => ar.Date.DayOfWeek == DayOfWeek.Saturday).IsApproved: false,
                IsPresentAfternoon = true,
                IsPresentMorning = true,
                // Add other properties as needed
            })
            .ToList();
            //IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => weekDates.Contains(x.Date) /*x.Date == attendanceDate*/);

            DataSourceResult result = attendancenewLists.ToDataSourceResult(request, attendanceNew => new
            {
                AttendanceNewId = attendanceNew.AttendanceNewId,
                EmployeeId = attendanceNew.EmployeeId,
                OrgStructureId = GetDepartment(attendanceNew.EmployeeId),
                Date = attendanceNew.Date,
                DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.Date.Day, attendanceNew.Date.Month, attendanceNew.Date.Year) + "(" + attendanceNew.Date.ToString("dddd") + ")",
                IsPresentMorning = attendanceNew.IsPresentMorning,
                IsPresentAfternoon = attendanceNew.IsPresentAfternoon,
                DayOneDate = attendanceNew.DayOneDate,
                DayOneAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayOneDate.Day, attendanceNew.DayOneDate.Month, attendanceNew.DayOneDate.Year) + "(" + attendanceNew.DayOneDate.ToString("dddd") + ")",
                IsPresentMorning1 = attendanceNew.IsPresentMorning1,
                IsPresentAfternoon1 = attendanceNew.IsPresentAfternoon1,
                DayTwoDate = attendanceNew.DayTwoDate,
                DayTwoAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayTwoDate.Day, attendanceNew.DayTwoDate.Month, attendanceNew.DayTwoDate.Year) + "(" + attendanceNew.DayTwoDate.ToString("dddd") + ")",
                IsPresentMorning2 = attendanceNew.IsPresentMorning2,
                IsPresentAfternoon2 = attendanceNew.IsPresentAfternoon2,
                DayThreeDate = attendanceNew.DayThreeDate,
                DayThreeAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayThreeDate.Day, attendanceNew.DayThreeDate.Month, attendanceNew.DayThreeDate.Year) + "(" + attendanceNew.DayThreeDate.ToString("dddd") + ")",
                IsPresentMorning3 = attendanceNew.IsPresentMorning3,
                IsPresentAfternoon3 = attendanceNew.IsPresentAfternoon3,
                DayFourDate = attendanceNew.DayFourDate,
                DayFourAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFourDate.Day, attendanceNew.DayFourDate.Month, attendanceNew.DayFourDate.Year) + "(" + attendanceNew.DayFourDate.ToString("dddd") + ")",
                IsPresentMorning4 = attendanceNew.IsPresentMorning4,
                IsPresentAfternoon4 = attendanceNew.IsPresentAfternoon4,
                DayFiveDate = attendanceNew.DayFiveDate,
                DayFiveAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DayFiveDate.Day, attendanceNew.DayFiveDate.Month, attendanceNew.DayFiveDate.Year) + "(" + attendanceNew.DayFiveDate.ToString("dddd") + ")",
                IsPresentMorning5 = attendanceNew.IsPresentMorning5,
                IsPresentAfternoon5 = attendanceNew.IsPresentAfternoon5,
                DaySixDate = attendanceNew.DaySixDate,
                DaySixAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(attendanceNew.DaySixDate.Day, attendanceNew.DaySixDate.Month, attendanceNew.DaySixDate.Year) + "(" + attendanceNew.DaySixDate.ToString("dddd") + ")",
                IsPresentMorning6 = attendanceNew.IsPresentMorning6,
                IsPresentAfternoon6 = attendanceNew.IsPresentAfternoon6,
                IsApproved = attendanceNew.IsApproved,
                ApproverEmpId = attendanceNew.ApproverEmpId,
                IsDepartmentApproved = attendanceNew.IsDepartmentApproved,
                DepartmentApproverId = attendanceNew.DepartmentApproverId,
            });

            return Json(result);
        }

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
