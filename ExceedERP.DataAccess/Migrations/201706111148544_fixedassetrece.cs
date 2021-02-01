namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetrece : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetCategoryID", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetSubCategoryID", c => c.Int(nullable: false));
            CreateIndex("EXCEED.FixedAssetReceiveItem", "FixedAssetCategoryID");
            CreateIndex("EXCEED.FixedAssetReceiveItem", "FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.FixedAssetReceiveItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetReceiveItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID", cascadeDelete: true);
            DropColumn("EXCEED.FixedAssetReceiveItem", "CategoryCode");
            DropColumn("EXCEED.FixedAssetReceiveItem", "SubCategoryCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetReceiveItem", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetReceiveItem", "CategoryCode", c => c.String(nullable: false));
            DropForeignKey("EXCEED.FixedAssetReceiveItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.FixedAssetReceiveItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropIndex("EXCEED.FixedAssetReceiveItem", new[] { "FixedAssetSubCategoryID" });
            DropIndex("EXCEED.FixedAssetReceiveItem", new[] { "FixedAssetCategoryID" });
            DropColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetSubCategoryID");
            DropColumn("EXCEED.FixedAssetReceiveItem", "FixedAssetCategoryID");
        }
    }
}
