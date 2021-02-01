namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archive4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Archive.OutgoingLetter", "InstitutionTypeId", "Archive.InstitutionType");
            DropIndex("Archive.OutgoingLetter", new[] { "InstitutionTypeId" });
            AddColumn("Archive.OutgoingLetter", "RefersTo", c => c.Int());
            AlterColumn("Archive.EmployeeIncommingLetter", "Shelve", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "Row", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "FolderNo", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "FileNo", c => c.String());
            AlterColumn("Archive.EmployeeIncommingLetter", "OutgoingNoOfAttachment", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("Archive.EmployeeIncommingLetter", "OutgoingNoOfAttachment", c => c.Int(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "FileNo", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "FolderNo", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "Row", c => c.String(nullable: false));
            AlterColumn("Archive.EmployeeIncommingLetter", "Shelve", c => c.String(nullable: false));
            DropColumn("Archive.OutgoingLetter", "RefersTo");
            CreateIndex("Archive.OutgoingLetter", "InstitutionTypeId");
            AddForeignKey("Archive.OutgoingLetter", "InstitutionTypeId", "Archive.InstitutionType", "InstitutionTypeId", cascadeDelete: true);
        }
    }
}
