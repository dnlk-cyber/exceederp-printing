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
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpDeliveryUser)]
    public class FurnitureProductDeliveryItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductDeliveryItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureProductDeliveryItems> productdeliveryitems = db.FurnitureProductDeliveryItems.Where(x=>x.FurnitureProductDeliveryId == id);
            DataSourceResult result = productdeliveryitems.ToDataSourceResult(request, productDeliveryItems => new {
                ProductDeliveryItemsId = productDeliveryItems.FurnitureProductDeliveryItemsId,
                ProductDeliveryId = productDeliveryItems.FurnitureProductDeliveryId,
                JobId = productDeliveryItems.FurnitureJobId,
                ItemCode = productDeliveryItems.ItemCode,
                VAT = productDeliveryItems.VAT,
                Amount = productDeliveryItems.Amount,
                ItemCategory = productDeliveryItems.ItemCategory,
                Description = productDeliveryItems.Description,
                UnitId = productDeliveryItems.UnitId,
                Quantity = productDeliveryItems.Quantity,
                UnitCost = productDeliveryItems.UnitCost,
                TotalCost = productDeliveryItems.TotalCost
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductDeliveryItems productDeliveryItems, int id)
        {
            var job = db.FurnitureJobs.Find(productDeliveryItems?.FurnitureJobId);
            var deliveredJobs = db.FurnitureProductDeliveryItems.Where(q => q.FurnitureJobId == productDeliveryItems.FurnitureJobId).ToList();
            var totQuan = deliveredJobs.Sum(s => s.Quantity) + productDeliveryItems.Quantity;
            var changeOfOrders = db.FurnitureChangeOrders.Where(q => q.FurnitureJobId == productDeliveryItems.FurnitureJobId).ToList();
            var orderedQuant = job?.Quantity + changeOfOrders.Sum(s => s.Quantity);
            if ((decimal)totQuan > orderedQuant)
            {
                ModelState.AddModelError("Error", "The items sum must be equals to the ordered quantity");
            }
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductDeliveryItems
                {
                    FurnitureProductDeliveryId = id,
                    ItemCode = productDeliveryItems.ItemCode,
                    FurnitureJobId = productDeliveryItems.FurnitureJobId,
                    ItemCategory = productDeliveryItems.ItemCategory,
                    Description = productDeliveryItems.Description,
                    UnitId = productDeliveryItems.UnitId,
                    VAT = productDeliveryItems.VAT,
                    Amount = productDeliveryItems.Amount,
                    Quantity = productDeliveryItems.Quantity,
                    UnitCost = productDeliveryItems.UnitCost,
                    TotalCost = productDeliveryItems.TotalCost
                };

                db.FurnitureProductDeliveryItems.Add(entity);
                db.SaveChanges();
                if (job != null)
                {
                    var jobStatus = db.FurnitureJobStatuses.Where(q => q.FurnitureJobId == job.FurnitureJobId
                    && q.JobPhase == FurnitureProductionDepartment.SalesDelivery).FirstOrDefault();
                    if (jobStatus != null)
                    {
                        GetJobLineItems(entity,jobStatus.FurnitureJobStatusId, id);
                    }
                    else
                    {
                        var productDelivery = db.FurnitureProductDeliveries.Find(entity.FurnitureProductDeliveryId);
                        var jobStatusEntity = new FurnitureJobStatus
                        {
                            FurnitureJobStatusId = id,
                            FurnitureJobId = job.FurnitureJobId,
                            FurnitureUserId = User.Identity.Name,
                            Time = DateTime.Now,
                            StartDate = productDelivery.DeliveryDate,
                            EndDate = productDelivery.DeliveryDate,
                            JobPhase = FurnitureProductionDepartment.SalesDelivery,
                            Status = FurnitureJobState.ParitiallyFinished
                        };
                        db.FurnitureJobStatuses.Add(jobStatusEntity);
                        db.SaveChanges();
                        GetJobLineItems(entity,jobStatusEntity.FurnitureJobStatusId, id);

                    }
                }

                productDeliveryItems.FurnitureProductDeliveryItemsId = entity.FurnitureProductDeliveryItemsId;
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }

        private void GetJobLineItems(FurnitureProductDeliveryItems entity, int FurniturejobStatusId, int id)
        {
            var deliveryProduct = db.FurnitureProductDeliveries.Find(id);
            var jobStatusItemEntity = new FurnitureJobStatusLineItem
            {
                FurnitureJobStatusId = FurniturejobStatusId,
                Date = DateTime.Now,
                Quantiy = (decimal)entity.Quantity,
                TotalPrice = (decimal)entity.TotalCost,
                //Invoice = deliveryProduct.,

                SalesVoucherNo = deliveryProduct.CashSaleNo,
                Cash = deliveryProduct.Cash,
                Credit = deliveryProduct.Credit,
                CashCreditInvoice = deliveryProduct.CreditSaleNo
            };

            DataSourceRequest request = new DataSourceRequest();
            FurnitureJobStatusLineItemController jsIC = new FurnitureJobStatusLineItemController();
            jsIC.JobStatusLineItems_Create(request,jobStatusItemEntity, FurniturejobStatusId);
        }

      

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductDeliveryItems productDeliveryItems)
        {
            var job = db.FurnitureJobs.Find(productDeliveryItems?.FurnitureJobId);
            var deliveredJobs = db.FurnitureProductDeliveryItems.Where(q => q.FurnitureJobId == productDeliveryItems.FurnitureJobId
            && q.FurnitureProductDeliveryId != productDeliveryItems.FurnitureProductDeliveryId).ToList();
            var totQuan = deliveredJobs.Sum(s => s.Quantity) + productDeliveryItems.Quantity;
            var orderedQuant = job?.Quantity;
            if ((decimal)totQuan > orderedQuant)
            {
                ModelState.AddModelError("Error", "The items sum must be equals to the ordered quantity");
            }
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductDeliveryItems
                {
                    FurnitureProductDeliveryItemsId = productDeliveryItems.FurnitureProductDeliveryItemsId,
                    FurnitureProductDeliveryId = productDeliveryItems.FurnitureProductDeliveryId,
                    FurnitureJobId = productDeliveryItems.FurnitureJobId,
                    ItemCategory = productDeliveryItems.ItemCategory,
                    ItemCode = productDeliveryItems.ItemCode,
                    Description = productDeliveryItems.Description,
                    UnitId = productDeliveryItems.UnitId,
                    Quantity = productDeliveryItems.Quantity,
                    VAT = productDeliveryItems.VAT,
                    Amount = productDeliveryItems.Amount,
                    UnitCost = productDeliveryItems.UnitCost,
                    TotalCost = productDeliveryItems.TotalCost
                };
                db.SaveChanges();
                db.FurnitureProductDeliveryItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductDeliveryItems productDeliveryItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductDeliveryItems
                {
                    FurnitureProductDeliveryItemsId = productDeliveryItems.FurnitureProductDeliveryItemsId,
                    FurnitureProductDeliveryId = productDeliveryItems.FurnitureProductDeliveryId,
                    FurnitureJobId = productDeliveryItems.FurnitureJobId,
                    ItemCategory = productDeliveryItems.ItemCategory,
                    ItemCode = productDeliveryItems.ItemCode,
                    Description = productDeliveryItems.Description,
                    UnitId = productDeliveryItems.UnitId,
                    VAT = productDeliveryItems.VAT,
                    Amount = productDeliveryItems.Amount,
                    Quantity = productDeliveryItems.Quantity,
                    UnitCost = productDeliveryItems.UnitCost,
                    TotalCost = productDeliveryItems.TotalCost
                };

                db.FurnitureProductDeliveryItems.Attach(entity);
                db.FurnitureProductDeliveryItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
