namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class item2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "HasTransaction", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryItem", "HasTransaction");
        }
    }
}
