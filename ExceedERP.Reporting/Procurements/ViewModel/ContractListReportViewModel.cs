using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Procurement.Settings;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class ContractListReportViewModel
    {
        public string CompanyName { get; set; }
        public string ItemName { get; set; }
        public string ContractRef { get; set; }
        public string TenderType { get; set; }
        public string BidNo { get; set; }
        public string ProcurementType { get; set; }
        public PurchaseCategory ProcurementMethod { get; set; }
        public DateTime SigningDate { get; set; }
        public double DateRange { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public string Branch { get; set; }
        public decimal Advance { get; set; }
        public decimal Cost { get; set; }
        public string Other { get; set; }
    }
}
