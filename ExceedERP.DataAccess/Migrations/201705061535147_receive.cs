namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class receive : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ParticipatingSupplier", "GLTaxRateID", "EXCEED.GLTaxRate");
            DropIndex("EXCEED.ParticipatingSupplier", new[] { "GLTaxRateID" });
            AddColumn("EXCEED.GoodReceiveItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ParticipatingSupplier", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ParticipatingSupplier", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ProformaItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ProformaItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ProformaItem", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.WinningSupplier", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.ParticipatingSupplier", "ItemCategoryCode");
            CreateIndex("EXCEED.ParticipatingSupplier", "ItemCode");
            CreateIndex("EXCEED.ProformaItem", "ItemCategoryCode");
            CreateIndex("EXCEED.ProformaItem", "ItemCode");
            CreateIndex("EXCEED.WinningSupplier", "ItemCategoryCode");
            CreateIndex("EXCEED.WinningSupplier", "ItemCode");
            AddForeignKey("EXCEED.ParticipatingSupplier", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ParticipatingSupplier", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ProformaItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ProformaItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.WinningSupplier", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.WinningSupplier", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.ParticipatingSupplier", "InventoryItem");
            DropColumn("EXCEED.ProformaItem", "InventoryItem");
            DropColumn("EXCEED.ProformaItem", "UOM");
            DropColumn("EXCEED.WinningSupplier", "InventoryItem");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.WinningSupplier", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaItem", "UOM", c => c.String());
            AddColumn("EXCEED.ProformaItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.ParticipatingSupplier", "InventoryItem", c => c.String(nullable: false));
            DropForeignKey("EXCEED.WinningSupplier", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.WinningSupplier", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.ProformaItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ProformaItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.ParticipatingSupplier", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ParticipatingSupplier", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.WinningSupplier", new[] { "ItemCode" });
            DropIndex("EXCEED.WinningSupplier", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.ProformaItem", new[] { "ItemCode" });
            DropIndex("EXCEED.ProformaItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.ParticipatingSupplier", new[] { "ItemCode" });
            DropIndex("EXCEED.ParticipatingSupplier", new[] { "ItemCategoryCode" });
            DropColumn("EXCEED.WinningSupplier", "ItemCode");
            DropColumn("EXCEED.WinningSupplier", "ItemCategoryCode");
            DropColumn("EXCEED.ProformaItem", "GLTaxRateID");
            DropColumn("EXCEED.ProformaItem", "ItemCode");
            DropColumn("EXCEED.ProformaItem", "ItemCategoryCode");
            DropColumn("EXCEED.ParticipatingSupplier", "ItemCode");
            DropColumn("EXCEED.ParticipatingSupplier", "ItemCategoryCode");
            DropColumn("EXCEED.GoodReceiveItem", "Total");
            CreateIndex("EXCEED.ParticipatingSupplier", "GLTaxRateID");
            AddForeignKey("EXCEED.ParticipatingSupplier", "GLTaxRateID", "EXCEED.GLTaxRate", "GLTaxRateID", cascadeDelete: true);
        }
    }
}
