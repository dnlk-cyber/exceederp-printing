﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureBillOfQuantityController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobType"] = db.FurnitureJobCategories.Select(s => new
            {
                Value = s.FurnitureJobCategoryId,
                Text = s.JobName
            }).ToList();
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            return View();
        }

        public ActionResult FurnitureBillOfQuantities_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureBillOfQuantity> furniturebillofquantities = db.FurnitureBillOfQuantities;
            DataSourceResult result = furniturebillofquantities.ToDataSourceResult(request, furnitureBillOfQuantity => new {
                FurnitureBillOfQuantityId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                JobTypeId = furnitureBillOfQuantity.JobTypeId,
                CustomerId = furnitureBillOfQuantity.CustomerId,
                Quantity = furnitureBillOfQuantity.Quantity,
                Size = furnitureBillOfQuantity.Size,
                Status = furnitureBillOfQuantity.Status,
                Remark = furnitureBillOfQuantity.Remark,
                IsVoid = furnitureBillOfQuantity.IsVoid,
                VoidBy = furnitureBillOfQuantity.VoidBy,
                VoidTime = furnitureBillOfQuantity.VoidTime,
                IsOnlineApproved = furnitureBillOfQuantity.IsOnlineApproved,
                OnlineApprovedBy = furnitureBillOfQuantity.OnlineApprovedBy,
                OnlineApprovedTime = furnitureBillOfQuantity.OnlineApprovedTime,
                IsOnlineTransferred = furnitureBillOfQuantity.IsOnlineTransferred,
                IsOnlineTransferredBy = furnitureBillOfQuantity.IsOnlineTransferredBy,
                OnlineTransferredTime = furnitureBillOfQuantity.OnlineTransferredTime,
                IsSendForApproval = furnitureBillOfQuantity.IsSendForApproval,
                OrgBranchName = furnitureBillOfQuantity.OrgBranchName,
                SendForApprovalBy = furnitureBillOfQuantity.SendForApprovalBy,
                SendForApprovalTime = furnitureBillOfQuantity.SendForApprovalTime,
                DateCreated = furnitureBillOfQuantity.DateCreated,
                LastModified = furnitureBillOfQuantity.LastModified,
                CreatedBy = furnitureBillOfQuantity.CreatedBy,
                ModifiedBy = furnitureBillOfQuantity.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBillOfQuantities_Create([DataSourceRequest]DataSourceRequest request, FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBillOfQuantity
                {
                    JobTypeId = furnitureBillOfQuantity.JobTypeId,
                    CustomerId = furnitureBillOfQuantity.CustomerId,
                    Quantity = furnitureBillOfQuantity.Quantity,
                    Size = furnitureBillOfQuantity.Size,
                    Status = furnitureBillOfQuantity.Status,
                    Remark = furnitureBillOfQuantity.Remark,
                    IsVoid = furnitureBillOfQuantity.IsVoid,
                    VoidBy = furnitureBillOfQuantity.VoidBy,
                    VoidTime = furnitureBillOfQuantity.VoidTime,
                    IsOnlineApproved = furnitureBillOfQuantity.IsOnlineApproved,
                    OnlineApprovedBy = furnitureBillOfQuantity.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureBillOfQuantity.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureBillOfQuantity.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureBillOfQuantity.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureBillOfQuantity.OnlineTransferredTime,
                    IsSendForApproval = furnitureBillOfQuantity.IsSendForApproval,
                    OrgBranchName = furnitureBillOfQuantity.OrgBranchName,
                    SendForApprovalBy = furnitureBillOfQuantity.SendForApprovalBy,
                    SendForApprovalTime = furnitureBillOfQuantity.SendForApprovalTime,
                    DateCreated = furnitureBillOfQuantity.DateCreated,
                    LastModified = furnitureBillOfQuantity.LastModified,
                    CreatedBy = furnitureBillOfQuantity.CreatedBy,
                    ModifiedBy = furnitureBillOfQuantity.ModifiedBy
                };

                db.FurnitureBillOfQuantities.Add(entity);
                db.SaveChanges();
                furnitureBillOfQuantity.FurnitureBillOfQuantityId = entity.FurnitureBillOfQuantityId;
            }

            return Json(new[] { furnitureBillOfQuantity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBillOfQuantities_Update([DataSourceRequest]DataSourceRequest request, FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBillOfQuantity
                {
                    FurnitureBillOfQuantityId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                    JobTypeId = furnitureBillOfQuantity.JobTypeId,
                    CustomerId = furnitureBillOfQuantity.CustomerId,
                    Quantity = furnitureBillOfQuantity.Quantity,
                    Size = furnitureBillOfQuantity.Size,
                    Status = furnitureBillOfQuantity.Status,
                    Remark = furnitureBillOfQuantity.Remark,
                    IsVoid = furnitureBillOfQuantity.IsVoid,
                    VoidBy = furnitureBillOfQuantity.VoidBy,
                    VoidTime = furnitureBillOfQuantity.VoidTime,
                    IsOnlineApproved = furnitureBillOfQuantity.IsOnlineApproved,
                    OnlineApprovedBy = furnitureBillOfQuantity.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureBillOfQuantity.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureBillOfQuantity.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureBillOfQuantity.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureBillOfQuantity.OnlineTransferredTime,
                    IsSendForApproval = furnitureBillOfQuantity.IsSendForApproval,
                    OrgBranchName = furnitureBillOfQuantity.OrgBranchName,
                    SendForApprovalBy = furnitureBillOfQuantity.SendForApprovalBy,
                    SendForApprovalTime = furnitureBillOfQuantity.SendForApprovalTime,
                    DateCreated = furnitureBillOfQuantity.DateCreated,
                    LastModified = furnitureBillOfQuantity.LastModified,
                    CreatedBy = furnitureBillOfQuantity.CreatedBy,
                    ModifiedBy = furnitureBillOfQuantity.ModifiedBy
                };

                db.FurnitureBillOfQuantities.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureBillOfQuantity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBillOfQuantities_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBillOfQuantity
                {
                    FurnitureBillOfQuantityId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                    JobTypeId = furnitureBillOfQuantity.JobTypeId,
                    CustomerId = furnitureBillOfQuantity.CustomerId,
                    Quantity = furnitureBillOfQuantity.Quantity,
                    Size = furnitureBillOfQuantity.Size,
                    Status = furnitureBillOfQuantity.Status,
                    Remark = furnitureBillOfQuantity.Remark,
                    IsVoid = furnitureBillOfQuantity.IsVoid,
                    VoidBy = furnitureBillOfQuantity.VoidBy,
                    VoidTime = furnitureBillOfQuantity.VoidTime,
                    IsOnlineApproved = furnitureBillOfQuantity.IsOnlineApproved,
                    OnlineApprovedBy = furnitureBillOfQuantity.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureBillOfQuantity.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureBillOfQuantity.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureBillOfQuantity.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureBillOfQuantity.OnlineTransferredTime,
                    IsSendForApproval = furnitureBillOfQuantity.IsSendForApproval,
                    OrgBranchName = furnitureBillOfQuantity.OrgBranchName,
                    SendForApprovalBy = furnitureBillOfQuantity.SendForApprovalBy,
                    SendForApprovalTime = furnitureBillOfQuantity.SendForApprovalTime,
                    DateCreated = furnitureBillOfQuantity.DateCreated,
                    LastModified = furnitureBillOfQuantity.LastModified,
                    CreatedBy = furnitureBillOfQuantity.CreatedBy,
                    ModifiedBy = furnitureBillOfQuantity.ModifiedBy
                };

                db.FurnitureBillOfQuantities.Attach(entity);
                db.FurnitureBillOfQuantities.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureBillOfQuantity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
