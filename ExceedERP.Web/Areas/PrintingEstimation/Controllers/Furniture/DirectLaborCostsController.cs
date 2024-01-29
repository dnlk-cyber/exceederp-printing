﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.PrintingEstimation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.PrintingEstimation.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Furniture;

namespace ExceedERP.Web.Areas.PrintingEstimation.Controllers.Furniture
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class DirectLaborCostsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DirectLaborCosts_Read([DataSourceRequest]DataSourceRequest request, int? estimationFormId)
        {
            IQueryable<DirectLaborCost> laborcosts = db.DirectLaborCosts.Where(q=>q.FurnitureCostEstimationId== estimationFormId).OrderByDescending(a => a.DirectLaborCostId);
            DataSourceResult result = laborcosts.ToDataSourceResult(request, laborCost => new {
                DirectLaborCostId = laborCost.DirectLaborCostId,
                FurnitureCostEstimationId = laborCost.FurnitureCostEstimationId,
                FurnitureCategoryId = laborCost.FurnitureCategoryId,
                WorkingTime = laborCost.WorkingTime,
                CostPerHour = laborCost.CostPerHour,
                TotalCost = laborCost.TotalCost,
                DateCreated = laborCost.DateCreated,
                LastModified = laborCost.LastModified
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DirectLaborCosts_Create([DataSourceRequest]DataSourceRequest request, DirectLaborCost laborCost,  int parentId)
        {
            if (ModelState.IsValid)
            {
                var costperHr = db.CriteriaCategories.Find(laborCost.FurnitureCategoryId);
                laborCost.FurnitureCostEstimationId = parentId;
                laborCost.CostPerHour = costperHr != null ? laborCost.CostPerHour : 0;
                laborCost.TotalCost = laborCost.WorkingTime * laborCost.CostPerHour;
                var entity = GetMappedObject(laborCost);
                db.DirectLaborCosts.Add(entity);
                db.SaveChanges();
                laborCost.DirectLaborCostId = entity.DirectLaborCostId;
                 var estimation = new UpdateFurnitureOverallCost();
                   estimation.update(parentId);

            }
        

            return Json(new[] { laborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLaborCosts_Update([DataSourceRequest]DataSourceRequest request, DirectLaborCost laborCost)
        {
            if (ModelState.IsValid)
            {
                var costperHr = db.CriteriaCategories.Find(laborCost.FurnitureCategoryId);
                laborCost.CostPerHour = costperHr != null ? laborCost.CostPerHour : 0;
                laborCost.TotalCost = laborCost.WorkingTime * laborCost.CostPerHour;
                var entity = GetMappedObject(laborCost);
                db.DirectLaborCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var estimation = new UpdateFurnitureOverallCost();
                estimation.update(laborCost.FurnitureCostEstimationId);
            }

            return Json(new[] { laborCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DirectLaborCosts_Destroy([DataSourceRequest]DataSourceRequest request, DirectLaborCost laborCost)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(laborCost);
                db.DirectLaborCosts.Attach(entity);
                db.DirectLaborCosts.Remove(entity);
                db.SaveChanges();
                var estimation = new UpdateFurnitureOverallCost();
                estimation.update(laborCost.FurnitureCostEstimationId);
            }

            return Json(new[] { laborCost }.ToDataSourceResult(request, ModelState));
        }
        private DirectLaborCost GetMappedObject(DirectLaborCost laborCost)
        {
            
            var entity = new DirectLaborCost
            {
                DirectLaborCostId = laborCost.DirectLaborCostId,
                FurnitureCostEstimationId = laborCost.FurnitureCostEstimationId,
                FurnitureCategoryId = laborCost.FurnitureCategoryId,
                WorkingTime = laborCost.WorkingTime,
                CostPerHour = laborCost.CostPerHour,
                TotalCost = laborCost.TotalCost,
                DateCreated = laborCost.DateCreated,
                LastModified = laborCost.LastModified

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
