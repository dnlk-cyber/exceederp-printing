namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payroll3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.PayrollProcessViewModel", "UnpaidLeaveDays", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "UnpaidLeaveDeduction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Payroll.PayrollProcessViewModel", "UnpaidLeaveDeduction");
            DropColumn("Payroll.PayrollProcessViewModel", "UnpaidLeaveDays");
        }
    }
}
