namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanningandHRmModuleChangeMigration : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "EXCEED.HolidayGiftRateSetting", newName: "HolidayGiftRateSettings");
            MoveTable(name: "EXCEED.HolidayGiftEmployee", newSchema: "HRM");
            MoveTable(name: "EXCEED.HolidayGiftRateSettings", newSchema: "HRM");
            CreateTable(
                "plan.StrategicPlans",
                c => new
                    {
                        StrategicPlanId = c.Int(nullable: false, identity: true),
                        IsDepartnental = c.Boolean(nullable: false),
                        IsPersonal = c.Boolean(nullable: false),
                        Description = c.String(),
                        FromGlFiscalYearId = c.Int(nullable: false),
                        ToGlFiscalYearId = c.Int(nullable: false),
                        StrategicObjective = c.String(),
                        TotalBudget = c.Decimal(precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StrategicPlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.FromGlFiscalYearId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.ToGlFiscalYearId)
                .Index(t => t.FromGlFiscalYearId)
                .Index(t => t.ToGlFiscalYearId);
            
            CreateTable(
                "plan.AnnualGoals",
                c => new
                    {
                        AnnualGoalId = c.Int(nullable: false, identity: true),
                        StrategicPlanId = c.Int(nullable: false),
                        GlFiscalYearId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnnualGoalId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId)
                .ForeignKey("plan.StrategicPlans", t => t.StrategicPlanId)
                .Index(t => t.StrategicPlanId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "EXCEED.BusinessPlanAttachment",
                c => new
                    {
                        BusinessPlanAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        BusinessPlanId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessPlanAttachmentId)
                .ForeignKey("plan.BusinessPlans", t => t.BusinessPlanId, cascadeDelete: true)
                .Index(t => t.BusinessPlanId);
            
            CreateTable(
                "HRM.ExpenseAndTaxSettings",
                c => new
                    {
                        ExpenseAndTaxSettingId = c.Int(nullable: false, identity: true),
                        ApSetupItemId = c.Int(nullable: false),
                        ExpenseItemType = c.Int(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExpenseAndTaxSettingId)
                .ForeignKey("EXCEED.ApSetupItem", t => t.ApSetupItemId, cascadeDelete: true)
                .Index(t => t.ApSetupItemId);
            
            CreateTable(
                "plan.GlobalKPI",
                c => new
                    {
                        GlobalKPIId = c.Int(nullable: false, identity: true),
                        KPIName = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        StrategicObjective = c.String(),
                        KeyResultArea = c.String(),
                        AnnualObjective = c.String(),
                        StandardKPI = c.String(),
                        MeasurementFormula = c.String(),
                        AnnualTargetText = c.String(),
                        ThresholdText = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlobalKPIId);
            
            CreateTable(
                "plan.GlobalSubKPI",
                c => new
                    {
                        GlobalSubKPIId = c.Int(nullable: false, identity: true),
                        SubKPIName = c.String(nullable: false),
                        GlobalKPIId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GlobalSubKPIId)
                .ForeignKey("plan.GlobalKPI", t => t.GlobalKPIId, cascadeDelete: true)
                .Index(t => t.GlobalKPIId);
            
            CreateTable(
                "HRM.HRMContributionSettings",
                c => new
                    {
                        HRMContributionSettingId = c.Int(nullable: false, identity: true),
                        ContributionTypeId = c.Int(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HRMContributionSettingId)
                .ForeignKey("HRM.ContributionType", t => t.ContributionTypeId, cascadeDelete: true)
                .Index(t => t.ContributionTypeId);
            
            CreateTable(
                "plan.KpiProgressTrackers",
                c => new
                    {
                        KpiProgressTrackerId = c.Int(nullable: false, identity: true),
                        GlobalKPIId = c.Int(nullable: false),
                        Weight = c.Decimal(precision: 18, scale: 2),
                        AnnualPlan = c.Decimal(precision: 18, scale: 2),
                        UpTo2ndQuarterPlan = c.Decimal(precision: 18, scale: 2),
                        UpTo2ndQuarterPerformance = c.Decimal(precision: 18, scale: 2),
                        UpTo2ndQuarterPercent = c.Decimal(precision: 18, scale: 2),
                        Evaluation = c.Decimal(precision: 18, scale: 2),
                        Remark = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.KpiProgressTrackerId)
                .ForeignKey("plan.GlobalKPI", t => t.GlobalKPIId, cascadeDelete: true)
                .Index(t => t.GlobalKPIId);
            
            CreateTable(
                "plan.LandManagementAttachments",
                c => new
                    {
                        LandManagementAttachmentId = c.Int(nullable: false, identity: true),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        LandManagementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LandManagementAttachmentId)
                .ForeignKey("plan.LandManagements", t => t.LandManagementId, cascadeDelete: true)
                .Index(t => t.LandManagementId);
            
            CreateTable(
                "plan.PastPlanAttachments",
                c => new
                    {
                        PastPlanAttachmentId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        PlanDate = c.DateTime(nullable: false),
                        Title = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 2000),
                        FileName = c.String(nullable: false, maxLength: 260),
                        StoredFileName = c.String(nullable: false, maxLength: 100),
                        ContentType = c.String(maxLength: 200),
                        FileSize = c.Int(nullable: false),
                        UploadedAt = c.DateTime(nullable: false),
                        UploadedBy = c.String(),
                    })
                .PrimaryKey(t => t.PastPlanAttachmentId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "plan.PlanApprovalFeedbacks",
                c => new
                    {
                        PlanApprovalFeedbackId = c.Int(nullable: false, identity: true),
                        BSCPlanId = c.Int(nullable: false),
                        Action = c.Int(nullable: false),
                        FromStatus = c.Int(nullable: false),
                        ToStatus = c.Int(nullable: false),
                        Feedback = c.String(maxLength: 2000),
                        ActionBy = c.String(maxLength: 256),
                        ActionTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PlanApprovalFeedbackId)
                .ForeignKey("plan.BSCPlans", t => t.BSCPlanId)
                .Index(t => t.BSCPlanId);
            
            CreateTable(
                "plan.QuickFixPlans",
                c => new
                    {
                        QuickFixPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        PlanId = c.Int(nullable: false),
                        AnnualPlan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpToQuarter = c.Int(nullable: false),
                        PlanUpToQuarter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Actual = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreviousPlanRemainingQuarter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RevisedPlanRemainingQuarter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QuickFixPlanId)
                .ForeignKey("plan.Plans", t => t.PlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId)
                .Index(t => t.GlFiscalYearId)
                .Index(t => t.PlanId);
            
            CreateTable(
                "HRM.RewardSettings",
                c => new
                    {
                        RewardSettingId = c.Int(nullable: false, identity: true),
                        MinimumMonthsRequired = c.Int(nullable: false),
                        AllowNewEmployees = c.Boolean(nullable: false),
                        AllowTransferredEmployees = c.Boolean(nullable: false),
                        AllowDisciplinedEmployees = c.Boolean(nullable: false),
                        MaximumDisciplinedMonthsRequired = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RewardSettingId);
            
            CreateTable(
                "plan.StrategicGoals",
                c => new
                    {
                        StrategicGoalId = c.Int(nullable: false, identity: true),
                        StrategicPlanId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Remark = c.String(),
                        StrategicObjective = c.String(),
                        Code = c.String(),
                        ResponsibleBody = c.String(),
                        UOM = c.String(),
                        Baseline = c.String(),
                        Target2015EFY = c.String(),
                        Target2016EFY = c.String(),
                        Target2017EFY = c.String(),
                        Target2018EFY = c.String(),
                        Target2019EFY = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StrategicGoalId)
                .ForeignKey("plan.StrategicPlans", t => t.StrategicPlanId)
                .Index(t => t.StrategicPlanId);
            
            CreateTable(
                "plan.SubKpiProgressTrackers",
                c => new
                    {
                        SubKpiProgressTrackerId = c.Int(nullable: false, identity: true),
                        GlobalSubKPIId = c.Int(nullable: false),
                        Indicators = c.String(),
                        Saf = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AnnualPlan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpTo2ndQuarterPlan = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpTo2ndQuarterPerformance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpTo2ndQuarterPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Evaluation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        KpiProgressTrackerId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SubKpiProgressTrackerId)
                .ForeignKey("plan.KpiProgressTrackers", t => t.KpiProgressTrackerId, cascadeDelete: true)
                .Index(t => t.KpiProgressTrackerId);
            
            CreateTable(
                "HRM.StaffDebtorBalance",
                c => new
                    {
                        StaffDebtorBalanceId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        TerminationId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        Description = c.String(),
                        Debit = c.Decimal(precision: 18, scale: 2),
                        Credit = c.Decimal(precision: 18, scale: 2),
                        Balance = c.Decimal(precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StaffDebtorBalanceId);
            
            CreateTable(
                "HRM.SalaryStructureVersion",
                c => new
                    {
                        SalaryStructureVersionId = c.Int(nullable: false, identity: true),
                        StructureVersionName = c.String(nullable: false),
                        Remark = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SalaryStructureVersionId);
            
            CreateTable(
                "HRM.PasswordResetRequests",
                c => new
                    {
                        PasswordResetRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        OrgStructureId = c.Int(),
                        BranchId = c.Int(nullable: false),
                        LookupId = c.Int(),
                        BusinessUnitId = c.Int(),
                        UserName = c.String(),
                        Email = c.String(),
                        Token = c.String(),
                        RequestedAt = c.DateTime(nullable: false),
                        RequestedAtAm = c.String(),
                        Expiration = c.DateTime(nullable: false),
                        ExpirationAm = c.String(),
                        ResetType = c.String(),
                        IsUsed = c.Boolean(nullable: false),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
                        ForwardedTime = c.DateTime(),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PasswordResetRequestId)
                .ForeignKey("HRM.Employee", t => t.EmployeeId, cascadeDelete: true)
                .Index(t => t.EmployeeId);
            
            CreateTable(
                "HRM.BusinessUnitBranch",
                c => new
                    {
                        BusinessUnitBranchId = c.Int(nullable: false, identity: true),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        IsSubBranch = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.BusinessUnitBranchId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("HRM.BusinessUnit", t => t.BusinessUnitId, cascadeDelete: true)
                .Index(t => t.BranchId)
                .Index(t => t.BusinessUnitId);
            
            CreateTable(
                "HRM.BranchRankMapping",
                c => new
                    {
                        BranchRankMappingId = c.Int(nullable: false, identity: true),
                        RankId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        MaximumSalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BranchRankMappingId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("HRM.Rank", t => t.RankId, cascadeDelete: true)
                .Index(t => t.RankId)
                .Index(t => t.BranchId);
            
            AddColumn("plan.Indicators", "Weight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("plan.Indicators", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("plan.Objectives", "StrategicPlanId", c => c.Int());
            AddColumn("plan.Objectives", "Weight", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectInformation", "ProjectTypeId", c => c.Int(nullable: false));
            AddColumn("HRM.MainCompanyStructure", "Code", c => c.String());
            AddColumn("HRM.MainCompanyStructure", "CostCenter", c => c.String());
            AddColumn("HRM.MainCompanyStructure", "BusinessUnitId", c => c.Int());
            AddColumn("plan.BSCPlans", "BranchId", c => c.Int());
            AddColumn("plan.BSCPlans", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("plan.BSCPlans", "SubmittedBy", c => c.String(maxLength: 256));
            AddColumn("plan.BSCPlans", "SubmittedTime", c => c.DateTime());
            AddColumn("plan.BSCPlans", "BranchApprovedBy", c => c.String(maxLength: 256));
            AddColumn("plan.BSCPlans", "BranchApprovedTime", c => c.DateTime());
            AddColumn("plan.BSCPlans", "HeadOfficeApprovedBy", c => c.String(maxLength: 256));
            AddColumn("plan.BSCPlans", "HeadOfficeApprovedTime", c => c.DateTime());
            AddColumn("plan.BSCPlans", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            AddColumn("plan.Plans", "Reason", c => c.String(maxLength: 2000));
            AddColumn("EXCEED.ConsProjectType", "BusinessUnit", c => c.String());
            AddColumn("HRM.EmployeeRewards", "Salary", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.EmployeeRewards", "PersonOfSalary", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("HRM.EmployeeRewards", "GlFiscalYearId", c => c.Int());
            AddColumn("HRM.EmployeeRewards", "GLPeriodId", c => c.Int());
            AddColumn("HRM.EmployeeRewards", "PayrollAccount", c => c.String());
            AddColumn("plan.GlobalIndicators", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("plan.GlobalIndicators", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("plan.PlanningDetails", "StrategicObjective", c => c.String());
            AddColumn("plan.PlanningDetails", "KeyResultArea", c => c.String());
            AddColumn("plan.PlanningDetails", "AnnualObjective", c => c.String());
            AddColumn("plan.PlanningDetails", "StandardKPI", c => c.String());
            AddColumn("plan.PlanningDetails", "MeasurementFormula", c => c.String());
            AddColumn("plan.PlanningDetails", "AnnualTargetText", c => c.String());
            AddColumn("plan.PlanningDetails", "ThresholdText", c => c.String());
            AddColumn("plan.RiskMitigationPlans", "IsClosed", c => c.Boolean(nullable: false));
            CreateIndex("plan.Objectives", "StrategicPlanId");
            AddForeignKey("plan.Objectives", "StrategicPlanId", "plan.StrategicPlans", "StrategicPlanId");
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.BranchRankMapping", "RankId", "HRM.Rank");
            DropForeignKey("HRM.BranchRankMapping", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.BusinessUnitBranch", "BusinessUnitId", "HRM.BusinessUnit");
            DropForeignKey("HRM.BusinessUnitBranch", "BranchId", "HRM.Branch");
            DropForeignKey("HRM.PasswordResetRequests", "EmployeeId", "HRM.Employee");
            DropForeignKey("plan.SubKpiProgressTrackers", "KpiProgressTrackerId", "plan.KpiProgressTrackers");
            DropForeignKey("plan.StrategicGoals", "StrategicPlanId", "plan.StrategicPlans");
            DropForeignKey("plan.QuickFixPlans", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.QuickFixPlans", "PlanId", "plan.Plans");
            DropForeignKey("plan.PlanApprovalFeedbacks", "BSCPlanId", "plan.BSCPlans");
            DropForeignKey("plan.PastPlanAttachments", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.LandManagementAttachments", "LandManagementId", "plan.LandManagements");
            DropForeignKey("plan.KpiProgressTrackers", "GlobalKPIId", "plan.GlobalKPI");
            DropForeignKey("HRM.HRMContributionSettings", "ContributionTypeId", "HRM.ContributionType");
            DropForeignKey("plan.GlobalSubKPI", "GlobalKPIId", "plan.GlobalKPI");
            DropForeignKey("HRM.ExpenseAndTaxSettings", "ApSetupItemId", "EXCEED.ApSetupItem");
            DropForeignKey("EXCEED.BusinessPlanAttachment", "BusinessPlanId", "plan.BusinessPlans");
            DropForeignKey("plan.AnnualGoals", "StrategicPlanId", "plan.StrategicPlans");
            DropForeignKey("plan.AnnualGoals", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.Objectives", "StrategicPlanId", "plan.StrategicPlans");
            DropForeignKey("plan.StrategicPlans", "ToGlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("plan.StrategicPlans", "FromGlFiscalYearId", "EXCEED.GlFiscalYear");
            DropIndex("HRM.BranchRankMapping", new[] { "BranchId" });
            DropIndex("HRM.BranchRankMapping", new[] { "RankId" });
            DropIndex("HRM.BusinessUnitBranch", new[] { "BusinessUnitId" });
            DropIndex("HRM.BusinessUnitBranch", new[] { "BranchId" });
            DropIndex("HRM.PasswordResetRequests", new[] { "EmployeeId" });
            DropIndex("plan.SubKpiProgressTrackers", new[] { "KpiProgressTrackerId" });
            DropIndex("plan.StrategicGoals", new[] { "StrategicPlanId" });
            DropIndex("plan.QuickFixPlans", new[] { "PlanId" });
            DropIndex("plan.QuickFixPlans", new[] { "GlFiscalYearId" });
            DropIndex("plan.PlanApprovalFeedbacks", new[] { "BSCPlanId" });
            DropIndex("plan.PastPlanAttachments", new[] { "GlFiscalYearId" });
            DropIndex("plan.LandManagementAttachments", new[] { "LandManagementId" });
            DropIndex("plan.KpiProgressTrackers", new[] { "GlobalKPIId" });
            DropIndex("HRM.HRMContributionSettings", new[] { "ContributionTypeId" });
            DropIndex("plan.GlobalSubKPI", new[] { "GlobalKPIId" });
            DropIndex("HRM.ExpenseAndTaxSettings", new[] { "ApSetupItemId" });
            DropIndex("EXCEED.BusinessPlanAttachment", new[] { "BusinessPlanId" });
            DropIndex("plan.AnnualGoals", new[] { "GlFiscalYearId" });
            DropIndex("plan.AnnualGoals", new[] { "StrategicPlanId" });
            DropIndex("plan.StrategicPlans", new[] { "ToGlFiscalYearId" });
            DropIndex("plan.StrategicPlans", new[] { "FromGlFiscalYearId" });
            DropIndex("plan.Objectives", new[] { "StrategicPlanId" });
            DropColumn("plan.RiskMitigationPlans", "IsClosed");
            DropColumn("plan.PlanningDetails", "ThresholdText");
            DropColumn("plan.PlanningDetails", "AnnualTargetText");
            DropColumn("plan.PlanningDetails", "MeasurementFormula");
            DropColumn("plan.PlanningDetails", "StandardKPI");
            DropColumn("plan.PlanningDetails", "AnnualObjective");
            DropColumn("plan.PlanningDetails", "KeyResultArea");
            DropColumn("plan.PlanningDetails", "StrategicObjective");
            DropColumn("plan.GlobalIndicators", "IsActive");
            DropColumn("plan.GlobalIndicators", "OrgStructureId");
            DropColumn("HRM.EmployeeRewards", "PayrollAccount");
            DropColumn("HRM.EmployeeRewards", "GLPeriodId");
            DropColumn("HRM.EmployeeRewards", "GlFiscalYearId");
            DropColumn("HRM.EmployeeRewards", "PersonOfSalary");
            DropColumn("HRM.EmployeeRewards", "Salary");
            DropColumn("EXCEED.ConsProjectType", "BusinessUnit");
            DropColumn("plan.Plans", "Reason");
            DropColumn("plan.BSCPlans", "RowVersion");
            DropColumn("plan.BSCPlans", "HeadOfficeApprovedTime");
            DropColumn("plan.BSCPlans", "HeadOfficeApprovedBy");
            DropColumn("plan.BSCPlans", "BranchApprovedTime");
            DropColumn("plan.BSCPlans", "BranchApprovedBy");
            DropColumn("plan.BSCPlans", "SubmittedTime");
            DropColumn("plan.BSCPlans", "SubmittedBy");
            DropColumn("plan.BSCPlans", "ApprovalStatus");
            DropColumn("plan.BSCPlans", "BranchId");
            DropColumn("HRM.MainCompanyStructure", "BusinessUnitId");
            DropColumn("HRM.MainCompanyStructure", "CostCenter");
            DropColumn("HRM.MainCompanyStructure", "Code");
            DropColumn("Construction.ProjectInformation", "ProjectTypeId");
            DropColumn("plan.Objectives", "Weight");
            DropColumn("plan.Objectives", "StrategicPlanId");
            DropColumn("plan.Indicators", "IsActive");
            DropColumn("plan.Indicators", "Weight");
            DropTable("HRM.BranchRankMapping");
            DropTable("HRM.BusinessUnitBranch");
            DropTable("HRM.PasswordResetRequests");
            DropTable("HRM.SalaryStructureVersion");
            DropTable("HRM.StaffDebtorBalance");
            DropTable("plan.SubKpiProgressTrackers");
            DropTable("plan.StrategicGoals");
            DropTable("HRM.RewardSettings");
            DropTable("plan.QuickFixPlans");
            DropTable("plan.PlanApprovalFeedbacks");
            DropTable("plan.PastPlanAttachments");
            DropTable("plan.LandManagementAttachments");
            DropTable("plan.KpiProgressTrackers");
            DropTable("HRM.HRMContributionSettings");
            DropTable("plan.GlobalSubKPI");
            DropTable("plan.GlobalKPI");
            DropTable("HRM.ExpenseAndTaxSettings");
            DropTable("EXCEED.BusinessPlanAttachment");
            DropTable("plan.AnnualGoals");
            DropTable("plan.StrategicPlans");
            MoveTable(name: "HRM.HolidayGiftRateSettings", newSchema: "EXCEED");
            MoveTable(name: "HRM.HolidayGiftEmployee", newSchema: "EXCEED");
            RenameTable(name: "EXCEED.HolidayGiftRateSettings", newName: "HolidayGiftRateSetting");
        }
    }
}
