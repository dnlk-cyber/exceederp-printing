namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrapay : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.OverTime", "OrgBranchName", c => c.String());
            AddColumn("Payroll.OverTime", "DateCreated", c => c.DateTime());
            AddColumn("Payroll.OverTime", "LastModified", c => c.DateTime());
            AddColumn("Payroll.OverTime", "CreatedBy", c => c.String());
            AddColumn("Payroll.OverTime", "ModifiedBy", c => c.String());
            AddColumn("Payroll.OverTimeTransfer", "NormalHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.OverTimeTransfer", "NightHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.OverTimeTransfer", "HolidayHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.OverTimeTransfer", "RestdayHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.OverTimeTransfer", "Check", c => c.Boolean(nullable: false));
            AddColumn("Payroll.OverTimeTransfer", "OrgBranchName", c => c.String());
            AddColumn("Payroll.OverTimeTransfer", "DateCreated", c => c.DateTime());
            AddColumn("Payroll.OverTimeTransfer", "LastModified", c => c.DateTime());
            AddColumn("Payroll.OverTimeTransfer", "CreatedBy", c => c.String());
            AddColumn("Payroll.OverTimeTransfer", "ModifiedBy", c => c.String());
            AddColumn("Payroll.PayrollEmployeeAccount", "OrgBranchName", c => c.String());
            AddColumn("Payroll.PayrollEmployeeAccount", "DateCreated", c => c.DateTime());
            AddColumn("Payroll.PayrollEmployeeAccount", "LastModified", c => c.DateTime());
            AddColumn("Payroll.PayrollEmployeeAccount", "ModifiedBy", c => c.String());
            DropColumn("Payroll.PayrollEmployeeAccount", "CreatedOn");
        }
        
        public override void Down()
        {
            AddColumn("Payroll.PayrollEmployeeAccount", "CreatedOn", c => c.DateTime());
            DropColumn("Payroll.PayrollEmployeeAccount", "ModifiedBy");
            DropColumn("Payroll.PayrollEmployeeAccount", "LastModified");
            DropColumn("Payroll.PayrollEmployeeAccount", "DateCreated");
            DropColumn("Payroll.PayrollEmployeeAccount", "OrgBranchName");
            DropColumn("Payroll.OverTimeTransfer", "ModifiedBy");
            DropColumn("Payroll.OverTimeTransfer", "CreatedBy");
            DropColumn("Payroll.OverTimeTransfer", "LastModified");
            DropColumn("Payroll.OverTimeTransfer", "DateCreated");
            DropColumn("Payroll.OverTimeTransfer", "OrgBranchName");
            DropColumn("Payroll.OverTimeTransfer", "Check");
            DropColumn("Payroll.OverTimeTransfer", "RestdayHours");
            DropColumn("Payroll.OverTimeTransfer", "HolidayHours");
            DropColumn("Payroll.OverTimeTransfer", "NightHours");
            DropColumn("Payroll.OverTimeTransfer", "NormalHours");
            DropColumn("Payroll.OverTime", "ModifiedBy");
            DropColumn("Payroll.OverTime", "CreatedBy");
            DropColumn("Payroll.OverTime", "LastModified");
            DropColumn("Payroll.OverTime", "DateCreated");
            DropColumn("Payroll.OverTime", "OrgBranchName");
        }
    }
}
