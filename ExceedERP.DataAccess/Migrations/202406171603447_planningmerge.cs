namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class planningmerge : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "plan.ActualDetail",
                c => new
                    {
                        ActualDetailId = c.Int(nullable: false, identity: true),
                        Unit = c.String(),
                        KPIType = c.Int(nullable: false),
                        Plan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Baseline = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HasSubIndicator = c.Boolean(nullable: false),
                        ParentId = c.Int(nullable: false),
                        PlanningDetailId = c.Int(),
                        GlobalIndicatorId = c.Int(nullable: false),
                        IndicatorId = c.Int(nullable: false),
                        BSCActualId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActualDetailId)
                .ForeignKey("plan.Indicators", t => t.IndicatorId, cascadeDelete: false)
                .ForeignKey("plan.BSCActuals", t => t.BSCActualId, cascadeDelete: false)
                .Index(t => t.IndicatorId)
                .Index(t => t.BSCActualId);
            
            CreateTable(
                "plan.Indicators",
                c => new
                    {
                        IndicatorId = c.Int(nullable: false, identity: true),
                        GlobalIndicatorId = c.Int(nullable: false),
                        Name = c.String(),
                        HasSubIndicator = c.Boolean(nullable: false),
                        ParentId = c.Int(nullable: false),
                        Description = c.String(),
                        Unit = c.String(),
                        KPIType = c.Int(nullable: false),
                        ObjectiveId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IndicatorId)
                .ForeignKey("plan.Objectives", t => t.ObjectiveId, cascadeDelete: false)
                .Index(t => t.ObjectiveId);
            
            CreateTable(
                "plan.Objectives",
                c => new
                    {
                        ObjectiveId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ObjectiveId);
            
            CreateTable(
                "plan.BSCActuals",
                c => new
                    {
                        BSCActualId = c.Int(nullable: false, identity: true),
                        ObjectiveId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BSCPlanId = c.Int(),
                        IsCopied = c.Boolean(nullable: false),
                        IsFirstLevelApproved = c.Boolean(nullable: false),
                        IsSecondLevel = c.Boolean(nullable: false),
                        IsThirdLevel = c.Boolean(nullable: false),
                        ActualId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BSCActualId)
                .ForeignKey("plan.Actuals", t => t.ActualId, cascadeDelete: false)
                .ForeignKey("plan.Objectives", t => t.ObjectiveId, cascadeDelete: false)
                .Index(t => t.ObjectiveId)
                .Index(t => t.ActualId);
            
            CreateTable(
                "plan.Actuals",
                c => new
                    {
                        ActualId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        PlanId = c.Int(),
                        IsDepartnental = c.Boolean(nullable: false),
                        IsPersonal = c.Boolean(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ActualId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: false)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "plan.BSCPlans",
                c => new
                    {
                        BSCPlanId = c.Int(nullable: false, identity: true),
                        ObjectiveId = c.Int(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        PlanId = c.Int(nullable: false),
                        IsCopied = c.Boolean(nullable: false),
                        IsFirstLevelApproved = c.Boolean(nullable: false),
                        IsSecondLevel = c.Boolean(nullable: false),
                        IsThirdLevel = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BSCPlanId)
                .ForeignKey("plan.Plans", t => t.PlanId, cascadeDelete: false)
                .ForeignKey("plan.Objectives", t => t.ObjectiveId, cascadeDelete: false)
                .Index(t => t.ObjectiveId)
                .Index(t => t.PlanId);
            
            CreateTable(
                "plan.Plans",
                c => new
                    {
                        PlanId = c.Int(nullable: false, identity: true),
                        IsDepartnental = c.Boolean(nullable: false),
                        IsPersonal = c.Boolean(nullable: false),
                        Description = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: false)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "plan.BusinessPlans",
                c => new
                    {
                        BusinessPlanId = c.Int(nullable: false, identity: true),
                        NameOfBusinessPlan = c.String(),
                        ProductType = c.Int(nullable: false),
                        StudyMethod = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BusinessPlanId);
            
            CreateTable(
                "plan.BusinessPlanValidations",
                c => new
                    {
                        BusinessPlanValidationId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        BusinessPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessPlanValidationId)
                .ForeignKey("plan.BusinessPlans", t => t.BusinessPlanId, cascadeDelete: false)
                .Index(t => t.BusinessPlanId);
            
            CreateTable(
                "EXCEED.ConsProjectType",
                c => new
                    {
                        ConsProjectTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ConsProjectTypeId);
            
            CreateTable(
                "plan.ExcutivePlanKPIs",
                c => new
                    {
                        ExcutivePlanKPIId = c.Int(nullable: false, identity: true),
                        IndicatorId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExcutivePlanKPIId);
            
            CreateTable(
                "plan.GlobalIndicators",
                c => new
                    {
                        GlobalIndicatorId = c.Int(nullable: false, identity: true),
                        Indicator = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlobalIndicatorId);
            
            CreateTable(
                "plan.LandManagements",
                c => new
                    {
                        LandManagementId = c.Int(nullable: false, identity: true),
                        ParticularNameLandPilot = c.String(),
                        Zone = c.String(),
                        CityOrTown = c.String(),
                        Area = c.String(),
                        TypesOfService = c.String(),
                        HoldingCertifcate = c.Int(nullable: false),
                        LandHoldingCertifcateNo = c.String(),
                        AcquiredBy = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LandManagementId);
            
            CreateTable(
                "plan.MonthlyActuals",
                c => new
                    {
                        MonthlyActualId = c.Int(nullable: false, identity: true),
                        GLPeriodId = c.Int(nullable: false),
                        PlanValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MonthlyPlanId = c.Int(),
                        ActualDetailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MonthlyActualId)
                .ForeignKey("plan.ActualDetail", t => t.ActualDetailId, cascadeDelete: false)
                .Index(t => t.ActualDetailId);
            
            CreateTable(
                "plan.MonthlyPlans",
                c => new
                    {
                        MonthlyPlanId = c.Int(nullable: false, identity: true),
                        GLPeriodId = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlanningDetailId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MonthlyPlanId)
                .ForeignKey("plan.PlanningDetails", t => t.PlanningDetailId, cascadeDelete: false)
                .Index(t => t.PlanningDetailId);
            
            CreateTable(
                "plan.PlanningDetails",
                c => new
                    {
                        PlanningDetailId = c.Int(nullable: false, identity: true),
                        Unit = c.String(),
                        KPIType = c.Int(nullable: false),
                        HasSubIndicator = c.Boolean(nullable: false),
                        ParentId = c.Int(nullable: false),
                        Plan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Baseline = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlobalIndicatorId = c.Int(nullable: false),
                        IndicatorId = c.Int(nullable: false),
                        BSCPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PlanningDetailId)
                .ForeignKey("plan.BSCPlans", t => t.BSCPlanId, cascadeDelete: false)
                .ForeignKey("plan.Indicators", t => t.IndicatorId, cascadeDelete: false)
                .Index(t => t.IndicatorId)
                .Index(t => t.BSCPlanId);
            
            CreateTable(
                "plan.ProjectActuals",
                c => new
                    {
                        ProjectActualId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        ProjectObjectiveId = c.Int(nullable: false),
                        ProjectKPIId = c.Int(nullable: false),
                        ProjectSubKPIId = c.Int(nullable: false),
                        Description = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectActualId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: false)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "plan.ProjectExcutiveSettings",
                c => new
                    {
                        ProjectExcutiveSettingId = c.Int(nullable: false, identity: true),
                        Category = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ProjectExcutiveSettingId);
            
            CreateTable(
                "plan.ProjectExcutiveSummarySettings",
                c => new
                    {
                        ProjectExcutiveSummarySettingId = c.Int(nullable: false, identity: true),
                        ProjectKPIId = c.Int(nullable: false),
                        ProjectSubKPIId = c.Int(nullable: false),
                        ProjectExcutiveSettingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectExcutiveSummarySettingId)
                .ForeignKey("plan.ProjectExcutiveSettings", t => t.ProjectExcutiveSettingId, cascadeDelete: false)
                .Index(t => t.ProjectExcutiveSettingId);
            
            CreateTable(
                "plan.ProjectKPIs",
                c => new
                    {
                        ProjectKPIId = c.Int(nullable: false, identity: true),
                        KPIName = c.String(nullable: false),
                        Description = c.String(),
                        ProjectKPIType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectKPIId);
            
            CreateTable(
                "plan.ProjectMonthlyActuals",
                c => new
                    {
                        ProjectMonthlyActualId = c.Int(nullable: false, identity: true),
                        GLPeriodId = c.Int(nullable: false),
                        PlanValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectActualId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectMonthlyActualId)
                .ForeignKey("plan.ProjectActuals", t => t.ProjectActualId, cascadeDelete: false)
                .Index(t => t.ProjectActualId);
            
            CreateTable(
                "plan.ProjectMonthlyPlans",
                c => new
                    {
                        ProjectMonthlyPlanId = c.Int(nullable: false, identity: true),
                        GLPeriodId = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectMonthlyPlanId)
                .ForeignKey("plan.ProjectPlans", t => t.ProjectPlanId, cascadeDelete: false)
                .Index(t => t.ProjectPlanId);
            
            CreateTable(
                "plan.ProjectPlans",
                c => new
                    {
                        ProjectPlanId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        ProjectObjectiveId = c.Int(nullable: false),
                        ProjectKPIId = c.Int(nullable: false),
                        ProjectSubKPIId = c.Int(nullable: false),
                        Baseline = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Target = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Plan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        GlFiscalYearId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectPlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: false)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "plan.ProjectObjectives",
                c => new
                    {
                        ProjectObjectiveId = c.Int(nullable: false, identity: true),
                        ProjectObjectiveName = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ProjectObjectiveId);
            
            CreateTable(
                "plan.ProjectSubKPIs",
                c => new
                    {
                        ProjectSubKPIId = c.Int(nullable: false, identity: true),
                        SubKPIName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        KPIType = c.Int(nullable: false),
                        Description = c.String(),
                        ProjectKPIId = c.Int(nullable: false),
                        Parent_ProjectSubKPIId = c.Int(),
                    })
                .PrimaryKey(t => t.ProjectSubKPIId)
                .ForeignKey("plan.ProjectSubKPIs", t => t.Parent_ProjectSubKPIId)
                .Index(t => t.Parent_ProjectSubKPIId);
            
            CreateTable(
                "plan.ProjectWeeklyActuals",
                c => new
                    {
                        ProjectWeeklyActualId = c.Int(nullable: false, identity: true),
                        WeekOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekFour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectMonthlyActualId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectWeeklyActualId)
                .ForeignKey("plan.ProjectMonthlyActuals", t => t.ProjectMonthlyActualId, cascadeDelete: false)
                .Index(t => t.ProjectMonthlyActualId);
            
            CreateTable(
                "plan.ProjectWeeklyPlans",
                c => new
                    {
                        ProjectWeeklyPlanId = c.Int(nullable: false, identity: true),
                        WeekOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekFour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectMonthlyPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectWeeklyPlanId)
                .ForeignKey("plan.ProjectMonthlyPlans", t => t.ProjectMonthlyPlanId, cascadeDelete: false)
                .Index(t => t.ProjectMonthlyPlanId);
            
            CreateTable(
                "plan.RiskFollowUps",
                c => new
                    {
                        RiskFollowUpId = c.Int(nullable: false, identity: true),
                        PlanDate = c.DateTime(nullable: false),
                        Description = c.String(),
                        TreatmentMethod = c.String(),
                        PlannedActionToBeTaken = c.String(),
                        ActualActionTaken = c.String(),
                        CurrentStatus = c.String(),
                        RiskMitigationPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RiskFollowUpId)
                .ForeignKey("plan.RiskMitigationPlans", t => t.RiskMitigationPlanId, cascadeDelete: false)
                .Index(t => t.RiskMitigationPlanId);
            
            CreateTable(
                "plan.RiskMitigationPlans",
                c => new
                    {
                        RiskMitigationPlanId = c.Int(nullable: false, identity: true),
                        IdentifiedRisk = c.String(),
                        RiskLevel = c.Int(nullable: false),
                        TreatmentMethod = c.String(),
                        ActionToBeTaken = c.String(),
                        MainResponsibly = c.String(),
                        SupportResponsibly = c.String(),
                        Opportunity = c.String(),
                        Time = c.String(),
                        MonitoringMethod = c.String(),
                        Evaluation = c.String(),
                        PlanDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RiskMitigationPlanId);
            
            CreateTable(
                "plan.ShareAndDividendFiscalYearDetails",
                c => new
                    {
                        ShareAndDividendFiscalYearDetailId = c.Int(nullable: false, identity: true),
                        PlanToPayShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaidShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DividendObtaind = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShareAndDividendId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ShareAndDividendFiscalYearDetailId)
                .ForeignKey("plan.ShareAndDividends", t => t.ShareAndDividendId, cascadeDelete: false)
                .Index(t => t.ShareAndDividendId);
            
            CreateTable(
                "plan.ShareAndDividends",
                c => new
                    {
                        ShareAndDividendId = c.Int(nullable: false, identity: true),
                        ShareCompanyName = c.String(),
                        SubscribedShareAmtNoofShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubscribedShareAmtBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaidUpShareAmtNoOfShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaidUpShareAmtBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GlFiscalYearId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ShareAndDividendId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: false)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "plan.WeeklyActuals",
                c => new
                    {
                        WeeklyActualId = c.Int(nullable: false, identity: true),
                        WeekOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekFour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeeklyPlanId = c.Int(),
                        MonthlyActualId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WeeklyActualId)
                .ForeignKey("plan.MonthlyActuals", t => t.MonthlyActualId, cascadeDelete: false)
                .Index(t => t.MonthlyActualId);
            
            CreateTable(
                "plan.WeeklyPlans",
                c => new
                    {
                        WeeklyPlanId = c.Int(nullable: false, identity: true),
                        WeekOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WeekFour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualWeekOne = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualWeekTwo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualWeekThree = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualWeekFour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MonthlyPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WeeklyPlanId)
                .ForeignKey("plan.MonthlyPlans", t => t.MonthlyPlanId, cascadeDelete: false)
                .Index(t => t.MonthlyPlanId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("plan.WeeklyPlans", "MonthlyPlanId", "plan.MonthlyPlans");
            DropForeignKey("plan.WeeklyActuals", "MonthlyActualId", "plan.MonthlyActuals");
            DropForeignKey("plan.ShareAndDividendFiscalYearDetails", "ShareAndDividendId", "plan.ShareAndDividends");
            DropForeignKey("plan.ShareAndDividends", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.RiskFollowUps", "RiskMitigationPlanId", "plan.RiskMitigationPlans");
            DropForeignKey("plan.ProjectWeeklyPlans", "ProjectMonthlyPlanId", "plan.ProjectMonthlyPlans");
            DropForeignKey("plan.ProjectWeeklyActuals", "ProjectMonthlyActualId", "plan.ProjectMonthlyActuals");
            DropForeignKey("plan.ProjectSubKPIs", "Parent_ProjectSubKPIId", "plan.ProjectSubKPIs");
            DropForeignKey("plan.ProjectMonthlyPlans", "ProjectPlanId", "plan.ProjectPlans");
            DropForeignKey("plan.ProjectPlans", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.ProjectMonthlyActuals", "ProjectActualId", "plan.ProjectActuals");
            DropForeignKey("plan.ProjectExcutiveSummarySettings", "ProjectExcutiveSettingId", "plan.ProjectExcutiveSettings");
            DropForeignKey("plan.ProjectActuals", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.MonthlyPlans", "PlanningDetailId", "plan.PlanningDetails");
            DropForeignKey("plan.PlanningDetails", "IndicatorId", "plan.Indicators");
            DropForeignKey("plan.PlanningDetails", "BSCPlanId", "plan.BSCPlans");
            DropForeignKey("plan.MonthlyActuals", "ActualDetailId", "plan.ActualDetail");
            DropForeignKey("plan.BusinessPlanValidations", "BusinessPlanId", "plan.BusinessPlans");
            DropForeignKey("plan.BSCPlans", "ObjectiveId", "plan.Objectives");
            DropForeignKey("plan.BSCPlans", "PlanId", "plan.Plans");
            DropForeignKey("plan.Plans", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.ActualDetail", "BSCActualId", "plan.BSCActuals");
            DropForeignKey("plan.BSCActuals", "ObjectiveId", "plan.Objectives");
            DropForeignKey("plan.BSCActuals", "ActualId", "plan.Actuals");
            DropForeignKey("plan.Actuals", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.ActualDetail", "IndicatorId", "plan.Indicators");
            DropForeignKey("plan.Indicators", "ObjectiveId", "plan.Objectives");
            DropIndex("plan.WeeklyPlans", new[] { "MonthlyPlanId" });
            DropIndex("plan.WeeklyActuals", new[] { "MonthlyActualId" });
            DropIndex("plan.ShareAndDividends", new[] { "GlFiscalYearId" });
            DropIndex("plan.ShareAndDividendFiscalYearDetails", new[] { "ShareAndDividendId" });
            DropIndex("plan.RiskFollowUps", new[] { "RiskMitigationPlanId" });
            DropIndex("plan.ProjectWeeklyPlans", new[] { "ProjectMonthlyPlanId" });
            DropIndex("plan.ProjectWeeklyActuals", new[] { "ProjectMonthlyActualId" });
            DropIndex("plan.ProjectSubKPIs", new[] { "Parent_ProjectSubKPIId" });
            DropIndex("plan.ProjectPlans", new[] { "GlFiscalYearId" });
            DropIndex("plan.ProjectMonthlyPlans", new[] { "ProjectPlanId" });
            DropIndex("plan.ProjectMonthlyActuals", new[] { "ProjectActualId" });
            DropIndex("plan.ProjectExcutiveSummarySettings", new[] { "ProjectExcutiveSettingId" });
            DropIndex("plan.ProjectActuals", new[] { "GlFiscalYearId" });
            DropIndex("plan.PlanningDetails", new[] { "BSCPlanId" });
            DropIndex("plan.PlanningDetails", new[] { "IndicatorId" });
            DropIndex("plan.MonthlyPlans", new[] { "PlanningDetailId" });
            DropIndex("plan.MonthlyActuals", new[] { "ActualDetailId" });
            DropIndex("plan.BusinessPlanValidations", new[] { "BusinessPlanId" });
            DropIndex("plan.Plans", new[] { "GlFiscalYearId" });
            DropIndex("plan.BSCPlans", new[] { "PlanId" });
            DropIndex("plan.BSCPlans", new[] { "ObjectiveId" });
            DropIndex("plan.Actuals", new[] { "GlFiscalYearId" });
            DropIndex("plan.BSCActuals", new[] { "ActualId" });
            DropIndex("plan.BSCActuals", new[] { "ObjectiveId" });
            DropIndex("plan.Indicators", new[] { "ObjectiveId" });
            DropIndex("plan.ActualDetail", new[] { "BSCActualId" });
            DropIndex("plan.ActualDetail", new[] { "IndicatorId" });
            DropTable("plan.WeeklyPlans");
            DropTable("plan.WeeklyActuals");
            DropTable("plan.ShareAndDividends");
            DropTable("plan.ShareAndDividendFiscalYearDetails");
            DropTable("plan.RiskMitigationPlans");
            DropTable("plan.RiskFollowUps");
            DropTable("plan.ProjectWeeklyPlans");
            DropTable("plan.ProjectWeeklyActuals");
            DropTable("plan.ProjectSubKPIs");
            DropTable("plan.ProjectObjectives");
            DropTable("plan.ProjectPlans");
            DropTable("plan.ProjectMonthlyPlans");
            DropTable("plan.ProjectMonthlyActuals");
            DropTable("plan.ProjectKPIs");
            DropTable("plan.ProjectExcutiveSummarySettings");
            DropTable("plan.ProjectExcutiveSettings");
            DropTable("plan.ProjectActuals");
            DropTable("plan.PlanningDetails");
            DropTable("plan.MonthlyPlans");
            DropTable("plan.MonthlyActuals");
            DropTable("plan.LandManagements");
            DropTable("plan.GlobalIndicators");
            DropTable("plan.ExcutivePlanKPIs");
            DropTable("EXCEED.ConsProjectType");
            DropTable("plan.BusinessPlanValidations");
            DropTable("plan.BusinessPlans");
            DropTable("plan.Plans");
            DropTable("plan.BSCPlans");
            DropTable("plan.Actuals");
            DropTable("plan.BSCActuals");
            DropTable("plan.Objectives");
            DropTable("plan.Indicators");
            DropTable("plan.ActualDetail");
        }
    }
}
