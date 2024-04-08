using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.ArchiveManagement
{
    public class InstitutionIncomingLetter : TrackUserOperation
    {
        public int InstitutionIncomingLetterId { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_BookNo", ResourceType = typeof(Localization.Resources))]
        public string BookNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterDate", ResourceType = typeof(Localization.Resources))]
        public DateTime LetterDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterDate", ResourceType = typeof(Localization.Resources))]
        public string LetterDateAmh { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterReferenceNo", ResourceType = typeof(Localization.Resources))]
        public string LetterReferenceNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedDate", ResourceType = typeof(Localization.Resources))]
        public DateTime RecivedDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedDate", ResourceType = typeof(Localization.Resources))]
        public string RecivedDateAmh { get; set; }
        [Display(Name = nameof(Localization.Resources.Archive_SenderName), ResourceType = typeof(Localization.Resources))]
        public string SenderName { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_InstitutionTypeId", ResourceType = typeof(Localization.Resources))]
        public int InstitutionTypeId { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Subject", ResourceType = typeof(Localization.Resources))]
        public string Subject { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Shelve", ResourceType = typeof(Localization.Resources))]
        public string Shelve { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Row", ResourceType = typeof(Localization.Resources))]
        public string Row { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_FolderNo", ResourceType = typeof(Localization.Resources))]
        public string FolderNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_FileNo", ResourceType = typeof(Localization.Resources))]
        public string FileNo { get; set; }
        //[Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_SerialNo", ResourceType = typeof(Localization.Resources))]
        //public int OrgStructureId { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_VolumeNo", ResourceType = typeof(Localization.Resources))]
        public string VolumeNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedTime", ResourceType = typeof(Localization.Resources))]
        public string RecivedTime { get; set; }
        [DisplayName("Subject Type")]
        public int SubjectTypeId { get; set; }
        /// to hide and show edit and delete buttons // when true hide the buttons and viceversa
        public bool IsTransfered { get; set; }
    }
}
