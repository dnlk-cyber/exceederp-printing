namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcrm6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FleetManagement.ExternalRentedEquipment",
                c => new
                    {
                        ExternalRentedEquipmentId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        RentId = c.Guid(nullable: false),
                        CurrentOdoMeter = c.String(),
                        AssetCode = c.String(),
                        EquipmentDetail = c.String(),
                        OperatorName = c.String(),
                        RentedRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentStatus = c.String(),
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
                .PrimaryKey(t => t.ExternalRentedEquipmentId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetId, cascadeDelete: true)
                .Index(t => t.FleetId);
            
            CreateTable(
                "FleetManagement.FleetPettyCashType",
                c => new
                    {
                        FleetPettyCashTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetPettyCashTypeId);
            
            CreateTable(
                "FleetManagement.OwningandOperatingCostConstant",
                c => new
                    {
                        OwningandOperatingCostConstantId = c.Int(nullable: false, identity: true),
                        ConstantType = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OwningandOperatingCostConstantId);
            
            CreateTable(
                "FleetManagement.OwningandOperatingCost",
                c => new
                    {
                        OwningandOperatingCostId = c.Int(nullable: false, identity: true),
                        FleetsID = c.Guid(nullable: false),
                        Model = c.String(),
                        Manufacture = c.String(),
                        OwnerShipPeriod = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EstimatedUsage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OwnerShipUsage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Interest = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOperatingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TyreReplacementCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalOperatingWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ResdualScrapValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        YerlyPremiuem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperatorDriverMonthlySalary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Depreciation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Insurance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PropertyTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubOil = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceParts = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tire = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceRepairCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SpecialWearItemsCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnderCarrageCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperatorCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalNetCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Profit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OverHead = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalRateWithFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalRateWithOutFuel = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.OwningandOperatingCostId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            AddColumn("FleetManagement.Accessories", "SerialNo", c => c.String());
            AddColumn("FleetManagement.FleetPettyCash", "Type", c => c.String());
            AddColumn("FleetManagement.FleetPettyCash", "JobOrderId", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("FleetManagement.OwningandOperatingCost", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.ExternalRentedEquipment", "FleetId", "FleetManagement.Fleets");
            DropIndex("FleetManagement.OwningandOperatingCost", new[] { "FleetsID" });
            DropIndex("FleetManagement.ExternalRentedEquipment", new[] { "FleetId" });
            DropColumn("FleetManagement.FleetPettyCash", "JobOrderId");
            DropColumn("FleetManagement.FleetPettyCash", "Type");
            DropColumn("FleetManagement.Accessories", "SerialNo");
            DropTable("FleetManagement.OwningandOperatingCost");
            DropTable("FleetManagement.OwningandOperatingCostConstant");
            DropTable("FleetManagement.FleetPettyCashType");
            DropTable("FleetManagement.ExternalRentedEquipment");
        }
    }
}
