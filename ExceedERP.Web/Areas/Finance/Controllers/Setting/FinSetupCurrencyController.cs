using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    public class FinSetupCurrencyController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinSetupCurrencies_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FinSetupCurrency> finsetupcurrencies = _db.FinSetupCurrencies;
            DataSourceResult result = finsetupcurrencies.ToDataSourceResult(request, finSetupCurrency => new {
                FinSetupCurrencyID = finSetupCurrency.FinSetupCurrencyID,
                Code = finSetupCurrency.Code,
                Name = finSetupCurrency.Name,
                Territory = finSetupCurrency.Territory,
                Enable = finSetupCurrency.Enable,
                DateCreated = finSetupCurrency.DateCreated,
                LastModified = finSetupCurrency.LastModified,
                CreatedBy = finSetupCurrency.CreatedBy,
                ModifiedBy = finSetupCurrency.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinSetupCurrencies_Create([DataSourceRequest]DataSourceRequest request, FinSetupCurrency finSetupCurrency)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinSetupCurrency
                {
                    Code = finSetupCurrency.Code,
                    Name = finSetupCurrency.Name,
                    Territory = finSetupCurrency.Territory,
                    Enable = finSetupCurrency.Enable,
                    DateCreated = DateTime.Now,
                    LastModified = finSetupCurrency.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = finSetupCurrency.ModifiedBy
                };

                _db.FinSetupCurrencies.Add(entity);
                _db.SaveChanges();
                finSetupCurrency.FinSetupCurrencyID = entity.FinSetupCurrencyID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance currency setup", "Success", User.Identity.Name, $"created currency {entity.Code}", Modules.Finance);
                #endregion
            }

            return Json(new[] { finSetupCurrency }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinSetupCurrencies_Update([DataSourceRequest]DataSourceRequest request, FinSetupCurrency finSetupCurrency)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinSetupCurrency
                {
                    FinSetupCurrencyID = finSetupCurrency.FinSetupCurrencyID,
                    Code = finSetupCurrency.Code,
                    Name = finSetupCurrency.Name,
                    Territory = finSetupCurrency.Territory,
                    Enable = finSetupCurrency.Enable,
                    DateCreated = finSetupCurrency.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = finSetupCurrency.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.FinSetupCurrencies.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance currency setup", "Success", User.Identity.Name, $"updated currency {entity.Code}", Modules.Finance);
                #endregion
            }

            return Json(new[] { finSetupCurrency }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinSetupCurrencies_Destroy([DataSourceRequest]DataSourceRequest request, FinSetupCurrency finSetupCurrency)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinSetupCurrency
                {
                    FinSetupCurrencyID = finSetupCurrency.FinSetupCurrencyID,
                    Code = finSetupCurrency.Code,
                    Name = finSetupCurrency.Name,
                    Territory = finSetupCurrency.Territory,
                    Enable = finSetupCurrency.Enable,
                    DateCreated = finSetupCurrency.DateCreated,
                    LastModified = finSetupCurrency.LastModified,
                    CreatedBy = finSetupCurrency.CreatedBy,
                    ModifiedBy = finSetupCurrency.ModifiedBy
                };

                _db.FinSetupCurrencies.Attach(entity);
                _db.FinSetupCurrencies.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { finSetupCurrency }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
