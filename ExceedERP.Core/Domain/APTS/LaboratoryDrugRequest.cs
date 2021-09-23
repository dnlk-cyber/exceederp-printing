using System;

namespace ExceedERP.Core.Domain.APTS
{
    public class LaboratoryDrugRequest
    {
        public int LaboratoryDrugRequestId { get; set; }
        public int BranchId { get; set; }
        public int GlFiscalYearId { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestDateAm { get; set; }
        public string RequestedBy { get; set; }
        public DateTime DateCreated { get; set; }   
        public bool IsSendForApproval { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsClosed { get; set; }
        public string Remark { get; set; }
    }
}
