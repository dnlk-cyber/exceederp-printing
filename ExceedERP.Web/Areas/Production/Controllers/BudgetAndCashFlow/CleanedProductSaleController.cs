using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.BudgetAndCashFlow
{
    public class CleanedProductSaleController : Controller
    {
        // GET: Production/Crop
        ExceedDbContext db;
        public CleanedProductSaleController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult Sales_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CleanedProductSale> sales = db.CleanedProductSales;
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new 
            {
                CleanedProductSaleId = sale.CleanedProductSaleId,
                BranchId = sale.BranchId,
                GlFiscalYearId = sale.GlFiscalYearId,                

                OrgBranchName = sale.OrgBranchName,
                DateCreated = sale.DateCreated,
                LastModified = sale.LastModified,
                CreatedBy = sale.CreatedBy,
                ModifiedBy = sale.ModifiedBy,
            });

            return Json(result);
        }
        private bool ItemExists(CleanedProductSale record)
        {
            return db.CleanedProductSales
                .FirstOrDefault(i => i.BranchId == record.BranchId && i.GlFiscalYearId == record.GlFiscalYearId && i.CleanedProductSaleId != record.CleanedProductSaleId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Create([DataSourceRequest]DataSourceRequest request, CleanedProductSale sale)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new CleanedProductSale
                    {
                        CleanedProductSaleId = sale.CleanedProductSaleId,
                        BranchId = sale.BranchId,
                        GlFiscalYearId = sale.GlFiscalYearId,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = sale.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = sale.ModifiedBy,
                    };

                    db.CleanedProductSales.Add(entity);
                    db.SaveChanges();

                    sale.CleanedProductSaleId = entity.CleanedProductSaleId;

                    //this.CreateMonthlyCashFlowItems(entity);
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sales_Update([DataSourceRequest]DataSourceRequest request, CleanedProductSale sale)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(sale))
                {
                    ModelState.AddModelError("", "Duplicate Values not Allowed");
                }
                else
                {
                    var entity = new CleanedProductSale
                    {
                        CleanedProductSaleId = sale.CleanedProductSaleId,
                        BranchId = sale.BranchId,
                        GlFiscalYearId = sale.GlFiscalYearId,

                        OrgBranchName = sale.OrgBranchName,
                        DateCreated = sale.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = sale.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.CleanedProductSales.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sale_Destroy([DataSourceRequest]DataSourceRequest request, CleanedProductSale sale)
        {
            if (ModelState.IsValid)
            {
                var entity = new CleanedProductSale
                {
                    CleanedProductSaleId = sale.CleanedProductSaleId,
                    BranchId = sale.BranchId,
                    GlFiscalYearId = sale.GlFiscalYearId,

                    OrgBranchName = sale.OrgBranchName,
                    DateCreated = sale.DateCreated,
                    LastModified = sale.LastModified,
                    CreatedBy = sale.CreatedBy,
                    ModifiedBy = sale.ModifiedBy,

                };

                db.CleanedProductSales.Attach(entity);
                db.CleanedProductSales.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { sale }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}