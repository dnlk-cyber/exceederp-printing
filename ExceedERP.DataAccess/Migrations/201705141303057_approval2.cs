namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approval2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "Enable", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.InventoryStore", "Enable", c => c.Boolean(nullable: false));
            DropColumn("EXCEED.InventoryItem", "Abbrevation");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.InventoryItem", "Abbrevation", c => c.String());
            DropColumn("EXCEED.InventoryStore", "Enable");
            DropColumn("EXCEED.InventoryItem", "Enable");
        }
    }
}
