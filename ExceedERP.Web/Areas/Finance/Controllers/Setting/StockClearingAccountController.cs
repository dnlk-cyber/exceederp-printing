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
    public class StockClearingAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StockClearingAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<StockClearingAccount> stockclearingaccounts = _db.StockClearingAccounts;
            DataSourceResult result = stockclearingaccounts.ToDataSourceResult(request, stockClearingAccount => new {
                StockClearingAccountId = stockClearingAccount.StockClearingAccountId,
                Type = stockClearingAccount.Type,
                Description = stockClearingAccount.Description,
                AccountCode = stockClearingAccount.AccountCode,
                Enable = stockClearingAccount.Enable,
                OrgBranchName = stockClearingAccount.OrgBranchName,
                DateCreated = stockClearingAccount.DateCreated,
                LastModified = stockClearingAccount.LastModified,
                CreatedBy = stockClearingAccount.CreatedBy,
                ModifiedBy = stockClearingAccount.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockClearingAccounts_Create([DataSourceRequest]DataSourceRequest request, StockClearingAccount stockClearingAccount)
        {
            var exisit = _db.StockClearingAccounts.Where(x => x.Type == stockClearingAccount.Type).FirstOrDefault();
            if (exisit != null)
                ModelState.AddModelError("exisit", "The type already exisits");

            if (ModelState.IsValid)
            {
                var entity = new StockClearingAccount
                {
                    Type = stockClearingAccount.Type,
                    Description = stockClearingAccount.Description,
                    AccountCode = stockClearingAccount.AccountCode,
                    Enable = stockClearingAccount.Enable,
                    OrgBranchName = stockClearingAccount.OrgBranchName,
                    DateCreated = stockClearingAccount.DateCreated,
                    LastModified = stockClearingAccount.LastModified,
                    CreatedBy = stockClearingAccount.CreatedBy,
                    ModifiedBy = stockClearingAccount.ModifiedBy
                };

                _db.StockClearingAccounts.Add(entity);
                _db.SaveChanges();
                stockClearingAccount.StockClearingAccountId = entity.StockClearingAccountId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance stock clearing account ", "Success", User.Identity.Name, $"created clearing account {entity.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { stockClearingAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockClearingAccounts_Update([DataSourceRequest]DataSourceRequest request, StockClearingAccount stockClearingAccount)
        {
            var exisit = _db.StockClearingAccounts
                .Where(x=>x.StockClearingAccountId!=stockClearingAccount.StockClearingAccountId)
                .Where(x => x.Type == stockClearingAccount.Type).FirstOrDefault();

            if (exisit != null)
                ModelState.AddModelError("exisit", "The type already exisits");
            if (ModelState.IsValid)
            {
                var entity = new StockClearingAccount
                {
                    StockClearingAccountId = stockClearingAccount.StockClearingAccountId,
                    Type = stockClearingAccount.Type,
                    Description = stockClearingAccount.Description,
                    AccountCode = stockClearingAccount.AccountCode,
                    Enable = stockClearingAccount.Enable,
                    OrgBranchName = stockClearingAccount.OrgBranchName,
                    DateCreated = stockClearingAccount.DateCreated,
                    LastModified = stockClearingAccount.LastModified,
                    CreatedBy = stockClearingAccount.CreatedBy,
                    ModifiedBy = stockClearingAccount.ModifiedBy
                };

                _db.StockClearingAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance stock clearing account ", "Success", User.Identity.Name, $"updated clearing account {entity.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { stockClearingAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StockClearingAccounts_Destroy([DataSourceRequest]DataSourceRequest request, StockClearingAccount stockClearingAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new StockClearingAccount
                {
                    StockClearingAccountId = stockClearingAccount.StockClearingAccountId,
                    Type = stockClearingAccount.Type,
                    Description = stockClearingAccount.Description,
                    AccountCode = stockClearingAccount.AccountCode,
                    Enable = stockClearingAccount.Enable,
                    OrgBranchName = stockClearingAccount.OrgBranchName,
                    DateCreated = stockClearingAccount.DateCreated,
                    LastModified = stockClearingAccount.LastModified,
                    CreatedBy = stockClearingAccount.CreatedBy,
                    ModifiedBy = stockClearingAccount.ModifiedBy
                };

                _db.StockClearingAccounts.Attach(entity);
                _db.StockClearingAccounts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { stockClearingAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
