﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin)]
    public class ActualDetailsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActualDetails_Read([DataSourceRequest]DataSourceRequest request,int mAId)
        {
            IQueryable<ActualDetail> actualdetails = db.ActualDetails.Where(s=>s.BSCActualId == mAId&&s.ParentId==0);
            DataSourceResult result = actualdetails.ToDataSourceResult(request, actualDetail => new {
                ActualDetailId = actualDetail.ActualDetailId,
                ParentId = actualDetail.ParentId,
                BSCActualId = actualDetail.BSCActualId,
                PlanningDetailId = actualDetail.PlanningDetailId,
                Unit = actualDetail.Unit,
                KPIType = actualDetail.KPIType,
                IndicatorId = actualDetail.IndicatorId,
                GlobalIndicatorId = actualDetail.GlobalIndicatorId,
                HasSubIndicator = actualDetail.HasSubIndicator,
                Plan = actualDetail.Plan,
                Baseline = actualDetail.Baseline,
                Target = actualDetail.Target
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualDetails_Create([DataSourceRequest]DataSourceRequest request, ActualDetail actualDetail,int mAId)
        {
            if (ModelState.IsValid)
            {

               
                actualDetail.BSCActualId = mAId;
                var entity = GetMappedObject(actualDetail);
                db.ActualDetails.Add(entity);
                db.SaveChanges();
                actualDetail.ActualDetailId = entity.ActualDetailId;
            }

            return Json(new[] { actualDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualDetails_Update([DataSourceRequest]DataSourceRequest request, ActualDetail actualDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(actualDetail);
                db.ActualDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { actualDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActualDetails_Destroy([DataSourceRequest]DataSourceRequest request, ActualDetail actualDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(actualDetail);
                db.ActualDetails.Attach(entity);
                db.ActualDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { actualDetail }.ToDataSourceResult(request, ModelState));
        }
        private ActualDetail GetMappedObject(ActualDetail actualDetail)
        {
            var entity = new ActualDetail
            {
                ActualDetailId = actualDetail.ActualDetailId,
                ParentId = actualDetail.ParentId,
                BSCActualId = actualDetail.BSCActualId,
                Unit = actualDetail.Unit,
                KPIType = actualDetail.KPIType,
                IndicatorId = actualDetail.IndicatorId,
                GlobalIndicatorId = actualDetail.GlobalIndicatorId,
                HasSubIndicator = actualDetail.HasSubIndicator,
                Plan = actualDetail.Plan,
                PlanningDetailId = actualDetail.PlanningDetailId,
                Baseline = actualDetail.Baseline,
                Target = actualDetail.Target
            };
            return entity;
        }
        //private string getSubIndigator(int indicatorid)
        //{
        //    var subindicators = db.Indicators.Find(indicatorid);
        //    if (subindicators != null)
        //    {
        //        return subindicators.SubIndicator;
        //    }
        //    return null;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
