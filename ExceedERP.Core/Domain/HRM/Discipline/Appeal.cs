using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class Appeal
    {
        [Key]
        public int AppealId { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_Appeal_AppealDetail", ResourceType = typeof(Localization.Resources))]
        public string AppealDetail { get; set; }
        [Display(Name = "HR_Discipline_Appeal_Date", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_DisciplinaryCaseId", ResourceType = typeof(Localization.Resources))]

        public int DisciplinaryCaseId { get; set; }
        public virtual DisciplinaryCase DisciplinaryCase { get; set; }
        [Display(Name = "HR_Discipline_Appeal_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get;set; }
    }
}