using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Archive
{
    public class InstitutionOutgoingLetter
    {
        [Key]
        public int InstitutionOutgoingLetterId { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_BookNo", ResourceType = typeof(Localization.Resources))]
        public string BookNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_LetterReferenceNo", ResourceType = typeof(Localization.Resources))]
        public string LetterReferenceNo { get; set; }        
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_OwnerName", ResourceType = typeof(Localization.Resources))]
        public string OwnerName { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_InstitutionTypeId", ResourceType = typeof(Localization.Resources))]
        public int InstitutionTypeId { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Subject", ResourceType = typeof(Localization.Resources))]
        public string Subject { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Shelve", ResourceType = typeof(Localization.Resources))]
        public string Shelve { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_Row", ResourceType = typeof(Localization.Resources))]
        public string Row { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_FolderNo", ResourceType = typeof(Localization.Resources))]
        public string FolderNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_FileNo", ResourceType = typeof(Localization.Resources))]
        public string FileNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_NoOfAttachment", ResourceType = typeof(Localization.Resources))]
        public int NoOfAttachment { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_SerialNo", ResourceType = typeof(Localization.Resources))]
        public string SerialNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewOutgoingLetter_RefernceType", ResourceType = typeof(Localization.Resources))]
        public string RefernceType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]

        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingLetterDate", ResourceType = typeof(Localization.Resources))]
        public DateTime OutgoingLetterDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingLetterDate", ResourceType = typeof(Localization.Resources))]
        public string OutgoingLetterDateAm { get; set; }   
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedTime", ResourceType = typeof(Localization.Resources))]
        public string OutgoingTime { get; set; }
        [DisplayName("Subject Type")]
        public int SubjectTypeId { get; set; }
        public int? RefersTo { get; set; }
    }
}