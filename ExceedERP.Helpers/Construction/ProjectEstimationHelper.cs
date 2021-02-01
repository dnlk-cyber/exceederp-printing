using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
//using System.Data;
using System.Linq;

namespace ExceedERP.Helpers.Construction
{
    public static class ProjectEstimationHelper
    {

        public static string GetConcatnatedWorkItemDescription(int workItemId)
        {
            var db = new ExceedDbContext();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            var concatnatedDescription = "";
            if (workItem != null)
            {

                concatnatedDescription = workItem.ItemName;
                var workItemBlock = db.Blocks.Find(workItem.BlockId);
                if (workItemBlock != null)
                {
                    concatnatedDescription = workItem.ItemName + " of " + workItemBlock.Name;
                }
            }
            return concatnatedDescription;
        }
        public static WorkCategory GetCustomWorkItemCategory(int categoryId)
        {
            var db = new ExceedDbContext();
            WorkCategory category = new WorkCategory();
            var temp = db.WorkCategory.Find(categoryId);
            if (temp != null)
            {
                category = temp;
            }
            return category;
        }
        public static Configuration GetWorkCategoryStructureType(int categoryId)
        {
            var db = new ExceedDbContext();
            Configuration config = new Configuration();
            var currentCategory = db.WorkCategory.Find(categoryId);
            if (currentCategory != null)
            {
                var temp = db.Configurations.Find(currentCategory.ConfigurationId);
                if (temp != null)
                {
                    config = temp;
                }
            }
            return config;
        }
        public static string GetBlockName(int? blockId)
        {
            int bId = 0;
            if (blockId != null)
            {
                bId = (int)blockId;
            }
            var blockName = "";
            var db = new ExceedDbContext();
            var currentblock = db.Blocks.Find(bId);
            if (currentblock != null)
            {
                blockName = currentblock.Name;
            }
            return blockName;
        }
        // for generic work items
        public static string GetSubProjectTypeName(int? id)
        {
            int subTypeId = 0;
            if (id != null)
            {
                subTypeId = (int)id;
            }
            var name = "";
            var db = new ExceedDbContext();
            var currentSubProject = db.SubProjectType.Find(subTypeId);
            if (currentSubProject != null)
            {
                name = currentSubProject.Name;
            }
            return name;
        }
        public static int GetSubProjectTypeOrderIndex(int? id)
        {
            int subTypeId = 0;
            if (id != null)
            {
                subTypeId = (int)id;
            }
            var index = 0;
            var db = new ExceedDbContext();
            var currentSubProject = db.SubProjectType.Find(subTypeId);
            if (currentSubProject != null)
            {
                index = currentSubProject.OrderIndex;
            }
            return index;
        }
        // for block-specific work items
        public static string GetWorkItemSubProjectTypeName(int? id)
        {
            var name = string.Empty;
            var db = new ExceedDbContext();
            var currentBlock = db.Blocks.Find(id);
            if (currentBlock != null)
            {
                var subProjectType = db.SubProjectType.Find(currentBlock.SubProjectTypeId);
                if (subProjectType != null)
                {
                    name = $"{subProjectType.OrderIndex} {subProjectType.Name}";
                }
            }
            return name;
        }
        public static int GetWorkItemSubProjectTypeOrderIndex(int? id)
        {
            var index = 1;
            var db = new ExceedDbContext();
            var currentBlock = db.Blocks.Find(id);
            if (currentBlock != null)
            {
                var subProjectType = db.SubProjectType.Find(currentBlock.SubProjectTypeId);
                if (subProjectType != null)
                {
                    index = subProjectType.OrderIndex;
                }
            }
            return index;
        }
        public static SubProjectType GetWorkItemSubProjectType(WorkItemPerProject record)
        {
            var subProject = new SubProjectType();
            var db = new ExceedDbContext();
            var currentBlock = db.Blocks.Find(record.BlockId);
            if (currentBlock != null)
            {
                var subProjectType = db.SubProjectType.Find(currentBlock.SubProjectTypeId);
                if (subProjectType != null)
                {
                    subProject = subProjectType;
                }
            }
            return subProject;
        }
        /// <summary>
        /// Update quantity of work in a given work item from take off sheet
        /// </summary>
        /// <param name="takeOffPerProjectId"></param>
        public static void UpdateWorkItemPerProjecQuantityOfWork(int takeOffPerProjectId)
        {
            var db = new ExceedDbContext();
            var currentTakeOffSheet = db.TakeOffPerProject.Find(takeOffPerProjectId);
            if (currentTakeOffSheet != null)
            {
                var currentWorkItem = db.GenericWorkItemPerProject.Find(currentTakeOffSheet.WorkItemPerProjectId);

                if (currentWorkItem != null)
                {
                    var totalQuantity = new decimal();

                    if (currentTakeOffSheet.IsReinforcementbarTakeOff)
                    {
                        var availableQuantities = db.TakeOffLineForKgPerProject.Where(take => take.TakeOffPerProjectId == currentTakeOffSheet.TakeOffPerProjectId).ToList();
                        if (availableQuantities.Any())
                        {
                            totalQuantity = availableQuantities.Sum(t => t.TotalWeight);
                        }
                    }
                    else
                    {
                        var availableQuantities = db.TakeOffLinePerProject.Where(take => take.TakeOffPerProjectId == currentTakeOffSheet.TakeOffPerProjectId).ToList();
                        if (availableQuantities.Any())
                        {
                            totalQuantity = availableQuantities.Sum(t => t.Quantity);
                        }
                    }
                   

                    currentWorkItem.TotalQuantityOfWork = totalQuantity;
                    db.GenericWorkItemPerProject.Attach(currentWorkItem);
                    db.Entry(currentWorkItem).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public static decimal GetWorkItemBudgetedCost(int workItemId)
        {
            var db = new ExceedDbContext();
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            decimal calculatedCost = new decimal();
            if (currentWorkItem != null)
            {
                var workItemDirectCost = currentWorkItem.DirectCost;
                var currentOverHeadRate = currentWorkItem.OverheadRate;
                var quantityOfWork = currentWorkItem.TotalQuantityOfWork;
                var unitRate = currentWorkItem.TotalUnitCost;

                var totalDirectCost = workItemDirectCost * quantityOfWork;
                var totalOverHeadCost = totalDirectCost * currentOverHeadRate / 100;

                calculatedCost = totalDirectCost + totalOverHeadCost;
            }
            return calculatedCost;
        }

        // 
        //  summary :
        //      unit price of template workItem
        public static decimal GetWorkItemUnitPrice(int workItemId)
        {
            var value = new decimal();
            var db = new ExceedDbContext();
            var currentWorkItem = db.ProjectEstimationWorkItem.Find(workItemId);
            if (currentWorkItem != null)
            {
                value = currentWorkItem.TotalUnitCost;
            }
            return value;
        }
        public static decimal GetEstimationWorkItemDirectCost(int workItemId)
        {
            var value = new decimal();
            var db = new ExceedDbContext();
            var currentWorkItem = db.ProjectEstimationWorkItem.Find(workItemId);
            if (currentWorkItem != null)
            {
                value = currentWorkItem.DirectCost;
            }
            return value;
        }
        // 
        // summary :
        //      Unit price of project work item
        public static decimal GetUnitPriceOfProjectWorkItem(int workItemId)
        {
            var db = new ExceedDbContext();
            var amnt = new decimal();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            if (workItem != null)
            {
                amnt = workItem.TotalUnitCost;
            }
            return amnt;
        }
        public static decimal GetWorkItemTotalQuantityOfWork(int workItemId)
        {
            var db = new ExceedDbContext();
            var qnty = new decimal();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            if (workItem != null)
            {
                qnty = workItem.TotalQuantityOfWork;
            }
            return qnty;
        }
        public static void UpdateCustomWorkItemUnitPrice(int id)
        {
            var db = new ExceedDbContext();
            decimal hourlyOutPut = 1;

            var currentWorkItem = db.ProjectEstimationCustomWorkItems.Find(id);
            var contents = db.EstimationCustomWorkItemContents.Where(c => c.ProjectEstimationCustomWorkItemId == id).ToList();
            var materials = db.EstimationCustomWorkItemMaterial.Where(m => m.ProjectEstimationCustomWorkItemId == id).ToList();
            var equimplents = db.EstimationCustomWorkItemEquipment.Where(e => e.ProjectEstimationCustomWorkItemId == id).ToList();
            var labors = db.EstimationCustomWorkItemDirectLabor.Where(l => l.ProjectEstimationCustomWorkItemId == id).ToList();

            var compCost = contents.Sum(c => c.UnitPrice);
            var materialCost = materials.Sum(m => m.TotalCost);
            var equCost = equimplents.Sum(e => e.TotalCost);
            var laborCost = labors.Sum(l => l.TotalCost);

            if (currentWorkItem != null)
            {
                hourlyOutPut = currentWorkItem.HourlyOutPut > 0 ? currentWorkItem.HourlyOutPut : 1;

                currentWorkItem.MaterialCost = materialCost + compCost;
                currentWorkItem.EquipmentCost = equCost;
                currentWorkItem.LaborCost = laborCost;

                currentWorkItem.MaterialCostPerUnit = (materialCost + compCost);
                currentWorkItem.EquipmentCostPerUnit = equCost / hourlyOutPut;
                currentWorkItem.LaborCostPerUnit = laborCost / hourlyOutPut;

                currentWorkItem.DirectCost = currentWorkItem.MaterialCostPerUnit + currentWorkItem.LaborCostPerUnit + currentWorkItem.EquipmentCostPerUnit;
                currentWorkItem.OverHeadCost = (currentWorkItem.OverheadRate / 100) * currentWorkItem.DirectCost;
                currentWorkItem.ProfitCost = (currentWorkItem.ProfitRate / 100) * currentWorkItem.DirectCost;

                currentWorkItem.TotalUnitRate = currentWorkItem.DirectCost + currentWorkItem.OverHeadCost + currentWorkItem.ProfitCost;

                db.ProjectEstimationCustomWorkItems.Attach(currentWorkItem);
                db.Entry(currentWorkItem).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        public static string GetItemNameFromEstimationWorkItem(int id)
        {
            var db = new ExceedDbContext();
            var name = "";
            var temp = db.ProjectEstimationWorkItem.Find(id);
            if (temp != null)
            {
                name = temp.ItemName;
            }
            return name;
        }

        public static void UpdateGenericWorkItem(int id)
        {
            var db = new ExceedDbContext();
            var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == id).ToList();
            var equipments = db.GenericProjectEquipment.Where(m => m.GenericWorkItemPerProjectId == id).ToList();
            var labors = db.GenericProjectDirectLabour.Where(m => m.GenericWorkItemPerProjectId == id).ToList();
            var matCost = materials.Sum(m => m.TotalCost);
            var eqCost = equipments.Sum(e => e.TotalCost);
            var labCost = labors.Sum(l => l.TotalCost);

            var commonOverHeadCostRate = ConsSettingHelper.GetCurrentOverHeadRate();
            var commonProfitMarginRate = ConsSettingHelper.GetCurrentProfitRate();

            var projectOverHeadRate = new decimal();
            var currentWorkItem = db.GenericWorkItemPerProject.Find(id);
            if (currentWorkItem != null)
            {
                //var currentProject = db.ProjectInformation.Find(currentWorkItem.ProjectInformationId);
                var hourlyOutPut = currentWorkItem.LabourHourlyOutPut > 0 ? currentWorkItem.LabourHourlyOutPut : 1;

                if (currentWorkItem.ContainsWorkItemAsInput)
                {
                    var customItems = db.GenericProjectCustomWorkItemDetails.Where(cm => cm.GenericWorkItemPerProjectId == currentWorkItem.GenericWorkItemPerProjectId).ToList();
                    if (customItems.Any()) { matCost += customItems.Sum(cm => cm.TotalCost); }
                }

                var overHeadRateAttribute = db.ProjectSpecificSettingAttributes
                    .FirstOrDefault(a => a.ProjectInformationId == currentWorkItem.ProjectInformationId &&
                    a.Attribute == ProjectSettingAttributes.Overhead_Cost_Rate);
                if (overHeadRateAttribute != null)
                {
                    projectOverHeadRate = overHeadRateAttribute.Value;
                }

                if (projectOverHeadRate == 0)
                {
                    projectOverHeadRate = commonOverHeadCostRate;
                }
                currentWorkItem.MaterialCost = matCost;
                currentWorkItem.LabourOutPut = labCost;
                currentWorkItem.EquipmentOutput = eqCost;
                currentWorkItem.MaterialCostPerUnit = matCost;
                currentWorkItem.LaborCostPerUnit = labCost / hourlyOutPut;
                currentWorkItem.EquipmentCostPerUnit = eqCost / hourlyOutPut;
                currentWorkItem.DirectCost = currentWorkItem.MaterialCostPerUnit + currentWorkItem.LaborCostPerUnit + currentWorkItem.EquipmentCostPerUnit;
                currentWorkItem.OverHeadCost = currentWorkItem.DirectCost * (projectOverHeadRate / 100);
                currentWorkItem.ProfitCost = currentWorkItem.DirectCost * (commonProfitMarginRate / 100);
                currentWorkItem.TotalUnitCost = currentWorkItem.DirectCost + currentWorkItem.ProfitCost + currentWorkItem.OverHeadCost;

                // multiply by thickness factor if necessary.
                if (currentWorkItem.ThicknessFactor > 0) {
                    currentWorkItem.TotalUnitCost = currentWorkItem.TotalUnitCost * currentWorkItem.ThicknessFactor;
                }
                if (currentWorkItem.HasAdditionalTransportationCost)
                {
                    currentWorkItem.CalculatedTransportCost = (currentWorkItem.TransportCostRate / 100) * currentWorkItem.MaterialCostPerUnit;
                    currentWorkItem.TotalUnitCostIncludingTransport = currentWorkItem.CalculatedTransportCost + currentWorkItem.TotalUnitCost;
                }
                db.GenericWorkItemPerProject.Attach(currentWorkItem);
                db.Entry(currentWorkItem).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
        public static void CleanFalseChildItems(int id)
        {
            var db = new ExceedDbContext();
            var childItems = db.GenericWorkItemPerProject.Where(wi => wi.ParentId == id);
            if (childItems.Any())
            {
                db.GenericWorkItemPerProject.RemoveRange(childItems);
                db.SaveChanges();
            }
        }
        public static void CleanReferenceAndOrphanedItemsOnTemplateWorkItemDeleted(int id)
        {
            var db = new ExceedDbContext();
            var childItems = db.ProjectEstimationWorkItem.Where(itm => itm.ParentId == id).ToList();
            if (childItems.Any())
            {
                foreach (ProjectEstimationWorkItem item in childItems)
                {
                    db.ProjectEstimationWorkItem.Attach(item);
                    db.ProjectEstimationWorkItem.Remove(item);
                    db.SaveChanges();
                    if (item.IsSubCategory)
                    {
                        CleanReferenceAndOrphanedItemsOnTemplateWorkItemDeleted(item.ProjectEstimationWorkItemId);
                    }
                    if (item.ContainsWorkItemAsInput)
                    {
                        var customDetails = db.EstimationTemplateCustomWorkItemDetails.Where(d => d.ParentId == id);
                        if (customDetails.Any())
                        {
                            foreach (EstimationTemplateCustomWorkItemDetail detail in customDetails)
                            {
                                db.EstimationTemplateCustomWorkItemDetails.Attach(detail);
                                db.EstimationTemplateCustomWorkItemDetails.Remove(detail);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

        }
        public static void CleanReferencesFromProjectOnGenericWorkItemDelete(GenericWorkItemPerProject workItem)
        {
            var db = new ExceedDbContext();
            
            if (workItem.ContainsWorkItemAsInput)
            {
                var customDetails = db.GenericProjectCustomWorkItemDetails.Where(c => c.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();

                db.GenericProjectCustomWorkItemDetails.RemoveRange(customDetails);
                db.SaveChanges();
            }
            if (workItem.IsSubCategory)
            {
                var childItems = db.GenericWorkItemPerProject.Where(wi => wi.ParentId == workItem.GenericWorkItemPerProjectId).ToList();
                if (childItems.Any())
                {
                    foreach(GenericWorkItemPerProject childItem in childItems)
                    {
                        db.GenericWorkItemPerProject.Attach(childItem);
                        db.GenericWorkItemPerProject.Remove(childItem);
                        db.SaveChanges();
                        CleanReferencesFromProjectOnGenericWorkItemDelete(childItem);
                    }
                }
            }
        }
        public static void DeleteGenericPieceRateAndCleanReferencesFromProject(int id)
        {
            var db = new ExceedDbContext();
            var currentPiecerate = db.GenericAreaWorkItem.Find(id);
            if (currentPiecerate != null)
            {
                db.GenericAreaWorkItem.Attach(currentPiecerate);
                db.GenericAreaWorkItem.Remove(currentPiecerate);
                db.SaveChanges();
            }
        }
        public static void DeleteGenericSubContractWorkItemAndCleanReferencesFromProject(int id)
        {
            var db = new ExceedDbContext();
            var item = db.GenericSubContractWorkItem.Find(id);
            if (item != null)
            {
                db.GenericSubContractWorkItem.Attach(item);
                db.GenericSubContractWorkItem.Remove(item);
                db.SaveChanges();
            }
        }

        public static decimal CalculateDurationPerCrew(decimal quantity, decimal productivity)
        {
            decimal durationPerCrew = new decimal();
            if (productivity != 0)
            {
                durationPerCrew = quantity / productivity;
                durationPerCrew = Math.Round(durationPerCrew, 4);
            }
            return durationPerCrew;
        }

        public static decimal CalculateWorkDuration(decimal durationPerCrew, int noOfCrew)
        {
            decimal result = new decimal();
            if (noOfCrew > 0)
            {
                result = durationPerCrew / noOfCrew;
                result = Math.Round(result, 4);
            }
            return result;
        }

        public static void SplitProjectAndAddWorkItemsToSpecificBlocks(int projectId)
        {
            var db = new ExceedDbContext();
            var currentProject = db.ProjectInformation.Find(projectId);
            if (currentProject != null)
            {
                #region remove existing data
                var tasks = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                //var taskDetail = db.TaskDetail.ToList();

                var workItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var pcRates = db.AreaWorkItemes.Where(p => p.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var scworkItems = db.SubContractWorkItem.Where(sc => sc.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var customWorkItems = db.ProjectCustomWorkItems.Where(c => c.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                var ids = new HashSet<int>(workItems.Select(w => w.WorkItemPerProjectId));
                var customContents = db.ProjectCustomWorkItemDetails.Where(c => ids.Contains(c.WorkItemPerProjectId)).ToList();

                var utlWorkItems = db.ResourceUtilizationWorkItem.Where(ru => ru.WorkItemPerProject.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                var blkMapping = db.BlockMappings.Where(bm => bm.Block.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var bloks = db.Blocks.Where(b => b.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                //var durationWorkItem = db.WorkItemDuration.Where(wd => wd.GenericWorkItemPerProject.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var durationWorkItem = db.WorkItemDuration.Where(wd => wd.ProjectInformationId == currentProject.ProjectInformationId).ToList();


                //db.TaskDetail.RemoveRange(taskDetail);
                //db.SaveChanges();

                db.WorkItemDuration.RemoveRange(durationWorkItem);
                db.SaveChanges();

                db.Tasks.RemoveRange(tasks);
                db.SaveChanges();

                db.ResourceUtilizationWorkItem.RemoveRange(utlWorkItems);
                db.SaveChanges();

                db.WorkItemPerProject.RemoveRange(workItems);
                db.ProjectCustomWorkItemDetails.RemoveRange(customContents);
                db.AreaWorkItemes.RemoveRange(pcRates);
                db.SubContractWorkItem.RemoveRange(scworkItems);
                db.ProjectCustomWorkItems.RemoveRange(customWorkItems);
                db.SaveChanges();

                db.BlockMappings.RemoveRange(blkMapping);
                db.SaveChanges();

                db.Blocks.RemoveRange(bloks);
                db.SaveChanges();

                #endregion
                var counter = 1;
                var subProjects = db.SubProjectType.Where(sb => sb.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                if (subProjects.Any())
                {
                    var totalBlocksInProject = subProjects.Sum(s => s.NumberOfBlocks);
                    foreach (SubProjectType subProject in subProjects)
                    {
                        if (currentProject.ProjectType == AvailableProjectTypes.Building)
                        {
                            for (int i = 1; i <= subProject.NumberOfBlocks; i++)
                            {
                                var blockToAdd = new Block
                                {
                                    Name = "B-" + counter,
                                    ProjectInformationId = currentProject.ProjectInformationId,
                                    SubProjectTypeId = subProject.SubProjectTypeId,

                                };
                                db.Blocks.Add(blockToAdd);
                                db.SaveChanges();
                                counter = counter + 1;

                                var subProjectMappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                                if (subProjectMappings.Any())
                                {
                                    foreach (SubProjectStructureTypeMapping mapping in subProjectMappings)
                                    {
                                        var blockMappingToAdd = new BlockMapping
                                        {
                                            BlockId = blockToAdd.BlockId,
                                            ConfigurationId = mapping.ConfigurationId,
                                            Remark = mapping.Remark,
                                        };
                                        db.BlockMappings.Add(blockMappingToAdd);
                                        db.SaveChanges();

                                    }
                                }
                                AddWorkItemFromGenericEstimation(subProject.SubProjectTypeId, blockToAdd.BlockId);
                            }
                        }
                        else if (currentProject.ProjectType == AvailableProjectTypes.Road)
                        {
                            for (int i = 1; i <= subProject.NumberOfBlocks; i++)
                            {
                                var blockToAdd = new Block
                                {
                                    Name = "R-" + counter,
                                    ProjectInformationId = currentProject.ProjectInformationId,
                                    SubProjectTypeId = subProject.SubProjectTypeId,
                                };
                                db.Blocks.Add(blockToAdd);
                                db.SaveChanges();
                                counter = counter + 1;

                                var subProjectMappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                                if (subProjectMappings.Any())
                                {
                                    foreach (SubProjectStructureTypeMapping mapping in subProjectMappings)
                                    {
                                        var blockMappingToAdd = new BlockMapping
                                        {
                                            BlockId = blockToAdd.BlockId,
                                            ConfigurationId = mapping.ConfigurationId,
                                            Remark = mapping.Remark,
                                        };
                                        db.BlockMappings.Add(blockMappingToAdd);
                                        db.SaveChanges();
                                    }
                                }
                                AddWorkItemFromGenericEstimation(subProject.SubProjectTypeId, blockToAdd.BlockId);
                            }
                        }
                    }

                    UpdateWorkItemsHeirarchy(currentProject.ProjectInformationId);
                }
            }
        }
        public static decimal GetIndexedHourlyPayPerJobTitle(int id)
        {
            var db = new ExceedDbContext();
            var jobTitles = db.ProjectEstimationIndexLabour.Where(x => x.ProjectEstimationIndexLabourId == id).FirstOrDefault();
            if (jobTitles != null)
            {
                return jobTitles.ApprovedIndexedHourlySalary;
            }
            return 0;
        }
        public static decimal GetEquipmentRentalRate(Guid id)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal hourlyOutputRate = 0;
            var fleet = db.Fleets.Where(x => x.FleetsID == id).FirstOrDefault();
            if (fleet != null)
            {
                if (fleet.IsExternal)
                {
                    var equipment = db.MachinaryRent.Where(em => em.FleetsID == id).FirstOrDefault();
                    if (equipment != null)
                    {
                        hourlyOutputRate = equipment.ApprovedCost;
                    }
                }
                else
                {
                    var equipment = db.InternalMachinaryRent.Where(im => im.FleetsID == id).FirstOrDefault();
                    if (equipment != null)
                    {
                        hourlyOutputRate = equipment.HourlyRate;
                    }
                }
            }

            return hourlyOutputRate;
        }
        private static void AddWorkItemFromGenericEstimation(int subProjectId, int blockId)
        {
            var db = new ExceedDbContext();
            #region work items
            var genericWorkItems = db.GenericWorkItemPerProject.Where(wi => wi.SubProjectTypeId == subProjectId).ToList();
            if (genericWorkItems.Any())
            {
                foreach (GenericWorkItemPerProject gWorkItem in genericWorkItems)
                {
                    var workItemToAdd = new WorkItemPerProject
                    {
                        ProjectEstimationWorkItemId = gWorkItem.ProjectEstimationWorkItemId,
                        IsSubCategory = gWorkItem.IsSubCategory,
                        ItemCode = gWorkItem.ItemCode,
                        ContainsWorkItemAsInput = gWorkItem.ContainsWorkItemAsInput,
                        GenericWorkItemPerProjectId = gWorkItem.GenericWorkItemPerProjectId,
                        ProjectInformationId = gWorkItem.ProjectInformationId,
                        BlockId = blockId,

                        ItemName = gWorkItem.ItemName,
                        TotalQuantityOfWork = gWorkItem.TotalQuantityOfWork,
                        DirectCost = gWorkItem.DirectCost,
                        MaterialCost = gWorkItem.MaterialCost,
                        LabourOutPut = gWorkItem.LabourOutPut,
                        EquipmentOutput = gWorkItem.EquipmentOutput,
                        MaterialCostPerUnit = gWorkItem.MaterialCostPerUnit,
                        LaborCostPerUnit = gWorkItem.LaborCostPerUnit,
                        EquipmentCostPerUnit = gWorkItem.EquipmentCostPerUnit,
                        TotalUnitCost = gWorkItem.TotalUnitCost,
                        OverHeadCost = gWorkItem.OverHeadCost,
                        ProfitCost = gWorkItem.ProfitCost,
                        Remark = gWorkItem.Remark,
                        LabourHourlyOutPut = gWorkItem.LabourHourlyOutPut,
                        UnitOfMeasurementId = gWorkItem.UnitOfMeasurementId,
                        OverheadRate = gWorkItem.OverheadRate,
                        ProfitMargin = gWorkItem.ProfitMargin,

                        OrgBranchName = gWorkItem.OrgBranchName,
                        DateCreated = gWorkItem.DateCreated,
                        LastModified = gWorkItem.LastModified,
                        CreatedBy = gWorkItem.CreatedBy,
                        ModifiedBy = gWorkItem.ModifiedBy,

                        ParentId = gWorkItem.ParentId,
                        ProjectType = gWorkItem.ProjectType,
                        WorkCategoryId = gWorkItem.WorkCategoryId
                    };
                    db.WorkItemPerProject.Add(workItemToAdd);
                    db.SaveChanges();

                    #region copy resources

                    if (gWorkItem.ContainsWorkItemAsInput)
                    {
                        var customDetails = db.GenericProjectCustomWorkItemDetails.Where(w => w.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                        if (customDetails.Any())
                        {
                            foreach (GenericProjectCustomWorkItemDetail sourceDetail in customDetails)
                            {
                                var recordDetail = new ProjectCustomWorkItemDetail
                                {
                                    ProjectEstimationWorkItemId = sourceDetail.ProjectEstimationWorkItemId,
                                    WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId,
                                    UnitCost = sourceDetail.UnitCost,
                                    QuantityPerUnit = sourceDetail.QuantityPerUnit,
                                    TotalCost = sourceDetail.TotalCost,
                                    CreatedBy = sourceDetail.CreatedBy,
                                    DateCreated = DateTime.Now,
                                    LastModified = DateTime.Now,
                                    ModifiedBy = sourceDetail.ModifiedBy,
                                    OrgBranchName = sourceDetail.OrgBranchName,

                                };
                                db.ProjectCustomWorkItemDetails.Add(recordDetail);
                                db.SaveChanges();
                            }
                        }
                    }


                    #region materials
                    var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                    if (materials.Any())
                    {
                        foreach (GenericProjectMaterial material in materials)
                        {
                            var materialToAdd = new ProjectMaterial
                            {
                                ItemCode = material.ItemCode,
                                ItemCategoryCode = material.ItemCategoryCode,
                                Unit = material.Unit,
                                Quantity = material.Quantity,
                                UnitPrice = material.UnitPrice,
                                TotalCost = material.TotalCost,
                                MaterialUnitCost = material.MaterialUnitCost,
                                OrgBranchName = material.OrgBranchName,
                                DateCreated = material.DateCreated,
                                LastModified = material.LastModified,
                                CreatedBy = material.CreatedBy,
                                ModifiedBy = material.ModifiedBy,
                                WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId
                            };
                            db.ProjectMaterial.Add(materialToAdd);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                    #region direct labor
                    var labors = db.GenericProjectDirectLabour.Where(m => m.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                    if (labors.Any())
                    {
                        foreach (GenericProjectDirectLabour labor in labors)
                        {
                            var laborToAdd = new ProjectDirectLabour
                            {
                                Unit = labor.Unit,
                                Quantity = labor.Quantity,
                                UnitFactory = labor.UnitFactory,
                                IndexedHourlySalary = labor.IndexedHourlySalary,
                                DailyPay = labor.DailyPay,
                                TotalCost = labor.TotalCost,
                                ManpowerUnitCost = labor.ManpowerUnitCost,
                                OrgBranchName = labor.OrgBranchName,
                                DateCreated = labor.DateCreated,
                                LastModified = labor.LastModified,
                                CreatedBy = labor.CreatedBy,
                                ModifiedBy = labor.ModifiedBy,
                                ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                                WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId
                            };
                            db.ProjectDirectLabour.Add(laborToAdd);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                    #region equipment
                    var equipments = db.GenericProjectEquipment.Where(m => m.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                    if (equipments.Any())
                    {
                        foreach (GenericProjectEquipment projectEquipment in equipments)
                        {
                            var equipmentToAdd = new ProjectEquipment
                            {
                                WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId,
                                FleetsID = projectEquipment.FleetsID,
                                Unit = projectEquipment.Unit,
                                Quantity = projectEquipment.Quantity,
                                UnitFactory = projectEquipment.UnitFactory,
                                RentalCost = projectEquipment.RentalCost,
                                TotalCost = projectEquipment.TotalCost,
                                EquipmentUnitCost = projectEquipment.EquipmentUnitCost,
                                EquipmentType = projectEquipment.EquipmentType,

                                OrgBranchName = projectEquipment.OrgBranchName,
                                DateCreated = projectEquipment.DateCreated,
                                LastModified = projectEquipment.LastModified,
                                CreatedBy = projectEquipment.CreatedBy,
                                ModifiedBy = projectEquipment.ModifiedBy,
                            };
                            db.ProjectEquipment.Add(equipmentToAdd);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion
            #region piece rates
            var genericPieceRates = db.GenericAreaWorkItem.Where(wi => wi.SubProjectTypeId == subProjectId).ToList();
            if (genericPieceRates.Any())
            {
                foreach (GenericAreaWorkItem pieceRate in genericPieceRates)
                {
                    var pieceRateToAdd = new AreaWorkItem
                    {
                        WorkCategoryId = pieceRate.WorkCategoryId,
                        Price = pieceRate.Price,
                        WorkItemName = pieceRate.WorkItemName,
                        ItemCode = pieceRate.ItemCode,
                        DirectCost = pieceRate.DirectCost,
                        Width = pieceRate.Width,
                        Height = pieceRate.Height,
                        Area = pieceRate.Area,
                        CylinderLock = pieceRate.CylinderLock,
                        OverHeadProfitCost = pieceRate.OverHeadProfitCost,
                        ProjectInformationId = pieceRate.ProjectInformationId,
                        MaterialCost = pieceRate.MaterialCost,
                        TransportCost = pieceRate.TransportCost,
                        GenericAreaWorkItemId = pieceRate.GenericAreaWorkItemId,
                        BlockId = blockId,

                        OrgBranchName = pieceRate.OrgBranchName,
                        DateCreated = pieceRate.DateCreated,
                        LastModified = pieceRate.LastModified,
                        CreatedBy = pieceRate.CreatedBy,
                        ModifiedBy = pieceRate.ModifiedBy,
                    };
                    db.AreaWorkItemes.Add(pieceRateToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region subcontract workItem
            var genericSubContractWorkItems = db.GenericSubContractWorkItem.Where(wi => wi.SubProjectTypeId == subProjectId).ToList();
            if (genericSubContractWorkItems.Any())
            {
                foreach (GenericSubContractWorkItem subContractWorkItem in genericSubContractWorkItems)
                {
                    var scToAdd = new SubContractWorkItem
                    {
                        BlockId = blockId,
                        WorkCategoryId = subContractWorkItem.WorkCategoryId,
                        UnitOfMeasurementId = subContractWorkItem.UnitOfMeasurementId,
                        WorkItemName = subContractWorkItem.WorkItemName,
                        ItemCode = subContractWorkItem.ItemCode,
                        Quntity = subContractWorkItem.Quntity,
                        SubContractCostOne = subContractWorkItem.SubContractCostOne,
                        SubContractCostTwo = subContractWorkItem.SubContractCostTwo,
                        SubContractCostThree = subContractWorkItem.SubContractCostThree,
                        SelectedCost = subContractWorkItem.SelectedCost,
                        TotalCost = subContractWorkItem.TotalCost,
                        ProjectInformationId = subContractWorkItem.ProjectInformationId,
                        GenericSubContractWorkItemId = subContractWorkItem.GenericSubContractWorkItemId,

                        OrgBranchName = subContractWorkItem.OrgBranchName,
                        DateCreated = subContractWorkItem.DateCreated,
                        LastModified = subContractWorkItem.LastModified,
                        CreatedBy = subContractWorkItem.CreatedBy,
                        ModifiedBy = subContractWorkItem.ModifiedBy
                    };
                    db.SubContractWorkItem.Add(scToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
        }
        // update the entire work items heirarchy .... 
        private static void UpdateWorkItemsHeirarchy(int projectId)
        {
            var db = new ExceedDbContext();
            var workItemsInProject = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == projectId && wi.ParentId != 0).ToList();
            if (workItemsInProject.Any())
            {
                foreach (var workItem in workItemsInProject)
                {
                    var workItemParent = db.WorkItemPerProject
                        .FirstOrDefault(w => w.ProjectInformationId == projectId &&
                                            w.BlockId == workItem.BlockId &&
                                            w.GenericWorkItemPerProjectId == workItem.ParentId);

                    if (workItemParent != null)
                    {
                        workItem.ParentId = workItemParent.WorkItemPerProjectId;
                        db.WorkItemPerProject.Attach(workItem);
                        db.Entry(workItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
