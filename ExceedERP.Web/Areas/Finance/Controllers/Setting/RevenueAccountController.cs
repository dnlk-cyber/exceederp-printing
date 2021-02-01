using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class RevenueAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RevenueAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RevenueAccount> revenueaccounts = _db.RevenueAccounts;
            DataSourceResult result = revenueaccounts.ToDataSourceResult(request, revenueAccount => new {
                RevenueAccountId = revenueAccount.RevenueAccountId,
                Name = revenueAccount.Name,
                ApplyToAr = revenueAccount.ApplyToAr,
                ApplyToAp = revenueAccount.ApplyToAp,
                ApplyToInventory = revenueAccount.ApplyToInventory,
                ArAccountCode = revenueAccount.ArAccountCode,
                ApAccountCode = revenueAccount.ApAccountCode,
                InventoryAccountCode = revenueAccount.InventoryAccountCode,
                Enable = revenueAccount.Enable,
                OrgBranchName = revenueAccount.OrgBranchName,
                DateCreated = revenueAccount.DateCreated,
                LastModified = revenueAccount.LastModified,
                CreatedBy = revenueAccount.CreatedBy,
                ModifiedBy = revenueAccount.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueAccounts_Create([DataSourceRequest]DataSourceRequest request, RevenueAccount revenueAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueAccount
                {
                    Name = revenueAccount.Name,
                    ApplyToAr = revenueAccount.ApplyToAr,
                    ApplyToAp = revenueAccount.ApplyToAp,
                    ApplyToInventory = revenueAccount.ApplyToInventory,
                    ArAccountCode = revenueAccount.ArAccountCode,
                    ApAccountCode = revenueAccount.ApAccountCode,
                    InventoryAccountCode = revenueAccount.InventoryAccountCode,
                    Enable = revenueAccount.Enable,
                    OrgBranchName = revenueAccount.OrgBranchName,
                    DateCreated = revenueAccount.DateCreated,
                    LastModified = revenueAccount.LastModified,
                    CreatedBy = revenueAccount.CreatedBy,
                    ModifiedBy = revenueAccount.ModifiedBy
                };

                _db.RevenueAccounts.Add(entity);
                _db.SaveChanges();
                revenueAccount.RevenueAccountId = entity.RevenueAccountId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue account", "Success", User.Identity.Name, $"created account {entity.RevenueAccountId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueAccounts_Update([DataSourceRequest]DataSourceRequest request, RevenueAccount revenueAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueAccount
                {
                    RevenueAccountId = revenueAccount.RevenueAccountId,
                    Name = revenueAccount.Name,
                    ApplyToAr = revenueAccount.ApplyToAr,
                    ApplyToAp = revenueAccount.ApplyToAp,
                    ApplyToInventory = revenueAccount.ApplyToInventory,
                    ArAccountCode = revenueAccount.ArAccountCode,
                    ApAccountCode = revenueAccount.ApAccountCode,
                    InventoryAccountCode = revenueAccount.InventoryAccountCode,
                    Enable = revenueAccount.Enable,
                    OrgBranchName = revenueAccount.OrgBranchName,
                    DateCreated = revenueAccount.DateCreated,
                    LastModified = revenueAccount.LastModified,
                    CreatedBy = revenueAccount.CreatedBy,
                    ModifiedBy = revenueAccount.ModifiedBy
                };

                _db.RevenueAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue account", "Success", User.Identity.Name, $"updated account {entity.RevenueAccountId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueAccounts_Destroy([DataSourceRequest]DataSourceRequest request, RevenueAccount revenueAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueAccount
                {
                    RevenueAccountId = revenueAccount.RevenueAccountId,
                    Name = revenueAccount.Name,
                    ApplyToAr = revenueAccount.ApplyToAr,
                    ApplyToAp = revenueAccount.ApplyToAp,
                    ApplyToInventory = revenueAccount.ApplyToInventory,
                    ArAccountCode = revenueAccount.ArAccountCode,
                    ApAccountCode = revenueAccount.ApAccountCode,
                    InventoryAccountCode = revenueAccount.InventoryAccountCode,
                    Enable = revenueAccount.Enable,
                    OrgBranchName = revenueAccount.OrgBranchName,
                    DateCreated = revenueAccount.DateCreated,
                    LastModified = revenueAccount.LastModified,
                    CreatedBy = revenueAccount.CreatedBy,
                    ModifiedBy = revenueAccount.ModifiedBy
                };

                _db.RevenueAccounts.Attach(entity);
                _db.RevenueAccounts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { revenueAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
