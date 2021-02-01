using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.ViewModel.Print
{
    public class ArMiscCollectionViewModel : Organization

    {
        public string VoucherId { get; set; }
        public string DocTitile { get; set; }
        public string CustomerName { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string ReceiptMethod { get; set; }
        public decimal ReceiptAmount { get; set; }
        //
        public string PreparedBy { get; set; }
        public string ApprovedBy { get; set; }
    }
}
