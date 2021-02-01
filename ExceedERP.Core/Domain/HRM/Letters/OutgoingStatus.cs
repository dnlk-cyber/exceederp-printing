using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Letters
{
    public class OutgoingStatus
    {
        [Key]
        public int StatusId { get; set; }
        [Required ]
        [Display(Name = "HR_Letters_OutgoingStatus_OutgoingId", ResourceType = typeof(Localization.Resources))]
        public int OutgoingId { get; set; }
        public virtual Outgoing Outgoing { get; set; }
        [Display(Name = "HR_Letters_OutgoingStatus_NoOfAttachment", ResourceType = typeof(Localization.Resources))]
        public int NoOfAttachment { get; set; }
        [Display(Name = "HR_Letters_OutgoingStatus_Subject", ResourceType = typeof(Localization.Resources))]
        public string Subject { get; set; }
        [Display(Name = "HR_Letters_OutgoingStatus_Destination", ResourceType = typeof(Localization.Resources))]
        public string Destination { get; set; }
    }
}