using System.Collections.Generic;

namespace ExceedERP.Reporting.Construction.ViewModel
{
    public class PEvProjectGeneralInfoVM
    {
        public string ContractSigningDate { get; set; }
        public decimal? ContractPeriod { get; set; }
        public string CommencementDate { get; set; }
        public decimal? ApprovedTimeExtension { get; set; }
        public decimal? TotalProjectPeriod { get; set; }
        public string OriginalcompletionTime { get; set; }
        public string RevisedCompletionDate { get; set; }
        public decimal? TimeElapsedToDate { get; set; }
        public decimal TimeElapsedTodatePercentage { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal? Rebate { get; set; }
        public decimal? Variation { get; set; }
        public decimal? Supplemetary { get; set; }
        public decimal? TotalProjectAmount { get; set; }
        public decimal? NetGrand { get; set; }
        public decimal? ExecutedToDate { get; set; }
        public decimal ExecutedToDatePercentage { get; set; }
    }
    public class PeriodVeiwModel
    {

        public PeriodVeiwModel()
        {
            GeneralInfoList = new List<PEvProjectGeneralInfoVM>();
            ProgressList = new List<MonthlyProgressViewModel>();
            QualityList = new List<MonthlyQualityViewModel>();
            CostList = new List<MonthlyCostViewModel>();
            PaymentList = new List<MonthlyPaymentViewModel>();
            PlanningList = new List<MonthlyPlanningViewModel>();
            ClaimList = new List<MonthlyClaimViewModel>();
            ReportList = new List<MonthlyReportViewModel>();
            CommuncationList = new List<MonthlyCommunicationViewModel>();
            ModifierList = new List<MonthlyPerformanceModifierVM>();

            PEvComponentsSummaryList = new List<PerformanceEvaluationComponentsSummaryViewModel>();
            PEvModifiersSummaryList = new List<PerformanceEvaluationModifiersSummaryViewModel>();
        }

        public string Name { get; set; }
        public string FromPeriod { get; set; }
        public string ToPeriod { get; set; }

        public List<PEvProjectGeneralInfoVM> GeneralInfoList { get; set; }
        public List<MonthlyProgressViewModel> ProgressList { get; set; }
        public List<MonthlyQualityViewModel> QualityList { get; set; }
        public List<MonthlyCostViewModel> CostList { get; set; }
        public List<MonthlyPaymentViewModel> PaymentList { get; set; }
        public List<MonthlyPlanningViewModel> PlanningList { get; set; }
        public List<MonthlyClaimViewModel> ClaimList { get; set; }
        public List<MonthlyReportViewModel> ReportList { get; set; }
        public List<MonthlyCommunicationViewModel> CommuncationList { get; set; }
        public List<MonthlyPerformanceModifierVM> ModifierList { get; set; }

        // for summary
        public List<PerformanceEvaluationComponentsSummaryViewModel> PEvComponentsSummaryList { get; set; }
        public List<PerformanceEvaluationModifiersSummaryViewModel> PEvModifiersSummaryList { get; set; }

    }
    public class MonthlyProgressViewModel
    {
        public decimal PlannedAmount { get; set; }
        // %
        public decimal PlannedAmountPercentage { get; set; }
        public decimal PlannedAmountTillPreviousMonth { get; set; }
        // %
        public decimal PlannedAmountTillPreviousMonthPercentage { get; set; }

        public decimal PlannedToDate { get; set; }
        // %
        public decimal PlannedToDatePercentage { get; set; }
        public decimal ExecutedAmount { get; set; }
        // %
        public decimal ExecutedAmountPercentageFromContract { get; set; }
        public decimal ExecutedAmountPercentageFromPlan { get; set; }
        // performance of excuted this month vs plan --- for road projects
        public decimal MonthlyExecutedVsPlanPerformance { get; set; }

        public decimal ExecutedAmountTillPreviousMonth { get; set; }
        // %
        public decimal ExecutedAmountTillPreviousMonthPercentageFromContract { get; set; }
        public decimal ExecutedAmountTillPreviousMonthPercentageFromPlan { get; set; }

