namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm39 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.AttachmentArchive",
                c => new
                    {
                        AttachmentArchiveId = c.Int(nullable: false, identity: true),
                        ReferenceType = c.Int(nullable: false),
                        Reference = c.String(),
                        Description = c.String(),
                        URL = c.String(),
                        BinaryFile = c.Binary(),
                        FileType = c.String(),
                        Index = c.Double(nullable: false),
                        FileName = c.String(),
                        Remark = c.String(),
                        MINUTE = c.String(),
                    })
                .PrimaryKey(t => t.AttachmentArchiveId);
            
            AlterColumn("Archive.EmployeeIncommingLetter", "RefersTo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Archive.EmployeeIncommingLetter", "RefersTo", c => c.String());
            DropTable("EXCEED.AttachmentArchive");
        }
    }
}
