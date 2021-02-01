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
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Budget;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
        [AuthorizeRoles( BudgetRoles.BudgetUser)] 
    public class BudgetTransferController : Controller
    {
        //
        // GET: /Budget/BudgetTransfer/


        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
             ViewData["MainCompanyStructures"] = db.GLChartOfAccountCostCenters.ToList();
            ViewData["GLPeriods"] = db.GLPeriods.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["BudgetCurrency"] = db.BudgetCurrencies.ToList();
            return View();
        }
        public ActionResult TransferCategory_Read([DataSourceRequest]DataSourceRequest request)
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
                GrandTotal = GetTransferTotalPlan(budgetCategory.PlanCategoryID)

            });

            return Json(result);
        }

      
        public ActionResult BudgetTransfer_Read(int RCategoryId,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetTransfer> BudgetTransfer = db.BudgetTransfers.Where(x => x.PlanCategoryId == RCategoryId);
            DataSourceResult result = BudgetTransfer.ToDataSourceResult(request, budgetTransfer => new BudgetTransfer
            {
                BudgetTransferID = budgetTransfer.BudgetTransferID,
                FromAccountCode = budgetTransfer.FromAccountCode,
                ToAccountCode = budgetTransfer.ToAccountCode,
                FromWorkUnit = budgetTransfer.FromWorkUnit,
                ToWorkUnit = budgetTransfer.ToWorkUnit,
                Month   = budgetTransfer.Month,
                AccountDesc = budgetTransfer.AccountDesc,
                BudgetGroup = budgetTransfer.BudgetGroup,
                BudgetYear = budgetTransfer.BudgetYear,
                Transferred = budgetTransfer.Transferred,
                Unit = budgetTransfer.Unit,
                PlanCategoryId =budgetTransfer.PlanCategoryId,
                CreatedBy = budgetTransfer.CreatedBy,
                DateCreated = budgetTransfer.DateCreated,
                LastModified = budgetTransfer.LastModified,
                ModifiedBy = budgetTransfer.ModifiedBy,
                OrgBranchName = budgetTransfer.OrgBranchName

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransfer_Create(int CCategoryId, [DataSourceRequest]DataSourceRequest request, BudgetTransfer BudgetTransfer)
        {
               if (ModelState.IsValid)
            {
               
           BudgetPlan FromPlan = db.BudgetPlans.Where(x => x.BudgetCategoryID == CCategoryId && x.AccountCode == BudgetTransfer.FromAccountCode && x.GLChartOfAccountCostCenterId.ToString() == BudgetTransfer.FromWorkUnit && x.BranchId.ToString() == BudgetTransfer.FromBranch).FirstOrDefault();
                 BudgetPlan ToPlan = db.BudgetPlans.Where(x => x.BudgetCategoryID == CCategoryId && x.AccountCode == BudgetTransfer.ToAccountCode && x.GLChartOfAccountCostCenterId.ToString() == BudgetTransfer.ToWorkUnit && x.BranchId.ToString() == BudgetTransfer.ToBranch).FirstOrDefault();

                 if ((FromPlan != null && ToPlan != null) && FromPlan.BudgetPlanMonthly.Sum(x=>x.CurrentBudget) > BudgetTransfer.Transferred)
                 {


                     ToPlan.CurrentYearlyBudget = ToPlan.CurrentYearlyBudget + BudgetTransfer.Transferred;
                     FromPlan.CurrentYearlyBudget = FromPlan.BudgetPlanMonthly.Sum(x => x.CurrentBudget) - BudgetTransfer.Transferred;


                     db.BudgetPlans.Attach(FromPlan);
                     db.Entry(FromPlan).State = System.Data.Entity.EntityState.Modified;
                     db.SaveChanges();

                     db.BudgetPlans.Attach(ToPlan);
                     db.Entry(ToPlan).State = System.Data.Entity.EntityState.Modified;
                     db.SaveChanges();


                     List<BudgetPlanMonthly> fromMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == FromPlan.BudgetPlanID).ToList();
                     decimal frommonthlybudget = FromPlan.TotalYearlyBudget / 12;
                     decimal transferedForEach = BudgetTransfer.Transferred / 12;
                     decimal FromCurentBudget = frommonthlybudget - transferedForEach;
                     foreach (var months in fromMonths)
                     {
                         months.CurrentBudget = FromCurentBudget;
                         months.TransferedBudget = transferedForEach;
                         db.BudgetPlanMonthlys.Attach(months);
                         db.Entry(months).State = System.Data.Entity.EntityState.Modified;
                         db.SaveChanges();

                     }

                     List<BudgetPlanMonthly> toMonths = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == ToPlan.BudgetPlanID).ToList();
                     decimal tomonthlybudget = ToPlan.TotalYearlyBudget / 12;
                     decimal ToCurentBudget = tomonthlybudget + transferedForEach;
                     foreach (var months in toMonths)
                     {
                         months.CurrentBudget = ToCurentBudget;
                         months.TransferedBudget = transferedForEach;

                         db.BudgetPlanMonthlys.Attach(months);
                         db.Entry(months).State = System.Data.Entity.EntityState.Modified;
                         db.SaveChanges();

                     }

                     var entity = new BudgetTransfer
                     {

                         BudgetTransferID = BudgetTransfer.BudgetTransferID,
                         FromAccountCode = BudgetTransfer.FromAccountCode,
                         ToAccountCode = BudgetTransfer.ToAccountCode,
                         FromBranch = BudgetTransfer.FromBranch,
                         ToBranch = BudgetTransfer.ToBranch,
                         AccountDesc = BudgetTransfer.AccountDesc,
                         FromWorkUnit = BudgetTransfer.FromWorkUnit,
                         PlanCategoryId = CCategoryId,
                         ToWorkUnit = BudgetTransfer.ToWorkUnit,
                         Month = BudgetTransfer.Month,
                         BudgetYear = BudgetTransfer.BudgetYear,
                         Transferred = BudgetTransfer.Transferred,
                         Unit = BudgetTransfer.Unit,

                         DateCreated = DateTime.Now,
                         CreatedBy = User.Identity.Name

                     };

                     db.BudgetTransfers.Add(entity);
                     db.SaveChanges();



                     BudgetTransfer.BudgetTransferID = entity.BudgetTransferID;
                 }
                 else
                 {
                     ModelState.AddModelError("InsufficentBalance", " Budget Plan Have Insufficient Balance for transfer! ");
                 }
            }


            return Json(new[] { BudgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransfer_Update([DataSourceRequest]DataSourceRequest request, BudgetTransfer BudgetTransfer)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetTransfer
                {
                    BudgetTransferID = BudgetTransfer.BudgetTransferID,
                    FromAccountCode = BudgetTransfer.FromAccountCode,
                    ToAccountCode = BudgetTransfer.ToAccountCode,
                    AccountDesc = BudgetTransfer.AccountDesc,
                    BudgetGroup = BudgetTransfer.BudgetGroup,
                    BudgetYear = BudgetTransfer.BudgetYear,
                    Transferred = BudgetTransfer.Transferred,
                    Unit = BudgetTransfer.Unit,

                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetTransfers.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
          


            return Json(new[] { BudgetTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransfer_Destroy([DataSourceRequest]DataSourceRequest request, BudgetTransfer BudgetTransfer)
        {
           
                var entity = new BudgetTransfer
                {
                    BudgetTransferID = BudgetTransfer.BudgetTransferID,
   
                };

                db.BudgetTransfers.Attach(entity);
                db.BudgetTransfers.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Transfer number " + BudgetTransfer.BudgetTransferID);

            return Json(new[] { BudgetTransfer }.ToDataSourceResult(request, ModelState));
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

   
	}
}