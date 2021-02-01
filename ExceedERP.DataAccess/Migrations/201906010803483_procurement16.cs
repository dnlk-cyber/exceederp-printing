namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ProcurementPlanItem", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProcurementPlanItem", "TotalPrice");
        }
    }
}
