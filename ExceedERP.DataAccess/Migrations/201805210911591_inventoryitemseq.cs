namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inventoryitemseq : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "ItemIndex", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryItem", "ItemIndex");
        }
    }
}
