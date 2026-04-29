using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class Contract:TrackUserOperation 
    {
        public int ContractId { get; set; }
        [Required ]
        [Display(Name = "HR_EmployeeDetail_Contract_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_Contract_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_EmployeeDetail_Contract_From", ResourceType = typeof(Localization.Resources))]
        public string FromContAm { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_Contract_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_EmployeeDetail_Contract_To", ResourceType = typeof(Localization.Resources))]
        public string ToContAm { get; set; }
        [Display(Name = "HR_EmployeeDetail_Contract_MonthlySalary", ResourceType = typeof(Localization.Resources))]
        public decimal MonthlySalary { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
		
    }

    public class ContactVM
    {
        public int EmployeeId { get; set; }
        public DateTime AssignmentDate { get; set; }
    }
}