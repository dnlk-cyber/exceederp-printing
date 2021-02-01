namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class awardsplit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.BidAwardedSupplierItem",
                c => new
                    {
                        BidAwardedSupplierItemId = c.Int(nullable: false, identity: true),
                        BidAwardedSupplierID = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        ItemType = c.Int(nullable: false),
                        Quanity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BidAwardedSupplierItemId)
                .ForeignKey("EXCEED.BidAwardedSupplier", t => t.BidAwardedSupplierID, cascadeDelete: true)
                .Index(t => t.BidAwardedSupplierID);
            
            CreateTable(
                "EXCEED.WinningSupplierItem",
                c => new
                    {
                        WinningSupplierItemId = c.Int(nullable: false, identity: true),
                        WinningSupplierID = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        IsApproved = c.Boolean(nullable: false),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        ItemType = c.Int(nullable: false),
                        Quanity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WinningSupplierItemId)
                .ForeignKey("EXCEED.WinningSupplier", t => t.WinningSupplierID, cascadeDelete: true)
                .Index(t => t.WinningSupplierID);
            
            DropColumn("EXCEED.BidAwardedSupplier", "CatCode");
            DropColumn("EXCEED.BidAwardedSupplier", "CategoryName");
            DropColumn("EXCEED.BidAwardedSupplier", "ItmCode");
            DropColumn("EXCEED.BidAwardedSupplier", "ItemName");
            DropColumn("EXCEED.BidAwardedSupplier", "PartNo");
            DropColumn("EXCEED.BidAwardedSupplier", "IsApproved");
            DropColumn("EXCEED.BidAwardedSupplier", "ItemSpecification");
            DropColumn("EXCEED.BidAwardedSupplier", "UoM");
            DropColumn("EXCEED.BidAwardedSupplier", "ItemType");
            DropColumn("EXCEED.BidAwardedSupplier", "Quanity");
            DropColumn("EXCEED.BidAwardedSupplier", "UPrice");
            DropColumn("EXCEED.BidAwardedSupplier", "GLTaxRateID");
            DropColumn("EXCEED.BidAwardedSupplier", "TaxAmount");
            DropColumn("EXCEED.BidAwardedSupplier", "TPrice");
            DropColumn("EXCEED.WinningSupplier", "CatCode");
            DropColumn("EXCEED.WinningSupplier", "CategoryName");
            DropColumn("EXCEED.WinningSupplier", "ItmCode");
            DropColumn("EXCEED.WinningSupplier", "ItemName");
            DropColumn("EXCEED.WinningSupplier", "PartNo");
            DropColumn("EXCEED.WinningSupplier", "IsApproved");
            DropColumn("EXCEED.WinningSupplier", "ItemSpecification");
            DropColumn("EXCEED.WinningSupplier", "UoM");
            DropColumn("EXCEED.WinningSupplier", "ItemType");
            DropColumn("EXCEED.WinningSupplier", "Quanity");
            DropColumn("EXCEED.WinningSupplier", "UPrice");
            DropColumn("EXCEED.WinningSupplier", "GLTaxRateID");
            DropColumn("EXCEED.WinningSupplier", "TaxAmount");
            DropColumn("EXCEED.WinningSupplier", "TPrice");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.WinningSupplier", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.WinningSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.WinningSupplier", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.WinningSupplier", "Quanity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.WinningSupplier", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "UoM", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "PartNo", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "ItemName", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.WinningSupplier", "CategoryName", c => c.String());
            AddColumn("EXCEED.WinningSupplier", "CatCode", c => c.String(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "Quanity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.BidAwardedSupplier", "ItemType", c => c.Int(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "UoM", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "ItemSpecification", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "PartNo", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "ItemName", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "ItmCode", c => c.String(nullable: false));
            AddColumn("EXCEED.BidAwardedSupplier", "CategoryName", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplier", "CatCode", c => c.String(nullable: false));
            DropForeignKey("EXCEED.WinningSupplierItem", "WinningSupplierID", "EXCEED.WinningSupplier");
            DropForeignKey("EXCEED.BidAwardedSupplierItem", "BidAwardedSupplierID", "EXCEED.BidAwardedSupplier");
            DropIndex("EXCEED.WinningSupplierItem", new[] { "WinningSupplierID" });
            DropIndex("EXCEED.BidAwardedSupplierItem", new[] { "BidAwardedSupplierID" });
            DropTable("EXCEED.WinningSupplierItem");
            DropTable("EXCEED.BidAwardedSupplierItem");
        }
    }
}
