namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemprefix : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "Prefix", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryItem", "Prefix");
        }
    }
}
