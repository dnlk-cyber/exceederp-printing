using ExceedERP.Core.Domain.Planning;
using ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment;
using ExceedERP.Core.Domain.Planning.Project;
using System.Data.Entity.ModelConfiguration;
namespace ExceedERP.DataAccess.Context
{
   public class GlobalKPIEntityTypeConfiguration : EntityTypeConfiguration<GlobalKPI>
   {

       public GlobalKPIEntityTypeConfiguration()
       {
           this.ToTable("GlobalKPI", "plan");
           this.HasKey<int>(t => t.GlobalKPIId);

       }
   }
    public class GlobalSubKPIEntityTypeConfiguration : EntityTypeConfiguration<GlobalSubKPI>
   {

       public GlobalSubKPIEntityTypeConfiguration()
       {
           this.ToTable("GlobalSubKPI", "plan");
           this.HasKey<int>(t => t.GlobalSubKPIId);

       }
   }

    public class KpiProgressTrackerEntityTypeConfiguration : EntityTypeConfiguration<KpiProgressTracker>
   {

       public KpiProgressTrackerEntityTypeConfiguration()
       {
           this.ToTable("KpiProgressTrackers", "plan");
           this.HasKey<int>(t => t.KpiProgressTrackerId);

       }
   }
    public class SubKpiProgressTrackerEntityTypeConfiguration : EntityTypeConfiguration<SubKpiProgressTracker>
   {

       public SubKpiProgressTrackerEntityTypeConfiguration()
       {
           this.ToTable("SubKpiProgressTrackers", "plan");
           this.HasKey<int>(t => t.SubKpiProgressTrackerId);

       }
   }
    public class GlobalIndicatorEntityTypeConfiguration : EntityTypeConfiguration<GlobalIndicator>
   {

       public GlobalIndicatorEntityTypeConfiguration()
       {
           this.ToTable("GlobalIndicators", "plan");
           this.HasKey<int>(t => t.GlobalIndicatorId);

       }
   }
   public class ObjectiveEntityTypeConfiguration : EntityTypeConfiguration<Objective>
   {

       public ObjectiveEntityTypeConfiguration()
       {
           this.ToTable("Objectives", "plan");
           this.HasKey<int>(t => t.ObjectiveId);
           this.HasOptional(t => t.StrategicPlan)
               .WithMany()
               .HasForeignKey(t => t.StrategicPlanId)
               .WillCascadeOnDelete(false);

       }
   }

   public class StrategicGoalEntityTypeConfiguration : EntityTypeConfiguration<StrategicGoal>
   {
       public StrategicGoalEntityTypeConfiguration()
       {
           this.ToTable("StrategicGoals", "plan");
           this.HasKey(t => t.StrategicGoalId);
           this.HasRequired(t => t.StrategicPlan)
               .WithMany()
               .HasForeignKey(t => t.StrategicPlanId)
               .WillCascadeOnDelete(false);
       }
   }

   public class AnnualGoalEntityTypeConfiguration : EntityTypeConfiguration<AnnualGoal>
   {
       public AnnualGoalEntityTypeConfiguration()
       {
           this.ToTable("AnnualGoals", "plan");
           this.HasKey(t => t.AnnualGoalId);
           this.HasRequired(t => t.StrategicPlan)
               .WithMany()
               .HasForeignKey(t => t.StrategicPlanId)
               .WillCascadeOnDelete(false);
           this.HasRequired(t => t.GlFiscalYear)
               .WithMany()
               .HasForeignKey(t => t.GlFiscalYearId)
               .WillCascadeOnDelete(false);
       }
   }

    public class IndicatorEntityTypeConfiguration : EntityTypeConfiguration<Indicator>
    {

        public IndicatorEntityTypeConfiguration()
        {
            this.ToTable("Indicators", "plan");
            this.HasKey<int>(t => t.IndicatorId);

        }
    }
    public class ExcutivePlanKPIEntityTypeConfiguration : EntityTypeConfiguration<ExcutivePlanKPI>
    {

        public ExcutivePlanKPIEntityTypeConfiguration()
        {
            this.ToTable("ExcutivePlanKPIs", "plan");
            this.HasKey<int>(t => t.ExcutivePlanKPIId);

        }
    }
    public class PlanEntityTypeConfiguration : EntityTypeConfiguration<Plan>
    {

