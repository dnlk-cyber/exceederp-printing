namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fuel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FuelItemSetting",
                c => new
                    {
                        FuelItemSettingId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FuelItemSettingId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            AddColumn("EXCEED.IssueFuelItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.IssueFuelItem", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.IssueFuelItem", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.IssueFuelItem", "IssuedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.IssueFuelItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("EXCEED.IssueFuelItem", "ItemCategoryCode");
            CreateIndex("EXCEED.IssueFuelItem", "ItemCode");
            AddForeignKey("EXCEED.IssueFuelItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.IssueFuelItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.IssueFuelItem", "Quantity");
            DropColumn("EXCEED.IssueFuelItem", "IssueGoodReceiveItemID");
            DropColumn("EXCEED.IssueFuelItem", "IssueGoodReceiveItemPrice");
            DropColumn("EXCEED.IssueFuelItem", "UOM");
            DropColumn("EXCEED.IssueFuelValidation", "Approver");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.IssueFuelValidation", "Approver", c => c.String());
            AddColumn("EXCEED.IssueFuelItem", "UOM", c => c.String());
            AddColumn("EXCEED.IssueFuelItem", "IssueGoodReceiveItemPrice", c => c.String());
            AddColumn("EXCEED.IssueFuelItem", "IssueGoodReceiveItemID", c => c.String());
            AddColumn("EXCEED.IssueFuelItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("EXCEED.IssueFuelItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.IssueFuelItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.FuelItemSetting", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.FuelItemSetting", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.IssueFuelItem", new[] { "ItemCode" });
            DropIndex("EXCEED.IssueFuelItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.FuelItemSetting", new[] { "ItemCode" });
            DropIndex("EXCEED.FuelItemSetting", new[] { "ItemCategoryCode" });
            AlterColumn("EXCEED.IssueFuelItem", "ItemCode", c => c.String(nullable: false));
            DropColumn("EXCEED.IssueFuelItem", "IssuedQuantity");
            DropColumn("EXCEED.IssueFuelItem", "ApprovedQuantity");
            DropColumn("EXCEED.IssueFuelItem", "RequestedQuantity");
            DropColumn("EXCEED.IssueFuelItem", "ItemCategoryCode");
            DropTable("EXCEED.FuelItemSetting");
        }
    }
}
