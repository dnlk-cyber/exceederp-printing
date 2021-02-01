namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construceven : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.EarnedValuePerItem",
                c => new
                    {
                        EarnedValuePerItemId = c.Int(nullable: false, identity: true),
                        WorkItemPerProjectId = c.Int(nullable: false),
                        Unit = c.String(),
                        BudgetedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PlannedQuanity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExecutedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BCWS = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BCWP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ACWP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PCP = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PCA = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CV = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CVPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SV = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SVPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TCPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TSPI = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ETC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ISAC = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EarnedValuePerItemId)
                .ForeignKey("Construction.WorkItemPerProject", t => t.WorkItemPerProjectId, cascadeDelete: true)
                .Index(t => t.WorkItemPerProjectId);
            
            AddColumn("Construction.ConstructionItemCategory", "IsFuel", c => c.Boolean(nullable: false));
            AddColumn("Construction.ConstructionItemCategory", "IsReinforcementBar", c => c.Boolean(nullable: false));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "HeavyInjuriesCount", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "LightInjuriesCount", c => c.Int(nullable: false));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ForcastedBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "RequestedBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "RequestDeviation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "PointFromReqDeviation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "UtilizedBudget", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "UtilizationDeviation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "PointFromUtlDeviation", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "Cement", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ReadyMixConcrete", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "ReinforcementBar", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "FormWork", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "Sand", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "Ceramic", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "SiteAccess", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "DelayedPayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "LackOfGoodGovernance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectPerformanceModifier", "InactionForSubContractor", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.EarnedValuePerItem", "WorkItemPerProjectId", "Construction.WorkItemPerProject");
            DropIndex("Construction.EarnedValuePerItem", new[] { "WorkItemPerProjectId" });
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "InactionForSubContractor");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "LackOfGoodGovernance");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "DelayedPayment");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "SiteAccess");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "Ceramic");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "Sand");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "FormWork");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ReinforcementBar");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ReadyMixConcrete");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "Cement");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "PointFromUtlDeviation");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "UtilizationDeviation");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "UtilizedBudget");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "PointFromReqDeviation");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "RequestDeviation");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "RequestedBudget");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "ForcastedBudget");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "LightInjuriesCount");
            DropColumn("Construction.MonthlyProjectPerformanceModifier", "HeavyInjuriesCount");
            DropColumn("Construction.ConstructionItemCategory", "IsReinforcementBar");
            DropColumn("Construction.ConstructionItemCategory", "IsFuel");
            DropTable("Construction.EarnedValuePerItem");
        }
    }
}
