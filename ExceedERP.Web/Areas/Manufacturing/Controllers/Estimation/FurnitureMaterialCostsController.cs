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
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Web.Areas.Manufacturing.Models;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureMaterialCostsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

     

        public ActionResult FurnitureMaterialCosts_Read([DataSourceRequest]DataSourceRequest request, int estimationFormId)
            
        {
            IQueryable<FurnitureMaterialCost> materialcosts = db.FurnitureMaterialCosts.Where(q=>q.FurnitureEstimationId == estimationFormId).OrderByDescending(a => a.FurnitureMaterialCostId);
            DataSourceResult result = materialcosts.ToDataSourceResult(request, materialCost => new {
                FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                FurnitureEstimationId = materialCost.FurnitureEstimationId,
                InventoryCategoryId = materialCost.InventoryCategoryId,
                InventoryId = materialCost.InventoryId,
                StoreCode = materialCost.StoreCode,
                Quantity = materialCost.Quantity,
                UnitPrice = materialCost.UnitPrice,
                TotalPrice = materialCost.TotalPrice,
                DateCreated = materialCost.DateCreated,
                LastModified = materialCost.LastModified
            });

            return Json(result);
        }
        public ActionResult MaterialJobOrders_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            //var estId = (int)db.Proformas.Find(db.Jobs.Find(id)?.ProformaId)?.EstimationFormId;
            IQueryable<FurnitureMaterialCost> materialcosts = db.FurnitureMaterialCosts.Where(q => q.JobId == id);
            DataSourceResult result = materialcosts.ToDataSourceResult(request, materialCost => new {
                FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                FurnitureEstimationId = materialCost.FurnitureEstimationId,
                InventoryCategoryId = materialCost.InventoryCategoryId,
                InventoryId = materialCost.InventoryId,
                Quantity = materialCost.Quantity,
                StoreCode = materialCost.StoreCode,
                UnitPrice = materialCost.UnitPrice,
                TotalPrice = materialCost.TotalPrice,
                DateCreated = materialCost.DateCreated,
                LastModified = materialCost.LastModified
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost, int parentId)
        {

            if (ModelState.IsValid)
            {
                var invs = db.FurnitureMaterialCosts.Where(q => q.InventoryId == materialCost.InventoryId&&q.JobId==parentId).ToList();
                if (invs.Any())
                {
                    ModelState.AddModelError("403", "Inventory already available");
                }
                materialCost.TotalPrice = this.totalPrice(materialCost.UnitPrice, materialCost.Quantity);
                var entity = new FurnitureMaterialCost
                {
                    FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                    FurnitureEstimationId = parentId,
                    JobId = parentId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    Quantity = materialCost.Quantity,
                    StoreCode = materialCost.StoreCode,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice
                };

                db.FurnitureMaterialCosts.Add(entity);
                db.SaveChanges();
                var estimation = new UpdateFurnitureOverallCost();
                estimation.update(parentId);

            }

            
            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult JobMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost, int id)
        {

            if (ModelState.IsValid)
            {
                int estId = 0;
                var job = db.Jobs.Find(id);
                if(job != null)
                {
                    var performa = db.Proformas.Find(job.ProformaId);
                    if(performa != null)
                    {
                        estId = (int)performa.EstimationFormId;
                       // var estId = (int)db.Proformas.Find(db.Jobs.Find(id)?.ProformaId)?.EstimationFormId;
                        var invs = db.MaterialCosts.Where(q => q.InventoryId == materialCost.InventoryId && q.EstimationFormId == performa.EstimationFormId).ToList();
                        if (invs.Any())
                        {
                            ModelState.AddModelError("403", "Inventory already available");
                        }
                    }
                }
                
                materialCost.TotalPrice = this.totalPrice(materialCost.UnitPrice, materialCost.Quantity);
                var entity = new FurnitureMaterialCost
                {
                    //MaterialCostId = materialCost.MaterialCostId,
                    JobId = id,
                    FurnitureEstimationId = estId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    Quantity = materialCost.Quantity,
                    StoreCode = materialCost.StoreCode,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice
                };

                db.FurnitureMaterialCosts.Add(entity);
                db.SaveChanges();
                var estimation = new FurnitureUpdateOverallCost();
                estimation.update(estId);

            }

            
            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        public decimal totalPrice(decimal UP, decimal Q)
        {
            return UP * Q;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureMaterialCosts_Update([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost)
        {
            if (ModelState.IsValid)
            {
                materialCost.TotalPrice = this.totalPrice(materialCost.UnitPrice, materialCost.Quantity);

                var entity = new FurnitureMaterialCost
                {
                    FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                    FurnitureEstimationId = materialCost.FurnitureEstimationId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    StoreCode = materialCost.StoreCode,
                    Quantity = materialCost.Quantity,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice
                };

                db.FurnitureMaterialCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var estimation = new UpdateFurnitureOverallCost();
                estimation.update(materialCost.FurnitureEstimationId);
            }

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureMaterialCosts_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost)
        {

            if (ModelState.IsValid)
            {
                var entity = new FurnitureMaterialCost
                {
                    FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    FurnitureEstimationId = materialCost.FurnitureEstimationId,
                    StoreCode = materialCost.StoreCode,
                    Quantity = materialCost.Quantity,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice

                };

                db.FurnitureMaterialCosts.Attach(entity);
                db.FurnitureMaterialCosts.Remove(entity);
                db.SaveChanges();
                var estimation = new UpdateFurnitureOverallCost();
                estimation.update(materialCost.FurnitureEstimationId);


            }

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
