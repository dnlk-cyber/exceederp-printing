using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Budget.Models;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Common;
using System.Threading.Tasks;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Budget;
using System.Net.Http;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
     [AuthorizeRoles(BudgetRoles.BudgetUser)]
    public class BudgetPlanRegistrationController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public async Task<ActionResult> Index(string Reset)
        {


            if (!string.IsNullOrWhiteSpace(Reset) && Reset.IsCaseInsensitiveEqual("Reset"))
            {
                List<PlanCategory> PlanCategory = db.PlanCategorys.ToList();

                foreach (var plan in PlanCategory)
                {
                    plan.BudgetType = GetBudgetCategoryType(plan.Name);
                    plan.CurrentTotal = GetCurentTotalPlan(plan.PlanCategoryID);
                    plan.BudgetStatus = GetBudgetStatus(plan.PlanCategoryID);
                    plan.Progress = GetUsageTotalPlanPercentage(plan.PlanCategoryID);
                    plan.ActualConsumption = GetUsagePerPlan(plan.PlanCategoryID);

                    db.PlanCategorys.Attach(plan);
                    db.Entry(plan).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();


                
                }


            }
            ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["BudgetCurrency"] = db.BudgetCurrencies.ToList();

            return View();
        }
        public string GetBudgetCategoryType(string Name)
        {

            var BudgetCategory = db.BudgetCategorys.Where(x => x.Name == Name).FirstOrDefault();
            if (BudgetCategory != null)
            {
                return BudgetCategory.Description;
            }

            return string.Empty;
        }
        public ActionResult BudgetPlanRegistration_Read([DataSourceRequest]DataSourceRequest request)
        {

            var PlanCategory = db.PlanCategorys.Select(s => new
            {
                PlanCategoryID = s.PlanCategoryID,
                Name = s.Name,
                GlFiscalYearId = s.GlFiscalYearId,

                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                LastModified = s.LastModified,
                ModifiedBy = s.ModifiedBy,
                GrandTotal = s.GrandTotal,
                IsOnlineApproved = s.IsOnlineApproved,
                IsSendForApproval = s.IsSendForApproval,
                OnlineApprovedBy = s.OnlineApprovedBy,
                OnlineApprovedTime = s.OnlineApprovedTime,
                Progress = s.Progress,
                Remark = s.Remark,
                ActualConsumption = s.ActualConsumption,
                BudgetStatus = s.BudgetStatus,
                BudgetType = s.BudgetType,
                CurrentTotal = s.CurrentTotal,
                Status = s.Status



            });



            return Json(PlanCategory.ToDataSourceResult(request));

        }

        public ActionResult UsageCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<PlanCategory> PlanCategory = db.PlanCategorys.ToList();

            DataSourceResult result = PlanCategory.ToDataSourceResult(request, budgetCategory => new PlanCategory
            {
                PlanCategoryID = budgetCategory.PlanCategoryID,
                Name = budgetCategory.Name,
                GlFiscalYearId = budgetCategory.GlFiscalYearId,

                CreatedBy = budgetCategory.CreatedBy,
                DateCreated = budgetCategory.DateCreated,
                LastModified = budgetCategory.LastModified,
                ModifiedBy = budgetCategory.ModifiedBy,
                GrandTotal = GetUsageTotalPlan(budgetCategory.PlanCategoryID)

            });

            db.Dispose();
            return Json(result);
        }

        public decimal GetTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;

            try
            {
                TotalPerCategory = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategory).Sum(x => x.TotalYearlyBudget);
            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;

        }

        public decimal GetTransferTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                TotalPerCategory = db.BudgetTransfers.Where(x => x.PlanCategoryId == PlanCategory).Sum(x => x.Transferred);
            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;
        }
        public decimal GetUsageTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            decimal UsagePercentage = 0;

            List<BudgetUsage> Usages = new List<BudgetUsage>();

            try
            {



                var PlanCategorys = db.PlanCategorys.Where(x => x.PlanCategoryID == PlanCategory).FirstOrDefault();

                var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == PlanCategorys.Name).FirstOrDefault();

                var GrowthBudgetPlan =
                    db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategorys.PlanCategoryID)
                        .Sum(x => x.TotalYearlyBudget);
                if (GrowthBudgetPlan == 0)
                {
                    GrowthBudgetPlan = 1;
                }

                var JournalDetails = db.GLRecordJournalDetails.Where(
          x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
          .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0).Where(x => x.GLRecordJournal.FiscalYear == PlanCategorys.GlFiscalYear.Name);
                foreach (var JournalDetail in JournalDetails)
                {

                    Usages.Add(new BudgetUsage
                    {
                        BudgetUsageID = JournalDetail.GlRecordJournalDetailId,
                        AccountCode = JournalDetail.AccountCode,
                        AccountDesc = JournalDetail.AccountDesc,
                        BudgetMonth = JournalDetail.GLRecordJournal.PeriodName,
                        BudgetYear = JournalDetail.GLRecordJournal.FiscalYear,
                        MonthlyActuals = JournalDetail.Credit,
                        Unit = "Birr",
                        WorkUnit = "",
                        CreatedBy = JournalDetail.CreatedBy,
                        DateCreated = JournalDetail.DateCreated,
                        LastModified = JournalDetail.LastModified,
                        ModifiedBy = JournalDetail.ModifiedBy,
                        PlanCategoryId = PlanCategorys.PlanCategoryID,
                        OrgBranchName = JournalDetail.OrgBranchName

                    });

                }
                UsagePercentage = (Usages.Sum(x => x.MonthlyActuals));
                return UsagePercentage;
            }
            catch (Exception e)
            { return 0; }

        }



        public decimal GetUsagePerPlan(int PlanCategory)
        {
            decimal UsagePercentage = 0;

            List<BudgetUsage> Usages = new List<BudgetUsage>();

            try
            {



                var PlanCategorys = db.PlanCategorys.Where(x => x.PlanCategoryID == PlanCategory).FirstOrDefault();

                var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == PlanCategorys.Name).FirstOrDefault();
                var BudgetControl =
                  db.BudgetControls.Where(x => x.BudgetControlId == BudgetCategorys.BudgetControlId).FirstOrDefault();

                var GrowthBudgetPlan = decimal.Zero;
                var BudgetPlan = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategory).ToList();
                foreach (var Plan in BudgetPlan)
                {
                    var BudgetPlanMonthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == Plan.BudgetPlanID).ToList();
                    GrowthBudgetPlan = GrowthBudgetPlan + BudgetPlanMonthly.Sum(x => x.CurrentBudget);
                }
                if (GrowthBudgetPlan == 0)
                {
                    GrowthBudgetPlan = 1;
                }

                var JournalDetails = db.GLRecordJournalDetails.Where(
          x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
          .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0).Where(x => x.GLRecordJournal.FiscalYear == PlanCategorys.GlFiscalYear.Name);
                UsagePercentage = (JournalDetails.Sum(x => x.Credit));

                return UsagePercentage;
            }
            catch (Exception e)
            { return 0; }

        }


        public decimal GetCurentTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;

            try
            {
                var BudgetPlan = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategory).ToList();
                foreach (var Plan in BudgetPlan)
                {
                    var BudgetPlanMonthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == Plan.BudgetPlanID).ToList();
                    TotalPerCategory = TotalPerCategory + (decimal)BudgetPlanMonthly.Sum(x => x.CurrentBudget);
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;

        }
        public decimal GetUsageTotalPlanPercentage(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            decimal UsagePercentage = 0;

            List<BudgetUsage> Usages = new List<BudgetUsage>();

            try
            {



                var PlanCategorys = db.PlanCategorys.Where(x => x.PlanCategoryID == PlanCategory).FirstOrDefault();

                var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == PlanCategorys.Name).FirstOrDefault();
                var BudgetControl =
                  db.BudgetControls.Where(x => x.BudgetControlId == BudgetCategorys.BudgetControlId).FirstOrDefault();

                var GrowthBudgetPlan = decimal.Zero;
                var BudgetPlan = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategory).ToList();
                foreach (var Plan in BudgetPlan)
                {
                    var BudgetPlanMonthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == Plan.BudgetPlanID).ToList();
                    GrowthBudgetPlan = GrowthBudgetPlan + BudgetPlanMonthly.Sum(x => x.CurrentBudget);
                }
                if (GrowthBudgetPlan == 0)
                {
                    GrowthBudgetPlan = 1;
                }

                var JournalDetails = db.GLRecordJournalDetails.Where(
          x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
          .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0).Where(x => x.GLRecordJournal.FiscalYear == PlanCategorys.GlFiscalYear.Name);
                UsagePercentage = (JournalDetails.Sum(x => x.Credit));
                GrowthBudgetPlan = (GrowthBudgetPlan * (BudgetControl.Percentage) / 100) + BudgetControl.ReserveAmount + GrowthBudgetPlan;

                UsagePercentage = (UsagePercentage / GrowthBudgetPlan.DivideByZeroHelper()) * 100;
                return UsagePercentage;
            }
            catch (Exception e)
            { return 0; }

        }
        private string GetBudgetStatus(int CategoryId)
        {
            string stat = "Bad";
            decimal TotalPerCategory = 0;
            decimal UsagePercentage = 0;

            List<BudgetUsage> Usages = new List<BudgetUsage>();

            try
            {



                var PlanCategorys = db.PlanCategorys.Where(x => x.PlanCategoryID == CategoryId).FirstOrDefault();

                var BudgetCategorys = db.BudgetCategorys.Where(x => x.Name == PlanCategorys.Name).FirstOrDefault();

                var BudgetControl =
                    db.BudgetControls.Where(x => x.BudgetControlId == BudgetCategorys.BudgetControlId).FirstOrDefault();

                var GrowthBudgetPlan =
                    db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategorys.PlanCategoryID)
                        .Sum(x => x.TotalYearlyBudget);
                if (GrowthBudgetPlan == 0)
                {
                    GrowthBudgetPlan = 1;
                }

                var JournalDetails = db.GLRecordJournalDetails.Where(
          x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.FromAccountCode) >= 0)
          .Where(x => x.AccountCode.ToUpper().CompareTo(BudgetCategorys.ToAccountCode) <= 0).Where(x => x.GLRecordJournal.FiscalYear == PlanCategorys.GlFiscalYear.Name);

                UsagePercentage = (JournalDetails.Sum(x => x.Credit));
                GrowthBudgetPlan = (GrowthBudgetPlan * (BudgetControl.Percentage) / 100) + BudgetControl.ReserveAmount + GrowthBudgetPlan;
                if (UsagePercentage > GrowthBudgetPlan)
                {
                    return "Bad";
                }
                else
                {
                    return "Good";
                }

            }
            catch (Exception e)
            { return string.Empty; }



        }


        public decimal GetRequestTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                //   List<BudgetRequest> budgetRequest = db.BudgetRequests.Where(x => x.PlanCategoryId == PlanCategory).ToList();

            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;
        }

        public decimal GetCurentAllocationTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                TotalPerCategory = db.BudgetAllocations.Where(x => x.PlanCategoryId == PlanCategory).Sum(x => x.Amount);
            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;
        }
        public decimal GetCurentTransferTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {
                TotalPerCategory = db.BudgetTransfers.Where(x => x.PlanCategoryId == PlanCategory).Sum(x => x.Transferred);
            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;
        }

        public decimal GetCurentUsageTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;

            try
            {
                TotalPerCategory = db.BudgetUsages.Where(x => x.PlanCategoryId == PlanCategory).Sum(x => x.MonthlyActuals);

            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;
        }
        [AllowAnonymous]
        public decimal GetCurentRequestTotalPlan(int PlanCategory)
        {
            decimal TotalPerCategory = 0;
            try
            {


            }
            catch (Exception e)
            { return 0; }
            return TotalPerCategory;
        }

        public async Task<ActionResult> BudgetPlanRegistration_Create([DataSourceRequest]DataSourceRequest request, PlanCategory PlanCategory)
        {
            List<PlanCategory> PlanCategorys = db.PlanCategorys.Where(p => p.Name == PlanCategory.Name && p.GlFiscalYearId == PlanCategory.GlFiscalYearId).ToList();
            if (PlanCategorys.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", " Budget Group Already Exists !");
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new PlanCategory
                    {
                        PlanCategoryID = PlanCategory.PlanCategoryID,
                        Name = PlanCategory.Name,
                        GrandTotal = PlanCategory.GrandTotal,
                        GlFiscalYearId = PlanCategory.GlFiscalYearId,
                        DateCreated = DateTime.Now,

                        CreatedBy = User.Identity.Name,
                        Remark = PlanCategory.Remark

                    };

                    db.PlanCategorys.Add(entity);
                    db.SaveChanges();
                    PlanCategory.PlanCategoryID = entity.PlanCategoryID;
 
                }
            }


            return Json(new[] { PlanCategory }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(BudgetRoles.BudgetAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public  async Task<ActionResult> BudgetPlanRegistration_Update([DataSourceRequest]DataSourceRequest request, PlanCategory PlanCategory)
        {
            List<PlanCategory> PlanCategorys = db.PlanCategorys.Where(p => p.Name == PlanCategory.Name && p.GlFiscalYearId == PlanCategory.GlFiscalYearId && p.PlanCategoryID != PlanCategory.PlanCategoryID).ToList();
            if (PlanCategorys.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", " Budget Group Already Exists !");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new PlanCategory
                    {
                        PlanCategoryID = PlanCategory.PlanCategoryID,
                        Name = PlanCategory.Name,
                        GrandTotal = PlanCategory.GrandTotal,
                        GlFiscalYearId = PlanCategory.GlFiscalYearId,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.PlanCategorys.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    AccountCodeRangeViewModel model = new AccountCodeRangeViewModel();
                    var BudgetCategory = db.BudgetCategorys.Where(x => x.Name == PlanCategory.Name).FirstOrDefault();
                    if (BudgetCategory != null)
                    {
                        model.BudgetCategory = BudgetCategory;
                        model.PlanCategory = entity;
                        model.GLPeriods = db.GLPeriods.ToList();

                    }
                    await Task.Run(async () =>
                    {
                        await  CreateBudgetPlan(model);
                    });
                 

                }

            }

            return Json(new[] { PlanCategory }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(BudgetRoles.BudgetAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> BudgetPlanRegistration_Destroy([DataSourceRequest]DataSourceRequest request, PlanCategory PlanCategory)
        {
           
                var entity = new PlanCategory
               {
                   PlanCategoryID = PlanCategory.PlanCategoryID   };


                db.PlanCategorys.Attach(entity);
                db.PlanCategorys.Remove(entity);
              await  db.SaveChangesAsync();
  
            return Json(new[] { PlanCategory }.ToDataSourceResult(request));
        }

          [AuthorizeRoles(BudgetRoles.BudgetUser)]
          [AcceptVerbs(HttpVerbs.Get)]
        public async Task<ActionResult> ProcessBudgetPlan(string PlanId)
        {

            var PlanCategory = db.PlanCategorys.Where(x => x.PlanCategoryID.ToString() == PlanId).FirstOrDefault();
            var IsCreated = db.BudgetPlans.Where(x => x.BudgetCategoryID.ToString() == PlanId).Any();
            if (PlanCategory != null && !IsCreated)
            {
                
                AccountCodeRangeViewModel model = new AccountCodeRangeViewModel();
                var BudgetCategory = db.BudgetCategorys.Where(x => x.Name == PlanCategory.Name).FirstOrDefault();
                var GLFiscalYears = db.GLFiscalYears.Where(x => x.GlFiscalYearId == PlanCategory.GlFiscalYearId).FirstOrDefault();
                var GLPeriods = db.GLPeriods.Where(x => x.GlFiscalYearId == PlanCategory.GlFiscalYearId).ToList();
                if (BudgetCategory != null)
                {
                    model.BudgetCategory = BudgetCategory;
                    model.PlanCategory = PlanCategory;
                    model.GlFiscalYear = GLFiscalYears;
                    model.GLPeriods = GLPeriods;

                }
                await Task.Run(async () =>
                {
                  await  CreateBudgetPlan(model);
                });
            }

            return Json(true);
        }
        public ActionResult BudgetPlanRegistration_SendForOnlineApproval(int PlanId)
        {

            var planCategorys = db.PlanCategorys.Where(x => x.PlanCategoryID == PlanId).FirstOrDefault();
            if (planCategorys != null)
            {
                planCategorys.IsSendForApproval = true;
                db.PlanCategorys.Attach(planCategorys);
                db.Entry(planCategorys).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
      
            return RedirectToAction("Index");
        }

        [AuthorizeRoles(BudgetRoles.BudgetUser)]
        [AcceptVerbs(HttpVerbs.Get)]
                public ActionResult BudgetPlanRegistration_RemoveDependancy(string PlanId)
        {
         var BudgetPlans = db.BudgetPlans.Where(x => x.BudgetCategoryID.ToString() ==  PlanId).ToList();

                  foreach (var plan in BudgetPlans)
                  {
                      var budgetPlansMonthlys = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == plan.BudgetPlanID).ToList();

                      db.BudgetPlanMonthlys.RemoveRange(budgetPlansMonthlys);
                        db.SaveChanges();

                  }
                  db.BudgetPlans.RemoveRange(BudgetPlans);
                    db.SaveChanges();
                  UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Plan Category number " +  PlanId);
                  return RedirectToAction("Index", new  { Reset="Reset" });
                }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanCategorys_Approve([DataSourceRequest]DataSourceRequest request, BudgetPlanApproval BudgetPlanApproval, int Id)
        {
            try
            {
                if (BudgetPlanApproval.Status == ApprovalStatuses.Approve)
                {
                    var entity = db.PlanCategorys.Where(x => x.PlanCategoryID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;

                    db.PlanCategorys.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                if (BudgetPlanApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.PlanCategorys.Where(x => x.PlanCategoryID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = false;
                    entity.IsSendForApproval = false;
                    entity.IsOnlineTransferred = true;
                    db.PlanCategorys.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new BudgetPlanApproval()
                {

                    Status = BudgetPlanApproval.Status,
                    BudgetPlanId = Id,
                    Remark = BudgetPlanApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetPlanApprovals.Add(approval);
                db.SaveChanges();
                BudgetPlanApproval.BudgetPlanApprovalId = approval.BudgetPlanApprovalId;

            }
            catch (Exception e)
            {


            }
            return Json(new[] { BudgetPlanApproval }.ToDataSourceResult(request));
        }

        public ActionResult PlanCategoryApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {


            List<BudgetPlanApproval> usageApproval = db.BudgetPlanApprovals.Where(x => x.BudgetPlanId == Id).ToList();
            var Category = db.PlanCategorys.Where(x => x.PlanCategoryID == Id).FirstOrDefault();

            if (Category.IsSendForApproval == true && Category.IsOnlineApproved == false)
            {
                usageApproval.Add(new BudgetPlanApproval()
                {
                    BudgetPlanId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,

                });
            }



            return Json(usageApproval.ToDataSourceResult(request, approval => new BudgetPlanApproval()
            {
                BudgetPlanApprovalId = approval.BudgetPlanApprovalId,
                BudgetPlanId = approval.BudgetPlanId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }
        [AllowAnonymous]
        public List<BindingItem> GetMonths(int Id)
        {
            List<BindingItem> Months = new List<BindingItem>();
            var Category = db.PlanCategorys.Where(x => x.PlanCategoryID == Id).FirstOrDefault();
            if (Category != null)
            {
                Months = db.GLPeriods.Where(x => x.GlFiscalYear.GlFiscalYearId == Category.GlFiscalYearId).Select(s => new BindingItem
                {
                    Text = s.Name,
                    Value = s.GLPeriodId
                }).ToList();

            }
            return Months;

        }
        public async Task CreateBudgetPlan(AccountCodeRangeViewModel accountCodeRange)
        {

            try
            {

                if (accountCodeRange != null)
                {                    
                    BudgetGLAccountViewModel budgetGlAccount = new BudgetGLAccountViewModel();
                    OtherSpecifcationViewModel otherSpecifcation = new OtherSpecifcationViewModel();
                    var branch = db.Branch.ToList();
                    var budgetCurrency = db.BudgetCurrencies.FirstOrDefault();
                    otherSpecifcation.Branches = branch;
                    otherSpecifcation.BudgetCurrency = budgetCurrency;
                    BudgetGenerationHelper budgetDistribution = new BudgetGenerationHelper();
                    await  budgetDistribution.GenrateAccountCodeByRange(accountCodeRange, otherSpecifcation, budgetGlAccount);


                }
            }


            catch (Exception e)
            {

            }


        }



    }

}