using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        public ActionResult Index()
        {
            var items = db.InventoryItems.ToList();
            ViewData["Items"] = items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }); ;
            var itemCategories = db.ConstructionItemCategories.ToList();
            ViewData["ItemCategorys"] = itemCategories.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }); ; ;
            return View();
        }

        public ActionResult ProjectEstimationMaterial_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
           
            IQueryable<ProjectEstimationMaterial> projectestimationmaterial = db.ProjectEstimationMaterial.Where( x=> x.ProjectEstimationWorkItemId ==id);
            DataSourceResult result = projectestimationmaterial.ToDataSourceResult(request, projectEstimationMaterial => new {
                ProjectEstimationMaterialId = projectEstimationMaterial.ProjectEstimationMaterialId,
                ItemCode = projectEstimationMaterial.ItemCode,
                Unit = projectEstimationMaterial.Unit,
                Quantity = projectEstimationMaterial.Quantity,
                UnitPrice = projectEstimationMaterial.UnitPrice,
                TotalCost = projectEstimationMaterial.TotalCost,
                ProjectEstimationWorkItemId = projectEstimationMaterial.ProjectEstimationWorkItemId,
                MaterialUnitCost = projectEstimationMaterial.MaterialUnitCost,
                OrgBranchName = projectEstimationMaterial.OrgBranchName,
                DateCreated = projectEstimationMaterial.DateCreated,
                LastModified = projectEstimationMaterial.LastModified,
                CreatedBy = projectEstimationMaterial.CreatedBy,
                ModifiedBy = projectEstimationMaterial.ModifiedBy ,
                ItemCategoryCode = projectEstimationMaterial.ItemCategoryCode,
            });

            return Json(result);
        }
        public ActionResult ProjectEstimationMateriall_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<ProjectEstimationMaterial> projectestimationmaterial = db.ProjectEstimationMaterial.Where(x => x.ProjectEstimationWorkItemId == id);
            DataSourceResult result = projectestimationmaterial.ToDataSourceResult(request, projectEstimationMaterial => new
            {
                ProjectEstimationMaterialId = projectEstimationMaterial.ProjectEstimationMaterialId,
                ItemCode = projectEstimationMaterial.ItemCode,
                Unit = projectEstimationMaterial.Unit,
                Quantity = projectEstimationMaterial.Quantity,
                UnitPrice = projectEstimationMaterial.UnitPrice,
                TotalCost = projectEstimationMaterial.TotalCost,
                ProjectEstimationWorkItemId = projectEstimationMaterial.ProjectEstimationWorkItemId,
                MaterialUnitCost = projectEstimationMaterial.MaterialUnitCost,
                OrgBranchName = projectEstimationMaterial.OrgBranchName,
                DateCreated = projectEstimationMaterial.DateCreated,
                LastModified = projectEstimationMaterial.LastModified,
                CreatedBy = projectEstimationMaterial.CreatedBy,
                ModifiedBy = projectEstimationMaterial.ModifiedBy
            });

            return Json(result);
        }
   
        private bool MaterialExists(int workItemId, string materialCode) {
            bool materialExists = false;
            var temp = db.ProjectEstimationMaterial.Where(m => m.ProjectEstimationWorkItemId == workItemId && m.ItemCode == materialCode).FirstOrDefault();
            if(temp != null)
            {
                materialExists = true;
            }
            return materialExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMaterial_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationMaterial projectEstimationMaterial, int idtow)
        {
            if (ModelState.IsValid)
            {
                if(MaterialExists(idtow, projectEstimationMaterial.ItemCode))
                {
                    ModelState.AddModelError("", "Material Exists already!");
                }
                else
                {
                    var entity = new ProjectEstimationMaterial
                    {
                        ItemCode = projectEstimationMaterial.ItemCode,
                        ItemCategoryCode = projectEstimationMaterial.ItemCategoryCode,
                        Unit = InventoryHelper.GetUnitOfMeasurment(projectEstimationMaterial.ItemCode),
                        Quantity = projectEstimationMaterial.Quantity,
                        UnitPrice = InventoryHelper.GetUnitPrice(projectEstimationMaterial.ItemCode),
                        TotalCost = projectEstimationMaterial.Quantity * projectEstimationMaterial.UnitPrice,
                        ProjectEstimationWorkItemId = idtow,
                        MaterialUnitCost = GetMaterialUnitCost(idtow),
                        OrgBranchName = projectEstimationMaterial.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEstimationMaterial.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEstimationMaterial.ModifiedBy
                    };
                    entity.TotalCost = entity.Quantity * entity.UnitPrice;
                    db = new ExceedDbContext();
                    db.ProjectEstimationMaterial.Add(entity);
                    db.SaveChanges();
                    projectEstimationMaterial.ProjectEstimationMaterialId = entity.ProjectEstimationMaterialId;

                    ProjectEstimationWorkItemController.UpdateWorkItemCost(idtow);
                    ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(idtow);
                }
            }

            return Json(new[] { projectEstimationMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMaterial_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationMaterial projectEstimationMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationMaterial
                {
                    ProjectEstimationMaterialId = projectEstimationMaterial.ProjectEstimationMaterialId,
                    ItemCode = projectEstimationMaterial.ItemCode,
                    Unit = InventoryHelper.GetUnitOfMeasurment(projectEstimationMaterial.ItemCode),
                    Quantity = projectEstimationMaterial.Quantity,
                    UnitPrice = InventoryHelper.GetUnitPrice(projectEstimationMaterial.ItemCode),
                    TotalCost = projectEstimationMaterial.Quantity * projectEstimationMaterial.UnitPrice,
                    ProjectEstimationWorkItemId = projectEstimationMaterial.ProjectEstimationWorkItemId,
                    MaterialUnitCost = GetMaterialUnitCost(projectEstimationMaterial.ProjectEstimationWorkItemId),
                    OrgBranchName = projectEstimationMaterial.OrgBranchName,
                    DateCreated = projectEstimationMaterial.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationMaterial.CreatedBy,
                    ModifiedBy = User.Identity .Name  ,
                    ItemCategoryCode = projectEstimationMaterial.ItemCategoryCode,
                };
                entity.TotalCost = entity.Quantity * entity.UnitPrice;
                db = new ExceedDbContext();
                db.ProjectEstimationMaterial.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationWorkItemController.UpdateWorkItemCost(projectEstimationMaterial.ProjectEstimationWorkItemId);
                ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(projectEstimationMaterial.ProjectEstimationWorkItemId);
            }

            return Json(new[] { projectEstimationMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMaterial_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationMaterial projectEstimationMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationMaterial
                {
                    ProjectEstimationMaterialId = projectEstimationMaterial.ProjectEstimationMaterialId,
                    ItemCode = projectEstimationMaterial.ItemCode,
                    Unit = projectEstimationMaterial.Unit,
                    Quantity = projectEstimationMaterial.Quantity,
                    UnitPrice = projectEstimationMaterial.UnitPrice,
                    TotalCost = projectEstimationMaterial.TotalCost,
                    ProjectEstimationWorkItemId = projectEstimationMaterial.ProjectEstimationWorkItemId,
                    MaterialUnitCost = projectEstimationMaterial.MaterialUnitCost,
                    OrgBranchName = projectEstimationMaterial.OrgBranchName,
                    DateCreated = projectEstimationMaterial.DateCreated,
                    LastModified = projectEstimationMaterial.LastModified,
                    CreatedBy = projectEstimationMaterial.CreatedBy,
                    ModifiedBy = projectEstimationMaterial.ModifiedBy ,
                    ItemCategoryCode = projectEstimationMaterial.ItemCategoryCode,
                };

                db.ProjectEstimationMaterial.Attach(entity);
                db.ProjectEstimationMaterial.Remove(entity);
                db.SaveChanges();

                ProjectEstimationWorkItemController.UpdateWorkItemCost(projectEstimationMaterial.ProjectEstimationWorkItemId);
                ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(projectEstimationMaterial.ProjectEstimationWorkItemId);
            }

            return Json(new[] { projectEstimationMaterial }.ToDataSourceResult(request, ModelState));
        }
        private decimal GetMaterialUnitCost(int id)
        {
            decimal labourCost = 0;
            var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEstimationMaterial.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
                if (labouCosts.Any())
                {

                    foreach (var m in labouCosts)
                    {
                        labourCost += m.TotalCost;
                    }
                }
                labourCost = labourCost / workItem.TotalQuantityOfWork;
            }
            totalMaterialCost = labourCost;
            return labourCost;
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
