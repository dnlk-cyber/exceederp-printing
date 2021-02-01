using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    public class HealthRankAndValueController : Controller
    {
        // GET: HRM/HealthRankAndValue
        ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Employee"] = db.Person_Employee.ToList();
            ViewData["Branch"] = db.Branch.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["Period"] = db.GLPeriods.ToList();

        }
        public decimal AllowedAmount(int id)
        {
            decimal allowedAmount = 0;
            decimal remainingAmount = 0;
            decimal prevUsed = 0;

            var placement = db.Placements.OrderByDescending(x => x.PlacementId).FirstOrDefault(x => x.EmployeeId == id);

            if (placement != null)
            {
                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                {
                    allowedAmount = Convert.ToDecimal(rank.CurrentValue);
                    // remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed;
                    //if(remainingAmount < 0)
                    //{
                    //    //ModelState.AddModelError("", "your are using more than your balance");
                    //    var error = "your are using more than your balance";
                    //}
                }
            }

            return allowedAmount;

        }
        public string GetEmployeeCode(int id)
        {
            string code = "";
            var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == id);
            if(person != null)
            {
                code = person.EmployeeCode;
            }
            return code;
        }
        public decimal GetRemainingAmount(int empId, DateTime recoredDate, int fiscalYearId, decimal allowed,decimal singleUsed)
        {
            decimal remaining = 0;

            var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == empId && x.RecoredDate < recoredDate && x.GlFiscalYearId == fiscalYearId);
            if(healthPayments.Any() && allowed > 0)
            {
                remaining = allowed - healthPayments.Sum(x=>x.UsedAmount) - singleUsed;
            }
            else if(allowed > 0)
            {
                remaining = allowed - singleUsed;
            }
            return remaining;
        }
        public string GetFiscalYearName(int id)
        {
            var name = "";
            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x=>x.GlFiscalYearId == id);
            if(fiscalYear != null)
            {
                name = fiscalYear.Name;
            }
            return name;
        }
        public JsonResult HealthRankAndValue_Read([DataSourceRequest] DataSourceRequest request)
        {
            var healths = db.HealthRankAndValues.ToList();
            var accidents = healths.Select(health => new HealthRankAndValue
            {
                HealthRankAndValueId = health.HealthRankAndValueId,
                AllowedAmount = AllowedAmount(health.EmployeeId),//health.AllowedAmount,
                BranchId = health.BranchId,
                ClinicName = health.ClinicName,
                Date = health.Date,
                DateAmh = health.DateAmh,
                EmployeeId = health.EmployeeId,
                GlFiscalYearId = health.GlFiscalYearId,
                FiscalYearName = GetFiscalYearName(health.GlFiscalYearId),
                GLPeriodId = health.GLPeriodId,
                RecoredDate = health.RecoredDate,
                RecoredDateAmh = health.RecoredDateAmh,
                RemainingAmount = GetRemainingAmount(health.EmployeeId,health.RecoredDate,health.GlFiscalYearId,health.AllowedAmount,health.UsedAmount),
                UsedAmount = health.UsedAmount,
                EmployeeCode = GetEmployeeCode(health.EmployeeId)

            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult HealthRankAndValue_Create([DataSourceRequest] DataSourceRequest request, HealthRankAndValue health)
        {
            ModelState.Clear();

            if (health.DateAmh != null)
            {
                var splitDate = health.DateAmh.Split('/');
                health.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "treatment date is required");
            }
            if (health.RecoredDateAmh != null)
            {
                var splitDate = health.RecoredDateAmh.Split('/');
                health.RecoredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "recored date is required");
            }
            var branchId = 0;

            decimal allowedAmount = 0;
            decimal remainingAmount = 0;
            decimal prevUsed = 0;
            var healthValue = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId);
            if (healthValue.Any())
            {
                prevUsed = healthValue.Sum(x => x.UsedAmount);
            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                branchId = placement.BranchId;
                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                {
                    allowedAmount = Convert.ToDecimal(rank.CurrentValue);
                    remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed;
                    //if(remainingAmount < 0)
                    //{
                    //    //ModelState.AddModelError("", "your are using more than your balance");
                    //    var error = "your are using more than your balance";
                    //}
                }
            }
            else
            {
                placement = db.Placements.Where(x => x.EmployeeId == health.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                if (placement != null)
                {
                    branchId = placement.BranchId;
                    var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                    if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                    {
                        allowedAmount = Convert.ToDecimal(rank.CurrentValue);
                        remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed;
                    }
                }
            }

            if (ModelState.IsValid)
            {

                var entity = new HealthRankAndValue
                {
                    AllowedAmount = allowedAmount,
                    BranchId = branchId,
                    ClinicName = health.ClinicName,
                    Date = health.Date,
                    DateAmh = health.DateAmh,
                    EmployeeId = health.EmployeeId,
                    GlFiscalYearId = health.GlFiscalYearId,
                    GLPeriodId = health.GLPeriodId,
                    RemainingAmount = remainingAmount,
                    UsedAmount = health.UsedAmount,
                    RecoredDate = health.RecoredDate,
                    RecoredDateAmh = health.RecoredDateAmh
                };
                db.HealthRankAndValues.Add(entity);
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Health payment by rank", "Success", User.Identity.Name, User.Identity.Name + " add new health payment ", Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValue_Update([DataSourceRequest] DataSourceRequest request, HealthRankAndValue health)
        {

            ModelState.Clear();
            decimal used = health.UsedAmount;
            decimal remain = health.RemainingAmount;
            decimal allowd = health.AllowedAmount;

            var hlth = db.HealthRankAndValues.FirstOrDefault(x => x.HealthRankAndValueId == health.HealthRankAndValueId);
            if (hlth != null)
            {
                used = hlth.UsedAmount;
                /*hlth.RemainingAmount = hlth.RemainingAmount + hlth.UsedAmount*/
                ;

                //db.HealthRankAndValues.Attach(hlth);
                db.Entry(hlth).State = EntityState.Detached;
                //db.SaveChanges();


            }

            if (health.DateAmh != null)
            {
                var splitDate = health.DateAmh.Split('/');
                health.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "treatment date is required");
            }
            if (health.RecoredDateAmh != null)
            {
                var splitDate = health.RecoredDateAmh.Split('/');
                health.RecoredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "recored date is required");
            }
            var branchId = 0;

            decimal allowedAmount = 0;
            decimal remainingAmount = 0;
            decimal prevUsed = 0;
            var healthValue = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId);
            if (healthValue.Any())
            {
                prevUsed = healthValue.Sum(x => x.UsedAmount);
            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
            if (placement != null)
            {
                branchId = placement.BranchId;
                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                {
                    allowedAmount = Convert.ToDecimal(rank.CurrentValue);
                    remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed + used;
                    health.RemainingAmount = remainingAmount;
                    //if(remainingAmount < 0)
                    //{
                    //    //ModelState.AddModelError("", "your are using more than your balance");
                    //    var error = "your are using more than your balance";
                    //}
                }
            }
            else
            {
                placement = db.Placements.Where(x => x.EmployeeId == health.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                if (placement != null)
                {
                    branchId = placement.BranchId;
                    var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                    if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                    {
                        allowedAmount = Convert.ToDecimal(rank.CurrentValue);
                        remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed + used;
                        health.RemainingAmount = remainingAmount;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new HealthRankAndValue
                {
                    HealthRankAndValueId = health.HealthRankAndValueId,
                    AllowedAmount = health.AllowedAmount,
                    BranchId = health.BranchId,
                    ClinicName = health.ClinicName,
                    Date = health.Date,
                    DateAmh = health.DateAmh,
                    EmployeeId = health.EmployeeId,
                    GlFiscalYearId = health.GlFiscalYearId,
                    GLPeriodId = health.GLPeriodId,
                    RemainingAmount = health.RemainingAmount,
                    UsedAmount = health.UsedAmount,
                    RecoredDate = health.RecoredDate,
                    RecoredDateAmh = health.RecoredDateAmh
                };
                db.HealthRankAndValues.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValue_Destroy([DataSourceRequest] DataSourceRequest request, HealthRankAndValue health)
        {
            ModelState.Clear();

            decimal used = health.UsedAmount;


            if (ModelState.IsValid)
            {

                var entity = new HealthRankAndValue
                {
                    HealthRankAndValueId = health.HealthRankAndValueId,
                    AllowedAmount = health.AllowedAmount,
                    BranchId = health.BranchId,
                    ClinicName = health.ClinicName,
                    Date = health.Date,
                    DateAmh = health.DateAmh,
                    EmployeeId = health.EmployeeId,
                    GlFiscalYearId = health.GlFiscalYearId,
                    GLPeriodId = health.GLPeriodId,
                    RemainingAmount = health.RemainingAmount,
                    UsedAmount = health.UsedAmount,
                    RecoredDate = health.RecoredDate,
                    RecoredDateAmh = health.RecoredDateAmh
                };

                db.HealthRankAndValues.Attach(entity);
                db.HealthRankAndValues.Remove(entity);
                db.SaveChanges();
                //db.Entry(entity).State = EntityState.Detached;

                //var healths = db.HealthRankAndValues.Where(x => x.EmployeeId == entity.EmployeeId && x.GlFiscalYearId == entity.GlFiscalYearId).OrderByDescending(x => x.Date).ToList();
                //if (healths.Any())
                //{
                //    var hl = healths.FirstOrDefault();

                //    hl.RemainingAmount = hl.RemainingAmount + used;

                //    db.Entry(hl).State = EntityState.Modified;
                //    db.SaveChanges();
                //}


                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Health payment history " + entity.HealthRankAndValueId);
                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        }
    }
}