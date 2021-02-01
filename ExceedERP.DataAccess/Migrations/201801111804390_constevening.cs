namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constevening : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.WorkDelay",
                c => new
                    {
                        WorkDelayId = c.Int(nullable: false, identity: true),
                        DelayAnalysisTaskId = c.Int(nullable: false),
                        PlannedDuration = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        Description = c.String(),
                        DelayTypeId = c.Int(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Duration = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkDelayId)
                .ForeignKey("Construction.DelayTypes", t => t.DelayTypeId, cascadeDelete: true)
                .ForeignKey("Construction.DelayAnalysisTask", t => t.DelayAnalysisTaskId, cascadeDelete: true)
                .Index(t => t.DelayAnalysisTaskId)
                .Index(t => t.DelayTypeId);
            
            AddColumn("Construction.ProjectEquipment", "FleetsID", c => c.Guid(nullable: false));
            AddColumn("Construction.MonthlyProjectClaim", "DailylyRecords", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectClaim", "IsClaimPreparedAndIssuedInTheMonth", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectClaim", "SubstantiveRequirementRefClauses", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectClaim", "ProceduralRequirementDocumentation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectClaim", "ProofRequirementDocumentation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectClaim", "WasThereissuetoClaim", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectClaim", "AnalysisType", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectClaim", "IsApprovalMadInTheMonth", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectClaim", "ClaimApprovalStatus", c => c.Int(nullable: false));
            AddColumn("Construction.DelayAnalysisTask", "HasDelay", c => c.Boolean(nullable: false));
            AddColumn("Construction.DelayTypes", "Type", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectCommunication", "CloseDiscussionWithinProject", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCommunication", "MobilizationAndDisscusstionWthinWorkers", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCommunication", "WrittenComn", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCommunication", "TelephoneComn", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCommunication", "EmailComn", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCommunication", "ComnLevel", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "IsRevisionTimeAccManual", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "RevisedSdlSubmitedAccordinglyAccToManual", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "SubmssinTimeAcctoManual", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "DaysAfterOrBeforeDueDateAccToManual", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "IsRevisionTimeAccContract", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "RevisedSdlSubmitedAccordinglyAccToContract", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "SubmssinTimeAcctoContract", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "DaysAfterOrBeforeDueDateAccToContract", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "WorkSchedule", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPlanning", "ResourceSchedule", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPlanning", "CashFlowSchedule", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPlanning", "BudgetRequirementSchedule", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPlanning", "IsApprovalMadeInTheMonth", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectPlanning", "ApprovalType", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectQuality", "DoPersonnelKnowTheQualityRequirements", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectQuality", "DoPersonnelKnowHowToMeetQualityRequirements", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectQuality", "MaterialTestsPerformed", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectQuality", "EvaluationOfMethod", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectQuality", "OverAllEvaluationOfLabour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectQuality", "NumberOfTestsPerformedOnExecutedWorks", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectQuality", "ExtentOfInspectionPerformedOnExecutedWorks", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectQuality", "FromClient", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectQuality", "FromEngineers", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectQuality", "FromHeadOffice", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectQuality", "FromQualityEngineer", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectReport", "Progress", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "QualityControl", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "HealthAndSafety", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "EarnedValueAnalysis", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "MaterialUtilization", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "LabourUtilixation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "EquipmentUtulization", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "FuelUtilization", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectReport", "ProperIdentificationOfProblems", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectEstmnEquipment", "FleetsID", c => c.Guid(nullable: false));
            AddColumn("Construction.ResourceUtilizationEquipment", "FleetsID", c => c.Guid(nullable: false));
            AlterColumn("Construction.DelayAnalysisTask", "DelayDuration", c => c.String());
            CreateIndex("Construction.ProjectEquipment", "FleetsID");
            CreateIndex("Construction.ProjectEstmnEquipment", "FleetsID");
            AddForeignKey("Construction.ProjectEquipment", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnEquipment", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            DropColumn("Construction.ProjectEquipment", "AssetNumber");
            DropColumn("Construction.ProjectEquipment", "EquipmentTypeID");
            DropColumn("Construction.ProjectEquipment", "EquipmentIdentityID");
            DropColumn("Construction.ProjectEstmnEquipment", "EquipmentTypeID");
            DropColumn("Construction.ProjectEstmnEquipment", "EquipmentIdentityID");
            DropColumn("Construction.ProjectEstmnEquipment", "AssetNumber");
            DropColumn("Construction.ResourceUtilizationEquipment", "AssetNumber");
            DropColumn("Construction.ResourceUtilizationEquipment", "EquipmentTypeID");
            DropColumn("Construction.ResourceUtilizationEquipment", "EquipmentIdentityID");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ResourceUtilizationEquipment", "EquipmentIdentityID", c => c.Guid(nullable: false));
            AddColumn("Construction.ResourceUtilizationEquipment", "EquipmentTypeID", c => c.Guid(nullable: false));
            AddColumn("Construction.ResourceUtilizationEquipment", "AssetNumber", c => c.Guid(nullable: false));
            AddColumn("Construction.ProjectEstmnEquipment", "AssetNumber", c => c.Guid(nullable: false));
            AddColumn("Construction.ProjectEstmnEquipment", "EquipmentIdentityID", c => c.Guid(nullable: false));
            AddColumn("Construction.ProjectEstmnEquipment", "EquipmentTypeID", c => c.Guid(nullable: false));
            AddColumn("Construction.ProjectEquipment", "EquipmentIdentityID", c => c.Guid(nullable: false));
            AddColumn("Construction.ProjectEquipment", "EquipmentTypeID", c => c.Guid(nullable: false));
            AddColumn("Construction.ProjectEquipment", "AssetNumber", c => c.Guid(nullable: false));
            DropForeignKey("Construction.WorkDelay", "DelayAnalysisTaskId", "Construction.DelayAnalysisTask");
            DropForeignKey("Construction.WorkDelay", "DelayTypeId", "Construction.DelayTypes");
            DropForeignKey("Construction.ProjectEstmnEquipment", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("Construction.ProjectEquipment", "FleetsID", "FleetManagement.Fleets");
            DropIndex("Construction.WorkDelay", new[] { "DelayTypeId" });
            DropIndex("Construction.WorkDelay", new[] { "DelayAnalysisTaskId" });
            DropIndex("Construction.ProjectEstmnEquipment", new[] { "FleetsID" });
            DropIndex("Construction.ProjectEquipment", new[] { "FleetsID" });
            AlterColumn("Construction.DelayAnalysisTask", "DelayDuration", c => c.Int(nullable: false));
            DropColumn("Construction.ResourceUtilizationEquipment", "FleetsID");
            DropColumn("Construction.ProjectEstmnEquipment", "FleetsID");
            DropColumn("Construction.MonthlyProjectReport", "ProperIdentificationOfProblems");
            DropColumn("Construction.MonthlyProjectReport", "FuelUtilization");
            DropColumn("Construction.MonthlyProjectReport", "EquipmentUtulization");
            DropColumn("Construction.MonthlyProjectReport", "LabourUtilixation");
            DropColumn("Construction.MonthlyProjectReport", "MaterialUtilization");
            DropColumn("Construction.MonthlyProjectReport", "EarnedValueAnalysis");
            DropColumn("Construction.MonthlyProjectReport", "HealthAndSafety");
            DropColumn("Construction.MonthlyProjectReport", "QualityControl");
            DropColumn("Construction.MonthlyProjectReport", "Progress");
            DropColumn("Construction.MonthlyProjectQuality", "FromQualityEngineer");
            DropColumn("Construction.MonthlyProjectQuality", "FromHeadOffice");
            DropColumn("Construction.MonthlyProjectQuality", "FromEngineers");
            DropColumn("Construction.MonthlyProjectQuality", "FromClient");
            DropColumn("Construction.MonthlyProjectQuality", "ExtentOfInspectionPerformedOnExecutedWorks");
            DropColumn("Construction.MonthlyProjectQuality", "NumberOfTestsPerformedOnExecutedWorks");
            DropColumn("Construction.MonthlyProjectQuality", "OverAllEvaluationOfLabour");
            DropColumn("Construction.MonthlyProjectQuality", "EvaluationOfMethod");
            DropColumn("Construction.MonthlyProjectQuality", "MaterialTestsPerformed");
            DropColumn("Construction.MonthlyProjectQuality", "DoPersonnelKnowHowToMeetQualityRequirements");
            DropColumn("Construction.MonthlyProjectQuality", "DoPersonnelKnowTheQualityRequirements");
            DropColumn("Construction.MonthlyProjectPlanning", "ApprovalType");
            DropColumn("Construction.MonthlyProjectPlanning", "IsApprovalMadeInTheMonth");
            DropColumn("Construction.MonthlyProjectPlanning", "BudgetRequirementSchedule");
            DropColumn("Construction.MonthlyProjectPlanning", "CashFlowSchedule");
            DropColumn("Construction.MonthlyProjectPlanning", "ResourceSchedule");
            DropColumn("Construction.MonthlyProjectPlanning", "WorkSchedule");
            DropColumn("Construction.MonthlyProjectPlanning", "DaysAfterOrBeforeDueDateAccToContract");
            DropColumn("Construction.MonthlyProjectPlanning", "SubmssinTimeAcctoContract");
            DropColumn("Construction.MonthlyProjectPlanning", "RevisedSdlSubmitedAccordinglyAccToContract");
            DropColumn("Construction.MonthlyProjectPlanning", "IsRevisionTimeAccContract");
            DropColumn("Construction.MonthlyProjectPlanning", "DaysAfterOrBeforeDueDateAccToManual");
            DropColumn("Construction.MonthlyProjectPlanning", "SubmssinTimeAcctoManual");
            DropColumn("Construction.MonthlyProjectPlanning", "RevisedSdlSubmitedAccordinglyAccToManual");
            DropColumn("Construction.MonthlyProjectPlanning", "IsRevisionTimeAccManual");
            DropColumn("Construction.MonthlyProjectCommunication", "ComnLevel");
            DropColumn("Construction.MonthlyProjectCommunication", "EmailComn");
            DropColumn("Construction.MonthlyProjectCommunication", "TelephoneComn");
            DropColumn("Construction.MonthlyProjectCommunication", "WrittenComn");
            DropColumn("Construction.MonthlyProjectCommunication", "MobilizationAndDisscusstionWthinWorkers");
            DropColumn("Construction.MonthlyProjectCommunication", "CloseDiscussionWithinProject");
            DropColumn("Construction.DelayTypes", "Type");
            DropColumn("Construction.DelayAnalysisTask", "HasDelay");
            DropColumn("Construction.MonthlyProjectClaim", "ClaimApprovalStatus");
            DropColumn("Construction.MonthlyProjectClaim", "IsApprovalMadInTheMonth");
            DropColumn("Construction.MonthlyProjectClaim", "AnalysisType");
            DropColumn("Construction.MonthlyProjectClaim", "WasThereissuetoClaim");
            DropColumn("Construction.MonthlyProjectClaim", "ProofRequirementDocumentation");
            DropColumn("Construction.MonthlyProjectClaim", "ProceduralRequirementDocumentation");
            DropColumn("Construction.MonthlyProjectClaim", "SubstantiveRequirementRefClauses");
            DropColumn("Construction.MonthlyProjectClaim", "IsClaimPreparedAndIssuedInTheMonth");
            DropColumn("Construction.MonthlyProjectClaim", "DailylyRecords");
            DropColumn("Construction.ProjectEquipment", "FleetsID");
            DropTable("Construction.WorkDelay");
        }
    }
}
