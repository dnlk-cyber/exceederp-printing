namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MonthlyAndJobTypePlanAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ProductFollowUp.ProductionMonthlyJobTypePlans",
                c => new
                    {
                        ProductionMonthlyJobTypePlanId = c.Int(nullable: false, identity: true),
                        ProductionMonthlyPlanId = c.Int(nullable: false),
                        JobTypeId = c.Int(nullable: false),
                        TotalProduct = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductionMonthlyJobTypePlanId);
            
            CreateTable(
                "ProductFollowUp.ProductionMonthlyPlans",
                c => new
                    {
                        ProductionMonthlyPlanId = c.Int(nullable: false, identity: true),
                        ProductionYearlyPlanId = c.Int(nullable: false),
                        GLPeriodId = c.Int(nullable: false),
                        TotalProduct = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductionMonthlyPlanId);
            
            CreateTable(
                "ProductFollowUp.ProductionYearlyPlans",
                c => new
                    {
                        ProductionYearlyPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        TotalProduct = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductionYearlyPlanId);
            
            DropTable("ProductFollowUp.ProductionPlans");
        }
        
        public override void Down()
        {
            CreateTable(
                "ProductFollowUp.ProductionPlans",
                c => new
                    {
                        ProductionPlanId = c.Int(nullable: false, identity: true),
                        GlFiscalYearId = c.Int(nullable: false),
                        TotalProduct = c.Decimal(precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ProductionPlanId);
            
            DropTable("ProductFollowUp.ProductionYearlyPlans");
            DropTable("ProductFollowUp.ProductionMonthlyPlans");
            DropTable("ProductFollowUp.ProductionMonthlyJobTypePlans");
        }
    }
}
