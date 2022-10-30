using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers.Setting
{
    public class UnitConvertorSetupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Products"] = db.Products.Select(p => new { Name = p.Name, ProductId = p.ProductID }).ToList();
            return View();
        }

        public ActionResult ProductUnitConvertorSetup_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductUnitConvertorSetup> productunitconvertorsetup = db.ProductUnitConvertorSetup;
            DataSourceResult result = productunitconvertorsetup.ToDataSourceResult(request, productUnitConvertorSetup => new
            {
                ProductUnitConvertorSetupId = productUnitConvertorSetup.ProductUnitConvertorSetupId,
                ProductId = productUnitConvertorSetup.ProductId,
                FromUoM = productUnitConvertorSetup.FromUoM,
                ToUoM = productUnitConvertorSetup.ToUoM,
                MultiplicationFactor = productUnitConvertorSetup.MultiplicationFactor,

                OrgBranchName = productUnitConvertorSetup.OrgBranchName,
                DateCreated = productUnitConvertorSetup.DateCreated,
                LastModified = productUnitConvertorSetup.LastModified,
                CreatedBy = productUnitConvertorSetup.CreatedBy,
                ModifiedBy = productUnitConvertorSetup.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductUnitConvertorSetup_Create([DataSourceRequest]DataSourceRequest request, ProductUnitConvertorSetup productUnitConvertorSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductUnitConvertorSetup
                {
                    ProductId = productUnitConvertorSetup.ProductId,
                    FromUoM = productUnitConvertorSetup.FromUoM,
                    ToUoM = productUnitConvertorSetup.ToUoM,
                    MultiplicationFactor = productUnitConvertorSetup.MultiplicationFactor,

                    OrgBranchName = productUnitConvertorSetup.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = productUnitConvertorSetup.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = productUnitConvertorSetup.ModifiedBy
                };

                db.ProductUnitConvertorSetup.Add(entity);
                db.SaveChanges();
                productUnitConvertorSetup.ProductUnitConvertorSetupId = entity.ProductUnitConvertorSetupId;
            }

            return Json(new[] { productUnitConvertorSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductUnitConvertorSetup_Update([DataSourceRequest]DataSourceRequest request, ProductUnitConvertorSetup productUnitConvertorSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductUnitConvertorSetup
                {
                    ProductUnitConvertorSetupId = productUnitConvertorSetup.ProductUnitConvertorSetupId,
                    ProductId = productUnitConvertorSetup.ProductId,
                    FromUoM = productUnitConvertorSetup.FromUoM,
                    ToUoM = productUnitConvertorSetup.ToUoM,
                    MultiplicationFactor = productUnitConvertorSetup.MultiplicationFactor,

                    OrgBranchName = productUnitConvertorSetup.OrgBranchName,
                    DateCreated = productUnitConvertorSetup.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = productUnitConvertorSetup.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProductUnitConvertorSetup.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { productUnitConvertorSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductUnitConvertorSetup_Destroy([DataSourceRequest]DataSourceRequest request, ProductUnitConvertorSetup productUnitConvertorSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductUnitConvertorSetup
                {
                    ProductUnitConvertorSetupId = productUnitConvertorSetup.ProductUnitConvertorSetupId,
                    ProductId = productUnitConvertorSetup.ProductId,
                    FromUoM = productUnitConvertorSetup.FromUoM,
                    ToUoM = productUnitConvertorSetup.ToUoM,
                    MultiplicationFactor = productUnitConvertorSetup.MultiplicationFactor,

                    OrgBranchName = productUnitConvertorSetup.OrgBranchName,
                    DateCreated = productUnitConvertorSetup.DateCreated,
                    LastModified = productUnitConvertorSetup.LastModified,
                    CreatedBy = productUnitConvertorSetup.CreatedBy,
                    ModifiedBy = productUnitConvertorSetup.ModifiedBy
                };

                db.ProductUnitConvertorSetup.Attach(entity);
                db.ProductUnitConvertorSetup.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { productUnitConvertorSetup }.ToDataSourceResult(request, ModelState));
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

        public string GetSelectedProductUoM(int productId)
        {
            return db.Products.Find(productId)?.UOM;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
