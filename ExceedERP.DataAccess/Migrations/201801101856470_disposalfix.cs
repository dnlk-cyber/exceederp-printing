namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class disposalfix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.DisposalItem", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.DisposalItem", new[] { "AssetRegistrationId" });
            AddColumn("EXCEED.AssetRegistration", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.DisposalItem", "FixedAssetCategoryID", c => c.Int(nullable: false));
            AddColumn("EXCEED.DisposalItem", "FixedAssetSubCategoryID", c => c.Int(nullable: false));
            CreateIndex("EXCEED.DisposalItem", "FixedAssetCategoryID");
            CreateIndex("EXCEED.DisposalItem", "FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.DisposalItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID", cascadeDelete: true);
            AddForeignKey("EXCEED.DisposalItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.DisposalItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.DisposalItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropIndex("EXCEED.DisposalItem", new[] { "FixedAssetSubCategoryID" });
            DropIndex("EXCEED.DisposalItem", new[] { "FixedAssetCategoryID" });
            DropColumn("EXCEED.DisposalItem", "FixedAssetSubCategoryID");
            DropColumn("EXCEED.DisposalItem", "FixedAssetCategoryID");
            DropColumn("EXCEED.AssetRegistration", "BranchId");
            CreateIndex("EXCEED.DisposalItem", "AssetRegistrationId");
            AddForeignKey("EXCEED.DisposalItem", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}
