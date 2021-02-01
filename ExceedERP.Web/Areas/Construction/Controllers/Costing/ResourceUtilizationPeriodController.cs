using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ResourceUtilizationPeriodController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Periods_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            IQueryable<ResourceUtilizationPeriod> utilizationPeriods = db.ResourceUtilizationPeriods.Where(x => x.ProjectInformationId == projectId);
            DataSourceResult result = utilizationPeriods.ToDataSourceResult(request, period => new
            {
                ResourceUtilizationPeriodId = period.ResourceUtilizationPeriodId,
                ConstructionPeriodId = period.ConstructionPeriodId,
                ProjectInformationId = period.ProjectInformationId,
                Description = period.Description,

                OrgBranchName = period.OrgBranchName,
                DateCreated = period.DateCreated,
                LastModified = period.LastModified,
                CreatedBy = period.CreatedBy,
                ModifiedBy = period.ModifiedBy
            });

            return Json(result);
        }

        private bool PeriodExists(ResourceUtilizationPeriod model, int projectId)
        {
            bool periodExists = false;
            var temp = db.ResourceUtilizationPeriods.FirstOrDefault(p => p.ConstructionPeriodId == model.ConstructionPeriodId && p.ProjectInformationId == projectId);
            if (temp != null)
            {
                periodExists = true;
            }
            return periodExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Create([DataSourceRequest]DataSourceRequest request, ResourceUtilizationPeriod period, int projectId)
        {
            if (ModelState.IsValid)
            {
                if (PeriodExists(period, projectId))
                {
                    ModelState.AddModelError("", "Period Exists Already!!");
                }
                else
                {
                    var entity = new ResourceUtilizationPeriod
                    {
                        ConstructionPeriodId = period.ConstructionPeriodId,
                        ProjectInformationId = projectId,
                        Description = period.Description,

                        OrgBranchName = period.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy

                    };

                    db.ResourceUtilizationPeriods.Add(entity);
                    db.SaveChanges();
                    period.ResourceUtilizationPeriodId = entity.ResourceUtilizationPeriodId;

                    //this.CopyAllWorkItemsToUtilizationPeriod(entity);
                    this.AddDaysWithInPeriodRange(entity);
                    //this.CopyAllWorkItemsToUtilizationPeriod(entity);
                }
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Update([DataSourceRequest]DataSourceRequest request, ResourceUtilizationPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationPeriod
                {
                    ResourceUtilizationPeriodId = period.ResourceUtilizationPeriodId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    ProjectInformationId = period.ProjectInformationId,
                    Description = period.Description,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ResourceUtilizationPeriods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Destroy([DataSourceRequest]DataSourceRequest request, ResourceUtilizationPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationPeriod
                {
                    ResourceUtilizationPeriodId = period.ResourceUtilizationPeriodId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    ProjectInformationId = period.ProjectInformationId,
                    Description = period.Description,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = period.LastModified,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = period.ModifiedBy
                };

                db.ResourceUtilizationPeriods.Attach(entity);
                db.ResourceUtilizationPeriods.Remove(entity);
                db.SaveChanges();

                this.RemoveDaysUnderPeriod(entity);

            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private void AddDaysWithInPeriodRange(ResourceUtilizationPeriod period)
        {
            var currentPeriod = db.ConstructionPeriod.Find(period.ConstructionPeriodId);
            if (currentPeriod != null)
            {
                DateTime start = currentPeriod.DateFrom;
                DateTime end = currentPeriod.DateTo;
                DateTime currentDateStart = start;
                TimeSpan span = end - start;
                TimeSpan daySpan = ((start.AddDays(1)) - (start.AddMilliseconds(10)));
                int days = span.Days + 1;
                for (int d = 1; d <= days; d++)
                {
                    var entity = new ResourceUtilizationDay
                    {
                        Name = currentDateStart.ToString("MMMM dd"),
                        DateFrom = currentDateStart,
                        DateTo = currentDateStart.Add(daySpan),
                        ResourceUtilizationPeriodId = period.ResourceUtilizationPeriodId
                    };
                    currentDateStart = entity.DateFrom.AddDays(1);
                    db.ResourceUtilizationDays.Add(entity);
                    db.SaveChanges();
                }

            }
            //this.CopyAllWorkItemsToUtilizationPeriod(period);
        }
        private void CopyAllWorkItemsToUtilizationPeriod(ResourceUtilizationPeriod period)
        {
            var currentProject = db.ProjectInformation.FirstOrDefault(p => p.ProjectInformationId == period.ProjectInformationId);
            if (currentProject != null)
            {
                var defaultMethodOfWork = db.MethodOfWork.FirstOrDefault();

               var subCategoryWorkItmes = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == currentProject.ProjectInformationId && wi.IsSubCategory).ToList();

                var noneSubCategoryWorkItems = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == currentProject.ProjectInformationId && !wi.IsSubCategory).ToList();

                var resourceUtilDays = db.ResourceUtilizationDays.Where(d => d.ResourceUtilizationPeriodId == period.ResourceUtilizationPeriodId).ToList();
                if (resourceUtilDays.Any() && (noneSubCategoryWorkItems.Any() || subCategoryWorkItmes.Any()))
                {
                    foreach (ResourceUtilizationDay day in resourceUtilDays)
                    {
                        #region add work items with child
                        foreach (WorkItemPerProject workItem in subCategoryWorkItmes)
                        {
                            var existingEntity = db.ResourceUtilizationWorkItem.FirstOrDefault(rw => rw.WorkItemPerProjectId == workItem.WorkItemPerProjectId && rw.ResourceUtilizationDayId == day.ResourceUtilizationDayId);
                            if (existingEntity == null && defaultMethodOfWork != null)
                            {
                                var entity = new ResourceUtilizationWorkItem
                                {
                                    MethodOfWorkId = defaultMethodOfWork.MethodOfWorkId,
                                    WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                                    ResourceUtilizationDayId = day.ResourceUtilizationDayId,
                                    ItemName = workItem.ItemName,
                                    IsSubCategory = workItem.IsSubCategory,
                                    ParentId = 0,
                                    UnitOfMeasurnment = this.GetWorkItemUnitOfMeasurnment(workItem.WorkItemPerProjectId).Abbreviation,
                                    ExecutedWork = 1,
                                    WorkDurationInHoursPerDay = 8,
                                    OutPutPerDay = workItem.LabourHourlyOutPut * 8,
                                    DirectContractUnitPrice = workItem.TotalUnitCost,
                                    //Income = workItem.Income,
                                    //TotalDirectCost = workItem.DirectCost,
                                    MaterialCost = 0,
                                    EquipmentCost = 0,
                                    LaborCost = 0,
                                    TotalCost = 0,
                                    ProfitCost = 0,
                                    OverheadCost = 0,

                                    OrgBranchName = null,
                                    DateCreated = DateTime.Now,
                                    LastModified = null,
                                    CreatedBy = null,
                                    ModifiedBy = null,

                                    SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId),
                                    BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId),
                                    StructureyType = workItem.WorkCategory.Configuration.CurrentValue.ToString(),
                                    WorkCategory = workItem.WorkCategory.Name,

                                };
                                entity.Income = entity.ExecutedWork * entity.DirectContractUnitPrice;
                                entity.OutPutPerDay = entity.ExecutedWork / (entity.WorkDurationInHoursPerDay / 8);
                                entity.TotalDirectCost = entity.MaterialCost + entity.EquipmentCost + entity.LaborCost;
                                entity.TotalCost = entity.TotalDirectCost + entity.ProfitCost + entity.OverheadCost;

                                db.ResourceUtilizationWorkItem.Add(entity);
                                db.SaveChanges();

                                #region add child work items
                                var childWorkItems = noneSubCategoryWorkItems.Where(w => w.ParentId == workItem.WorkItemPerProjectId).ToList();
                                if (childWorkItems.Any())
                                {
                                    foreach (WorkItemPerProject childWorkItem in childWorkItems)
                                    {
                                        var existingChildWorkItem = db.ResourceUtilizationWorkItem.FirstOrDefault(ruw => ruw.WorkItemPerProjectId == childWorkItem.WorkItemPerProjectId && ruw.ResourceUtilizationDayId == day.ResourceUtilizationDayId);
                                        if (existingChildWorkItem == null)
                                        {
                                            var childResourceUtlWorkItem = new ResourceUtilizationWorkItem
                                            {
                                                MethodOfWorkId = defaultMethodOfWork.MethodOfWorkId,
                                                WorkItemPerProjectId = childWorkItem.WorkItemPerProjectId,
                                                ResourceUtilizationDayId = day.ResourceUtilizationDayId,
                                                ItemName = childWorkItem.ItemName,
                                                IsSubCategory = childWorkItem.IsSubCategory,
                                                ParentId = entity.ResourceUtilizationWorkItemId,
                                                UnitOfMeasurnment = this.GetWorkItemUnitOfMeasurnment(childWorkItem.WorkItemPerProjectId).Abbreviation,
                                                ExecutedWork = 1,
                                                WorkDurationInHoursPerDay = 8,
                                                OutPutPerDay = childWorkItem.LabourHourlyOutPut * 8,
                                                DirectContractUnitPrice = childWorkItem.TotalUnitCost,
                                                //Income = workItem.Income,
                                                //TotalDirectCost = workItem.DirectCost,
                                                MaterialCost = 0,
                                                EquipmentCost = 0,
                                                LaborCost = 0,
                                                TotalCost = 0,
                                                ProfitCost = 0,
                                                OverheadCost = 0,

                                                OrgBranchName = null,
                                                DateCreated = DateTime.Now,
                                                LastModified = null,
                                                CreatedBy = null,
                                                ModifiedBy = null,

                                                SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(childWorkItem.BlockId),
                                                BlockName = ProjectEstimationHelper.GetBlockName(childWorkItem.BlockId),
                                                StructureyType = childWorkItem.WorkCategory.Configuration.CurrentValue.ToString(),
                                                WorkCategory = childWorkItem.WorkCategory.Name,
                                            };
                                            childResourceUtlWorkItem.Income = childResourceUtlWorkItem.ExecutedWork * childResourceUtlWorkItem.DirectContractUnitPrice;
                                            childResourceUtlWorkItem.OutPutPerDay = childResourceUtlWorkItem.ExecutedWork / (childResourceUtlWorkItem.WorkDurationInHoursPerDay / 8);
                                            childResourceUtlWorkItem.TotalDirectCost = childResourceUtlWorkItem.MaterialCost + childResourceUtlWorkItem.EquipmentCost + childResourceUtlWorkItem.LaborCost;
                                            childResourceUtlWorkItem.TotalCost = childResourceUtlWorkItem.TotalDirectCost + childResourceUtlWorkItem.ProfitCost + childResourceUtlWorkItem.OverheadCost;

                                            db.ResourceUtilizationWorkItem.Add(childResourceUtlWorkItem);
                                            db.SaveChanges();
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        #endregion
                        #region add work items with no child
                        foreach (WorkItemPerProject workItem in noneSubCategoryWorkItems)
                        {
                            var existingEntity = db.ResourceUtilizationWorkItem.FirstOrDefault(rw => rw.WorkItemPerProjectId == workItem.WorkItemPerProjectId && rw.ResourceUtilizationDayId == day.ResourceUtilizationDayId);
                            if (existingEntity == null && defaultMethodOfWork != null)
                            {
                                var entity = new ResourceUtilizationWorkItem
                                {
                                    MethodOfWorkId = defaultMethodOfWork.MethodOfWorkId,
                                    WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                                    ResourceUtilizationDayId = day.ResourceUtilizationDayId,
                                    ItemName = workItem.ItemName,
                                    IsSubCategory = workItem.IsSubCategory,
                                    ParentId = 0,
                                    UnitOfMeasurnment = this.GetWorkItemUnitOfMeasurnment(workItem.WorkItemPerProjectId).Abbreviation,
                                    ExecutedWork = 1,
                                    WorkDurationInHoursPerDay = 8,
                                    OutPutPerDay = workItem.LabourHourlyOutPut * 8,
                                    DirectContractUnitPrice = workItem.TotalUnitCost,
                                    //Income = workItem.Income,
                                    //TotalDirectCost = workItem.DirectCost,
                                    MaterialCost = 0,
                                    EquipmentCost = 0,
                                    LaborCost = 0,
                                    TotalCost = 0,
                                    ProfitCost = 0,
                                    OverheadCost = 0,

                                    OrgBranchName = null,
                                    DateCreated = DateTime.Now,
                                    LastModified = null,
                                    CreatedBy = null,
                                    ModifiedBy = null,

                                    SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId),
                                    BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId),
                                    StructureyType = workItem.WorkCategory.Configuration.CurrentValue.ToString(),
                                    WorkCategory = workItem.WorkCategory.Name,
                                };
                                entity.Income = entity.ExecutedWork * entity.DirectContractUnitPrice;
                                entity.OutPutPerDay = entity.ExecutedWork / (entity.WorkDurationInHoursPerDay / 8);
                                entity.TotalDirectCost = entity.MaterialCost + entity.EquipmentCost + entity.LaborCost;
                                entity.TotalCost = entity.TotalDirectCost + entity.ProfitCost + entity.OverheadCost;

                                db.ResourceUtilizationWorkItem.Add(entity);
                                db.SaveChanges();
                            }
                        } 
                        #endregion
                    }
                }
            }
        }

        private decimal GetMaterialCost(int id)
        {
            decimal matCost = new decimal();
            var materials = db.ResourceUtilizationMaterials.Where(m => m.ResourceUtilizationWorkItemId == id).ToList();
            if (materials.Any())
            {
                matCost = materials.Sum(m => m.TotalCost);
            }
            return matCost;
        }
        private decimal GetEquipmentCost(int id)
        {
            decimal equCost = new decimal();
            var equipments = db.ResourceUtilizationEquipment.Where(m => m.ResourceUtilizationWorkItemId == id).ToList();
            if (equipments.Any())
            {
                equCost = equipments.Sum(e => e.TotalCost);
            }
            return equCost;
        }
        private decimal GetLaborCost(int id)
        {
            decimal laborCost = new decimal();
            var labors = db.ResourceUtilizationDirectLabours.Where(dl => dl.ResourceUtilizationWorkItemId == id).ToList();
            if (labors.Any())
            {
                laborCost = labors.Sum(m => m.TotalCost);
            }
            return laborCost;
        }
        private WorkCategory GetWorkItemCategory(int workItemId)
        {
            WorkCategory category = new WorkCategory();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            if (workItem != null)
            {
                var tempCategory = db.WorkCategory.Find(workItem.WorkCategoryId);
                if (tempCategory != null)
                {
                    category = tempCategory;
                }
            }

            return category;
        }
        private UnitOfMeasurement GetWorkItemUnitOfMeasurnment(int workItemId)
        {
            UnitOfMeasurement unit = new UnitOfMeasurement();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            if (workItem != null)
            {
                var tempUnit = db.UnitOfMeasurement.Find(workItem.UnitOfMeasurementId);
                if (tempUnit != null)
                {
                    unit = tempUnit;
                }
            }

            return unit;
        }
        private WorkItemPerProject GetWorkItem(int id)
        {
            WorkItemPerProject workItem = new WorkItemPerProject();
            var tempWorkItem = db.WorkItemPerProject.Find(id);
            if (tempWorkItem != null)
            {
                workItem = tempWorkItem;
            }
            return workItem;

        }
        private Block GetBlock(int? id)
        {
            Block block = new Block();
            if (id != null)
            {
                var temp = db.Blocks.Find(id);
                if (temp != null)
                {
                    block = temp;
                }
            }
            return block;

        }
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }
        private void RemoveDaysUnderPeriod(ResourceUtilizationPeriod period) {
            var days = db.ResourceUtilizationDays.Where(d => d.ResourceUtilizationPeriodId == period.ResourceUtilizationPeriodId).ToList();

            if (days.Any())
            {
                db.ResourceUtilizationDays.RemoveRange(days);
                db.SaveChanges();

                var ids = new HashSet<int>(days.Select(d => d.ResourceUtilizationDayId));
                var utlworkItems = db.ResourceUtilizationWorkItem.Where(w => (ids.Contains(w.ResourceUtilizationDayId)));
                if (utlworkItems.Any())
                {
                    db.ResourceUtilizationWorkItem.RemoveRange(utlworkItems);
                    db.SaveChanges();
                }
                
            }
        }
    }
}