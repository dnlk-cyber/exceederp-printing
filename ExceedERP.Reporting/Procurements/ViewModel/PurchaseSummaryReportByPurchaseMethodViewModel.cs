using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class PurchaseSummaryReportByPurchaseMethodViewModel
    {
        public  string Descrption { get; set; }
        public decimal OpenTenders { get; set; }
        public decimal SelectedTenders { get; set; }
        public decimal Proforma { get; set; }
        public decimal DirectPurchase { get; set; }
        public decimal Other { get; set; }
        public string Remark { get; set; }
    }
}
