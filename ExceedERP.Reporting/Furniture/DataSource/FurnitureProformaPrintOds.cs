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
    public class FurnitureProformaPrintOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FurnitureProformaViewModel ProformaHeader(int id)
        {
            
            FurnitureProformaViewModel vm = new FurnitureProformaViewModel();
            var proforma = db.FurnitureProformaInvoices.FirstOrDefault(x => x.FurnitureProformaInvoiceId == id);
            if(proforma != null)
            {
                var glTaxRate = db.GLTaxRates.Find(proforma.GLTaxRateID);
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == proforma.CustomerId);
                return new FurnitureProformaViewModel
                {
                    CustomerName = customer.TradeName,
                    TransportCost = proforma.TransporCost,
                    VAT = glTaxRate?.TaxName ?? "",
                    VATAmount = proforma.TransporCost * glTaxRate?.CalculatedRate ?? 0,
                    TransportCostWithVAT = proforma.TransportCostTotal,
                    DeliveryDate = proforma.DeliveryDate,
                    ModeOfPayment = proforma.ModeOfPayment,
                    PriceValidityPeriod = proforma.PriceValidityPeriod,
                    ProformaNo = proforma.FurnitureProformaInvoiceId,
                    ApprovedBy = proforma.OnlineApprovedBy,
                    ApprovedDate = proforma.OnlineApprovedTime,
                    CheckedBy = proforma.SendForApprovalBy,
                    CheckedDate = proforma.SendForApprovalTime,
                    CreatedBy = proforma.CreatedBy,
                    DateCreated = proforma.DateCreated,
                    CreatedBySignature = SignatureHelper.GetUserSignature(proforma.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(proforma.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(proforma.SendForApprovalBy)
                };
            }
            return vm;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureProformaItemViewModel> ProformaItem(int id)
        {
            List<FurnitureProformaItemViewModel> list = new List<FurnitureProformaItemViewModel>();
            var proforma = db.FurnitureProformaInvoices.FirstOrDefault(x => x.FurnitureProformaInvoiceId == id);
            if(proforma != null)
            {
                var invoiceItems = db.FurnitureProformaInvoiceItems.Where(x => x.FurnitureProformaInvoiceId == proforma.FurnitureProformaInvoiceId).ToList();
                foreach (var item in invoiceItems)
                {
                    var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == item.JobTypeId);
                    var glTaxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == item.GLTaxRateID);
                    list.Add(new FurnitureProformaItemViewModel
                    {
                        JobType = jobType?.JobTypeName ?? "",
                        Quantity = item.Quantity,
                        SellingPrice= item.TotalPrice,
                        UnitPriceBeforeVAT = item.UnitPriceBeforeVAT,
                        UOM = item.UnitOfMeasurment,
                        VAT = glTaxRate?.CalculatedRate ?? 0
                    });

                }
                
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public FurnitureTotalViewModel GetTotal(int id) 
        {
            var proforma = db.FurnitureProformaInvoices.FirstOrDefault(x => x.FurnitureProformaInvoiceId == id);
            if (proforma != null)
            {
                var invoiceItems = db.FurnitureProformaInvoiceItems.Where(x => x.FurnitureProformaInvoiceId == proforma.FurnitureProformaInvoiceId).ToList();
                var total = invoiceItems.Sum(s => s.TotalPrice) + proforma.TransportCostTotal;
                return new FurnitureTotalViewModel
                {
                    Total = total
                };
            }
            return new FurnitureTotalViewModel();
        }
    }
}
