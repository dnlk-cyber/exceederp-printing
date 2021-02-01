using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Configuration : TrackUserSettingOperation
    {
        public int ConfigurationId { get; set; }
          [Display(Name = "Common_Configuration_Category", ResourceType = typeof(Localization.Resources))]
        public string Category { get; set; }
        [Required]
        [Display(Name = "Common_Configuration_Reference", ResourceType = typeof(Localization.Resources))]
        public string Reference { get; set; }
        [Required]
        [Display(Name = "Common_Configuration_Attribute", ResourceType = typeof(Localization.Resources))]
        public string Attribute { get; set; }
        [Required]
        [Display(Name = "Common_Configuration_CurrentValue", ResourceType = typeof(Localization.Resources))]
        public string CurrentValue { get; set; }

        public string PreviousValue { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}