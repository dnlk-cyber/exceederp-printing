using System.Data.Entity.ModelConfiguration;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.Letters;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Core.Domain.HRM.Prohibition;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.Core.Domain.HRM.Severance;
using System;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;
using ExceedERP.Core.Domain.HRM.HRMLoans;

namespace ExceedERP.DataAccess
{

    #region HRM
   
    public class ExpenseAndTaxSettingEntityTypeConfiguration : EntityTypeConfiguration<ExpenseAndTaxSetting>
    {

        public ExpenseAndTaxSettingEntityTypeConfiguration()
        {
            this.ToTable("ExpenseAndTaxSettings", "HRM");
            this.HasKey<int>(o => o.ExpenseAndTaxSettingId);
        }
    }

    public class HRMContributionSettingEntityTypeConfiguration : EntityTypeConfiguration<HRMContributionSetting>
    {

        public HRMContributionSettingEntityTypeConfiguration()
        {
            this.ToTable("HRMContributionSettings", "HRM");
            this.HasKey<int>(o => o.HRMContributionSettingId);
        }
    }
    public class DatePreferenceSettingEntityTypeConfiguration : EntityTypeConfiguration<DatePreferenceSetting>
    {

        public DatePreferenceSettingEntityTypeConfiguration()
        {
            this.ToTable("DatePreferenceSetting", "Performance");
            this.HasKey<int>(o => o.DatePreferenceSettingId);
        }
    }


    public class EmployeeExternalFundingEntityTypeConfiguration : EntityTypeConfiguration<EmployeeExternalFunding>
    {

        public EmployeeExternalFundingEntityTypeConfiguration()
        {
            this.ToTable("EmployeeExternalFundings", "HRM");
            this.HasKey<int>(o => o.EmployeeExternalFundingId);
        }
    }

    public class ExternalSalaryEntityTypeConfiguration : EntityTypeConfiguration<ExternalSalary>
    {
        public ExternalSalaryEntityTypeConfiguration()
        {
            this.ToTable("ExternalSalaries", "HRM");
            this.HasKey<int>(o => o.ExternalSalaryId);
            this.Property(o => o.Salary).HasPrecision(18, 5);
        }
    }



    public class GeneralJobtitleSettingEntityTypeConfiguration : EntityTypeConfiguration<GeneralJobtitleSetting>
    {

        public GeneralJobtitleSettingEntityTypeConfiguration()
        {
            this.ToTable("GeneralJobtitleSettings", "HRM");
            this.HasKey<int>(o => o.GeneralJobtitleSettingId);

        }
    }
    public class EmployeeProbationSettingEntityTypeConfiguration : EntityTypeConfiguration<EmployeeProbationSetting>
    {

        public EmployeeProbationSettingEntityTypeConfiguration()
        {
            this.ToTable("EmployeeProbationSettings", "HRM");
            this.HasKey<int>(o => o.EmployeeProbationSettingId);

        }
    }

    public class LeaveTransferEntityTypeConfiguration : EntityTypeConfiguration<LeaveTransfer>
    {

        public LeaveTransferEntityTypeConfiguration()
        {
            this.ToTable("LeaveTransfer", "HRM");
            this.HasKey<int>(o => o.LeaveTransferId);

        }
    }

    public class WaterMarkEntityTypeConfiguration : EntityTypeConfiguration<WaterMark>
    {
        public WaterMarkEntityTypeConfiguration()
        {
            this.ToTable("WaterMark", "HRM");
            this.HasKey<int>(o => o.WaterMarkId);
        }
    }

    public class TemplateHeaderImageEntityTypeConfiguration : EntityTypeConfiguration<TemplateHeaderImage>
    {
        public TemplateHeaderImageEntityTypeConfiguration()
        {
            this.ToTable("TemplateHeaderImage", "HRM");
            this.HasKey<int>(o => o.TemplateHeaderImageId);
        }
    }


    public class EmpPayrollNoDaysWorkedEntityTypeConfiguration : EntityTypeConfiguration<EmpPayrollNoDaysWorked>
    {
        public EmpPayrollNoDaysWorkedEntityTypeConfiguration()
        {
            this.ToTable("EmpPayrollNoDaysWorked", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollNoDaysWorkedId);
            this.Property(o => o.Days).HasPrecision(10, 5);
        }
    }
    public class EmployeeCodeSequenceSettingEntityTypeConfiguration : EntityTypeConfiguration<EmployeeCodeSequenceSetting>
    {

        public EmployeeCodeSequenceSettingEntityTypeConfiguration()
        {
            this.ToTable("EmployeeCodeSequenceSetting", "HRM");
            this.HasKey<int>(o => o.EmployeeCodeSequenceSettingId);

        }
    }
    public class EmployeeSalaryIncrementSettingEntityTypeConfiguration : EntityTypeConfiguration<EmployeeSalaryIncrementSetting>
    {

        public EmployeeSalaryIncrementSettingEntityTypeConfiguration()
        {
            this.ToTable("EmployeeSalaryIncrementSettings", "HRM");
            this.HasKey<int>(o => o.EmployeeSalaryIncrementSettingId);

        }
    }
    public class EmployeeCodeSettingEntityTypeConfiguration : EntityTypeConfiguration<EmployeeCodeSetting>
    {

        public EmployeeCodeSettingEntityTypeConfiguration()
        {
            this.ToTable("EmployeeCodeSetting", "HRM");
            this.HasKey<int>(o => o.EmployeeCodeSettingId);

        }
    }
    public class ComplainResponseEntityTypeConfiguration : EntityTypeConfiguration<ComplainResponse>
    {

        public ComplainResponseEntityTypeConfiguration()
        {
            this.ToTable("ComplainResponse", "HRM");
            this.HasKey<int>(o => o.ComplainResponseId);
        }
    }



    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An allowance defination entity type configuration. </summary>
    ///
    /// <remarks>  started  Mekanehiwot, -11/11/2016. </remarks>
    /// <remarks>  modifid and Added New Features by Kibrom Abera, 12/01/2021 - Today. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AllowanceSettingEntityTypeConfiguration : EntityTypeConfiguration<AllowanceSetting>
    {

        public AllowanceSettingEntityTypeConfiguration()
        {
            this.ToTable("AllowanceSettings", "HRM");
            this.HasKey<int>(o => o.AllowanceSettingId);
        }
    }
    public class AllowanceDefinationEntityTypeConfiguration : EntityTypeConfiguration<AllowanceDefination>
    {

        public AllowanceDefinationEntityTypeConfiguration()
        {
            this.ToTable("AllowanceDefination", "HRM");
            this.HasKey<int>(o => o.AllowanceDefinationId);
        }
    }
    public class AllowanceMappingEntityTypeConfiguration : EntityTypeConfiguration<AllowanceMapping>
    {

        public AllowanceMappingEntityTypeConfiguration()
        {
            this.ToTable("AllowanceMapping", "HRM");
            this.HasKey<int>(o => o.AllowanceMappingId);
        }
    }
    public class CourtAllowanceMappingEntityTypeConfiguration : EntityTypeConfiguration<CourtAllowanceMapping>
    {

        public CourtAllowanceMappingEntityTypeConfiguration()
        {
            this.ToTable("CourtAllowanceMapping", "HRM");
            this.HasKey<int>(o => o.AllowanceMappingId);
        }
    }

    public class DelegationStepIncrementSettingEntityTypeConfiguration : EntityTypeConfiguration<DelegationStepIncrementSetting>
    {

        public DelegationStepIncrementSettingEntityTypeConfiguration()
        {
            this.ToTable("DelegationStepIncrementSettings", "HRM");
            this.HasKey<int>(o => o.DelegationStepIncrementSettingId);
        }
    }

    
    public class DelegationEntityTypeConfiguration : EntityTypeConfiguration<Delegation>
    {

        public DelegationEntityTypeConfiguration()
        {
            this.ToTable("Delegation", "HRM");
            this.HasKey<int>(o => o.DelegationId);
        }
    }
    public class OverTimeSettingEntityTypeConfiguration : EntityTypeConfiguration<OverTimeSetting>
    {

        public OverTimeSettingEntityTypeConfiguration()
        {
            this.ToTable("OverTimeSettings", "HRM");
            this.HasKey<int>(o => o.OverTimeSettingId);
        }
    }
    public class DelegationReasonEntityTypeConfiguration : EntityTypeConfiguration<DelegationReason>
    {

        public DelegationReasonEntityTypeConfiguration()
        {
            this.ToTable("DelegationReason", "HRM");
            this.HasKey<int>(o => o.DelegationReasonId);
        }
    }
    public class AllowancePerEmployeeEntityTypeConfiguration : EntityTypeConfiguration<AllowancePerEmployee>
    {

        public AllowancePerEmployeeEntityTypeConfiguration()
        {
            this.ToTable("AllowancePerEmployee", "HRM");
            this.HasKey<int>(o => o.AllowanceId);
        }
    }
    
    public class ActingEntityTypeConfiguration : EntityTypeConfiguration<Acting>
    {

        public ActingEntityTypeConfiguration()
        {
            this.ToTable("Actings", "HRM");
            this.HasKey<int>(o => o.ActingId);
        }
    }
    public class ActingAllowanceEntityTypeConfiguration : EntityTypeConfiguration<ActingAllowance>
    {

        public ActingAllowanceEntityTypeConfiguration()
        {
            this.ToTable("ActingAllowances", "HRM");
            this.HasKey<int>(o => o.ActingAllowanceId);
        }
    }
    public class GuarantorOrSuretyEntityTypeConfiguration : EntityTypeConfiguration<GuarantorOrSurety>
    {

        public GuarantorOrSuretyEntityTypeConfiguration()
        {
            this.ToTable("GuarantorOrSureties", "HRM");
            this.HasKey<int>(o => o.GuarantorOrSuretyId);
        }
    }
    public class GuarantorOrSuretyDetailEntityTypeConfiguration : EntityTypeConfiguration<GuarantorOrSuretyDetail>
    {

        public GuarantorOrSuretyDetailEntityTypeConfiguration()
        {
            this.ToTable("GuarantorOrSuretyDetails", "HRM");
            this.HasKey<int>(o => o.GuarantorOrSuretyDetailId);
        }
    }
    public class GuarantorOrSuretyAttachmentEntityTypeConfiguration : EntityTypeConfiguration<GuarantorOrSuretyAttachment>
    {

        public GuarantorOrSuretyAttachmentEntityTypeConfiguration()
        {
            this.ToTable("GuarantorOrSuretyAttachments", "HRM");
            this.HasKey<int>(o => o.GuarantorOrSuretyAttachmentId);
        }
    }

    public class PromotionYearSettingEntityTypeConfiguration : EntityTypeConfiguration<PromotionYearSetting>
    {

        public PromotionYearSettingEntityTypeConfiguration()
        {
            this.ToTable("PromotionYearSettings", "HRM");
            this.HasKey<int>(o => o.PromotionYearSettingId);
        }
    }
    public class AttendanceEntityTypeConfiguration : EntityTypeConfiguration<Attendance>
    {

        public AttendanceEntityTypeConfiguration()
        {
            this.ToTable("Attendance", "HRM");
            this.HasKey<int>(o => o.AttendanceId);
        }
    }
    public class AttendanceExceptionEntityTypeConfiguration : EntityTypeConfiguration<AttendanceException>
    {

        public AttendanceExceptionEntityTypeConfiguration()
        {
            this.ToTable("AttendanceExceptions", "HRM");
            this.HasKey<int>(o => o.AttendanceExceptionId);
        }
    }

    public class DailyAttendanceSettingEntityTypeConfiguration : EntityTypeConfiguration<DailyAttendanceSetting>
    {

        public DailyAttendanceSettingEntityTypeConfiguration()
        {
            this.ToTable("DailyAttendanceSettings", "HRM");
            this.HasKey<int>(o => o.DailyAttendanceSettingId);
        }
    }

    public class AttendanceNewEntityTypeConfiguration : EntityTypeConfiguration<AttendanceNew>
    {
        public AttendanceNewEntityTypeConfiguration()
        {
            this.ToTable("AttendanceNew", "HRM");
            this.HasKey<int>(o => o.AttendanceNewId);
        }
    }

    public class AttendanceBioEntityTypeConfiguration : EntityTypeConfiguration<AttendanceBio>
    {
        public AttendanceBioEntityTypeConfiguration()
        {
            this.ToTable("AttendanceBio", "HRM");
            this.HasKey<int>(o => o.AttendanceBioId);
        }
    }

    public class WeeklyAttendanceEntityTypeConfiguration : EntityTypeConfiguration<WeeklyAttendance>
    {

        public WeeklyAttendanceEntityTypeConfiguration()
        {
            this.ToTable("WeeklyAttendance", "HRM");
            this.HasKey<int>(o => o.WeeklyAttendanceId);
        }
    }
    public class CommitmentEntityTypeConfiguration : EntityTypeConfiguration<Commitment>
    {

