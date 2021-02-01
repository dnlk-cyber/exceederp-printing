namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class agingperiodsetup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.AgingPeriodSetup",
                c => new
                    {
                        AgingPeriodSetupId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Period1 = c.Int(nullable: false),
                        Period2 = c.Int(nullable: false),
                        Period3 = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AgingPeriodSetupId);
            
        }
        
        public override void Down()
        {
            DropTable("EXCEED.AgingPeriodSetup");
        }
    }
}
