using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Construction
{
    class ConstructionEnum
    {
    }
    public enum ConstructionSettingCategories
    {
        OVER_HEAD_COST_RATE,
        PROFIT_COST_RATE,
        INDEX_FaCTOR,
        //TRANSPORTATION_COST_BIRR_LITTER,
        //DISTRIBUTION_COST_BIRR_LITTER,
        STRUCTURE_TYPE,
        WORKING_DAYS_PER_YEAR,
    }
    public enum CostType
    {
        DIRECT_MATERIAL,
        DIRECT_MANPOWER,
        DIRECT_EQUIPMENT,
        OVERHEAD_COST
    }

    public enum TypeOfConstruction
    {
        MAIN_CONSTRUCTION_WORK, ADDITIONAL_WORK
    }
    public enum Days { SAT, SUN, MON, TUE, WED, THU, FRI };
    public enum ProjectCategory { CONTRACT_PROJECT, SUB_CONTRACT_PROJECT, DIRECT_PROJECT };
    public enum ProjectTypes { MAIN_PROJECT, BLOCK };
    public enum RiskStatus
    {
        OPEN, CLOSE
    }

    public enum RiskImpact
    {
        HIGH, LOW, MEDIUM
    }

    public enum RiskProbablity
    {
        HIGH, LOW, MEDIUM
    }
    public enum ProjectStatus
    {
        ESTIMATION,
        WAITING_APPROVAL,
        APPROVED,
        STARTED,
        CLOSED


    }

    public enum TakeOffSheetType
    {
        TAKEOFF_SHEET_ONE,
        TAKEOFF_SHEET_TWO
    }

    public enum VariationRequestPriority
    {
        Critical,
        High,
        Medium,
        Low
    }
    public enum VariationRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public enum ContractClaimRequestStatus
    {
        Pending,
        Granted,
        Denied
    }
    public enum Months
    {

        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec,
        Jan,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
    }
    public enum MachineryFuelTypes
    {
        Gasoil,
        Benzin
    }
    public enum EarnedValueManagementTermCategory
    {
        Input,
        Indicator,
        Predictor
    }
    public enum EVMOperation
    {
        Multiplication,
        Division,
        Subtraction,
        Addition
    }
    public enum EVMParentOperand
    {
        OperandOne,
        OperandTwo
    }
    public enum ProjectQualityComplaintSource
    {
        Client,
        Engineers,
        Head_Office,
        Quality_Engineer,
        Other
    }
    public enum ProjectPlanScheduleType
    {
        Work_Schedule,
        Resource_Schedule,
        Cash_Flow_Schedule,
        Budget_Requirement_Schedule
    }
    //public enum MonthlyReportReportingTime
    //{
    //    Before_Due_Date,
    //    On_Due_Date,
    //    After_Due_Date
    //}
    //public enum MonthlyReportCompletenessAttributes
    //{
    //}

    public enum MonthlyProjectCommunicationLevel
    {
        Very_Good,
        Good,
        Not_Good,
        Poor
    }
    public enum MonthlyProjectCommunicationMethod
    {
        Email,
        Telephone,
        Written,
        Other
    }
    public enum PerformanceModifierInjuryLevel
    {
        Heavy,
        Medium,
        Light
    }
    public enum MonthlyDueDate
    {
        On_Due_Date,
        Before_Due_Date,
        After_Due_Date
    }
    public enum RevisedScheduleApprovalStatus
    {
        Approved_Without_Correction,
        Approved_After_One_Correction,
        Approved_After_Two_Correction,
        Approved_After_Many_Correction
    }
    public enum AnalysisTechniqueForMonthlyClaims
    {
        Appropriate_Delay_Analysis_Technics,
        Delay_Analysis_Technic_Applied,
        No_Delay_Analysis_Technic_Applied
    }
    public enum MonthlyClaimRequestApprovalStatus
    {
        Above_80Percent_Approved,
        Above_60Percent_Approved,
        Above_50Percent_Approved,
        Below_50Percent_Approved,

    }

    public enum PerformanceEvaluationComponentWeightSetting
    {
        PROGRESS,
        PROGRESS_MONTHLY,
        PROGRESS_TODATE,
        QUALITY,
        COST,
        PAYMENT,
        PAYMENT_ACCOMPLISHMENT,
        PAYMENT_PRICE_ADJUSTMENT,
        PAYMENT_MATERIAL_ON_SITE,
        PAYMENT_APPROVAL_TIME,
        PLANNING,
        CLAIM,
        REPORT,
        COMMUNICATION,
        MODIFIERS,
        MODIFIER_HEALTH_AND_SAFETY,
        MODIFIER_BUDGET_UTILIZATION,
        MODIFIER_MATERIAL_UTILIZATION,
        MODIFIER_SUBCONTRACT_ADMINISTRATION
    }

    public enum WorkDelayTypes
    {
        NONEXCUSABLE_NONCOMPENSABLE,
        EXCUSABLE_COMPANSABLE,
        EXCUSABLE_NONCOMPENSABLE
    }
    public enum ProjectPerformanceEvaluationComponents
    {
        PROGRESS,
        PROGRESS_MONTHLY,
        PROGRESS_TODATE,
        QUALITY,
        COST,
        PAYMENT,
        PAYMENT_ACCOMPLISHMENT,
        PAYMENT_PRICE_ADJUSTMENT,
        PAYMENT_MATERIAL_ON_SITE,
        PAYMENT_APPROVAL_TIME,
        PLANNING,
        CLAIM,
        REPORT,
        COMMUNICATION,
        MODIFIERS,
        MODIFIER_HEALTH_AND_SAFETY,
        MODIFIER_BUDGET_UTILIZATION,
        MODIFIER_MATERIAL_UTILIZATION,
        MODIFIER_SUBCONTRACT_ADMINISTRATION
    }
    public enum PerformanceModifierPRPImplementationLevel
    {
        NotImplemented,
        ImplementedNotOrganizedNoResult,
        ImplementedAndOrganizedNoResult,
        ImplementedAndOrganizedWithResultButPoorReporting
    }
    public enum ProjectPaymentAttributeName
    {
        Retention,
        Advance_Payment_Rate,
        Project_Percentage_for_Complete_Advance_Repayment
    }
    public enum ProjectScheduleStatus
    {

        New = 0,
        Approved = 1,
        Rejected = 2,
        WaitingApproval = 3,
        Editing = 4,
        Void = 5,

    }
    public enum AvailableProjectTypes
    {
        Building,
        Road,
        Dam,
        Irrigation
    }
    public enum ProjectSettingAttributes
    {
        [Description("Overhead Cost Rate")]
        Overhead_Cost_Rate,
        //LiabilityPeriod,
        [Description("Expected Profit Rate")]
        ExpectedProfitRate,
        [Description("Transport Cot Rate")]
        TransportCostRate,
        //AverageDistanceOfProjectCenterFromAddisAbaba,
        //AverageDistanceOfProjectCenterFromMaterialSource
    }

    public enum WorkInProgressStatus
    {
        Complete,
        Partial
    }

    public enum ConstructionTransactionClass
    {
        Invoice,
        Credit_Memo,
        Debit_Memo
    }

    public enum ConstructionInvoiceStatus
    {
        NotCollectted = 0,
        PartiallyCollected = 1,
        Collected = 2
    }

    public enum InvoiceEditabilityStatus
    {
        Editing,
        Close
    }

    public enum ConstructionInvoiceSource
    {
        WIP,
        Manual
    }

    public enum OperationStatuses
    {
        New = 0,
        Approved = 1,
        Declined = 2,
        Issued = 3,
        WaitingApproval = 4,
        Editing = 5,
        SenderApproved = 6,
        ReceiverApproved = 7,
        Rejected = 8,
        WaitingSecondApproval = 9,
        Void = 10,
        SenderVoid = 11,
        ReceiverVoid = 12,
        WaitingMiddleApproval = 13,
        SendForPayment = 14,
    }

    public enum StandardCollectionReceiptType
    {
        Invoice,
        Prepayment
    }
    public enum VariationOrderTypeCategory
    {
        MainContract,
        SubContract
    }
    public enum PEVCostRateSettingAttribute
    {
        PlannedDirectCostRate,
        PlannedProjectOHCostRate,
        PlannedProfitAndHeadOfficeOHCostRate
    }

    public enum MaterialTransportCostAttriute
    {
        DrivingHourFromAAToSource,
        DrivingHourFromQuarryToSite,
        AverageDriverSalaryPerMonth,
        AverageDriverBenefitPerDay,
        AverageDriverAssistantSalaryPerMonth,
        AverageDriverAssistantBenefitPerDay,
        AAToSourceInKm,
        VehicleCapacity
    }
    public enum ConstructionCustomerAccountTypes
    {
        WorkInProgress,
        Penality,
        AdvancePayment,
        ReceivableAccount,
        Retention,
        Others
    }
    public enum WorkItemCostNegotiationTypes
    {
        PP,
        PS,
        LS,
        CC
    }
    public enum RepresentativeElementIndexType
    {
        Material,
        Equipment,
        Labor,
        NonAdjustable,
        Other
    }
    public enum PriceAdjustmentFormulaType
    {
        TwoThousandSix = 2006,
        TwoThousandEleven = 2011
    }
    public enum WorkingHourType
    {
        Morning = 0,
        Afternoon = 1
    }
    public enum ControllerActionType
    {
        Read = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }
    public enum AgingPeriodTimeUnit
    {
        Day,
        Month,
        Year
    }
    public enum DurationItemSource
    {
        Estimation,
        Manual
    }
}
