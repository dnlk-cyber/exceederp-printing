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
            ViewData["JobType"] = db.PrintingJobTypes.Select(s => new
            {
                Value = s.PrintingJobTypeId,
                Text = s.JobTypeName
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

        public JsonResult ApproveEstimationSummary(int id)
        {
            Object response = null;
            var summary = db.PrintingEstimationSummaries.Find(id);
            var overAllCost = db.PrintingEstimationCostSummaries.Where(x => x.PrintingEstimationSummaryId == summary.PrintingEstimationSummaryId).ToList();
            if (overAllCost.Any())
            {
                summary.IsOnlineApproved = true;
                summary.OnlineApprovedBy = User.Identity.Name;
                summary.OnlineApprovedTime = DateTime.Today;
                summary.Status = "Approved";
                db.Entry(summary).State = EntityState.Modified;
                db.SaveChanges();
                AssignMarginToOVerallCost(summary.CustomerId, overAllCost.Single().ProfitMargin);
                response = new { Success = true, Message = "Successfully Approved!" };
            }
            else
            {
                response = new { Success = false, Message = "Margin not set" };

            }
            return Json(response);
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
            var overAllCost = db.PrintingEstimationCostSummaries.Where(x => x.PrintingEstimationSummaryId == summary.PrintingEstimationSummaryId).ToList();
            if (overAllCost.Any())
            {
                summary.IsOnlineApproved = false;
                summary.OnlineApprovedBy = User.Identity.Name;
                summary.OnlineApprovedTime = DateTime.Today;
                summary.Status = "Rejected";
                db.Entry(summary).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Approved!" };
            }
            else
            {
                response = new { Success = false, Message = "Margin not set" };

            }
            return Json(response);

        }

    }
}