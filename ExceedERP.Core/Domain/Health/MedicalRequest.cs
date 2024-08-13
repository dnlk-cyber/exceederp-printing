using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Health
{
    public class MedicalRequest : DepartmentOperations
    {
        [Key]
        public int MedicalRequestId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Display(Name = "HR_SelfCare_MedicalRequest_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_MedicalRequest_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_SelfCare_MedicalRequest_Date", ResourceType = typeof(Localization.Resources))]
        public string DateAmharic { get; set; }
        [Required ]
        [Display(Name = "HR_SelfCare_MedicalRequest_DeptId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public virtual MainCompanyStructure MainCompanyStructure { get; set; }

        //  [Required]
        // [Display(Name = "Transfer Time")]
        // public DateTime ReferredTime { get; set; }

        // [DataType(DataType.Time)]
        //  [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        //// [Required]
        // [Display(Name = "Returned Time")]
        // public DateTime Time { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }

    }
    public class MedicalAttachment
    {
        public int MedicalAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int MedicalRequestId { get; set; }
        public virtual MedicalRequest Medical { get; set; }

    }
}