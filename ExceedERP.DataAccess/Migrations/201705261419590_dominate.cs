namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dominate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetIssueItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "ItemCode" });
            AddColumn("EXCEED.FixedAssetIssueItem", "CategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetIssueItem", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetIssueItem", "AssetRegistrationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetIssueItem", "SendToUserCard", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID", c => c.Int());
            AddColumn("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", c => c.Int());
            CreateIndex("EXCEED.FixedAssetIssueItem", "AssetRegistrationId");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.FixedAssetIssueItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID");
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID");
            DropColumn("EXCEED.FixedAssetIssueItem", "ItemCategoryCode");
            DropColumn("EXCEED.FixedAssetIssueItem", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetIssueItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.FixedAssetIssueItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetSubCategory_FixedAssetSubCategoryID" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetCategory_FixedAssetCategoryID" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "AssetRegistrationId" });
            DropColumn("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            DropColumn("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID");
            DropColumn("EXCEED.FixedAssetIssueItem", "SendToUserCard");
            DropColumn("EXCEED.FixedAssetIssueItem", "AssetRegistrationId");
            DropColumn("EXCEED.FixedAssetIssueItem", "SubCategoryCode");
            DropColumn("EXCEED.FixedAssetIssueItem", "CategoryCode");
            CreateIndex("EXCEED.FixedAssetIssueItem", "ItemCode");
            CreateIndex("EXCEED.FixedAssetIssueItem", "ItemCategoryCode");
            AddForeignKey("EXCEED.FixedAssetIssueItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetIssueItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
        }
    }
}
