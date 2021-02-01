namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Payroll.Association", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.FixedContribution", "GLPeriodId", "Payroll.PayrollPeriod");
            DropForeignKey("Payroll.PeriodStatus", "GLPeriodId", "Payroll.PayrollPeriod");
            DropIndex("Payroll.Association", new[] { "GLPeriodId" });
            DropIndex("Payroll.FixedContribution", new[] { "GLPeriodId" });
            DropIndex("Payroll.PeriodStatus", new[] { "GLPeriodId" });
            AddColumn("HRM.Education", "IsHighLevel", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("HRM.Education", "IsHighLevel");
            CreateIndex("Payroll.PeriodStatus", "GLPeriodId");
            CreateIndex("Payroll.FixedContribution", "GLPeriodId");
            CreateIndex("Payroll.Association", "GLPeriodId");
            AddForeignKey("Payroll.PeriodStatus", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.FixedContribution", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
            AddForeignKey("Payroll.Association", "GLPeriodId", "Payroll.PayrollPeriod", "GLPeriodId", cascadeDelete: true);
        }
    }
}
