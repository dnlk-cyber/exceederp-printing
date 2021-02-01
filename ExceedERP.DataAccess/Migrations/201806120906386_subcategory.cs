namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subcategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.InventoryItem", "SubCategoryCode", c => c.String());
            DropColumn("EXCEED.ItemCategory", "SubCategoryCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ItemCategory", "SubCategoryCode", c => c.String());
            DropColumn("EXCEED.InventoryItem", "SubCategoryCode");
        }
    }
}
