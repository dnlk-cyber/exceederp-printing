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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpDeliveryUser)]
    public class ProductDeliveriesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            ViewData["JobOrder"] = db.Jobs.ToList().Select(s=>
            new {
                JobId =s.JobId,
                JobNo = s.JobNo+"-"+ db.JobCategories.Find(s.JobTypeId).JobName
            }).ToList();
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
                Credit = productDelivery.Credit,
                CashSaleNo = productDelivery.CashSaleNo,
                Cash = productDelivery.Cash,
                DeliveryStatus = productDelivery.DeliveryStatus,
                DeliveryDate = productDelivery.DeliveryDate,
                PreparedBy = productDelivery.PreparedBy,
                ApprovedBy = productDelivery.ApprovedBy,
                IdentificationNo = productDelivery.IdentificationNo,
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
                var entity = new ProductDelivery
                {
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CustomerId = productDelivery.CustomerId,
                    Credit = productDelivery.Credit,
                    CashSaleNo = productDelivery.CashSaleNo,
                    DeliveryDate = productDelivery.DeliveryDate,
                    Cash = productDelivery.Cash,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    DateCreated = DateTime.Now,
                    IdentificationNo = productDelivery.IdentificationNo,
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

                var entity = new ProductDelivery
                {
                    ProductDeliveryId = productDelivery.ProductDeliveryId,
                    CustomerRefNo = productDelivery.CustomerRefNo,
                    CreditSaleNo = productDelivery.CreditSaleNo,
                    CustomerId = productDelivery.CustomerId,
                    Credit = productDelivery.Credit,
                    CashSaleNo = productDelivery.CashSaleNo,
                    Cash = productDelivery.Cash,
                    DeliveryDate = productDelivery.DeliveryDate,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    LastModified = DateTime.Now,
                    IdentificationNo = productDelivery.IdentificationNo,
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
                    Credit = productDelivery.Credit,
                    CashSaleNo = productDelivery.CashSaleNo,
                    Cash = productDelivery.Cash,
                    DeliveryDate = productDelivery.DeliveryDate,
                    PreparedBy = productDelivery.PreparedBy,
                    ApprovedBy = productDelivery.ApprovedBy,
                    IdentificationNo = productDelivery.IdentificationNo,
                };

                db.ProductDeliveries.Attach(entity);
                db.ProductDeliveries.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { productDelivery }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult VoidDelivery(int id)
        {
            var delivery = db.ProductDeliveries.Find(id);
            if (delivery != null)
            {
                delivery.DeliveryStatus = DeliveryStatus.Voided;
                db.ProductDeliveries.Attach(delivery);
                db.Entry(delivery).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Delivery has been voided");

            }
            return Json("Delivery is not found");


        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
