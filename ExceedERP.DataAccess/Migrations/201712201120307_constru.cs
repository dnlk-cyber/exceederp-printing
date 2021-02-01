namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class constru : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("Construction.ConstructionItemCategory");
            DropPrimaryKey("Construction.SubContractWorkItemEstmn");
            DropPrimaryKey("Construction.EstimationAreaWorkItem");
            CreateTable(
                "Construction.DelayTypes",
                c => new
                    {
                        DelayTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Abbriviation = c.String(),
                    })
                .PrimaryKey(t => t.DelayTypeId);
            
            CreateTable(
                "Construction.MonthlyProjectCost",
                c => new
                    {
                        MonthlyProjectCostId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        PlannedDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedProjectOHCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedProfitAndOHCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualDirectCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualProjectOHCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualProfitAndOHCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedDirectCostToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedProjectOHCostToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedProfitAndOHToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualDirectCostToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualProjectOHCostToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualProfitAndOHToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostTracking = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectCostId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.PeriodMapping",
                c => new
                    {
                        PeriodMappingId = c.Int(nullable: false, identity: true),
                        ConstructionPeriodId = c.Int(nullable: false),
                        ProjectInformationId = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.PeriodMappingId)
                .ForeignKey("Construction.ConstructionPeriod", t => t.ConstructionPeriodId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectInformation", t => t.ProjectInformationId, cascadeDelete: true)
                .Index(t => t.ConstructionPeriodId)
                .Index(t => t.ProjectInformationId);
            
            CreateTable(
                "Construction.MonthlyProjectProgress",
                c => new
                    {
                        MonthlyProjectProgressId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        PlannedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedAmountTillPreviousMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExecutedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExecutedAmountTillPreviousMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExecutedAmountToDate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScheduleBrokenDownToWeeklyBase = c.Boolean(nullable: false),
                        AreFormenAwareOfIt = c.Boolean(nullable: false),
                        NumberOfEvaluationMeething = c.Int(nullable: false),
                        CorrectionFactor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectProgressId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.MonthlyProjectQualityComplaints",
                c => new
                    {
                        MonthlyProjectQualityComplaintId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        Source = c.Int(nullable: false),
                        Remark = c.String(),
                        NumOfComplaints = c.Int(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MonthlyProjectQualityComplaintId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.QualityAssurance",
                c => new
                    {
                        QualityAssuranceId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        Description = c.String(),
                        MaxPoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QualityAssuranceId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.QualityControl",
                c => new
                    {
                        QualityControlId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        Description = c.String(),
                        MaxPoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QualityControlId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            CreateTable(
                "Construction.QualityPlanning",
                c => new
                    {
                        QualityPlanningId = c.Int(nullable: false, identity: true),
                        PeriodMappingId = c.Int(nullable: false),
                        Description = c.String(),
                        MaxPoints = c.Decimal(precision: 18, scale: 2),
                        Points = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.QualityPlanningId)
                .ForeignKey("Construction.PeriodMapping", t => t.PeriodMappingId, cascadeDelete: true)
                .Index(t => t.PeriodMappingId);
            
            AddColumn("Construction.ProjectContract", "RevisedContractAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "RevisedCompletionDate", c => c.DateTime());
            AddColumn("Construction.ProjectContract", "ContractPeriod", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "ApprovedTimeExtension", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "TotalProjectPeriod", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "TimeElapsedToDate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "Rebate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "Variation", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "Supplemetary", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "TotalProjectAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "NetGrand", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ProjectContract", "ExecutedToDate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Construction.ConstructionItemCategory", "ConstructionItemCategoryId", c => c.Int(nullable: false, identity: true));
            AddColumn("Construction.SubContractWorkItemEstmn", "EstimationSubContractWorkItemId", c => c.Int(nullable: false, identity: true));
            AddColumn("Construction.EstimationAreaWorkItem", "ProjectEstimationAreaWorkItemId", c => c.Int(nullable: false, identity: true));
            AlterColumn("EXCEED.InventoryItem", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.InventoryItem", "AvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.StoreItemAssignment", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.StoreItemAssignment", "AvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.StoreItemAssignment", "BegBalance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.StoreItemAssignment", "BegAvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BranchItemAssignment", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BranchItemAssignment", "AvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("Construction.LaborRateIndexFactorAttribute", "NumberOfDays", c => c.Decimal(precision: 18, scale: 2));
            AddPrimaryKey("Construction.ConstructionItemCategory", "ConstructionItemCategoryId");
            AddPrimaryKey("Construction.SubContractWorkItemEstmn", "EstimationSubContractWorkItemId");
            AddPrimaryKey("Construction.EstimationAreaWorkItem", "ProjectEstimationAreaWorkItemId");
            DropColumn("Construction.ConstructionItemCategory", "CategoryId");
            DropColumn("Construction.SubContractWorkItemEstmn", "SubContractWorkItemId");
            DropColumn("Construction.EstimationAreaWorkItem", "AreaWorkItemId");
        }
        
        public override void Down()
        {
            AddColumn("Construction.EstimationAreaWorkItem", "AreaWorkItemId", c => c.Int(nullable: false, identity: true));
            AddColumn("Construction.SubContractWorkItemEstmn", "SubContractWorkItemId", c => c.Int(nullable: false, identity: true));
            AddColumn("Construction.ConstructionItemCategory", "CategoryId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("Construction.QualityPlanning", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityControl", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.QualityAssurance", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectQualityComplaints", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectProgress", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.MonthlyProjectCost", "PeriodMappingId", "Construction.PeriodMapping");
            DropForeignKey("Construction.PeriodMapping", "ProjectInformationId", "Construction.ProjectInformation");
            DropForeignKey("Construction.PeriodMapping", "ConstructionPeriodId", "Construction.ConstructionPeriod");
            DropIndex("Construction.QualityPlanning", new[] { "PeriodMappingId" });
            DropIndex("Construction.QualityControl", new[] { "PeriodMappingId" });
            DropIndex("Construction.QualityAssurance", new[] { "PeriodMappingId" });
            DropIndex("Construction.MonthlyProjectQualityComplaints", new[] { "PeriodMappingId" });
            DropIndex("Construction.MonthlyProjectProgress", new[] { "PeriodMappingId" });
            DropIndex("Construction.PeriodMapping", new[] { "ProjectInformationId" });
            DropIndex("Construction.PeriodMapping", new[] { "ConstructionPeriodId" });
            DropIndex("Construction.MonthlyProjectCost", new[] { "PeriodMappingId" });
            DropPrimaryKey("Construction.EstimationAreaWorkItem");
            DropPrimaryKey("Construction.SubContractWorkItemEstmn");
            DropPrimaryKey("Construction.ConstructionItemCategory");
            AlterColumn("Construction.LaborRateIndexFactorAttribute", "NumberOfDays", c => c.Int());
            AlterColumn("EXCEED.BranchItemAssignment", "AvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BranchItemAssignment", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "BegAvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "BegBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "AvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.StoreItemAssignment", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.InventoryItem", "AvgPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.InventoryItem", "Balance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("Construction.EstimationAreaWorkItem", "ProjectEstimationAreaWorkItemId");
            DropColumn("Construction.SubContractWorkItemEstmn", "EstimationSubContractWorkItemId");
            DropColumn("Construction.ConstructionItemCategory", "ConstructionItemCategoryId");
            DropColumn("Construction.ProjectContract", "ExecutedToDate");
            DropColumn("Construction.ProjectContract", "NetGrand");
            DropColumn("Construction.ProjectContract", "TotalProjectAmount");
            DropColumn("Construction.ProjectContract", "Supplemetary");
            DropColumn("Construction.ProjectContract", "Variation");
            DropColumn("Construction.ProjectContract", "Rebate");
            DropColumn("Construction.ProjectContract", "TimeElapsedToDate");
            DropColumn("Construction.ProjectContract", "TotalProjectPeriod");
            DropColumn("Construction.ProjectContract", "ApprovedTimeExtension");
            DropColumn("Construction.ProjectContract", "ContractPeriod");
            DropColumn("Construction.ProjectContract", "RevisedCompletionDate");
            DropColumn("Construction.ProjectContract", "RevisedContractAmount");
            DropTable("Construction.QualityPlanning");
            DropTable("Construction.QualityControl");
            DropTable("Construction.QualityAssurance");
            DropTable("Construction.MonthlyProjectQualityComplaints");
            DropTable("Construction.MonthlyProjectProgress");
            DropTable("Construction.PeriodMapping");
            DropTable("Construction.MonthlyProjectCost");
            DropTable("Construction.DelayTypes");
            AddPrimaryKey("Construction.EstimationAreaWorkItem", "AreaWorkItemId");
            AddPrimaryKey("Construction.SubContractWorkItemEstmn", "SubContractWorkItemId");
            AddPrimaryKey("Construction.ConstructionItemCategory", "CategoryId");
        }
    }
}
