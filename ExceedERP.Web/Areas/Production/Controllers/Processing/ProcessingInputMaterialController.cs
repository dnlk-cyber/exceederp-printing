using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Helpers.Production;
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
    public class ProcessingInputMaterialController : Controller
    {
        ExceedDbContext db;

        public ProcessingInputMaterialController()
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

        public ActionResult Materials_Read([DataSourceRequest]DataSourceRequest request, int itemId, ProductType productionType)
        {
            IQueryable<ProcessingInputMaterial> materials = db.ProcessingInputMaterials.Where(m => m.SeedProcessingOrderItemId == itemId || m.GumProcessingOrderItemId == itemId);
            if (productionType == ProductType.Seed)
            {
                materials = materials.Where(a => a.SeedProcessingOrderItemId == itemId);
            }
            else if (productionType == ProductType.NaturalGum)
            {
                materials = materials.Where(a => a.GumProcessingOrderItemId == itemId);
            }
            DataSourceResult result = materials.ToDataSourceResult(request, material => new
            {
                ProcessingInputMaterialId = material.ProcessingInputMaterialId,
                SeedProcessingOrderItemId = material.SeedProcessingOrderItemId,
                GumProcessingOrderItemId = material.GumProcessingOrderItemId,
                ItemCategoryCode = material.ItemCategoryCode,
                ItemCode = material.ItemCode,
                Unit = material.Unit,

                ProductionPeriodId = material.ProductionPeriodId,
                DateOfInformation = material.DateOfInformation,
                //BugdetYear = material.BugdetYear,
                Reference = material.Reference,

                UnitPrice = material.UnitPrice,
                Quantity = material.Quantity,
                TotalCost = material.TotalCost,

                OrgBranchName = material.OrgBranchName,
                DateCreated = material.DateCreated,
                LastModified = material.LastModified,
                CreatedBy = material.CreatedBy,
                ModifiedBy = material.ModifiedBy,
            });

            return Json(result);
        }
        private bool IsInvalidTimeRange(int periodId, DateTime time)
        {
            var isInvalid = false;
            var currentPeriod = db.ProductionPeriods.Find(periodId);
            if (currentPeriod != null)
            {
                if (currentPeriod.DateFrom > time || currentPeriod.DateTo < time)
                {
                    isInvalid = true;
                }
            }
            return isInvalid;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Create([DataSourceRequest]DataSourceRequest request, ProcessingInputMaterial material, int itemId, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(material.ProductionPeriodId, material.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");

                }
                else
                {
                    var entity = new ProcessingInputMaterial
                    {
                        SeedProcessingOrderItemId = productionType == ProductType.Seed ? itemId : material.SeedProcessingOrderItemId,
                        GumProcessingOrderItemId = productionType ==  ProductType.NaturalGum ? itemId : material.GumProcessingOrderItemId,
                        ItemCategoryCode = material.ItemCategoryCode,
                        ItemCode = material.ItemCode,
                        Unit = InventoryHelper.GetUnitOfMeasurment(material.ItemCode),

                        ProductionPeriodId = material.ProductionPeriodId,
                        DateOfInformation = material.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(material.ProductionPeriodId),
                        Reference = material.Reference,

                        UnitPrice = material.UnitPrice,
                        Quantity = material.Quantity,
                        TotalCost = material.TotalCost,

                        OrgBranchName = material.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = material.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = material.ModifiedBy,

                    };
                    entity.TotalCost = entity.UnitPrice * entity.Quantity;

                    db.ProcessingInputMaterials.Add(entity);
                    db.SaveChanges();

                    material.ProcessingInputMaterialId = entity.ProcessingInputMaterialId;
                    material.TotalCost = entity.TotalCost;
                    material.Unit = entity.Unit;
                    if (productionType == ProductType.Seed)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.SeedProcessingOrderItemId, productionType);
                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.GumProcessingOrderItemId, productionType);
                    }
                }

            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Update([DataSourceRequest]DataSourceRequest request, ProcessingInputMaterial material, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(material.ProductionPeriodId, material.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");

                }
                else
                {
                    var entity = new ProcessingInputMaterial
                    {
                        ProcessingInputMaterialId = material.ProcessingInputMaterialId,
                        SeedProcessingOrderItemId = material.SeedProcessingOrderItemId,
                        GumProcessingOrderItemId = material.GumProcessingOrderItemId,
                        ItemCategoryCode = material.ItemCategoryCode,
                        ItemCode = material.ItemCode,
                        Unit = InventoryHelper.GetUnitOfMeasurment(material.ItemCode),

                        ProductionPeriodId = material.ProductionPeriodId,
                        DateOfInformation = material.DateOfInformation,
                        //BugdetYear = ProductionOrderHelper.GetProductionPeriodFiscalYear(material.ProductionPeriodId),
                        Reference = material.Reference,

                        UnitPrice = material.UnitPrice,
                        Quantity = material.Quantity,
                        TotalCost = material.TotalCost,

                        OrgBranchName = material.OrgBranchName,
                        DateCreated = material.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = material.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                    entity.TotalCost = entity.UnitPrice * entity.Quantity;

                    db.ProcessingInputMaterials.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    material.TotalCost = entity.TotalCost;
                    material.Unit = entity.Unit;

                    if (productionType == ProductType.Seed)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.SeedProcessingOrderItemId, productionType);
                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.GumProcessingOrderItemId, productionType);
                    }
                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Destroy([DataSourceRequest]DataSourceRequest request, ProcessingInputMaterial material, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcessingInputMaterial
                {
                    ProcessingInputMaterialId = material.ProcessingInputMaterialId,
                    SeedProcessingOrderItemId = material.SeedProcessingOrderItemId,
                    GumProcessingOrderItemId = material.GumProcessingOrderItemId,
                    ItemCategoryCode = material.ItemCategoryCode,
                    ItemCode = material.ItemCode,
                    Unit = material.Unit,

                    ProductionPeriodId = material.ProductionPeriodId,
                    DateOfInformation = material.DateOfInformation,
                    //BugdetYear = material.BugdetYear,
                    Reference = material.Reference,

                    UnitPrice = material.UnitPrice,
                    Quantity = material.Quantity,
                    TotalCost = material.TotalCost,

                    OrgBranchName = material.OrgBranchName,
                    DateCreated = material.DateCreated,
                    LastModified = material.LastModified,
                    CreatedBy = material.CreatedBy,
                    ModifiedBy = material.ModifiedBy,

                };

                db.ProcessingInputMaterials.Attach(entity);
                db.ProcessingInputMaterials.Remove(entity);
                db.SaveChanges();

                if (productionType == ProductType.Seed)
                {
                    ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.SeedProcessingOrderItemId, productionType);
                }
                else if (productionType == ProductType.NaturalGum)
                {
                    ProcessingOrderHelper.UpdateProcessingOrderInCostItemChange(entity.GumProcessingOrderItemId, productionType);
                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}