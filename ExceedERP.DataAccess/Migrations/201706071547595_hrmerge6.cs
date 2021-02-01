namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmerge6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.Addition", "OrgBranchName", c => c.String());
            AddColumn("Payroll.Addition", "DateCreated", c => c.DateTime());
            AddColumn("Payroll.Addition", "LastModified", c => c.DateTime());
            AddColumn("Payroll.Addition", "CreatedBy", c => c.String());
            AddColumn("Payroll.Addition", "ModifiedBy", c => c.String());
            AddColumn("Payroll.Advance", "OrgBranchName", c => c.String());
            AddColumn("Payroll.Advance", "DateCreated", c => c.DateTime());
            AddColumn("Payroll.Advance", "LastModified", c => c.DateTime());
            AddColumn("Payroll.Advance", "CreatedBy", c => c.String());
            AddColumn("Payroll.Advance", "ModifiedBy", c => c.String());
            AddColumn("Archive.IncomingLetter", "OrgStructureId", c => c.Int(nullable: false));
            AlterColumn("HRM.EmployeeAssociation", "Name", c => c.String(nullable: false));
            AlterColumn("Archive.RecivedLetters", "Receiver", c => c.Int(nullable: false));
            CreateIndex("Archive.IncomingLetter", "OrgStructureId");
            AddForeignKey("Archive.IncomingLetter", "OrgStructureId", "HRM.MainCompanyStructure", "OrgStructureId", cascadeDelete: true);
            DropColumn("Archive.IncomingLetter", "SerialNo");
        }
        
        public override void Down()
        {
            AddColumn("Archive.IncomingLetter", "SerialNo", c => c.String());
            DropForeignKey("Archive.IncomingLetter", "OrgStructureId", "HRM.MainCompanyStructure");
            DropIndex("Archive.IncomingLetter", new[] { "OrgStructureId" });
            AlterColumn("Archive.RecivedLetters", "Receiver", c => c.String());
            AlterColumn("HRM.EmployeeAssociation", "Name", c => c.String());
            DropColumn("Archive.IncomingLetter", "OrgStructureId");
            DropColumn("Payroll.Advance", "ModifiedBy");
            DropColumn("Payroll.Advance", "CreatedBy");
            DropColumn("Payroll.Advance", "LastModified");
            DropColumn("Payroll.Advance", "DateCreated");
            DropColumn("Payroll.Advance", "OrgBranchName");
            DropColumn("Payroll.Addition", "ModifiedBy");
            DropColumn("Payroll.Addition", "CreatedBy");
            DropColumn("Payroll.Addition", "LastModified");
            DropColumn("Payroll.Addition", "DateCreated");
            DropColumn("Payroll.Addition", "OrgBranchName");
        }
    }
}
