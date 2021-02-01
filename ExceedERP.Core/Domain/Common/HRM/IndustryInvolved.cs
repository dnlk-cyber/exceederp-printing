using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class IndustryInvolved:TrackUserOperation 
    {
        public int IndustryInvolvedID { get; set; }

        public double Index { get; set; }

        public string Reference { get; set; }     

        public string ReferenceType { get; set; }
        [Display(Name = "Common_IndustryInvolved_Industry", ResourceType = typeof(Localization.Resources))]
        public IndustryType Industry { get; set; }
        [Display(Name = "Common_Identification_Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

    }
}