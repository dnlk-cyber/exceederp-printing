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
    public class FinanceSettingController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.FinanceSettings.Count();
            return View();
        }

        public ActionResult FinanceSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FinanceSetting> financesettings = _db.FinanceSettings;
            DataSourceResult result = financesettings.ToDataSourceResult(request, financeSetting => new {
                FinanceSettingId = financeSetting.FinanceSettingId,
                ProfitTaxRate = financeSetting.ProfitTaxRate,
                ProfitTaxCalculatedRate = financeSetting.ProfitTaxCalculatedRate,
                RetentionPercentage = financeSetting.RetentionPercentage,

                RetentionApAccount = financeSetting.RetentionApAccount,
                RetentionArAccount = financeSetting.RetentionArAccount,
                RetainedEarningAccount = financeSetting.RetainedEarningAccount,
                DiscountArAccount = financeSetting.DiscountArAccount,
                BankServiceChargeExpense = financeSetting.BankServiceChargeExpense,

                DateCreated = financeSetting.DateCreated,
                LastModified = financeSetting.LastModified,
                CreatedBy = financeSetting.CreatedBy,
                ModifiedBy = financeSetting.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceSettings_Create([DataSourceRequest]DataSourceRequest request, FinanceSetting financeSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceSetting
                {
                    ProfitTaxRate = financeSetting.ProfitTaxRate,
                    ProfitTaxCalculatedRate = financeSetting.ProfitTaxRate/100,
                    RetentionPercentage = financeSetting.RetentionPercentage,

                    RetentionApAccount = financeSetting.RetentionApAccount,
                    RetentionArAccount = financeSetting.RetentionArAccount,
                    RetainedEarningAccount = financeSetting.RetainedEarningAccount,
                    DiscountArAccount = financeSetting.DiscountArAccount,
                    BankServiceChargeExpense = financeSetting.BankServiceChargeExpense,

                    DateCreated = financeSetting.DateCreated,
                    LastModified = financeSetting.LastModified,
                    CreatedBy = financeSetting.CreatedBy,
                    ModifiedBy = financeSetting.ModifiedBy
                };

                _db.FinanceSettings.Add(entity);
                _db.SaveChanges();
                financeSetting.FinanceSettingId = entity.FinanceSettingId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance setting", "Success", User.Identity.Name, $"created setting {entity.FinanceSettingId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { financeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceSettings_Update([DataSourceRequest]DataSourceRequest request, FinanceSetting financeSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceSetting
                {
                    FinanceSettingId = financeSetting.FinanceSettingId,
                    ProfitTaxRate = financeSetting.ProfitTaxRate,
                    ProfitTaxCalculatedRate = financeSetting.ProfitTaxRate / 100,
                    RetentionPercentage = financeSetting.RetentionPercentage,

                    RetentionApAccount = financeSetting.RetentionApAccount,
                    RetentionArAccount = financeSetting.RetentionArAccount,
                    RetainedEarningAccount = financeSetting.RetainedEarningAccount,
                    DiscountArAccount = financeSetting.DiscountArAccount,
                    BankServiceChargeExpense = financeSetting.BankServiceChargeExpense,

                    DateCreated = financeSetting.DateCreated,
                    LastModified = financeSetting.LastModified,
                    CreatedBy = financeSetting.CreatedBy,
                    ModifiedBy = financeSetting.ModifiedBy
                };

                _db.FinanceSettings.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance setting", "Success", User.Identity.Name, $"updated setting {entity.FinanceSettingId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { financeSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceSettings_Destroy([DataSourceRequest]DataSourceRequest request, FinanceSetting financeSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceSetting
                {
                    FinanceSettingId = financeSetting.FinanceSettingId,
                    ProfitTaxRate = financeSetting.ProfitTaxRate,
                    ProfitTaxCalculatedRate = financeSetting.ProfitTaxCalculatedRate,
                    RetentionPercentage = financeSetting.RetentionPercentage,

                    RetentionApAccount = financeSetting.RetentionApAccount,
                    RetentionArAccount = financeSetting.RetentionArAccount,
                    RetainedEarningAccount = financeSetting.RetainedEarningAccount,
                    DiscountArAccount = financeSetting.DiscountArAccount,

                    DateCreated = financeSetting.DateCreated,
                    LastModified = financeSetting.LastModified,
                    CreatedBy = financeSetting.CreatedBy,
                    ModifiedBy = financeSetting.ModifiedBy
                };

                _db.FinanceSettings.Attach(entity);
                _db.FinanceSettings.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted finance setting information of setting " + financeSetting.FinanceSettingId, Modules.Finance);
                #endregion
            }

            return Json(new[] { financeSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
