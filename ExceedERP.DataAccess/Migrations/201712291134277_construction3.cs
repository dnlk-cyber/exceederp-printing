namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.MonthlyProjectQualityComplaints", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityAssurance", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityControl", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityPlanning", "PeriodMappingId", "Construction.PeriodMapping");
            DropIndex("Construction.MonthlyProjectQualityComplaints", new[] { "PeriodMappingId" });
            DropIndex("Construction.QualityAssurance", new[] { "PeriodMappingId" });
            DropIndex("Construction.QualityControl", new[] { "PeriodMappingId" });
            DropIndex("Construction.QualityPlanning", new[] { "PeriodMappingId" });
            RenameColumn(table: "Construction.QualityAssurance", name: "PeriodMappingId", newName: "PeriodMapping_PeriodMappingId");
            RenameColumn(table: "Construction.QualityControl", name: "PeriodMappingId", newName: "PeriodMapping_PeriodMappingId");
            RenameColumn(table: "Construction.QualityPlanning", name: "PeriodMappingId", newName: "PeriodMapping_PeriodMappingId");
            CreateTable(
                "Construction.AnalysisTechniqueApplicationForMonthlyClaims",
                c => new
                    {
                        AnalysisTechniqueApplicationForMonthlyClaimsId = c.Int(nullable: false, identity: true),
                        MonthlyProjectClaimId = c.Int(nullable: false),
                        WasThereIssueToClaim = c.Boolean(nullable: false),
                        AnalysisTechnique = c.Int(nullable: false),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AnalysisTechniqueApplicationForMonthlyClaimsId)
                .ForeignKey("Construction.MonthlyProjectClaim", t => t.MonthlyProjectClaimId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectClaimId);
            
            CreateTable(
                "Construction.MonthlyProjectClaim",
                c => new
                    {
                        MonthlyProjectClaimId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        ContempraryRecordKeeping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FullFillingRequirementForClaims = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AnalysisTechnique = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ApprovalStatusOfClaims = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsClaimPreparedAndIssuedInThisMonth = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectClaimId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.ApprovalStatusForClaimsResponded",
                c => new
                    {
                        ApprovalStatusForClaimsRespondedId = c.Int(nullable: false, identity: true),
                        MonthlyProjectClaimId = c.Int(nullable: false),
                        IsApprovalMadeInTheMonth = c.Boolean(nullable: false),
                        ApprovalStatus = c.Int(nullable: false),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApprovalStatusForClaimsRespondedId)
                .ForeignKey("Construction.MonthlyProjectClaim", t => t.MonthlyProjectClaimId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectClaimId);
            
            CreateTable(
                "Construction.ContempraryRecord",
                c => new
                    {
                        ContempraryRecordId = c.Int(nullable: false, identity: true),
                        MonthlyProjectClaimId = c.Int(nullable: false),
                        Description = c.String(),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ContempraryRecordId)
                .ForeignKey("Construction.MonthlyProjectClaim", t => t.MonthlyProjectClaimId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectClaimId);
            
            CreateTable(
                "Construction.RequirementForMonthlyClaims",
                c => new
                    {
                        RequirementForMonthlyClaimsId = c.Int(nullable: false, identity: true),
                        MonthlyProjectClaimId = c.Int(nullable: false),
                        Description = c.String(),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RequirementForMonthlyClaimsId)
                .ForeignKey("Construction.MonthlyProjectClaim", t => t.MonthlyProjectClaimId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectClaimId);
            
            CreateTable(
                "Construction.MonthlyProjectCommunication",
                c => new
                    {
                        MonthlyProjectCommunicationId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        ComnWithinProject = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ComnWithHeadOffice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ComnWithConsultant = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfComplaints = c.Int(nullable: false),
                        GoodGovernanceComplaints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        TotalPoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectCommunicationId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.MonthlyProjectCommunicationWithHeadOffice",
                c => new
                    {
                        MonthlyProjectCommunicationWithHeadOfficeId = c.Int(nullable: false, identity: true),
                        MonthlyProjectCommunicationId = c.Int(nullable: false),
                        CommunicationMethod = c.Int(nullable: false),
                        MaxAllowedPoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectCommunicationWithHeadOfficeId)
                .ForeignKey("Construction.MonthlyProjectCommunication", t => t.MonthlyProjectCommunicationId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectCommunicationId);
            
            CreateTable(
                "Construction.MonthlyProjectCommunicationWithinProject",
                c => new
                    {
                        MonthlyProjectCommunicationWithinProjectId = c.Int(nullable: false, identity: true),
                        MonthlyProjectCommunicationId = c.Int(nullable: false),
                        Description = c.String(),
                        MaxAllowedPoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectCommunicationWithinProjectId)
                .ForeignKey("Construction.MonthlyProjectCommunication", t => t.MonthlyProjectCommunicationId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectCommunicationId);
            
            CreateTable(
                "Construction.MonthlyProjectCommunicationWithConsultant",
                c => new
                    {
                        MonthlyProjectCommunicationWithConsultantId = c.Int(nullable: false, identity: true),
                        MonthlyProjectCommunicationId = c.Int(nullable: false),
                        CommunicationLevel = c.Int(nullable: false),
                        Description = c.String(),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectCommunicationWithConsultantId)
                .ForeignKey("Construction.MonthlyProjectCommunication", t => t.MonthlyProjectCommunicationId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectCommunicationId);
            
            CreateTable(
                "Construction.MonthlyProjectPayment",
                c => new
                    {
                        MonthlyProjectPaymentId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        ApprovedTillPreMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedToBeApprovedThisMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountApprovedThisMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmounNotApprovedThisMonth = c.Decimal(precision: 18, scale: 2),
                        ApprovedToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountNotApprovedToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceAdjustmentTillPreMonth = c.Decimal(precision: 18, scale: 2),
                        PriceAdjustmentThisMonth = c.Decimal(precision: 18, scale: 2),
                        PriceAdjustmentApprovedToDate = c.Decimal(precision: 18, scale: 2),
                        MatOnSiteThisMonth = c.Decimal(precision: 18, scale: 2),
                        MatOnSiteApprovedThisMonth = c.Decimal(precision: 18, scale: 2),
                        MatOnSiteNotAppprovedThiMonth = c.Decimal(precision: 18, scale: 2),
                        ApprovalTime = c.Int(nullable: false),
                        Days = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectPaymentId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.MonthlyProjectPerformanceModifier",
                c => new
                    {
                        MonthlyProjectPerformanceModifierId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        HealthAndSafety = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BudgetRequestAndUtilization = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaterialUtilization = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubContractAdministration = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAllowablePoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectPerformanceModifierId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.MonthlyProjectPlanning",
                c => new
                    {
                        MonthlyProjectPlanningId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        RevScheduleSubAcToManual = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RevScheduleSubAcToContract = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Completness = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QualityAndAcceptablity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        TotalPoints = c.Decimal(precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectPlanningId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.ProjectPnanningCompleteness",
                c => new
                    {
                        ProjectPnanningCompletenessId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPlanningId = c.Int(nullable: false),
                        ScheduleType = c.Int(nullable: false),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectPnanningCompletenessId)
                .ForeignKey("Construction.MonthlyProjectPlanning", t => t.MonthlyProjectPlanningId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPlanningId);
            
            CreateTable(
                "Construction.ProjectPlanningQualityAndAcceptance",
                c => new
                    {
                        ProjectPlanningQualityAndAcceptanceId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPlanningId = c.Int(nullable: false),
                        IsApprovalMadeInTheMonth = c.Boolean(nullable: false),
                        ApprovalType = c.Int(nullable: false),
                        points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectPlanningQualityAndAcceptanceId)
                .ForeignKey("Construction.MonthlyProjectPlanning", t => t.MonthlyProjectPlanningId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPlanningId);
            
            CreateTable(
                "Construction.RScheduleSubmissionAcToContract",
                c => new
                    {
                        RScheduleSubmissionAcToContractId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPlanningId = c.Int(nullable: false),
                        IsRevisionTime = c.Boolean(nullable: false),
                        SubmissionTime = c.Int(nullable: false),
                        Days = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RScheduleSubmissionAcToContractId)
                .ForeignKey("Construction.MonthlyProjectPlanning", t => t.MonthlyProjectPlanningId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPlanningId);
            
            CreateTable(
                "Construction.RScheduleSubmissionAcToOperationManual",
                c => new
                    {
                        RScheduleSubmissionAcToOperationManualId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPlanningId = c.Int(nullable: false),
                        IsRevisionTime = c.Boolean(nullable: false),
                        SubmissionTime = c.Int(nullable: false),
                        Days = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RScheduleSubmissionAcToOperationManualId)
                .ForeignKey("Construction.MonthlyProjectPlanning", t => t.MonthlyProjectPlanningId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPlanningId);
            
            CreateTable(
                "Construction.MonthlyProjectQuality",
                c => new
                    {
                        MonthlyProjectQualityId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        QualityPlanning = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QualityAssurance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QualityControl = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QualityComplaint = c.Int(nullable: false),
                        MaxAllowablePoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectQualityId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.MonthlyProjectReport",
                c => new
                    {
                        MonthlyProjectReportId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        ReportingTime = c.Int(nullable: false),
                        Days = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Completeness = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Narrative = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxAllowablePoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPoints = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectReportId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.MonthlyProjectReportCompletnessComponent",
                c => new
                    {
                        MonthlyProjectReportCompletnessComponentId = c.Int(nullable: false, identity: true),
                        MonthlyProjectReportId = c.Int(nullable: false),
                        Attribute = c.String(),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectReportCompletnessComponentId)
                .ForeignKey("Construction.MonthlyProjectReport", t => t.MonthlyProjectReportId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectReportId);
            
            CreateTable(
                "Construction.MonthlyProjectReportNarrative",
                c => new
                    {
                        MonthlyProjectReportNarrativeId = c.Int(nullable: false, identity: true),
                        MonthlyProjectReportId = c.Int(nullable: false),
                        Attribute = c.String(),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectReportNarrativeId)
                .ForeignKey("Construction.MonthlyProjectReport", t => t.MonthlyProjectReportId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectReportId);
            
            CreateTable(
                "Construction.PerformanceModifierBudgetRequest",
                c => new
                    {
                        PerformanceModifierBudgetRequestId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPerformanceModifierId = c.Int(nullable: false),
                        ForecastedBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RequestedBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Deviation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceModifierBudgetRequestId)
                .ForeignKey("Construction.MonthlyProjectPerformanceModifier", t => t.MonthlyProjectPerformanceModifierId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPerformanceModifierId);
            
            CreateTable(
                "Construction.PerformanceModifierBudgetUtilization",
                c => new
                    {
                        PerformanceModifierBudgetUtilizationId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPerformanceModifierId = c.Int(nullable: false),
                        RequestedBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UtilizedBudget = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Deviation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceModifierBudgetUtilizationId)
                .ForeignKey("Construction.MonthlyProjectPerformanceModifier", t => t.MonthlyProjectPerformanceModifierId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPerformanceModifierId);
            
            CreateTable(
                "Construction.PerformanceModifierHealthAndSafety",
                c => new
                    {
                        PerformanceModifierHealthAndSafetyId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPerformanceModifierId = c.Int(nullable: false),
                        InjuryLevel = c.Int(nullable: false),
                        InjuryCount = c.Int(nullable: false),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceModifierHealthAndSafetyId)
                .ForeignKey("Construction.MonthlyProjectPerformanceModifier", t => t.MonthlyProjectPerformanceModifierId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPerformanceModifierId);
            
            CreateTable(
                "Construction.PerformanceModifierMaterialUtilization",
                c => new
                    {
                        PerformanceModifierMaterialUtilizationId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPerformanceModifierId = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 128),
                        ItemCategoryCode = c.String(),
                        UtilizedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceModifierMaterialUtilizationId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("Construction.MonthlyProjectPerformanceModifier", t => t.MonthlyProjectPerformanceModifierId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPerformanceModifierId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "Construction.PerformanceModifierSubContractAdministration",
                c => new
                    {
                        PerformanceModifierSubContractAdministrationId = c.Int(nullable: false, identity: true),
                        MonthlyProjectPerformanceModifierId = c.Int(nullable: false),
                        Description = c.String(),
                        MaxAllowablePoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PerformanceModifierSubContractAdministrationId)
                .ForeignKey("Construction.MonthlyProjectPerformanceModifier", t => t.MonthlyProjectPerformanceModifierId, cascadeDelete: true)
                .Index(t => t.MonthlyProjectPerformanceModifierId);
            
            AddColumn("Construction.MonthlyProjectQualityComplaints", "MonthlyProjectQualityId", c => c.Int(nullable: false));
            AddColumn("Construction.QualityAssurance", "MonthlyProjectQualityId", c => c.Int(nullable: false));
            AddColumn("Construction.QualityControl", "MonthlyProjectQualityId", c => c.Int(nullable: false));
            AddColumn("Construction.QualityPlanning", "MonthlyProjectQualityId", c => c.Int(nullable: false));
            AlterColumn("Construction.MonthlyProjectQualityComplaints", "NumOfComplaints", c => c.Int(nullable: false));
            AlterColumn("Construction.QualityAssurance", "PeriodMapping_PeriodMappingId", c => c.Int());
            AlterColumn("Construction.QualityControl", "PeriodMapping_PeriodMappingId", c => c.Int());
            AlterColumn("Construction.QualityPlanning", "PeriodMapping_PeriodMappingId", c => c.Int());
            CreateIndex("Construction.MonthlyProjectQualityComplaints", "MonthlyProjectQualityId");
            CreateIndex("Construction.QualityAssurance", "MonthlyProjectQualityId");
            CreateIndex("Construction.QualityAssurance", "PeriodMapping_PeriodMappingId");
            CreateIndex("Construction.QualityControl", "MonthlyProjectQualityId");
            CreateIndex("Construction.QualityControl", "PeriodMapping_PeriodMappingId");
            CreateIndex("Construction.QualityPlanning", "MonthlyProjectQualityId");
            CreateIndex("Construction.QualityPlanning", "PeriodMapping_PeriodMappingId");
            AddForeignKey("Construction.MonthlyProjectQualityComplaints", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality", "MonthlyProjectQualityId", cascadeDelete: true);
            AddForeignKey("Construction.QualityAssurance", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality", "MonthlyProjectQualityId", cascadeDelete: true);
            AddForeignKey("Construction.QualityControl", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality", "MonthlyProjectQualityId", cascadeDelete: true);
            AddForeignKey("Construction.QualityPlanning", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality", "MonthlyProjectQualityId", cascadeDelete: true);
            AddForeignKey("Construction.QualityAssurance", "PeriodMapping_PeriodMappingId", "Construction.PeriodMapping", "PeriodMappingId");
            AddForeignKey("Construction.QualityControl", "PeriodMapping_PeriodMappingId", "Construction.PeriodMapping", "PeriodMappingId");
            AddForeignKey("Construction.QualityPlanning", "PeriodMapping_PeriodMappingId", "Construction.PeriodMapping", "PeriodMappingId");
            DropColumn("Construction.MonthlyProjectQualityComplaints", "PeriodMappingId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.MonthlyProjectQualityComplaints", "PeriodMappingId", c => c.Int(nullable: false));
            DropForeignKey("Construction.QualityPlanning", "PeriodMapping_PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityControl", "PeriodMapping_PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityAssurance", "PeriodMapping_PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityPlanning", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality");
            DropForeignKey("Construction.QualityControl", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality");
            DropForeignKey("Construction.QualityAssurance", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality");
            DropForeignKey("Construction.PerformanceModifierSubContractAdministration", "MonthlyProjectPerformanceModifierId", "Construction.MonthlyProjectPerformanceModifier");
            DropForeignKey("Construction.PerformanceModifierMaterialUtilization", "MonthlyProjectPerformanceModifierId", "Construction.MonthlyProjectPerformanceModifier");
            DropForeignKey("Construction.PerformanceModifierMaterialUtilization", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.PerformanceModifierHealthAndSafety", "MonthlyProjectPerformanceModifierId", "Construction.MonthlyProjectPerformanceModifier");
            DropForeignKey("Construction.PerformanceModifierBudgetUtilization", "MonthlyProjectPerformanceModifierId", "Construction.MonthlyProjectPerformanceModifier");
            DropForeignKey("Construction.PerformanceModifierBudgetRequest", "MonthlyProjectPerformanceModifierId", "Construction.MonthlyProjectPerformanceModifier");
            DropForeignKey("Construction.MonthlyProjectReport", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectReportNarrative", "MonthlyProjectReportId", "Construction.MonthlyProjectReport");
            DropForeignKey("Construction.MonthlyProjectReportCompletnessComponent", "MonthlyProjectReportId", "Construction.MonthlyProjectReport");
            DropForeignKey("Construction.MonthlyProjectQualityComplaints", "MonthlyProjectQualityId", "Construction.MonthlyProjectQuality");
            DropForeignKey("Construction.MonthlyProjectQuality", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.RScheduleSubmissionAcToOperationManual", "MonthlyProjectPlanningId", "Construction.MonthlyProjectPlanning");
            DropForeignKey("Construction.RScheduleSubmissionAcToContract", "MonthlyProjectPlanningId", "Construction.MonthlyProjectPlanning");
            DropForeignKey("Construction.ProjectPlanningQualityAndAcceptance", "MonthlyProjectPlanningId", "Construction.MonthlyProjectPlanning");
            DropForeignKey("Construction.ProjectPnanningCompleteness", "MonthlyProjectPlanningId", "Construction.MonthlyProjectPlanning");
            DropForeignKey("Construction.MonthlyProjectPlanning", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectPerformanceModifier", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectPayment", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectCommunication", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectCommunicationWithConsultant", "MonthlyProjectCommunicationId", "Construction.MonthlyProjectCommunication");
            DropForeignKey("Construction.MonthlyProjectCommunicationWithinProject", "MonthlyProjectCommunicationId", "Construction.MonthlyProjectCommunication");
            DropForeignKey("Construction.MonthlyProjectCommunicationWithHeadOffice", "MonthlyProjectCommunicationId", "Construction.MonthlyProjectCommunication");
            DropForeignKey("Construction.RequirementForMonthlyClaims", "MonthlyProjectClaimId", "Construction.MonthlyProjectClaim");
            DropForeignKey("Construction.MonthlyProjectClaim", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.ContempraryRecord", "MonthlyProjectClaimId", "Construction.MonthlyProjectClaim");
            DropForeignKey("Construction.ApprovalStatusForClaimsResponded", "MonthlyProjectClaimId", "Construction.MonthlyProjectClaim");
            DropForeignKey("Construction.AnalysisTechniqueApplicationForMonthlyClaims", "MonthlyProjectClaimId", "Construction.MonthlyProjectClaim");
            DropIndex("Construction.QualityPlanning", new[] { "PeriodMapping_PeriodMappingId" });
            DropIndex("Construction.QualityPlanning", new[] { "MonthlyProjectQualityId" });
            DropIndex("Construction.QualityControl", new[] { "PeriodMapping_PeriodMappingId" });
            DropIndex("Construction.QualityControl", new[] { "MonthlyProjectQualityId" });
            DropIndex("Construction.QualityAssurance", new[] { "PeriodMapping_PeriodMappingId" });
            DropIndex("Construction.QualityAssurance", new[] { "MonthlyProjectQualityId" });
            DropIndex("Construction.PerformanceModifierSubContractAdministration", new[] { "MonthlyProjectPerformanceModifierId" });
            DropIndex("Construction.PerformanceModifierMaterialUtilization", new[] { "ItemCode" });
            DropIndex("Construction.PerformanceModifierMaterialUtilization", new[] { "MonthlyProjectPerformanceModifierId" });
            DropIndex("Construction.PerformanceModifierHealthAndSafety", new[] { "MonthlyProjectPerformanceModifierId" });
            DropIndex("Construction.PerformanceModifierBudgetUtilization", new[] { "MonthlyProjectPerformanceModifierId" });
            DropIndex("Construction.PerformanceModifierBudgetRequest", new[] { "MonthlyProjectPerformanceModifierId" });
            DropIndex("Construction.MonthlyProjectReportNarrative", new[] { "MonthlyProjectReportId" });
            DropIndex("Construction.MonthlyProjectReportCompletnessComponent", new[] { "MonthlyProjectReportId" });
            DropIndex("Construction.MonthlyProjectReport", new[] { "PeriodMappingId" });
            DropIndex("Construction.MonthlyProjectQualityComplaints", new[] { "MonthlyProjectQualityId" });
            DropIndex("Construction.MonthlyProjectQuality", new[] { "PeriodMappingId" });
            DropIndex("Construction.RScheduleSubmissionAcToOperationManual", new[] { "MonthlyProjectPlanningId" });
            DropIndex("Construction.RScheduleSubmissionAcToContract", new[] { "MonthlyProjectPlanningId" });
            DropIndex("Construction.ProjectPlanningQualityAndAcceptance", new[] { "MonthlyProjectPlanningId" });
            DropIndex("Construction.ProjectPnanningCompleteness", new[] { "MonthlyProjectPlanningId" });
            DropIndex("Construction.MonthlyProjectPlanning", new[] { "PeriodMappingId" });
            DropIndex("Construction.MonthlyProjectPerformanceModifier", new[] { "PeriodMappingId" });
            DropIndex("Construction.MonthlyProjectPayment", new[] { "PeriodMappingId" });
            DropIndex("Construction.MonthlyProjectCommunicationWithConsultant", new[] { "MonthlyProjectCommunicationId" });
            DropIndex("Construction.MonthlyProjectCommunicationWithinProject", new[] { "MonthlyProjectCommunicationId" });
            DropIndex("Construction.MonthlyProjectCommunicationWithHeadOffice", new[] { "MonthlyProjectCommunicationId" });
            DropIndex("Construction.MonthlyProjectCommunication", new[] { "PeriodMappingId" });
            DropIndex("Construction.RequirementForMonthlyClaims", new[] { "MonthlyProjectClaimId" });
            DropIndex("Construction.ContempraryRecord", new[] { "MonthlyProjectClaimId" });
            DropIndex("Construction.ApprovalStatusForClaimsResponded", new[] { "MonthlyProjectClaimId" });
            DropIndex("Construction.MonthlyProjectClaim", new[] { "PeriodMappingId" });
            DropIndex("Construction.AnalysisTechniqueApplicationForMonthlyClaims", new[] { "MonthlyProjectClaimId" });
            AlterColumn("Construction.QualityPlanning", "PeriodMapping_PeriodMappingId", c => c.Int(nullable: false));
            AlterColumn("Construction.QualityControl", "PeriodMapping_PeriodMappingId", c => c.Int(nullable: false));
            AlterColumn("Construction.QualityAssurance", "PeriodMapping_PeriodMappingId", c => c.Int(nullable: false));
            AlterColumn("Construction.MonthlyProjectQualityComplaints", "NumOfComplaints", c => c.Int());
            DropColumn("Construction.QualityPlanning", "MonthlyProjectQualityId");
            DropColumn("Construction.QualityControl", "MonthlyProjectQualityId");
            DropColumn("Construction.QualityAssurance", "MonthlyProjectQualityId");
            DropColumn("Construction.MonthlyProjectQualityComplaints", "MonthlyProjectQualityId");
            DropTable("Construction.PerformanceModifierSubContractAdministration");
            DropTable("Construction.PerformanceModifierMaterialUtilization");
            DropTable("Construction.PerformanceModifierHealthAndSafety");
            DropTable("Construction.PerformanceModifierBudgetUtilization");
            DropTable("Construction.PerformanceModifierBudgetRequest");
            DropTable("Construction.MonthlyProjectReportNarrative");
            DropTable("Construction.MonthlyProjectReportCompletnessComponent");
            DropTable("Construction.MonthlyProjectReport");
            DropTable("Construction.MonthlyProjectQuality");
            DropTable("Construction.RScheduleSubmissionAcToOperationManual");
            DropTable("Construction.RScheduleSubmissionAcToContract");
            DropTable("Construction.ProjectPlanningQualityAndAcceptance");
            DropTable("Construction.ProjectPnanningCompleteness");
            DropTable("Construction.MonthlyProjectPlanning");
            DropTable("Construction.MonthlyProjectPerformanceModifier");
            DropTable("Construction.MonthlyProjectPayment");
            DropTable("Construction.MonthlyProjectCommunicationWithConsultant");
            DropTable("Construction.MonthlyProjectCommunicationWithinProject");
            DropTable("Construction.MonthlyProjectCommunicationWithHeadOffice");
            DropTable("Construction.MonthlyProjectCommunication");
            DropTable("Construction.RequirementForMonthlyClaims");
            DropTable("Construction.ContempraryRecord");
            DropTable("Construction.ApprovalStatusForClaimsResponded");
            DropTable("Construction.MonthlyProjectClaim");
            DropTable("Construction.AnalysisTechniqueApplicationForMonthlyClaims");
            RenameColumn(table: "Construction.QualityPlanning", name: "PeriodMapping_PeriodMappingId", newName: "PeriodMappingId");
            RenameColumn(table: "Construction.QualityControl", name: "PeriodMapping_PeriodMappingId", newName: "PeriodMappingId");
            RenameColumn(table: "Construction.QualityAssurance", name: "PeriodMapping_PeriodMappingId", newName: "PeriodMappingId");
            CreateIndex("Construction.QualityPlanning", "PeriodMappingId");
            CreateIndex("Construction.QualityControl", "PeriodMappingId");
            CreateIndex("Construction.QualityAssurance", "PeriodMappingId");
            CreateIndex("Construction.MonthlyProjectQualityComplaints", "PeriodMappingId");
            AddForeignKey("Construction.QualityPlanning", "PeriodMappingId", "Construction.PeriodMapping", "PeriodMappingId", cascadeDelete: true);
            AddForeignKey("Construction.QualityControl", "PeriodMappingId", "Construction.PeriodMapping", "PeriodMappingId", cascadeDelete: true);
            AddForeignKey("Construction.QualityAssurance", "PeriodMappingId", "Construction.PeriodMapping", "PeriodMappingId", cascadeDelete: true);
            AddForeignKey("Construction.MonthlyProjectQualityComplaints", "PeriodMappingId", "Construction.PeriodMapping", "PeriodMappingId", cascadeDelete: true);
        }
    }
}
