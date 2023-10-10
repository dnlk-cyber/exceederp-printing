using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.CPOManagement
{
    public class CPOManagement : TrackUserOperation
    {
        public int CPOManagementID { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Required]
        [Display(Name = "Item Description")]
        public string ItemDesc { get; set; }
        [Required]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }
        [Display(Name = "Advance Amount")]
        public decimal? AdvanceAmount { get; set; }
        [Display(Name = "Advance Deadline")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AdvanceDeadline { get; set; }
        [Display(Name = "Performance Amount")]
        public decimal? PerformanceAmount { get; set; }
        [Display(Name = "Performance Deadline")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PerformanceDeadline { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public string Bank { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}
