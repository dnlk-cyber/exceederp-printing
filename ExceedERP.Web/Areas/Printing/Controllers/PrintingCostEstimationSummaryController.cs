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
    public class PrintingCostEstimationSummaryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingCostEstimationSummary
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();

            ViewData["Category"] = db.PrintingMaterialCategories.Select(s => new
            {
                Value = s.PrintingMaterialCategoryId,
                Text = s.CategoryName
            }).ToList();
            ViewData["Item"] = db.PrintingMaterialCategoryItems.Select(s => new
            {
                Value = s.PrintingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
            return View();
        }

        public ActionResult PrintingEstimationSummaries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingEstimationSummary> estimationsummaries = db.PrintingEstimationSummaries;
            return Json(estimationsummaries.ToDataSourceResult(request));
        }
        public ActionResult PrintingEstimationDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var detail = db.PrintingEstimationDetails.Where(x => x.PrintingEstimationSummaryId == id);
            return Json(detail.ToDataSourceResult(request));
        }

        public JsonResult CheckEstSummary(int id)
        {
            Object response = null;
            var estSummary = db.PrintingEstimationSummaries.Find(id);
            if (estSummary != null)
            {

                var details = db.PrintingEstimationDetails.Where(x => x.PrintingEstimationSummaryId == estSummary.PrintingEstimationSummaryId);
                foreach (var item in details)
                {

                    if (item.ProfitMargin == 0)
                    {
                        response = new { Success = true, Message = "Profit Margin Missing!" };
                        return Json(response);
                    }
                }

                estSummary.IsSendForApproval = true;
                estSummary.SendForApprovalBy = User.Identity.Name;
                estSummary.SendForApprovalTime = DateTime.Today;
                estSummary.Status = "Checked";
                db.Entry(estSummary).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Checked" };

            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!" };
            }
            return Json(response);
        }
        public JsonResult ApproveEstimationSummary(int id)
        {
            Object response = null;
            var summary = db.PrintingEstimationSummaries.Find(id);
            var estimationDetails = db.PrintingEstimationDetails
                .Where(x => x.PrintingEstimationSummaryId == summary.PrintingEstimationSummaryId && x.ProfitMargin != 0)
                .ToList();

            var overAllCost = db.PrintingEstimationCostSummaries
                .Where(x => x.PrintingEstimationSummaryId == summary.PrintingEstimationSummaryId)
                .ToList();
            if (estimationDetails.Any())
            {
                summary.IsOnlineApproved = true;
                summary.OnlineApprovedBy = User.Identity.Name;
                summary.OnlineApprovedTime = DateTime.Today;
                summary.Status = "Approved";
                db.Entry(summary).State = EntityState.Modified;
                db.SaveChanges();
                AssignMarginToEstimation(estimationDetails);
                response = new { Success = true, Message = "Successfully Approved!" };
            }
            else
            {
                response = new { Success = false, Message = "Margin not set" };

            }
            return Json(response);
        }
        private void AssignMarginToEstimation(List<PrintingEstimationDetail> models)
        {
            foreach (var item in models)
            {
                var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == item.PrintingCostEstimationId);
                if (estimation != null)
                {
                    var OverAllCost = db.PrintingOverAllCosts.FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId);
                    estimation.IsMarginAssigned = true;
                    estimation.Status = "Approved";
                    db.Entry(estimation).State = EntityState.Modified;

                    OverAllCost.ProfitMargin = item.ProfitMargin;
                    OverAllCost.SellingPrice = item.TotalPrice;
                    db.Entry(OverAllCost).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
        private void AssignMarginToOVerallCost(int customerId, decimal profitMargin)
        {
            var estimations = db.PrintingCostEstimations.Where(x => x.CustomerId == customerId && !x.IsMarginAssigned && x.IsOnlineApproved).ToList();
            foreach (var estimation in estimations)
            {
                var OverAllCost = db.PrintingOverAllCosts.FirstOrDefault(x => x.PrintingCostEstimationId == estimation.PrintingCostEstimationId);
                if (OverAllCost != null)
                {
                    estimation.IsMarginAssigned = true;
                    estimation.Status = "Approved";
                    db.Entry(estimation).State = EntityState.Modified;

                    OverAllCost.ProfitMargin = profitMargin;
                    OverAllCost.SellingPrice = (OverAllCost.GrandTotal * profitMargin) + OverAllCost.GrandTotal;
                    db.Entry(OverAllCost).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public JsonResult RejectEstimationSummary(int id)
        {
            Object response = null;
            var summary = db.PrintingEstimationSummaries.Find(id);

            summary.IsOnlineApproved = false;
            summary.IsSendForApproval = false;
            var details = db.PrintingEstimationDetails.Where(x => x.PrintingEstimationSummaryId == summary.PrintingEstimationSummaryId).ToList();
            foreach (var item in details)
            {
                ReverseEstimation(item.PrintingCostEstimationId);
            }
            
            summary.Status = "Rejected";
            db.Entry(summary).State = EntityState.Modified;
            db.SaveChanges();
            response = new { Success = true, Message = "Successfully Rejected!" };

            return Json(response);

        }
        private void ReverseEstimation(int id)
        {
            var estimationForm = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == id);
            estimationForm.IsOnlineTransferred = false;
            estimationForm.IsOnlineApproved = false;
            
            db.Entry(estimationForm).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}