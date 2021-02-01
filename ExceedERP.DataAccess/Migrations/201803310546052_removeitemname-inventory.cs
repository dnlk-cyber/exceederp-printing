namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeitemnameinventory : DbMigration
    {
        public override void Up()
        {
            AddColumn("Archive.EmployeeLetterSender", "SenderDepartment", c => c.Int(nullable: false));
            DropColumn("EXCEED.InventoryItem", "ItemCodeName");
            DropColumn("EXCEED.GoodReceiveItem", "ItemName");
            DropColumn("EXCEED.GoodSellingAndLendingItem", "ItemName");
            DropColumn("EXCEED.IssueItem", "ItemName");
            DropColumn("EXCEED.StoreRequisitionItem", "ItemName");
            DropColumn("EXCEED.ItemTransferItem", "ItemName");
            DropColumn("EXCEED.ItemTransferItemReceive", "ItemName");
            DropColumn("EXCEED.StockAdjustmentItems", "ItemName");
            DropColumn("EXCEED.StoreReturnItems", "ItemName");
            DropColumn("EXCEED.TempGoodReceiveItem", "ItemName");
        }
        
        public override void Down()
        {
            AddColumn("EXCEED.TempGoodReceiveItem", "ItemName", c => c.String());
            AddColumn("EXCEED.StoreReturnItems", "ItemName", c => c.String());
            AddColumn("EXCEED.StockAdjustmentItems", "ItemName", c => c.String());
            AddColumn("EXCEED.ItemTransferItemReceive", "ItemName", c => c.String());
            AddColumn("EXCEED.ItemTransferItem", "ItemName", c => c.String());
            AddColumn("EXCEED.StoreRequisitionItem", "ItemName", c => c.String());
            AddColumn("EXCEED.IssueItem", "ItemName", c => c.String());
            AddColumn("EXCEED.GoodSellingAndLendingItem", "ItemName", c => c.String());
            AddColumn("EXCEED.GoodReceiveItem", "ItemName", c => c.String());
            AddColumn("EXCEED.InventoryItem", "ItemCodeName", c => c.String());
            DropColumn("Archive.EmployeeLetterSender", "SenderDepartment");
        }
    }
}
