using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Furniture.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.DataSource
{
    [DataObject]
    public class FurnitureEstimationSummaryOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FurnitureEstimationSummaryViewModel SummaryHeader(int id)
        {

            FurnitureEstimationSummaryViewModel vm = new FurnitureEstimationSummaryViewModel();
            var estimationSummary = db.EstimationSummaries.FirstOrDefault(x => x.EstimationSummaryId == id);
            if (estimationSummary != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == estimationSummary.CustomerId);
                return new FurnitureEstimationSummaryViewModel
                {
                    CustomerName = customer?.TradeName ?? "",
                    Status = estimationSummary.Status,
                    ApprovedBy = estimationSummary.OnlineApprovedBy,
                    ApprovedDate = estimationSummary.OnlineApprovedTime,
                    CheckedBy = estimationSummary.SendForApprovalBy,
                    CheckedDate = estimationSummary.SendForApprovalTime,
                    CreatedBy = estimationSummary.CreatedBy,
                    DateCreated = estimationSummary.DateCreated,
                    CreatedBySignature = SignatureHelper.GetUserSignature(estimationSummary.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(estimationSummary.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(estimationSummary.SendForApprovalBy)
                };
            }
            return vm;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureSummaryDetail> SummaryDetail(int id)
        {
            List<FurnitureSummaryDetail> list = new List<FurnitureSummaryDetail>();
            var summaryHeader = db.EstimationSummaries.FirstOrDefault(x => x.EstimationSummaryId == id);
            if (summaryHeader != null)
            {
                var summaryDetails = db.EstimationDetails.Where(x => x.EstimationSummaryId == summaryHeader.EstimationSummaryId).ToList();
                foreach (var item in summaryDetails)
                {
                    var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == item.JobTypeId);
                   
                    list.Add(new FurnitureSummaryDetail
                    {
                        JobType = jobType?.JobTypeName ?? "",
                        Quantity = item.Quantity,
                        AdminCost = item.AdministrativeCost,
                        LabourCost = item.LabourCost,
                        ManufacturingCost = item.ManufacturingCost,
                        MaterialCost = item.MaterialCost,
                        OverHeadCost = item.OverHeadCost,
                        ProfitMargin = item.ProfitMargin,
                        TotalCost = item.GrandTotalCost,
                        
                    });

                }

            }
            return list;
        }
    }
}
