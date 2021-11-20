﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Operation
{
    public class ProductDeliveriesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            ViewData["JobOrder"] = db.Jobs.ToList();
            ViewData["Units"] = db.Units.ToList();
            PopulateViewData();

            return View();
        }
     

        private void PopulateViewData()
        {

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;

            var branches = db.Branch;
            var costCenter = db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();
            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            ViewData["costCenters"] = selectedcostCenter;

            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["stores"] = stores;

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            //var workitem = db.ProjectEstimationWorkItem.Where(w => !string.IsNullOrEmpty(w.ItemCode));
            //var selectedWorkitem = workitem.Select(
            //    i => new
            //    {
            //        Name = i.ItemCode + "-" + i.ItemName,
            //        Code = i.ItemCode
            //    }).AsQueryable();
            //ViewData["WorkItem"] = selectedWorkitem;
        }

        public ActionResult ProductDeliveries_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductDelivery> productdeliveries = db.ProductDeliveries;
            DataSourceResult result = productdeliveries.ToDataSourceResult(request, productDelivery => new {
                ProductDeliveryId = productDelivery.ProductDeliveryId,
                CustomerId = productDelivery.CustomerId,
                CustomerRefNo = productDelivery.CustomerRefNo,
                CreditSaleNo = productDelivery.CreditSaleNo,
                CashSaleNo = productDelivery.CashSaleNo,
                PreparedBy = productDelivery.PreparedBy,
                ApprovedBy = productDelivery.ApprovedBy,
                IdentificationNo = productDelivery.IdentificationNo,
                JobId = productDelivery.JobId,
                IsVoid = productDelivery.IsVoid,
                VoidBy = productDelivery.VoidBy,
                VoidTime = productDelivery.VoidTime,
                IsOnlineApproved = productDelivery.IsOnlineApproved,
                OnlineApprovedBy = productDelivery.OnlineApprovedBy,
                OnlineApprovedTime = productDelivery.OnlineApprovedTime,
                IsSendForApproval = productDelivery.IsSendForApproval,
                SendForApprovalBy = productDelivery.SendForApprovalBy,
                SendForApprovalTime = productDelivery.SendForApprovalTime,
                IsOnlineTransferred = productDelivery.IsOnlineTransferred,
                IsOnlineTransferredBy = productDelivery.IsOnlineTransferredBy,
                OnlineTransferredTime = productDelivery.OnlineTransferredTime,

                DateCreated = productDelivery.DateCreated,
                CreatedBy = productDelivery.CreatedBy,
                LastModified = productDelivery.LastModified,
                ModifiedBy = productDelivery.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveries_Create([DataSourceRequest]DataSourceRequest request, ProductDelivery productDelivery)
        {
            if (ModelState.IsValid)
            {
                var finishedGood = db.Jobs.Find(productDelivery.JobId);
                var entity = new ProductDelivery
                {
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CustomerId = finishedGood.CustomerId,
                    CashSaleNo = productDelivery.CashSaleNo,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    IdentificationNo = productDelivery.IdentificationNo,
                    JobId = productDelivery.JobId
                };

                db.ProductDeliveries.Add(entity);
                db.SaveChanges();
                productDelivery.ProductDeliveryId = entity.ProductDeliveryId;
            }

            return Json(new[] { productDelivery }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveries_Update([DataSourceRequest]DataSourceRequest request, ProductDelivery productDelivery)
        {
            if (ModelState.IsValid)
            {
                var finishedGood = db.Jobs.Find(productDelivery.JobId);

                var entity = new ProductDelivery
                {
                    ProductDeliveryId = productDelivery.ProductDeliveryId,
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CustomerId = finishedGood.CustomerId,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CashSaleNo = productDelivery.CashSaleNo,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    IdentificationNo = productDelivery.IdentificationNo,
                    JobId = productDelivery.JobId
                };

                db.ProductDeliveries.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { productDelivery }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveries_Destroy([DataSourceRequest]DataSourceRequest request, ProductDelivery productDelivery)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductDelivery
                {
                    ProductDeliveryId = productDelivery.ProductDeliveryId,
                    CustomerId = productDelivery.CustomerId,
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CashSaleNo = productDelivery.CashSaleNo,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    IdentificationNo = productDelivery.IdentificationNo,
                    JobId = productDelivery.JobId
                };

                db.ProductDeliveries.Attach(entity);
                db.ProductDeliveries.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { productDelivery }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
