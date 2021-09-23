////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Domain\HRM\Payroll\PayrollInputs.cs
//
// summary:	Implements the payroll inputs class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Payroll
{
    public class PayrollInputs
    {
    }
   #region Addition

   ////////////////////////////////////////////////////////////////////////////////////////////////////
   /// <summary>    An addition. </summary>
   ///
   /// <remarks>    Mekanehiwot, 10/31/2016. </remarks>
   ////////////////////////////////////////////////////////////////////////////////////////////////////

   public class Addition:TrackUserOperation
    {
       [Key ]
       public int AdditionId { get; set; }
       [Required]
       [Display(Name = "HR_Payroll_PayrollInputs_Addition_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
       public virtual Person_Employee Employee { get; set; }
       [Required ]
       [Display(Name = "HR_Payroll_PayrollInputs_Addition_PeriodId", ResourceType = typeof(Localization.Resources))]
       public int GLPeriodId { get; set; }
       //public virtual PayrollPeriod GLPeriod { get; set; }
       [Required ]
       [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
       [Display(Name = "HR_Payroll_PayrollInputs_Addition_Amount", ResourceType = typeof(Localization.Resources))]
       public decimal Amount { get; set; }
       [Required ]
       [Display(Name = "HR_Payroll_PayrollInputs_Addition_LookupId", ResourceType = typeof(Localization.Resources))]
       public int LookupId { get; set; }
       public virtual Lookup Lookup { get; set; }
       [Display(Name = "HR_Payroll_PayrollInputs_Addition_AdditionTaxType", ResourceType = typeof(Localization.Resources))]
       public AdditionTaxType AdditionTaxType { get; set; }
         [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
      
       public string Remark;


    }
#endregion 
#region

    public class PeriodStatus
    {
        [Key]
        public int PeriodStatusId { get; set; }
        [Required ]
        public int GLPeriodId { get; set; }
       // public virtual PayrollPeriod GLPeriod { get; set; }
          //[Required]
        public int BranchId { get; set; }
        //  public virtual Branch Branch { get; set; }
          [Required]
        public PeriodStatusList  Status { get; set; }
        public string Remark { get; set; }
        public bool IsLocked { get; set; }
    }

    #endregion
}
