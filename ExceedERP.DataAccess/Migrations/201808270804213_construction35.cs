namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class construction35 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Construction.PriceAdjustmentWeightItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectRepresentativeElement", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("Construction.ProjectRepresentativeElement", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropIndex("Construction.PriceAdjustmentWeightItem", new[] { "ItemCode" });
            DropIndex("Construction.ProjectRepresentativeElement", new[] { "ItemCode" });
            DropIndex("Construction.ProjectRepresentativeElement", new[] { "ItemCategoryCode" });
            CreateTable(
                "Construction.RepresentativeElementWeightRange",
                c => new
                    {
                        RepresentativeElementWeightRangeId = c.Int(nullable: false, identity: true),
                        ProjectRepresentativeElementId = c.Int(nullable: false),
                        PercentageMin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PercentageMax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConfigurationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RepresentativeElementWeightRangeId)
                .ForeignKey("Common.Configuration", t => t.ConfigurationId, cascadeDelete: true)
                .ForeignKey("Construction.ProjectRepresentativeElement", t => t.ProjectRepresentativeElementId, cascadeDelete: true)
                .Index(t => t.ProjectRepresentativeElementId)
                .Index(t => t.ConfigurationId);
            
            AddColumn("Construction.ConsReceivableInvoiceDetail", "ProjectPaymentCertificateId", c => c.Int(nullable: false));
            AddColumn("Construction.PriceAdjustmentWeightItem", "ProjectRepresentativeElementId", c => c.Int(nullable: false));
            AddColumn("Construction.PriceAdjustmentWeightItem", "FleetsID", c => c.Guid());
            AddColumn("Construction.PriceAdjustmentWeightItem", "ProjectEstimationIndexLabourId", c => c.Int());
            AddColumn("Construction.PriceAdjustmentWeightItem", "ApprovedAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Construction.ProjectRepresentativeElement", "FleetsID", c => c.Guid());
            AddColumn("Construction.ProjectRepresentativeElement", "ProjectEstimationIndexLabourId", c => c.Int());
            AddColumn("Construction.ProjectRepresentativeElement", "IndexType", c => c.Int(nullable: false));
            AddColumn("Construction.ProjectRepresentativeElement", "IndexCode", c => c.String());
            AddColumn("Construction.ProjectRepresentativeElement", "Description", c => c.String());
            AddColumn("Construction.RepresentativeElementCostInBill", "ProjectRepresentativeElementId", c => c.Int(nullable: false));
            AddColumn("Construction.RepresentativeElementCostInBill", "FleetsID", c => c.Guid());
            AddColumn("Construction.RepresentativeElementCostInBill", "ProjectEstimationIndexLabourId", c => c.Int());
            AddColumn("Construction.RepresentativeElementCostInBill", "BidderProposedWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Construction.PriceAdjustmentWeightItem", "ItemCode", c => c.String());
            AlterColumn("Construction.ProjectRepresentativeElement", "ItemCode", c => c.String());
            CreateIndex("Construction.PriceAdjustmentWeightItem", "ProjectRepresentativeElementId");
            CreateIndex("Construction.RepresentativeElementCostInBill", "ProjectRepresentativeElementId");
            //AddForeignKey("Construction.PriceAdjustmentWeightItem", "ProjectRepresentativeElementId", "Construction.ProjectRepresentativeElement", "ProjectRepresentativeElementId", cascadeDelete: false);
            AddForeignKey("Construction.RepresentativeElementCostInBill", "ProjectRepresentativeElementId", "Construction.ProjectRepresentativeElement", "ProjectRepresentativeElementId", cascadeDelete: false);
            DropColumn("Construction.ProjectRepresentativeElement", "ItemCategoryCode");
            DropColumn("Construction.ProjectRepresentativeElement", "Abbreviation");
        }
        
        public override void Down()
        {
            AddColumn("Construction.ProjectRepresentativeElement", "Abbreviation", c => c.String());
            AddColumn("Construction.ProjectRepresentativeElement", "ItemCategoryCode", c => c.String(maxLength: 128));
            DropForeignKey("Construction.RepresentativeElementWeightRange", "ProjectRepresentativeElementId", "Construction.ProjectRepresentativeElement");
            DropForeignKey("Construction.RepresentativeElementWeightRange", "ConfigurationId", "Common.Configuration");
            DropForeignKey("Construction.RepresentativeElementCostInBill", "ProjectRepresentativeElementId", "Construction.ProjectRepresentativeElement");
            DropForeignKey("Construction.PriceAdjustmentWeightItem", "ProjectRepresentativeElementId", "Construction.ProjectRepresentativeElement");
            DropIndex("Construction.RepresentativeElementWeightRange", new[] { "ConfigurationId" });
            DropIndex("Construction.RepresentativeElementWeightRange", new[] { "ProjectRepresentativeElementId" });
            DropIndex("Construction.RepresentativeElementCostInBill", new[] { "ProjectRepresentativeElementId" });
            DropIndex("Construction.PriceAdjustmentWeightItem", new[] { "ProjectRepresentativeElementId" });
            AlterColumn("Construction.ProjectRepresentativeElement", "ItemCode", c => c.String(maxLength: 128));
            AlterColumn("Construction.PriceAdjustmentWeightItem", "ItemCode", c => c.String(maxLength: 128));
            DropColumn("Construction.RepresentativeElementCostInBill", "BidderProposedWeight");
            DropColumn("Construction.RepresentativeElementCostInBill", "ProjectEstimationIndexLabourId");
            DropColumn("Construction.RepresentativeElementCostInBill", "FleetsID");
            DropColumn("Construction.RepresentativeElementCostInBill", "ProjectRepresentativeElementId");
            DropColumn("Construction.ProjectRepresentativeElement", "Description");
            DropColumn("Construction.ProjectRepresentativeElement", "IndexCode");
            DropColumn("Construction.ProjectRepresentativeElement", "IndexType");
            DropColumn("Construction.ProjectRepresentativeElement", "ProjectEstimationIndexLabourId");
            DropColumn("Construction.ProjectRepresentativeElement", "FleetsID");
            DropColumn("Construction.PriceAdjustmentWeightItem", "ApprovedAmount");
            DropColumn("Construction.PriceAdjustmentWeightItem", "ProjectEstimationIndexLabourId");
            DropColumn("Construction.PriceAdjustmentWeightItem", "FleetsID");
            DropColumn("Construction.PriceAdjustmentWeightItem", "ProjectRepresentativeElementId");
            DropColumn("Construction.ConsReceivableInvoiceDetail", "ProjectPaymentCertificateId");
            DropTable("Construction.RepresentativeElementWeightRange");
            CreateIndex("Construction.ProjectRepresentativeElement", "ItemCategoryCode");
            CreateIndex("Construction.ProjectRepresentativeElement", "ItemCode");
            CreateIndex("Construction.PriceAdjustmentWeightItem", "ItemCode");
            AddForeignKey("Construction.ProjectRepresentativeElement", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
            AddForeignKey("Construction.ProjectRepresentativeElement", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
            AddForeignKey("Construction.PriceAdjustmentWeightItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
        }
    }
}
