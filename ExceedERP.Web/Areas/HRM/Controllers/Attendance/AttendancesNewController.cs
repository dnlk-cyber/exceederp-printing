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
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{

    public class AttendancesNewController : Controller
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
            ApplicationDbContext adb = new ApplicationDbContext();

            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent && x.IsHead && x.IsApprove);
            if (placement != null)
            {
                List<int> employeesId = GetEmployeesUnderBoss(empId);

                foreach (var emp in employeesId)
                {
                    var check = db.AttendanceNews.FirstOrDefault(x => x.EmployeeId == emp && x.Date == date);
                    if (check == null)
                    {
                        var attendance = new AttendanceNew
                        {
                            EmployeeId = emp,
                            OrgStructureId = placement.OrgStructureId,
                            IsPresentMorning = true,
                            IsPresentAfternoon = true,
                            MorningAttendance = TakeAttendance.Present,
                            AfternoonAttendance = TakeAttendance.Present,
                            Date = date,
                            IsDepartmentApproved = false,
                            IsApproved = false
                        };

                        attendance.DateAm = XAPI.EthiopicDateTime.GetEthiopicDate(date.Day, date.Month, date.Year);
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
        public List<int> GetEmployeesUnderBoss(int id)
        {
            List<int> employeesIdList = new List<int>();

            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                var placements = db.Placements.Where(x => x.OrgStructureId == placement.OrgStructureId &&x.BranchId==placement.BranchId&& x.isCurrent);
                if (placements.Any())
                {
                    var empIdList = placements.Select(x => x.EmployeeId).ToList();
                    employeesIdList.AddRange(empIdList);
                }

                var employeesUnderThisParent = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId).ToList();
                foreach (var und in employeesUnderThisParent)
                {
                    var employeesPlacedInThisParent = db.Placements.Where(x => x.OrgStructureId == und.OrgStructureId&& x.BranchId == placement.BranchId).ToList();
                    if (employeesPlacedInThisParent.Any())
                    {
                        var empIds = employeesPlacedInThisParent.Select(x => x.EmployeeId).ToList();

                        employeesIdList.AddRange(empIds);
                    }

                }

            }

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
        public ActionResult AttendanceNews_Read([DataSourceRequest]DataSourceRequest request, string date)
        {
            DateTime attendanceDate = DateTime.Today;

            int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employees = GetEmployeesUnderBoss(empId);

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

            IQueryable<AttendanceNew> attendancenews = db.AttendanceNews.Where(x => employees.Contains(x.EmployeeId)).Where(x => x.Date == attendanceDate);

            DataSourceResult result = attendancenews.ToDataSourceResult(request, attendanceNew => new
            {
                AttendanceNewId = attendanceNew.AttendanceNewId,
                EmployeeId = attendanceNew.EmployeeId,
                //OrgStructureId = GetDepartment(attendanceNew.EmployeeId),
                OrgStructureId = attendanceNew.OrgStructureId,
                MorningAttendance = attendanceNew.MorningAttendance,
                AfternoonAttendance = attendanceNew.AfternoonAttendance,
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
        public ActionResult AttendanceNews_Create([DataSourceRequest]DataSourceRequest request, AttendanceNew attendanceNew)
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
                    MorningAttendance = attendanceNew.MorningAttendance,
                    AfternoonAttendance = attendanceNew.AfternoonAttendance,
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
        public ActionResult AttendanceNews_Update([DataSourceRequest]DataSourceRequest request, AttendanceNew attendanceNew)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == attendanceNew.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                attendanceNew.OrgStructureId = placement.OrgStructureId;
            }
            if (attendanceNew.MorningAttendance == TakeAttendance.Absent)
            {
                attendanceNew.IsPresentMorning = false;
            }
            if (attendanceNew.AfternoonAttendance == TakeAttendance.Absent)
            {
                attendanceNew.IsPresentAfternoon = false;
            }
            if (ModelState.IsValid)
            {
                var entity = new AttendanceNew
                {
                    AttendanceNewId = attendanceNew.AttendanceNewId,
                    EmployeeId = attendanceNew.EmployeeId,
                    OrgStructureId = attendanceNew.OrgStructureId,
                    MorningAttendance = attendanceNew.MorningAttendance,
                    AfternoonAttendance = attendanceNew.AfternoonAttendance,
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
        public ActionResult AttendanceNews_Destroy([DataSourceRequest]DataSourceRequest request, AttendanceNew attendanceNew)
        {
            if (ModelState.IsValid)
            {
                var entity = new AttendanceNew
                {
                    AttendanceNewId = attendanceNew.AttendanceNewId,
                    EmployeeId = attendanceNew.EmployeeId,
                    OrgStructureId = attendanceNew.OrgStructureId,
                    MorningAttendance = attendanceNew.MorningAttendance,
                    AfternoonAttendance = attendanceNew.AfternoonAttendance,
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
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AttendanceNew> attendances)
        {
            ModelState.Clear();
            var results = new List<WeeklyAttendance>();

            if (attendances.Any() && ModelState.IsValid)
            {
                foreach (var att in attendances)
                {
                    var attendance = db.AttendanceNews.FirstOrDefault(x => x.AttendanceNewId == att.AttendanceNewId);
                    if (attendance != null)
                    {
                        
                        attendance.MorningAttendance = att.MorningAttendance;
                        attendance.AfternoonAttendance = att.AfternoonAttendance;
                        if(att.MorningAttendance == TakeAttendance.Absent)
                        {
                            attendance.IsPresentMorning = false;
                            attendance.MorningAttendance = TakeAttendance.Absent;
                        }
                        if (att.AfternoonAttendance == TakeAttendance.Absent)
                        {
                            attendance.IsPresentAfternoon = false;
                            attendance.AfternoonAttendance = TakeAttendance.Absent;
                        }
                        if (att.MorningAttendance != TakeAttendance.Absent)
                        {
                            attendance.IsPresentMorning = true;
                        }
                        if (att.AfternoonAttendance != TakeAttendance.Absent)
                        {
                            attendance.IsPresentAfternoon = true;
                        }
                       
                        var leaveTaken = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.Date && x.To >= att.Date);

                        if (leaveTaken != null)
                        {
                            if (leaveTaken.Day == LeaveDay.Half_Day)
                            {
                                if (leaveTaken.MorningOrAfternoon == MorningOrAfternoon.Morning)
                                {
                                    attendance.MorningAttendance = TakeAttendance.Leave;
                                    attendance.IsPresentMorning = true;
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
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }
        public JsonResult DepartmentApproveAttendance(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                DateTime attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

                var attendances = db.AttendanceNews.Where(x => x.Date == attendanceDate).ToList();
                foreach (var att in attendances)
                {
                    att.IsDepartmentApproved = true;
                    int empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    int orgId = UserInformationHelper.GetEmployeeDepartmentIdByEmpId(empId);
                    att.ApproverEmpId = empId;
                    att.DepartmentApproverId = orgId;

                    db.Entry(att).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    if (att != null)
                    {
                        NotifModels notif = new NotifModels
                        {
                            UserId = UserInformationHelper.GetEmployeeUserId(att.EmployeeId),
                            Title = "attendance ",
                            Message = "Your attendance  is Approved by Department"
                        };

                        Notify.NotifyUser(notif);
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
        public JsonResult ReturnAttendance(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var splitDateTo = date.Split('/');
                DateTime attendanceDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));

                var attendances = db.AttendanceNews.Where(x => x.Date == attendanceDate && !x.IsApproved).ToList();

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
                        if (att != null)
                        {
                            NotifModels notif = new NotifModels
                            {
                                UserId = UserInformationHelper.GetEmployeeUserId(att.EmployeeId),
                                Title = "attendance ",
                                Message = "Your attendance  is returned by Department"
                            };

                            Notify.NotifyUser(notif);
                        }
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