        public decimal ExecutedAmountToDate { get; set; }
        // %
        public decimal ExecutedAmountToDatePercentageFromContract { get; set; }
        public decimal ExecutedAmountToDatePercentageFromPlan { get; set; }
        // performance of excuted todate vs plan ----  for road projects
        public decimal TodateExecutedVsPlanPerformance { get; set; }

        public string ScheduleBrokenDownToWeeklyBase { get; set; }
        public string AreFormenAwareOfIt { get; set; }

        public string ActionPlanIsPreparedWithAmountAndProblemsAreAddressed { get; set; }
        public string WellDefinedTeamStructureExists { get; set; }

        public int NumberOfEvaluationMeething { get; set; }
        public decimal CorrectionFactor { get; set; }
    }
    public class MonthlyQualityViewModel
    {

        public MonthlyQualityViewModel()
        {
            QPlanningList = new List<QualityPlanningViewModel>();
            QAssuranceList = new List<QualityAssuranceViewModel>();
            QControlList = new List<QualityControlViewModel>();
            QComplaintList = new List<QualityComplaintsViewModel>();
        }
        public decimal MaxAllowablePoint { get; set; }
        public decimal TotalPoints { get; set; }

        public decimal QualityPlanning { get; set; }
        public decimal QualityAssurance { get; set; }
        public decimal QualityControl { get; set; }
        public int QualityComplaint { get; set; }
        public decimal MaxAllowablePoints { get; set; }

        // quality planning sub-components
        public decimal DoPersonnelKnowTheQualityRequirements { get; set; }
        public decimal DoPersonnelKnowHowToMeetQualityRequirements { get; set; }

        // quality assurance sub-components
        public decimal MaterialTestsPerformed { get; set; }
        public decimal EvaluationOfMethod { get; set; }
        public decimal OverAllEvaluationOfLabour { get; set; }

        // quality control sub-components
        public decimal NumberOfTestsPerformedOnExecutedWorks { get; set; }
        public decimal ExtentOfInspectionPerformedOnExecutedWorks { get; set; }

        // quality complaints sub-components
        public int FromClient { get; set; }
        public int FromEngineers { get; set; }
        public int FromHeadOffice { get; set; }
        public int FromQualityEngineer { get; set; }

        public List<QualityPlanningViewModel> QPlanningList { get; set; }
        public List<QualityAssuranceViewModel> QAssuranceList { get; set; }
        public List<QualityControlViewModel> QControlList { get; set; }
        public List<QualityComplaintsViewModel> QComplaintList { get; set; }
    }
    public class MonthlyCostViewModel
    {
        public decimal PlannedDirectCost { get; set; }
        public decimal PlannedProjectOHCost { get; set; }
        public decimal PlannedProfitAndOHCost { get; set; }
        public decimal ActualDirectCost { get; set; }
        public decimal ActualProjectOHCost { get; set; }
        public decimal ActualProfitAndOHCost { get; set; }
        public decimal PlannedDirectCostToDate { get; set; }
        public decimal PlannedProjectOHCostToDate { get; set; }
        public decimal PlannedProfitAndOHToDate { get; set; }
        public decimal ActualDirectCostToDate { get; set; }
        public decimal ActualProjectOHCostToDate { get; set; }
        public decimal ActualProfitAndOHToDate { get; set; }
        public decimal CostTracking { get; set; }

        // resource utilization (road projects)
        public decimal EquipmentUtilizationEfficiency { get; set; }
        public string IsFuelUtilizedBasedOnStandard { get; set; }
        // mark
        public decimal FuelUtilizationBasedOnStandardMarks { get; set; }
        public string IsProperAndAccurateEarnedValueAnalysisUsed { get; set; }
        // mark
        public decimal ProperAndAccurateEarnedValueAnalysisMarks { get; set; }
        public string IsProductivitiyOfLaborAndEquipmentRecorded { get; set; }
        // mark
        public decimal ProductivityOfLaborAndEquipmentMarks { get; set; }
        public string IsReusingOfConstructionMaterialsApplied { get; set; }
        // mark
        public decimal ReusingOfConstructionMaterialsMark { get; set; }
        public decimal EquipmentUtilizationPerformance { get; set; }
        // mark
        public decimal CostFromExpectationMarks { get; set; }
        //

