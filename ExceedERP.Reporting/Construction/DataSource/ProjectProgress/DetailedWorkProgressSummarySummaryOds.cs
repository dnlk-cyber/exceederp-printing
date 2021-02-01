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
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class DetailedWorkProgressSummarySummaryOds : List<DetailedWorkProgressSummarySubProjectTypeViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DetailedWorkProgressSummarySubProjectTypeViewModel> GetAllSubProjectTypes(string projectId,string subProjectTypeId, string periodId, string subPeriodId)
        {
            List<DetailedWorkProgressSummarySubProjectTypeViewModel> list = new List<DetailedWorkProgressSummarySubProjectTypeViewModel>();

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
                int timeRangeId = Convert.ToInt32(subPeriodId);
                int subId = Convert.ToInt32(subProjectTypeId);
                var subTypes = db.SubProjectType.Where(b => b.ProjectInformationId == id).ToList();
               // var specificBlock = db.Blocks.Where(b => b.BlockId == bId && b.ProjectInformationId == id).ToList();
                if (subProjectTypeId != null)
                {
                    subTypes = subTypes.Where(s => s.SubProjectTypeId == subId).ToList();
                }
                if (subTypes.Any())
                {
                    foreach(SubProjectType subProject in subTypes)
                    {
                        var subProjectTypeRecord = new DetailedWorkProgressSummarySubProjectTypeViewModel
                        {
                            Name = subProject.Name,
                        };

                        var blocks = db.Blocks.Where(b => b.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                        if (blocks.Any())
                        {
                            foreach (Block block in blocks)
                            {
                                #region block cost calculation
                                var blockCost = new decimal();
                                var actualAmountThisMonth = new decimal();
                                var actualAmountTillPrevMonth = new decimal();
                                var blockWorkItems = db.WorkItemPerProject.Where(w => w.BlockId == block.BlockId).ToList();
                                if (blockWorkItems.Any())
                                {
                                    blockCost = blockWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                                    foreach(WorkItemPerProject workItem in blockWorkItems)
                                    {
                                        var quantityThisMonth = ProjectProgressHelper.GetExecudtedQuantityOfWorkThisMonth(currentPeriod.ConstructionPeriodId, timeRangeId, workItem.WorkItemPerProjectId);
                                        var quantityTillPrevMonth = ProjectProgressHelper.GetExecudtedQuantityOfWorkTillPrevMonth(currentPeriod.ConstructionPeriodId, timeRangeId, workItem.WorkItemPerProjectId);

                                        actualAmountThisMonth += quantityThisMonth * workItem.TotalUnitCost;
                                        actualAmountTillPrevMonth = quantityTillPrevMonth * workItem.TotalUnitCost;
                                    }
                                }
                                #endregion
                                var blockRecord = new DetailedWorkProgressSummaryBlockViewModel
                                {
                                    Name = block.Name,
                                    TotalCost = blockCost,

                                    PlannedAmountThisMonth = ProjectProgressHelper.GetPlannedAmountOfBlockThisMonth(block.BlockId, currentPeriod.ConstructionPeriodId),
                                    //ActualAccomplishedAmountThisMonth = ProjectProgressHelper.GetActualAccomplishedAmountForBlockThisMonth(block.BlockId, currentPeriod.ConstructionPeriodId),
                                    ActualAccomplishedAmountThisMonth = actualAmountThisMonth,

                                    PlannedAmountTillPrevMonth = ProjectProgressHelper.GetPlannedAmountForBlockTillPrevMonth(block.BlockId, currentPeriod.ConstructionPeriodId),

                                    //AccomplishedAmountTillPrevMonth = ProjectProgressHelper.GetActualAccomplishedAmountForBlockTillPrevMonth(block.BlockId, currentPeriod.ConstructionPeriodId),
                                    AccomplishedAmountTillPrevMonth = actualAmountTillPrevMonth,
                                };
                                blockRecord.PercentageOfActualToPlannedAmountThisMonth = blockRecord.PlannedAmountThisMonth > 0 ? blockRecord.ActualAccomplishedAmountThisMonth / blockRecord.PlannedAmountThisMonth : 0;
                                blockRecord.PlannedAmountToDate = blockRecord.PlannedAmountThisMonth + blockRecord.PlannedAmountTillPrevMonth;

                                blockRecord.AccomplishedAmountToDate = blockRecord.ActualAccomplishedAmountThisMonth + blockRecord.AccomplishedAmountTillPrevMonth;
                                blockRecord.PercentageOfActualToPlannedAmountToDate = blockRecord.PlannedAmountToDate > 0 ? blockRecord.AccomplishedAmountToDate / blockRecord.PlannedAmountToDate : 0;

                                blockRecord.AccomplishedVsContractValueThisMonth = blockRecord.TotalCost > 0 ? blockRecord.ActualAccomplishedAmountThisMonth / blockRecord.TotalCost : 0;
                                blockRecord.AccomplishedVsContractValueToDate = blockRecord.TotalCost > 0 ? blockRecord.AccomplishedAmountToDate / blockRecord.TotalCost : 0;

                                subProjectTypeRecord.Blocks.Add(blockRecord);
                            }
                        }
                        list.Add(subProjectTypeRecord);
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
