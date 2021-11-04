using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = HR_Attendance_User + "," + HR_Admin)]
    public class EmpPayrollNoDaysWorkedController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Attendance_User = "HR_Attendance_User";
        private const string HR_Admin = "HR_Admin";
        HRMInputs inputs = new HRMInputs();
        private static List<EmployeeIdView> List { get; set; }
        public ActionResult Index()
        {
            List = new List<EmployeeIdView>();
            GetPeriod();
            List = inputs.GetPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = List.Where(x => x.IsActive);
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["CurrentYearPeriods"] = db.GLPeriods.ToList();
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
            ViewData["PeriodList"] = listt.Select(x => x.g).ToList(); ;

            return Json(listt.Select(p => new { GLPeriodId = p.g.GLPeriodId, Name = p.g.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentYearPeriods()
        {
            List<GLPeriod> gLPeriods = new List<GLPeriod>();

            var periodList = db.GLPeriods.ToList();
            var glyear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now);
            if (glyear != null)
            {
                var periodLists = periodList.Where(x => x.GlFiscalYearId == glyear.GlFiscalYearId).ToList();
                foreach (var pl in periodLists)
                {
                    var checkexistancep = db.PayrollProcessViewModel.FirstOrDefault(x => x.GLPeriodId == pl.GLPeriodId);
                    if (checkexistancep == null)
                    {
                        gLPeriods.Add(pl);
                    }
                }

            }



            return Json(gLPeriods.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Summary(int period)
        {
            double count = 0;
            var perioddetail = db.PayrollPeriods.Where(x => x.GLPeriodId == period).FirstOrDefault();
            if (perioddetail != null)
            {
                var checkRegistredAttendance =
                    db.WeeklyAttendances.Where(x => x.Month == perioddetail.DateFrom.Month).FirstOrDefault();
                if (checkRegistredAttendance == null)
                {
                    return new JavaScriptResult() { Script = "alert('" + "There is no registered Attendance!!" + "');" };
                }
                var checkexistancep = db.PayrollProcessViewModel.FirstOrDefault(x => x.GLPeriodId == period);
                if (checkexistancep != null)
                {

                    // ModelState.AddModelError("", "You cannot create this data. It already processed!!");
                    return new JavaScriptResult() { Script = "alert('" + "You cannot create this data. It already processed!!" + "');" };
                }
                foreach (var l in List)
                {
                    var check = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.EmployeeId == l.PersonId && x.GLPeriodId == period);
                    if (check != null)
                    {
                        db.EmpPayrollNoDaysWorkeds.Attach(check);
                        db.EmpPayrollNoDaysWorkeds.Remove(check);
                        db.SaveChanges();
                    }
                    count = GetAbscentDays(l.PersonId, perioddetail.DateFrom, perioddetail.DateTo);
                    if (count > 0)
                    {


                        var entity = new EmpPayrollNoDaysWorked
                        {
                            EmployeeId = l.PersonId,
                            GLPeriodId = period,
                            Days = Convert.ToDecimal(count),
                            Remark = null,
                            DateCreated = DateTime.Now,
                            LastModified = null,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            Reason = null,
                            MeasureTaken = null,
                        };
                        db = new ExceedDbContext();
                        db.EmpPayrollNoDaysWorkeds.Add(entity);
                        db.SaveChanges();
                    }

                }
            }
            return null;
        }

        public ActionResult RefreshNew(int period)
        {
            var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == period).FirstOrDefault();
            if (checkexistancep != null)
            {
                var error = "You cannot create this data. It already processed!!";
                return new JavaScriptResult() { Script = "alert('" + error + "');" };
            }

            var attendances = db.AttendanceNews.Where(x => x.GlPeriodId == period).Where(x => !x.IsPresentMorning || !x.IsPresentAfternoon).ToList();
            foreach (var att in attendances)
            {
                double day = 0;
                if (!att.IsPresentMorning && !att.IsPresentAfternoon)
                {
                    day = 1;
                }
                else if (!att.IsPresentMorning || !att.IsPresentAfternoon)
                {
                    day = 0.5;
                }

                var leave = db.Leaves.FirstOrDefault(x => x.EmployeeId == att.EmployeeId && x.From <= att.Date && x.To >= att.Date);
                if (leave != null)
                {
                    if (leave.Day == LeaveDay.ሙሉ_ቀን)
                    {
                        day = day - 1;
                    }
                    else if (leave.Day == LeaveDay.ግማሽ_ቀን)
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

            return new JavaScriptResult() { Script = "alert('" + "Employee Absent days are successfully generated" + "');" };
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
        private double GetAbscentDays(int empId, DateTime startDate, DateTime endDate)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            double count = 0;
            var weeklyAttendances = db.WeeklyAttendances.Where(x => x.EmployeeId == empId && x.Year == DateTime.Now.Year && employees.Contains(x.EmployeeId.ToString())).ToList();
            foreach (var w in weeklyAttendances)
            {
                if (startDate <= w.DayOneDate && endDate >= w.DayOneDate)
                {
                    if (!w.DayOne)
                    {
                        count = count + 0.5;
                    }
                    if (!w.DayOneAN)
                    {
                        count = count + 0.5;
                    }
                }
                if (startDate <= w.DayTwoDate && endDate >= w.DayTwoDate)
                {
                    if (!w.DayTwo)
                    {
                        count = count + 0.5;
                    }
                    if (!w.DayTwoAN)
                    {
                        count = count + 0.5;
                    }
                }
                if (startDate <= w.DayThreeDate && endDate >= w.DayThreeDate)
                {
                    if (!w.DayThree)
                    {
                        count = count + 0.5;
                    }
                    if (!w.DayThreeAN)
                    {
                        count = count + 0.5;
                    }
                }
                if (startDate <= w.DayFourDate && endDate >= w.DayFourDate)
                {
                    if (!w.DayFour)
                    {
                        count = count + 0.5;
                    }
                    if (!w.DayFourAN)
                    {
                        count = count + 0.5;
                    }
                }
                if (startDate <= w.DayFiveDate && endDate >= w.DayFiveDate)
                {
                    if (!w.DayFive)
                    {
                        count = count + 0.5;
                    }
                    if (!w.DayFiveAN)
                    {
                        count = count + 0.5;
                    }
                }
                if (startDate <= w.DaySixDate && endDate >= w.DaySixDate)
                {
                    if (!w.DaySix)
                    {
                        count = count + 0.5;
                    }
                    if (!w.DaySixAN)
                    {
                        count = count + 0.5;
                    }
                }
                if (startDate <= w.DaySevenDate && endDate >= w.DaySevenDate)
                {
                    if (!w.DaySeven)
                    {
                        count = count + 0.5;
                    }
                    if (!w.DaySevenAN)
                    {
                        count = count + 0.5;
                    }
                }
            }
            return count;
        }
        [Authorize(Roles = HR_Attendance_User + "," + HR_Admin)]
        public ActionResult EmpPayrollNoDaysWorkeds_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<EmpPayrollNoDaysWorked> emppayrollnodaysworkeds = db.EmpPayrollNoDaysWorkeds.Where(x => employees.Contains(x.EmployeeId.ToString()) && x.Days > 0);
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
        public ActionResult EmpPayrollNoDaysWorkeds_Create([DataSourceRequest]DataSourceRequest request, EmpPayrollNoDaysWorked empPayrollNoDaysWorked)
        {
            ModelState.Clear();
            var checkEmp = db.EmpPayrollNoDaysWorkeds.FirstOrDefault(x => x.EmployeeId == empPayrollNoDaysWorked.EmployeeId && x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId);
            if (checkEmp != null)
            {
                ModelState.AddModelError("", "this employee is already recoreded");
            }

            var checkexistancep = db.PayrollProcessViewModel.FirstOrDefault(x => x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId);
            if (checkexistancep != null)
            {
                ModelState.AddModelError("", "You cannot create this data. It already processed!!");
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
        public ActionResult EmpPayrollNoDaysWorkeds_Update([DataSourceRequest]DataSourceRequest request, EmpPayrollNoDaysWorked empPayrollNoDaysWorked)
        {
            ModelState.Clear();

            var checkexistance = db.PayrollProcessViewModel.FirstOrDefault(x => x.EmployeeId == empPayrollNoDaysWorked.EmployeeId && x.GLPeriodId == empPayrollNoDaysWorked.GLPeriodId);
            if (checkexistance != null)
            {

                ModelState.AddModelError("", "You cannot delete this data. It already processed!!");
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
        public ActionResult EmpPayrollNoDaysWorkeds_Destroy([DataSourceRequest]DataSourceRequest request, EmpPayrollNoDaysWorked empPayrollNoDaysWorked)
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
