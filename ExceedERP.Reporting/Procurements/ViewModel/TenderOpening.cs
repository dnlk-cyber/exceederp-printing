using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   public class TenderOpening : Organization
    {
        public string TenderRefNo { get; set; }
        public string Branch { get; set; }
        public DateTime? FinancialOpeningDate { get; set; }
        public DateTime? TechnicalOpeningDate { get; set; }
        public int? OpeningHour { get; set; }
        public string SupplierAddress { get; set; }
        public string Let1 { get; set; }
        public string Let2 { get; set; }
        public string Let3 { get; set; }
        public string Let4 { get; set; }
        public string Let5 { get; set; }
        public string Subject { get; set; }
        public string Summary { get; set; }
    }
}
