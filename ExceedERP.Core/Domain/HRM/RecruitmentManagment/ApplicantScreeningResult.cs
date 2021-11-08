using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantScreenResult
    {
        [Key]
        public int ApplicantScreenResultId { get; set; }
        public int VacancyId { get; set; }
        [NotMapped]
        public string JobTitle { get; set; }
        [NotMapped]
        public string Location { get; set; }
        public string Remark { get; set; }
    }
}