using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class DisciplineMeasures
    {
        [Key]
        public int MeasuresId { get; set; }
        [Required ]
        [Display(Name = "HR_Discipline_DisciplineMeasures_MeasuresType", ResourceType = typeof(Localization.Resources))]
      
        public string MeasuresType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}