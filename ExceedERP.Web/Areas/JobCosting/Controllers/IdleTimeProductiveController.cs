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
    public class IdleTimeProductiveController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeProductives_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IdleTimeProductive> idletimeproductives = db.IdleTimeProductives.Where(q=>q.IdleTimeID == id);
            DataSourceResult result = idletimeproductives.ToDataSourceResult(request, idleTimeProductive => new {
                IdleTimeProductiveID = idleTimeProductive.IdleTimeProductiveID,
                IdleTimeID = idleTimeProductive.IdleTimeID,
                ProductiveManualHour = idleTimeProductive.ProductiveManualHour,
                ProductiveManualPercentage = idleTimeProductive.ProductiveManualPercentage,
                ProductiveMechanicalHour = idleTimeProductive.ProductiveMechanicalHour,
                ProductiveMechanicalPercentage = idleTimeProductive.ProductiveMechanicalPercentage,
                ProductiveCameraHour = idleTimeProductive.ProductiveCameraHour,
                ProductiveCameraPercentage = idleTimeProductive.ProductiveCameraPercentage,
                ProductiveOffsetHour = idleTimeProductive.ProductiveOffsetHour,
                ProductiveOffsetPercentage = idleTimeProductive.ProductiveOffsetPercentage,
                ProductiveLetterPressHour = idleTimeProductive.ProductiveLetterPressHour,
                ProductiveLetterPressPercentage = idleTimeProductive.ProductiveLetterPressPercentage,
                ProductiveBindingHour = idleTimeProductive.ProductiveBindingHour,
                ProductiveBindingPercentage = idleTimeProductive.ProductiveBindingPercentage,
                ProductiveComputerHour = idleTimeProductive.ProductiveComputerHour,
                ProductiveComputerPercentage = idleTimeProductive.ProductiveComputerPercentage,
                ProductiveWebHour = idleTimeProductive.ProductiveWebHour,
                ProductiveWebPercentage = idleTimeProductive.ProductiveWebPercentage,
                ProductiveTotalHour = idleTimeProductive.ProductiveTotalHour,
                ProductiveTotalPercentage = idleTimeProductive.ProductiveTotalPercentage
            });

            return Json(result);
        }


        public ActionResult IdleTimeProductives_Create([DataSourceRequest]DataSourceRequest request, IdleTimeProductive idleTimeProductive, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeProductive)this.GetObject(idleTimeProductive, true, id);
                db.IdleTimeProductives.Add(entity);
                db.SaveChanges();
                idleTimeProductive.IdleTimeProductiveID = entity.IdleTimeProductiveID;
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeProductives_Update([DataSourceRequest]DataSourceRequest request, IdleTimeProductive idleTimeProductive)
        {
            if (ModelState.IsValid)
            {
                var entity = (IdleTimeProductive)this.GetObject(idleTimeProductive, false);

                db.IdleTimeProductives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeProductives_Destroy([DataSourceRequest]DataSourceRequest request, IdleTimeProductive idleTimeProductive)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeProductive)this.GetObject(idleTimeProductive, false);

                db.IdleTimeProductives.Attach(entity);
                db.IdleTimeProductives.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(IdleTimeProductive idleTimeProductive, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeProductive.IdleTimeID = id;
            }
            var entity = new IdleTimeProductive
            {
                IdleTimeProductiveID = idleTimeProductive.IdleTimeProductiveID,
                IdleTimeID = idleTimeProductive.IdleTimeID,
                ProductiveManualHour = idleTimeProductive.ProductiveManualHour,
                ProductiveManualPercentage = idleTimeProductive.ProductiveManualPercentage,
                ProductiveMechanicalHour = idleTimeProductive.ProductiveMechanicalHour,
                ProductiveMechanicalPercentage = idleTimeProductive.ProductiveMechanicalPercentage,
                ProductiveCameraHour = idleTimeProductive.ProductiveCameraHour,
                ProductiveCameraPercentage = idleTimeProductive.ProductiveCameraPercentage,
                ProductiveOffsetHour = idleTimeProductive.ProductiveOffsetHour,
                ProductiveOffsetPercentage = idleTimeProductive.ProductiveOffsetPercentage,
                ProductiveLetterPressHour = idleTimeProductive.ProductiveLetterPressHour,
                ProductiveLetterPressPercentage = idleTimeProductive.ProductiveLetterPressPercentage,
                ProductiveBindingHour = idleTimeProductive.ProductiveBindingHour,
                ProductiveBindingPercentage = idleTimeProductive.ProductiveBindingPercentage,
                ProductiveComputerHour = idleTimeProductive.ProductiveComputerHour,
                ProductiveComputerPercentage = idleTimeProductive.ProductiveComputerPercentage,
                ProductiveWebHour = idleTimeProductive.ProductiveWebHour,
                ProductiveWebPercentage = idleTimeProductive.ProductiveWebPercentage,
                ProductiveTotalHour = idleTimeProductive.ProductiveTotalHour,
                ProductiveTotalPercentage = idleTimeProductive.ProductiveTotalPercentage

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
