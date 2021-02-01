namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class goodreceive2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.GoodReceiveItem", "IsMachinery");
            DropColumn("EXCEED.GoodReceiveItem", "BatchNo");
            DropColumn("EXCEED.GoodReceiveItem", "Model");
            DropColumn("EXCEED.GoodReceiveItem", "SerialNo");
            DropColumn("EXCEED.GoodReceiveItem", "Condition");
            DropColumn("EXCEED.GoodReceiveItem", "PlateNo");
            DropColumn("EXCEED.GoodReceiveItem", "ChassisNo");
            DropColumn("EXCEED.GoodReceiveItem", "EngineNo");
            DropColumn("EXCEED.GoodReceiveItem", "EngineModelNo");
            DropColumn("EXCEED.GoodReceiveItem", "EngineElectricalNo");
            DropColumn("EXCEED.GoodReceiveItem", "EngineElectricalModelNo");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GoodReceiveItem", "EngineElectricalModelNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "EngineElectricalNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "EngineModelNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "EngineNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "ChassisNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "PlateNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "Condition", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "SerialNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "Model", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "BatchNo", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "IsMachinery", c => c.Boolean(nullable: false));
        }
    }
}
