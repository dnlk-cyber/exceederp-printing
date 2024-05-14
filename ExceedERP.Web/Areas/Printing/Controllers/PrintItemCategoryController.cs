﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintItemCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintItemCategorys_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintItemCategory> printitemcategorys = db.PrintItemCategorys;
            DataSourceResult result = printitemcategorys.ToDataSourceResult(request, printItemCategory => new {
                PrintItemCategoryId = printItemCategory.PrintItemCategoryId,
                InventoryCategoryCode = printItemCategory.InventoryCategoryCode,
                PrintCategory = printItemCategory.PrintCategory,
                CostAccountCode = printItemCategory.CostAccountCode,
                InventoryAccountCode = printItemCategory.InventoryAccountCode,
                ProductionAccountCode = printItemCategory.ProductionAccountCode,
                IsActive = printItemCategory.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintItemCategorys_Create([DataSourceRequest]DataSourceRequest request, PrintItemCategory printItemCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintItemCategory
                {
                    InventoryCategoryCode = printItemCategory.InventoryCategoryCode,
                    PrintCategory = printItemCategory.PrintCategory,
                    CostAccountCode = printItemCategory.CostAccountCode,
                    InventoryAccountCode = printItemCategory.InventoryAccountCode,
                    ProductionAccountCode = printItemCategory.ProductionAccountCode,
                    IsActive = printItemCategory.IsActive
                };

                db.PrintItemCategorys.Add(entity);
                db.SaveChanges();
                printItemCategory.PrintItemCategoryId = entity.PrintItemCategoryId;
            }

            return Json(new[] { printItemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintItemCategorys_Update([DataSourceRequest]DataSourceRequest request, PrintItemCategory printItemCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintItemCategory
                {
                    PrintItemCategoryId = printItemCategory.PrintItemCategoryId,
                    InventoryCategoryCode = printItemCategory.InventoryCategoryCode,
                    PrintCategory = printItemCategory.PrintCategory,
                    CostAccountCode = printItemCategory.CostAccountCode,
                    InventoryAccountCode = printItemCategory.InventoryAccountCode,
                    ProductionAccountCode = printItemCategory.ProductionAccountCode,
                    IsActive = printItemCategory.IsActive
                };

                db.PrintItemCategorys.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printItemCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintItemCategorys_Destroy([DataSourceRequest]DataSourceRequest request, PrintItemCategory printItemCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintItemCategory
                {
                    PrintItemCategoryId = printItemCategory.PrintItemCategoryId,
                    InventoryCategoryCode = printItemCategory.InventoryCategoryCode,
                    PrintCategory = printItemCategory.PrintCategory,
                    CostAccountCode = printItemCategory.CostAccountCode,
                    InventoryAccountCode = printItemCategory.InventoryAccountCode,
                    ProductionAccountCode = printItemCategory.ProductionAccountCode,
                    IsActive = printItemCategory.IsActive
                };

                db.PrintItemCategorys.Attach(entity);
                db.PrintItemCategorys.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printItemCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
