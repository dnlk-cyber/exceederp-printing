namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset21 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "CustodianBeforeTransfer", c => c.Int());
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "TransferTo", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "TransferTo", c => c.Int(nullable: false));
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "CustodianBeforeTransfer", c => c.Int(nullable: false));
        }
    }
}
