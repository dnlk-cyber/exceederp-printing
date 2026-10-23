using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles( BudgetRoles.BudgetUser)] 
	public class BudgetAllocationController : Controller
	{
		
		
		private ExceedDbContext db = new ExceedDbContext();

        // GET: /Budget/BudgetAllocation/
        public ActionResult Index()
		{
            var costcenterSeg = db.GlSegmentSetups.Where(x => x.SegmentType == DynamicSegmentTypes.CostCenter).Select(x => x.GLSegmentSetupId).ToList();
            ViewData["MainCompanyStructures"] = db.GlChartOfAccountSegments.Where(x => costcenterSeg.Contains(x.GLSegmentSetupId)).ToList();

            //var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
            //ViewData["MainCompanyStructures"] = glChartOfAccountCostCenters.ToList();
			ViewData["GLPeriods"] = db.GLPeriods.ToList();
			ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
			ViewData["BudgetCurrency"] = db.FinSetupCurrencies.ToList();
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
               /*gets the budgetplan that has the same workunit/GlChartOfAccountCostCenterId as the selected workunit*/
               /*User have to select a workunit that exists in the BudgetPlan */
				BudgetPlan BPlan = db.BudgetPlans
                       .Where(x => x.PlanCategoryId == CCategoryId ).FirstOrDefault();
                //Added null reference checker
                //checks if the selected workunit is in the budgetplan and adds the allocated amount int the budgetallocation.amount to the budgetplan.currentYearlyBudget
                if (BPlan != null)
                {
                    //adds the allocated amount to the currentyearlybudget
                    BPlan.CurrentYearlyBudget = BPlan.CurrentYearlyBudget + BudgetAllocation.Amount;
                    db.BudgetPlans.Attach(BPlan);
                    db.Entry(BPlan).State = EntityState.Modified;
                    db.SaveChanges();


                    //if no month is provided gets all the data from BPlan divids it by 12 and divids the allocated amount by 12 then assignes each to the month.allocatedBudget and month.currentBudget
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
                        //adds the allocated amount to the month we selected BudgetPlanMonthly's currentBudget column and to AllocatedBudget Column
                        BudgetPlanMonthly toMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == BPlan.BudgetPlanID && x.GLPeriod.Name == BudgetAllocation.Month).FirstOrDefault();

                        decimal AllocatedForEach = BudgetAllocation.Amount;
                        decimal CurrentBudget = toMonths.CurrentBudget + BudgetAllocation.Amount;

                        toMonths.AllocatedBudget = BudgetAllocation.Amount;
                        toMonths.CurrentBudget = CurrentBudget;
                        db.BudgetPlanMonthlys.Attach(toMonths);
                        db.Entry(toMonths).State = EntityState.Modified;
                        db.SaveChanges();



                    }
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