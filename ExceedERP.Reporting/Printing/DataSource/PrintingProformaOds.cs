
using ExceedERP.Core.Domain.Printing;
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
    public class PrintingProformaOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintingProformaViewModel GetProformaHeader(int id)
        {
            var proformaHead = db.PrintingProformaInvoices.Find(id);
            if(proformaHead != null)
            {
                var glTax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == proformaHead.GLTaxRateID);
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == proformaHead.CustomerId);
                return new PrintingProformaViewModel
                {
                    CustomerName = customer?.TradeName ?? string.Empty,
                    DeliveryPeriod = proformaHead.DeliveryPeriod,
                    ProformaNumber = proformaHead.PrintingProformaInvoiceId,
                    Remark = proformaHead.Remark,
                    ApprovedBy = proformaHead.OnlineApprovedBy,
                    ApprovedDate = proformaHead.OnlineApprovedTime,
                    CheckedBy = proformaHead.SendForApprovalBy,
                    CheckedDate = proformaHead.SendForApprovalTime,
                    CreatedBy = proformaHead.CreatedBy,
                    DateCreated = proformaHead.DateCreated,
                    PriceValidityPeriod = proformaHead.PriceValidityPeriod,
                    ModeOfPayment = proformaHead.ModeOfPayment,
                    PaymentType = proformaHead.PaymentType.ToString(),
                    TransportCost = proformaHead.TransporCost,
                    VAT = glTax?.TaxName ?? string.Empty,
                    TotalTransportCost = proformaHead.TransportCostTotal,
                    VATAmount =proformaHead.TransporCost * glTax?.CalculatedRate ?? 0,

                };

            }
            return new PrintingProformaViewModel();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrintingProformaItemsViewModel> GetProformaItems(int id)
        {
            List<PrintingProformaItemsViewModel> list = new List<PrintingProformaItemsViewModel>();
            var proformaHead = db.PrintingProformaInvoices.Find(id);
            if (proformaHead == null)
            {
                return list;
            }
            var proformaItems = db.PrintingProformaInvoiceItems.Where(x => x.PrintingProformaInvoiceId == proformaHead.PrintingProformaInvoiceId).ToList();
            foreach (var item in proformaItems)
            {
                var estimation = db.PrintingCostEstimations.Find(item.PrintingCostEstimationId);
                var text = db.PrintingEstimationMaterialCosts
                    .FirstOrDefault(x => x.PrintingCostEstimationId == item.PrintingCostEstimationId &&
                    x.EstimationMaterialType == EstimationMaterialType.TextPaper);
                var cover = db.PrintingEstimationMaterialCosts
                    .FirstOrDefault(x => x.PrintingCostEstimationId == item.PrintingCostEstimationId &&
                    x.EstimationMaterialType == EstimationMaterialType.CoverPaper);
                var materialItemText = db.PrintingMaterialCategoryItems
                    .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == text.PrintingMaterialCategoryItemId);
                var materialItemCover = db.PrintingMaterialCategoryItems
                    .FirstOrDefault(x => x.PrintingMaterialCategoryItemId == cover.PrintingMaterialCategoryItemId);
                var paperSize = db.PrintingPaperSizes
                    .Find(item.TextPaperSizeId);
                var glTaxRate = db.GLTaxRates
                    .FirstOrDefault(x => x.GLTaxRateID == item.GLTaxRateID);

                list.Add(new PrintingProformaItemsViewModel
                {
                    JobType = item.PrintJobType,
                    JobDescription = item.JobDescription,
                    NoOfPage = item.NumberOfPages,
                    Quantity = item.Quantity,
                    Remark = item.Remark,
                    SellingPrice = item.GrandTotalIncludingVAT,
                    TotalPrice = item.TotalPrice,
                    UnitPrice = item.UnitPrice,
                    VAT = glTaxRate?.TaxName ?? string.Empty,
                    BindingType = item?.BindingStyle.ToString() ?? string.Empty,
                    PaperSize = paperSize?.PrintPaperSize.ToString() ?? string.Empty,
                    CoverPaperType = materialItemCover?.ItemName ?? string.Empty,
                    TextPaperType = materialItemText?.ItemName ?? string.Empty,
                    ColorText = estimation?.TextNoOfColors ?? 0,
                    ColorCover = estimation?.CoverNoOfColor ?? 0
                });
            }
            return list;
        }
        
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintTotalViewModel GetTotal(int id)
        {
            var proformaHead = db.PrintingProformaInvoices.Find(id);
            if (proformaHead != null)
            {
                var proformaItems = db.PrintingProformaInvoiceItems
                    .Where(x => x.PrintingProformaInvoiceId == proformaHead.PrintingProformaInvoiceId)
                    .ToList();
                var total = proformaItems.Sum(s => s.TotalPrice) + proformaHead.TransportCostTotal;
                return new PrintTotalViewModel
                {
                    Total = total
                };
            }
            return new PrintTotalViewModel();
        }
    }
}
