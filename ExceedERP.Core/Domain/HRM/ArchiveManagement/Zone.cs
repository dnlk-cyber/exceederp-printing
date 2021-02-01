using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class Zone
    {
        [Key]
        public int ZoneId { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_Zone_ZoneName", ResourceType = typeof(Localization.Resources))]
        public string ZoneName { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}