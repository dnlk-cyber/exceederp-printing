using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Person_Employee : TrackUserOperation
    {
        [Key]
        public int EmployeeId { get; set; }
        [Display(Name = "Common_PersonEmployee_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        [Required]
        // [Index("IMEIIndex",IsUnique = true)]
        public string EmployeeCode { get; set; }
        [Display(Name = "Common_PersonEmployee_Title", ResourceType = typeof(Localization.Resources))]
        public int Title { get; set; }
        [Required]
        [Display(Name = "Common_PersonEmployee_FirstName", ResourceType = typeof(Localization.Resources))]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Common_PersonEmployee_MiddleName", ResourceType = typeof(Localization.Resources))]

        public string MiddleName { get; set; }
        [Display(Name = "Common_PersonEmployee_LastName", ResourceType = typeof(Localization.Resources))]

        [Required]
        public string LastName { get; set; }
        [Display(Name = "Common_PersonEmployee_Nationality", ResourceType = typeof(Localization.Resources))]
        public string Nationality { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_PersonEmployee_DateOfBirth", ResourceType = typeof(Localization.Resources))]

        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Common_PersonEmployee_DateOfBirth", ResourceType = typeof(Localization.Resources))]
        public string DateOfBirthAm { get; set; }
        [Required]
        [Display(Name = "Common_PersonEmployee_Gender", ResourceType = typeof(Localization.Resources))]
        public Gender Gender { get; set; }
        public int CategoryID { get; set; }
        [Display(Name = "Common_PersonEmployee_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }

        [Display(Name = "Common_PersonEmployee_MothersFullName", ResourceType = typeof(Localization.Resources))]

        public string MothersFullName { get; set; }
        [Required]

        [Display(Name = "Common_PersonEmployee_BirthPlace", ResourceType = typeof(Localization.Resources))]
        public string BirthPlace { get; set; }
        [Display(Name = "Common_PersonEmployee_Nation", ResourceType = typeof(Localization.Resources))]
        public string Nation { get; set; }
        [Display(Name = "Common_PersonEmployee_Region", ResourceType = typeof(Localization.Resources))]
        public Region Region { get; set; }
        [Display(Name = "Common_PersonEmployee_Religion", ResourceType = typeof(Localization.Resources))]
        public ReligionList Religion { get; set; }
        [Display(Name = "Common_PersonEmployee_MartialStatus", ResourceType = typeof(Localization.Resources))]

        public MartialStatus MartialStatus { get; set; }
        [Required]
        [Display(Name = "Common_PersonEmployee_Status", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus Status { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_PersonEmployee_HiredDate", ResourceType = typeof(Localization.Resources))]
        public DateTime HiredDate { get; set; }
        [Display(Name = "Common_PersonEmployee_HiredDate", ResourceType = typeof(Localization.Resources))]
        public string HiredDateAmharic { get; set; }
        public virtual ICollection<UserCardItems> UserCardItems { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

        [Required]
        [Display(Name = "Common_PersonEmployee_FirstNameEnglish", ResourceType = typeof(Localization.Resources))]
        public string FirstNameEnglish { get; set; }
        [Required]
        [Display(Name = "Common_PersonEmployee_MiddleNameEnglish", ResourceType = typeof(Localization.Resources))]

        public string MiddleNameEnglish { get; set; }
        [Display(Name = "Common_PersonEmployee_LastNameEnglish", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string LastNameEnglish { get; set; }
        [NotMapped]
        public int ProhabitionState { get; set; }
        [Display(Name = "Common_PersonEmployee_IsInsured", ResourceType = typeof(Localization.Resources))]
        public bool IsInsured { get; set; }
        public List<Education> Education { get; set; }
        public List<Experience> Experience { get; set; }
        public List<Training> Training { get; set; }
        [NotMapped]
        public string FullNameAmharic => string.Concat(FirstName + " " + MiddleName+" "+LastName);
        [NotMapped]
        public string FullNameaEnglish => string.Concat(FirstNameEnglish + " " + MiddleNameEnglish + " " + LastNameEnglish);
        [NotMapped]
        public bool IsProbationEnd { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        
    }
}