using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Areas.CRM.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class PriceEstimationQuoteVoucherServiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult ServiceLineItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PriceEstimationQuoteVoucherService> PriceEstimationQuoteVoucherServices = db.PriceEstimationQuoteVoucherServices.Where(x => x.PriceEstimationQuoteVoucherId == id);
            DataSourceResult result = PriceEstimationQuoteVoucherServices.ToDataSourceResult(request, PriceEstimationQuoteVoucherService => new
            {
                PriceEstimationQuoteVoucherServiceId = PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherServiceId,
                PriceEstimationQuoteVoucherId = PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherId,
                CategoryCode = PriceEstimationQuoteVoucherService.CategoryCode,
                SubCategoryCode = PriceEstimationQuoteVoucherService.SubCategoryCode,
                //ServiceGroup = PriceEstimationQuoteVoucherService.ServiceGroup,
                ServiceTypeId = PriceEstimationQuoteVoucherService.ServiceTypeId,
                Quantity = PriceEstimationQuoteVoucherService.Quantity,
                UnitAmount = PriceEstimationQuoteVoucherService.UnitAmount,
                SubTotal = PriceEstimationQuoteVoucherService.SubTotal,
                TaxTypeId = PriceEstimationQuoteVoucherService.TaxTypeId,
                TaxAmount = PriceEstimationQuoteVoucherService.TaxAmount,
                WithholdingTypeId = PriceEstimationQuoteVoucherService.WithholdingTypeId,
                WithholdingAmount = PriceEstimationQuoteVoucherService.WithholdingAmount,
                WithholdTax = PriceEstimationQuoteVoucherService.WithholdTax,
                WithholdTaxAmount = PriceEstimationQuoteVoucherService.WithholdTaxAmount,
                TotalAmount = PriceEstimationQuoteVoucherService.TotalAmount,
              
            });

            return Json(result);
        }

       

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Create([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherService PriceEstimationQuoteVoucherService, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceEstimationQuoteVoucherService
                {
                    PriceEstimationQuoteVoucherServiceId = PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherServiceId,
                    PriceEstimationQuoteVoucherId = id,
                    CategoryCode = PriceEstimationQuoteVoucherService.CategoryCode,
                    SubCategoryCode = PriceEstimationQuoteVoucherService.SubCategoryCode,
                    //ServiceGroup = PriceEstimationQuoteVoucherService.ServiceGroup,
                    ServiceTypeId = PriceEstimationQuoteVoucherService.ServiceTypeId,
                    Quantity = PriceEstimationQuoteVoucherService.Quantity,
                    UnitAmount = PriceEstimationQuoteVoucherService.UnitAmount,
                    SubTotal = PriceEstimationQuoteVoucherService.SubTotal,
                    TaxTypeId = PriceEstimationQuoteVoucherService.TaxTypeId,
                    TaxAmount = PriceEstimationQuoteVoucherService.TaxAmount,
                    WithholdingTypeId = PriceEstimationQuoteVoucherService.WithholdingTypeId,
                    WithholdingAmount = PriceEstimationQuoteVoucherService.WithholdingAmount,
                    WithholdTax = PriceEstimationQuoteVoucherService.WithholdTax,
                    WithholdTaxAmount = PriceEstimationQuoteVoucherService.WithholdTaxAmount,
                    TotalAmount = PriceEstimationQuoteVoucherService.TotalAmount,
                };
                entity.Quantity = 1;
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

                db.PriceEstimationQuoteVoucherServices.Add(entity);
                db.SaveChanges();
                PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherServiceId = entity.PriceEstimationQuoteVoucherServiceId;
            }


            return Json(new[] { PriceEstimationQuoteVoucherService }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Update([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherService PriceEstimationQuoteVoucherService)
        {
          
            var entity = new PriceEstimationQuoteVoucherService
            {
                PriceEstimationQuoteVoucherServiceId = PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherServiceId,
                PriceEstimationQuoteVoucherId = PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherId,
                CategoryCode = PriceEstimationQuoteVoucherService.CategoryCode,
                SubCategoryCode = PriceEstimationQuoteVoucherService.SubCategoryCode,
                //ServiceGroup = PriceEstimationQuoteVoucherService.ServiceGroup,
                ServiceTypeId = PriceEstimationQuoteVoucherService.ServiceTypeId,
                Quantity = PriceEstimationQuoteVoucherService.Quantity,
                UnitAmount = PriceEstimationQuoteVoucherService.UnitAmount,
                SubTotal = PriceEstimationQuoteVoucherService.SubTotal,
                TaxTypeId = PriceEstimationQuoteVoucherService.TaxTypeId,
                TaxAmount = PriceEstimationQuoteVoucherService.TaxAmount,
                WithholdingTypeId = PriceEstimationQuoteVoucherService.WithholdingTypeId,
                WithholdingAmount = PriceEstimationQuoteVoucherService.WithholdingAmount,
                WithholdTax = PriceEstimationQuoteVoucherService.WithholdTax,
                WithholdTaxAmount = PriceEstimationQuoteVoucherService.WithholdTaxAmount,
                TotalAmount = PriceEstimationQuoteVoucherService.TotalAmount,
            };
            entity.Quantity = 1;
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

            db.PriceEstimationQuoteVoucherServices.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new[] { PriceEstimationQuoteVoucherService }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceLineItems_Destroy([DataSourceRequest]DataSourceRequest request, PriceEstimationQuoteVoucherService PriceEstimationQuoteVoucherService)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceEstimationQuoteVoucherService
                {
                    PriceEstimationQuoteVoucherServiceId = PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherServiceId,
                    PriceEstimationQuoteVoucherId = PriceEstimationQuoteVoucherService.PriceEstimationQuoteVoucherId,
                    CategoryCode = PriceEstimationQuoteVoucherService.CategoryCode,
                    SubCategoryCode = PriceEstimationQuoteVoucherService.SubCategoryCode,
                    //ServiceGroup = PriceEstimationQuoteVoucherService.ServiceGroup,
                    ServiceTypeId = PriceEstimationQuoteVoucherService.ServiceTypeId,
                    Quantity = PriceEstimationQuoteVoucherService.Quantity,
                    UnitAmount = PriceEstimationQuoteVoucherService.UnitAmount,
                    SubTotal = PriceEstimationQuoteVoucherService.SubTotal,
                    TaxTypeId = PriceEstimationQuoteVoucherService.TaxTypeId,
                    TaxAmount = PriceEstimationQuoteVoucherService.TaxAmount,
                    WithholdingTypeId = PriceEstimationQuoteVoucherService.WithholdingTypeId,
                    WithholdingAmount = PriceEstimationQuoteVoucherService.WithholdingAmount,
                    WithholdTax = PriceEstimationQuoteVoucherService.WithholdTax,
                    WithholdTaxAmount = PriceEstimationQuoteVoucherService.WithholdTaxAmount,
                    TotalAmount = PriceEstimationQuoteVoucherService.TotalAmount,
                };

                db.PriceEstimationQuoteVoucherServices.Attach(entity);
                db.PriceEstimationQuoteVoucherServices.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { PriceEstimationQuoteVoucherService }.ToDataSourceResult(request, ModelState));
        }

         protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
