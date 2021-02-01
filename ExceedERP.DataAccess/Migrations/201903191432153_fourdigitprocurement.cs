namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fourdigitprocurement : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.BidAwardedSupplierItem", "Quanity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "FinalUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "Addition", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplier", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplier", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplier", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.WinningSupplierItem", "Quanity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.WinningSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.WinningSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.WinningSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseOrderItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.PurchaseOrderItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.PurchaseOrderItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.PurchaseOrderItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.PurchaseOrderItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.WinningSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.WinningSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.WinningSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.WinningSupplierItem", "Quanity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipatingSupplier", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipatingSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipatingSupplier", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipatingSupplier", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ContractManagementItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ContractManagementItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ContractManagementItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ContractManagementItem", "Addition", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ContractManagementItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipantsItem", "DiscountAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipantsItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipantsItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipantsItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipantsItem", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ParticipantsItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "FinalUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "Quanity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
