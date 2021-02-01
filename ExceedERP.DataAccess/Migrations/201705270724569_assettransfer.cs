namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assettransfer : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "TransferDesc", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.MachineryAndVehicleTransfer", "TransferDesc", c => c.DateTime(nullable: false));
        }
    }
}
