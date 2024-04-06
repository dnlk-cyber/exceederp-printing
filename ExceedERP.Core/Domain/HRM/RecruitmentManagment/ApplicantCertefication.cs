using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.RecruitmentManagment
{
    public class ApplicantCertefication
    {
        public int ApplicantCerteficationId { get; set; }
        public int ApplicantDetailId { get; set; }
        public int ApplicantRegistrationId { get; set; }
        [Display(Name = "CerteficateName", ResourceType = typeof(Localization.Resources))]
        public string CerteficationName { get; set; }
        [Display(Name = "CerteficateType", ResourceType = typeof(Localization.Resources))]
        public string CerteficationType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}
