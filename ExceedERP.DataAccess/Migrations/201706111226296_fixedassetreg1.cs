namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetreg1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetCategory_FixedAssetCategoryID" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetSubCategory_FixedAssetSubCategoryID" });
            RenameColumn(table: "EXCEED.FixedAssetIssueItem", name: "FixedAssetCategory_FixedAssetCategoryID", newName: "FixedAssetCategoryID");
            RenameColumn(table: "EXCEED.FixedAssetIssueItem", name: "FixedAssetSubCategory_FixedAssetSubCategoryID", newName: "FixedAssetSubCategoryID");
            AddColumn("EXCEED.AssetRegistration", "FixedAssetCategoryID", c => c.Int(nullable: false));
            AddColumn("EXCEED.AssetRegistration", "FixedAssetSubCategoryID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID", c => c.Int(nullable: false));
            CreateIndex("EXCEED.AssetRegistration", "FixedAssetCategoryID");
            CreateIndex("EXCEED.AssetRegistration", "FixedAssetSubCategoryID");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.AssetRegistration", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID", cascadeDelete: true);
            AddForeignKey("EXCEED.AssetRegistration", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID", cascadeDelete: true);
            DropColumn("EXCEED.AssetRegistration", "CategoryCode");
            DropColumn("EXCEED.AssetRegistration", "SubCategoryCode");
            DropColumn("EXCEED.FixedAssetIssueItem", "CategoryCode");
            DropColumn("EXCEED.FixedAssetIssueItem", "SubCategoryCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetIssueItem", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetIssueItem", "CategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.AssetRegistration", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.AssetRegistration", "CategoryCode", c => c.String(nullable: false));
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.AssetRegistration", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.AssetRegistration", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetSubCategoryID" });
            DropIndex("EXCEED.FixedAssetIssueItem", new[] { "FixedAssetCategoryID" });
            DropIndex("EXCEED.AssetRegistration", new[] { "FixedAssetSubCategoryID" });
            DropIndex("EXCEED.AssetRegistration", new[] { "FixedAssetCategoryID" });
            AlterColumn("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategoryID", c => c.Int());
            AlterColumn("EXCEED.FixedAssetIssueItem", "FixedAssetCategoryID", c => c.Int());
            DropColumn("EXCEED.AssetRegistration", "FixedAssetSubCategoryID");
            DropColumn("EXCEED.AssetRegistration", "FixedAssetCategoryID");
            RenameColumn(table: "EXCEED.FixedAssetIssueItem", name: "FixedAssetSubCategoryID", newName: "FixedAssetSubCategory_FixedAssetSubCategoryID");
            RenameColumn(table: "EXCEED.FixedAssetIssueItem", name: "FixedAssetCategoryID", newName: "FixedAssetCategory_FixedAssetCategoryID");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            CreateIndex("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID");
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.FixedAssetIssueItem", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID");
        }
    }
}
