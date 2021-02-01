namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "Type", c => c.Int(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "ToStoreCode", c => c.String());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FromBranchId", c => c.Int());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "ToBranchId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "ToBranchId");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FromBranchId");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "ToStoreCode");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "Type");
        }
    }
}
