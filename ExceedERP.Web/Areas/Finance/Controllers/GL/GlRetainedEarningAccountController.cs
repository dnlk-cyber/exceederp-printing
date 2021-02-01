using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class GlRetainedEarningAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        AccountHelper _accountHelper = new AccountHelper();
        public ActionResult Index()
        {
           
            return View();
        }

        

        public ActionResult GlRetainedEarningAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlRetainedEarningAccount> glRetainedEarningAccounts = _db.GlRetainedEarningAccounts;
            DataSourceResult result = glRetainedEarningAccounts.ToDataSourceResult(request, glRetainedEarningAccount => new {
                GlRetainedEarningAccountId = glRetainedEarningAccount.GlRetainedEarningAccountId,
                AccountCode = glRetainedEarningAccount.AccountCode,
                AccountDesc = glRetainedEarningAccount.AccountDesc,
                Type = glRetainedEarningAccount.Type,
                DateCreated = glRetainedEarningAccount.DateCreated,
                LastModified = glRetainedEarningAccount.LastModified,
                CreatedBy = glRetainedEarningAccount.CreatedBy,
                ModifiedBy = glRetainedEarningAccount.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRetainedEarningAccounts_Create([DataSourceRequest]DataSourceRequest request, GlRetainedEarningAccount glRetainedEarningAccount)
        {
            #region validation

            var exist = _db.GlRetainedEarningAccounts.Where(x => x.AccountCode == glRetainedEarningAccount.AccountCode).ToList();

            if(exist.Any())
                ModelState.AddModelError("exisit","The account code is already defined");

            var exist1 = _db.GlRetainedEarningAccounts.Where(x => x.Type == glRetainedEarningAccount.Type).ToList();

            if (exist1.Any())
                ModelState.AddModelError("exisit", "The account type is already defined");

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GlRetainedEarningAccount
                {
                    AccountCode = glRetainedEarningAccount.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(glRetainedEarningAccount.AccountCode),
                    Type = glRetainedEarningAccount.Type,
                    DateCreated = glRetainedEarningAccount.DateCreated,
                    LastModified = glRetainedEarningAccount.LastModified,
                    CreatedBy = glRetainedEarningAccount.CreatedBy,
                    ModifiedBy = glRetainedEarningAccount.ModifiedBy
                };

                _db.GlRetainedEarningAccounts.Add(entity);
                _db.SaveChanges();
                glRetainedEarningAccount.GlRetainedEarningAccountId = entity.GlRetainedEarningAccountId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL retained earning setup", "Success", User.Identity.Name, $"created retained earning setup {entity.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glRetainedEarningAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlRetainedEarningAccounts_Update([DataSourceRequest]DataSourceRequest request, GlRetainedEarningAccount glRetainedEarningAccount)
        {
            #region validation

            var exist = _db.GlRetainedEarningAccounts.Where(x=>x.GlRetainedEarningAccountId!=glRetainedEarningAccount.GlRetainedEarningAccountId).Where(x => x.AccountCode == glRetainedEarningAccount.AccountCode).ToList();

            if (exist.Any())
                ModelState.AddModelError("exisit", "The account code is already defined");

            var exist1 = _db.GlRetainedEarningAccounts.Where(x => x.GlRetainedEarningAccountId != glRetainedEarningAccount.GlRetainedEarningAccountId).Where(x => x.Type == glRetainedEarningAccount.Type).ToList();

            if (exist1.Any())
                ModelState.AddModelError("exisit", "The account type is already defined");

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new GlRetainedEarningAccount
                {
                    GlRetainedEarningAccountId = glRetainedEarningAccount.GlRetainedEarningAccountId,
                    AccountCode = glRetainedEarningAccount.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(glRetainedEarningAccount.AccountCode),
                    Type = glRetainedEarningAccount.Type,
                    DateCreated = glRetainedEarningAccount.DateCreated,
                    LastModified = glRetainedEarningAccount.LastModified,
                    CreatedBy = glRetainedEarningAccount.CreatedBy,
                    ModifiedBy = glRetainedEarningAccount.ModifiedBy
                };

                _db.GlRetainedEarningAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL retained earning setup", "Success", User.Identity.Name, $"updated retained earning setup {entity.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glRetainedEarningAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
