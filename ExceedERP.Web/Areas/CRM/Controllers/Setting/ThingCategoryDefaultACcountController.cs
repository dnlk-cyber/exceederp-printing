using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class ThingCategoryDefaultACcountController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var uom = db.Lookups;
            ViewData["ControlAccountList"] = db.ControlAccounts.ToList();
            ViewData["ThingCategoryList"] = db.ProductCategories.ToList();
            ViewData["AccountList"] = db.Accounts.ToList();
            return View();
        }
        
        public ActionResult ThingCategoryDefaultAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ThingCategoryDefaultAccount> thingcategorydefaultaccounts = db.ThingCategoryDefaultAccounts;
            DataSourceResult result = thingcategorydefaultaccounts.ToDataSourceResult(request, thingCategoryDefaultAccount => new {
                ThingCategoryDefaultAccountsID = thingCategoryDefaultAccount.ThingCategoryDefaultAccountID,
                ControlAccountID = thingCategoryDefaultAccount.ControlAccountID,
                AccountsID = thingCategoryDefaultAccount.AccountID,
                ProductCategoryId=thingCategoryDefaultAccount.ProductCategoryId,
                CreatedBy = thingCategoryDefaultAccount.CreatedBy,
                DateCreated = thingCategoryDefaultAccount.DateCreated,
                LastModified = thingCategoryDefaultAccount.LastModified,
                ModifiedBy = thingCategoryDefaultAccount.ModifiedBy,
                OrgBranchName = thingCategoryDefaultAccount.OrgBranchName,
                Remark = thingCategoryDefaultAccount.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ThingCategoryDefaultAccounts_Create([DataSourceRequest]DataSourceRequest request, ThingCategoryDefaultAccount thingCategoryDefaultAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ThingCategoryDefaultAccount
                {
                    ProductCategoryId = thingCategoryDefaultAccount.ProductCategoryId,
                    ControlAccountID = thingCategoryDefaultAccount.ControlAccountID,
                    AccountID = thingCategoryDefaultAccount.AccountID,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = thingCategoryDefaultAccount.Remark
                };

                db.ThingCategoryDefaultAccounts.Add(entity);
                db.SaveChanges();
                thingCategoryDefaultAccount.ThingCategoryDefaultAccountID = entity.ThingCategoryDefaultAccountID;
            }

            return Json(new[] { thingCategoryDefaultAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ThingCategoryDefaultAccounts_Update([DataSourceRequest]DataSourceRequest request, ThingCategoryDefaultAccount thingCategoryDefaultAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ThingCategoryDefaultAccount
                {
                    ThingCategoryDefaultAccountID = thingCategoryDefaultAccount.ThingCategoryDefaultAccountID,
                    ProductCategoryId = thingCategoryDefaultAccount.ProductCategoryId,
                    ControlAccountID = thingCategoryDefaultAccount.ControlAccountID,
                    AccountID = thingCategoryDefaultAccount.AccountID,
                    Remark = thingCategoryDefaultAccount.Remark
                };

                db.ThingCategoryDefaultAccounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { thingCategoryDefaultAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ThingCategoryDefaultAccounts_Destroy([DataSourceRequest]DataSourceRequest request, ThingCategoryDefaultAccount thingCategoryDefaultAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ThingCategoryDefaultAccount
                {
                    ThingCategoryDefaultAccountID = thingCategoryDefaultAccount.ThingCategoryDefaultAccountID,
                    ProductCategoryId = thingCategoryDefaultAccount.ProductCategoryId,
                    ControlAccountID = thingCategoryDefaultAccount.ControlAccountID,
                    AccountID = thingCategoryDefaultAccount.AccountID,
                    Remark = thingCategoryDefaultAccount.Remark
                };

                db.ThingCategoryDefaultAccounts.Attach(entity);
                db.ThingCategoryDefaultAccounts.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Product Default Category number " + thingCategoryDefaultAccount.ThingCategoryDefaultAccountID);
      
            }

            return Json(new[] { thingCategoryDefaultAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
