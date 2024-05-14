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
    public class PrintWorkOrderTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintWorkOrderTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintWorkOrderType> printworkordertypes = db.PrintWorkOrderTypes;
            DataSourceResult result = printworkordertypes.ToDataSourceResult(request, printWorkOrderType => new {
                PrintWorkOrderTypeId = printWorkOrderType.PrintWorkOrderTypeId,
                PrintWorkOrderTypeCode = printWorkOrderType.PrintWorkOrderTypeCode,
                Description = printWorkOrderType.Description,
                DateCreated = printWorkOrderType.DateCreated,
                LastModified = printWorkOrderType.LastModified,
                CreatedBy = printWorkOrderType.CreatedBy,
                ModifiedBy = printWorkOrderType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintWorkOrderTypes_Create([DataSourceRequest]DataSourceRequest request, PrintWorkOrderType printWorkOrderType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintWorkOrderType
                {
                    PrintWorkOrderTypeCode = printWorkOrderType.PrintWorkOrderTypeCode,
                    Description = printWorkOrderType.Description,
                    DateCreated = printWorkOrderType.DateCreated,
                    LastModified = printWorkOrderType.LastModified,
                    CreatedBy = printWorkOrderType.CreatedBy,
                    ModifiedBy = printWorkOrderType.ModifiedBy
                };

                db.PrintWorkOrderTypes.Add(entity);
                db.SaveChanges();
                printWorkOrderType.PrintWorkOrderTypeId = entity.PrintWorkOrderTypeId;
            }

            return Json(new[] { printWorkOrderType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintWorkOrderTypes_Update([DataSourceRequest]DataSourceRequest request, PrintWorkOrderType printWorkOrderType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintWorkOrderType
                {
                    PrintWorkOrderTypeId = printWorkOrderType.PrintWorkOrderTypeId,
                    PrintWorkOrderTypeCode = printWorkOrderType.PrintWorkOrderTypeCode,
                    Description = printWorkOrderType.Description,
                    DateCreated = printWorkOrderType.DateCreated,
                    LastModified = printWorkOrderType.LastModified,
                    CreatedBy = printWorkOrderType.CreatedBy,
                    ModifiedBy = printWorkOrderType.ModifiedBy
                };

                db.PrintWorkOrderTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printWorkOrderType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintWorkOrderTypes_Destroy([DataSourceRequest]DataSourceRequest request, PrintWorkOrderType printWorkOrderType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintWorkOrderType
                {
                    PrintWorkOrderTypeId = printWorkOrderType.PrintWorkOrderTypeId,
                    PrintWorkOrderTypeCode = printWorkOrderType.PrintWorkOrderTypeCode,
                    Description = printWorkOrderType.Description,
                    DateCreated = printWorkOrderType.DateCreated,
                    LastModified = printWorkOrderType.LastModified,
                    CreatedBy = printWorkOrderType.CreatedBy,
                    ModifiedBy = printWorkOrderType.ModifiedBy
                };

                db.PrintWorkOrderTypes.Attach(entity);
                db.PrintWorkOrderTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printWorkOrderType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
