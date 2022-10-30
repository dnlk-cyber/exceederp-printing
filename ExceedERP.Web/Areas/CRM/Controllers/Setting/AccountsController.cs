using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

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
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class AccountsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var controlAccountList = db.ControlAccounts;
            ViewData["ControlAccountList"] = controlAccountList != null
           ? controlAccountList.ToList()
           : new List<ControlAccount>();
            ViewData["ListOfAccounts"] = db.Accounts.ToList();
            return View();
        }

        public ActionResult Accounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Accounts> accounts = db.Accounts;
            DataSourceResult result = accounts.ToDataSourceResult(request, account => new Accounts
            {
                AccountsID = account.AccountsID,
                ControlAccount = account.ControlAccount,
                Description = account.Description,
                IsActive = account.IsActive,
                Parent = account.Parent,
                CreatedBy = account.CreatedBy,
                DateCreated = account.DateCreated,
                ModifiedBy = account.ModifiedBy,
                LastModified = account.LastModified,
                Remark = account.Remark

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Accounts_Create([DataSourceRequest]DataSourceRequest request, Accounts account)
        {
            if (ModelState.IsValid)
            {
                var entity = new Accounts
                {
                    ControlAccount = account.ControlAccount,
                    Description = account.Description,
                    IsActive = account.IsActive,
                    Parent = account.Parent,
                    CreatedBy=User.Identity.Name,
                    DateCreated=DateTime.Now,
                    Remark = account.Remark
                };

                db.Accounts.Add(entity);
                db.SaveChanges();
                account.AccountsID = entity.AccountsID;
            }

            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Accounts_Update([DataSourceRequest]DataSourceRequest request, Accounts account)
        {
            if (ModelState.IsValid)
            {
                var entity = new Accounts
                {
                    AccountsID = account.AccountsID,
                    ControlAccount = account.ControlAccount,
                    Description = account.Description,
                    IsActive = account.IsActive,
                    Parent = account.Parent,
                    Remark = account.Remark
                };

                db.Accounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Accounts_Destroy([DataSourceRequest]DataSourceRequest request, Accounts account)
        {
            if (ModelState.IsValid)
            {
                var entity = new Accounts
                {
                    AccountsID = account.AccountsID,
                    ControlAccount = account.ControlAccount,
                    Description = account.Description,
                    IsActive = account.IsActive,
                    Parent = account.Parent,
                    Remark = account.Remark
                };

                db.Accounts.Attach(entity);
                db.Accounts.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Accounts number " + account.AccountsID);
			
            }

            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
