namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class itemfulldesc : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "ItemCodeName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.InventoryItem", "ItemCodeName");
        }
    }
}