        public CommitmentEntityTypeConfiguration()
        {
            this.ToTable("Commitment", "HRM");
            this.HasKey<int>(o => o.CommitmentId);
        }



    }
    public class CompetencyEntityTypeConfiguration : EntityTypeConfiguration<Competency>
    {

        public CompetencyEntityTypeConfiguration()
        {
            this.ToTable("Competency", "HRM");
            this.HasKey<int>(o => o.CompetencyId);
        }
    }
    public class DisabilityEntityTypeConfiguration : EntityTypeConfiguration<Disability>
    {

        public DisabilityEntityTypeConfiguration()
        {
            this.ToTable("Disability", "HRM");
            this.HasKey<int>(o => o.DisabilityId);
        }
    }
    public class ExperienceEntityTypeConfiguration : EntityTypeConfiguration<Experience>
    {

        public ExperienceEntityTypeConfiguration()
        {
            this.ToTable("Experience", "HRM");
            this.HasKey<int>(o => o.ExperienceId);
        }
    }

    public class JobTitleCodeEntityTypeConfiguration : EntityTypeConfiguration<JobTitleCode>
    {

        public JobTitleCodeEntityTypeConfiguration()
        {
            this.ToTable("JobTitleCode", "HRM");
            this.HasKey<int>(o => o.JobTitleCodeId);
        }
    }
    public class LeaveEntitleStartingDateEntityTypeConfiguration : EntityTypeConfiguration<LeaveEntitleStartingDate>
    {
        public LeaveEntitleStartingDateEntityTypeConfiguration()
        {
            this.ToTable("LeaveEntitleStartingDate", "HRM");
            this.HasKey<int>(o => o.LeaveEntitleStartingDateId);
        }
    }
    public class LeaveEntitlementSettingEntityTypeConfiguration : EntityTypeConfiguration<LeaveEntitlementSetting>
    {
        public LeaveEntitlementSettingEntityTypeConfiguration()
        {
            this.ToTable("LeaveEntitlementSettings", "HRM");
            this.HasKey<int>(o => o.LeaveEntitlementSettingId);
        }
    }
    public class LeaveEntitlementHistoryEntityTypeConfiguration : EntityTypeConfiguration<LeaveEntitlementHistory>
    {

        public LeaveEntitlementHistoryEntityTypeConfiguration()
        {
            this.ToTable("LeaveEntitlementHistories", "HRM");
            this.HasKey<int>(o => o.LeaveEntitlementHistoryId);

        }
    }
    public class LeaveEntitleEntityTypeConfiguration : EntityTypeConfiguration<LeaveEntitle>
    {
        public LeaveEntitleEntityTypeConfiguration()
        {
            this.ToTable("LeaveEntitle", "HRM");
            this.HasKey<int>(o => o.LeaveEntitleId);
            //this.HasIndex(u => new { u.GlFiscalYearId, u.EmployeeId }).IsUnique(true);
        }


    }
    public class LeaveTypeSettingEntityTypeConfiguration : EntityTypeConfiguration<LeaveTypeSetting>
    {
        public LeaveTypeSettingEntityTypeConfiguration()
        {
            this.ToTable("LeaveTypeSetting", "HRM");
            this.HasKey<int>(o => o.LeaveTypeSettingId);
        }


    }

    public class LeaveEntityTypeConfiguration : EntityTypeConfiguration<Leave>
    {

        public LeaveEntityTypeConfiguration()
        {
            this.ToTable("Leave", "HRM");
            this.HasKey<int>(o => o.TakenLeaveId);

        }
    }

    public class LeaveNewAwajStartYearEntityTypeConfiguration : EntityTypeConfiguration<LeaveNewAwajStartYear>
    {

        public LeaveNewAwajStartYearEntityTypeConfiguration()
        {
            this.ToTable("LeaveNewAwajStartYear", "HRM");
            this.HasKey<int>(o => o.LeaveNewAwajStartYearId);

        }
    }

    public class IncommingEntityTypeConfiguration : EntityTypeConfiguration<Incomming>
    {

        public IncommingEntityTypeConfiguration()
        {
            this.ToTable("Incoming", "HRM");
            this.HasKey<int>(o => o.IncommingId);

        }
    }
    public class OutgoingEntityTypeConfiguration : EntityTypeConfiguration<Outgoing>
    {

        public OutgoingEntityTypeConfiguration()
        {
            this.ToTable("Outgoing", "HRM");
            this.HasKey<int>(o => o.OutgoingId);

        }
    }
    public class OutgoingStatusEntityTypeConfiguration : EntityTypeConfiguration<OutgoingStatus>
    {

        public OutgoingStatusEntityTypeConfiguration()
        {
            this.ToTable("OutgoingStatus", "HRM");
            this.HasKey<int>(o => o.StatusId);

        }
    }
    public class IncommingStatusEntityTypeConfiguration : EntityTypeConfiguration<IncommingStatus>
    {

        public IncommingStatusEntityTypeConfiguration()
        {
            this.ToTable("IncomingStatus", "HRM");
            this.HasKey<int>(o => o.IncommingStatusId);

        }
    }







    public class PlacementHistoryEntityTypeConfiguration : EntityTypeConfiguration<PlacementHistory>
    {

        public PlacementHistoryEntityTypeConfiguration()
        {
            this.ToTable("PlacementHistory", "HRM");
            this.HasKey<int>(o => o.PlacementHistoryId);

        }
    }
    public class PlacementEntityTypeConfiguration : EntityTypeConfiguration<Placement>
    {

        public PlacementEntityTypeConfiguration()
        {
            this.ToTable("Placement", "HRM");
            this.HasKey<int>(o => o.PlacementId);

        }
    }

    public class PlacementWorkflowSettingEntityTypeConfiguration : EntityTypeConfiguration<PlacementWorkflowSetting>
    {

        public PlacementWorkflowSettingEntityTypeConfiguration()
        {
            this.ToTable("PlacementWorkflowSettings", "HRM");
            this.HasKey<int>(o => o.PlacementWorkflowSettingId);

        }
    }

    public class PlacementAfterSystemtEntityTypeConfiguration : EntityTypeConfiguration<PlacementAfterSystem>
    {

        public PlacementAfterSystemtEntityTypeConfiguration()
        {
            this.ToTable("PlacementAfterSystem", "HRM");
            this.HasKey<int>(o => o.PlacementAfterSystemId);

        }
    }
    //public class PlanEntityTypeConfiguration : EntityTypeConfiguration<Plan>
    //{

    //    public PlanEntityTypeConfiguration()
    //    {
    //        this.ToTable("Plan", "HRM");
    //        this.HasKey<int>(o => o.PlanId);

    //    }
    //}
    public class ProhibitionEntityTypeConfiguration : EntityTypeConfiguration<Prohibition>
    {

        public ProhibitionEntityTypeConfiguration()
        {
            this.ToTable("Probation", "HRM");
            this.HasKey<int>(o => o.ProhibitionId);

        }
    }



    public class TerminationEntityTypeConfiguration : EntityTypeConfiguration<Termination>
    {
        public TerminationEntityTypeConfiguration()
        {
            this.ToTable("Termination", "HRM");
            this.HasKey<int>(o => o.TerminationId);
        }
    }
    public class TerminationPMValidationEntityTypeConfiguration : EntityTypeConfiguration<TerminationPMValidation>
    {
        public TerminationPMValidationEntityTypeConfiguration()
        {
            this.ToTable("TerminationPMValidations", "HRM");
            this.HasKey<int>(o => o.TerminationPMValidationId);
        }
    }

    public class TerminationHRManagerValidationEntityTypeConfiguration : EntityTypeConfiguration<TerminationHRManagerValidation>
    {
        public TerminationHRManagerValidationEntityTypeConfiguration()
        {
            this.ToTable("TerminationHRManagerValidations", "HRM");
            this.HasKey<int>(o => o.TerminationHRManagerValidationId);
        }
    }

    public class StaffDebtorBalanceEntityTypeConfiguration : EntityTypeConfiguration<StaffDebtorBalance>
    {
        public StaffDebtorBalanceEntityTypeConfiguration()
        {
            this.ToTable("StaffDebtorBalance", "HRM");
            this.HasKey<int>(o => o.StaffDebtorBalanceId);
        }
    }

    public class StepEntityTypeConfiguration : EntityTypeConfiguration<Step>
    {

        public StepEntityTypeConfiguration()
        {
            this.ToTable("Step", "HRM");
            this.HasKey<int>(o => o.StepId);
        }
    }
    public class RankEntityTypeConfiguration : EntityTypeConfiguration<Rank>
    {

        public RankEntityTypeConfiguration()
        {
            this.ToTable("Rank", "HRM");
            this.HasKey<int>(o => o.RankId);
        }
    }
    public class SalaryStructureVersionEntityTypeConfiguration : EntityTypeConfiguration<SalaryStructureVersion>
    {

        public SalaryStructureVersionEntityTypeConfiguration()
        {
            this.ToTable("SalaryStructureVersion", "HRM");
            this.HasKey<int>(o => o.SalaryStructureVersionId);
        }
    }
    public class SalaryEntityTypeConfiguration : EntityTypeConfiguration<Salary>
    {

        public SalaryEntityTypeConfiguration()
        {
            this.ToTable("Salary", "HRM");
            this.HasKey<int>(o => o.SalaryStructureId);
        }
    }
    public class EmpTrainingEducationEntityTypeConfiguration : EntityTypeConfiguration<Training>
    {

        public EmpTrainingEducationEntityTypeConfiguration()
        {
            this.ToTable("Training", "HRM");
            this.HasKey<int>(o => o.EmpEduTrainId);
        }
    }
    public class NeedAssessmentEntityTypeConfiguration : EntityTypeConfiguration<NeedAssessment>
    {

        public NeedAssessmentEntityTypeConfiguration()
        {
            this.ToTable("NeedAssessment", "HRM");
            this.HasKey<int>(o => o.AssesmentId);
        }
    }

    public class NeedAssessmentAttachmentEntityTypeConfiguration : EntityTypeConfiguration<NeedAssessmentAttachment>
    {

        public NeedAssessmentAttachmentEntityTypeConfiguration()
        {
            this.ToTable("NeedAssessmentAttachments", "HRM");
            this.HasKey<int>(o => o.NeedAssessmentAttachmentId);
        }
    }

    public class EmployeesInAssessmentEntityTypeConfiguration : EntityTypeConfiguration<EmployeesInAssessment>
    {

        public EmployeesInAssessmentEntityTypeConfiguration()
        {
            this.ToTable("EmployeesInAssessment", "HRM");
            this.HasKey<int>(o => o.EmployeesInAssessmentId);
        }
    }
    public class CourseTrackingEntityTypeConfiguration : EntityTypeConfiguration<CourseTracking>
    {

        public CourseTrackingEntityTypeConfiguration()
        {
            this.ToTable("CourseTracking", "HRM");
            this.HasKey<int>(o => o.CourseTrackingId);
        }
    }

    public class TrainingEvaluationCriteriaCategoryEntityTypeConfiguration : EntityTypeConfiguration<TrainingEvaluationCriteriaCategory>
    {

        public TrainingEvaluationCriteriaCategoryEntityTypeConfiguration()
        {
            this.ToTable("TrainingEvaluationCriteriaCategory", "HRM");
            this.HasKey<int>(o => o.TrainingEvaluationSettingCategoryId);
        }
    }

    public class TrainingEvaluationCriteriaEntityTypeConfiguration : EntityTypeConfiguration<TrainingEvaluationCriteria>
    {

        public TrainingEvaluationCriteriaEntityTypeConfiguration()
        {
            this.ToTable("TrainingEvaluationCriteria", "HRM");
            this.HasKey<int>(o => o.TrainingEvaluationCriteriaId);
        }
    }

    public class TrainingEvaluationEntityTypeConfiguration : EntityTypeConfiguration<TrainingEvaluation>
    {

        public TrainingEvaluationEntityTypeConfiguration()
        {
            this.ToTable("TrainingEvaluation", "HRM");
            this.HasKey<int>(o => o.TrainingEvaluationId);
        }
    }

    public class MainCompanyEntityTypeConfiguration : EntityTypeConfiguration<MainCompany>
    {

        public MainCompanyEntityTypeConfiguration()
        {
            this.ToTable("MainCompany", "HRM");
            this.HasKey<int>(o => o.MainCompanyId);
        }
    }

    public class ProvidedTrainingEntityTypeConfiguration : EntityTypeConfiguration<ProvidedTraining>
    {

        public ProvidedTrainingEntityTypeConfiguration()
        {
            this.ToTable("ProvidedTrainings", "HRM");
            this.HasKey<int>(o => o.ProvidedTrainingId);
        }
    }

