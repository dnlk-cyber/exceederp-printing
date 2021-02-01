using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   public class GuaranteeListReportViewModel
    {
        public string ContractWarranty { get; set; }
        public string SupplierName { get; set; }
        public string Purpose { get; set; }
        public string Bank { get; set; }
        public decimal Amount { get; set; }
        public string GuaranteeRefNo { get; set; }
        public string ContractRefNo { get; set; }
        public string Remark { get; set; }
    }
}
