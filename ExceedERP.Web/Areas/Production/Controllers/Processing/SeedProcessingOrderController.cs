using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Processing
{
    public class SeedProcessingOrderController : Controller
    {
        ExceedDbContext db;

        public SeedProcessingOrderController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            //var stages = db.ProductionSettings.Where(s => s.Type == ProductionSettingType.SeedProductionStage || s.Type == ProductionSettingType.NaturalGumProductionStage).ToList();
            //ViewData["LandStatus"] = stages;

            ViewData["Branches"] = db.Branch.ToList();

            return View();
        }

        public ActionResult ProcessingItems_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SeedProcessingOrder> items = db.SeedProcessingOrders;
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                SeedProcessingOrderId = item.SeedProcessingOrderId,
                ProcessingPlantId = item.ProcessingPlantId,
                ProductionPeriodId = item.ProductionPeriodId,
                

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,


            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Create([DataSourceRequest]DataSourceRequest request, SeedProcessingOrder item)
        {
            if (ModelState.IsValid)
            {
                var entity = new SeedProcessingOrder
                {
                    SeedProcessingOrderId = item.SeedProcessingOrderId,
                    ProcessingPlantId = item.ProcessingPlantId,
                    ProductionPeriodId = item.ProductionPeriodId,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy,
                };

                db.SeedProcessingOrders.Add(entity);
                db.SaveChanges();
                item.SeedProcessingOrderId = entity.SeedProcessingOrderId;

                this.InsertProcessingOrderFromIssues(entity);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Update([DataSourceRequest]DataSourceRequest request, SeedProcessingOrder item)
        {
            if (ModelState.IsValid)
            {
                var entity = new SeedProcessingOrder
                {
                    SeedProcessingOrderId = item.SeedProcessingOrderId,
                    ProcessingPlantId = item.ProcessingPlantId,
                    ProductionPeriodId = item.ProductionPeriodId,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.SeedProcessingOrders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Destroy([DataSourceRequest]DataSourceRequest request, SeedProcessingOrder item)
        {
            if (ModelState.IsValid)
            {
                var entity = new SeedProcessingOrder
                {
                    SeedProcessingOrderId = item.SeedProcessingOrderId,
                    ProcessingPlantId = item.ProcessingPlantId,
                    ProductionPeriodId = item.ProductionPeriodId,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,
                };

                db.SeedProcessingOrders.Attach(entity);
                db.SeedProcessingOrders.Remove(entity);
                db.SaveChanges();

                //UpdateProductionOrderItemStatusOnProcessingOrderDelete(entity.SeedProductionOrderItemId);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
       private void InsertProcessingOrderFromIssues(SeedProcessingOrder order) {
            var issues = db.ProductionIssues.Where(i => i.IsOnlineApproved && i.ProcessingPlantId == order.ProcessingPlantId).ToList();
            foreach(ProductionIssue issue in issues)
            {
                var issueItems = db.ProductionIssueItems.Where(ii => ii.ProductionIssueId == issue.ProductionIssueId).ToList();
                foreach(ProductionIssueItem item in issueItems)
                {
                    
                    var mappings = db.IssueAndProcessingOrderItemMappings.Where(m => m.Type == ProductType.Seed && m.ProductionIssueItemId == item.ProductionIssueItemId).ToList();
                    var usedQty = mappings.Any() ? mappings.Sum(m => m.UsedQuantity) : 0; 
                    if(item.IssuedQuantity > usedQty)
                    {
                        var existingOrderItem = db.SeedProcessingOrderItems.FirstOrDefault(oi => oi.SeedProcessingOrderId == order.SeedProcessingOrderId && oi.ItemCode == item.ItemCode);
                        if (existingOrderItem == null)
                        {
                            #region create new order item and its mapping
                            var record = new SeedProcessingOrderItem
                            {
                                SeedProcessingOrderId = order.SeedProcessingOrderId,
                                ItemCategoryCode = item.ItemCategoryCode,
                                ItemCode = item.ItemCode,
                                RawQuantity = item.IssuedQuantity - usedQty,
                            };
                            record.QuantityProduced = record.RawQuantity;
                            record.RecoveryRate = 100;

                            record.TotalProductionCost = record.RawQuantity * item.UnitPrice;
                            db.SeedProcessingOrderItems.Add(record);
                            db.SaveChanges();

                            record.ProcessingCode = record.SeedProcessingOrderItemId.ToString("D12");
                            db.SeedProcessingOrderItems.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            var mappingRecord = new IssueAndProcessingOrderItemMapping
                            {
                                ProcessingOrderItemId = record.SeedProcessingOrderItemId,
                                ProductionIssueItemId = item.ProductionIssueItemId,
                                Type = ProductType.Seed,
                                UsedQuantity = record.RawQuantity,

                            };
                            db.IssueAndProcessingOrderItemMappings.Add(mappingRecord);
                            db.SaveChanges(); 
                            #endregion
                        }
                        else
                        {
                            #region Modify the existing order item and add or modify its mapping
                            existingOrderItem.RawQuantity += (item.IssuedQuantity - usedQty);
                            existingOrderItem.TotalProductionCost = existingOrderItem.RawQuantity * item.UnitPrice;

                            existingOrderItem.QuantityProduced = existingOrderItem.RawQuantity;

                            db.SeedProcessingOrderItems.Attach(existingOrderItem);
                            db.Entry(existingOrderItem).State = EntityState.Modified;
                            db.SaveChanges();

                            var existingMapping = mappings.FirstOrDefault(m => m.ProcessingOrderItemId == existingOrderItem.SeedProcessingOrderId);
                            if (existingMapping != null)
                            {
                                existingMapping.UsedQuantity = existingOrderItem.RawQuantity;
                                db.IssueAndProcessingOrderItemMappings.Attach(existingMapping);
                                db.SaveChanges();
                            }
                            else
                            {
                                var newMapping = new IssueAndProcessingOrderItemMapping
                                {
                                    ProcessingOrderItemId = existingOrderItem.SeedProcessingOrderItemId,
                                    ProductionIssueItemId = item.ProductionIssueItemId,
                                    Type = ProductType.Seed,
                                    UsedQuantity = existingOrderItem.RawQuantity
                                };
                                db.IssueAndProcessingOrderItemMappings.Add(newMapping);
                                db.SaveChanges();
                            } 
                            #endregion
                        }
                    }
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