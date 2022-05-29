namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payroll : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Payroll.PayrollEmployeeAccount", "AccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter");
            DropForeignKey("Payroll.PayrollEmployeeAccount", "AccountLocation_Values", "EXCEED.GLChartOfAccountLocation");
            DropIndex("Payroll.PayrollEmployeeAccount", new[] { "AccountCostCenter_Values" });
            DropIndex("Payroll.PayrollEmployeeAccount", new[] { "AccountLocation_Values" });
            DropColumn("Payroll.PayrollEmployeeAccount", "AccountCostCenter_Values");
            DropColumn("Payroll.PayrollEmployeeAccount", "AccountLocation_Values");
        }
        
        public override void Down()
        {
            AddColumn("Payroll.PayrollEmployeeAccount", "AccountLocation_Values", c => c.String(maxLength: 128));
            AddColumn("Payroll.PayrollEmployeeAccount", "AccountCostCenter_Values", c => c.String(maxLength: 128));
            CreateIndex("Payroll.PayrollEmployeeAccount", "AccountLocation_Values");
            CreateIndex("Payroll.PayrollEmployeeAccount", "AccountCostCenter_Values");
            AddForeignKey("Payroll.PayrollEmployeeAccount", "AccountLocation_Values", "EXCEED.GLChartOfAccountLocation", "Values");
            AddForeignKey("Payroll.PayrollEmployeeAccount", "AccountCostCenter_Values", "EXCEED.GLChartOfAccountCostCenter", "Values");
        }
    }
}
