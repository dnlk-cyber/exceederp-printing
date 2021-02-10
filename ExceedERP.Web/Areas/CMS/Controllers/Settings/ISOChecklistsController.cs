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
    public class ISOChecklistsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ISOChecklists_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ISOChecklist> checklists = db.ISOChecklists;
            DataSourceResult result = checklists.ToDataSourceResult(request, isoType => new ISOChecklist {
                ISOChecklistId = isoType.ISOChecklistId,
                ISOTypeId = isoType.ISOTypeId,
                Code = isoType.Code,
                Name = isoType.Name,
                Description = isoType.Description,
                ValueType = isoType.ValueType,
                
                DateCreated = isoType.DateCreated,
                LastModified = isoType.LastModified,
                CreatedBy = isoType.CreatedBy,
                ModifiedBy = isoType.ModifiedBy
            });

            return Json(result);
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISOChecklist_Create([DataSourceRequest]DataSourceRequest request, ISOChecklist isoType, int id)
        {
            //if (DuplicateDefaultExists(isoType))
            //{
            //    ModelState.AddModelError("DuplicateDefault", "Duplicate Default ISO not allowed");
            //}
            //if (DuplicateISODefinitionExists(isoType))
            //{
            //    ModelState.AddModelError("", "Duplicate ISO Type definition not allowed");
            //}
            if (ModelState.IsValid)
            {
                var entity = new ISOChecklist
                {
                    ISOChecklistId = isoType.ISOChecklistId,
                    ISOTypeId = id,
                    Code = isoType.Code,
                    Name = isoType.Name,
                    Description = isoType.Description,
                    ValueType = isoType.ValueType,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.ISOChecklists.Add(entity);
                db.SaveChanges();
                isoType.ISOChecklistId = entity.ISOChecklistId;
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISOChecklist_Update([DataSourceRequest]DataSourceRequest request, ISOChecklist isoType)
        {
            //if (DuplicateDefaultExists(isoType))
            //{
            //    ModelState.AddModelError("DuplicateDefault", "Duplicate Default ISO not allowed");
            //}
            //if (DuplicateISODefinitionExists(isoType))
            //{
            //    ModelState.AddModelError("", "Duplicate ISO Type definition not allowed");
            //}
            if (ModelState.IsValid)
            {
                var entity = new ISOChecklist
                {
                    ISOChecklistId = isoType.ISOChecklistId,
                    ISOTypeId = isoType.ISOTypeId,
                    Code = isoType.Code,
                    Name = isoType.Name,
                    Description = isoType.Description,
                    ValueType = isoType.ValueType,

                    DateCreated = isoType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = isoType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ISOChecklists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { isoType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ISOChecklist_Destroy([DataSourceRequest]DataSourceRequest request, ISOChecklist isoType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ISOChecklist
                {
                    ISOChecklistId = isoType.ISOChecklistId,
                    ISOTypeId = isoType.ISOTypeId,
                    Code = isoType.Code,
                    Name = isoType.Name,
                    Description = isoType.Description,
                    ValueType = isoType.ValueType,

                    DateCreated = isoType.DateCreated,
                    LastModified = isoType.LastModified,
                    CreatedBy = isoType.CreatedBy,
                    ModifiedBy = isoType.ModifiedBy
                };

                db.ISOChecklists.Attach(entity);
                db.ISOChecklists.Remove(entity);
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
        //private bool DuplicateDefaultExists(ISOChecklist iSOType)
        //{
        //    return db.ISOChecklists.Any(i => i.ISOChecklistId != iSOType.ISOChecklistId && i.IsDefault && iSOType.IsDefault);
        //}
        //private bool DuplicateISODefinitionExists(ISOChecklist iSOType)
        //{
        //    return db.ISOChecklists.Any(i => i.Name == iSOType.Name && i.ISOChecklistId != iSOType.ISOChecklistId);
        //}

        #endregion
    }
}
