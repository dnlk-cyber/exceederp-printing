namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Construction.PriceAdjustmentWeightItem",
                c => new
                    {
                        PriceAdjustmentWeightItemId = c.Int(nullable: false, identity: true),
                        PriceAdjustmentWeightId = c.Int(nullable: false),
                        ItemName = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceAdjustmentWeightItemId)
                .ForeignKey("Construction.PriceAdjustmentWeight", t => t.PriceAdjustmentWeightId, cascadeDelete: true)
                .Index(t => t.PriceAdjustmentWeightId);
            
            CreateTable(
                "Construction.PriceAdjustmentWeight",
                c => new
                    {
                        PriceAdjustmentWeightId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        WorkCategoryId = c.Int(nullable: false),
                        ContractAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceAdjustmentWeightId)
                .ForeignKey("Construction.WorkCategory", t => t.WorkCategoryId, cascadeDelete: true)
                .Index(t => t.WorkCategoryId);
            
            CreateTable(
                "Construction.ProjectRepresentativeElement",
                c => new
                    {
                        ProjectRepresentativeElementId = c.Int(nullable: false, identity: true),
                        ProjectInformationId = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 128),
                        ItemCategoryCode = c.String(maxLength: 128),
                        Abbreviation = c.String(),
                        Unit = c.String(),
                        PreviousPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ProjectRepresentativeElementId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .Index(t => t.ItemCode)
                .Index(t => t.ItemCategoryCode);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Construction.ProjectRepresentativeElement", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("Construction.ProjectRepresentativeElement", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.PriceAdjustmentWeight", "WorkCategoryId", "Construction.WorkCategory");
            DropForeignKey("Construction.PriceAdjustmentWeightItem", "PriceAdjustmentWeightId", "Construction.PriceAdjustmentWeight");
            DropIndex("Construction.ProjectRepresentativeElement", new[] { "ItemCategoryCode" });
            DropIndex("Construction.ProjectRepresentativeElement", new[] { "ItemCode" });
            DropIndex("Construction.PriceAdjustmentWeight", new[] { "WorkCategoryId" });
            DropIndex("Construction.PriceAdjustmentWeightItem", new[] { "PriceAdjustmentWeightId" });
            DropTable("Construction.ProjectRepresentativeElement");
            DropTable("Construction.PriceAdjustmentWeight");
            DropTable("Construction.PriceAdjustmentWeightItem");
        }
    }
}
