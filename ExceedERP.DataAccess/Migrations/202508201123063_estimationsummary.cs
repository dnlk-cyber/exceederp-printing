namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class estimationsummary : DbMigration
    {
        public override void Up()
        {
            AddColumn("Manufacturing.EstimationDetail", "ProfitMargin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Manufacturing.EstimationDetail", "ProfitMargin");
        }
    }
}
