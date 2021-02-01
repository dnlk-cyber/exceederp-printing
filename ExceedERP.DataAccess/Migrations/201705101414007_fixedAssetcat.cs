namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedAssetcat : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.FixedAssetCategory", "FixedAssetGroupID", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetCategory", "Abbreviation", c => c.String());
            AddColumn("EXCEED.FixedAssetCategory", "FixedAssetClass", c => c.Int(nullable: false));
            AddColumn("EXCEED.FixedAssetSubCategory", "CategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetSubCategory", "Abbreviation", c => c.String());
            AlterColumn("EXCEED.FixedAssetCategory", "DepreciationCategory", c => c.String());
            DropColumn("EXCEED.FixedAssetCategory", "AssetGroup");
            DropColumn("EXCEED.FixedAssetSubCategory", "CategoryName");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetSubCategory", "CategoryName", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetCategory", "AssetGroup", c => c.String(nullable: false));
            AlterColumn("EXCEED.FixedAssetCategory", "DepreciationCategory", c => c.String(nullable: false));
            DropColumn("EXCEED.FixedAssetSubCategory", "Abbreviation");
            DropColumn("EXCEED.FixedAssetSubCategory", "CategoryCode");
            DropColumn("EXCEED.FixedAssetCategory", "FixedAssetClass");
            DropColumn("EXCEED.FixedAssetCategory", "Abbreviation");
            DropColumn("EXCEED.FixedAssetCategory", "FixedAssetGroupID");
        }
    }
}
