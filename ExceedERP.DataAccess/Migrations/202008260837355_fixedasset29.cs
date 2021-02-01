namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset29 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "Unit", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "Unit");
        }
    }
}
