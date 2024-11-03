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
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Hubs;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
    [AuthorizeRoles(HrmConstants.HRAttendanceUser,HrmConstants.HRAdmin)]

    public class AttendanceNewHRController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["EmployeeList"] = db.Person_Employee.Select(x=>new { x.EmployeeId, FullNameaEnglish =x.FirstNameEnglish+" "+x.MiddleNameEnglish+" "+x.LastNameEnglish}).AsQueryable();
            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();
            UpdateDepartment();

            return View();
        }
        private void UpdateDepartment()
        {
            var attendancenews = db.AttendanceNews.Where(x => x.OrgStructureId == 0).ToList();
            foreach(var attendanceNew in attendancenews)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == attendanceNew.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    attendanceNew.OrgStructureId = placement.OrgStructureId;
                    db.AttendanceNews.Attach(attendanceNew);
                    db.Entry(attendanceNew).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }


        }
        public ActionResult AttendanceNews_Read([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime attendanceDate = DateTime.Today;

            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
            }
            List<int> branchIds = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            List<int> EmployeeIds = new List<int>(db.Placements.Where(x => x.isCurrent && branchIds.Contains(x.BranchId)).Select(x => x.EmployeeId));
            IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x=> EmployeeIds.Contains(x.EmployeeId)).Where(x => x.Date == attendanceDate && x.IsDepartmentApproved);

            DataSourceResult result = attendancenews.ToDataSourceResult(request, attendanceNew => new
            {
                AttendanceNewId = attendanceNew.AttendanceNewId,
                EmployeeId = attendanceNew.EmployeeId,
                OrgStructureId = attendanceNew.OrgStructureId,
                Date = attendanceNew.Date,
                MorningAttendance = attendanceNew.MorningAttendance,
                AfternoonAttendance = attendanceNew.AfternoonAttendance,
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

                var attendances = db.AttendanceNews.Where(x => x.Date == attendanceDate && x.IsDepartmentApproved).ToList();
                foreach (var att in attendances)
                {
                    att.IsApproved = true;
                    int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    int orgId = UserInformationHelper.GetEmployeeDepartmentIdByEmpId(empId);
                    att.ApproverEmpId = empId;
                    att.DepartmentApproverId = orgId;
                    db.Entry(att).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    if (att != null)
                    {
                        //NotifModels notif = new NotifModels
                        //{
                        //    UserId = UserInformationHelper.GetEmployeeUserId(att.EmployeeId),
                        //    Title = "attendance ",
                        //    Message = "Your attendance  is Approved"
                        //};

                        //Notify.NotifyUser(notif);
                    }
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