    public class EmployeeEntityTypeConfiguration : EntityTypeConfiguration<Person_Employee>
    {

        public EmployeeEntityTypeConfiguration()
        {
            this.ToTable("Employee", "HRM");
            this.HasKey<int>(o => o.EmployeeId);

        }
    }
    public class LegacyEmployeeEntityTypeConfiguration : EntityTypeConfiguration<LegacyEmployee>
    {

        public LegacyEmployeeEntityTypeConfiguration()
        {
            this.ToTable("LegacyEmployees", "HRM");
            this.HasKey<int>(o => o.LegacyEmployeeId);

        }
    }

    public class PasswordResetRequestEntityTypeConfiguration : EntityTypeConfiguration<PasswordResetRequest>
    {

        public PasswordResetRequestEntityTypeConfiguration()
        {
            this.ToTable("PasswordResetRequests", "HRM");
            this.HasKey<int>(o => o.PasswordResetRequestId);

        }
    }

    public class MainCompanyStructureEntityTypeConfiguration : EntityTypeConfiguration<MainCompanyStructure>
    {

        public MainCompanyStructureEntityTypeConfiguration()
        {
            this.ToTable("MainCompanyStructure", "HRM");
            this.HasKey<int>(o => o.OrgStructureId);
        }
    }
    public class CosignEntityTypeConfiguration : EntityTypeConfiguration<Cosign>
    {

        public CosignEntityTypeConfiguration()
        {
            this.ToTable("Cosign", "HRM");
            this.HasKey<int>(o => o.CosignId);
        }
    }


    public class RegionNationEntityTypeConfiguration : EntityTypeConfiguration<RegionNation>
    {

