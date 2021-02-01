namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inveprocu : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.PurchaseRequisitionItem", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ItemTransferItemReceive", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ItemTransferItemReceive", "ShippedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ItemTransferItemReceive", "ReceivedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ItemTransferItemReceive", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("EXCEED.ItemTransferItemReceive", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("EXCEED.ItemTransferItemReceive", "TPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ItemTransferItemReceive", "UPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ItemTransferItemReceive", "ReceivedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ItemTransferItemReceive", "ShippedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("EXCEED.ItemTransferItemReceive", "RequestedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("EXCEED.PurchaseRequisitionItem", "RequestedQuantity");
        }
    }
}
