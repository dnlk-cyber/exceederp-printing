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

namespace ExceedERP.Web.Areas.JobCosting.Controllers
{
    // [AuthorizeRoles(JobCostingRoles.JobCostingIdleTime)]
    public class IdleTimeNonProductiveNonControllableController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeNonProductiveNonControllables_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<IdleTimeNonProductiveNonControllable> idletimenonproductivenoncontrollables = db.IdleTimeNonProductiveNonControllables;
            DataSourceResult result = idletimenonproductivenoncontrollables.ToDataSourceResult(request, idleTimeNonProductiveNonControllable => new {
                IdleTimeNonProductiveNonControllableID = idleTimeNonProductiveNonControllable.IdleTimeNonProductiveNonControllableID,
                IdleTimeID = idleTimeNonProductiveNonControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveNonControllable.IdleTimeCategoryId,
                NPNCManualHour = idleTimeNonProductiveNonControllable.NPNCManualHour,
                NPNCManualPercentage = idleTimeNonProductiveNonControllable.NPNCManualPercentage,
                NPNCMechanicalHour = idleTimeNonProductiveNonControllable.NPNCMechanicalHour,
                NPNCMechanicalPercentage = idleTimeNonProductiveNonControllable.NPNCMechanicalPercentage,
                NPNCCameraHour = idleTimeNonProductiveNonControllable.NPNCCameraHour,
                NPNCCameraPercentage = idleTimeNonProductiveNonControllable.NPNCCameraPercentage,
                NPNCOffsetHour = idleTimeNonProductiveNonControllable.NPNCOffsetHour,
                NPNCOffsetPercentage = idleTimeNonProductiveNonControllable.NPNCOffsetPercentage,
                NPNCLetterPressHour = idleTimeNonProductiveNonControllable.NPNCLetterPressHour,
                NPNCLetterPressPercentage = idleTimeNonProductiveNonControllable.NPNCLetterPressPercentage,
                NPNCBindingHour = idleTimeNonProductiveNonControllable.NPNCBindingHour,
                NPNCBindingPercentage = idleTimeNonProductiveNonControllable.NPNCBindingPercentage,
                NPNCComputerHour = idleTimeNonProductiveNonControllable.NPNCComputerHour,
                NPNCComputerPercentage = idleTimeNonProductiveNonControllable.NPNCComputerPercentage,
                NPNCWebHour = idleTimeNonProductiveNonControllable.NPNCWebHour,
                NPNCWebPercentage = idleTimeNonProductiveNonControllable.NPNCWebPercentage,
                NPNCTotalHour = idleTimeNonProductiveNonControllable.NPNCTotalHour,
                NPNCTotalPercentage = idleTimeNonProductiveNonControllable.NPNCTotalPercentage
            });

            return Json(result);
        }

        public ActionResult IdleTimeNonProductiveNonControllables_Create([DataSourceRequest]DataSourceRequest request, IdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, true, id);
                db.IdleTimeNonProductiveNonControllables.Add(entity);
                db.SaveChanges();
                idleTimeNonProductiveNonControllable.IdleTimeNonProductiveNonControllableID = entity.IdleTimeNonProductiveNonControllableID;
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveNonControllables_Update([DataSourceRequest]DataSourceRequest request, IdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable)
        {
            if (ModelState.IsValid)
            {
                var entity = (IdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, false);

                db.IdleTimeNonProductiveNonControllables.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveNonControllables_Destroy([DataSourceRequest]DataSourceRequest request, IdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, false);

                db.IdleTimeNonProductiveNonControllables.Attach(entity);
                db.IdleTimeNonProductiveNonControllables.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(IdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeNonProductiveNonControllable.IdleTimeID = id;
            }
            var entity = new IdleTimeNonProductiveNonControllable
            {
                IdleTimeNonProductiveNonControllableID = idleTimeNonProductiveNonControllable.IdleTimeNonProductiveNonControllableID,
                IdleTimeID = idleTimeNonProductiveNonControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveNonControllable.IdleTimeCategoryId,
                NPNCManualHour = idleTimeNonProductiveNonControllable.NPNCManualHour,
                NPNCManualPercentage = idleTimeNonProductiveNonControllable.NPNCManualPercentage,
                NPNCMechanicalHour = idleTimeNonProductiveNonControllable.NPNCMechanicalHour,
                NPNCMechanicalPercentage = idleTimeNonProductiveNonControllable.NPNCMechanicalPercentage,
                NPNCCameraHour = idleTimeNonProductiveNonControllable.NPNCCameraHour,
                NPNCCameraPercentage = idleTimeNonProductiveNonControllable.NPNCCameraPercentage,
                NPNCOffsetHour = idleTimeNonProductiveNonControllable.NPNCOffsetHour,
                NPNCOffsetPercentage = idleTimeNonProductiveNonControllable.NPNCOffsetPercentage,
                NPNCLetterPressHour = idleTimeNonProductiveNonControllable.NPNCLetterPressHour,
                NPNCLetterPressPercentage = idleTimeNonProductiveNonControllable.NPNCLetterPressPercentage,
                NPNCBindingHour = idleTimeNonProductiveNonControllable.NPNCBindingHour,
                NPNCBindingPercentage = idleTimeNonProductiveNonControllable.NPNCBindingPercentage,
                NPNCComputerHour = idleTimeNonProductiveNonControllable.NPNCComputerHour,
                NPNCComputerPercentage = idleTimeNonProductiveNonControllable.NPNCComputerPercentage,
                NPNCWebHour = idleTimeNonProductiveNonControllable.NPNCWebHour,
                NPNCWebPercentage = idleTimeNonProductiveNonControllable.NPNCWebPercentage,
                NPNCTotalHour = idleTimeNonProductiveNonControllable.NPNCTotalHour,
                NPNCTotalPercentage = idleTimeNonProductiveNonControllable.NPNCTotalPercentage

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
