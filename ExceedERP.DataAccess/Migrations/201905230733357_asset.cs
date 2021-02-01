namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asset : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReturnItem", "Quantity", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetReturnItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FixedAssetReturnItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.FixedAssetReturnItem", "Total");
            DropColumn("EXCEED.FixedAssetReturnItem", "UnitPrice");
            DropColumn("EXCEED.FixedAssetReturnItem", "Quantity");
        }
    }
}
