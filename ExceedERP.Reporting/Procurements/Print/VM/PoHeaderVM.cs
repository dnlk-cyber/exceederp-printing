using System;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.Print.VM
{
    

    public class PoHeader : Organization
    {
        public string PoNo { get; set; }
        public string PRNo { get; set; }
        public DateTime PoDate { get; set; }
        public string Supplier { get; set; }
        public string DeliverTo { get; set; }
        public DateTime DeliveryPeriod { get; set; }
        public string Proforma { get; set; }
        public string CreatedBy { get; set; }
        public string CreateDate { get; set; }
        public string Checker { get; set; }
        public string CheckedDate { get; set; }
        public string Approver { get; set; }
        public string ApprovedDate { get; set; }
    }
}
