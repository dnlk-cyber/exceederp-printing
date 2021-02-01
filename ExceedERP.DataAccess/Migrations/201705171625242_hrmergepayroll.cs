namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrmergepayroll : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("HRM.TrainingPlan", "PeriodId", "common.Period");
            DropIndex("HRM.TrainingPlan", new[] { "PeriodId" });
            AddColumn("HRM.LeaveRequest", "IsForOther", c => c.Boolean(nullable: false));
            AddColumn("Payroll.OverTimeJobResult", "OverTimeRequestId", c => c.Int(nullable: false));
            AddColumn("HRM.TrainingPlan", "GlFiscalYearId", c => c.Int(nullable: false));
            CreateIndex("Payroll.OverTimeJobResult", "OverTimeRequestId");
            CreateIndex("HRM.TrainingPlan", "GlFiscalYearId");
            AddForeignKey("Payroll.OverTimeJobResult", "OverTimeRequestId", "HRM.OverTimeRequest", "OverTimeRequestId", cascadeDelete: true);
            AddForeignKey("HRM.TrainingPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear", "GlFiscalYearId", cascadeDelete: true);
            DropColumn("HRM.TrainingPlan", "PeriodId");
        }
        
        public override void Down()
        {
            AddColumn("HRM.TrainingPlan", "PeriodId", c => c.Int(nullable: false));
            DropForeignKey("HRM.TrainingPlan", "GlFiscalYearId", "EXCEED.GlFiscalYear");
            DropForeignKey("Payroll.OverTimeJobResult", "OverTimeRequestId", "HRM.OverTimeRequest");
            DropIndex("HRM.TrainingPlan", new[] { "GlFiscalYearId" });
            DropIndex("Payroll.OverTimeJobResult", new[] { "OverTimeRequestId" });
            DropColumn("HRM.TrainingPlan", "GlFiscalYearId");
            DropColumn("Payroll.OverTimeJobResult", "OverTimeRequestId");
            DropColumn("HRM.LeaveRequest", "IsForOther");
            CreateIndex("HRM.TrainingPlan", "PeriodId");
            AddForeignKey("HRM.TrainingPlan", "PeriodId", "common.Period", "PeriodId", cascadeDelete: true);
        }
    }
}
