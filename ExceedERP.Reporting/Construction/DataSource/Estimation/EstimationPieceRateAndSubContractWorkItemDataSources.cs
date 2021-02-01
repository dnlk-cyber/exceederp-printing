using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Construction.ViewModel;

namespace ExceedERP.Reporting.Construction.DataSource
{
    // 
    //  summary:
    //      Project estimation piece rate work item object data source
    [DataObject]
    public class EstimationPieceRateWorkItemOds : List<EstimationPieceRateWorkItemViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EstimationPieceRateWorkItemViewModel> GetAvailablePieceRateWorkItems(string projectTypeId)
        {
            List<EstimationPieceRateWorkItemViewModel> list = new List<EstimationPieceRateWorkItemViewModel> { };

            var workItems = _db.ProjectEstimationAreaWorkItem.ToList();

            int typeId = Convert.ToInt32(projectTypeId);

            if (projectTypeId != null)
            {
                workItems = workItems.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();
            }
            foreach (ProjectEstimationAreaWorkItem workItem in workItems)
            {
                var  record = new EstimationPieceRateWorkItemViewModel
                {
                    ProjectType =  workItem.ProjectType.ToString(),
                    WorkCategory =  workItem.WorkCategory.Name,
                    WorkItemName =  workItem.WorkItemName,
                    Width = workItem.Width,
                    Height = workItem.Height,
                    Area = workItem.Area,
                    Price = workItem.Price,
                    MaterialCost = workItem.MaterialCost,
                    TransportCost = workItem.TransportCost,
                    CylinderLock = workItem.CylinderLock,
                    DirectCost = workItem.DirectCost,
                    OverHeadProfitCost = workItem.OverHeadProfitCost
                };
                list.Add(record);
            }
            return list;
        }
    }

    // 
    //  summary:
    //      Project estimation sub contract work item object data source
    [DataObject]
    public class EstimationSubcontractWorkItemOds : List<EstimationSubContractWorkItemViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EstimationSubContractWorkItemViewModel> GetAvailableSubContractWorkItems(string projectTypeId)
        {
            List<EstimationSubContractWorkItemViewModel> list = new List<EstimationSubContractWorkItemViewModel>();
            var workItems = _db.EstimationSubContractWorkItem.ToList();

            int typeId = Convert.ToInt32(projectTypeId);

            if (projectTypeId != null)
            {
                workItems = workItems.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();
            }
            foreach (EstimationSubContractWorkItem workItem in workItems)
            {
                var record = new EstimationSubContractWorkItemViewModel
                {
                    ProjectType = workItem.ProjectType.ToString(),
                    WorkCategory = workItem.WorkCategory.Name,
                    WorkItemName = workItem.WorkItemName,
                    UnitOfMeasurement = this.GetUnitOfMearurnment(workItem.UnitOfMeasurementId),
                    Quntity = workItem.Quntity,
                    SubContractCostOne = workItem.SubContractCostOne,
                    SubContractCostTwo = workItem.SubContractCostTwo,
                    SubContractCostThree = workItem.SubContractCostThree,
                    SelectedCost = workItem.SelectedCost,
                    OverHeadProfit = workItem.OverHeadProfit,
                    TotalCost = workItem.TotalCost
                };
                list.Add(record);
            }
            return list;
        }

        private string GetUnitOfMearurnment(int id)
        {
            var unit = new Core.Domain.Construction.UnitOfMeasurement();
            var temp = _db.UnitOfMeasurement.Find(id);
            if (temp != null)
            {
                unit = temp;
            }
            return unit.Abbreviation;
        }
    }
}
