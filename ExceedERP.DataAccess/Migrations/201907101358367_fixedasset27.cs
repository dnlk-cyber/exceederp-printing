namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset27 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.GroupAssetBalance",
                c => new
                    {
                        GroupAssetBalanceId = c.Int(nullable: false, identity: true),
                        FixedAssetCategoryID = c.Int(nullable: false),
                        FixedAssetSubCategoryID = c.Int(nullable: false),
                        Name = c.String(),
                        Size = c.String(),
                        GroupCode = c.String(),
                        LocationType = c.Int(nullable: false),
                        EmployeeId = c.String(),
                        StoreCode = c.String(),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        BranchId = c.Int(nullable: false),
                        OrgStructureId = c.String(),
                        TransactionType = c.String(),
                        TransactionNo = c.String(),
                        Quanity = c.Int(nullable: false),
                        UnitPrice = c.Int(nullable: false),
                        TotalPrice = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.GroupAssetBalanceId)
                .ForeignKey("EXCEED.FixedAssetCategory", t => t.FixedAssetCategoryID, cascadeDelete: true)
                .ForeignKey("EXCEED.FixedAssetSubCategory", t => t.FixedAssetSubCategoryID, cascadeDelete: true)
                .Index(t => t.FixedAssetCategoryID)
                .Index(t => t.FixedAssetSubCategoryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.GroupAssetBalance", "FixedAssetSubCategoryID", "EXCEED.FixedAssetSubCategory");
            DropForeignKey("EXCEED.GroupAssetBalance", "FixedAssetCategoryID", "EXCEED.FixedAssetCategory");
            DropIndex("EXCEED.GroupAssetBalance", new[] { "FixedAssetSubCategoryID" });
            DropIndex("EXCEED.GroupAssetBalance", new[] { "FixedAssetCategoryID" });
            DropTable("EXCEED.GroupAssetBalance");
        }
    }
}
