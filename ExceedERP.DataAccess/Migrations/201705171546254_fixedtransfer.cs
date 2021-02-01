namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixedtransfer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EXCEED.Accessory",
                c => new
                    {
                        AccessoryId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferId = c.Int(nullable: false),
                        Description = c.String(),
                        Quantity = c.Int(nullable: false),
                        SerialNo = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccessoryId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransfer", t => t.MachineryAndVehicleTransferId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferId);
            
            CreateTable(
                "EXCEED.MachineryAndVehicleTransfer",
                c => new
                    {
                        MachineryAndVehicleTransferId = c.Int(nullable: false, identity: true),
                        RequestNumber = c.String(),
                        TransferDate = c.DateTime(nullable: false),
                        UsageDays = c.Int(nullable: false),
                        TransferDesc = c.DateTime(nullable: false),
                        CustodianBeforeTransfer = c.String(),
                        LocationBeforeTransfer = c.String(),
                        TransferTo = c.String(),
                        TransferReason = c.String(),
                        TransferBy = c.Int(nullable: false),
                        DriverName = c.String(),
                        TransferPlateNo = c.String(),
                        Manufacturer = c.String(),
                        ModelNo = c.String(),
                        ChasisNo = c.String(),
                        SerialNo = c.String(),
                        PlateNo = c.String(),
                        EngineNo = c.String(),
                        TagNo = c.String(),
                        BarCodeNo = c.String(),
                        KmHourReading = c.String(),
                        Condition = c.Int(nullable: false),
                        FuelVolue = c.String(),
                        HydraulicOilVolume = c.String(),
                        BatteryAmpere = c.String(),
                        BatteryStatus = c.Int(nullable: false),
                        BatteryQuantity = c.Int(nullable: false),
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
                .PrimaryKey(t => t.MachineryAndVehicleTransferId);
            
            CreateTable(
                "EXCEED.DamagedPart",
                c => new
                    {
                        DamagedPartId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferId = c.Int(nullable: false),
                        DamagePart = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DamagedPartId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransfer", t => t.MachineryAndVehicleTransferId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferId);
            
            CreateTable(
                "EXCEED.MissedPart",
                c => new
                    {
                        MissedPartId = c.Int(nullable: false, identity: true),
                        MachineryAndVehicleTransferId = c.Int(nullable: false),
                        MissingPart = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MissedPartId)
                .ForeignKey("EXCEED.MachineryAndVehicleTransfer", t => t.MachineryAndVehicleTransferId, cascadeDelete: true)
                .Index(t => t.MachineryAndVehicleTransferId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EXCEED.MissedPart", "MachineryAndVehicleTransferId", "EXCEED.MachineryAndVehicleTransfer");
            DropForeignKey("EXCEED.DamagedPart", "MachineryAndVehicleTransferId", "EXCEED.MachineryAndVehicleTransfer");
            DropForeignKey("EXCEED.Accessory", "MachineryAndVehicleTransferId", "EXCEED.MachineryAndVehicleTransfer");
            DropIndex("EXCEED.MissedPart", new[] { "MachineryAndVehicleTransferId" });
            DropIndex("EXCEED.DamagedPart", new[] { "MachineryAndVehicleTransferId" });
            DropIndex("EXCEED.Accessory", new[] { "MachineryAndVehicleTransferId" });
            DropTable("EXCEED.MissedPart");
            DropTable("EXCEED.DamagedPart");
            DropTable("EXCEED.MachineryAndVehicleTransfer");
            DropTable("EXCEED.Accessory");
        }
    }
}
