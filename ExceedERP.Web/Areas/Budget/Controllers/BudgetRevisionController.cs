using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
 
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
        [AuthorizeRoles( BudgetRoles.BudgetAdmin)] 
    public class BudgetRevisionController : Controller
    {
        //
        // GET: /Budget/BudgetRevision/



        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
              ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["BudgetCurrency"] = db.BudgetCurrencies.ToList();
            return View();
        }
        public ActionResult RevisionCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<PlanCategory> PlanCategory = db.PlanCategorys.Where(x => x.IsOnlineApproved == true).ToList();

            DataSourceResult result = PlanCategory.ToDataSourceResult(request, budgetCategory => new
            {
                PlanCategoryID = budgetCategory.PlanCategoryID,
                Name = budgetCategory.Name,
                GlFiscalYearId = budgetCategory.GlFiscalYearId,
                CreatedBy = budgetCategory.CreatedBy,
                DateCreated = budgetCategory.DateCreated,
                LastModified = budgetCategory.LastModified,
                ModifiedBy = budgetCategory.ModifiedBy,
                GrandTotal = GetCurentTotalPlan(budgetCategory.PlanCategoryID)

            });

            return Json(result);
        }
        public ActionResult BudgetPlan_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetPlan> BudgetPlan = db.BudgetPlans;
            DataSourceResult result = BudgetPlan.ToDataSourceResult(request, budgetPlan => new BudgetPlan
            {
                BudgetPlanID = budgetPlan.BudgetPlanID,
                AccountCode = budgetPlan.AccountCode,
                AccountDesc = budgetPlan.AccountDesc,
                BudgetYear = budgetPlan.BudgetYear,
                TotalYearlyBudget = budgetPlan.TotalYearlyBudget,
                Unit = budgetPlan.Unit,
                CreatedBy = budgetPlan.CreatedBy,
                DateCreated = budgetPlan.DateCreated,
                LastModified = budgetPlan.LastModified,
                ModifiedBy = budgetPlan.ModifiedBy,

                OrgBranchName = budgetPlan.OrgBranchName

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlan_Create([DataSourceRequest]DataSourceRequest request, BudgetPlan budgetPlan)
        {
            
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlan
                {
                    BudgetPlanID = budgetPlan.BudgetPlanID,
                    AccountCode = budgetPlan.AccountCode,
                    AccountDesc = budgetPlan.AccountDesc,
                    BudgetYear = budgetPlan.BudgetYear,
                    TotalYearlyBudget = budgetPlan.TotalYearlyBudget,
                    Unit = budgetPlan.Unit,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetPlans.Add(entity);
                db.SaveChanges();



                budgetPlan.BudgetPlanID = entity.BudgetPlanID;
            }


            return Json(new[] { budgetPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlan_Update([DataSourceRequest]DataSourceRequest request, BudgetPlan budgetPlan)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetPlan
                {
                    BudgetPlanID = budgetPlan.BudgetPlanID,
                    AccountCode = budgetPlan.AccountCode,
                    AccountDesc = budgetPlan.AccountDesc,
                    BudgetYear = budgetPlan.BudgetYear,
                    TotalYearlyBudget = budgetPlan.TotalYearlyBudget,
                    Unit = budgetPlan.Unit,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetPlans.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { budgetPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlan_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlan BudgetPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlan
                {
                    BudgetPlanID = BudgetPlan.BudgetPlanID,


                };

                db.BudgetPlans.Attach(entity);
                db.BudgetPlans.Remove(entity);
                db.SaveChanges();
            }
            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Revision number " + BudgetPlan.BudgetPlanID);
    

            return Json(new[] { BudgetPlan }.ToDataSourceResult(request, ModelState));
        }


        public double GetCurentTotalPlan(int PlanCategory)
        {
            double TotalPerCategory = 0;

            try
            {
                var BudgetPlan = db.BudgetPlans.Where(x => x.BudgetCategoryID == PlanCategory).ToList();
                foreach (var Plan in BudgetPlan)
                {
                    var BudgetPlanMonthly = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == Plan.BudgetPlanID).ToList();
                    TotalPerCategory = TotalPerCategory + (double)BudgetPlanMonthly.Sum(x => x.CurrentBudget);
                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return TotalPerCategory;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


	}
}