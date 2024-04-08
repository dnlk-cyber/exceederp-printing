using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class LetterSender
    {
        [Key]
        public int LetterSenderId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_ArchiveManagement_LetterSender_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
       
        public string Time { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Receiver", ResourceType = typeof(Localization.Resources))]
        public int Receiver { get; set; }
        [NotMapped]
        public int[] ReceiverSelected { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Description", ResourceType = typeof(Localization.Resources))]
        public string Description { get; set; }
        [Display(Name = "HR_ArchiveManagement_LetterSender_Status", ResourceType = typeof(Localization.Resources))]
        public LettersStatus Status { get; set; }
         [Display(Name = "HR_ArchiveManagement_LetterSender_SenderDepartment", ResourceType = typeof(Localization.Resources))]
        public int SenderDepartment { get; set; }
        public int InstitutionIncomingLetterId { get; set; }
      
        [Display(Name = "HR_ArchiveManagement_LetterSender_StatusName", ResourceType = typeof(Localization.Resources))]
        public int StatusName { get; set; }
        [NotMapped ]
        public int CurrentStatus { get; set; }
        public bool IsTransfered { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }


    }
}