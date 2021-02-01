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

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionOrders
{
    public class ProductionOrderInputMaterialController : Controller
    {
        ExceedDbContext db;

        public ProductionOrderInputMaterialController()
        {
            db = new ExceedDbContext();
        }

        public ActionResult Materials_Read([DataSourceRequest]DataSourceRequest request, int itemId, ProductType productionType)
        {
            IQueryable<ProductionOrderInputMaterial> materials = db.ProductionOrderInputMaterial.Where(m => m.SeedProductionOrderItemId == itemId || m.GumProductionOrderItemId == itemId);
            if (productionType == ProductType.Seed)
            {
                materials = materials.Where(a => a.SeedProductionOrderItemId == itemId);
            }
            else if (productionType == ProductType.NaturalGum)
            {
                materials = materials.Where(a => a.GumProductionOrderItemId == itemId);
            }
            DataSourceResult result = materials.ToDataSourceResult(request, material => new
            {
                ProductionOrderInputMaterialId = material.ProductionOrderInputMaterialId,
                SeedProductionOrderItemId = material.SeedProductionOrderItemId,
                GumProductionOrderItemId = material.GumProductionOrderItemId,
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
        public ActionResult Material_Create([DataSourceRequest]DataSourceRequest request, ProductionOrderInputMaterial material, int itemId, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(material.ProductionPeriodId, material.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");
                }
                else
                {
                    var entity = new ProductionOrderInputMaterial
                    {
                        SeedProductionOrderItemId = productionType == ProductType.Seed ? itemId : material.SeedProductionOrderItemId,
                        GumProductionOrderItemId = productionType == ProductType.NaturalGum ? itemId : material.GumProductionOrderItemId,
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

                    db.ProductionOrderInputMaterial.Add(entity);
                    db.SaveChanges();

                    material.ProductionOrderInputMaterialId = entity.ProductionOrderInputMaterialId;
                    material.TotalCost = entity.TotalCost;
                    material.Unit = entity.Unit;

                    if(productionType == ProductType.Seed)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                    }
                    else if(productionType == ProductType.NaturalGum)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                    }
                }

            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Update([DataSourceRequest]DataSourceRequest request, ProductionOrderInputMaterial material, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                if (IsInvalidTimeRange(material.ProductionPeriodId, material.DateOfInformation))
                {
                    ModelState.AddModelError("", "Date of info. must be within the period");

                }
                else
                {
                    var entity = new ProductionOrderInputMaterial
                    {
                        ProductionOrderInputMaterialId = material.ProductionOrderInputMaterialId,
                        SeedProductionOrderItemId = material.SeedProductionOrderItemId,
                        GumProductionOrderItemId = material.GumProductionOrderItemId,
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

                    db.ProductionOrderInputMaterial.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    material.TotalCost = entity.TotalCost;
                    material.Unit = entity.Unit;

                    if (productionType == ProductType.Seed)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                    }
                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Material_Destroy([DataSourceRequest]DataSourceRequest request, ProductionOrderInputMaterial material, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionOrderInputMaterial
                {
                    ProductionOrderInputMaterialId = material.ProductionOrderInputMaterialId,
                    SeedProductionOrderItemId = material.SeedProductionOrderItemId,
                    GumProductionOrderItemId = material.GumProductionOrderItemId,
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

                db.ProductionOrderInputMaterial.Attach(entity);
                db.ProductionOrderInputMaterial.Remove(entity);
                db.SaveChanges();

                if (productionType == ProductType.Seed)
                {
                    ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                }
                else if (productionType == ProductType.NaturalGum)
                {
                    ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
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