namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class upgradeprocinfinance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.ApTravelExpenseDistribution", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropIndex("EXCEED.ApTravelExpenseDistribution", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelOrderDistribution", new[] { "ApTravelOrderId" });
            CreateTable(
                "EXCEED.ApPaymentSettlementDepartmentValidation",
                c => new
                    {
                        ApPaymentSettlementDepartmentValidationId = c.Int(nullable: false, identity: true),
                        ApPaymentSettlementId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ApPaymentSettlementDepartmentValidationId)
                .ForeignKey("EXCEED.ApPaymentSettlement", t => t.ApPaymentSettlementId, cascadeDelete: true)
                .Index(t => t.ApPaymentSettlementId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseClaimAttachment",
                c => new
                    {
                        ApTravelExpenseClaimAttachmentId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelExpenseClaimAttachmentId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelOrdertAttachment",
                c => new
                    {
                        ApTravelOrdertAttachmentId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                    })
                .PrimaryKey(t => t.ApTravelOrdertAttachmentId)
                .ForeignKey("EXCEED.ApTravelOrder", t => t.ApTravelOrderId, cascadeDelete: true)
                .Index(t => t.ApTravelOrderId);
            
            CreateTable(
                "EXCEED.PoAdditionalCharge",
                c => new
                    {
                        PoAdditionalChargeId = c.Int(nullable: false, identity: true),
                        PurchaseOrderID = c.Int(nullable: false),
                        AmountType = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PoAdditionalChargeId)
                .ForeignKey("EXCEED.PurchaseOrder", t => t.PurchaseOrderID, cascadeDelete: true)
                .Index(t => t.PurchaseOrderID);
            
            AddColumn("EXCEED.ApDirectPayment", "IsSettled", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "BranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "IsDepartmentOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "DepartmentOnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ApPaymentSettlement", "DepartmentOnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ApPaymentSettlement", "IsDepartmentSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "DepartmentSendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ApPaymentSettlement", "DepartmentSendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ApTravelExpenseClaim", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "FromBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "FromBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "VehicleDistributionID", c => c.Guid());
            AddColumn("EXCEED.FundTransfer", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "FromBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "FromBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.GLTaxRate", "IsCost", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Issue", "SupplierId", c => c.String());
            AddColumn("EXCEED.PoPaymentSchedule", "AmountType", c => c.Int(nullable: false));
            AddColumn("EXCEED.PurchaseOrderItem", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PurchaseRequisition", "ForBusinessUnitId", c => c.Int());
            AddColumn("EXCEED.ServiceReceive", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "ServiceJobOrderNo", c => c.String());
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "PrevRequestDate", c => c.DateTime());
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "PrevKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "PrevLT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "CurrentKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "DifferenceKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "Consumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "StandardConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "External", c => c.Boolean(nullable: false));
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "ExternalRequesterName", c => c.String());
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "VehicleDistributionID", c => c.Guid());
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "ServiceJobOrderNo", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "PrevRequestDate", c => c.DateTime());
            AddColumn("EXCEED.StoreRequisition", "PrevKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisition", "PrevLT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisition", "CurrentKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisition", "DifferenceKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisition", "Consumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisition", "StandardConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.StoreRequisition", "External", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "ExternalRequesterName", c => c.String());
            AddColumn("EXCEED.StoreRequisition", "VehicleDistributionID", c => c.Guid());
            AddColumn("EXCEED.StoreRequisition", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApPaymentSettlementLine", "Source", c => c.String());
            AlterColumn("EXCEED.ApPaymentSettlementLine", "SourceId", c => c.Int());
            AlterColumn("EXCEED.ApPaymentSettlementLine", "WithholdingType", c => c.Int());
            DropTable("EXCEED.ApTravelExpenseDistribution");
            DropTable("EXCEED.ApTravelOrderDistribution");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.ApTravelOrderDistribution",
                c => new
                    {
                        ApTravelOrderDistributionId = c.Int(nullable: false, identity: true),
                        ApTravelOrderId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ApTravelOrderDistributionId);
            
            CreateTable(
                "EXCEED.ApTravelExpenseDistribution",
                c => new
                    {
                        ApTravelExpenseDistributionId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ApTravelExpenseDistributionId);
            
            DropForeignKey("EXCEED.PoAdditionalCharge", "PurchaseOrderID", "EXCEED.PurchaseOrder");
            DropForeignKey("EXCEED.ApTravelOrdertAttachment", "ApTravelOrderId", "EXCEED.ApTravelOrder");
            DropForeignKey("EXCEED.ApTravelExpenseClaimAttachment", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApPaymentSettlementDepartmentValidation", "ApPaymentSettlementId", "EXCEED.ApPaymentSettlement");
            DropIndex("EXCEED.PoAdditionalCharge", new[] { "PurchaseOrderID" });
            DropIndex("EXCEED.ApTravelOrdertAttachment", new[] { "ApTravelOrderId" });
            DropIndex("EXCEED.ApTravelExpenseClaimAttachment", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApPaymentSettlementDepartmentValidation", new[] { "ApPaymentSettlementId" });
            AlterColumn("EXCEED.ApPaymentSettlementLine", "WithholdingType", c => c.String());
            AlterColumn("EXCEED.ApPaymentSettlementLine", "SourceId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApPaymentSettlementLine", "Source", c => c.Int(nullable: false));
            DropColumn("EXCEED.StoreRequisition", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.StoreRequisition", "VehicleDistributionID");
            DropColumn("EXCEED.StoreRequisition", "ExternalRequesterName");
            DropColumn("EXCEED.StoreRequisition", "External");
            DropColumn("EXCEED.StoreRequisition", "StandardConsumption");
            DropColumn("EXCEED.StoreRequisition", "Consumption");
            DropColumn("EXCEED.StoreRequisition", "DifferenceKm");
            DropColumn("EXCEED.StoreRequisition", "CurrentKm");
            DropColumn("EXCEED.StoreRequisition", "PrevLT");
            DropColumn("EXCEED.StoreRequisition", "PrevKm");
            DropColumn("EXCEED.StoreRequisition", "PrevRequestDate");
            DropColumn("EXCEED.StoreRequisition", "ServiceJobOrderNo");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "CreatedAtOrgStructureId");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "VehicleDistributionID");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "ExternalRequesterName");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "External");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "StandardConsumption");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "Consumption");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "DifferenceKm");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "CurrentKm");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "PrevLT");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "PrevKm");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "PrevRequestDate");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "ServiceJobOrderNo");
            DropColumn("EXCEED.ServiceReceive", "BusinessUnitId");
            DropColumn("EXCEED.PurchaseRequisition", "ForBusinessUnitId");
            DropColumn("EXCEED.PurchaseOrderItem", "Discount");
            DropColumn("EXCEED.PoPaymentSchedule", "AmountType");
            DropColumn("EXCEED.Issue", "SupplierId");
            DropColumn("EXCEED.GLTaxRate", "IsCost");
            DropColumn("EXCEED.PaymentRequest", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.PaymentRequest", "FromBusinessUnitId");
            DropColumn("EXCEED.PaymentRequest", "FromBranchId");
            DropColumn("EXCEED.FundTransfer", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.ApTravelOrder", "VehicleDistributionID");
            DropColumn("EXCEED.ApTravelOrder", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.ApTravelOrder", "FromBusinessUnitId");
            DropColumn("EXCEED.ApTravelOrder", "FromBranchId");
            DropColumn("EXCEED.ApTravelExpenseClaim", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.ApPaymentSettlement", "DepartmentSendForApprovalTime");
            DropColumn("EXCEED.ApPaymentSettlement", "DepartmentSendForApprovalBy");
            DropColumn("EXCEED.ApPaymentSettlement", "IsDepartmentSendForApproval");
            DropColumn("EXCEED.ApPaymentSettlement", "DepartmentOnlineApprovedTime");
            DropColumn("EXCEED.ApPaymentSettlement", "DepartmentOnlineApprovedBy");
            DropColumn("EXCEED.ApPaymentSettlement", "IsDepartmentOnlineApproved");
            DropColumn("EXCEED.ApPaymentSettlement", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.ApPaymentSettlement", "CreatedAtBranchId");
            DropColumn("EXCEED.ApPaymentSettlement", "BusinessUnitId");
            DropColumn("EXCEED.ApPaymentSettlement", "BranchId");
            DropColumn("EXCEED.ApDirectPayment", "IsSettled");
            DropTable("EXCEED.PoAdditionalCharge");
            DropTable("EXCEED.ApTravelOrdertAttachment");
            DropTable("EXCEED.ApTravelExpenseClaimAttachment");
            DropTable("EXCEED.ApPaymentSettlementDepartmentValidation");
            CreateIndex("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId");
            CreateIndex("EXCEED.ApTravelExpenseDistribution", "ApTravelExpenseClaimId");
            AddForeignKey("EXCEED.ApTravelOrderDistribution", "ApTravelOrderId", "EXCEED.ApTravelOrder", "ApTravelOrderId", cascadeDelete: true);
            AddForeignKey("EXCEED.ApTravelExpenseDistribution", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim", "ApTravelExpenseClaimId", cascadeDelete: true);
        }
    }
}
