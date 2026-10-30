using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class DisciplinaryCase: DepartmentNewOperations
    {
        [Key]
        public int DisciplinaryCaseId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_DisciplinaryCase_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "HR_Discipline_DisciplinaryCase_TransgressionDate", ResourceType = typeof(Localization.Resources))]
        public DateTime TransgressionDate { get; set; }
        [Display(Name = "HR_Discipline_DisciplinaryCase_TransgressionDate", ResourceType = typeof(Localization.Resources))]
        public string TransgressionDateAm { get; set; }
        [Required]
       
        [Display(Name = "HR_Discipline_DisciplinaryCase_NatureofTransgression", ResourceType = typeof(Localization.Resources))]
        public string NatureofTransgression { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_DisciplinaryCase_Evidence", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.MultilineText)]
        public string Evidence { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public bool IsActive { get; set; }
     
    }
}