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
    public class PrintCostCenterController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintCostCenters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintCostCenter> printcostcenters = db.PrintCostCenters;
            DataSourceResult result = printcostcenters.ToDataSourceResult(request, printCostCenter => new {
                PrintCostCenterId = printCostCenter.PrintCostCenterId,
                PrintCostCenterCode = printCostCenter.PrintCostCenterCode,
                PrintCostCenterName = printCostCenter.PrintCostCenterName,
                DateCreated = printCostCenter.DateCreated,
                LastModified = printCostCenter.LastModified,
                CreatedBy = printCostCenter.CreatedBy,
                ModifiedBy = printCostCenter.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintCostCenters_Create([DataSourceRequest]DataSourceRequest request, PrintCostCenter printCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintCostCenter
                {
                    PrintCostCenterCode = printCostCenter.PrintCostCenterCode,
                    PrintCostCenterName = printCostCenter.PrintCostCenterName,
                    DateCreated = printCostCenter.DateCreated,
                    LastModified = printCostCenter.LastModified,
                    CreatedBy = printCostCenter.CreatedBy,
                    ModifiedBy = printCostCenter.ModifiedBy
                };

                db.PrintCostCenters.Add(entity);
                db.SaveChanges();
                printCostCenter.PrintCostCenterId = entity.PrintCostCenterId;
            }

            return Json(new[] { printCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintCostCenters_Update([DataSourceRequest]DataSourceRequest request, PrintCostCenter printCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintCostCenter
                {
                    PrintCostCenterId = printCostCenter.PrintCostCenterId,
                    PrintCostCenterCode = printCostCenter.PrintCostCenterCode,
                    PrintCostCenterName = printCostCenter.PrintCostCenterName,
                    DateCreated = printCostCenter.DateCreated,
                    LastModified = printCostCenter.LastModified,
                    CreatedBy = printCostCenter.CreatedBy,
                    ModifiedBy = printCostCenter.ModifiedBy
                };

                db.PrintCostCenters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintCostCenters_Destroy([DataSourceRequest]DataSourceRequest request, PrintCostCenter printCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintCostCenter
                {
                    PrintCostCenterId = printCostCenter.PrintCostCenterId,
                    PrintCostCenterCode = printCostCenter.PrintCostCenterCode,
                    PrintCostCenterName = printCostCenter.PrintCostCenterName,
                    DateCreated = printCostCenter.DateCreated,
                    LastModified = printCostCenter.LastModified,
                    CreatedBy = printCostCenter.CreatedBy,
                    ModifiedBy = printCostCenter.ModifiedBy
                };

                db.PrintCostCenters.Attach(entity);
                db.PrintCostCenters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printCostCenter }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
