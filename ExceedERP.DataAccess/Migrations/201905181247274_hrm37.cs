namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm37 : DbMigration
    {
        public override void Up()
        {
            AddColumn("HRM.WeeklyAttendance", "GlFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.WeeklyAttendance", "GLPeriodId", c => c.Int(nullable: false));
            DropColumn("Insurance.AccidentRecord", "EmployeeCode");
        }
        
        public override void Down()
        {
            AddColumn("Insurance.AccidentRecord", "EmployeeCode", c => c.String());
            DropColumn("HRM.WeeklyAttendance", "GLPeriodId");
            DropColumn("HRM.WeeklyAttendance", "GlFiscalYearId");
        }
    }
}
