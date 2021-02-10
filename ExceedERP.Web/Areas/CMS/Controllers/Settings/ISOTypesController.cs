﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CMS;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CMS.Controllers
{
    public class ISOTypesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ISOTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ISOType> types = db.ISOTypes;
            DataSourceResult result = types.ToDataSourceResult(request, isoType => new {
                ISOTypeId = isoType.ISOTypeId,
                Name = isoType.Name,
                Description = isoType.Description,
                IsDefault = isoType.IsDefault,

                DateCreated = isoType.DateCreated,
                LastModified = isoType.LastModified,
                CreatedBy = isoType.CreatedBy,
                ModifiedBy = isoType.ModifiedBy
            });

            return Json(result);
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISOType_Create([DataSourceRequest]DataSourceRequest request, ISOType isoType)
        {
            if (DuplicateDefaultExists(isoType))
            {
                ModelState.AddModelError("DuplicateDefault", "Duplicate Default ISO not allowed");
            }
            if (DuplicateISODefinitionExists(isoType))
            {
                ModelState.AddModelError("", "Duplicate ISO Type definition not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new ISOType
                {
                    Name = isoType.Name,
                    Description = isoType.Description,
                    IsDefault = isoType.IsDefault,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.ISOTypes.Add(entity);
                db.SaveChanges();
                isoType.ISOTypeId = entity.ISOTypeId;
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISOType_Update([DataSourceRequest]DataSourceRequest request, ISOType isoType)
        {
            if (DuplicateDefaultExists(isoType))
            {
                ModelState.AddModelError("DuplicateDefault", "Duplicate Default ISO not allowed");
            }
            if (DuplicateISODefinitionExists(isoType))
            {
                ModelState.AddModelError("", "Duplicate ISO Type definition not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new ISOType
                {
                    ISOTypeId = isoType.ISOTypeId,
                    Name = isoType.Name,
                    Description = isoType.Description,
                    IsDefault = isoType.IsDefault,

                    DateCreated = isoType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = isoType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ISOTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISOType_Destroy([DataSourceRequest]DataSourceRequest request, ISOType isoType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ISOType
                {
                    ISOTypeId = isoType.ISOTypeId,
                    Name = isoType.Name,
                    Description = isoType.Description,
                    IsDefault = isoType.IsDefault,

                    DateCreated = isoType.DateCreated,
                    LastModified = isoType.LastModified,
                    CreatedBy = isoType.CreatedBy,
                    ModifiedBy = isoType.ModifiedBy
                };

                db.ISOTypes.Attach(entity);
                db.ISOTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region local methods
        private bool DuplicateDefaultExists(ISOType iSOType)
        {
            return db.ISOTypes.Any(i => i.ISOTypeId != iSOType.ISOTypeId && i.IsDefault && iSOType.IsDefault);
        }
        private bool DuplicateISODefinitionExists(ISOType iSOType)
        {
            return db.ISOTypes.Any(i => i.Name == iSOType.Name && i.ISOTypeId != iSOType.ISOTypeId);
        }

        #endregion
    }
}
