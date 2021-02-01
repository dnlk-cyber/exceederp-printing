namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fa3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "Supplier", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "Manufacturer", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "ManufacturDate", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetReceiveItem", "IsBuilding", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetReceiveItem", "Location", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "Contractor", c => c.String());
            AddColumn("EXCEED.FixedAssetReceiveItem", "ConstructionStartDate", c => c.DateTime());
            AddColumn("EXCEED.FixedAssetReceiveItem", "ConstructionEndDate", c => c.DateTime());
            DropColumn("EXCEED.FixedAssetReceiveItem", "UOM");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "UOM", c => c.String());
            DropColumn("EXCEED.FixedAssetReceiveItem", "ConstructionEndDate");
            DropColumn("EXCEED.FixedAssetReceiveItem", "ConstructionStartDate");
            DropColumn("EXCEED.FixedAssetReceiveItem", "Contractor");
            DropColumn("EXCEED.FixedAssetReceiveItem", "Location");
            DropColumn("EXCEED.FixedAssetReceiveItem", "IsBuilding");
            DropColumn("EXCEED.FixedAssetReceiveItem", "ManufacturDate");
            DropColumn("EXCEED.FixedAssetReceiveItem", "Manufacturer");
            DropColumn("EXCEED.FixedAssetReceiveItem", "Supplier");
        }
    }
}
