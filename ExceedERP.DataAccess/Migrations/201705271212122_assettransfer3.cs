namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assettransfer3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AssetRegistration", "BarCodeNo", c => c.String());
            AddColumn("EXCEED.AssetRegistration", "EngineNo", c => c.String());
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "CustodianBeforeTransfer", c => c.Int(nullable: false));
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "TransferTo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "TransferTo", c => c.String());
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "CustodianBeforeTransfer", c => c.String());
            DropColumn("EXCEED.AssetRegistration", "EngineNo");
            DropColumn("EXCEED.AssetRegistration", "BarCodeNo");
        }
    }
}
