namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cont2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ProformTechnical",
                c => new
                    {
                        ProformTechnicalId = c.Int(nullable: false, identity: true),
                        SupplierId = c.Int(nullable: false),
                        ItemCode = c.String(),
                        Result = c.Single(nullable: false),
                        ProformaPurchaseID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProformTechnicalId)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ProformTechnical", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropIndex("EXCEED.ProformTechnical", new[] { "ProformaPurchaseID" });
            DropTable("EXCEED.ProformTechnical");
        }
    }
}
