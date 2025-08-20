using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class EstimationCostSummaryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EstimationCostSummaries_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EstimationCostSummary> estimationcostsummaries = db.EstimationCostSummaries.Where(x => x.EstimationSummaryId == id);
            DataSourceResult result = estimationcostsummaries.ToDataSourceResult(request, estimationCostSummary => new
            {
                EstimationCostSummaryId = estimationCostSummary.EstimationCostSummaryId,
                ProfitMargin = estimationCostSummary.ProfitMargin,
                FinalPriceIncludingProfit = estimationCostSummary.FinalPriceIncludingProfit,
                EstimationSummaryId = estimationCostSummary.EstimationSummaryId,
                CommulativeCost = estimationCostSummary.CommulativeCost,
                IsApprovedMargin = estimationCostSummary.IsApprovedMargin
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationCostSummaries_Update([DataSourceRequest]DataSourceRequest request, EstimationCostSummary model)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCostSummary
                {
                    CommulativeCost = model.CommulativeCost,
                    EstimationSummaryId = model.EstimationSummaryId,
                    EstimationCostSummaryId = model.EstimationCostSummaryId,
                    FinalPriceIncludingProfit = (model.CommulativeCost * model.ProfitMargin) + model.CommulativeCost,
                    ProfitMargin = model.ProfitMargin,
                    IsApprovedMargin = model.IsApprovedMargin
                };
                model.FinalPriceIncludingProfit = entity.FinalPriceIncludingProfit;
                db.EstimationCostSummaries.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult CalculateOverAllCost(int id)
        {
            var msg = new
            {
                success = "Calculation succeeded",
                error = "Failed to calculate make sure you have material and labour cost",
                exists = "Already calculated"
            };
            var estimationSummary = db.EstimationSummaries.FirstOrDefault(x => x.EstimationSummaryId == id);
            if (estimationSummary != null)
            {
                estimationSummary.IsCalculated = true;
                db.Entry(estimationSummary).State = EntityState.Modified;
                var estimationDetails = db.EstimationDetails.Where(x => x.EstimationSummaryId == estimationSummary.EstimationSummaryId);

                var entity = new EstimationCostSummary
                {
                    EstimationSummaryId = estimationSummary.EstimationSummaryId,
                    CommulativeCost = estimationDetails?.Sum(x => (decimal?)x.GrandTotalCost) ?? 0
                };
                db.EstimationCostSummaries.Add(entity);
                db.SaveChanges();
                return Json(msg.success, JsonRequestBehavior.AllowGet);
            }
            return Json(msg.error, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveMargin(int id)
        {

            object response = null;
            var overAllCost = db.EstimationCostSummaries.FirstOrDefault(x => x.EstimationCostSummaryId == id);

            if (overAllCost != null)
            {
                var estimationSummary = db.EstimationSummaries.FirstOrDefault(x => x.EstimationSummaryId == overAllCost.EstimationSummaryId);
                estimationSummary.IsMarginApproved = true; // to hide and show approve button
                overAllCost.IsApprovedMargin = true;

                db.Entry(overAllCost).State = EntityState.Modified;
                db.SaveChanges();
                ExcludeOtherMargin(overAllCost.EstimationSummaryId);
                AddMarginToSummaryDetail(overAllCost.EstimationSummaryId, overAllCost.ProfitMargin);
                response = new { Success = true, Message = "Margin approved successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Estimation not found!" };
            }

            return Json(response);
        }
        private void ExcludeOtherMargin(int id)
        {
            var overAllCosts = db.EstimationCostSummaries
                .Where(x => x.EstimationSummaryId == id && !x.IsApprovedMargin)
                .ToList();
            db.EstimationCostSummaries.RemoveRange(overAllCosts);
            db.SaveChanges();
        }
        private void AddMarginToSummaryDetail(int id, decimal margin)
        {
            var details = db.EstimationDetails.Where(x => x.EstimationSummaryId == id).ToList();
            details.ForEach(x => x.ProfitMargin = margin);
            
            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