        public PlanEntityTypeConfiguration()
        {
            this.ToTable("Plans", "plan");
            this.HasKey<int>(t => t.PlanId);

        }
    }
    public class StrategicPlanEntityTypeConfiguration : EntityTypeConfiguration<StrategicPlan>
    {
        public StrategicPlanEntityTypeConfiguration()
        {
            this.ToTable("StrategicPlans", "plan");
            this.HasKey(t => t.StrategicPlanId);
            this.HasRequired(t => t.FromGlFiscalYear)
                .WithMany()
                .HasForeignKey(t => t.FromGlFiscalYearId)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.ToGlFiscalYear)
                .WithMany()
                .HasForeignKey(t => t.ToGlFiscalYearId)
                .WillCascadeOnDelete(false);
        }
    }
    public class QuickFixPlanEntityTypeConfiguration : EntityTypeConfiguration<QuickFixPlan>
    {
        public QuickFixPlanEntityTypeConfiguration()
        {
            this.ToTable("QuickFixPlans", "plan");
            this.HasKey(t => t.QuickFixPlanId);
            this.Property(t => t.AnnualPlan).HasPrecision(18, 2);
            this.Property(t => t.PlanUpToQuarter).HasPrecision(18, 2);
            this.Property(t => t.Actual).HasPrecision(18, 2);
            this.Property(t => t.PreviousPlanRemainingQuarter).HasPrecision(18, 2);
            this.Property(t => t.RevisedPlanRemainingQuarter).HasPrecision(18, 2);
            this.HasRequired(t => t.FiscalYear)
                .WithMany()
                .HasForeignKey(t => t.GlFiscalYearId)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.AnnualPlanSource)
                .WithMany()
                .HasForeignKey(t => t.PlanId)
                .WillCascadeOnDelete(false);
        }
    }
    public class ActualEntityTypeConfiguration : EntityTypeConfiguration<Actual>
    {

            public ActualEntityTypeConfiguration()
            {
                this.ToTable("Actuals", "plan");
                this.HasKey<int>(t => t.ActualId);

            }
    }
    public class ActualDetailEntityTypeConfiguration : EntityTypeConfiguration<ActualDetail>
    {

            public ActualDetailEntityTypeConfiguration()
            {
                this.ToTable("ActualDetail", "plan");
                this.HasKey<int>(t => t.ActualDetailId);

            }
    }
   
    public class BSCPlanEntityTypeConfiguration : EntityTypeConfiguration<BSCPlan>
    {

        public BSCPlanEntityTypeConfiguration()
        {
            this.ToTable("BSCPlans", "plan");
            this.HasKey<int>(t => t.BSCPlanId);
            this.Property(t => t.SubmittedBy).HasMaxLength(256);
            this.Property(t => t.BranchApprovedBy).HasMaxLength(256);
            this.Property(t => t.HeadOfficeApprovedBy).HasMaxLength(256);

        }
    }
    public class PlanApprovalFeedbackEntityTypeConfiguration : EntityTypeConfiguration<PlanApprovalFeedback>
    {
        public PlanApprovalFeedbackEntityTypeConfiguration()
        {
            this.ToTable("PlanApprovalFeedbacks", "plan");
            this.HasKey(t => t.PlanApprovalFeedbackId);
            this.Property(t => t.Feedback).HasMaxLength(2000);
            this.Property(t => t.ActionBy).HasMaxLength(256);
            this.HasRequired(t => t.BSCPlan)
                .WithMany()
                .HasForeignKey(t => t.BSCPlanId)
                .WillCascadeOnDelete(false);
        }
    }
    public class BSCActualEntityTypeConfiguration : EntityTypeConfiguration<BSCActual>
    {

        public BSCActualEntityTypeConfiguration()
        {
            this.ToTable("BSCActuals", "plan");
            this.HasKey<int>(t => t.BSCActualId);

        }
    }
      
    public class PlanningDetailEntityTypeConfiguration : EntityTypeConfiguration<PlanningDetail>
    {

            public PlanningDetailEntityTypeConfiguration()
            {
                this.ToTable("PlanningDetails", "plan");
                this.HasKey<int>(t => t.PlanningDetailId);

            }
    }
    //public class PlanningWithSubIndicatorEntityTypeConfiguration : EntityTypeConfiguration<PlanningWithSubIndicator>
    //{

    //        public PlanningWithSubIndicatorEntityTypeConfiguration()
    //        {
    //            this.ToTable("PlanningWithSubIndicators", "plan");
    //            this.HasKey<int>(t => t.PlanningWithSubIndicatorId);

    //        }
    //}
    public class MonthlyPlanEntityTypeConfiguration : EntityTypeConfiguration<MonthlyPlan>
    {

         public MonthlyPlanEntityTypeConfiguration()
         {
             this.ToTable("MonthlyPlans", "plan");
             this.HasKey<int>(t => t.MonthlyPlanId);

         }
    }
    public class PastPlanAttachmentEntityTypeConfiguration : EntityTypeConfiguration<PastPlanAttachment>
    {
        public PastPlanAttachmentEntityTypeConfiguration()
        {
            this.ToTable("PastPlanAttachments", "plan");
            this.HasKey(t => t.PastPlanAttachmentId);
            this.Property(t => t.Title).IsRequired().HasMaxLength(200);
            this.Property(t => t.Description).HasMaxLength(2000);
            this.Property(t => t.FileName).IsRequired().HasMaxLength(260);
            this.Property(t => t.StoredFileName).IsRequired().HasMaxLength(100);
            this.Property(t => t.ContentType).HasMaxLength(200);
            this.HasRequired(t => t.GlFiscalYear)
                .WithMany()
                .HasForeignKey(t => t.GlFiscalYearId)
                .WillCascadeOnDelete(false);
        }
    }
    public class WeeklyPlanEntityTypeConfiguration : EntityTypeConfiguration<WeeklyPlan>
        {

            public WeeklyPlanEntityTypeConfiguration()
            {
                this.ToTable("WeeklyPlans", "plan");
                this.HasKey<int>(t => t.WeeklyPlanId);

            }
        }
      //public class ActualDetailEntityTypeConfiguration : EntityTypeConfiguration<ActualDetail>
      //{

      //      public ActualDetailEntityTypeConfiguration()
      //      {
      //          this.ToTable("ActualDetails", "plan");
      //          this.HasKey<int>(t => t.ActualDetailId);

      //      }
      //}
    public class WeeklyActualEntityTypeConfiguration : EntityTypeConfiguration<WeeklyActual>
    {

            public WeeklyActualEntityTypeConfiguration()
            {
                this.ToTable("WeeklyActuals", "plan");
                this.HasKey<int>(t => t.WeeklyActualId);

            }
    }
    public class MonthlyActualEntityTypeConfiguration : EntityTypeConfiguration<MonthlyActual>
    {

        public MonthlyActualEntityTypeConfiguration()
        {
            this.ToTable("MonthlyActuals", "plan");
            this.HasKey<int>(t => t.MonthlyActualId);

        }
    }
    public class RiskMitigationPlanEntityTypeConfiguration : EntityTypeConfiguration<RiskMitigationPlan>
    {

        public RiskMitigationPlanEntityTypeConfiguration()
        {
            this.ToTable("RiskMitigationPlans", "plan");
            this.HasKey<int>(t => t.RiskMitigationPlanId);

        }
    }
    public class RiskFollowUpEntityTypeConfiguration : EntityTypeConfiguration<RiskFollowUp>
    {

        public RiskFollowUpEntityTypeConfiguration()
        {
            this.ToTable("RiskFollowUps", "plan");
            this.HasKey<int>(t => t.RiskFollowUpId);

        }
    }

    #region Project 
    public class ProjectObjectiveEntityTypeConfiguration : EntityTypeConfiguration<ProjectObjective>
    {

        public ProjectObjectiveEntityTypeConfiguration()
        {
            this.ToTable("ProjectObjectives", "plan");
            this.HasKey<int>(t => t.ProjectObjectiveId);

        }
    }
    public class ProjectKPIEntityTypeConfiguration : EntityTypeConfiguration<ProjectKPI>
    {

        public ProjectKPIEntityTypeConfiguration()
        {
            this.ToTable("ProjectKPIs", "plan");
            this.HasKey<int>(t => t.ProjectKPIId);

        }
    }
    public class ProjectSubKPIEntityTypeConfiguration : EntityTypeConfiguration<ProjectSubKPI>
    {

        public ProjectSubKPIEntityTypeConfiguration()
        {
            this.ToTable("ProjectSubKPIs", "plan");
            this.HasKey<int>(t => t.ProjectSubKPIId);

        }
    }
    public class ProjectPlanEntityTypeConfiguration : EntityTypeConfiguration<ProjectPlan>
    {

        public ProjectPlanEntityTypeConfiguration()
        {
            this.ToTable("ProjectPlans", "plan");
            this.HasKey<int>(t => t.ProjectPlanId);

        }
    }
    public class ProjectActualEntityTypeConfiguration : EntityTypeConfiguration<ProjectActual>
    {

        public ProjectActualEntityTypeConfiguration()
        {
            this.ToTable("ProjectActuals", "plan");
            this.HasKey<int>(t => t.ProjectActualId);

        }
    }
    class ProjectMonthlyPlanEntityTypeConfiguration : EntityTypeConfiguration<ProjectMonthlyPlan>
    {

        public ProjectMonthlyPlanEntityTypeConfiguration()
        {
            this.ToTable("ProjectMonthlyPlans", "plan");
            this.HasKey<int>(t => t.ProjectMonthlyPlanId);

        }
    }
    class ProjectMonthlyActualEntityTypeConfiguration : EntityTypeConfiguration<ProjectMonthlyActual>
    {

        public ProjectMonthlyActualEntityTypeConfiguration()
        {
            this.ToTable("ProjectMonthlyActuals", "plan");
            this.HasKey<int>(t => t.ProjectMonthlyActualId);

        }
    }
    class ProjectWeeklyPlanEntityTypeConfiguration : EntityTypeConfiguration<ProjectWeeklyPlan>
    {

        public ProjectWeeklyPlanEntityTypeConfiguration()
        {
            this.ToTable("ProjectWeeklyPlans", "plan");
            this.HasKey<int>(t => t.ProjectWeeklyPlanId);

        }
    }
    class ProjectWeeklyActualEntityTypeConfiguration : EntityTypeConfiguration<ProjectWeeklyActual>
    {

        public ProjectWeeklyActualEntityTypeConfiguration()
        {
            this.ToTable("ProjectWeeklyActuals", "plan");
            this.HasKey<int>(t => t.ProjectWeeklyActualId);

        }
    }
    class ProjectExcutiveSettingEntityTypeConfiguration : EntityTypeConfiguration<ProjectExcutiveSetting>
    {

        public ProjectExcutiveSettingEntityTypeConfiguration()
        {
            this.ToTable("ProjectExcutiveSettings", "plan");
            this.HasKey<int>(t => t.ProjectExcutiveSettingId);

        }
    }
    class ProjectExcutiveSummarySettingEntityTypeConfiguration : EntityTypeConfiguration<ProjectExcutiveSummarySetting>
    {

        public ProjectExcutiveSummarySettingEntityTypeConfiguration()
        {
            this.ToTable("ProjectExcutiveSummarySettings", "plan");
            this.HasKey<int>(t => t.ProjectExcutiveSummarySettingId);

        }
    }
    #endregion
    #region Bussiness Plan
    public class BusinessPlanEntityTypeConfiguration : EntityTypeConfiguration<BusinessPlan>
    {

        public BusinessPlanEntityTypeConfiguration()
        {
            this.ToTable("BusinessPlans", "plan");
            this.HasKey<int>(t => t.BusinessPlanId);

        }
    }
    public class BusinessPlanAttachmentEntityTypeConfiguration : EntityTypeConfiguration<BusinessPlanAttachment>
    {

        public BusinessPlanAttachmentEntityTypeConfiguration()
        {
            this.ToTable("BusinessPlanAttachment", "plan");
            this.HasKey<int>(t => t.BusinessPlanAttachmentId);

        }
    }
    public class BusinessPlanValidationEntityTypeConfiguration : EntityTypeConfiguration<BusinessPlanValidation>
    {

        public BusinessPlanValidationEntityTypeConfiguration()
        {
            this.ToTable("BusinessPlanValidations", "plan");
            this.HasKey<int>(t => t.BusinessPlanValidationId);

        }
    }
    public class ShareAndDividendEntityTypeConfiguration : EntityTypeConfiguration<ShareAndDividend>
    {

        public ShareAndDividendEntityTypeConfiguration()
        {
            this.ToTable("ShareAndDividends", "plan");
            this.HasKey<int>(t => t.ShareAndDividendId);

        }
    }
    public class ShareAndDividendFiscalYearDetailEntityTypeConfiguration : EntityTypeConfiguration<ShareAndDividendFiscalYearDetail>
    {

        public ShareAndDividendFiscalYearDetailEntityTypeConfiguration()
        {
            this.ToTable("ShareAndDividendFiscalYearDetails", "plan");
            this.HasKey<int>(t => t.ShareAndDividendFiscalYearDetailId);

        }
    }
    public class LandManagementEntityTypeConfiguration : EntityTypeConfiguration<LandManagement>
    {

        public LandManagementEntityTypeConfiguration()
        {
            this.ToTable("LandManagements", "plan");
            this.HasKey<int>(t => t.LandManagementId);

        }
    }
    public class LandManagementAttachmentEntityTypeConfiguration : EntityTypeConfiguration<LandManagementAttachment>
    {

        public LandManagementAttachmentEntityTypeConfiguration()
        {
            this.ToTable("LandManagementAttachments", "plan");
            this.HasKey<int>(t => t.LandManagementAttachmentId);

        }
    }
    #endregion

}

