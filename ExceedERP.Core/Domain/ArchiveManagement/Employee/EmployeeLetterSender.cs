using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Archive
{
    public class EmployeeLetterSender
    {
        [Key]
        public int LetterSenderId { get; set; }
        public int EmployeeIncomingLetterId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_ArchiveManagement_LetterSender_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        public string Time { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Receiver", ResourceType = typeof(Localization.Resources))]
        public int Receiver { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Status", ResourceType = typeof(Localization.Resources))]
        public LettersStatus Status { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_StatusName", ResourceType = typeof(Localization.Resources))]
        public int StatusName { get; set; }
        public bool IsTransfered { get; set; }
    }
}