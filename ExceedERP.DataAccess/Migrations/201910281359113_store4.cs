namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryStore", "IsTemp", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryStore", "IsTemp");
        }
    }
}
