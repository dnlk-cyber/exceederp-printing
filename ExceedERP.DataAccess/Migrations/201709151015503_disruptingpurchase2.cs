namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disruptingpurchase2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.RequestedSupplier", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.RequestedSupplier", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.WinningSupplier", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.WinningSupplier", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.RequestedSupplier", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.RequestedSupplier", new[] { "ItemCode" });
            DropIndex("EXCEED.WinningSupplier", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.WinningSupplier", new[] { "ItemCode" });
            AddColumn("EXCEED.RequestedSupplier", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "PartNo", c => c.String());
            AddColumn("EXCEED.RequestedSupplier", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.RequestedSupplier", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.RequestedSupplier", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.RequestedSupplier", "UoM", c => c.String());
            AddColumn("EXCEED.RequestedSupplier", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "PartNo", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "UoM", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "ItemType", c => c.Int(nullable: false));
            DropColumn("EXCEED.RequestedSupplier", "ItemCategoryCode");
            DropColumn("EXCEED.RequestedSupplier", "ItemCode");
            DropColumn("EXCEED.WinningSupplier", "ItemCategoryCode");
            DropColumn("EXCEED.WinningSupplier", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.WinningSupplier", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.WinningSupplier", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.RequestedSupplier", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.RequestedSupplier", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            DropColumn("EXCEED.WinningSupplier", "ItemType");
            DropColumn("EXCEED.WinningSupplier", "UoM");
            DropColumn("EXCEED.WinningSupplier", "ItemSpecification");
            DropColumn("EXCEED.WinningSupplier", "IsApproved");
            DropColumn("EXCEED.WinningSupplier", "PartNo");
            DropColumn("EXCEED.WinningSupplier", "ItemName");
            DropColumn("EXCEED.WinningSupplier", "ItmCode");
            DropColumn("EXCEED.WinningSupplier", "CategoryName");
            DropColumn("EXCEED.WinningSupplier", "CatCode");
            DropColumn("EXCEED.RequestedSupplier", "ItemType");
            DropColumn("EXCEED.RequestedSupplier", "UoM");
            DropColumn("EXCEED.RequestedSupplier", "ItemSpecification");
            DropColumn("EXCEED.RequestedSupplier", "IsApproved");
            DropColumn("EXCEED.RequestedSupplier", "Quantity");
            DropColumn("EXCEED.RequestedSupplier", "PartNo");
            DropColumn("EXCEED.RequestedSupplier", "ItemName");
            DropColumn("EXCEED.RequestedSupplier", "ItmCode");
            DropColumn("EXCEED.RequestedSupplier", "CategoryName");
            DropColumn("EXCEED.RequestedSupplier", "CatCode");
            CreateIndex("EXCEED.WinningSupplier", "ItemCode");
            CreateIndex("EXCEED.WinningSupplier", "ItemCategoryCode");
            CreateIndex("EXCEED.RequestedSupplier", "ItemCode");
            CreateIndex("EXCEED.RequestedSupplier", "ItemCategoryCode");
            AddForeignKey("EXCEED.WinningSupplier", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.WinningSupplier", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.RequestedSupplier", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.RequestedSupplier", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
    }
}
