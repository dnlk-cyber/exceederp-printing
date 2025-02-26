
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
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == proformaHead.CustomerId);
                return new PrintingProformaViewModel
                {
                    CustomerName = customer.TradeName,
                    DeliveryPeriod = proformaHead.DeliveryPeriod,
                    ProformaNumber = proformaHead.PrintingProformaInvoiceId,
                    Remark = proformaHead.Remark,
                    ApprovedBy = proformaHead.OnlineApprovedBy,
                    ApprovedDate = proformaHead.OnlineApprovedTime,
                    CheckedBy = proformaHead.SendForApprovalBy,
                    CheckedDate = proformaHead.SendForApprovalTime 
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
                var jobType = db.PrintingJobTypes.FirstOrDefault(x => x.PrintingJobTypeId == item.JobTypeId);
                var glTaxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == item.GLTaxRateID);
                var glTaxRateTransport = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == item.GLTaxRateTransportId);
                list.Add(new PrintingProformaItemsViewModel
                {
                    JobType = jobType?.JobTypeName ?? "",
                    JobDescription = item.JobDescription,
                    NoOfPage = item.NumberOfPages,
                    Quantity = item.Quantity,
                    Remark = item.Remark,
                    SellingPrice = item.GrandTotalIncludingVAT,
                    TotalPrice = item.TotalPrice,
                    TransportCost = item.TransportCost,
                    UnitPrice = item.UnitPrice,
                    VAT = glTaxRate?.TaxName ?? "",
                    TransportVAT = glTaxRateTransport?.TaxName ?? "",
                    SellingPriceIncludingTransCost = item.GrandTotalIncludingTransport
                });
            }
            return list;
        }
    }
}
