using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingProformaItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingProformaItem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingProformaItem_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var printingProformaInvoiceItems = db.PrintingProformaInvoiceItems.Where(x => x.PrintingProformaInvoiceId == id);
            return Json(printingProformaInvoiceItems.ToDataSourceResult(request));
        }

        public ActionResult PrintingProformaItem_Create([DataSourceRequest]DataSourceRequest request, PrintingProformaInvoiceItem model, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProformaInvoiceItem
                {
                    PrintingProformaInvoiceId = id,
                    Quantity = model.Quantity,
                    TotalPrice = model.TotalPrice,
                    TransportCost = model.TransportCost,
                    JobTypeId = model.JobTypeId,
                    GLTaxRateID = model.GLTaxRateID,
                    UnitOfMeasurment = model.UnitOfMeasurment,
                    UnitPrice = model.UnitPrice,
                    Remark = model.Remark,
                    PrintingCostEstimationId = model.PrintingCostEstimationId

                };
                db.PrintingProformaInvoiceItems.Add(entity);
                db.SaveChanges();
                model.PrintingProformaInvoiceItemId = entity.PrintingProformaInvoiceItemId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult PrintingProformaItem_Update([DataSourceRequest]DataSourceRequest request, PrintingProformaInvoiceItem model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProformaInvoiceItem
                {
                    PrintingProformaInvoiceId = model.PrintingProformaInvoiceId,
                    Quantity = model.Quantity,
                    TotalPrice = model.TotalPrice,
                    TransportCost = model.TransportCost,
                    JobTypeId = model.JobTypeId,
                    GLTaxRateID = model.GLTaxRateID,
                    UnitOfMeasurment = model.UnitOfMeasurment,
                    UnitPrice = model.UnitPrice,
                    Remark = model.Remark,
                    PrintingCostEstimationId = model.PrintingCostEstimationId
                };
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                model.PrintingProformaInvoiceItemId = entity.PrintingProformaInvoiceItemId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult PrintingProformaItem_Delete([DataSourceRequest]DataSourceRequest request, PrintingProformaInvoiceItem model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingProformaInvoiceItems.Find(model.PrintingProformaInvoiceItemId);
                db.PrintingProformaInvoiceItems.Remove(entity);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}