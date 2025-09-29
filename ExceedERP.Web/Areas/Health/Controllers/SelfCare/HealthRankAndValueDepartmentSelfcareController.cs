using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService,HrmConstants.HRDepartmentHead)]
    public class HealthRankAndValueDepartmentSelfcareController : BaseController
    {
        // GET: HRM/HealthRankAndValue
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            ViewData["Employee"] = db.Person_Employee.AsQueryable();
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name });
            ViewData["branches"] = db.Branch.Select(x => new { x.BranchId, Name = x.Name }).AsQueryable();
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name});
            ViewData["businessUnits"] = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name }).AsQueryable();
            ; ViewData["FiscalYear"] = db.GLFiscalYears.Select(p => new { p.GlFiscalYearId, p.Name }).AsQueryable();
            ViewData["Period"] = db.GLPeriods.Select(p=>new { p.GLPeriodId, p.Name}).AsQueryable();

        }
        public JsonResult SendForApproval(int id)
        {
            var healthRankAndValue = db.HealthRankAndValues.Find(id);
            if (healthRankAndValue == null)
            {
                return Json(new { Message = "Request not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
           
            if ( healthRankAndValue.IsDepartmentSendForApproval||healthRankAndValue.IsSendForApproval)
            {
                return Json(new { Message = "The  Request is already sent for approval.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }
            
            healthRankAndValue.IsSendForApproval = true;
            healthRankAndValue.IsDepartmentSendForApproval = true;
            healthRankAndValue.SendForApprovalBy = User.Identity.Name;
            healthRankAndValue.SendForApprovalTime = DateTime.Now;
            healthRankAndValue.DepartmentSendForApprovalBy = User.Identity.Name;
            healthRankAndValue.DepartmentSendForApprovalTime = DateTime.Now;
            db.Entry(healthRankAndValue).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { Message = "Request successfully sent for approval.", Status = "Ok" }, JsonRequestBehavior.AllowGet);
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
        private static bool IsNumeric(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return decimal.TryParse(input,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out _);
        }
        private static bool IsAlphabetic(string input)
        {
            //return Regex.IsMatch(input, @"^[a-zA-Z]+$");
            // Return true for null, empty, or whitespace strings
            if (string.IsNullOrWhiteSpace(input))
                return true;

            // Otherwise, check:
            // 1. Contains at least one letter (a-zA-Z), OR
            // 2. Contains no digits (\d) or decimal separators [.,]
            return Regex.IsMatch(input, @"[a-zA-Z]") ||
                   !Regex.IsMatch(input, @"[\d.,]");
        }
        private string GetEmployeeCode(int id)
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
        public string GetApproverName(string username)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(username);
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId ==employeeId );

            return employee!=null?employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish:username;
        }
        public ActionResult HealthRankAndValue_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeesId = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId);
            var healthRankAndValues = db.HealthRankAndValues.Where(x=> employeesId.Contains(x.EmployeeId)||x.EmployeeId==employeeId).AsQueryable();
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
                CurrentApprovalStep = health.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = health.DepartmentOnlineApprovedBy,
                DepartmentSendForApprovalBy = health.DepartmentSendForApprovalBy,
                DepartmentOnlineApprovedTime = health.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalTime = health.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = health.IsDepartmentSendForApproval,
                ForwardedFrom = health.ForwardedFrom,
                ForwardedTo = health.ForwardedTo,
                IsDepartmentOnlineApproved = health.IsDepartmentOnlineApproved,
                IsFinalApproval = health.IsFinalApproval,
                SendForApprovalTime = health.SendForApprovalTime,
                DateCreated = health.DateCreated,
                LastModified = health.LastModified,
                CreatedBy = health.CreatedBy,
                ModifiedBy = health.ModifiedBy

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
            if (string.IsNullOrEmpty(healthValue.DateAmh))
            {
                ModelState.AddModelError("", "Treatment Date is Required");
                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

            }
            if (string.IsNullOrEmpty(healthValue.RecoredDateAmh))
            {
                ModelState.AddModelError("", "Record Date is Required");
                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

            }
            if (!string.IsNullOrEmpty(healthValue.DateAmh))
            {
                healthValue.Date = HrHelper.ParseEthiopicDate(healthValue.DateAmh);
            }

            if (!string.IsNullOrEmpty(healthValue.RecoredDateAmh))
            {
                healthValue.RecoredDate = HrHelper.ParseEthiopicDate(healthValue.RecoredDateAmh);
            }
            if (healthValue.RecoredDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError(
                        "RecordDate",
                        "Record date cannot be in the future. Please select a date that is today or earlier."
                    ); return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

            }
            if (healthValue.Date.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError(
                       "TreatedDate",
                       "Treated date cannot be in the future. Please enter a valid date (today or earlier).");          
                       return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

            }
           
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == healthValue.EmployeeId && x.isCurrent);
            if (placement == null)
            {

                ModelState.AddModelError("", "No active placement found for this employee, The  Employee is either not Placed or Terminated");
                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

            }
            healthValue.BranchId = healthValue.BranchId == 0 ? placement.BranchId : healthValue.BranchId;

            var healthandPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == healthValue.EmployeeId
                                                                     && x.GlFiscalYearId == healthValue.GlFiscalYearId
                                                                     && x.BenefitType == healthValue.BenefitType)
                                                            .OrderByDescending(x => x.RecoredDate)
                                                            .ThenByDescending(x=>x.HealthRankAndValueId)
                                                            .ToList();
            var healthandPayment = db.HealthRankAndValues.Where(x => x.EmployeeId == healthValue.EmployeeId
                                                                     && x.GlFiscalYearId == healthValue.GlFiscalYearId
                                                                     && x.BenefitType == healthValue.BenefitType)
                                                            .OrderByDescending(x => x.RecoredDate)
                                                            .ThenByDescending(x=>x.HealthRankAndValueId)
                                                            .FirstOrDefault();
            var level1 = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel);
            if (level1 != null && IsNumeric(level1.CurrentValue))
            {

                if (healthValue.BenefitType != HealthBenefitType.Maternity
                    && healthValue.BenefitType != HealthBenefitType.EyeGlass)
                {
                    
                    var initialEyeGlassBenefit = Convert.ToDecimal(level1.CurrentValue);
                    usedAmount = healthandPayments.Any() ? healthandPayments.Sum(x => x.UsedAmount) : 0;
                    decimal remainingAmount = initialEyeGlassBenefit - usedAmount;

                    if (remainingAmount <= 0)
                    {
                        ModelState.AddModelError("", "The Remaining Balance is Zero(0). You don't have a balance.");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }
                }
            }


            if (!ModelState.IsValid)
            {
                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
            }
            
            switch (healthValue.BenefitType)
            {
                case HealthBenefitType.EyeGlass:
                    var eyGlassPayment = db.HealthBenefitSettings
                        .FirstOrDefault(x => x.BenefitType == HealthBenefitType.EyeGlass);
                    if (eyGlassPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Eye-glass Benefit first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }
                    var eyeGlassRequest = db.HealthRankAndValues
                                                    .Where(x=>x.BenefitType== HealthBenefitType.EyeGlass
                                                              &&x.EmployeeId==healthValue.EmployeeId)
                                                        .OrderByDescending(x=>x.HealthRankAndValueId)
                                                        .FirstOrDefault();
                    if(eyeGlassRequest != null)
                    {
                        var fiscalYear = db.GLFiscalYears.Find(eyeGlassRequest.GlFiscalYearId);
                        if (fiscalYear != null)
                        {
                            decimal yearsDifference = Math.Round((decimal)(DateTime.Now - fiscalYear.DateFrom).TotalDays / 365.25m, 2);
                            if (yearsDifference <= 2.0m)
                            {
                                ModelState.AddModelError("403", $"You can't request  Eye-glass Benefit with in less than  two(2) years you asked before {yearsDifference} ");
                                return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));

                            }
                        }
                    }

                       
                        healthValue.AllowedAmount = eyGlassPayment.BenfitValue ?? 0;
                        healthValue.UsedAmount = Math.Min(eyGlassPayment.BenfitValue ?? 0, healthValue.TotalPaid);
                        var healthPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == healthValue.EmployeeId
                                               && x.RecoredDate < healthValue.RecoredDate
                                               && x.GlFiscalYearId == healthValue.GlFiscalYearId
                                               && x.BenefitType == HealthBenefitType.EyeGlass)
                                               .OrderByDescending(x => x.HealthRankAndValueId);
                         var initialEyeBenefit = eyGlassPayment?.BenfitValue ?? 0m;
                         usedAmount = healthPayments.Any()?healthPayments.Sum(x => x.UsedAmount):0;
                        decimal eyeGlassRemainingAmount = initialEyeBenefit - usedAmount;
                        if (eyeGlassRemainingAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is zero. You don't have a balance.");
                            return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                        }
                        else
                        {
                            healthValue.UsedAmount = Math.Min(healthValue.TotalPaid, eyeGlassRemainingAmount);
                            healthValue.RemainingAmount = eyeGlassRemainingAmount - healthValue.UsedAmount;

                        }

                   
                   
                    break;

                case HealthBenefitType.Maternity:
                    var maternityPayment = db.HealthBenefitSettings
                        //.Where(x => x.ResponsibilityLevel == placement.ResponsibilityLevel)
                        .FirstOrDefault(x => x.BenefitType == HealthBenefitType.Maternity);
                    if (maternityPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Maternity Benefit setting first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }

                   
                    if (placement.ResponsibilityLevel==ResponsibilityLevel.STAFF)
                    {

                        // Ensure maternityPayment is already initialized before this block

                        var healthTotalPayments = db.HealthRankAndValues
                            .Where(x => x.EmployeeId == healthValue.EmployeeId
                                        && x.GlFiscalYearId == healthValue.GlFiscalYearId
                                        && x.BenefitType == HealthBenefitType.Maternity);

                        decimal usedAmounts = healthTotalPayments.Any() ? healthTotalPayments.Sum(x => x.UsedAmount) : 0;
                        decimal remainingMaternityAmount = maternityPayment.BenfitValue??0  - usedAmounts;

                        if (remainingMaternityAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is Zero(0). You don't have a balance.");
                            return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                        }

                        else
                        {
                            healthValue.AllowedAmount = maternityPayment.BenfitValue??0;
                            healthValue.UsedAmount = Math.Min(healthValue.TotalPaid, remainingMaternityAmount);
                            healthValue.RemainingAmount = Math.Max(0,remainingMaternityAmount - healthValue.UsedAmount);
                            healthValue.StaffContribution = (100 - healthValue.PaymentCoverage) * healthValue.TotalPaid;

                        }

                        
                    }
                   
                    break;
                case HealthBenefitType.Accident:
                    var accidentPayment = db.HealthBenefitSettings
                        //.Where(x => x.ResponsibilityLevel == placement.ResponsibilityLevel)
                        .FirstOrDefault(x => x.BenefitType == HealthBenefitType.Accident);

                    if (accidentPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Accident Benefit setting first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }

                    decimal? initialBenefit = accidentPayment?.BenfitValue;

                    // unlimited benefit if BenfitValue is null or 0
                    if (!initialBenefit.HasValue || initialBenefit.Value == 0)
                    {
                        // Treat as unlimited → just calculate coverage normally
                        healthValue.UsedAmount = (healthValue.TotalPaid * healthValue.PaymentCoverage) / 100;
                        healthValue.RemainingAmount = 0m; // or some flag to indicate unlimited
                    }
                    else
                    {
                        var healthaccidentPayments = db.HealthRankAndValues
                            .Where(x => x.EmployeeId == healthValue.EmployeeId
                                        && x.RecoredDate < healthValue.RecoredDate
                                        && x.GlFiscalYearId == healthValue.GlFiscalYearId
                                        && x.BenefitType == HealthBenefitType.Accident)
                            .OrderByDescending(x => x.HealthRankAndValueId);

                        usedAmount = healthaccidentPayments.Any()
                            ? healthaccidentPayments.Sum(x => x.UsedAmount)
                            : 0;

                        decimal remainingAccidentAmount = initialBenefit.Value - usedAmount;

                        if (remainingAccidentAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is Zero(0). You don't have a balance.");
                            return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                        }

                        healthValue.UsedAmount = Math.Min(healthValue.TotalPaid, remainingAccidentAmount);
                        healthValue.RemainingAmount = Math.Max(0, remainingAccidentAmount - healthValue.UsedAmount);
                    }

                    break;

                case HealthBenefitType.Family:
                    var familyPayment = db.HealthBenefitSettings
                        //.Where(x => x.ResponsibilityLevel == placement.ResponsibilityLevel)
                        .FirstOrDefault(x => x.BenefitType == HealthBenefitType.Family);
                    if (familyPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Family Benefit setting first");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }

                    decimal? initialFamilyBenefit = familyPayment?.BenfitValue;

                    // unlimited benefit if null or 0
                    if (!initialFamilyBenefit.HasValue || initialFamilyBenefit.Value == 0)
                    {
                        // Unlimited → allow full coverage
                        healthValue.UsedAmount = (healthValue.TotalPaid * healthValue.PaymentCoverage) / 100;
                        healthValue.AllowedAmount = decimal.MaxValue; // or null, depending on how your UI expects it
                        healthValue.RemainingAmount = decimal.MaxValue;
                    }
                    else
                    {
                        var healthFamilyPayments = db.HealthRankAndValues
                            .Where(x => x.EmployeeId == healthValue.EmployeeId
                                        && x.RecoredDate < healthValue.RecoredDate
                                        && x.GlFiscalYearId == healthValue.GlFiscalYearId
                                        && x.BenefitType == HealthBenefitType.Family)
                            .OrderByDescending(x => x.HealthRankAndValueId);

                        usedAmount = healthFamilyPayments.Any()
                            ? healthFamilyPayments.Sum(x => x.UsedAmount)
                            : 0;

                        decimal remainingFamilyAmount = initialFamilyBenefit.Value - usedAmount;

                        if (remainingFamilyAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is Zero(0). You don't have a balance.");
                            return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                        }

                        healthValue.AllowedAmount = remainingFamilyAmount;
                        healthValue.UsedAmount = Math.Min(healthValue.TotalPaid, remainingFamilyAmount);
                        healthValue.RemainingAmount = Math.Max(0, remainingFamilyAmount - healthValue.UsedAmount);
                    }
                    break;

                default:
                     usedAmount = db.HealthRankAndValues
                                .Where(x => x.EmployeeId == healthValue.EmployeeId
                                         && x.BenefitType == healthValue.BenefitType
                                         && x.GlFiscalYearId == healthValue.GlFiscalYearId)
                                .Sum(x => (decimal?)x.UsedAmount) ?? 0;

                    decimal currentPaid = (healthValue.PaymentCoverage * healthValue.TotalPaid) / 100;
                    healthValue.StaffContribution = healthValue.TotalPaid - usedAmount;
                    var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                    var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel);

                    // Prefer Rank if available, else Level
                   
                    if (level != null)
                    {
                        ProcessHealthBySetting(healthValue, level.CurrentValue, currentPaid, usedAmount, request);
                    }
                    else if (rank != null)
                    {
                        ProcessHealthBySetting(healthValue, rank.CurrentValue, currentPaid, usedAmount, request);
                    }
                    else
                    {
                        ModelState.AddModelError("", "No level/ rank setting found for this employee.");
                        return Json(new[] { healthValue }.ToDataSourceResult(request, ModelState));
                    }
                   
                    break;
            }

           
            if (ModelState.IsValid)
            {
                healthValue.DateCreated = DateTime.Now;
                healthValue.CreatedBy = User.Identity.Name;
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
            decimal usedAmount = 0;
            if (!health.IsOtherBenifit)
            {
                health.BenefitType = HealthBenefitType.NA;
            }

            if (string.IsNullOrEmpty(health.DateAmh))
            {
                ModelState.AddModelError("", "Treatment date is required");
                return Json(new[] { health }.ToDataSourceResult(request, ModelState));
            }

            if (string.IsNullOrEmpty(health.RecoredDateAmh))
            {
                ModelState.AddModelError("", "Record date is required");
                return Json(new[] { health }.ToDataSourceResult(request, ModelState));

            }
            if (!string.IsNullOrEmpty(health.DateAmh))
            {
                health.Date = HrHelper.ParseEthiopicDate(health.DateAmh);
            }
            if (!string.IsNullOrEmpty(health.RecoredDateAmh))
            {
                health.RecoredDate = HrHelper.ParseEthiopicDate(health.RecoredDateAmh);
            }
            if (health.Date.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Treated date can't be future date");
                return Json(new[] { health }.ToDataSourceResult(request, ModelState));

            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId && x.isCurrent);
            if (placement == null)
            {
                ModelState.AddModelError("", "The  employee is not placed or Terminated");
                return Json(new[] { health }.ToDataSourceResult(request, ModelState));

            }
            health.BranchId = health.BranchId == 0 ? placement.BranchId : health.BranchId;
            var healthandPayments = db.HealthRankAndValues
                                   .Where(x => x.EmployeeId == health.EmployeeId
                                               && x.GlFiscalYearId == health.GlFiscalYearId
                                               && x.BenefitType == health.BenefitType
                                               && x.HealthRankAndValueId != health.HealthRankAndValueId)
                                   .OrderByDescending(x => x.RecoredDate).ThenByDescending(x=>x.HealthRankAndValueId)
                                   .ToList();
            var healthandPayment = db.HealthRankAndValues
                                   .Where(x => x.EmployeeId == health.EmployeeId
                                               && x.GlFiscalYearId == health.GlFiscalYearId
                                               && x.BenefitType == health.BenefitType
                                               && x.HealthRankAndValueId != health.HealthRankAndValueId)
                                   .OrderByDescending(x => x.RecoredDate).ThenByDescending(x=>x.HealthRankAndValueId)
                                   .FirstOrDefault();
            var level1 = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel);
            if (level1 != null && IsNumeric(level1.CurrentValue))
            {
                if( health.BenefitType != HealthBenefitType.Maternity
                    && health.BenefitType !=HealthBenefitType.EyeGlass)

                {
                    decimal.TryParse(level1.CurrentValue, out decimal initialSetting);
                    decimal usedAmounts = healthandPayments?.Sum(x => x.UsedAmount) ?? 0m;

                    // 3. Calculate remaining amount with validation
                    decimal remainingAmount = Math.Max(0, initialSetting - usedAmounts);
                    if (remainingAmount <= 0)
                    {
                        ModelState.AddModelError("", "The Remaining Balance is zero. You don't have a balance.");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }

                }
                   
                   
            }
            if (!ModelState.IsValid)
            {
                return Json(new[] { health }.ToDataSourceResult(request, ModelState));
            }
           
            switch (health.BenefitType)
            {
                case HealthBenefitType.EyeGlass:
                    var eyGlassPayment = db.HealthBenefitSettings/*.Where(x => x.ResponsibilityLevel == placement.ResponsibilityLevel)*/
                                                                .FirstOrDefault(x => x.BenefitType == HealthBenefitType.EyeGlass);
                    if (eyGlassPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Eye-glass Benefit first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }
                    var eyeGlassRequest = db.HealthRankAndValues
                                                .Where(x => x.BenefitType == HealthBenefitType.EyeGlass
                                                           && x.EmployeeId == health.EmployeeId
                                                           && x.HealthRankAndValueId != health.HealthRankAndValueId)
                                                        .OrderByDescending(x => x.HealthRankAndValueId)
                                                        .FirstOrDefault();
                    if (eyeGlassRequest != null)
                    {
                        var fiscalYear = db.GLFiscalYears.Find(eyeGlassRequest.GlFiscalYearId);
                        if (fiscalYear != null)
                        {
                            decimal yearsDifference = Math.Round((decimal)(DateTime.Now - fiscalYear.DateFrom).TotalDays / 365.25m, 2);
                            if (yearsDifference <= 2.0m)
                            {
                                ModelState.AddModelError("403", $"You can't request  Eye-glass Benefit with in less than  two(2) years you asked before {yearsDifference} ");
                                return Json(new[] { health }.ToDataSourceResult(request, ModelState));

                            }
                           
                        }
                    }


                        if (healthandPayment != null && healthandPayment.RemainingAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is zero. You don't have a balance.");
                            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                        }
                        health.AllowedAmount = eyGlassPayment.BenfitValue ?? 0;
                        health.UsedAmount = Math.Min(eyGlassPayment.BenfitValue ?? 0, health.TotalPaid);
                        var healthEyeGlassPayments = db.HealthRankAndValues.Where(x => x.EmployeeId == health.EmployeeId
                                               && x.RecoredDate < health.RecoredDate
                                               && x.GlFiscalYearId == health.GlFiscalYearId
                                               && x.HealthRankAndValueId != health.HealthRankAndValueId
                                               && x.BenefitType == HealthBenefitType.EyeGlass)
                                               .OrderByDescending(x => x.HealthRankAndValueId);
                        var initialEyeGlassBenefit = eyGlassPayment?.BenfitValue ?? 0m;
                         decimal usedEyeGlassAmount = healthEyeGlassPayments.Any() ? healthEyeGlassPayments.Sum(x => x.UsedAmount) : 0;
                        decimal eyeGlassRemainingAmount =Math.Max(0, initialEyeGlassBenefit - usedEyeGlassAmount);
                        if (eyeGlassRemainingAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is zero. You don't have a balance.");
                            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                        }
                        else 
                        {
                            health.UsedAmount = Math.Min(health.TotalPaid, eyeGlassRemainingAmount);
                            health.RemainingAmount = Math.Max(0,eyeGlassRemainingAmount - health.UsedAmount);

                        }
                       

                  
                    break;
                case HealthBenefitType.Maternity:
                    var maternityPayment = db.HealthBenefitSettings/*.Where(x => x.ResponsibilityLevel == placement.ResponsibilityLevel)*/
                        .FirstOrDefault(x => x.BenefitType == HealthBenefitType.Maternity);
                    if (maternityPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Maternity Benefit setting first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }
                    //if (health.Organization == OrganizationName.NA)
                    //{
                    //    ModelState.AddModelError("403", "Select the Organization please");
                    //    return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    //}
                    var healthPayments = db.HealthRankAndValues
                                           .Where(x => x.EmployeeId == health.EmployeeId 
                                               && x.RecoredDate < health.RecoredDate 
                                               && x.GlFiscalYearId == health.GlFiscalYearId 
                                               && x.BenefitType == HealthBenefitType.Maternity)
                                               .OrderByDescending(x => x.HealthRankAndValueId);
                    //if(health.Organization == OrganizationName.Non_Governmental
                    //    &&placement.ResponsibilityLevel==ResponsibilityLevel.STAFF)
                    //{
                        // Ensure maternityPayment is already initialized before this block

                        var healthTotalPayments = db.HealthRankAndValues
                            .Where(x => x.EmployeeId == health.EmployeeId
                                        && x.GlFiscalYearId == health.GlFiscalYearId
                                        && x.HealthRankAndValueId != health.HealthRankAndValueId
                                        && x.BenefitType == HealthBenefitType.Maternity);

                        decimal usedMaternityAmount = healthTotalPayments.Any() ? healthTotalPayments.Sum(x => x.UsedAmount) : 0;
                        decimal remainingMaternityAmount = (maternityPayment.BenfitValue ?? 0) - usedMaternityAmount;

                        if (remainingMaternityAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is Zero(0). You don't have a balance.");
                            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                        }

                        else
                        {
                            health.UsedAmount = Math.Min(health.TotalPaid, remainingMaternityAmount);
                            health.RemainingAmount = Math.Max(0, remainingMaternityAmount - health.UsedAmount);
                            health.AllowedAmount = remainingMaternityAmount;
                            health.StaffContribution = (100 - health.PaymentCoverage) * health.AllowedAmount;

                        }
                        
                    //}
                    //else
                    //{
                    //    health.UsedAmount = health.TotalPaid;
                    //    health.RemainingAmount = 0;
                    //    health.AllowedAmount = health.TotalPaid;
                    //}
                  
                    break;
                case HealthBenefitType.Accident:
                    var accidentPayment = db.HealthBenefitSettings
                        //.Where(x => x.ResponsibilityLevel == placement.ResponsibilityLevel)
                        .FirstOrDefault(x => x.BenefitType == HealthBenefitType.Accident);

                    if (accidentPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Accident Benefit setting first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }

                    decimal? initialBenefit = accidentPayment?.BenfitValue;

                    // unlimited benefit if BenfitValue is null or 0
                    if (!initialBenefit.HasValue || initialBenefit.Value == 0)
                    {
                        // Treat as unlimited → just calculate coverage normally
                        health.UsedAmount = (health.TotalPaid * health.PaymentCoverage) / 100;
                        health.RemainingAmount = 0m; // or some flag to indicate unlimited
                    }
                    else
                    {
                        var healthaccidentPayments = db.HealthRankAndValues
                            .Where(x => x.EmployeeId == health.EmployeeId
                                        && x.HealthRankAndValueId != health.HealthRankAndValueId
                                        && x.GlFiscalYearId == health.GlFiscalYearId
                                        && x.BenefitType == HealthBenefitType.Accident)
                            .OrderByDescending(x => x.HealthRankAndValueId);

                        usedAmount = healthaccidentPayments.Any()
                            ? healthaccidentPayments.Sum(x => x.UsedAmount)
                            : 0;

                        decimal remainingAccidentAmount = initialBenefit.Value - usedAmount;

                        if (remainingAccidentAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is Zero(0). You don't have a balance.");
                            return Json(new[] { usedAmount }.ToDataSourceResult(request, ModelState));
                        }

                        health.UsedAmount = Math.Min(health.TotalPaid, remainingAccidentAmount);
                        health.RemainingAmount = Math.Max(0, remainingAccidentAmount - health.UsedAmount);
                    }

                    break;

                case HealthBenefitType.Family:
                    var familyPayment = db.HealthBenefitSettings
                        //.Where(x => x.ResponsibilityLevel == placement.ResponsibilityLevel)
                        .FirstOrDefault(x => x.BenefitType == HealthBenefitType.Family);
                    if (familyPayment == null)
                    {
                        ModelState.AddModelError("403", "Set Family Benefit setting first");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }

                    decimal? initialFamilyBenefit = familyPayment?.BenfitValue;

                    // unlimited benefit if null or 0
                    if (!initialFamilyBenefit.HasValue || initialFamilyBenefit.Value == 0)
                    {
                        // Unlimited → allow full coverage
                        health.UsedAmount = (health.TotalPaid * health.PaymentCoverage) / 100;
                        health.AllowedAmount = decimal.MaxValue; // or null, depending on how your UI expects it
                        health.RemainingAmount = decimal.MaxValue;
                    }
                    else
                    {
                        var healthFamilyPayments = db.HealthRankAndValues
                            .Where(x => x.EmployeeId == health.EmployeeId
                                        && x.HealthRankAndValueId != health.HealthRankAndValueId
                                        && x.GlFiscalYearId == health.GlFiscalYearId
                                        && x.BenefitType == HealthBenefitType.Family)
                            .OrderByDescending(x => x.HealthRankAndValueId);

                        usedAmount = healthFamilyPayments.Any()
                            ? healthFamilyPayments.Sum(x => x.UsedAmount)
                            : 0;

                        decimal remainingFamilyAmount = initialFamilyBenefit.Value - usedAmount;

                        if (remainingFamilyAmount <= 0)
                        {
                            ModelState.AddModelError("", "The Remaining Balance is Zero(0). You don't have a balance.");
                            return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                        }

                        health.AllowedAmount = remainingFamilyAmount;
                        health.UsedAmount = Math.Min(health.TotalPaid, remainingFamilyAmount);
                        health.RemainingAmount = Math.Max(0, remainingFamilyAmount - health.UsedAmount);
                    }
                    break;

                default:
                    usedAmount = db.HealthRankAndValues
                               .Where(x => x.EmployeeId == health.EmployeeId
                                        && x.HealthRankAndValueId != health.HealthRankAndValueId
                                        && x.BenefitType == health.BenefitType
                                        && x.GlFiscalYearId == health.GlFiscalYearId)
                               .Sum(x => (decimal?)x.UsedAmount) ?? 0;

                    decimal currentPaid = (health.PaymentCoverage * health.TotalPaid) / 100;
                    health.StaffContribution = health.TotalPaid - usedAmount;
                    var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placement.RankId);
                    var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placement.ResponsibilityLevel);

                    // Prefer Level if available, else rank

                    if (level != null)
                    {
                        ProcessHealthBySetting(health, level.CurrentValue, currentPaid, usedAmount, request);
                    }
                    else if (rank != null)
                    {
                        ProcessHealthBySetting(health, rank.CurrentValue, currentPaid, usedAmount, request);
                    }
                    else
                    {
                        ModelState.AddModelError("", "No level/ rank setting found for this employee.");
                        return Json(new[] { health }.ToDataSourceResult(request, ModelState));
                    }
                    
                    break;
            }
           
            if (ModelState.IsValid)
            {
                health.LastModified = DateTime.Now;
                health.ModifiedBy = User.Identity.Name;
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
                //Organization = health.Organization,
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
                CurrentApprovalStep = health.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = health.DepartmentOnlineApprovedBy,
                DepartmentSendForApprovalBy = health.DepartmentSendForApprovalBy,
                DepartmentOnlineApprovedTime = health.DepartmentOnlineApprovedTime,
                DepartmentSendForApprovalTime = health.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = health.IsDepartmentSendForApproval,
                ForwardedFrom = health.ForwardedFrom,
                ForwardedTo = health.ForwardedTo,
                IsDepartmentOnlineApproved = health.IsDepartmentOnlineApproved,
                IsFinalApproval = health.IsFinalApproval,
                SendForApprovalTime = health.SendForApprovalTime,
                DateCreated = health.DateCreated,
                LastModified = health.LastModified,
                CreatedBy = health.CreatedBy,
                ModifiedBy = health.ModifiedBy,
            };
            return entity;

        }
        private const decimal UnlimitedValue = 1000000m;


        private void ProcessHealthBySetting(HealthRankAndValue healthValue, string currentValue, decimal currentPaid, decimal usedAmount, DataSourceRequest request)
        {
            if (IsNumeric(currentValue))
            {
                decimal allowedAmount = Convert.ToDecimal(currentValue);
                if (allowedAmount <= 0)
                {
                    ModelState.AddModelError("", "The allowed amount is Zero (0). Please set a valid allowed amount.");
                    return;
                }

                // Remaining after considering history
                decimal remaining = Math.Max(0, allowedAmount - usedAmount);

                if (remaining <= 0)
                {
                    ModelState.AddModelError("", "The Remaining Balance is Zero (0). You don't have a balance.");
                    return;
                }

                decimal used = Math.Min(currentPaid, remaining);
                healthValue.AllowedAmount = remaining;
                healthValue.UsedAmount = used;
                healthValue.RemainingAmount = Math.Max(0, remaining - currentPaid);
                healthValue.StaffContribution = healthValue.TotalPaid - currentPaid;
            }
            else if (IsAlphabetic(currentValue))
            {
                healthValue.AllowedAmount = UnlimitedValue;
                healthValue.RemainingAmount = UnlimitedValue;
                healthValue.UsedAmount = healthValue.TotalPaid;
                healthValue.StaffContribution = 0;
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}