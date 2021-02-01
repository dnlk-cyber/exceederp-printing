////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	domain\hrm\prohibition.cs
//
// summary:	Implements the prohibition class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Prohibition
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>    A prohibition. </summary>
    ///
    /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Prohibition : TrackUserOperation
    {
        [Key]
        public int ProhibitionId { get; set; }

        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_Prohibition_Prohibition_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_Prohibition_Prohibition_EndDate", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EndDate", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string EndDateAm { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_Status", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus LookupId { get; set; }

        [Required]
        [Display(Name = "HR_Prohibition_Prohibition_EvaluationResult", ResourceType = typeof(Localization.Resources))]
        public decimal EvaluationResult { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public int Decision { get; set; }
    }
}
