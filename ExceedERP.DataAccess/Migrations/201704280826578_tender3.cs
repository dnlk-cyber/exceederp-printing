namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tender3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.LotItems", "ItemCategory", c => c.String(nullable: false));
            AddColumn("EXCEED.LotItems", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.LotItems", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "ItemCategory", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipantsItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipantsItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.ParticipantsItem", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ParticipantsItem", "ItemCode", c => c.String());
            DropColumn("EXCEED.ParticipantsItem", "DiscountAmount");
            DropColumn("EXCEED.ParticipantsItem", "InventoryItemDesc");
            DropColumn("EXCEED.ParticipantsItem", "ItemCategoryDesc");
            DropColumn("EXCEED.ParticipantsItem", "InventoryItem");
            DropColumn("EXCEED.ParticipantsItem", "ItemCategory");
            DropColumn("EXCEED.LotItems", "InventoryItemDesc");
            DropColumn("EXCEED.LotItems", "ItemCategoryDesc");
            DropColumn("EXCEED.LotItems", "ItemCategory");
        }
    }
}
