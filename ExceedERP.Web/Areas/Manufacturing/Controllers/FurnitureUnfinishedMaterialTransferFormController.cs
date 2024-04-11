﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureUnfinishedMaterialTransferFormController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FurnitureUnfinishedMaterialTransferForms_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureUnfinishedMaterialTransferForm> furnitureunfinishedmaterialtransferforms = db.FurnitureUnfinishedMaterialTransferForms.Where(x => x.FurnitureJobOrderProductionId == id);
            DataSourceResult result = furnitureunfinishedmaterialtransferforms.ToDataSourceResult(request, furnitureUnfinishedMaterialTransferForm => new FurnitureUnfinishedMaterialTransferForm
            {
                FurnitureUnfinishedMaterialTransferFormId = furnitureUnfinishedMaterialTransferForm.FurnitureUnfinishedMaterialTransferFormId,
                JobTypeId = furnitureUnfinishedMaterialTransferForm.JobTypeId,
                Quantity = furnitureUnfinishedMaterialTransferForm.Quantity,
                TransferDate = furnitureUnfinishedMaterialTransferForm.TransferDate,
                TransfererId = furnitureUnfinishedMaterialTransferForm.TransfererId,
                ReceiverId = furnitureUnfinishedMaterialTransferForm.ReceiverId,
                QualityGrade = furnitureUnfinishedMaterialTransferForm.QualityGrade,
                WorkProgressInPercentage = furnitureUnfinishedMaterialTransferForm.WorkProgressInPercentage,
                FurnitureJobOrderProductionId = furnitureUnfinishedMaterialTransferForm.FurnitureJobOrderProductionId,
                IsReceived = furnitureUnfinishedMaterialTransferForm.IsReceived,
                IsSent = furnitureUnfinishedMaterialTransferForm.IsSent
            });

            return Json(result);
        }
        public ActionResult FurnitureUnfinishedMaterialReceive_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureUnfinishedMaterialTransferForm> furnitureunfinishedmaterialtransferforms = db.FurnitureUnfinishedMaterialTransferForms.Where(x => x.FurnitureJobOrderProductionId == id && x.IsSent);
            DataSourceResult result = furnitureunfinishedmaterialtransferforms.ToDataSourceResult(request, furnitureUnfinishedMaterialTransferForm => new FurnitureUnfinishedMaterialTransferForm
            {
                FurnitureUnfinishedMaterialTransferFormId = furnitureUnfinishedMaterialTransferForm.FurnitureUnfinishedMaterialTransferFormId,
                JobTypeId = furnitureUnfinishedMaterialTransferForm.JobTypeId,
                Quantity = furnitureUnfinishedMaterialTransferForm.Quantity,
                TransferDate = furnitureUnfinishedMaterialTransferForm.TransferDate,
                TransfererId = furnitureUnfinishedMaterialTransferForm.TransfererId,
                ReceiverId = furnitureUnfinishedMaterialTransferForm.ReceiverId,
                QualityGrade = furnitureUnfinishedMaterialTransferForm.QualityGrade,
                WorkProgressInPercentage = furnitureUnfinishedMaterialTransferForm.WorkProgressInPercentage,
                FurnitureJobOrderProductionId = furnitureUnfinishedMaterialTransferForm.FurnitureJobOrderProductionId,
                IsReceived = furnitureUnfinishedMaterialTransferForm.IsReceived,
                IsSent = furnitureUnfinishedMaterialTransferForm.IsSent
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureUnfinishedMaterialTransferForms_Create([DataSourceRequest]DataSourceRequest request, FurnitureUnfinishedMaterialTransferForm furnitureUnfinishedMaterialTransferForm, int id)
        {
            var production = db.FurnitureJobOrderProductions.FirstOrDefault(x => x.FurnitureJobOrderProductionId == id);
            var transferItems = db.FurnitureUnfinishedMaterialTransferForms.Where(x => x.FurnitureJobOrderProductionId == production.FurnitureJobOrderProductionId).ToList();
            if (transferItems.Any())
            {
                var totalTransferItems = transferItems.Sum(s => s.Quantity);
                totalTransferItems = furnitureUnfinishedMaterialTransferForm.Quantity + totalTransferItems;
                if (totalTransferItems > production.Quantity)
                {
                    ModelState.AddModelError(string.Empty, "You have reached the maximum quantity that was ordered!");
                }
            }
            if(furnitureUnfinishedMaterialTransferForm.Quantity > production.Quantity)
            {
                ModelState.AddModelError(string.Empty, "You can not exceed the maximum quantity ordered");
            }
            
            if (ModelState.IsValid)
            {
                
                furnitureUnfinishedMaterialTransferForm.JobTypeId = production.JobTypeId;
                var entity = new FurnitureUnfinishedMaterialTransferForm
                {
                    JobTypeId = production.JobTypeId,
                    Quantity = furnitureUnfinishedMaterialTransferForm.Quantity,
                    TransferDate = DateTime.Today,
                    TransfererId = furnitureUnfinishedMaterialTransferForm.TransfererId,
                    ReceiverId = furnitureUnfinishedMaterialTransferForm.ReceiverId,
                    QualityGrade = furnitureUnfinishedMaterialTransferForm.QualityGrade,
                    WorkProgressInPercentage = furnitureUnfinishedMaterialTransferForm.WorkProgressInPercentage,
                    FurnitureJobOrderProductionId = id,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    
                    
                };

                db.FurnitureUnfinishedMaterialTransferForms.Add(entity);
                db.SaveChanges();
                furnitureUnfinishedMaterialTransferForm.FurnitureUnfinishedMaterialTransferFormId = entity.FurnitureUnfinishedMaterialTransferFormId;
            }

            return Json(new[] { furnitureUnfinishedMaterialTransferForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureUnfinishedMaterialTransferForms_Update([DataSourceRequest]DataSourceRequest request, FurnitureUnfinishedMaterialTransferForm furnitureUnfinishedMaterialTransferForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureUnfinishedMaterialTransferForm
                {
                    FurnitureUnfinishedMaterialTransferFormId = furnitureUnfinishedMaterialTransferForm.FurnitureUnfinishedMaterialTransferFormId,
                    JobTypeId = furnitureUnfinishedMaterialTransferForm.JobTypeId,
                    Quantity = furnitureUnfinishedMaterialTransferForm.Quantity,
                    TransferDate = furnitureUnfinishedMaterialTransferForm.TransferDate,
                    TransfererId = furnitureUnfinishedMaterialTransferForm.TransfererId,
                    ReceiverId = furnitureUnfinishedMaterialTransferForm.ReceiverId,
                    QualityGrade = furnitureUnfinishedMaterialTransferForm.QualityGrade,
                    WorkProgressInPercentage = furnitureUnfinishedMaterialTransferForm.WorkProgressInPercentage,
                    FurnitureJobOrderProductionId = furnitureUnfinishedMaterialTransferForm.FurnitureJobOrderProductionId,
                    DateCreated = furnitureUnfinishedMaterialTransferForm.DateCreated,
                    CreatedBy = furnitureUnfinishedMaterialTransferForm.CreatedBy,
                    LastModified = DateTime.Today,
                    ModifiedBy = User.Identity.Name
                };

                db.FurnitureUnfinishedMaterialTransferForms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureUnfinishedMaterialTransferForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureUnfinishedMaterialTransferForms_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureUnfinishedMaterialTransferForm furnitureUnfinishedMaterialTransferForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureUnfinishedMaterialTransferForm
                {
                    FurnitureUnfinishedMaterialTransferFormId = furnitureUnfinishedMaterialTransferForm.FurnitureUnfinishedMaterialTransferFormId,
                    JobTypeId = furnitureUnfinishedMaterialTransferForm.JobTypeId,
                    Quantity = furnitureUnfinishedMaterialTransferForm.Quantity,
                    TransferDate = furnitureUnfinishedMaterialTransferForm.TransferDate,
                    TransfererId = furnitureUnfinishedMaterialTransferForm.TransfererId,
                    ReceiverId = furnitureUnfinishedMaterialTransferForm.ReceiverId,
                    QualityGrade = furnitureUnfinishedMaterialTransferForm.QualityGrade,
                    WorkProgressInPercentage = furnitureUnfinishedMaterialTransferForm.WorkProgressInPercentage,
                    FurnitureJobOrderProductionId = furnitureUnfinishedMaterialTransferForm.FurnitureJobOrderProductionId,
                    ModifiedBy = furnitureUnfinishedMaterialTransferForm.ModifiedBy,
                    LastModified = furnitureUnfinishedMaterialTransferForm.LastModified,
                    CreatedBy = furnitureUnfinishedMaterialTransferForm.CreatedBy,
                    DateCreated = furnitureUnfinishedMaterialTransferForm.DateCreated,
                    IsReceived = furnitureUnfinishedMaterialTransferForm.IsReceived
                };

                db.FurnitureUnfinishedMaterialTransferForms.Attach(entity);
                db.FurnitureUnfinishedMaterialTransferForms.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureUnfinishedMaterialTransferForm }.ToDataSourceResult(request, ModelState));
        }


        public JsonResult ReceiveUnfinishedGood(int id)
        {
            Object response = null;
            var unfinishedGoods = db.FurnitureUnfinishedMaterialTransferForms.Find(id);
            if (unfinishedGoods != null)
            {
                unfinishedGoods.IsReceived = true;
                unfinishedGoods.ReceivedBy = User.Identity.Name;
                unfinishedGoods.ReceivedDate = DateTime.Today;
                db.Entry(unfinishedGoods).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Received" };
            }
            else
            {
                response = new { Success = false, Message = "Something's Wrong!" };
            }
            return Json(response);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
