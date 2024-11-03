using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{

    public class GuarantorOrSurety: TrackUserOperation
    {
        public int GuarantorOrSuretyId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Cosign_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        public string Remark { get; set; }


    }
    public class GuarantorOrSuretyDetail: TrackUserOperation
    {
        public int GuarantorOrSuretyDetailId { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        public string Organiztion { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_SelfCare_OverTimeRequest_FromDate", ResourceType = typeof(Localization.Resources))]
        public DateTime FromDate { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_FromDate", ResourceType = typeof(Localization.Resources))]
        public string FromDateAm { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_SelfCare_OverTimeRequest_ToDate", ResourceType = typeof(Localization.Resources))]
        public DateTime ToDate { get; set; }
        [Display(Name = "HR_SelfCare_OverTimeRequest_ToDate", ResourceType = typeof(Localization.Resources))]
        public string ToDateAm { get; set; }
        public bool IsActive { get; set; }
        public int GuarantorOrSuretyId { get; set; }
        public virtual GuarantorOrSurety Guarantor { get; set; }

    }
}