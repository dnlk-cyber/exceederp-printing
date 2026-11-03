using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>    Acting . </summary>
    ///
    /// <remarks>   Kibrom Abera, 10/28/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class EmployeeCard : TrackUserOperation
    {
        [Key]
        public int EmployeeCardId { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "To Employee")]
        public int? ToEmployeeId { get; set; }
        [DisplayName("From Work Unit")]
        public int? FromOrgStructureId { get; set; }
        [DisplayName("From Business Unit ")]
        public int? FromBusinessUnitId { get; set; }
        [Display(Name = "From Branch")]
        public int FromBranchId { get; set; }
        [Display(Name = "From JobTitle")]
        public int? FromLookupId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FromDate { get; set; }
        [Required]
        public string FromDateAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //[Required]
        [Display(Name = "To date")]
        public DateTime? ToDate { get; set; }
        [Display(Name = "To date")]
        public string ToDateTimeAm { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public decimal? Salary { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "each Record Primary Key(Idex) ")]
        public int EachPrimaryId { get; set; }
        [Display(Name = "Action Type")]
        public EmployeeCardType CardType { get; set; }
        [Display(Name = "Action taken")]
        public string ActionTaken { get; set; }
        public string ActionTakenBy { get; set; }
        public DateTime? ActionTakenDate { get; set; }
      

    }

    public enum EmployeeCardType
    {
        New_Placement,
        Placement_Change,
        Leave,
        Attendance,
        Delegation,
        Accident,
        Temination,
        Password_Reset,
        EmployeeTransfer,
        Others

    }

}