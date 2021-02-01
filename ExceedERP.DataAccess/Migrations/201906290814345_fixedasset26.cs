namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedasset26 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.AccessoryReceive",
                c => new
                    {
                        AccessoryReceiveId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferReceiveId = c.Int(nullable: false),
                        Description = c.String(),
                        Quantity = c.Int(nullable: false),
                        SerialNo = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccessoryReceiveId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransferReceive", t => t.MachineryAndVehicleTransferReceiveId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferReceiveId);
            
            CreateTable(
                "EXCEED.MachineryAndVehicleTransferReceive",
                c => new
                    {
                        MachineryAndVehicleTransferReceiveId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferId = c.Int(nullable: false),
                        ReceiveDate = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.MachineryAndVehicleTransferReceiveId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransfer", t => t.MachineryAndVehicleTransferId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferId);
            
            CreateTable(
                "EXCEED.DamagedPartReceive",
                c => new
                    {
                        DamagedPartReceiveId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferReceiveId = c.Int(nullable: false),
                        DamagePart = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DamagedPartReceiveId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransferReceive", t => t.MachineryAndVehicleTransferReceiveId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferReceiveId);
            
            CreateTable(
                "EXCEED.MachineryAndVehicleTransferReceiveValidation",
                c => new
                    {
                        MachineryAndVehicleTransferReceiveValidationId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MachineryAndVehicleTransferReceiveValidationId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransferReceive", t => t.MachineryAndVehicleTransferReceiveId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferReceiveId);
            
            CreateTable(
                "EXCEED.MissedPartReceive",
                c => new
                    {
                        MissedPartReceiveId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferReceiveId = c.Int(nullable: false),
                        MissingPart = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MissedPartReceiveId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransferReceive", t => t.MachineryAndVehicleTransferReceiveId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferReceiveId);
            
            CreateTable(
                "EXCEED.AssetTransferReceiveItem",
                c => new
                    {
                        AssetTransferReceiveItemId = c.Int(nullable: false, identity: true),
                        AssetTransferReceiveId = c.Int(nullable: false),
                        FixedAssetCategoryId = c.Int(nullable: false),
                        FixedAssetSubCategoryId = c.Int(nullable: false),
                        AssetRegistrationId = c.Int(nullable: false),
                        ItemName = c.String(),
                        Name = c.String(),
                        Unit = c.String(),
                        GrnNo = c.String(),
                        Size = c.String(),
                        ModelNo = c.String(),
                        SerialNo = c.String(),
                        TagNo = c.String(),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetTransferReceiveItemId)
                .ForeignKey("EXCEED.AssetTransferReceive", t => t.AssetTransferReceiveId, cascadeDelete: true)
                .Index(t => t.AssetTransferReceiveId);
            
            CreateTable(
                "EXCEED.AssetTransferReceive",
                c => new
                    {
                        AssetTransferReceiveId = c.Int(nullable: false, identity: true),
                        AssetTransferId = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        ReceiveDate = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.AssetTransferReceiveId)
                .ForeignKey("EXCEED.AssetTransfer", t => t.AssetTransferId, cascadeDelete: true)
                .Index(t => t.AssetTransferId);
            
            CreateTable(
                "EXCEED.AssetTransferReceiveValidation",
                c => new
                    {
                        AssetTransferReceiveValidationId = c.Int(nullable: false, identity: true),
                        AssetTransferReceiveId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AssetTransferReceiveValidationId)
                .ForeignKey("EXCEED.AssetTransferReceive", t => t.AssetTransferReceiveId, cascadeDelete: true)
                .Index(t => t.AssetTransferReceiveId);
            
            DropTable("EXCEED.DeprciationReverse");
            DropTable("EXCEED.DepreciationPeriod");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.DepreciationPeriod",
                c => new
                    {
                        DepreciationPeriodId = c.Int(nullable: false, identity: true),
                        Period = c.Int(nullable: false),
                        FiscalYear = c.Int(nullable: false),
                        StoreCode = c.String(),
                        Status = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DepreciationPeriodId);
            
            CreateTable(
                "EXCEED.DeprciationReverse",
                c => new
                    {
                        DeprciationReverseId = c.Int(nullable: false, identity: true),
                        RegistrationID = c.Int(nullable: false),
                        StoreCode = c.String(),
                        Period = c.String(),
                        FiscalYear = c.String(),
                        BookValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DeprciationReverseId);
            
            DropForeignKey("EXCEED.AssetTransferReceive", "AssetTransferId", "EXCEED.AssetTransfer");
            DropForeignKey("EXCEED.AssetTransferReceiveValidation", "AssetTransferReceiveId", "EXCEED.AssetTransferReceive");
            DropForeignKey("EXCEED.AssetTransferReceiveItem", "AssetTransferReceiveId", "EXCEED.AssetTransferReceive");
            DropForeignKey("EXCEED.MachineryAndVehicleTransferReceive", "MachineryAndVehicleTransferId", "EXCEED.MachineryAndVehicleTransfer");
            DropForeignKey("EXCEED.MissedPartReceive", "MachineryAndVehicleTransferReceiveId", "EXCEED.MachineryAndVehicleTransferReceive");
            DropForeignKey("EXCEED.MachineryAndVehicleTransferReceiveValidation", "MachineryAndVehicleTransferReceiveId", "EXCEED.MachineryAndVehicleTransferReceive");
            DropForeignKey("EXCEED.DamagedPartReceive", "MachineryAndVehicleTransferReceiveId", "EXCEED.MachineryAndVehicleTransferReceive");
            DropForeignKey("EXCEED.AccessoryReceive", "MachineryAndVehicleTransferReceiveId", "EXCEED.MachineryAndVehicleTransferReceive");
            DropIndex("EXCEED.AssetTransferReceiveValidation", new[] { "AssetTransferReceiveId" });
            DropIndex("EXCEED.AssetTransferReceive", new[] { "AssetTransferId" });
            DropIndex("EXCEED.AssetTransferReceiveItem", new[] { "AssetTransferReceiveId" });
            DropIndex("EXCEED.MissedPartReceive", new[] { "MachineryAndVehicleTransferReceiveId" });
            DropIndex("EXCEED.MachineryAndVehicleTransferReceiveValidation", new[] { "MachineryAndVehicleTransferReceiveId" });
            DropIndex("EXCEED.DamagedPartReceive", new[] { "MachineryAndVehicleTransferReceiveId" });
            DropIndex("EXCEED.MachineryAndVehicleTransferReceive", new[] { "MachineryAndVehicleTransferId" });
            DropIndex("EXCEED.AccessoryReceive", new[] { "MachineryAndVehicleTransferReceiveId" });
            DropTable("EXCEED.AssetTransferReceiveValidation");
            DropTable("EXCEED.AssetTransferReceive");
            DropTable("EXCEED.AssetTransferReceiveItem");
            DropTable("EXCEED.MissedPartReceive");
            DropTable("EXCEED.MachineryAndVehicleTransferReceiveValidation");
            DropTable("EXCEED.DamagedPartReceive");
            DropTable("EXCEED.MachineryAndVehicleTransferReceive");
            DropTable("EXCEED.AccessoryReceive");
        }
    }
}
