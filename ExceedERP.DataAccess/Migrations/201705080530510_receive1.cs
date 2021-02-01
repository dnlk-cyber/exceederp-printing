namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receive1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("EXCEED.GoodReceiveItem", "TotalAmount");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.GoodReceiveItem", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
