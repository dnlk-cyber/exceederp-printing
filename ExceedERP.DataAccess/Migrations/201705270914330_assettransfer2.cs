namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assettransfer2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "ChassisNo", c => c.String());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FuelVolume", c => c.String());
            AddColumn("EXCEED.AssetRegistration", "ChassisNo", c => c.String());
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "ChasisNo");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FuelVolue");
            DropColumn("EXCEED.AssetRegistration", "ChasisNo");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.AssetRegistration", "ChasisNo", c => c.String());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FuelVolue", c => c.String());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "ChasisNo", c => c.String());
            DropColumn("EXCEED.AssetRegistration", "ChassisNo");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FuelVolume");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "ChassisNo");
        }
    }
}
