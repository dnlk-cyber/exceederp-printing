using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.HRM.Allowance
{
    public class EmployeeBenefit : TrackUserOperation
    {
        [Key]
        public int EmployeeBenefitId { get; set; }
        [Display(Name = nameof(Resources.HR_Prohibition_Prohibition_EmployeeCode), ResourceType = typeof(Resources))]
        public string EmployeeCode { get; set; }
        [Display(Name = nameof(Resources.Placement_FullName), ResourceType = typeof(Resources))]
        [Required]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = nameof(Resources.Fiscal_Year), ResourceType = typeof(Resources))]
        [ForeignKey(nameof(FiscalYear))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear FiscalYear { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = nameof(Resources.HR_EmployeeDetail_Placement_OrgStructureId), ResourceType = typeof(Resources))]

        public int OrgStructureId { get; set; }
        [Display(Name = nameof(Resources.Rpt_Branch), ResourceType = typeof(Resources))]

        public int BranchId { get; set; }
        [Display(Name = nameof(Resources.BussinessUnit), ResourceType = typeof(Resources))]
        public int BusinessUnitId { get; set; }
        [Display(Name = "Effective From")]
        [DataType(DataType.Date)]
        public DateTime EffectiveFrom { get; set; }
        [Display(Name = "Effective To")]
        [DataType(DataType.Date)]
        public DateTime? EffectiveTo { get; set; }
        public virtual ICollection<EmployeeBenefitDetail> BenefitDetails { get; set; }
    }

    public class EmployeeBenefitDetail : TrackUserOperation
    {
        [Key]
        public int EmployeeBenefitDetailId { get; set; }
        [Display(Name = "HR_Allowance_OtherBenefit_BenefitName", ResourceType = typeof(Localization.Resources))]
        [Required]
        [ForeignKey(nameof(BenefitType))]
        public int BenefitTypeId { get; set; }
        public virtual Lookup BenefitType { get; set; }
        [Display(Name = "HR_Allowance_OtherBenefit_UnitofMeasurement", ResourceType = typeof(Localization.Resources))]
        public int UnitofMeasurement { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = nameof(Resources.Rpt_Quantity), ResourceType = typeof(Resources))]
        public decimal Quantity { get; set; }
        [Display(Name = "Utilized Quantity")]
        public decimal? UtilizedQuantity { get; set; }  // Renamed from UsedQuantity
        public bool IsUtilized { get; set; }  // Renamed from IsTaken
        [StringLength(500)]
        public string Remark { get; set; }
        [ForeignKey(nameof(Benefit))]
        public int EmployeeBenefitId { get; set; }
        public virtual EmployeeBenefit Benefit { get; set; }
    }
    

}