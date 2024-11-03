using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Telerik.Reporting;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Attendance
{
    [AuthorizeRoles(HrmConstants.HRAttendanceUser,
      HrmConstants.HRDepartmentHead,
      HrmConstants.HREmployeeSelfService)]
    public class PreviousAttendanceController : Controller
    {
        static int Period { get; set; }
        static int Week { get; set; }
        static int Employee { get; set; }
        // GET: HRM/PreviousAttendance
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var empIdList = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(empId);
            var list = inputs.GetEmployees().ToList();//.Where(x => empIdList.Contains(x.PersonId) && x.PersonId != empId).ToList();
            ViewData["EmployeeList"] = list;
            ViewData["AllEmployeeList"] = inputs.GetAllPlacmentEmployees(User.Identity.Name);
            // GenerateCalander(list);
        }
        public ActionResult PreviousAttendance_Read([DataSourceRequest]DataSourceRequest request, string period, string week, string employee)
        {
           
            var weeklyattendances = db.WeeklyAttendances.ToList();
            var currentDate = DateTime.Today;
            var glYear = db.GLFiscalYears.FirstOrDefault(x=>x.DateFrom <= currentDate && x.DateTo >= currentDate);
            int yid = 0;
            if(glYear != null)
            {
                yid = glYear.GlFiscalYearId;
            }

            if (period != null)
            {
                weeklyattendances = weeklyattendances.Where(x =>x.GlFiscalYearId == yid && x.GLPeriodId.ToString() == period).ToList();
            }
            if (week != null)
            {
                weeklyattendances = weeklyattendances.Where(x => x.Week.ToString() == week).ToList();
            }
            if (employee != "")
            {
                weeklyattendances = weeklyattendances.Where(x => x.EmployeeId.ToString() == employee).ToList();
            }

            DataSourceResult result = weeklyattendances.ToDataSourceResult(request, weeklyAttendance => new
            {
                WeeklyAttendanceId = weeklyAttendance.WeeklyAttendanceId,
                DayOne = weeklyAttendance.DayOne,
                EmployeeId = weeklyAttendance.EmployeeId,
                DayOneDate = weeklyAttendance.DayOneDate,
                DayTwo = weeklyAttendance.DayTwo,
                DayTwoDate = weeklyAttendance.DayTwoDate,
                DayThree = weeklyAttendance.DayThree,
                DayThreeDate = weeklyAttendance.DayThreeDate,
                DayFour = weeklyAttendance.DayFour,
                DayFourDate = weeklyAttendance.DayFourDate,
                DayFive = weeklyAttendance.DayFive,
                DayFiveDate = weeklyAttendance.DayFiveDate,
                DaySix = weeklyAttendance.DaySix,
                DaySixDate = weeklyAttendance.DaySixDate,
                DaySeven = weeklyAttendance.DaySeven,
                DaySevenDate = weeklyAttendance.DaySevenDate,
                Remark = weeklyAttendance.Remark,
                Week = weeklyAttendance.Week,
                Month = weeklyAttendance.Month,
                Year = weeklyAttendance.Year,
                DayOneAmharicDate = weeklyAttendance.DayOneAmharicDate,
                DayTwoAmharicDate = weeklyAttendance.DayTwoAmharicDate,
                DayThreeAmharicDate = weeklyAttendance.DayThreeAmharicDate,
                DayFourAmharicDate = weeklyAttendance.DayFourAmharicDate,
                DayFiveAmharicDate = weeklyAttendance.DayFiveAmharicDate,
                DaySixAmharicDate = weeklyAttendance.DaySixAmharicDate,
                DaySevenAmharicDate = weeklyAttendance.DaySevenAmharicDate,
                DayOneAN = weeklyAttendance.DayOneAN,
                DayTwoAN = weeklyAttendance.DayTwoAN,
                DayThreeAN = weeklyAttendance.DayThreeAN,
                DayFourAN = weeklyAttendance.DayFourAN,
                DayFiveAN = weeklyAttendance.DayFiveAN,
                DaySixAN = weeklyAttendance.DaySixAN,
                DaySevenAN = weeklyAttendance.DaySevenAN,
            });

            return Json(result);
        }
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<WeeklyAttendance> attendances)
        {
            ModelState.Clear();
            var results = new List<WeeklyAttendance>();
            ExceedDbContext dbs = new ExceedDbContext();
            var info = "Emplyee/s in leave    ";
            var success = "===Succussfull=== ";
            bool IsLeave = false;
            var message = "";



            if (attendances.Any() && ModelState.IsValid)
            {
                foreach (var weeklyAttendance in attendances)
                {
                    var prevAttendance =
                        db.WeeklyAttendances.FirstOrDefault(
                            x => x.WeeklyAttendanceId == weeklyAttendance.WeeklyAttendanceId);

                    var todayMonth = DateTime.Now.Date.Month;

                    var one = prevAttendance.DayOneDate.Value.Month;
                    var two = prevAttendance.DayTwoDate.Value.Month;
                    var three = prevAttendance.DayThreeDate.Value.Month;
                    var four = prevAttendance.DayFourDate.Value.Month;
                    var five = prevAttendance.DayFiveDate.Value.Month;
                    var six = prevAttendance.DaySixDate.Value.Month;
                    var seven = prevAttendance.DaySevenDate.Value.Month;

                    //var one = weeklyAttendance.DayOneDate.Value.Month;
                    //var two = weeklyAttendance.DayTwoDate.Value.Month;
                    //var three = weeklyAttendance.DayThreeDate.Value.Month;
                    //var four = weeklyAttendance.DayFourDate.Value.Month;
                    //var five = weeklyAttendance.DayFiveDate.Value.Month;
                    //var six = weeklyAttendance.DaySixDate.Value.Month;
                    //var seven = weeklyAttendance.DaySevenDate.Value.Month;

                    #region check it again 
                    if (one < todayMonth)
                    {
                        continue;
                    }
                    if (two < todayMonth)
                    {
                        continue;
                    }
                    if (three < todayMonth)
                    {
                        continue;
                    }
                    if (four < todayMonth)
                    {
                        continue;
                    }
                    if (five < todayMonth)
                    {
                        continue;
                    }
                    if (six < todayMonth)
                    {
                        continue;
                    }
                    if (seven < todayMonth)
                    {
                        continue;
                    }
                    #endregion
                    if (prevAttendance != null)
                    {


                        prevAttendance.DayOne = weeklyAttendance.DayOne;
                        prevAttendance.DayTwo = weeklyAttendance.DayTwo;
                        prevAttendance.DayThree = weeklyAttendance.DayThree;
                        prevAttendance.DayFour = weeklyAttendance.DayFour;
                        prevAttendance.DayFive = weeklyAttendance.DayFive;
                        prevAttendance.DaySix = weeklyAttendance.DaySix;
                        prevAttendance.DaySeven = weeklyAttendance.DaySeven;

                        prevAttendance.DayOneAN = weeklyAttendance.DayOneAN;
                        prevAttendance.DayTwoAN = weeklyAttendance.DayTwoAN;
                        prevAttendance.DayThreeAN = weeklyAttendance.DayThreeAN;
                        prevAttendance.DayFourAN = weeklyAttendance.DayFourAN;
                        prevAttendance.DayFiveAN = weeklyAttendance.DayFiveAN;
                        prevAttendance.DaySixAN = weeklyAttendance.DaySixAN;
                        prevAttendance.DaySevenAN = weeklyAttendance.DaySevenAN;



                        var splitDateOne = weeklyAttendance.DayOneAmharicDate.Split('/');
                        DateTime oneFrom = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateOne[0]), Convert.ToInt32(splitDateOne[1]), Convert.ToInt32(splitDateOne[2]));

                        var leaveOne = db.Leaves.FirstOrDefault(x => x.EmployeeId == weeklyAttendance.EmployeeId && x.From <= oneFrom && x.To >= oneFrom);
                        if (leaveOne != null && (prevAttendance.DayOne == false || prevAttendance.DayOneAN == false))
                        {

                            prevAttendance.DayOne = true;
                            prevAttendance.DayOneAN = true;
                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveOne.EmployeeId);
                            if (employee != null)
                            {
                                message = message + employee.FullNameaEnglish + ", ";
                                IsLeave = true;
                            }
                            else
                            {
                                message = success;
                            }
                        }


                        //////////////////////////////////

                        var splitDateTwo = weeklyAttendance.DayTwoAmharicDate.Split('/');
                        DateTime twoFrom = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTwo[0]), Convert.ToInt32(splitDateTwo[1]), Convert.ToInt32(splitDateTwo[2]));


                        var leaveTwo = db.Leaves.FirstOrDefault(x => x.EmployeeId == weeklyAttendance.EmployeeId && x.From <= twoFrom && x.To >= twoFrom);
                        if (leaveTwo != null && (prevAttendance.DayTwo == false || prevAttendance.DayTwoAN == false))
                        {
                            prevAttendance.DayTwo = true;
                            prevAttendance.DayTwoAN = true;

                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveTwo.EmployeeId);
                            if (employee != null)
                            {
                                message = message + employee.FullNameaEnglish + ", ";
                                IsLeave = true;
                            }
                            else
                            {
                                message = success;
                            }
                        }



                        /////////////////////////////////////////////////////////////////////////////////////////
                        var splitDateThree = weeklyAttendance.DayThreeAmharicDate.Split('/');
                        DateTime threeFrom = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateThree[0]), Convert.ToInt32(splitDateThree[1]), Convert.ToInt32(splitDateThree[2]));

                        var leaveThree = db.Leaves.FirstOrDefault(x => x.EmployeeId == weeklyAttendance.EmployeeId && x.From <= threeFrom && x.To >= threeFrom);
                        if (leaveThree != null && (prevAttendance.DayThree == false || prevAttendance.DayThreeAN == false))
                        {
                            prevAttendance.DayThree = true;
                            prevAttendance.DayThreeAN = true;

                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveThree.EmployeeId);
                            if (employee != null)
                            {
                                message = message + employee.FullNameaEnglish + ", ";
                                IsLeave = true;
                            }
                            else
                            {
                                message = success;
                            }
                        }

                        //////////////////////////////////////////////////
                        var splitDateFour = weeklyAttendance.DayFourAmharicDate.Split('/');
                        DateTime fourFrom = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFour[0]), Convert.ToInt32(splitDateFour[1]), Convert.ToInt32(splitDateFour[2]));

                        var leaveFour = db.Leaves.FirstOrDefault(x => x.EmployeeId == weeklyAttendance.EmployeeId && x.From <= fourFrom && x.To >= fourFrom);
                        if (leaveFour != null && (prevAttendance.DayFour == false || prevAttendance.DayFourAN == false))
                        {
                            prevAttendance.DayFour = true;
                            prevAttendance.DayFourAN = true;
                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveFour.EmployeeId);
                            if (employee != null)
                            {
                                message = message + employee.FullNameaEnglish + ", ";
                                IsLeave = true;

                            }
                            else
                            {
                                message = success;
                            }

                        }

                        /////////////////////////////////////////////////////////////
                        var splitDateFive = weeklyAttendance.DayFiveAmharicDate.Split('/');
                        DateTime fiveFrom = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFive[0]), Convert.ToInt32(splitDateFive[1]), Convert.ToInt32(splitDateFive[2]));

                        var leaveFive = db.Leaves.FirstOrDefault(x => x.EmployeeId == weeklyAttendance.EmployeeId && x.From <= fiveFrom && x.To >= fiveFrom);
                        if (leaveFive != null && (prevAttendance.DayFive == false || prevAttendance.DayFiveAN == false))
                        {
                            prevAttendance.DayFive = true;
                            prevAttendance.DayFiveAN = true;

                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveFive.EmployeeId);
                            if (employee != null)
                            {
                                message = message + employee.FullNameaEnglish + ", ";
                                IsLeave = true;
                            }
                            else
                            {
                                message = success;
                            }
                        }

                        /////////////////////////////////////////////////////////////////////////
                        var splitDateSix = weeklyAttendance.DaySixAmharicDate.Split('/');
                        DateTime sixFrom = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateSix[0]), Convert.ToInt32(splitDateSix[1]), Convert.ToInt32(splitDateSix[2]));

                        var leaveSix = db.Leaves.FirstOrDefault(x => x.EmployeeId == weeklyAttendance.EmployeeId && x.From <= sixFrom && x.To >= sixFrom);
                        if (leaveSix != null && (prevAttendance.DaySix == false || prevAttendance.DaySixAN == false))
                        {
                            prevAttendance.DaySix = true;
                            prevAttendance.DaySixAN = true;

                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveSix.EmployeeId);
                            if (employee != null)
                            {
                                message = message + employee.FullNameaEnglish + ", ";
                                IsLeave = true;
                            }
                            else
                            {
                                message = success;
                            }
                        }

                        ////////////////////////////////////////////////////////////////////
                        var splitDateSeven = weeklyAttendance.DaySevenAmharicDate.Split('/');
                        DateTime sevenFrom = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateSeven[0]), Convert.ToInt32(splitDateSeven[1]), Convert.ToInt32(splitDateSeven[2]));

                        var leaveSeven = db.Leaves.FirstOrDefault(x => x.EmployeeId == weeklyAttendance.EmployeeId && x.From <= sevenFrom && x.To >= sevenFrom);
                        if (leaveSeven != null && (prevAttendance.DaySeven == false || prevAttendance.DaySevenAN == false))
                        {
                            prevAttendance.DaySeven = true;
                            prevAttendance.DaySevenAN = true;

                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveSeven.EmployeeId);
                            if (employee != null)
                            {
                                message = message + employee.FullNameaEnglish + ", ";
                                IsLeave = true;
                            }
                            else
                            {
                                message = success;
                            }
                        }

                        ////////////////////////////////////////////

                        db.WeeklyAttendances.Attach(prevAttendance);
                        db.Entry(prevAttendance).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
                if (IsLeave == true)
                {
                    message = info + message;
                    return new JavaScriptResult() { Script = "alert('" + message + "');" };
                }
                else
                {
                    return new JavaScriptResult() { Script = "alert('" + success + "');" };
                }



            }


            return Json(results.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetMonths()
        {

            List<string> monthList = new List<string>();

            monthList.Add("September");
            monthList.Add("October");
            monthList.Add("November");
            monthList.Add("December");
            monthList.Add("January");
            monthList.Add("February");
            monthList.Add("March");
            monthList.Add("April");
            monthList.Add("May");
            monthList.Add("June");
            monthList.Add("July");
            monthList.Add("Augest");


            return Json(monthList.Select(p =>
            new
            {
                Id = p,
                Name = p

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGlPeriods()
        {

            var periods = db.GLPeriods.ToList();
            return Json(periods.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPeriod(int period)
        {
            Period = period;

            return Index();
        }
        public ActionResult GetWeek(int week)
        {
            Week = week;

            return Index();
        }
        public ActionResult GetEmployee(int employee)
        {
            Employee = employee;

            return Index();
        }
        public JsonResult GetWeeks()
        {

            List<string> weekList = new List<string>();

            weekList.Add("1");
            weekList.Add("2");
            weekList.Add("3");
            weekList.Add("4");
            weekList.Add("5");
            weekList.Add("6");


            return Json(weekList.Select(p => new
            {
                Id = p,
                Name = p

            }), JsonRequestBehavior.AllowGet);
        }
    }
}