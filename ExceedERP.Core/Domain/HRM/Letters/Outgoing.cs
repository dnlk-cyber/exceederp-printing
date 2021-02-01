using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.Letters
{
    public class Outgoing:TrackUserOperation 
    {
        [Key]
        public int OutgoingId { get; set; }
        [Required]
        [Display(Name = "HR_Letters_Outgoing_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
      
        [Required]
        [Display(Name = "HR_Letters_Outgoing_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_Letters_Outgoing_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Display(Name = "HR_Letters_Outgoing_FolderNo", ResourceType = typeof(Localization.Resources))]
        public string FolderNo { get; set; }
        [Display(Name = "HR_Letters_Outgoing_CardSerialNo", ResourceType = typeof(Localization.Resources))]
        public string CardSerialNo { get; set; }
        [Required]
        [Display(Name = "HR_Letters_Outgoing_Owner", ResourceType = typeof(Localization.Resources))]
        public LetterOwner  Owner { get; set; }
        
    }
}