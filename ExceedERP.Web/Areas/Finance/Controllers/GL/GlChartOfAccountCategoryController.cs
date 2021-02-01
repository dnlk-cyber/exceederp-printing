using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlChartOfAccountCategoryController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
       

       [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin ,FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            return View();
        }

       [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin ,FinanceConstants.GeneralLedgerView)]
        public ActionResult GlChartOfAccountCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlChartOfAccountCategory> glchartofaccountcategories = _db.GlChartOfAccountCategories;
            DataSourceResult result = glchartofaccountcategories.ToDataSourceResult(request, glChartOfAccountCategory => new {
                GlChartOfAccountCategoryId = glChartOfAccountCategory.GlChartOfAccountCategoryId,
                AccountType = glChartOfAccountCategory.AccountType,
                AccountSubType = glChartOfAccountCategory.AccountSubType,
                DateCreated = glChartOfAccountCategory.DateCreated,
                LastModified = glChartOfAccountCategory.LastModified,
                CreatedBy = glChartOfAccountCategory.CreatedBy,
                ModifiedBy = glChartOfAccountCategory.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlChartOfAccountCategories_Create([DataSourceRequest]DataSourceRequest request, GlChartOfAccountCategory glChartOfAccountCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlChartOfAccountCategory
                {
                    AccountType = glChartOfAccountCategory.AccountType,
                    AccountSubType = glChartOfAccountCategory.AccountSubType,
                    DateCreated = glChartOfAccountCategory.DateCreated,
                    LastModified = glChartOfAccountCategory.LastModified,
                    CreatedBy = glChartOfAccountCategory.CreatedBy,
                    ModifiedBy = glChartOfAccountCategory.ModifiedBy
                };

                _db.GlChartOfAccountCategories.Add(entity);
                _db.SaveChanges();
                glChartOfAccountCategory.GlChartOfAccountCategoryId = entity.GlChartOfAccountCategoryId;

               
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account category", "Success", User.Identity.Name, $"created chart of account category under {entity.AccountType} sub type {entity.AccountSubType}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glChartOfAccountCategory }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlChartOfAccountCategories_Update([DataSourceRequest]DataSourceRequest request, GlChartOfAccountCategory glChartOfAccountCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlChartOfAccountCategory
                {
                    GlChartOfAccountCategoryId = glChartOfAccountCategory.GlChartOfAccountCategoryId,
                    AccountType = glChartOfAccountCategory.AccountType,
                    AccountSubType = glChartOfAccountCategory.AccountSubType,
                    DateCreated = glChartOfAccountCategory.DateCreated,
                    LastModified = glChartOfAccountCategory.LastModified,
                    CreatedBy = glChartOfAccountCategory.CreatedBy,
                    ModifiedBy = glChartOfAccountCategory.ModifiedBy
                };

                _db.GlChartOfAccountCategories.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account category", "Success", User.Identity.Name, $"updated chart of account category under {entity.AccountType} sub type {entity.AccountSubType}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glChartOfAccountCategory }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlChartOfAccountCategories_Destroy([DataSourceRequest]DataSourceRequest request, GlChartOfAccountCategory glChartOfAccountCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlChartOfAccountCategory
                {
                    GlChartOfAccountCategoryId = glChartOfAccountCategory.GlChartOfAccountCategoryId,
                    AccountType = glChartOfAccountCategory.AccountType,
                    AccountSubType = glChartOfAccountCategory.AccountSubType,
                    DateCreated = glChartOfAccountCategory.DateCreated,
                    LastModified = glChartOfAccountCategory.LastModified,
                    CreatedBy = glChartOfAccountCategory.CreatedBy,
                    ModifiedBy = glChartOfAccountCategory.ModifiedBy
                };

                _db.GlChartOfAccountCategories.Attach(entity);
                _db.GlChartOfAccountCategories.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { glChartOfAccountCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
