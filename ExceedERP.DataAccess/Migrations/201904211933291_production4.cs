namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class production4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Production.MachineryMaintenance",
                c => new
                    {
                        MachineryMaintenanceId = c.Int(nullable: false, identity: true),
                        SeedProductionOrderItemId = c.Int(nullable: false),
                        GumProductionOrderItemId = c.Int(nullable: false),
                        MachineryDescription = c.String(),
                        ActivityPerformed = c.String(),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductionPeriodId = c.Int(nullable: false),
                        DateOfInformation = c.DateTime(nullable: false),
                        BugdetYear = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MachineryMaintenanceId);
            
        }
        
        public override void Down()
        {
            DropTable("Production.MachineryMaintenance");
        }
    }
}
