using System;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.Print.VM
{
    

    public class ProcurementNeedHeaderVm : Organization
    {
        public string VoucherId { get; set; }
        public string Year { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string Requester { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public string CreateDate { get; set; }
        public string Approver { get; set; }
        public string ApprovedDate { get; set; }
    }
}
