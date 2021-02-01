using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Inventory;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ResourceEstimationMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult ResourceEstimation_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            ExceedDbContext dbc = new ExceedDbContext();
            dbc = new ExceedDbContext();
            IQueryable<ResourceEstimationMaterial> labors = dbc.ResourceEstimationMaterials.Where(x => x.WorkItemDurationId == itemId).AsQueryable();
            DataSourceResult result = labors.ToDataSourceResult(request, material => new
            {
                ResourceEstimationMaterialId = material.ResourceEstimationMaterialId,
                WorkItemDurationId = material.WorkItemDurationId,
                ItemCategoryCode = material.ItemCategoryCode,
                ItemCode = material.ItemCode,
                Unit = material.Unit,
                UnitPrice = material.UnitPrice,
                QuantityPerUnit = material.QuantityPerUnit,
                TotalQuantity = material.TotalQuantity,
                TotalCost = material.TotalCost,

                OrgBranchName = material.OrgBranchName,
                DateCreated = material.DateCreated,
                LastModified = material.LastModified,
                CreatedBy = material.CreatedBy,
                ModifiedBy = material.ModifiedBy
            });

            return Json(result);
        }
        private bool MaterialExists(int workItemId, ResourceEstimationMaterial material )
        {
            return db.ResourceEstimationMaterials
                .FirstOrDefault(m => m.WorkItemDurationId == workItemId &&
                                m.ItemCode == material.ItemCode &&
                                m.ResourceEstimationMaterialId != material.ResourceEstimationMaterialId)
                                != null;
           
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Create([DataSourceRequest]DataSourceRequest request, ResourceEstimationMaterial material, int itemId)
        {
            if (ModelState.IsValid)
            {
                if(MaterialExists(itemId, material))
                {
                    ModelState.AddModelError("", "This Labor exists already!");
                }
                else
                {
                    var entity = new ResourceEstimationMaterial
                    {
                        ResourceEstimationMaterialId = material.ResourceEstimationMaterialId,
                        WorkItemDurationId = itemId,
                        ItemCategoryCode = material.ItemCategoryCode,
                        ItemCode = material.ItemCode,
                        Unit = material.Unit,
                        UnitPrice = InventoryHelper.GetUnitPrice(material.ItemCode),
                        QuantityPerUnit = material.QuantityPerUnit,
                        TotalQuantity = material.TotalQuantity,
                        TotalCost = material.TotalCost,


                        OrgBranchName = material.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = material.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = material.ModifiedBy
                    };

                    var parentItem = db.WorkItemDuration.Find(itemId);
                    if(parentItem != null)
                    {
                        entity.TotalQuantity =  parentItem.Quantity * entity.QuantityPerUnit;
                    }
                    entity.TotalCost = entity.UnitPrice * entity.TotalQuantity;

                    db.ResourceEstimationMaterials.Add(entity);
                    db.SaveChanges();

                    material.UnitPrice = entity.UnitPrice;
                    material.ResourceEstimationMaterialId = entity.ResourceEstimationMaterialId;
                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Update([DataSourceRequest]DataSourceRequest request, ResourceEstimationMaterial material)
        {
            if (ModelState.IsValid)
            {
                if (MaterialExists(material.WorkItemDurationId, material))
                {
                    ModelState.AddModelError("", "This Material exists already!");
                }
                else
                {
                    var entity = new ResourceEstimationMaterial
                    {
                        ResourceEstimationMaterialId = material.ResourceEstimationMaterialId,
                        WorkItemDurationId = material.WorkItemDurationId,
                        ItemCategoryCode = material.ItemCategoryCode,
                        ItemCode = material.ItemCode,
                        Unit = material.Unit,
                        UnitPrice = InventoryHelper.GetUnitPrice(material.ItemCode),
                        QuantityPerUnit = material.QuantityPerUnit,
                        TotalQuantity = material.TotalQuantity,
                        TotalCost = material.TotalCost,
                        
                        OrgBranchName = material.OrgBranchName,
                        DateCreated = material.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = material.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    var parentItem = db.WorkItemDuration.Find(entity.WorkItemDurationId);
                    if (parentItem != null)
                    {
                        entity.TotalQuantity = parentItem.Quantity * entity.QuantityPerUnit;
                    }
                    entity.TotalCost = entity.UnitPrice * entity.TotalQuantity;

                    db.ResourceEstimationMaterials.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceEstimation_Destroy([DataSourceRequest]DataSourceRequest request, ResourceEstimationMaterial material)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ResourceEstimationMaterial
                {
                    ResourceEstimationMaterialId = material.ResourceEstimationMaterialId,
                    WorkItemDurationId = material.WorkItemDurationId,
                    ItemCategoryCode = material.ItemCategoryCode,
                    ItemCode = material.ItemCode,
                    Unit = material.Unit,
                    UnitPrice = InventoryHelper.GetUnitPrice(material.ItemCode),
                    QuantityPerUnit = material.QuantityPerUnit,
                    TotalQuantity = material.TotalQuantity,
                    TotalCost = material.TotalCost,

                    OrgBranchName = material.OrgBranchName,
                    DateCreated = material.DateCreated,
                    LastModified = material.LastModified,
                    CreatedBy = material.CreatedBy,
                    ModifiedBy = material.ModifiedBy
                };

                db.ResourceEstimationMaterials.Attach(entity);
                db.ResourceEstimationMaterials.Remove(entity);
                db.SaveChanges();

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
