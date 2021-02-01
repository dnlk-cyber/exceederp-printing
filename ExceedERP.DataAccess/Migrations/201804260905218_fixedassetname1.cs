namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedassetname1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.FixedAssetSubCategoryName",
                c => new
                    {
                        FixedAssetSubCategoryNameId = c.Int(nullable: false, identity: true),
                        FixedAssetSubCategoryID = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FixedAssetSubCategoryNameId)
                .ForeignKey("EXCEED.FixedAssetSubCategory", t => t.FixedAssetSubCategoryID, cascadeDelete: true)
                .Index(t => t.FixedAssetSubCategoryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.FixedAssetSubCategoryName", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropIndex("EXCEED.FixedAssetSubCategoryName", new[] { "FixedAssetSubCategoryID" });
            DropTable("EXCEED.FixedAssetSubCategoryName");
        }
    }
}
