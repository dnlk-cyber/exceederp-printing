﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class EstimationSummaryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewDataMethods();
            return View();
        }
        private void ViewDataMethods()
        {
            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });
            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            var selecteditems = db.InventoryItems.Select(
               i => new
               {
                   Name = i.ItemCode + "_" + i.Name,
                   Code = i.ItemCode
               }).ToList();
            var categories = db.ItemCategorys.Select(
               i => new
               {
                   Name = i.ItemCategoryCode + "_" + i.Name,
                   Code = i.ItemCategoryCode
               }).ToList();
            ViewData["Categorys"] = categories;
            var myCustomers = db.OrganizationCustomers.Select(
                s => new {
                    Code = s.OrganizationCustomerID,
                    Name = s.TradeName
                }).ToList();
            ViewData["Customers"] = myCustomers;

            //IList<Inventory> inventories = db.Inventories.ToList();
            ViewData["Inventories"] = selecteditems;

            IList<FurnitureStep> FurnitureSteps = db.FurnitureSteps.ToList();
            ViewData["FurnitureSteps"] = FurnitureSteps;

            var materialCategories = db.ManufacturingMaterialCategories.Select(s => new
            {
                Text = s.ManufacturingCategory,
                Value = s.ManufacturingMaterialCategoryId
            }).ToList();
            ViewData["MaterialCategory"] = materialCategories;
            ViewData["MaterialItem"] = db.ManufacturingMaterialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
        }

        public ActionResult EstimationSummaries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EstimationSummary> estimationsummaries = db.EstimationSummaries;
            
            return Json(estimationsummaries.ToDataSourceResult(request));
        }
        public ActionResult EstimationSummaries_Update([DataSourceRequest]DataSourceRequest request, EstimationSummary model)
        {
            if (ModelState.IsValid)
            {
                
                db.EstimationSummaries.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult CheckEstimationSummary(int id)
        {
            Object response = null;
            var summary = db.EstimationSummaries.FirstOrDefault(x => x.EstimationSummaryId == id);
            var overAllCost = db.EstimationCostSummaries.Where(x => x.EstimationSummaryId == summary.EstimationSummaryId).ToList();
            if (overAllCost.Any())
            {
                summary.IsSendForApproval = true;
                summary.SendForApprovalBy = User.Identity.Name;
                summary.SendForApprovalTime = DateTime.Today;
                summary.Status = "pending approval";
                db.Entry(summary).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully sent!" };
            }
            else
            {
                response = new { Success = false, Message = "Margin not set" };

            }
            return Json(response);
        }
        public JsonResult ApproveEstimationSummary(int id)
        {
            Object response = null;
            var summary = db.EstimationSummaries.FirstOrDefault(x => x.EstimationSummaryId == id);
            var overAllCost = db.EstimationCostSummaries.Where(x => x.EstimationSummaryId == summary.EstimationSummaryId).ToList();
            if (overAllCost.Any())
            {
                if(!overAllCost.Exists(x => x.IsApprovedMargin))
                {
                    response = new { Success = false, Message = "Approve margin first!" };
                }
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
            var estimations = db.FurnitureEstimationForms.Where(x => x.CustomerId == customerId && !x.IsMarginAssigned && x.IsOnlineApproved).ToList();
            foreach (var estimation in estimations)
            {
                var OverAllCost = db.FurnitureOverallCosts.FirstOrDefault(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId);
                if(OverAllCost != null)
                {
                    estimation.IsMarginAssigned = true;
                    estimation.Status = "Approved";
                    db.Entry(estimation).State = EntityState.Modified;

                    OverAllCost.ProfitMargin = profitMargin;
                    OverAllCost.SellingPrice = (OverAllCost.GrandTotalCost * profitMargin) + OverAllCost.GrandTotalCost;
                    db.Entry(OverAllCost).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
           
        }
        public JsonResult RejectEstimationSummary(int id)
        {
            Object response = null;
            var summary = db.EstimationSummaries.Find(id);
            var overAllCost = db.EstimationCostSummaries.Where(x => x.EstimationSummaryId == summary.EstimationSummaryId).ToList();
            if (overAllCost.Any())
            {
                var details = db.EstimationDetails
                    .Where(x => x.EstimationSummaryId == summary.EstimationSummaryId)
                    .ToList();
                foreach (var item in details)
                {
                    ReverseEstimation(item.FurnitureEstimationId);
                    DeleteSummaryDetail(item);
                }
                summary.IsOnlineApproved = false;
                summary.IsSendForApproval = false;
                
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

        private void ReverseEstimation(int id)
        {
            var estimationForm = db.FurnitureEstimationForms
                .FirstOrDefault(x => x.FurnitureEstimationId == id);
            estimationForm.IsSendForApproval = false;
            estimationForm.IsOnlineApproved = false;
            
            db.Entry(estimationForm).State = EntityState.Modified;
            db.SaveChanges();
        }
        private void DeleteSummaryDetail(EstimationDetail model)
        {
            DeleteMargin(model.EstimationSummaryId);
            db.EstimationDetails.Remove(model);
            db.SaveChanges();
        }
        private void DeleteMargin(int id)
        {
            var margin = db.EstimationCostSummaries.FirstOrDefault(x => x.EstimationSummaryId == id);
            if (margin != null)
            {
                db.EstimationCostSummaries.Remove(margin);
                db.SaveChanges();
            }

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
