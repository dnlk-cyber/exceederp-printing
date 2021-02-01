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
    public class PurchaseSupplierController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

       
        public ActionResult Suppliers_Read([DataSourceRequest]DataSourceRequest request, int pId)
        {
            IQueryable<PurchaseSupplier> suppliers = db.PurchaseSuppliers.Where(p => p.PurchasePriceComparisonId == pId);
            DataSourceResult result = suppliers.ToDataSourceResult(request, supplier => new {
                PurchasePriceComparisonId = supplier.PurchasePriceComparisonId,
                PurchaseSupplierId = supplier.PurchaseSupplierId,
                SupplierName = supplier.SupplierName,
                UnitPrice = supplier.UnitPrice,
                IsSelectedSupplier = supplier.IsSelectedSupplier,

                OrgBranchName = supplier.OrgBranchName,
                DateCreated = supplier.DateCreated,
                LastModified = supplier.LastModified,
                CreatedBy = supplier.CreatedBy,
                ModifiedBy = supplier.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Supplier_Create([DataSourceRequest]DataSourceRequest request, PurchaseSupplier supplier, int pId)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseSupplier
                {
                    PurchasePriceComparisonId = pId,
                    PurchaseSupplierId = supplier.PurchaseSupplierId,
                    SupplierName = supplier.SupplierName,
                    UnitPrice = supplier.UnitPrice,
                    IsSelectedSupplier = supplier.IsSelectedSupplier,

                    OrgBranchName = supplier.OrgBranchName,
                    DateCreated = supplier.DateCreated,
                    LastModified = supplier.LastModified,
                    CreatedBy = supplier.CreatedBy,
                    ModifiedBy = supplier.ModifiedBy
                };

                db.PurchaseSuppliers.Add(entity);
                db.SaveChanges();
                supplier.PurchaseSupplierId = entity.PurchaseSupplierId;
            }

            return Json(new[] { supplier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Supplier_Update([DataSourceRequest]DataSourceRequest request, PurchaseSupplier supplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseSupplier
                {
                    PurchasePriceComparisonId = supplier.PurchasePriceComparisonId,
                    PurchaseSupplierId = supplier.PurchaseSupplierId,
                    SupplierName = supplier.SupplierName,
                    UnitPrice = supplier.UnitPrice,
                    IsSelectedSupplier = supplier.IsSelectedSupplier,

                    OrgBranchName = supplier.OrgBranchName,
                    DateCreated = supplier.DateCreated,
                    LastModified = supplier.LastModified,
                    CreatedBy = supplier.CreatedBy,
                    ModifiedBy = supplier.ModifiedBy
                };

                db.PurchaseSuppliers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { supplier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Supplier_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseSupplier supplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseSupplier
                {
                    PurchasePriceComparisonId = supplier.PurchasePriceComparisonId,
                    PurchaseSupplierId = supplier.PurchaseSupplierId,
                    SupplierName = supplier.SupplierName,
                    UnitPrice = supplier.UnitPrice,
                    IsSelectedSupplier = supplier.IsSelectedSupplier,

                    OrgBranchName = supplier.OrgBranchName,
                    DateCreated = supplier.DateCreated,
                    LastModified = supplier.LastModified,
                    CreatedBy = supplier.CreatedBy,
                    ModifiedBy = supplier.ModifiedBy
                };

                db.PurchaseSuppliers.Attach(entity);
                db.PurchaseSuppliers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { supplier }.ToDataSourceResult(request, ModelState));
        }
        public void SetSelectedSupplier(int supplierId) {
            var currentSupplier = db.PurchaseSuppliers.Find(supplierId);
            if(currentSupplier != null)
            {
                var comparisonObj = db.PurchasePriceComparison.Find(currentSupplier.PurchasePriceComparisonId);
                if(comparisonObj != null)
                {
                    comparisonObj.SelectedPrice = currentSupplier.UnitPrice;
                    comparisonObj.SelectedSupplier = currentSupplier.SupplierName;

                    db.PurchasePriceComparison.Attach(comparisonObj);
                    db.Entry(comparisonObj).State = EntityState.Modified;
                    db.SaveChanges();


                    currentSupplier.IsSelectedSupplier = true;

                    db.PurchaseSuppliers.Attach(currentSupplier);
                    db.Entry(currentSupplier).State = EntityState.Modified;
                    db.SaveChanges();

                    var otherSuppliers = db.PurchaseSuppliers.Where(s => s.PurchasePriceComparisonId == comparisonObj.PurchasePriceComparisonId &&
                                            s.PurchaseSupplierId != currentSupplier.PurchaseSupplierId).ToList();
                    if (otherSuppliers.Any())
                    {
                        foreach(PurchaseSupplier supplier in otherSuppliers)
                        {
                            supplier.IsSelectedSupplier = false;
                            db.PurchaseSuppliers.Attach(supplier);
                            db.Entry(supplier).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}