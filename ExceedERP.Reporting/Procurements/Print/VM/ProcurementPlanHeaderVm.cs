using System;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.Print.VM
{
    

    public class ProcurementPlanHeaderVm : Organization
    {
        public string VoucherId { get; set; }
        public string Year { get; set; }
        public string Branch { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public string Proforma { get; set; }
        public string CreatedBy { get; set; }
        public string CreateDate { get; set; }
        public string Checker { get; set; }
        public string CheckedDate { get; set; }
        public string Approver { get; set; }
        public string ApprovedDate { get; set; }
    }
}
