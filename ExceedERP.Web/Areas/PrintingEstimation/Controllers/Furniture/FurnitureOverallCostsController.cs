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
using ExceedERP.Core.Domain.Furniture;

namespace ExceedERP.Web.Areas.PrintingEstimation.Controllers.Furniture
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureOverallCostsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult FurnitureOverallCosts_Read([DataSourceRequest]DataSourceRequest request,int estimationFormId)
        {

            IQueryable<FurnitureOverallCost> overallcosts = db.FurnitureOverallCosts.Where(q=>q.FurnitureCostEstimationId == estimationFormId);
            decimal materialCost = db.FurnitureMaterialCosts.Where(q => q.FurnitureCostEstimationId == estimationFormId).ToList().Sum(q => q.TotalPrice);
            decimal laborCost = db.DirectLaborCosts.Where(q => q.FurnitureCostEstimationId == estimationFormId).ToList().Sum(q => q.TotalCost);
            decimal lab_mate_cost = materialCost + laborCost;
            //var Tcost = ((overallCost.OverHeadCost) / 100) * lab_mate_cost + lab_mate_cost;
            //var NBTCost = ((overallCost.MarginalCost) / 100) * Tcost + Tcost;
            //var GTCost = ((overallCost.TInpercent) / 100) * NBTCost + NBTCost;
             var result = overallcosts.AsEnumerable().Select(overallCost => new FurnitureOverallCost {
                 FurnitureOverallCostId = overallCost.FurnitureOverallCostId,
                FurnitureCostEstimationId = overallCost.FurnitureCostEstimationId,
                MaterialCost = materialCost,
                LaborCost = laborCost,
                OverHeadCost = overallCost.OverHeadCost,
                T = overallCost.T,
                MarginalCost = overallCost.MarginalCost,
                NBT =  overallCost.NBT,
                TInpercent = overallCost.TInpercent,
                GT = overallCost.GT,
                Graphic = overallCost.Graphic,
                EstimatedNetPrice =  overallCost.EstimatedNetPrice
             }).ToList();

            return Json(result.ToTreeDataSourceResult(request));
        }
        public ActionResult FurnitureOverallCosts_Create([DataSourceRequest]DataSourceRequest request, FurnitureOverallCost overallCost, int parentId)
        {
            
            if (ModelState.IsValid)
            {
                var materialCost = db.FurnitureMaterialCosts.Where(q => q.FurnitureCostEstimationId == parentId).ToList().Sum(q => q.TotalPrice);
                var laborCost = db.DirectLaborCosts.Where(q => q.FurnitureCostEstimationId == parentId).ToList().Sum(q => q.TotalCost);
                var lab_mate_cost = materialCost + laborCost;
                var Tcost = ((overallCost.OverHeadCost) / 100) * lab_mate_cost + lab_mate_cost;
                var NBTCost = ((overallCost.MarginalCost) / 100) * Tcost + Tcost;
                var GTCost = ((overallCost.TInpercent) / 100) * NBTCost + NBTCost;
                var entity = new FurnitureOverallCost
                {
                    OverHeadCost = overallCost.OverHeadCost,
                    FurnitureCostEstimationId = parentId,
                    T = Tcost,
                    MarginalCost = overallCost.MarginalCost,
                    NBT = NBTCost,
                    TInpercent = overallCost.TInpercent,
                    GT = GTCost,
                    Graphic = overallCost.Graphic,
                    EstimatedNetPrice = GTCost+ overallCost.Graphic

                };

                db.FurnitureOverallCosts.Add(entity);
                db.SaveChanges();
               
             

            }
            return Json(new[] { overallCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureOverallCosts_Update([DataSourceRequest]DataSourceRequest request, FurnitureOverallCost overallCost)
        {
           
                
                if (ModelState.IsValid)
                {
                decimal materialCost = db.FurnitureMaterialCosts.Where(q => q.FurnitureCostEstimationId == overallCost.FurnitureCostEstimationId).ToList().Sum(q => q.TotalPrice);
                decimal laborCost = db.DirectLaborCosts.Where(q => q.FurnitureCostEstimationId == overallCost.FurnitureCostEstimationId).ToList().Sum(q => q.TotalCost);
                decimal lab_mate_cost = materialCost + laborCost;
                decimal Tcost = ((overallCost.OverHeadCost) / 100) * lab_mate_cost + lab_mate_cost;
                decimal NBTCost = ((overallCost.MarginalCost) / 100) * Tcost + Tcost;
                decimal GTCost = ((overallCost.TInpercent) / 100) * NBTCost + NBTCost;
                var entity = new FurnitureOverallCost
                {
                    FurnitureOverallCostId = overallCost.FurnitureOverallCostId,
                    FurnitureCostEstimationId = overallCost.FurnitureCostEstimationId,
                    OverHeadCost = overallCost.OverHeadCost,
                        T = Tcost,
                        MarginalCost = overallCost.MarginalCost,
                        NBT = NBTCost,
                        TInpercent = overallCost.TInpercent,
                        GT = GTCost,
                        Graphic = overallCost.Graphic,
                        EstimatedNetPrice = GTCost + overallCost.Graphic

                    };

                    db.FurnitureOverallCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                
            }

            return Json(new[] { overallCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureOverallCosts_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureOverallCost overallCost)
        {

            if (ModelState.IsValid)
            {
                var entity = new FurnitureOverallCost
                {
                    FurnitureOverallCostId = overallCost.FurnitureOverallCostId,
                    OverHeadCost = overallCost.OverHeadCost,
                    T = overallCost.T,
                    MarginalCost = overallCost.MarginalCost,
                    TInpercent = overallCost.TInpercent,
                    GT = overallCost.GT,
                    Graphic = overallCost.Graphic,
                    EstimatedNetPrice = overallCost.EstimatedNetPrice

                };

                db.FurnitureOverallCosts.Attach(entity);
                db.FurnitureOverallCosts.Remove(entity);
                db.SaveChanges();
               
            }

            return Json(new[] { overallCost }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
