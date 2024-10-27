namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finprocurementinventoryupgrade : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.APTravelClaimDeparture",
                c => new
                    {
                        APTravelClaimDepartureId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        From = c.String(),
                        To = c.String(),
                        Date = c.DateTime(nullable: false),
                        ArrivalDate = c.DateTime(nullable: false),
                        TransportMode = c.String(),
                        DestinationBranchId = c.Int(),
                        Duration = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.APTravelClaimDepartureId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.ApTravelFuelExpenseClaim",
                c => new
                    {
                        ApTravelFuelExpenseClaimId = c.Int(nullable: false, identity: true),
                        ApTravelExpenseClaimId = c.Int(nullable: false),
                        PlateNo = c.String(),
                        InvoiceNo = c.String(),
                        FuelStation = c.String(),
                        DepartureKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReturnKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DifferenceKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelType = c.Int(nullable: false),
                        FuelQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilQTY = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Minormaintenance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ApTravelFuelExpenseClaimId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
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
                .PrimaryKey(t => t.ApTravelExpenseDistributionId)
                .ForeignKey("EXCEED.ApTravelExpenseClaim", t => t.ApTravelExpenseClaimId, cascadeDelete: true)
                .Index(t => t.ApTravelExpenseClaimId);
            
            CreateTable(
                "EXCEED.BankUserAssignment",
                c => new
                    {
                        BankUserAssignmentId = c.Int(nullable: false, identity: true),
                        BankAccount = c.String(nullable: false),
                        User = c.String(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BankUserAssignmentId);
            
            CreateTable(
                "EXCEED.FundReceiveDistribution",
                c => new
                    {
                        FundReceiveDistributionId = c.Int(nullable: false, identity: true),
                        FundReceiveId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.FundReceiveDistributionId)
                .ForeignKey("EXCEED.FundReceive", t => t.FundReceiveId, cascadeDelete: true)
                .Index(t => t.FundReceiveId);
            
            CreateTable(
                "EXCEED.FundReceive",
                c => new
                    {
                        FundReceiveId = c.Int(nullable: false, identity: true),
                        FundTransferId = c.Int(nullable: false),
                        ReceiveDate = c.DateTime(nullable: false),
                        AmountReceived = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        FromBranchId = c.Int(nullable: false),
                        FromBusinessUnitId = c.Int(nullable: false),
                        BankAccountId = c.Int(nullable: false),
                        CreatedAtBranchId = c.Int(nullable: false),
                        CreatedAtBusinessUnitId = c.Int(nullable: false),
                        CreatedAtOrgStructureId = c.Int(nullable: false),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.FundReceiveId)
                .ForeignKey("EXCEED.FundTransfer", t => t.FundTransferId, cascadeDelete: true)
                .Index(t => t.FundTransferId);
            
            CreateTable(
                "EXCEED.FundReceiveValidation",
                c => new
                    {
                        FundReceiveValidationId = c.Int(nullable: false, identity: true),
                        FundReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FundReceiveValidationId)
                .ForeignKey("EXCEED.FundReceive", t => t.FundReceiveId, cascadeDelete: true)
                .Index(t => t.FundReceiveId);
            
            CreateTable(
                "EXCEED.PriceIndexValidation",
                c => new
                    {
                        PriceIndexValidationId = c.Int(nullable: false, identity: true),
                        ItemPriceIndexID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PriceIndexValidationId)
                .ForeignKey("EXCEED.ItemPriceIndex", t => t.ItemPriceIndexID, cascadeDelete: true)
                .Index(t => t.ItemPriceIndexID);
            
            AddColumn("EXCEED.ApMiscellaneousPayment", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "OrgStuructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "CreatedAtOrgStructureId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "ForwardedFrom", c => c.String());
            AddColumn("EXCEED.ApPaymentSettlement", "ForwardedTo", c => c.String());
            AddColumn("EXCEED.ApPaymentSettlement", "IsFinalApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ApPaymentSettlement", "CurrentApprovalStep", c => c.Int());
            AddColumn("EXCEED.ApTravelExpenseClaim", "Difference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApTravelExpenseClaim", "FromBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "FromBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseExpense", "ItemName", c => c.String(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "Claimed", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "PaymentAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PaymentRequest", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ItemPriceIndex", "Status", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemPriceIndex", "VoidBy", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.ItemPriceIndex", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemPriceIndex", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.ItemPriceIndex", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemPriceIndex", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("EXCEED.ItemPriceIndex", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.ItemPriceIndex", "SendForApprovalBy", c => c.String());
            AddColumn("EXCEED.ItemPriceIndex", "SendForApprovalTime", c => c.DateTime());
            AddColumn("EXCEED.ParticipatingSupplier", "AdditionalChargePercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.WinningSupplier", "AdditionalChargePercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.PropertyCountItem", "Unit", c => c.String());
            AddColumn("Inventory_Archive.StoreRequisitionArchive", "IsForProduction", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.StoreRequisition", "IsForProduction", c => c.Boolean(nullable: false));
            AlterColumn("EXCEED.AnnualNeedAssesment", "BusinessUnitId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApTravelOrderAdvancePayment", "ItemName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.PriceIndexValidation", "ItemPriceIndexID", "EXCEED.ItemPriceIndex");
            DropForeignKey("EXCEED.FundReceive", "FundTransferId", "EXCEED.FundTransfer");
            DropForeignKey("EXCEED.FundReceiveValidation", "FundReceiveId", "EXCEED.FundReceive");
            DropForeignKey("EXCEED.FundReceiveDistribution", "FundReceiveId", "EXCEED.FundReceive");
            DropForeignKey("EXCEED.ApTravelExpenseDistribution", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.ApTravelFuelExpenseClaim", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropForeignKey("EXCEED.APTravelClaimDeparture", "ApTravelExpenseClaimId", "EXCEED.ApTravelExpenseClaim");
            DropIndex("EXCEED.PriceIndexValidation", new[] { "ItemPriceIndexID" });
            DropIndex("EXCEED.FundReceiveValidation", new[] { "FundReceiveId" });
            DropIndex("EXCEED.FundReceive", new[] { "FundTransferId" });
            DropIndex("EXCEED.FundReceiveDistribution", new[] { "FundReceiveId" });
            DropIndex("EXCEED.ApTravelExpenseDistribution", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.ApTravelFuelExpenseClaim", new[] { "ApTravelExpenseClaimId" });
            DropIndex("EXCEED.APTravelClaimDeparture", new[] { "ApTravelExpenseClaimId" });
            AlterColumn("EXCEED.ApTravelOrderAdvancePayment", "ItemName", c => c.String());
            AlterColumn("EXCEED.AnnualNeedAssesment", "BusinessUnitId", c => c.Int());
            DropColumn("EXCEED.StoreRequisition", "IsForProduction");
            DropColumn("Inventory_Archive.StoreRequisitionArchive", "IsForProduction");
            DropColumn("EXCEED.PropertyCountItem", "Unit");
            DropColumn("EXCEED.WinningSupplier", "AdditionalChargePercent");
            DropColumn("EXCEED.ParticipatingSupplier", "AdditionalChargePercent");
            DropColumn("EXCEED.ItemPriceIndex", "SendForApprovalTime");
            DropColumn("EXCEED.ItemPriceIndex", "SendForApprovalBy");
            DropColumn("EXCEED.ItemPriceIndex", "IsSendForApproval");
            DropColumn("EXCEED.ItemPriceIndex", "OnlineTransferredTime");
            DropColumn("EXCEED.ItemPriceIndex", "IsOnlineTransferredBy");
            DropColumn("EXCEED.ItemPriceIndex", "IsOnlineTransferred");
            DropColumn("EXCEED.ItemPriceIndex", "OnlineApprovedTime");
            DropColumn("EXCEED.ItemPriceIndex", "OnlineApprovedBy");
            DropColumn("EXCEED.ItemPriceIndex", "IsOnlineApproved");
            DropColumn("EXCEED.ItemPriceIndex", "VoidTime");
            DropColumn("EXCEED.ItemPriceIndex", "VoidBy");
            DropColumn("EXCEED.ItemPriceIndex", "IsVoid");
            DropColumn("EXCEED.ItemPriceIndex", "Status");
            DropColumn("EXCEED.PaymentRequest", "AmountDue");
            DropColumn("EXCEED.PaymentRequest", "PaymentAmount");
            DropColumn("EXCEED.ApTravelOrder", "Claimed");
            DropColumn("EXCEED.ApTravelExpenseExpense", "ItemName");
            DropColumn("EXCEED.ApTravelExpenseClaim", "FromBusinessUnitId");
            DropColumn("EXCEED.ApTravelExpenseClaim", "FromBranchId");
            DropColumn("EXCEED.ApTravelExpenseClaim", "Difference");
            DropColumn("EXCEED.ApPaymentSettlement", "CurrentApprovalStep");
            DropColumn("EXCEED.ApPaymentSettlement", "IsFinalApproval");
            DropColumn("EXCEED.ApPaymentSettlement", "ForwardedTo");
            DropColumn("EXCEED.ApPaymentSettlement", "ForwardedFrom");
            DropColumn("EXCEED.ApPaymentSettlement", "CreatedAtOrgStructureId");
            DropColumn("EXCEED.ApPaymentSettlement", "OrgStuructureId");
            DropColumn("EXCEED.ApMiscellaneousPayment", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.ApMiscellaneousPayment", "CreatedAtBranchId");
            DropTable("EXCEED.PriceIndexValidation");
            DropTable("EXCEED.FundReceiveValidation");
            DropTable("EXCEED.FundReceive");
            DropTable("EXCEED.FundReceiveDistribution");
            DropTable("EXCEED.BankUserAssignment");
            DropTable("EXCEED.ApTravelExpenseDistribution");
            DropTable("EXCEED.ApTravelFuelExpenseClaim");
            DropTable("EXCEED.APTravelClaimDeparture");
        }
    }
}
