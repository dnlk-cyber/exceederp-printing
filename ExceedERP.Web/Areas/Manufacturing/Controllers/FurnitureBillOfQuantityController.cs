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
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;

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

            ViewData["ItemCode"] = db.InventoryItems.Select(s => new
            {
                Value = s.ItemCode,
                Text = s.Name
            }).ToList();

            ViewData["MaterialCategory"] = db.ManufacturingMaterialCategories.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryId,
                Text = s.ManufacturingCategory
            }).ToList();

            ViewData["MaterialCategoryItem"] = db.ManufacturingMaterialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
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
                var labor = db.FurnitureBOQLabors.Where(x => x.FurnitureBillOfQuantityId == entity.FurnitureBillOfQuantityId);
                var material = db.FurnitureBOQMaterials.Where(x => x.FurnitureBillOfQuantityId == entity.FurnitureBillOfQuantityId);
                db.FurnitureBOQLabors.RemoveRange(labor);
                db.FurnitureBOQMaterials.RemoveRange(material);
                
                db.FurnitureBillOfQuantities.Attach(entity);
                db.FurnitureBillOfQuantities.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureBillOfQuantity }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult CheckBOQ(int id)
        {
            object response = null;
            var billOfQuantity = db.FurnitureBillOfQuantities.Find(id);
            if(billOfQuantity != null)
            {
                billOfQuantity.IsSendForApproval = true;
                billOfQuantity.SendForApprovalBy = User.Identity.Name;
                billOfQuantity.SendForApprovalTime = DateTime.Today;

                db.FurnitureBillOfQuantities.Attach(billOfQuantity);
                db.Entry(billOfQuantity).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Bill of Quantity Checked Successfully!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        public JsonResult ApproveBOQ(int id)
        {
            object response = null;
            var billOfQuantity = db.FurnitureBillOfQuantities.Find(id);
            if (billOfQuantity != null)
            {
                billOfQuantity.IsOnlineApproved = true;
                billOfQuantity.OnlineApprovedBy = User.Identity.Name;
                billOfQuantity.OnlineApprovedTime = DateTime.Today;

                db.FurnitureBillOfQuantities.Attach(billOfQuantity);
                db.Entry(billOfQuantity).State = EntityState.Modified;
                db.SaveChanges();
                //--Generate Estimation--//
                GenerateEstimation(billOfQuantity);
                response = new { Success = true, Message = "Bill of Quantity Approved!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        private void GenerateEstimation(FurnitureBillOfQuantity billOfQuantity)
        {
            var boqMaterial = db.FurnitureBOQMaterials.Where(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId).ToList();
            var boqLabor = db.FurnitureBOQLabors.Where(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId).ToList();
            if(boqMaterial.Any() && boqLabor.Any())
            {
                var estimation = new FurnitureEstimationForm
                {
                    JobTypeId = billOfQuantity.JobTypeId,
                    CustomerId = billOfQuantity.CustomerId,
                    Quantity = billOfQuantity.Quantity,
                    FurnitureBillOfQuantityId = billOfQuantity.FurnitureBillOfQuantityId,
                };
                db.FurnitureEstimationForms.Add(estimation);
                db.SaveChanges();

                foreach (var item in boqMaterial)
                {
                    var material = new FurnitureMaterialCost
                    {
                        TaskType = item.WorkshopType,
                        ManufacturingMaterialCategoryId = item.ManufacturingMaterialCategoryId,
                        ManufacturingMaterialCategoryItemId = item.ManufacturingMaterialCategoryItemId,
                        UnitOfMeasurment = item.UnitOfMeasurement,
                        Quantity = item.Quantity,
                        UnitPrice = 0,
                        TotalPrice = 0,
                        FurnitureEstimationId = estimation.FurnitureEstimationId
                    };
                    db.FurnitureMaterialCosts.Add(material);
                    db.SaveChanges();
                }
                foreach (var item in boqLabor)
                {
                    var labor = new DirectLaborCost
                    {
                        TaskType = item.WorkShopType,
                        WorkingTime = item.EstimatedMinute,
                        FurnitureEstimationId = estimation.FurnitureEstimationId
                    };
                    db.DirectLaborCosts.Add(labor);
                    db.SaveChanges();
                }
               
            }
            
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
