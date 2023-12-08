using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class PriceEstimationQuoteVoucherLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult ServiceLineItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PriceEstimationQuoteVoucherLine> PriceEstimationQuoteVoucherLines = db.PriceEstimationQuoteVoucherLines.Where(x => x.PriceEstimationQuoteVoucherId == id);
            DataSourceResult result = PriceEstimationQuoteVoucherLines.ToDataSourceResult(request, PriceEstimationQuoteVoucherLine => new
            {
                PriceEstimationQuoteVoucherLineId = PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherLineId,
                PriceEstimationQuoteVoucherId = PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherId,
                CategoryId = PriceEstimationQuoteVoucherLine.CategoryId,
                ProductId = PriceEstimationQuoteVoucherLine.ProductId,
                Quantity = PriceEstimationQuoteVoucherLine.Quantity,
                UnitAmount = PriceEstimationQuoteVoucherLine.UnitAmount,
                SubTotal = PriceEstimationQuoteVoucherLine.SubTotal,
                TaxTypeId = PriceEstimationQuoteVoucherLine.TaxTypeId,
                TaxAmount = PriceEstimationQuoteVoucherLine.TaxAmount,
                WithholdingTypeId = PriceEstimationQuoteVoucherLine.WithholdingTypeId,
                WithholdingAmount = PriceEstimationQuoteVoucherLine.WithholdingAmount,
                WithholdTax = PriceEstimationQuoteVoucherLine.WithholdTax,
                WithholdTaxAmount = PriceEstimationQuoteVoucherLine.WithholdTaxAmount,
                TotalAmount = PriceEstimationQuoteVoucherLine.TotalAmount,
              
            });

            return Json(result);
        }

       

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Create([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherLine PriceEstimationQuoteVoucherLine, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceEstimationQuoteVoucherLine
                {
                    PriceEstimationQuoteVoucherLineId = PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherLineId,
                    PriceEstimationQuoteVoucherId = id,
                    CategoryId = PriceEstimationQuoteVoucherLine.CategoryId,
                    ProductId = PriceEstimationQuoteVoucherLine.ProductId,
                    Quantity = PriceEstimationQuoteVoucherLine.Quantity,
                    UnitAmount = PriceEstimationQuoteVoucherLine.UnitAmount,
                    SubTotal = PriceEstimationQuoteVoucherLine.SubTotal,
                    TaxTypeId = PriceEstimationQuoteVoucherLine.TaxTypeId,
                    TaxAmount = PriceEstimationQuoteVoucherLine.TaxAmount,
                    WithholdingTypeId = PriceEstimationQuoteVoucherLine.WithholdingTypeId,
                    WithholdingAmount = PriceEstimationQuoteVoucherLine.WithholdingAmount,
                    WithholdTax = PriceEstimationQuoteVoucherLine.WithholdTax,
                    WithholdTaxAmount = PriceEstimationQuoteVoucherLine.WithholdTaxAmount,
                    TotalAmount = PriceEstimationQuoteVoucherLine.TotalAmount,
                };
                entity.SubTotal = entity.Quantity * entity.UnitAmount;
                entity.TaxAmount = CRMSalesHelper.CalculateTaxAmount(entity.TaxTypeId, entity.SubTotal);
                if (entity.WithholdTax)
                {
                    entity.WithholdTaxAmount = CRMSalesHelper.CalculateVatWithholdingAmount(entity.SubTotal, entity.TaxAmount);
                }
                else
                {
                    entity.WithholdTaxAmount = 0;
                }
                entity.WithholdingAmount = CRMSalesHelper.CalculateTaxAmount(entity.WithholdingTypeId, entity.SubTotal);
                entity.TotalAmount = entity.SubTotal + entity.TaxAmount;

                db.PriceEstimationQuoteVoucherLines.Add(entity);
                db.SaveChanges();
                PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherLineId = entity.PriceEstimationQuoteVoucherLineId;

                PriceEstimationQuoteVoucherLine.TaxAmount = entity.TaxAmount;
                PriceEstimationQuoteVoucherLine.SubTotal = entity.SubTotal;
                PriceEstimationQuoteVoucherLine.WithholdingAmount = entity.WithholdingAmount;
                PriceEstimationQuoteVoucherLine.TotalAmount = entity.TotalAmount;
            }


            return Json(new[] { PriceEstimationQuoteVoucherLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Update([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherLine PriceEstimationQuoteVoucherLine)
        {
          
            var entity = new PriceEstimationQuoteVoucherLine
            {
                PriceEstimationQuoteVoucherLineId = PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherLineId,
                PriceEstimationQuoteVoucherId = PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherId,
                CategoryId = PriceEstimationQuoteVoucherLine.CategoryId,
                ProductId = PriceEstimationQuoteVoucherLine.ProductId,
                Quantity = PriceEstimationQuoteVoucherLine.Quantity,
                UnitAmount = PriceEstimationQuoteVoucherLine.UnitAmount,
                SubTotal = PriceEstimationQuoteVoucherLine.SubTotal,
                TaxTypeId = PriceEstimationQuoteVoucherLine.TaxTypeId,
                TaxAmount = PriceEstimationQuoteVoucherLine.TaxAmount,
                WithholdingTypeId = PriceEstimationQuoteVoucherLine.WithholdingTypeId,
                WithholdingAmount = PriceEstimationQuoteVoucherLine.WithholdingAmount,
                WithholdTax = PriceEstimationQuoteVoucherLine.WithholdTax,
                WithholdTaxAmount = PriceEstimationQuoteVoucherLine.WithholdTaxAmount,
                TotalAmount = PriceEstimationQuoteVoucherLine.TotalAmount,
            };
            entity.SubTotal = entity.Quantity * entity.UnitAmount;
            entity.TaxAmount = CRMSalesHelper.CalculateTaxAmount(entity.TaxTypeId, entity.SubTotal);
            if (entity.WithholdTax)
            {
                entity.WithholdTaxAmount = CRMSalesHelper.CalculateVatWithholdingAmount(entity.SubTotal, entity.TaxAmount);
            }
            else
            {
                entity.WithholdTaxAmount = 0;
            }
            entity.WithholdingAmount = CRMSalesHelper.CalculateTaxAmount(entity.WithholdingTypeId, entity.SubTotal);
            entity.TotalAmount = entity.SubTotal + entity.TaxAmount;

            db.PriceEstimationQuoteVoucherLines.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            PriceEstimationQuoteVoucherLine.TaxAmount = entity.TaxAmount;
            PriceEstimationQuoteVoucherLine.SubTotal = entity.SubTotal;
            PriceEstimationQuoteVoucherLine.WithholdingAmount = entity.WithholdingAmount;
            PriceEstimationQuoteVoucherLine.TotalAmount = entity.TotalAmount;
            //}

            return Json(new[] { PriceEstimationQuoteVoucherLine }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Destroy([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherLine PriceEstimationQuoteVoucherLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceEstimationQuoteVoucherLine
                {
                    PriceEstimationQuoteVoucherLineId = PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherLineId,
                    PriceEstimationQuoteVoucherId = PriceEstimationQuoteVoucherLine.PriceEstimationQuoteVoucherId,
                    CategoryId = PriceEstimationQuoteVoucherLine.CategoryId,
                    ProductId = PriceEstimationQuoteVoucherLine.ProductId,
                    Quantity = PriceEstimationQuoteVoucherLine.Quantity,
                    UnitAmount = PriceEstimationQuoteVoucherLine.UnitAmount,
                    SubTotal = PriceEstimationQuoteVoucherLine.SubTotal,
                    TaxTypeId = PriceEstimationQuoteVoucherLine.TaxTypeId,
                    TaxAmount = PriceEstimationQuoteVoucherLine.TaxAmount,
                    WithholdingTypeId = PriceEstimationQuoteVoucherLine.WithholdingTypeId,
                    WithholdingAmount = PriceEstimationQuoteVoucherLine.WithholdingAmount,
                    WithholdTax = PriceEstimationQuoteVoucherLine.WithholdTax,
                    WithholdTaxAmount = PriceEstimationQuoteVoucherLine.WithholdTaxAmount,
                    TotalAmount = PriceEstimationQuoteVoucherLine.TotalAmount,
                };

                db.PriceEstimationQuoteVoucherLines.Attach(entity);
                db.PriceEstimationQuoteVoucherLines.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { PriceEstimationQuoteVoucherLine }.ToDataSourceResult(request, ModelState));
        }

         protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
