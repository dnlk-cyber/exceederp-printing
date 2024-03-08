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
    public class FurnitureJobOrderFormController : Controller
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

        public ActionResult FurnitureJobOrderForms_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderForm> furniturejoborderforms = db.FurnitureJobOrderForms;
            DataSourceResult result = furniturejoborderforms.ToDataSourceResult(request, furnitureJobOrderForm => new {
                FurnitureJobOrderFormId = furnitureJobOrderForm.FurnitureJobOrderFormId,
                CustomerId = furnitureJobOrderForm.CustomerId,
                JobTypeId = furnitureJobOrderForm.JobTypeId,
                UnitOfMeasurment = furnitureJobOrderForm.UnitOfMeasurment,
                Quantity = furnitureJobOrderForm.Quantity,
                UnitPriceBeforeVAT = furnitureJobOrderForm.UnitPriceBeforeVAT,
                UnitPriceWithVAT = furnitureJobOrderForm.UnitPriceWithVAT,
                DeliveryDate = furnitureJobOrderForm.DeliveryDate,
                CRVNo = furnitureJobOrderForm.CRVNo,
                AdvancePaymentInPercent = furnitureJobOrderForm.AdvancePaymentInPercent,
                FurnitureProformaInvoiceId = furnitureJobOrderForm.FurnitureProformaInvoiceId,
                Status = furnitureJobOrderForm.Status,
                Remark = furnitureJobOrderForm.Remark,
                IsVoid = furnitureJobOrderForm.IsVoid,
                VoidBy = furnitureJobOrderForm.VoidBy,
                VoidTime = furnitureJobOrderForm.VoidTime,
                IsOnlineApproved = furnitureJobOrderForm.IsOnlineApproved,
                OnlineApprovedBy = furnitureJobOrderForm.OnlineApprovedBy,
                OnlineApprovedTime = furnitureJobOrderForm.OnlineApprovedTime,
                IsOnlineTransferred = furnitureJobOrderForm.IsOnlineTransferred,
                IsOnlineTransferredBy = furnitureJobOrderForm.IsOnlineTransferredBy,
                OnlineTransferredTime = furnitureJobOrderForm.OnlineTransferredTime,
                IsSendForApproval = furnitureJobOrderForm.IsSendForApproval,
                OrgBranchName = furnitureJobOrderForm.OrgBranchName,
                SendForApprovalBy = furnitureJobOrderForm.SendForApprovalBy,
                SendForApprovalTime = furnitureJobOrderForm.SendForApprovalTime,
                DateCreated = furnitureJobOrderForm.DateCreated,
                LastModified = furnitureJobOrderForm.LastModified,
                CreatedBy = furnitureJobOrderForm.CreatedBy,
                ModifiedBy = furnitureJobOrderForm.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderForms_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderForm furnitureJobOrderForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderForm
                {
                    CustomerId = furnitureJobOrderForm.CustomerId,
                    JobTypeId = furnitureJobOrderForm.JobTypeId,
                    UnitOfMeasurment = furnitureJobOrderForm.UnitOfMeasurment,
                    Quantity = furnitureJobOrderForm.Quantity,
                    UnitPriceBeforeVAT = furnitureJobOrderForm.UnitPriceBeforeVAT,
                    UnitPriceWithVAT = furnitureJobOrderForm.UnitPriceWithVAT,
                    DeliveryDate = furnitureJobOrderForm.DeliveryDate,
                    CRVNo = furnitureJobOrderForm.CRVNo,
                    AdvancePaymentInPercent = furnitureJobOrderForm.AdvancePaymentInPercent,
                    FurnitureProformaInvoiceId = furnitureJobOrderForm.FurnitureProformaInvoiceId,
                    Status = furnitureJobOrderForm.Status,
                    Remark = furnitureJobOrderForm.Remark,
                    IsVoid = furnitureJobOrderForm.IsVoid,
                    VoidBy = furnitureJobOrderForm.VoidBy,
                    VoidTime = furnitureJobOrderForm.VoidTime,
                    IsOnlineApproved = furnitureJobOrderForm.IsOnlineApproved,
                    OnlineApprovedBy = furnitureJobOrderForm.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureJobOrderForm.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureJobOrderForm.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureJobOrderForm.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureJobOrderForm.OnlineTransferredTime,
                    IsSendForApproval = furnitureJobOrderForm.IsSendForApproval,
                    OrgBranchName = furnitureJobOrderForm.OrgBranchName,
                    SendForApprovalBy = furnitureJobOrderForm.SendForApprovalBy,
                    SendForApprovalTime = furnitureJobOrderForm.SendForApprovalTime,
                    DateCreated = furnitureJobOrderForm.DateCreated,
                    LastModified = furnitureJobOrderForm.LastModified,
                    CreatedBy = furnitureJobOrderForm.CreatedBy,
                    ModifiedBy = furnitureJobOrderForm.ModifiedBy
                };

                db.FurnitureJobOrderForms.Add(entity);
                db.SaveChanges();
                furnitureJobOrderForm.FurnitureJobOrderFormId = entity.FurnitureJobOrderFormId;
            }

            return Json(new[] { furnitureJobOrderForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderForms_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderForm furnitureJobOrderForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderForm
                {
                    FurnitureJobOrderFormId = furnitureJobOrderForm.FurnitureJobOrderFormId,
                    CustomerId = furnitureJobOrderForm.CustomerId,
                    JobTypeId = furnitureJobOrderForm.JobTypeId,
                    UnitOfMeasurment = furnitureJobOrderForm.UnitOfMeasurment,
                    Quantity = furnitureJobOrderForm.Quantity,
                    UnitPriceBeforeVAT = furnitureJobOrderForm.UnitPriceBeforeVAT,
                    UnitPriceWithVAT = furnitureJobOrderForm.UnitPriceWithVAT,
                    DeliveryDate = furnitureJobOrderForm.DeliveryDate,
                    CRVNo = furnitureJobOrderForm.CRVNo,
                    AdvancePaymentInPercent = furnitureJobOrderForm.AdvancePaymentInPercent,
                    FurnitureProformaInvoiceId = furnitureJobOrderForm.FurnitureProformaInvoiceId,
                    Status = furnitureJobOrderForm.Status,
                    Remark = furnitureJobOrderForm.Remark,
                    IsVoid = furnitureJobOrderForm.IsVoid,
                    VoidBy = furnitureJobOrderForm.VoidBy,
                    VoidTime = furnitureJobOrderForm.VoidTime,
                    IsOnlineApproved = furnitureJobOrderForm.IsOnlineApproved,
                    OnlineApprovedBy = furnitureJobOrderForm.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureJobOrderForm.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureJobOrderForm.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureJobOrderForm.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureJobOrderForm.OnlineTransferredTime,
                    IsSendForApproval = furnitureJobOrderForm.IsSendForApproval,
                    OrgBranchName = furnitureJobOrderForm.OrgBranchName,
                    SendForApprovalBy = furnitureJobOrderForm.SendForApprovalBy,
                    SendForApprovalTime = furnitureJobOrderForm.SendForApprovalTime,
                    DateCreated = furnitureJobOrderForm.DateCreated,
                    LastModified = furnitureJobOrderForm.LastModified,
                    CreatedBy = furnitureJobOrderForm.CreatedBy,
                    ModifiedBy = furnitureJobOrderForm.ModifiedBy
                };

                db.FurnitureJobOrderForms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderForms_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderForm furnitureJobOrderForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderForm
                {
                    FurnitureJobOrderFormId = furnitureJobOrderForm.FurnitureJobOrderFormId,
                    CustomerId = furnitureJobOrderForm.CustomerId,
                    JobTypeId = furnitureJobOrderForm.JobTypeId,
                    UnitOfMeasurment = furnitureJobOrderForm.UnitOfMeasurment,
                    Quantity = furnitureJobOrderForm.Quantity,
                    UnitPriceBeforeVAT = furnitureJobOrderForm.UnitPriceBeforeVAT,
                    UnitPriceWithVAT = furnitureJobOrderForm.UnitPriceWithVAT,
                    DeliveryDate = furnitureJobOrderForm.DeliveryDate,
                    CRVNo = furnitureJobOrderForm.CRVNo,
                    AdvancePaymentInPercent = furnitureJobOrderForm.AdvancePaymentInPercent,
                    FurnitureProformaInvoiceId = furnitureJobOrderForm.FurnitureProformaInvoiceId,
                    Status = furnitureJobOrderForm.Status,
                    Remark = furnitureJobOrderForm.Remark,
                    IsVoid = furnitureJobOrderForm.IsVoid,
                    VoidBy = furnitureJobOrderForm.VoidBy,
                    VoidTime = furnitureJobOrderForm.VoidTime,
                    IsOnlineApproved = furnitureJobOrderForm.IsOnlineApproved,
                    OnlineApprovedBy = furnitureJobOrderForm.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureJobOrderForm.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureJobOrderForm.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureJobOrderForm.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureJobOrderForm.OnlineTransferredTime,
                    IsSendForApproval = furnitureJobOrderForm.IsSendForApproval,
                    OrgBranchName = furnitureJobOrderForm.OrgBranchName,
                    SendForApprovalBy = furnitureJobOrderForm.SendForApprovalBy,
                    SendForApprovalTime = furnitureJobOrderForm.SendForApprovalTime,
                    DateCreated = furnitureJobOrderForm.DateCreated,
                    LastModified = furnitureJobOrderForm.LastModified,
                    CreatedBy = furnitureJobOrderForm.CreatedBy,
                    ModifiedBy = furnitureJobOrderForm.ModifiedBy
                };

                db.FurnitureJobOrderForms.Attach(entity);
                db.FurnitureJobOrderForms.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderForm }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
