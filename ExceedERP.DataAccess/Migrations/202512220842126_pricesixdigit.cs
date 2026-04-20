namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pricesixdigit : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.BidAwardedSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "FinalUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipantsItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipantsItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipantsItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipantsItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ContractManagementItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ContractManagementItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ContractManagementItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.GoodReceiveItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.GoodReceiveItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.GoodReceiveItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.GoodReceiveItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.IssueItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.IssueItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipatingSupplier", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipatingSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.ParticipatingSupplier", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.WinningSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.WinningSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.WinningSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.PurchaseOrderItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.PurchaseOrderItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.PurchaseOrderItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 6));
            AlterColumn("EXCEED.PurchaseOrderItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 6));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.PurchaseOrderItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.PurchaseOrderItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.WinningSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.WinningSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.WinningSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplier", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplier", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplier", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipatingSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.IssueItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.IssueItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ContractManagementItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ParticipantsItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "FinalUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BidAwardedSupplierItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
    }
}
