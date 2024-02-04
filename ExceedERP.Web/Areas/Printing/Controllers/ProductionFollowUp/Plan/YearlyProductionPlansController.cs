using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Plan
{
    public class YearlyProductionPlansController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears.ToList();
            ViewData["Periods"] = db.GLPeriods.ToList();
            return View();
        }
        public ActionResult YearlyProductionPlans_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<YearlyProductionPlan> YearlyProductionPlans = db.YearlyProductionPlans;


            DataSourceResult result = YearlyProductionPlans.ToDataSourceResult(request, yearlyPlan => new
            {
                YearlyProductionPlanId = yearlyPlan.YearlyProductionPlanId,
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
        public ActionResult YearlyProductionPlans_Create([DataSourceRequest] DataSourceRequest request, YearlyProductionPlan yearlyPlan)
        {



            var entity = new YearlyProductionPlan();
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
                            entity = new YearlyProductionPlan
                            {

                                YearlyProductionPlanId = yearlyPlan.YearlyProductionPlanId,
                                GlFiscalYearId = yearlyPlan.GlFiscalYearId,
                                StartDate = startDate,
                                StartDateEth = startDateEth,
                                EndDate = endDate,
                                EndDateEth = endDateEth,
                                ProductionPlanQuantity = yearlyPlan.ProductionPlanQuantity,
                                ProductionPlanBirr = yearlyPlan.ProductionPlanBirr
                            };

                            db.YearlyProductionPlans.Add(entity);
                            db.SaveChanges();
                            yearlyPlan.YearlyProductionPlanId = entity.YearlyProductionPlanId;

                            var quarterYearPlan2 = new QuarterlyProductionPlan();

                            var halfYearPlan1 = new HalfYearlyProductionPlan
                            {
                                YearlyProductionPlanId = yearlyPlan.YearlyProductionPlanId,
                                Name = "First Half Year",
                                NameAm = "የመጀመሪያ አጋማሽ ዓመት",
                                StartDate = startDate,
                                StartDateEth = startDateEth,
                                EndDate = leap ? startDate.AddDays(185) : startDate.AddDays(184),
                                EndDateEth = "30/04/" + (year + 1),
                                ProductionPlanQuantity = 0,
                                ProductionPlanBirr = 0
                            };
                            db.HalfYearlyProductionPlans.Add(halfYearPlan1);

                            var halfYearPlan2 = new HalfYearlyProductionPlan
                            {
                                YearlyProductionPlanId = entity.YearlyProductionPlanId,
                                Name = "Second Half Year",
                                NameAm = "ሁለተኛ አጋማሽ ዓመት",
                                StartDate = halfYearPlan1.EndDate.Value.AddDays(1),
                                StartDateEth = "01/05/" + (year + 1),
                                EndDate = halfYearPlan1.EndDate.Value.AddDays(180),
                                EndDateEth = "30/10/" + (year + 1),
                                ProductionPlanQuantity = 0,
                                ProductionPlanBirr = 0
                            };
                            db.HalfYearlyProductionPlans.Add(halfYearPlan2);
                            db.SaveChanges();
                            var halfYearPlans = db.HalfYearlyProductionPlans.Where(q => q.YearlyProductionPlanId == entity.YearlyProductionPlanId).ToList();
                            int i = 0;
                            foreach (var halfYearPlan in halfYearPlans)
                            {



                                if (i == 0)
                                {
                                    var quarterYearPlan1 = new QuarterlyProductionPlan
                                    {
                                        HalfYearlyProductionPlanId = halfYearPlan.HalfYearlyProductionPlanId,
                                        Name = "First Quarter Year",
                                        NameAm = "የመጀመሪያ ሩብ ዓመት",
                                        StartDate = startDate,
                                        StartDateEth = startDateEth,
                                        EndDate = leap ? startDate.AddDays(95) : startDate.AddDays(94),
                                        EndDateEth = "30/01/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.QuarterlyProductionPlans.Add(quarterYearPlan1);

                                    quarterYearPlan2 = new QuarterlyProductionPlan
                                    {
                                        HalfYearlyProductionPlanId = halfYearPlan.HalfYearlyProductionPlanId,
                                        Name = "Second Quarter Year",
                                        NameAm = "ሁለተኛ ሩብ ዓመት",
                                        StartDate = quarterYearPlan1.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/02/" + (year + 1),
                                        EndDate = quarterYearPlan1.EndDate.Value.AddDays(90),
                                        EndDateEth = "30/04/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.QuarterlyProductionPlans.Add(quarterYearPlan2);

                                }
                                if (i == 1)
                                {
                                    var quarterYearPlan3 = new QuarterlyProductionPlan
                                    {
                                        HalfYearlyProductionPlanId = halfYearPlan.HalfYearlyProductionPlanId,
                                        Name = "Third Quarter Year",
                                        NameAm = "ሶስተኛ ሩብ ዓመት",
                                        StartDate = quarterYearPlan2.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/05/" + (year + 1),
                                        EndDate = quarterYearPlan2.EndDate.Value.AddDays(90),
                                        EndDateEth = "30/07/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.QuarterlyProductionPlans.Add(quarterYearPlan3);

                                    var quarterYearPlan4 = new QuarterlyProductionPlan
                                    {
                                        HalfYearlyProductionPlanId = halfYearPlan.HalfYearlyProductionPlanId,
                                        Name = "Fourth Quarter Year",
                                        NameAm = "ኣራተኛ ሩብ ዓመት",
                                        StartDate = quarterYearPlan3.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/08/" + (year + 1),
                                        EndDate = quarterYearPlan3.EndDate.Value.AddDays(90),
                                        EndDateEth = "30/10/" + (year + 1),
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.QuarterlyProductionPlans.Add(quarterYearPlan4);
                                }

                                if (i == 2)
                                {
                                    break;
                                }
                                i++;
                            }
                            db.SaveChanges();

                            i = 0;
                            var halfYearIds = db.HalfYearlyProductionPlans.Where(q => q.YearlyProductionPlanId == entity.YearlyProductionPlanId).Select(s => s.HalfYearlyProductionPlanId); ;
                            var quarterPlans = db.QuarterlyProductionPlans.Where(q => halfYearIds.Contains(q.HalfYearlyProductionPlanId)).ToList();
                            var monthYearPlan3 = new MonthlyProductionPlan();
                            var monthYearPlan6 = new MonthlyProductionPlan();
                            var monthYearPlan9 = new MonthlyProductionPlan();
                            foreach (var quarterPlan in quarterPlans)
                            {
                                if (i == 0)
                                {
                                    var monthYearPlan1 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan1);
                                    var monthYearPlan2 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan2);
                                    monthYearPlan3 = new MonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "September",
                                        NameAm = "መስከረም",
                                        StartDate = monthYearPlan2.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/01/" + (year + 1),
                                        EndDate = leap ? monthYearPlan2.EndDate.Value.AddDays(36) : monthYearPlan2.EndDate.Value.AddDays(35),                                        
                                        EndDateEth = "30/01/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.MonthlyProductionPlans.Add(monthYearPlan3);

                                }
                                if (i == 1)
                                {
                                    var monthYearPlan4 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan4);
                                    var monthYearPlan5 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan5);
                                    monthYearPlan6 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan6);

                                }
                                if (i == 2)
                                {
                                    var monthYearPlan7 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan7);
                                    var monthYearPlan8 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan8);
                                    monthYearPlan9 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan9);

                                }
                                if (i == 3)
                                {
                                    var monthYearPlan10 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan10);
                                    var monthYearPlan11 = new MonthlyProductionPlan
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
                                    db.MonthlyProductionPlans.Add(monthYearPlan11);
                                    var monthYearPlan12 = new MonthlyProductionPlan
                                    {
                                        QuarterlyProductionPlanId = quarterPlan.QuarterlyProductionPlanId,
                                        Name = "June",
                                        NameAm = "ሰኔ",
                                        StartDate = monthYearPlan11.EndDate.Value.AddDays(1),
                                        StartDateEth = "01/10/" + (year + 1),
                                        EndDate = monthYearPlan11.EndDate.Value.AddDays(30),
                                        EndDateEth = "30/10/" + (year + 1),
                                        WorkingDays = 0,
                                        WorkingHours = 0,
                                        ProductionPlanQuantity = 0,
                                        ProductionPlanBirr = 0
                                    };
                                    db.MonthlyProductionPlans.Add(monthYearPlan12);

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

                                var weeklyPlan1 = new WeeklyProductionPlan
                                {
                                    MonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
                                    Name = "First Week",
                                    NameAm = "የመጀመርያ ሳምንት",
                                    StartDate = monthPlan.StartDate,
                                    StartDateEth = monthPlan.StartDateEth,
                                    EndDate = monthPlan.StartDate.Value.AddDays(6),
                                    EndDateEth = XAPI.EthiopicDateTime.GetEthiopicDate(monthPlan.StartDate.Value.AddDays(6).Day, monthPlan.StartDate.Value.AddDays(6).Month, monthPlan.StartDate.Value.AddDays(6).Year),
                                    ProductionPlanQuantity = 0,
                                    ProductionPlanBirr = 0
                                };
                                db.WeeklyProductionPlans.Add(weeklyPlan1);
                                var weeklyPlan2 = new WeeklyProductionPlan
                                {
                                    MonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
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
                                db.WeeklyProductionPlans.Add(weeklyPlan2);
                                var weeklyPlan3 = new WeeklyProductionPlan
                                {
                                    MonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
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
                                db.WeeklyProductionPlans.Add(weeklyPlan3);
                                var weeklyPlan4 = new WeeklyProductionPlan
                                {
                                    MonthlyProductionPlanId = monthPlan.MonthlyProductionPlanId,
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
                                db.WeeklyProductionPlans.Add(weeklyPlan4);

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
        public ActionResult YearlyProductionPlans_Update([DataSourceRequest]DataSourceRequest request, YearlyProductionPlan yearlyPlan)
        {

            if (ModelState.IsValid)
            {

                var entity = db.YearlyProductionPlans.Find(yearlyPlan.YearlyProductionPlanId);
                if (entity != null)
                {

                    entity.YearlyProductionPlanId = yearlyPlan.YearlyProductionPlanId;
                    entity.GlFiscalYearId = yearlyPlan.GlFiscalYearId;
                    entity.StartDate = yearlyPlan.StartDate;
                    entity.EndDate = yearlyPlan.EndDate;
                    entity.ProductionPlanBirr = yearlyPlan.ProductionPlanBirr;
                    entity.ProductionPlanQuantity = yearlyPlan.ProductionPlanQuantity;

                    db.YearlyProductionPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }


            }

            return Json(new[] { yearlyPlan }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult YearlyProductionPlans_Destroy([DataSourceRequest]DataSourceRequest request, YearlyProductionPlan yearlyPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new YearlyProductionPlan
                {
                    YearlyProductionPlanId = yearlyPlan.YearlyProductionPlanId,
                    GlFiscalYearId = yearlyPlan.GlFiscalYearId,
                    StartDate = yearlyPlan.StartDate,
                    EndDate = yearlyPlan.EndDate,
                    ProductionPlanQuantity = yearlyPlan.ProductionPlanQuantity,
                    ProductionPlanBirr = yearlyPlan.ProductionPlanBirr,

                };



                db.YearlyProductionPlans.Attach(entity);
                db.YearlyProductionPlans.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { yearlyPlan }.ToDataSourceResult(request, ModelState));
        }


    }
}