        #region Percentage of each values
        public decimal PlannedDirectCostPercentage { get; set; }
        public decimal PlannedProjectOHCostPercentage { get; set; }
        public decimal PlannedProfitAndOHCostPercentage { get; set; }
        public decimal ActualDirectCostPercentage { get; set; }
        public decimal ActualProjectOHCostPercentage { get; set; }
        public decimal ActualProfitAndOHCostPercentage { get; set; }
        public decimal PlannedDirectCostToDatePercentage { get; set; }
        public decimal PlannedProjectOHCostToDatePercentage { get; set; }
        public decimal PlannedProfitAndOHToDatePercentage { get; set; }
        public decimal ActualDirectCostToDatePercentage { get; set; }
        public decimal ActualProjectOHCostToDatePercentage { get; set; }
        public decimal ActualProfitAndOHToDatePercentage { get; set; }
        #endregion
    }
    public class MonthlyPaymentViewModel
    {
        //public MonthlyPaymentViewModel()
        //{
        //    ApprovalTimeList = new List<PaymentApprovalTimeViewMoedl>();
        //}
        public decimal ApprovedTillPreMonth { get; set; }
        public decimal ExpectedToBeApprovedThisMonth { get; set; }
        public decimal AmountApprovedThisMonth { get; set; }
        public decimal? AmounNotApprovedThisMonth { get; set; }
        public decimal ApprovedToDate { get; set; }
        public decimal AmountNotApprovedToDate { get; set; }
        public decimal? PriceAdjustmentTillPreMonth { get; set; }
        public decimal? PriceAdjustmentThisMonth { get; set; }
        public decimal? PriceAdjustmentApprovedToDate { get; set; }

        public decimal? MatOnSiteThisMonth { get; set; }
        public decimal? MatOnSiteApprovedThisMonth { get; set; }
        public decimal? MatOnSiteNotAppprovedThiMonth { get; set; }

        public string ApprovalTime { get; set; }
        //public decimal? Days { get; set; }
        public decimal Points { get; set; }
        //public List<PaymentApprovalTimeViewMoedl> ApprovalTimeList { get; set; }
    }
    public class PaymentApprovalTimeViewMoedl
    {
        public string ApprovalTimeConcatnated { get; set; }
        public decimal Points { get; set; }
    }
    public class MonthlyPlanningViewModel
    {
        public MonthlyPlanningViewModel()
        {
            RvAcToContractList = new List<RevisedScheduleAcToContractViewMode>();
            RvAcToManualList = new List<RevisedScheduleAcToManualViewModel>();
            CompletnessList = new List<PlanningCompletnessViewModel>();
            QualityAndAcceptabilityList = new List<PlanningQualityAndAcceptablityViewModel>();
        }
        public int Id { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal? TotalPoints { get; set; }


        public decimal RevScheduleSubAcToManual { get; set; }
        public decimal RevScheduleSubAcToContract { get; set; }
        public decimal Completness { get; set; }
        public decimal QualityAndAcceptablity { get; set; }

        // rev according to manual
        public string IsRevisionTimeAccManual { get; set; }
        public string RevisedSdlSubmitedAccordinglyAccToManual { get; set; }
        public string SubmssinTimeAcctoManual { get; set; }
        public int DaysAfterOrBeforeDueDateAccToManual { get; set; }

        // rev according to contrat
        public string IsRevisionTimeAccContract { get; set; }
        public string RevisedSdlSubmitedAccordinglyAccToContract { get; set; }
        public string SubmssinTimeAcctoContract { get; set; }
        public int DaysAfterOrBeforeDueDateAccToContract { get; set; }

        // completness
        public decimal WorkSchedule { get; set; }
        public decimal ResourceSchedule { get; set; }
        public decimal CashFlowSchedule { get; set; }
        public decimal BudgetRequirementSchedule { get; set; }

        // acceptablity and ..
        public string IsApprovalMadeInTheMonth { get; set; }
        public string ApprovalType { get; set; }

        public List<RevisedScheduleAcToManualViewModel> RvAcToManualList { get; set; }
        public List<RevisedScheduleAcToContractViewMode> RvAcToContractList { get; set; }
        public List<PlanningCompletnessViewModel> CompletnessList { get; set; }
        public List<PlanningQualityAndAcceptablityViewModel> QualityAndAcceptabilityList { get; set; }
    }
    public class MonthlyClaimViewModel
    {
        public MonthlyClaimViewModel()
        {
            ClaimContempraryRecordList = new List<ClaimContempraryRecordViewModel>();
            ClaimRequirementList = new List<ClaimRequirementViewModel>();
            ClaimAnalysisTechniqueList = new List<ClaimAnalysisTechniqueViewModel>();
            ClaimApprovalStatusList = new List<ClaimApprovalStatusViewModel>();
        }
        public int Id { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
        public decimal ContempraryRecordKeeping { get; set; }
        public decimal FullFillingRequirementForClaims { get; set; }
        public decimal AnalysisTechnique { get; set; }
        public decimal ApprovalStatusOfClaims { get; set; }


        // contemprary record
        public decimal DailylyRecords { get; set; }

        // req ...
        public string IsClaimPreparedAndIssuedInTheMonth { get; set; }
        public decimal SubstantiveRequirementRefClauses { get; set; }
        public decimal ProceduralRequirementDocumentation { get; set; }
        public decimal ProofRequirementDocumentation { get; set; }

        // analysis ..
        public string WasThereissuetoClaim { get; set; }
        public string AnalysisType { get; set; }

        // approval status
        public string IsApprovalMadInTheMonth { get; set; }
        public string ClaimApprovalStatus { get; set; }


        public List<ClaimContempraryRecordViewModel> ClaimContempraryRecordList { get; set; }
        public List<ClaimRequirementViewModel> ClaimRequirementList { get; set; }
        public List<ClaimAnalysisTechniqueViewModel> ClaimAnalysisTechniqueList { get; set; }
        public List<ClaimApprovalStatusViewModel> ClaimApprovalStatusList { get; set; }

    }
    public class MonthlyReportViewModel
    {
        public MonthlyReportViewModel()
        {
            ReportCompletnessList = new List<ReportCompletnessViewModel>();
            ReportNarrativeList = new List<ReportNarrativeViewModel>();
        }


