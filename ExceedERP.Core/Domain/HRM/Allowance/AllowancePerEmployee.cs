using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    /// <summary>   An allowance per employee. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AllowancePerEmployee :TrackUserOperation 
    {
        [Key ]
        public int AllowanceId { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_AllowancePerEmployee_AllowanceDefnId", ResourceType = typeof(Localization.Resources))]
        public int AllowanceDefinationId { get; set; }
        public virtual AllowanceDefination AllowanceDefination
        {
            get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_Allowance_AllowancePerEmployee_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_Allowance_AllowancePerEmployee_Pend", ResourceType = typeof(Localization.Resources))]
        public bool Pend { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]       
        [Display(Name = "HHR_Allowance_AllowanceDefination_From", ResourceType = typeof(Localization.Resources))]
        public DateTime? From { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        
        [Display(Name = "HR_Allowance_AllowanceDefination_To", ResourceType = typeof(Localization.Resources))]
        public DateTime? To { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}