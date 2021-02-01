using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class ContractByCategoryReportViewModel
    {
        public string ContractWarranty { get; set; }
        public string SupplierName { get; set; }
        public decimal ContractAmount { get; set; }
        public string ContractRefNo { get; set; }
        public string TenderRef { get; set; }
        public decimal Advance { get; set; }
        public string Remark { get; set; }
    }
}
