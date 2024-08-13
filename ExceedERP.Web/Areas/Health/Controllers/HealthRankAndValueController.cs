using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    [Authorize]
    public class HealthRankAndValueController : Controller
    {
        // GET: HRM/HealthRankAndValue
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Employee"] = db.Person_Employee.ToList();
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits; ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
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
        public static bool IsNumeric(string input)
        {
            return Regex.IsMatch(input, @"^\d+$");
        }

        public static bool IsAlphabetic(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z]+$");
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

            var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == empId && x.RecoredDate < recoredDate && x.GlFiscalYearId == fiscalYearId&&x.BenefitType==HealthBenefitType.NA);
            if(healthPayments.Any() && allowed > 0)
            {
                remaining = allowed - healthPayments.Sum(x=>x.UsedAmount) - singleUsed;
            }
            else if( allowed > 0&& singleUsed <=allowed)
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
        public ActionResult HealthRankAndValue_Read([DataSourceRequest] DataSourceRequest request)
        {
            var healthRankAndValues = db.HealthRankAndValues.ToList();
            DataSourceResult result = healthRankAndValues.ToDataSourceResult(request, health => new
            {

                HealthRankAndValueId = health.HealthRankAndValueId,
                PaymentRequestId = health.PaymentRequestId,
                AllowedAmount = health.AllowedAmount,
                BranchId = health.BranchId,
                BusinessUnitId = health.BusinessUnitId,
                ClinicName = health.ClinicName,
                Date = health.Date,
                DateAmh = health.DateAmh,
                PaymentCoverage = health.PaymentCoverage,
                EmployeeId = health.EmployeeId,
                GlFiscalYearId = health.GlFiscalYearId,
                GLPeriodId = health.GLPeriodId,
                RemainingAmount = health.RemainingAmount,
                TotalPaid = health.TotalPaid,
                UsedAmount = health.UsedAmount,
                BenefitType = health.BenefitType,
                PaymentType = health.PaymentType,
                IsOtherBenifit = health.IsOtherBenifit,
                StaffContribution = health.StaffContribution,
                RecoredDate = health.RecoredDate,
                RecoredDateAmh = health.RecoredDateAmh,
                IsPartnerOrGov = health.IsPartnerOrGov,
                Status = health.Status,
                Remark = health.Remark,
                IsVoid = health.IsVoid,
                VoidBy = health.VoidBy,
                VoidTime = health.VoidTime,
                IsOnlineApproved = health.IsOnlineApproved,
                OnlineApprovedBy = health.OnlineApprovedBy,
                OnlineApprovedTime = health.OnlineApprovedTime,
                IsOnlineTransferred = health.IsOnlineTransferred,
                IsOnlineTransferredBy = health.IsOnlineTransferredBy,
                OnlineTransferredTime = health.OnlineTransferredTime,
                IsSendForApproval = health.IsSendForApproval,
                OrgBranchName = health.OrgBranchName,
                SendForApprovalBy = health.SendForApprovalBy,
                SendForApprovalTime = health.SendForApprovalTime,
                DateCreated = health.DateCreated,
                LastModified = health.LastModified,
                CreatedBy = health.CreatedBy,
                ModifiedBy = health.ModifiedBy,
                //AllowedAmount = AllowedAmount(health.EmployeeId),//health.AllowedAmount,
                //AllowedAmount = health.AllowedAmount,
                //FiscalYearName = GetFiscalYearName(health.GlFiscalYearId),
                EmployeeCode = GetEmployeeCode(health.EmployeeId),
                //RemainingAmount = GetRemainingAmount(health.EmployeeId, health.RecoredDate, health.GlFiscalYearId, health.AllowedAmount, health.UsedAmount),
               
            });

            return Json(result,JsonRequestBehavior.AllowGet);
       
        }
        #region comment Out
        //public JsonResult HealthRankAndValue_Create([DataSourceRequest] DataSourceRequest request, HealthRankAndValue health)
        //{
        //    ModelState.Clear();
        //    if (!health.IsOtherBenifit)
        //    {
        //        health.BenefitType = HealthBenefitType.NA;
        //    }
        //    decimal allowedAmount = 0;
        //    decimal remainingAmount = 0;
        //    decimal usedAmount = 0;

        //    if (health.DateAmh != null)
        //    {
        //        var splitDate = health.DateAmh.Split('/');
        //        health.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "treatment date is required");
        //    }
        //    if (health.RecoredDateAmh != null)
        //    {
        //        var splitDate = health.RecoredDateAmh.Split('/');
        //        health.RecoredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "recored date is required");

        //    }
        //    if (health.BenefitType == HealthBenefitType.EyeGlass)
        //    {
        //        var eyGlassPayment = db.HealthBenefitSettings.Where(x => x.BenefitType == health.BenefitType).FirstOrDefault();
        //        if(eyGlassPayment != null)
        //        {
                   
        //            health.AllowedAmount = eyGlassPayment.BenfitValue;
        //            if(eyGlassPayment.BenfitValue <= health.TotalPaid)
        //            {
        //                health.UsedAmount = eyGlassPayment.BenfitValue;
        //                health.AllowedAmount = eyGlassPayment.BenfitValue;
                       

        //            }
        //            health.UsedAmount = eyGlassPayment.BenfitValue;

        //        }
        //        else
        //        {
        //            ModelState.AddModelError("403", "Set Eye-glass Benfit first");
        //        }


        //    }
        //    else if (health.BenefitType == HealthBenefitType.Maternity)
        //    {
        //        var maternityPayment = db.HealthBenefitSettings.Where(x => x.BenefitType == health.BenefitType).FirstOrDefault();

        //        if(maternityPayment != null)
        //        {
        //            var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId && x.RecoredDate < health.RecoredDate && x.GlFiscalYearId == health.GlFiscalYearId && x.BenefitType == HealthBenefitType.Maternity).OrderByDescending(x=>x.HealthRankAndValueId);
        //            if (healthPayments.Any() && maternityPayment.BenfitValue > 0)
        //            {
        //                if(maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount) > 0)
        //                {
        //                    if ((maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount)) > health.TotalPaid)
        //                    {
        //                        health.UsedAmount = health.TotalPaid;
        //                        health.RemainingAmount = maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount) - health.TotalPaid;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount);
        //                        health.RemainingAmount = 0;
        //                    }
        //                }
        //                else
        //                {
        //                    health.UsedAmount = 0;
        //                    health.AllowedAmount = 0;
        //                }
        //            }
        //            else
        //            {
        //                if (health.TotalPaid > maternityPayment.BenfitValue)
        //                {
        //                    health.UsedAmount = maternityPayment.BenfitValue;
        //                    health.AllowedAmount = maternityPayment.BenfitValue;
        //                    health.RemainingAmount = 0;


        //                }
        //                else
        //                {
        //                    health.UsedAmount = health.TotalPaid;
        //                    health.AllowedAmount = maternityPayment.BenfitValue;
        //                    health.RemainingAmount = maternityPayment.BenfitValue-health.TotalPaid;




        //                }
        //            }

                   

        //        }
        //        else
        //        {
        //            ModelState.AddModelError("403", "Set Maternity  Benfit setting first");
        //        }


        //    }
        //    else
        //    {
        //        usedAmount = (health.PaymentCoverage * health.TotalPaid) / 100;
        //        var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId &&x.RecoredDate<health.RecoredDate&& x.GlFiscalYearId == health.GlFiscalYearId);
        //        if (healthPayments.Any())
        //        {
        //            var maxRecordDate = healthPayments.Max(x=>x.RecoredDate);
        //            var remainingAmountCal = healthPayments
        //                .Where(x => x.RecoredDate == maxRecordDate)
        //                .Select(x => x.RemainingAmount).FirstOrDefault();
        //            var placement2 = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //            if (placement2 != null)
        //            {

        //                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement2.RankId);

        //                var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement2.ResponsibilityLevel);

        //                //if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
 
        //                if (rank !=null &&IsNumeric(rank.CurrentValue))
        //                {
        //                    if (usedAmount > remainingAmountCal)
        //                    {
        //                        health.UsedAmount = remainingAmountCal;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount =  0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount = remainingAmountCal- usedAmount;
        //                    }

        //                }
        //                if (rank !=null &&IsAlphabetic(rank.CurrentValue))
        //                {
        //                    health.AllowedAmount = 1000000;
        //                    health.RemainingAmount = 1000000;

        //                }
        //               //else if (level != null && level.CurrentValue.ToLower() != "unlimited")
        //                else if (level !=null&&IsAlphabetic(level.CurrentValue))
        //                {
        //                    health.AllowedAmount = 1000000;
        //                    health.RemainingAmount = 1000000;
        //                }
        //                else if (level !=null&&IsNumeric(level.CurrentValue))
        //                {
        //                    if (usedAmount > remainingAmountCal)
        //                    {
        //                        health.UsedAmount = remainingAmountCal;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount = 0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount = remainingAmountCal - usedAmount;
        //                    }
        //                }
        //            }

        //        }
        //        else
        //        {

        //            var placement1 = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //            if (placement1 != null)
        //            {
        //                health.BranchId = placement1.BranchId;

        //                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement1.RankId);

        //                var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement1.ResponsibilityLevel);

        //                //if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
        //                if (IsNumeric(rank.CurrentValue))
        //                {
        //                    if (usedAmount > Convert.ToDecimal(rank.CurrentValue))
        //                    {
        //                        health.UsedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.RemainingAmount = 0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
        //                        health.RemainingAmount = Convert.ToDecimal(rank.CurrentValue) - usedAmount;
        //                    }
        //                }
        //                //if (rank != null && rank.CurrentValue.ToLower() == "unlimited")

        //               else if (IsAlphabetic(rank.CurrentValue))
        //                {
        //                    health.AllowedAmount = 1000000;
        //                    health.RemainingAmount = 1000000;

        //                }

        //                //if (level != null && level.CurrentValue.ToLower() != "unlimited")
        //                else if (IsNumeric(level.CurrentValue))
        //                {
        //                    if (usedAmount > Convert.ToDecimal(rank.CurrentValue))
        //                    {
        //                        health.UsedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.RemainingAmount = 0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
        //                        health.RemainingAmount = Convert.ToDecimal(rank.CurrentValue) - usedAmount;
        //                    }
        //                }
        //               //else if (level != null && level.CurrentValue.ToLower() == "unlimited")
        //               else if (IsAlphabetic(level.CurrentValue))
        //                {
        //                    health.AllowedAmount = 1000000;
        //                    health.RemainingAmount  = 1000000;

        //                }
        //            }


        //        }
        //    }

        //    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //    if (placement != null)
        //    {
        //        health.BranchId = placement.BranchId;
        //    }

        //    if (ModelState.IsValid)
        //    {

        //        var entity = new HealthRankAndValue
        //        {
        //            AllowedAmount = allowedAmount,
        //            BranchId = health.BranchId,
        //            ClinicName = health.ClinicName,
        //            Date = health.Date,
        //            DateAmh = health.DateAmh,
        //            PaymentCoverage = health.PaymentCoverage,
        //            BenefitType = health.BenefitType,
        //            IsOtherBenifit = health.IsOtherBenifit,
        //            EmployeeId = health.EmployeeId,
        //            GlFiscalYearId = health.GlFiscalYearId,
        //            GLPeriodId = health.GLPeriodId,
        //            TotalPaid = health.TotalPaid,
        //            UsedAmount = health.UsedAmount,
        //            StaffContribution = health.StaffContribution,
        //            RemainingAmount = remainingAmount,
        //            IsPartnerOrGov = health.IsPartnerOrGov,
        //            RecoredDate = health.RecoredDate,
        //            RecoredDateAmh = health.RecoredDateAmh,
        //            Status= health.Status,
        //            Remark = health.Remark,
        //            IsVoid = health.IsVoid,
        //            VoidBy = health.VoidBy,
        //            VoidTime = health.VoidTime,
        //            IsOnlineApproved = health.IsOnlineApproved,
        //            OnlineApprovedBy = health.OnlineApprovedBy,
        //            OnlineApprovedTime = health.OnlineApprovedTime,
        //            IsOnlineTransferred = health.IsOnlineTransferred,
        //            IsOnlineTransferredBy = health.IsOnlineTransferredBy,
        //            OnlineTransferredTime = health.OnlineTransferredTime,
        //            IsSendForApproval = health.IsSendForApproval,
        //            OrgBranchName = health.OrgBranchName,
        //            SendForApprovalBy = health.SendForApprovalBy,
        //            SendForApprovalTime = health.SendForApprovalTime,
        //            DateCreated = health.DateCreated,
        //            LastModified = health.LastModified,
        //            CreatedBy = health.CreatedBy,
        //            ModifiedBy = health.ModifiedBy,
        //        };
        //        db.HealthRankAndValues.Add(entity);
        //        db.SaveChanges();

        //        #region operation log                

        //        UserHelper.OperationLog(Request.UserHostAddress, "Health payment by rank", "Success", User.Identity.Name, User.Identity.Name + " add new health payment ", Modules.HealthAndInsurance);

        //        #endregion
        //    }

        //    return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        //}
        //public JsonResult HealthRankAndValue_Create([DataSourceRequest] DataSourceRequest request, HealthRankAndValue health)
        //{
        //    ModelState.Clear();
        //    var branchId = 0;
        //    decimal allowedAmount = 0;
        //    decimal remainingAmount = 0;
        //    decimal prevUsed = 0;
        //    if (health.IsPartnerOrGov)
        //    {
        //        health.UsedAmount = (85 * health.TotalPaid) / 100;
        //        health.StaffContribution = (15 * health.TotalPaid) / 100;
        //    }
        //    else
        //    {
        //        health.UsedAmount = (50 * health.TotalPaid) / 100;
        //        health.StaffContribution = (50 * health.TotalPaid) / 100;
        //    }
        //    if (health.DateAmh != null)
        //    {
        //        var splitDate = health.DateAmh.Split('/');
        //        health.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "treatment date is required");
        //    }
        //    if (health.RecoredDateAmh != null)
        //    {
        //        var splitDate = health.RecoredDateAmh.Split('/');
        //        health.RecoredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "recored date is required");
        //    }
        //    health.UsedAmount = (health.PaymentCoverage * health.TotalPaid) / 100;
        //    var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId);
        //    if (healthPayments.Any())
        //    {

        //        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //        if (placement != null)
        //        {

        //            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);

        //            var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel);

        //            if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
        //            {
        //                health.AllowedAmount = health.UsedAmount;
        //                health.RemainingAmount = (healthPayments.Sum(x => x.UsedAmount)) > health.UsedAmount ? (healthPayments.Sum(x => x.UsedAmount) - health.UsedAmount) : 0;
        //            }
        //            if (level != null && level.CurrentValue.ToLower() != "unlimited")
        //            {
        //                health.AllowedAmount = health.UsedAmount;
        //                health.RemainingAmount = (healthPayments.Sum(x => x.UsedAmount)) > health.UsedAmount ? (healthPayments.Sum(x => x.UsedAmount) - health.UsedAmount) : 0;
        //            }
        //        }

        //    }
        //    else
        //    {

        //        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //        if (placement != null)
        //        {
        //            branchId = placement.BranchId;

        //            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);

        //            var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel);

        //            if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
        //            {
        //                allowedAmount = Convert.ToDecimal(rank.CurrentValue);
        //                remainingAmount = allowedAmount > health.UsedAmount ? (allowedAmount - health.UsedAmount) : 0;
        //            }
        //            if (rank != null && rank.CurrentValue.ToLower() == "unlimited")
        //            {
        //                allowedAmount = 0;
        //                remainingAmount = 0;
        //            }

        //            if (level != null && level.CurrentValue.ToLower() != "unlimited")
        //            {
        //                allowedAmount = Convert.ToDecimal(level.CurrentValue);
        //                remainingAmount = allowedAmount > health.UsedAmount ? (allowedAmount - health.UsedAmount) : 0;
        //            }
        //            if (level != null && level.CurrentValue.ToLower() == "unlimited")
        //            {
        //                allowedAmount = 0;
        //                remainingAmount = 0;
        //            }
        //        }


        //    }

        //    var branchId = 0;

        //    decimal allowedAmount = 0;
        //    decimal remainingAmount = 0;
        //    decimal prevUsed = 0;
        //    var healthValue = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId);
        //    if (healthValue.Any())
        //    {
        //        prevUsed = healthValue.Sum(x => x.UsedAmount);
        //    }
        //    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //    if (placement != null)
        //    {
        //        branchId = placement.BranchId;
        //        var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId || !placement.IsHead);
        //        if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
        //        {
        //            allowedAmount = Convert.ToDecimal(rank.CurrentValue);
        //            remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed;
        //            //if (remainingAmount < 0)
        //            //{
        //            //    ModelState.AddModelError("", "your are used all Allowed Amounts");
        //            //  //  var error = "your are using more than your balance";
        //            //}
        //        }
        //        var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel || !placement.IsHead);
        //        if (level != null && level.CurrentValue.ToLower() != "unlimited")
        //        {
        //            allowedAmount = Convert.ToDecimal(rank.CurrentValue);
        //            remainingAmount = Convert.ToDecimal(rank.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed;
        //            //if (remainingAmount < 0)
        //            //{
        //            //    ModelState.AddModelError("", "your are used all Allowed Amounts");
        //            //  //  var error = "your are using more than your balance";
        //            //}
        //        }


        //    }
        //    else
        //    {
        //        placement = db.Placements.Where(x => x.EmployeeId == health.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
        //        if (placement != null)
        //        {
        //            branchId = placement.BranchId;
        //            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId || !placement.IsHead);
        //            if (rank != null && rank.CurrentValue.ToLower() == "unlimited")
        //            {
        //                allowedAmount = health.TotalPaid;

        //                remainingAmount = health.TotalPaid;
        //            }

        //        }
        //        var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel || !placement.IsHead);
        //        if (level != null && level.CurrentValue.ToLower() == "unlimited")
        //        {
        //            allowedAmount = Convert.ToDecimal(level.CurrentValue);
        //            remainingAmount = Convert.ToDecimal(level.CurrentValue) - Convert.ToDecimal(health.UsedAmount) - prevUsed;

        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {

        //        var entity = new HealthRankAndValue
        //        {
        //            AllowedAmount = allowedAmount,
        //            BranchId = branchId,
        //            ClinicName = health.ClinicName,
        //            Date = health.Date,
        //            DateAmh = health.DateAmh,
        //            PaymentCoverage = health.PaymentCoverage,
        //            EmployeeId = health.EmployeeId,
        //            GlFiscalYearId = health.GlFiscalYearId,
        //            GLPeriodId = health.GLPeriodId,
        //            TotalPaid = health.TotalPaid,
        //            UsedAmount = health.UsedAmount,
        //            StaffContribution = health.StaffContribution,
        //            RemainingAmount = remainingAmount,
        //            IsPartnerOrGov = health.IsPartnerOrGov,
        //            RecoredDate = health.RecoredDate,
        //            RecoredDateAmh = health.RecoredDateAmh,
        //            Status = health.Status,
        //            Remark = health.Remark,
        //            IsVoid = health.IsVoid,
        //            VoidBy = health.VoidBy,
        //            VoidTime = health.VoidTime,
        //            IsOnlineApproved = health.IsOnlineApproved,
        //            OnlineApprovedBy = health.OnlineApprovedBy,
        //            OnlineApprovedTime = health.OnlineApprovedTime,
        //            IsOnlineTransferred = health.IsOnlineTransferred,
        //            IsOnlineTransferredBy = health.IsOnlineTransferredBy,
        //            OnlineTransferredTime = health.OnlineTransferredTime,
        //            IsSendForApproval = health.IsSendForApproval,
        //            OrgBranchName = health.OrgBranchName,
        //            SendForApprovalBy = health.SendForApprovalBy,
        //            SendForApprovalTime = health.SendForApprovalTime,
        //            DateCreated = health.DateCreated,
        //            LastModified = health.LastModified,
        //            CreatedBy = health.CreatedBy,
        //            ModifiedBy = health.ModifiedBy,
        //        };
        //        db.HealthRankAndValues.Add(entity);
        //        db.SaveChanges();

        //        #region operation log                

        //        UserHelper.OperationLog(Request.UserHostAddress, "Health payment by rank", "Success", User.Identity.Name, User.Identity.Name + " add new health payment ", Modules.HealthAndInsurance);

        //        #endregion
        //    }

        //    return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public JsonResult HealthRankAndValue_Update([DataSourceRequest] DataSourceRequest request, HealthRankAndValue health)
        //{
        //    decimal allowedAmount = 0;
        //    decimal remainingAmount = 0;
        //    decimal usedAmount = 0;
        //    if (health.DateAmh != null)
        //    {
        //        var splitDate = health.DateAmh.Split('/');
        //        health.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "treatment date is required");
        //    }
        //    if (health.RecoredDateAmh != null)
        //    {
        //        var splitDate = health.RecoredDateAmh.Split('/');
        //        health.RecoredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "recored date is required");
        //    }
        //    if (health.BenefitType == HealthBenefitType.EyeGlass)
        //    {
        //        var eyGlassPayment = db.HealthBenefitSettings.Where(x => x.BenefitType == health.BenefitType).FirstOrDefault();
        //        if (eyGlassPayment != null)
        //        {

        //            health.AllowedAmount = eyGlassPayment.BenfitValue;
        //            if (eyGlassPayment.BenfitValue <= health.TotalPaid)
        //            {
        //                health.UsedAmount = eyGlassPayment.BenfitValue;
        //                health.AllowedAmount = eyGlassPayment.BenfitValue;


        //            }
        //            health.UsedAmount = eyGlassPayment.BenfitValue;

        //        }
        //        else
        //        {
        //            ModelState.AddModelError("403", "Set Eye-glass Benfit first");
        //        }


        //    }
        //    else if (health.BenefitType == HealthBenefitType.Maternity)
        //    {
        //        var maternityPayment = db.HealthBenefitSettings.Where(x => x.BenefitType == health.BenefitType).FirstOrDefault();

        //        if (maternityPayment != null)
        //        {
        //            var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId && x.RecoredDate < health.RecoredDate && x.GlFiscalYearId == health.GlFiscalYearId && x.BenefitType == HealthBenefitType.Maternity).OrderByDescending(x => x.HealthRankAndValueId);
        //            if (healthPayments.Any() && maternityPayment.BenfitValue > 0)
        //            {
        //                if (maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount) > 0)
        //                {
        //                    if ((maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount)) > health.TotalPaid)
        //                    {
        //                        health.UsedAmount = health.TotalPaid;
        //                        health.RemainingAmount = maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount) - health.TotalPaid;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = maternityPayment.BenfitValue - healthPayments.Sum(x => x.UsedAmount);
        //                        health.RemainingAmount = 0;
        //                    }
        //                }
        //                else
        //                {
        //                    health.UsedAmount = 0;
        //                    health.AllowedAmount = 0;
        //                }
        //            }
        //            else
        //            {
        //                if (health.TotalPaid > maternityPayment.BenfitValue)
        //                {
        //                    health.UsedAmount = maternityPayment.BenfitValue;
        //                    health.AllowedAmount = maternityPayment.BenfitValue;
        //                    health.RemainingAmount = 0;


        //                }
        //                else
        //                {
        //                    health.UsedAmount = health.TotalPaid;
        //                    health.AllowedAmount = maternityPayment.BenfitValue;
        //                    health.RemainingAmount = maternityPayment.BenfitValue - health.TotalPaid;




        //                }
        //            }



        //        }
        //        else
        //        {
        //            ModelState.AddModelError("403", "Set Maternity  Benfit setting first");
        //        }


        //    }
        //    else
        //    {
        //        usedAmount = (health.PaymentCoverage * health.TotalPaid) / 100;
        //        var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId && x.RecoredDate < health.RecoredDate && x.GlFiscalYearId == health.GlFiscalYearId);
        //        if (healthPayments.Any())
        //        {
        //            var maxRecordDate = healthPayments.Max(x => x.RecoredDate);
        //            var remainingAmountCal = healthPayments
        //                .Where(x => x.RecoredDate == maxRecordDate)
        //                .Select(x => x.RemainingAmount).FirstOrDefault();
        //            var placement2 = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //            if (placement2 != null)
        //            {

        //                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement2.RankId);

        //                var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement2.ResponsibilityLevel);

        //                //if (rank != null && rank.CurrentValue.ToLower() != "unlimited")

        //                if (rank != null && IsNumeric(rank.CurrentValue))
        //                {
        //                    if (usedAmount > remainingAmountCal)
        //                    {
        //                        health.UsedAmount = remainingAmountCal;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount = 0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount = remainingAmountCal - usedAmount;
        //                    }

        //                }
        //                if (rank != null && IsAlphabetic(rank.CurrentValue))
        //                {
        //                    health.AllowedAmount = 1000000;
        //                    health.RemainingAmount = 1000000;

        //                }
        //                //else if (level != null && level.CurrentValue.ToLower() != "unlimited")
        //                else if (level != null && IsAlphabetic(level.CurrentValue))
        //                {
        //                    health.AllowedAmount = 1000000;
        //                    health.RemainingAmount = 1000000;
        //                }
        //                else if (level != null && IsNumeric(level.CurrentValue))
        //                {
        //                    if (usedAmount > remainingAmountCal)
        //                    {
        //                        health.UsedAmount = remainingAmountCal;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount = 0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = remainingAmountCal;
        //                        health.RemainingAmount = remainingAmountCal - usedAmount;
        //                    }
        //                }
        //            }

        //        }
        //        else
        //        {

        //            var placement1 = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //            if (placement1 != null)
        //            {
        //                health.BranchId = placement1.BranchId;

        //                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement1.RankId);

        //                var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement1.ResponsibilityLevel);

        //                //if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
        //                if (IsNumeric(rank.CurrentValue))
        //                {
        //                    if (usedAmount > Convert.ToDecimal(rank.CurrentValue))
        //                    {
        //                        health.UsedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.RemainingAmount = 0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
        //                        health.RemainingAmount = Convert.ToDecimal(rank.CurrentValue) - usedAmount;
        //                    }
        //                }
        //                //if (rank != null && rank.CurrentValue.ToLower() == "unlimited")

        //                else if (IsAlphabetic(rank.CurrentValue))
        //                {
        //                    allowedAmount = 1000000;
        //                    remainingAmount = 1000000;
        //                }

        //                //if (level != null && level.CurrentValue.ToLower() != "unlimited")
        //                else if (IsNumeric(level.CurrentValue))
        //                {
        //                    if (usedAmount > Convert.ToDecimal(rank.CurrentValue))
        //                    {
        //                        health.UsedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue); ;
        //                        health.RemainingAmount = 0;

        //                    }
        //                    else
        //                    {
        //                        health.UsedAmount = usedAmount;
        //                        health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
        //                        health.RemainingAmount = Convert.ToDecimal(rank.CurrentValue) - usedAmount;
        //                    }
        //                }
        //                //else if (level != null && level.CurrentValue.ToLower() == "unlimited")
        //                else if (IsAlphabetic(level.CurrentValue))
        //                {
        //                    allowedAmount = 1000000;
        //                    remainingAmount = 1000000;

        //                }
        //            }


        //        }
        //    }

        //    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
        //    if (placement != null)
        //    {
        //        health.BranchId = placement.BranchId;
        //    }


        //    if (ModelState.IsValid)
        //    {
        //        var entity = new HealthRankAndValue
        //        {
        //            HealthRankAndValueId = health.HealthRankAndValueId,
        //            BranchId = health.BranchId,
        //            ClinicName = health.ClinicName,
        //            Date = health.Date,
        //            DateAmh = health.DateAmh,
        //            PaymentCoverage = health.PaymentCoverage,
        //            EmployeeId = health.EmployeeId,
        //            GlFiscalYearId = health.GlFiscalYearId,
        //            GLPeriodId = health.GLPeriodId,
        //            RemainingAmount = health.RemainingAmount,
        //            AllowedAmount = health.AllowedAmount,
        //            UsedAmount = health.UsedAmount,
        //            TotalPaid = health.TotalPaid,
        //            BenefitType = health.BenefitType,
        //            IsOtherBenifit = health.IsOtherBenifit,
        //            StaffContribution = health.StaffContribution,
        //            RecoredDate = health.RecoredDate,
        //            RecoredDateAmh = health.RecoredDateAmh,
        //            IsPartnerOrGov = health.IsPartnerOrGov,
        //            Status = health.Status,
        //            Remark = health.Remark,
        //            IsVoid = health.IsVoid,
        //            VoidBy = health.VoidBy,
        //            VoidTime = health.VoidTime,
        //            IsOnlineApproved = health.IsOnlineApproved,
        //            OnlineApprovedBy = health.OnlineApprovedBy,
        //            OnlineApprovedTime = health.OnlineApprovedTime,
        //            IsOnlineTransferred = health.IsOnlineTransferred,
        //            IsOnlineTransferredBy = health.IsOnlineTransferredBy,
        //            OnlineTransferredTime = health.OnlineTransferredTime,
        //            IsSendForApproval = health.IsSendForApproval,
        //            OrgBranchName = health.OrgBranchName,
        //            SendForApprovalBy = health.SendForApprovalBy,
        //            SendForApprovalTime = health.SendForApprovalTime,
        //            DateCreated = health.DateCreated,
        //            LastModified = health.LastModified,
        //            CreatedBy = health.CreatedBy,
        //            ModifiedBy = health.ModifiedBy,
        //        };
        //        db.HealthRankAndValues.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { health }.ToDataSourceResult(request, ModelState));
        //}
        #endregion
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValue_Create([DataSourceRequest] DataSourceRequest request, HealthRankAndValue healthValue)
        {
            ModelState.Clear();
            decimal usedAmount = 0;
            if (!healthValue.IsOtherBenifit)
            {
                healthValue.BenefitType = HealthBenefitType.NA;
            }

            if (healthValue.DateAmh == null)
            {
                ModelState.AddModelError("", "Treatment date is required");
                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

            }

            if (healthValue.RecoredDateAmh == null)
            {
                ModelState.AddModelError("", "Record date is required");
            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == healthValue.EmployeeId && x.isCurrent);
            if (placement == null)
            {

                ModelState.AddModelError("", "The  employee is not placed or Terminated");
                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

            }
            healthValue.BranchId = healthValue.BranchId == 0 ? placement.BranchId : healthValue.BranchId;

            if (!ModelState.IsValid)
            {
                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
            }
            
            if (healthValue.DateAmh != null)
            {
                var splitDate = healthValue.DateAmh.Split('/');
                healthValue.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }

            if (healthValue.RecoredDateAmh != null)
            {
                var splitDate = healthValue.RecoredDateAmh.Split('/');
                healthValue.RecoredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            switch (healthValue.BenefitType)
            {
                case HealthBenefitType.EyeGlass:
                    var eyGlassPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.EyeGlass);
                    if (eyGlassPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Eye-glass Benefit first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }

                    healthValue.AllowedAmount = eyGlassPayment.BenfitValue;
                    healthValue.UsedAmount = Math.Min(eyGlassPayment.BenfitValue, healthValue.TotalPaid);
                    break;

                case HealthBenefitType.Maternity:
                    var maternityPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.Maternity);
                    if (maternityPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Maternity Benefit setting first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }

                    var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == healthValue.EmployeeId 
                                            && x.RecoredDate < healthValue.RecoredDate 
                                            && x.GlFiscalYearId == healthValue.GlFiscalYearId 
                                            && x.BenefitType == HealthBenefitType.Maternity)
                                            .OrderByDescending(x => x.HealthRankAndValueId);
                    decimal remainingMaternityAmount = maternityPayment.BenfitValue 
                                                          - (healthPayments.Any()?healthPayments.Sum(x => x.UsedAmount):0);

                    if (remainingMaternityAmount > 0)
                    {
                        healthValue.UsedAmount = Math.Min(healthValue.TotalPaid, remainingMaternityAmount);
                        healthValue.RemainingAmount = remainingMaternityAmount - healthValue.UsedAmount;
                    }
                    else
                    {
                        healthValue.UsedAmount = 0;
                        healthValue.AllowedAmount = 0;
                    }
                    break;
                case HealthBenefitType.Accident:
                    var accidentPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.Accident);
                    if (accidentPayment == null)
                    {
                        ModelState.AddModelError("403", "Set accident Benefit setting first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }

                    var healthaccidentPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == healthValue.EmployeeId 
                                                        && x.RecoredDate < healthValue.RecoredDate 
                                                        && x.GlFiscalYearId == healthValue.GlFiscalYearId 
                                                        && x.BenefitType == HealthBenefitType.Accident).OrderByDescending(x => x.HealthRankAndValueId);

                    decimal remainingAccidentAmount = accidentPayment.BenfitValue - (healthaccidentPayments.Any()? healthaccidentPayments.Sum(x => x.UsedAmount):0);

                    if (remainingAccidentAmount > 0)
                    {
                        healthValue.UsedAmount = Math.Min(healthValue.TotalPaid, remainingAccidentAmount);
                        healthValue.RemainingAmount = remainingAccidentAmount - healthValue.UsedAmount;
                    }
                    else
                    {
                        healthValue.UsedAmount = 0;
                        healthValue.AllowedAmount = 0;
                    }
                    break;

                case HealthBenefitType.Family:
                    var familyPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.Family);
                    if (familyPayment == null)
                    {
                        ModelState.AddModelError("403", "Set family Benefit setting first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }

                    var healthFamilyPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == healthValue.EmployeeId 
                                                        && x.RecoredDate < healthValue.RecoredDate 
                                                        && x.GlFiscalYearId == healthValue.GlFiscalYearId 
                                                        && x.BenefitType == HealthBenefitType.Family).OrderByDescending(x => x.HealthRankAndValueId);

                    decimal remainingFamilyAmount = familyPayment.BenfitValue - (healthFamilyPayments.Any()? healthFamilyPayments.Sum(x => x.UsedAmount):0);

                    if (remainingFamilyAmount > 0)
                    {
                        healthValue.UsedAmount = Math.Min(healthValue.TotalPaid, remainingFamilyAmount);
                        healthValue.RemainingAmount = remainingFamilyAmount - healthValue.UsedAmount;


                    }
                    else
                    {
                        healthValue.UsedAmount = 0;
                        healthValue.AllowedAmount = 0;

                    }
                    break;

                default:
                    usedAmount = (healthValue.PaymentCoverage * healthValue.TotalPaid) / 100;
                    healthValue.StaffContribution = healthValue.TotalPaid - usedAmount;
                    var otherHealthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == healthValue.EmployeeId 
                                                        && x.RecoredDate < healthValue.RecoredDate 
                                                        && x.BenefitType < HealthBenefitType.NA 
                                                        && x.GlFiscalYearId == healthValue.GlFiscalYearId).ToList();
                    if (otherHealthPayments.Any())
                    {
                        var maxRecordDate = otherHealthPayments.Max(x => x.RecoredDate);
                        var remainingAmountCal = otherHealthPayments
                            .Where(x => x.RecoredDate == maxRecordDate)
                            .Select(x => x.RemainingAmount)
                            .FirstOrDefault();

                        var placement2 = db.Placements.FirstOrDefault(x => x.EmployeeId == healthValue.EmployeeId && x.isCurrent);

                        if (placement2 != null)
                        {
                            healthValue.BranchId = healthValue.BranchId == 0 ? placement2.BranchId: healthValue.BranchId;
                            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement2.RankId);
                            var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement2.ResponsibilityLevel);

                            if (rank != null && IsNumeric(rank.CurrentValue))
                            {
                                if (usedAmount > remainingAmountCal)
                                {
                                    healthValue.UsedAmount = remainingAmountCal;
                                    healthValue.AllowedAmount = remainingAmountCal;
                                    healthValue.RemainingAmount = 0;
                                }
                                else
                                {
                                   healthValue.UsedAmount = usedAmount;
                                   healthValue.AllowedAmount = remainingAmountCal;
                                    healthValue.RemainingAmount = remainingAmountCal - usedAmount;
                                }
                            }
                            else if (rank != null && IsAlphabetic(rank.CurrentValue))
                            {
                                healthValue.AllowedAmount = 1000000;
                                healthValue.RemainingAmount = 1000000;
                                healthValue.UsedAmount = healthValue.TotalPaid;
                            }
                            else if (level != null && IsAlphabetic(level.CurrentValue))
                            {
                                healthValue.AllowedAmount = 1000000;
                                healthValue.RemainingAmount = 1000000;
                                healthValue.UsedAmount = healthValue.TotalPaid;

                            }
                            else if (level != null && IsNumeric(level.CurrentValue))
                            {
                                if (usedAmount > remainingAmountCal)
                                {
                                    healthValue.UsedAmount = remainingAmountCal;
                                    healthValue.AllowedAmount = remainingAmountCal;
                                    healthValue.RemainingAmount = 0;
                                }
                                else
                                {
                                    healthValue.UsedAmount = usedAmount;
                                    healthValue.AllowedAmount = remainingAmountCal;
                                    healthValue.RemainingAmount = remainingAmountCal - usedAmount;
                                }
                            }
                        }
                    }
                    else
                    {
                        var placement1 = db.Placements.FirstOrDefault(x => x.EmployeeId == healthValue.EmployeeId && x.isCurrent);
                        if (placement1 != null)
                        {
                            healthValue.BranchId = healthValue.BranchId == 0 ? placement1.BranchId : healthValue.BranchId;

                            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement1.RankId);
                            var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement1.ResponsibilityLevel);

                            if (rank != null && IsNumeric( rank.CurrentValue))
                            {
                                if (usedAmount > Convert.ToDecimal(rank.CurrentValue))
                                {
                                    healthValue.UsedAmount = Convert.ToDecimal(rank.CurrentValue);
                                    healthValue.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
                                    healthValue.RemainingAmount = 0;
                                }
                                else
                                {
                                    healthValue.UsedAmount = usedAmount;
                                    healthValue.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
                                    healthValue.RemainingAmount = Convert.ToDecimal(rank.CurrentValue) - usedAmount;
                                }
                            }
                            else if (rank !=null && IsAlphabetic(rank.CurrentValue))
                            {
                                healthValue.AllowedAmount = 1000000;
                                healthValue.RemainingAmount = 1000000;
                                healthValue.UsedAmount = healthValue.TotalPaid;
                            }
                            else if (level !=null && IsNumeric(level.CurrentValue))
                            {
                                if (usedAmount > Convert.ToDecimal(level.CurrentValue))
                                {
                                    healthValue.UsedAmount = Convert.ToDecimal(level.CurrentValue);
                                    healthValue.AllowedAmount = Convert.ToDecimal(level.CurrentValue);
                                    healthValue.RemainingAmount = 0;
                                }
                                else
                                {
                                    healthValue.UsedAmount = usedAmount;
                                    healthValue.AllowedAmount = Convert.ToDecimal(level.CurrentValue);
                                    healthValue.RemainingAmount = Convert.ToDecimal(level.CurrentValue) - usedAmount;
                                }
                            }
                            else if (level !=null && IsAlphabetic(level.CurrentValue))
                            {
                                healthValue.AllowedAmount = 1000000;
                                healthValue.RemainingAmount = 1000000;
                                healthValue.UsedAmount = healthValue.TotalPaid;
                            }
                        }
                    }
                    break;
            }

           
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(healthValue);
                db.HealthRankAndValues.Add(entity);
                db.SaveChanges();

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Health payment ", "Success", User.Identity.Name, User.Identity.Name + " add new health payment ", Modules.Health);

                #endregion
            }


            return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthRankAndValue_Update([DataSourceRequest] DataSourceRequest request, HealthRankAndValue health)
        {
            ModelState.Clear();
           
            if (!health.IsOtherBenifit)
            {
                health.BenefitType = HealthBenefitType.NA;
            }

            if (health.DateAmh == null)
            {
                ModelState.AddModelError("", "Treatment date is required");
            }

            if (health.RecoredDateAmh == null)
            {
                ModelState.AddModelError("", "Record date is required");
            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
            if (placement == null)
            {
                ModelState.AddModelError("", "The  employee is not placed or Terminated");
            }
            health.BranchId = health.BranchId == 0 ? placement.BranchId : health.BranchId;

            if (!ModelState.IsValid)
            {
                return Json(new[] { health }.ToDataSourceResult(request, ModelState));
            }
            if (health.DateAmh != null)
            {
                var splitDate = health.DateAmh.Split('/');
                health.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (health.RecoredDateAmh != null)
            {
                var splitDate = health.RecoredDateAmh.Split('/');
                health.RecoredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            switch (health.BenefitType)
            {
                case HealthBenefitType.EyeGlass:
                    var eyGlassPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.EyeGlass);
                    if (eyGlassPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Eye-glass Benefit first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }
                    health.AllowedAmount = eyGlassPayment.BenfitValue;
                    health.UsedAmount = Math.Min(eyGlassPayment.BenfitValue, health.TotalPaid);
                    break;
                case HealthBenefitType.Maternity:
                    var maternityPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.Maternity);
                    if (maternityPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Maternity Benefit setting first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }

                    var healthPayments = db.HealthRankAndValues
                                           .Where(x => x.EmployeeId == health.EmployeeId 
                                               && x.RecoredDate < health.RecoredDate 
                                               && x.GlFiscalYearId == health.GlFiscalYearId 
                                               && x.BenefitType == HealthBenefitType.Maternity)
                                               .OrderByDescending(x => x.HealthRankAndValueId);
                    decimal remainingMaternityAmount = maternityPayment.BenfitValue - (healthPayments.Any()?healthPayments.Sum(x => x.UsedAmount):0);

                    if (remainingMaternityAmount > 0)
                    {
                        health.UsedAmount = Math.Min(health.TotalPaid, remainingMaternityAmount);
                        health.RemainingAmount = remainingMaternityAmount - health.UsedAmount;
                        health.AllowedAmount = remainingMaternityAmount;
                    }
                    else
                    {
                        health.UsedAmount = 0;
                        health.AllowedAmount = 0;
                        health.RemainingAmount = 0;
                    }
                    break;
                case HealthBenefitType.Accident:
                    var accidentPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.Accident);
                    if (accidentPayment == null)
                    {
                        ModelState.AddModelError("403", "Set accident Benefit setting first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }

                    var healthaccidentPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId 
                                                                                     && x.RecoredDate < health.RecoredDate 
                                                                                     && x.GlFiscalYearId == health.GlFiscalYearId
                                                                                     && x.BenefitType == HealthBenefitType.Accident)
                                                                                     .OrderByDescending(x => x.HealthRankAndValueId);
                    decimal remainingAccidentAmount = accidentPayment.BenfitValue - 
                                                                    (healthaccidentPayments.Any() ? healthaccidentPayments.Sum(x => x.UsedAmount) : 0);

                    if (remainingAccidentAmount > 0)
                    {
                        health.UsedAmount = Math.Min(health.TotalPaid, remainingAccidentAmount);
                        health.RemainingAmount = remainingAccidentAmount - health.UsedAmount;
                        health.AllowedAmount = remainingAccidentAmount;
                    }
                    else
                    {
                        health.UsedAmount = 0;
                        health.AllowedAmount = 0;
                        health.RemainingAmount = 0;
                    }
                    break;

                case HealthBenefitType.Family:
                    var familyPayment = db.HealthBenefitSettings.FirstOrDefault(x => x.BenefitType == HealthBenefitType.Family);
                    if (familyPayment == null)
                    {
                        ModelState.AddModelError("403", "Set family Benefit setting first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }

                    var healthFamilyPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId
                                                        && x.RecoredDate < health.RecoredDate
                                                        && x.GlFiscalYearId == health.GlFiscalYearId
                                                        && x.BenefitType == HealthBenefitType.Family).OrderByDescending(x => x.HealthRankAndValueId);

                    decimal remainingFamilyAmount = familyPayment.BenfitValue - (healthFamilyPayments.Any() ? healthFamilyPayments.Sum(x => x.UsedAmount) : 0);

                    if (remainingFamilyAmount > 0)
                    {
                        health.UsedAmount = Math.Min(health.TotalPaid, remainingFamilyAmount);
                        health.RemainingAmount = remainingFamilyAmount - health.UsedAmount;


                    }
                    else
                    {
                        health.UsedAmount = 0;
                        health.AllowedAmount = 0;

                    }
                    break;

                default:
                    var usedAmount = (health.PaymentCoverage * health.TotalPaid) / 100;
                    health.StaffContribution = health.TotalPaid - usedAmount;
                    var otherHealthPayments = db.HealthRankAndValues
                                                  .Where(x => x.EmployeeId == health.EmployeeId 
                                                              && x.RecoredDate < health.RecoredDate 
                                                              && x.BenefitType < HealthBenefitType.NA 
                                                              && x.GlFiscalYearId == health.GlFiscalYearId).ToList();

                    if (otherHealthPayments.Any())
                    {
                        var maxRecordDate = otherHealthPayments.Max(x => x.RecoredDate);
                        var remainingAmountCal = otherHealthPayments
                            .Where(x => x.RecoredDate == maxRecordDate)
                            .Select(x => x.RemainingAmount)
                            .FirstOrDefault();

                        var placement2 = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);

                        if (placement2 != null)
                        {
                            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement2.RankId);
                            var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement2.ResponsibilityLevel);

                            if (rank != null && IsNumeric(rank.CurrentValue))
                            {
                                if (usedAmount > remainingAmountCal)
                                {
                                    health.UsedAmount = remainingAmountCal;
                                    health.AllowedAmount = remainingAmountCal;
                                    health.RemainingAmount = 0;
                                }
                                else
                                {
                                    health.UsedAmount = usedAmount;
                                    health.AllowedAmount = remainingAmountCal;
                                    health.RemainingAmount = remainingAmountCal - usedAmount;
                                }
                            }
                            else if (rank != null && IsAlphabetic(rank.CurrentValue))
                            {
                                health.AllowedAmount = 1000000;
                                health.RemainingAmount = 1000000;
                                health.UsedAmount = health.TotalPaid;
                            }
                            else if (level != null && IsAlphabetic(level.CurrentValue))
                            {
                                health.AllowedAmount = 1000000;
                                health.RemainingAmount = 1000000;
                                health.UsedAmount = health.TotalPaid;

                            }
                            else if (level != null && IsNumeric(level.CurrentValue))
                            {
                                if (usedAmount > remainingAmountCal)
                                {
                                    health.UsedAmount = remainingAmountCal;
                                    health.AllowedAmount = remainingAmountCal;
                                    health.RemainingAmount = 0;
                                }
                                else
                                {
                                    health.UsedAmount = usedAmount;
                                    health.AllowedAmount = remainingAmountCal;
                                    health.RemainingAmount = remainingAmountCal - usedAmount;
                                }
                            }
                        }
                    }
                    else
                    {
                        var placement1 = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
                        if (placement1 != null)
                        {
                            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement1.RankId);
                            var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement1.ResponsibilityLevel);

                            if (rank !=null && IsNumeric(rank.CurrentValue))
                            {
                                if (usedAmount > Convert.ToDecimal(rank.CurrentValue))
                                {
                                    health.UsedAmount = Convert.ToDecimal(rank.CurrentValue);
                                    health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
                                    health.RemainingAmount = 0;
                                }
                                else
                                {
                                    health.UsedAmount = usedAmount;
                                    health.AllowedAmount = Convert.ToDecimal(rank.CurrentValue);
                                    health.RemainingAmount = Convert.ToDecimal(rank.CurrentValue) - usedAmount;
                                }
                            }
                            else if (rank !=null&& IsAlphabetic(rank.CurrentValue))
                            {
                                health.AllowedAmount = 1000000;
                                health.RemainingAmount = 1000000;
                                health.UsedAmount = health.TotalPaid;

                            }
                            else if (level !=null&& IsNumeric(level.CurrentValue))
                            {
                                if (usedAmount > Convert.ToDecimal(level.CurrentValue))
                                {
                                    health.UsedAmount = Convert.ToDecimal(level.CurrentValue);
                                    health.AllowedAmount = Convert.ToDecimal(level.CurrentValue);
                                    health.RemainingAmount = 0;
                                }
                                else
                                {
                                    health.UsedAmount = usedAmount;
                                    health.AllowedAmount = Convert.ToDecimal(level.CurrentValue);
                                    health.RemainingAmount = Convert.ToDecimal(level.CurrentValue) - usedAmount;
                                }
                            }
                            else if (IsAlphabetic(level.CurrentValue))
                            {
                                health.AllowedAmount = 1000000;
                                health.RemainingAmount = 1000000;
                                health.UsedAmount = health.TotalPaid;

                            }
                        }
                    }
                    break;
            }
           
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HealthRankAndValues.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Health payment is updatted By ", "Success", "Health payment is updatted By " + User.Identity.Name, User.Identity.Name , Modules.Health);
                #endregion
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

                var entity = GetMappedObject(health);
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
        private HealthRankAndValue GetMappedObject(HealthRankAndValue health)
        {
            var entity = new HealthRankAndValue
            {
                HealthRankAndValueId = health.HealthRankAndValueId,
                PaymentRequestId = health.PaymentRequestId,
                AllowedAmount = health.AllowedAmount,
                BranchId = health.BranchId,
                BusinessUnitId = health.BusinessUnitId,
                ClinicName = health.ClinicName,
                Date = health.Date,
                DateAmh = health.DateAmh,
                PaymentCoverage = health.PaymentCoverage,
                EmployeeId = health.EmployeeId,
                GlFiscalYearId = health.GlFiscalYearId,
                GLPeriodId = health.GLPeriodId,
                RemainingAmount = health.RemainingAmount,
                TotalPaid = health.TotalPaid,
                UsedAmount = health.UsedAmount,
                BenefitType = health.BenefitType,
                PaymentType = health.PaymentType,
                IsOtherBenifit = health.IsOtherBenifit,
                StaffContribution = health.StaffContribution,
                RecoredDate = health.RecoredDate,
                RecoredDateAmh = health.RecoredDateAmh,
                IsPartnerOrGov = health.IsPartnerOrGov,
                Status = health.Status,
                Remark = health.Remark,
                IsVoid = health.IsVoid,
                VoidBy = health.VoidBy,
                VoidTime = health.VoidTime,
                IsOnlineApproved = health.IsOnlineApproved,
                OnlineApprovedBy = health.OnlineApprovedBy,
                OnlineApprovedTime = health.OnlineApprovedTime,
                IsOnlineTransferred = health.IsOnlineTransferred,
                IsOnlineTransferredBy = health.IsOnlineTransferredBy,
                OnlineTransferredTime = health.OnlineTransferredTime,
                IsSendForApproval = health.IsSendForApproval,
                OrgBranchName = health.OrgBranchName,
                SendForApprovalBy = health.SendForApprovalBy,
                SendForApprovalTime = health.SendForApprovalTime,
                DateCreated = health.DateCreated,
                LastModified = health.LastModified,
                CreatedBy = health.CreatedBy,
                ModifiedBy = health.ModifiedBy,
            };
            return entity;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}