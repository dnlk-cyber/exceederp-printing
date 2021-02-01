using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class EmpDisciplinaryHearing
    {
        [Key]
        public int DisciplinaryHearingId { get; set; }


        [Display(Name = "HR_Discipline_EmpDisciplinaryHearing_DisciplinaryHearingId", ResourceType = typeof(Localization.Resources))]
        public int DisciplinaryCaseId { get; set; }
        public virtual DisciplinaryCase DisciplinaryCase { get; set; }
        [Display(Name = "HR_Discipline_EmpDisciplinaryHearing_HearingDate", ResourceType = typeof(Localization.Resources))]

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HearingDate { get; set; }
        [Display(Name = "HR_Discipline_EmpDisciplinaryHearing_HearingDate", ResourceType = typeof(Localization.Resources))]
        public string HearingDateAmharic { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_EmpDisciplinaryHearing_HearingOutcome", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.MultilineText)]
        public string HearingOutcome { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

        //Hearing Date


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