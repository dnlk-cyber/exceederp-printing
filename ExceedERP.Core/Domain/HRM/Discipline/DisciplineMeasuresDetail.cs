using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Discipline
{
    public class DisciplineMeasuresDetail
    {
        [Key]
        public int DetailMeasuresId { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_DisciplineMeasuresDetail_MeasuresId", ResourceType = typeof(Localization.Resources))]
        
        public int? MeasuresId { get; set; }
        //public virtual DisciplineMeasures DisciplineMeasures { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_DisciplineMeasuresDetail_Level", ResourceType = typeof(Localization.Resources))]
        public string Level { get; set; }
        [Required]
        [Display(Name = "HR_Discipline_DisciplineMeasuresDetail_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}