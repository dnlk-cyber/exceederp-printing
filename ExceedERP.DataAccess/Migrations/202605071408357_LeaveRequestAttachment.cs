namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeaveRequestAttachment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.LeaveRequestAttachement",
                c => new
                    {
                        LeaveRequestAttachementId = c.Int(nullable: false, identity: true),
                        LeaveRequestId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LeaveRequestAttachementId)
                .ForeignKey("HRM.LeaveRequest", t => t.LeaveRequestId, cascadeDelete: true)
                .Index(t => t.LeaveRequestId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.LeaveRequestAttachement", "LeaveRequestId", "HRM.LeaveRequest");
            DropIndex("EXCEED.LeaveRequestAttachement", new[] { "LeaveRequestId" });
            DropTable("EXCEED.LeaveRequestAttachement");
        }
    }
}
