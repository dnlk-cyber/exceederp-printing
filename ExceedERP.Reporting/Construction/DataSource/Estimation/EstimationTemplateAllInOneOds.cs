using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Construction.ViewModel;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class EstimationTemplateAllInOneOds : List<ProjectEstimaionTemplateViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectEstimaionTemplateViewModel> GetAllProjectEstimationWorkItems(string projectType)
        {
            List<ProjectEstimaionTemplateViewModel> list = new List<ProjectEstimaionTemplateViewModel>();

            var workItems = _db.ProjectEstimationWorkItem.Where(w => !w.IsSubCategory).ToList();
            var pieceRates = _db.ProjectEstimationAreaWorkItem.ToList();
            var subcontractWorkItems = _db.EstimationSubContractWorkItem.ToList();
            var customWorkItems = _db.ProjectEstimationCustomWorkItems.ToList();

            int typeId = Convert.ToInt32(projectType);

            if (projectType != null)
            {
                workItems = workItems.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();
                pieceRates = pieceRates.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();
                subcontractWorkItems = subcontractWorkItems.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();
                customWorkItems = customWorkItems.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();

            }
            if (workItems.Any() || pieceRates.Any() || subcontractWorkItems.Any() || customWorkItems.Any())
            {
                var record = new ProjectEstimaionTemplateViewModel();

                // add work items
                foreach (ProjectEstimationWorkItem workItem in workItems)
                {
                    var workItemRecord = new ProjectEstimaionTemplateWorkItemViewModel
                    {
                        Name = workItem.ItemName,
                        ItemCode = workItem.ItemCode,
                        Category = workItem.WorkCategory.Name,
                        UnitRate = workItem.TotalUnitCost
                    };
                    record.WorkItemList.Add(workItemRecord);
                }
                foreach (ProjectEstimationAreaWorkItem pieceRate in pieceRates)
                {
                    var pieceRateRecord = new ProjectEstimaionTemplatePieceRateViewModel
                    {
                        Name = pieceRate.WorkItemName,
                        Category = pieceRate.WorkCategory.Name,
                        UnitRate = pieceRate.OverHeadProfitCost + pieceRate.DirectCost,
                        ItemCode = pieceRate.ItemCode
                    };
                    record.PieceRateList.Add(pieceRateRecord);
                }
                foreach (EstimationSubContractWorkItem subContractWorkItem in subcontractWorkItems)
                {
                    var subContractWorkItemRecord = new ProjectEstimaionTemplateSubContractViewModel
                    {
                        Name = subContractWorkItem.WorkItemName,
                        Category = subContractWorkItem.WorkCategory.Name,
                        UnitRate = subContractWorkItem.TotalCost,
                        ItemCode = subContractWorkItem.ItemCode
                    };
                    record.SubContractWorkItemList.Add(subContractWorkItemRecord);
                }
                foreach (ProjectEstimationCustomWorkItem customWorkItem in customWorkItems)
                {
                    var customWorkItemRecord = new ProjectEstimaionTemplateCustomWorkItemViewModel
                    {
                        Name = customWorkItem.WorkItemName,
                        UnitRate = customWorkItem.TotalUnitRate
                    };
                    record.CustomWorkItemList.Add(customWorkItemRecord);
                }
                list.Add(record);
            }
            

            return list;
        }
    }
}
