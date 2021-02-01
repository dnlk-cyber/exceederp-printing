namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm11 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.InstitutionIncommingUploadDbModel", "BookNo", "Archive.IncomingLetter");
            DropIndex("EXCEED.InstitutionIncommingUploadDbModel", new[] { "BookNo" });
            CreateTable(
                "HRM.EducationRequest",
                c => new
                    {
                        EducationRequestId = c.Int(nullable: false, identity: true),
                        OrgStructureId = c.Int(nullable: false),
                        LookupId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAmharic = c.String(),
                        Remark = c.String(),
                        EducationType = c.String(),
                    })
                .PrimaryKey(t => t.EducationRequestId)
                .ForeignKey("Common.Lookup", t => t.LookupId, cascadeDelete: true)
                .ForeignKey("HRM.MainCompanyStructure", t => t.OrgStructureId, cascadeDelete: true)
                .Index(t => t.OrgStructureId)
                .Index(t => t.LookupId);
            
            AddColumn("Archive.EmployeeLetterSender", "File", c => c.Binary());
            AddColumn("Archive.EmployeeLetterSender", "FileType", c => c.String());
            AddColumn("Archive.LetterSender", "File", c => c.Binary());
            AddColumn("Archive.LetterSender", "FileType", c => c.String());
            DropTable("EXCEED.InstitutionIncommingUploadDbModel");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.InstitutionIncommingUploadDbModel",
                c => new
                    {
                        InstitutionIncommingUploadDbModelId = c.Int(nullable: false, identity: true),
                        BookNo = c.String(maxLength: 128),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.InstitutionIncommingUploadDbModelId);
            
            DropForeignKey("HRM.EducationRequest", "OrgStructureId", "HRM.MainCompanyStructure");
            DropForeignKey("HRM.EducationRequest", "LookupId", "Common.Lookup");
            DropIndex("HRM.EducationRequest", new[] { "LookupId" });
            DropIndex("HRM.EducationRequest", new[] { "OrgStructureId" });
            DropColumn("Archive.LetterSender", "FileType");
            DropColumn("Archive.LetterSender", "File");
            DropColumn("Archive.EmployeeLetterSender", "FileType");
            DropColumn("Archive.EmployeeLetterSender", "File");
            DropTable("HRM.EducationRequest");
            CreateIndex("EXCEED.InstitutionIncommingUploadDbModel", "BookNo");
            AddForeignKey("EXCEED.InstitutionIncommingUploadDbModel", "BookNo", "Archive.IncomingLetter", "BookNo");
        }
    }
}
