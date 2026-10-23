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



namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)] 
    public class BudgetPlanReserveController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();

 
      
    
        public ActionResult BudgetPlanReserve_Read([DataSourceRequest]DataSourceRequest request, int Rid)
        {
            IQueryable<BudgetPlanReserve> BudgetPlanReserve = db.BudgetPlanReserves.AsNoTracking().Where(x => x.BudgetPlanID == Rid);
            DataSourceResult result = BudgetPlanReserve.ToDataSourceResult(request, budgetPlanReserve => new 
            {
                BudgetPlanReserveId = budgetPlanReserve.BudgetPlanReserveId,
                
                Type = budgetPlanReserve.Type,
                SourceId = budgetPlanReserve.SourceId,
                BudgetPlanID = budgetPlanReserve.BudgetPlanID,
                Amount = budgetPlanReserve.Amount,
                ReserveStatus = budgetPlanReserve.ReserveStatus,

                CreatedBy = budgetPlanReserve.CreatedBy,
                DateCreated = budgetPlanReserve.DateCreated,
                LastModified = budgetPlanReserve.LastModified,
                ModifiedBy = budgetPlanReserve.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanReserve_Create([DataSourceRequest]DataSourceRequest request, BudgetPlanReserve BudgetPlanReserve, int Cid)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanReserve
                {
                    BudgetPlanReserveId = BudgetPlanReserve.BudgetPlanReserveId,                 
                    Type = BudgetPlanReserve.Type,
                    SourceId = BudgetPlanReserve.SourceId,
                    Amount = BudgetPlanReserve.Amount,
                    ReserveStatus = BudgetPlanReserve.ReserveStatus,
                    BudgetPlanID = Cid,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetPlanReserves.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                BudgetPlanReserve.BudgetPlanReserveId = entity.BudgetPlanReserveId;

            }
            return Json(new[] { BudgetPlanReserve }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanReserve_Update([DataSourceRequest]DataSourceRequest request, BudgetPlanReserve BudgetPlanReserve)
        {
         
            if (ModelState.IsValid)
            {
                //decimal RevisedAmount = BudgetPlanReserve.SourceId + BudgetPlanReserve.Amount;
                var entity = new BudgetPlanReserve
                {
                    BudgetPlanReserveId = BudgetPlanReserve.BudgetPlanReserveId,
                    Type = BudgetPlanReserve.Type,
                    Amount = BudgetPlanReserve.Amount,
                    ReserveStatus = BudgetPlanReserve.ReserveStatus,
                    SourceId = BudgetPlanReserve.SourceId,
                    BudgetPlanID = BudgetPlanReserve.BudgetPlanID,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetPlanReserves.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }          

            return Json(new[] { BudgetPlanReserve }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetPlanReserve_Destroy([DataSourceRequest]DataSourceRequest request, BudgetPlanReserve BudgetPlanReserve)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetPlanReserve
                {
                    BudgetPlanReserveId = BudgetPlanReserve.BudgetPlanReserveId,
                    Type = BudgetPlanReserve.Type,
                    SourceId = BudgetPlanReserve.SourceId,
                    BudgetPlanID = BudgetPlanReserve.BudgetPlanID,

                };

                db.BudgetPlanReserves.Attach(entity);
                db.BudgetPlanReserves.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted budget Plan Monthly number " + BudgetPlanReserve.BudgetPlanReserveId);
            }

            return Json(new[] { BudgetPlanReserve }.ToDataSourceResult(request, ModelState));
        }

        


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}





