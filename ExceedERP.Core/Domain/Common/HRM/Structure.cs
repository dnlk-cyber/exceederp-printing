using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Structure : TrackUserOperation 
    {

        [Key]
        public int StructureId { get; set; }
        [Required]
        [Display(Name = "Common_Company_Structure_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


    }
}