using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseApprover, ProcurementRoles.ProcurementForeignPurchaseApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseApprover, FinanceConstants.AccountPayableView, ProcurementRoles.ProcurementForeignPurchaseUser)]
        public ActionResult ForeignPurchaseAccounts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForeignPurchaseAccount> foreignPurchaseAccounts = _db.ForeignPurchaseAccounts.Where(x=>x.ForeignPurchaseId==id);
            DataSourceResult result = foreignPurchaseAccounts.ToDataSourceResult(request, foreignPurchaseAccount => new {
                ForeignPurchaseId = foreignPurchaseAccount.ForeignPurchaseId,
                ForeignPurchaseAccountId = foreignPurchaseAccount.ForeignPurchaseAccountId,
                AccountCode = foreignPurchaseAccount.AccountCode,
                OrgBranchName = foreignPurchaseAccount.OrgBranchName,
                DateCreated = foreignPurchaseAccount.DateCreated,
                LastModified = foreignPurchaseAccount.LastModified,
                CreatedBy = foreignPurchaseAccount.CreatedBy,
                ModifiedBy = foreignPurchaseAccount.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseAccounts_Create(int id,[DataSourceRequest]DataSourceRequest request, ForeignPurchaseAccount foreignPurchaseAccount)
        {
            
            if (ModelState.IsValid)
            {
                   
                var entity = new ForeignPurchaseAccount
                {
                    ForeignPurchaseId = id,
                    AccountCode = foreignPurchaseAccount.AccountCode,
                    OrgBranchName = foreignPurchaseAccount.OrgBranchName,
                    DateCreated = foreignPurchaseAccount.DateCreated,
                    LastModified = foreignPurchaseAccount.LastModified,
                    CreatedBy = foreignPurchaseAccount.CreatedBy,
                    ModifiedBy = foreignPurchaseAccount.ModifiedBy
                };

                _db.ForeignPurchaseAccounts.Add(entity);
                _db.SaveChanges();
                foreignPurchaseAccount.ForeignPurchaseAccountId = entity.ForeignPurchaseAccountId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase account setup", "Success", User.Identity.Name, $"created account {entity.AccountCode}", Modules.Procurement);
                #endregion

            }

            return Json(new[] { foreignPurchaseAccount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseAccounts_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseAccount foreignPurchaseAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseAccount
                {
                    ForeignPurchaseId= foreignPurchaseAccount.ForeignPurchaseId,
                    ForeignPurchaseAccountId = foreignPurchaseAccount.ForeignPurchaseAccountId,
                    AccountCode = foreignPurchaseAccount.AccountCode,
                    OrgBranchName = foreignPurchaseAccount.OrgBranchName,
                    DateCreated = foreignPurchaseAccount.DateCreated,
                    LastModified = foreignPurchaseAccount.LastModified,
                    CreatedBy = foreignPurchaseAccount.CreatedBy,
                    ModifiedBy = foreignPurchaseAccount.ModifiedBy
                };

                _db.ForeignPurchaseAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase account setup", "Success", User.Identity.Name, $"updated account {entity.AccountCode}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseAccount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseAccounts_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseAccount foreignPurchaseAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseAccount
                {
                    ForeignPurchaseId = foreignPurchaseAccount.ForeignPurchaseId,
                    ForeignPurchaseAccountId = foreignPurchaseAccount.ForeignPurchaseAccountId,
                    AccountCode = foreignPurchaseAccount.AccountCode,
                    OrgBranchName = foreignPurchaseAccount.OrgBranchName,
                    DateCreated = foreignPurchaseAccount.DateCreated,
                    LastModified = foreignPurchaseAccount.LastModified,
                    CreatedBy = foreignPurchaseAccount.CreatedBy,
                    ModifiedBy = foreignPurchaseAccount.ModifiedBy
                };

                _db.ForeignPurchaseAccounts.Attach(entity);
                _db.ForeignPurchaseAccounts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
