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
    public class PrintingBindingStyleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingBindingStyles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingBindingStyle> printingbindingstyles = db.PrintingBindingStyles;
            DataSourceResult result = printingbindingstyles.ToDataSourceResult(request, printingBindingStyle => new PrintingBindingStyle
            {
                PrintingBindingStyleId = printingBindingStyle.PrintingBindingStyleId,
                BindingStyleName = printingBindingStyle.BindingStyleName,
                UnitCost = printingBindingStyle.UnitCost,
                Remark = printingBindingStyle.Remark,
                DateCreated = printingBindingStyle.DateCreated,
                LastModified = printingBindingStyle.LastModified,
                CreatedBy = printingBindingStyle.CreatedBy,
                ModifiedBy = printingBindingStyle.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingBindingStyles_Create([DataSourceRequest]DataSourceRequest request, PrintingBindingStyle printingBindingStyle)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingBindingStyle
                {
                    BindingStyleName = printingBindingStyle.BindingStyleName,
                    UnitCost = printingBindingStyle.UnitCost,
                    Remark = printingBindingStyle.Remark,
                    DateCreated = DateTime.Today,
                    LastModified = printingBindingStyle.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = printingBindingStyle.ModifiedBy
                };

                db.PrintingBindingStyles.Add(entity);
                db.SaveChanges();
                printingBindingStyle.PrintingBindingStyleId = entity.PrintingBindingStyleId;
            }

            return Json(new[] { printingBindingStyle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingBindingStyles_Update([DataSourceRequest]DataSourceRequest request, PrintingBindingStyle printingBindingStyle)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingBindingStyle
                {
                    PrintingBindingStyleId = printingBindingStyle.PrintingBindingStyleId,
                    BindingStyleName = printingBindingStyle.BindingStyleName,
                    UnitCost = printingBindingStyle.UnitCost,
                    Remark = printingBindingStyle.Remark,
                    DateCreated = printingBindingStyle.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = printingBindingStyle.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PrintingBindingStyles.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingBindingStyle }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingBindingStyles_Destroy([DataSourceRequest]DataSourceRequest request, PrintingBindingStyle printingBindingStyle)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingBindingStyle
                {
                    PrintingBindingStyleId = printingBindingStyle.PrintingBindingStyleId,
                    BindingStyleName = printingBindingStyle.BindingStyleName,
                    UnitCost = printingBindingStyle.UnitCost,
                    Remark = printingBindingStyle.Remark,
                    DateCreated = printingBindingStyle.DateCreated,
                    LastModified = printingBindingStyle.LastModified,
                    CreatedBy = printingBindingStyle.CreatedBy,
                    ModifiedBy = printingBindingStyle.ModifiedBy
                };

                db.PrintingBindingStyles.Attach(entity);
                db.PrintingBindingStyles.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingBindingStyle }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
