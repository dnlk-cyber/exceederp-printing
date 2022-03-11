namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanAdded : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("ProductFollowUp.ProductionPlans");
        }
    }
}
