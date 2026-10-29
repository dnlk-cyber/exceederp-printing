using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.Common
{
    public enum InfoDeskModuleReportType
    {
        ICT_Directorate_Maintainance_Form,
        Software_License,
        IT_Support_Report
    }
    public enum HealthAndInsuranceModuleReportType
    {
        Employee_Health_and_Value_Payment,
        Clinic_Report,
        Patient_Referral_Report
    }
    public enum InternalAuditModuleReportType
    {
        Internal_Audit_Finding_Report,
        Audit_Finding_FollowUp_CheckList_Report,
        Performance_Audit_Finding_Follow_Up_Report,
        Consumable_Item_Count_Report,
        Bank_Statement_Balance_Report,
        Cash_Book_Balance_Report,
        Cash_And_Property_Document_Cut_Off_Report,
        Audit_Narrative_Report,
        Audit_OverView_Report,
        Feasibility_Study_Work_Report,
        Fixed_Asset_Count_Report,
        Fixed_Asset_Balance_Report,
        Fixed_Asset_Ledger_Balance_Report,
        Vehicle_Usage_Report,
        Machinery_Count_Report,
        Vehicle_Count_Report,
        Bank_Book_Balance_Report,
        Employee_Ledger_Balance_Report,
        Ledger_Balance_Report,
        Receivable_and_Payable_Ledger_Balance_Report,
        Receivable_And_Payable_Ledger_Balance_Long_Stayed


    }
    public enum ArchiveModuleReportType
    {
        Incoming_Letters_Report,
        Outgoing_Letters_Report,
    }
    public enum LegalCaseModuleReportType
    {
        Case_Report,
        Auditor_Report,
        Case_Summary,
        Case_Progress,
        Case_Summary_By_Category_Report,
        Research_Report,
        Legal_Advisory_Request_Report,
        Legal_Awareness,
        Legal_Advisory_Contract_Claim_Report,
        Get_Response,
        Legal_Case_Report,
        Closed_Legal_Case_Files_Report,
        Hearing_Proceedings_Report,
        New_Case_Report,
        Decided_Case_Report
    }
    public enum ProductionModuleReportType
    {
        Work_Order_sheet_Report,
        Production_Report,
        Quality_Test_Record,
        HCB,
        HCB_Quaity_Test_Report,
        Compressive_Strength_Report,
        Balasting_Material_Utilazation_Report,
        Production_Summary,
        Production_Details_Report,
        Machine_Work_Time_Sheet,
        Plat_Work_Time_Sheet,
        Explosive_Material_Utilization_Report

    }

    public enum PlanningModuleReportType
    {
        Excutive_Summary_Report,
        Project_Excutive_Summary,
        Project_Information_Plan_Quartery_Report,
        Project_Qurterly_Summary_Report,
        Plan_Report,
        Actual_Report,
        Project_Plan_Report,
        Project_Actual_Report,
        Planning_And_Actual_Report,
        Project_Plan_VS_Actual_Report,
        Risk_Mitigation_Plan_Report,
        Risk_Follow_Up_Report,
        Project_Summary_By_Sectors,
        Plan_Vs_Actual_for_Weekly_Report,
        Project_Plan_Vs_Project_Actual_for_Weekly_Report,
        Project_Excutive_Summary_Per_Project,
        Quarterly_Plan_Report,
        Plan_VS_Actual_Report,
        KPI_Progress_Tracker_Report
    }

    public enum HRMModuleReportType
    {
        Employee_Placement_Report,
        Employee_Summary_by_Age,
        Employee_Performance_Total_Summary_Report,
        Employment_Type,
        Performance_Summary,
        Employee_Performance_Summary,
        Employee_Performance,
        Employee_Performance_Department,
        Employee_Yearly_or_Half_yearly_or_Quarterly_Performance,
        Employee_Bsc_Performance,
        Probation_Measure,
        My_Performance,
        Employee_Education,
        Attendance_Report,
        Employee_Salary,
        Employee_Birth_Date,
        Long_Term_Education,
        Employee_Summary,
        Termination_Summary,
        Termination_Type_Summary,
        Job_Title,
        Discipline_Measure_Summary,
        Retirement_Notification,
        Employee_Disciplinary_Measure,
        Employee_Job_Title,
        Employee_Pension,
        Educational_Level,
        Educational_Field,
        Employee_Summary_By_Nation,
        Employee_Summary_By_Rank,
        Employee_Termination,
        Employee_Training,
        Employee_Experience,
        Employee_By_Branch,
        Vacant_Postion,
        Employee_Judge_By_Region,
        Employee_Promotion_PerYear,
        Work_and_Cost_Summary,
        Employee_to_be_Promoted,
        Employee_Master_Report,
        Employee_Health_and_Value_Payment,
        Employee_Health_and_Value_Payment_selfCare,
        Education_and_Training_One_By_One,
        Employee_Type_Summary_Report,
        Training_One_By_One,
        Training_Request_Report,
        Not_Placed_Employees,
        Company_Structure,
        Monthly_Salary_Per_Period,
        Employee_Profile_Report,
        Attendance_Letter,
        Employment_Contract_Termination_Letter,
        Rehirement_Letter,
        Training_Evaluation,
        Change_Of_Payroll,
        Payable_Leave,
        Leave_Plan,
        Internal_Vacancy_For_SelfCare,
        Job_Vacancy_Announcement,
        Internal_Applicant,
        External_Applicant,
        Vacancy_For_Media,
        Day_Absent_And_Late_Coming_Report,
        Leave_Payable_Report,
        Annual_Leave_Report,
        Leave_Request_Status,
        Employee_Leave_Summary,
        Employee_In_Leave,
        Leave_Summary,
        Severance_Payable_Report,
        Over_Time_Hours,
        Commitment_report,
        Benefit_Report,
        Not_Evaluated_Employee_Report,
        Man_Power_Planning_Request_Letter,
        Employee_Perfromance_Leave_Dispiline_Bonus_Summary,
        Employee_Perfromance_With_Planning_Summary,
        Contract_Employee,
        Employee_Summary_By_Business_Unit,
        Employee_Termination_Summary_By_Business_Unit,
        Training_Need_Report,
        Training_Action_Plan,
        Employee_Promation_Summay,
        Employee_Bonus_Summary,
        Employee_Bonus_Summary_For_Manager,
        Employee_Bonus_Summary_For_Sector,
        Employee_Bonus_Summary_For_Project_Manager,
        Contract_Hirement,
        Temination_For_Manager,
        Temination_Letter,
        New_Assignment_for_Manager,
        Tranfer_Letter_for_Manager,
        Transfer_Letter,
        Anunal_Increement_letter,
        Promotion_Letter,
        Termination_Warning_Letter,
        Assignment_Letter,
        Recruitment_Letter,
        Probation_Letter,
        Employee_Transfer,
        Internal_Applicants,
        Permanent_Letter,
        Clearance_Letter,
        Clearance_Report,
        Education_Opportunity_Report,
        Education_Opportunity_Summary_Report,
        Daily_Labor_Report,
        Daily_Labor_Payroll_Sheet_Report,
        Daily_Labourer_Income_Tax,
        Daily_Labourers_Montly_Pension,
        Daily_Labourer_Daynamic_Payroll_Detail,
        Training_Report,
        Employee_Cosign,
        Employee_Guarant_or_Surety_Report,
        Employee_Responsiblity_Level_Summary,
        Employee_JobTitle_Summary,
        BusinessUnit_Branch_Summary,
        Employee_Fixed_Asset,
        Salary_Summary,
        Employee_Attendance_Summary,
        Employee_National_ID,
        Employee_Recruitment,
        Employee_Religion,
        Leave_Usage_Report,
        Employee_Holiday_Gift_Report,
        Employee_Assignment_Summary,
        Natinal_Bank_General_Summary,
        Staff_Education_Qualification_Summary,
        Gender_Percentage_Summary,
        Promotion_and_Salary_Adjustment,
        Template_Letter,
        Employee_Identification,
        Employee_Payroll_Summary,
        Day_Absent_Summary

    }

    public enum ProcurementModuleReportType
    {
        
        Service_Requestion,
        Purchase_Order,
        Purchase_Requisition,
        Purchase_Payment_Request,
        Proforma_Result_Summary,
        Award,
    }
    public enum FinalizeType
    {
        WAITING,
        ASSIGNED,
        RESOLVED,
        CLOSED

    }
    public enum EvaluationType
    {
        NA,
        [Display(Name = "Enum_Evaluation_Type_Direct_Boss", ResourceType = typeof(Resources))]
        DirectBoss,
        [Display(Name = "Enum_Evaluation_Type_Self_Evaluation", ResourceType = typeof(Resources))]
        SelfEvaluation,
        [Display(Name = "Enum_Evaluation_Type_Peer_Evaluation", ResourceType = typeof(Resources))]
        PeerEvaluation,
        [Display(Name = "Enum_Evaluation_Type_Internal_Customer", ResourceType = typeof(Resources))]
        InternalCustomer,
        [Display(Name = "Enum_Evaluation_Type_Company_level", ResourceType = typeof(Resources))]
        Companylevel,
        [Display(Name = "Enum_Evaluation_Type_Corporate_Result", ResourceType = typeof(Resources))]
        Corporate_Result,
        [Display(Name = "Enum_Evaluation_Type_Sector_Result", ResourceType = typeof(Resources))]
        Sector_Result,
        [Display(Name = "Enum_Evaluation_Type_Main_Department_Level", ResourceType = typeof(Resources))]
        MainDepartmentLevel,
        [Display(Name = "Enum_LookUpTypes_PROJECT_MANAGER", ResourceType = typeof(Resources))]
        Project_Manager,
        [Display(Name = "Enum_Evaluation_Type_Others", ResourceType = typeof(Resources))]
        Others

    }
    public enum ExamType
    {
        [Display(Name = "Enum_RecruitmentSetting_THEORY_EXAM", ResourceType = typeof(Resources))]
        THEORY_EXAM,
        [Display(Name = "Enum_RecruitmentSetting_PRACTICAL_EXAM", ResourceType = typeof(Resources))]
        PRACTICAL_EXAM,
        [Display(Name = "Enum_RecruitmentSetting_INTERVIEW", ResourceType = typeof(Resources))]
        INTERVIEW,
    }
    public enum RetireTypeSetting
    {
        Heads,
        Performers
    }
    public enum EvaluationLevel
    {
        All,
        Global,
        Department
    }
    public enum SelfServiceRequestType
    {
        [Display(Name = "Enum_Self_Service_Request_Type_Expreience", ResourceType = typeof(Resources))]
        Expreience,
        [Display(Name = "Enum_Self_Service_Request_Type_Support_Letter", ResourceType = typeof(Resources))]
        [Description("Support Letter")]
        Support_Letter,
        [Display(Name = "Enum_Self_Service_Request_Type_Warranty_Letter", ResourceType = typeof(Resources))]
        [Description("Warranty Letter")]
        Warranty_Letter
    }
    public enum LeaveDay
    {
        [Display(Name = "Enum_Leave_Day_Full_Time", ResourceType = typeof(Resources))]
        Full_Day,
        [Display(Name = "Enum_Leave_Day_Half_Time", ResourceType = typeof(Resources))]
        Half_Day,
        Mixed
    }

    public enum MorningOrAfternoon
    {
        [Display(Name = "Morning", ResourceType = typeof(Resources))]
        Morning,
        [Display(Name = "Afternoon", ResourceType = typeof(Resources))]
        Afternoon
    }

    public enum BranchTypes
    {
        [Display(Name = "Enum_BranchTypes_Branch", ResourceType = typeof(Resources))]
        HEAD_OFFICE,
        [Display(Name = "Enum_BranchTypes_Project", ResourceType = typeof(Resources))]
        PROJECT
    }
    public enum PeriodType
    {
        //[Display(Name = "Enum_PAYROLL_PERIOD", ResourceType = typeof(Resources))]
        //PAYROLL_PERIOD,
        [Display(Name = "Enum_ACCOUNTING_PERIOD", ResourceType = typeof(Resources))]
        ACCOUNTING_PERIOD,
        [Display(Name = "Enum_HOLIDAY", ResourceType = typeof(Resources))]
        HOLIDAY,
        [Display(Name = "Enum_SEASON", ResourceType = typeof(Resources))]
        SEASON,
        [Display(Name = "Enum_FISCAL_YEAR", ResourceType = typeof(Resources))]
        FISCAL_YEAR,
        [Display(Name = "Enum_QUARTER", ResourceType = typeof(Resources))]
        QUARTER,
        [Display(Name = "Enum_SEMI_ANNUAL", ResourceType = typeof(Resources))]
        SEMI_ANNUAL,
        [Display(Name = "Enum_WEEKLY", ResourceType = typeof(Resources))]
        WEEKLY,
        //[Display(Name = "Enum_LEAVE_PERIOD", ResourceType = typeof(Resources))]
        //LEAVE_PERIOD,
        //[Display(Name = "Enum_EVALUATION_PERIOD", ResourceType = typeof(Resources))]
        //EVALUATION_PERIOD
        LIABILITY_PERIOD


    }



    public enum PlanType
    {
        [Display(Name = "Enum_RECRUITMENT", ResourceType = typeof(Resources))]
        RECRUITMENT
    }
    public enum PeriodStatusList
    {
        [Display(Name = "Enum_PeriodStatusList_NEW", ResourceType = typeof(Resources))]
        NEW,
        [Display(Name = "Enum_PeriodStatusList_PROCESSING", ResourceType = typeof(Resources))]
        PROCESSING,
        [Display(Name = "Enum_PeriodStatusList_CLOSED", ResourceType = typeof(Resources))]
        CLOSED,
        [Display(Name = "Enum_PeriodStatusList_TRANSFERRED", ResourceType = typeof(Resources))]
        TRANSFERRED,
        REVERSING,
        PROOF_SHEET
    }
    public enum LetterType
    {
        [Display(Name = "Enum_LetterType_INCOMING", ResourceType = typeof(Resources))]
        INCOMING,
        [Display(Name = "Enum_LetterType_OUTGOING", ResourceType = typeof(Resources))]
        OUTGOING



    }
    public enum ContributionCategory
    {
        [Display(Name = "Enum_ContributionCategory_FIXED", ResourceType = typeof(Resources))]
        FIXED,
        [Display(Name = "Enum_ContributionCategory_REGULAR", ResourceType = typeof(Resources))]
        REGULAR



    }
    public enum LettersStatus
    {
        [Display(Name = "Enum_LettersStatus_NEW", ResourceType = typeof(Resources))]
        NEW,
        [Display(Name = "Enum_LettersStatus_ANSWERED", ResourceType = typeof(Resources))]
        ANSWERED,
        [Display(Name = "Enum_LettersStatus_TRANSFERED", ResourceType = typeof(Resources))]
        TRANSFERED,
        WAITING





    }
    public enum LetterOwner
    {
        [Display(Name = "Enum_LetterOwner_ORGANIZATION", ResourceType = typeof(Resources))]
        ORGANIZATION,
        [Display(Name = "Enum_LetterOwner_PRIVATE", ResourceType = typeof(Resources))]
        PRIVATE



    }

    public enum AttachmentType
    {
        PHOTO,
        EXPERIENCE,
        TRAINING,
        EDUCATION,
        COMMITMENT,
        INCOMING_LETTER,
        OUTGOING_LETTER,
        CLEARANCE,
        LOGO,
        OTHER,
        LETTER,
        COSIGN,
        MINUTE,
        DOCUMENT,
        PROJECT_DOCUMENT,
        ISO_LOGO,
        STAMP
    }

    public enum AccountRequirementDefinition
    {
        SALES_ACCOUNT,
        INVENTORY_ACCOUNT,
        COSTOFSALES_ACCOUNT,

    }

    public enum AddressType
    {
        [Display(Name = "Enum_AddressType_OFFICE_PHONE", ResourceType = typeof(Resources))]
        OFFICE_PHONE,
        [Display(Name = "Enum_AddressType_OFFICE_HOME_PHONE", ResourceType = typeof(Resources))]
        HOME_PHONE,
        [Display(Name = "Enum_AddressType_OFFICE_MOBILE_PHONE_1", ResourceType = typeof(Resources))]
        MOBILE_PHONE_1,
        [Display(Name = "Enum_AddressType_OFFICE_MOBILE_PHONE_2", ResourceType = typeof(Resources))]
        MOBILE_PHONE_2,
        [Display(Name = "Enum_AddressType_OFFICE_FACEBOOK", ResourceType = typeof(Resources))]
        FACEBOOK,
        [Display(Name = "Enum_AddressType_OFFICE_TWITTER", ResourceType = typeof(Resources))]
        TWITTER,
        [Display(Name = "Enum_AddressType_OFFICE_LINKEDIN", ResourceType = typeof(Resources))]
        LINKEDIN,
        [Display(Name = "Enum_AddressType_OFFICE_SKYPE", ResourceType = typeof(Resources))]
        SKYPE,
        [Display(Name = "Enum_AddressType_OFFICE_REGION", ResourceType = typeof(Resources))]
        REGION,
        [Display(Name = "Enum_AddressType_OFFICE_CONTACT_PERSON", ResourceType = typeof(Resources))]
        CONTACT_PERSON,
        [Display(Name = "Enum_AddressType_OFFICE_WORADA", ResourceType = typeof(Resources))]
        WORADA,
        [Display(Name = "Enum_AddressType_OFFICE_HOUSE_NO", ResourceType = typeof(Resources))]
        HOUSE_NO,
        [Display(Name = "Enum_AddressType_OFFICE_STREET", ResourceType = typeof(Resources))]
        STREET,
        [Display(Name = "Enum_AddressType_OFFICE_SPECIAL_LOCATION", ResourceType = typeof(Resources))]
        SPECIAL_LOCATION,
        [Display(Name = "Enum_AddressType_OFFICE_OFFICE_EMAIL", ResourceType = typeof(Resources))]
        OFFICE_EMAIL,
        [Display(Name = "Enum_AddressType_OFFICE_PERSONAL_EMAIL", ResourceType = typeof(Resources))]
        PERSONAL_EMAIL,
        [Display(Name = "Enum_AddressType_OFFICE_WEBSITE", ResourceType = typeof(Resources))]
        WEBSITE,
        [Display(Name = "Enum_AddressType_OFFICE_COUNTRY", ResourceType = typeof(Resources))]
        COUNTRY,
        [Display(Name = "Enum_AddressType_FAX", ResourceType = typeof(Resources))]
        FAX,
        [Display(Name = "Enum_AddressType_OFFICE_POBOX", ResourceType = typeof(Resources))]
        POBOX,
        [Display(Name = "Enum_AddressType_OFFICE_CITYPROVINCE", ResourceType = typeof(Resources))]
        CITYPROVINCE,
        [Display(Name = "Enum_AddressType_KIFLEKETEMA", ResourceType = typeof(Resources))]
        KIFLEKETEMA,
        [Display(Name = "Enum_AddressType_KEBELE", ResourceType = typeof(Resources))]
        KEBELE

    }


    public enum PersonTypes
    {
        EMPLOYEE_PERSON,
        CUSTOMER_PERSON,
        SUPPLIER_PERSON,
        GUEST_PERSON,
        INTERN_PERSON,
        RECRUIT_PERSON,
        PATIENT_PERSON,
        OWNER_PERSON,
        AGENT_PERSON,
        SHAREHOLDERS_PERSON,
        CONTACT_PERSON


    }

    public enum Thing_Types
    {
        ITEM,
        PRODUCT,
        SEMIFINISHED_PRODUCT,
        SERVICE,
        FIXED_ASSET


    }

    public enum Price_Types
    {
        QTY_PRICE,
        WHOLE_SALE,
        RETAIL_PRICE,
        CONSUMER_PRICE,
        ON_HOLIDAY,
        NORMAL_PRICE,
        DISCOUNTED_PRICE

    }
    public enum OrganizationTypes
    {
        CUSTOMER_ORGANIZATION,
        SUPPLIER_ORGANIZATION,
        AGENT_ORGANIZATION,
        COMPETITOR_ORGANIZATION,
        STAKEHOLDER_ORGANIZATION,
        SHAREHOLDER_ORGANIZATION,
        INSURANCE_ORGANIZATION,
        BANK_ORGANIZATION,
        BUSINESSSOURCE_ORGANIZATION,
        GROUP_ORGANIZATION

    }
    public enum PaymentMethodType
    {
        CASH,
        CHECK

    }

    public enum Gender
    {
        [Display(Name = "Enum_Male", ResourceType = typeof(Resources))]
        MALE = 5,
        [Display(Name = "Enum_Female", ResourceType = typeof(Resources))]
        FEMALE = 6
    }
    //for those who have salaries from two companies
    public enum PayrollCategory
    {
        Main_Category,
        Seconday_category
    }
    public enum AllowanceType
    {
        [Display(Name = "Enum_AllowanceType_TRANSPORT", ResourceType = typeof(Resources))]
        TRANSPORT,
        [Display(Name = "Enum_AllowanceType_TELEPHONE", ResourceType = typeof(Resources))]
        TELEPHONE,
        [Display(Name = "Enum_AllowanceType_HOME", ResourceType = typeof(Resources))]
        HOME,
        [Display(Name = "Enum_AllowanceType_POSITION", ResourceType = typeof(Resources))]
        POSITION,
        [Display(Name = "Enum_AllowanceType_PROFESSIONAL", ResourceType = typeof(Resources))]
        PROFESSIONAL,
        // [Display(Name = "Enum_AllowanceType_TOP_UP", ResourceType = typeof(Resources))]
        //TOP_UP,
        [Display(Name = "Enum_AllowanceType_DESERT", ResourceType = typeof(Resources))]
        DESERT,
        [Display(Name = "Enum_AllowanceType_PROJECT", ResourceType = typeof(Resources))]
        PROJECT,
        [Display(Name = "Enum_AllowanceType_RESPONSIBILITY", ResourceType = typeof(Resources))]
        RESPONSIBILITY,
        DISTURBANCE,
        REFRESHMENT,
        HARDSHIP,
        PERDIEM_TAXABLE,
        PERDIEM_NON_TAXABLE,

    }

    public enum OrganizationUnitDefinitionType
    {
        PROCESS_COUNCIL,
        PROCESS,
        TEAM,
        CASE_TEAM,

        BRANCH,
        BOARD,
        DEPARTMENT,
        DIVISION,
        DIRECTORATE,
        HEAD_OFFICE,
        SECTION,

        ROLE



    }
    public enum CommitmentType
    {
        [Display(Name = "Enum_CommitmentType_EDUCATION", ResourceType = typeof(Resources))]
        EDUCATION,
        [Display(Name = "Enum_CommitmentType_TRAINING", ResourceType = typeof(Resources))]
        TRAINING

    }

    public enum PersonTitle
    {
        [Display(Name = "Enum_PersonTitle_W_RO", ResourceType = typeof(Resources))]
        W_RO,
        [Display(Name = "Enum_PersonTitle_W_T", ResourceType = typeof(Resources))]

        W_T,
        [Display(Name = "Enum_PersonTitle_ATO", ResourceType = typeof(Resources))]
        ATO,
        [Display(Name = "Enum_PersonTitle_CAPITAIN", ResourceType = typeof(Resources))]
        CAPITAIN,
        [Display(Name = "Enum_PersonTitle_PASTER", ResourceType = typeof(Resources))]
        PASTER,
        [Display(Name = "Enum_PersonTitle_SHEK", ResourceType = typeof(Resources))]
        SHEK,
        [Display(Name = "Enum_PersonTitle_LOWRET", ResourceType = typeof(Resources))]
        LOWRET,
        [Display(Name = "Enum_PersonTitle_PROFFESOR", ResourceType = typeof(Resources))]
        PROFFESOR,
        [Display(Name = "Enum_PersonTitle_BRIGADIER", ResourceType = typeof(Resources))]
        BRIGADIER,
        [Display(Name = "Enum_PersonTitle_GENERAL", ResourceType = typeof(Resources))]
        GENERAL,
        [Display(Name = "Enum_PersonTitle_CRENEL", ResourceType = typeof(Resources))]
        CRENEL,
        [Display(Name = "Enum_PersonTitle_AMBASADOR", ResourceType = typeof(Resources))]
        AMBASADOR,
        [Display(Name = "Enum_PersonTitle_SIR", ResourceType = typeof(Resources))]
        SIR,
        [Display(Name = "Enum_PersonTitle_MR", ResourceType = typeof(Resources))]
        MR,
        [Display(Name = "Enum_PersonTitle_MAJOR", ResourceType = typeof(Resources))]
        MAJOR,
        [Display(Name = "Enum_PersonTitle_BISHOP", ResourceType = typeof(Resources))]
        BISHOP,
        [Display(Name = "Enum_PersonTitle_POPE", ResourceType = typeof(Resources))]
        POPE,
        [Display(Name = "Enum_PersonTitle_HISEXCELLENCY", ResourceType = typeof(Resources))]
        HISEXCELLENCY,
        [Display(Name = "Enum_PersonTitle_HONORABLE", ResourceType = typeof(Resources))]
        HONORABLE,
        [Display(Name = "Enum_PersonTitle_MRS", ResourceType = typeof(Resources))]
        MRS,
        [Display(Name = "Enum_PersonTitle_DR", ResourceType = typeof(Resources))]
        DR,
        [Display(Name = "Enum_PersonTitle_ENGINEER", ResourceType = typeof(Resources))]
        ENGINEER,
        [Display(Name = "Enum_PersonTitle_DIPLOMAT", ResourceType = typeof(Resources))]
        DIPLOMAT


    }

    public enum IdentificationType
    {

        [Display(Name = "Enum_IdentificationType_RESIDENTIAL_ID", ResourceType = typeof(Resources))]
        RESIDENTIAL_ID,
        [Display(Name = "Enum_IdentificationType_PENSION_NO", ResourceType = typeof(Resources))]
        PENSION_NO,
        [Display(Name = "Enum_IdentificationType_STUDENT_ID", ResourceType = typeof(Resources))]
        STUDENT_ID,
        [Display(Name = "Enum_IdentificationType_WORK_ID", ResourceType = typeof(Resources))]
        WORK_ID,
        [Display(Name = "Enum_IdentificationType_DRIVING_LICENSE", ResourceType = typeof(Resources))]
        DRIVING_LICENSE,
        [Display(Name = "Enum_IdentificationType_SMART_CARD", ResourceType = typeof(Resources))]
        SMART_CARD,
        [Display(Name = "Enum_IdentificationType_PASSPORT", ResourceType = typeof(Resources))]
        PASSPORT,
        [Display(Name = "Enum_IdentificationType_VISA", ResourceType = typeof(Resources))]
        VISA,
        [Display(Name = "Enum_IdentificationType_BIOMETRIC_ID", ResourceType = typeof(Resources))]
        BIOMETRIC_ID,
        [Display(Name = "Enum_IdentificationType_TIN", ResourceType = typeof(Resources))]
        TIN,
        [Display(Name = "Enum_IdentificationType_VAT_NUMBER", ResourceType = typeof(Resources))]
        VAT_NUMBER,
        [Display(Name = "Enum_IdentificationType_LICENSE_NUMBER", ResourceType = typeof(Resources))]
        LICENSE_NUMBER,
        [Display(Name = "Enum_IdentificationType_PROVIDENT_FUND", ResourceType = typeof(Resources))]
        PROVIDENT_FUND,
        [Display(Name = "Enum_IdentificationType_Fayda_ID_Or_National_ID", ResourceType = typeof(Resources))]
        Fayda_ID_Or_National_ID


    }

    public enum IndustryType
    {
        [Display(Name = "Enum_IndustryType_RETAIL", ResourceType = typeof(Resources))]
        RETAIL,
        [Display(Name = "Enum_IndustryType_WHOLESALES", ResourceType = typeof(Resources))]
        WHOLESALES,
        [Display(Name = "Enum_IndustryType_PHARMACY", ResourceType = typeof(Resources))]
        PHARMACY,
        [Display(Name = "Enum_IndustryType_HOSPITALITY", ResourceType = typeof(Resources))]
        HOSPITALITY,
        [Display(Name = "Enum_IndustryType_RESTAURANT", ResourceType = typeof(Resources))]
        RESTAURANT,
        [Display(Name = "Enum_IndustryType_CAFEE", ResourceType = typeof(Resources))]
        CAFEE,
        [Display(Name = "Enum_IndustryType_HOSPITAL", ResourceType = typeof(Resources))]
        HOSPITAL,
        [Display(Name = "Enum_IndustryType_CLINIC", ResourceType = typeof(Resources))]
        CLINIC,
        [Display(Name = "Enum_IndustryType_MANUFACTURING", ResourceType = typeof(Resources))]
        MANUFACTURING,
        [Display(Name = "Enum_IndustryType_AUTOMOTIVE", ResourceType = typeof(Resources))]
        AUTOMOTIVE,
        [Display(Name = "Enum_IndustryType_ELECTRONICS", ResourceType = typeof(Resources))]
        ELECTRONICS,
        [Display(Name = "Enum_IndustryType_CONSTRUCTION", ResourceType = typeof(Resources))]
        CONSTRUCTION,
        [Display(Name = "Enum_IndustryType_REAL_STATE", ResourceType = typeof(Resources))]
        REAL_STATE,
        [Display(Name = "Enum_IndustryType_BOOK_AND_PUBLICATION", ResourceType = typeof(Resources))]
        BOOK_AND_PUBLICATION,
        [Display(Name = "Enum_IndustryType_GARMENT", ResourceType = typeof(Resources))]
        GARMENT


    }


    public enum BankAccountType
    {
        [Display(Name = "Enum_BankAccountType_PAYROLL", ResourceType = typeof(Resources))]
        PAYROLL,
        [Display(Name = "Enum_BankAccountType_SAVING", ResourceType = typeof(Resources))]
        SAVING,
        [Display(Name = "Enum_BankAccountType_CREDIT", ResourceType = typeof(Resources))]
        CREDIT


    }
    public enum WeekendName
    {
        [Display(Name = "Enum_Weekend_SUNDAY", ResourceType = typeof(Resources))]
        SUNDAY,
        [Display(Name = "Enum_Weekend_SATURDAY", ResourceType = typeof(Resources))]
        SATURDAY,



    }
    public enum BusinessType
    {
        NONE_GOVERNMENTAL,
        GOVERNMENTAL,
        PRIVATE,
        SHARE_COMPANY,
        LIABLITY_LIMITED_COMPANY,
        PRIVATE_LIMITED_COMPANY,
        SOLE_PROPRITES

    }

    public enum NormalBalanceType
    {
        DEBIT,
        CREDIT

    }

    public enum OrganizationIdentificationType
    {
        BUSINESS_REGISTRATION,
        BUSINESS_LICENSE,
        TAX_IDENTIFICATION_NO,
        VAT_CERTIFICATE,
        INVESTMENT_LICENSE,
        TIN

    }

    public enum UnitOfMeasurement
    {
        METER_SQUARE,
        METER_CUBE,
        METER,
        PCS,
        KG,
        PS, LS




    }

    public enum StructureType
    {
        SUPER_STRUCTURE,
        SUB_STRUCTURE


    }

    public enum StockBalanceType
    {
        CLOSING_BALANCE,
        MANUALCOUNT,
        INSPECTION_BALANCE,
        BEGINNING_BALANCE

    }

    public enum AssocationType
    {
        [Display(Name = "Enum_AssocationType_CREDIT", ResourceType = typeof(Resources))]
        CREDIT,
        [Display(Name = "Enum_AssocationType_LABOUR", ResourceType = typeof(Resources))]
        LABOUR,
        [Display(Name = "Enum_AssocationType_SPORT_FEE", ResourceType = typeof(Resources))]
        SPORT_FEE

    }

    public enum PenalityType
    {
        [Display(Name = "Enum_PenalityType_PENALITY", ResourceType = typeof(Resources))]
        EMPLOYEE_PENALITY,
    }

    public enum TrainingLocation
    {
        [Display(Name = "Enum_TrainingLocation_INTERNALTRAINING", ResourceType = typeof(Resources))]
        INTERNAL,
        [Display(Name = "Enum_TrainingLocation_EXTERNAL_TRAINING", ResourceType = typeof(Resources))]
        EXTERNAL

    }
    public enum TrainingTerm
    {
        [Display(Name = "Enum_TrainingTerm_SHORT", ResourceType = typeof(Resources))]
        SHORT,
        [Display(Name = "Enum_TrainingTerm_LONG", ResourceType = typeof(Resources))]
        LONG

    }
    public enum TrainingType
    {
        [Display(Name = "Enum_TrainingType_EDUCATION", ResourceType = typeof(Resources))]
        EDUCATION,
        [Display(Name = "Enum_TrainingType_TRAINING", ResourceType = typeof(Resources))]
        TRAINING

    }
    public enum LookUpTypes //in put for look up object-type attribute
    {

        [Display(Name = "Enum_LookUpTypes_ADDITION_TYPE", ResourceType = typeof(Resources))]
        ADDITION_TYPE,
        [Display(Name = "Enum_LookUpTypes_OTHER_BENEFIT", ResourceType = typeof(Resources))]
        OTHER_BENEFIT,
        [Display(Name = "Enum_LookUpTypes_COURSE", ResourceType = typeof(Resources))]
        COURSE,
        [Display(Name = "Enum_LookUpTypes_COMMITTEE_TYPE", ResourceType = typeof(Resources))]
        COMMITTEE_TYPE,
        [Display(Name = "Enum_LookUpTypes_CONTRIBUTION_TYPE", ResourceType = typeof(Resources))]
        CONTRIBUTION_TYPE,
        [Display(Name = "Enum_LookUpTypes_DOCUMENT_TYPE", ResourceType = typeof(Resources))]
        DOCUMENT_TYPE,
        [Display(Name = "Enum_LookUpTypes_EDUCATION_LEVEL", ResourceType = typeof(Resources))]
        EDUCATION_LEVEL,
        [Display(Name = "Enum_LookUpTypes_EDUCATION_FIELD", ResourceType = typeof(Resources))]
        EDUCATION_FIELD,
        [Display(Name = "Enum_LookUpTypes_JOB_TITLE", ResourceType = typeof(Resources))]
        JOB_TITLE,
        [Display(Name = "Enum_LookUpTypes_NATION", ResourceType = typeof(Resources))]
        NATION,
        [Display(Name = "Enum_LookUpTypes_PROBATION_STATUS", ResourceType = typeof(Resources))]
        PROBATION_STATUS,
        [Display(Name = "Enum_LookUpTypes_RELATIONSHIP", ResourceType = typeof(Resources))]
        RELATIONSHIP,
        [Display(Name = "Enum_LookUpTypes_TARGET_GROUP", ResourceType = typeof(Resources))]
        TARGET_GROUP,
        [Display(Name = "Enum_LookUpTypes_TERMINATIONTYPE", ResourceType = typeof(Resources))]
        TERMINATIONTYPE,
        [Display(Name = "Enum_LookUpTypes_TRAINING_TYPE", ResourceType = typeof(Resources))]
        TRAINING_TYPE,
        [Display(Name = "Enum_LookUpTypes_OVERTIME_TYPE", ResourceType = typeof(Resources))]
        OVERTIME_TYPE,
        [Display(Name = "Enum_LookUpTypes_UOM", ResourceType = typeof(Resources))]
        UOM,
        [Display(Name = "Enum_LookUpTypes_INSURANCE_CLASS_OF_BUSINESS", ResourceType = typeof(Resources))]
        INSURANCE_CLASS_OF_BUSINESS,
        [Display(Name = "Enum_LookUpTypes_EVALUATION_TYPE", ResourceType = typeof(Resources))]
        EVALUATION_TYPE,
        [Display(Name = "Enum_LookUpTypes_COSIGN_TYPE", ResourceType = typeof(Resources))]
        COSIGN_TYPE,
        [Display(Name = "Enum_LookUpTypes_MEDIA", ResourceType = typeof(Resources))]
        MEDIA,
        [Display(Name = "Enum_LookUpTypes_SKILL_TYPE", ResourceType = typeof(Resources))]
        SKILL_TYPE,
        [Display(Name = "Enum_LookUpTypes_COMMITMENT_TYPE", ResourceType = typeof(Resources))]
        COMMITMENT_TYPE,
        [Display(Name = "Enum_LookUpTypes_ROW_DATA", ResourceType = typeof(Resources))]
        ROW_DATA,
        [Display(Name = "Enum_LookUpTypes_WORK_ITEM", ResourceType = typeof(Resources))]
        WORK_ITEM,
        [Display(Name = "Enum_LookUpTypes_PROJECT_TYPE", ResourceType = typeof(Resources))]
        PROJECT_TYPE,
        [Display(Name = "Enum_LookUpTypes_CONSTRUCTION_TYPE", ResourceType = typeof(Resources))]
        CONSTRUCTION_TYPE,
        [Display(Name = "Enum_LookUpTypes_COMPLAIN_TYPE", ResourceType = typeof(Resources))]
        COMPLAIN_TYPE,
        TERM_TYPE,
        [Display(Name = "Enum_LookUpTypes_Person_Title", ResourceType = typeof(Resources))]
        PERSON_TITLE,
        [Display(Name = "Enum_LookUpTypes_PROBATION_MEASUREMENT", ResourceType = typeof(Resources))]
        PROBATION_MEASUREMENT,
        [Display(Name = "Enum_LookUpTypes_OLD_JOB_TITLE", ResourceType = typeof(Resources))]
        OLD_JOB_TITLE,
        Jobtitle_External_Experience,
        [Display(Name = "Enum_LookUpTypes_CLEARANCE_TYPE", ResourceType = typeof(Resources))]
        CLEARANCE_TYPE


    }
    public enum LookUpCategories //in put for look up object-type attribute
    {
        EDUCATION,
        DOCUMENT,
        STATUS,
        REASON,
        JOB,
        TRAINING,
        MEASURE_TYPE,
        RECOGNITION_TYPE,
        OTHER



    }

    public enum PayrollAccountType
    {
        [Display(Name = "Enum_PayrollAccountType_BASIC_SALARY_PERMANENT", ResourceType = typeof(Resources))]
        BASIC_SALARY_PERMANENT,
        [Display(Name = "Enum_PayrollAccountType_BASIC_SALARY_CONRACT", ResourceType = typeof(Resources))]
        BASIC_SALARY_CONRACT,
        [Display(Name = "Enum_PayrollAccountType_OVER_TIME", ResourceType = typeof(Resources))]
        OVER_TIME,
        [Display(Name = "Enum_PayrollAccountType_ADDITIONS", ResourceType = typeof(Resources))]
        ADDITIONS,
        [Display(Name = "Enum_PayrollAccountType_INCOME_TAX", ResourceType = typeof(Resources))]
        INCOME_TAX,
        [Display(Name = "Enum_PayrollAccountType_PENSION_COMPANY", ResourceType = typeof(Resources))]
        PENSION_COMPANY,
        [Display(Name = "Enum_PayrollAccountType_PENSION_EXPENSE", ResourceType = typeof(Resources))]
        PENSION_EXPENSE,
        [Display(Name = "Enum_PayrollAccountType_LOAN", ResourceType = typeof(Resources))]
        LOAN,
        [Display(Name = "Enum_PayrollAccountType_ADVANCE", ResourceType = typeof(Resources))]
        ADVANCE,
        [Display(Name = "Enum_PayrollAccountType_NET_PAY", ResourceType = typeof(Resources))]
        NET_PAY,
        [Display(Name = "Enum_PayrollAccountType_PENSION_PAYABLE", ResourceType = typeof(Resources))]
        PENSION_PAYABLE,
        [Display(Name = "Enum_PayrollAccountType_ASSOCIATION", ResourceType = typeof(Resources))]
        ASSOCIATION,
        [Display(Name = "Enum_PayrollAccountType_LABOR_UNION", ResourceType = typeof(Resources))]
        LABOR_UNION,
        [Display(Name = "Enum_AssocationType_SPORT_FEE", ResourceType = typeof(Resources))]
        SPORT_FEE,
        [Display(Name = "Enum_AssocationType_OTHER", ResourceType = typeof(Resources))]
        OTHER,

        PROVIDENT_COMPANY,

        PROVIDENT_EXPENSE,
        [Display(Name = "Enum_PayrollAccountType_PENSION_PAYABLE", ResourceType = typeof(Resources))]
        PROVIDENT_PAYABLE,
        CAMP_BENEFIT,
        SOFT_BENEFIT,
        MILK_BENEFIT,
        HOUSE_RENT_BENEFIT
    }
    public enum LeaveSetting
    {
        [Display(Name = "Enum_LeaveSetting_LEAVE_FOR_LEADRES", ResourceType = typeof(Resources))]
        LEAVE_FOR_LEADRES_OLD,
        [Display(Name = "Enum_LeaveSetting_LEAVE_FOR_PERFORMERS", ResourceType = typeof(Resources))]
        LEAVE_FOR_PERFORMERS_OLD,
        [Display(Name = "Enum_LeaveSetting_MAXIMUM_AMOUNT", ResourceType = typeof(Resources))]
        MAXIMUM_LEAVE_ENTITLEMENT,
        [Display(Name = "Enum_LeaveSetting_LEAVE_ENTITLE_THRESHOLD", ResourceType = typeof(Resources))]
        LEAVE_ENTITLE_THRESHOLD,
        [Display(Name = "Initial Leave for Leaders")]
        LEAVE_FOR_LEADRES_NEW,

        [Display(Name = "Initial Leave for Performers")]
        LEAVE_FOR_PERFORMERS_NEW,

        [Display(Name = "Old Initial Leave End Year")]
        Old_Initial_Leave_End_Year,

        [Display(Name = "Maximum Leave for Leaders")]
        MAXIMUM_Leave_For_Leaders,

        [Display(Name = "Apply Working Days for Maternity Leave")]
        APPLY_WORKING_DAYS_FOR_MATERNITY,

        [Display(Name = "Leave Entitlement Increase every  (Year)")]
        LEAVE_ENTITLEMENT_INCREEMENT_YEAR,

        [Display(Name = "Staff Leave Entitlement Increase Every (Years)")]
        Staff_Leave_Entitlement_Increase_Period,

        [Display(Name = "Management Leave Entitlement Increase Every (Years)")]
        Management_Leave_Entitlement_Increase_Period


    }

    public enum RecruitmentSetting
    {
        [Display(Name = "Enum_RecruitmentSetting_EDUCATIONAL_FIELD", ResourceType = typeof(Resources))]
        EDUCATIONAL_FIELD,
        [Display(Name = "Enum_RecruitmentSetting_EDUCATION_LEVEL", ResourceType = typeof(Resources))]
        EDUCATION_LEVEL,
        [Display(Name = "Enum_RecruitmentSetting_RELATED_EXPERIENCE", ResourceType = typeof(Resources))]
        RELATED_EXPERIENCE,
        [Display(Name = "Enum_RecruitmentSetting_RELEVANT_EXPERIENCE", ResourceType = typeof(Resources))]
        RELEVANT_EXPERIENCE,
        [Display(Name = "Enum_RecruitmentSetting_GENDER", ResourceType = typeof(Resources))]
        GENDER,
        [Display(Name = "Enum_RecruitmentSetting_DISABILITY", ResourceType = typeof(Resources))]
        DISABILITY,
        [Display(Name = "Enum_RecruitmentSetting_PROFILE", ResourceType = typeof(Resources))]
        PROFILE,
        [Display(Name = "Enum_RecruitmentSetting_TRAINING", ResourceType = typeof(Resources))]
        TRAINING,
        [Display(Name = "Enum_RecruitmentSetting_DISCIPLINE", ResourceType = typeof(Resources))]
        DISCIPLINE,
        [Display(Name = "Enum_RecruitmentSetting_THEORY_EXAM", ResourceType = typeof(Resources))]
        THEORY_EXAM,
        [Display(Name = "Enum_RecruitmentSetting_PRACTICAL_EXAM", ResourceType = typeof(Resources))]
        PRACTICAL_EXAM,
        [Display(Name = "Enum_RecruitmentSetting_INTERVIEW", ResourceType = typeof(Resources))]
        INTERVIEW,
        [Display(Name = "Enum_RecruitmentSetting_NATION", ResourceType = typeof(Resources))]
        NATION

    }
    public enum PayrollSettingCategories
    {
        [Display(Name = "Enum_PayrollSettingCategories_PROVIDENT_FUND", ResourceType = typeof(Resources))]
        PROVIDENT_FUND,
        [Display(Name = "Enum_PayrollSettingCategories_PENSION", ResourceType = typeof(Resources))]
        PENSION,
        [Display(Name = "Enum_PayrollSettingCategories_OVERTIME", ResourceType = typeof(Resources))]
        OVERTIME,
        [Display(Name = "Enum_PayrollSettingCategories_BONUS", ResourceType = typeof(Resources))]
        BONUS,
        [Display(Name = "Enum_PayrollSetting_ApplyProbationDayLength", ResourceType = typeof(Resources))]
        APPLY_PROBATION_DAY_LENGTH,
        DISABLE_PENSION_FOR_CONTRACT,
        DEDUCT_LEAVE_WITHOUT_PAYMENT,
        DELEGATION_BENEFIT_ALLOWED,
        DELEGATION_BENEFIT_RULE,
        APPLY_PROOF_SHEET,
       // DEDUCT_ALLOWANCE_BY_DAYS_ABSENT
        //DESERT_APPLY_WORKING_DAYS //we have otehr setting on allowance type rule
    }
    


    public enum PromotionSetting
    {
        [Display(Name = "Enum_PromotionSetting_PROMOTION_PER_YEAR", ResourceType = typeof(Resources))]
        PROMOTION_PER_YEAR,
        [Display(Name = "Enum_PromotionSetting_YEARLY_PROMOTION_PER_YEAR", ResourceType = typeof(Resources))]
        YEARLY_PROMOTION_PER_YEAR,
        [Display(Name = "Enum_PromotionSetting_VACANCY_LIFE_TIME_INDAY", ResourceType = typeof(Resources))]
        VACANCY_LIFE_TIME_INDAY,
        [Display(Name = "Enum_PromotionSetting_VACANCY_PASS_MARK", ResourceType = typeof(Resources))]
        PASS_MARK
    }
    public enum PensionAttribute
    {
        [Display(Name = "Enum_PensionAttribute_EMPLOYEE_CONTRIBUTION", ResourceType = typeof(Resources))]
        EMPLOYEE_CONTRIBUTION,
        [Display(Name = "Enum_PensionAttribute_COMPANY_CONTRIBUTION", ResourceType = typeof(Resources))]
        COMPANY_CONTRIBUTION,
        [Display(Name = "Enum_PensionAttribute_RETIREMENT_AGE", ResourceType = typeof(Resources))]
        RETIREMENT_AGE,
        [Display(Name = "Enum_PensionAttribute_PROBATION_DAY_LENGTH", ResourceType = typeof(Resources))]
        PROBATION_DAY_LENGTH,
        MILITARY_EMP_CONTRIBUTION,
        MILITARY_COMPANY_CONTRIBUTION,
    }
    public enum BonusAttribute
    {
        [Display(Name = "Enum_BonusAttribute_EMPLOYEE_MULTIPLIER", ResourceType = typeof(Resources))]
        MULTIPLIER
    }
    public enum BenefittAtribute
    {
        [Display(Name = "Enum_BenefittAtribute_TAXABLE", ResourceType = typeof(Resources))]
        TAXABLE,
        [Display(Name = "Enum_BenefittAtribute_NONTAXABLE", ResourceType = typeof(Resources))]
        NON_TAXABLE,
    }
    public enum DelegationBenefittAtribute
    {       
        ELIGIBILITY_DAYS,
        BASIC_SALARY_DIFFERENCE_PERCENT,
        ALLOWANCE_PERCENT,
    }
    public enum OverTimeAttribute
    {
        [Display(Name = "Enum_OverTimeAttribute_NIGHT_RATE", ResourceType = typeof(Resources))]
        NIGHT_RATE,
        [Display(Name = "Enum_OverTimeAttribute_NORMAL_RATE", ResourceType = typeof(Resources))]
        NORMAL_RATE,
        [Display(Name = "Enum_OverTimeAttribute_HOLIDAY_RATE", ResourceType = typeof(Resources))]
        HOLIDAY_RATE,
        [Display(Name = "Enum_OverTimeAttribute_RESTDAY_RATE", ResourceType = typeof(Resources))]
        RESTDAY_RATE,
        [Display(Name = "Enum_OverTimeAttribute_COEFFICIENT", ResourceType = typeof(Resources))]
        COEFFICIENT


    }
    public enum ProvidentAttribute
    {
        [Display(Name = "Enum_ProvidentAttribute_EMPLOYEE_CONTRIBUTION", ResourceType = typeof(Resources))]
        PV_EMPLOYEE_CONTRIBUTION,
        [Display(Name = "Enum_ProvidentAttribute_COMPANY_CONTRIBUTION", ResourceType = typeof(Resources))]
        PV_COMPANY_CONTRIBUTION,
        [Display(Name = "Enum_ProvidentAttribute_ADDITIONAL_EMPLOYEE_CONTRIBUTION", ResourceType = typeof(Resources))]
        ADDITIONAL_EMPLOYEE_CONTRIBUTION,
        [Display(Name = "Enum_ProvidentAttribute_ADDITIONAL_COMPANY_CONTRIBUTION", ResourceType = typeof(Resources))]
        ADDITIONAL_COMPANY_CONTRIBUTION,


    }
    public enum BooleanList
    {
        TRUE,
        FALSE

    }

    public enum MartialStatus
    {
        [Display(Name = "Enum_MartialStatus_SINGLE", ResourceType = typeof(Resources))]
        SINGLE = 10,
        [Display(Name = "Enum_MartialStatus_MARRIED", ResourceType = typeof(Resources))]
        MARRIED = 11,
        [Display(Name = "Enum_MartialStatus_SEPARATED", ResourceType = typeof(Resources))]
        SEPARATED = 12,
        [Display(Name = "Enum_MartialStatus_DIVORCED", ResourceType = typeof(Resources))]
        DIVORCED = 13,
        [Display(Name = "Enum_MartialStatus_OTHER", ResourceType = typeof(Resources))]
        OTHER = 14
        //  ,
        // [Display(Name = "Enum_MartialStatus_SINGLEF", ResourceType = typeof(Resources))]
        //SINGLEF,
        // [Display(Name = "Enum_MartialStatus_MARRIEDF", ResourceType = typeof(Resources))]
        //MARRIEDF,
        // [Display(Name = "Enum_MartialStatus_SEPARATEDF", ResourceType = typeof(Resources))]
        //SEPARATEDF,
        // [Display(Name = "Enum_MartialStatus_DIVORCEDF", ResourceType = typeof(Resources))]
        //DIVORCEDF

    }
    public enum EmployeeStatus
    {
        [Display(Name = "Enum_EmployeeStatus_NOTSELECTED", ResourceType = typeof(Resources))]
        NOTSELECTED = -1,
        [Display(Name = "Enum_EmployeeStatus_PERMANENT", ResourceType = typeof(Resources))]
        PERMANENT = 0,
        [Display(Name = "Enum_EmployeeStatus_CONTRACT", ResourceType = typeof(Resources))]
        CONTRACT = 1,
        [Display(Name = "Enum_EmployeeStatus_PROBATION", ResourceType = typeof(Resources))]
        PROBATION,
        FREELANCER,
        [Display(Name = "Enum_EmployeeStatus_NO_STATUS", ResourceType = typeof(Resources))]
        NO_STATUS,
        SPECIAL,
        TEMPORARY



    }
    public enum LeaveType
    {
        [Display(Name = "Enum_LeaveType_ANNUAL", ResourceType = typeof(Resources))]
        ANNUAL,
        [Display(Name = "Enum_LeaveType_MATERNITY", ResourceType = typeof(Resources))]
        MATERNITY,
        [Display(Name = "Enum_LeaveType_PATERNITY", ResourceType = typeof(Resources))]
        PATERNITY,
        [Display(Name = "Enum_LeaveType_MARRIAGE", ResourceType = typeof(Resources))]
        MARRIAGE,
        [Display(Name = "Enum_LeaveType_SICK_LEAVE", ResourceType = typeof(Resources))]
        SICK_LEAVE,
        [Display(Name = "Enum_LeaveType_OTHER", ResourceType = typeof(Resources))]
        OTHER,
        [Display(Name = "Enum_LeaveType_Leave_With_Out_Pay", ResourceType = typeof(Resources))]
        Leave_With_Out_Pay,
        [Display(Name = "Enum_LeaveType_Bereavement", ResourceType = typeof(Resources))]
        Bereavement,
        DayOff

    }

    public enum ReligionList
    {
        [Display(Name = "Enum_ReligionList_ORTHODOX", ResourceType = typeof(Resources))]
        ORTHODOX = 10,
        [Display(Name = "Enum_ReligionList_MUSLIM", ResourceType = typeof(Resources))]
        MUSLIM = 11,
        [Display(Name = "Enum_ReligionList_CATHOLIC", ResourceType = typeof(Resources))]
        CATHOLIC = 12,
        [Display(Name = "Enum_ReligionList_PROTESTANT", ResourceType = typeof(Resources))]
        PROTESTANT = 13,
        [Display(Name = "Enum_ReligionList_WAAQEFFATAA", ResourceType = typeof(Resources))]
        WAAQEFFATAA = 14,
        [Display(Name = "Enum_ReligionList_OTHER", ResourceType = typeof(Resources))]
        OTHER = 20

    }

    public enum Region
    {

        [Display(Name = "Enum_Region_TIGRAY", ResourceType = typeof(Resources))]
        TIGRAY = 15,
        [Display(Name = "Enum_Region_AFAR", ResourceType = typeof(Resources))]
        AFAR = 16,
        [Display(Name = "Enum_Region_AMHARA", ResourceType = typeof(Resources))]
        AMHARA = 17,
        [Display(Name = "Enum_Region_OROMO", ResourceType = typeof(Resources))]
        OROMIA = 18,
        [Display(Name = "Enum_Region_SOMALIA", ResourceType = typeof(Resources))]
        SOMALIA = 19,
        [Display(Name = "Enum_Region_SOUTH_NATION_AND_NATIONALITIES", ResourceType = typeof(Resources))]
        SOUTH_NATION_AND_NATIONALITIES = 20,
        [Display(Name = "Enum_Region_BENSHANGUL_GUMUZ", ResourceType = typeof(Resources))]
        BENSHANGUL_GUMUZ = 21,
        [Display(Name = "Enum_Region_HARRARI", ResourceType = typeof(Resources))]
        HARRARI = 22,
        [Display(Name = "Enum_Region_GAMBELLA", ResourceType = typeof(Resources))]
        GAMBELLA = 23,
        [Display(Name = "Enum_Region_ADDISABEBA", ResourceType = typeof(Resources))]
        ADDIS_ABEBA = 24,
        [Display(Name = "Enum_Region_DIREDAWA", ResourceType = typeof(Resources))]
        DIRE_DAWA = 25,
        [Display(Name = "Enum_Region_OTHER", ResourceType = typeof(Resources))]
        OTHER = 40


    }

    public enum TaxCategory
    {
        SALES_TAX,
        INCOME_TAX

    }

    public enum Proficiency
    {
        [Display(Name = "Enum_Proficiency_EXCELLENT", ResourceType = typeof(Resources))]
        EXCELLENT,
        [Display(Name = "Enum_Proficiency_VERY_GOOD", ResourceType = typeof(Resources))]
        VERY_GOOD,
        [Display(Name = "Enum_Proficiency_GOOD", ResourceType = typeof(Resources))]
        GOOD,
        [Display(Name = "Enum_Proficiency_POOR", ResourceType = typeof(Resources))]
        POOR

    }

    public enum LanguageType
    {
        [Display(Name = "Enum_LanguageType_INTERNATIONAL_LANGUAGE", ResourceType = typeof(Resources))]
        INTERNATIONAL_LANGUAGE,
        [Display(Name = "Enum_LanguageType_LOCAL_LANGUAGE", ResourceType = typeof(Resources))]
        LOCAL_LANGUAGE

    }






    public enum RecruitmentSource
    {
        Select,
        [Display(Name = "INTERNAL")]
        INTERNAL = 1,
        [Display(Name = "INTERNAL PROMOTION")]
        INTERNALPROMOTION = 2,
        [Display(Name = "INTERNAL TRANSFER")]
        INTERNALTRANSFER = 3,
        [Display(Name = "Enum_RecruitmentSource_EXTERNAL", ResourceType = typeof(Resources))]
        EXTERNAL = 4

    }
  
    public enum VacancyType
    {
        [Display(Name = "Enum_VacancyType_CONTRACT", ResourceType = typeof(Resources))]
        CONTRACT,
        [Display(Name = "Enum_VacancyType_PERMANENT", ResourceType = typeof(Resources))]
        PERMANENT

    }
    public enum ApplicationType
    {
        [Display(Name = "Enum_ApplicationType_PROMOTION", ResourceType = typeof(Resources))]
        PROMOTION,
        [Display(Name = "Enum_ApplicationType_TRANSFER", ResourceType = typeof(Resources))]
        TRANSFER

    }

    public enum CommunicationType
    {
        WEBSITE,
        EXHIBITION,
        TV,
        RADIO,
        SOCIALMEDIA,
        SPONSORSHIP,
        MEGAZIN

    }
    public enum InsuranceAttribute
    {
        [Display(Name = "Enum_InsuranceAttribute_JUDGE", ResourceType = typeof(Resources))]
        JUDGE,
        [Display(Name = "Enum_InsuranceAttribute_WIFE_OR_HUSBAND", ResourceType = typeof(Resources))]
        WIFE_OR_HUSBAND,
        [Display(Name = "Enum_InsuranceAttribute_CHILDREN", ResourceType = typeof(Resources))]
        CHILDREN,
        [Display(Name = "Enum_InsuranceAttribute_MINAGE", ResourceType = typeof(Resources))]
        MINIMUM_AGE,
        [Display(Name = "Enum_InsuranceAttribute_MAXAGE", ResourceType = typeof(Resources))]
        MAXIMUM_AGE
    }
    public enum AssociationType
    {
        [Display(Name = "Enum_AssociationType_WIFE", ResourceType = typeof(Resources))]
        WIFE,
        [Display(Name = "Enum_AssociationType_HUSBAND", ResourceType = typeof(Resources))]
        HUSBAND,
        [Display(Name = "Enum_AssociationType_CHILD", ResourceType = typeof(Resources))]
        CHILD,
        [Display(Name = "Enum_AssociationType_FATHER", ResourceType = typeof(Resources))]
        FATHER,
        [Display(Name = "Enum_AssociationType_MOTHER", ResourceType = typeof(Resources))]

        MOTHER,
        [Display(Name = "Enum_AssociationType_OTHER", ResourceType = typeof(Resources))]
        OTHER


    }
    public enum JobTitlePlan
    {
        [Display(Name = "Enum_JobTitlePlan_UNBOUNDED", ResourceType = typeof(Resources))]
        UNBOUNDED,
        [Display(Name = "Enum_JobTitlePlan_BOUNDED", ResourceType = typeof(Resources))]
        BOUNDED

    }
    public enum RequestStatus
    {
        [Display(Name = "Enum_RequestStatus_WAITING_FOR_APPROVAL", ResourceType = typeof(Resources))]
        WAITING_FOR_APPROVAL,
        [Display(Name = "Enum_RequestStatus_DEPARTMENT_APPROVAL", ResourceType = typeof(Resources))]
        DEPARTMENT_APPROVAL,
        [Display(Name = "Enum_RequestStatus_APPROVE", ResourceType = typeof(Resources))]
        APPROVE,
        [Display(Name = "Enum_RequestStatus_PEND", ResourceType = typeof(Resources))]
        PEND,
        [Display(Name = "Enum_RequestStatus_REJECT", ResourceType = typeof(Resources))]
        REJECT

    }
    public enum RecruitmentResult
    {
        [Display(Name = "Enum_RecruitmentResult_PASS", ResourceType = typeof(Resources))]
        PASS,
        [Display(Name = "Enum_RecruitmentResult_FAIL", ResourceType = typeof(Resources))]
        FAIL
    }
    public enum CommitteType
    {
        [Display(Name = "Enum_CommitteType_RECRUITMENT", ResourceType = typeof(Resources))]
        RECRUITMENT,
        [Display(Name = "Enum_CommitteType_OBSERVANT", ResourceType = typeof(Resources))]
        OBSERVANT,
        [Display(Name = "Enum_CommitteType_PROMOTION", ResourceType = typeof(Resources))]
        PROMOTION,
        [Display(Name = "Enum_CommitteType_PERFORMANCE", ResourceType = typeof(Resources))]
        PERFORMANCE,
        [Display(Name = "Enum_CommitteType_DISCIPLINE", ResourceType = typeof(Resources))]
        DISCIPLINE

    }
    public enum FixedContAmtType
    {
        [Display(Name = "Enum_FixedContAmtType_GROSS", ResourceType = typeof(Resources))]
        FROM_GROSS,
        [Display(Name = "Enum_FixedContAmtType_NET", ResourceType = typeof(Resources))]
        FROM_NET


    }
    public enum FixedContPecntType
    {
        [Display(Name = "Enum_FixedContAmtType_AMOUNT", ResourceType = typeof(Resources))]
        AMOUNT,
        [Display(Name = "Enum_FixedContAmtType_PERCENT", ResourceType = typeof(Resources))]
        PERCENT
    }
    public enum AdditionTaxType
    {
        [Display(Name = "Enum_AdditionTaxType_TAXABLE", ResourceType = typeof(Resources))]
        TAXABLE,
        [Display(Name = "Enum_AdditionTaxType_NON_TAXABLE", ResourceType = typeof(Resources))]
        NON_TAXABLE
    }
    public enum AccidentType
    {
        [Display(Name = "Enum_AccidentType_REVERSE", ResourceType = typeof(Resources))]
        REVERSE,

        [Display(Name = "Enum_AccidentType_CRASH", ResourceType = typeof(Resources))]
        CRASH
    }
    public enum BonusType
    {
        [Display(Name = "Enum_BonusType_WITH_PERFORMANCE", ResourceType = typeof(Resources))]
        WITH_PERFORMANCE,
        [Display(Name = "Enum_BonusType_From_PROFIT", ResourceType = typeof(Resources))]
        FROM_PROFIT
    }
    public enum AccidentLevel
    {
        [Display(Name = "Enum_AccidentLevel_HIGH", ResourceType = typeof(Resources))]
        HIGH,

        [Display(Name = "Enum_AccidentLevel_MEDIUM", ResourceType = typeof(Resources))]
        MEDIUM,
        [Display(Name = "Enum_AccidentLevel_LOW", ResourceType = typeof(Resources))]
        LOW
    }

    public enum InsuranceGroupType
    {
        Employee,
        PPE,
        Other
    }



    // new enums
    public enum TrainingScore
    {
        Very_High = 1,
        [Display(Name = "Enum_AccidentLevel_HIGH", ResourceType = typeof(Resources))]
        High = 2,
        [Display(Name = "Enum_AccidentLevel_MEDIUM", ResourceType = typeof(Resources))]
        Medium = 3,
        [Display(Name = "Enum_AccidentLevel_LOW", ResourceType = typeof(Resources))]
        Low = 4,
        Very_Low = 5
    }
    public enum OrganizationType
    {
        All = 0,
        NGO = 1,
        Govermental = 2,
        Private = 3
    }

    public enum GenderForVacancyGender
    {
        [Display(Name = "Enum_Male", ResourceType = typeof(Resources))]
        MALE = 5,
        [Display(Name = "Enum_Female", ResourceType = typeof(Resources))]
        FEMALE = 6,
        BOTH

    }
    public enum Languages
    {
        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Enum_Languages_Not_Required))]
        Not_Required = 1,
        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Enum_Languages_Amharic))]
        Amharic = 2,
        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Enum_Languages_Afaan_Oromoo))]
        Afaan_Oromoo = 3,
        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Enum_Languages_Tigrigna))]
        Tigrigna = 4,
        [Display(ResourceType = typeof(Resources), Name = nameof(Resources.Enum_Languages_Somali))]
        Somali = 5,
    }
    public enum ExperienceLevel
    {
        Fresh = 1,
        Junior = 2,
        Senior = 3
    }
    public enum TrainingRequestType
    {
        With_Plan,
        Without_Plan
    }
    public enum LetterLang
    {
        English,
        Amharic
    }
    public enum DataSource
    {
        Profile = 1,
        Placement = 2
    }
}
