namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class decimalpoint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("EXCEED.BinCardLine", "ReceivedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLine", "ReceivedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLine", "IssuedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLine", "IssuedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLine", "BalanceUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.BinCardLine", "BalanceTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.GoodReceiveItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.IssueItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.IssueItem", "AdditionalCost", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.IssueItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ItemTransferItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ItemTransferItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ItemTransferItem", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ItemTransferItem", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.IssueItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.IssueItem", "AdditionalCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.IssueItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.GoodReceiveItem", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.GoodReceiveItem", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.GoodReceiveItem", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.GoodReceiveItem", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "BalanceTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "BalanceUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "IssuedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "IssuedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "ReceivedTPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.BinCardLine", "ReceivedUPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
