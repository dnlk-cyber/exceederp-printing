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
    public class TraineeIdNumberComponentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult TraineeIdNumberComponents_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TraineeIdNumberComponent> components = db.TraineeIdNumberComponents.Where(c => c.CmsTraineeId == id);
            DataSourceResult result = components.ToDataSourceResult(request, traineeIdNumberComponent => new {
                TraineeIdNumberComponentId = traineeIdNumberComponent.TraineeIdNumberComponentId,
                RegistrationNumberPrefixId = traineeIdNumberComponent.RegistrationNumberPrefixId,
                PrefixValue = traineeIdNumberComponent.PrefixValue,
                ISOTypeId = traineeIdNumberComponent.ISOTypeId,
                ISOType = traineeIdNumberComponent.ISOType,
                CmsTraineeId = traineeIdNumberComponent.CmsTraineeId,
                SystemId = traineeIdNumberComponent.SystemId,
                SequenceNumber = traineeIdNumberComponent.SequenceNumber,
                CombinedIdNumber = traineeIdNumberComponent.CombinedIdNumber,

                DateCreated = traineeIdNumberComponent.DateCreated,
                LastModified = traineeIdNumberComponent.LastModified,
                CreatedBy = traineeIdNumberComponent.CreatedBy,
                ModifiedBy = traineeIdNumberComponent.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TraineeIdNumberComponent_Create([DataSourceRequest]DataSourceRequest request, TraineeIdNumberComponent traineeIdNumberComponent, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new TraineeIdNumberComponent
                {
                    CmsTraineeId = id,

                    RegistrationNumberPrefixId = traineeIdNumberComponent.RegistrationNumberPrefixId,
                    PrefixValue = traineeIdNumberComponent.PrefixValue,
                    ISOTypeId = traineeIdNumberComponent.ISOTypeId,
                    ISOType = traineeIdNumberComponent.ISOType,
                    SystemId = traineeIdNumberComponent.SystemId,
                    SequenceNumber = traineeIdNumberComponent.SequenceNumber,
                    CombinedIdNumber = traineeIdNumberComponent.CombinedIdNumber,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.TraineeIdNumberComponents.Add(entity);
                db.SaveChanges();
                traineeIdNumberComponent.TraineeIdNumberComponentId = entity.TraineeIdNumberComponentId;
            }

            return Json(new[] { traineeIdNumberComponent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TraineeIdNumberComponent_Update([DataSourceRequest]DataSourceRequest request, TraineeIdNumberComponent traineeIdNumberComponent)
        {
            if (ModelState.IsValid)
            {
                var entity = new TraineeIdNumberComponent
                {
                    TraineeIdNumberComponentId = traineeIdNumberComponent.TraineeIdNumberComponentId,
                    RegistrationNumberPrefixId = traineeIdNumberComponent.RegistrationNumberPrefixId,
                    PrefixValue = traineeIdNumberComponent.PrefixValue,
                    ISOTypeId = traineeIdNumberComponent.ISOTypeId,
                    ISOType = traineeIdNumberComponent.ISOType,
                    CmsTraineeId = traineeIdNumberComponent.CmsTraineeId,
                    SystemId = traineeIdNumberComponent.SystemId,
                    SequenceNumber = traineeIdNumberComponent.SequenceNumber,
                    CombinedIdNumber = traineeIdNumberComponent.CombinedIdNumber,

                    DateCreated = traineeIdNumberComponent.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = traineeIdNumberComponent.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.TraineeIdNumberComponents.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { traineeIdNumberComponent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TraineeIdNumberComponent_Destroy([DataSourceRequest]DataSourceRequest request, TraineeIdNumberComponent traineeIdNumberComponent)
        {
            if (ModelState.IsValid)
            {
                var entity = new TraineeIdNumberComponent
                {
                    TraineeIdNumberComponentId = traineeIdNumberComponent.TraineeIdNumberComponentId,
                    RegistrationNumberPrefixId = traineeIdNumberComponent.RegistrationNumberPrefixId,
                    PrefixValue = traineeIdNumberComponent.PrefixValue,
                    ISOTypeId = traineeIdNumberComponent.ISOTypeId,
                    ISOType = traineeIdNumberComponent.ISOType,
                    CmsTraineeId = traineeIdNumberComponent.CmsTraineeId,
                    SystemId = traineeIdNumberComponent.SystemId,
                    SequenceNumber = traineeIdNumberComponent.SequenceNumber,
                    CombinedIdNumber = traineeIdNumberComponent.CombinedIdNumber,

                    DateCreated = traineeIdNumberComponent.DateCreated,
                    LastModified = traineeIdNumberComponent.LastModified,
                    CreatedBy = traineeIdNumberComponent.CreatedBy,
                    ModifiedBy = traineeIdNumberComponent.ModifiedBy
                };

                db.TraineeIdNumberComponents.Attach(entity);
                db.TraineeIdNumberComponents.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { traineeIdNumberComponent }.ToDataSourceResult(request, ModelState));
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
    }
}
