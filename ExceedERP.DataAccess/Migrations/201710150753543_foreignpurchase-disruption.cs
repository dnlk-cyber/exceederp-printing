namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignpurchasedisruption : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.PurchaseFollowUpServiceItem", "PurchaseFollowUpID", "EXCEED.PurchaseFollowUp");
            DropForeignKey("EXCEED.PurchaseFollowUpServiceItem", "ServiceCategoryCode", "EXCEED.ServiceCategory");
            DropForeignKey("EXCEED.PurchaseFollowUpServiceItem", "ServiceItemCode", "EXCEED.ServiceItem");
            DropIndex("EXCEED.PurchaseFollowUpServiceItem", new[] { "PurchaseFollowUpID" });
            DropIndex("EXCEED.PurchaseFollowUpServiceItem", new[] { "ServiceCategoryCode" });
            DropIndex("EXCEED.PurchaseFollowUpServiceItem", new[] { "ServiceItemCode" });
            CreateTable(
                "EXCEED.ForeignPurchaseFollowUpItems",
                c => new
                    {
                        ForeignPurchaseFollowUpItemsID = c.Int(nullable: false, identity: true),
                        ForeignPurchaseId = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        ItemType = c.Int(nullable: false),
                        PartNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseFollowUpItemsID)
                .ForeignKey("EXCEED.ForeignPurchase", t => t.ForeignPurchaseId, cascadeDelete: true)
                .Index(t => t.ForeignPurchaseId);
            
            CreateTable(
                "EXCEED.ForeignPurchaseFollowUpStatus",
                c => new
                    {
                        ForeignPurchaseFollowUpStatusId = c.Int(nullable: false, identity: true),
                        ForeignPurchaseId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseFollowUpStatusId)
                .ForeignKey("EXCEED.ForeignPurchase", t => t.ForeignPurchaseId, cascadeDelete: true)
                .Index(t => t.ForeignPurchaseId);
            
            CreateTable(
                "EXCEED.ForeignPurchaseFollowUpSystemStatus",
                c => new
                    {
                        ForeignPurchaseFollowUpSystemStatusId = c.Int(nullable: false, identity: true),
                        ForeignPurchaseId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Description = c.String(),
                        User = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseFollowUpSystemStatusId)
                .ForeignKey("EXCEED.ForeignPurchase", t => t.ForeignPurchaseId, cascadeDelete: true)
                .Index(t => t.ForeignPurchaseId);
            
            CreateTable(
                "EXCEED.ForeignPurchaseCostType",
                c => new
                    {
                        ForeignPurchaseCostTypeId = c.Int(nullable: false, identity: true),
                        CostName = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseCostTypeId);
            
            AddColumn("EXCEED.ForeignPurchaseCost", "ForeignPurchaseId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "Date", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.ForeignPurchaseCost", "CostType", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseCost", "Currency", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "ExchangeRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseCost", "Remark", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "OrgBranchName", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "DateCreated", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchaseCost", "LastModified", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchaseCost", "CreatedBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "ModifiedBy", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "TenderRef", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "Supplier", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "ItemDesc", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "ProcurementMethod", c => c.Int(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "ApprovalDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "OpenigDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "TechnicalDocSendDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "FinancialOpenigDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "MinuteApprovedDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "AwardDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "ContractDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "PaymentMethod", c => c.Int(nullable: false));
            AddColumn("EXCEED.ForeignPurchase", "PaymentOpenDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "PaymentExtensionDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "DocReceivedFromBankDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "ClearanceStartDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "TransitProcessDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "DeliveryDate", c => c.DateTime());
            AddColumn("EXCEED.ForeignPurchase", "DeliveryNo", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchase", "Remark", c => c.String());
            CreateIndex("EXCEED.ForeignPurchaseCost", "ForeignPurchaseId");
            AddForeignKey("EXCEED.ForeignPurchaseCost", "ForeignPurchaseId", "EXCEED.ForeignPurchase", "ForeignPurchaseId", cascadeDelete: true);
            DropColumn("EXCEED.ForeignPurchaseCost", "ForeignCost");
            DropColumn("EXCEED.ForeignPurchaseCost", "CostValue");
            DropColumn("EXCEED.ForeignPurchase", "ItemCode");
            DropColumn("EXCEED.ForeignPurchase", "Unit");
            DropColumn("EXCEED.ForeignPurchase", "Amount");
            DropColumn("EXCEED.ForeignPurchase", "LcReference");
            DropColumn("EXCEED.ForeignPurchase", "InsurancePolicy");
            DropColumn("EXCEED.ForeignPurchase", "TransitOperation");
            DropColumn("EXCEED.ForeignPurchase", "Status");
            DropColumn("EXCEED.ForeignPurchase", "CNo");
            DropColumn("EXCEED.ForeignPurchase", "TotalCost");
            DropColumn("EXCEED.ForeignPurchase", "GrnNo");
            DropTable("EXCEED.ForeignPurchaseStatus");
            DropTable("EXCEED.PurchaseFollowUpServiceItem");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.PurchaseFollowUpServiceItem",
                c => new
                    {
                        PurchaseFollowUpServiceItemID = c.Int(nullable: false, identity: true),
                        PurchaseFollowUpID = c.Int(nullable: false),
                        ServiceCategoryCode = c.String(nullable: false, maxLength: 128),
                        ServiceItemCode = c.String(nullable: false, maxLength: 128),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemSpecification = c.String(),
                        UoM = c.String(),
                        Supplier = c.String(),
                        PurchaserOrder = c.String(),
                        FCAppliedDate = c.DateTime(),
                        FCPermitDate = c.DateTime(),
                        FCPermitNo = c.String(),
                        FCPermitExpiryDate = c.DateTime(),
                        LCAppliedDate = c.DateTime(),
                        LCNo = c.String(),
                        LCOpenedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PurchaseFollowUpServiceItemID);
            
            CreateTable(
                "EXCEED.ForeignPurchaseStatus",
                c => new
                    {
                        ForeignPurchaseStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Date = c.DateTime(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ForeignPurchaseStatusId);
            
            AddColumn("EXCEED.ForeignPurchase", "GrnNo", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "TotalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchase", "CNo", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "Status", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "TransitOperation", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "InsurancePolicy", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "LcReference", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchase", "Unit", c => c.String());
            AddColumn("EXCEED.ForeignPurchase", "ItemCode", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseCost", "CostValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ForeignPurchaseCost", "ForeignCost", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.ForeignPurchaseFollowUpSystemStatus", "ForeignPurchaseId", "EXCEED.ForeignPurchase");
            DropForeignKey("EXCEED.ForeignPurchaseFollowUpStatus", "ForeignPurchaseId", "EXCEED.ForeignPurchase");
            DropForeignKey("EXCEED.ForeignPurchaseFollowUpItems", "ForeignPurchaseId", "EXCEED.ForeignPurchase");
            DropForeignKey("EXCEED.ForeignPurchaseCost", "ForeignPurchaseId", "EXCEED.ForeignPurchase");
            DropIndex("EXCEED.ForeignPurchaseFollowUpSystemStatus", new[] { "ForeignPurchaseId" });
            DropIndex("EXCEED.ForeignPurchaseFollowUpStatus", new[] { "ForeignPurchaseId" });
            DropIndex("EXCEED.ForeignPurchaseFollowUpItems", new[] { "ForeignPurchaseId" });
            DropIndex("EXCEED.ForeignPurchaseCost", new[] { "ForeignPurchaseId" });
            DropColumn("EXCEED.ForeignPurchase", "Remark");
            DropColumn("EXCEED.ForeignPurchase", "UnitPrice");
            DropColumn("EXCEED.ForeignPurchase", "DeliveryNo");
            DropColumn("EXCEED.ForeignPurchase", "DeliveryDate");
            DropColumn("EXCEED.ForeignPurchase", "TransitProcessDate");
            DropColumn("EXCEED.ForeignPurchase", "ClearanceStartDate");
            DropColumn("EXCEED.ForeignPurchase", "DocReceivedFromBankDate");
            DropColumn("EXCEED.ForeignPurchase", "PaymentExtensionDate");
            DropColumn("EXCEED.ForeignPurchase", "PaymentOpenDate");
            DropColumn("EXCEED.ForeignPurchase", "PaymentMethod");
            DropColumn("EXCEED.ForeignPurchase", "ContractDate");
            DropColumn("EXCEED.ForeignPurchase", "AwardDate");
            DropColumn("EXCEED.ForeignPurchase", "MinuteApprovedDate");
            DropColumn("EXCEED.ForeignPurchase", "FinancialOpenigDate");
            DropColumn("EXCEED.ForeignPurchase", "TechnicalDocSendDate");
            DropColumn("EXCEED.ForeignPurchase", "OpenigDate");
            DropColumn("EXCEED.ForeignPurchase", "ApprovalDate");
            DropColumn("EXCEED.ForeignPurchase", "ProcurementMethod");
            DropColumn("EXCEED.ForeignPurchase", "OrgStructureId");
            DropColumn("EXCEED.ForeignPurchase", "ItemDesc");
            DropColumn("EXCEED.ForeignPurchase", "Supplier");
            DropColumn("EXCEED.ForeignPurchase", "TenderRef");
            DropColumn("EXCEED.ForeignPurchaseCost", "ModifiedBy");
            DropColumn("EXCEED.ForeignPurchaseCost", "CreatedBy");
            DropColumn("EXCEED.ForeignPurchaseCost", "LastModified");
            DropColumn("EXCEED.ForeignPurchaseCost", "DateCreated");
            DropColumn("EXCEED.ForeignPurchaseCost", "OrgBranchName");
            DropColumn("EXCEED.ForeignPurchaseCost", "Remark");
            DropColumn("EXCEED.ForeignPurchaseCost", "ExchangeRate");
            DropColumn("EXCEED.ForeignPurchaseCost", "Currency");
            DropColumn("EXCEED.ForeignPurchaseCost", "Amount");
            DropColumn("EXCEED.ForeignPurchaseCost", "CostType");
            DropColumn("EXCEED.ForeignPurchaseCost", "Date");
            DropColumn("EXCEED.ForeignPurchaseCost", "ForeignPurchaseId");
            DropTable("EXCEED.ForeignPurchaseCostType");
            DropTable("EXCEED.ForeignPurchaseFollowUpSystemStatus");
            DropTable("EXCEED.ForeignPurchaseFollowUpStatus");
            DropTable("EXCEED.ForeignPurchaseFollowUpItems");
            CreateIndex("EXCEED.PurchaseFollowUpServiceItem", "ServiceItemCode");
            CreateIndex("EXCEED.PurchaseFollowUpServiceItem", "ServiceCategoryCode");
            CreateIndex("EXCEED.PurchaseFollowUpServiceItem", "PurchaseFollowUpID");
            AddForeignKey("EXCEED.PurchaseFollowUpServiceItem", "ServiceItemCode", "EXCEED.ServiceItem", "ServiceItemCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseFollowUpServiceItem", "ServiceCategoryCode", "EXCEED.ServiceCategory", "ServiceCategoryCode", cascadeDelete: true);
            AddForeignKey("EXCEED.PurchaseFollowUpServiceItem", "PurchaseFollowUpID", "EXCEED.PurchaseFollowUp", "PurchaseFollowUpID", cascadeDelete: true);
        }
    }
}
