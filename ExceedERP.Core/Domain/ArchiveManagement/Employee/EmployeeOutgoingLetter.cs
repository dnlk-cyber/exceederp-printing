using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class EmployeeOutgoingLetter
    {
        public int EmployeeOutgoingLetterId { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_LetterReferenceNo", ResourceType = typeof(Localization.Resources))]
        public string LetterReferenceNo { get; set; }
        // sender is always department
        [DisplayName("Sender")]
        public int SenderOrgId { get; set; }
        // receiver is always employee
        [DisplayName("Receiver")]
        public int ReceiverEmpId { get; set; }
       
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Subject", ResourceType = typeof(Localization.Resources))]
        public string Subject { get; set; }
        public string RoomNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Shelve", ResourceType = typeof(Localization.Resources))]
        public string Shelve { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Row", ResourceType = typeof(Localization.Resources))]
        public string Row { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_FolderNo", ResourceType = typeof(Localization.Resources))]
        public string FolderNo { get; set; }
        public string Volume { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_FileNo", ResourceType = typeof(Localization.Resources))]
        public string FileNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingLetterDate", ResourceType = typeof(Localization.Resources))]
        public DateTime OutgoingLetterDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingLetterDate", ResourceType = typeof(Localization.Resources))]
        public string OutgoingLetterDateAm { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedTime", ResourceType = typeof(Localization.Resources))]
        public string Time { get; set; }
        public bool IsTransfered { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}