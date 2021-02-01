using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class ConstructionCustomerAccountController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Accounts_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            IQueryable<ConstructionCustomerAccount> accounts = db.ConstructionCustomerAccounts.Where(wip => wip.ProjectInformationId == projectId);
            DataSourceResult result = accounts.ToDataSourceResult(request, account => new
            {
                ConstructionCustomerAccountId = account.ConstructionCustomerAccountId,
                ProjectInformationId = account.ProjectInformationId,
                AccountCode = account.AccountCode,
                Description = account.Description,
                AccountType = account.AccountType,

                OrgBranchName = account.OrgBranchName,
                DateCreated = account.DateCreated,
                LastModified = account.LastModified,
                CreatedBy = account.CreatedBy,
                ModifiedBy = account.ModifiedBy ?? " ",

            });

            return Json(result);
        }

        private bool AccountExists(int projectId, ConstructionCustomerAccount account)
        {
            bool accountExists = false;
            var acc = db.ConstructionCustomerAccounts.FirstOrDefault(ap => (ap.ProjectInformationId == projectId && ap.AccountType == account.AccountType) || ap.AccountCode == account.AccountCode);
            if (acc != null)
            {
                accountExists = true;
            }
            return accountExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Account_Create([DataSourceRequest]DataSourceRequest request, ConstructionCustomerAccount account, int projectId)
        {
            if (ModelState.IsValid)
            {
                if (AccountExists(projectId, account))
                {
                    ModelState.AddModelError("", "Account Type Exists");
                }
                else
                {
                    var entity = new ConstructionCustomerAccount
                    {
                        ConstructionCustomerAccountId = account.ConstructionCustomerAccountId,
                        ProjectInformationId = projectId,
                        AccountCode = account.AccountCode,
                        Description = account.Description,
                        AccountType = account.AccountType,

                        OrgBranchName = account.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = account.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = account.ModifiedBy,

                    };

                    db.ConstructionCustomerAccounts.Add(entity);
                    db.SaveChanges();

                }
            }

            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Account_Update([DataSourceRequest]DataSourceRequest request, ConstructionCustomerAccount account)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionCustomerAccount
                {
                    ConstructionCustomerAccountId = account.ConstructionCustomerAccountId,
                    ProjectInformationId = account.ProjectInformationId,
                    AccountCode = account.AccountCode,
                    Description = account.Description,
                    AccountType = account.AccountType,

                    OrgBranchName = account.OrgBranchName,
                    DateCreated = account.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = account.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                };

                db.ConstructionCustomerAccounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { account }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Account_Destroy([DataSourceRequest]DataSourceRequest request, ConstructionCustomerAccount account)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionCustomerAccount
                {
                    ConstructionCustomerAccountId = account.ConstructionCustomerAccountId,
                    ProjectInformationId = account.ProjectInformationId,
                    AccountCode = account.AccountCode,
                    Description = account.Description,
                    AccountType = account.AccountType,

                    OrgBranchName = account.OrgBranchName,
                    DateCreated = account.DateCreated,
                    LastModified = account.LastModified,
                    CreatedBy = account.CreatedBy,
                    ModifiedBy = account.ModifiedBy,

                };

                db.ConstructionCustomerAccounts.Attach(entity);
                db.ConstructionCustomerAccounts.Remove(entity);
                db.SaveChanges();
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