using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class DetailedWorkProgressOds : List<DetailedWorkProgressSummaryBlockViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DetailedWorkProgressSummaryBlockViewModel> GetAllBlocks(string projectId, string subProjectTypeId, string blockId, string periodId, string subPeriodId)
        {
            List<DetailedWorkProgressSummaryBlockViewModel> list = new List<DetailedWorkProgressSummaryBlockViewModel>();
            if (projectId == null || periodId == null)
            {
                return list;
            }
            int id = Convert.ToInt32(projectId);
            int pId = Convert.ToInt32(periodId);
            int subPId = Convert.ToInt32(subPeriodId);
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == pId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();
            if (project != null && currentPeriod != null)
            {
                var allWorkItemsInProject = db.WorkItemPerProject.Where(w => w.ProjectInformationId == project.ProjectInformationId).ToList();
                var blocks = db.Blocks.Where(b => b.ProjectInformationId == id).ToList();

                int subId = Convert.ToInt32(subProjectTypeId);
                if (subProjectTypeId != null)
                {
                    blocks = blocks.Where(b => b.SubProjectTypeId == subId).ToList();
                }
                int bId = Convert.ToInt32(blockId);
                //var specificBlock = db.Blocks.Where(b => b.BlockId == bId && b.ProjectInformationId == id).ToList();
                if (blockId != null)
                {
                    blocks = blocks.Where(b => b.BlockId == bId).ToList();
                }
                if (blocks.Any())
                {
                    foreach (Block block in blocks)
                    {
                        #region block cost calculation
                        var blockCost = new decimal();
                        var blockWorkItems = allWorkItemsInProject.Where(w => w.BlockId == block.BlockId).ToList();
                        if (blockWorkItems.Any())
                        {
                            blockCost = blockWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                        }
                        #endregion
                        var blockRecord = new DetailedWorkProgressSummaryBlockViewModel
                        {
                            Name = block.Name + " [ " + ProjectEstimationHelper.GetSubProjectTypeName(block.SubProjectTypeId) + " ]",
                            TotalCost = blockCost
                        };

                        #region structures
                        #region  LINQ for structure types under this very block
                        //var structureTypes = (from c in db.Configurations
                        //                      join w in db.ProjectTypeMappings
                        //                          on c.ConfigurationId equals w.ConfigurationId

                        //                      join p in db.ProjectInformation
                        //                          on w.ProjectType equals p.ProjectType
                        //                      join b in db.Blocks
                        //                     on p.ProjectInformationId equals b.ProjectInformationId

                        //                      where
                        //                     p.ProjectInformationId == id &&
                        //                      c.Reference == "STRUCTURE_TYPE" &&
                        //                      b.BlockId == block.BlockId
                        //                      select (new
                        //                      {
                        //                          c.CurrentValue,
                        //                          c.ConfigurationId
                        //                      })).ToList();
                        #endregion
                        var mappings = db.BlockMappings.Where(sb => sb.BlockId == block.BlockId).ToList();
                        var idsHash = new HashSet<int>(mappings.Select(st => st.ConfigurationId));
                        var structureTypes = db.Configurations.Where(c => c.Reference == "STRUCTURE_TYPE" && idsHash.Contains(c.ConfigurationId)).ToList();
                        if (structureTypes.Any())
                        {
                            foreach (var str in structureTypes)
                            {
                                var structureRecord = new DetailedWorkProgressSummaryStructureViewModel
                                {
                                    Name = ConsSettingHelper.GetStructureTypeCode(str.ConfigurationId, project.ProjectType) + " " + str.CurrentValue.Replace('_', ' ')
                                };
                                //structureRecord.Name = structureRecord.Name.Replace('_', ' ');

                                #region work categories under this very structure
                                var workCategoriesUnderStructure = db.WorkCategory.Where(wc => wc.ConfigurationId == str.ConfigurationId).ToList();
                                if (workCategoriesUnderStructure.Any())
                                {
                                    foreach (WorkCategory category in workCategoriesUnderStructure)
                                    {
                                        #region total cost of work category calculation

                                        var categoryCost = new decimal();
                                        //var categoryWorkItems = db.WorkItemPerProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.BlockId == block.BlockId).ToList();
                                        var categoryWorkItems = blockWorkItems.Where(w => w.WorkCategoryId == category.WorkCategoryId).ToList();
                                        if (categoryWorkItems.Any())
                                        {
                                            categoryCost = categoryWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                                        }
                                        #endregion
                                        var categoryRecord = new DetailedWorkProgressSummaryCategoryViewModel
                                        {
                                            OrderIndex = category.Index,
                                            Description = category.Name,
                                            ContractValue = categoryCost
                                        };
                                        foreach (WorkItemPerProject workItem in categoryWorkItems)
                                        {
                                            if (workItem.IsSubCategory)
                                            {
                                                var workItemRecord = new DetailedWorkProgressWorkItemViewModel
                                                {
                                                    ItemCode = workItem.ItemCode,
                                                    Description = workItem.ItemName,
                                                    HasChildItems = workItem.IsSubCategory,
                                                    ContractQuantity = workItem.TotalQuantityOfWork,
                                                    Unit = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                                    UnitPrice = workItem.TotalUnitCost,
                                                    ActualTakeOffQuantity = workItem.ActualQuantityOfWork,
                                                };

                                                categoryRecord.WorkItems.Add(workItemRecord);
                                            }
                                            else
                                            {
                                                var workItemRecord = new DetailedWorkProgressWorkItemViewModel
                                                {
                                                    ItemCode = workItem.ItemCode,
                                                    Description = workItem.ItemName,
                                                    ContractQuantity = workItem.TotalQuantityOfWork,
                                                    Unit = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                                    UnitPrice = workItem.TotalUnitCost,
                                                    ActualTakeOffQuantity = workItem.ActualQuantityOfWork,

                                                    PreviousPlannedQuantity = ProjectProgressHelper.GetPlannedQuantityOfWorkTillPrevMonth(currentPeriod.ConstructionPeriodId, subId, workItem.WorkItemPerProjectId),
                                                    QuantityOfWorkPlannedThisMonth = ProjectProgressHelper.GetPlannedQuantityForThisMonth(currentPeriod.ConstructionPeriodId, subId, workItem.WorkItemPerProjectId),

                                                    QuantityOfWorkExcutedThisMonth = ProjectProgressHelper.GetExecudtedQuantityOfWorkThisMonth(currentPeriod.ConstructionPeriodId, subId, workItem.WorkItemPerProjectId),
                                                    QuantityOfWorkExecutedTillPrevMonth = ProjectProgressHelper.GetExecudtedQuantityOfWorkTillPrevMonth(currentPeriod.ConstructionPeriodId, subId, workItem.WorkItemPerProjectId),


                                                    //CostOfWorkThisMonth = ProjectProgressHelper.GetExcutedAmountThisMonth(currentPeriod.ConstructionPeriodId, workItem.WorkItemPerProjectId),
                                                    //CostOfWorkTillPrevMonth = ProjectProgressHelper.GetExcutedAmountTillPrevMonth(currentPeriod.ConstructionPeriodId, workItem.WorkItemPerProjectId),
                                                };
                                                workItemRecord.ToDatePlannedQuantity = workItemRecord.PreviousPlannedQuantity + workItemRecord.QuantityOfWorkPlannedThisMonth;
                                                workItemRecord.ToDateActualQuantity = workItemRecord.QuantityOfWorkExcutedThisMonth + workItemRecord.QuantityOfWorkExecutedTillPrevMonth;

                                                workItemRecord.TotalAccVsContractQuantity = Math.Round(workItemRecord.ContractQuantity > 0 ? workItemRecord.ToDateActualQuantity / workItemRecord.ContractQuantity : 0, 2);
                                                workItemRecord.ExcVsPlanThisMonth = Math.Round(workItemRecord.QuantityOfWorkPlannedThisMonth > 0 ? workItemRecord.QuantityOfWorkExcutedThisMonth / workItemRecord.QuantityOfWorkPlannedThisMonth : 0, 2);

                                                workItemRecord.CostOfWorkThisMonth = workItemRecord.QuantityOfWorkExcutedThisMonth * workItem.TotalUnitCost;
                                                workItemRecord.CostOfWorkTillPrevMonth = workItemRecord.QuantityOfWorkExecutedTillPrevMonth * workItem.TotalUnitCost;
                                                workItemRecord.CostOfWorkToDate = workItemRecord.CostOfWorkThisMonth + workItemRecord.CostOfWorkTillPrevMonth;

                                                categoryRecord.WorkItems.Add(workItemRecord);
                                            }

                                        }
                                        //if (categoryRecord.ContractValue > 0)
                                        //{
                                            structureRecord.CategoryList.Add(categoryRecord);
                                        //}
                                        //structureRecord.CategoryList.Add(categoryRecord);
                                    }
                                }
                                #endregion
                                if (structureRecord.CategoryList.Any())
                                {
                                    blockRecord.StructureList.Add(structureRecord);
                                }
                            }
                        }
                        #endregion

                        list.Add(blockRecord);
                    }
                }
            }

            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkProgressReportPeriodViewModel> GetAvailablePeriods(string projectId)
        {

            List<WorkProgressReportPeriodViewModel> periodsList = new List<WorkProgressReportPeriodViewModel>();
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var utilizationPeriods = db.ResourceUtilizationPeriods.Where(up => up.ProjectInformationId == project.ProjectInformationId).ToList();
                var ids = new HashSet<int>(utilizationPeriods.Select(p => p.ConstructionPeriodId));
                var filteredPeriods = db.ConstructionPeriod.Where(p => (ids.Contains(p.ConstructionPeriodId))).ToList();
                if (filteredPeriods.Any())
                {
                    foreach (ConstructionPeriod period in filteredPeriods)
                    {
                        var periodRecord = new WorkProgressReportPeriodViewModel
                        {
                            PeriodId = period.ConstructionPeriodId,
                            Name = period.Name
                        };
                        periodsList.Add(periodRecord);
                    }
                }

            }
            return periodsList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }
    }
}
