using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class PurchaseFollowUpReportViewModel
    {
        public int PurchaseRequistionId { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Branch { get; set; }
        public string AllowanceDate { get; set; }
        public DateTime StartingDate { get; set; }
        public string PurchaserName { get; set; }
        public string ProcurementMethod { get; set; }
        public DateTime? TechnicalOpen { get; set; }
        public DateTime? FinancialOpen { get; set; }
        public string ContractOngoing { get; set; }
        public string NotComplated { get; set; }
        public string Remark { get; set; }
    }
}
