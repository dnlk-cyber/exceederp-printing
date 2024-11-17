using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureOverallCostsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult FurnitureOverallCosts_Read([DataSourceRequest]DataSourceRequest request, int estimationFormId)
        {

            IQueryable<FurnitureOverallCost> overallcosts = db.FurnitureOverallCosts.Where(q => q.FurnitureEstimationId == estimationFormId);

            var result = overallcosts.ToDataSourceResult(request, model => new FurnitureOverallCost
            {
                AdministrativeCost = model.AdministrativeCost,
                Estimation = model.Estimation,
                FurnitureEstimationId = model.FurnitureEstimationId,
                FurnitureOverallCostId = model.FurnitureOverallCostId,
                GrandTotalCost = model.GrandTotalCost,
                LabourCost = model.LabourCost,
                ManufacturingCost = model.ManufacturingCost,
                MaterialCost = model.MaterialCost,
                OverHeadCost = model.OverHeadCost,
                ProfitMargin = model.ProfitMargin,
                SellingPrice = model.SellingPrice                
            });

            return Json(result);
        }
        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureOverallCosts_Update([DataSourceRequest]DataSourceRequest request, FurnitureOverallCost overallCost)
        {

            //if (overallCost.OverHeadCost == 0 || overallCost.AdministrativeCost == 0)
            //    ModelState.AddModelError("", "Overhead.cost/Adm.Cost/Profit margin missing");
            if (ModelState.IsValid)
            {
                
                overallCost.OverHeadCost = overallCost.LabourCost * overallCost.OverHeadCost;
                overallCost.AdministrativeCost = (overallCost.MaterialCost + overallCost.LabourCost + overallCost.OverHeadCost) * overallCost.AdministrativeCost;
                overallCost.ManufacturingCost = overallCost.MaterialCost + overallCost.LabourCost + overallCost.OverHeadCost;

                overallCost.GrandTotalCost = overallCost.ManufacturingCost + overallCost.AdministrativeCost;
                //var profit = overallCost.GrandTotalCost * overallCost.ProfitMargin;
                //overallCost.FinalPriceIncludingProfit = overallCost.GrandTotalCost + profit;

                db.FurnitureOverallCosts.Attach(overallCost);
                db.Entry(overallCost).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { overallCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureOverallCosts_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureOverallCost overallCost)
        {

            if (ModelState.IsValid)
            {
                var furnitureOverAllCost = db.FurnitureOverallCosts.Find(overallCost.FurnitureOverallCostId);
                db.FurnitureOverallCosts.Attach(furnitureOverAllCost);
                db.FurnitureOverallCosts.Remove(furnitureOverAllCost);
                db.SaveChanges();

            }

            return Json(new[] { overallCost }.ToDataSourceResult(request, ModelState));
        }

        //------ajax method------//
        public JsonResult CalculateOverAllCost(int id)
        {
            var msg = new
            {
                success = "Calculation succeeded",
                error = "Failed to calculate make sure you have material and labour cost",
                exists = "Already calculated"
            };
            var estimation = db.FurnitureEstimationForms.Find(id);
            if (estimation != null)
            {
                var overAllCost = db.OverallCosts.Where(x => x.EstimationFormId == estimation.FurnitureEstimationId).ToList();
                if (overAllCost.Any()) return Json(msg.exists);
                var labourCost = db.DirectLaborCosts.Where(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId).ToList();
                if (labourCost.Any())
                {
                    var materialCost = db.FurnitureMaterialCosts.Where(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId).ToList();
                    if (materialCost.Any())
                    {
                        var totalLabourCost = labourCost.Sum(x => x.TotalCost);
                        var totalMaterialCost = materialCost.Sum(s => s.TotalPrice);
                        var entity = new FurnitureOverallCost
                        {
                            FurnitureEstimationId = estimation.FurnitureEstimationId,
                            LabourCost = totalLabourCost,
                            MaterialCost = totalMaterialCost,
                            OverHeadCost = 0,
                            ManufacturingCost = totalLabourCost + totalMaterialCost,
                            AdministrativeCost = 0,
                            GrandTotalCost = totalLabourCost + totalMaterialCost
                        };
                        db.FurnitureOverallCosts.Add(entity);
                        db.SaveChanges();
                        
                        return Json(msg.success, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(msg.error, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
