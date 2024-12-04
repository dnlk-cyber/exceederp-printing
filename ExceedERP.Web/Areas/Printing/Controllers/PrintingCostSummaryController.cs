using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingCostSummaryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingCostSummary
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingEstimationSummaries_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PrintingEstimationCostSummary> estimationCostsummaries = db.PrintingEstimationCostSummaries.Where(x => x.PrintingEstimationSummaryId == id);
            return Json(estimationCostsummaries.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationCostSummaries_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationCostSummary model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingEstimationCostSummary
                {
                    CommulativeCost = model.CommulativeCost,
                    PrintingEstimationSummaryId = model.PrintingEstimationSummaryId,
                    PrintingEstimationCostSummaryId = model.PrintingEstimationCostSummaryId,
                    FinalPriceIncludingProfit = (model.CommulativeCost * model.ProfitMargin) + model.CommulativeCost,
                    ProfitMargin = model.ProfitMargin,
                    IsApprovedMargin = model.IsApprovedMargin
                };
                model.FinalPriceIncludingProfit = entity.FinalPriceIncludingProfit;
                db.PrintingEstimationCostSummaries.Attach(entity);
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
            var estimationSummary = db.PrintingEstimationSummaries.FirstOrDefault(x => x.PrintingEstimationSummaryId == id);
            if (estimationSummary != null)
            {
                estimationSummary.IsCalculated = true;
                db.Entry(estimationSummary).State = EntityState.Modified;
                var estimationDetails = db.PrintingEstimationDetails.Where(x => x.PrintingEstimationSummaryId == estimationSummary.PrintingEstimationSummaryId);

                var entity = new PrintingEstimationCostSummary
                {
                    PrintingEstimationSummaryId = estimationSummary.PrintingEstimationSummaryId,
                    CommulativeCost = estimationDetails?.Sum(x => (decimal?)x.GrandTotalCost) ?? 0
                };
                db.PrintingEstimationCostSummaries.Add(entity);
                db.SaveChanges();
                return Json(msg.success, JsonRequestBehavior.AllowGet);
            }
            return Json(msg.error, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveMargin(int id)
        {

            object response = null;
            var overAllCost = db.PrintingEstimationCostSummaries.FirstOrDefault(x => x.PrintingEstimationCostSummaryId == id);

            if (overAllCost != null)
            {
                overAllCost.IsApprovedMargin = true;

                db.Entry(overAllCost).State = EntityState.Modified;
                db.SaveChanges();
                ExcludeOtherMargin(overAllCost.PrintingEstimationSummaryId);
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
            var overAllCosts = db.PrintingEstimationCostSummaries
                .Where(x => x.PrintingEstimationSummaryId == id && !x.IsApprovedMargin)
                .ToList();
            db.PrintingEstimationCostSummaries.RemoveRange(overAllCosts);
            db.SaveChanges();
        }
    }
}