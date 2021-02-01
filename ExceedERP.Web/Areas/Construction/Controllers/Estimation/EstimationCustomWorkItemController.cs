using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstimationCustomWorkItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index(string name)
        {
            return View();
        }

        public ActionResult CustomWorkItem_Read([DataSourceRequest]DataSourceRequest request, int type)
        {
            var projectType = (AvailableProjectTypes)type;
            IQueryable<ProjectEstimationCustomWorkItem> customWorkItems = db.ProjectEstimationCustomWorkItems
                .Where(x => x.ProjectType == projectType);
            DataSourceResult result = customWorkItems.ToDataSourceResult(request, customWorkItem => new
            {
                ProjectEstimationCustomWorkItemId = customWorkItem.ProjectEstimationCustomWorkItemId,
                WorkCategoryId = customWorkItem.WorkCategoryId,
                WorkItemName = customWorkItem.WorkItemName,
                ItemCode = customWorkItem.ItemCode,

                ProjectType = customWorkItem.ProjectType,
                TotalUnitRate = customWorkItem.TotalUnitRate,
                Remark = customWorkItem.Remark,
                LaborCost = customWorkItem.LaborCost,
                MaterialCost = customWorkItem.MaterialCost,
                EquipmentCost = customWorkItem.EquipmentCost,

                TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                HourlyOutPut = customWorkItem.HourlyOutPut,
                UnitOfMeasurementId = customWorkItem.UnitOfMeasurementId,
                ProfitRate = customWorkItem.ProfitRate,
                OverheadRate = customWorkItem.OverheadRate,
                ProfitCost = customWorkItem.ProfitCost,
                OverHeadCost = customWorkItem.OverHeadCost,
                MaterialCostPerUnit = customWorkItem.MaterialCostPerUnit,
                LaborCostPerUnit = customWorkItem.LaborCostPerUnit,
                EquipmentCostPerUnit = customWorkItem.EquipmentCostPerUnit,
                DirectCost = customWorkItem.DirectCost,

                OrgBranchName = customWorkItem.OrgBranchName,
                DateCreated = customWorkItem.DateCreated,
                LastModified = customWorkItem.LastModified,
                CreatedBy = customWorkItem.CreatedBy,
                ModifiedBy = customWorkItem.ModifiedBy,
            });

            return Json(result);

        }
        public ActionResult SpecificWorkItemDetail_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            var currentWorkItem = db.ProjectEstimationCustomWorkItems.Find(itemId);
            List<WorkItemDetailViewModel> list = new List<WorkItemDetailViewModel>();
            if (currentWorkItem != null)
            {
                #region add components
                var nameRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Item Name",
                    Value = currentWorkItem.WorkItemName
                };
                var qtyRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Qty of work",
                    Value = currentWorkItem.TotalQuantityOfWork.ToString("N4")
                };
                var materialCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Material Unit Cost",
                    Value = currentWorkItem.MaterialCostPerUnit.ToString("N4")
                };
                var equipmentCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Equipment Unit Cost",
                    Value = currentWorkItem.EquipmentCostPerUnit.ToString("N4")
                };
                var laborCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Manpower Unit Cost",
                    Value = currentWorkItem.LaborCostPerUnit.ToString("N4")
                };
                var directCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Direct Cost of Work Item",
                    Value = currentWorkItem.DirectCost.ToString("N4")
                };
                var profitCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Profit Cost (" + currentWorkItem.ProfitRate + "%)",
                    Value = currentWorkItem.ProfitCost.ToString("N4")
                };
                var overheadCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Overhead Cost (" + currentWorkItem.OverheadRate + "%)",
                    Value = currentWorkItem.OverHeadCost.ToString("N4")
                };
                var totalCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Unit Cost",
                    Value = currentWorkItem.TotalUnitRate.ToString("N4")
                };
                #endregion
                //list.Add(nameRecord);
                //list.Add(qtyRecord);

                list.Add(materialCostRecord);
                list.Add(equipmentCostRecord);
                list.Add(laborCostRecord);
                list.Add(directCostRecord);
                list.Add(profitCostRecord);
                list.Add(overheadCostRecord);
                list.Add(totalCostRecord);

            }
            //IQueryable<TemplateWorkItemDetailViewModel> result = list.AsQueryable();
            return Json(list.AsQueryable().ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationCustomWorkItem customWorkItem, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationCustomWorkItem
                {
                    ProjectEstimationCustomWorkItemId = customWorkItem.ProjectEstimationCustomWorkItemId,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    WorkItemName = customWorkItem.WorkItemName,
                    ItemCode = customWorkItem.ItemCode,
                    ProjectType = projectType,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    Remark = customWorkItem.Remark,
                    LaborCost = customWorkItem.LaborCost,
                    MaterialCost = customWorkItem.MaterialCost,
                    EquipmentCost = customWorkItem.EquipmentCost,

                    TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                    HourlyOutPut = customWorkItem.HourlyOutPut,
                    UnitOfMeasurementId = customWorkItem.UnitOfMeasurementId,
                    ProfitRate = ConsSettingHelper.GetCurrentProfitRate(),
                    OverheadRate = ConsSettingHelper.GetCurrentOverHeadRate(),
                    ProfitCost = customWorkItem.ProfitCost,
                    OverHeadCost = customWorkItem.OverHeadCost,
                    MaterialCostPerUnit = customWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = customWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = customWorkItem.EquipmentCostPerUnit,
                    DirectCost = customWorkItem.DirectCost,

                    OrgBranchName = customWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = customWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = customWorkItem.ModifiedBy,
                };

                db.ProjectEstimationCustomWorkItems.Add(entity);
                db.SaveChanges();
                customWorkItem.ProjectEstimationCustomWorkItemId = entity.ProjectEstimationCustomWorkItemId;

            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationCustomWorkItem
                {
                    ProjectEstimationCustomWorkItemId = customWorkItem.ProjectEstimationCustomWorkItemId,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    WorkItemName = customWorkItem.WorkItemName,
                    ItemCode = customWorkItem.ItemCode,
                    ProjectType = customWorkItem.ProjectType,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    Remark = customWorkItem.Remark,
                    LaborCost = customWorkItem.LaborCost,
                    MaterialCost = customWorkItem.MaterialCost,
                    EquipmentCost = customWorkItem.EquipmentCost,

                    TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                    HourlyOutPut = customWorkItem.HourlyOutPut,
                    UnitOfMeasurementId = customWorkItem.UnitOfMeasurementId,
                    ProfitRate = ConsSettingHelper.GetCurrentProfitRate(),
                    OverheadRate = ConsSettingHelper.GetCurrentOverHeadRate(),
                    ProfitCost = customWorkItem.ProfitCost,
                    OverHeadCost = customWorkItem.OverHeadCost,
                    MaterialCostPerUnit = customWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = customWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = customWorkItem.EquipmentCostPerUnit,
                    DirectCost = customWorkItem.DirectCost,

                    OrgBranchName = customWorkItem.OrgBranchName,
                    DateCreated = customWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = customWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.ProjectEstimationCustomWorkItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);


            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationCustomWorkItem
                {
                    ProjectEstimationCustomWorkItemId = customWorkItem.ProjectEstimationCustomWorkItemId,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    WorkItemName = customWorkItem.WorkItemName,
                    ItemCode = customWorkItem.ItemCode,
                    ProjectType = customWorkItem.ProjectType,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    Remark = customWorkItem.Remark,
                    LaborCost = customWorkItem.LaborCost,
                    MaterialCost = customWorkItem.MaterialCost,
                    EquipmentCost = customWorkItem.EquipmentCost,

                    TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                    HourlyOutPut = customWorkItem.HourlyOutPut,
                    UnitOfMeasurementId = customWorkItem.UnitOfMeasurementId,
                    ProfitRate = customWorkItem.ProfitRate,
                    OverheadRate = customWorkItem.OverheadRate,
                    ProfitCost = customWorkItem.ProfitCost,
                    OverHeadCost = customWorkItem.OverHeadCost,
                    MaterialCostPerUnit = customWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = customWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = customWorkItem.EquipmentCostPerUnit,
                    DirectCost = customWorkItem.DirectCost,

                    OrgBranchName = customWorkItem.OrgBranchName,
                    DateCreated = customWorkItem.DateCreated,
                    LastModified = customWorkItem.LastModified,
                    CreatedBy = customWorkItem.CreatedBy,
                    ModifiedBy = customWorkItem.ModifiedBy,
                };

                db.ProjectEstimationCustomWorkItems.Attach(entity);
                db.ProjectEstimationCustomWorkItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }
        //public static void UpdateCustomWorkItem(int id)
        //{
        //var db = new ExceedDbContext();
        //decimal hourlyOutPut = 1;

        //var currentWorkItem = db.ProjectEstimationCustomWorkItems.Find(id);
        //var contents = db.EstimationCustomWorkItemContents.Where(c => c.ProjectEstimationCustomWorkItemId == id).ToList();
        //var materials = db.EstimationCustomWorkItemMaterial.Where(m => m.ProjectEstimationCustomWorkItemId == id).ToList();
        //var equimplents = db.EstimationCustomWorkItemEquipment.Where(e => e.ProjectEstimationCustomWorkItemId == id).ToList();
        //var labors = db.EstimationCustomWorkItemDirectLabor.Where(l => l.ProjectEstimationCustomWorkItemId == id).ToList();

        //var compCost = contents.Sum(c => c.UnitPrice);
        //var materialCost = materials.Sum(m => m.TotalCost);
        //var equCost = equimplents.Sum(e => e.TotalCost);
        //var laborCost = labors.Sum(l => l.TotalCost);

        //if(currentWorkItem != null)
        //{
        //    hourlyOutPut = currentWorkItem.HourlyOutPut > 0 ? currentWorkItem.HourlyOutPut : 1;

        //    currentWorkItem.MaterialCost = materialCost + compCost;
        //    currentWorkItem.EquipmentCost = equCost;
        //    currentWorkItem.LaborCost = laborCost;

        //    currentWorkItem.MaterialCostPerUnit = (materialCost + compCost) / hourlyOutPut;
        //    currentWorkItem.EquipmentCostPerUnit = equCost / hourlyOutPut;
        //    currentWorkItem.LaborCostPerUnit = laborCost / hourlyOutPut;

        //    currentWorkItem.DirectCost = currentWorkItem.MaterialCostPerUnit + currentWorkItem.LaborCostPerUnit + currentWorkItem.EquipmentCostPerUnit;
        //    currentWorkItem.OverHeadCost = (currentWorkItem.OverheadRate / 100) * currentWorkItem.DirectCost;
        //    currentWorkItem.ProfitCost = (currentWorkItem.ProfitRate / 100) * currentWorkItem.DirectCost;

        //    currentWorkItem.TotalUnitRate = currentWorkItem.DirectCost + currentWorkItem.OverHeadCost + currentWorkItem.ProfitCost;

        //    db.ProjectEstimationCustomWorkItems.Attach(currentWorkItem);
        //    db.Entry(currentWorkItem).State = EntityState.Modified;
        //    db.SaveChanges();
        //}

        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}