﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureProformaInvoiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();

            ViewData["JobType"] = db.FurnitureJobCategories.Select(s => new
            {
                Value = s.FurnitureJobCategoryId,
                Text = s.JobName
            }).ToList();

            return View();
        }

        public ActionResult FurnitureProformaInvoices_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureProformaInvoice> furnitureproformainvoices = db.FurnitureProformaInvoices;
            DataSourceResult result = furnitureproformainvoices.ToDataSourceResult(request, furnitureProformaInvoice => new FurnitureProformaInvoice
            {
                FurnitureProformaInvoiceId = furnitureProformaInvoice.FurnitureProformaInvoiceId,
                FurnitureEstimationId = furnitureProformaInvoice.FurnitureEstimationId,
                CustomerId = furnitureProformaInvoice.CustomerId,
                DeliveryPeriod = furnitureProformaInvoice.DeliveryPeriod,
                DeliveryDate = furnitureProformaInvoice.DeliveryDate,
                PriceValidityPeriod = furnitureProformaInvoice.PriceValidityPeriod,
                ModeOfPayment = furnitureProformaInvoice.ModeOfPayment,
                Remark = furnitureProformaInvoice.Remark,
                Status = furnitureProformaInvoice.Status,
                IsVoid = furnitureProformaInvoice.IsVoid,
                VoidBy = furnitureProformaInvoice.VoidBy,
                VoidTime = furnitureProformaInvoice.VoidTime,
                IsOnlineApproved = furnitureProformaInvoice.IsOnlineApproved,
                OnlineApprovedBy = furnitureProformaInvoice.OnlineApprovedBy,
                OnlineApprovedTime = furnitureProformaInvoice.OnlineApprovedTime,
                IsOnlineTransferred = furnitureProformaInvoice.IsOnlineTransferred,
                IsOnlineTransferredBy = furnitureProformaInvoice.IsOnlineTransferredBy,
                OnlineTransferredTime = furnitureProformaInvoice.OnlineTransferredTime,
                IsSendForApproval = furnitureProformaInvoice.IsSendForApproval,
                OrgBranchName = furnitureProformaInvoice.OrgBranchName,
                SendForApprovalBy = furnitureProformaInvoice.SendForApprovalBy,
                SendForApprovalTime = furnitureProformaInvoice.SendForApprovalTime,
                DateCreated = furnitureProformaInvoice.DateCreated,
                LastModified = furnitureProformaInvoice.LastModified,
                CreatedBy = furnitureProformaInvoice.CreatedBy,
                ModifiedBy = furnitureProformaInvoice.ModifiedBy,
                PaymentType = furnitureProformaInvoice.PaymentType,
                HaveEstimation = furnitureProformaInvoice.HaveEstimation,
                IsExistingCustomer = furnitureProformaInvoice.IsExistingCustomer
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoices_Create([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoice furnitureProformaInvoice)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProformaInvoice
                {
                    FurnitureEstimationId = furnitureProformaInvoice.FurnitureEstimationId,
                    CustomerId = furnitureProformaInvoice.CustomerId,
                    DeliveryPeriod = furnitureProformaInvoice.DeliveryPeriod,
                    DeliveryDate = furnitureProformaInvoice.DeliveryDate,
                    PriceValidityPeriod = furnitureProformaInvoice.PriceValidityPeriod,
                    ModeOfPayment = furnitureProformaInvoice.ModeOfPayment,
                    Remark = furnitureProformaInvoice.Remark,
                    
                    DateCreated = DateTime.Today,
                    
                    CreatedBy = User.Identity.Name,
                    
                };

                db.FurnitureProformaInvoices.Add(entity);
                db.SaveChanges();
                furnitureProformaInvoice.FurnitureProformaInvoiceId = entity.FurnitureProformaInvoiceId;
            }

            return Json(new[] { furnitureProformaInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoices_Update([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoice furnitureProformaInvoice)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProformaInvoice
                {
                    FurnitureProformaInvoiceId = furnitureProformaInvoice.FurnitureProformaInvoiceId,
                    FurnitureEstimationId = furnitureProformaInvoice.FurnitureEstimationId,
                    CustomerId = furnitureProformaInvoice.CustomerId,
                    DeliveryPeriod = furnitureProformaInvoice.DeliveryPeriod,
                    DeliveryDate = furnitureProformaInvoice.DeliveryDate,
                    PriceValidityPeriod = furnitureProformaInvoice.PriceValidityPeriod,
                    ModeOfPayment = furnitureProformaInvoice.ModeOfPayment,
                    Remark = furnitureProformaInvoice.Remark,
                    
                    DateCreated = furnitureProformaInvoice.DateCreated,
                    LastModified = DateTime.Today,
                    CreatedBy = furnitureProformaInvoice.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.FurnitureProformaInvoices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureProformaInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformaInvoices_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProformaInvoice furnitureProformaInvoice)
        {
            if (ModelState.IsValid)
            {
                var proformaInvoice = db.FurnitureProformaInvoices.Find(furnitureProformaInvoice.FurnitureProformaInvoiceId);

                var proformaItems = db.FurnitureProformaInvoiceItems.Where(x => x.FurnitureProformaInvoiceId == proformaInvoice.FurnitureProformaInvoiceId);

                db.FurnitureProformaInvoiceItems.RemoveRange(proformaItems);

                db.FurnitureProformaInvoices.Remove(proformaInvoice);

                db.SaveChanges();
            }

            return Json(new[] { furnitureProformaInvoice }.ToDataSourceResult(request, ModelState));
        }

        //public JsonResult ApproveProforma(int id)
        //{
        //    var jobOrder = db.FurnitureProformaInvoices
        //    var item = db
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
