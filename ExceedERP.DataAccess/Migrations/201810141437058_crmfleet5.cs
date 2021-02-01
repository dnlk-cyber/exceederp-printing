namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class crmfleet5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FleetManagement.ExternalRentedEquipmentCheckList",
                c => new
                    {
                        ExternalRentedEquipmentCheckListId = c.Guid(nullable: false),
                        ExternalRentedEquipmentId = c.Guid(nullable: false),
                        EquipmentType = c.String(),
                        HorsePower = c.String(),
                        MeterCube = c.String(),
                        SeatNo = c.String(),
                        Location = c.String(),
                        Manufacturer = c.String(),
                        Model = c.String(),
                        EngineManufacturer = c.String(),
                        ManufacturerDate = c.String(),
                        RentingCompany = c.String(),
                        CheckedRentingCompany = c.String(),
                        LibreNo = c.String(),
                        CheckingRequestingWorkUnit = c.String(),
                        CheckingLocation = c.String(),
                        ExcavatorTeeth = c.Int(nullable: false),
                        ChainType = c.Int(nullable: false),
                        TireType = c.Int(nullable: false),
                        AirConditioner = c.Int(nullable: false),
                        BrakeType = c.Int(nullable: false),
                        ElectricalSystem = c.Int(nullable: false),
                        HydrolicLeak = c.Int(nullable: false),
                        EngineStarting = c.Int(nullable: false),
                        EngineLeak = c.Int(nullable: false),
                        EngineSmoke = c.Int(nullable: false),
                        CheckingType = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ExternalRentedEquipmentCheckListId);
            
            CreateTable(
                "FleetManagement.FleetCondition",
                c => new
                    {
                        FleetConditionId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetConditionId);
            
            CreateTable(
                "FleetManagement.FleetReturnApproval",
                c => new
                    {
                        FleetReturnApprovalId = c.Int(nullable: false, identity: true),
                        FleetReturnId = c.Int(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetReturnApprovalId);
            
            CreateTable(
                "FleetManagement.FleetReturnItem",
                c => new
                    {
                        FleetReturnItemId = c.Int(nullable: false, identity: true),
                        FleetReturnId = c.Int(nullable: false),
                        IdentifcationNo = c.String(),
                        CostCode = c.String(),
                        Description = c.String(),
                        PartNo = c.String(),
                        Location = c.String(),
                        Requested = c.String(),
                        Recieved = c.String(),
                        Unit = c.String(),
                        UnitPriceBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceCent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPriceCent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPriceBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.FleetReturnItemId);
            
            CreateTable(
                "FleetManagement.FleetReturn",
                c => new
                    {
                        FleetReturnId = c.Int(nullable: false, identity: true),
                        FleetsId = c.Guid(nullable: false),
                        From = c.String(),
                        DeliveredTo = c.String(),
                        DebitAccountNo = c.String(),
                        CreditAccountNo = c.String(),
                        PlateNo = c.String(),
                        EquipmentName = c.String(),
                        Manufacturer = c.String(),
                        ChancesSerialNo = c.String(),
                        EngineNo = c.String(),
                        OwnerCertificateNo = c.String(),
                        Condition = c.String(),
                        AssetNo = c.String(),
                        Causeofreturn = c.String(),
                        CheckList = c.String(),
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
                .PrimaryKey(t => t.FleetReturnId);
            
            CreateTable(
                "EXCEED.LicenseType",
                c => new
                    {
                        LicenseTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LicenseTypeId);
            
            CreateTable(
                "FleetManagement.OperatorLicense",
                c => new
                    {
                        OperatorLicenseId = c.Int(nullable: false, identity: true),
                        OperatorId = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.OperatorLicenseId);
            
            AddColumn("FleetManagement.Accident", "Operator", c => c.String());
            AddColumn("FleetManagement.Fleets", "IsReturned", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalRentedEquipment", "EquipmentName", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "SubEquipmentName", c => c.String(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "OdoMeter", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "LoadedPlateNo", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "TransferReason", c => c.String());
            AlterColumn("FleetManagement.SiteVechicleTransferDetail", "Type", c => c.String());
            AlterColumn("FleetManagement.SiteVechicleTransferTyreDetail", "SerialNo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("FleetManagement.SiteVechicleTransferTyreDetail", "SerialNo", c => c.String(nullable: false));
            AlterColumn("FleetManagement.SiteVechicleTransferDetail", "Type", c => c.String(nullable: false));
            DropColumn("FleetManagement.SitesVehicleTransfer", "TransferReason");
            DropColumn("FleetManagement.SitesVehicleTransfer", "LoadedPlateNo");
            DropColumn("FleetManagement.SitesVehicleTransfer", "OdoMeter");
            DropColumn("FleetManagement.RequestedEquipment", "SubEquipmentName");
            DropColumn("FleetManagement.ExternalRentedEquipment", "EquipmentName");
            DropColumn("FleetManagement.Fleets", "IsReturned");
            DropColumn("FleetManagement.Accident", "Operator");
            DropTable("FleetManagement.OperatorLicense");
            DropTable("EXCEED.LicenseType");
            DropTable("FleetManagement.FleetReturn");
            DropTable("FleetManagement.FleetReturnItem");
            DropTable("FleetManagement.FleetReturnApproval");
            DropTable("FleetManagement.FleetCondition");
            DropTable("FleetManagement.ExternalRentedEquipmentCheckList");
        }
    }
}
