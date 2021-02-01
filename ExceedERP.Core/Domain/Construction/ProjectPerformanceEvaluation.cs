using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    //class ProjectPerformanceEvaluation
    //{
    //}
    public class PerformanceEvaluationComponentWeight : TrackUserOperation
    {
        [Key]
        public int PerformanceEvaluationComponentWeightId { get; set; }
        [Display(Name = "Name")]
        public ProjectPerformanceEvaluationComponents ComponentName { get; set; }
        [Display(Name = "Value(Building Projects)")]
        public decimal Value { get; set; }
        [Display(Name = "Value(Road Projects)")]
        public decimal ValueForRoadProjects { get; set; }
    }
    public class PerformanceProjectGeneralInformationVM
    {
        public int ProjectInformationId { get; set; }
        public AvailableProjectTypes ProjectType { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public string ContractSigningDate { get; set; }
        public decimal? ContractPeriod { get; set; }
        public string CommencementDate { get; set; }
        public decimal? ApprovedTimeExtension { get; set; }
        public decimal? TotalProjectPeriod { get; set; }
        public string OriginalcompletionTime { get; set; }
        public string RevisedCompletionDate { get; set; }
        public decimal? TimeElapsedToDate { get; set; }
        public decimal ContractAmount { get; set; }
        public decimal? Rebate { get; set; }
        public decimal? Variation { get; set; }
        public decimal? Supplemetary { get; set; }
        public decimal? TotalProjectAmount { get; set; }
        public decimal? NetGrand { get; set; }
        public decimal? ExecutedToDate { get; set; }
    }
    public class PeriodMapping
    {
        [Key]
        public int PeriodMappingId { get; set; }
        public int ConstructionPeriodId { get; set; }
        public int ProjectInformationId { get; set; }
        public string Remark { get; set; }

        public virtual ProjectInformation ProjectInformation { get; set; }
        public virtual ConstructionPeriod ConstructionPeriod { get; set; }
    }
    public class MonthlyProjectProgress : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectProgressId { get; set; }

        [Display(Name = "Period")]
        public int PeriodMappingId { get; set; }
        //public int ProjectInformationId { get; set; }

        [Display(Name = "Planned Amount for this Month")]
        public decimal PlannedAmount { get; set; }

        [Display(Name = "Planned Amnt till Prev. Month")]
        public decimal PlannedAmountTillPreviousMonth { get; set; }

        [Display(Name = "Planned Amnt. to Date")]
        public decimal PlannedToDate { get; set; }

        [Display(Name = "Executed Amnt for this Month")]
        public decimal ExecutedAmount { get; set; }

        [Display(Name = "Executed Amnt. till Prev. Month")]
        public decimal ExecutedAmountTillPreviousMonth { get; set; }

        [Display(Name = "Executed Amnt. to Date")]
        public decimal ExecutedAmountToDate { get; set; }

        [Display(Name = "Executed Amnt. to Date + Price Adj. to date")]
        public decimal ExecutedAmountToDateIncludingPriceAdjustment { get; set; }


        // price adjustment
        [Display(Name ="Price adj. for this Month")]
        public decimal PriceAdjustmentThisMonth { get; set; }
        [Display(Name ="Price adj. till previous Month")]
        public decimal PriceAdjustmentTillPrevMonth { get; set; }
        [Display(Name ="Price adj. to date")]
        public decimal PriceAdjustmentToDate { get; set; }

        [Display(Name = "Is the schedule brocken down in weekly bases?")]
        public bool ScheduleBrokenDownToWeeklyBase { get; set; }

        [Display(Name = "Are all formen aware of it? ")]
        public bool AreFormenAwareOfIt { get; set; }

        // for road projects only
        [Display(Name = "Is the action plan prepared with amount, responsible body and are problems addressed? Yes/no")]
        public bool ActionPlanIsPreparedWithAmountAndProblemsAreAddressed { get; set; }
        [Display(Name = "Is there weldefined team structure with required and limited resources? Yes/no")]
        public bool WellDefinedTeamStructureExists { get; set; }
        //

        [Display(Name = "Number of Evaluation Meetings")]
        public int NumberOfEvaluationMeething { get; set; }

        [Display(Name = "Correction Factor")]
        public decimal CorrectionFactor { get; set; }
        public virtual PeriodMapping PeriodMapping { get; set; }
        //public virtual ProjectInformation ProjectInformation { get; set; }
    }
    //public class MonthlyQualitySummaryVM
    //{
    //    public int PeriodMappingId { get; set; }
    //    public decimal Planning { get; set; }
    //    public decimal Assurance { get; set; }
    //    public decimal Control { get; set; }
    //    public int Complaint { get; set; }
    //}
    public class MonthlyProjectQuality : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectQualityId { get; set; }
        public int PeriodMappingId { get; set; }

        [Display(Name = "Quality Planning")]
        public decimal QualityPlanning { get; set; }
        [Display(Name = "Quality Assurance")]
        public decimal QualityAssurance { get; set; }
        [Display(Name = "Quality Control")]
        public decimal QualityControl { get; set; }
        [Display(Name = "Quality complaints")]
        public int QualityComplaint { get; set; }
        public decimal MaxAllowablePoints { get; set; }
        public decimal TotalPoints { get; set; }

        // quality planning sub-components
        [Display(Name = "Do Formen and skilled laborers know the qualily requirments of the works excuted in the month")]
        public decimal DoPersonnelKnowTheQualityRequirements { get; set; }
        [Display(Name = "Do Formen and skilled laborers know how to meet/satisfy these quality requirments")]
        public decimal DoPersonnelKnowHowToMeetQualityRequirements { get; set; }

        // quality assurance sub-components
        [Display(Name = "Material Tests/Approvals Performed with respect to Expected")]
        public decimal MaterialTestsPerformed { get; set; }
        [Display(Name = "Evaluation of the construction method/ process applied with respect to quality requirments")]
        public decimal EvaluationOfMethod { get; set; }
        [Display(Name = "Overall evaluation the of qualifications of skilled labor employed")]
        public decimal OverAllEvaluationOfLabour { get; set; }

        // quality control sub-components
        [Display(Name = "The number of  Tests Performed on excueted works with respect to Expected")]
        public decimal NumberOfTestsPerformedOnExecutedWorks { get; set; }
        [Display(Name = "The extent of  Inspections Performed on executed works with respect to Expected")]
        public decimal ExtentOfInspectionPerformedOnExecutedWorks { get; set; }

        // quality complaints sub-components
        [Display(Name = "From Client")]
        public int FromClient { get; set; }
        [Display(Name = "From Engineers")]
        public int FromEngineers { get; set; }
        [Display(Name = "From Head Office")]
        public int FromHeadOffice { get; set; }
        [Display(Name = "From Quality Engineer / Office Eng. ")]
        public int FromQualityEngineer { get; set; }

        public virtual PeriodMapping PeriodMapping { get; set; }
    }
    /// <summary>
    /// a base class that contains the common attributes
    /// </summary>
    public class MonthlyProjectQualityBase : TrackUserOperation
    {
        //public int PeriodMappingId { get; set; }
        public int MonthlyProjectQualityId { get; set; }

        //public int ProjectInformationId { get; set; }
        public string Description { get; set; }
        public decimal? MaxPoints { get; set; }
        public decimal Points { get; set; }
        public virtual PeriodMapping PeriodMapping { get; set; }
        public virtual MonthlyProjectQuality MonthlyProjectQuality { get; set; }
    }
    public class QualityPlanning : MonthlyProjectQualityBase
    {
        [Key]
        public int QualityPlanningId { get; set; }
    }
    public class QualityAssurance : MonthlyProjectQualityBase
    {
        [Key]
        public int QualityAssuranceId { get; set; }
    }
    public class QualityControl : MonthlyProjectQualityBase
    {
        [Key]
        public int QualityControlId { get; set; }
    }
    public class MonthlyProjectQualityComplaint : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectQualityComplaintId { get; set; }
        [Display(Name = "Period")]
        //public int PeriodMappingId { get; set; }
        public int MonthlyProjectQualityId { get; set; }

        //public int ProjectInformationId { get; set; }
        public ProjectQualityComplaintSource Source { get; set; }
        public string Remark { get; set; }
        public int NumOfComplaints { get; set; }
        public virtual MonthlyProjectQuality MonthlyProjectQuality { get; set; }
        //public virtual ProjectInformation ProjectInformation { get; set; }
    }

    public class MonthlyProjectCost : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectCostId { get; set; }
        public int PeriodMappingId { get; set; }
        //public int ProjectInformationId { get; set; }

        [Display(Name = "Planned direct cost for the month")]
        public decimal PlannedDirectCost { get; set; }
        [Display(Name = "Planned project OH cost for the month")]
        public decimal PlannedProjectOHCost { get; set; }
        [Display(Name = "Planned profit and Head Office OH for the month")]
        public decimal PlannedProfitAndOHCost { get; set; }
        [Display(Name = "Actual direct cost for the month")]
        public decimal ActualDirectCost { get; set; }
        [Display(Name = "Actual  project overhead cost for the month")]
        public decimal ActualProjectOHCost { get; set; }
        [Display(Name = "Actual profit & Head Office OH for the month")]
        public decimal ActualProfitAndOHCost { get; set; }
        [Display(Name = "Planned direct cost todate")]
        public decimal PlannedDirectCostToDate { get; set; }
        [Display(Name = "Planned project OH cost for todate")]
        public decimal PlannedProjectOHCostToDate { get; set; }
        [Display(Name = "Planned profit and Head Office OH todate")]
        public decimal PlannedProfitAndOHToDate { get; set; }
        [Display(Name = "Actual direct cost till prev. month")]
        public decimal ActualDirectCostTillPrevMonth { get; set; }
        [Display(Name = "Actual direct cost todate")]
        public decimal ActualDirectCostToDate { get; set; }
        [Display(Name = "Actual  project overhead cost till prev. month")]
        public decimal ActualProjectOHCostTillPrevMonth { get; set; }
        [Display(Name = "Actual  project overhead cost todate")]
        public decimal ActualProjectOHCostToDate { get; set; }
        [Display(Name = "Actual  project overhead cost till prev. month")]
        public decimal ActualProfitAndOHTillPrevMonth { get; set; }
        [Display(Name = "Actual profit & Head Office OH todate")]
        public decimal ActualProfitAndOHToDate { get; set; }
        [Display(Name = "Actual Unit cost Detrmination and Evaluation with respect to Budget")]
        public decimal CostTracking { get; set; }

        // resource utilization (road projects)
        [Display(Name = "Equipment utilization efficiencies (>80=4, 65-80=3, 50-65 = 2, <50 = 0)")]
        public decimal EquipmentUtilizationEfficiency { get; set; }
        [Display(Name = "Fuel Utilization based on the standard (Within Range = 4, at marigin = 3 and out of Range =0)")]
        public decimal FuelUtilizationBasedOnStandard { get; set; }
        [Display(Name = "Equipment cost, material cost, labour cost, fuel cost and project overhead cost from expectation (4 pts)")]
        public decimal CostsFromExpectation { get; set; }
        [Display(Name = "Proper and accurate Earned Value Analysis (3 pts)")]
        public decimal ProperAndAccurateEarnedValueAnalysis { get; set; }
        [Display(Name = "Proper measure of labor and equipment productivity (equipment = 2, labour =1)")]
        public decimal ProductivitiyOfLaborAndEquipmentMeaure { get; set; }
        [Display(Name = "Re-using of construction materials (2 pts)")]
        public decimal ReusingOfConstructionMaterials { get; set; }
        //
        public virtual PeriodMapping PeriodMapping { get; set; }
        //public virtual ProjectInformation ProjectInformation { get; set; }

    }
    public class MonthlyProjectPayment : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectPaymentId { get; set; }
        public int PeriodMappingId { get; set; }

        [Display(Name = "Approved till previous month")]
        public decimal ApprovedTillPreMonth { get; set; }
        [Display(Name = "Expected tobe Approved this month")]
        public decimal ExpectedToBeApprovedThisMonth { get; set; }
        [Display(Name = "Amount Approved this month")]
        public decimal AmountApprovedThisMonth { get; set; }
        [Display(Name = "Amount not Approved this month")]
        public decimal? AmounNotApprovedThisMonth { get; set; }
        [Display(Name = "Approved todate")]
        public decimal ApprovedToDate { get; set; }
        [Display(Name = "Amount not approved to date")]
        public decimal AmountNotApprovedToDate { get; set; }
        [Display(Name = "Price adjustment till previous month")]
        public decimal? PriceAdjustmentTillPreMonth { get; set; }
        [Display(Name = "Price adjustment this month")]
        public decimal? PriceAdjustmentThisMonth { get; set; }
        [Display(Name = "Price adjustment appoved todate")]
        public decimal? PriceAdjustmentApprovedToDate { get; set; }
        [Display(Name = "Mat.on site this month")]
        public decimal? MatOnSiteThisMonth { get; set; }
        [Display(Name = "Mat.on site appoved this month")]
        public decimal? MatOnSiteApprovedThisMonth { get; set; }
        [Display(Name = "Mat.on site not appoved this month")]
        public decimal? MatOnSiteNotAppprovedThiMonth { get; set; }

        [Display(Name = "Payment Approval Time")]
        public MonthlyDueDate ApprovalTime { get; set; }
        [Display(Name = "Days")]
        public decimal? Days { get; set; }
        public decimal Points { get; set; }
        public virtual PeriodMapping PeriodMapping { get; set; }

    }
    public class MonthlyProjectPlanning : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectPlanningId { get; set; }
        public int PeriodMappingId { get; set; }

        [Display(Name = "Revised Schedule Submission According to Oprational Manual")]
        public decimal RevScheduleSubAcToManual { get; set; }

        [Display(Name = "Revised Schedule Submission According to Contract")]
        public decimal RevScheduleSubAcToContract { get; set; }

        [Display(Name = "Completeness average of both revisions or one of them")]
        public decimal Completness { get; set; }

        [Display(Name = "Quality and Acceptability avg of both revisions or one of them")]
        public decimal QualityAndAcceptablity { get; set; }

        public decimal? MaxAllowablePoints { get; set; }
        public decimal? TotalPoints { get; set; }

        // rev according to manual
        [Display(Name = "Is this month revision time?")]
        public bool IsRevisionTimeAccManual { get; set; }
        [Display(Name = "Is the revisised shedule submitted accordingly")]
        public bool RevisedSdlSubmitedAccordinglyAccToManual { get; set; }
        [Display(Name = "Schedule Submission")]
        public MonthlyDueDate SubmssinTimeAcctoManual { get; set; }
        [Display(Name = "Days (af. or bef. due date)")]
        public int DaysAfterOrBeforeDueDateAccToManual { get; set; }

        // rev according to contrat
        [Display(Name = "Is this month revision time?")]
        public bool IsRevisionTimeAccContract { get; set; }
        [Display(Name = "Is the revisised shedule submitted accordingly")]
        public bool RevisedSdlSubmitedAccordinglyAccToContract { get; set; }
        [Display(Name = "Schedule Submission")]
        public MonthlyDueDate SubmssinTimeAcctoContract { get; set; }
        [Display(Name = "Days (af. or bef. due date)")]
        public int DaysAfterOrBeforeDueDateAccToContract { get; set; }

        // completness
        [Display(Name = "Work Schedule (1 pts)")]
        public decimal WorkSchedule { get; set; }
        [Display(Name = "Resource Schedule (0.5 pts)")]
        public decimal ResourceSchedule { get; set; }
        [Display(Name = "Cash flow Schedule (0.25 pts)")]
        public decimal CashFlowSchedule { get; set; }
        [Display(Name = "Budget requirment Schedule (0.25 pts)")]
        public decimal BudgetRequirementSchedule { get; set; }

        // acceptablity and ..
        [Display(Name = "Is Approval made in the month ")]
        public bool IsApprovalMadeInTheMonth { get; set; }
        [Display(Name = "Approval Type")]
        public RevisedScheduleApprovalStatus ApprovalType { get; set; }
        public virtual PeriodMapping PeriodMapping { get; set; }

        public ICollection<RScheduleSubmissionAcToOperationManual> SubmissionAcToManualList { get; set; }
        public ICollection<RScheduleSubmissionAcToContract> SubmissionAcToContractList { get; set; }
        public ICollection<ProjectPnanningCompleteness> PlaninngCompletnessList { get; set; }
        public ICollection<ProjectPlanningQualityAndAcceptance> QualityAndAcceptanceList { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class RScheduleSubmissionAcToOperationManual : TrackUserOperation
    {
        [Key]
        public int RScheduleSubmissionAcToOperationManualId { get; set; }
        public int MonthlyProjectPlanningId { get; set; }

        [Display(Name = "Is this month revision time yes/no")]
        public bool IsRevisionTime { get; set; }
        [Display(Name = "revisised shedule submission")]
        public MonthlyDueDate SubmissionTime { get; set; }
        public decimal? Days { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectPlanning MonthlyProjectPlanning { get; set; }
    }
    public class RScheduleSubmissionAcToContract : TrackUserOperation
    {
        [Key]
        public int RScheduleSubmissionAcToContractId { get; set; }
        public int MonthlyProjectPlanningId { get; set; }

        [Display(Name = "Is this month revision time yes/no")]
        public bool IsRevisionTime { get; set; }
        [Display(Name = "revisised shedule submission")]
        public MonthlyDueDate SubmissionTime { get; set; }
        public decimal? Days { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectPlanning MonthlyProjectPlanning { get; set; }

    }
    public class ProjectPnanningCompleteness : TrackUserOperation
    {
        [Key]
        public int ProjectPnanningCompletenessId { get; set; }
        public int MonthlyProjectPlanningId { get; set; }

        public ProjectPlanScheduleType ScheduleType { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectPlanning MonthlyProjectPlanning { get; set; }

    }
    public class ProjectPlanningQualityAndAcceptance : TrackUserOperation
    {
        [Key]
        public int ProjectPlanningQualityAndAcceptanceId { get; set; }
        public int MonthlyProjectPlanningId { get; set; }

        [Display(Name = "Is Approval made in the month yes/no")]
        public bool IsApprovalMadeInTheMonth { get; set; }
        public RevisedScheduleApprovalStatus ApprovalType { get; set; }
        public decimal points { get; set; }
        public virtual MonthlyProjectPlanning MonthlyProjectPlanning { get; set; }
    }


    public class MonthlyProjectClaim : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectClaimId { get; set; }
        public int PeriodMappingId { get; set; }

        [Display(Name = "Contemprary record keeping")]
        public decimal ContempraryRecordKeeping { get; set; }
        [Display(Name = "Full Filling Requirments for claims submitted in the month")]
        public decimal FullFillingRequirementForClaims { get; set; }
        [Display(Name = "Analysis Technique Application for claim submitted in the month")]
        public decimal AnalysisTechnique { get; set; }
        [Display(Name = "Approval status for claims responded in the month")]
        public decimal ApprovalStatusOfClaims { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
        public bool IsClaimPreparedAndIssuedInThisMonth { get; set; }

        // contemprary record
        [Display(Name = "Daily Records (2 points for 26 days record)")]
        public decimal DailylyRecords { get; set; }

        // req ...
        [Display(Name = "Is Claim prepared and issued in this month")]
        public bool IsClaimPreparedAndIssuedInTheMonth { get; set; }
        [Display(Name = "Substantive requirements refering clauses (1 pts)")]
        public decimal SubstantiveRequirementRefClauses { get; set; }
        [Display(Name = "Procedural requirements Claim intent (1 pts)")]
        public decimal ProceduralRequirementDocumentation { get; set; }
        [Display(Name = "Proof requirements documentaion (1 pts)")]
        public decimal ProofRequirementDocumentation { get; set; }

        // analysis ..
        [Display(Name = "Was there issue to claim in the month")]
        public bool WasThereissuetoClaim { get; set; }
        [Display(Name = "Delay Analysis ...")]
        public AnalysisTechniqueForMonthlyClaims AnalysisType { get; set; }

        // approval status
        [Display(Name = "Is Approval made in the month")]
        public bool IsApprovalMadInTheMonth { get; set; }
        [Display(Name = "Approval Status ...")]
        public MonthlyClaimRequestApprovalStatus ClaimApprovalStatus { get; set; }

        public virtual PeriodMapping PeriodMapping { get; set; }
        public ICollection<ContempraryRecord> ContempraryRecordList { get; set; }
        public ICollection<RequirementForMonthlyClaims> RequirementForMonthlyClaimsList { get; set; }
        public ICollection<AnalysisTechniqueApplicationForMonthlyClaims> AnalysisTechniqueForMonthlyClaimsList { get; set; }
        public ICollection<ApprovalStatusForClaimsResponded> ApprovalStatusForClaimsRespondedList { get; set; }
    }
    public class ContempraryRecord : TrackUserOperation
    {
        [Key]
        public int ContempraryRecordId { get; set; }
        public int MonthlyProjectClaimId { get; set; }

        public string Description { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectClaim MonthlyProjectClaim { get; set; }
    }
    public class RequirementForMonthlyClaims : TrackUserOperation
    {
        [Key]
        public int RequirementForMonthlyClaimsId { get; set; }
        public int MonthlyProjectClaimId { get; set; }

        public string Description { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectClaim MonthlyProjectClaim { get; set; }

    }
    public class AnalysisTechniqueApplicationForMonthlyClaims : TrackUserOperation
    {
        [Key]
        public int AnalysisTechniqueApplicationForMonthlyClaimsId { get; set; }
        public int MonthlyProjectClaimId { get; set; }

        public bool WasThereIssueToClaim { get; set; }
        public AnalysisTechniqueForMonthlyClaims AnalysisTechnique { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectClaim MonthlyProjectClaim { get; set; }

    }
    public class ApprovalStatusForClaimsResponded : TrackUserOperation
    {
        [Key]
        public int ApprovalStatusForClaimsRespondedId { get; set; }
        public int MonthlyProjectClaimId { get; set; }

        public bool IsApprovalMadeInTheMonth { get; set; }
        public MonthlyClaimRequestApprovalStatus ApprovalStatus { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectClaim MonthlyProjectClaim { get; set; }

    }
    public class MonthlyProjectReport : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectReportId { get; set; }
        public int PeriodMappingId { get; set; }


        public decimal Completeness { get; set; }
        public decimal Narrative { get; set; }
        public decimal MaxAllowablePoints { get; set; }
        public decimal TotalPoints { get; set; }

        // reporting time
        [Display(Name = "Reporting Time")]
        public MonthlyDueDate ReportingTime { get; set; }
        [Display(Name = "Days (af. or bef. due date)")]
        public decimal? Days { get; set; }
        [Display(Name = "Reporting Time")]
        public decimal Points { get; set; }

        // completness
        [Display(Name = "Progess/Income/Expense evaluation (1 pt)")]
        public decimal Progress { get; set; }
        [Display(Name = "Quality control/assuracne (1/2 pt)")]
        public decimal QualityControl { get; set; }
        [Display(Name = "Health & Safety (1/2)")]
        public decimal HealthAndSafety { get; set; }
        [Display(Name = "Earned Value Analysis (1 pt)")]
        public decimal EarnedValueAnalysis { get; set; }
        [Display(Name = "Material utilization (1/4 pt)")]
        public decimal MaterialUtilization { get; set; }
        [Display(Name = "Labor Utilization (1/4 pt)")]
        public decimal LabourUtilixation { get; set; }
        [Display(Name = "Equipment Utilization (3/4 pt)")]
        public decimal EquipmentUtulization { get; set; }
        [Display(Name = "Fuel Utilization")]
        public decimal FuelUtilization { get; set; }

        // narratives
        [Display(Name = "Proper Identificaton of problems wrt affected activities")]
        public decimal ProperIdentificationOfProblems { get; set; }

        public virtual PeriodMapping PeriodMapping { get; set; }
        public ICollection<MonthlyProjectReportCompletnessComponent> CompletenessList { get; set; }
        public ICollection<MonthlyProjectReportNarrative> Narratives { get; set; }

    }
    public class MonthlyProjectReportCompletnessComponent : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectReportCompletnessComponentId { get; set; }
        public int MonthlyProjectReportId { get; set; }
        public string Attribute { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectReport ProjectReport { get; set; }
    }
    public class MonthlyProjectReportNarrative : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectReportNarrativeId { get; set; }
        public int MonthlyProjectReportId { get; set; }
        public string Attribute { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectReport ProjectReport { get; set; }
    }

    public class MonthlyProjectCommunication : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectCommunicationId { get; set; }
        public int PeriodMappingId { get; set; }

        [Display(Name = "With in the project")]
        public decimal ComnWithinProject { get; set; }
        [Display(Name = "With the Head Office")]
        public decimal ComnWithHeadOffice { get; set; }
        [Display(Name = "With Consultnant ")]
        public decimal ComnWithConsultant { get; set; }
        [Display(Name = "Number of Good goovernace complaints inside the project raised in the month")]
        public int NumberOfComplaints { get; set; }
        [Display(Name = "Pts from Good Governance Complaints")]
        public decimal GoodGovernanceComplaints { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal TotalPoints { get; set; }

        // within project
        [Display(Name = " close discusions and coopration within project commette")]
        public decimal CloseDiscussionWithinProject { get; set; }
        [Display(Name = " Mobilization and close discusions with all workers")]
        public decimal MobilizationAndDisscusstionWthinWorkers { get; set; }
        // road
        public decimal CommunicationAndSupportBetweenProjects { get; set; }
        //public decimal PoorCommunicationBetween

        // with head office
        [Display(Name = "Written Comn.")]
        public decimal WrittenComn { get; set; }
        [Display(Name = "Telephone Comn.")]
        public decimal TelephoneComn { get; set; }
        [Display(Name = "Email Comn.")]
        public decimal EmailComn { get; set; }
        [Display(Name = "Timely respond of requests from head office")]
        public decimal ResponseFromHeadOffice { get; set; }

        [Display(Name = "Level of Comn.")]
        public MonthlyProjectCommunicationLevel ComnLevel { get; set; }
        public virtual PeriodMapping PeriodMapping { get; set; }

        public virtual ICollection<MonthlyProjectCommunicationWithinProject> CommunicationWihinProjectList { get; set; }
        public virtual ICollection<MonthlyProjectCommunicationWithHeadOffice> CommunicationWihHeadOfficeList { get; set; }
        public virtual ICollection<MonthlyProjectCommunicationWithConsultant> CommunicationWithConsaltantList { get; set; }

    }
    public class MonthlyProjectCommunicationWithinProject : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectCommunicationWithinProjectId { get; set; }
        public int MonthlyProjectCommunicationId { get; set; }

        public string Description { get; set; }
        public decimal? MaxAllowedPoints { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectCommunication MonthlyProjectCommunication { get; set; }

    }
    //public class GoodGovernanceComplaintWithinProject
    //{
    //    [Key]
    //    public int GoodGovernanceComplaintWithinProjectId { get; set; }
    //    public int MonthlyProjectCommunicationId { get; set; }

    //    public int NumOfGovernanceComplaintsInMonth { get; set; }
    //    public string Remark { get; set; }
    //    public virtual MonthlyProjectCommunication MonthlyProjectCommunication { get; set; }

    //}
    public class MonthlyProjectCommunicationWithHeadOffice : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectCommunicationWithHeadOfficeId { get; set; }
        public int MonthlyProjectCommunicationId { get; set; }

        public MonthlyProjectCommunicationMethod CommunicationMethod { get; set; }
        public decimal? MaxAllowedPoints { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectCommunication MonthlyProjectCommunication { get; set; }

    }
    public class MonthlyProjectCommunicationWithConsultant : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectCommunicationWithConsultantId { get; set; }
        public int MonthlyProjectCommunicationId { get; set; }

        public MonthlyProjectCommunicationLevel CommunicationLevel { get; set; }
        public string Description { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectCommunication MonthlyProjectCommunication { get; set; }

    }

    public class MonthlyProjectPerformanceModifier : TrackUserOperation
    {
        [Key]
        public int MonthlyProjectPerformanceModifierId { get; set; }
        public int PeriodMappingId { get; set; }
        [Display(Name = "Health and Safety")]
        public decimal HealthAndSafety { get; set; }
        [Display(Name = "Budget Request and Utilization")]
        public decimal BudgetRequestAndUtilization { get; set; }
        [Display(Name = "Material Utilization with respect to Budget ")]
        public decimal MaterialUtilization { get; set; }
        [Display(Name = "Conduct Quality Assurance")]
        public decimal ConductQualityAssurance { get; set; }
        [Display(Name = "appropriate Sub contract Compliants  ")]
        public decimal SubContractAdministration { get; set; }
        public decimal MaxAllowablePoints { get; set; }

        public decimal TotalPoints { get; set; }

        // health and safety
        [Display(Name = "Heavy injury (-5pts for each injury)")]
        public int HeavyInjuriesCount { get; set; }
        [Display(Name = "Light injury (-1pt for each injury)")]
        public int LightInjuriesCount { get; set; }

        // budget request and utilization
        [Display(Name = "Forcasted Budget")]
        public decimal ForcastedBudget { get; set; }
        [Display(Name = "Requested Budget")]
        public decimal RequestedBudget { get; set; }
        [Display(Name = "Deviation of Req. Budg.")]
        public decimal RequestDeviation { get; set; }
        [Display(Name = "-2pts for deviation>15%")]
        public decimal PointFromReqDeviation { get; set; }
        [Display(Name = "Utilized Budget")]
        public decimal UtilizedBudget { get; set; }
        [Display(Name = "Deviation of Utl Budg.")]
        public decimal UtilizationDeviation { get; set; }
        [Display(Name = "-3pts for deviation >15%")]
        public decimal PointFromUtlDeviation { get; set; }

        // material utl
        //[Display(Name = "Cement")]
        //public decimal Cement { get; set; }
        //[Display(Name = "Ready Mix Concrete")]
        //public decimal ReadyMixConcrete { get; set; }
        //[Display(Name = "Reinforcement Bar")]
        //public decimal ReinforcementBar { get; set; }
        //[Display(Name = "Form Work")]
        //public decimal  FormWork { get; set; }
        //[Display(Name = "Sand")]
        //public decimal  Sand { get; set; }
        //[Display(Name = "Porcelain/Ceramic ")]
        //public decimal Ceramic { get; set; }

        // Project Resource Planning (PRP) implementation - road projects use such approach
        [Display(Name = "PRP implementation")]
        public PerformanceModifierPRPImplementationLevel PRPImplementationLevel { get; set; }
        
        // Conduct Quality Assurance - road projects
        [Display(Name = "if rework happended (-1 pts for each rework)")]
        public decimal ReworkHappened { get; set; }
        [Display(Name = "consultant complain (-1 pts for each quality comlains)")]
        public decimal ConsultantComplaint { get; set; }
        [Display(Name = "Defects occurred (-1 pts for each defects)")]
        public decimal DefectOccured { get; set; }

        // sub-contract complaints
        [Display(Name = "Site access/working place (-2pt for each)")]
        public decimal SiteAccess { get; set; }
        [Display(Name = "Delayed payment (-2pt for each)")]
        public decimal DelayedPayment { get; set; }
        [Display(Name = "Lack of Good govenance (-2pt each)")]
        public decimal LackOfGoodGovernance { get; set; }
        [Display(Name = "Inaction for sub contrator defults(-5pt each)")]
        public decimal InactionForSubContractor { get; set; }

        public virtual PeriodMapping PeriodMapping { get; set; }
    }
    public class PerformanceModifierHealthAndSafety : TrackUserOperation
    {
        [Key]
        public int PerformanceModifierHealthAndSafetyId { get; set; }
        public int MonthlyProjectPerformanceModifierId { get; set; }

        public PerformanceModifierInjuryLevel InjuryLevel { get; set; }
        public int InjuryCount { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectPerformanceModifier Modifier { get; set; }
    }
    public class PerformanceModifierBudgetRequest : TrackUserOperation
    {
        [Key]
        public int PerformanceModifierBudgetRequestId { get; set; }
        public int MonthlyProjectPerformanceModifierId { get; set; }

        public decimal ForecastedBudget { get; set; }
        public decimal RequestedBudget { get; set; }
        public decimal Deviation { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectPerformanceModifier Modifier { get; set; }

    }
    public class PerformanceModifierBudgetUtilization : TrackUserOperation
    {
        [Key]
        public int PerformanceModifierBudgetUtilizationId { get; set; }
        public int MonthlyProjectPerformanceModifierId { get; set; }

        public decimal RequestedBudget { get; set; }
        public decimal UtilizedBudget { get; set; }
        public decimal Deviation { get; set; }
        public decimal Points { get; set; }
        public virtual MonthlyProjectPerformanceModifier Modifier { get; set; }

    }
    public class PerformanceModifierMaterialUtilization : TrackUserOperation
    {
        [Key]
        public int PerformanceModifierMaterialUtilizationId { get; set; }
        public int MonthlyProjectPerformanceModifierId { get; set; }

        [Display(Name = "Material")]
        public string ItemCode { get; set; }
        public virtual ExceedERP.Core.Domain.Inventory.InventoryItem Item { get; set; }
        public string ItemCategoryCode { get; set; }
        public decimal UtilizedAmount { get; set; }
        public virtual MonthlyProjectPerformanceModifier Modifier { get; set; }

    }
    public class PerformanceModifierSubContractAdministration : TrackUserOperation
    {
        [Key]
        public int PerformanceModifierSubContractAdministrationId { get; set; }
        public int MonthlyProjectPerformanceModifierId { get; set; }

        public string Description { get; set; }
        public decimal? MaxAllowablePoints { get; set; }
        public decimal? Points { get; set; }

        public virtual MonthlyProjectPerformanceModifier Modifier { get; set; }
    }
    public class PEVCostSetting : TrackUserOperation
    {
        [Key]
        public int PEVCostSettingId { get; set; }
        [Display(Name = "Attribute")]
        public PEVCostRateSettingAttribute Attribute { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
        public int ProjectInformationId { get; set; }
    }

}
