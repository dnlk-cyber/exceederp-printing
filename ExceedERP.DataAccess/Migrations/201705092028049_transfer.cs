namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transfer : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.ItemTransfer", "PlateNo", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "SupplierName", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "DriverType", c => c.String());
            DropColumn("EXCEED.ItemTransfer", "VehiclePlate");
            DropColumn("EXCEED.ItemTransfer", "DriverTye");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ItemTransfer", "DriverTye", c => c.String());
            AddColumn("EXCEED.ItemTransfer", "VehiclePlate", c => c.String());
            DropColumn("EXCEED.ItemTransfer", "DriverType");
            DropColumn("EXCEED.ItemTransfer", "SupplierName");
            DropColumn("EXCEED.ItemTransfer", "PlateNo");
        }
    }
}
