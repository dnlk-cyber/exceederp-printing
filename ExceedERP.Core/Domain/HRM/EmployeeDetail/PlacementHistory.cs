using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>   A placement detail. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class PlacementHistory:TrackUserOperation 
    {
        [Key]
        public int PlacementHistoryId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }      
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }     
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_From", ResourceType = typeof(Localization.Resources))]
        public DateTime  From { get; set; }
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_From", ResourceType = typeof(Localization.Resources))]       
        public string FromAmharic { get; set; }       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_To", ResourceType = typeof(Localization.Resources))]       
        public string ToAmharic { get; set; }     
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_JobTitle", ResourceType = typeof(Localization.Resources))]
        public string JobTitle { get; set; }     
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_Rank", ResourceType = typeof(Localization.Resources))]
        public string Rank { get; set; }        
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_Step", ResourceType = typeof(Localization.Resources))]
        public string Step { get; set; }       
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal  Salary { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Common_PersonEmployee_Status", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus EmployementType { get; set; }
        [Display(Name = "HR_EmployeeDetail_PlacementHistory_JobTitle", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
    }
}