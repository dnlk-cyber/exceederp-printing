using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.Templates
{
    public class Template : TrackUserOperation
    {
        public int TemplateId { get; set; }
        [Display(Name = "PerformanceCriteriaActionType", ResourceType = typeof(Localization.Resources))]

        public string TempOne { get; set; }
        [Display(Name = "HR_Company_Announcment_Title", ResourceType = typeof(Localization.Resources))]
        public string Title { get; set; }
        [Display(Name = "Enum_Template_Type", ResourceType = typeof(Localization.Resources))]
        public int? TemplateTypeSettingId { get; set; }
        [Display(Name = "Enum_Template_Type", ResourceType = typeof(Localization.Resources))]
        [NotMapped]
        public string TemplateTypes { get; set; }
        [Display(Name = "Enum_Template_Type", ResourceType = typeof(Localization.Resources))]
        public TemplateTypes TemplateType { get; set; }
        [Display(Name = "PerformanceCriteriaActionType", ResourceType = typeof(Localization.Resources))]
        public string Document { get; set; }
    }

    public class TemplateTypeSetting: TrackUserOperation 
    {
        public int TemplateTypeSettingId { get; set; }
        [Display(Name = "Enum_Template_Type", ResourceType = typeof(Localization.Resources))]
        public string TemplateType { get; set; }
    }

    public enum RecordKeyType
    {
        ApplicantName,
        HimHer,
        HisHer,
        Experiences,
        IsTerminated,
        IfTerminated,
        BirthDate,
        HireDate,
        HiredDate,
        EducationInfo,
        FromWorkUnit,
        Department,
        JobTitle,
        Rank,
        FromBranch,
        Branch,
        FromStep,
        Step,
        WorkUnit,
        BusinessUnit,
        FromBusinessUnit,
        FromSalary,
        Salary,
        PreviousDepartement,
        CurrentDepartement,
        PreviousJobTitle,
        CurrentJobTitle,
        JobTitleEnglish,
        FromJobTitle,
        CurrentRank,
        CurrentSalary,
        AssignmentDate,
        Diffrence,
        DiffrenceAmount,
        CurrentAmount,
        ApplicationDate,
        ToName,
        FromDate,
        ToDate,
        // Newly added
        EmploymentType,
        EmployeeAllowance,
        Allowance,
        PersonalTitle,
        DisciplineTaken,
        TerminationReason,
        EmployeeCode,
        TerminationApplicationDate,
        MarriageStatus,
        Age,
        Sex,
        HouseNo,
        TelephoneNo,
        Region,
        DistrictWoreda,
        KebeleSubDistrict,
        POBoxNo,
        BirthPlace,
        EmergencyContactPerson,
        LanguageProficiency


    }

}
