using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class EmployeeLetterSender
    {
        [Key]
        public int LetterSenderId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_ArchiveManagement_LetterSender_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_LetterSender_Receiver", ResourceType = typeof(Localization.Resources))]
        public string Receiver { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_LetterSender_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Status", ResourceType = typeof(Localization.Resources))]
        public LettersStatus Status { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Sender", ResourceType = typeof(Localization.Resources))]
        public string Sender { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_BookNo", ResourceType = typeof(Localization.Resources))]
        public string BookNo { get; set; }
        public virtual EmployeeIncomingLetter IncomingLetterRegistration { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_OwnerName", ResourceType = typeof(Localization.Resources))]
        public string OwnerName { get; set; }
        // public string Url { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_FileName", ResourceType = typeof(Localization.Resources))]
        public string FileName { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_StatusName", ResourceType = typeof(Localization.Resources))]
        public int StatusName { get; set; }
        [NotMapped ]
        public int CurrentStatus { get; set; }
        public byte[] File { get; set; }
        public string FileType { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_SenderDepartment", ResourceType = typeof(Localization.Resources))]
        public int SenderDepartment { get; set; }
        public string TransferDescription { get; set; }
    }
}