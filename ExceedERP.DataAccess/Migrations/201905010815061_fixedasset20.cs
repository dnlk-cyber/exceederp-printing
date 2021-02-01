namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FromStoreCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FromStoreCode");
        }
    }
}
