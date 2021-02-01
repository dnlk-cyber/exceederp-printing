using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class EmpDisciplineMeasure
    {
        [Key]
        public int EmpDisciplineMeasureId { get; set; }
        //[Required]
        //[Display(Name = "Employee")]
        //public int EmployeeId { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_DisciplinaryCaseId", ResourceType = typeof(Localization.Resources))]

        public int DisciplinaryCaseId { get; set; }
        public virtual DisciplinaryCase DisciplinaryCase { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_MeasuresId", ResourceType = typeof(Localization.Resources))]

        public int MeasuresId { get; set; }
        // public virtual DisciplineMeasures DisciplineMeasures { get; set; } 
        //[Required]
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_DetailMeasuresId", ResourceType = typeof(Localization.Resources))]

        public int? DetailMeasuresId { get; set; }
        public virtual DisciplineMeasuresDetail DisciplineMea { get; set; }
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_DecisionDate", ResourceType = typeof(Localization.Resources))]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DecisionDate { get; set; }
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_DecisionDate", ResourceType = typeof(Localization.Resources))]
        public string DecisionDateAm { get; set; }
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_IsExpired", ResourceType = typeof(Localization.Resources))]

        public bool IsExpired { get; set; }
        [Display(Name = "HR_Discipline_EmpDisciplineMeasure_IsReversedByAppeal", ResourceType = typeof(Localization.Resources))]

        public bool IsReversedByAppeal { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        public int Month { get; set; }

        //[Display(Name = "Branch Name")]

        //public string OrgBranchName { get; set; }

        //public string Status { get; set; }
        //public bool isSendForApproval { get; set; }
        //public bool isOnlineApproved { get; set; }
        //public string OnlineApprovedBy { get; set; }
        //public DateTime? OnlineApprovedTime { get; set; }
        //public bool isVoid { get; set; }
        //public string VoidBy { get; set; }
        //public DateTime? VoidTime { get; set; }
    }
}