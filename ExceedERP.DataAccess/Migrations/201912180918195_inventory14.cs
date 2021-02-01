namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventory14 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.StoreReturnAttachment",
                c => new
                    {
                        StoreReturnAttachmentId = c.Int(nullable: false, identity: true),
                        StoreReturnID = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreReturnAttachmentId)
                .ForeignKey("EXCEED.StoreReturn", t => t.StoreReturnID, cascadeDelete: true)
                .Index(t => t.StoreReturnID);
            
            AddColumn("EXCEED.PropertyCount", "StoreKeeperId", c => c.String());
            AddColumn("EXCEED.StoreReturn", "ConfirmationLetter", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.StoreReturnAttachment", "StoreReturnID", "EXCEED.StoreReturn");
            DropIndex("EXCEED.StoreReturnAttachment", new[] { "StoreReturnID" });
            DropColumn("EXCEED.StoreReturn", "ConfirmationLetter");
            DropColumn("EXCEED.PropertyCount", "StoreKeeperId");
            DropTable("EXCEED.StoreReturnAttachment");
        }
    }
}
