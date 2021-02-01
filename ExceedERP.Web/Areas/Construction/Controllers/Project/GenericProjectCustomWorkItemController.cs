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

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class GenericProjectCustomWorkItemController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult CustomWorkItem_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<GenericProjectCustomWorkItem> customWorkItems = db.GenericProjectCustomWorkItem;
            DataSourceResult result = customWorkItems.ToDataSourceResult(request, customWorkItem => new
            {
                GenericProjectCustomWorkItemId = customWorkItem.GenericProjectCustomWorkItemId,
                ProjectInformationId = customWorkItem.ProjectInformationId,
                SubProjectTypeId = customWorkItem.SubProjectTypeId,
                ProjectType = customWorkItem.ProjectType,
                WorkCategoryId = customWorkItem.WorkCategoryId,
                WorkItemName = customWorkItem.WorkItemName,
                TotalUnitRate = customWorkItem.TotalUnitRate,
                TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                TotalCost = customWorkItem.TotalCost,
                Remark = customWorkItem.Remark,
                ItemCode = customWorkItem.ItemCode,

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
        public ActionResult CustomWorkItemPerStructure_Read([DataSourceRequest]DataSourceRequest request, int strId, int subProjectMappingId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId);
            List<GenericProjectCustomWorkItem> workItems = new List<GenericProjectCustomWorkItem>();
            SubProjectStructureTypeMapping currentBlock = new SubProjectStructureTypeMapping();

            SubProjectStructureTypeMapping otherOne = new SubProjectStructureTypeMapping();
            var temp2 = db.SubProjectStructureTypeMapping.Find(subProjectMappingId);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.GenericProjectCustomWorkItem.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.SubProjectTypeId == temp2.SubProjectTypeId);
                if (workItemList.Any())
                {
                    foreach (GenericProjectCustomWorkItem item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<GenericProjectCustomWorkItem> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, customWorkItem => new
            {
                GenericProjectCustomWorkItemId = customWorkItem.GenericProjectCustomWorkItemId,
                ProjectInformationId = customWorkItem.ProjectInformationId,
                SubProjectTypeId = customWorkItem.SubProjectTypeId,
                ProjectType = customWorkItem.ProjectType,
                WorkCategoryId = customWorkItem.WorkCategoryId,
                WorkItemName = customWorkItem.WorkItemName,
                TotalUnitRate = customWorkItem.TotalUnitRate,
                TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                TotalCost = customWorkItem.TotalCost,
                Remark = customWorkItem.Remark,
                ItemCode = customWorkItem.ItemCode,

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
                ModifiedBy = customWorkItem.ModifiedBy
            });

            return Json(result);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Create([DataSourceRequest]DataSourceRequest request, GenericProjectCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectCustomWorkItem
                {
                    ProjectInformationId = customWorkItem.ProjectInformationId,
                    SubProjectTypeId = customWorkItem.SubProjectTypeId,
                    ProjectType = customWorkItem.ProjectType,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    WorkItemName = customWorkItem.WorkItemName,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                    TotalCost = customWorkItem.TotalCost,
                    Remark = customWorkItem.Remark,
                    ItemCode = customWorkItem.ItemCode,

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
                    DateCreated = DateTime.Now.Date,
                    LastModified = customWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = customWorkItem.ModifiedBy,
                };

                db.GenericProjectCustomWorkItem.Add(entity);
                db.SaveChanges();
                customWorkItem.GenericProjectCustomWorkItemId = entity.GenericProjectCustomWorkItemId;

            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Update([DataSourceRequest]DataSourceRequest request, GenericProjectCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectCustomWorkItem
                {
                    GenericProjectCustomWorkItemId = customWorkItem.GenericProjectCustomWorkItemId,
                    ProjectInformationId = customWorkItem.ProjectInformationId,
                    SubProjectTypeId = customWorkItem.SubProjectTypeId,
                    ProjectType = customWorkItem.ProjectType,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    WorkItemName = customWorkItem.WorkItemName,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                    TotalCost = customWorkItem.TotalQuantityOfWork * customWorkItem.TotalUnitRate,
                    Remark = customWorkItem.Remark,
                    ItemCode = customWorkItem.ItemCode,

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
                    LastModified = DateTime.Now.Date,
                    CreatedBy = customWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.GenericProjectCustomWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, GenericProjectCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectCustomWorkItem
                {
                    GenericProjectCustomWorkItemId = customWorkItem.GenericProjectCustomWorkItemId,
                    ProjectInformationId = customWorkItem.ProjectInformationId,
                    SubProjectTypeId = customWorkItem.SubProjectTypeId,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    ProjectType = customWorkItem.ProjectType,
                    WorkItemName = customWorkItem.WorkItemName,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    TotalQuantityOfWork = customWorkItem.TotalQuantityOfWork,
                    TotalCost = customWorkItem.TotalCost,
                    Remark = customWorkItem.Remark,
                    ItemCode = customWorkItem.ItemCode,

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

                db.GenericProjectCustomWorkItem.Attach(entity);
                db.GenericProjectCustomWorkItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}