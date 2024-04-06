using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Templates
{
    public class Template
    {
        public int TemplateId { get; set; }
        [Display(Name = "PerformanceCriteriaActionType", ResourceType = typeof(Localization.Resources))]

        public string TempOne { get; set; }
        [Display(Name = "Enum_Template_Type", ResourceType = typeof(Localization.Resources))]

        public TemplateTypes TemplateType { get; set; }
        [Display(Name = "PerformanceCriteriaActionType", ResourceType = typeof(Localization.Resources))]

        public string Document { get; set; }
    }
}
