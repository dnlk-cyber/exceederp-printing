namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class req : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.StoreRequisitionDistribution", "StoreRequisitionID", "EXCEED.StoreRequisition");
            DropIndex("EXCEED.StoreRequisitionDistribution", new[] { "StoreRequisitionID" });
            AddColumn("EXCEED.InventoryItem", "PartNo", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PlanItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.PlanItem", "ItemCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.StoreRequisitionItem", "ItemCategoryCode", c => c.String(nullable: false, maxLength: 128));
            AddColumn("EXCEED.StoreRequisitionItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "FirstApprovalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisitionItem", "SecondApprovalQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryCode");
            CreateIndex("EXCEED.AnnualNeedAssesmentItem", "ItemCode");
            CreateIndex("EXCEED.PlanItem", "ItemCategoryCode");
            CreateIndex("EXCEED.PlanItem", "ItemCode");
            CreateIndex("EXCEED.StoreRequisitionItem", "ItemCategoryCode");
            AddForeignKey("EXCEED.AnnualNeedAssesmentItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PlanItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PlanItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.StoreRequisitionItem", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode", cascadeDelete: true);
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategory");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItem");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryDesc");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItemDesc");
            DropColumn("EXCEED.PlanItem", "ItemCategory");
            DropColumn("EXCEED.PlanItem", "InventoryItem");
            DropColumn("EXCEED.PlanItem", "ItemCategoryDesc");
            DropColumn("EXCEED.PlanItem", "InventoryItemDesc");
            DropColumn("EXCEED.StoreRequisitionItem", "ApprovedQuantity");
            DropColumn("EXCEED.StoreRequisitionItem", "UOM");
            DropColumn("EXCEED.StoreRequisitionItem", "OrgBranchName");
            DropColumn("EXCEED.StoreRequisitionValidation", "Approver");
            DropTable("EXCEED.StoreRequisitionDistribution");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.StoreRequisitionDistribution",
                c => new
                    {
                        StoreRequisitionDistributionID = c.Int(nullable: false, identity: true),
                        StoreRequisitionID = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.StoreRequisitionDistributionID);
            
            AddColumn("EXCEED.StoreRequisitionValidation", "Approver", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "UOM", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PlanItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.PlanItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.PlanItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.PlanItem", "ItemCategory", c => c.String(nullable: false));
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItemDesc", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryDesc", c => c.String());
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "InventoryItem", c => c.String(nullable: false));
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategory", c => c.String(nullable: false));
            DropForeignKey("EXCEED.StoreRequisitionItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PlanItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.PlanItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.AnnualNeedAssesmentItem", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.StoreRequisitionItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.PlanItem", new[] { "ItemCode" });
            DropIndex("EXCEED.PlanItem", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.AnnualNeedAssesmentItem", new[] { "ItemCode" });
            DropIndex("EXCEED.AnnualNeedAssesmentItem", new[] { "ItemCategoryCode" });
            DropColumn("EXCEED.StoreRequisitionItem", "SecondApprovalQuantity");
            DropColumn("EXCEED.StoreRequisitionItem", "FirstApprovalQuantity");
            DropColumn("EXCEED.StoreRequisitionItem", "InventoryItemDesc");
            DropColumn("EXCEED.StoreRequisitionItem", "ItemCategoryCode");
            DropColumn("EXCEED.PlanItem", "ItemCode");
            DropColumn("EXCEED.PlanItem", "ItemCategoryCode");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCode");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ItemCategoryCode");
            DropColumn("EXCEED.InventoryItem", "PartNo");
            CreateIndex("EXCEED.StoreRequisitionDistribution", "StoreRequisitionID");
            AddForeignKey("EXCEED.StoreRequisitionDistribution", "StoreRequisitionID", "EXCEED.StoreRequisition", "StoreRequisitionID", cascadeDelete: true);
        }
    }
}
