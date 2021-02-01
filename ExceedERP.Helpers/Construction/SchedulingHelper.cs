using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Helpers.Construction
{
    public static class SchedulingHelper
    {
        private static ExceedDbContext db = new ExceedDbContext();

        public static void PrepareScheduleBreakDown(int id)
        {
            var currentTask = db.Tasks.Find(id);
            if (currentTask != null)
            {
                //foreach (Task task in allScheduleTasks)
                //{
                DateTime start = currentTask.Start;
                DateTime end = currentTask.End;

                var periodsInTimeRange = db.ConstructionPeriod.Where(p => p.DateFrom <= start && p.DateTo >= start ||
                                                                          p.DateFrom >= start && p.DateTo <= end ||
                                                                          p.DateFrom <= end && p.DateTo >= end
                                                                    ).ToList();
                if (periodsInTimeRange.Any() && currentTask.WorkItemDuration != null)
                {
                    var numOfPeriods = periodsInTimeRange.Count;
                    var workItem = currentTask.WorkItemDuration;


                    foreach (ConstructionPeriod period in periodsInTimeRange)
                    {
                        var subPeriods = db.ConstructionSubPeriod.Where(sp => sp.ConstructionPeriodId == period.ConstructionPeriodId).ToList();
                        var existingItem = db.TaskDetail.FirstOrDefault(td => td.ID == currentTask.ID && td.ConstructionPeriodId == period.ConstructionPeriodId);
                        if (existingItem == null)
                        {
                            var entity = new TaskDetail
                            {
                                ConstructionPeriodId = period.ConstructionPeriodId,
                                ID = currentTask.ID,
                                PlannedQuantityOfWork = currentTask.TotalQuantityOfWork / numOfPeriods,
                                //PlannedMaterialCost = currentTask.MaterialCost / numOfPeriods,
                                //PlannedEquipmentCost = currentTask.EquipmentCost / numOfPeriods,
                                //PlannedLaborCost = currentTask.LabourCost / numOfPeriods,

                            };
                            entity.PlannedMaterialCost = entity.PlannedQuantityOfWork * workItem.MaterialCost;
                            entity.PlannedEquipmentCost = entity.PlannedQuantityOfWork * workItem.EquipmentCost;
                            entity.PlannedLaborCost = entity.PlannedQuantityOfWork * workItem.LabourCost;
                            entity.PlannedProfitCost = entity.PlannedQuantityOfWork * workItem.ProfitCost;
                            entity.PlannedOverHeadCost = entity.PlannedQuantityOfWork * workItem.OverHeadCost;

                            entity.PlannedDirectCost = entity.PlannedMaterialCost + entity.PlannedEquipmentCost + entity.PlannedLaborCost;
                            entity.TotalCost = entity.PlannedDirectCost + entity.PlannedProfitCost + entity.PlannedOverHeadCost;

                            db.TaskDetail.Add(entity);
                            db.SaveChanges();

                            #region further break down
                            if (subPeriods.Any())
                            {
                                var numOfSubPeriods = subPeriods.Count;
                                foreach (ConstructionSubPeriod subPeriod in subPeriods)
                                {
                                    var existingSubPeriodData = db.TaskDetailBreakDown
                                        .FirstOrDefault(b => b.TaskDetailId == entity.TaskDetailId &&
                                        b.ConstructionSubPeriodId == subPeriod.ConstructionSubPeriodId);
                                    if (existingSubPeriodData == null)
                                    {
                                        var subPeriodDataToAdd = new TaskDetailBreakDown
                                        {
                                            TaskDetailId = entity.TaskDetailId,
                                            ConstructionSubPeriodId = subPeriod.ConstructionSubPeriodId,
                                            PlannedQuantityOfWork = entity.PlannedQuantityOfWork / numOfSubPeriods,
                                            //PlannedMaterialCost = entity.PlannedMaterialCost / numOfSubPeriods,
                                            //PlannedEquipmentCost = entity.PlannedEquipmentCost / numOfSubPeriods,
                                            //PlannedLaborCost = entity.PlannedLaborCost / numOfSubPeriods,

                                        };
                                        subPeriodDataToAdd.PlannedMaterialCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.MaterialCost;
                                        subPeriodDataToAdd.PlannedLaborCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.LabourCost;
                                        subPeriodDataToAdd.PlannedEquipmentCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.EquipmentCost;
                                        subPeriodDataToAdd.PlannedProfitCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.ProfitCost;
                                        subPeriodDataToAdd.PlannedOverHeadCost = subPeriodDataToAdd.PlannedQuantityOfWork * workItem.OverHeadCost;

                                        subPeriodDataToAdd.PlannedDirectCost = subPeriodDataToAdd.PlannedMaterialCost + subPeriodDataToAdd.PlannedEquipmentCost + subPeriodDataToAdd.PlannedLaborCost;
                                        subPeriodDataToAdd.TotalCost = subPeriodDataToAdd.PlannedDirectCost + subPeriodDataToAdd.PlannedProfitCost + subPeriodDataToAdd.PlannedOverHeadCost;

                                        db.TaskDetailBreakDown.Add(subPeriodDataToAdd);
                                        db.SaveChanges();
                                    }
                                }
                            }
                            #endregion
                        }

                    }
                }
                //}
            }
        }
        public static void UpdateExistingTasksProjectSpecificId()
        {
            var db = new ExceedDbContext();
            var projects = db.ProjectInformation.ToList();
            foreach (ProjectInformation project in projects)
            {
                var tasks = db.Tasks
                    .Where(t => t.WorkItemDuration.ProjectInformationId == project.ProjectInformationId).OrderBy(t => t.ID).ToList();
                var tArray = tasks.ToArray();
                for (int i = 0; i < tasks.Count; i++)
                {
                    var currentTask = tArray[i];
                    currentTask.ProjectSpecificId = i + 1;

                    db.Tasks.Attach(currentTask);
                    db.Entry(currentTask).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public static int GetProjectSpecificId(int projectId)
        {
            var db = new ExceedDbContext();
            //var counter = 1;
            //var currentTask = db.Tasks.Find(taskId);
            //if (currentTask != null)
            //{
            return 
                (db.Tasks
                .Where(t => t.WorkItemDuration.ProjectInformationId == projectId)
                .Select(t => t.ProjectSpecificId)
                .DefaultIfEmpty(0)
                .Max()) + 1;


        }
        public static void AddVariationAndSupplementaryWorkToSchedule(List<WorkItemDuration> listToAdd, int projectId, ScheduleRevision currentVersion)
        {
            var db = new ExceedDbContext();

            var mainContract = ContractMgmtHelper.GetProjectMainContract(projectId);
            var projectMobilizationDate = mainContract.CommencementDate;


            var subCategoryWorkItems = listToAdd.Where(w => w.HasSubWorkItems).ToList();
            var nonSubCategoryWorkItems = listToAdd.Where(w => !w.HasSubWorkItems).ToList();

            #region sub-category work items
            foreach (var subCategoryWorkItem in subCategoryWorkItems)
            {
                var childWorkItems =
                            db.WorkItemDuration.Where(w => w.ProjectInformationId == subCategoryWorkItem.ProjectInformationId &&
                                                w.Parent == subCategoryWorkItem.WorkItemDurationId
                                                ).ToList();

                var existingWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemDurationId == subCategoryWorkItem.WorkItemDurationId).FirstOrDefault();
                if (existingWorkItemInSchedule == null)
                {
                    // project specific Id --- found by adding 1 to the maximum of the id of task under the given project
                    var idOfWorkItemInSchedule = GetProjectSpecificId(projectId);

                    var taskToInsert = new TaskViewModel();

                    var listOfChildTasks = new List<Task>();

                    taskToInsert.Start = projectMobilizationDate;
                    taskToInsert.End = projectMobilizationDate;

                    taskToInsert.Title = subCategoryWorkItem.WorkDescription;
                    taskToInsert.PercentComplete = 0;

                    taskToInsert.Summary = subCategoryWorkItem.HasSubWorkItems ? true : false;
                    taskToInsert.Expanded = true;

                    taskToInsert.WorkItemDurationId = subCategoryWorkItem.WorkItemDurationId;
                    taskToInsert.WorkCategoryId = subCategoryWorkItem.WorkCategoryId;
                    taskToInsert.ConfigurationId = subCategoryWorkItem.ConfigurationId;
                    taskToInsert.BlockId = subCategoryWorkItem.BlockId;
                    taskToInsert.SubProjectTypeId = subCategoryWorkItem.SubProjectTypeId;

                    taskToInsert.SubProjectTypeName = subCategoryWorkItem.SubProjectTypeName;
                    taskToInsert.BlockName = subCategoryWorkItem.BlockName;
                    taskToInsert.StructureyType = subCategoryWorkItem.StructureyType;
                    taskToInsert.WorkCategory = subCategoryWorkItem.WorkCategory;

                    taskToInsert.EstimatedDuration = 1;
                    taskToInsert.TotalQuantityOfWork = subCategoryWorkItem.Quantity;
                    taskToInsert.BudgetCost = subCategoryWorkItem.BudgetCost;
                    taskToInsert.DirectCost = subCategoryWorkItem.DirectCost;
                    taskToInsert.MaterialCost = subCategoryWorkItem.MaterialCost;
                    taskToInsert.EquipmentCost = subCategoryWorkItem.EquipmentCost;
                    taskToInsert.LabourCost = subCategoryWorkItem.LabourCost;
                    taskToInsert.OrderId = SetWorkItemOrderInSchedule(subCategoryWorkItem);
                    taskToInsert.Version = currentVersion.Version;

                    taskToInsert.ActualDuration = taskToInsert.EstimatedDuration;
                    taskToInsert.ProjectSpecificId = idOfWorkItemInSchedule;

                    var entity = taskToInsert.ToEntity();
                    //var tempDb = new ExceedDbContext();
                    db.Tasks.Add(entity);
                    db.SaveChanges();
                    taskToInsert.TaskID = entity.ID;

                    #region add child tasks

                    if (childWorkItems.Any())
                    {
                        //var currentParentTask = db.Tasks.FirstOrDefault(t => t.WorkItemDurationId == workItemDuration.WorkItemDurationId);
                        //if (currentParentTask != null)
                        //{
                        foreach (var childWI in childWorkItems)
                        {
                            var existingChildWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemDurationId == childWI.WorkItemPerProjectId).FirstOrDefault();
                            if (existingChildWorkItemInSchedule == null)
                            {
                                var idOfChildWorkItemInSchedule = GetProjectSpecificId(projectId);

                                var childTaskToAdd = new TaskViewModel();
                                //var todaysDate = DateTime.Now;
                                var duration = childWI.EstimatedDuration;
                                childTaskToAdd.ParentID = entity.ID;
                                childTaskToAdd.Start = projectMobilizationDate;
                                childTaskToAdd.End = projectMobilizationDate.AddDays((int)duration);

                                childTaskToAdd.Title = childWI.WorkDescription;
                                childTaskToAdd.PercentComplete = 0;

                                childTaskToAdd.Summary = childWI.HasSubWorkItems ? true : false;
                                childTaskToAdd.Expanded = true;

                                childTaskToAdd.WorkItemDurationId = childWI.WorkItemDurationId;
                                childTaskToAdd.WorkCategoryId = childWI.WorkCategoryId;
                                childTaskToAdd.ConfigurationId = childWI.ConfigurationId;
                                childTaskToAdd.BlockId = childWI.BlockId;
                                childTaskToAdd.SubProjectTypeId = taskToInsert.SubProjectTypeId;

                                childTaskToAdd.SubProjectTypeName = taskToInsert.SubProjectTypeName;
                                childTaskToAdd.BlockName = taskToInsert.BlockName;
                                childTaskToAdd.StructureyType = childWI.StructureyType;
                                childTaskToAdd.WorkCategory = childWI.WorkCategory;

                                childTaskToAdd.EstimatedDuration = duration;
                                childTaskToAdd.TotalQuantityOfWork = childWI.Quantity;
                                childTaskToAdd.BudgetCost = childWI.BudgetCost;
                                childTaskToAdd.DirectCost = childWI.DirectCost;
                                childTaskToAdd.MaterialCost = childWI.MaterialCost;
                                childTaskToAdd.EquipmentCost = childWI.EquipmentCost;
                                childTaskToAdd.LabourCost = childWI.LabourCost;
                                childTaskToAdd.OrderId = SetWorkItemOrderInSchedule(childWI);
                                childTaskToAdd.Version = currentVersion.Version;

                                childTaskToAdd.ActualDuration = childTaskToAdd.EstimatedDuration;
                                //taskToInsert.OriginalDuration = 1;
                                //taskToInsert.RemainingDuration = 1;
                                childTaskToAdd.ProjectSpecificId = idOfChildWorkItemInSchedule;

                                var record = childTaskToAdd.ToEntity();
                                db.Tasks.Add(record);
                                db.SaveChanges();

                                listOfChildTasks.Add(record);

                                PrepareScheduleBreakDown(record.ID);

                            }
                        }

                        //}
                    }
                    #endregion

                    if (listOfChildTasks.Any())
                    {
                        entity.EstimatedDuration = listOfChildTasks.Max(t => t.EstimatedDuration);
                        entity.End = entity.Start.AddDays((int)entity.EstimatedDuration);
                        entity.ActualDuration = entity.EstimatedDuration;
                        entity.BudgetCost = listOfChildTasks.Sum(c => c.BudgetCost);
                        //entity.TotalQuantityOfWork = listOfChildTasks.Sum(c => c.TotalQuantityOfWork);

                        db.Tasks.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();

                        listOfChildTasks.Clear();
                    }
                }
            }
            #endregion

            #region non sub-category work items
            foreach (WorkItemDuration workItem in nonSubCategoryWorkItems)
            {
                //
                //  check if the work item is added to schedule
                var existingWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemDurationId == workItem.WorkItemDurationId).FirstOrDefault();
                if (existingWorkItemInSchedule == null)
                {
                    var idOfWorkItemInSchedule = GetProjectSpecificId(projectId);

                    var taskToInsert = new TaskViewModel();
                    var duration = workItem.EstimatedDuration;


                    taskToInsert.Start = projectMobilizationDate;
                    taskToInsert.End = projectMobilizationDate.AddDays((int)duration);

                    taskToInsert.Title = workItem.WorkDescription;
                    taskToInsert.PercentComplete = 0;

                    taskToInsert.Summary = workItem.HasSubWorkItems ? true : false;
                    taskToInsert.Expanded = true;

                    taskToInsert.WorkItemDurationId = workItem.WorkItemDurationId;
                    taskToInsert.WorkCategoryId = workItem.WorkCategoryId;
                    taskToInsert.ConfigurationId = workItem.ConfigurationId;
                    taskToInsert.BlockId = workItem.BlockId;
                    taskToInsert.SubProjectTypeId = workItem.SubProjectTypeId;

                    taskToInsert.SubProjectTypeName = workItem.SubProjectTypeName;
                    taskToInsert.BlockName = workItem.BlockName;
                    taskToInsert.StructureyType = workItem.StructureyType;
                    taskToInsert.WorkCategory = workItem.WorkCategory;

                    taskToInsert.EstimatedDuration = duration;
                    taskToInsert.TotalQuantityOfWork = workItem.Quantity;
                    taskToInsert.BudgetCost = workItem.BudgetCost;
                    taskToInsert.DirectCost = workItem.DirectCost;
                    taskToInsert.MaterialCost = workItem.MaterialCost;
                    taskToInsert.EquipmentCost = workItem.EquipmentCost;
                    taskToInsert.LabourCost = workItem.LabourCost;
                    taskToInsert.OrderId = SetWorkItemOrderInSchedule(workItem);
                    taskToInsert.Version = currentVersion.Version;

                    taskToInsert.ActualDuration = taskToInsert.EstimatedDuration;
                    taskToInsert.ProjectSpecificId = idOfWorkItemInSchedule;

                    var record = taskToInsert.ToEntity();
                    db.Tasks.Add(record);
                    db.SaveChanges();

                    PrepareScheduleBreakDown(record.ID);
                }
            }
            #endregion
        }
        private static int SetWorkItemOrderInSchedule(WorkItemDuration model)
        {
            var db = new ExceedDbContext();
            int order = 0;
            var currentProject = db.ProjectInformation.Find(model.ProjectInformationId);
            if (currentProject != null)
            {
                var availableTasks = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == currentProject.ProjectInformationId);
                if (availableTasks.Any())
                {
                    order = availableTasks.Max(task => task.OrderID) + 1;
                }
            }
            return order;
        }
        private static DateTime[] GetScheduleVersionStartAndEndDates(int projectId, int revisionId)
        {
            var db = new ExceedDbContext();


            var largestDate = DateTime.Now;
            var smallestDate = DateTime.Now;
            var currentProj = db.ProjectInformation.Find(projectId);
            var currentScheduleVersion = db.ScheduleRevisions.Find(revisionId);
            if (currentProj != null && currentScheduleVersion != null)
            {
                var projectTasks = db.Tasks.Where(task => task.WorkItemDuration.ProjectInformationId == projectId && task.Version == currentScheduleVersion.Version).AsQueryable();
                if (projectTasks.Count() > 0)
                {
                    var tsk = projectTasks.FirstOrDefault();
                    if (tsk != null)
                    {
                        smallestDate = tsk.Start;
                        largestDate = tsk.End;

                        foreach (var t in projectTasks)
                        {
                            if (t.Start < smallestDate)
                            {
                                smallestDate = t.Start;
                            }
                        }
                        foreach (var t in projectTasks)
                        {
                            if (t.End > largestDate)
                            {
                                largestDate = t.End;
                            }
                        }
                    }
                }
            }
            return new DateTime[] { smallestDate, largestDate };
        }

        private static decimal GetEstimatedDurationOfWorkItem(WorkItemPerProject record)
        {
            var db = new ExceedDbContext();
            var duration = new decimal();
            //var rootWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(wi => wi.GenericWorkItemPerProjectId == record.GenericWorkItemPerProjectId);
            //var durationEstimatinModel = db.WorkItemDuration.FirstOrDefault(d => d.GenericWorkItemPerProjectId == record.GenericWorkItemPerProjectId);

            //if (durationEstimatinModel != null && rootWorkItem != null)
            //{
            //    var subProjectType = db.SubProjectType.Find(rootWorkItem.SubProjectTypeId);
            //    if (subProjectType != null)
            //    {
            //        duration = subProjectType.NumberOfBlocks > 0 ? durationEstimatinModel.EstimatedDuration / subProjectType.NumberOfBlocks
            //            : durationEstimatinModel.EstimatedDuration;
            //    }
            //}
            return duration;
        }

        public static void UpdateResourcesOnDurationItemUpdate(WorkItemDuration durationItem) {
            var db = new ExceedDbContext();
            var materials = db.ResourceEstimationMaterials.Where(m => m.WorkItemDurationId == durationItem.WorkItemDurationId).ToList();
            var equipments = db.ResourceEstimationEquipments.Where(e => e.WorkItemDurationId == durationItem.WorkItemDurationId).ToList();
            var labors = db.ResourceEstimationLabour.Where(l => l.WorkItemDurationId == durationItem.WorkItemDurationId).ToList();

            if (materials.Any())
            {
                foreach(ResourceEstimationMaterial material in materials)
                {
                    material.TotalQuantity = material.QuantityPerUnit * durationItem.Quantity;
                    material.TotalCost = material.TotalQuantity * material.UnitPrice;

                    db.ResourceEstimationMaterials.Attach(material);
                    db.Entry(material).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (equipments.Any())
            {
                foreach(ResourceEstimationEquipment equipemnt in equipments)
                {
                    equipemnt.NumberPerDay = durationItem.NumberOfCrew > 0 ? equipemnt.UnitFactorAndNumberPerCrew * durationItem.NumberOfCrew : equipemnt.UnitFactorAndNumberPerCrew;
                    equipemnt.TotalCostPerDay = 8 * equipemnt.HourlyCost * equipemnt.NumberPerDay;

                    db.ResourceEstimationEquipments.Attach(equipemnt);
                    db.Entry(equipemnt).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            if (labors.Any())
            {
                foreach(ResourceEstimationLabour labor in labors)
                {
                    labor.NumberPerDay = durationItem.NumberOfCrew > 0 ? labor.UnitFactorAndNumberPerCrew * durationItem.NumberOfCrew : labor.UnitFactorAndNumberPerCrew;
                    labor.TotalCostPerDay = 8 * labor.IndexedHourlySalary * labor.NumberPerDay;

                    db.ResourceEstimationLabour.Attach(labor);
                    db.Entry(labor).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

    }
}
