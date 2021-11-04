namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payrollhr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.AttendanceNew",
                c => new
                    {
                        AttendanceNewId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DateAm = c.String(),
                        IsPresentMorning = c.Boolean(nullable: false),
                        IsPresentAfternoon = c.Boolean(nullable: false),
                        GlFisicalYearId = c.Int(nullable: false),
                        GlPeriodId = c.Int(nullable: false),
                        IsDepartmentApproved = c.Boolean(nullable: false),
                        DepartmentApproverId = c.Int(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        ApproverEmpId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AttendanceNewId);
            
            CreateTable(
                "EXCEED.DelegationTempoRole",
                c => new
                    {
                        DelegationTempoRoleId = c.Int(nullable: false, identity: true),
                        FromUserId = c.String(),
                        ToUserId = c.String(),
                        RoleName = c.String(),
                    })
                .PrimaryKey(t => t.DelegationTempoRoleId);
            
            AddColumn("HRM.Delegation", "IsOpen", c => c.Boolean(nullable: false));
            AlterColumn("HRM.Delegation", "FromDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("HRM.Delegation", "FromDateTime", c => c.DateTime(nullable: false));
            DropColumn("HRM.Delegation", "IsOpen");
            DropTable("EXCEED.DelegationTempoRole");
            DropTable("EXCEED.AttendanceNew");
        }
    }
}
