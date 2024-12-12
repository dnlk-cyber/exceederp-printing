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
    public class PrintingEstimationMarginController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingEstimationMargin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingEstimationMargin_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var margin = db.PrintingEstimationMargins.Where(x => x.PrintingEstimationDetailId == id);
            return Json(margin.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingEstimationMargin_Update([DataSourceRequest]DataSourceRequest request, PrintingEstimationMargin model)
        {
            if (ModelState.IsValid)
            {
                model.TotalPrice = (model.TotalCost * model.ProfitMargin) + model.TotalCost;
                db.PrintingEstimationMargins.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        #region Calculate
        public JsonResult CalculateOverAllCost(int id)
        {
            var msg = new
            {
                success = "Calculation succeeded",
                error = "Failed to calculate make sure you have material and labour cost",
                exists = "Already calculated"
            };
            var estimationDetail = db.PrintingEstimationDetails.FirstOrDefault(x => x.PrintingEstimationDetailId == id);
            if (estimationDetail != null)
            {
                estimationDetail.IsCalculated = true;
                db.Entry(estimationDetail).State = EntityState.Modified;
                var entity = new PrintingEstimationMargin
                {
                    PrintingEstimationDetailId = estimationDetail.PrintingEstimationDetailId,
                    TotalCost = estimationDetail.TotalCost
                };
                db.PrintingEstimationMargins.Add(entity);
                db.SaveChanges();
                return Json(msg.success, JsonRequestBehavior.AllowGet);
            }
            return Json(msg.error, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveMargin(int id)
        {
            object response = null;
            var margin = db.PrintingEstimationMargins.FirstOrDefault(x => x.PrintingEstimationMarginId == id);
            if (margin != null)
            {
                margin.IsApprovedMargin = true;
                db.Entry(margin).State = EntityState.Modified;
                db.SaveChanges();
                ExcludeOtherMargin(margin.PrintingEstimationDetailId);
                AssignMarginToDetail(margin.PrintingEstimationDetailId, margin);
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
            var overAllCosts = db.PrintingEstimationMargins
                .Where(x => x.PrintingEstimationDetailId == id && !x.IsApprovedMargin)
                .ToList();
            db.PrintingEstimationMargins.RemoveRange(overAllCosts);
            db.SaveChanges();
        }
        private void AssignMarginToDetail(int id, PrintingEstimationMargin model)
        {
            var estimationDetail = db.PrintingEstimationDetails.FirstOrDefault(x => x.PrintingEstimationDetailId == id);
            estimationDetail.ProfitMargin = model.ProfitMargin;
            estimationDetail.TotalPrice = (model.TotalCost * model.ProfitMargin) + model.TotalCost;
            db.PrintingEstimationDetails.Attach(estimationDetail);
            db.Entry(estimationDetail).State = EntityState.Modified;
            db.SaveChanges();
        }
        #endregion
    }
}