        public RegionNationEntityTypeConfiguration()
        {
            this.ToTable("RegionNation", "HRM");
            this.HasKey<int>(o => o.RegionNationId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class ClearanceEntityTypeConfiguration : EntityTypeConfiguration<Clearance>
    {

        public ClearanceEntityTypeConfiguration()
        {
            this.ToTable("Clearance", "HRM");
            this.HasKey<int>(o => o.ClearanceId);
        }
    }

    public class LeavePlanEntityTypeConfiguration : EntityTypeConfiguration<LeavePlan>
    {

        public LeavePlanEntityTypeConfiguration()
        {
            this.ToTable("LeavePlan", "HRM");
            this.HasKey<int>(o => o.LeavePlanId);
        }
    }
    public class LeavePlanValidationEntityTypeConfiguration : EntityTypeConfiguration<LeavePlanValidation>
    {

        public LeavePlanValidationEntityTypeConfiguration()
        {
            this.ToTable("LeavePlanValidation", "HRM");
            this.HasKey<int>(o => o.LeavePlanValidationId);
        }
    }
    public class LeavePlanFinalValidationEntityTypeConfiguration : EntityTypeConfiguration<LeavePlanFinalValidation>
    {

        public LeavePlanFinalValidationEntityTypeConfiguration()
        {
            this.ToTable("LeavePlanFinalValidation", "HRM");
            this.HasKey<int>(o => o.LeavePlanFinalValidationId);
        }
    }
    public class TrainingPlanEntityTypeConfiguration : EntityTypeConfiguration<TrainingPlan>
    {

        public TrainingPlanEntityTypeConfiguration()
        {
            this.ToTable("TrainingPlan", "HRM");
            this.HasKey<int>(o => o.TrainingPlanId);
        }
    }


    public class ExitInterviewEntityTypeConfiguration : EntityTypeConfiguration<ExitInterview>
    {

        public ExitInterviewEntityTypeConfiguration()
        {
            this.ToTable("ExitInterview", "HRM");
            this.HasKey<int>(o => o.ExitIntervId);
        }
    }
    public class LeavePaidEntityTypeConfiguration : EntityTypeConfiguration<LeavePaid>
    {
        public LeavePaidEntityTypeConfiguration()
        {
            this.ToTable("LeavePaid", "HRM");
            this.HasKey<int>(o => o.LeavePaidId);

            // Set precision/scale for all decimal properties
            this.Property(p => p.AmountInBirr).HasPrecision(18, 2);
            this.Property(p => p.BasicSalary).HasPrecision(18, 2);
            this.Property(p => p.DailyRate).HasPrecision(18, 2);
            this.Property(p => p.GrossAmount).HasPrecision(18, 2);
            this.Property(p => p.TaxPayable).HasPrecision(18, 2);
            this.Property(p => p.NetPay).HasPrecision(18, 2);
        }
    }
    public class LeavePaidDeductionDetailEntityTypeConfiguration : EntityTypeConfiguration<LeavePaidDeductionDetail>
    {
        public LeavePaidDeductionDetailEntityTypeConfiguration()
        {
            this.ToTable("LeavePaidDeductionDetails", "HRM");
            this.HasKey<int>(o => o.LeavePaidDeductionDetailId);

            this.Property(p => p.DeductedDays).HasPrecision(18, 2);
        }
    }
    public class SalaryJobTitleEntityTypeConfiguration : EntityTypeConfiguration<SalaryJobTitle>
    {

        public SalaryJobTitleEntityTypeConfiguration()
        {
            this.ToTable("SalaryJobTitle", "HRM");
            this.HasKey<int>(o => o.SalJobTiId);
        }
    }
    public class OtherBenefitEntityTypeConfiguration : EntityTypeConfiguration<OtherBenefit>
    {

        public OtherBenefitEntityTypeConfiguration()
        {
            this.ToTable("OtherBenefits", "HRM");
            this.HasKey<int>(o => o.OtherBenefitId);
        }
    }
    public class EmployeeBenefitEntityTypeConfiguration : EntityTypeConfiguration<EmployeeBenefit>
    {

        public EmployeeBenefitEntityTypeConfiguration()
        {
            this.ToTable("EmployeeBenefits", "HRM");
            this.HasKey<int>(o => o.EmployeeBenefitId);
        }
    }
    public class EmployeeBenefitDetailEntityTypeConfiguration : EntityTypeConfiguration<EmployeeBenefitDetail>
    {

        public EmployeeBenefitDetailEntityTypeConfiguration()
        {
            this.ToTable("EmployeeBenefitDetails", "HRM");
            this.HasKey<int>(o => o.EmployeeBenefitDetailId);
        }
    }
    public class DisciplineMeasuresEntityTypeConfiguration : EntityTypeConfiguration<DisciplineMeasures>
    {

        public DisciplineMeasuresEntityTypeConfiguration()
        {
            this.ToTable("DisciplineMeasures", "HRM");
            this.HasKey<int>(o => o.MeasuresId);
        }
    }
    public class DisciplineRecordEntityTypeConfiguration : EntityTypeConfiguration<DisciplineRecord>
    {

        public DisciplineRecordEntityTypeConfiguration()
        {
            this.ToTable("DisciplineRecord", "HRM");
            this.HasKey<int>(o => o.RecordId);
        }
    }
    public class DisciplineMeasuerByMoneyEntityTypeConfiguration : EntityTypeConfiguration<DisciplineMeasuerByMoney>
    {

        public DisciplineMeasuerByMoneyEntityTypeConfiguration()
        {
            this.ToTable("DisciplineMeasureByMoney", "HRM");
            this.HasKey<int>(o => o.DisciplineMeasuerByMoneyId);
        }
    }
   
    public class CommitteeEntityTypeConfiguration : EntityTypeConfiguration<Committee>
    {

        public CommitteeEntityTypeConfiguration()
        {
            this.ToTable("Committee", "HRM");
            this.HasKey<int>(o => o.CommitteeId);
        }
    }
    public class CommitteeMemberEntityTypeConfiguration : EntityTypeConfiguration<CommitteeMember>
    {

        public CommitteeMemberEntityTypeConfiguration()
        {
            this.ToTable("CommitteeMember", "HRM");
            this.HasKey<int>(o => o.CommitteeMemberId);
        }
    }
    public class CommitteMinuteEntityTypeConfiguration : EntityTypeConfiguration<CommitteMinute>
    {

        public CommitteMinuteEntityTypeConfiguration()
        {
            this.ToTable("CommitteMinute", "HRM");
            this.HasKey<int>(o => o.CommitteMinuteId);
        }
    }
    public class DisciplineMeasuresDetailEntityTypeConfiguration : EntityTypeConfiguration<DisciplineMeasuresDetail>
    {

        public DisciplineMeasuresDetailEntityTypeConfiguration()
        {
            this.ToTable("DisciplineMeasuresDetail", "HRM");
            this.HasKey<int>(o => o.DetailMeasuresId);
        }
    }
    public class EmergencyEntityTypeConfiguration : EntityTypeConfiguration<Emergency>
    {

        public EmergencyEntityTypeConfiguration()
        {
            this.ToTable("Emergency", "HRM");
            this.HasKey<int>(o => o.EmergencyId);
        }
    }
    public class ContractEntityTypeConfiguration : EntityTypeConfiguration<Contract>
    {
        public ContractEntityTypeConfiguration()
        {
            this.ToTable("Contract", "HRM");
            this.HasKey<int>(o => o.ContractId);
        }
    }

    public class EmpDisciplinaryHearingEntityTypeConfiguration : EntityTypeConfiguration<EmpDisciplinaryHearing>
    {
        public EmpDisciplinaryHearingEntityTypeConfiguration()
        {
            this.ToTable("EmpDisciplinaryHearing", "HRM");
            this.HasKey<int>(o => o.DisciplinaryHearingId);
        }
    }

    public class EmpDisciplineMeasureEntityTypeConfiguration : EntityTypeConfiguration<EmpDisciplineMeasure>
    {
        public EmpDisciplineMeasureEntityTypeConfiguration()
        {
            this.ToTable("EmpDisciplineMeasure", "HRM");
            this.HasKey<int>(o => o.EmpDisciplineMeasureId);
        }
    }
    public class DisciplinaryCaseEntityTypeConfiguration : EntityTypeConfiguration<DisciplinaryCase>
    {
        public DisciplinaryCaseEntityTypeConfiguration()
        {
            this.ToTable("DisciplinaryCase", "HRM");
            this.HasKey<int>(o => o.DisciplinaryCaseId);
        }
    }
    public class DisciplineMeasuresTypeEntityTypeConfiguration : EntityTypeConfiguration<DisciplineMeasuresType>
    {
        public DisciplineMeasuresTypeEntityTypeConfiguration()
        {
            this.ToTable("DisciplineMeasuresType", "HRM");
            this.HasKey<int>(o => o.DisciplineTypeId);
        }
    }
    public class AbsenceEntityTypeConfiguration : EntityTypeConfiguration<Absence>
    {
        public AbsenceEntityTypeConfiguration()
        {
            this.ToTable("Absence", "HRM");
            this.HasKey<int>(o => o.AbsenceId);
        }
    }
    public class EmployeeAttendanceSummaryEntityTypeConfiguration : EntityTypeConfiguration<EmployeeAttendanceSummary>
    {
        public EmployeeAttendanceSummaryEntityTypeConfiguration()
        {
            this.ToTable("EmployeeAttendanceSummary", "HRM");
            this.HasKey<int>(o => o.EmployeeAttendanceSummaryId);
            this.Property(o => o.LateComing).HasPrecision(8, 6);
            this.Property(o => o.Absent).HasPrecision(8, 6);
            this.Property(o => o.OvertimeDays).HasPrecision(8, 6);
            this.Property(o => o.OvertimeHours).HasPrecision(8, 6);
        }
    }
    public class LateCommingEntityTypeConfiguration : EntityTypeConfiguration<LateComming>
    {
        public LateCommingEntityTypeConfiguration()
        {
            this.ToTable("LateComing", "HRM");
            this.HasKey<int>(o => o.LateCommingId);
        }
    }
    public class LateComingSummaryEntityTypeConfiguration : EntityTypeConfiguration<LateComingSummary>
    {
        public LateComingSummaryEntityTypeConfiguration()
        {
            this.ToTable("LateComingSummary", "HRM");
            this.HasKey<int>(o => o.LateComingSummaryId);
            // matches Payroll.EmpPayrollNoDaysWorked.Days precision so the source TotalDays isn't rounded before deduction
            this.Property(o => o.TotalDays).HasPrecision(10, 5);
        }
    }



    public class AllowanceTypeRuleEntityTypeConfiguration : EntityTypeConfiguration<AllowanceTypeRule>
    {
        public AllowanceTypeRuleEntityTypeConfiguration()
        {
            this.ToTable("AllowanceTypeRule", "HRM");
            this.HasKey<int>(o => o.AllowanceTypeRuleId);
        }
    }
    public class AllowanceGenerationSettingEntityTypeConfiguration : EntityTypeConfiguration<AllowanceGenerationSetting>
    {
        public AllowanceGenerationSettingEntityTypeConfiguration()
        {
            this.ToTable("AllowanceGenerationSettings", "HRM");
            this.HasKey<int>(o => o.AllowanceGenerationSettingId);
        }
    }




    #endregion
    #region SelfCare
    //public class MedicalRequestEntityTypeConfiguration : EntityTypeConfiguration<MedicalRequest>
    //{
    //    public MedicalRequestEntityTypeConfiguration()
    //    {
    //        this.ToTable("MedicalRequest", "Health");
    //        this.HasKey<int>(o => o.MedicalRequestId);
    //    }
    //}

    public class RequestesStatusEntityTypeConfiguration : EntityTypeConfiguration<RequestesStatus>
    {
        public RequestesStatusEntityTypeConfiguration()
        {
            this.ToTable("RequestesStatus", "HRM");
            this.HasKey<int>(o => o.RequestStatusId);
        }
    }
    public class PromotionRequestEntityTypeConfiguration : EntityTypeConfiguration<PromotionRequest>
    {
        public PromotionRequestEntityTypeConfiguration()
        {
            this.ToTable("PromotionRequest", "HRM");
            this.HasKey<int>(o => o.PromotionRequestId);
        }
    }


    public class ApplicantTrainingEntityTypeConfiguration : EntityTypeConfiguration<ApplicantTraining>
    {
        public ApplicantTrainingEntityTypeConfiguration()
        {
            this.ToTable("ApplicantTraining", "HRM");
            this.HasKey<int>(o => o.ApplicantTrainingId);
        }
    }
    public class TrainingApplicantAttachmentEntityTypeConfiguration : EntityTypeConfiguration<TrainingApplicantAttachment>
    {
        public TrainingApplicantAttachmentEntityTypeConfiguration()
        {
            this.ToTable("TrainingApplicantAttachments", "HRM");
            this.HasKey<int>(o => o.TrainingApplicantAttachmentId);
        }
    }
    public class TrainingApplicantSecondValidationEntityTypeConfiguration : EntityTypeConfiguration<TrainingApplicantSecondValidation>
    {
        public TrainingApplicantSecondValidationEntityTypeConfiguration()
        {
            this.ToTable("TrainingApplicantSecondValidations", "HRM");
            this.HasKey<int>(o => o.TrainingApplicantSecondValidationId);
        }
    }


    public class LeaveRequestEntityTypeConfiguration : EntityTypeConfiguration<LeaveRequest>
    {
        public LeaveRequestEntityTypeConfiguration()
        {
            this.ToTable("LeaveRequest", "HRM");
            this.HasKey<int>(o => o.LeaveRequestId);
        }
    }
    public class LeaveRequestAttachementEntityTypeConfiguration : EntityTypeConfiguration<LeaveRequestAttachement>
    {
        public LeaveRequestAttachementEntityTypeConfiguration()
        {
            this.ToTable("LeaveRequestAttachements", "HRM");
            this.HasKey<int>(o => o.LeaveRequestAttachementId);
        }
    }

    public class LeaveRequestPMValidationEntityTypeConfiguration : EntityTypeConfiguration<LeaveRequestPMValidation>
    {
        public LeaveRequestPMValidationEntityTypeConfiguration()
        {
            this.ToTable("LeaveRequestPMValidations", "HRM");
            this.HasKey<int>(o => o.LeaveRequestPMValidationId);
        }
    }
    public class LeaveRequestHRManagerValidationEntityTypeConfiguration : EntityTypeConfiguration<LeaveRequestHRManagerValidation>
    {
        public LeaveRequestHRManagerValidationEntityTypeConfiguration()
        {
            this.ToTable("LeaveRequestHRManagerValidations", "HRM");
            this.HasKey<int>(o => o.LeaveRequestHRManagerValidationId);
        }
    }

    public class ExperienceRequestEntityTypeConfiguration : EntityTypeConfiguration<ExperienceRequest>
    {
        public ExperienceRequestEntityTypeConfiguration()
        {
            this.ToTable("ExperienceRequest", "HRM");
            this.HasKey<int>(o => o.ExperienceRequestId);
        }
    }
    public class TrainingApplicantEntityTypeConfiguration : EntityTypeConfiguration<TrainingApplicant>
    {

        public TrainingApplicantEntityTypeConfiguration()
        {
            this.ToTable("TrainingApplicant", "HRM");
            this.HasKey<int>(o => o.TrainingApplicantId);
        }
    }
    public class EducationRequestEntityTypeConfiguration : EntityTypeConfiguration<EducationRequest>
    {

        public EducationRequestEntityTypeConfiguration()
        {
            this.ToTable("EducationRequest", "HRM");
            this.HasKey<int>(o => o.EducationRequestId);
        }
    }
    public class EducationRequestAttachementEntityTypeConfiguration : EntityTypeConfiguration<EducationRequestAttachement>
    {

        public EducationRequestAttachementEntityTypeConfiguration()
        {
            this.ToTable("EducationRequestAttachement", "HRM");
            this.HasKey<int>(o => o.EducationRequestAttachementId);
        }
    }
    public class OverTimeRequestEntityTypeConfiguration : EntityTypeConfiguration<OverTimeRequest>
    {

        public OverTimeRequestEntityTypeConfiguration()
        {
            this.ToTable("OverTimeRequest", "HRM");
            this.HasKey<int>(o => o.OverTimeRequestId);
            this.Property(o => o.TotalHours).HasPrecision(18, 5);
            this.Property(o => o.TotalDays).HasPrecision(18, 5);
            this.Property(o => o.GrandTotalHours).HasPrecision(18, 5);
            this.Property(o => o.ConvertedTotalDays).HasPrecision(18, 5);
        }
    }
    public class OverTimeRequestEmployeeEntityTypeConfiguration : EntityTypeConfiguration<OverTimeRequestEmployee>
    {

        public OverTimeRequestEmployeeEntityTypeConfiguration()
        {
            this.ToTable("OverTimeRequestEmployee", "HRM");
            this.HasKey<int>(o => o.OverTimeRequestEmployeeId);
        }
    }
    #endregion

    #region Performance
    public class MainCompanyEvalutionSettingEntityTypeConfiguration : EntityTypeConfiguration<MainCompanyEvalutionSetting>
    {

        public MainCompanyEvalutionSettingEntityTypeConfiguration()
        {
            this.ToTable("MainCompanyEvalutionSettings", "Performance");
            this.HasKey<int>(o => o.MainCompanyEvalutionSettingId);
        }
    }
    public class MeasurementDefinitionEntityTypeConfiguration : EntityTypeConfiguration<MeasurementDefinition>
    {

        public MeasurementDefinitionEntityTypeConfiguration()
        {
            this.ToTable("MeasurementDefinition", "Performance");
            this.HasKey<int>(o => o.MeasurementDefinitionId);
        }
    }
    public class MeasurementEntityTypeConfiguration : EntityTypeConfiguration<Measurement>
    {

        public MeasurementEntityTypeConfiguration()
        {
            this.ToTable("Measurement", "Performance");
            this.HasKey<int>(o => o.MeasurementId);
        }
    }
    public class PerformanceActionEntityTypeConfiguration : EntityTypeConfiguration<PerformanceAction>
    {

        public PerformanceActionEntityTypeConfiguration()
        {
            this.ToTable("PerformanceAction", "Performance");
            this.HasKey<int>(o => o.ActionId);
        }
    }
    public class EmployeePerformanceEntityTypeConfiguration : EntityTypeConfiguration<EmployeePerformance>
    {

        public EmployeePerformanceEntityTypeConfiguration()
        {
            this.ToTable("EmployeePerformance", "Performance");
            this.HasKey<int>(o => o.EmployeePerformanceId);
        }
    }
    public class EmployeePerformanceSummaryEntityTypeConfiguration : EntityTypeConfiguration<EmployeePerformanceSummary>
    {

        public EmployeePerformanceSummaryEntityTypeConfiguration()
        {
            this.ToTable("EmployeePerformanceSummary", "Performance");
            this.HasKey<int>(o => o.EmployeePerformanceSummaryId);
        }
    }

    public class EmployeeMeasurementEntityTypeConfiguration : EntityTypeConfiguration<EmployeeMeasurement>
    {

        public EmployeeMeasurementEntityTypeConfiguration()
        {
            this.ToTable("EmployeeMeasurement", "Performance");
            this.HasKey<int>(o => o.EmpMeasurementId);
        }
    }
    public class InternalCustomerDetailEntityTypeConfiguration : EntityTypeConfiguration<InternalCustomerDetail>
    {

        public InternalCustomerDetailEntityTypeConfiguration()
        {
            this.ToTable("InternalCustomerDetail", "Performance");
            this.HasKey<int>(o => o.InternalCustomerDetailId);
        }
    }
    public class CriteriaCategoryEntityTypeConfiguration : EntityTypeConfiguration<CriteriaCategory>
    {

        public CriteriaCategoryEntityTypeConfiguration()
        {
            this.ToTable("CriteriaCategories", "HRM");
            this.HasKey<int>(o => o.CriteriaCategoryId);

        }
    }
    //public class EmployeeMeasurementDefinitionEntityTypeConfiguration : EntityTypeConfiguration<EmployeeMeasurementDefinition>
    //{

    //    public EmployeeMeasurementDefinitionEntityTypeConfiguration()
    //    {
    //        this.ToTable("EmployeeMeasurementDefinition", "Performance");
    //        this.HasKey<int>(o => o.EmpMeasurementDefinitionId);
    //    }
    //}
    public class PerformanceSettingEntityTypeConfiguration : EntityTypeConfiguration<PerformanceSetting>
    {

        public PerformanceSettingEntityTypeConfiguration()
        {
            this.ToTable("PerformanceSettings", "Performance");
            this.HasKey<int>(o => o.PerformanceSettingId);
        }
    }

    public class EvaluationCriteriaActionTypeEntityTypeConfiguration : EntityTypeConfiguration<EvaluationCriteriaActionType>
    {
        public EvaluationCriteriaActionTypeEntityTypeConfiguration()
        {
            this.ToTable("EvaluationCriteriaActionType", "Performance");
            this.HasKey<int>(o => o.EvaluationCriteriaActionTypeId);
        }
    }

    public class PerformanceRequestPeriodEntityTypeConfiguration : EntityTypeConfiguration<PerformanceRequestPeriod>
    {

        public PerformanceRequestPeriodEntityTypeConfiguration()
        {
            this.ToTable("PerformanceRequestPeriods", "Performance");
            this.HasKey<int>(o => o.PerformanceRequestPeriodId);

        }
    }
    public class PeerToPeerPerformanceEvaluationEntityTypeConfiguration : EntityTypeConfiguration<PeerToPeerPerformanceEvaluation>
    {

        public PeerToPeerPerformanceEvaluationEntityTypeConfiguration()
        {
            this.ToTable("PeerToPeerPerformanceEvaluations", "Performance");
            this.HasKey<int>(o => o.PeerToPeerPerformanceEvaluationId);

        }
    }
    public class InternalCustomerPerformanceEvaluationEntityTypeConfiguration : EntityTypeConfiguration<InternalCustomerPerformanceEvaluation>
    {

        public InternalCustomerPerformanceEvaluationEntityTypeConfiguration()
        {
            this.ToTable("InternalCustomerPerformanceEvaluations", "Performance");
            this.HasKey<int>(o => o.InternalCustomerPerformanceEvaluationId);

        }
    }
    public class InternalCustomerEntityTypeConfiguration : EntityTypeConfiguration<InternalCustomer>
    {

        public InternalCustomerEntityTypeConfiguration()
        {
            this.ToTable("InternalCustomers", "Performance");
            this.HasKey<int>(o => o.InternalCustomerId);

        }
    }
    public class InternalCustomerEmployeePerformanceEntityTypeConfiguration : EntityTypeConfiguration<InternalCustomerEmployeePerformance>
    {

        public InternalCustomerEmployeePerformanceEntityTypeConfiguration()
        {
            this.ToTable("InternalCustomerEmployeePerformances", "Performance");
            this.HasKey<int>(o => o.InternalCustomerEmployeePerformanceId);

        }
    }
    public class InternalCustomerEmployeeMeasurementEntityTypeConfiguration : EntityTypeConfiguration<InternalCustomerEmployeeMeasurement>
    {

        public InternalCustomerEmployeeMeasurementEntityTypeConfiguration()
        {
            this.ToTable("InternalCustomerEmployeeMeasurements", "Performance");
            this.HasKey<int>(o => o.InternalCustomerEmployeeMeasurementId);

        }
    }
    public class InternalCustomerEmployeePerformanceScoreEntityTypeConfiguration : EntityTypeConfiguration<InternalCustomerEmployeePerformanceScore>

    {

        public InternalCustomerEmployeePerformanceScoreEntityTypeConfiguration()
        {
            this.ToTable("InternalCustomerEmployeePerformanceScores", "Performance");
            this.HasKey<int>(o => o.InternalCustomerEmployeePerformanceScoreId);

        }
    }

    public class PeerToPeerEmployeePerformanceEntityTypeConfiguration : EntityTypeConfiguration<PeerToPeerEmployeePerformance>
    {

        public PeerToPeerEmployeePerformanceEntityTypeConfiguration()
        {
            this.ToTable("PeerToPeerEmployeePerformances", "Performance");
            this.HasKey<int>(o => o.PeerToPeerEmployeePerformanceId);

        }
    }
    public class PeerEmployeeMeasurementEntityTypeConfiguration : EntityTypeConfiguration<PeerEmployeeMeasurement>
    {

        public PeerEmployeeMeasurementEntityTypeConfiguration()
        {
            this.ToTable("PeerEmployeeMeasurements", "Performance");
            this.HasKey<int>(o => o.PeerEmployeeMeasurementId);

        }
    }
    public class PeerToPeerEmployeePerformanceScoreEntityTypeConfiguration : EntityTypeConfiguration<PeerToPeerEmployeePerformanceScore>
    {

        public PeerToPeerEmployeePerformanceScoreEntityTypeConfiguration()
        {
            this.ToTable("PeerToPeerEmployeePerformanceScores", "Performance");
            this.HasKey<int>(o => o.PeerToPeerEmployeePerformanceScoreId);

        }
    }
    #endregion


    #region Company
   
    public class BranchRankMappingEntityTypeConfiguration : EntityTypeConfiguration<BranchRankMapping>
    {

        public BranchRankMappingEntityTypeConfiguration()
        {
            this.ToTable("BranchRankMapping", "HRM");
            this.HasKey<int>(o => o.BranchRankMappingId);
        }
    }
    public class JobTitleAndRankMappingEntityTypeConfiguration : EntityTypeConfiguration<JobTitleAndRankMapping>
    {

        public JobTitleAndRankMappingEntityTypeConfiguration()
        {
            this.ToTable("JobTitleAndRankMappings", "HRM");
            this.HasKey<int>(o => o.JobTitleAndRankMappingId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class HolidayEntityTypeConfiguration : EntityTypeConfiguration<Holiday>
    {

        public HolidayEntityTypeConfiguration()
        {
            this.ToTable("Holiday", "HRM");
            this.HasKey<int>(o => o.HolidayId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class WeekendEntityTypeConfiguration : EntityTypeConfiguration<Weekend>
    {

        public WeekendEntityTypeConfiguration()
        {
            this.ToTable("Weekend", "HRM");
            this.HasKey<int>(o => o.WeekendId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class WeekendExceptionEntityTypeConfiguration : EntityTypeConfiguration<WeekendException>
    {

        public WeekendExceptionEntityTypeConfiguration()
        {
            this.ToTable("WeekendExceptions", "HRM");
            this.HasKey<int>(o => o.WeekendExceptionId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class JobTitleMappingEntityTypeConfiguration : EntityTypeConfiguration<JobTitleMapping>
    {

        public JobTitleMappingEntityTypeConfiguration()
        {
            this.ToTable("JobTitleMapping", "HRM");
            this.HasKey<int>(o => o.JobTitleMappingId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class HolidayGiftRateSettingEntityTypeConfiguration : EntityTypeConfiguration<HolidayGiftRateSetting>
    {

        public HolidayGiftRateSettingEntityTypeConfiguration()
        {
            this.ToTable("HolidayGiftRateSettings", "HRM");
            this.HasKey<int>(o => o.HolidayGiftRateSettingId);
        }
    }

    public class HolidayGiftEmployeeEntityTypeConfiguration : EntityTypeConfiguration<HolidayGiftEmployee>
    {

        public HolidayGiftEmployeeEntityTypeConfiguration()
        {
            this.ToTable("HolidayGiftEmployee", "HRM");
            this.HasKey<int>(o => o.HolidayGiftEmployeeId);
        }
    }

    public class JobTitleMappingTempEntityTypeConfiguration : EntityTypeConfiguration<JobTitleMappingTemp>
    {

        public JobTitleMappingTempEntityTypeConfiguration()
        {
            this.ToTable("JobTitleMappingTemp", "HRM");
            this.HasKey<int>(o => o.JobTitleMappingId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class BranchCostCenterEntityTypeConfiguration : EntityTypeConfiguration<BranchCostCenter>
    {

        public BranchCostCenterEntityTypeConfiguration()
        {
            this.ToTable("BranchCostCenter", "HRM");
            this.HasKey<int>(o => o.BranchCostCenterId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }

    public class JobTitleMappingDetailEntityTypeConfiguration : EntityTypeConfiguration<JobTitleMappingDetail>
    {

        public JobTitleMappingDetailEntityTypeConfiguration()
        {
            this.ToTable("JobTitleMappingDetail", "HRM");
            this.HasKey<int>(o => o.JobTitleMappingDetailId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class JobTitleMappingMianEntityTypeConfiguration : EntityTypeConfiguration<JobTitleMappingMain>
    {

        public JobTitleMappingMianEntityTypeConfiguration()
        {
            this.ToTable("JobTitleMappingMain", "HRM");
            this.HasKey<int>(o => o.JobTitleMappingMianId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }

    public class JobTitleMappingDetailTempEntityTypeConfiguration : EntityTypeConfiguration<JobTitleMappingDetailTemp>
    {

        public JobTitleMappingDetailTempEntityTypeConfiguration()
        {
            this.ToTable("JobTitleMappingDetailTemp", "HRM");
            this.HasKey<int>(o => o.JobTitleMappingDetailId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    public class JobTitleMappingMainTempEntityTypeConfiguration : EntityTypeConfiguration<JobTitleMappingMainTemp>
    {

        public JobTitleMappingMainTempEntityTypeConfiguration()
        {
            this.ToTable("JobTitleMappingMainTemp", "HRM");
            this.HasKey<int>(o => o.JobTitleMappingMianId);
            // this.HasRequired<int>(s => s.JobTitle);
        }
    }
    #endregion
    #region Recruitment

    public class ScreeningSettingEntityTypeConfiguration : EntityTypeConfiguration<ScreeningSetting>
    {
        public ScreeningSettingEntityTypeConfiguration()
        {
            this.ToTable("ScreeningSettings", "HRM");
            this.HasKey<int>(o => o.ScreeningSettingId);
        }
    }
    public class InternalApplicantsEntityTypeConfiguration : EntityTypeConfiguration<InternalApplicants>
    {
        public InternalApplicantsEntityTypeConfiguration()
        {
            this.ToTable("InternalApplicants", "HRM");
            this.HasKey<int>(o => o.InternalApplicantsId);
        }
    }
    public class InternalApplicantExamResultEntityTypeConfiguration : EntityTypeConfiguration<InternalApplicantExamResult>
    {
        public InternalApplicantExamResultEntityTypeConfiguration()
        {
            this.ToTable("InternalApplicantExamResults", "HRM");
            this.HasKey<int>(o => o.InternalApplicantExamResultId);
        }
    }
    public class InternalApplicantScreeningEntityTypeConfiguration : EntityTypeConfiguration<InternalApplicantScreening>
    {
        public InternalApplicantScreeningEntityTypeConfiguration()
        {
            this.ToTable("InternalApplicantScreenings", "HRM");
            this.HasKey<int>(o => o.InternalApplicantScreeningId);
        }
    }
    public class InternalApplicantScreeningResultEntityTypeConfiguration : EntityTypeConfiguration<InternalApplicantScreeningResult>
    {
        public InternalApplicantScreeningResultEntityTypeConfiguration()
        {
            this.ToTable("InternalApplicantScreeningResults", "HRM");
            this.HasKey<int>(o => o.InternalApplicantScreeningResultId);
        }
    }
    public class ApplicantScreeningResultAttachmentEntityTypeConfiguration : EntityTypeConfiguration<ApplicantScreeningResultAttachment>
    {
        public ApplicantScreeningResultAttachmentEntityTypeConfiguration()
        {
            this.ToTable("ApplicantScreeningResultAttachments", "HRM");
            this.HasKey<int>(o => o.ApplicantScreeningResultAttachmentId);
        }
    }
    public class RecruitmentRequestEntityTypeConfiguration : EntityTypeConfiguration<RecruitmentRequest>
    {
        public RecruitmentRequestEntityTypeConfiguration()
        {
            this.ToTable("RecruitmentRequest", "HRM");
            this.HasKey(o => o.RecruitmentRequestId);

         

           

            // Hierarchy Validation Navigation Mappings
            this.HasMany(e => e.RecruitmentRequestProjectHRValidations)
                .WithRequired(v => v.RecruitmentRequest)
                .HasForeignKey(v => v.RecruitmentRequestId)
                .WillCascadeOnDelete(true);

            this.HasMany(e => e.RecruitmentRequestPMValidations)
                .WithRequired(v => v.RecruitmentRequest)
                .HasForeignKey(v => v.RecruitmentRequestId)
                .WillCascadeOnDelete(true);

            this.HasMany(e => e.RecruitmentRequestDepartmentValidations)
                .WithRequired(v => v.RecruitmentRequest)
                .HasForeignKey(v => v.RecruitmentRequestId)
                .WillCascadeOnDelete(true);

            this.HasMany(e => e.RecruitmentRequestHRValidations)
                .WithRequired(v => v.RecruitmentRequest)
                .HasForeignKey(v => v.RecruitmentRequestId)
                .WillCascadeOnDelete(true);

            this.HasMany(e => e.RecruitmentRequestDGMValidations)
                .WithRequired(v => v.RecruitmentRequest)
                .HasForeignKey(v => v.RecruitmentRequestId)
                .WillCascadeOnDelete(true);

            this.HasMany(e => e.RecruitmentRequestGMValidations)
                .WithRequired(v => v.RecruitmentRequest)
                .HasForeignKey(v => v.RecruitmentRequestId)
                .WillCascadeOnDelete(true);

            // Legacy Table
            this.HasMany(e => e.RecruitmentRequestSecondValidations)
                .WithRequired(v => v.RecruitmentRequest)
                .HasForeignKey(v => v.RecruitmentRequestId)
                .WillCascadeOnDelete(true);
        }
    }

    public class RecruitmentRequestProjectHRValidationConfiguration : EntityTypeConfiguration<RecruitmentRequestProjectHRValidation>
    {
        public RecruitmentRequestProjectHRValidationConfiguration()
        {
            this.ToTable("RecruitmentRequestProjectHRValidation", "HRM");
            this.HasKey(o => o.RecruitmentRequestProjectHRValidationId);
        }
    }

    public class RecruitmentRequestPMValidationConfiguration : EntityTypeConfiguration<RecruitmentRequestPMValidation>
    {
        public RecruitmentRequestPMValidationConfiguration()
        {
            this.ToTable("RecruitmentRequestPMValidation", "HRM");
            this.HasKey(o => o.RecruitmentRequestPMValidationId);
        }
    }

    public class RecruitmentRequestDepartmentValidationConfiguration : EntityTypeConfiguration<RecruitmentRequestDepartmentValidation>
    {
        public RecruitmentRequestDepartmentValidationConfiguration()
        {
            this.ToTable("RecruitmentRequestDepartmentValidation", "HRM");
            this.HasKey(o => o.RecruitmentRequestDepartmentValidationId);
        }
    }

    public class RecruitmentRequestHRValidationConfiguration : EntityTypeConfiguration<RecruitmentRequestHRValidation>
    {
        public RecruitmentRequestHRValidationConfiguration()
        {
            this.ToTable("RecruitmentRequestHRValidation", "HRM");
            this.HasKey(o => o.RecruitmentRequestHRValidationId);
        }
    }

    public class RecruitmentRequestDGMValidationConfiguration : EntityTypeConfiguration<RecruitmentRequestDGMValidation>
    {
        public RecruitmentRequestDGMValidationConfiguration()
        {
            this.ToTable("RecruitmentRequestDGMValidation", "HRM");
            this.HasKey(o => o.RecruitmentRequestDGMValidationId);
        }
    }

    public class RecruitmentRequestGMValidationConfiguration : EntityTypeConfiguration<RecruitmentRequestGMValidation>
    {
        public RecruitmentRequestGMValidationConfiguration()
        {
            this.ToTable("RecruitmentRequestGMValidation", "HRM");
            this.HasKey(o => o.RecruitmentRequestGMValidationId);
        }
    }

    public class RecruitmentRequestSecondValidationConfiguration : EntityTypeConfiguration<RecruitmentRequestSecondValidation>
    {
        public RecruitmentRequestSecondValidationConfiguration()
        {
            this.ToTable("RecruitmentRequestSecondValidation", "HRM");
            this.HasKey(o => o.RecruitmentRequestSecondValidationId);
        }
    }
    public class ApplicantExperienceEntityTypeConfiguration : EntityTypeConfiguration<ApplicantExperience>
    {
        public ApplicantExperienceEntityTypeConfiguration()
        {
            this.ToTable("ApplicantExperience", "HRM");
            this.HasKey<int>(o => o.ApplicantExpId);
        }
    }
    public class ApplicantEducationEntityTypeConfiguration : EntityTypeConfiguration<ApplicantEducation>
    {
        public ApplicantEducationEntityTypeConfiguration()
        {
            this.ToTable("ApplicantEducation", "HRM");
            this.HasKey<int>(o => o.ApplicantEducationId);
        }
    }
    public class ApplicantionFormEntityTypeConfiguration : EntityTypeConfiguration<ApplicantionForm>
    {

        public ApplicantionFormEntityTypeConfiguration()
        {
            this.ToTable("ApplicantionForm", "HRM");
            this.HasKey<int>(o => o.ApplicantionFormId);
        }
    }
    public class VacancyPlanEntityTypeConfiguration : EntityTypeConfiguration<Vacancy>
    {

        public VacancyPlanEntityTypeConfiguration()
        {
            this.ToTable("Vacancy", "HRM");
            this.HasKey<int>(o => o.VacancyId);
        }
    }
    public class VacancyDetailEntityTypeConfiguration : EntityTypeConfiguration<VacancyDetail>
    {

        public VacancyDetailEntityTypeConfiguration()
        {
            this.ToTable("VacancyDetails", "HRM");
            this.HasKey<int>(o => o.VacancyDetailId);
        }
    }
    public class VacancyLanguageEntityTypeConfiguration
    : EntityTypeConfiguration<VacancyLanguage>
    {
        public VacancyLanguageEntityTypeConfiguration()
        {
            // Table
            ToTable("VacancyLanguages", "HRM");

            // Primary Key
            HasKey(x => x.VacancyLanguageId);

            // Properties
            Property(x => x.LanguageName)
                .IsRequired()
                .HasMaxLength(100);

            Property(x => x.Description)
                .IsOptional()
                .HasMaxLength(500);

            Property(x => x.ProficiencyLevel)
                .IsOptional()
                .HasMaxLength(50);

            Property(x => x.IsRequired)
                .IsRequired();

            // Relationship
            HasRequired(x => x.Vacancy)
                .WithMany(x => x.VacancyLanguages)
                .HasForeignKey(x => x.VacancyId)
                .WillCascadeOnDelete(true);
        }
    }
    public class RecruitmentPlanEntityTypeConfiguration : EntityTypeConfiguration<RecruitmentPlan>
    {
        public RecruitmentPlanEntityTypeConfiguration()
        {
            this.ToTable("RecruitmentPlan", "HRM");
            this.HasKey<int>(o => o.RecruitmentPlanId);
        }
    }
    public class ApplicantRegistrationEntityTypeConfiguration : EntityTypeConfiguration<ApplicantRegistration>
    {
        public ApplicantRegistrationEntityTypeConfiguration()
        {
            this.ToTable("ApplicantRegistration", "HRM");
            this.HasKey<int>(o => o.ApplicantRegistrationId);
        }
    }
    public class RecruitmentEntityTypeConfiguration : EntityTypeConfiguration<Recruitment>
    {
        public RecruitmentEntityTypeConfiguration()
        {
            this.ToTable("Recruitment", "HRM");
            this.HasKey<int>(o => o.RecruitmentId);
        }
    }
    public class InternalRecruitmentCriteriaTypeSettingEntityTypeConfiguration : EntityTypeConfiguration<InternalRecruitmentCriteriaTypeSetting>
    {
        public InternalRecruitmentCriteriaTypeSettingEntityTypeConfiguration()
        {
            this.ToTable("InternalRecruitmentCriteriaTypeSettings", "HRM");
            this.HasKey<int>(o => o.InternalRecruitmentCriteriaTypeSettingId);
        }
    }
    public class ApplicantExamResultEntityTypeConfiguration : EntityTypeConfiguration<ApplicantExamResult>
    {
        public ApplicantExamResultEntityTypeConfiguration()
        {
            this.ToTable("ApplicantExamResult", "HRM");
            this.HasKey<int>(o => o.ApplicantResultId);
        }
    }
    public class ApplicantScreenResultEntityTypeConfiguration : EntityTypeConfiguration<ApplicantScreenResult>
    {
        public ApplicantScreenResultEntityTypeConfiguration()
        {
            this.ToTable("ApplicantScreenResults", "HRM");
            this.HasKey<int>(o => o.ApplicantScreenResultId);
        }
    }
    public class RecruitmentVacancyEntityTypeConfiguration : EntityTypeConfiguration<RecruitmentVacancy>
    {
        public RecruitmentVacancyEntityTypeConfiguration()
        {
            this.ToTable("RecruitmentVacancy", "HRM");
            this.HasKey<int>(o => o.RecruitmentVacancyId);
        }
    }
    public class EmployeeTransferRequestEntityTypeConfiguration : EntityTypeConfiguration<EmployeeTransferRequest>
    {
        public EmployeeTransferRequestEntityTypeConfiguration()
        {
            this.ToTable("EmployeeTransferRequests", "HRM");
            this.HasKey<int>(o => o.EmployeeTransferRequestId);
        }
    }
    public class InternalVacancyEligibilitySettingEntityTypeConfiguration : EntityTypeConfiguration<InternalVacancyEligibilitySetting>
    {
        public InternalVacancyEligibilitySettingEntityTypeConfiguration()
        {
            this.ToTable("InternalVacancyEligibilitySettings", "HRM");
            this.HasKey<int>(o => o.InternalVacancyEligibilitySettingId);
        }
    }
    public class TransferRequestDetailEntityTypeConfiguration : EntityTypeConfiguration<TransferRequestDetail>
    {
        public TransferRequestDetailEntityTypeConfiguration()
        {
            this.ToTable("TransferRequestDetails", "HRM");
            this.HasKey<int>(o => o.TransferRequestDetailId);
        }
    }

    public class CompanyApprovalSettingEntityTypeConfiguration : EntityTypeConfiguration<CompanyApprovalSetting>
    {
        public CompanyApprovalSettingEntityTypeConfiguration()
        {
            this.ToTable("CompanyApprovalSettings", "HRM");
            this.HasKey<int>(o => o.CompanyApprovalSettingId);
        }
    }
    public class TransferRequestValidationEntityTypeConfiguration : EntityTypeConfiguration<TransferRequestValidation>
    {
        public TransferRequestValidationEntityTypeConfiguration()
        {
            this.ToTable("TransferRequestValidations", "HRM");
            this.HasKey<int>(o => o.TransferRequestValidationId);
        }
    }
    public class TransferRequestGMValidationEntityTypeConfiguration : EntityTypeConfiguration<TransferRequestGMValidation>
    {
        public TransferRequestGMValidationEntityTypeConfiguration()
        {
            this.ToTable("TransferRequestGMValidations", "HRM");
            this.HasKey<int>(o => o.TransferRequestGMValidationId);
        }
    }
    public class TransferRequestDGMValidationEntityTypeConfiguration : EntityTypeConfiguration<TransferRequestDGMValidation>
    {
        public TransferRequestDGMValidationEntityTypeConfiguration()
        {
            this.ToTable("TransferRequestDGMValidations", "HRM");
            this.HasKey<int>(o => o.TransferRequestDGMValidationId);
        }
    }
    public class TransferRequestDepartmentValidationEntityTypeConfiguration : EntityTypeConfiguration<TransferRequestDepartmentValidation>
    {
        public TransferRequestDepartmentValidationEntityTypeConfiguration()
        {
            this.ToTable("TransferRequestDepartmentValidations", "HRM");
            this.HasKey<int>(o => o.TransferRequestDepartmentValidationId);
        }
    }
    public class EmployeeTransferAttachmentEntityTypeConfiguration : EntityTypeConfiguration<EmployeeTransferAttachment>
    {
        public EmployeeTransferAttachmentEntityTypeConfiguration()
        {
            this.ToTable("EmployeeTransferAttachments", "HRM");
            this.HasKey<int>(o => o.EmployeeTransferAttachmentId);
        }
    }
    public class ExperienceJobTitleDetailEntityTypeConfiguration : EntityTypeConfiguration<ExperienceJobTitleDetail>
    {
        public ExperienceJobTitleDetailEntityTypeConfiguration()
        {
            this.ToTable("ExperienceJobTitleDetails", "HRM");
            this.HasKey<int>(o => o.ExperienceJobTitleDetailId);
        }
    }
    public class EducationFieldDetailEntityTypeConfiguration : EntityTypeConfiguration<EducationFieldDetail>
    {
        public EducationFieldDetailEntityTypeConfiguration()
        {
            this.ToTable("EducationFieldDetails", "HRM");
            this.HasKey<int>(o => o.EducationFieldDetailId);
        }
    }
    public class RelatedEducationFieldDetailEntityTypeConfiguration : EntityTypeConfiguration<RelatedEducationFieldDetail>
    {
        public RelatedEducationFieldDetailEntityTypeConfiguration()
        {
            this.ToTable("RelatedEducationFieldDetails", "HRM");
            this.HasKey<int>(o => o.RelatedEducationFieldDetailId);
        }
    }
    public class EducationLevelDetailEntityTypeConfiguration : EntityTypeConfiguration<EducationLevelDetail>
    {
        public EducationLevelDetailEntityTypeConfiguration()
        {
            this.ToTable("EducationLevelDetail", "HRM");
            this.HasKey<int>(o => o.EducationLevelDetailId);
        }
    }
    public class ExperienceJobTitleEntityTypeConfiguration : EntityTypeConfiguration<ExperienceJobTitle>
    {
        public ExperienceJobTitleEntityTypeConfiguration()
        {
            this.ToTable("ExperienceJobTitles", "HRM");
            this.HasKey<int>(o => o.ExperienceJobTitleId);
        }
    }
    #endregion

    #region leave ,Benefit, Others
    public class MonthlyLeavePlanEntityTypeConfiguration : EntityTypeConfiguration<MonthlyLeavePlan>
    {
        public MonthlyLeavePlanEntityTypeConfiguration()
        {
            this.ToTable("MonthlyLeavePlan", "HRM");
            this.HasKey<int>(o => o.MonthlyLeavePlanId);

        }
    }
    public class MonthlyLeavePlanDetailEntityTypeConfiguration : EntityTypeConfiguration<MonthlyLeavePlanDetail>
    {
        public MonthlyLeavePlanDetailEntityTypeConfiguration()
        {
            this.ToTable("MonthlyLeavePlanDetail", "HRM");
            this.HasKey<int>(o => o.MonthlyLeavePlanDetailId);
        }
    }
    public class EmployeePerformanceActionsEntityTypeConfiguration : EntityTypeConfiguration<EmployeePerformanceScore>
    {
        public EmployeePerformanceActionsEntityTypeConfiguration()
        {
            this.ToTable("EmployeePerformanceScore", "Performance");
            this.HasKey<int>(o => o.EmployeePerformanceScoreId);
        }
    }
    public class ProbationPerformanceSettingEntityTypeConfiguration : EntityTypeConfiguration<ProbationPerformanceSetting>
    {
        public ProbationPerformanceSettingEntityTypeConfiguration()
        {
            this.ToTable("ProbationPerformanceSetting", "HRM");
            this.HasKey<int>(o => o.ProbationPerformanceSettingId);
        }
    }
    public class PerformanceEvaluationSettingEntityTypeConfiguration : EntityTypeConfiguration<PerformanceEvaluationSetting>
    {
        public PerformanceEvaluationSettingEntityTypeConfiguration()
        {
            this.ToTable("PerformanceEvaluationSettings", "HRM");
            this.HasKey<int>(o => o.PerformanceEvaluationSettingId);
        }
    }
    public class PerformanceAllowedScoreEntityTypeConfiguration : EntityTypeConfiguration<PerformanceAllowedScore>
    {
        public PerformanceAllowedScoreEntityTypeConfiguration()
        {
            this.ToTable("PerformanceAllowedScores", "HRM");
            this.HasKey<int>(o => o.PerformanceAllowedScoreId);
        }
    }
    public class PerformanceEvaluationSettingsEntityTypeConfiguration : EntityTypeConfiguration<PerformanceEvaluationSetting>
    {
        public PerformanceEvaluationSettingsEntityTypeConfiguration()
        {
            this.ToTable("PerformanceEvaluationSettings", "HRM");
            this.HasKey<int>(o => o.PerformanceEvaluationSettingId);
        }
    }
    public class EmployeePeerToPeerAndInternalCustomerPerformanceSettingEntityTypeConfiguration : EntityTypeConfiguration<EmployeePeerToPeerAndInternalCustomerPerformanceSetting>
    {
        public EmployeePeerToPeerAndInternalCustomerPerformanceSettingEntityTypeConfiguration()
        {
            this.ToTable("EmployeePeerToPeerAndInternalCustomerPerformanceSettings", "HRM");
            this.HasKey<int>(o => o.EmployeePeerToPeerAndInternalCustomerPerformanceSettingId);
        }
    }


    class DeligationCCEntityTypeConfiguration : EntityTypeConfiguration<DeligationCC>
    {
        public DeligationCCEntityTypeConfiguration()
        {
            this.ToTable("DeligationCC", "HRM");
            this.HasKey<int>(vd => vd.DeligationCCId);
        }
    }

    public class ManPowerPlanningRequestEntityTypeConfiguration : EntityTypeConfiguration<ManPowerPlanningRequest>
    {
        public ManPowerPlanningRequestEntityTypeConfiguration()
        {
            this.ToTable("ManPowerPlanningRequest", "HRM");
            this.HasKey<int>(o => o.ManPowerPlanningRequestId);
        }
    }
    public class BusinessUnitEntityTypeConfiguration : EntityTypeConfiguration<BusinessUnit>
    {
        public BusinessUnitEntityTypeConfiguration()
        {
            this.ToTable("BusinessUnit", "HRM");
            this.HasKey<int>(o => o.BusinessUnitId);
        }
    }
    public class BusinessUnitBranchEntityTypeConfiguration : EntityTypeConfiguration<BusinessUnitBranch>
    {
        public BusinessUnitBranchEntityTypeConfiguration()
        {
            this.ToTable("BusinessUnitBranch", "HRM");
            this.HasKey<int>(o => o.BusinessUnitBranchId);
        }
    }
    public class SickLeaveSettingEntityTypeConfiguration : EntityTypeConfiguration<SickLeaveSetting>
    {
        public SickLeaveSettingEntityTypeConfiguration()
        {
            this.ToTable("SickLeaveSetting", "HRM");
            this.HasKey<int>(o => o.SickLeaveSettingId);
        }
    }
    public class CompanyLeaveWorkflowSettingEntityTypeConfiguration : EntityTypeConfiguration<CompanyLeaveWorkflowSetting>
    {
        public CompanyLeaveWorkflowSettingEntityTypeConfiguration()
        {
            this.ToTable("CompanyLeaveWorkflowSettings", "HRM");
            this.HasKey<int>(o => o.CompanyLeaveWorkflowSettingId);
        }
    }
    public class LeavePayableSettingEntityTypeConfiguration : EntityTypeConfiguration<LeavePayableSetting>
    {
        public LeavePayableSettingEntityTypeConfiguration()
        {
            this.ToTable("LeavePayableSettings", "HRM");
            this.HasKey<int>(o => o.LeavePayableSettingId);
        }
    }
    public class LeavePayableEntityTypeConfiguration : EntityTypeConfiguration<LeavePayable>
    {
        public LeavePayableEntityTypeConfiguration()
        {
            this.ToTable("LeavePayables", "HRM");
            this.HasKey<int>(o => o.LeavePayableId);
        }
    }
    public class LeavePayableDetailEntityTypeConfiguration : EntityTypeConfiguration<LeavePayableDetail>
    {
        public LeavePayableDetailEntityTypeConfiguration()
        {
            this.ToTable("LeavePayableDetails", "HRM");
            this.HasKey<int>(o => o.LeavePayableDetailId);
            this.Property(d => d.BasicSalary).HasPrecision(18, 6);
            this.Property(d => d.Balance).HasPrecision(18, 6);
            this.Property(d => d.PayableDays).HasPrecision(18, 6);
            this.Property(d => d.Year1).HasPrecision(18, 6);
            this.Property(d => d.Year2).HasPrecision(18, 6);
            this.Property(d => d.AnnualLeavePerMonth).HasPrecision(18, 6);
            this.Property(d => d.Balance).HasPrecision(18, 6);
            this.Property(d => d.AgregateIncomeTax).HasPrecision(18, 6);
            this.Property(d => d.AgregateTaxableAmount).HasPrecision(18, 6);
            this.Property(d => d.AgregateIncomeTax).HasPrecision(18, 6);
            this.Property(d => d.BasicSalaryIncomeTax).HasPrecision(18, 6);
            this.Property(d => d.TaxDifference).HasPrecision(18, 6);
            this.Property(d => d.NetPayment).HasPrecision(18, 6);


        }
    }
    public class LeavePayableWithDetailFiscalYearEntityTypeConfiguration : EntityTypeConfiguration<LeavePayableWithDetailFiscalYear>
    {
        public LeavePayableWithDetailFiscalYearEntityTypeConfiguration()
        {
            this.ToTable("LeavePayableWithDetailFiscalYears", "HRM");
            this.HasKey<int>(d => d.LeavePayableWithDetailFiscalYearId);

        }
    }
    #region Severance
    public class SeverancePayableSettingEntityTypeConfiguration : EntityTypeConfiguration<SeverancePayableSetting>
    {
        public SeverancePayableSettingEntityTypeConfiguration()
        {
            this.ToTable("SeverancePayableSettings", "HRM");
            this.HasKey<int>(o => o.SeverancePayableSettingId);
            this.Property(d => d.SalaryValue).HasPrecision(18, 6);

        }
    }
    public class SeveranceCalculationSettingEntityTypeConfiguration: EntityTypeConfiguration<SeveranceCalculationSetting>
    {
        public SeveranceCalculationSettingEntityTypeConfiguration()
        {
            ToTable("SeveranceCalculationSettings", "HRM");
            HasKey(x => x.SeveranceCalculationSettingId);
            Property(x => x.Description).HasMaxLength(500);
            Property(x => x.FormulaType).IsRequired();
            Property(x => x.IsActive).IsRequired();
        }
    }
    public class SeverancePayableEntityTypeConfiguration : EntityTypeConfiguration<SeverancePayable>
    {
        public SeverancePayableEntityTypeConfiguration()
        {
            this.ToTable("SeverancePayables", "HRM");
            this.HasKey<int>(o => o.SeverancePayableId);

        }
    }
    public class SeverancePayableDetailEntityTypeConfiguration : EntityTypeConfiguration<SeverancePayableDetail>
    {
        public SeverancePayableDetailEntityTypeConfiguration()
        {
            this.ToTable("SeverancePayableDetail", "HRM");
            this.HasKey<int>(o => o.SeverancePayableDetailId);
            this.Property(d => d.BasicSalary).HasPrecision(18, 6);
            this.Property(d => d.SpecialSalaryDeduction).HasPrecision(18, 6);
            this.Property(d => d.Balance).HasPrecision(18, 6);
            this.Property(d => d.TaxWithheldComTax).HasPrecision(18, 6);
            this.Property(d => d.NoneTaxableTotal).HasPrecision(18, 6);
            this.Property(d => d.TotalDeductions).HasPrecision(18, 6);
            this.Property(d => d.TaxWithheldIncomeTax).HasPrecision(18, 6);
            this.Property(d => d.GrossComponsation).HasPrecision(18, 6);
            this.Property(d => d.GrossCompensationPayment).HasPrecision(18, 6);
            this.Property(d => d.CompensationPaymentT).HasPrecision(18, 6);
            this.Property(d => d.TotalServiceMonths).HasPrecision(18, 6);
            this.Property(d => d.TaxDifference).HasPrecision(18, 6);
            this.Property(d => d.TaxDifference).HasPrecision(18, 6);
            this.Property(d => d.AccTaxDifference).HasPrecision(18, 6);
            this.Property(d => d.TaxOnMonthlySalaryAndCompensation).HasPrecision(18, 6);
            this.Property(d => d.Months).HasPrecision(18, 6);
            this.Property(d => d.Years).HasPrecision(18, 6);
            this.Property(d => d.Days).HasPrecision(18, 6);
            this.Property(d => d.Months).HasPrecision(18, 6);
            this.Property(d => d.NetPay).HasPrecision(18, 6);
        }
    }
    public class SeveranceWithProbabilityEntityTypeConfiguration : EntityTypeConfiguration<SeveranceWithProbability>
    {
        public SeveranceWithProbabilityEntityTypeConfiguration()
        {
            this.ToTable("SeveranceWithProbabilities", "HRM");
            this.HasKey<int>(o => o.SeveranceWithProbabilityId);
            this.Property(d => d.MortalityRate).HasPrecision(18, 6);
            this.Property(d => d.ProbabilityOfTurnover).HasPrecision(18, 6);
            this.Property(d => d.RemainingNoOfYears).HasPrecision(18, 6);
            this.Property(d => d.SalaryUpOnRetirement).HasPrecision(18, 6);
            this.Property(d => d.PVOfSalaryUponRetirement).HasPrecision(18, 6);
            this.Property(d => d.Salary).HasPrecision(18, 6);
            this.Property(d => d.MaxEntitelementAmount).HasPrecision(18, 6);
            this.Property(d => d.MaxAllowedPeriod).HasPrecision(18, 6);
            this.Property(d => d.EV1).HasPrecision(18, 6);
            this.Property(d => d.EV2).HasPrecision(18, 6);
            this.Property(d => d.TotalEV).HasPrecision(18, 6);
        }
    }
    #endregion
    public class ComplainDetailEntityTypeConfiguration : EntityTypeConfiguration<ComplainDetail>
    {
        public ComplainDetailEntityTypeConfiguration()
        {
            this.ToTable("ComplainDetail", "HRM");
            this.HasKey<int>(o => o.ComplainDetailId);
        }
    }

    public class ProbationPerformanceDetailEntityTypeConfiguration : EntityTypeConfiguration<ProbationPerformanceDetail>
    {
        public ProbationPerformanceDetailEntityTypeConfiguration()
        {
            this.ToTable("ProbationPerformanceDetail", "HRM");
            this.HasKey<int>(o => o.ProbationPerformanceDetailId);
        }
    }

    public class ProbationPerformanceValidationEntityTypeConfiguration : EntityTypeConfiguration<ProbationPerformanceValidation>
    {
        public ProbationPerformanceValidationEntityTypeConfiguration()
        {
            this.ToTable("ProbationPerformanceValidations", "HRM");
            this.HasKey<int>(o => o.ProbationPerformanceValidationId);
        }
    }

    public class ProbationPerformanceEntityTypeConfiguration : EntityTypeConfiguration<ProbationPerformance>
    {
        public ProbationPerformanceEntityTypeConfiguration()
        {
            this.ToTable("ProbationPerformance", "HRM");
            this.HasKey<int>(o => o.ProbationPerformanceId);
        }
    }
    public class ComplainCCEntityTypeConfiguration : EntityTypeConfiguration<ComplainCC>
    {
        public ComplainCCEntityTypeConfiguration()
        {
            this.ToTable("ComplainCC", "HRM");
            this.HasKey<int>(o => o.ComplainCCId);
        }
    }
    public class LeaveReturnEntityTypeConfiguration : EntityTypeConfiguration<LeaveReturn>
    {
        public LeaveReturnEntityTypeConfiguration()
        {
            this.ToTable("LeaveReturn", "HRM");
            this.HasKey<int>(o => o.LeaveReturnId);
        }
    }
    public class RetirementSettingEntityTypeConfiguration : EntityTypeConfiguration<RetirementSetting>
    {
        public RetirementSettingEntityTypeConfiguration()
        {
            this.ToTable("RetirementSetting", "HRM");
            this.HasKey<int>(o => o.RetirementSettingId);
        }
    }
    public class EducationEntityTypeConfiguration : EntityTypeConfiguration<Education>
    {
        public EducationEntityTypeConfiguration()
        {
            this.ToTable("Education", "HRM");
            this.HasKey<int>(o => o.EmpEduId);
        }
    }
    public class BranchEntityTypeConfiguration : EntityTypeConfiguration<Branch>
    {
        public BranchEntityTypeConfiguration()
        {
            this.ToTable("Branch", "HRM");
            this.HasKey<int>(o => o.BranchId);
        }
    }
    public class StructureEntityTypeConfiguration : EntityTypeConfiguration<Structure>
    {
        public StructureEntityTypeConfiguration()
        {
            this.ToTable("Structure", "HRM");
            this.HasKey<int>(o => o.StructureId);
        }
    }
    public class TraineeEntityTypeConfiguration : EntityTypeConfiguration<Trainee>
    {
        public TraineeEntityTypeConfiguration()
        {
            this.ToTable("Trainee", "HRM");
            this.HasKey<int>(o => o.TraineeId);
        }
    }

    public class TemplateEntityTypeConfiguration : EntityTypeConfiguration<Template>
    {
        public TemplateEntityTypeConfiguration()
        {
            this.ToTable("Template", "HRM");
            this.HasKey<int>(o => o.TemplateId);
        }
    }

    public class TemplateTypeSettingEntityTypeConfiguration : EntityTypeConfiguration<TemplateTypeSetting>
    {
        public TemplateTypeSettingEntityTypeConfiguration()
        {
            this.ToTable("TemplateTypeSettings", "HRM");
            this.HasKey<int>(o => o.TemplateTypeSettingId);
        }
    }
    public class LoanRequestEntityTypeConfiguration : EntityTypeConfiguration<LoanRequest>
    {
        public LoanRequestEntityTypeConfiguration()
        {
            this.ToTable("LoanRequest", "HRM");
            this.HasKey<int>(o => o.LoanRequestId);
        }
    }
    public class LoanRequestDetailEntityTypeConfiguration : EntityTypeConfiguration<LoanRequestDetail>
    {
        public LoanRequestDetailEntityTypeConfiguration()
        {
            this.ToTable("LoanRequestDetails", "HRM");
            this.HasKey<int>(o => o.LoanRequestDetailId);
        }
    }
    public class LoanRequestAttachmentEntityTypeConfiguration : EntityTypeConfiguration<LoanRequestAttachment>
    {
        public LoanRequestAttachmentEntityTypeConfiguration()
        {
            this.ToTable("LoanRequestAttachments", "HRM");
            this.HasKey<int>(o => o.LoanRequestAttachmentId);
        }
    }
    public class LoanRequestDepartmentValidationEntityTypeConfiguration : EntityTypeConfiguration<LoanRequestDepartmentValidation>
    {
        public LoanRequestDepartmentValidationEntityTypeConfiguration()
        {
            this.ToTable("LoanRequestDepartmentValidations", "HRM");
            this.HasKey<int>(o => o.LoanRequestDepartmentValidationId);
        }
    }
    public class LoanRequestValidationEntityTypeConfiguration : EntityTypeConfiguration<LoanRequestValidation>
    {
        public LoanRequestValidationEntityTypeConfiguration()
        {
            this.ToTable("LoanRequestValidations", "HRM");
            this.HasKey<int>(o => o.LoanRequestValidationId);
        }
    }
    public class ComplainEntityTypeConfiguration : EntityTypeConfiguration<Complain>
    {
        public ComplainEntityTypeConfiguration()
        {
            this.ToTable("Complain", "HRM");
            this.HasKey<int>(o => o.ComplainId);
        }
    }
    public class ComplainAttachementEntityTypeConfiguration : EntityTypeConfiguration<ComplainAttachement>
    {
        public ComplainAttachementEntityTypeConfiguration()
        {
            this.ToTable("ComplainAttachement", "HRM");
            this.HasKey<int>(o => o.ComplainAttachementId);
        }
    }
    public class AppealEntityTypeConfiguration : EntityTypeConfiguration<Appeal>
    {

        public AppealEntityTypeConfiguration()
        {
            this.ToTable("Appeal", "HRM");
            this.HasKey<int>(o => o.AppealId);

        }
    }

    public class TrainingStatusEntityTypeConfiguration : EntityTypeConfiguration<TrainingStatus>
    {

        public TrainingStatusEntityTypeConfiguration()
        {
            this.ToTable("TrainingStatus", "HRM");
            this.HasKey<int>(o => o.TrainingStatusId);

        }
    }
    public class TrainingNeedRequestEntityTypeConfiguration : EntityTypeConfiguration<TrainingNeedRequest>
    {

        public TrainingNeedRequestEntityTypeConfiguration()
        {
            this.ToTable("TrainingNeedRequests", "HRM");
            this.HasKey<int>(o => o.TrainingNeedRequestId);

        }
    }
    public class EmployeeAssociationEntityTypeConfiguration : EntityTypeConfiguration<EmployeeAssociation>
    {

        public EmployeeAssociationEntityTypeConfiguration()
        {
            this.ToTable("EmployeeAssociation", "HRM");
            this.HasKey<int>(o => o.AssociationId);

        }
    }
    public class EmployeeCourtAssociationEntityTypeConfiguration : EntityTypeConfiguration<EmployeeCourtAssociation>
    {

        public EmployeeCourtAssociationEntityTypeConfiguration()
        {
            this.ToTable("EmployeeCourtAssociation", "HRM");
            this.HasKey<int>(o => o.AssociationId);

        }
    }


    public class OverTimeJobResultEntityTypeConfiguration : EntityTypeConfiguration<OverTimeJobResult>
    {

        public OverTimeJobResultEntityTypeConfiguration()
        {
            this.ToTable("OverTimeJobResult", "Payroll");
            this.HasKey<int>(o => o.OverTimeJobResultId);

        }
    }


    public class PerformanceRankEntityTypeConfiguration : EntityTypeConfiguration<PerformanceRank>
    {

        public PerformanceRankEntityTypeConfiguration()
        {
            this.ToTable("PerformanceRank", "Payroll");
            this.HasKey<int>(o => o.PerformanceRankId);

        }
    }
    public class AnnouncmentEntityTypeConfiguration : EntityTypeConfiguration<Announcment>
    {

        public AnnouncmentEntityTypeConfiguration()
        {
            this.ToTable("Announcment", "HRM");
            this.HasKey<int>(o => o.AnnouncmentId);

        }
    }
    public class AnnouncmentDetailEntityTypeConfiguration : EntityTypeConfiguration<AnnouncmentDetail>
    {

        public AnnouncmentDetailEntityTypeConfiguration()
        {
            this.ToTable("AnnouncmentDetail", "HRM");
            this.HasKey<int>(o => o.AnnouncDetailId);

        }
    }
    public class AnnouncmentAttachmentEntityTypeConfiguration : EntityTypeConfiguration<AnnouncmentAttachment>
    {

        public AnnouncmentAttachmentEntityTypeConfiguration()
        {
            this.ToTable("AnnouncmentAttachments", "HRM");
            this.HasKey<int>(o => o.AnnouncmentAttachmentId);

        }
    }



    public class CompanyEmailEntityTypeConfiguration : EntityTypeConfiguration<CompanyEmail>
    {

        public CompanyEmailEntityTypeConfiguration()
        {
            this.ToTable("CompanyEmail", "HRM");
            this.HasKey<int>(o => o.CompanyEmailId);

        }
    }
    public class QuarterLengthEntityTypeConfiguration : EntityTypeConfiguration<QuarterLength>
    {

        public QuarterLengthEntityTypeConfiguration()
        {
            this.ToTable("QuarterLength", "HRM");
            this.HasKey<int>(o => o.QuarterLengthId);

        }
    }
    public class QuarterEntityTypeConfiguration : EntityTypeConfiguration<Quarter>
    {
        public QuarterEntityTypeConfiguration()
        {
            this.ToTable("Quarter", "HRM");
            this.HasKey<int>(o => o.QuarterId);
        }
    }
    public class EmployeeTypeEntityTypeConfiguration : EntityTypeConfiguration<EmployeeType>
    {
        public EmployeeTypeEntityTypeConfiguration()
        {
            this.ToTable("EmployeeTypes", "HRM");
            this.HasKey<int>(o => o.EmployeeTypeId);
        }
    }

    public class ExprianceRequestEntityTypeConfiguration : EntityTypeConfiguration<ExperienceRequest>
    {
        public ExprianceRequestEntityTypeConfiguration()
        {
            this.ToTable("ExperienceRequest", "HRM");
            this.HasKey<int>(o => o.ExperienceRequestId);
        }
    }

    public class SalarySettingEntityTypeConfiguration : EntityTypeConfiguration<SalarySetting>
    {
        public SalarySettingEntityTypeConfiguration()
        {
            this.ToTable("SalarySetting", "HRM");
            this.HasKey<int>(o => o.SalarySettingId);
        }
    }
    public class NeedAssessmentSummaryEntityTypeConfiguration : EntityTypeConfiguration<NeedAssessmentSummary>
    {
        public NeedAssessmentSummaryEntityTypeConfiguration()
        {
            this.ToTable("NeedAssessmentSummary", "HRM");
            this.HasKey<int>(o => o.NeedAssessmentSummaryId);
        }
    }
    public class NeedAssessmentSummaryDetailEntityTypeConfiguration : EntityTypeConfiguration<NeedAssessmentSummaryDetail>
    {
        public NeedAssessmentSummaryDetailEntityTypeConfiguration()
        {
            this.ToTable("NeedAssessmentSummaryDetails", "HRM");
            this.HasKey<int>(o => o.NeedAssessmentSummaryDetailId);
        }
    }
    public class TrainingEvaluationBeforeEntityTypeConfiguration : EntityTypeConfiguration<TrainingEvaluationBefore>
    {
        public TrainingEvaluationBeforeEntityTypeConfiguration()
        {
            this.ToTable("TrainingEvaluationBefores", "HRM");
            this.HasKey<int>(o => o.TrainingEvaluationBeforeId);
        }
    }
    public class TrainingEvaluationAfterEntityTypeConfiguration : EntityTypeConfiguration<TrainingEvaluationAfter>
    {
        public TrainingEvaluationAfterEntityTypeConfiguration()
        {
            this.ToTable("TrainingEvaluationAfters", "HRM");
            this.HasKey<int>(o => o.TrainingEvaluationAfterId);
        }
    }
    public class ClearanceProviderEntityTypeConfiguration : EntityTypeConfiguration<ClearanceProvider>
    {

        public ClearanceProviderEntityTypeConfiguration()
        {
            this.ToTable("ClearanceProvider", "HRM");
            this.HasKey<int>(o => o.ClearanceProviderId);

        }
    }
    public class ClearanceProviderDetailEntityTypeConfiguration : EntityTypeConfiguration<ClearanceProviderDetail>
    {
        public ClearanceProviderDetailEntityTypeConfiguration()
        {
            this.ToTable("ClearanceProviderDetails", "HRM");
            this.HasKey<int>(o => o.ClearanceProviderDetailId);
        }
    }
    public class ClearanceRequestEntityTypeConfiguration : EntityTypeConfiguration<ClearanceRequest>
    {
        public ClearanceRequestEntityTypeConfiguration()
        {
            this.ToTable("ClearanceRequests", "HRM");
            this.HasKey<int>(o => o.ClearanceRequestId);
        }
    }
    public class ClearanceRequestApproverEntityTypeConfiguration : EntityTypeConfiguration<ClearanceRequestApprover>
    {
        public ClearanceRequestApproverEntityTypeConfiguration()
        {
            this.ToTable("ClearanceRequestApprovers", "HRM");
            this.HasKey<int>(o => o.ClearanceRequestApproverId);
        }
    }
    public class ClearanceRequestStatusEntityTypeConfiguration : EntityTypeConfiguration<ClearanceRequestStatus>
    {
        public ClearanceRequestStatusEntityTypeConfiguration()
        {
            this.ToTable("ClearanceRequestStatus", "HRM");
            this.HasKey<int>(o => o.ClearanceRequestStatusId);
        }
    }

    public class ProbationsSettingEntityTypeConfiguration : EntityTypeConfiguration<ProbationsSetting>
    {
        public ProbationsSettingEntityTypeConfiguration()
        {
            this.ToTable("ProbationsSettings", "HRM");
            this.HasKey<int>(o => o.ProbationsSettingId);
        }
    }
    public class TerminationSettingEntityTypeConfiguration : EntityTypeConfiguration<TerminationSetting>
    {
        public TerminationSettingEntityTypeConfiguration()
        {
            this.ToTable("TerminationSettings", "HRM");
            this.HasKey<int>(o => o.TerminationSettingId);
        }
    }
    public class SickLeaveSalarySettingEntityTypeConfiguration : EntityTypeConfiguration<SickLeaveSalarySetting>
    {
        public SickLeaveSalarySettingEntityTypeConfiguration()
        {
            this.ToTable("SickLeaveSalarySetting", "HRM");
            this.HasKey<int>(o => o.SickLeaveSalarySettingId);
        }
    }
    public class EmployeeCardEntityTypeConfiguration : EntityTypeConfiguration<EmployeeCard>
    {
        public EmployeeCardEntityTypeConfiguration()
        {
            this.ToTable("EmployeeCards", "HRM");
            this.HasKey<int>(o => o.EmployeeCardId);
        }
    }
    public class EmployeeRewardEntityTypeConfiguration : EntityTypeConfiguration<EmployeeReward>
    {
        public EmployeeRewardEntityTypeConfiguration()
        {
            this.ToTable("EmployeeRewards", "HRM");
            this.HasKey<int>(o => o.EmployeeRewardId);
        }
    }
    public class RewardSettingEntityTypeConfiguration : EntityTypeConfiguration<RewardSetting>
    {
        public RewardSettingEntityTypeConfiguration()
        {
            this.ToTable("RewardSettings", "HRM");
            this.HasKey<int>(o => o.RewardSettingId);
        }
    }
    public class DelegationBenefitSettingEntityTypeConfiguration : EntityTypeConfiguration<DelegationBenefitSetting>
    {
        public DelegationBenefitSettingEntityTypeConfiguration()
        {
            this.ToTable("DelegationBenefitSettings", "HRM");
            this.HasKey<int>(o => o.DelegationBenefitSettingId);
        }
    }
    #endregion
}