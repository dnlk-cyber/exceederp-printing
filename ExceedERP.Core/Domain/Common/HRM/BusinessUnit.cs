using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class BusinessUnit : TrackUserOperation 
    {
        public int BusinessUnitId { get; set; }
        [Required]
        [Display(Name = "Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "AccountCode", ResourceType = typeof(Localization.Resources))]
        public string Accountcode { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}