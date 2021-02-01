namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ProformaUploadDbModel",
                c => new
                    {
                        ProformaUploadDbModelId = c.Int(nullable: false, identity: true),
                        ProformaPurchaseID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.ProformaUploadDbModelId)
                .ForeignKey("EXCEED.ProformaPurchase", t => t.ProformaPurchaseID, cascadeDelete: true)
                .Index(t => t.ProformaPurchaseID);
            
            AddColumn("EXCEED.PassFailResult", "Remark", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ProformaUploadDbModel", "ProformaPurchaseID", "EXCEED.ProformaPurchase");
            DropIndex("EXCEED.ProformaUploadDbModel", new[] { "ProformaPurchaseID" });
            DropColumn("EXCEED.ProformPassFail", "Remark");
            DropColumn("EXCEED.PassFailResult", "Remark");
            DropTable("EXCEED.ProformaUploadDbModel");
        }
    }
}
