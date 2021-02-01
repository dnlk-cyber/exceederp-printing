using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Settings
{
    public class ProcurementPOType : TrackUserOperation
    {
        public int ProcurementPOTypeID { get; set; }
        [Required]
        [Display(Name = "PO Type")]
        public string POType { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
    }
}