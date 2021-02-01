using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ResourceUtilizationMaterialController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;

        public ActionResult UtilizationMaterial_Read([DataSourceRequest]DataSourceRequest request, int wItemIdRead)
        {
            IQueryable<ResourceUtilizationMaterial> materials = db.ResourceUtilizationMaterials.Where(x => x.ResourceUtilizationWorkItemId == wItemIdRead);
            DataSourceResult result = materials.ToDataSourceResult(request, projectMaterial => new
            {
                ResourceUtilizationMaterialId = projectMaterial.ResourceUtilizationMaterialId,
                ResourceUtilizationWorkItemId = projectMaterial.ResourceUtilizationWorkItemId,
                ItemCode = projectMaterial.ItemCode,
                ItemCategoryCode = projectMaterial.ItemCategoryCode,
                Unit = projectMaterial.Unit,
                Quantity = projectMaterial.Quantity,
                UnitPrice = projectMaterial.UnitPrice,
                TotalCost = projectMaterial.TotalCost,
                MaterialUnitCost = projectMaterial.MaterialUnitCost,
                OrgBranchName = projectMaterial.OrgBranchName,
                DateCreated = projectMaterial.DateCreated,
                LastModified = projectMaterial.LastModified,
                CreatedBy = projectMaterial.CreatedBy,
                ModifiedBy = projectMaterial.ModifiedBy,
                //WorkItemPerProjectId = projectMaterial.WorkItemPerProjectId
            });

            return Json(result);
        }
        private bool MaterialExists(ResourceUtilizationMaterial material, int utilizationWorkItemId)
        {
            bool materialExists = false;
            var mat = db.ResourceUtilizationMaterials.FirstOrDefault(
                e => e.ResourceUtilizationWorkItemId == utilizationWorkItemId && e.ItemCode == material.ItemCode);
            if (mat != null)
            {
                materialExists = true;
            }
            return materialExists;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationMaterial_Create([DataSourceRequest]DataSourceRequest request, ResourceUtilizationMaterial projectMaterial, int wItemIdCreate)
        {
            if (ModelState.IsValid)
            {
                if (MaterialExists(projectMaterial, wItemIdCreate))
                {
                    ModelState.AddModelError("", "Material Registered already!");
                }
                else
                {
                    var entity = new ResourceUtilizationMaterial
                    {
                        ResourceUtilizationWorkItemId = wItemIdCreate,
                        ItemCode = projectMaterial.ItemCode,
                        ItemCategoryCode = projectMaterial.ItemCategoryCode,
                        Unit = GetUnitOfMeasurment(projectMaterial.ItemCode),
                        Quantity = projectMaterial.Quantity,
                        UnitPrice = this.GetUnitPrice(projectMaterial.ItemCode),
                        TotalCost = projectMaterial.TotalCost,
                        MaterialUnitCost = GetMaterialUnitCost(wItemIdCreate),
                        OrgBranchName = projectMaterial.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectMaterial.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectMaterial.ModifiedBy
                    };
                    entity.TotalCost = entity.Quantity * entity.UnitPrice;
                    db.ResourceUtilizationMaterials.Add(entity);
                    db.SaveChanges();
                    projectMaterial.ResourceUtilizationMaterialId = entity.ResourceUtilizationMaterialId;
                    projectMaterial.TotalCost = entity.TotalCost;
                    //projectMaterial.ResourceUtilizationWorkItemId = entity.ResourceUtilizationWorkItemId;
                    //ProjectListController.UpdateProjectTotalCost(idtwo);

                    ResourceUtilizationController controller = new ResourceUtilizationController();
                    controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
                }
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationMaterial_Update([DataSourceRequest]DataSourceRequest request, ResourceUtilizationMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationMaterial
                {
                    ResourceUtilizationMaterialId = projectMaterial.ResourceUtilizationMaterialId,
                    ItemCategoryCode = projectMaterial.ItemCategoryCode,
                    ItemCode = projectMaterial.ItemCode,
                    Unit = GetUnitOfMeasurment(projectMaterial.ItemCode),
                    Quantity = projectMaterial.Quantity,
                    UnitPrice = this.GetUnitPrice(projectMaterial.ItemCode),
                    TotalCost = projectMaterial.TotalCost,
                    MaterialUnitCost = GetMaterialUnitCost(projectMaterial.ResourceUtilizationWorkItemId),
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = projectMaterial.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectMaterial.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ResourceUtilizationWorkItemId = projectMaterial.ResourceUtilizationWorkItemId
                };
                entity.TotalCost = entity.Quantity * entity.UnitPrice;
                db.ResourceUtilizationMaterials.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                projectMaterial.TotalCost = entity.TotalCost;
                //ProjectListController.UpdateProjectTotalCost(projectMaterial.WorkItemPerProjectId);

                ResourceUtilizationController controller = new ResourceUtilizationController();
                controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationMaterial_Destroy([DataSourceRequest]DataSourceRequest request, ResourceUtilizationMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationMaterial
                {
                    ResourceUtilizationMaterialId = projectMaterial.ResourceUtilizationMaterialId,
                    Unit = projectMaterial.Unit,
                    ItemCategoryCode = projectMaterial.ItemCategoryCode,
                    Quantity = projectMaterial.Quantity,
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    MaterialUnitCost = projectMaterial.MaterialUnitCost,
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = projectMaterial.DateCreated,
                    LastModified = projectMaterial.LastModified,
                    CreatedBy = projectMaterial.CreatedBy,
                    ModifiedBy = projectMaterial.ModifiedBy,
                    ResourceUtilizationWorkItemId = projectMaterial.ResourceUtilizationWorkItemId,
                    ItemCode = projectMaterial.ItemCode,
                };

                db.ResourceUtilizationMaterials.Attach(entity);
                db.ResourceUtilizationMaterials.Remove(entity);
                db.SaveChanges();
                //ProjectListController.UpdateProjectTotalCost(projectMaterial.WorkItemPerProjectId);

                ResourceUtilizationController controller = new ResourceUtilizationController();
                controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }
        #region Methods

        private decimal GetMaterialUnitCost(int id)
        {
            decimal labourCost = 0;
            totalMaterialCost = 0;
            var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectMaterial.Where(x => x.WorkItemPerProjectId == id).ToList();
                if (labouCosts.Any())
                {


                    labourCost = labouCosts.Sum(x => x.TotalCost);

                }
                labourCost = labourCost / workItem.TotalQuantityOfWork;
            }
            totalMaterialCost = labourCost;
            return labourCost;
        }
        // 
        // Get the unit price (the approved unit price) of Items from the Material Price Index
        private decimal GetUnitPrice(string itemCode)
        {
            var price = new decimal();
            var currentItem = db.MaterialPriceIndexIndices.Where(material => material.ItemCode == itemCode).FirstOrDefault();
            if (currentItem != null)
            {
                price = currentItem.ApprovedCost;
            }
            return price;
        }
        private string GetUnitOfMeasurment(string itemCode)
        {
            var items = db.InventoryItems.Where(x => x.ItemCode == itemCode).FirstOrDefault();
            if (items != null)
            {
                return items.UoMName;
            }
            return "";
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}