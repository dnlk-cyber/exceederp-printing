using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class IncomingLetterRegistration:TrackUserOperation 
    {
        [Key]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_BookNo", ResourceType = typeof(Localization.Resources))]
        public string BookNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterDate", ResourceType = typeof(Localization.Resources))]
        public DateTime LetterDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterDate", ResourceType = typeof(Localization.Resources))]
        public string LetterAmharic{ get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_LetterReferenceNo", ResourceType = typeof(Localization.Resources))]
        [Required ]
        public string LetterReferenceNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedDate", ResourceType = typeof(Localization.Resources))]
        public DateTime RecivedDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedDate", ResourceType = typeof(Localization.Resources))]
        public string RecivedDateAmharic { get; set; }
       
        [Required ]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OwnerName", ResourceType = typeof(Localization.Resources))]
        public string OwnerName { get; set; }
        [Required ]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_InstitutionTypeId", ResourceType = typeof(Localization.Resources))]
        public int InstitutionTypeId { get; set; }
        public virtual InstitutionType InstitutionType { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Subject", ResourceType = typeof(Localization.Resources))]
        public string Subject { get; set; }
        [Required ]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Shelve", ResourceType = typeof(Localization.Resources))]
        public string Shelve { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Row", ResourceType = typeof(Localization.Resources))]
        public string Row { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_FolderNo", ResourceType = typeof(Localization.Resources))]
        public string FolderNo { get; set; }
        [Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_FileNo", ResourceType = typeof(Localization.Resources))]
        public string FileNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_NoOfAttachment", ResourceType = typeof(Localization.Resources))]
        public int NoOfAttachment { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_SerialNo", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure {get;set;}
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RefernceType", ResourceType = typeof(Localization.Resources))]
        public string RefernceType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]

        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingLetterDate", ResourceType = typeof(Localization.Resources))]
        public DateTime? OutgoingLetterDate { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingLetterDate", ResourceType = typeof(Localization.Resources))]
        public string OutgoingLetterDateAm { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingLetterReferenceNo", ResourceType = typeof(Localization.Resources))]
       
        public string OutgoingLetterReferenceNo { get; set; }


        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingOwnerName", ResourceType = typeof(Localization.Resources))]
        public string OutgoingOwnerName { get; set; }

        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingInstitutionTypeId", ResourceType = typeof(Localization.Resources))]
        public int OutgoingInstitutionTypeId { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingSubject", ResourceType = typeof(Localization.Resources))]
       
        public string OutgoingSubject { get; set; }

        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Shelve", ResourceType = typeof(Localization.Resources))]
        public string OutgoingShelve { get; set; }

        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_Row", ResourceType = typeof(Localization.Resources))]
        public string OutgoingRow { get; set; }

        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingFolderNo", ResourceType = typeof(Localization.Resources))]
        public string OutgoingFolderNo { get; set; }

        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingFileNo", ResourceType = typeof(Localization.Resources))]
        public string OutgoingFileNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingNoOfAttachment", ResourceType = typeof(Localization.Resources))]
        public int OutgoingNoOfAttachment { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingSerialNo", ResourceType = typeof(Localization.Resources))]
        public string OutgoingSerialNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_OutgoingRefernceType", ResourceType = typeof(Localization.Resources))]
        public string OutgoingRefernceType { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]

        public string OutgoingRemark { get; set; }
        public virtual IQueryable<LettersStatus> LettersStatus { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_VolumeNo", ResourceType = typeof(Localization.Resources))]
        public string VolumeNo { get; set; }
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_VolumeNo", ResourceType = typeof(Localization.Resources))]
        public string OutgoingVolumeNo { get; set; }
        public bool EnableDisbale { get; set; }
        public int Status { get; set; }
        //[DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        //[Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedTime", ResourceType = typeof(Localization.Resources))]
        public string OutgoingTime { get; set; }
        //[DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        //[Required]
        [Display(Name = "HR_ArchiveManagement_NewIncommingLetterRegistration_RecivedTime", ResourceType = typeof(Localization.Resources))]
        public string RecivedTime { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionType_InstitutionName", ResourceType = typeof(Localization.Resources))]
        public string InstitutionName { get; set; }
        [Display(Name = "HR_ArchiveManagement_InstitutionType_InstitutionName", ResourceType = typeof(Localization.Resources))]
        public string OutgoingInstitutionName { get; set; }
    }
}