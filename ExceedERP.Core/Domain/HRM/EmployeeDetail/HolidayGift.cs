using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Company;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>    HolidayGiftEmployee . </summary>
    ///
    /// <remarks>   Kibrom Abera, 04/06/2026. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class HolidayGiftEmployee : DepartmentNewOperations
    {
        [Key]
        public int HolidayGiftEmployeeId { get; set; }
        [Display(Name = "P.R")]
        public int? PaymentRequestId { get; set; }
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "Holiday-Name")]
        public EthiopianHoliday EthiopianHoliday { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int? BranchId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int? LookupId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? PreparedTime { get; set; }
        public int? PreparedById { get; set; }
        public string CheckedBy { get; set; }
        public DateTime? CheckedTime { get; set; }
        public int? CheckedById { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HolidayGiftAmount { get; set; }
        public bool IsActive { get; set; }
        public int HolidayGiftRateSettingId { get; set; }
        public virtual HolidayGiftRateSetting HolidayGift { get; set; }
    }


    public enum EthiopianHoliday
    {
        [Display(Name = "Ethiopian New Year (Enkutatash)")]
        NewYear = 1,

        [Display(Name = "Meskel (Finding of the True Cross)")]
        Meskel = 2,

        [Display(Name = "Adwa Victory Day")]
        AdwaVictoryDay = 3,

        [Display(Name = "Labour Day")]
        LabourDay = 4,

        [Display(Name = "Patriots Victory Day")]
        PatriotsVictoryDay = 5,

        [Display(Name = "Derg Downfall Day")]
        DergDownfallDay = 6,

        [Display(Name = "Christmas (Genna)")]
        Christmas = 7,

        [Display(Name = "Epiphany (Timket)")]
        Epiphany = 8,

        [Display(Name = "Good Friday")]
        GoodFriday = 9,

        [Display(Name = "Easter (Fasika)")]
        Easter = 10,

        [Display(Name = "Eid al-Fitr")]
        EidAlFitr = 11,

        [Display(Name = "Eid al-Adha")]
        EidAlAdha = 12,

        [Display(Name = "Mawlid (Prophet's Birthday)")]
        Mawlid = 13
    }
}