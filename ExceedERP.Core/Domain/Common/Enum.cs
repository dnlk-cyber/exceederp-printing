using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.Common
{
    public enum FinalizeType
    {
        WAITING,
        ASSIGNED,
        RESOLVED,
        CLOSED

    }
    public enum RetireSetting
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
        የሥራ_ልምድ,
        የትብብር_ደብዳቤ,
        የዋስትና_ደብዳቤ
    }
    public enum LeaveDay
    {
        ሙሉ_ቀን,
        ግማሽ_ቀን
    }

    public enum MorningOrAfternoon
    {
        ጥዋት,
        ከሰዓት
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
        REVERSING
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
        TRANSFERED





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
        PROJECT_DOCUMENT


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
        MALE,
        [Display(Name = "Enum_Female", ResourceType = typeof(Resources))]
        FEMALE

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
        RESPONSIBILITY


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
        PROVIDENT_FUND

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
        PROBATION_MEASUREMENT


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
        LEAVE_FOR_LEADRES_NEW,
        LEAVE_FOR_PERFORMERS_NEW,
        Old_Initial_Leave_End_Year

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
        DEDUCT_LEAVE_WITHOUT_PAYMENT
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
        PROBATION_DAY_LENGTH
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
        SINGLE,
        [Display(Name = "Enum_MartialStatus_MARRIED", ResourceType = typeof(Resources))]
        MARRIED,
        [Display(Name = "Enum_MartialStatus_SEPARATED", ResourceType = typeof(Resources))]
        SEPARATED,
        [Display(Name = "Enum_MartialStatus_DIVORCED", ResourceType = typeof(Resources))]
        DIVORCED,
        [Display(Name = "Enum_MartialStatus_OTHER", ResourceType = typeof(Resources))]
        OTHER
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
        [Display(Name = "Enum_EmployeeStatus_PERMANENT", ResourceType = typeof(Resources))]
        PERMANENT,
        [Display(Name = "Enum_EmployeeStatus_CONTRACT", ResourceType = typeof(Resources))]
        CONTRACT,
        [Display(Name = "Enum_EmployeeStatus_PROBATION", ResourceType = typeof(Resources))]
        PROBATION,
        FREELANCER,
        [Display(Name = "Enum_EmployeeStatus_NO_STATUS", ResourceType = typeof(Resources))]
        NO_STATUS
      


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
        Bereavement

    }

    public enum ReligionList
    {
        [Display(Name = "Enum_ReligionList_ORTHODOX", ResourceType = typeof(Resources))]
        ORTHODOX,
        [Display(Name = "Enum_ReligionList_MUSLIM", ResourceType = typeof(Resources))]
        MUSLIM,
        [Display(Name = "Enum_ReligionList_CATHOLIC", ResourceType = typeof(Resources))]
        CATHOLIC,
        [Display(Name = "Enum_ReligionList_PROTESTANT", ResourceType = typeof(Resources))]
        PROTESTANT,
        [Display(Name = "Enum_ReligionList_WAAQEFFATAA", ResourceType = typeof(Resources))]
        WAAQEFFATAA,
        [Display(Name = "Enum_ReligionList_OTHER", ResourceType = typeof(Resources))]
        OTHER

    }
    public enum Region
    {
       
        [Display(Name = "Enum_Region_TIGRAY", ResourceType = typeof(Resources))]
        TIGRAY,
        [Display(Name = "Enum_Region_AFAR", ResourceType = typeof(Resources))]
        AFAR,
        [Display(Name = "Enum_Region_AMHARA", ResourceType = typeof(Resources))]
        AMHARA,
        [Display(Name = "Enum_Region_OROMO", ResourceType = typeof(Resources))]
        OROMO,
        [Display(Name = "Enum_Region_SOMALIA", ResourceType = typeof(Resources))]
        SOMALIA,
        [Display(Name = "Enum_Region_SOUTH_NATION_AND_NATIONALITIES", ResourceType = typeof(Resources))]
        SOUTH_NATION_AND_NATIONALITIES,
        [Display(Name = "Enum_Region_BENSHANGUL_GUMUZ", ResourceType = typeof(Resources))]
        BENSHANGUL_GUMUZ,
        [Display(Name = "Enum_Region_HARRARI", ResourceType = typeof(Resources))]
        HARRARI,
        [Display(Name = "Enum_Region_GAMBELLA", ResourceType = typeof(Resources))]
        GAMBELLA,
        [Display(Name = "Enum_Region_ADDISABEBA", ResourceType = typeof(Resources))]
        ADDIS_ABEBA,
        [Display(Name = "Enum_Region_DIREDAWA", ResourceType = typeof(Resources))]
        DIRE_DAWA,
        [Display(Name = "Enum_Region_OTHER", ResourceType = typeof(Resources))]
        OTHER
        

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
        [Display(Name = "Enum_RecruitmentSource_INTERNAL", ResourceType = typeof(Resources))]
        INTERNAL,
        [Display(Name = "Enum_RecruitmentSource_EXTERNAL", ResourceType = typeof(Resources))]
        EXTERNAL

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


}
