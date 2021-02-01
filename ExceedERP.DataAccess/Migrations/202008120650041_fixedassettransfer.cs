namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassettransfer : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "UPrice");
        }
    }
}