        public decimal Completeness { get; set; }
        public decimal Narrative { get; set; }
        public decimal MaxAllowablePoints { get; set; }
        public decimal TotalPoints { get; set; }

        // reporting time
        public string ReportingTime { get; set; }
        public decimal? Days { get; set; }
        public decimal Points { get; set; }


        // completness
        public decimal Progress { get; set; }
        public decimal QualityControl { get; set; }
        public decimal HealthAndSafety { get; set; }
        public decimal EarnedValueAnalysis { get; set; }
        public decimal MaterialUtilization { get; set; }
        public decimal LabourUtilization { get; set; }
        public decimal EquipmentUtilization { get; set; }
        public decimal FuelUtilization { get; set; }

        // narratives
        public decimal ProperIdentificationOfProblems { get; set; }


        public List<ReportCompletnessViewModel> ReportCompletnessList { get; set; }
        public List<ReportNarrativeViewModel> ReportNarrativeList { get; set; }
    }
    public class MonthlyCommunicationViewModel
    {
        public MonthlyCommunicationViewModel()
        {
            ComnWithinProjectList = new List<ComnWithinProjectViewModel>();
            ComnWithHeadOfficeList = new List<ComnWithHeadOfficeViewModel>();
            ComnWithConsultantList = new List<ComnWithConsultantViewModel>();
        }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal TotalPoints { get; set; }

        public decimal ComnWithinProject { get; set; }
        public decimal ComnWithHeadOffice { get; set; }
        public decimal ComnWithConsultant { get; set; }
        public int NumberOfComplaints { get; set; }
        public decimal GovernancePoints { get; set; }

        // within project
        public decimal CloseDiscussionWithinProject { get; set; }
        public decimal MobilizationAndDisscusstionWthinWorkers { get; set; }
        public decimal CommunicationAndSupportBetweenProjects { get; set; }

