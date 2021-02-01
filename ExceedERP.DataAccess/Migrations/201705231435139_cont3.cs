namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cont3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ProformPassFail",
                c => new
                    {
                        ProformPassFailId = c.Int(nullable: false, identity: true),
                        SupplierId = c.Int(nullable: false),
                        ItemCode = c.String(nullable: false),
                        Result = c.Int(nullable: false),
                        ProformaPurchaseID = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProformPassFailId)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            AddColumn("EXCEED.ProformTechnical", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ProformTechnical", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ProformTechnical", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ProformTechnical", "ModifiedBy", c => c.String());
            AlterColumn("EXCEED.ProformTechnical", "ItemCode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ProformPassFail", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropIndex("EXCEED.ProformPassFail", new[] { "ProformaPurchaseID" });
            AlterColumn("EXCEED.ProformTechnical", "ItemCode", c => c.String());
            DropColumn("EXCEED.ProformTechnical", "ModifiedBy");
            DropColumn("EXCEED.ProformTechnical", "CreatedBy");
            DropColumn("EXCEED.ProformTechnical", "LastModified");
            DropColumn("EXCEED.ProformTechnical", "DateCreated");
            DropTable("EXCEED.ProformPassFail");
        }
    }
}
