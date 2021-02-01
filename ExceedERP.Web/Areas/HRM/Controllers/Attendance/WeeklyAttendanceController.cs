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
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]

    public class WeeklyAttendanceController : BaseController
    {

        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        private int WeekNumber { get; set; }
        private DateTime WeekStartDate { get; set; }
        private DateTime WeekEndDate { get; set; }
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var empIdList = EmployeeHelper.GetSubordinates(empId);
            var list = inputs.GetEmployees().Where(x => x.IsActive && empIdList.Contains(x.PersonId) && x.PersonId != empId).ToList();
            ViewData["EmployeeList"] = list;
            ViewData["AllEmployeeList"] = inputs.GetAllPlacmentEmployees(User.Identity.Name);
            GenerateCalander(list);

            ViewData["Department"] = db.MainCompanyStructures.AsQueryable();
        }
        private void GenerateCalander(List<EmployeeIdView> list)
        {
            WeekStartDate = FirstDayOfCurrentWeek();
            WeekEndDate = LastDayOfCurrentWeek();
            #region CurrentMonth
            SetEmployeeAttendance(list, DateTime.Now.Month);
            #endregion

            if (DateTime.Now.Month != WeekStartDate.Month)
            {
                #region Week start date Month

                SetEmployeeAttendance(list, WeekStartDate.Month);

                #endregion
            }
            if (DateTime.Now.Month != WeekEndDate.Month)
            {
                #region PreviousMonth

                SetEmployeeAttendance(list, WeekEndDate.Month);

                #endregion
            }
        }
        public DateTime FirstDayOfWeek(DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Sunday)
            {
                candidateDate = candidateDate.AddDays(-1);
            }
            return candidateDate;
        }
        public DateTime FirstDayOfCurrentWeek()
        {
            return FirstDayOfWeek(DateTime.Today);
        }
        public DateTime LastDayOfWeek(DateTime date)
        {
            var candidateDate = date;
            while (candidateDate.DayOfWeek != DayOfWeek.Saturday)
            {
                candidateDate = candidateDate.AddDays(1);
            }
            return candidateDate;
        }
        public DateTime LastDayOfCurrentWeek()
        {
            return LastDayOfWeek(DateTime.Today);
        }


        private void ArrangeCalander(int id)
        {
            var list = db.WeeklyAttendances.Where(x => x.WeeklyAttendanceId == id).FirstOrDefault();
            WeeklyAttendance weeklyAttendance = new WeeklyAttendance();
            List<DateTime> arrangedDates = new List<DateTime>();
            if (list != null)
            {
                weeklyAttendance = list;
                if (weeklyAttendance.DaySevenDate != null)
                {
                    if (weeklyAttendance.DaySevenDate < weeklyAttendance.DayOneDate)
                    {
                        weeklyAttendance.DaySixDate = Convert.ToDateTime(weeklyAttendance.DaySevenDate).AddDays(-1);
                        weeklyAttendance.DayFiveDate = Convert.ToDateTime(weeklyAttendance.DaySixDate).AddDays(-1);
                        weeklyAttendance.DayFourDate = Convert.ToDateTime(weeklyAttendance.DayFiveDate).AddDays(-1);
                        weeklyAttendance.DayThreeDate = Convert.ToDateTime(weeklyAttendance.DayFourDate).AddDays(-1);
                        weeklyAttendance.DayTwoDate = Convert.ToDateTime(weeklyAttendance.DayThreeDate).AddDays(-1);
                        weeklyAttendance.DayOneDate = Convert.ToDateTime(weeklyAttendance.DayTwoDate).AddDays(-1);
                    }
                }
                else
                {
                    if (weeklyAttendance.DaySixDate != null)
                    {
                        weeklyAttendance.DaySevenDate = Convert.ToDateTime(weeklyAttendance.DaySixDate).AddDays(1);
                    }
                    else if (weeklyAttendance.DayFiveDate != null)
                    {
                        weeklyAttendance.DaySevenDate = Convert.ToDateTime(weeklyAttendance.DayFiveDate).AddDays(2);
                    }
                    else if (weeklyAttendance.DayFourDate != null)
                    {
                        weeklyAttendance.DaySevenDate = Convert.ToDateTime(weeklyAttendance.DayFourDate).AddDays(3);
                    }
                    else if (weeklyAttendance.DayThreeDate != null)
                    {
                        weeklyAttendance.DaySevenDate = Convert.ToDateTime(weeklyAttendance.DayThreeDate).AddDays(4);
                    }
                    else if (weeklyAttendance.DayTwoDate != null)
                    {
                        weeklyAttendance.DaySevenDate = Convert.ToDateTime(weeklyAttendance.DayTwoDate).AddDays(5);
                    }
                    else if (weeklyAttendance.DayOneDate != null)
                    {
                        weeklyAttendance.DaySevenDate = Convert.ToDateTime(weeklyAttendance.DayOneDate).AddDays(6);
                    }
                    weeklyAttendance.DaySixDate = Convert.ToDateTime(weeklyAttendance.DaySevenDate).AddDays(-1);
                    weeklyAttendance.DayFiveDate = Convert.ToDateTime(weeklyAttendance.DaySixDate).AddDays(-1);
                    weeklyAttendance.DayFourDate = Convert.ToDateTime(weeklyAttendance.DayFiveDate).AddDays(-1);
                    weeklyAttendance.DayThreeDate = Convert.ToDateTime(weeklyAttendance.DayFourDate).AddDays(-1);
                    weeklyAttendance.DayTwoDate = Convert.ToDateTime(weeklyAttendance.DayThreeDate).AddDays(-1);
                    weeklyAttendance.DayOneDate = Convert.ToDateTime(weeklyAttendance.DayTwoDate).AddDays(-1);

                }


            }
            var entity = new WeeklyAttendance
            {
                WeeklyAttendanceId = weeklyAttendance.WeeklyAttendanceId,
                EmployeeId = weeklyAttendance.EmployeeId,
                DayOne = true,// weeklyAttendance.DayOne,
                DayOneAN = true,// weeklyAttendance.DayOneAN,
                DayOneDate = weeklyAttendance.DayOneDate,
                DayTwo = true,//  weeklyAttendance.DayTwo,
                DayTwoAN = true,//  weeklyAttendance.DayTwoAN,
                DayTwoDate = weeklyAttendance.DayTwoDate,
                DayThree = true,// weeklyAttendance.DayThree,
                DayThreeAN = true,// weeklyAttendance.DayThreeAN,
                DayThreeDate = weeklyAttendance.DayThreeDate,
                DayFour = true,//  weeklyAttendance.DayFour,
                DayFourAN = true,//  weeklyAttendance.DayFourAN,
                DayFourDate = weeklyAttendance.DayFourDate,
                DayFive = true,// weeklyAttendance.DayFive,
                DayFiveAN = true,// weeklyAttendance.DayFiveAN,
                DayFiveDate = weeklyAttendance.DayFiveDate,
                DaySix = true,// weeklyAttendance.DaySix,
                DaySixAN = true,// weeklyAttendance.DaySixAN,
                DaySixDate = weeklyAttendance.DaySixDate,
                DaySeven = true,//  weeklyAttendance.DaySeven,
                DaySevenAN = true,//  weeklyAttendance.DaySevenAN,
                DaySevenDate = weeklyAttendance.DaySevenDate,
                Remark = weeklyAttendance.Remark,
                Week = weeklyAttendance.Week,
                Month = weeklyAttendance.Month,
                Year = weeklyAttendance.Year,
                GLPeriodId = weeklyAttendance.GLPeriodId,
                GlFiscalYearId = weeklyAttendance.GlFiscalYearId



            };
            //var period = db.GLPeriods.FirstOrDefault(x=>x.GLPeriodId == weeklyAttendance.GLPeriodId);
            //if(period != null)
            //{
            //    var week1 = period.DateFrom.AddDays(7);
            //}

            //var wperiod = db.GLPeriods.FirstOrDefault(x=>x.);



            if (entity.DayOneDate != null)
            {
                entity.DayOneAmharicDate =
                    XAPI.EthiopicDateTime.GetEthiopicDate(Convert.ToDateTime(entity.DayOneDate).Day,
                        Convert.ToDateTime(entity.DayOneDate).Month,
                        Convert.ToDateTime(entity.DayOneDate).Year);
            }

            if (entity.DayTwoDate != null)
            {
                entity.DayTwoAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(
                    Convert.ToDateTime(entity.DayTwoDate).Day,
                    Convert.ToDateTime(entity.DayTwoDate).Month,
                    Convert.ToDateTime(entity.DayTwoDate).Year);
            }

            if (entity.DayThreeDate != null)
            {
                entity.DayThreeAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(
                    Convert.ToDateTime(entity.DayThreeDate).Day,
                    Convert.ToDateTime(entity.DayThreeDate).Month,
                    Convert.ToDateTime(entity.DayThreeDate).Year);
            }

            if (entity.DayFourDate != null)
            {

                entity.DayFourAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(
                    Convert.ToDateTime(entity.DayFourDate).Day,
                    Convert.ToDateTime(entity.DayFourDate).Month,
                    Convert.ToDateTime(entity.DayFourDate).Year);
            }

            if (entity.DayFiveDate != null)
            {
                entity.DayFiveAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(
                    Convert.ToDateTime(entity.DayFiveDate).Day,
                    Convert.ToDateTime(entity.DayFiveDate).Month,
                    Convert.ToDateTime(entity.DayFiveDate).Year);
            }

            if (entity.DaySixDate != null)
            {
                entity.DaySixAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(
                    Convert.ToDateTime(entity.DaySixDate).Day,
                    Convert.ToDateTime(entity.DaySixDate).Month,
                    Convert.ToDateTime(entity.DaySixDate).Year);
            }


            if (entity.DaySevenDate != null)
            {
                entity.DaySevenAmharicDate = XAPI.EthiopicDateTime.GetEthiopicDate(
                    Convert.ToDateTime(entity.DaySevenDate).Day,
                    Convert.ToDateTime(entity.DaySevenDate).Month,
                    Convert.ToDateTime(entity.DaySevenDate).Year);
            }

            db = new ExceedDbContext();
            db.WeeklyAttendances.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

        }
        private void SetEmployeeAttendance(List<EmployeeIdView> employeeList, int month)
        {

            var weekfive = new List<DateTime>();
            var weeksix = new List<DateTime>();
            var entity = new WeeklyAttendance();

            List<DateTime> weekDateTimes = GetWeekDayDates(month, DateTime.Now.Year);

            var weekone = weekDateTimes.GetRange(0, 7);
            var weektwo = weekDateTimes.GetRange(7, 7);
            var weekthree = weekDateTimes.GetRange(14, 7);
            var weekfour = weekDateTimes.GetRange(21, 7);
            if (weekDateTimes.Count == 30)
            {
                weekfive = weekDateTimes.GetRange(28, 2);
            }
            else if (weekDateTimes.Count == 31)
            {
                weeksix = weekDateTimes.GetRange(28, 3);
            }
            else
            {
            }

            DateTime cY = DateTime.Today;
            int cM = DateTime.Now.Month;
            int yid = 0, pid = 0;
            var glYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= cY && x.DateTo >= cY);
            if (glYear != null)
            {
                yid = glYear.GlFiscalYearId;
                var gPeriod = db.GLPeriods.FirstOrDefault(x => x.GlFiscalYearId == yid && (x.DateFrom.Month == cM || x.DateTo.Month == cM));
                if (gPeriod != null)
                {
                    pid = gPeriod.GLPeriodId;
                }
            }

            foreach (var e in employeeList)
            {
                //      var check =
                //db.WeeklyAttendances.Where(x => x.Month == month && x.Year == DateTime.Now.Year && x.EmployeeId == e.PersonId)
                //    .FirstOrDefault();

                var check =
       db.WeeklyAttendances.Where(x => x.GLPeriodId == pid && x.GlFiscalYearId == yid && x.EmployeeId == e.PersonId)
           .FirstOrDefault();

                if (check == null)
                {
                    
                  
                    #region Week One

                    weekone = weekone.OrderBy(x => x.Date).ToList();
                    for (int i = 0; i < weekone.Count; i++)
                    {
                        entity = new WeeklyAttendance();
                        entity.EmployeeId = e.PersonId;
                        entity.Week = 1;
                        entity.Month = month;
                        entity.Year = DateTime.Now.Year;
                        entity.GlFiscalYearId = yid;
                        entity.GLPeriodId = pid;

                        ///////////////////////////
                        ////// To be checked//////
                        //////////////////////////

                        //          var checkemp =                          
                        //db.WeeklyAttendances.Where(x => x.Month == month && x.Year == DateTime.Now.Year && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                        //    .FirstOrDefault();
                        var checkemp =
                       db.WeeklyAttendances.Where(x => x.GLPeriodId == pid && x.GlFiscalYearId == yid && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                           .FirstOrDefault();
                        if (checkemp != null)
                        {
                            entity = checkemp;
                        }
                        switch (weekone[i].DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                entity.DayOne = true;
                                entity.DayOneAN = true;
                                entity.DayOneDate = weekone[i];
                                break;
                            case DayOfWeek.Monday:
                                entity.DayTwo = true;
                                entity.DayTwoAN = true;
                                entity.DayTwoDate = weekone[i];
                                break;
                            case DayOfWeek.Tuesday:
                                entity.DayThree = true;
                                entity.DayThreeAN = true;
                                entity.DayThreeDate = weekone[i];
                                break;
                            case DayOfWeek.Wednesday:
                                entity.DayFour = true;
                                entity.DayFourAN = true;
                                entity.DayFourDate = weekone[i];
                                break;
                            case DayOfWeek.Thursday:
                                entity.DayFive = true;
                                entity.DayFiveAN = true;
                                entity.DayFiveDate = weekone[i];
                                break;
                            case DayOfWeek.Friday:
                                entity.DaySix = true;


                                entity.DaySixAN = true;
                                entity.DaySixDate = weekone[i];
                                break;
                            case DayOfWeek.Saturday:
                                entity.DaySeven = true;
                                entity.DaySevenAN = true;
                                entity.DaySevenDate = weekone[i];
                                break;

                        }

                        if (checkemp == null)
                        {
                            db.WeeklyAttendances.Add(entity);
                            db.SaveChanges();
                        }
                        else
                        {
                            db = new ExceedDbContext();
                            entity.WeeklyAttendanceId = checkemp.WeeklyAttendanceId;
                            db.WeeklyAttendances.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }
                    ArrangeCalander(entity.WeeklyAttendanceId);
                    #endregion

                    #region Week Two
                    weektwo = weektwo.OrderBy(x => x.Date).ToList();
                    for (int k = 0; k < weektwo.Count; k++)
                    {
                        entity = new WeeklyAttendance();
                        entity.EmployeeId = e.PersonId;
                        entity.Week = 2;
                        entity.Month = month;
                        entity.Year = DateTime.Now.Year;
                        entity.GlFiscalYearId = yid;
                        entity.GLPeriodId = pid;

                        //           var checkemp =
                        //db.WeeklyAttendances.Where(x => x.Month == month && x.Year == DateTime.Now.Year && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                        //    .FirstOrDefault();
                        var checkemp =
                        db.WeeklyAttendances.Where(x => x.GLPeriodId == pid && x.GlFiscalYearId == yid && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                            .FirstOrDefault();
                        if (checkemp != null)
                        {
                            entity = checkemp;
                        }
                        switch (weektwo[k].DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                entity.DayOne = true;
                                entity.DayOneAN = true;
                                entity.DayOneDate = weektwo[k];
                                break;
                            case DayOfWeek.Monday:
                                entity.DayTwo = true;
                                entity.DayTwoAN = true;
                                entity.DayTwoDate = weektwo[k];
                                break;
                            case DayOfWeek.Tuesday:
                                entity.DayThree = true;
                                entity.DayThreeAN = true;
                                entity.DayThreeDate = weektwo[k];
                                break;
                            case DayOfWeek.Wednesday:
                                entity.DayFour = true;
                                entity.DayFourAN = true;
                                entity.DayFourDate = weektwo[k];
                                break;
                            case DayOfWeek.Thursday:
                                entity.DayFive = true;
                                entity.DayFiveAN = true;
                                entity.DayFiveDate = weektwo[k];
                                break;
                            case DayOfWeek.Friday:
                                entity.DaySix = true;
                                entity.DaySixAN = true;
                                entity.DaySixDate = weektwo[k];
                                break;
                            case DayOfWeek.Saturday:
                                entity.DaySeven = true;
                                entity.DaySevenAN = true;
                                entity.DaySevenDate = weektwo[k];
                                break;

                        }


                        if (checkemp == null)
                        {
                            db.WeeklyAttendances.Add(entity);
                            db.SaveChanges();
                        }
                        else
                        {
                            db = new ExceedDbContext();
                            entity.WeeklyAttendanceId = checkemp.WeeklyAttendanceId;
                            db.WeeklyAttendances.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                    }

                    ArrangeCalander(entity.WeeklyAttendanceId);
                    #endregion

                    #region Week Three
                    weekthree = weekthree.OrderBy(x => x.Date).ToList();
                    for (int j = 0; j < weekthree.Count; j++)
                    {
                        entity = new WeeklyAttendance();
                        entity.EmployeeId = e.PersonId;
                        entity.Week = 3;
                        entity.Month = month;
                        entity.Year = DateTime.Now.Year;
                        entity.GlFiscalYearId = yid;
                        entity.GLPeriodId = pid;

                        //               var checkemp =
                        //db.WeeklyAttendances.Where(x => x.Month == month && x.Year == DateTime.Now.Year && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                        //    .FirstOrDefault();
                        var checkemp =
                       db.WeeklyAttendances.Where(x => x.GLPeriodId == pid && x.GlFiscalYearId == yid && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                           .FirstOrDefault();
                        if (checkemp != null)
                        {
                            entity = checkemp;
                        }
                        switch (weekthree[j].DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                entity.DayOne = true;
                                entity.DayOneAN = true;
                                entity.DayOneDate = weekthree[j];
                                break;
                            case DayOfWeek.Monday:
                                entity.DayTwo = true;
                                entity.DayTwoAN = true;
                                entity.DayTwoDate = weekthree[j];
                                break;
                            case DayOfWeek.Tuesday:
                                entity.DayThree = true;
                                entity.DayThreeAN = true;
                                entity.DayThreeDate = weekthree[j];
                                break;
                            case DayOfWeek.Wednesday:
                                entity.DayFour = true;
                                entity.DayFourAN = true;
                                entity.DayFourDate = weekthree[j];
                                break;
                            case DayOfWeek.Thursday:
                                entity.DayFive = true;
                                entity.DayFiveAN = true;
                                entity.DayFiveDate = weekthree[j];
                                break;
                            case DayOfWeek.Friday:
                                entity.DaySix = true;
                                entity.DaySixAN = true;
                                entity.DaySixDate = weekthree[j];
                                break;
                            case DayOfWeek.Saturday:
                                entity.DaySeven = true;
                                entity.DaySevenAN = true;
                                entity.DaySevenDate = weekthree[j];
                                break;

                        }


                        if (checkemp == null)
                        {
                            db.WeeklyAttendances.Add(entity);
                            db.SaveChanges();
                        }
                        else
                        {
                            db = new ExceedDbContext();
                            entity.WeeklyAttendanceId = checkemp.WeeklyAttendanceId;
                            db.WeeklyAttendances.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                        }


                    }
                    ArrangeCalander(entity.WeeklyAttendanceId);

                    #endregion

                    #region Week Four
                    weekfour = weekfour.OrderBy(x => x.Date).ToList();
                    for (int x = 0; x < weekfour.Count; x++)
                    {
                        entity = new WeeklyAttendance();
                        entity.EmployeeId = e.PersonId;
                        entity.Week = 4;
                        entity.Month = month;
                        entity.Year = DateTime.Now.Year;
                        entity.GlFiscalYearId = yid;
                        entity.GLPeriodId = pid;

                        //               var checkemp =
                        //db.WeeklyAttendances.Where(xx => xx.Month == month && xx.Year == DateTime.Now.Year && xx.Week == entity.Week && xx.EmployeeId == entity.EmployeeId)
                        //    .FirstOrDefault();

                        var checkemp =
        db.WeeklyAttendances.Where(xx => xx.GLPeriodId == pid && xx.GlFiscalYearId == yid && xx.Week == entity.Week && xx.EmployeeId == entity.EmployeeId)
            .FirstOrDefault();

                        if (checkemp != null)
                        {
                            entity = checkemp;
                        }
                        switch (weekfour[x].DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                entity.DayOne = true;
                                entity.DayOneAN = true;
                                entity.DayOneDate = weekfour[x];
                                break;
                            case DayOfWeek.Monday:
                                entity.DayTwo = true;
                                entity.DayTwoAN = true;
                                entity.DayTwoDate = weekfour[x];
                                break;
                            case DayOfWeek.Tuesday:
                                entity.DayThree = true;
                                entity.DayThreeAN = true;
                                entity.DayThreeDate = weekfour[x];
                                break;
                            case DayOfWeek.Wednesday:
                                entity.DayFour = true;
                                entity.DayFourAN = true;
                                entity.DayFourDate = weekfour[x];
                                break;
                            case DayOfWeek.Thursday:
                                entity.DayFive = true;
                                entity.DayFiveAN = true;
                                entity.DayFiveDate = weekfour[x];
                                break;
                            case DayOfWeek.Friday:
                                entity.DaySix = true;
                                entity.DaySixAN = true;
                                entity.DaySixDate = weekfour[x];
                                break;
                            case DayOfWeek.Saturday:
                                entity.DaySeven = true;
                                entity.DaySevenAN = true;
                                entity.DaySevenDate = weekfour[x];
                                break;

                        }


                        if (checkemp == null)
                        {
                            db.WeeklyAttendances.Add(entity);
                            db.SaveChanges();
                        }
                        else
                        {
                            db = new ExceedDbContext();
                            entity.WeeklyAttendanceId = checkemp.WeeklyAttendanceId;
                            db.WeeklyAttendances.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }
                    ArrangeCalander(entity.WeeklyAttendanceId);
                    #endregion

                    #region Week Five
                    weekfive = weekfive.OrderBy(x => x.Date).ToList();
                    for (int y = 0; y < weekfive.Count; y++)
                    {
                        entity = new WeeklyAttendance();
                        entity.EmployeeId = e.PersonId;
                        entity.Week = 5;
                        entity.Month = month;
                        entity.Year = DateTime.Now.Year;
                        entity.GlFiscalYearId = yid;
                        entity.GLPeriodId = pid;

                        //               var checkemp =
                        //db.WeeklyAttendances.Where(x => x.Month == month && x.Year == DateTime.Now.Year && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                        //    .FirstOrDefault();

                        var checkemp =
         db.WeeklyAttendances.Where(x => x.GLPeriodId == pid && x.GlFiscalYearId == yid && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
             .FirstOrDefault();

                        if (checkemp != null)
                        {
                            entity = checkemp;
                        }
                        switch (weekfive[y].DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                entity.DayOne = true;
                                entity.DayOneAN = true;
                                entity.DayOneDate = weekfive[y];
                                break;
                            case DayOfWeek.Monday:
                                entity.DayTwo = true;
                                entity.DayTwoAN = true;
                                entity.DayTwoDate = weekfive[y];
                                break;
                            case DayOfWeek.Tuesday:
                                entity.DayThree = true;
                                entity.DayThreeAN = true;
                                entity.DayThreeDate = weekfive[y];
                                break;
                            case DayOfWeek.Wednesday:
                                entity.DayFour = true;
                                entity.DayFourAN = true;
                                entity.DayFourDate = weekfive[y];
                                break;
                            case DayOfWeek.Thursday:
                                entity.DayFive = true;
                                entity.DayFiveAN = true;
                                entity.DayFiveDate = weekfive[y];
                                break;
                            case DayOfWeek.Friday:
                                entity.DaySix = true;
                                entity.DaySixAN = true;
                                entity.DaySixDate = weekfive[y];
                                break;
                            case DayOfWeek.Saturday:
                                entity.DaySeven = true;
                                entity.DaySevenAN = true;
                                entity.DaySevenDate = weekfive[y];
                                break;

                        }


                        if (checkemp == null)
                        {
                            db.WeeklyAttendances.Add(entity);
                            db.SaveChanges();
                        }
                        else
                        {
                            db = new ExceedDbContext();
                            entity.WeeklyAttendanceId = checkemp.WeeklyAttendanceId;
                            db.WeeklyAttendances.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }

                    ArrangeCalander(entity.WeeklyAttendanceId);
                    #endregion

                    #region Week Six
                    weeksix = weeksix.OrderBy(x => x.Date).ToList();
                    for (int z = 0; z < weeksix.Count; z++)
                    {
                        entity = new WeeklyAttendance();
                        entity.EmployeeId = e.PersonId;
                        entity.Week = 6;
                        entity.Month = month;
                        entity.Year = DateTime.Now.Year;
                        entity.GlFiscalYearId = yid;
                        entity.GLPeriodId = pid;

                        //               var checkemp =
                        //db.WeeklyAttendances.Where(x => x.Month == month && x.Year == DateTime.Now.Year && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
                        //    .FirstOrDefault();

                        var checkemp =
         db.WeeklyAttendances.Where(x => x.GLPeriodId == pid && x.GlFiscalYearId == yid && x.Week == entity.Week && x.EmployeeId == entity.EmployeeId)
             .FirstOrDefault();

                        if (checkemp != null)
                        {
                            entity = checkemp;
                        }
                        switch (weeksix[z].DayOfWeek)
                        {
                            case DayOfWeek.Sunday:
                                entity.DayOne = true;
                                entity.DayOneAN = true;
                                entity.DayOneDate = weeksix[z];
                                break;
                            case DayOfWeek.Monday:
                                entity.DayTwo = true;
                                entity.DayTwoAN = true;
                                entity.DayTwoDate = weeksix[z];
                                break;
                            case DayOfWeek.Tuesday:
                                entity.DayThree = true;
                                entity.DayThreeAN = true;
                                entity.DayThreeDate = weeksix[z];
                                break;
                            case DayOfWeek.Wednesday:
                                entity.DayFour = true;
                                entity.DayFourAN = true;
                                entity.DayFourDate = weeksix[z];
                                break;
                            case DayOfWeek.Thursday:
                                entity.DayFive = true;
                                entity.DayFiveAN = true;
                                entity.DayFiveDate = weeksix[z];
                                break;
                            case DayOfWeek.Friday:
                                entity.DaySix = true;
                                entity.DaySixAN = true;
                                entity.DaySixDate = weeksix[z];
                                break;
                            case DayOfWeek.Saturday:
                                entity.DaySeven = true;
                                entity.DaySevenAN = true;
                                entity.DaySevenDate = weeksix[z];
                                break;

                        }


                        if (checkemp == null)
                        {
                            db.WeeklyAttendances.Add(entity);
                            db.SaveChanges();
                        }
                        else
                        {
                            db = new ExceedDbContext();
                            entity.WeeklyAttendanceId = checkemp.WeeklyAttendanceId;
                            db.WeeklyAttendances.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                        }


                    }
                    ArrangeCalander(entity.WeeklyAttendanceId);
                    #endregion
                }
            }
        }

        private int WeekNo(int ppid,int yyid, DateTime date)
        {
            int month = 0;
            var period = db.GLPeriods.FirstOrDefault(x=>x.GlFiscalYearId == yyid && x.GLPeriodId == ppid);
            if(period != null)
            {
                month = period.DateFrom.Month;
            }
            List<DateTime> weekDateTimes = GetWeekDayDates(DateTime.Now.Month, DateTime.Now.Year);
            //var allweekdays = db.WeeklyAttendances.Where(x => x.Month == month && x.Year == DateTime.Now.Year).ToList();
            var allweekdays = db.WeeklyAttendances.Where(x => x.GLPeriodId == ppid && x.GlFiscalYearId == yyid).ToList();
            foreach (var a in allweekdays)
            {
                
                if (a.DayOneDate == date.Date)
                {
                    WeekNumber = a.Week;
                    break;
                }
                else if (a.DayTwoDate == date.Date)
                {
                    WeekNumber = a.Week;
                    break;
                }
                else if (a.DayThreeDate == date.Date)
                {
                    WeekNumber = a.Week;
                    break;
                }
                else if (a.DayFourDate == date.Date)
                {
                    WeekNumber = a.Week;
                    break;
                }
                else if (a.DayFiveDate == date.Date)
                {
                    WeekNumber = a.Week;
                    break;
                }
                else if (a.DaySixDate == date.Date)
                {
                    WeekNumber = a.Week;
                    break;
                }
                else if (a.DaySevenDate == date.Date)
                {
                    WeekNumber = a.Week;
                    break;
                }
            }
            return WeekNumber;
        }    

        public int GetDepartment(int id)
        {
            var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == id && x.isCurrent);
            if(place != null)
            {
                return place.OrgStructureId;
            }

            return 0;
        }
        public ActionResult WeeklyAttendances_Read([DataSourceRequest]DataSourceRequest request)
        {
            DateTime cY = DateTime.Today;
            int cM = DateTime.Now.Month;
            int yid = 0, pid = 0;
            var glYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= cY && x.DateTo >= cY);
            if (glYear != null)
            {
                yid = glYear.GlFiscalYearId;
                var gPeriod = db.GLPeriods.FirstOrDefault(x => x.GlFiscalYearId == yid && (x.DateFrom.Month == cM || x.DateTo.Month == cM));
                if (gPeriod != null)
                {
                    pid = gPeriod.GLPeriodId;
                }
            }

            //WeekNo(DateTime.Now.Month, DateTime.Now.Date);
            WeekNo(pid,yid, DateTime.Now.Date);
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var empIdList = EmployeeHelper.GetSubordinates(empId);
            //IQueryable<WeeklyAttendance> weeklyattendances = db.WeeklyAttendances.Where(
            //        x => x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year && x.Week == WeekNumber && empIdList.Contains(x.EmployeeId)).AsQueryable();// DetailViewModels().AsQueryable();

            IQueryable<WeeklyAttendance> weeklyattendances = db.WeeklyAttendances.Where(
                  x => x.GLPeriodId == pid && x.GlFiscalYearId == yid && x.Week == WeekNumber && empIdList.Contains(x.EmployeeId)).AsQueryable();// DetailViewModels().AsQueryable();

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
                Department = GetDepartment(weeklyAttendance.EmployeeId)
            });

            return Json(result);
        }

        private List<DateTime> GetWeekDayDates(int month, int year)
        {

            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                   .Select(day => new DateTime(year, month, day))
                  .ToList();

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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyAttendances_Create([DataSourceRequest]DataSourceRequest request, WeeklyAttendance weeklyAttendance)
        {
            if (ModelState.IsValid)
            {
                var entity = new WeeklyAttendance
                {
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
                    DaySevenAmharicDate = weeklyAttendance.DaySevenAmharicDate
                };

                db.WeeklyAttendances.Add(entity);
                db.SaveChanges();
                weeklyAttendance.WeeklyAttendanceId = entity.WeeklyAttendanceId;
            }

            return Json(new[] { weeklyAttendance }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyAttendances_Update([DataSourceRequest]DataSourceRequest request, WeeklyAttendance weeklyAttendance)
        {
            if (ModelState.IsValid)
            {
                var entity = new WeeklyAttendance
                {
                    WeeklyAttendanceId = weeklyAttendance.WeeklyAttendanceId,
                    EmployeeId = weeklyAttendance.EmployeeId,
                    DayOne = weeklyAttendance.DayOne,
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
                    DaySevenAmharicDate = weeklyAttendance.DaySevenAmharicDate
                };

                db.WeeklyAttendances.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { weeklyAttendance }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeeklyAttendances_Destroy([DataSourceRequest]DataSourceRequest request, WeeklyAttendance weeklyAttendance)
        {
            if (ModelState.IsValid)
            {
                var entity = new WeeklyAttendance
                {
                    WeeklyAttendanceId = weeklyAttendance.WeeklyAttendanceId,
                    EmployeeId = weeklyAttendance.EmployeeId,
                    DayOne = weeklyAttendance.DayOne,
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
                    DaySevenAmharicDate = weeklyAttendance.DaySevenAmharicDate
                };

                db.WeeklyAttendances.Attach(entity);
                db.WeeklyAttendances.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weeklyAttendance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

   
}