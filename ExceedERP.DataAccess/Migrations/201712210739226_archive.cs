namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archive : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.EmployeeIncommingUploadDbModel",
                c => new
                    {
                        EmployeeIncommingUploadDbModelId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(maxLength: 128),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeIncommingUploadDbModelId)
                .ForeignKey("Archive.EmployeeIncommingLetter", t => t.BookNo)
                .Index(t => t.BookNo);
            
            CreateTable(
                "EXCEED.InstitutionIncommingUploadDbModel",
                c => new
                    {
                        InstitutionIncommingUploadDbModelId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(maxLength: 128),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.InstitutionIncommingUploadDbModelId)
                .ForeignKey("Archive.IncomingLetter", t => t.BookNo)
                .Index(t => t.BookNo);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.InstitutionIncommingUploadDbModel", "BookNo", "Archive.IncomingLetter");
            DropForeignKey("EXCEED.EmployeeIncommingUploadDbModel", "BookNo", "Archive.EmployeeIncommingLetter");
            DropIndex("EXCEED.InstitutionIncommingUploadDbModel", new[] { "BookNo" });
            DropIndex("EXCEED.EmployeeIncommingUploadDbModel", new[] { "BookNo" });
            DropTable("EXCEED.InstitutionIncommingUploadDbModel");
            DropTable("EXCEED.EmployeeIncommingUploadDbModel");
        }
    }
}
