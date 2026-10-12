using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM
{
    public enum LeaveEntitleStarting
    {
        Fiscal_Year_Initial_Date,
        Fiscal_Year_End_Date
    }
    public enum PerformanceSelfEvaluation
    {
        Enable_Self_Evaluation
    }
    public enum HRMSettingCatagories
    {
        EmployeeID,
        Leave,
        Probation_Setting,
        Self_Evaluation
    }
    //////////////////

    public enum ProbationSetting
    {
        Probation_Period
    }

    public enum EmployeeIDAbbreviationType
    {
        Company,
        Year,
        Sequential_Number
    }

    public enum LeaveHrmSetting
    {
        Frequency,
        [Display(Name = "Initial Year/Gerigorian/")]
        InitialYear,
        Maternitiy,
        Paternity
    }
    public enum EmployeeIdAttribute
    {
        Auto_Generate,
        Manual
    }

    public enum ProbationMeasurement
    {
        Very_Good,
        Good,
        Enough,
        Low
    }

    public enum LeaveActionTypes
    {
        Without_Plan,
        With_Plan,
        Forced
    }
    public enum BiometricEmployeeAutoRegistration
    {
        BiometricEmployeeAutoRegistration,
        Manual
    }

    public enum BiometricEmployeeAutoExempt
    {
        BiometricEmployeeAutoExempt,
        BiometricEmployeeAutoNotExempt
    }
    public enum TemplateTypes
    {
        [Display(Name = "Enum_Template_Type_Experiance_Amharic", ResourceType = typeof(Localization.Resources))]
        ExperianceAm = 1,
        [Display(Name = "Enum_Template_Type_Experiance_English", ResourceType = typeof(Localization.Resources))]
        ExperianceEng = 2,
        [Display(Name = "Enum_Template_Type_Warranty_Amharic", ResourceType = typeof(Localization.Resources))]
        WarrantyAm = 3,
        [Display(Name = "Enum_Template_Type_Termination_Acceptance_Amharic", ResourceType = typeof(Localization.Resources))]
        TerminationAcceptanceAm = 4,
        [Display(Name = "Enum_Template_Type_Recruitment_External_Letter_Amharic", ResourceType = typeof(Localization.Resources))]
        RecruitmentExternalLetterAm = 5,
        [Display(Name = "Enum_Template_Type_Promotion_Amharic", ResourceType = typeof(Localization.Resources))]
        PromotionAm = 6,
        [Display(Name = "Enum_Template_Type_Promotion_English", ResourceType = typeof(Localization.Resources))]
        PromotionEnglish = 7,
        [Display(Name = "Enum_Template_Type_Transfer_Amharic", ResourceType = typeof(Localization.Resources))]
        TransferAm,
        [Display(Name = "Enum_Template_Type_Transfer_English", ResourceType = typeof(Localization.Resources))]
        TransferEng,
        [Display(Name = "Enum_Template_Type_Salary_Incremen_Amharic", ResourceType = typeof(Localization.Resources))]
        SalaryIncrementAm,
        [Display(Name = "Enum_Template_Type_Salary_Incremen_English", ResourceType = typeof(Localization.Resources))]
        SalaryIncrementEng,
        [Display(Name = "Enum_Template_Type_Title_Change_Amharic", ResourceType = typeof(Localization.Resources))]
        TitleChangeAm,
        [Display(Name = "Enum_Template_Type_Title_Change_English", ResourceType = typeof(Localization.Resources))]
        TitleChangeEng,
        [Display(Name = "Enum_Template_Type_Reemployement_Amharic", ResourceType = typeof(Localization.Resources))]
        ReemployementAm,
        [Display(Name = "Enum_Template_Type_Reemployement_English", ResourceType = typeof(Localization.Resources))]
        ReemployementEng,
        [Display(Name = "Enum_Template_Type_Experiance_Affan_Oromo", ResourceType = typeof(Localization.Resources))]
        ExperianceOromo,
        [Display(Name = "Enum_Template_Type_Recruitment_Letter_Affan_Oromo", ResourceType = typeof(Localization.Resources))]
        RecruitmentLetterAffanOromo,
        [Display(Name = "Enum_Template_Type_Salary_Incremen_Affan_Oromo", ResourceType = typeof(Localization.Resources))]
        SalaryIncrementAffanOromo,
        [Display(Name = "Enum_Template_Type_Termination_Acceptance_Affan_Oromo", ResourceType = typeof(Localization.Resources))]
        TerminationAcceptancAffanOromo,
        [Display(Name = "Enum_Template_Type_Transfer_Affan_Oromo", ResourceType = typeof(Localization.Resources))]
        TransferAffanOromo,
        [Display(Name = "Promotion Affan-Oromo")]
        PromotionAffanOromo,
        [Display(Name = "የስራ_ልምድ_በአማርኛ_ከተሰናበቱ_በኋላ")]
        ExperienceAfterTerminbation,
    }

}
