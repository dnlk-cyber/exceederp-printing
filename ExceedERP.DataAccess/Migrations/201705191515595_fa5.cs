namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fa5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.IndexingCount", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.IndexingCount", new[] { "ItemCode" });
            AddColumn("EXCEED.IndexingCount", "SubCategoryCode", c => c.String());
            AddColumn("EXCEED.IndexingCount", "FixedAssetSubCategory_FixedAssetSubCategoryID", c => c.Int());
            CreateIndex("EXCEED.IndexingCount", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.IndexingCount", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID");
            DropColumn("EXCEED.IndexingCount", "ItemCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.IndexingCount", "ItemCode", c => c.String(maxLength: 128));
            DropForeignKey("EXCEED.IndexingCount", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropIndex("EXCEED.IndexingCount", new[] { "FixedAssetSubCategory_FixedAssetSubCategoryID" });
            DropColumn("EXCEED.IndexingCount", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            DropColumn("EXCEED.IndexingCount", "SubCategoryCode");
            CreateIndex("EXCEED.IndexingCount", "ItemCode");
            AddForeignKey("EXCEED.IndexingCount", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
        }
    }
}
