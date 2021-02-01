namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class assettransfer1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "CategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "AssetRegistrationId", c => c.Int(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID", c => c.Int());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID", c => c.Int());
            CreateIndex("EXCEED.MachineryAndVehicleTransfer", "AssetRegistrationId");
            CreateIndex("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID");
            CreateIndex("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.MachineryAndVehicleTransfer", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
            AddForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID");
            AddForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID");
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.MachineryAndVehicleTransfer", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.MachineryAndVehicleTransfer", new[] { "FixedAssetSubCategory_FixedAssetSubCategoryID" });
            DropIndex("EXCEED.MachineryAndVehicleTransfer", new[] { "FixedAssetCategory_FixedAssetCategoryID" });
            DropIndex("EXCEED.MachineryAndVehicleTransfer", new[] { "AssetRegistrationId" });
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "AssetRegistrationId");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "SubCategoryCode");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "CategoryCode");
        }
    }
}
