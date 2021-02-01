namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class proforma6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ParticipatingSupplierItem",
                c => new
                    {
                        ParticipatingSupplierItemId = c.Int(nullable: false, identity: true),
                        ParticipatingSupplierID = c.Int(nullable: false),
                        CatCode = c.String(),
                        CategoryName = c.String(),
                        ItmCode = c.String(),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        Name = c.String(),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        ItemType = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        IsOutOfStock = c.Boolean(nullable: false),
                        Remark = c.String(),
                        ProformaItemID = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ParticipatingSupplierItemId)
                .ForeignKey("EXCEED.ParticipatingSupplier", t => t.ParticipatingSupplierID, cascadeDelete: true)
                .Index(t => t.ParticipatingSupplierID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ParticipatingSupplierItem", "ParticipatingSupplierID", "EXCEED.ParticipatingSupplier");
            DropIndex("EXCEED.ParticipatingSupplierItem", new[] { "ParticipatingSupplierID" });
            DropTable("EXCEED.ParticipatingSupplierItem");
        }
    }
}
