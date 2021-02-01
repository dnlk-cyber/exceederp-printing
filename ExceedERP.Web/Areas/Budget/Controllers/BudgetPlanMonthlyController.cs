using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Budget;



using System.Reflection;
 
using ExceedERP.Helpers.Common;  



namespace ExceedERP.Web.Areas.Budget.Controllers
{
     [AuthorizeRoles(BudgetRoles.BudgetUser)] 
    public class BudgetPlanMonthlyController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();

 
      
    
        public ActionResult BudgetPlanMonthly_Read([DataSourceRequest]DataSourceRequest request, int Rid)
        {
            IQueryable<BudgetPlanMonthly> BudgetPlanMonthly = db.BudgetPlanMonthlys.AsNoTracking().Where(x => x.BudgetDefineID == Rid);
            DataSourceResult result = BudgetPlanMonthly.ToDataSourceResult(request, budgetPlanMonthly => new 
            {
                BudgetPlanMonthlyID = budgetPlanMonthly.BudgetPlanMonthlyID,
                AccountCode = budgetPlanMonthly.AccountCode,
                AccountDesc = budgetPlanMonthly.AccountDesc,
                GLPeriodId = budgetPlanMonthly.GLPeriodId,
                MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                BudgetDefineID = budgetPlanMonthly.BudgetDefineID,
                RevisedBudget = budgetPlanMonthly.RevisedBudget,
                TransferedBudget = budgetPlanMonthly.TransferedBudget,
                AllocatedBudget = budgetPlanMonthly.AllocatedBudget,
                CurrentBudget = (double) budgetPlanMonthly.CurrentBudget,
                Unit = budgetPlanMonthly.Unit,
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
 
            if (db.BudgetPlanMonthlys.Any(p => p.AccountDesc == budgetPlanMonthly.AccountDesc))
            {
                ModelState.AddModelError("DuplicateEntrie", "Monthly Budget Plan Already exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetPlanMonthly
                    {
                        BudgetPlanMonthlyID = budgetPlanMonthly.BudgetPlanMonthlyID,
                        AccountCode = budgetPlanMonthly.AccountCode,
                        AccountDesc = budgetPlanMonthly.AccountDesc,
                        GLPeriodId = budgetPlanMonthly.GLPeriodId,
                        MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                        RevisedBudget = budgetPlanMonthly.RevisedBudget,
                        CurrentBudget = budgetPlanMonthly.MonthlyBudget,
                        TransferedBudget = budgetPlanMonthly.TransferedBudget,
                        AllocatedBudget = budgetPlanMonthly.AllocatedBudget,
                        BudgetDefineID = Cid,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetPlanMonthlys.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    budgetPlanMonthly.BudgetPlanMonthlyID = entity.BudgetPlanMonthlyID;
                }

            }
            return Json(new[] { budgetPlanMonthly }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanMonthly_Update([DataSourceRequest]DataSourceRequest request, BudgetPlanMonthly budgetPlanMonthly)
        {
         
            if (ModelState.IsValid)
            {
                decimal RevisedAmount = budgetPlanMonthly.MonthlyBudget + budgetPlanMonthly.RevisedBudget;
                var entity = new BudgetPlanMonthly
                {
                    BudgetPlanMonthlyID = budgetPlanMonthly.BudgetPlanMonthlyID,
                    AccountCode = budgetPlanMonthly.AccountCode,
                    AccountDesc = budgetPlanMonthly.AccountDesc,
                    GLPeriodId = budgetPlanMonthly.GLPeriodId,
                    RevisedBudget = budgetPlanMonthly.RevisedBudget,
                    TransferedBudget = budgetPlanMonthly.TransferedBudget,
                    AllocatedBudget = budgetPlanMonthly.AllocatedBudget,
                    MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                    CurrentBudget = RevisedAmount,
                   
                    BudgetDefineID = budgetPlanMonthly.BudgetDefineID,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetPlanMonthlys.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();




            }

            List<BudgetPlanMonthly> BudgetPlanMonthlys = db.BudgetPlanMonthlys.Where(x => x.BudgetDefineID == budgetPlanMonthly.BudgetDefineID).ToList();

            decimal Totalbudget = BudgetPlanMonthlys.Sum(x => x.MonthlyBudget);
            BudgetPlan BPlan = db.BudgetPlans.Where(x => x.BudgetPlanID == budgetPlanMonthly.BudgetDefineID).FirstOrDefault();
            BPlan.CurrentYearlyBudget = Totalbudget;

            db.BudgetPlans.Attach(BPlan);
            db.Entry(BPlan).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

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
                    AccountCode = budgetPlanMonthly.AccountCode,
                    AccountDesc = budgetPlanMonthly.AccountDesc,
                    GLPeriodId = budgetPlanMonthly.GLPeriodId,
                    MonthlyBudget = budgetPlanMonthly.MonthlyBudget,
                    BudgetDefineID = budgetPlanMonthly.BudgetDefineID,

                };

                db.BudgetPlanMonthlys.Attach(entity);
                db.BudgetPlanMonthlys.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted budget Plan Monthly number " + budgetPlanMonthly.BudgetPlanMonthlyID);
            }

            return Json(new[] { budgetPlanMonthly }.ToDataSourceResult(request, ModelState));
        }

   

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}





