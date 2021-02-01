namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction45 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.MonthlyProjectProgress", "PriceAdjustmentThisMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectProgress", "PriceAdjustmentTillPrevMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectProgress", "PriceAdjustmentToDate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Construction.MonthlyProjectProgress", "PriceAdjustmentToDate");
            DropColumn("Construction.MonthlyProjectProgress", "PriceAdjustmentTillPrevMonth");
            DropColumn("Construction.MonthlyProjectProgress", "PriceAdjustmentThisMonth");
        }
    }
}
