namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductionPlanAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProductFollowUp.HalfYearlyProductionPlans",
                c => new
                    {
                        HalfYearlyProductionPlanId = c.Int(nullable: false, identity: true),
                        YearlyPlanId = c.Int(nullable: false),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YearlyPlan_YearlyProductionPlanId = c.Int(),
                    })
                .PrimaryKey(t => t.HalfYearlyProductionPlanId)
                .ForeignKey("ProductFollowUp.YearlyProductionPlans", t => t.YearlyPlan_YearlyProductionPlanId)
                .Index(t => t.YearlyPlan_YearlyProductionPlanId);
            
            CreateTable(
                "ProductFollowUp.QuarterlyProductionPlans",
                c => new
                    {
                        QuarterlyProductionPlanId = c.Int(nullable: false, identity: true),
                        HalfYearlyPlanId = c.Int(nullable: false),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HalfYearlyProductionPlan_HalfYearlyProductionPlanId = c.Int(),
                    })
                .PrimaryKey(t => t.QuarterlyProductionPlanId)
                .ForeignKey("ProductFollowUp.HalfYearlyProductionPlans", t => t.HalfYearlyProductionPlan_HalfYearlyProductionPlanId)
                .Index(t => t.HalfYearlyProductionPlan_HalfYearlyProductionPlanId);
            
            CreateTable(
                "ProductFollowUp.MonthlyProductionPlans",
                c => new
                    {
                        MonthlyProductionPlanId = c.Int(nullable: false, identity: true),
                        QuarterlyPlanId = c.Int(nullable: false),
                        WorkingDays = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkingHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuarterlyProductionPlan_QuarterlyProductionPlanId = c.Int(),
                    })
                .PrimaryKey(t => t.MonthlyProductionPlanId)
                .ForeignKey("ProductFollowUp.QuarterlyProductionPlans", t => t.QuarterlyProductionPlan_QuarterlyProductionPlanId)
                .Index(t => t.QuarterlyProductionPlan_QuarterlyProductionPlanId);
            
            CreateTable(
                "ProductFollowUp.WeeklyProductionPlans",
                c => new
                    {
                        WeeklyProductionPlanId = c.Int(nullable: false, identity: true),
                        MonthlyPlanId = c.Int(nullable: false),
                        NameAm = c.String(),
                        Name = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MonthlyPlan_MonthlyProductionPlanId = c.Int(),
                    })
                .PrimaryKey(t => t.WeeklyProductionPlanId)
                .ForeignKey("ProductFollowUp.MonthlyProductionPlans", t => t.MonthlyPlan_MonthlyProductionPlanId)
                .Index(t => t.MonthlyPlan_MonthlyProductionPlanId);
            
            CreateTable(
                "ProductFollowUp.YearlyProductionPlans",
                c => new
                    {
                        YearlyProductionPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        StartDateEth = c.String(),
                        EndDateEth = c.String(),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.YearlyProductionPlanId)
                .ForeignKey("EXCEED.GlFiscalYear", t => t.GlFiscalYearId, cascadeDelete: true)
                .Index(t => t.GlFiscalYearId);
            
            CreateTable(
                "ProductFollowUp.ProductionJobTypePlans",
                c => new
                    {
                        ProductionJobTypePlanId = c.Int(nullable: false, identity: true),
                        WeeklyProductionPlanId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        ProductionPlanQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPlanBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductionJobTypePlanId);
            
        
        }
        
        public override void Down()
        {
            DropForeignKey("ProductFollowUp.JobStatusLineItems", "JobStatusId", "ProductFollowUp.JobStatusEntityTypeConfiguration");
            DropForeignKey("ProductFollowUp.HalfYearlyProductionPlans", "YearlyPlan_YearlyProductionPlanId", "ProductFollowUp.YearlyProductionPlans");
            DropForeignKey("ProductFollowUp.YearlyProductionPlans", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("ProductFollowUp.WeeklyProductionPlans", "MonthlyPlan_MonthlyProductionPlanId", "ProductFollowUp.MonthlyProductionPlans");
            DropForeignKey("ProductFollowUp.MonthlyProductionPlans", "QuarterlyProductionPlan_QuarterlyProductionPlanId", "ProductFollowUp.QuarterlyProductionPlans");
            DropForeignKey("ProductFollowUp.QuarterlyProductionPlans", "HalfYearlyProductionPlan_HalfYearlyProductionPlanId", "ProductFollowUp.HalfYearlyProductionPlans");
            DropIndex("ProductFollowUp.JobStatusLineItems", new[] { "JobStatusId" });
            DropIndex("ProductFollowUp.YearlyProductionPlans", new[] { "GlFiscalYearId" });
            DropIndex("ProductFollowUp.WeeklyProductionPlans", new[] { "MonthlyPlan_MonthlyProductionPlanId" });
            DropIndex("ProductFollowUp.MonthlyProductionPlans", new[] { "QuarterlyProductionPlan_QuarterlyProductionPlanId" });
            DropIndex("ProductFollowUp.QuarterlyProductionPlans", new[] { "HalfYearlyProductionPlan_HalfYearlyProductionPlanId" });
            DropIndex("ProductFollowUp.HalfYearlyProductionPlans", new[] { "YearlyPlan_YearlyProductionPlanId" });
            DropTable("ProductFollowUp.ProductionJobTypePlans");
            DropTable("ProductFollowUp.YearlyProductionPlans");
            DropTable("ProductFollowUp.WeeklyProductionPlans");
            DropTable("ProductFollowUp.MonthlyProductionPlans");
            DropTable("ProductFollowUp.QuarterlyProductionPlans");
            DropTable("ProductFollowUp.HalfYearlyProductionPlans");
        }
    }
}
