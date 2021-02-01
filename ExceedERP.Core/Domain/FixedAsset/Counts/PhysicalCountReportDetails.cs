using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public enum PhysicalStatus
    {
        Good,
        Damaged,
        Lost
    }
    public class  PhysicalCountReportDetails : TrackUserOperation
    {
        public int  PhysicalCountReportDetailsID { get; set; }
        public int  PhysicalCountReportID { get; set; }
        public virtual  PhysicalCountReport  PhysicalCountReport { get; set; }
        public int  RegistrationID { get; set; }
        public virtual  AssetRegistration  AssetRegistration { get; set; }
        [Display(Name = "Physical Status")]
        [UIHint("PhysicalStatus")]
        public PhysicalStatus Status { get; set; }
        [Display(Name = "Cost of Maintenance/Replacement")]
        public decimal Cost { get; set; }
        public string Description { get; set; }
    }
}
