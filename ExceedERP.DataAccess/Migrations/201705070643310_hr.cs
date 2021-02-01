namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hr : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.AllowanceDefination", "LookupId", "common.Lookup");
            DropForeignKey("Performance.EmployeeMeasurement", "PeriodId", "common.Period");
            DropForeignKey("Performance.EmployeeMeasurementDefinition", "PeriodId", "common.Period");
            DropForeignKey("Performance.EmployeePerformanceAction", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeavePaid", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeavePlan", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeaveRequest", "PeriodId", "common.Period");
            DropForeignKey("HRM.Leave", "PeriodId", "common.Period");
            DropForeignKey("HRM.LeaveEntitle", "PeriodId", "common.Period");
            DropForeignKey("Health.PatientsLeave", "PeriodId", "common.Period");
            DropIndex("HRM.AllowanceDefination", new[] { "LookupId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "PeriodId" });
            DropIndex("Performance.EmployeeMeasurementDefinition", new[] { "PeriodId" });
            DropIndex("Performance.EmployeePerformanceAction", new[] { "PeriodId" });
            DropIndex("HRM.LeavePaid", new[] { "PeriodId" });
            DropIndex("HRM.LeavePlan", new[] { "PeriodId" });
            DropIndex("HRM.LeaveRequest", new[] { "PeriodId" });
            DropIndex("HRM.Leave", new[] { "PeriodId" });
            DropIndex("HRM.LeaveEntitle", new[] { "PeriodId" });
            DropIndex("Health.PatientsLeave", new[] { "PeriodId" });
            AddColumn("Performance.EmployeeMeasurement", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurementDefinition", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceAction", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.FixedContribution", "AmountPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.FixedContribution", "NetGross", c => c.Int(nullable: false));
            AddColumn("Payroll.FixedContribution", "AmtPercnet", c => c.Int(nullable: false));
            AddColumn("HRM.LeavePaid", "GLFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.LeavePlan", "GLFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "GLFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.Leave", "GLFiscalYearId", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveEntitle", "GLFiscalYearId", c => c.Int(nullable: false));
            AddColumn("Health.PatientsLeave", "GLFiscalYearId", c => c.Int(nullable: false));
            AlterColumn("HRM.AllowanceDefination", "LookupId", c => c.Int());
            CreateIndex("Performance.EmployeeMeasurement", "GLPeriodId");
            CreateIndex("Performance.EmployeeMeasurementDefinition", "GLPeriodId");
            CreateIndex("Performance.EmployeePerformanceAction", "GLPeriodId");
            CreateIndex("HRM.LeavePaid", "GLFiscalYearId");
            CreateIndex("HRM.LeavePlan", "GLFiscalYearId");
            CreateIndex("HRM.LeaveRequest", "GLFiscalYearId");
            CreateIndex("HRM.Leave", "GLFiscalYearId");
            CreateIndex("HRM.LeaveEntitle", "GLFiscalYearId");
            CreateIndex("Health.PatientsLeave", "GLFiscalYearId");
            AddForeignKey("Performance.EmployeeMeasurement", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurementDefinition", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeePerformanceAction", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("HRM.LeavePaid", "GLFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.LeavePlan", "GLFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.LeaveRequest", "GLFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.Leave", "GLFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("HRM.LeaveEntitle", "GLFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            AddForeignKey("Health.PatientsLeave", "GLFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            DropColumn("Performance.EmployeeMeasurement", "PeriodId");
            DropColumn("Performance.EmployeeMeasurementDefinition", "PeriodId");
            DropColumn("Performance.EmployeePerformanceAction", "PeriodId");
            DropColumn("HRM.LeavePaid", "PeriodId");
            DropColumn("HRM.LeavePlan", "PeriodId");
            DropColumn("HRM.LeaveRequest", "PeriodId");
            DropColumn("HRM.Leave", "PeriodId");
            DropColumn("HRM.LeaveEntitle", "PeriodId");
            DropColumn("Health.PatientsLeave", "PeriodId");
        }
        
        public override void Down()
        {
            AddColumn("Health.PatientsLeave", "PeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveEntitle", "PeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.Leave", "PeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.LeaveRequest", "PeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.LeavePlan", "PeriodId", c => c.Int(nullable: false));
            AddColumn("HRM.LeavePaid", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeePerformanceAction", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurementDefinition", "PeriodId", c => c.Int(nullable: false));
            AddColumn("Performance.EmployeeMeasurement", "PeriodId", c => c.Int(nullable: false));
            DropForeignKey("Health.PatientsLeave", "GLFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.LeaveEntitle", "GLFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.Leave", "GLFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.LeaveRequest", "GLFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.LeavePlan", "GLFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("HRM.LeavePaid", "GLFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Performance.EmployeePerformanceAction", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Performance.EmployeeMeasurementDefinition", "GLPeriodId", "EXCEED.GLPeriod");
            DropForeignKey("Performance.EmployeeMeasurement", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("Health.PatientsLeave", new[] { "GLFiscalYearId" });
            DropIndex("HRM.LeaveEntitle", new[] { "GLFiscalYearId" });
            DropIndex("HRM.Leave", new[] { "GLFiscalYearId" });
            DropIndex("HRM.LeaveRequest", new[] { "GLFiscalYearId" });
            DropIndex("HRM.LeavePlan", new[] { "GLFiscalYearId" });
            DropIndex("HRM.LeavePaid", new[] { "GLFiscalYearId" });
            DropIndex("Performance.EmployeePerformanceAction", new[] { "GLPeriodId" });
            DropIndex("Performance.EmployeeMeasurementDefinition", new[] { "GLPeriodId" });
            DropIndex("Performance.EmployeeMeasurement", new[] { "GLPeriodId" });
            AlterColumn("HRM.AllowanceDefination", "LookupId", c => c.Int(nullable: false));
            DropColumn("Health.PatientsLeave", "GLFiscalYearId");
            DropColumn("HRM.LeaveEntitle", "GLFiscalYearId");
            DropColumn("HRM.Leave", "GLFiscalYearId");
            DropColumn("HRM.LeaveRequest", "GLFiscalYearId");
            DropColumn("HRM.LeavePlan", "GLFiscalYearId");
            DropColumn("HRM.LeavePaid", "GLFiscalYearId");
            DropColumn("Payroll.FixedContribution", "AmtPercnet");
            DropColumn("Payroll.FixedContribution", "NetGross");
            DropColumn("Payroll.FixedContribution", "AmountPercent");
            DropColumn("Performance.EmployeePerformanceAction", "GLPeriodId");
            DropColumn("Performance.EmployeeMeasurementDefinition", "GLPeriodId");
            DropColumn("Performance.EmployeeMeasurement", "GLPeriodId");
            CreateIndex("Health.PatientsLeave", "PeriodId");
            CreateIndex("HRM.LeaveEntitle", "PeriodId");
            CreateIndex("HRM.Leave", "PeriodId");
            CreateIndex("HRM.LeaveRequest", "PeriodId");
            CreateIndex("HRM.LeavePlan", "PeriodId");
            CreateIndex("HRM.LeavePaid", "PeriodId");
            CreateIndex("Performance.EmployeePerformanceAction", "PeriodId");
            CreateIndex("Performance.EmployeeMeasurementDefinition", "PeriodId");
            CreateIndex("Performance.EmployeeMeasurement", "PeriodId");
            CreateIndex("HRM.AllowanceDefination", "LookupId");
            AddForeignKey("Health.PatientsLeave", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.LeaveEntitle", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.Leave", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.LeaveRequest", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.LeavePlan", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.LeavePaid", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeePerformanceAction", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurementDefinition", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("Performance.EmployeeMeasurement", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
            AddForeignKey("HRM.AllowanceDefination", "LookupId", "common.Lookup", "LookupId", cascadeDelete: true);
        }
    }
}
