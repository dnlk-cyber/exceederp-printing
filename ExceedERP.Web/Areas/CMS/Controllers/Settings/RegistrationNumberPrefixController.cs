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
    public class RegistrationNumberPrefixController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegistrationNumberPrefixs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RegistrationNumberPrefix> data = db.RegistrationNumberPrefixes;
            DataSourceResult result = data.ToDataSourceResult(request, prefix => new {
                RegistrationNumberPrefixId = prefix.RegistrationNumberPrefixId,
                PrefixValue = prefix.PrefixValue,
                Description = prefix.Description,
                IsDefault = prefix.IsDefault,

                DateCreated = prefix.DateCreated,
                LastModified = prefix.LastModified,
                CreatedBy = prefix.CreatedBy,
                ModifiedBy = prefix.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegistrationNumberPrefixs_Create([DataSourceRequest]DataSourceRequest request, RegistrationNumberPrefix prefix)
        {
            if (DuplicateDefaultPrefixExists(prefix))
            {
                ModelState.AddModelError("DuplicateDefault", "Duplicate Default Id Prefix not allowed");
            }
            if (DuplicatePrefixDefinitionExists(prefix))
            {
                ModelState.AddModelError("", "Duplicate Id Prefix definition not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new RegistrationNumberPrefix
                {
                    PrefixValue = prefix.PrefixValue,
                    Description = prefix.Description,
                    IsDefault = prefix.IsDefault,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.RegistrationNumberPrefixes.Add(entity);
                db.SaveChanges();
                prefix.RegistrationNumberPrefixId = entity.RegistrationNumberPrefixId;
            }

            return Json(new[] { prefix }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegistrationNumberPrefixs_Update([DataSourceRequest]DataSourceRequest request, RegistrationNumberPrefix prefix)
        {
            if (DuplicateDefaultPrefixExists(prefix))
            {
                ModelState.AddModelError("DuplicateDefault", "Duplicate Default Id Prefix not allowed");
            }
            if (DuplicatePrefixDefinitionExists(prefix))
            {
                ModelState.AddModelError("", "Duplicate Id Prefix definition not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new RegistrationNumberPrefix
                {
                    RegistrationNumberPrefixId = prefix.RegistrationNumberPrefixId,
                    PrefixValue = prefix.PrefixValue,
                    Description = prefix.Description,
                    IsDefault = prefix.IsDefault,

                    DateCreated = prefix.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = prefix.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.RegistrationNumberPrefixes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { prefix }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegistrationNumberPrefixs_Destroy([DataSourceRequest]DataSourceRequest request, RegistrationNumberPrefix prefix)
        {
            if (ModelState.IsValid)
            {
                var entity = new RegistrationNumberPrefix
                {
                    RegistrationNumberPrefixId = prefix.RegistrationNumberPrefixId,
                    PrefixValue = prefix.PrefixValue,
                    Description = prefix.Description,
                    IsDefault = prefix.IsDefault,

                    DateCreated = prefix.DateCreated,
                    LastModified = prefix.LastModified,
                    CreatedBy = prefix.CreatedBy,
                    ModifiedBy = prefix.ModifiedBy
                };

                db.RegistrationNumberPrefixes.Attach(entity);
                db.RegistrationNumberPrefixes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { prefix }.ToDataSourceResult(request, ModelState));
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
        private bool DuplicateDefaultPrefixExists(RegistrationNumberPrefix registrationNumberPrefix)
        {
            return db.RegistrationNumberPrefixes.Any(i => i.RegistrationNumberPrefixId != registrationNumberPrefix.RegistrationNumberPrefixId && i.IsDefault && registrationNumberPrefix.IsDefault);
        }
        private bool DuplicatePrefixDefinitionExists(RegistrationNumberPrefix registrationNumberPrefix)
        {
            return db.RegistrationNumberPrefixes.Any(i => i.PrefixValue == registrationNumberPrefix.PrefixValue && i.RegistrationNumberPrefixId != registrationNumberPrefix.RegistrationNumberPrefixId);
        }

        #endregion
    }
}
