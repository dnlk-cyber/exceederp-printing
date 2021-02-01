using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.Budget.Controllers
{
      [AuthorizeRoles( BudgetRoles.BudgetUser)] 
    public class BudgetAllocationController : Controller
    {
        //
        // GET: /Budget/BudgetAllocation/



        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["BudgetCurrency"] = db.BudgetCurrencies.ToList();
            ViewData["Branches"] = db.Branch.ToList();
            return View();
        }
        public ActionResult AllocationCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<PlanCategory> PlanCategory = db.PlanCategorys.Where(x => x.IsOnlineApproved == true).ToList();

            DataSourceResult result = PlanCategory.ToDataSourceResult(request, budgetCategory => new PlanCategory
            {
                PlanCategoryID = budgetCategory.PlanCategoryID,
                Name = budgetCategory.Name,
                GlFiscalYearId = budgetCategory.GlFiscalYearId,

                CreatedBy = budgetCategory.CreatedBy,
                DateCreated = budgetCategory.DateCreated,
                LastModified = budgetCategory.LastModified,
                ModifiedBy = budgetCategory.ModifiedBy,
                GrandTotal = GetAllocationTotalPlan(budgetCategory.PlanCategoryID)

            });

            return Json(result);
        }
    
    
        public ActionResult BudgetAllocation_Read(int RCategoryId, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetAllocation> BudgetAllocation = db.BudgetAllocations.Where(x=>x.PlanCategoryId == RCategoryId);
            DataSourceResult result = BudgetAllocation.ToDataSourceResult(request, budgetAllocation => new BudgetAllocation
            {
                BudgetAllocationID = budgetAllocation.BudgetAllocationID,
                AccountCode = budgetAllocation.AccountCode,
                Branch = budgetAllocation.Branch,
                Amount = budgetAllocation.Amount,
                BudgetGroup = budgetAllocation.BudgetGroup,
                BudgetYear = budgetAllocation.BudgetYear,
                Month = budgetAllocation.Month,
                WorkUnit = budgetAllocation.WorkUnit,
                PlanCategoryId = budgetAllocation.PlanCategoryId,
                CreatedBy = budgetAllocation.CreatedBy,
                DateCreated = budgetAllocation.DateCreated,
                LastModified = budgetAllocation.LastModified,
                ModifiedBy = budgetAllocation.ModifiedBy,

                OrgBranchName = budgetAllocation.OrgBranchName

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAllocation_Create(int CCategoryId,[DataSourceRequest]DataSourceRequest request, BudgetAllocation BudgetAllocation)
        {
    
            if (ModelState.IsValid)
            {
                var entity = new BudgetAllocation
                {
                    BudgetAllocationID = BudgetAllocation.BudgetAllocationID,
                    AccountCode = BudgetAllocation.AccountCode,
                    Amount=BudgetAllocation.Amount,
                    BudgetGroup = BudgetAllocation.BudgetGroup,
                    BudgetYear = BudgetAllocation.BudgetYear,
                    WorkUnit = BudgetAllocation.WorkUnit,
                    PlanCategoryId = CCategoryId,
                    Branch = BudgetAllocation.Branch,
                    Month = BudgetAllocation.Month,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetAllocations.Add(entity);
                db.SaveChanges();

        

                     BudgetAllocation.BudgetAllocationID = entity.BudgetAllocationID;
                        BudgetPlan BPlan = db.BudgetPlans.Where(x => x.BudgetCategoryID == CCategoryId && x.GLChartOfAccountCostCenterId.ToString() == BudgetAllocation.WorkUnit).FirstOrDefault();

                BPlan.CurrentYearlyBudget = BPlan.CurrentYearlyBudget + BudgetAllocation.Amount;

                db.BudgetPlans.Attach(BPlan);
                db.Entry(BPlan).State = EntityState.Modified;
                db.SaveChanges();
                if (string.IsNullOrEmpty(BudgetAllocation.Month))
                {
                    List<BudgetPlanMonthly> toMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == BPlan.BudgetPlanID).ToList();
                    decimal tomonthlybudget = BPlan.TotalYearlyBudget / 12;
                    decimal AllocatedForEach = BudgetAllocation.Amount / 12;
                   
                    foreach (var months in toMonths)
                    {
                        months.AllocatedBudget = AllocatedForEach;
                        months.CurrentBudget = months.CurrentBudget + AllocatedForEach;
                        db.BudgetPlanMonthlys.Attach(months);
                        db.Entry(months).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                else 
                {
                    BudgetPlanMonthly toMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == BPlan.BudgetPlanID && x.GLPeriod.Name ==BudgetAllocation.Month).FirstOrDefault();
                    
                    decimal AllocatedForEach = BudgetAllocation.Amount;
                    decimal CurrentBudget = toMonths.CurrentBudget + BudgetAllocation.Amount;

                    toMonths.AllocatedBudget = BudgetAllocation.Amount;
                    toMonths.CurrentBudget = CurrentBudget;
                    db.BudgetPlanMonthlys.Attach(toMonths);
                    db.Entry(toMonths).State = EntityState.Modified;
                        db.SaveChanges();

                   
                
                }
            } 


            return Json(new[] { BudgetAllocation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAllocation_Update([DataSourceRequest]DataSourceRequest request, BudgetAllocation BudgetAllocation)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetAllocation
                {
                    BudgetAllocationID = BudgetAllocation.BudgetAllocationID,
                    AccountCode = BudgetAllocation.AccountCode,
                    Amount = BudgetAllocation.Amount,
                    Branch = BudgetAllocation.Branch,
                    BudgetGroup = BudgetAllocation.BudgetGroup,
                    WorkUnit = BudgetAllocation.WorkUnit,
                    BudgetYear = BudgetAllocation.BudgetYear,
               
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetAllocations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { BudgetAllocation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAllocation_Destroy([DataSourceRequest]DataSourceRequest request, BudgetAllocation BudgetAllocation)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetAllocation
                {
                    BudgetAllocationID = BudgetAllocation.BudgetAllocationID,


                };

                db.BudgetAllocations.Attach(entity);
                db.BudgetAllocations.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Allocation " + BudgetAllocation.BudgetAllocationID);
            }

            return Json(new[] { BudgetAllocation }.ToDataSourceResult(request, ModelState));
        }


        public decimal GetAllocationTotalPlan(int PlanCategory)
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


	}
}