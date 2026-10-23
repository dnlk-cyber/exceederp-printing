using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;



namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class BudgetPlanMonthlySelfCareController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();

 
      
    
        public ActionResult BudgetPlanMonthly_Read([DataSourceRequest]DataSourceRequest request, int Rid)
        {
            IQueryable<BudgetPlanMonthly> BudgetPlanMonthly = db.BudgetPlanMonthlys.AsNoTracking().Where(x => x.BudgetDefineID == Rid);
            DataSourceResult result = BudgetPlanMonthly.ToDataSourceResult(request, budgetPlanMonthly => new 
            {
                BudgetPlanMonthlyID = budgetPlanMonthly.BudgetPlanMonthlyID,
                
                GLPeriodId = budgetPlanMonthly.GLPeriodId,
                MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                BudgetDefineID = budgetPlanMonthly.BudgetDefineID,
                RevisedBudget = budgetPlanMonthly.RevisedBudget,
                TransferedBudget = budgetPlanMonthly.TransferedBudget,
                TransferedOutBudget = budgetPlanMonthly.TransferedOutBudget,
                AllocatedBudget = budgetPlanMonthly.AllocatedBudget,
                CurrentBudget =  budgetPlanMonthly.CurrentBudget,
              
                CreatedBy = budgetPlanMonthly.CreatedBy,
                DateCreated = budgetPlanMonthly.DateCreated,
                LastModified = budgetPlanMonthly.LastModified,
                ModifiedBy = budgetPlanMonthly.ModifiedBy,
                OrgBranchName = budgetPlanMonthly.OrgBranchName

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanMonthly_Create([DataSourceRequest]DataSourceRequest request, BudgetPlanMonthly budgetPlanMonthly, int Cid)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanMonthly
                {
                    BudgetPlanMonthlyID = budgetPlanMonthly.BudgetPlanMonthlyID,                 
                    GLPeriodId = budgetPlanMonthly.GLPeriodId,
                    MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                    RevisedBudget = budgetPlanMonthly.RevisedBudget,
                    CurrentBudget = budgetPlanMonthly.MonthlyBudget,
                    TransferedBudget = budgetPlanMonthly.TransferedBudget,
                    TransferedOutBudget = budgetPlanMonthly.TransferedOutBudget,
                    AllocatedBudget = budgetPlanMonthly.AllocatedBudget,
                    BudgetDefineID = Cid,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetPlanMonthlys.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                budgetPlanMonthly.BudgetPlanMonthlyID = entity.BudgetPlanMonthlyID;
                
                UpdateYearlyTotalPlan(entity);
            }
            return Json(new[] { budgetPlanMonthly }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanMonthly_Update([DataSourceRequest]DataSourceRequest request, BudgetPlanMonthly budgetPlanMonthly)
        {
         
            if (ModelState.IsValid)
            {
                //decimal RevisedAmount = budgetPlanMonthly.MonthlyBudget + budgetPlanMonthly.RevisedBudget;
                var entity = new BudgetPlanMonthly
                {
                    BudgetPlanMonthlyID = budgetPlanMonthly.BudgetPlanMonthlyID,
                    GLPeriodId = budgetPlanMonthly.GLPeriodId,
                    RevisedBudget = budgetPlanMonthly.RevisedBudget,
                    TransferedBudget = budgetPlanMonthly.TransferedBudget,
                    TransferedOutBudget = budgetPlanMonthly.TransferedOutBudget,
                    AllocatedBudget = budgetPlanMonthly.MonthlyBudget,
                    MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                    CurrentBudget = budgetPlanMonthly.MonthlyBudget,


                    BudgetDefineID = budgetPlanMonthly.BudgetDefineID,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetPlanMonthlys.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                UpdateYearlyTotalPlan(entity);



            }

         
           
            return Json(new[] { budgetPlanMonthly }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanMonthly_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlanMonthly budgetPlanMonthly)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanMonthly
                {
                    BudgetPlanMonthlyID = budgetPlanMonthly.BudgetPlanMonthlyID,
                    GLPeriodId = budgetPlanMonthly.GLPeriodId,
                    MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                    BudgetDefineID = budgetPlanMonthly.BudgetDefineID,
                    RevisedBudget = budgetPlanMonthly.RevisedBudget,
                    TransferedBudget = budgetPlanMonthly.TransferedBudget,
                    AllocatedBudget = budgetPlanMonthly.AllocatedBudget,
                    CurrentBudget = budgetPlanMonthly.CurrentBudget,

                    TransferedOutBudget = budgetPlanMonthly.TransferedOutBudget,

                };

                db.BudgetPlanMonthlys.Attach(entity);
                db.BudgetPlanMonthlys.Remove(entity);
                db.SaveChanges();
                UpdateYearlyTotalPlan(budgetPlanMonthly);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted budget Plan Monthly number " + budgetPlanMonthly.BudgetPlanMonthlyID);
            }

            return Json(new[] { budgetPlanMonthly }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateYearlyTotalPlan(BudgetPlanMonthly monthPlan)
        {

            var budgetPlan = db.BudgetPlans.Where(x => x.BudgetPlanID == monthPlan.BudgetDefineID).FirstOrDefault();
            if (budgetPlan != null)
            {
                var list = db.BudgetPlanMonthlys
                 .Where(x => x.BudgetDefineID == monthPlan.BudgetDefineID).ToList();

                if (list.Any())
                {
                    budgetPlan.TotalYearlyBudget = list.Sum(x => x.MonthlyBudget);
                    budgetPlan.CurrentYearlyBudget = list.Sum(x => x.CurrentBudget);
                    db.BudgetPlans.Attach(budgetPlan);
                    db.Entry(budgetPlan).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}





