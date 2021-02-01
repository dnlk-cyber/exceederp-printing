namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset25 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetTransfer", "DeliveredBy", c => c.String());
            AddColumn("EXCEED.AssetTransfer", "PlateNo", c => c.String());
            AddColumn("EXCEED.AssetTransfer", "SupplierName", c => c.String());
            AddColumn("EXCEED.AssetTransfer", "DriverType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.AssetTransfer", "DriverType");
            DropColumn("EXCEED.AssetTransfer", "SupplierName");
            DropColumn("EXCEED.AssetTransfer", "PlateNo");
            DropColumn("EXCEED.AssetTransfer", "DeliveredBy");
        }
    }
}
