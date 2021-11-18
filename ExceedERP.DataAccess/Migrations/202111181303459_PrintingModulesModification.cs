namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrintingModulesModification : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "JobCosting.Units", newName: "Unit");
            MoveTable(name: "JobCosting.JobCategories", newSchema: "ProductFollowUp");
            MoveTable(name: "EXCEED.PFProformaItems", newSchema: "ProductFollowUp");
            MoveTable(name: "JobCosting.Unit", newSchema: "EXCEED");
            CreateTable(
                "ProductFollowUp.FinishedGoodsStoreReceiveDistribution",
                c => new
                    {
                        FinishedGoodsStoreReceiveDistributionId = c.Int(nullable: false, identity: true),
                        FinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        ReceiveItemIDTrackBack = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        AdditionalDescription = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodsStoreReceiveDistributionId)
                .ForeignKey("ProductFollowUp.FinishedGoodsStoreReceive", t => t.FinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.FinishedGoodsStoreReceiveId);
            
            CreateTable(
                "ProductFollowUp.FinishedGoodsStoreReceive",
                c => new
                    {
                        FinishedGoodsStoreReceiveId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        VoucherNo = c.String(),
                        StoreCode = c.String(),
                        JobId = c.Int(nullable: false),
                        ReceiveDate = c.DateTime(nullable: false),
                        ProductionPeriodId = c.Int(nullable: false),
                        DeliveredBy = c.String(),
                        ReceivedBy = c.String(),
                        Supervisor = c.String(),
                        IsPostedToSecondLedger = c.Boolean(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodsStoreReceiveId);
            
            CreateTable(
                "ProductFollowUp.FinishedGoodsStoreReceiveItem",
                c => new
                    {
                        FinishedGoodsStoreReceiveItemId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(maxLength: 128),
                        ItemCode = c.String(maxLength: 128),
                        FinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UOM = c.String(),
                        AccountCode = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodsStoreReceiveItemId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode)
                .ForeignKey("ProductFollowUp.FinishedGoodsStoreReceive", t => t.FinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode)
                .Index(t => t.FinishedGoodsStoreReceiveId);
            
            CreateTable(
                "ProductFollowUp.FinishedGoodsStoreReceiveValidation",
                c => new
                    {
                        FinishedGoodsStoreReceiveValidationId = c.Int(nullable: false, identity: true),
                        FinishedGoodsStoreReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodsStoreReceiveValidationId)
                .ForeignKey("ProductFollowUp.FinishedGoodsStoreReceive", t => t.FinishedGoodsStoreReceiveId, cascadeDelete: true)
                .Index(t => t.FinishedGoodsStoreReceiveId);
            
            AddColumn("JobCosting.DailyLaborRates", "CostCenter", c => c.String());
            AddColumn("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId", c => c.Int());
            AddColumn("ProductFollowUp.JobCategories", "GLTaxRateID", c => c.Int(nullable: false));
            AddColumn("JobCosting.JobLaborCosts", "CostCenter", c => c.String());
            AddColumn("JobCosting.JobMaterialCosts", "CostCenter", c => c.String());
            AddColumn("JobCosting.JobMaterialReturns", "CostCenter", c => c.String());
            AddColumn("ProductFollowUp.Job", "BrandName", c => c.String());
            AddColumn("ProductFollowUp.Job", "TinNo", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "BrandName", c => c.String(maxLength: 50));
            AddColumn("ProductFollowUp.Proforma", "TinNo", c => c.String());
            AddColumn("JobCosting.YearlyOverHeadRates", "Values", c => c.String());
            CreateIndex("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId");
            AddForeignKey("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId", "HRM.EmployeeTypes", "EmployeeTypeId");
            DropColumn("JobCosting.DailyLaborRates", "CostCenterId");
            DropColumn("JobCosting.DailyLaborRates", "EmployeeType");
            DropColumn("JobCosting.JobLaborCosts", "CostCenterId");
            DropColumn("JobCosting.JobMaterialCosts", "CostCenterId");
            DropColumn("JobCosting.JobMaterialReturns", "CostCenterId");
            DropColumn("ProductFollowUp.Job", "ContactName");
            DropColumn("ProductFollowUp.Job", "ContactPhone");
            DropColumn("ProductFollowUp.Job", "City");
            DropColumn("ProductFollowUp.Job", "SubCity");
            DropColumn("ProductFollowUp.Job", "Woreda");
            DropColumn("ProductFollowUp.Job", "CustomerVAT");
            DropColumn("ProductFollowUp.Job", "CustomerMargin");
            DropColumn("ProductFollowUp.Job", "CustomerOverHead");
            DropColumn("ProductFollowUp.Job", "POBox");
            DropColumn("ProductFollowUp.Job", "CustomerDescription");
            DropColumn("ProductFollowUp.Job", "OrderType");
            DropColumn("ProductFollowUp.Job", "JobTypeItemId");
            DropColumn("ProductFollowUp.Proforma", "ContactName");
            DropColumn("ProductFollowUp.Proforma", "ContactPhone");
            DropColumn("ProductFollowUp.Proforma", "City");
            DropColumn("ProductFollowUp.Proforma", "SubCity");
            DropColumn("ProductFollowUp.Proforma", "Woreda");
            DropColumn("ProductFollowUp.Proforma", "POBox");
            DropColumn("ProductFollowUp.Proforma", "CustomerVAT");
            DropColumn("ProductFollowUp.Proforma", "CustomerMargin");
            DropColumn("ProductFollowUp.Proforma", "CustomerOverHead");
            DropColumn("ProductFollowUp.Proforma", "CustomerDescription");
            DropColumn("JobCosting.YearlyOverHeadRates", "CostCenterID");
            DropTable("EXCEED.CostCenter");
            DropTable("ProductFollowUp.FinishedGoodReceiving");
            DropTable("EXCEED.JobTypeItem");
            DropTable("EXCEED.JobType");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.JobType",
                c => new
                    {
                        JobTypeId = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        GLTaxRateID = c.Int(nullable: false),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Margin = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.JobTypeId);
            
            CreateTable(
                "EXCEED.JobTypeItem",
                c => new
                    {
                        JobTypeItemId = c.Int(nullable: false, identity: true),
                        JobTypeId = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.JobTypeItemId);
            
            CreateTable(
                "ProductFollowUp.FinishedGoodReceiving",
                c => new
                    {
                        FinishedGoodReceivingId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        VoucherNo = c.String(),
                        Store = c.String(),
                        JobId = c.Int(nullable: false),
                        DeliveredBy = c.String(),
                        ReceivedBy = c.String(),
                        Supervisor = c.String(),
                    })
                .PrimaryKey(t => t.FinishedGoodReceivingId);
            
            CreateTable(
                "EXCEED.CostCenter",
                c => new
                    {
                        CostCenterId = c.Int(nullable: false, identity: true),
                        CostCenterName = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.CostCenterId);
            
            AddColumn("JobCosting.YearlyOverHeadRates", "CostCenterID", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Proforma", "CustomerDescription", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "CustomerOverHead", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Proforma", "CustomerMargin", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Proforma", "CustomerVAT", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Proforma", "POBox", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "Woreda", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "SubCity", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "City", c => c.String());
            AddColumn("ProductFollowUp.Proforma", "ContactPhone", c => c.String(maxLength: 50));
            AddColumn("ProductFollowUp.Proforma", "ContactName", c => c.String(maxLength: 50));
            AddColumn("ProductFollowUp.Job", "JobTypeItemId", c => c.Int());
            AddColumn("ProductFollowUp.Job", "OrderType", c => c.Int(nullable: false));
            AddColumn("ProductFollowUp.Job", "CustomerDescription", c => c.String());
            AddColumn("ProductFollowUp.Job", "POBox", c => c.String());
            AddColumn("ProductFollowUp.Job", "CustomerOverHead", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Job", "CustomerMargin", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Job", "CustomerVAT", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("ProductFollowUp.Job", "Woreda", c => c.String());
            AddColumn("ProductFollowUp.Job", "SubCity", c => c.String());
            AddColumn("ProductFollowUp.Job", "City", c => c.String());
            AddColumn("ProductFollowUp.Job", "ContactPhone", c => c.String());
            AddColumn("ProductFollowUp.Job", "ContactName", c => c.String());
            AddColumn("JobCosting.JobMaterialReturns", "CostCenterId", c => c.Int(nullable: false));
            AddColumn("JobCosting.JobMaterialCosts", "CostCenterId", c => c.Int(nullable: false));
            AddColumn("JobCosting.JobLaborCosts", "CostCenterId", c => c.Int(nullable: false));
            AddColumn("JobCosting.DailyLaborRates", "EmployeeType", c => c.Int(nullable: false));
            AddColumn("JobCosting.DailyLaborRates", "CostCenterId", c => c.Int(nullable: false));
            DropForeignKey("ProductFollowUp.FinishedGoodsStoreReceiveValidation", "FinishedGoodsStoreReceiveId", "ProductFollowUp.FinishedGoodsStoreReceive");
            DropForeignKey("ProductFollowUp.FinishedGoodsStoreReceiveItem", "FinishedGoodsStoreReceiveId", "ProductFollowUp.FinishedGoodsStoreReceive");
            DropForeignKey("ProductFollowUp.FinishedGoodsStoreReceiveItem", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("ProductFollowUp.FinishedGoodsStoreReceiveItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("ProductFollowUp.FinishedGoodsStoreReceiveDistribution", "FinishedGoodsStoreReceiveId", "ProductFollowUp.FinishedGoodsStoreReceive");
            DropForeignKey("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId", "HRM.EmployeeTypes");
            DropIndex("ProductFollowUp.FinishedGoodsStoreReceiveValidation", new[] { "FinishedGoodsStoreReceiveId" });
            DropIndex("ProductFollowUp.FinishedGoodsStoreReceiveItem", new[] { "FinishedGoodsStoreReceiveId" });
            DropIndex("ProductFollowUp.FinishedGoodsStoreReceiveItem", new[] { "ItemCode" });
            DropIndex("ProductFollowUp.FinishedGoodsStoreReceiveItem", new[] { "ItemCategoryCode" });
            DropIndex("ProductFollowUp.FinishedGoodsStoreReceiveDistribution", new[] { "FinishedGoodsStoreReceiveId" });
            DropIndex("JobCosting.DailyLaborRates", new[] { "EmployeeType_EmployeeTypeId" });
            DropColumn("JobCosting.YearlyOverHeadRates", "Values");
            DropColumn("ProductFollowUp.Proforma", "TinNo");
            DropColumn("ProductFollowUp.Proforma", "BrandName");
            DropColumn("ProductFollowUp.Job", "TinNo");
            DropColumn("ProductFollowUp.Job", "BrandName");
            DropColumn("JobCosting.JobMaterialReturns", "CostCenter");
            DropColumn("JobCosting.JobMaterialCosts", "CostCenter");
            DropColumn("JobCosting.JobLaborCosts", "CostCenter");
            DropColumn("ProductFollowUp.JobCategories", "GLTaxRateID");
            DropColumn("JobCosting.DailyLaborRates", "EmployeeType_EmployeeTypeId");
            DropColumn("JobCosting.DailyLaborRates", "CostCenter");
            DropTable("ProductFollowUp.FinishedGoodsStoreReceiveValidation");
            DropTable("ProductFollowUp.FinishedGoodsStoreReceiveItem");
            DropTable("ProductFollowUp.FinishedGoodsStoreReceive");
            DropTable("ProductFollowUp.FinishedGoodsStoreReceiveDistribution");
            MoveTable(name: "EXCEED.Unit", newSchema: "JobCosting");
            MoveTable(name: "ProductFollowUp.PFProformaItems", newSchema: "EXCEED");
            MoveTable(name: "ProductFollowUp.JobCategories", newSchema: "JobCosting");
            RenameTable(name: "JobCosting.Unit", newName: "Units");
        }
    }
}
