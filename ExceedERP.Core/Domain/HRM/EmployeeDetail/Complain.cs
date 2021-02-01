using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class Complain
    {
        [Key ]
        public int ComplainId{get;set;}
        [Display(Name = "HR_EmployeeDetail_Complain_LookupId", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_EmployeeDetail_Complain_Date", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set;}
        [Display(Name = "HR_EmployeeDetail_Complain_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Required ]
        [Display(Name = "HR_EmployeeDetail_Complain_ComplainDescription", ResourceType = typeof(Localization.Resources))]

        public string ComplainDescription { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Complain_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Person_Employee { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public int RefersTo { get; set; }
        //public int CC { get; set; }
        public int SendTo { get; set; }
    }
}