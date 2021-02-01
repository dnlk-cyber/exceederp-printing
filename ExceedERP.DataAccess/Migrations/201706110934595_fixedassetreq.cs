namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetreq : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "FixedAssetCategory_FixedAssetCategoryID" });
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "FixedAssetSubCategory_FixedAssetSubCategoryID" });
            RenameColumn(table: "EXCEED.FixedAssetRequisitionItem", name: "FixedAssetCategory_FixedAssetCategoryID", newName: "FixedAssetCategoryID");
            RenameColumn(table: "EXCEED.FixedAssetRequisitionItem", name: "FixedAssetSubCategory_FixedAssetSubCategoryID", newName: "FixedAssetSubCategoryID");
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategoryID", c => c.Int(nullable: false));
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategoryID", c => c.Int(nullable: false));
            CreateIndex("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategoryID");
            CreateIndex("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID", cascadeDelete: true);
            AddForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID", cascadeDelete: true);
            DropColumn("EXCEED.FixedAssetRequisitionItem", "CategoryCode");
            DropColumn("EXCEED.FixedAssetRequisitionItem", "SubCategoryCode");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FixedAssetRequisitionItem", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("EXCEED.FixedAssetRequisitionItem", "CategoryCode", c => c.String(nullable: false));
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "FixedAssetSubCategoryID" });
            DropIndex("EXCEED.FixedAssetRequisitionItem", new[] { "FixedAssetCategoryID" });
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategoryID", c => c.Int());
            AlterColumn("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategoryID", c => c.Int());
            RenameColumn(table: "EXCEED.FixedAssetRequisitionItem", name: "FixedAssetSubCategoryID", newName: "FixedAssetSubCategory_FixedAssetSubCategoryID");
            RenameColumn(table: "EXCEED.FixedAssetRequisitionItem", name: "FixedAssetCategoryID", newName: "FixedAssetCategory_FixedAssetCategoryID");
            CreateIndex("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategory_FixedAssetSubCategoryID");
            CreateIndex("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategory_FixedAssetCategoryID");
            AddForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetSubCategory_FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory", "FixedAssetSubCategoryID");
            AddForeignKey("EXCEED.FixedAssetRequisitionItem", "FixedAssetCategory_FixedAssetCategoryID", "EXCEED.FixedAssetCategory", "FixedAssetCategoryID");
        }
    }
}
