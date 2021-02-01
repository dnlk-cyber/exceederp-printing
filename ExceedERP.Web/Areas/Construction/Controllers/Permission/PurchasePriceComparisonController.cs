using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Permission
{
    public class PurchasePriceComparisonController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchasePriceComparisons_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchasePriceComparison> priceComparisons = db.PurchasePriceComparison.Where(p => p.ProjectInformationId == id);
            DataSourceResult result = priceComparisons.ToDataSourceResult(request, price => new {
                PurchasePriceComparisonId = price.PurchasePriceComparisonId,
                ProjectInformationId = price.ProjectInformationId,
                ItemName = price.ItemName,
                UnitOfMeasurementId = price.UnitOfMeasurementId,
                Quantity = price.Quantity,
                SelectedPrice = price.SelectedPrice,
                SelectedSupplier = price.SelectedSupplier,
                SubTotal = price.SubTotal,
                VatAmount = price.VatAmount,
                TotalAmount = price.TotalAmount,


                OrgBranchName = price.OrgBranchName,
                DateCreated = price.DateCreated,
                LastModified = price.LastModified,
                CreatedBy = price.CreatedBy,
                ModifiedBy = price.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePriceComparison_Create([DataSourceRequest]DataSourceRequest request, PurchasePriceComparison price, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePriceComparison
                {
                    PurchasePriceComparisonId = price.PurchasePriceComparisonId,
                    ProjectInformationId = id,
                    ItemName = price.ItemName,
                    UnitOfMeasurementId = price.UnitOfMeasurementId,
                    Quantity = price.Quantity,
                    SelectedPrice = price.SelectedPrice,
                    SelectedSupplier = price.SelectedSupplier,
                    SubTotal = price.SubTotal,
                    VatAmount = price.VatAmount,
                    TotalAmount = price.TotalAmount,

                    OrgBranchName = price.OrgBranchName,
                    DateCreated = price.DateCreated,
                    LastModified = price.LastModified,
                    CreatedBy = price.CreatedBy,
                    ModifiedBy = price.ModifiedBy
                };

                db.PurchasePriceComparison.Add(entity);
                db.SaveChanges();
                price.PurchasePriceComparisonId = entity.PurchasePriceComparisonId;
            }

            return Json(new[] { price }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePriceComparison_Update([DataSourceRequest]DataSourceRequest request, PurchasePriceComparison price)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePriceComparison
                {
                    PurchasePriceComparisonId = price.PurchasePriceComparisonId,
                    ProjectInformationId = price.ProjectInformationId,
                    ItemName = price.ItemName,
                    UnitOfMeasurementId = price.UnitOfMeasurementId,
                    Quantity = price.Quantity,
                    SelectedPrice = price.SelectedPrice,
                    SelectedSupplier = price.SelectedSupplier,
                    SubTotal = price.SubTotal,
                    VatAmount = price.VatAmount,
                    TotalAmount = price.TotalAmount,

                    OrgBranchName = price.OrgBranchName,
                    DateCreated = price.DateCreated,
                    LastModified = price.LastModified,
                    CreatedBy = price.CreatedBy,
                    ModifiedBy = price.ModifiedBy
                };

                db.PurchasePriceComparison.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { price }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchasePriceComparison_Destroy([DataSourceRequest]DataSourceRequest request, PurchasePriceComparison price)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchasePriceComparison
                {
                    PurchasePriceComparisonId = price.PurchasePriceComparisonId,
                    ProjectInformationId = price.ProjectInformationId,
                    ItemName = price.ItemName,
                    UnitOfMeasurementId = price.UnitOfMeasurementId,
                    Quantity = price.Quantity,
                    SelectedPrice = price.SelectedPrice,
                    SelectedSupplier = price.SelectedSupplier,
                    SubTotal = price.SubTotal,
                    VatAmount = price.VatAmount,
                    TotalAmount = price.TotalAmount,

                    OrgBranchName = price.OrgBranchName,
                    DateCreated = price.DateCreated,
                    LastModified = price.LastModified,
                    CreatedBy = price.CreatedBy,
                    ModifiedBy = price.ModifiedBy
                };

                db.PurchasePriceComparison.Attach(entity);
                db.PurchasePriceComparison.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { price }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}