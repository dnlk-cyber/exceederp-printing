namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disruptingpurchase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.LotItems", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.LotItems", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ProformaItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.ProformaItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PurchaseOrderItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.PurchaseOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.LotItems", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.LotItems", new[] { "ItemCode" });
            DropIndex("EXCEED.ProformaItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.ProformaItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PurchaseRequisitionItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.PurchaseRequisitionItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PurchaseOrderItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.PurchaseOrderItem", new[] { "ItemCode" });
            AddColumn("EXCEED.LotItems", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.LotItems", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.LotItems", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.LotItems", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.LotItems", "PartNo", c => c.String());
            AddColumn("EXCEED.LotItems", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.LotItems", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.LotItems", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.ProformaItem", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaItem", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaItem", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaItem", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.ProformaItem", "PartNo", c => c.String());
            AddColumn("EXCEED.ProformaItem", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaItem", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.ProformaItem", "UoM", c => c.String());
            AddColumn("EXCEED.ProformaItem", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisition", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionItem", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionItem", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionItem", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionItem", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseRequisitionItem", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "PartNo", c => c.String());
            AddColumn("EXCEED.PurchaseOrderItem", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.PurchaseOrderItem", "UoM", c => c.String());
            AddColumn("EXCEED.PurchaseOrderItem", "ItemType", c => c.Int(nullable: false));
            DropColumn("EXCEED.LotItems", "ItemCategoryCode");
            DropColumn("EXCEED.LotItems", "ItemCode");
            DropColumn("EXCEED.LotItems", "Description");
            DropColumn("EXCEED.ProformaItem", "ItemCategoryCode");
            DropColumn("EXCEED.ProformaItem", "ItemCode");
            DropColumn("EXCEED.ProformaItem", "Description");
            DropColumn("EXCEED.PurchaseRequisition", "IsService");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ItemCode");
            DropColumn("EXCEED.PurchaseOrderItem", "ItemCategoryCode");
            DropColumn("EXCEED.PurchaseOrderItem", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.PurchaseOrderItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseOrderItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseRequisitionItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PurchaseRequisition", "IsService", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ProformaItem", "Description", c => c.String());
            AddColumn("EXCEED.ProformaItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.ProformaItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.LotItems", "Description", c => c.String());
            AddColumn("EXCEED.LotItems", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.LotItems", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            DropColumn("EXCEED.PurchaseOrderItem", "ItemType");
            DropColumn("EXCEED.PurchaseOrderItem", "UoM");
            DropColumn("EXCEED.PurchaseOrderItem", "ItemSpecification");
            DropColumn("EXCEED.PurchaseOrderItem", "IsApproved");
            DropColumn("EXCEED.PurchaseOrderItem", "PartNo");
            DropColumn("EXCEED.PurchaseOrderItem", "ItemName");
            DropColumn("EXCEED.PurchaseOrderItem", "ItmCode");
            DropColumn("EXCEED.PurchaseOrderItem", "CategoryName");
            DropColumn("EXCEED.PurchaseOrderItem", "CatCode");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ItemType");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ItemName");
            DropColumn("EXCEED.PurchaseRequisitionItem", "ItmCode");
            DropColumn("EXCEED.PurchaseRequisitionItem", "CategoryName");
            DropColumn("EXCEED.PurchaseRequisitionItem", "CatCode");
            DropColumn("EXCEED.PurchaseRequisition", "ItemType");
            DropColumn("EXCEED.ProformaItem", "ItemType");
            DropColumn("EXCEED.ProformaItem", "UoM");
            DropColumn("EXCEED.ProformaItem", "ItemSpecification");
            DropColumn("EXCEED.ProformaItem", "IsApproved");
            DropColumn("EXCEED.ProformaItem", "PartNo");
            DropColumn("EXCEED.ProformaItem", "ItemName");
            DropColumn("EXCEED.ProformaItem", "ItmCode");
            DropColumn("EXCEED.ProformaItem", "CategoryName");
            DropColumn("EXCEED.ProformaItem", "CatCode");
            DropColumn("EXCEED.LotItems", "ItemType");
            DropColumn("EXCEED.LotItems", "ItemSpecification");
            DropColumn("EXCEED.LotItems", "IsApproved");
            DropColumn("EXCEED.LotItems", "PartNo");
            DropColumn("EXCEED.LotItems", "ItemName");
            DropColumn("EXCEED.LotItems", "ItmCode");
            DropColumn("EXCEED.LotItems", "CategoryName");
            DropColumn("EXCEED.LotItems", "CatCode");
            CreateIndex("EXCEED.PurchaseOrderItem", "ItemCode");
            CreateIndex("EXCEED.PurchaseOrderItem", "ItemCategoryCode");
            CreateIndex("EXCEED.PurchaseRequisitionItem", "ItemCode");
            CreateIndex("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode");
            CreateIndex("EXCEED.ProformaItem", "ItemCode");
            CreateIndex("EXCEED.ProformaItem", "ItemCategoryCode");
            CreateIndex("EXCEED.LotItems", "ItemCode");
            CreateIndex("EXCEED.LotItems", "ItemCategoryCode");
            AddForeignKey("EXCEED.PurchaseOrderItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseOrderItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseRequisitionItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ProformaItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.ProformaItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.LotItems", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.LotItems", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
    }
}
