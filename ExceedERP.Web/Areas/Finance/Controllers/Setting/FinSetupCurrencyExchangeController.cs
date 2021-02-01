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
    public class FinSetupCurrencyExchangeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
       

        public ActionResult Index()
        {
          
            var currencies = _db.FinSetupCurrencies.Where(c => c.Enable == true);

            ViewData["currencies"] = currencies.Select(c => new FinSetupCurrency
            {
                Name = c.Name,
                Code = c.Code
            }).Where(d => d.Enable == true);

            return View();
        }

        public ActionResult FinSetupCurrencyExchanges_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FinSetupCurrencyExchange> finsetupcurrencyexchanges = _db.FinSetupCurrencyExchanges;
            DataSourceResult result = finsetupcurrencyexchanges.ToDataSourceResult(request, finSetupCurrencyExchange => new {
                FinSetupCurrencyExchangeID = finSetupCurrencyExchange.FinSetupCurrencyExchangeID,
                FromCurrency = finSetupCurrencyExchange.FromCurrency,
                ToCurrency = finSetupCurrencyExchange.ToCurrency,
                DateFrom = finSetupCurrencyExchange.DateFrom,
                DateTo = finSetupCurrencyExchange.DateTo,
                RateOne = finSetupCurrencyExchange.RateOne,
                RateTwo = finSetupCurrencyExchange.RateTwo,
                DateCreated = finSetupCurrencyExchange.DateCreated,
                LastModified = finSetupCurrencyExchange.LastModified,
                CreatedBy = finSetupCurrencyExchange.CreatedBy,
                ModifiedBy = finSetupCurrencyExchange.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinSetupCurrencyExchanges_Create([DataSourceRequest]DataSourceRequest request, FinSetupCurrencyExchange finSetupCurrencyExchange)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinSetupCurrencyExchange
                {
                    FromCurrency = finSetupCurrencyExchange.FromCurrency,
                    ToCurrency = finSetupCurrencyExchange.ToCurrency,
                    DateFrom = finSetupCurrencyExchange.DateFrom,
                    DateTo = finSetupCurrencyExchange.DateTo,
                    RateOne = finSetupCurrencyExchange.RateOne,
                    RateTwo = finSetupCurrencyExchange.RateTwo,
                    DateCreated = DateTime.Now,
                    LastModified = finSetupCurrencyExchange.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = finSetupCurrencyExchange.ModifiedBy
                };

                _db.FinSetupCurrencyExchanges.Add(entity);
                _db.SaveChanges();
                finSetupCurrencyExchange.FinSetupCurrencyExchangeID = entity.FinSetupCurrencyExchangeID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance currency exchange rate", "Success", User.Identity.Name, $"created currency exchange rate {entity.FinSetupCurrencyExchangeID}", Modules.Finance);
                #endregion
            }

            return Json(new[] { finSetupCurrencyExchange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinSetupCurrencyExchanges_Update([DataSourceRequest]DataSourceRequest request, FinSetupCurrencyExchange finSetupCurrencyExchange)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinSetupCurrencyExchange
                {
                    FinSetupCurrencyExchangeID = finSetupCurrencyExchange.FinSetupCurrencyExchangeID,
                    FromCurrency = finSetupCurrencyExchange.FromCurrency,
                    ToCurrency = finSetupCurrencyExchange.ToCurrency,
                    DateFrom = finSetupCurrencyExchange.DateFrom,
                    DateTo = finSetupCurrencyExchange.DateTo,
                    RateOne = finSetupCurrencyExchange.RateOne,
                    RateTwo = finSetupCurrencyExchange.RateTwo,
                    DateCreated = finSetupCurrencyExchange.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = finSetupCurrencyExchange.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.FinSetupCurrencyExchanges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance currency exchange rate", "Success", User.Identity.Name, $"updated currency exchange rate {entity.FinSetupCurrencyExchangeID}", Modules.Finance);
                #endregion
            }

            return Json(new[] { finSetupCurrencyExchange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinSetupCurrencyExchanges_Destroy([DataSourceRequest]DataSourceRequest request, FinSetupCurrencyExchange finSetupCurrencyExchange)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinSetupCurrencyExchange
                {
                    FinSetupCurrencyExchangeID = finSetupCurrencyExchange.FinSetupCurrencyExchangeID,
                    FromCurrency = finSetupCurrencyExchange.FromCurrency,
                    ToCurrency = finSetupCurrencyExchange.ToCurrency,
                    DateFrom = finSetupCurrencyExchange.DateFrom,
                    DateTo = finSetupCurrencyExchange.DateTo,
                    RateOne = finSetupCurrencyExchange.RateOne,
                    RateTwo = finSetupCurrencyExchange.RateTwo,
                    DateCreated = finSetupCurrencyExchange.DateCreated,
                    LastModified = finSetupCurrencyExchange.LastModified,
                    CreatedBy = finSetupCurrencyExchange.CreatedBy,
                    ModifiedBy = finSetupCurrencyExchange.ModifiedBy
                };

                _db.FinSetupCurrencyExchanges.Attach(entity);
                _db.FinSetupCurrencyExchanges.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { finSetupCurrencyExchange }.ToDataSourceResult(request, ModelState));
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
