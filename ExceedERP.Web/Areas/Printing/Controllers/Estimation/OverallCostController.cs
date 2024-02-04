﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.printing.PrintingEstimation;

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class OverallCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult OverallCosts_Read([DataSourceRequest]DataSourceRequest request,int estimationFormId)
        {

            IQueryable<OverallCost> overallcosts = db.OverallCosts.Where(q=>q.EstimationFormId == estimationFormId);
            decimal materialCost = db.MaterialCosts.Where(q => q.EstimationFormId == estimationFormId).ToList().Sum(q => q.TotalPrice);
            decimal laborCost = db.LaborCosts.Where(q => q.EstimationFormId == estimationFormId).ToList().Sum(q => q.TotalCost);
            decimal lab_mate_cost = materialCost + laborCost;
            //var Tcost = ((overallCost.OverHeadCost) / 100) * lab_mate_cost + lab_mate_cost;
            //var NBTCost = ((overallCost.MarginalCost) / 100) * Tcost + Tcost;
            //var GTCost = ((overallCost.TInpercent) / 100) * NBTCost + NBTCost;
             var result = overallcosts.AsEnumerable().Select(overallCost => new OverallCost { 
                OverallCostId = overallCost.OverallCostId,
                EstimationFormId = overallCost.EstimationFormId,
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
        public ActionResult OverallCosts_Create([DataSourceRequest]DataSourceRequest request, OverallCost overallCost, int parentId)
        {
            
            if (ModelState.IsValid)
            {
                var materialCost = db.MaterialCosts.Where(q => q.EstimationFormId == parentId).ToList().Sum(q => q.TotalPrice);
                var laborCost = db.LaborCosts.Where(q => q.EstimationFormId == parentId).ToList().Sum(q => q.TotalCost);
                var lab_mate_cost = materialCost + laborCost;
                var Tcost = ((overallCost.OverHeadCost) / 100) * lab_mate_cost + lab_mate_cost;
                var NBTCost = ((overallCost.MarginalCost) / 100) * Tcost + Tcost;
                var GTCost = ((overallCost.TInpercent) / 100) * NBTCost + NBTCost;
                var entity = new OverallCost
                {
                    OverHeadCost = overallCost.OverHeadCost,
                    EstimationFormId = parentId,
                    T = Tcost,
                    MarginalCost = overallCost.MarginalCost,
                    NBT = NBTCost,
                    TInpercent = overallCost.TInpercent,
                    GT = GTCost,
                    Graphic = overallCost.Graphic,
                    EstimatedNetPrice = GTCost+ overallCost.Graphic

                };

                db.OverallCosts.Add(entity);
                db.SaveChanges();
               
             

            }
            return Json(new[] { overallCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverallCosts_Update([DataSourceRequest]DataSourceRequest request, OverallCost overallCost)
        {
           
                
                if (ModelState.IsValid)
                {
                decimal materialCost = db.MaterialCosts.Where(q => q.EstimationFormId == overallCost.EstimationFormId).ToList().Sum(q => q.TotalPrice);
                decimal laborCost = db.LaborCosts.Where(q => q.EstimationFormId == overallCost.EstimationFormId).ToList().Sum(q => q.TotalCost);
                decimal lab_mate_cost = materialCost + laborCost;
                decimal Tcost = ((overallCost.OverHeadCost) / 100) * lab_mate_cost + lab_mate_cost;
                decimal NBTCost = ((overallCost.MarginalCost) / 100) * Tcost + Tcost;
                decimal GTCost = ((overallCost.TInpercent) / 100) * NBTCost + NBTCost;
                var entity = new OverallCost
                    {
                    OverallCostId = overallCost.OverallCostId,
                    EstimationFormId = overallCost.EstimationFormId,
                    OverHeadCost = overallCost.OverHeadCost,
                        T = Tcost,
                        MarginalCost = overallCost.MarginalCost,
                        NBT = NBTCost,
                        TInpercent = overallCost.TInpercent,
                        GT = GTCost,
                        Graphic = overallCost.Graphic,
                        EstimatedNetPrice = GTCost + overallCost.Graphic

                    };

                    db.OverallCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                
            }

            return Json(new[] { overallCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OverallCosts_Destroy([DataSourceRequest]DataSourceRequest request, OverallCost overallCost)
        {

            if (ModelState.IsValid)
            {
                var entity = new OverallCost
                {
                    OverallCostId = overallCost.OverallCostId,
                    OverHeadCost = overallCost.OverHeadCost,
                    T = overallCost.T,
                    MarginalCost = overallCost.MarginalCost,
                    TInpercent = overallCost.TInpercent,
                    GT = overallCost.GT,
                    Graphic = overallCost.Graphic,
                    EstimatedNetPrice = overallCost.EstimatedNetPrice

                };

                db.OverallCosts.Attach(entity);
                db.OverallCosts.Remove(entity);
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
