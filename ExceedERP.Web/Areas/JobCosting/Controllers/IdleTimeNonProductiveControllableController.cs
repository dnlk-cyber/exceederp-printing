﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.JobCosting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.JobCosting.Controllers
{
   // [AuthorizeRoles(JobCostingRoles.JobCostingIdleTime)]
    public class IdleTimeNonProductiveControllableController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeNonProductiveControllables_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IdleTimeNonProductiveControllable> idletimenonproductivecontrollables = db.IdleTimeNonProductiveControllables;
            DataSourceResult result = idletimenonproductivecontrollables.ToDataSourceResult(request, idleTimeNonProductiveControllable => new {
                IdleTimeNonProductiveControllableID = idleTimeNonProductiveControllable.IdleTimeNonProductiveControllableID,
                IdleTimeID = idleTimeNonProductiveControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveControllable.IdleTimeCategoryId,
                NPCManualHour = idleTimeNonProductiveControllable.NPCManualHour,
                NPCManualPercentage = idleTimeNonProductiveControllable.NPCManualPercentage,
                NPCMechanicalHour = idleTimeNonProductiveControllable.NPCMechanicalHour,
                NPCMechanicalPercentage = idleTimeNonProductiveControllable.NPCMechanicalPercentage,
                NPCCameraHour = idleTimeNonProductiveControllable.NPCCameraHour,
                NPCCameraPercentage = idleTimeNonProductiveControllable.NPCCameraPercentage,
                NPCOffsetHour = idleTimeNonProductiveControllable.NPCOffsetHour,
                NPCOffsetPercentage = idleTimeNonProductiveControllable.NPCOffsetPercentage,
                NPCLetterPressHour = idleTimeNonProductiveControllable.NPCLetterPressHour,
                NPCLetterPressPercentage = idleTimeNonProductiveControllable.NPCLetterPressPercentage,
                NPCBindingHour = idleTimeNonProductiveControllable.NPCBindingHour,
                NPCBindingPercentage = idleTimeNonProductiveControllable.NPCBindingPercentage,
                NPCComputerHour = idleTimeNonProductiveControllable.NPCComputerHour,
                NPCComputerPercentage = idleTimeNonProductiveControllable.NPCComputerPercentage,
                NPCWebHour = idleTimeNonProductiveControllable.NPCWebHour,
                NPCWebPercentage = idleTimeNonProductiveControllable.NPCWebPercentage,
                NPCTotalHour = idleTimeNonProductiveControllable.NPCTotalHour,
                NPCTotalPercentage = idleTimeNonProductiveControllable.NPCTotalPercentage
            });

            return Json(result);
        }

        public ActionResult IdleTimeNonProductiveControllables_Create([DataSourceRequest]DataSourceRequest request, IdleTimeNonProductiveControllable idleTimeNonProductiveControllable, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeNonProductiveControllable)this.GetObject(idleTimeNonProductiveControllable, true, id);
                db.IdleTimeNonProductiveControllables.Add(entity);
                db.SaveChanges();
                idleTimeNonProductiveControllable.IdleTimeNonProductiveControllableID = entity.IdleTimeNonProductiveControllableID;
            }

            return Json(new[] { idleTimeNonProductiveControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveControllables_Update([DataSourceRequest]DataSourceRequest request, IdleTimeNonProductiveControllable idleTimeNonProductiveControllable)
        {
            if (ModelState.IsValid)
            {
                var entity = (IdleTimeNonProductiveControllable)this.GetObject(idleTimeNonProductiveControllable, false);

                db.IdleTimeNonProductiveControllables.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveControllables_Destroy([DataSourceRequest]DataSourceRequest request, IdleTimeNonProductiveControllable idleTimeNonProductiveControllable)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeNonProductiveControllable)this.GetObject(idleTimeNonProductiveControllable, false);

                db.IdleTimeNonProductiveControllables.Attach(entity);
                db.IdleTimeNonProductiveControllables.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveControllable }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(IdleTimeNonProductiveControllable idleTimeNonProductiveControllable, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeNonProductiveControllable.IdleTimeID = id;
            }
            var entity = new IdleTimeNonProductiveControllable
            {
                IdleTimeNonProductiveControllableID = idleTimeNonProductiveControllable.IdleTimeNonProductiveControllableID,
                IdleTimeID = idleTimeNonProductiveControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveControllable.IdleTimeCategoryId,
                NPCManualHour = idleTimeNonProductiveControllable.NPCManualHour,
                NPCManualPercentage = idleTimeNonProductiveControllable.NPCManualPercentage,
                NPCMechanicalHour = idleTimeNonProductiveControllable.NPCMechanicalHour,
                NPCMechanicalPercentage = idleTimeNonProductiveControllable.NPCMechanicalPercentage,
                NPCCameraHour = idleTimeNonProductiveControllable.NPCCameraHour,
                NPCCameraPercentage = idleTimeNonProductiveControllable.NPCCameraPercentage,
                NPCOffsetHour = idleTimeNonProductiveControllable.NPCOffsetHour,
                NPCOffsetPercentage = idleTimeNonProductiveControllable.NPCOffsetPercentage,
                NPCLetterPressHour = idleTimeNonProductiveControllable.NPCLetterPressHour,
                NPCLetterPressPercentage = idleTimeNonProductiveControllable.NPCLetterPressPercentage,
                NPCBindingHour = idleTimeNonProductiveControllable.NPCBindingHour,
                NPCBindingPercentage = idleTimeNonProductiveControllable.NPCBindingPercentage,
                NPCComputerHour = idleTimeNonProductiveControllable.NPCComputerHour,
                NPCComputerPercentage = idleTimeNonProductiveControllable.NPCComputerPercentage,
                NPCWebHour = idleTimeNonProductiveControllable.NPCWebHour,
                NPCWebPercentage = idleTimeNonProductiveControllable.NPCWebPercentage,
                NPCTotalHour = idleTimeNonProductiveControllable.NPCTotalHour,
                NPCTotalPercentage = idleTimeNonProductiveControllable.NPCTotalPercentage

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
