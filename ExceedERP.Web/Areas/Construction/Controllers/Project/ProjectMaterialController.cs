using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        public ActionResult Index()
        {
            var items = db.InventoryItems.ToList();
            ViewData["Items"] = items;
            var itemCategories = db.ItemCategorys.ToList();
            ViewData["ItemCategorys"] = itemCategories.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }); ; ;
            return View();
        }

        public ActionResult ProjectMaterial_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProjectMaterial> projectmaterial = db.ProjectMaterial.Where (x=> x.WorkItemPerProjectId ==id);
            DataSourceResult result = projectmaterial.ToDataSourceResult(request, projectMaterial => new {
                ProjectMaterialId = projectMaterial.ProjectMaterialId,
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
                WorkItemPerProjectId = projectMaterial.WorkItemPerProjectId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Create([DataSourceRequest]DataSourceRequest request, ProjectMaterial projectMaterial, int idtwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMaterial
                {
                    ItemCode =projectMaterial .ItemCode ,
                    ItemCategoryCode = projectMaterial.ItemCategoryCode,
                    Unit = InventoryHelper.GetUnitOfMeasurment(projectMaterial.ItemCode),
                    Quantity = projectMaterial.Quantity,
                    //UnitPrice = InventoryHelper.GetUnitPrice(projectMaterial.ItemCode),
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    MaterialUnitCost = GetMaterialUnitCost(idtwo),
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectMaterial.LastModified,
                    CreatedBy =User.Identity .Name ,
                    ModifiedBy = projectMaterial.ModifiedBy,
                    WorkItemPerProjectId = idtwo 
                };
                entity.TotalCost = entity.Quantity * entity.UnitPrice;
                db.ProjectMaterial.Add(entity);
                db.SaveChanges();
                projectMaterial.ProjectMaterialId = entity.ProjectMaterialId;
                ProjectListController.UpdateProjectTotalCost(idtwo);
               
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Update([DataSourceRequest]DataSourceRequest request, ProjectMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMaterial
                {
                    ProjectMaterialId = projectMaterial.ProjectMaterialId,
                    ItemCategoryCode = projectMaterial.ItemCategoryCode,
                    ItemCode= projectMaterial .ItemCode ,                    
                    Unit = InventoryHelper.GetUnitOfMeasurment(projectMaterial.ItemCode),
                    Quantity = projectMaterial.Quantity,
                    //UnitPrice = InventoryHelper.GetUnitPrice(projectMaterial.ItemCode),
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    MaterialUnitCost = GetMaterialUnitCost(projectMaterial.WorkItemPerProjectId),
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = projectMaterial.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectMaterial.CreatedBy,
                    ModifiedBy = User.Identity .Name ,
                    WorkItemPerProjectId = projectMaterial.WorkItemPerProjectId
                };
                entity.TotalCost = entity.Quantity * entity.UnitPrice;
                db.ProjectMaterial.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                ProjectListController.UpdateProjectTotalCost(projectMaterial.WorkItemPerProjectId);
                
            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Destroy([DataSourceRequest]DataSourceRequest request, ProjectMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMaterial
                {
                    ProjectMaterialId = projectMaterial.ProjectMaterialId,
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
                    WorkItemPerProjectId = projectMaterial.WorkItemPerProjectId,
                    ItemCode = projectMaterial.ItemCode,
                };

                db.ProjectMaterial.Attach(entity);
                db.ProjectMaterial.Remove(entity);
                db.SaveChanges();
                ProjectListController.UpdateProjectTotalCost(projectMaterial.WorkItemPerProjectId);
                
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
        //private decimal GetUnitPrice(string itemCode)
        //{
        //    var price = new decimal();
        //    var currentItem = db.MaterialPriceIndexIndices.Where(material => material.ItemCode == itemCode).FirstOrDefault();
        //    if (currentItem != null)
        //    {
        //        price = currentItem.ApprovedCost;
        //    }
        //    return price;
        //}
        //private string GetUnitOfMeasurment(string itemCode)
        //{
        //    var items = db.InventoryItems.Where(x => x.ItemCode == itemCode).FirstOrDefault();
        //    if (items != null)
        //    {
        //        return items.UoMName;
        //    }
        //    return "";
        //}
        public  decimal GetUnitPrice(string itemCode)
        {
            var db = new ExceedDbContext();
            var price = new decimal();
            var currentItem = db.MaterialPriceIndexIndices.FirstOrDefault(material => material.ItemCode == itemCode);
            if (currentItem != null)
            {
                price = currentItem.ApprovedCost;
            }
            return price;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
