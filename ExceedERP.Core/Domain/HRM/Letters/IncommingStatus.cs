using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.Letters
{
    public class IncommingStatus
    {
        [Key]
        public int IncommingStatusId { get; set; }
        [Required ]
        //[Display(Name = "Incomming", ResourceType = typeof(Localization.Resources))]
        public int IncommingId { get; set; }
        public virtual Incomming Incomming { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_Letters_IncommingStatus_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_Letters_IncommingStatus_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        
        [Display(Name = "HR_Letters_IncommingStatus_Decsion", ResourceType = typeof(Localization.Resources))]
        public string Decsion { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}