using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class PrintJobCardOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobCardViewModel GetJobCard(int id)
        {
            var jobCard = db.PrintingJobOrderProductions.FirstOrDefault(x => x.PrintingJobOrderProductionId == id);
            if(jobCard != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobCard.CustomerId);
                var textPaper = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == jobCard.TextPaperSizeId);
                var coverPaper = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == jobCard.CoverPaperSizeId);
                return new JobCardViewModel
                {
                    CoverNoColor = jobCard.CoverNoOfColor,
                    CustomerName = customer.TradeName,
                    DeliveryDate = jobCard.DeliveryDate,
                    JobOrderNo = jobCard.PrintingJobOrderItemId.ToString(),
                    JobDescription = jobCard.JobDescription,
                    JobType = jobCard.PrintJobType.ToString(),
                    OrderedDate = jobCard.DateOrdered,
                    Page = jobCard.TotalTextNoPages,
                    PaperForCover = coverPaper.PaperSizeName,
                    PaperForText = textPaper.PaperSizeName,
                    Quantity = jobCard.Quantity,
                    TextNoColor = jobCard.TextNoOfColors,
                    ApprovedBy = jobCard.OnlineApprovedBy,
                    ApprovedDate = jobCard.OnlineApprovedTime,
                    CheckedBy = jobCard.IsOnlineTransferredBy,
                    CheckedDate = jobCard.OnlineTransferredTime,
                    DateCreated = jobCard.DateCreated,
                    CreatedBy = jobCard.CreatedBy,
                    CreatedBySignature = SignatureHelper.GetUserSignature(jobCard.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(jobCard.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(jobCard.SendForApprovalBy)
                };
            }
            return new JobCardViewModel();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BOQMaterialViewModel> GetMaterialItem(int id)
        {
            List<BOQMaterialViewModel> list = new List<BOQMaterialViewModel>();
            var jobCard = db.PrintingJobOrderProductions.FirstOrDefault(x => x.PrintingJobOrderProductionId == id);
            if(jobCard != null)
            {
                var items = db.PrintingProductionMaterialCosts.Where(x => x.PrintingJobOrderProductionId == jobCard.PrintingJobOrderProductionId).ToList();

                foreach (var item in items)
                {
                    var materialCatItem = db.PrintingMaterialCategoryItems.FirstOrDefault(x => x.PrintingMaterialCategoryItemId == item.PrintingMaterialCategoryItemId);
                    list.Add(new BOQMaterialViewModel
                    {
                        MaterialType = item.EstimationMaterialType.ToString(),
                        ItemName = materialCatItem?.ItemName ?? "",
                        Quantity = item.Quantity,
                        UOM = item.UnitOfMeasurment
                    });
                }
            }            
            return list;
        }
    }
}
