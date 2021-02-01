using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
     [Authorize(Roles = AccountReceivableAdmin)]
    public class ArSetupTaxRateController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private const string AccountReceivableAdmin = "Account_Receivable_Admin";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARSetupTaxRates_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARSetupTaxRate> arsetuptaxrates = _db.ARSetupTaxRates;
            DataSourceResult result = arsetuptaxrates.ToDataSourceResult(request, aRSetupTaxRate => new {
                ARSetupTaxRateID = aRSetupTaxRate.ARSetupTaxRateID,
                Tax = aRSetupTaxRate.Tax,
                Rate = aRSetupTaxRate.Rate,
                DateCreated = aRSetupTaxRate.DateCreated,
                LastModified = aRSetupTaxRate.LastModified,
                CreatedBy = aRSetupTaxRate.CreatedBy,
                ModifiedBy = aRSetupTaxRate.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTaxRates_Create([DataSourceRequest]DataSourceRequest request, ARSetupTaxRate aRSetupTaxRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTaxRate
                {
                    Tax = aRSetupTaxRate.Tax,
                    Rate = aRSetupTaxRate.Rate,
                    DateCreated = DateTime.Now,
                    LastModified = aRSetupTaxRate.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRSetupTaxRate.ModifiedBy
                };

                _db.ARSetupTaxRates.Add(entity);
                _db.SaveChanges();
                aRSetupTaxRate.ARSetupTaxRateID = entity.ARSetupTaxRateID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR tax rates", "Success", User.Identity.Name, $"created  {aRSetupTaxRate.Tax}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupTaxRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTaxRates_Update([DataSourceRequest]DataSourceRequest request, ARSetupTaxRate aRSetupTaxRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTaxRate
                {
                    ARSetupTaxRateID = aRSetupTaxRate.ARSetupTaxRateID,
                    Tax = aRSetupTaxRate.Tax,
                    Rate = aRSetupTaxRate.Rate,
                    DateCreated = aRSetupTaxRate.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRSetupTaxRate.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARSetupTaxRates.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR tax rates", "Success", User.Identity.Name, $"updated  {aRSetupTaxRate.Tax}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupTaxRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTaxRates_Destroy([DataSourceRequest]DataSourceRequest request, ARSetupTaxRate aRSetupTaxRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTaxRate
                {
                    ARSetupTaxRateID = aRSetupTaxRate.ARSetupTaxRateID,
                    Tax = aRSetupTaxRate.Tax,
                    Rate = aRSetupTaxRate.Rate,
                    DateCreated = aRSetupTaxRate.DateCreated,
                    LastModified = aRSetupTaxRate.LastModified,
                    CreatedBy = aRSetupTaxRate.CreatedBy,
                    ModifiedBy = aRSetupTaxRate.ModifiedBy
                };

                _db.ARSetupTaxRates.Attach(entity);
                _db.ARSetupTaxRates.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRSetupTaxRate }.ToDataSourceResult(request, ModelState));
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
