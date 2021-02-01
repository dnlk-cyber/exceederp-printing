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
    public class GumProcessingOrderController : Controller
    {
        ExceedDbContext db;

        public GumProcessingOrderController()
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
            IQueryable<GumProcessingOrder> items = db.GumProcessingOrders;
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                GumProcessingOrderId = item.GumProcessingOrderId,
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
        public ActionResult ProcessingItem_Create([DataSourceRequest]DataSourceRequest request, GumProcessingOrder item)
        {
            if (ModelState.IsValid)
            {
                var entity = new GumProcessingOrder
                {
                    GumProcessingOrderId = item.GumProcessingOrderId,
                    ProcessingPlantId = item.ProcessingPlantId,
                    ProductionPeriodId = item.ProductionPeriodId,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy,
                };

                db.GumProcessingOrders.Add(entity);
                db.SaveChanges();
                item.GumProcessingOrderId = entity.GumProcessingOrderId;
                
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Update([DataSourceRequest]DataSourceRequest request, GumProcessingOrder item)
        {
            if (ModelState.IsValid)
            {
                var entity = new GumProcessingOrder
                {
                    GumProcessingOrderId = item.GumProcessingOrderId,
                    ProcessingPlantId = item.ProcessingPlantId,
                    ProductionPeriodId = item.ProductionPeriodId,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.GumProcessingOrders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Destroy([DataSourceRequest]DataSourceRequest request, GumProcessingOrder item)
        {
            if (ModelState.IsValid)
            {
                var entity = new GumProcessingOrder
                {
                    GumProcessingOrderId = item.GumProcessingOrderId,
                    ProcessingPlantId = item.ProcessingPlantId,
                    ProductionPeriodId = item.ProductionPeriodId,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,
                };

                db.GumProcessingOrders.Attach(entity);
                db.GumProcessingOrders.Remove(entity);
                db.SaveChanges();

                //UpdateProductionOrderItemStatusOnProcessingOrderDelete(entity.SeedProductionOrderItemId);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}