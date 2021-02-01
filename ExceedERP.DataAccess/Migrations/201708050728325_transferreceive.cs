namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transferreceive : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.ItemTransferItemReceive",
                c => new
                    {
                        ItemTransferItemReceiveId = c.Int(nullable: false, identity: true),
                        ItemTransferReceiveId = c.Int(nullable: false),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        PartOrCodeNumber = c.String(),
                        RequestedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ShippedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemTransferItemReceiveId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemTransferReceive", t => t.ItemTransferReceiveId, cascadeDelete: true)
                .Index(t => t.ItemTransferReceiveId)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.ItemTransferReceive",
                c => new
                    {
                        ItemTransferReceiveId = c.Int(nullable: false, identity: true),
                        ItemTransferID = c.Int(nullable: false),
                        DocumentRef = c.String(),
                        ReceiveDate = c.DateTime(nullable: false),
                        FromStore = c.String(nullable: false),
                        ToStore = c.String(nullable: false),
                        DeliveredBy = c.String(),
                        PlateNo = c.String(),
                        SupplierName = c.String(),
                        DriverType = c.String(),
                        GLPeriodId = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ItemTransferReceiveId);
            
            CreateTable(
                "EXCEED.ItemTransferReceiveDistribution",
                c => new
                    {
                        ItemTransferReceiveDistributionId = c.Int(nullable: false, identity: true),
                        ItemTransferReceiveId = c.Int(nullable: false),
                        AccountCode = c.String(),
                        AccountDesc = c.String(),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ItemTransferReceiveDistributionId)
                .ForeignKey("EXCEED.ItemTransferReceive", t => t.ItemTransferReceiveId, cascadeDelete: true)
                .Index(t => t.ItemTransferReceiveId);
            
            CreateTable(
                "EXCEED.ItemTransferReceiverValidation",
                c => new
                    {
                        ItemTransferReceiverValidationId = c.Int(nullable: false, identity: true),
                        ItemTransferReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ItemTransferReceiverValidationId)
                .ForeignKey("EXCEED.ItemTransferReceive", t => t.ItemTransferReceiveId, cascadeDelete: true)
                .Index(t => t.ItemTransferReceiveId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.ItemTransferReceiverValidation", "ItemTransferReceiveId", "EXCEED.ItemTransferReceive");
            DropForeignKey("EXCEED.ItemTransferReceiveDistribution", "ItemTransferReceiveId", "EXCEED.ItemTransferReceive");
            DropForeignKey("EXCEED.ItemTransferItemReceive", "ItemTransferReceiveId", "EXCEED.ItemTransferReceive");
            DropForeignKey("EXCEED.ItemTransferItemReceive", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ItemTransferItemReceive", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.ItemTransferReceiverValidation", new[] { "ItemTransferReceiveId" });
            DropIndex("EXCEED.ItemTransferReceiveDistribution", new[] { "ItemTransferReceiveId" });
            DropIndex("EXCEED.ItemTransferItemReceive", new[] { "ItemCode" });
            DropIndex("EXCEED.ItemTransferItemReceive", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.ItemTransferItemReceive", new[] { "ItemTransferReceiveId" });
            DropTable("EXCEED.ItemTransferReceiverValidation");
            DropTable("EXCEED.ItemTransferReceiveDistribution");
            DropTable("EXCEED.ItemTransferReceive");
            DropTable("EXCEED.ItemTransferItemReceive");
        }
    }
}
