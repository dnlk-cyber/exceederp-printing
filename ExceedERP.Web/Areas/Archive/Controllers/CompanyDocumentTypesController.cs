﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class CompanyDocumentTypesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompanyDocumentTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CompanyDocumentType> companydocumenttypes = db.CompanyDocumentTypes;
            DataSourceResult result = companydocumenttypes.ToDataSourceResult(request, companyDocumentType => new {
                CompanyDocumentTypeId = companyDocumentType.CompanyDocumentTypeId,
                DocumentType = companyDocumentType.DocumentType
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyDocumentTypes_Create([DataSourceRequest]DataSourceRequest request, CompanyDocumentType companyDocumentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new CompanyDocumentType
                {
                    DocumentType = companyDocumentType.DocumentType
                };

                db.CompanyDocumentTypes.Add(entity);
                db.SaveChanges();
                companyDocumentType.CompanyDocumentTypeId = entity.CompanyDocumentTypeId;
            }

            return Json(new[] { companyDocumentType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyDocumentTypes_Update([DataSourceRequest]DataSourceRequest request, CompanyDocumentType companyDocumentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new CompanyDocumentType
                {
                    CompanyDocumentTypeId = companyDocumentType.CompanyDocumentTypeId,
                    DocumentType = companyDocumentType.DocumentType
                };

                db.CompanyDocumentTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { companyDocumentType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompanyDocumentTypes_Destroy([DataSourceRequest]DataSourceRequest request, CompanyDocumentType companyDocumentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new CompanyDocumentType
                {
                    CompanyDocumentTypeId = companyDocumentType.CompanyDocumentTypeId,
                    DocumentType = companyDocumentType.DocumentType
                };

                db.CompanyDocumentTypes.Attach(entity);
                db.CompanyDocumentTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { companyDocumentType }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
