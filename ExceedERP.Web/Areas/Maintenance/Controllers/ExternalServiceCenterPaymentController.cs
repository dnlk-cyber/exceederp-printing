using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class ExternalServiceCenterPaymentController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Maintenance/ExternalServiceCenterPayment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExternalServiceCenterPayment_Read([DataSourceRequest] DataSourceRequest request, Guid scId)
        {
            var externalServicePayment = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == scId);

            var result = externalServicePayment.ToDataSourceResult(request, model => new ExternalServiceCenterPayment
            {
                ServiceCenterJobId = model.ServiceCenterJobId,
                CostType = model.CostType,
                Date = model.Date,
                ExternalServiceCenterPaymentId = model.ExternalServiceCenterPaymentId,
                Quantity = model.Quantity,
                SubTotal = model.SubTotal,
                Invoice=model.Invoice,
                Tax = model.Tax,
                Total = model.Total,
                UnitPrice = model.UnitPrice,
                GlTaxRateId = model.GlTaxRateId
            });
            return Json(result);
        }

        public ActionResult ExternalServiceCenterPayment_Create([DataSourceRequest] DataSourceRequest request, ExternalServiceCenterPayment model, Guid scId)
        {
            ModelState.Clear();
            model.ServiceCenterJobId = scId;
            var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == model.GlTaxRateId);
            if (tax == null)
                ModelState.AddModelError("", "Tax rate needed");
            if (ModelState.IsValid)
            {
                model.SubTotal = model.UnitPrice * model.Quantity;
                model.Total = model.SubTotal + model.SubTotal * tax.Rate/100;
                
                db.ExternalServiceCenterPayments.Add(model);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ExternalServiceCenterPayment_Update([DataSourceRequest] DataSourceRequest request, ExternalServiceCenterPayment model)
        {
            var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == model.GlTaxRateId);
            if (tax == null)
                ModelState.AddModelError("", "Tax rate needed");
            if (ModelState.IsValid)
            {
                model.SubTotal = model.UnitPrice * model.Quantity;
                model.Total = model.SubTotal + model.SubTotal * tax.Rate;
                db.ExternalServiceCenterPayments.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ExternalServiceCenterPayment_Delete([DataSourceRequest] DataSourceRequest request, ExternalServiceCenterPayment model)
        {
            if (ModelState.IsValid)
            {
                var externalServiceCenterPayment = db.ExternalServiceCenterPayments.Find(model.ExternalServiceCenterPaymentId);
                db.ExternalServiceCenterPayments.Remove(externalServiceCenterPayment);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}