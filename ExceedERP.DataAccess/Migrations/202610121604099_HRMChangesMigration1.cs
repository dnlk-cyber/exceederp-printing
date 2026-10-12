namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HRMChangesMigration1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GeneralJobtitleSetting",
                c => new
                    {
                        GeneralJobtitleSettingId = c.Int(nullable: false, identity: true),
                        IsManual = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GeneralJobtitleSettingId);
            
            AddColumn("HRM.Cosign", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("HRM.EmployeeAssociation", "DateOfBirth", c => c.DateTime());
            AddColumn("HRM.EmployeeAssociation", "DateOfBirthAm", c => c.String());
            AddColumn("HRM.SalarySetting", "OrgBranchName", c => c.String());
            AddColumn("HRM.SalarySetting", "DateCreated", c => c.DateTime());
            AddColumn("HRM.SalarySetting", "LastModified", c => c.DateTime());
            AddColumn("HRM.SalarySetting", "CreatedBy", c => c.String());
            AddColumn("HRM.SalarySetting", "ModifiedBy", c => c.String());
            AlterColumn("HRM.Experience", "ClearanceDateTime", c => c.DateTime());
            AlterColumn("HRM.Training", "To", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("HRM.Training", "To", c => c.DateTime(nullable: false));
            AlterColumn("HRM.Experience", "ClearanceDateTime", c => c.DateTime(nullable: false));
            DropColumn("HRM.SalarySetting", "ModifiedBy");
            DropColumn("HRM.SalarySetting", "CreatedBy");
            DropColumn("HRM.SalarySetting", "LastModified");
            DropColumn("HRM.SalarySetting", "DateCreated");
            DropColumn("HRM.SalarySetting", "OrgBranchName");
            DropColumn("HRM.EmployeeAssociation", "DateOfBirthAm");
            DropColumn("HRM.EmployeeAssociation", "DateOfBirth");
            DropColumn("HRM.Cosign", "IsActive");
            DropTable("EXCEED.GeneralJobtitleSetting");
        }
    }
}
