using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Plan

{
    public class FurnitureProductionPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["BudgetYears"] = db.GLFiscalYears.ToList();
            ViewData["Periods"] = db.GLPeriods.ToList();
            return View();
        }
        public ActionResult YearlyProductionPlans_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<FurnitureYearlyProductionPlan> YearlyProductionPlans = db.FurnitureYearlyProductionPlans;


            DataSourceResult result = YearlyProductionPlans.ToDataSourceResult(request, yearlyPlan => new
            {
                YearlyProductionPlanId = yearlyPlan.FurnitureYearlyProductionPlanId,
                GlFiscalYearId = yearlyPlan.GlFiscalYearId,

                StartDate = yearlyPlan.StartDate,
                StartDateEth = yearlyPlan.StartDateEth,
                EndDateEth = yearlyPlan.EndDateEth,
                EndDate = yearlyPlan.EndDate,
                ProductionPlanQuantity = yearlyPlan.ProductionPlanQuantity,
                ProductionPlanBirr = yearlyPlan.ProductionPlanBirr,
                Name = yearlyPlan.Name
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyProductionPlans_Create([DataSourceRequest] DataSourceRequest request, FurnitureYearlyProductionPlan yearlyPlan)
        {



            var entity = new FurnitureYearlyProductionPlan();
            var budgetYear = db.GLFiscalYears.Find(yearlyPlan.GlFiscalYearId);
            if (budgetYear != null)
            {
                var checkPlans = db.YearlyProductionPlans.Where(q => q.GlFiscalYearId == budgetYear.GlFiscalYearId);
                if (checkPlans.Any())
                {
                    ModelState.AddModelError("Error", "Fiscal Year Plan Already Existed");

                }
                if (ModelState.IsValid)
                {

                    int year = (budgetYear.DateFrom.Year) - 8;
                    bool leap = false;
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var startDateEth = "01/11/" + year;
                            var endDateEth = "30/10/" + (year + 1);
                            var startDateArr = startDateEth.Split('/');
                            var endDateArr = endDateEth.Split('/');
                            DateTime startDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(startDateArr[0]), Convert.ToInt32(startDateArr[1]), Convert.ToInt32(startDateArr[2]));
                            DateTime endDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(endDateArr[0]), Convert.ToInt32(endDateArr[1]), Convert.ToInt32(endDateArr[2]));
                            entity = new FurnitureYearlyProductionPlan
                            {

                                FurnitureYearlyProductionPlanId = yearlyPlan.FurnitureYearlyProductionPlanId,
                                GlFiscalYearId = yearlyPlan.GlFiscalYearId,
                                StartDate = startDate,
                                StartDateEth = startDateEth,
                                EndDate = endDate,
                                EndDateEth = endDateEth,
                                ProductionPlanQuantity = yearlyPlan.ProductionPlanQuantity,
                                ProductionPlanBirr = yearlyPlan.ProductionPlanBirr
                            };

                            db.FurnitureYearlyProductionPlans.Add(entity);
                            db.SaveChanges();
                            yearlyPlan.FurnitureYearlyProductionPlanId = entity.FurnitureYearlyProductionPlanId;

                            var quarterYearPlan2 = new FurnitureQuarterlyProductionPlan();

                            var halfYearPlan1 = new FurnitureHalfYearlyProductionPlan
                            {
                                YearlyProductionPlanId = yearlyPlan.FurnitureYearlyProductionPlanId,
                                Name = "First Half Year",
                                NameAm = "የመጀመሪያ አጋማሽ ዓመት",
                                StartDate = startDate,
                                StartDateEth = startDateEth,
                                EndDate = leap ? startDate.AddDays(185) : startDate.AddDays(184),
                                EndDateEth = "30/04/" + (year + 1),
                                ProductionPlanQuantity = 0,
                                ProductionPlanBirr = 0
                            };
                            db.FurnitureHalfYearlyProductionPlans.Add(halfYearPlan1);

                            var halfYearPlan2 = new FurnitureHalfYearlyProductionPlan
                            {
                                YearlyProductionPlanId = entity.FurnitureYearlyProductionPlanId,
                                Name = "Second Half Year",
                                NameAm = "ሁለተኛ አጋማሽ ዓመት",
                                StartDate = halfYearPlan1.EndDate.Value.AddDays(1),
                                StartDateEth = "01/05/" + (year + 1),
                                EndDate = halfYearPlan1.EndDate.Value.AddDays(180),
                                EndDateEth = "30/10/" + (year + 1),
                                ProductionPlanQuantity = 0,
                                ProductionPlanBirr = 0
                            };
                            db.FurnitureHalfYearlyProductionPlans.Add(halfYearPlan2);
                            db.SaveChanges();
                            var halfYearPlans = db.FurnitureHalfYearlyProductionPlans.Where(q => q.YearlyProductionPlanId == entity.FurnitureYearlyProductionPlanId).ToList();
                            int i = 0;
                            foreach (var halfYearPlan in halfYearPlans)
                            {



                                if (i == 0)
                                {
                                    var quarterYearPlan1 = new FurnitureQuarterlyProductionPlan
                                    {
                                        FurnitureHalfYearlyProductionPlanId = halfYearPlan.FurnitureHalfYearlyProductionPlanId,
                                        Name = "First Quarter Year",
                                        NameAm = "የመጀመሪያ ሩብ ዓመት",
                                        StartDate = startDate,
                                        StartDateEth = startDateEth,
                                        EndDate = leap ? startDate.AddDays(95) : startDate.AddDays(94),
                                        EndDateEth = "30/01/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureQuarterlyProductionPlans.Add(quarterYearPlan1);

                                    quarterYearPlan2 = new FurnitureQuarterlyProductionPlan
                                    {
                                        FurnitureHalfYearlyProductionPlanId = halfYearPlan.FurnitureHalfYearlyProductionPlanId,
                                        Name = "Second Quarter Year",
                                        NameAm = "ሁለተኛ ሩብ ዓመት",
                                        StartDate = quarterYearPlan1.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/02/" + (year + 1),
                                        EndDate = quarterYearPlan1.EndDate.Value.AddDays(90),
                                        EndDateEth = "30/04/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureQuarterlyProductionPlans.Add(quarterYearPlan2);

                                }
                                if (i == 1)
                                {
                                    var quarterYearPlan3 = new FurnitureQuarterlyProductionPlan
                                    {
                                        FurnitureHalfYearlyProductionPlanId = halfYearPlan.FurnitureHalfYearlyProductionPlanId,
                                        Name = "Third Quarter Year",
                                        NameAm = "ሶስተኛ ሩብ ዓመት",
                                        StartDate = quarterYearPlan2.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/05/" + (year + 1),
                                        EndDate = quarterYearPlan2.EndDate.Value.AddDays(90),
                                        EndDateEth = "30/07/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureQuarterlyProductionPlans.Add(quarterYearPlan3);

                                    var quarterYearPlan4 = new FurnitureQuarterlyProductionPlan
                                    {
                                        FurnitureHalfYearlyProductionPlanId = halfYearPlan.FurnitureHalfYearlyProductionPlanId,
                                        Name = "Fourth Quarter Year",
                                        NameAm = "ኣራተኛ ሩብ ዓመት",
                                        StartDate = quarterYearPlan3.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/08/" + (year + 1),
                                        EndDate = quarterYearPlan3.EndDate.Value.AddDays(90),
                                        EndDateEth = "30/10/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureQuarterlyProductionPlans.Add(quarterYearPlan4);
                                }

                                if (i == 2)
                                {
                                    break;
                                }
                                i++;
                            }
                            db.SaveChanges();

                            i = 0;
                            var halfYearIds = db.FurnitureHalfYearlyProductionPlans.Where(q => q.YearlyProductionPlanId == entity.FurnitureYearlyProductionPlanId).Select(s => s.FurnitureHalfYearlyProductionPlanId); ;
                            var quarterPlans = db.QuarterlyProductionPlans.Where(q => halfYearIds.Contains(q.HalfYearlyProductionPlanId)).ToList();
                            var monthYearPlan3 = new FurnitureMonthlyProductionPlan();
                            var monthYearPlan6 = new FurnitureMonthlyProductionPlan();
                            var monthYearPlan9 = new FurnitureMonthlyProductionPlan();
                            foreach (var quarterPlan in quarterPlans)
                            {
                                if (i == 0)
                                {
                                    var monthYearPlan1 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "July",
                                        NameAm = "ሓምሌ",
                                        StartDate = startDate,
                                        StartDateEth = startDateEth,
                                        EndDate = startDate.AddDays(29),
                                        EndDateEth = "30/11/" + year,
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan1);
                                    var monthYearPlan2 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "August",
                                        NameAm = "ነሓሴ",
                                        StartDate = monthYearPlan1.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/12/" + year,
                                        EndDate = monthYearPlan1.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/12/" + year,
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan2);
                                    monthYearPlan3 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "September",
                                        NameAm = "መስከረም",
                                        StartDate = monthYearPlan2.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/01/" + (year + 1),
                                        EndDate = monthYearPlan2.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/01/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan3);

                                }
                                if (i == 1)
                                {
                                    var monthYearPlan4 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "October",
                                        NameAm = "ጥቅምት",
                                        StartDate = monthYearPlan3.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/02/" + (year + 1),
                                        EndDate = monthYearPlan3.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/02/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan4);
                                    var monthYearPlan5 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "November",
                                        NameAm = "ሕዳር",
                                        StartDate = monthYearPlan4.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/03/" + (year + 1),
                                        EndDate = monthYearPlan4.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/03/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan5);
                                    monthYearPlan6 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "December",
                                        NameAm = "ታሕሳስ",
                                        StartDate = monthYearPlan5.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/04/" + (year + 1),
                                        EndDate = monthYearPlan5.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/04/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan6);

                                }
                                if (i == 2)
                                {
                                    var monthYearPlan7 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "January",
                                        NameAm = "ጥር",
                                        StartDate = monthYearPlan6.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/05/" + (year + 1),
                                        EndDate = monthYearPlan6.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/05/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan7);
                                    var monthYearPlan8 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "February",
                                        NameAm = "ለካቲት",
                                        StartDate = monthYearPlan7.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/06/" + (year + 1),
                                        EndDate = monthYearPlan7.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/06/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan8);
                                    monthYearPlan9 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "March",
                                        NameAm = "መጋቢት",
                                        StartDate = monthYearPlan8.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/07/" + (year + 1),
                                        EndDate = monthYearPlan8.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/07/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan9);

                                }
                                if (i == 3)
                                {
                                    var monthYearPlan10 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "April",
                                        NameAm = "ሚያዝያ",
                                        StartDate = monthYearPlan9.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/08/" + (year + 1),
                                        EndDate = monthYearPlan9.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/08/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan10);
                                    var monthYearPlan11 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "May",
                                        NameAm = "ግንቦት",
                                        StartDate = monthYearPlan10.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/09/" + (year + 1),
                                        EndDate = monthYearPlan10.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/09/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan11);
                                    var monthYearPlan12 = new FurnitureMonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "June",
                                        NameAm = "ሰኔ",
                                        StartDate = monthYearPlan10.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/10/" + (year + 1),
                                        EndDate = monthYearPlan10.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/10/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.FurnitureMonthlyProductionPlans.Add(monthYearPlan12);

                                }
                                if (i == 4)
                                {
                                    break;
                                }
                                i++;
                            }
                            db.SaveChanges();

                            i = 0;
                            var quarterYearIds = quarterPlans.Select(s => s.QuarterlyProductionPlanId);
                            var monthPlans = db.MonthlyProductionPlans.Where(q => quarterYearIds.Contains(q.QuarterlyProductionPlanId)).ToList();
                            foreach (var monthPlan in monthPlans)
                            {

                                var weeklyPlan1 = new FurnitureWeeklyProductionPlan
                                {
                                    FurnitureMonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
                                    Name = "First Week",
                                    NameAm = "የመጀመርያ ሳምንት",
                                    StartDate = monthPlan.StartDate,
                                    StartDateEth = monthPlan.StartDateEth,
                                    EndDate = monthPlan.StartDate.Value.AddDays(6),
                                    EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(monthPlan.StartDate.Value.AddDays(6).Day, monthPlan.StartDate.Value.AddDays(6).Month, monthPlan.StartDate.Value.AddDays(6).Year),
                                    ProductionPlanQuantity = 0,
                                    ProductionPlanBirr = 0
                                };
                                db.FurnitureWeeklyProductionPlans.Add(weeklyPlan1);
                                var weeklyPlan2 = new FurnitureWeeklyProductionPlan
                                {
                                    FurnitureMonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
                                    Name = "Second Week",
                                    NameAm = "ሁለተኛ ሳምንት",
                                    StartDate = weeklyPlan1.EndDate.Value.AddDays(1),
                                    // StartDateEth = monthPlan.StartDateEth,
                                    StartDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan1.EndDate.Value.AddDays(1).Day, weeklyPlan1.EndDate.Value.AddDays(1).Month, weeklyPlan1.EndDate.Value.AddDays(1).Year),
                                    EndDate = weeklyPlan1.EndDate.Value.AddDays(7),
                                    //  EndDateEth = "30/10/" + (year + 1),
                                    EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan1.EndDate.Value.AddDays(7).Day, weeklyPlan1.EndDate.Value.AddDays(7).Month, weeklyPlan1.EndDate.Value.AddDays(7).Year),
                                    ProductionPlanQuantity = 0,
                                    ProductionPlanBirr = 0
                                };
                                db.FurnitureWeeklyProductionPlans.Add(weeklyPlan2);
                                var weeklyPlan3 = new FurnitureWeeklyProductionPlan
                                {
                                    FurnitureMonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
                                    Name = "Third Week",
                                    NameAm = "ሶስተኛ ሳምንት",
                                    StartDate = weeklyPlan2.EndDate.Value.AddDays(1),
                                    // StartDateEth = monthPlan.StartDateEth,
                                    StartDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan2.EndDate.Value.AddDays(1).Day, weeklyPlan2.EndDate.Value.AddDays(1).Month, weeklyPlan2.EndDate.Value.AddDays(1).Year),

                                    EndDate = weeklyPlan2.EndDate.Value.AddDays(7),
                                    // EndDateEth = "30/10/" + (year + 1),
                                    EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan2.EndDate.Value.AddDays(7).Day, weeklyPlan2.EndDate.Value.AddDays(7).Month, weeklyPlan2.EndDate.Value.AddDays(7).Year),
                                    ProductionPlanQuantity = 0,
                                    ProductionPlanBirr = 0
                                };
                                db.FurnitureWeeklyProductionPlans.Add(weeklyPlan3);
                                var weeklyPlan4 = new FurnitureWeeklyProductionPlan
                                {
                                    FurnitureMonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
                                    Name = "Fourth Week",
                                    NameAm = "ኣራተኛ ሳምንት",
                                    StartDate = weeklyPlan3.EndDate.Value.AddDays(1),
                                    //  StartDateEth = monthPlan.StartDateEth,
                                    StartDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(weeklyPlan3.EndDate.Value.AddDays(1).Day, weeklyPlan3.EndDate.Value.AddDays(1).Month, weeklyPlan3.EndDate.Value.AddDays(1).Year),
                                    EndDate = monthPlan.EndDate,
                                    EndDateEth = monthPlan.EndDateEth,
                                    ProductionPlanQuantity = 0,
                                    ProductionPlanBirr = 0
                                };
                                db.FurnitureWeeklyProductionPlans.Add(weeklyPlan4);

                            }

                            db.SaveChanges();
                            transaction.Commit();
                            #region Create log
                            // UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Created Market Price Entry Plan Record with Id: " + entity.YearlyProductionPlanId, Core.Domain.Global.GlobalEnum.SuperSettingAdminMode.DailyMarket);
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            // roll back all database operations, if any thing goes wrong      
                            transaction.Rollback();
                            ModelState.AddModelError("Error", ex);
                            throw;
                        }
                    }

                }

            }

            return Json(new[] { yearlyPlan }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyProductionPlans_Update([DataSourceRequest]DataSourceRequest request, FurnitureYearlyProductionPlan yearlyPlan)
        {

            if (ModelState.IsValid)
            {

                var entity = db.FurnitureYearlyProductionPlans.Find(yearlyPlan.FurnitureYearlyProductionPlanId);
                if (entity != null)
                {

                    entity.FurnitureYearlyProductionPlanId = yearlyPlan.FurnitureYearlyProductionPlanId;
                    entity.GlFiscalYearId = yearlyPlan.GlFiscalYearId;
                    entity.StartDate = yearlyPlan.StartDate;
                    entity.EndDate = yearlyPlan.EndDate;
                    entity.ProductionPlanBirr = yearlyPlan.ProductionPlanBirr;
                    entity.ProductionPlanQuantity = yearlyPlan.ProductionPlanQuantity;

                    db.FurnitureYearlyProductionPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }


            }

            return Json(new[] { yearlyPlan }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyProductionPlans_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureYearlyProductionPlan yearlyPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureYearlyProductionPlan
                {
                    FurnitureYearlyProductionPlanId = yearlyPlan.FurnitureYearlyProductionPlanId,
                    GlFiscalYearId = yearlyPlan.GlFiscalYearId,
                    StartDate = yearlyPlan.StartDate,
                    EndDate = yearlyPlan.EndDate,
                    ProductionPlanQuantity = yearlyPlan.ProductionPlanQuantity,
                    ProductionPlanBirr = yearlyPlan.ProductionPlanBirr,

                };



                db.FurnitureYearlyProductionPlans.Attach(entity);
                db.FurnitureYearlyProductionPlans.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { yearlyPlan }.ToDataSourceResult(request, ModelState));
        }


    }
}