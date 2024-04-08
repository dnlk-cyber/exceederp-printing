﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureDailyFollowUpDelayReasonController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureProductionDailyFollowUpDelayReasons_Read([DataSourceRequest]DataSourceRequest request, int followUpId)
        {
            IQueryable<FurnitureProductionDailyFollowUpDelayReason> furnitureproductiondailyfollowupdelayreasons = db.FurnitureProductionDailyFollowUpDelayReasons.Where(x => x.FurnitureDailyProductionFollowUpId == followUpId);
            DataSourceResult result = furnitureproductiondailyfollowupdelayreasons.ToDataSourceResult(request, furnitureProductionDailyFollowUpDelayReason => new FurnitureProductionDailyFollowUpDelayReason
            {
                FurnitureProductionDailyFollowUpDelayReasonId = furnitureProductionDailyFollowUpDelayReason.FurnitureProductionDailyFollowUpDelayReasonId,
                ProductionDelayReason = furnitureProductionDailyFollowUpDelayReason.ProductionDelayReason,
                Remark = furnitureProductionDailyFollowUpDelayReason.Remark,
                FurnitureDailyProductionFollowUpId = furnitureProductionDailyFollowUpDelayReason.FurnitureDailyProductionFollowUpId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionDailyFollowUpDelayReasons_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionDailyFollowUpDelayReason furnitureProductionDailyFollowUpDelayReason, int followUpId)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionDailyFollowUpDelayReason
                {
                    ProductionDelayReason = furnitureProductionDailyFollowUpDelayReason.ProductionDelayReason,
                    Remark = furnitureProductionDailyFollowUpDelayReason.Remark,
                    FurnitureDailyProductionFollowUpId = followUpId
                };

                db.FurnitureProductionDailyFollowUpDelayReasons.Add(entity);
                db.SaveChanges();
                furnitureProductionDailyFollowUpDelayReason.FurnitureProductionDailyFollowUpDelayReasonId = entity.FurnitureProductionDailyFollowUpDelayReasonId;
            }

            return Json(new[] { furnitureProductionDailyFollowUpDelayReason }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionDailyFollowUpDelayReasons_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionDailyFollowUpDelayReason furnitureProductionDailyFollowUpDelayReason)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionDailyFollowUpDelayReason
                {
                    FurnitureProductionDailyFollowUpDelayReasonId = furnitureProductionDailyFollowUpDelayReason.FurnitureProductionDailyFollowUpDelayReasonId,
                    ProductionDelayReason = furnitureProductionDailyFollowUpDelayReason.ProductionDelayReason,
                    Remark = furnitureProductionDailyFollowUpDelayReason.Remark,
                    FurnitureDailyProductionFollowUpId = furnitureProductionDailyFollowUpDelayReason.FurnitureDailyProductionFollowUpId
                };

                db.FurnitureProductionDailyFollowUpDelayReasons.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureProductionDailyFollowUpDelayReason }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProductionDailyFollowUpDelayReasons_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionDailyFollowUpDelayReason furnitureProductionDailyFollowUpDelayReason)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionDailyFollowUpDelayReason
                {
                    FurnitureProductionDailyFollowUpDelayReasonId = furnitureProductionDailyFollowUpDelayReason.FurnitureProductionDailyFollowUpDelayReasonId,
                    ProductionDelayReason = furnitureProductionDailyFollowUpDelayReason.ProductionDelayReason,
                    Remark = furnitureProductionDailyFollowUpDelayReason.Remark,
                };

                db.FurnitureProductionDailyFollowUpDelayReasons.Attach(entity);
                db.FurnitureProductionDailyFollowUpDelayReasons.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureProductionDailyFollowUpDelayReason }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
