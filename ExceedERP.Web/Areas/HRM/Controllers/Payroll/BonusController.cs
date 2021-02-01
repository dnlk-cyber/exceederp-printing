using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Admin + "," + Payroll_Process)]
    public class BonusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        private const string Payroll_Process = "Payroll_Process";

        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            GetPeriod();
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears.ToList();//.Where(x => x.Status == FiscalYearStatus.Open);
            var listt = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            ViewData["PeriodList"] = periodList;// listt.Select(x => x.g).ToList(); ;

            return Json(periodList.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetPeriod()
        //{
        //    var periodList = db.GLFiscalYears.Where(x => x.Status == FiscalYearStatus.Open);
        //    var listt = from p in periodList
        //                join g in db.GLPeriods
        //                    on p.Name equals g.Name
        //                select (new
        //                {
        //                    g
        //                });
        //    ViewData["PeriodList"] = listt.Select(x => x.g).ToList(); ;

        //    return Json(listt.Select(p => new
        //    {
        //        GLPeriodId = p.g.GLPeriodId,
        //        Name = p.g.Name
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Bonuses_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Bonus> bonuses = db.Bonuses.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = bonuses.ToDataSourceResult(request, bonus => new
            {
                BonusId = bonus.BonusId,
                EmployeeId = bonus.EmployeeId,
                GlFiscalYearId = bonus.GlFiscalYearId,
                BounsFactor = bonus.BounsFactor,
                PerformanceRate = bonus.PerformanceRate,
                BonusType = bonus.BonusType,
                TotalBonus = bonus.TotalBonus,
                TotalIncomeTax = bonus.TotalIncomeTax,
                Remark = bonus.Remark,
                IsClosed = bonus.IsClosed,
                DateCreated = bonus.DateCreated,
                LastModified = bonus.LastModified,
                CreatedBy = bonus.CreatedBy,
                OrgBranchName = bonus.OrgBranchName,
                ModifiedBy = bonus.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Bonuses_Create([DataSourceRequest]DataSourceRequest request, Bonus bonus)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bonus
                {
                    EmployeeId = bonus.EmployeeId,
                    GlFiscalYearId = bonus.GlFiscalYearId,
                    BounsFactor = bonus.BounsFactor,
                    PerformanceRate = bonus.PerformanceRate,
                    BonusType = bonus.BonusType,
                    TotalBonus = bonus.TotalBonus,
                    TotalIncomeTax = bonus.TotalIncomeTax,
                    Remark = bonus.Remark,
                    IsClosed = bonus.IsClosed,
                    DateCreated = DateTime.Now,
                    LastModified = bonus.LastModified,
                    CreatedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    ModifiedBy = bonus.ModifiedBy
                };

                db.Bonuses.Add(entity);
                db.SaveChanges();
                bonus.BonusId = entity.BonusId;
            }

            return Json(new[] { bonus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Bonuses_Update([DataSourceRequest]DataSourceRequest request, Bonus bonus)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bonus
                {
                    EmployeeId = bonus.EmployeeId,
                    GlFiscalYearId = bonus.GlFiscalYearId,
                    BonusId = bonus.BonusId,
                    BounsFactor = bonus.BounsFactor,
                    PerformanceRate = bonus.PerformanceRate,
                    BonusType = bonus.BonusType,
                    TotalBonus = bonus.TotalBonus,
                    TotalIncomeTax = bonus.TotalIncomeTax,
                    Remark = bonus.Remark,
                    IsClosed = bonus.IsClosed,
                    DateCreated = bonus.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = bonus.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = bonus.OrgBranchName,
                };

                db.Bonuses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { bonus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Bonuses_Destroy([DataSourceRequest]DataSourceRequest request, Bonus bonus)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bonus
                {
                    EmployeeId = bonus.EmployeeId,
                    GlFiscalYearId = bonus.GlFiscalYearId,
                    BonusId = bonus.BonusId,
                    BounsFactor = bonus.BounsFactor,
                    PerformanceRate = bonus.PerformanceRate,
                    BonusType = bonus.BonusType,
                    TotalBonus = bonus.TotalBonus,
                    TotalIncomeTax = bonus.TotalIncomeTax,
                    Remark = bonus.Remark,
                    IsClosed = bonus.IsClosed,
                    DateCreated = bonus.DateCreated,
                    LastModified = bonus.LastModified,
                    CreatedBy = bonus.CreatedBy,
                    OrgBranchName = bonus.OrgBranchName,
                    ModifiedBy = bonus.ModifiedBy
                };

                db.Bonuses.Attach(entity);
                db.Bonuses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bonus }.ToDataSourceResult(request, ModelState));
        }

        public string CheckStatus(int id)
        {
            var list = db.Bonuses.Where(x => x.GlFiscalYearId == id).FirstOrDefault();
            if (list != null)
            {
                if (list.IsClosed)
                {
                    return "c";
                }
                else
                {
                    return "n";
                }
            }
            else
            {
                return "r";
            }
        }
        public ActionResult Close(int period)
        {
            var list = db.Bonuses.Where(x => x.GlFiscalYearId == period).ToList();
            if (!list.Any())
            {
                return new JavaScriptResult() { Script = "alert('" + "selected period is not processed!!" + "');" };
            }
            foreach (var l in list)
            {

                l.IsClosed = true;

                db.Entry(l).State = EntityState.Modified;
                db.SaveChanges();
            }
            return null;
        }
        public ActionResult Reverse(int period, int bType)
        {
            var list = db.Bonuses.Where(x => x.GlFiscalYearId == period).ToList();
            db.Bonuses.RemoveRange(list);
            db.SaveChanges();
            return null;
        }

        public ActionResult PrepareBonus(int period, string bType)
        {

            decimal bFactor = 0;
            decimal pRate = 0;
            decimal salary = 0;
            var placments = db.Placements.Where(x => x.isCurrent).ToList();

            var updates = db.Bonuses.FirstOrDefault(x => x.GlFiscalYearId == period && x.IsClosed);
            if (updates != null)
            {
                var formattedData = new
                {
                    Message = "You can't process closed bonuses.",
                    Status = "FAIL"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (bType == "0")
            {
                Bonus bonus = new Bonus();
                var bonusSetting = db.BonusSettings.Where(x => x.BunusType == BonusType.WITH_PERFORMANCE);
                if (bonusSetting.Any())
                {
                    decimal score = 0;
                    var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
                    foreach (var l in list)
                    {
                        var entity = new Bonus();
                        salary = 0;
                        entity.EmployeeId = l.PersonId;
                        entity.GlFiscalYearId = period;

                        var year = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId == period);
                        if (year != null)
                        {
                            var quarters = db.Quarters.Where(x => x.QuarterLengthId == year.QuarterLengthId).Select(x => x.QuarterId).ToList();
                            var performanceScore = db.EmployeePerformanceScores.Where(x => x.EmployeeId == l.PersonId && quarters.Contains(x.QuarterId));

                            if (performanceScore.Any())
                            {
                                score = performanceScore.Sum(x => x.Result)/year.Length;
                            }
                            var rank = db.PerformanceRank.FirstOrDefault(x=>x.Minimum <= score && x.Maximum >= score);
                            if(rank != null)
                            {
                                var checkMultiplayer = bonusSetting.FirstOrDefault(x => x.Multiplier.ToLower() == rank.Description.ToLower());
                                if(checkMultiplayer != null)
                                {
                                    entity.BounsFactor = checkMultiplayer.CurrentValue;
                                    entity.PerformanceRate = checkMultiplayer.Multiplier;


                                    var placemet = placments.FirstOrDefault(x => x.EmployeeId == l.PersonId && x.isCurrent);
                                    if (placemet != null)
                                    {
                                        var singleEmpBonus = placemet.Salary * checkMultiplayer.CurrentValue;
                                        var singleEmpBonusRound = Math.Round(singleEmpBonus, 2);
                                        entity.BonusType = BonusType.WITH_PERFORMANCE;
                                        

                                        entity.TotalBonus = Convert.ToDouble(singleEmpBonusRound);//Convert.ToDouble((salary * bFactor) * (Convert.ToInt32(entity.PerformanceRate) / 100));

                                        entity.TotalIncomeTax = Math.Round(XAPI.IncomeTax.GetIncomeTax((float)entity.TotalBonus), 2);
                                    }
                                }
                            }                          

                            entity.Remark = bonus.Remark;
                            entity.DateCreated = DateTime.Now;
                            entity.LastModified = null;
                            entity.CreatedBy = User.Identity.Name;
                            entity.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            entity.ModifiedBy = null;

                            var checkEmpBonus = db.Bonuses.FirstOrDefault(x => x.EmployeeId == l.PersonId && x.GlFiscalYearId == period && x.BonusType == BonusType.WITH_PERFORMANCE);

                            if (checkEmpBonus != null)
                            {
                                entity.BonusId = checkEmpBonus.BonusId;

                                db.Entry(checkEmpBonus).State = EntityState.Detached;
                                db.Bonuses.Attach(entity);
                                db.Entry(entity).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Bonuses.Add(entity);
                                db.SaveChanges();
                                bonus.BonusId = entity.BonusId;
                            }
                        }

                        score = 0;

                    }
                }
            }
            if (bType == "1")
            {
                Bonus bonus = new Bonus();
                var bonusSetting = db.BonusSettings.FirstOrDefault(x => x.BunusType == BonusType.FROM_PROFIT && x.GlFiscalYearId == period);
                if (bonusSetting != null)
                {
                    var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
                    foreach (var l in list)
                    {
                        var empLists = list.Select(x => x.PersonId);
                        var pp = placments.Where(x => empLists.Contains(x.EmployeeId));
                        decimal totalEmpSalary = 0;
                        if (pp.Any())
                        {
                            totalEmpSalary = pp.Sum(x => x.Salary);
                        }

                        var entity = new Bonus();
                        salary = 0;
                        entity.EmployeeId = l.PersonId;
                        entity.GlFiscalYearId = period;
                        var empPlacemnt = placments.FirstOrDefault(x => x.EmployeeId == l.PersonId);
                        if (empPlacemnt != null)
                        {
                            salary = empPlacemnt.Salary;
                        }

                        var singleEmpBonus = salary / totalEmpSalary * bonusSetting.CurrentValue;
                        var singleEmpBonusRound = Math.Round(singleEmpBonus, 2);
                        entity.BonusType = BonusType.FROM_PROFIT;
                        entity.BounsFactor = bFactor;
                        entity.PerformanceRate = pRate.ToString();
                        //entity.PerformanceRate = GetPerformanceRate(l.PersonId, period).ToString();

                        entity.TotalBonus = Convert.ToDouble(singleEmpBonusRound);//Convert.ToDouble((salary * bFactor) * (Convert.ToInt32(entity.PerformanceRate) / 100));

                        entity.TotalIncomeTax = Math.Round(XAPI.IncomeTax.GetIncomeTax((float)entity.TotalBonus), 2);
                        entity.Remark = bonus.Remark;
                        entity.DateCreated = DateTime.Now;
                        entity.LastModified = null;
                        entity.CreatedBy = User.Identity.Name;
                        entity.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        entity.ModifiedBy = null;

                        var checkEmpBonus = db.Bonuses.FirstOrDefault(x => x.EmployeeId == l.PersonId && x.GlFiscalYearId == period && x.BonusType == BonusType.FROM_PROFIT);

                        if (checkEmpBonus != null)
                        {
                            entity.BonusId = checkEmpBonus.BonusId;

                            db.Entry(checkEmpBonus).State = EntityState.Detached;
                            db.Bonuses.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Bonuses.Add(entity);
                            db.SaveChanges();
                            bonus.BonusId = entity.BonusId;
                        }

                    }
                }
                else
                {
                    return new JavaScriptResult() { Script = "alert('" + "you must set profit of current year in Bonus Setting" + "');" };
                }
            }


            return null;
        }

        private decimal GetPerformanceRate(int empId, int periodId)
        {
            decimal pRate = 100;
            var qauretrLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId == periodId);
            if (qauretrLength != null)
            {
                var quarters = db.Quarters.Where(x => x.QuarterLengthId == qauretrLength.QuarterLengthId).Select(x => x.QuarterId);
                var performances = db.EmployeeMeasurementDefinition.Where(x => quarters.Contains(x.QuarterId)).ToList();
                var performanceRanks = db.PerformanceRank.ToList();
                if (performances.Any())
                {
                    var result = performances.Sum(x => x.Result) / performances.Count;
                    var getoneRate = performanceRanks.Where(x => x.Minimum <= result && x.Maximum >= result).FirstOrDefault();
                    if (getoneRate != null)
                    {

                        if (getoneRate.BonusRate != 0)
                        {
                            pRate = getoneRate.BonusRate;
                        }

                    }

                }
            }


            return pRate;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
