namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction46 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Construction.MonthlyProjectCost", "ActualDirectCostTillPrevMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "ActualProjectOHCostTillPrevMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectCost", "ActualProfitAndOHTillPrevMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.MonthlyProjectProgress", "ExecutedAmountToDateIncludingPriceAdjustment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Construction.MonthlyProjectProgress", "ExecutedAmountToDateIncludingPriceAdjustment");
            DropColumn("Construction.MonthlyProjectCost", "ActualProfitAndOHTillPrevMonth");
            DropColumn("Construction.MonthlyProjectCost", "ActualProjectOHCostTillPrevMonth");
            DropColumn("Construction.MonthlyProjectCost", "ActualDirectCostTillPrevMonth");
        }
    }
}
