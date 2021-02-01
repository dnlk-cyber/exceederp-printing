namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dashdic : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropIndex("EXCEED.MachineryAndVehicleTransfer", new[] { "FixedAssetCategory_FixedAssetCategoryID" });
            DropIndex("EXCEED.MachineryAndVehicleTransfer", new[] { "FixedAssetSubCategory_FixedAssetSubCategoryID" });
            CreateTable(
                "EXCEED.DashboardDictionary",
                c => new
                    {
                        DashboardDictionaryId = c.Int(nullable: false, identity: true),
                        Category = c.String(),
                        Name = c.String(),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DashboardDictionaryId);
            
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategoryID", c => c.Int(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategoryID", c => c.Int(nullable: false));
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "CategoryCode");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "SubCategoryCode");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID", c => c.Int());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID", c => c.Int());
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.MachineryAndVehicleTransfer", "CategoryCode", c => c.String(nullable: false));
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategoryID");
            DropColumn("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategoryID");
            DropTable("EXCEED.DashboardDictionary");
            CreateIndex("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            CreateIndex("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID");
            AddForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.MachineryAndVehicleTransfer", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID");
        }
    }
}
