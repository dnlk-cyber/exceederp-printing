using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Helpers.Inventory;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstimationCustomWorkItemMaterialController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult EstimationCustomWorkItemMaterial_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {

            IQueryable<EstimationCustomWorkItemMaterial> materials = db.EstimationCustomWorkItemMaterial.Where(x => x.ProjectEstimationCustomWorkItemId == workItemId);
            DataSourceResult result = materials.ToDataSourceResult(request, material => new {
                EstimationCustomWorkItemMaterialId = material.EstimationCustomWorkItemMaterialId,
                ProjectEstimationCustomWorkItemId = material.ProjectEstimationCustomWorkItemId,
                ItemCode = material.ItemCode,
                Unit = material.Unit,
                Quantity = material.Quantity,
                UnitPrice = material.UnitPrice,
                TotalCost = material.TotalCost,
                OrgBranchName = material.OrgBranchName,
                DateCreated = material.DateCreated,
                LastModified = material.LastModified,
                CreatedBy = material.CreatedBy,
                ModifiedBy = material.ModifiedBy,
                ItemCategoryCode = material.ItemCategoryCode,
            });

            return Json(result);
        }
        
        private bool MaterialExists(int workItemId, string materialCode)
        {
            bool materialExists = false;
            var temp = db.EstimationCustomWorkItemMaterial.Where(m => m.ProjectEstimationCustomWorkItemId == workItemId && m.ItemCode == materialCode).FirstOrDefault();
            if (temp != null)
            {
                materialExists = true;
            }
            return materialExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationCustomWorkItemMaterial_Create([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemMaterial material, int workItemId)
        {
            if (ModelState.IsValid)
            {
                if (MaterialExists(workItemId, material.ItemCode))
                {
                    ModelState.AddModelError("", "Material Exists already!");
                }
                else
                {
                    var entity = new EstimationCustomWorkItemMaterial
                    {
                        ItemCode = material.ItemCode,
                        ItemCategoryCode = material.ItemCategoryCode,
                        Unit = InventoryHelper.GetUnitOfMeasurment(material.ItemCode),
                        Quantity = material.Quantity,
                        UnitPrice = InventoryHelper.GetUnitPrice(material.ItemCode),
                        TotalCost = material.Quantity * material.UnitPrice,
                        ProjectEstimationCustomWorkItemId = workItemId,
                        OrgBranchName = material.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = material.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = material.ModifiedBy
                    };

                    entity.TotalCost = entity.Quantity * entity.UnitPrice;
                    db.EstimationCustomWorkItemMaterial.Add(entity);
                    db.SaveChanges();
                    material.EstimationCustomWorkItemMaterialId = entity.EstimationCustomWorkItemMaterialId;

                    ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(workItemId);
                }
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationCustomWorkItemMaterial_Update([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemMaterial material)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemMaterial
                {
                    EstimationCustomWorkItemMaterialId = material.EstimationCustomWorkItemMaterialId,
                    ItemCode = material.ItemCode,
                    Unit = InventoryHelper.GetUnitOfMeasurment(material.ItemCode),
                    Quantity = material.Quantity,
                    UnitPrice = InventoryHelper.GetUnitPrice(material.ItemCode),
                    TotalCost = material.Quantity * material.UnitPrice,
                    ProjectEstimationCustomWorkItemId = material.ProjectEstimationCustomWorkItemId,
                    OrgBranchName = material.OrgBranchName,
                    DateCreated = material.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = material.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ItemCategoryCode = material.ItemCategoryCode,
                };
                entity.TotalCost = entity.Quantity * entity.UnitPrice;
                db.EstimationCustomWorkItemMaterial.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
            }

            return Json(new[] { material }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationCustomWorkItemMaterial_Destroy([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemMaterial material)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemMaterial
                {
                    EstimationCustomWorkItemMaterialId = material.EstimationCustomWorkItemMaterialId,
                    ItemCode = material.ItemCode,
                    Unit = material.Unit,
                    Quantity = material.Quantity,
                    UnitPrice = material.UnitPrice,
                    TotalCost = material.TotalCost,
                    ProjectEstimationCustomWorkItemId = material.ProjectEstimationCustomWorkItemId,
                    OrgBranchName = material.OrgBranchName,
                    DateCreated = material.DateCreated,
                    LastModified = material.LastModified,
                    CreatedBy = material.CreatedBy,
                    ModifiedBy = material.ModifiedBy,
                    ItemCategoryCode = material.ItemCategoryCode,
                };

                db.EstimationCustomWorkItemMaterial.Attach(entity);
                db.EstimationCustomWorkItemMaterial.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
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