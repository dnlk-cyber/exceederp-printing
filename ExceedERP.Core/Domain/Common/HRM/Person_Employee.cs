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

        [Required]
        [Display(Name = "Common_PersonEmployee_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }

        [Display(Name = "Common_PersonEmployee_Title", ResourceType = typeof(Localization.Resources))]
        public int Title { get; set; }

        [Required]
        [Display(Name = "Common_PersonEmployee_FirstName", ResourceType = typeof(Localization.Resources))]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Common_PersonEmployee_MiddleName", ResourceType = typeof(Localization.Resources))]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Common_PersonEmployee_LastName", ResourceType = typeof(Localization.Resources))]
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

        [Required]
        [Display(Name = "Common_PersonEmployee_FirstNameEnglish", ResourceType = typeof(Localization.Resources))]
        public string FirstNameEnglish { get; set; }

        [Required]
        [Display(Name = "Common_PersonEmployee_MiddleNameEnglish", ResourceType = typeof(Localization.Resources))]
        public string MiddleNameEnglish { get; set; }

        [Required]
        [Display(Name = "Common_PersonEmployee_LastNameEnglish", ResourceType = typeof(Localization.Resources))]
        public string LastNameEnglish { get; set; }

        [Display(Name = "Common_PersonEmployee_IsInsured", ResourceType = typeof(Localization.Resources))]
        public bool IsInsured { get; set; }

        [Display(Name = "Common_PersonEmployee_IsPlaced", ResourceType = typeof(Localization.Resources))]
        public bool IsPlaced { get; set; }

        public bool IsReHired { get; set; }
        public bool IsBiometricRegistered { get; set; }  // quick check
        public int? BiometricEmployeeId { get; set; }    // API reference

        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }

        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }

        [Display(Name = "Employee Type")]
        public int? EmployeeTypeId { get; set; }

        // Contact Info
        public string PhoneNo { get; set; }
        public string City { get; set; }
        public string SubCity { get; set; }
        public string Wereda { get; set; }

        // Navigation Properties
        public List<Education> Education { get; set; }
        public List<Experience> Experience { get; set; }
        public List<Training> Training { get; set; }
        public virtual ICollection<UserCardItems> UserCardItems { get; set; }

        public int? ProhabitionState { get; set; }

        public bool? IsProbationEnd { get; set; }
        [Display(Name = "Exempt from Attendance")]
        public bool IsAttendanceExempt { get; set; }
        public int? ContratLeftDay { get; set; }
        //public int? Age => (int)((DateTime.Now - DateOfBirth).TotalDays / 365.25);

        public int? Age
        {
            get
            {

                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;

                if (DateOfBirth.Date > today.AddYears(-age)) age--;

                return age;
            }
             set { } // dummy setter so model binders / serializers don’t complain
        }

        // NotMapped properties

        [NotMapped]
        public int? RemainingContractDays { get; set; }
        [Display(Name ="Internal Experience")]
        [NotMapped]
        public string InternalExperience { get; set; }
        [Display(Name = "External Experience")]

        [NotMapped]
        public string ExternalExperience { get; set; }
       
        [NotMapped]
        public string EmployeeAge
        {
            get
            {
                if (DateOfBirth == null)
                    return string.Empty;

                var today = DateTime.Today;
                var ageYears = today.Year - DateOfBirth.Year;
                var ageMonths = today.Month - DateOfBirth.Month;
                var ageDays = today.Day - DateOfBirth.Day;

                // Adjust if birthday hasn’t occurred yet this year
                if (ageDays < 0)
                {
                    ageMonths--;
                    ageDays += DateTime.DaysInMonth(today.Year, (today.Month == 1 ? 12 : today.Month - 1));
                }

                if (ageMonths < 0)
                {
                    ageYears--;
                    ageMonths += 12;
                }

                return $"{ageYears} years, {ageMonths} months, {ageDays} days";
            }
            set { }
        }
        [Display(Name = "Total Experience")]
        [NotMapped]
        public string EmployeeExperience
        {
            get
            {
                if (HiredDate == null)
                    return string.Empty;

                var today = DateTime.Today;
                var ageYears = today.Year - HiredDate.Year;
                var ageMonths = today.Month - HiredDate.Month;
                var ageDays = today.Day - HiredDate.Day;

                // Adjust if birthday hasn’t occurred yet this year
                if (ageDays < 0)
                {
                    ageMonths--;
                    ageDays += DateTime.DaysInMonth(today.Year, (today.Month == 1 ? 12 : today.Month - 1));
                }

                if (ageMonths < 0)
                {
                    ageYears--;
                    ageMonths += 12;
                }

                return $"{ageYears} years, {ageMonths} months, {ageDays} days";
            }
            set { }
        }

        [NotMapped]
        public string FullNameAmharic => $"{FirstName} {MiddleName} {LastName}";

        [NotMapped]
        public string FullNameaEnglish => $"{FirstNameEnglish} {MiddleNameEnglish} {LastNameEnglish}";
       
    }

    public class EmployeeExternalFunding : TrackUserOperation
    {
        [Key]
        public int EmployeeExternalFundingId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_EmployeeAssociation_EmployeeId", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "Funding Source")]
        [Required]
        public string FundingSource { get; set; } // was Donor  
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; } 
        [Display(Name = "OutCome")]
        public decimal OutCome { get; set; } 
        [Display(Name = "Level of Effort (%)")]
        public decimal LevelOfEffortPercentage { get; set; } // was LoE
    }
    public class LegacyEmployee: TrackUserOperation
    {
        // ========================
        // Given Fields (Exact Match)
        // ========================
        [Key]
        public int LegacyEmployeeId { get; set; }
        [Display(Name = "ID No.")]
        public string EmployeeCode { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Gender")]
        public string Sex { get; set; }

        [Display(Name = "Age")]
        public int? Age { get; set; }

        [Display(Name = "Department")]
        public string Dept { get; set; }

        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Display(Name = "Grade / Level")]
        public string Grade { get; set; }

        [Display(Name = "Salary")]
        public decimal PresentSalary { get; set; }

        [Display(Name = "Education")]
        public string Education { get; set; }

        [Display(Name = "Field of Study")]
        public string FieldOfStudy { get; set; }

        [Display(Name = "Internal Experience (Years)")]
        public decimal WithInExperience { get; set; }

        [Display(Name = "External Experience (Years)")]
        public decimal OutExperience { get; set; }

        [Display(Name = "Total Experience (Years)")]
        public decimal TotalExperience { get; set; }

        [Display(Name = "Leave Balance (Days)")]
        public decimal LeaveBalance { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Recruitment Date")]
        public DateTime RecruitmentDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Termination Date")]
        public DateTime? DateofTermination { get; set; }

        [Display(Name = "Reason for Termination")]
        public string ReasonforTermination { get; set; }

        [Display(Name = "Branch")]
        public string Branch { get; set; }

        [Display(Name = "Sub-Branch")]
        public string SubBranch { get; set; }

        [Display(Name = "Processed")]
        public bool IsProcessed { get; set; } // Indicates if moved to real Employee
        // ========================
        // Helper Properties (for mapping)
        // ========================

        // Split FullName → Your system requires First/Middle/Last
        public string FirstName => FullName?.Split(' ').Length > 0 ? FullName.Split(' ')[0] : "";

        public string MiddleName => FullName?.Split(' ').Length > 1 ? FullName.Split(' ')[1] : "";

        public string LastName => FullName?.Split(' ').Length > 2 ? FullName.Split(' ')[2] : "";

        // Convert Sex → Your enum
        public Gender Gender =>
            Sex?.ToLower() == "male" ? Gender.MALE :
            Sex?.ToLower() == "female" ? Gender.FEMALE :
            Gender.MALE; // default fallback
    }

}