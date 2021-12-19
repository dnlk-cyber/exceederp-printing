using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Core.Domain.CRM;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.ArchiveManagement;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
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
using ExceedERP.Core.Domain.HRM.Payroll.Setting;

namespace ExceedERP.DataAccess
{

    #region HRM

    public class WaterMarkEntityTypeConfiguration : EntityTypeConfiguration<WaterMark>
    {
        public WaterMarkEntityTypeConfiguration()
        {
            this.ToTable("WaterMark", "HRM");
            this.HasKey<int>(o => o.WaterMarkId);
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
    public class LeaveEntitleStartingDateEntityTypeConfiguration : EntityTypeConfiguration<LeaveEntitleStartingDate>
    {
        public LeaveEntitleStartingDateEntityTypeConfiguration()
        {
            this.ToTable("LeaveEntitleStartingDate", "HRM");
            this.HasKey<int>(o => o.LeaveEntitleStartingDateId);
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
    public class AdditionEntityTypeConfiguration : EntityTypeConfiguration<Addition>
    {

        public AdditionEntityTypeConfiguration()
        {
            this.ToTable("Addition", "Payroll");
            this.HasKey<int>(o => o.AdditionId);

        }

    }
    public class BonusEntityTypeConfiguration : EntityTypeConfiguration<Bonus>
    {

        public BonusEntityTypeConfiguration()
        {
            this.ToTable("Bonus", "Payroll");
            this.HasKey<int>(o => o.BonusId);

        }

    }
    public class BackPaymentEntityTypeConfiguration : EntityTypeConfiguration<BackPayment>
    {

        public BackPaymentEntityTypeConfiguration()
        {
            this.ToTable("BackPayment", "Payroll");
            this.HasKey<int>(o => o.BackPaymentId);
            this.Property(o => o.BasicSalary).HasPrecision(18, 9);
            this.Property(o => o.Allowances).HasPrecision(18, 9);
            this.Property(o => o.IncomeTax).HasPrecision(18, 9);
            this.Property(o => o.Pension).HasPrecision(18, 9);
            this.Property(o => o.Net).HasPrecision(18, 9);

        }

    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An allowance defination entity type configuration. </summary>
    ///
    /// <remarks>   Mekanehiwot, 11/11/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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
    public class ContributionTypeEntityTypeConfiguration : EntityTypeConfiguration<ContributionType>
    {

        public ContributionTypeEntityTypeConfiguration()
        {
            this.ToTable("ContributionType", "HRM");
            this.HasKey<int>(o => o.ContributionTypeId);
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
    public class AssociationEntityTypeConfiguration : EntityTypeConfiguration<Association>
    {

        public AssociationEntityTypeConfiguration()
        {
            this.ToTable("Association", "Payroll");
            this.HasKey<int>(o => o.AssociationId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
            this.Property(o => o.MonthlyAmount).HasPrecision(18, 9);
            this.Property(o => o.AdditionalAmount).HasPrecision(18, 9);

        }
    }

    public class PayrollAccountsEntityTypeConfiguration : EntityTypeConfiguration<PayrollAccounts>
    {

        public PayrollAccountsEntityTypeConfiguration()
        {
            this.ToTable("PayrollAccounts", "Payroll");
            this.HasKey<int>(o => o.PayrollAccountsId);
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
    public class AttendanceNewEntityTypeConfiguration : EntityTypeConfiguration<AttendanceNew>
    {
        public AttendanceNewEntityTypeConfiguration()
        {
            this.ToTable("AttendanceNew", "HRM");
            this.HasKey<int>(o => o.AttendanceNewId);
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
            //  this.HasRequired(s => s.EmployeeId  );



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
    public class FixedContributionEntityTypeConfiguration : EntityTypeConfiguration<FixedContribution>
    {

        public FixedContributionEntityTypeConfiguration()
        {
            this.ToTable("FixedContribution", "Payroll");
            this.HasKey<int>(o => o.FixedContributionId);
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
    public class LeaveEntitleEntityTypeConfiguration : EntityTypeConfiguration<LeaveEntitle>
    {

        public LeaveEntitleEntityTypeConfiguration()
        {
            this.ToTable("LeaveEntitle", "HRM");
            this.HasKey<int>(o => o.LeaveEntitleId);

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
    public class LoanEntityTypeConfiguration : EntityTypeConfiguration<Loan>
    {

        public LoanEntityTypeConfiguration()
        {
            this.ToTable("Loan", "Payroll");
            this.HasKey<int>(o => o.LoanId);
        }
    }
    public class OverTimeEntityTypeConfiguration : EntityTypeConfiguration<OverTime>
    {

        public OverTimeEntityTypeConfiguration()
        {
            this.ToTable("OverTime", "Payroll");
            this.HasKey<int>(o => o.OverTimeId);
        }
    }
    public class PerdimeSettingEntityTypeConfiguration : EntityTypeConfiguration<PerdimeSetting>
    {

        public PerdimeSettingEntityTypeConfiguration()
        {
            this.ToTable("PerdimeSetting", "Payroll");
            this.HasKey<int>(o => o.PerdimeSettingId);
        }
    }
    public class OverTimeTransferEntityTypeConfiguration : EntityTypeConfiguration<OverTimeTransfer>
    {

        public OverTimeTransferEntityTypeConfiguration()
        {
            this.ToTable("OverTimeTransfer", "Payroll");
            this.HasKey<int>(o => o.OverTimeTransferId);
        }
    }
    public class OverTimeTrasferDetailEntityTypeConfiguration : EntityTypeConfiguration<OverTimeTrasferDetail>
    {

        public OverTimeTrasferDetailEntityTypeConfiguration()
        {
            this.ToTable("OverTimeTrasferDetail", "Payroll");
            this.HasKey<int>(o => o.OverTimeTrasferDetailId);
        }
    }
    public class OverTimePerDayEntityTypeConfiguration : EntityTypeConfiguration<OverTimePerDay>
    {

        public OverTimePerDayEntityTypeConfiguration()
        {
            this.ToTable("OverTimePerDay", "Payroll");
            this.HasKey<string>(o => o.TicketNumber);
        }
    }
    public class PerformanceBehaviorEntityTypeConfiguration : EntityTypeConfiguration<PerformanceBehavior>
    {

        public PerformanceBehaviorEntityTypeConfiguration()
        {
            this.ToTable("PerformanceBehavior", "HRM");
            this.HasKey<int>(o => o.PerformanceBehaviorId);

        }
    }
    //public class DepartmentDefineMeasurmentEntityTypeConfiguration : EntityTypeConfiguration<DepartmentDefineMeasurment>
    //{

    //    public DepartmentDefineMeasurmentEntityTypeConfiguration()
    //    {
    //        this.ToTable("DepartmentDefineMeasurment", "HRM");
    //        this.HasKey<int>(o => o.DepartmentDefineMeasurmentId);

    //    }
    //}
    //public class DepartmentMeasurmentEntityTypeConfiguration : EntityTypeConfiguration<DepartmentMeasurment>
    //{

    //    public DepartmentMeasurmentEntityTypeConfiguration()
    //    {
    //        this.ToTable("DepartmentMeasurment", "HRM");
    //        this.HasKey<int>(o => o.DepartmentMeasurmentId);

    //    }
    //}
    //public class DepartmentPerformanceActionEntityTypeConfiguration : EntityTypeConfiguration<DepartmentPerformanceAction>
    //{

    //    public DepartmentPerformanceActionEntityTypeConfiguration()
    //    {
    //        this.ToTable("DepartmentPerformanceAction", "HRM");
    //        this.HasKey<int>(o => o.DepartmentPerformanceActionId);

    //    }
    //}
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

    public class RecruitmentEntityTypeConfiguration : EntityTypeConfiguration<Recruitment>
    {

        public RecruitmentEntityTypeConfiguration()
        {
            this.ToTable("Recruitment", "HRM");
            this.HasKey<int>(o => o.RecruitmentId);


        }
    }
    public class RegularContributionEntityTypeConfiguration : EntityTypeConfiguration<RegularContribution>
    {

        public RegularContributionEntityTypeConfiguration()
        {
            this.ToTable("RegularContribution", "Payroll");
            this.HasKey<int>(o => o.RegContributionId);
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

    public class EmployeeEntityTypeConfiguration : EntityTypeConfiguration<Person_Employee>
    {

        public EmployeeEntityTypeConfiguration()
        {
            this.ToTable("Employee", "HRM");
            this.HasKey<int>(o => o.EmployeeId);

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

    public class ApplicantionFormEntityTypeConfiguration : EntityTypeConfiguration<ApplicantionForm>
    {

        public ApplicantionFormEntityTypeConfiguration()
        {
            this.ToTable("ApplicantionForm", "HRM");
            this.HasKey<int>(o => o.ApplicantionFormId);
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
    public class AdvanceEntityTypeConfiguration : EntityTypeConfiguration<Advance>
    {

        public AdvanceEntityTypeConfiguration()
        {
            this.ToTable("Advance", "Payroll");
            this.HasKey<int>(o => o.AdvanceId);
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
    public class TrainingPlanEntityTypeConfiguration : EntityTypeConfiguration<TrainingPlan>
    {

        public TrainingPlanEntityTypeConfiguration()
        {
            this.ToTable("TrainingPlan", "HRM");
            this.HasKey<int>(o => o.TrainingPlanId);
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
    public class VacancyPlanEntityTypeConfiguration : EntityTypeConfiguration<Vacancy>
    {

        public VacancyPlanEntityTypeConfiguration()
        {
            this.ToTable("Vacancy", "HRM");
            this.HasKey<int>(o => o.VacancyId);
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
            this.ToTable("OtherBenefit", "HRM");
            this.HasKey<int>(o => o.OtherBenefitId);
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
        }
    }
    public class PayrollProcessLongTermDeductionsEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessLongTermDeductions>
    {
        public PayrollProcessLongTermDeductionsEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessLongTermDeductions", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollLongTermDeductionId);
            this.Property(o => o.Amount).HasPrecision(18, 9);
        }
    }

    public class EmpPayrollServiceChargeViewModelEntityTypeConfiguration :
        EntityTypeConfiguration<EmpPayrollServiceChargeViewModel>
    {
        public EmpPayrollServiceChargeViewModelEntityTypeConfiguration()
        {
            this.ToTable("EmpPayrollServiceChargeViewModel", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollServiceChargeViewModelId);
        }

    }
    public class PeriodStatusEntityTypeConfiguration : EntityTypeConfiguration<PeriodStatus>
    {
        public PeriodStatusEntityTypeConfiguration()
        {
            this.ToTable("PeriodStatus", "Payroll");
            this.HasKey<int>(o => o.PeriodStatusId);
        }
    }
    public class EmpPayrollNoDaysWorkedEntityTypeConfiguration : EntityTypeConfiguration<EmpPayrollNoDaysWorked>
    {
        public EmpPayrollNoDaysWorkedEntityTypeConfiguration()
        {
            this.ToTable("EmpPayrollNoDaysWorked", "Payroll");
            this.HasKey<int>(o => o.EmpPayrollNoDaysWorkedId);
        }
    }
    public class PayrollEmployeeCostCenterEntityTypeConfiguration : EntityTypeConfiguration<PayrollEmployeeCostCenter>
    {
        public PayrollEmployeeCostCenterEntityTypeConfiguration()
        {
            this.ToTable("PayrollEmployeeCostCenter", "Payroll");
            this.HasKey<int>(o => o.PayrollEmployeeCostCenterId);
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
    public class PayrollProcessAllowancesEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessAllowances>
    {
        public PayrollProcessAllowancesEntityTypeConfiguration()
        {
            this.ToTable("PayrollProcessAllowances", "Payroll");
            this.HasKey<int>(o => o.PayrollProcessAllowancesId);
            this.Property(o => o.NonTaxable).HasPrecision(18, 9);
            this.Property(o => o.Taxable).HasPrecision(18, 9);
            this.Property(o => o.Total).HasPrecision(18, 9);

        }
    }
    //public class EmpPayrollAdditionEntityTypeConfiguration : EntityTypeConfiguration<EmpPayrollAddition>
    //{
    //    public EmpPayrollAdditionEntityTypeConfiguration()
    //    {
    //        this.ToTable("EmpPayrollAddition", "Payroll");
    //        this.HasKey<int>(o => o.EmpPayrollAdditionID);
    //    }
    //}
    public class PayrollProcessAdditionsEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessAdditions>
    {
        public PayrollProcessAdditionsEntityTypeConfiguration()
    {
        this.ToTable("PayrollProcessAdditions", "Payroll");
        this.HasKey<int>(o => o.PayrollProcessAdditionsId);
        this.Property(o => o.Amount).HasPrecision(18, 9);

    }
}
public class EmpPayrollDeductionEntityTypeConfiguration : EntityTypeConfiguration<EmpPayrollDeduction>
{
    public EmpPayrollDeductionEntityTypeConfiguration()
    {
        this.ToTable("EmpPayrollDeduction", "Payroll");
        this.HasKey<int>(o => o.EmpPayrollDeductionId);
    }
}
public class PayrollProcessDeductionsEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessDeductions>
{
    public PayrollProcessDeductionsEntityTypeConfiguration()
    {
        this.ToTable("PayrollProcessDeductions", "Payroll");
        this.HasKey<int>(o => o.PayrollProcessDeductionsId);
        this.Property(o => o.Amount).HasPrecision(18, 9);
        this.Property(o => o.OutstandingAmount).HasPrecision(18, 9);
    }
}
public class EmpPayrollLongTermDeductionEntityTypeConfiguration : EntityTypeConfiguration<EmpPayrollLongTermDeduction>
{
    public EmpPayrollLongTermDeductionEntityTypeConfiguration()
    {
        this.ToTable("EmpPayrollLongTermDeduction", "Payroll");
        this.HasKey<int>(o => o.EmpPayrollLongTermDeductionId);
    }
}
public class PayrollProcessLoansEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessLoans>
{
    public PayrollProcessLoansEntityTypeConfiguration()
    {
        this.ToTable("PayrollProcessLoans", "Payroll");
        this.HasKey<int>(o => o.PayrollProcessLoansId);
        this.Property(o => o.Amount).HasPrecision(18, 9);
        this.Property(o => o.Outstanding).HasPrecision(18, 9);

    }
}
public class PayrollProcessAdvancesEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessAdvances>
{
    public PayrollProcessAdvancesEntityTypeConfiguration()
    {
        this.ToTable("PayrollProcessAdvances", "Payroll");
        this.HasKey<int>(o => o.PayrollProcessAdvancesId);
        this.Property(o => o.Amount).HasPrecision(18, 9);
        this.Property(o => o.Outstanding).HasPrecision(18, 9);
    }
}
public class PayrollProcessFixedContributionEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessFixedContribution>
{
    public PayrollProcessFixedContributionEntityTypeConfiguration()
    {
        this.ToTable("PayrollProcessFixedContribution", "Payroll");
        this.HasKey<int>(o => o.PayrollProcessFixedContributionID);
        this.Property(o => o.Amount).HasPrecision(18, 9);
        this.Property(o => o.Outstanding).HasPrecision(18, 9);
    }
}
public class PayrollProcessRegularContributionEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessRegularContribution>
{
    public PayrollProcessRegularContributionEntityTypeConfiguration()
    {
        this.ToTable("PayrollProcessRegularContribution", "Payroll");
        this.HasKey<int>(o => o.PayrollProcessRegularContributionID);
        this.Property(o => o.Amount).HasPrecision(18, 9);

    }
}
public class PayrollProcessViewModelEntityTypeConfiguration : EntityTypeConfiguration<PayrollProcessViewModel>
{
    public PayrollProcessViewModelEntityTypeConfiguration()
    {
        this.ToTable("PayrollProcessViewModel", "Payroll");
        this.HasKey<int>(o => o.PayrollProcessViewModelId);
        this.Property(o => o.AbsentDeduction).HasPrecision(18, 9);
        this.Property(o => o.AssocationContribution).HasPrecision(18, 9);
        this.Property(o => o.BasicSalary).HasPrecision(18, 9);
        this.Property(o => o.DaysAbsent).HasPrecision(18, 9);
        this.Property(o => o.GrossDeduction).HasPrecision(18, 9);
        this.Property(o => o.GrossEarning).HasPrecision(18, 9);
        this.Property(o => o.GrossNonTaxableEarning).HasPrecision(18, 9);
        this.Property(o => o.GrossTaxableEarning).HasPrecision(18, 9);
        this.Property(o => o.IncomeTax).HasPrecision(18, 9);
        this.Property(o => o.LabourUnionContribution).HasPrecision(18, 9);
        this.Property(o => o.NetEarning).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableHouseAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxablePositionAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableProfessionalAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableProjectAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableResponsiblityAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableTelephoneAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableTopUpAllowance).HasPrecision(18, 9);
        this.Property(o => o.NonTaxableTransportAllowance).HasPrecision(18, 9);
        this.Property(o => o.PensionFromCompany).HasPrecision(18, 9);
        this.Property(o => o.PensionFromEmployee).HasPrecision(18, 9);
        this.Property(o => o.ServiceCharge).HasPrecision(18, 9);
        this.Property(o => o.SportFee).HasPrecision(18, 9);
        this.Property(o => o.TaxableAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxableHouseAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxablePositionAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxableProfessionalAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxableProjectAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxableResponsiblityAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxableTelephoneAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxableTopUpAllowance).HasPrecision(18, 9);
        this.Property(o => o.TaxableTransportAllowance).HasPrecision(18, 9);

        this.Property(o => o.TotalAdditions).HasPrecision(18, 9);
        this.Property(o => o.TotalAdvance).HasPrecision(18, 9);
        this.Property(o => o.TotalAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalDeductions).HasPrecision(18, 9);
        this.Property(o => o.TotalFixedContribution).HasPrecision(18, 9);
        this.Property(o => o.TotalHouseAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalLoan).HasPrecision(18, 9);
        this.Property(o => o.TotalLongTermDeductions).HasPrecision(18, 9);
        this.Property(o => o.TotalOvertime).HasPrecision(18, 9);
        this.Property(o => o.TotalPenality).HasPrecision(18, 9);
        this.Property(o => o.TotalPension).HasPrecision(18, 9);
        this.Property(o => o.TotalPositionAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalProfessionalAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalProjectAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalRegularContribution).HasPrecision(18, 9);
        this.Property(o => o.TotalResponsiblityAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalTelephoneAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalTopUpAllowance).HasPrecision(18, 9);
        this.Property(o => o.TotalTransportAllowance).HasPrecision(18, 9);

    }
}
public class PayrollDistributionEntityTypeConfiguration : EntityTypeConfiguration<PayrollDistribution>
{
    public PayrollDistributionEntityTypeConfiguration()
    {
        this.ToTable("PayrollDistribution", "Payroll");
        this.HasKey<int>(o => o.PayrollDistributionId);
        this.Property(o => o.Credit).HasPrecision(18, 9);
        this.Property(o => o.Debit).HasPrecision(18, 9);
    }
}

public class PayrollContributionTypesEntityTypeConfiguration : EntityTypeConfiguration<PayrollContributionTypes>
{
    public PayrollContributionTypesEntityTypeConfiguration()
    {
        this.ToTable("PayrollContributionTypes", "Payroll");
        this.HasKey<int>(o => o.PayrollContributionTypesId);


    }
}
public class PayrollCostCenterEntityTypeConfiguration : EntityTypeConfiguration<PayrollCostCenter>
{
    public PayrollCostCenterEntityTypeConfiguration()
    {
        this.ToTable("PayrollCostCenter", "Payroll");
        this.HasKey<int>(o => o.PayrollCostCenterId);
    }
}
public class PayrollEmployeeAccountEntityTypeConfiguration : EntityTypeConfiguration<PayrollEmployeeAccount>
{
    public PayrollEmployeeAccountEntityTypeConfiguration()
    {
        this.ToTable("PayrollEmployeeAccount", "Payroll");
        this.HasKey<int>(o => o.PayrollEmployeeAccountId);
    }
}
    #endregion
    #region Health and Insurance
    //public class FixedAssetAccidentRecordEntityTypeConfiguration : EntityTypeConfiguration<FixedAssetAccidentRecord>
    //{
    //    public FixedAssetAccidentRecordEntityTypeConfiguration()
    //    {
    //        this.ToTable("FixedAssetAccidentRecord", "Health");
    //        this.HasKey<int>(o => o.FixedAssetAccidentRecordId);
    //    }
    //}
    public class PatientResultEntityTypeConfiguration : EntityTypeConfiguration<PatientResult>
    {
        public PatientResultEntityTypeConfiguration()
        {
            this.ToTable("PatientResult", "Health");
            this.HasKey<int>(o => o.PatientResultId);
        }
    }
    public class PatientResultDetailEntityTypeConfiguration : EntityTypeConfiguration<PatientResultDetail>
    {
        public PatientResultDetailEntityTypeConfiguration()
        {
            this.ToTable("PatientResultDetail", "Health");
            this.HasKey<int>(o => o.PatientResultDetailId);
        }
    }
    public class PatientDrugOrderEntityTypeConfiguration : EntityTypeConfiguration<PatientDrugOrder>
    {
        public PatientDrugOrderEntityTypeConfiguration()
        {
            this.ToTable("PatientDrugOrder", "Health");
            this.HasKey<int>(o => o.PatientDrugOrderId);
        }
    }
    public class DiagnosisEntityTypeConfiguration : EntityTypeConfiguration<Diagnosis>
    {
        public DiagnosisEntityTypeConfiguration()
        {
            this.ToTable("Diagnosis", "Health");
            this.HasKey<int>(o => o.DiagnosisId);
        }
    }
    //public class VehicleAccidentDetailEntityTypeConfiguration : EntityTypeConfiguration<VehicleAccidentDetail>
    //{
    //    public VehicleAccidentDetailEntityTypeConfiguration()
    //    {
    //        this.ToTable("VehicleAccidentDetail", "Insurance");
    //        this.HasKey<int>(o => o.VehicleAccidentDetailId);
    //    }
    //}
    public class ReferralFormEntityTypeConfiguration : EntityTypeConfiguration<ReferralForm>
    {
        public ReferralFormEntityTypeConfiguration()
        {
            this.ToTable("ReferralForm", "Health");
            this.HasKey<int>(o => o.ReferralFormId);
        }
    }
    public class ReferralSheetEntityTypeConfiguration : EntityTypeConfiguration<ReferralSheet>
    {
        public ReferralSheetEntityTypeConfiguration()
        {
            this.ToTable("ReferralSheet", "Health");
            this.HasKey<int>(o => o.ReferralSheetId);
        }
    }
    public class PatientsLeaveEntityTypeConfiguration : EntityTypeConfiguration<PatientsLeave>
    {
        public PatientsLeaveEntityTypeConfiguration()
        {
            this.ToTable("PatientsLeave", "Health");
            this.HasKey<int>(o => o.PatientsLeaveId);
        }
    }
    public class PrescriptionEntityTypeConfiguration : EntityTypeConfiguration<Prescription>
    {
        public PrescriptionEntityTypeConfiguration()
        {
            this.ToTable("Prescription", "Health");
            this.HasKey<int>(o => o.PrescriptionId);
        }
    }
    public class TreatmentEntityTypeConfiguration : EntityTypeConfiguration<Treatment>
    {
        public TreatmentEntityTypeConfiguration()
        {
            this.ToTable("Treatment", "Health");
            this.HasKey<int>(o => o.TreatmentId);
        }
    }
    public class PatientCardEntityTypeConfiguration : EntityTypeConfiguration<PatientCard>
    {
        public PatientCardEntityTypeConfiguration()
        {
            this.ToTable("PatientCard", "Health");
            //this.HasKey<string>(o => o. PatientCardId);
            this.HasKey<int>(o => o.PatientId);
        }
    }
    public class PatientCardDetailEntityTypeConfiguration : EntityTypeConfiguration<PatientCardDetail>
    {
        public PatientCardDetailEntityTypeConfiguration()
        {
            this.ToTable("PatientCardDetail", "Health");
            this.HasKey<int>(o => o.PatientCardDetailId);
        }
    }
    public class InsuranceGroupEntityTypeConfiguration : EntityTypeConfiguration<InsuranceGroup>
    {
        public InsuranceGroupEntityTypeConfiguration()
        {
            this.ToTable("InsuranceGroup", "Insurance");
            this.HasKey<int>(o => o.InsuranceGroupId);
        }
    }
    public class AccidentRecordEntityTypeConfiguration : EntityTypeConfiguration<AccidentRecord>
    {
        public AccidentRecordEntityTypeConfiguration()
        {
            this.ToTable("AccidentRecord", "Insurance");
            this.HasKey<int>(o => o.AccidentRecordId);
        }
    }
    public class AccidentMedicalRecordEntityTypeConfiguration : EntityTypeConfiguration<AccidentMedicalRecord>
    {
        public AccidentMedicalRecordEntityTypeConfiguration()
        {
            this.ToTable("AccidentMedicalRecord", "Insurance");
            this.HasKey<int>(o => o.AccidentMedicalRecordId);
        }
    }
    public class RestitutionEntityTypeConfiguration : EntityTypeConfiguration<Restitution>
    {
        public RestitutionEntityTypeConfiguration()
        {
            this.ToTable("Restitution", "Insurance");
            this.HasKey<int>(o => o.RestitutionId);
        }
    }
    public class InsuranceEntityTypeConfiguration : EntityTypeConfiguration<Insurance>
    {
        public InsuranceEntityTypeConfiguration()
        {
            this.ToTable("Insurance", "Insurance");
            this.HasKey<int>(o => o.InsuranceId);
        }
    }
    public class CourtInsuranceEntityTypeConfiguration : EntityTypeConfiguration<CourtInsurance>
    {
        public CourtInsuranceEntityTypeConfiguration()
        {
            this.ToTable("CourtInsurance", "Insurance");
            this.HasKey<int>(o => o.CourtInsuranceId);
        }
    }
    public class InsuranceCategoryEntityTypeConfiguration : EntityTypeConfiguration<InsuranceCategory>
    {
        public InsuranceCategoryEntityTypeConfiguration()
        {
            this.ToTable("InsuranceCategory", "Insurance");
            this.HasKey<int>(o => o.InsuranceCategoryId);
        }
    }
    public class InsuranceTypeEntityTypeConfiguration : EntityTypeConfiguration<InsuranceType>
    {
        public InsuranceTypeEntityTypeConfiguration()
        {
            this.ToTable("InsuranceType", "Insurance");
            this.HasKey<int>(o => o.InsuranceTypeId);
        }
    }
    public class OtherInsuranceEntityTypeConfiguration : EntityTypeConfiguration<OtherInsurance>
    {
        public OtherInsuranceEntityTypeConfiguration()
        {
            this.ToTable("OtherInsurance", "Insurance");
            this.HasKey<int>(o => o.OtherInsuranceId);
        }
    }
    #endregion
    #region SelfCare
    public class MedicalRequestEntityTypeConfiguration : EntityTypeConfiguration<MedicalRequest>
    {
        public MedicalRequestEntityTypeConfiguration()
        {
            this.ToTable("MedicalRequest", "Health");
            this.HasKey<int>(o => o.MedicalRequestId);
        }
    }

    public class RequestesStatusEntityTypeConfiguration : EntityTypeConfiguration<RequestesStatus>
    {
        public RequestesStatusEntityTypeConfiguration()
        {
            this.ToTable("RequestesStatus", "HRM");
            this.HasKey<int>(o => o.RequestStatusId);
        }
    }
    //public class PromotionRequestEntityTypeConfiguration : EntityTypeConfiguration<PromotionRequest >
    //{
    //    public PromotionRequestEntityTypeConfiguration()
    //    {
    //        this.ToTable("PromotionRequest", "HRM");
    //        this.HasKey<int>(o => o.PromotionRequestId);
    //    }
    //}
    public class RecruitmentRequestEntityTypeConfiguration : EntityTypeConfiguration<RecruitmentRequest>
    {
        public RecruitmentRequestEntityTypeConfiguration()
        {
            this.ToTable("RecruitmentRequest", "HRM");
            this.HasKey<int>(o => o.RecruitmentRequestId);
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
    public class LeaveRequestEntityTypeConfiguration : EntityTypeConfiguration<LeaveRequest>
    {
        public LeaveRequestEntityTypeConfiguration()
        {
            this.ToTable("LeaveRequest", "HRM");
            this.HasKey<int>(o => o.LeaveRequestId);
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
    public class OverTimeRequestEntityTypeConfiguration : EntityTypeConfiguration<OverTimeRequest>
    {

        public OverTimeRequestEntityTypeConfiguration()
        {
            this.ToTable("OverTimeRequest", "HRM");
            this.HasKey<int>(o => o.OverTimeRequestId);
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
    #region ArchiveManagement
    public class NewIncommingLetterEntityTypeConfiguration : EntityTypeConfiguration<IncomingLetterRegistration>
    {

        public NewIncommingLetterEntityTypeConfiguration()
        {
            this.ToTable("IncomingLetter", "Archive");
            this.HasKey<string>(o => o.BookNo);
        }
    }
    //public class AttachmentArchiveEntityTypeConfiguration : EntityTypeConfiguration<AttachmentArchive>
    //{

    //    public AttachmentArchiveEntityTypeConfiguration()
    //    {
    //        this.ToTable("AttachmentArchive", "Archive");
    //        this.HasKey<string>(o => o.AttachmentArchiveId);
    //    }
    //}
    public class EmployeeIncommingLetterEntityTypeConfiguration : EntityTypeConfiguration<EmployeeIncomingLetter>
    {

        public EmployeeIncommingLetterEntityTypeConfiguration()
        {
            this.ToTable("EmployeeIncommingLetter", "Archive");
            this.HasKey<string>(o => o.BookNo);
        }
    }
    public class NewOutgoingLetterEntityTypeConfiguration : EntityTypeConfiguration<OutgoingLetter>
    {

        public NewOutgoingLetterEntityTypeConfiguration()
        {
            this.ToTable("OutgoingLetter", "Archive");
            this.HasKey<int>(o => o.NewOutgoingLetterLetterId);
        }
    }
    public class InstitutionTypeEntityTypeConfiguration : EntityTypeConfiguration<InstitutionType>
    {

        public InstitutionTypeEntityTypeConfiguration()
        {
            this.ToTable("InstitutionType", "Archive");
            this.HasKey<int>(o => o.InstitutionTypeId);
        }
    }
    public class ZoneEntityTypeConfiguration : EntityTypeConfiguration<Zone>
    {

        public ZoneEntityTypeConfiguration()
        {
            this.ToTable("Zone", "Archive");
            this.HasKey<int>(o => o.ZoneId);
        }
    }
    public class PersonalArchiveNoEntityTypeConfiguration : EntityTypeConfiguration<PersonalArchiveNo>
    {

        public PersonalArchiveNoEntityTypeConfiguration()
        {
            this.ToTable("PersonalArchiveNo", "Archive");
            this.HasKey<int>(o => o.PersonalArchiveNoId);
        }
    }
    public class InstitutionArchiveNoEntityTypeConfiguration : EntityTypeConfiguration<InstitutionArchiveNo>
    {

        public InstitutionArchiveNoEntityTypeConfiguration()
        {
            this.ToTable("InstitutionArchiveNo", "Archive");
            this.HasKey<int>(o => o.InstiArchiveNoId);
        }
    }

    //public class ExprianceRequestEntityTypeConfiguration : EntityTypeConfiguration<ExperienceRequest>
    //{
    //    public ExprianceRequestEntityTypeConfiguration()
    //    {
    //        this.ToTable("ExperienceRequest", "HRM");
    //        this.HasKey<int>(o => o.ExperienceRequestId);
    //    }
    //}
    public class LetterStatusEntityTypeConfiguration : EntityTypeConfiguration<LetterStatus>
    {

        public LetterStatusEntityTypeConfiguration()
        {
            this.ToTable("LetterStatus", "Archive");
            this.HasKey<int>(o => o.LetterStatusId);
        }
    }
    public class LetterSenderEntityTypeConfiguration : EntityTypeConfiguration<LetterSender>
    {

        public LetterSenderEntityTypeConfiguration()
        {
            this.ToTable("LetterSender", "Archive");
            this.HasKey<int>(o => o.LetterSenderId);
        }
    }
    public class EmployeeLetterSenderEntityTypeConfiguration : EntityTypeConfiguration<EmployeeLetterSender>
    {

        public EmployeeLetterSenderEntityTypeConfiguration()
        {
            this.ToTable("EmployeeLetterSender", "Archive");
            this.HasKey<int>(o => o.LetterSenderId);
        }
    }
    public class RecivedLettersEntityTypeConfiguration : EntityTypeConfiguration<RecivedLetters>
    {

        public RecivedLettersEntityTypeConfiguration()
        {
            this.ToTable("RecivedLetters", "Archive");
            this.HasKey<int>(o => o.RecivedLetterId);
        }
    }
    #endregion
    #region Performance
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
    public class EmployeeMeasurementDefinitionEntityTypeConfiguration : EntityTypeConfiguration<EmployeeMeasurementDefinition>
    {

        public EmployeeMeasurementDefinitionEntityTypeConfiguration()
        {
            this.ToTable("EmployeeMeasurementDefinition", "Performance");
            this.HasKey<int>(o => o.EmpMeasurementDefinitionId);
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

    public class EmployeePerformanceActionEntityTypeConfiguration : EntityTypeConfiguration<EmployeePerformanceAction>
    {

        public EmployeePerformanceActionEntityTypeConfiguration()
        {
            this.ToTable("EmployeePerformanceAction", "Performance");
            this.HasKey<int>(o => o.EmpActionId);
        }
    }

    #endregion
    #region Company
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
    public class JobTitleMappingEntityTypeConfiguration : EntityTypeConfiguration<JobTitleMapping>
    {

        public JobTitleMappingEntityTypeConfiguration()
        {
            this.ToTable("JobTitleMapping", "HRM");
            this.HasKey<int>(o => o.JobTitleMappingId);
            // this.HasRequired<int>(s => s.JobTitle);
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
    public class ApplicantRegistrationEntityTypeConfiguration : EntityTypeConfiguration<ApplicantRegistration>
    {
        public ApplicantRegistrationEntityTypeConfiguration()
        {
            this.ToTable("ApplicantRegistration", "HRM");
            this.HasKey<int>(o => o.ApplicantRegistrationId);
        }
    }
    //public class ApplicantScreeningResultEntityTypeConfiguration : EntityTypeConfiguration<ApplicantScreeningResult>
    //{
    //    public ApplicantScreeningResultEntityTypeConfiguration()
    //    {
    //        this.ToTable("ApplicantScreeningResult", "HRM");
    //        this.HasKey<int>(o => o.ScreeningResultId);
    //    }
    //}
    public class ApplicantExamResultEntityTypeConfiguration : EntityTypeConfiguration<ApplicantExamResult>
    {
        public ApplicantExamResultEntityTypeConfiguration()
        {
            this.ToTable("ApplicantExamResult", "HRM");
            this.HasKey<int>(o => o.ApplicantResultId);
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
    #endregion
    //public class LeaveNewAwajStartYearEntityTypeConfiguration : EntityTypeConfiguration<LeaveNewAwajStartYear>
    //{

    //    public LeaveNewAwajStartYearEntityTypeConfiguration()
    //    {
    //        this.ToTable("LeaveNewAwajStartYear", "HRM");
    //        this.HasKey<int>(o => o.LeaveNewAwajStartYearId);

    //    }
    //}
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

    public class SubjectTypeEntityTypeConfiguration : EntityTypeConfiguration<SubjectType>
    {

        public SubjectTypeEntityTypeConfiguration()
        {
            this.ToTable("SubjectType", "Archive");
            this.HasKey<int>(o => o.SubjectTypeId);
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
    //class DelegationReasonEntityTypeConfiguration : EntityTypeConfiguration<DelegationReason>
    //{

    //    public DelegationReasonEntityTypeConfiguration()
    //    {
    //        this.ToTable("DelegationReason", "HRM");
    //        this.HasKey<int>(vd => vd.DelegationReasonId);

    //    }
    //}
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
    public class SickLeaveSettingEntityTypeConfiguration : EntityTypeConfiguration<SickLeaveSetting>
    {

        public SickLeaveSettingEntityTypeConfiguration()
        {
            this.ToTable("SickLeaveSetting", "HRM");
            this.HasKey<int>(o => o.SickLeaveSettingId);

        }

    }
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
    public class ComplainEntityTypeConfiguration : EntityTypeConfiguration<Complain>
    {

        public ComplainEntityTypeConfiguration()
        {
            this.ToTable("Complain", "HRM");
            this.HasKey<int>(o => o.ComplainId);

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
    public class ClearanceProviderEntityTypeConfiguration : EntityTypeConfiguration<ClearanceProvider>
    {

        public ClearanceProviderEntityTypeConfiguration()
        {
            this.ToTable("ClearanceProvider", "HRM");
            this.HasKey<int>(o => o.ClearanceProviderId);

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
    public class BonusSettingEntityTypeConfiguration : EntityTypeConfiguration<BonusSetting>
    {

        public BonusSettingEntityTypeConfiguration()
        {
            this.ToTable("BonusSetting", "Payroll");
            this.HasKey<int>(o => o.BonusSettingId);

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
    public class PayrollPeriodEntityTypeConfiguration : EntityTypeConfiguration<PayrollPeriod>
    {

        public PayrollPeriodEntityTypeConfiguration()
        {
            this.ToTable("PayrollPeriod", "Payroll");
            this.HasKey<int>(o => o.GLPeriodId);

        }
    }

    public class ProvidentFundEntityTypeConfiguration : EntityTypeConfiguration<ProvidentFund>
    {

        public ProvidentFundEntityTypeConfiguration()
        {
            this.ToTable("ProvidentFund", "Payroll");
            this.HasKey<int>(o => o.ProvidentFundId);

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

    #region APTS
    ////==============================================================
    ////public class DespensaryEntityTypeConfiguration : EntityTypeConfiguration<Despensary>
    ////{

    ////    public DespensaryEntityTypeConfiguration()
    ////    {
    ////        this.ToTable("Despensary", "APTS");
    ////        this.HasKey<int>(o => o.DespensaryId);

    ////    }
    ////}
    //public class DrugListsEntityTypeConfiguration : EntityTypeConfiguration<DrugLists>
    //{

    //    public DrugListsEntityTypeConfiguration()
    //    {
    //        this.ToTable("DrugLists", "APTS");
    //        this.HasKey<int>(o => o.DrugListsId);

    //    }
    //}
    //public class DespensaryStoreEntityTypeConfiguration : EntityTypeConfiguration<DespensaryStore>
    //{

    //    public DespensaryStoreEntityTypeConfiguration()
    //    {
    //        this.ToTable("DespensaryStoreS", "APTS");
    //        this.HasKey<int>(o => o.DespensaryStoreId);

    //    }
    //}

    //public class DrugStoreEntityTypeConfiguration : EntityTypeConfiguration<DrugStore>
    //{

    //    public DrugStoreEntityTypeConfiguration()
    //    {
    //        this.ToTable("DrugStores", "APTS");
    //        this.HasKey<int>(o => o.DrugStoreId);

    //    }
    //}
    //public class DrugReceivalModel19EntityTypeConfiguration : EntityTypeConfiguration<DrugReceivalModel19>
    //{

    //    public DrugReceivalModel19EntityTypeConfiguration()
    //    {
    //        this.ToTable("DrugReceivalModel19", "APTS");
    //        this.HasKey<int>(o => o.DrugReceivalModel19Id);

    //    }
    //}
    ////public class MedicinPurchaseRequestEntityTypeConfiguration : EntityTypeConfiguration<MedicinPurchaseRequest>
    ////{

    ////    public MedicinPurchaseRequestEntityTypeConfiguration()
    ////    {
    ////        this.ToTable("MedicinPurchaseRequest", "APTS");
    ////        this.HasKey<int>(o => o.MedicinPurchaseRequestId);

    ////    }
    ////}
    //public class Model22EntityTypeConfiguration : EntityTypeConfiguration<Model22>
    //{

    //    public Model22EntityTypeConfiguration()
    //    {
    //        this.ToTable("Model22", "APTS");
    //        this.HasKey<int>(o => o.Model22Id);

    //    }
    //}
    //public class PatientRegisterEntityTypeConfiguration : EntityTypeConfiguration<PatientRegister>
    //{

    //    public PatientRegisterEntityTypeConfiguration()
    //    {
    //        this.ToTable("PatientRegister", "APTS");
    //        this.HasKey<int>(o => o.PatientRegisterId);

    //    }
    //}
    ////public class ProductAuditingForDispensariesEntityTypeConfiguration : EntityTypeConfiguration<ProductAuditingForDispensaries>
    ////{

    ////    public ProductAuditingForDispensariesEntityTypeConfiguration()
    ////    {
    ////        this.ToTable("ProductAuditingForDispensaries", "APTS");
    ////        this.HasKey<int>(o => o.ProductAuditingForDispensariesId);

    ////    }
    ////}
    //public class RequisitionModel20EntityTypeConfiguration : EntityTypeConfiguration<RequisitionModel20>
    //{

    //    public RequisitionModel20EntityTypeConfiguration()
    //    {
    //        this.ToTable("RequisitionModel20", "APTS");
    //        this.HasKey<int>(o => o.RequisitionModel20Id);

    //    }
    //}
    ////public class SaleTicketPadRegisterEntityTypeConfiguration : EntityTypeConfiguration<SaleTicketPadRegister>
    ////{

    ////    public SaleTicketPadRegisterEntityTypeConfiguration()
    ////    {
    ////        this.ToTable("SaleTicketPadRegister", "APTS");
    ////        this.HasKey<int>(o => o.SaleTicketPadRegisterId);

    ////    }
    ////}
    ////public class StoreInventoryEntityTypeConfiguration : EntityTypeConfiguration<StoreInventory>
    ////{

    ////    public StoreInventoryEntityTypeConfiguration()
    ////    {
    ////        this.ToTable("StoreInventory", "APTS");
    ////        this.HasKey<int>(o => o.StoreInventoryId);

    ////    }
    ////}

    #endregion
}



