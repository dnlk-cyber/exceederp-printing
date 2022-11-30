using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.JobCosting.ViewModel
{
   public class JobOrderCardVM : Organization
    {
        public string ReceivedDate { get; set; }
        public string JobOrderNo { get; set; }
        public string Year { get; set; }

        public decimal Quantity { get; set; }
        public string Customer { get; set; }
        public string ReceiptNo { get; set; }
        public string OrderSize { get; set; }
        public decimal Remaining { get; set; }
        public decimal SalesPrice { get; set; }
        public string Description { get; set; }
        public virtual List<JobOrderCardItem> JobOrderCardItems { get; set; }
    }
    public class JobOrderCardItem
    {
        public string CostCenter { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Date { get; set; }
        public string Reference { get; set; }
        public decimal MaterialQty { get; set; }
        public decimal WorkingHour { get; set; }
        public decimal DirectMaterial { get; set; }
        public decimal DirectLabor { get; set; }
        public decimal MOH { get; set; }
        public decimal Total { get; set; }
    }

}
