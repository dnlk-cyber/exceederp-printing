namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class financeprocsecupg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.BidParticipatingSupplierItem",
                c => new
                    {
                        BidParticipatingSupplierItemId = c.Int(nullable: false, identity: true),
                        BidPParticipatingSuppliersID = c.Int(nullable: false),
                        CatCode = c.String(nullable: false),
                        CategoryName = c.String(),
                        ItmCode = c.String(nullable: false),
                        ItemName = c.String(),
                        PartNo = c.String(),
                        ItemSpecification = c.String(),
                        Name = c.String(),
                        ModelNo = c.String(),
                        UoM = c.String(),
                        ItemType = c.Int(nullable: false),
                        SupplierId = c.String(),
                        IsNewSupplier = c.Boolean(nullable: false),
                        SupplierName = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LotID = c.Int(nullable: false),
                        GLTaxRateID = c.Int(nullable: false),
                        Technical = c.Single(nullable: false),
                        LotItemID = c.Int(nullable: false),
                        IsOutOfStock = c.Boolean(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BidParticipatingSupplierItemId)
                .ForeignKey("EXCEED.BidPParticipatingSuppliers", t => t.BidPParticipatingSuppliersID, cascadeDelete: true)
                .ForeignKey("EXCEED.Lot", t => t.LotID, cascadeDelete: false)
                .Index(t => t.BidPParticipatingSuppliersID)
                .Index(t => t.LotID);
            
            AddColumn("EXCEED.ApDirectPayment", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApDirectPayment", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApMiscellaneousPayment", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "WithholdTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "NetPayable", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApMiscellaneousPayment", "DeductAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApPayment", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApPayment", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelExpenseClaim", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "TotalDuration", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.ApTravelOrder", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.ApTravelOrder", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FundTransfer", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FundTransfer", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("EXCEED.PaymentRequest", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AlterColumn("EXCEED.ApTravelOrder", "PurposeOfTravel", c => c.String(nullable: false));
            AlterColumn("EXCEED.FundTransfer", "ReasonOfTransfer", c => c.String(nullable: false));
            AlterColumn("EXCEED.PaymentRequest", "ReasonOfPayment", c => c.String(nullable: false));
            DropColumn("EXCEED.FundTransfer", "RequestorBranchId");
            DropColumn("EXCEED.FundTransfer", "RequestorBusinessUnitId");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.FundTransfer", "RequestorBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("EXCEED.FundTransfer", "RequestorBranchId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.BidParticipatingSupplierItem", "LotID", "EXCEED.Lot");
            DropForeignKey("EXCEED.BidParticipatingSupplierItem", "BidPParticipatingSuppliersID", "EXCEED.BidPParticipatingSuppliers");
            DropIndex("EXCEED.BidParticipatingSupplierItem", new[] { "LotID" });
            DropIndex("EXCEED.BidParticipatingSupplierItem", new[] { "BidPParticipatingSuppliersID" });
            AlterColumn("EXCEED.PaymentRequest", "ReasonOfPayment", c => c.String());
            AlterColumn("EXCEED.FundTransfer", "ReasonOfTransfer", c => c.String());
            AlterColumn("EXCEED.ApTravelOrder", "PurposeOfTravel", c => c.String());
            DropColumn("EXCEED.PaymentRequest", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.PaymentRequest", "CreatedAtBranchId");
            DropColumn("EXCEED.FundTransfer", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.FundTransfer", "CreatedAtBranchId");
            DropColumn("EXCEED.ApTravelOrder", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.ApTravelOrder", "CreatedAtBranchId");
            DropColumn("EXCEED.ApTravelOrder", "TotalDuration");
            DropColumn("EXCEED.ApTravelExpenseClaim", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.ApTravelExpenseClaim", "CreatedAtBranchId");
            DropColumn("EXCEED.ApPayment", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.ApPayment", "CreatedAtBranchId");
            DropColumn("EXCEED.ApMiscellaneousPayment", "DeductAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "NetPayable");
            DropColumn("EXCEED.ApMiscellaneousPayment", "WithholdTaxAmount");
            DropColumn("EXCEED.ApMiscellaneousPayment", "WithholdingAmount");
            DropColumn("EXCEED.ApDirectPayment", "CreatedAtBusinessUnitId");
            DropColumn("EXCEED.ApDirectPayment", "CreatedAtBranchId");
            DropTable("EXCEED.BidParticipatingSupplierItem");
        }
    }
}
