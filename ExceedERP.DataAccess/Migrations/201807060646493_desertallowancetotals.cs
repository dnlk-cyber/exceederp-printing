namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class desertallowancetotals : DbMigration
    {
        public override void Up()
        {
            AddColumn("Payroll.PayrollProcessViewModel", "TaxableDesertAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "NonTaxableDesertAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Payroll.PayrollProcessViewModel", "TotalDesertAllowance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Payroll.PayrollProcessViewModel", "TotalDesertAllowance");
            DropColumn("Payroll.PayrollProcessViewModel", "NonTaxableDesertAllowance");
            DropColumn("Payroll.PayrollProcessViewModel", "TaxableDesertAllowance");
        }
    }
}
