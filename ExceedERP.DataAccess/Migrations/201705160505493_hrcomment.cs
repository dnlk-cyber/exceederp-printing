namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrcomment : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.Addition", "AdditionTaxType", c => c.Int(nullable: false));
            AddColumn("HRM.AllowanceDefination", "AmtPercent", c => c.Int(nullable: false));
            AddColumn("HRM.Delegation", "GLPeriodId", c => c.Int(nullable: false));
            AddColumn("Payroll.PayrollProcessViewModel", "TotalTaxAdditions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "TotalNonTaxAdditions", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("HRM.Delegation", "GLPeriodId");
            AddForeignKey("HRM.Delegation", "GLPeriodId", "EXCEED.GLPeriod", "GLPeriodId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("HRM.Delegation", "GLPeriodId", "EXCEED.GLPeriod");
            DropIndex("HRM.Delegation", new[] { "GLPeriodId" });
            DropColumn("Payroll.PayrollProcessViewModel", "TotalNonTaxAdditions");
            DropColumn("Payroll.PayrollProcessViewModel", "TotalTaxAdditions");
            DropColumn("HRM.Delegation", "GLPeriodId");
            DropColumn("HRM.AllowanceDefination", "AmtPercent");
            DropColumn("Payroll.Addition", "AdditionTaxType");
        }
    }
}
