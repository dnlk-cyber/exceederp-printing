using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Printing.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.DataSource
{
    [DataObject]
    public class PrintingobOrderOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintingJobOrderViewModel GetJobHeader(int id)
        {
            var jobOrder = db.PrintingJobOrders.Find(id);
            if (jobOrder != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);
                return new PrintingJobOrderViewModel
                {
                    CustomerName = customer.TradeName,
                    
                    Remark = jobOrder.Remark,
                    ApprovedBy = jobOrder.OnlineApprovedBy,
                    ApprovedDate = jobOrder.OnlineApprovedTime,
                    CheckedBy = jobOrder.SendForApprovalBy,
                    CheckedDate = jobOrder.SendForApprovalTime,
                    CreatedBy = jobOrder.CreatedBy,
                    DateCreated = jobOrder.DateCreated,
                    CRVNo = jobOrder.CRVNo,
                    JobDeliveryDate = jobOrder.DeliveryDate,
                    JobOrderStartDate = jobOrder.JobStartDate
                };

            }
            return new PrintingJobOrderViewModel();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderDetail> GetSummaryDetail(int id)
        {
            List<JobOrderDetail> list = new List<JobOrderDetail>();
            var details = db.PrintingJobOrderItems.Where(x => x.PrintingJobOrderId == id).ToList();
            foreach (var item in details)
            {
                var paperSize = db.PrintingPaperSizes.Find(item.TextPaperSizeId);
                var costEstimation = db.PrintingCostEstimations.Find(item.PrintingCostEstimationId);
                list.Add(new JobOrderDetail
                {
                    BindingStyle = item.BindingStyle.ToString(),
                    JobDescription = item.JobDescription,
                    JobOrderNo = item.PrintingJobOrderItemId.ToString(),
                    JobType = item.PrintJobType.ToString(),
                    Pages = item.NumberOfPages,
                    PaperSize = paperSize?.PrintPaperSize.ToString() ?? "NONE",
                    Quantity = item.Quantity,
                    Remark = item.Remark,
                    TotalPrice = item.TotalPrice,
                    UnitPrice = item.UnitPrice,
                    CoverColor = costEstimation?.CoverNoOfColor ?? 0,
                    TextColor = costEstimation?.TextNoOfColors ?? 0
                });
            }
            return list;
        }
    }
}
