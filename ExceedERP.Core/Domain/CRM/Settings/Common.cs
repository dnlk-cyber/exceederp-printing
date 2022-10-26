using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.CRM
{
    #region Commented Abstract Classes


    public class BankOrganizationUnitDefinition
    {
        public int BankOrganizationUnitDefinitionID { get; set; }
        public int BankID { get; set; }
        public string Description { get; set; }
        public string ParentId { get; set; }
        public string Specialization { get; set; }
        public string Abbrivation { get; set; }
        public string Remark { get; set; }
    }
    #endregion

 
   
    
   

  
    public class VoucherDefinition
    {
        public int VoucherDefinitionID { get; set; }
        public string Type { get; set; }

        public string Name { get; set; }

        public string Abbriviation { get; set; }

        public bool IsActive { get; set; }

        public string JournalType { get; set; }

        public string Remark { get; set; }
    }

   

    
   




  
    public class Range
    {
        public int RangeID { get; set; }
        public string Description { get; set; }
        public decimal Minimum { get; set; }

        public decimal Maximum { get; set; }

        public string Remak { get; set; }
    }
  
    public class User
    {
        public int UserID { get; set; }
        public bool IsActive { get; set; }
        public string Person { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Remark { get; set; }

    }

    public class LifeTime
    {
        public int LifeTimeID { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string Factor { get; set; }
        public double Value { get; set; }
        public string Remark { get; set; }

    }


  
    public class ThingCategoryDefaultAccount : TrackUserOperation
    {
        public int ThingCategoryDefaultAccountID { get; set; }

        public int ProductCategoryId { get; set; }
        public int ControlAccountID { get; set; }

        public int AccountID { get; set; }

        public string Remark { get; set; }
    }
    
  
   





  

   
   
   




    public class JournalTemplate
    {
        public int JournalTemplateID { get; set; }
        public string VoucherDefinition { get; set; }

        public string ControlAccount { get; set; }

        public bool IsDebit { get; set; }

        public string Remark { get; set; }


    }

    public class JournalDetail
    {
        public int JournalDetailID { get; set; }

        public string VoucherType { get; set; }
        public string Voucher { get; set; }

        public string Account { get; set; }

        public double Debit { get; set; }


        public double Credit { get; set; }

        public string Remark { get; set; }

    }


    public class Currency
    {
        public int CurrencyID { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }

        public string Abbrevation { get; set; }

        public bool IsDefault { get; set; }

        public string Remark { get; set; }

    }
    public class CurrencyPreference
    {
        public int CurrencyPreferenceID { get; set; }

        public string Currency { get; set; }

        public string Reference { get; set; }

        public string Remark { get; set; }


    }

  


 

    public class Religion
    {
        public int ReligionID { get; set; }

        public string Person { get; set; }

        public string Remark { get; set; }
    }
    public class ValueFactorDefinition
    {
        public int ValueFactorDefinitionID { get; set; }
        public string ValueFactorDefinitionType { get; set; }

        public string Description { get; set; }

        public bool IsPercent { get; set; }

        public double Value { get; set; }

        public string Remark { get; set; }

    }

    public class ValueFactor
    {
        public int ValueFactorID { get; set; }

        public string ReferenceType { get; set; }
        public string Reference { get; set; }

        public string ValueFactorDefinition { get; set; }

        public string Remark { get; set; }
    }

    [NotMapped]
    public class SalesQuoteVoucherDocument : SalesQuoteVoucherView
    {


    }





  
 


   


    public class Role : TrackUserOperation
    {
        public int RoleID { get; set; }

        public string Description { get; set; }


        public int? ParentID { get; set; }

        public string Remark { get; set; }
    }


    public class ObjectAccountRequirement : TrackUserOperation
    {
        public int ObjectAccountRequirementID { get; set; }

        public string ControlAccount { get; set; }

        public string ObjectType { get; set; }

    }


  
    public class BankAccountDetail : TrackUserOperation
    {
        public int BankAccountDetailID { get; set; }
        public string Reference { get; set; }

        public string Description { get; set; }

        public BankAccountType BankAccountDetailType { get; set; }

        public string Bank { get; set; }

        public string AccountNumber { get; set; }

        public string Remark { get; set; }
    }

  

    public class BinCardLineReserve
    {
        public int BinCardLineReserveID { get; set; }
        public int StockLevelID { get; set; }
        public DateTime Date { get; set; }

        public string VoucherDefinitionID { get; set; }

        public string VoucherID { get; set; }

        public decimal BalanceQTY { get; set; }

        public decimal RemainingBalanceQTY { get; set; }

        public decimal InActiveAfterDays { get; set; }

        public bool IsTemporal { get; set; }
        public string ReserveStatus { get; set; }
    }

    public enum HolidayType
    {
        RELIGIOUS,
        POLITICAL,
        NATIONAL,
        UNOFFICIAL,
        INTERNATIONAL_HOLIDAY
    }
    public class HolidayDefinition : TrackUserOperation
    {
        public int HolidayDefinitionID { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public HolidayType Type { get; set; }

        public bool IsFixed { get; set; }

        public bool WillClose { get; set; }


        public string Remark { get; set; }

    }

    public class CRMHoliday : TrackUserOperation
    {
        public int CRMHolidayID { get; set; }
        [DisplayName("Holiday Definition")]
        [Required]
        public int HolidayDefinitionID { get; set; }
        [DisplayName("Period")]
        [Required]
        public int PeriodID { get; set; }
        [Required]
        public string Remark { get; set; }
        public virtual HolidayDefinition HolidayDefinition { get; set; }
        public virtual Period Period { get; set; }


    }
    //public enum PeriodType
    //{
    //    PAYROLL_PERIOD,
    //    ACCOUNTING_PERIOD,
    //    HOLIDAY,
    //    SEASON,
    //    FISCAL_YEAR,
    //    QUARTER,
    //    SEMI_ANNUAL,
    //    WEEKLY,
    //    LEAVE_PERIOD


    //}

 
    public enum PlanType
    {
        RECRUITMENT
    }
    public enum CustomerType
    {
        LOCAL, FOREIGN
    }
    public enum PeriodStatusList
    {
        NEW,
        PROCESSING,
        CLOSED
    }
    public enum LetterType
    {
        INCOMING,
        OUTGOING



    }

    public enum LineItemStatus
    {
        PENDING,
        COMMITTED




    }
    public enum LetterOwner
    {
        ORGANIZATION,
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
        OTHER


    }

    public enum AccountRequirementDefinition
    {
        SALES_ACCOUNT,
        INVENTORY_ACCOUNT,
        COSTOFSALES_ACCOUNT,

    }

    public enum AddressType
    {
        WORK_PHONE,
        HOME_PHONE,
        MOBILE_PHONE_1,
        MOBILE_PHONE_2,
        FACEBOOK,
        TWITTER,
        LINKEDIN,
        SKYPE,
        REGION,
        CONTACT_PERSON,
        WORADA,
        HOUSE_NO,
        STREET,
        SPECIAL_LOCATION,
        OFFICE_EMAIL,
        PERSONAL_EMAIL,
        WEBSITE,
        COUNTRY,
        FAX,
        POBOX,
        CITYPROVINCE,
        KIFLEKETEMA,
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
        DISCOUNTED_PRICE,
        LOCATION_PRICE
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

    public enum RecieveMethodType
    {
        CASH,
        CHECK,
        CPO

    }
   
    public enum AllowanceType
    {
        TRANSPORT,
        TELEPHONE,
        HOME,
        POSITION,
        PROFESSIONAL,
        TOP_UP,
        FOOD



    }
    public enum OrganizationUnitDefinitionType
    {

        BRANCH,
        BOARD,
        DEPARTMENT,
        DIVISION,
        DIRECTORATE,
        HEAD_OFFICE,
        SECTION,
        TEAM,

        ROLE



    }
    public enum CommitmentType
    {
        EDUCATION,
        TRAINING

    }

    public enum PersonTitle
    {
        W_RO,
        ATO,
        CAPITAIN,
        PASTER,
        SHEK,
        LOWRET,
        PROFFESOR,
        BRIGADIER,
        GENERAL,
        CRENEL,
        AMBASADOR,
        SIR,
        MR,
        MAJOR,
        BISHOP,
        POPE,
        HISEXCELLENCY,
        HONORABLE,
        MRS,
        DR,
        ENGINEER,
        DIPLOMAT

    }
    public enum IdentificationType
    {
        RESIDENTIAL_ID,
        PENSION_NO,
        STUDENT_ID,
        WORK_ID,
        DRIVING_LICENSE,
        SMART_CARD,
        PASSPORT,
        VISA,
        BIOMETRIC_ID,
        TIN,
        VAT_NUMBER,
        LICENSE_NUMBER

    }

    public enum IndustryType
    {
        RETAIL,
        WHOLESALES,
        PHARMACY,
        HOSPITALITY,
        RESTAURANT,
        CAFEE,
        HOSPITAL,
        CLINIC,
        MANUFACTURING,
        AUTOMOTIVE,
        ELECTRONICS,
        CONSTRUCTION,
        REAL_STATE,
        BOOK_AND_PUBLICATION,
        GARMENT,
        FLORICULTURE


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
        GLASS,
        METER_SQUARE,
        SET,
        CENTI_METER,
        LITTER,
        ROLE,
        METER,
        PCS,
        KG,
        QUNTAL,
        BOTTEL,
        TONE

    }

    public enum StockBalanceType
    {
        CLOSING_BALANCE,
        MANUALCOUNT,
        INSPECTION_BALANCE,
        BEGINNING_BALANCE

    }

    public enum BankCategory
    {
        LOCAL_BANK,
        FOREIGN_BANK
    }

    public enum AssocationType
    {
        CREDIT,
        LABOUR

    }



    //public enum LookUpTypes //input for look up object-type attribute
    //{
    //    ADDITION_TYPE,
    //    BENEFITS,
    //    COURSE,
    //    COMMITTEE_TYPE,
    //    CONTRIBUTION_TYPE,
    //    DOCUMENT_TYPE,
    //    EDUCATION_LEVEL,
    //    EDUCATION_FIELD,
    //    JOB_TITLE,
    //    NATION,
    //    PROBATION_STATUS,
    //    RELATIONSHIP,
    //    TARGET_GROUP,
    //    TERMINATIONTYPE,
    //    TRAINING_TYPE,
    //    OVERTIME_TYPE,
    //    UOM,
    //    INSURANCE_CLASS_OF_BUSINESS,
    //    EVALUATION_TYPE,
    //    SPECIFICATION_CATEGORY_TYPE,
    //    BANK_TYPE,
    //    TERM_TYPE
    //}
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

    public enum LeaveSetting
    {
        LEAVE_FOR_LEADRES,
        LEAVE_FOR_PERFORMERS

    }

    public enum PayrollSettingCategories
    {
        PROVIDENT_FUND,
        PENSION,
        OVERTIME

    }
    public enum SettingCategories
    {
        VOUCHER_CONFIG


    }
    public enum PensionAttribute
    {
        EMPLOYEE_CONTRIBUTION,
        COMPANY_CONTRIBUTION,
        RETIREMENT_AGE


    }
    public enum OverTimeAttribute
    {
        NIGHT_RATE,
        NORMAL_RATE,
        HOLIDAY_RATE,
        RESTDAY_RATE,
        COEFFICIENT


    }
    public enum ProvidentAttribute
    {
        EMPLOYEE_CONTRIBUTION,
        COMPANY_CONTRIBUTION,
        ADDITIONAL_EMPLOYEE_CONTRIBUTION,
        ADDITIONAL_COMPANY_CONTRIBUTION,


    }
    public enum PriceSelection
    {
        PRICE_SELECTION
    }
    public enum PriceSelectionValue
    {
        FLEXIBLE,
        FIXED
    }
    public enum BooleanList
    {
        TRUE,
        FALSE

    }

    public enum MartialStatus
    {
        SINGLE,
        MARRIED,
        SEPARATED,
        DIVORCED


    }
    

   

    
    public enum Region
    {

        TIGRAY,
        AFAR,
        AMHARA,
        OROMO,
        SOMALIA,
        SOUTH_NATION_AND_NATIONALITIES,
        BENSHANGUL_GUMUZ,
        HARRARI,
        GURAGE,
        GAMBELLA

    }

    public enum TaxCategory
    {
        SALES_TAX,
        INCOME_TAX

    }

    public enum Proficiency
    {
        EXCELLENT,
        VERY_GOOD,
        GOOD,
        POOR

    }

    public enum LanguageType
    {
        INTERNATIONAL_LANGUAGE,
        LOCAL_LANGUAGE

    }
    
    
    public enum ApplicationType
    {
        PROMOTION,
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
    public enum TermCategory
    {
        TERMS_OF_DELIVERY,
        TERMS_OF_VALIDITY,
        TERMS_OF_WARRANTY,
        TERMS_OF_PAYMENT
    }

    public enum ContinentType
    {
        EUROPE,
        AFRICA,
        SOUTH_AMERICA,
        ASIA,
        AUSTRALIA,
        NORTH_AMERICA,
        ANTARCTICA,
        OCEANIA
    }

    public enum ReserveStatus
    {
        WAITING,
        CLOSED
    }
    public class Reference : TrackUserOperation
    {

        public string Code { get; set; }

        public string Description { get; set; }

    }
}