        // with head office
        public decimal WrittenComn { get; set; }
        public decimal TelephoneComn { get; set; }
        public decimal EmailComn { get; set; }
        public string ComnLevel { get; set; }
        public decimal ResponseFromHeadOffice { get; set; }
        public List<ComnWithinProjectViewModel> ComnWithinProjectList { get; set; }
        public List<ComnWithHeadOfficeViewModel> ComnWithHeadOfficeList { get; set; }
        public List<ComnWithConsultantViewModel> ComnWithConsultantList { get; set; }
    }
    public class MonthlyPerformanceModifierVM
    {
        public MonthlyPerformanceModifierVM()
        {
            ModifierHelthAndSafetyList = new List<ModifierHelthAndSafetyVM>();
            ModifierBudgetReqAndUtilList = new List<ModifierBudgetRequestAndUtilizationVM>();
            ModifierMaterialUtlList = new List<ModifierMaterialUtlVM>();
            ModifierSubContractAdmnList = new List<ModifierSubContractAdmVM>();
        }
        public decimal HealthAndSafety { get; set; }
        public decimal BudgetRequestAndUtilization { get; set; }
        public decimal MaterialUtilization { get; set; }
        public decimal ConductQualityAssurance { get; set; }
        public decimal SumOfPRPAndQualityAssurance { get; set; }
        public decimal SubContractAdministration { get; set; }
        public decimal TotalPoints { get; set; }

        // health and safety
        public int HeavyInjuriesCount { get; set; }
        public int LightInjuriesCount { get; set; }

        // budget request and utilization
        public decimal ForcastedBudget { get; set; }
        public decimal RequestedBudget { get; set; }
        public decimal RequestDeviation { get; set; }
        public decimal PointFromReqDeviation { get; set; }
        public decimal UtilizedBudget { get; set; }
        public decimal UtilizationDeviation { get; set; }
        public decimal PointFromUtlDeviation { get; set; }

        // material utl
        //public decimal Cement { get; set; }
        //public decimal ReadyMixConcrete { get; set; }
        //public decimal ReinforcementBar { get; set; }
        //public decimal FormWork { get; set; }
        //public decimal Sand { get; set; }
        //public decimal Ceramic { get; set; }

        // PRP implementation
        public string PRPImplementationLevel { get; set; }

        // Conduct Quality Assurance - road projects
        public decimal ReworkHappened { get; set; }
        public decimal ConsultantComplaint { get; set; }
        public decimal DefectOccured { get; set; }

        // sub-contract complaints
        public decimal SiteAccess { get; set; }
        public decimal DelayedPayment { get; set; }
        public decimal LackOfGoodGovernance { get; set; }
        public decimal InactionForSubContractor { get; set; }

        public List<ModifierHelthAndSafetyVM> ModifierHelthAndSafetyList { get; set; }
        public List<ModifierBudgetRequestAndUtilizationVM> ModifierBudgetReqAndUtilList { get; set; }
        public List<ModifierMaterialUtlVM> ModifierMaterialUtlList { get; set; }
        public List<ModifierSubContractAdmVM> ModifierSubContractAdmnList { get; set; }
    }

