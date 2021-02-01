namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pr4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.LotItems", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.LotItems", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ParticipantsItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ParticipantsItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ParticipantsItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ParticipantsItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("EXCEED.LotItems", "ItemCategoryCode");
            CreateIndex("EXCEED.LotItems", "ItemCode");
            CreateIndex("EXCEED.ParticipantsItem", "ItemCategoryCode");
            CreateIndex("EXCEED.ParticipantsItem", "ItemCode");
            AddForeignKey("EXCEED.LotItems", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.LotItems", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ParticipantsItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ParticipantsItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.LotItems", "ItemCategory");
            DropColumn("EXCEED.LotItems", "InventoryItem");
            DropColumn("EXCEED.LotItems", "ItemCategoryDesc");
            DropColumn("EXCEED.LotItems", "InventoryItemDesc");
            DropColumn("EXCEED.ParticipantsItem", "ItemCategory");
            DropColumn("EXCEED.ParticipantsItem", "InventoryItem");
            DropColumn("EXCEED.ParticipantsItem", "ItemCategoryDesc");
            DropColumn("EXCEED.ParticipantsItem", "InventoryItemDesc");
            DropColumn("EXCEED.ParticipantsItem", "TPrice");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ParticipantsItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ParticipantsItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipantsItem", "ItemCategory", c => c.String(nullable: false));
            AddColumn("EXCEED.LotItems", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.LotItems", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.LotItems", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.LotItems", "ItemCategory", c => c.String(nullable: false));
            DropForeignKey("EXCEED.ParticipantsItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ParticipantsItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.LotItems", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.LotItems", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.ParticipantsItem", new[] { "ItemCode" });
            DropIndex("EXCEED.ParticipantsItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.LotItems", new[] { "ItemCode" });
            DropIndex("EXCEED.LotItems", new[] { "ItemCategoryCode" });
            DropColumn("EXCEED.ParticipantsItem", "Total");
            DropColumn("EXCEED.ParticipantsItem", "SubTotal");
            DropColumn("EXCEED.ParticipantsItem", "ItemCode");
            DropColumn("EXCEED.ParticipantsItem", "ItemCategoryCode");
            DropColumn("EXCEED.LotItems", "ItemCode");
            DropColumn("EXCEED.LotItems", "ItemCategoryCode");
        }
    }
}
