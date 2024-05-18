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
    public class PrintingJobTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingJobTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingJobType> printingjobtypes = db.PrintingJobTypes;
            DataSourceResult result = printingjobtypes.ToDataSourceResult(request, printingJobType => new {
                PrintingJobTypeId = printingJobType.PrintingJobTypeId,
                JobTypeName = printingJobType.JobTypeName,
                NoOfPagesFactor = printingJobType.NoOfPagesFactor,
                DefaultNoOfPages = printingJobType.DefaultNoOfPages,
                IsSpinRequired = printingJobType.IsSpinRequired,
                IsBindingRequired = printingJobType.IsBindingRequired,
                DateCreated = printingJobType.DateCreated,
                LastModified = printingJobType.LastModified,
                CreatedBy = printingJobType.CreatedBy,
                ModifiedBy = printingJobType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingJobTypes_Create([DataSourceRequest]DataSourceRequest request, PrintingJobType printingJobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingJobType
                {
                    JobTypeName = printingJobType.JobTypeName,
                    NoOfPagesFactor = printingJobType.NoOfPagesFactor,
                    DefaultNoOfPages = printingJobType.DefaultNoOfPages,
                    IsSpinRequired = printingJobType.IsSpinRequired,
                    IsBindingRequired = printingJobType.IsBindingRequired,
                    DateCreated = printingJobType.DateCreated,
                    LastModified = printingJobType.LastModified,
                    CreatedBy = printingJobType.CreatedBy,
                    ModifiedBy = printingJobType.ModifiedBy
                };

                db.PrintingJobTypes.Add(entity);
                db.SaveChanges();
                printingJobType.PrintingJobTypeId = entity.PrintingJobTypeId;
            }

            return Json(new[] { printingJobType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingJobTypes_Update([DataSourceRequest]DataSourceRequest request, PrintingJobType printingJobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingJobType
                {
                    PrintingJobTypeId = printingJobType.PrintingJobTypeId,
                    JobTypeName = printingJobType.JobTypeName,
                    NoOfPagesFactor = printingJobType.NoOfPagesFactor,
                    DefaultNoOfPages = printingJobType.DefaultNoOfPages,
                    IsSpinRequired = printingJobType.IsSpinRequired,
                    IsBindingRequired = printingJobType.IsBindingRequired,
                    DateCreated = printingJobType.DateCreated,
                    LastModified = printingJobType.LastModified,
                    CreatedBy = printingJobType.CreatedBy,
                    ModifiedBy = printingJobType.ModifiedBy
                };

                db.PrintingJobTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { printingJobType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingJobTypes_Destroy([DataSourceRequest]DataSourceRequest request, PrintingJobType printingJobType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingJobType
                {
                    PrintingJobTypeId = printingJobType.PrintingJobTypeId,
                    JobTypeName = printingJobType.JobTypeName,
                    NoOfPagesFactor = printingJobType.NoOfPagesFactor,
                    DefaultNoOfPages = printingJobType.DefaultNoOfPages,
                    IsSpinRequired = printingJobType.IsSpinRequired,
                    IsBindingRequired = printingJobType.IsBindingRequired,
                    DateCreated = printingJobType.DateCreated,
                    LastModified = printingJobType.LastModified,
                    CreatedBy = printingJobType.CreatedBy,
                    ModifiedBy = printingJobType.ModifiedBy
                };

                db.PrintingJobTypes.Attach(entity);
                db.PrintingJobTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { printingJobType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
