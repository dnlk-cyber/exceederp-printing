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

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class GenericProjectMaterialController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;

        public ActionResult ProjectMaterial_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GenericProjectMaterial> projectmaterial = db.GenericProjectMaterial.Where(x => x.GenericWorkItemPerProjectId == id);
            DataSourceResult result = projectmaterial.ToDataSourceResult(request, projectMaterial => new {
                GenericProjectMaterialId = projectMaterial.GenericProjectMaterialId,
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
                GenericWorkItemPerProjectId = projectMaterial.GenericWorkItemPerProjectId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Create([DataSourceRequest]DataSourceRequest request, GenericProjectMaterial projectMaterial, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectMaterial
                {
                    ItemCode = projectMaterial.ItemCode,
                    ItemCategoryCode = projectMaterial.ItemCategoryCode,
                    Unit = InventoryHelper.GetUnitOfMeasurment(projectMaterial.ItemCode),
                    Quantity = projectMaterial.Quantity,
                    //UnitPrice = InventoryHelper.GetUnitPrice(projectMaterial.ItemCode),
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    //MaterialUnitCost = GetMaterialUnitCost(id),
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectMaterial.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = projectMaterial.ModifiedBy,
                    GenericWorkItemPerProjectId = id
                };
                entity.TotalCost = entity.Quantity * entity.UnitPrice;
                db.GenericProjectMaterial.Add(entity);
                db.SaveChanges();
                projectMaterial.GenericProjectMaterialId = entity.GenericProjectMaterialId;

                ProjectEstimationHelper.UpdateGenericWorkItem(id);

            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Update([DataSourceRequest]DataSourceRequest request, GenericProjectMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectMaterial
                {
                    GenericProjectMaterialId = projectMaterial.GenericProjectMaterialId,
                    ItemCategoryCode = projectMaterial.ItemCategoryCode,
                    ItemCode = projectMaterial.ItemCode,
                    Unit = InventoryHelper.GetUnitOfMeasurment(projectMaterial.ItemCode),
                    Quantity = projectMaterial.Quantity,
                    //UnitPrice = InventoryHelper.GetUnitPrice(projectMaterial.ItemCode),
                    UnitPrice = projectMaterial.UnitPrice,
                    TotalCost = projectMaterial.TotalCost,
                    //MaterialUnitCost = GetMaterialUnitCost(projectMaterial.GenericWorkItemPerProjectId),
                    OrgBranchName = projectMaterial.OrgBranchName,
                    DateCreated = projectMaterial.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectMaterial.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    GenericWorkItemPerProjectId = projectMaterial.GenericWorkItemPerProjectId
                };
                entity.TotalCost = entity.Quantity * entity.UnitPrice;
                db.GenericProjectMaterial.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectMaterial_Destroy([DataSourceRequest]DataSourceRequest request, GenericProjectMaterial projectMaterial)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectMaterial
                {
                    GenericProjectMaterialId = projectMaterial.GenericProjectMaterialId,
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
                    GenericWorkItemPerProjectId = projectMaterial.GenericWorkItemPerProjectId,
                    ItemCode = projectMaterial.ItemCode,
                };

                db.GenericProjectMaterial.Attach(entity);
                db.GenericProjectMaterial.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

            }

            return Json(new[] { projectMaterial }.ToDataSourceResult(request, ModelState));
        }
        #region Methods

        //private decimal GetMaterialUnitCost(int id)
        //{
        //    decimal labourCost = 0;
        //    totalMaterialCost = 0;
        //    var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

        //    if (workItem != null)
        //    {
        //        var labouCosts =
        //            db.ProjectMaterial.Where(x => x.WorkItemPerProjectId == id).ToList();
        //        if (labouCosts.Any())
        //        {


        //            labourCost = labouCosts.Sum(x => x.TotalCost);

        //        }
        //        labourCost = labourCost / workItem.TotalQuantityOfWork;
        //    }
        //    totalMaterialCost = labourCost;
        //    return labourCost;
        //}
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
        public decimal GetUnitPrice(string itemCode)
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