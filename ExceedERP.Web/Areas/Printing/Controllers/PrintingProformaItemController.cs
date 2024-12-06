using ExceedERP.Core.Domain.Finance.GL;
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
            GLTaxRate glTaxRate = new GLTaxRate();
            var printingEstimation = db.PrintingCostEstimations.Find(model.PrintingCostEstimationId);
            if (printingEstimation == null) ModelState.AddModelError("", "Can't find proforma");

         
            var overAllCost = db.PrintingOverAllCosts.FirstOrDefault(x => x.PrintingCostEstimationId == printingEstimation.PrintingCostEstimationId);
            if (overAllCost == null) ModelState.AddModelError(string.Empty, "Overall cost not found");
            if(model.GLTaxRateID != null)
            {
                glTaxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == model.GLTaxRateID);
                if (glTaxRate == null)
                {
                    ModelState.AddModelError(string.Empty, "Tax rate not found!");
                }
            }
            

            if (ModelState.IsValid)
            {
                var entity = new PrintingProformaInvoiceItem
                {
                    PrintingProformaInvoiceId = id,
                    Quantity = printingEstimation.ProductQuantity,
                    NumberOfPages = printingEstimation.TotalTextNoPages,
                    TotalPrice = overAllCost.SellingPrice,
                    TransportCost = model.TransportCost,
                    JobTypeId = printingEstimation.JobTypeId,
                    GLTaxRateID = model.GLTaxRateID,
                    UnitOfMeasurment = model.UnitOfMeasurment,                    
                    Remark = model.Remark,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    PrintingCostEstimationId = model.PrintingCostEstimationId
                };
                entity.UnitPrice = entity.TotalPrice/entity.Quantity;
                if(glTaxRate != null)
                {
                    entity.GrandTotalIncludingVAT = (entity.TotalPrice * glTaxRate.CalculatedRate) + entity.TotalPrice + entity.TransportCost;
                }
                else
                {
                    entity.GrandTotalIncludingVAT = entity.TotalPrice + entity.TransportCost;
                }
                db.PrintingProformaInvoiceItems.Add(entity);
                db.SaveChanges();
                model.PrintingProformaInvoiceItemId = entity.PrintingProformaInvoiceItemId;
                model.GrandTotalIncludingVAT = entity.GrandTotalIncludingVAT;
                model.TotalPrice = entity.TotalPrice;
                model.JobTypeId = entity.JobTypeId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult PrintingProformaItem_Update([DataSourceRequest]DataSourceRequest request, PrintingProformaInvoiceItem model)
        {
            GLTaxRate glTaxRate = new GLTaxRate();
            var printingEstimation = db.PrintingCostEstimations.Find(model.PrintingCostEstimationId);
            if (printingEstimation == null) ModelState.AddModelError("", "Can't find proforma");

           
            var overAllCost = db.PrintingOverAllCosts.FirstOrDefault(x => x.PrintingCostEstimationId == printingEstimation.PrintingCostEstimationId);
            if (overAllCost == null) ModelState.AddModelError(string.Empty, "Overall cost not found");
            if (model.GLTaxRateID != null)
            {
                glTaxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == model.GLTaxRateID);
                if (glTaxRate == null)
                {
                    ModelState.AddModelError(string.Empty, "Tax rate not found!");
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new PrintingProformaInvoiceItem
                {
                    PrintingProformaInvoiceId = model.PrintingProformaInvoiceId,
                    Quantity = printingEstimation.ProductQuantity,
                    NumberOfPages = printingEstimation.TotalTextNoPages,
                    TotalPrice = overAllCost.SellingPrice,
                    TransportCost = model.TransportCost,
                    JobTypeId = printingEstimation.JobTypeId,
                    GLTaxRateID = model.GLTaxRateID,
                    UnitOfMeasurment = model.UnitOfMeasurment,
                    Remark = model.Remark,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today,
                    PrintingCostEstimationId = model.PrintingCostEstimationId
                };
                entity.UnitPrice = entity.TotalPrice / entity.Quantity;
                if (glTaxRate != null)
                {
                    entity.GrandTotalIncludingVAT = (entity.TotalPrice * glTaxRate.CalculatedRate) + entity.TotalPrice + entity.TransportCost;
                }
                else
                {
                    entity.GrandTotalIncludingVAT = entity.TotalPrice + entity.TransportCost;
                }
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