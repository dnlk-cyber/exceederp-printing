namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class drivertype : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.ItemTransfer", "DriverType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ItemTransfer", "DriverType", c => c.String());
        }
    }
}
