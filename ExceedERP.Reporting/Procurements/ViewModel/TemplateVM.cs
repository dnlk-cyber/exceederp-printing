using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
    public class TemplateVM : Organization
    {
        public string Let1 { get; set; }
        public string Let2 { get; set; }
        public string Let3 { get; set; }
        public string Let4 { get; set; }
        public string Let5 { get; set; }
        public string Let6 { get; set; }
        public string Summary { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }

        //for Proforma
        public string WinnerSupplier { get; set; }
        public int ProformaId { get; set; }
        //for tender
        public string TenderRefNo { get; set; }
        public string Branch { get; set; }
        public string EvaluationDate { get; set; }
        public string TenderReference { get; set; }
        public string Description { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedDate { get; set; }
        public string CheckedBy { get; set; }
        public string CheckedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string SupplierName { get; set; }
    }
}
