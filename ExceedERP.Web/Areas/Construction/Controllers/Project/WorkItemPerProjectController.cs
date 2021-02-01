using ExceedERP.Core.Domain.Common;
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
    public class WorkItemPerProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult WorkItemPerProject_Read([DataSourceRequest]DataSourceRequest request)
        {
            //IQueryable<WorkItemPerProject> workItems = db.WorkItemPerProject.Where(x => x.ProjectInformationId == ProjectId && x.ParentId == 0);
            IQueryable<WorkItemPerProject> workItems = db.WorkItemPerProject.Where(x => x.ParentId == 0);

            DataSourceResult result = workItems.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                ItemCode = workItemPerProject.ItemCode,
                ItemName = workItemPerProject.ItemName,
                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });

            return Json(result);
        }

        public ActionResult WorkItemUnderWorkItem_Read([DataSourceRequest]DataSourceRequest request, int parentWorkItemId)
        {
            var currentWorkItem = db.WorkItemPerProject.Find(parentWorkItemId);
            if (currentWorkItem != null)
            {

            }
            IQueryable<WorkItemPerProject> workItems = db.WorkItemPerProject.Where(x => x.ParentId == parentWorkItemId);
            DataSourceResult result = workItems.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                ItemCode = workItemPerProject.ItemCode,
                ItemName = workItemPerProject.ItemName,
                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ActualQuantityOfWork = workItemPerProject.ActualQuantityOfWork,
                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });



            return Json(result);
        }
        // work items per a work category
        public ActionResult WorkItemPerCategory_Read([DataSourceRequest]DataSourceRequest request, int categoryId)
        {
            IQueryable<WorkItemPerProject> workitemperproject = db.WorkItemPerProject.Where(x => x.WorkCategoryId == categoryId);
            DataSourceResult result = workitemperproject.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                ItemCode = workItemPerProject.ItemCode,

                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });
            return Json(result);
        }

        /// <summary>
        /// returns the work items under a given Structure type
        /// </summary>
        /// <param name="request"></param>
        /// <param name="strId"></param>
        /// <returns></returns>
        public ActionResult WorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int strId, int blkId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId);
            List<WorkItemPerProject> workItems = new List<WorkItemPerProject>();
            BlockMapping currentBlock = new BlockMapping();
          
            BlockMapping otherOne = new BlockMapping();
            var temp2 = db.BlockMappings.Find(blkId);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.WorkItemPerProject.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.BlockId == temp2.BlockId && wi.ParentId == 0);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (WorkItemPerProject item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<WorkItemPerProject> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, workItemPerProject => new
            {
                WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
                ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
                IsSubCategory = workItemPerProject.IsSubCategory,
                ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                ItemCode = workItemPerProject.ItemCode,

                ItemName = workItemPerProject.ItemName,
                TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                DirectCost = workItemPerProject.DirectCost,
                MaterialCost = workItemPerProject.MaterialCost,
                LabourOutPut = workItemPerProject.LabourOutPut,
                EquipmentOutput = workItemPerProject.EquipmentOutput,
                MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                TotalUnitCost = workItemPerProject.TotalUnitCost,
                OverHeadCost = workItemPerProject.OverHeadCost,
                ProfitCost = workItemPerProject.ProfitCost,
                Remark = workItemPerProject.Remark,
                LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
                OverheadRate = workItemPerProject.OverheadRate,
                ProfitMargin = workItemPerProject.ProfitMargin,

                OrgBranchName = workItemPerProject.OrgBranchName,
                DateCreated = workItemPerProject.DateCreated,
                LastModified = workItemPerProject.LastModified,
                CreatedBy = workItemPerProject.CreatedBy,
                ModifiedBy = workItemPerProject.ModifiedBy,

                ParentId = workItemPerProject.ParentId,
                ProjectType = workItemPerProject.ProjectType,
                ProjectInformationId = workItemPerProject.ProjectInformationId,
                WorkCategoryId = workItemPerProject.WorkCategoryId
            });
            return Json(result);
        }
        public ActionResult SpecificWorkItemDetail_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            List<WorkItemDetailViewModel> list = new List<WorkItemDetailViewModel>();
            if (currentWorkItem != null)
            {
                #region add components
                var nameRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Item Name",
                    Value = currentWorkItem.ItemName
                };
                var qtyRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Qty of work",
                    Value = currentWorkItem.TotalQuantityOfWork.ToString("N2")
                };
                var materialCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Material Unit Cost",
                    Value = currentWorkItem.MaterialCostPerUnit.ToString("N2")
                };
                var equipmentCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Equipment Unit Cost",
                    Value = currentWorkItem.EquipmentCostPerUnit.ToString("N2")
                };
                var laborCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Manpower Unit Cost",
                    Value = currentWorkItem.LaborCostPerUnit.ToString("N2")
                };
                var directCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Direct Cost of Work Item",
                    Value = currentWorkItem.DirectCost.ToString("N2")
                };
                var profitCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Profit Cost (" + currentWorkItem.ProfitMargin + "%)",
                    Value = currentWorkItem.ProfitCost.ToString("N2")
                };
                var overheadCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Overhead Cost (" + currentWorkItem.OverheadRate + "%)",
                    Value = currentWorkItem.OverHeadCost.ToString("N2")
                };
                var totalCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Unit Cost",
                    Value = currentWorkItem.TotalUnitCost.ToString("N2")
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
                if (currentWorkItem.HasAdditionalTransportationCost)
                {
                    var totalcostIncludingTransportRecord = new WorkItemDetailViewModel
                    {
                        Attribute = "Total including transportation cost (" + currentWorkItem.TransportCostRate.ToString("N2") + "%)",
                        Value = currentWorkItem.TotalUnitCostIncludingTransport.ToString("N2")
                    };
                    list.Add(totalcostIncludingTransportRecord);
                }

            }
            //IQueryable<TemplateWorkItemDetailViewModel> result = list.AsQueryable();
            return Json(list.AsQueryable().ToDataSourceResult(request));
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult WorkItemPerProject_Create([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new WorkItemPerProject
        //        {
        //            ItemName = "name from other",
        //            //Name = "name",
        //            TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
        //            ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
        //            IsSubCategory = workItemPerProject.IsSubCategory,
        //            ItemCode = workItemPerProject.ItemCode,

        //            DirectCost = workItemPerProject.DirectCost,
        //            MaterialCost = workItemPerProject.MaterialCost,
        //            LabourOutPut = workItemPerProject.LabourOutPut,
        //            EquipmentOutput = workItemPerProject.EquipmentOutput,
        //            TotalUnitCost = workItemPerProject.TotalUnitCost,
        //            OverHeadCost = workItemPerProject.OverHeadCost,
        //            ProfitCost = workItemPerProject.ProfitCost,
        //            Remark = workItemPerProject.Remark,
        //            LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
        //            UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
        //            OverheadRate = workItemPerProject.OverheadRate,
        //            ProfitMargin = workItemPerProject.ProfitMargin,
        //            ParentId = workItemPerProject.ParentId,
        //            OrgBranchName = workItemPerProject.OrgBranchName,
        //            DateCreated = DateTime.Now,
        //            LastModified = workItemPerProject.LastModified,
        //            CreatedBy = User.Identity.Name,
        //            ModifiedBy = workItemPerProject.ModifiedBy,
        //            ProjectInformationId = ProjectId,
        //            WorkCategoryId = workItemPerProject.WorkCategoryId
        //        };
        //        entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ?
        //                                        (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
        //                                        entity.DirectCost + entity.OverHeadCost + entity.ProfitCost;
        //        db.WorkItemPerProject.Add(entity);
        //        db.SaveChanges();
        //        workItemPerProject.WorkItemPerProjectId = entity.WorkItemPerProjectId;
        //    }

        //    return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult WorkItemPerProject_Update([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new WorkItemPerProject
        //        {
        //            WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
        //            ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
        //            IsSubCategory = workItemPerProject.IsSubCategory,
        //            ItemCode = workItemPerProject.ItemCode,
        //            ItemName = string.IsNullOrEmpty(workItemPerProject.ItemName) ?
        //                GetEstimationWorkItem(workItemPerProject.ProjectEstimationWorkItemId) :
        //                workItemPerProject.ItemName,
        //            BlockId = workItemPerProject.BlockId,

        //            TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
        //            DirectCost = workItemPerProject.DirectCost,
        //            MaterialCost = workItemPerProject.MaterialCost,
        //            LabourOutPut = workItemPerProject.LabourOutPut,
        //            EquipmentOutput = workItemPerProject.EquipmentOutput,
        //            TotalUnitCost = workItemPerProject.TotalUnitCost,
        //            OverHeadCost = workItemPerProject.OverHeadCost,
        //            ProfitCost = workItemPerProject.ProfitCost,
        //            ProjectType = workItemPerProject.ProjectType,
        //            Remark = workItemPerProject.Remark,
        //            LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
        //            UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
        //            OverheadRate = workItemPerProject.OverheadRate,
        //            ProfitMargin = workItemPerProject.ProfitMargin,
        //            ParentId = workItemPerProject.ParentId,
        //            ProjectInformationId = workItemPerProject.ProjectInformationId,
        //            WorkCategoryId = workItemPerProject.WorkCategoryId,

        //            OrgBranchName = workItemPerProject.OrgBranchName,
        //            DateCreated = workItemPerProject.DateCreated,
        //            LastModified = DateTime.Now,
        //            CreatedBy = workItemPerProject.CreatedBy,
        //            ModifiedBy = User.Identity.Name,

        //        };

        //        entity.TotalUnitCost = entity.LabourHourlyOutPut > 0 ? (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost) / entity.LabourHourlyOutPut :
        //                                     (entity.DirectCost + entity.OverHeadCost + entity.ProfitCost);
        //        db.WorkItemPerProject.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult WorkItemPerProject_Destroy([DataSourceRequest]DataSourceRequest request, WorkItemPerProject workItemPerProject)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new WorkItemPerProject
        //        {
        //            WorkItemPerProjectId = workItemPerProject.WorkItemPerProjectId,
        //            ProjectEstimationWorkItemId = workItemPerProject.ProjectEstimationWorkItemId,
        //            IsSubCategory = workItemPerProject.IsSubCategory,
        //            ItemCode = workItemPerProject.ItemCode,

        //            TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
        //            DirectCost = workItemPerProject.DirectCost,
        //            MaterialCost = workItemPerProject.MaterialCost,
        //            LabourOutPut = workItemPerProject.LabourOutPut,
        //            EquipmentOutput = workItemPerProject.EquipmentOutput,
        //            TotalUnitCost = workItemPerProject.TotalUnitCost,
        //            OverHeadCost = workItemPerProject.OverHeadCost,
        //            ProfitCost = workItemPerProject.ProfitCost,
        //            ProjectType = workItemPerProject.ProjectType,
        //            Remark = workItemPerProject.Remark,
        //            LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
        //            UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId,
        //            OverheadRate = workItemPerProject.OverheadRate,
        //            ProfitMargin = workItemPerProject.ProfitMargin,

        //            ParentId = workItemPerProject.ParentId,
        //            OrgBranchName = workItemPerProject.OrgBranchName,
        //            DateCreated = workItemPerProject.DateCreated,
        //            LastModified = workItemPerProject.LastModified,
        //            CreatedBy = workItemPerProject.CreatedBy,
        //            ModifiedBy = workItemPerProject.ModifiedBy,
        //            ProjectInformationId = workItemPerProject.ProjectInformationId,
        //            WorkCategoryId = workItemPerProject.WorkCategoryId,



        //        };

        //        db.WorkItemPerProject.Attach(entity);
        //        db.WorkItemPerProject.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { workItemPerProject }.ToDataSourceResult(request, ModelState));
        //}
        private string GetEstimationWorkItem(int id)
        {
            ProjectEstimationWorkItem workItem = new ProjectEstimationWorkItem();
            var model = db.ProjectEstimationWorkItem.Find(id);
            if (model != null)
            {
                workItem = model;
            }
            return workItem.ItemName;
            //return "temporary name";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}