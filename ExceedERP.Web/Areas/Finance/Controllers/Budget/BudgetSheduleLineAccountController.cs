using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Budget
{
    public class BudgetSheduleLineAccountController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /Budget/AccountSheduleLine/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetLineSheduleAccounts_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            IQueryable<BudgetLineSheduleAccounts> BudgetLineSheduleAccounts = db.BudgetLineSheduleAccounts.Where(x => x.BudgetAccountSheduleLineId == Id);
            DataSourceResult result = BudgetLineSheduleAccounts.ToDataSourceResult(request, AccountShedule => new
            {
                BudgetLineSheduleAccountsId = AccountShedule.BudgetLineSheduleAccountsId,
                BudgetAccountSheduleLineId = AccountShedule.BudgetAccountSheduleLineId,
                FromAccountCode = AccountShedule.FromAccountCode,
                ToAccountCode = AccountShedule.ToAccountCode,
                CreatedBy = AccountShedule.CreatedBy,
                DateCreated = AccountShedule.DateCreated,
                LastModified = AccountShedule.LastModified,
                ModifiedBy = AccountShedule.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetLineSheduleAccounts_Create([DataSourceRequest]DataSourceRequest request, BudgetLineSheduleAccounts BudgetLineSheduleAccounts, int Id)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetLineSheduleAccounts
                {
                    BudgetLineSheduleAccountsId = BudgetLineSheduleAccounts.BudgetLineSheduleAccountsId,
                    BudgetAccountSheduleLineId = Id,
                    FromAccountCode = BudgetLineSheduleAccounts.FromAccountCode,
                    ToAccountCode = BudgetLineSheduleAccounts.ToAccountCode,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.BudgetLineSheduleAccounts.Add(entity);
                db.SaveChanges();



                BudgetLineSheduleAccounts.BudgetLineSheduleAccountsId = entity.BudgetLineSheduleAccountsId;

            }

            return Json(new[] { BudgetLineSheduleAccounts }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetLineSheduleAccounts_Update([DataSourceRequest]DataSourceRequest request, BudgetLineSheduleAccounts AccountShedule)
        {

            if (ModelState.IsValid)
            {
                var entity = new BudgetLineSheduleAccounts
                {
                    BudgetLineSheduleAccountsId = AccountShedule.BudgetLineSheduleAccountsId,
                    BudgetAccountSheduleLineId = AccountShedule.BudgetAccountSheduleLineId,
                    FromAccountCode = AccountShedule.FromAccountCode,
                    ToAccountCode = AccountShedule.ToAccountCode,
                    DateCreated = AccountShedule.DateCreated,
                    CreatedBy = AccountShedule.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetLineSheduleAccounts.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { AccountShedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetLineSheduleAccounts_Destroy([DataSourceRequest]DataSourceRequest request, BudgetLineSheduleAccounts BudgetLineSheduleAccounts)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetLineSheduleAccounts
                {
                    BudgetLineSheduleAccountsId = BudgetLineSheduleAccounts.BudgetLineSheduleAccountsId,


                };

                db.BudgetLineSheduleAccounts.Attach(entity);
                db.BudgetLineSheduleAccounts.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Line number " + BudgetLineSheduleAccounts.BudgetLineSheduleAccountsId);
            }

            return Json(new[] { BudgetLineSheduleAccounts }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }



    }
}