    #region view models for Monthly Project Quality sub-components
    public class QualityPlanningViewModel
    {
        public string Description { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal Points { get; set; }
    }
    public class QualityAssuranceViewModel
    {
        public string Description { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal Points { get; set; }
    }
    public class QualityControlViewModel
    {
        public string Description { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal Points { get; set; }
    }
    public class QualityComplaintsViewModel
    {
        public string Source { get; set; }
        public int Complaint { get; set; }
    }
    #endregion
    #region Planning sub components
    public class RevisedScheduleAcToManualViewModel
    {
        public string IsRevisionTime { get; set; }
        public string SubmissionTime { get; set; }
        public decimal? Days { get; set; }
        public decimal Points { get; set; }
    }
    public class RevisedScheduleAcToContractViewMode
    {
        public string IsRevisionTime { get; set; }
        public string SubmissionTime { get; set; }
        public decimal? Days { get; set; }
        public decimal Points { get; set; }
    }
    public class PlanningCompletnessViewModel
    {
        public string ScheduleType { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
    }
    public class PlanningQualityAndAcceptablityViewModel
    {
        public string IsApprovalMadeInTheMonth { get; set; }
        public string ApprovalType { get; set; }
        public decimal points { get; set; }
    }
    #endregion
    #region claim sub-components
    public class ClaimContempraryRecordViewModel
    {
        public string Description { get; set; }
        public decimal Points { get; set; }

    }
    public class ClaimRequirementViewModel
    {
        public string IsClaimPreparedAndIssuedInThisMonth { get; set; }
        public string Description { get; set; }
        public decimal Points { get; set; }
    }
    public class ClaimAnalysisTechniqueViewModel
    {
        public string WasThereIssueToClaim { get; set; }
        public string AnalysisTechnique { get; set; }
        public decimal Points { get; set; }
    }
    public class ClaimApprovalStatusViewModel
    {
        public string IsApprovalMadeInTheMonth { get; set; }
        public string ApprovalStatus { get; set; }
        public decimal Points { get; set; }
    }
    #endregion
    #region Report sub-components
    public class ReportCompletnessViewModel
    {
        public string Attribute { get; set; }
        public decimal Points { get; set; }
    }
    public class ReportNarrativeViewModel
    {
        public string Attribute { get; set; }
        public decimal Points { get; set; }
    }
    #endregion
    #region communication sub-components
    public class ComnWithinProjectViewModel
    {
        public string Description { get; set; }
        public decimal Points { get; set; }
    }
    public class ComnWithHeadOfficeViewModel
    {
        public string CommunicationMethod { get; set; }
        public decimal Points { get; set; }
    }
    public class ComnWithConsultantViewModel
    {
        public string CommunicationLevel { get; set; }
        public decimal Points { get; set; }
    }
    #endregion
    public class ModifierHelthAndSafetyVM
    {
        public string InjuryLevel { get; set; }
        public int InjuryCount { get; set; }
        public decimal Points { get; set; }
    }
    public class ModifierBudgetRequestAndUtilizationVM
    {
        public ModifierBudgetRequestAndUtilizationVM()
        {
            BudgetRequestList = new List<ModifierBudgetReqVM>();
            BudgetUtilizationList = new List<ModifierBudgetUtlVM>();
        }
        public decimal MaxAllowablePoints { get; set; }
        public decimal TotalPoints { get; set; }
        public List<ModifierBudgetUtlVM> BudgetUtilizationList { get; set; }
        public List<ModifierBudgetReqVM> BudgetRequestList { get; set; }
    }
    public class ModifierBudgetReqVM
    {
        public decimal ForecastedBudget { get; set; }
        public decimal RequestedBudget { get; set; }
        public decimal Deviation { get; set; }
        public decimal Points { get; set; }

    }
    public class ModifierBudgetUtlVM
    {
        public decimal RequestedBudget { get; set; }
        public decimal UtilizedBudget { get; set; }
        public decimal Deviation { get; set; }
        public decimal Points { get; set; }
    }
    public class ModifierMaterialUtlVM
    {
        public string Material { get; set; }
        public decimal UtilizedAmount { get; set; }
    }
    public class ModifierSubContractAdmVM
    {
        public string Description { get; set; }
        public decimal? Points { get; set; }
    }

    public class PerformanceEvaluationPeriodViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class PerformanceEvaluationComponentsSummaryViewModel
    {
        public PerformanceEvaluationComponentsSummaryViewModel()
        {
            SubComponentList = new List<PerformanceEvaluationSubComponentsSummaryViewModel>();
        }
        public string EvaluationComponent { get; set; }
        public decimal Weight { get; set; }
        public decimal RowData { get; set; }
        public decimal Points { get; set; }
        public decimal Result { get; set; }
        public bool HasNoSub { get; set; }
        public List<PerformanceEvaluationSubComponentsSummaryViewModel> SubComponentList { get; set; }

    }
    public class PerformanceEvaluationSubComponentsSummaryViewModel
    {
        public string EvaluationComponent { get; set; }
        public decimal Weight { get; set; }
        public decimal RowData { get; set; }
        public decimal Points { get; set; }
        public decimal Result { get; set; }
    }
    public class PerformanceEvaluationModifiersSummaryViewModel
    {
        public string EvaluationComponent { get; set; }
        public decimal Weight { get; set; }
        public decimal RowData { get; set; }
        public decimal Points { get; set; }
        public decimal Result { get; set; }
    }
    public class PerformanceEvaluationModifiersSubComponentSummaryViewModel
    {
        public string EvaluationComponent { get; set; }
        public decimal Weight { get; set; }
        public decimal RowData { get; set; }
        public decimal Points { get; set; }
        public decimal Result { get; set; }
    }
}
