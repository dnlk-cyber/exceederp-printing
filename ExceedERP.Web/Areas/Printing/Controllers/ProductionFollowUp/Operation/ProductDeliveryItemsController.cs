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
    public class ProductDeliveryItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductDeliveryItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductDeliveryItems> productdeliveryitems = db.ProductDeliveryItems.Where(x=>x.ProductDeliveryId == id);
            DataSourceResult result = productdeliveryitems.ToDataSourceResult(request, productDeliveryItems => new {
                ProductDeliveryItemsId = productDeliveryItems.ProductDeliveryItemsId,
                ProductDeliveryId = productDeliveryItems.ProductDeliveryId,
                JobId = productDeliveryItems.JobId,
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
        public ActionResult ProductDeliveryItems_Create([DataSourceRequest]DataSourceRequest request, ProductDeliveryItems productDeliveryItems, int id)
        {
            var job = db.Jobs.Find(productDeliveryItems?.JobId);
            var deliveredJobs = db.ProductDeliveryItems.Where(q => q.JobId == productDeliveryItems.JobId).ToList();
            var totQuan = deliveredJobs.Sum(s => s.Quantity) + productDeliveryItems.Quantity;
            var changeOfOrders = db.ChangeOrders.Where(q => q.JobId == productDeliveryItems.JobId).ToList();
            var orderedQuant = job?.Quantity + changeOfOrders.Sum(s => s.Quantity);
            if ((decimal)totQuan > orderedQuant)
            {
                ModelState.AddModelError("Error", "The items sum must be equals to the ordered quantity");
            }
            if (ModelState.IsValid)
            {
                var entity = new ProductDeliveryItems
                {
                    ProductDeliveryId = id,
                    ItemCode = productDeliveryItems.ItemCode,
                    JobId = productDeliveryItems.JobId,
                    ItemCategory = productDeliveryItems.ItemCategory,
                    Description = productDeliveryItems.Description,
                    UnitId = productDeliveryItems.UnitId,
                    VAT = productDeliveryItems.VAT,
                    Amount = productDeliveryItems.Amount,
                    Quantity = productDeliveryItems.Quantity,
                    UnitCost = productDeliveryItems.UnitCost,
                    TotalCost = productDeliveryItems.TotalCost
                };

                db.ProductDeliveryItems.Add(entity);
                db.SaveChanges();
                if (job != null)
                {
                    var jobStatus = db.JobStatuses.Where(q => q.JobId == job.JobId
                    && q.JobPhase == ProductionDepartment.SalesDelivery).FirstOrDefault();
                    if (jobStatus != null)
                    {
                        GetJobLineItems(entity,jobStatus.JobStatusId,id);
                    }
                    else
                    {
                        var productDelivery = db.ProductDeliveries.Find(entity.ProductDeliveryId);
                        var jobStatusEntity = new JobStatus
                        {
                            JobStatusId = id,
                            JobId = job.JobId,
                            UserId = User.Identity.Name,
                            Time = DateTime.Now,
                            StartDate = productDelivery.DeliveryDate,
                            EndDate = productDelivery.DeliveryDate,
                            JobPhase = ProductionDepartment.SalesDelivery,
                            Status = Core.Domain.printing.PrintingEstimation.Setting.JobState.ParitiallyFinished
                        };
                        db.JobStatuses.Add(jobStatusEntity);
                        db.SaveChanges();
                        GetJobLineItems(entity,jobStatusEntity.JobStatusId, id);

                    }
                }

                productDeliveryItems.ProductDeliveryItemsId = entity.ProductDeliveryItemsId;
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }

        private void GetJobLineItems(ProductDeliveryItems entity, int jobStatusId, int id)
        {
            var deliveryProduct = db.ProductDeliveries.Find(id);
            var jobStatusItemEntity = new JobStatusLineItem
            {
                JobStatusId = jobStatusId,
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
            JobStatusLineItemController jsIC = new JobStatusLineItemController();
            jsIC.JobStatusLineItems_Create(request,jobStatusItemEntity, jobStatusId   );
        }

      

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Update([DataSourceRequest]DataSourceRequest request, ProductDeliveryItems productDeliveryItems)
        {
            var job = db.Jobs.Find(productDeliveryItems?.JobId);
            var deliveredJobs = db.ProductDeliveryItems.Where(q => q.JobId == productDeliveryItems.JobId
            && q.ProductDeliveryId!= productDeliveryItems.ProductDeliveryId).ToList();
            var totQuan = deliveredJobs.Sum(s => s.Quantity) + productDeliveryItems.Quantity;
            var orderedQuant = job?.Quantity;
            if ((decimal)totQuan > orderedQuant)
            {
                ModelState.AddModelError("Error", "The items sum must be equals to the ordered quantity");
            }
            if (ModelState.IsValid)
            {
                var entity = new ProductDeliveryItems
                {
                    ProductDeliveryItemsId = productDeliveryItems.ProductDeliveryItemsId,
                    ProductDeliveryId = productDeliveryItems.ProductDeliveryId,
                    JobId = productDeliveryItems.JobId,
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
                db.ProductDeliveryItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { productDeliveryItems }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductDeliveryItems_Destroy([DataSourceRequest]DataSourceRequest request, ProductDeliveryItems productDeliveryItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductDeliveryItems
                {
                    ProductDeliveryItemsId = productDeliveryItems.ProductDeliveryItemsId,
                    ProductDeliveryId = productDeliveryItems.ProductDeliveryId,
                    JobId = productDeliveryItems.JobId,
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

                db.ProductDeliveryItems.Attach(entity);
                db.ProductDeliveryItems.Remove(entity);
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
