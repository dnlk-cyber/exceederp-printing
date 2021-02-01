namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hrm18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.PayrollProcessDeductions", "Months", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Payroll.PayrollProcessDeductions", "Months");
        }
    }
}
