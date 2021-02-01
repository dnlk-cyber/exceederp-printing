using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class LetterStatus
    {
        [Key ]
        public int LetterStatusId { get; set; }
        [Required ]
        [Display(Name = "HR_ArchiveManagement_LetterStatus_BookNo", ResourceType = typeof(Localization.Resources))]
        public string BookNo { get; set; }
        //  public virtual IncommingLetterRegistration IncommingLetterRegistration { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterStatus_Decision", ResourceType = typeof(Localization.Resources))]
        public string Decision { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterStatus_Location", ResourceType = typeof(Localization.Resources))]
        public string Location { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_ArchiveManagement_LetterStatus_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterStatus_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
       
    }
}