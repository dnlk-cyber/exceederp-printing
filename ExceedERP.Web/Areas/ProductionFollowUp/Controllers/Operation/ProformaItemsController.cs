using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{
    public class ProformaItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProformaItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PFProformaItems> proformaitems = db.PFProformaItems.Where(x => x.ProformaId == id);
            DataSourceResult result = proformaitems.ToDataSourceResult(request, proformaItems => new
            {
                ProformaItemsId = proformaItems.PFProformaItemsId,
                ProformaId = proformaItems.ProformaId,
                JobTypeId = proformaItems.JobTypeId,
                JobTypeItemId = proformaItems.JobTypeItemId,
                SubCategory = proformaItems.SubCategory,
                Quantity = proformaItems.Quantity,
                Page = proformaItems.Page,
                Size = proformaItems.Size,
                UnitPrice = proformaItems.UnitPrice,
                Tax = proformaItems.Tax,
                BeforeTax = proformaItems.BeforeTax,
                TotalPrice = proformaItems.TotalPrice,               
                Description = proformaItems.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Create([DataSourceRequest]DataSourceRequest request, PFProformaItems pFProformaItems, int idCreate)
        {
            //var jobType = db.JobTypes.FirstOrDefault(x => x.JobTypeId == pFProformaItems.JobTypeId);
            //if (jobType != null)
            //{
            //    var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
            //    if (tax != null)
            //    {
            //        pFProformaItems.Tax = tax.Rate;

            //        pFProformaItems.TotalPrice = (pFProformaItems.UnitPrice * pFProformaItems.Quantity) + (pFProformaItems.UnitPrice * pFProformaItems.Quantity * tax.CalculatedRate);
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "Add VAT setting first");
            //    }

            //}
            //else
            //{
            //    ModelState.AddModelError("", "No Such Job Type");
            //}
            if (ModelState.IsValid)
            {
                var entity = new PFProformaItems
                {
                    ProformaId = idCreate,
                    JobTypeId = pFProformaItems.JobTypeId,
                    JobTypeItemId = pFProformaItems.JobTypeItemId,
                    SubCategory = pFProformaItems.SubCategory,
                    Quantity = pFProformaItems.Quantity,
                    Page = pFProformaItems.Page,
                    Size = pFProformaItems.Size,
                    UnitPrice = pFProformaItems.UnitPrice,
                    Tax = pFProformaItems.Tax,
                    BeforeTax = pFProformaItems.BeforeTax,
                    TotalPrice = pFProformaItems.TotalPrice,
                    Description = pFProformaItems.Description
                };

                db.PFProformaItems.Add(entity);
                db.SaveChanges();
                pFProformaItems.PFProformaItemsId = entity.PFProformaItemsId;
            }

            return Json(new[] { pFProformaItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Update([DataSourceRequest]DataSourceRequest request, PFProformaItems pFProformaItems)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new PFProformaItems
                {
                    PFProformaItemsId = pFProformaItems.PFProformaItemsId,
                    ProformaId = pFProformaItems.ProformaId,
                    JobTypeId = pFProformaItems.JobTypeId,
                    JobTypeItemId = pFProformaItems.JobTypeItemId,
                    SubCategory = pFProformaItems.SubCategory,
                    Quantity = pFProformaItems.Quantity,
                    Page = pFProformaItems.Page,
                    Size = pFProformaItems.Size,
                    UnitPrice = pFProformaItems.UnitPrice,
                    Tax = pFProformaItems.Tax,
                    BeforeTax = pFProformaItems.BeforeTax,
                    TotalPrice = pFProformaItems.TotalPrice,
                    Description = pFProformaItems.Description
                };

                db.PFProformaItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { pFProformaItems }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaItems_Destroy([DataSourceRequest]DataSourceRequest request, PFProformaItems pFProformaItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new PFProformaItems
                {
                    PFProformaItemsId = pFProformaItems.PFProformaItemsId,
                    ProformaId = pFProformaItems.ProformaId,
                    JobTypeId = pFProformaItems.JobTypeId,
                    JobTypeItemId = pFProformaItems.JobTypeItemId,
                    SubCategory = pFProformaItems.SubCategory,
                    Quantity = pFProformaItems.Quantity,
                    Page = pFProformaItems.Page,
                    Size = pFProformaItems.Size,
                    UnitPrice = pFProformaItems.UnitPrice,
                    Tax = pFProformaItems.Tax,
                    BeforeTax = pFProformaItems.BeforeTax,
                    TotalPrice = pFProformaItems.TotalPrice,
                    Description = pFProformaItems.Description
                };

                db.PFProformaItems.Attach(entity);
                db.PFProformaItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { pFProformaItems }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult GetBeforeVatAndTotalPerice(int jobTypeId, int quantity, double unitPrice)
        {
            var jobType = db.JobTypes.FirstOrDefault(x => x.JobTypeId == jobTypeId);
            if (jobType != null)
            {
                var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                if (tax != null)
                {

                    double beforeTax = quantity * unitPrice;
                    double taxBirr = quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate);
                    double withTax = (quantity * unitPrice) + (quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate));

                    var calculatedField = Json(new
                    {
                        Status = "OK",
                        BeforeTax = Math.Round(beforeTax, 3),
                        Tax = Math.Round(taxBirr, 3),
                        TotalPrice = Math.Round(withTax, 3)
                    });

                    return calculatedField;
                }
            }

            var nodata = Json(new
            {
                Status = "OK",
                BeforeTax = 0,
                Tax = 0,
                TotalPrice = 0
            });

            return nodata;

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
