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
using ExceedERP.Core.Domain.printing.PrintingEstimation;
using ExceedERP.Web.Areas.Printing.Models;

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class MaterialCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

     

        public ActionResult MaterialCosts_Read([DataSourceRequest]DataSourceRequest request, int estimationFormId)
            
        {
            IQueryable<MaterialCost> materialcosts = db.MaterialCosts.Where(q=>q.EstimationFormId == estimationFormId).OrderByDescending(a => a.MaterialCostId);
            DataSourceResult result = materialcosts.ToDataSourceResult(request, materialCost => new {
                MaterialCostId = materialCost.MaterialCostId,
                EstimationFormId = materialCost.EstimationFormId,
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
            IQueryable<MaterialCost> materialcosts = db.MaterialCosts.Where(q => q.JobId == id);
            DataSourceResult result = materialcosts.ToDataSourceResult(request, materialCost => new {
                MaterialCostId = materialCost.MaterialCostId,
                EstimationFormId = materialCost.EstimationFormId,
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
        public ActionResult MaterialCosts_Create([DataSourceRequest]DataSourceRequest request, MaterialCost materialCost, int parentId)
        {

            if (ModelState.IsValid)
            {
                var invs = db.MaterialCosts.Where(q => q.InventoryId == materialCost.InventoryId&&q.JobId==parentId).ToList();
                if (invs.Any())
                {
                    ModelState.AddModelError("403", "Inventory already available");
                }
                materialCost.TotalPrice = this.totalPrice(materialCost.UnitPrice, materialCost.Quantity);
                var entity = new MaterialCost
                {
                    MaterialCostId = materialCost.MaterialCostId,
                    EstimationFormId = parentId,
                    JobId = parentId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    Quantity = materialCost.Quantity,
                    StoreCode = materialCost.StoreCode,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice
                };

                db.MaterialCosts.Add(entity);
                db.SaveChanges();
                var estimation = new UpdateOverallCost();
                estimation.update(parentId);

            }

            
            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult JobMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, MaterialCost materialCost, int id)
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
                var entity = new MaterialCost
                {
                    //MaterialCostId = materialCost.MaterialCostId,
                    JobId = id,
                    EstimationFormId = estId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    Quantity = materialCost.Quantity,
                    StoreCode = materialCost.StoreCode,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice
                };

                db.MaterialCosts.Add(entity);
                db.SaveChanges();
                var estimation = new UpdateOverallCost();
                estimation.update(estId);

            }

            
            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        public decimal totalPrice(decimal UP, decimal Q)
        {
            return UP * Q;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialCosts_Update([DataSourceRequest]DataSourceRequest request, MaterialCost materialCost)
        {
            if (ModelState.IsValid)
            {
                materialCost.TotalPrice = this.totalPrice(materialCost.UnitPrice, materialCost.Quantity);

                var entity = new MaterialCost
                {
                    MaterialCostId = materialCost.MaterialCostId,
                    EstimationFormId = materialCost.EstimationFormId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    StoreCode = materialCost.StoreCode,
                    Quantity = materialCost.Quantity,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice
                };

                db.MaterialCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                var estimation = new UpdateOverallCost();
                estimation.update(materialCost.EstimationFormId);
            }

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaterialCosts_Destroy([DataSourceRequest]DataSourceRequest request, MaterialCost materialCost)
        {

            if (ModelState.IsValid)
            {
                var entity = new MaterialCost
                {
                    MaterialCostId = materialCost.MaterialCostId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    EstimationFormId = materialCost.EstimationFormId,
                    StoreCode = materialCost.StoreCode,
                    Quantity = materialCost.Quantity,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice

                };

                db.MaterialCosts.Attach(entity);
                db.MaterialCosts.Remove(entity);
                db.SaveChanges();
                var estimation = new UpdateOverallCost();
                estimation.update(materialCost.EstimationFormId);


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
