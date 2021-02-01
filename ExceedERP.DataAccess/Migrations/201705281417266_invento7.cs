namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invento7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryItem", "TotalPrice");
        }
    }
}
