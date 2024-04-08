using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Archive
{
    public class EmployeeIncomingLetter : TrackUserOperation
    {
        public int EmployeeIncomingLetterId { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_BookNo", ResourceType = typeof(Localization.Resources))]
        public string BookNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterDate", ResourceType = typeof(Localization.Resources))]
        public DateTime LetterDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterDate", ResourceType = typeof(Localization.Resources))]
        public string LetterAmharic { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterReferenceNo", ResourceType = typeof(Localization.Resources))]
        public string LetterReferenceNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedDate", ResourceType = typeof(Localization.Resources))]
        public DateTime RecivedDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedDate", ResourceType = typeof(Localization.Resources))]
        public string RecivedDateAmharic { get; set; }
        public int Sender { get; set; }
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
        //public int Receiver { get; set; }
       
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_VolumeNo", ResourceType = typeof(Localization.Resources))]
        public string VolumeNo { get; set; }
        public int Status { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedTime", ResourceType = typeof(Localization.Resources))]
        public string RecivedTime { get; set; }
        public string RoomNo { get; set; }
        public bool IsTransfered { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}