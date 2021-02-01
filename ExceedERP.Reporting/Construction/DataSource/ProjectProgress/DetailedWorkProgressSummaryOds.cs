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
    public class DetailedWorkProgressSummaryOds : List<DetailedWorkProgressSummaryBlockViewModel>
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
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == pId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();
            if (project != null && currentPeriod != null)
            {
                int timeRangeId = Convert.ToInt32(subProjectTypeId);

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
                        var blockWorkItems = db.WorkItemPerProject.Where(w => w.BlockId == block.BlockId).ToList();
                        if (blockWorkItems.Any())
                        {
                            blockCost = blockWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                        }
                        #endregion
                        var blockRecord = new DetailedWorkProgressSummaryBlockViewModel {
                            Name = block.Name + " [ " + ProjectEstimationHelper.GetSubProjectTypeName(block.SubProjectTypeId) + " ]",
                            TotalCost = blockCost
                        };

                        #region structures
                        #region  LINQ for structure types under this very block
                        var structureTypes = (from c in db.Configurations
                                              join w in db.ProjectTypeMappings
                                                  on c.ConfigurationId equals w.ConfigurationId

                                              join p in db.ProjectInformation
                                                  on w.ProjectType equals p.ProjectType
                                              join b in db.Blocks
                                             on p.ProjectInformationId equals b.ProjectInformationId

                                              where
                                             p.ProjectInformationId == id &&
                                              c.Reference == "STRUCTURE_TYPE" &&
                                              b.BlockId == block.BlockId
                                              select (new
                                              {
                                                  c.CurrentValue,
                                                  c.ConfigurationId
                                              })).ToList();
                        #endregion
                        if (structureTypes.Any())
                        {
                            foreach(var str in structureTypes)
                            {
                                var structureRecord = new DetailedWorkProgressSummaryStructureViewModel {
                                    Name = str.CurrentValue
                                };
                                structureRecord.Name = structureRecord.Name.Replace('_', ' ');

                                #region work categories under this very structure
                                var workCategoriesUnderStructure = db.WorkCategory.Where(wc => wc.ConfigurationId == str.ConfigurationId).ToList();
                                if (workCategoriesUnderStructure.Any())
                                {
                                    foreach (WorkCategory category in workCategoriesUnderStructure)
                                    {
                                        #region total cost of work category calculation

                                        var categoryCost = new decimal();
                                        var categoryActualAmountThisMonth = new decimal();
                                        var categoryActualAmountTillPrevMonth = new decimal();
                                        var actualQuantityOfWorkAccomplishedThisMonth = new decimal();
                                        var actualQuantityOfWorkAccomplishedTillPrev = new decimal();
                                        var categoryWorkItems = db.WorkItemPerProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.BlockId == block.BlockId).ToList();
                                        if (categoryWorkItems.Any())
                                        {
                                            categoryCost = categoryWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                                            foreach(WorkItemPerProject workItem in categoryWorkItems)
                                            {
                                                var accomplishedQuantityThisMonth = ProjectProgressHelper.GetExecudtedQuantityOfWorkThisMonth(currentPeriod.ConstructionPeriodId, timeRangeId, workItem.WorkItemPerProjectId);
                                                categoryActualAmountThisMonth += accomplishedQuantityThisMonth * workItem.TotalUnitCost;
                                                actualQuantityOfWorkAccomplishedThisMonth += accomplishedQuantityThisMonth;

                                                var accomplishedQuantityTillPrevMonth = ProjectProgressHelper.GetExecudtedQuantityOfWorkTillPrevMonth(currentPeriod.ConstructionPeriodId, timeRangeId, workItem.WorkItemPerProjectId);
                                                categoryActualAmountTillPrevMonth += accomplishedQuantityTillPrevMonth * workItem.TotalUnitCost;
                                                actualQuantityOfWorkAccomplishedTillPrev += accomplishedQuantityTillPrevMonth;
                                            }
                                        }
                                        #endregion
                                        var categoryRecord = new DetailedWorkProgressSummaryCategoryViewModel
                                        {
                                            Description = category.Name,
                                            ContractValue = categoryCost,
                                            ActualAccomplishedThisMonth = categoryActualAmountThisMonth,
                                            //ActualAccomplishedThisMonth = ProjectProgressHelper.GetActualAccomplishedAmountThisMonth(block.BlockId, category.WorkCategoryId, currentPeriod.ConstructionPeriodId),
                                            PlannedToAccomplishThisMonth = ProjectProgressHelper.GetPlannedAmount(block.BlockId, category.WorkCategoryId, currentPeriod.ConstructionPeriodId),
                                            PlannedToAccomplishTillPrevMonth = ProjectProgressHelper.GetPlannedAmountTillPrevMonth(block.BlockId, category.WorkCategoryId, currentPeriod.ConstructionPeriodId),
                                            //ActualWorkAccomplishedTillPrevMonth = ProjectProgressHelper.GetActualAccomplishedAmountTillPrevMonth(block.BlockId, category.WorkCategoryId, currentPeriod.ConstructionPeriodId),
                                            ActualWorkAccomplishedTillPrevMonth = categoryActualAmountTillPrevMonth,
                                            QuantityOfWorkAccomplishedThisMonth = actualQuantityOfWorkAccomplishedThisMonth,
                                            QuantityOfWorkAccomplishedTillPrevMonth = actualQuantityOfWorkAccomplishedTillPrev,
                                        };
                                        categoryRecord.PercentageOfActualToPlannedThisMonth = categoryRecord.PlannedToAccomplishThisMonth > 0 ? Math.Round(categoryRecord.ActualAccomplishedThisMonth / categoryRecord.PlannedToAccomplishThisMonth, 2) : 0;
                                        categoryRecord.PlannedToAccomplishToDate = categoryRecord.PlannedToAccomplishTillPrevMonth + categoryRecord.PlannedToAccomplishThisMonth;

                                        categoryRecord.QuantityOfWorkAccomplishedToDate = categoryRecord.QuantityOfWorkAccomplishedThisMonth + categoryRecord.QuantityOfWorkAccomplishedTillPrevMonth;

                                        categoryRecord.ActualWorkAccomplishedToDate = categoryRecord.ActualAccomplishedThisMonth + categoryRecord.ActualWorkAccomplishedTillPrevMonth;
                                        categoryRecord.PercentageOfActualToPlannedTillPrevMonth = categoryRecord.PlannedToAccomplishTillPrevMonth > 0 ? categoryRecord.ActualWorkAccomplishedTillPrevMonth / categoryRecord.PlannedToAccomplishTillPrevMonth : 0;
                                        categoryRecord.PercentageOfActualToPlannedTillPrevMonth = Math.Round(categoryRecord.PercentageOfActualToPlannedTillPrevMonth, 2);

                                        categoryRecord.RatioOfActualAccomplishedToContractValueThisMonth = categoryRecord.ContractValue > 0 ? categoryRecord.ActualAccomplishedThisMonth / categoryRecord.ContractValue : 0;
                                        categoryRecord.RatioOfActualAccomplishedToContractValueThisMonth = Math.Round(categoryRecord.RatioOfActualAccomplishedToContractValueThisMonth, 4);

                                        categoryRecord.RatioOfActualAccmoplishedToContractValueToDate = categoryRecord.ContractValue > 0 ? categoryRecord.ActualWorkAccomplishedToDate / categoryRecord.ContractValue : 0;
                                        categoryRecord.RatioOfActualAccmoplishedToContractValueToDate = Math.Round(categoryRecord.RatioOfActualAccmoplishedToContractValueToDate, 4);
                                        structureRecord.CategoryList.Add(categoryRecord);
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
