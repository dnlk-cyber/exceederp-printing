using System;
using System.Collections.Generic;
using ExceedERP.DataAccess.Context;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Health;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class HealthAndInsuranceDataProviderController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public JsonResult GetPaymentCoverage()
        {
            return Json(db.PaymentCoverageSettings.Select(p => new
            {
                PaymentCoverage = p.Payment.ToString()+"%",
                Payment = p.Payment
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessUnitAccounts()
        {
            return Json(db.BusinessUnits.Select(b => new
            {
                b.BusinessUnitId,
                b.Accountcode,
            }),
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApSetupItem()
        {
            return Json(db.ApSetupItems.Select(b => new
            {
                b.ApSetupItemId,
                b.Name,
            }),
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGLTaxRate()
        {
            return Json(db.GLTaxRates.Select(b => new
            {
                b.GLTaxRateID,
                b.TaxName,
            }),
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBenefitTypeEnumWithOutNA()
        {

            var statuses = Enum.GetValues(typeof(HealthBenefitType))
                           .Cast<HealthBenefitType>()
                           .Where(x => x != HealthBenefitType.NA)
                           .Select(v => v)
                           .ToList();

            return Json(statuses.Select(p => new
            {
                Value = Convert.ToInt32(p),
                Display = p.ToString()
            }), JsonRequestBehavior.AllowGet);
        }

    }
}