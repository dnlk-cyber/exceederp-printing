namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetcon : DbMigration
    {
        public override void Up()
        {
            MoveTable(name: "EXCEED.LineCreditSettlement", newSchema: "CRM");
            DropForeignKey("FleetManagement.EquipmentIdentity", "Fleets_FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.EquipmentType", "Fleets_FleetsID", "FleetManagement.Fleets");
            DropForeignKey("Construction.ProjectJobTitle", "RankId", "HRM.Rank");
            DropForeignKey("Construction.ProjectJobTitle", "StepId", "HRM.Step");
            DropForeignKey("Construction.ProjectEstmnIndexLabour", "ProjectJobTitleId", "Construction.ProjectJobTitle");
            DropForeignKey("FleetManagement.SalvageStoreGoodReceiving", "SparePartID", "FleetManagement.SparePart");
            DropForeignKey("FleetManagement.JobOrderDetail", "JobOrderID", "FleetManagement.JobOrder");
            DropForeignKey("FleetManagement.OutsourceMaintenanceApproval", "OutsourceMaintenanceId", "FleetManagement.OutsourceMaintenance");
            DropForeignKey("FleetManagement.SalvageStore", "SparePartID", "FleetManagement.SparePart");
            DropForeignKey("FleetManagement.ServiceSpare", "ServiceSpare_ServiceSpareID", "FleetManagement.ServiceSpare");
            DropForeignKey("FleetManagement.ServiceSpare", "SparePartID", "FleetManagement.SparePart");
            DropIndex("FleetManagement.EquipmentIdentity", new[] { "Fleets_FleetsID" });
            DropIndex("FleetManagement.EquipmentType", new[] { "Fleets_FleetsID" });
            DropIndex("Construction.ProjectEstmnIndexLabour", new[] { "ProjectJobTitleId" });
            DropIndex("Construction.ProjectJobTitle", new[] { "RankId" });
            DropIndex("Construction.ProjectJobTitle", new[] { "StepId" });
            DropIndex("FleetManagement.SalvageStoreGoodReceiving", new[] { "SparePartID" });
            DropIndex("FleetManagement.JobOrderDetail", new[] { "JobOrderID" });
            DropIndex("FleetManagement.OutsourceMaintenanceApproval", new[] { "OutsourceMaintenanceId" });
            DropIndex("FleetManagement.SalvageStore", new[] { "SparePartID" });
            DropIndex("FleetManagement.ServiceSpare", new[] { "SparePartID" });
            DropIndex("FleetManagement.ServiceSpare", new[] { "ServiceSpare_ServiceSpareID" });
            CreateTable(
                "FleetManagement.BatteryApproval",
                c => new
                    {
                        BatteryApprovalId = c.Guid(nullable: false),
                        BatteryId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.BatteryApprovalId);
            
            CreateTable(
                "FleetManagement.BatteryIssue",
                c => new
                    {
                        BatteryIssueId = c.Guid(nullable: false),
                        BatteryReturnId = c.Guid(nullable: false),
                        EquipmentBatteryId = c.Guid(nullable: false),
                        PlateNumber = c.String(),
                        OperatorName = c.String(),
                        BatteryType = c.String(nullable: false),
                        StoreCode = c.String(),
                        InstalledDate = c.DateTime(nullable: false),
                        Brand = c.String(nullable: false),
                        SerialNo = c.String(nullable: false),
                        KMReading = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DescriptionReason = c.String(nullable: false),
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
                .PrimaryKey(t => t.BatteryIssueId);
            
            CreateTable(
                "FleetManagement.BatteryReturn",
                c => new
                    {
                        BatteryReturnId = c.Guid(nullable: false),
                        BatteryId = c.Guid(nullable: false),
                        PlateNumber = c.String(),
                        OperatorName = c.String(),
                        MakeCountry = c.String(),
                        StoreCode = c.String(),
                        BatteryType = c.String(nullable: false),
                        InstalledDate = c.DateTime(nullable: false),
                        Brand = c.String(nullable: false),
                        SerialNo = c.String(nullable: false),
                        Condition = c.String(),
                        KMReading = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DescriptionReason = c.String(nullable: false),
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
                .PrimaryKey(t => t.BatteryReturnId);
            
            CreateTable(
                "FleetManagement.EquipmentArrival",
                c => new
                    {
                        EquipmentArrivalId = c.Guid(nullable: false),
                        AssignmentId = c.Guid(nullable: false),
                        FleetsId = c.Guid(nullable: false),
                        EquipmentType = c.String(),
                        Model = c.String(),
                        DCENo = c.String(),
                        OwnedBy = c.String(),
                        Date = c.DateTime(nullable: false),
                        KMHRReading = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MeansOfTravelling = c.String(nullable: false),
                        OperatorName = c.String(),
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
                .PrimaryKey(t => t.EquipmentArrivalId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsId, cascadeDelete: true)
                .Index(t => t.FleetsId);
            
            CreateTable(
                "FleetManagement.EquipmentArrivalApproval",
                c => new
                    {
                        EquipmentArrivalApprovalId = c.Guid(nullable: false),
                        EquipmentArrivalId = c.Guid(nullable: false),
                        Status = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentArrivalApprovalId)
                .ForeignKey("FleetManagement.EquipmentArrival", t => t.EquipmentArrivalId, cascadeDelete: true)
                .Index(t => t.EquipmentArrivalId);
            
            CreateTable(
                "FleetManagement.EquipmentBattery",
                c => new
                    {
                        EquipmentBatteryId = c.Guid(nullable: false),
                        StoreCode = c.String(nullable: false),
                        SerialNo = c.String(nullable: false),
                        Make = c.String(),
                        MakeCountry = c.String(),
                        Type = c.String(),
                        Condition = c.String(),
                        Odometer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fleetId = c.Guid(nullable: false),
                        Isattached = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentBatteryId);
            
            CreateTable(
                "FleetManagement.EquipmentBatteryLog",
                c => new
                    {
                        EquipmentBatteryLogId = c.Guid(nullable: false),
                        EquipmentBatteryId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        Name = c.String(),
                        StoreCode = c.String(),
                        SerialNo = c.String(),
                        Type = c.String(),
                        Make = c.String(),
                        MakeCountry = c.String(),
                        Size = c.String(),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProjectName = c.String(),
                        IntialOdoMeter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinalOdoMeter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Reason = c.String(),
                        Isattached = c.Boolean(nullable: false),
                        Condition = c.String(),
                        Efficency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        Fleet_FleetsID = c.Guid(),
                    })
                .PrimaryKey(t => t.EquipmentBatteryLogId)
                .ForeignKey("FleetManagement.Fleets", t => t.Fleet_FleetsID)
                .Index(t => t.Fleet_FleetsID);
            
            CreateTable(
                "FleetManagement.EquipmentTyreLog",
                c => new
                    {
                        EquipmentTyreLogId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        Type = c.String(),
                        StoreCode = c.String(),
                        SerialNo = c.String(nullable: false),
                        Make = c.String(),
                        MakeCountry = c.String(),
                        ProjectName = c.String(),
                        Size = c.String(),
                        IntialOdoMeter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinalOdoMeter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Isattached = c.Boolean(nullable: false),
                        Condition = c.String(),
                        Reason = c.String(),
                        Efficency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        Fleet_FleetsID = c.Guid(),
                    })
                .PrimaryKey(t => t.EquipmentTyreLogId)
                .ForeignKey("FleetManagement.Fleets", t => t.Fleet_FleetsID)
                .Index(t => t.Fleet_FleetsID);
            
            CreateTable(
                "FleetManagement.ExternalComponent",
                c => new
                    {
                        ExternalComponentId = c.Guid(nullable: false),
                        JobId = c.Guid(nullable: false),
                        ComponentName = c.String(),
                        EnterpriseName = c.String(),
                        EquipmentType = c.String(),
                        Manufacturer = c.String(),
                        Model = c.String(),
                        ChasisNo = c.String(),
                        Code = c.String(),
                        KMReading = c.String(),
                        MaintaingCompany = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalComponentId);
            
            CreateTable(
                "FleetManagement.ExternalComponentDetail",
                c => new
                    {
                        ExternalComponentDetailId = c.Guid(nullable: false),
                        ExternalComponentId = c.Guid(nullable: false),
                        Name = c.String(),
                        ApprovalStatus = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalComponentDetailId);
            
            CreateTable(
                "FleetManagement.FollowUp",
                c => new
                    {
                        FollowUpId = c.Guid(nullable: false),
                        JobOrderId = c.Guid(nullable: false),
                        DurationHr = c.String(),
                        Status = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FollowUpId);
            
            CreateTable(
                "FleetManagement.FuelConsumptionAlertLevel",
                c => new
                    {
                        FuelConsumptionAlertLevelId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        FromValue = c.Int(nullable: false),
                        ToValue = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FuelConsumptionAlertLevelId);
            
            CreateTable(
                "FleetManagement.HRFleetMapping",
                c => new
                    {
                        HRFleetMappingId = c.Guid(nullable: false),
                        JobTitleCode = c.Int(nullable: false),
                        PlacmentType = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.HRFleetMappingId);
            
            CreateTable(
                "FleetManagement.InventorySparePartMapping",
                c => new
                    {
                        InventorySparePartMappingId = c.Guid(nullable: false),
                        InventoryCategoryId = c.String(),
                        SpareType = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InventorySparePartMappingId);
            
            CreateTable(
                "FleetManagement.JobCauses",
                c => new
                    {
                        JobCausesId = c.Guid(nullable: false),
                        OrderId = c.Guid(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobCausesId);
            
            CreateTable(
                "FleetManagement.JobRequest",
                c => new
                    {
                        JobRequestId = c.Guid(nullable: false),
                        Num = c.Int(nullable: false, identity: true),
                        RequestNumber = c.Int(),
                        FleetsID = c.Guid(nullable: false),
                        EquipmentName = c.String(),
                        Requester = c.String(),
                        OperatorName = c.String(),
                        FuelAmount = c.String(),
                        Odometer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AssetNum = c.String(),
                        ProjectName = c.String(),
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
                .PrimaryKey(t => t.JobRequestId);
            
            CreateTable(
                "FleetManagement.JobStoreRequisition",
                c => new
                    {
                        JobStoreRequisitionId = c.Guid(nullable: false),
                        JobId = c.Guid(nullable: false),
                        RequisitionId = c.Int(nullable: false),
                        Type = c.String(),
                        IsSalvage = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.JobStoreRequisitionId);
            
            CreateTable(
                "FleetManagement.MaintenaceInspectionLog",
                c => new
                    {
                        MaintenaceInspectionLogId = c.Guid(nullable: false),
                        JobId = c.Guid(nullable: false),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenaceInspectionLogId);
            
            CreateTable(
                "FleetManagement.MaintenanceType",
                c => new
                    {
                        MaintenanceTypeId = c.Guid(nullable: false),
                        Name = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceTypeId);
            
            CreateTable(
                "FleetManagement.MechanicOnHand",
                c => new
                    {
                        MechanicOnHandId = c.Guid(nullable: false),
                        ServiceId = c.Guid(nullable: false),
                        Name = c.String(),
                        SpareName = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MechanicOnHandId);
            
            CreateTable(
                "FleetManagement.WorkUnitSection",
                c => new
                    {
                        WorkUnitSectionId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Branch = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkUnitSectionId);
            
            AddColumn("FleetManagement.Fleets", "RentType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "IdealRentalRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.Fleets", "BookValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentType", "FixedAssetCategoryID", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireIssue", "TireReturnId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.TireIssue", "EquipmentTireId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.TireIssue", "FleetId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.TireIssue", "StoreCode", c => c.String());
            AddColumn("FleetManagement.TireReturn", "TireId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.TireReturn", "FleetId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.TireReturn", "StoreCode", c => c.String());
            AddColumn("FleetManagement.AssignedEquipment", "IsArrived", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.EquipmentTyre", "StoreCode", c => c.String(nullable: false));
            AddColumn("FleetManagement.EquipmentTyre", "MakeCountry", c => c.String());
            AddColumn("FleetManagement.EquipmentTyre", "Tube", c => c.String());
            AddColumn("FleetManagement.EquipmentTyre", "OdoMeter", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ExternalMaterial", "Type", c => c.Int(nullable: false));
            AddColumn("FleetManagement.ExternalMaterial", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ExternalMaterial", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ExternalMaterial", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ExternalMaterial", "Status", c => c.String());
            AddColumn("FleetManagement.ExternalMaterial", "Remark", c => c.String());
            AddColumn("FleetManagement.ExternalMaterial", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalMaterial", "VoidBy", c => c.String());
            AddColumn("FleetManagement.ExternalMaterial", "VoidTime", c => c.DateTime());
            AddColumn("FleetManagement.ExternalMaterial", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalMaterial", "OnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.ExternalMaterial", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.ExternalMaterial", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalMaterial", "IsOnlineTransferredBy", c => c.String());
            AddColumn("FleetManagement.ExternalMaterial", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("FleetManagement.ExternalMaterial", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ExternalMaterial", "OrgBranchName", c => c.String());
            AddColumn("FleetManagement.ExternalMaterial", "SendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.ExternalMaterial", "SendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.SalvageStoreGoodReceiving", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.SalvageStoreGoodReceiving", "SparePartName", c => c.String());
            AddColumn("FleetManagement.SalvageStoreGoodReceiving", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.SalvageStoreGoodReceiving", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.JobOrder", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.JobOrder", "OperatorName", c => c.String());
            AddColumn("FleetManagement.JobOrder", "AssetNum", c => c.String());
            AddColumn("FleetManagement.JobOrder", "IsExternal", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobOrder", "EquipmentType", c => c.String());
            AddColumn("FleetManagement.JobOrder", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.JobOrder", "CurrentFuel", c => c.String());
            AddColumn("FleetManagement.JobOrder", "Referance", c => c.Int());
            AddColumn("FleetManagement.JobOrderDetail", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobOrderDetail", "IsSecondOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobOrderDetail", "SecondOnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.JobOrderDetail", "SecondOnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.JobOrderDetail", "IsSecondSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.JobOrderDetail", "SecondSendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.JobOrderDetail", "SecondSendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.Service", "SectionId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.ServiceTool", "Status", c => c.String());
            AddColumn("FleetManagement.ServiceTool", "Remark", c => c.String());
            AddColumn("FleetManagement.ServiceTool", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceTool", "VoidBy", c => c.String());
            AddColumn("FleetManagement.ServiceTool", "VoidTime", c => c.DateTime());
            AddColumn("FleetManagement.ServiceTool", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceTool", "OnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.ServiceTool", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.ServiceTool", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceTool", "IsOnlineTransferredBy", c => c.String());
            AddColumn("FleetManagement.ServiceTool", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("FleetManagement.ServiceTool", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceTool", "SendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.ServiceTool", "SendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.MechanicCrew", "SectionId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.ServiceCrew", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceCrew", "ActivatedDate", c => c.DateTime());
            AddColumn("FleetManagement.OutsourceMaintenanceApproval", "JobOrderId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.RentRequest", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.RequestedEquipment", "ManufacturedYear", c => c.String());
            AddColumn("FleetManagement.SalvageStore", "JobId", c => c.Guid());
            AddColumn("FleetManagement.SalvageStore", "SalvageStoreGoodReceivingId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.SalvageStore", "SparePartName", c => c.String());
            AddColumn("FleetManagement.SalvageStore", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.SalvageStore", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.SalvageStore", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.SalvageStoreApproval", "ApprovedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ServiceSpare", "InventoryItemId", c => c.String());
            AddColumn("FleetManagement.ServiceSpare", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ServiceSpare", "UsedQuantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.ServiceSpare", "Status", c => c.String());
            AddColumn("FleetManagement.ServiceSpare", "Remark", c => c.String());
            AddColumn("FleetManagement.ServiceSpare", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceSpare", "VoidBy", c => c.String());
            AddColumn("FleetManagement.ServiceSpare", "VoidTime", c => c.DateTime());
            AddColumn("FleetManagement.ServiceSpare", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceSpare", "OnlineApprovedBy", c => c.String());
            AddColumn("FleetManagement.ServiceSpare", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.ServiceSpare", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceSpare", "IsOnlineTransferredBy", c => c.String());
            AddColumn("FleetManagement.ServiceSpare", "OnlineTransferredTime", c => c.DateTime());
            AddColumn("FleetManagement.ServiceSpare", "IsSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.ServiceSpare", "SendForApprovalBy", c => c.String());
            AddColumn("FleetManagement.ServiceSpare", "SendForApprovalTime", c => c.DateTime());
            AddColumn("FleetManagement.ServiceSpare", "InventoryItems_ItemCode", c => c.String(maxLength: 128));
            AddColumn("FleetManagement.TimeSheet", "IdealRentalRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.TimeSheet", "ReasonType", c => c.String());
            AddColumn("FleetManagement.TimeSheet", "BreakDownTime", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TimeSheet", "RentType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "Num", c => c.Int(nullable: false, identity: true));
            AddColumn("FleetManagement.VehicleDistribution", "Num", c => c.Int(nullable: false, identity: true));
            AlterColumn("FleetManagement.Fleets", "FuelStandard", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Fleets", "PreviousOdoMeter", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.Fleets", "CurentOdoMeter", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.Fleets", "PurchasingCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.FleetInsurance", "PremiumCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.TireIssue", "SerialNo", c => c.String(nullable: false));
            AlterColumn("FleetManagement.TireIssue", "KMReading", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.TireIssue", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.TireReturn", "KMReading", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.TireReturn", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.JobOrder", "Odometer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.JobOrder", "InspectorComments", c => c.String());
            AlterColumn("FleetManagement.OutsourceMaintenanceApproval", "Status", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.ServiceSpare", "UnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.ServiceSpare", "Type", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.TimeSheet", "RentalRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.TimeSheet", "KMHrReading", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            CreateIndex("FleetManagement.Fleets", "EquipmentIdentityID");
            CreateIndex("FleetManagement.Fleets", "EquipmentTypeID");
            CreateIndex("FleetManagement.OutsourceMaintenanceApproval", "JobOrderId");
            CreateIndex("FleetManagement.ServiceSpare", "InventoryItems_ItemCode");
            AddForeignKey("FleetManagement.Fleets", "EquipmentIdentityID", "FleetManagement.EquipmentIdentity", "EquipmentIdentityID", cascadeDelete: true);
            AddForeignKey("FleetManagement.Fleets", "EquipmentTypeID", "FleetManagement.EquipmentType", "EquipmentTypeID", cascadeDelete: true);
            AddForeignKey("FleetManagement.OutsourceMaintenanceApproval", "JobOrderId", "FleetManagement.JobOrder", "JobOrderID", cascadeDelete: true);
            AddForeignKey("FleetManagement.ServiceSpare", "InventoryItems_ItemCode", "EXCEED.InventoryItem", "ItemCode");
           // DropColumn("FleetManagement.EquipmentIdentity", "Fleets_FleetsID");
            //DropColumn("FleetManagement.EquipmentType", "Fleets_FleetsID");
            DropColumn("FleetManagement.TireIssue", "Quantity");
            DropColumn("FleetManagement.TireReturn", "Quantity");
            DropColumn("Construction.ProjectJobTitle", "RankId");
            DropColumn("Construction.ProjectJobTitle", "StepId");
            //DropColumn("FleetManagement.SalvageStoreGoodReceiving", "SparePartID");
            DropColumn("FleetManagement.JobOrder", "PlateNum");
            DropColumn("FleetManagement.JobOrder", "LabourCost");
            DropColumn("FleetManagement.JobOrder", "MaterialCost");
            DropColumn("FleetManagement.JobOrder", "ApprovalStatus");
            DropColumn("FleetManagement.ServiceCrew", "Date");
            //DropColumn("FleetManagement.OutsourceMaintenanceApproval", "OutsourceMaintenanceId");
           // DropColumn("FleetManagement.SalvageStore", "SparePartID");
            //DropColumn("FleetManagement.ServiceSpare", "SparePartID");
            //DropColumn("FleetManagement.ServiceSpare", "ServiceSpare_ServiceSpareID");
        }
        
        public override void Down()
        {
            AddColumn("FleetManagement.ServiceSpare", "ServiceSpare_ServiceSpareID", c => c.Guid());
            AddColumn("FleetManagement.ServiceSpare", "SparePartID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.SalvageStore", "SparePartID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.OutsourceMaintenanceApproval", "OutsourceMaintenanceId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.ServiceCrew", "Date", c => c.DateTime());
            AddColumn("FleetManagement.JobOrder", "ApprovalStatus", c => c.Int(nullable: false));
            AddColumn("FleetManagement.JobOrder", "MaterialCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.JobOrder", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.JobOrder", "PlateNum", c => c.String());
            AddColumn("FleetManagement.SalvageStoreGoodReceiving", "SparePartID", c => c.Guid(nullable: false));
            AddColumn("Construction.ProjectJobTitle", "StepId", c => c.Int());
            AddColumn("Construction.ProjectJobTitle", "RankId", c => c.Int());
            AddColumn("FleetManagement.TireReturn", "Quantity", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireIssue", "Quantity", c => c.Int());
            AddColumn("FleetManagement.EquipmentType", "Fleets_FleetsID", c => c.Guid());
            AddColumn("FleetManagement.EquipmentIdentity", "Fleets_FleetsID", c => c.Guid());
            DropForeignKey("FleetManagement.ServiceSpare", "InventoryItems_ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("FleetManagement.OutsourceMaintenanceApproval", "JobOrderId", "FleetManagement.JobOrder");
            DropForeignKey("FleetManagement.EquipmentTyreLog", "Fleet_FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.EquipmentBatteryLog", "Fleet_FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.EquipmentArrivalApproval", "EquipmentArrivalId", "FleetManagement.EquipmentArrival");
            DropForeignKey("FleetManagement.EquipmentArrival", "FleetsId", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.Fleets", "EquipmentTypeID", "FleetManagement.EquipmentType");
            DropForeignKey("FleetManagement.Fleets", "EquipmentIdentityID", "FleetManagement.EquipmentIdentity");
            DropIndex("FleetManagement.ServiceSpare", new[] { "InventoryItems_ItemCode" });
            DropIndex("FleetManagement.OutsourceMaintenanceApproval", new[] { "JobOrderId" });
            DropIndex("FleetManagement.EquipmentTyreLog", new[] { "Fleet_FleetsID" });
            DropIndex("FleetManagement.EquipmentBatteryLog", new[] { "Fleet_FleetsID" });
            DropIndex("FleetManagement.EquipmentArrivalApproval", new[] { "EquipmentArrivalId" });
            DropIndex("FleetManagement.EquipmentArrival", new[] { "FleetsId" });
            DropIndex("FleetManagement.Fleets", new[] { "EquipmentTypeID" });
            DropIndex("FleetManagement.Fleets", new[] { "EquipmentIdentityID" });
            AlterColumn("FleetManagement.TimeSheet", "KMHrReading", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.TimeSheet", "RentalRate", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("FleetManagement.ServiceSpare", "Type", c => c.String());
            AlterColumn("FleetManagement.ServiceSpare", "UnitPrice", c => c.Int());
            AlterColumn("FleetManagement.OutsourceMaintenanceApproval", "Status", c => c.String());
            AlterColumn("FleetManagement.JobOrder", "InspectorComments", c => c.String(nullable: false));
            AlterColumn("FleetManagement.JobOrder", "Odometer", c => c.Int());
            AlterColumn("FleetManagement.TireReturn", "UnitPrice", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.TireReturn", "KMReading", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.TireIssue", "UnitPrice", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.TireIssue", "KMReading", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.TireIssue", "SerialNo", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.FleetInsurance", "PremiumCost", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.Fleets", "PurchasingCost", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("FleetManagement.Fleets", "CurentOdoMeter", c => c.Int());
            AlterColumn("FleetManagement.Fleets", "PreviousOdoMeter", c => c.Int());
            AlterColumn("FleetManagement.Fleets", "FuelStandard", c => c.String());
            DropColumn("FleetManagement.VehicleDistribution", "Num");
            DropColumn("FleetManagement.VehicleDemand", "Num");
            DropColumn("FleetManagement.TimeSheet", "RentType");
            DropColumn("FleetManagement.TimeSheet", "BreakDownTime");
            DropColumn("FleetManagement.TimeSheet", "ReasonType");
            DropColumn("FleetManagement.TimeSheet", "IdealRentalRate");
            DropColumn("FleetManagement.ServiceSpare", "InventoryItems_ItemCode");
            DropColumn("FleetManagement.ServiceSpare", "SendForApprovalTime");
            DropColumn("FleetManagement.ServiceSpare", "SendForApprovalBy");
            DropColumn("FleetManagement.ServiceSpare", "IsSendForApproval");
            DropColumn("FleetManagement.ServiceSpare", "OnlineTransferredTime");
            DropColumn("FleetManagement.ServiceSpare", "IsOnlineTransferredBy");
            DropColumn("FleetManagement.ServiceSpare", "IsOnlineTransferred");
            DropColumn("FleetManagement.ServiceSpare", "OnlineApprovedTime");
            DropColumn("FleetManagement.ServiceSpare", "OnlineApprovedBy");
            DropColumn("FleetManagement.ServiceSpare", "IsOnlineApproved");
            DropColumn("FleetManagement.ServiceSpare", "VoidTime");
            DropColumn("FleetManagement.ServiceSpare", "VoidBy");
            DropColumn("FleetManagement.ServiceSpare", "IsVoid");
            DropColumn("FleetManagement.ServiceSpare", "Remark");
            DropColumn("FleetManagement.ServiceSpare", "Status");
            DropColumn("FleetManagement.ServiceSpare", "UsedQuantity");
            DropColumn("FleetManagement.ServiceSpare", "Quantity");
            DropColumn("FleetManagement.ServiceSpare", "InventoryItemId");
            DropColumn("FleetManagement.SalvageStoreApproval", "ApprovedQuantity");
            DropColumn("FleetManagement.SalvageStore", "ApprovedQuantity");
            DropColumn("FleetManagement.SalvageStore", "Quantity");
            DropColumn("FleetManagement.SalvageStore", "Price");
            DropColumn("FleetManagement.SalvageStore", "SparePartName");
            DropColumn("FleetManagement.SalvageStore", "SalvageStoreGoodReceivingId");
            DropColumn("FleetManagement.SalvageStore", "JobId");
            DropColumn("FleetManagement.RequestedEquipment", "ManufacturedYear");
            DropColumn("FleetManagement.RentRequest", "Num");
            DropColumn("FleetManagement.OutsourceMaintenanceApproval", "JobOrderId");
            DropColumn("FleetManagement.ServiceCrew", "ActivatedDate");
            DropColumn("FleetManagement.ServiceCrew", "IsActive");
            DropColumn("FleetManagement.MechanicCrew", "SectionId");
            DropColumn("FleetManagement.ServiceTool", "SendForApprovalTime");
            DropColumn("FleetManagement.ServiceTool", "SendForApprovalBy");
            DropColumn("FleetManagement.ServiceTool", "IsSendForApproval");
            DropColumn("FleetManagement.ServiceTool", "OnlineTransferredTime");
            DropColumn("FleetManagement.ServiceTool", "IsOnlineTransferredBy");
            DropColumn("FleetManagement.ServiceTool", "IsOnlineTransferred");
            DropColumn("FleetManagement.ServiceTool", "OnlineApprovedTime");
            DropColumn("FleetManagement.ServiceTool", "OnlineApprovedBy");
            DropColumn("FleetManagement.ServiceTool", "IsOnlineApproved");
            DropColumn("FleetManagement.ServiceTool", "VoidTime");
            DropColumn("FleetManagement.ServiceTool", "VoidBy");
            DropColumn("FleetManagement.ServiceTool", "IsVoid");
            DropColumn("FleetManagement.ServiceTool", "Remark");
            DropColumn("FleetManagement.ServiceTool", "Status");
            DropColumn("FleetManagement.Service", "SectionId");
            DropColumn("FleetManagement.JobOrderDetail", "SecondSendForApprovalTime");
            DropColumn("FleetManagement.JobOrderDetail", "SecondSendForApprovalBy");
            DropColumn("FleetManagement.JobOrderDetail", "IsSecondSendForApproval");
            DropColumn("FleetManagement.JobOrderDetail", "SecondOnlineApprovedTime");
            DropColumn("FleetManagement.JobOrderDetail", "SecondOnlineApprovedBy");
            DropColumn("FleetManagement.JobOrderDetail", "IsSecondOnlineApproved");
            DropColumn("FleetManagement.JobOrderDetail", "IsApproved");
            DropColumn("FleetManagement.JobOrder", "Referance");
            DropColumn("FleetManagement.JobOrder", "CurrentFuel");
            DropColumn("FleetManagement.JobOrder", "EquipmentTypeId");
            DropColumn("FleetManagement.JobOrder", "EquipmentType");
            DropColumn("FleetManagement.JobOrder", "IsExternal");
            DropColumn("FleetManagement.JobOrder", "AssetNum");
            DropColumn("FleetManagement.JobOrder", "OperatorName");
            DropColumn("FleetManagement.JobOrder", "Num");
            DropColumn("FleetManagement.SalvageStoreGoodReceiving", "UnitPrice");
            DropColumn("FleetManagement.SalvageStoreGoodReceiving", "Quantity");
            DropColumn("FleetManagement.SalvageStoreGoodReceiving", "SparePartName");
            DropColumn("FleetManagement.SalvageStoreGoodReceiving", "Num");
            DropColumn("FleetManagement.ExternalMaterial", "SendForApprovalTime");
            DropColumn("FleetManagement.ExternalMaterial", "SendForApprovalBy");
            DropColumn("FleetManagement.ExternalMaterial", "OrgBranchName");
            DropColumn("FleetManagement.ExternalMaterial", "IsSendForApproval");
            DropColumn("FleetManagement.ExternalMaterial", "OnlineTransferredTime");
            DropColumn("FleetManagement.ExternalMaterial", "IsOnlineTransferredBy");
            DropColumn("FleetManagement.ExternalMaterial", "IsOnlineTransferred");
            DropColumn("FleetManagement.ExternalMaterial", "OnlineApprovedTime");
            DropColumn("FleetManagement.ExternalMaterial", "OnlineApprovedBy");
            DropColumn("FleetManagement.ExternalMaterial", "IsOnlineApproved");
            DropColumn("FleetManagement.ExternalMaterial", "VoidTime");
            DropColumn("FleetManagement.ExternalMaterial", "VoidBy");
            DropColumn("FleetManagement.ExternalMaterial", "IsVoid");
            DropColumn("FleetManagement.ExternalMaterial", "Remark");
            DropColumn("FleetManagement.ExternalMaterial", "Status");
            DropColumn("FleetManagement.ExternalMaterial", "TotalAmount");
            DropColumn("FleetManagement.ExternalMaterial", "Quantity");
            DropColumn("FleetManagement.ExternalMaterial", "UnitPrice");
            DropColumn("FleetManagement.ExternalMaterial", "Type");
            DropColumn("FleetManagement.EquipmentTyre", "OdoMeter");
            DropColumn("FleetManagement.EquipmentTyre", "Tube");
            DropColumn("FleetManagement.EquipmentTyre", "MakeCountry");
            DropColumn("FleetManagement.EquipmentTyre", "StoreCode");
            DropColumn("FleetManagement.AssignedEquipment", "IsArrived");
            DropColumn("FleetManagement.TireReturn", "StoreCode");
            DropColumn("FleetManagement.TireReturn", "FleetId");
            DropColumn("FleetManagement.TireReturn", "TireId");
            DropColumn("FleetManagement.TireIssue", "StoreCode");
            DropColumn("FleetManagement.TireIssue", "FleetId");
            DropColumn("FleetManagement.TireIssue", "EquipmentTireId");
            DropColumn("FleetManagement.TireIssue", "TireReturnId");
            DropColumn("FleetManagement.EquipmentType", "FixedAssetCategoryID");
            DropColumn("FleetManagement.Fleets", "BookValue");
            DropColumn("FleetManagement.Fleets", "IdealRentalRate");
            DropColumn("FleetManagement.Fleets", "RentType");
            DropTable("FleetManagement.WorkUnitSection");
            DropTable("FleetManagement.MechanicOnHand");
            DropTable("FleetManagement.MaintenanceType");
            DropTable("FleetManagement.MaintenaceInspectionLog");
            DropTable("FleetManagement.JobStoreRequisition");
            DropTable("FleetManagement.JobRequest");
            DropTable("FleetManagement.JobCauses");
            DropTable("FleetManagement.InventorySparePartMapping");
            DropTable("FleetManagement.HRFleetMapping");
            DropTable("FleetManagement.FuelConsumptionAlertLevel");
            DropTable("FleetManagement.FollowUp");
            DropTable("FleetManagement.ExternalComponentDetail");
            DropTable("FleetManagement.ExternalComponent");
            DropTable("FleetManagement.EquipmentTyreLog");
            DropTable("FleetManagement.EquipmentBatteryLog");
            DropTable("FleetManagement.EquipmentBattery");
            DropTable("FleetManagement.EquipmentArrivalApproval");
            DropTable("FleetManagement.EquipmentArrival");
            DropTable("FleetManagement.BatteryReturn");
            DropTable("FleetManagement.BatteryIssue");
            DropTable("FleetManagement.BatteryApproval");
            CreateIndex("FleetManagement.ServiceSpare", "ServiceSpare_ServiceSpareID");
            CreateIndex("FleetManagement.ServiceSpare", "SparePartID");
            CreateIndex("FleetManagement.SalvageStore", "SparePartID");
            CreateIndex("FleetManagement.OutsourceMaintenanceApproval", "OutsourceMaintenanceId");
            CreateIndex("FleetManagement.JobOrderDetail", "JobOrderID");
            CreateIndex("FleetManagement.SalvageStoreGoodReceiving", "SparePartID");
            CreateIndex("Construction.ProjectJobTitle", "StepId");
            CreateIndex("Construction.ProjectJobTitle", "RankId");
            CreateIndex("Construction.ProjectEstmnIndexLabour", "ProjectJobTitleId");
            CreateIndex("FleetManagement.EquipmentType", "Fleets_FleetsID");
            CreateIndex("FleetManagement.EquipmentIdentity", "Fleets_FleetsID");
            AddForeignKey("FleetManagement.ServiceSpare", "SparePartID", "FleetManagement.SparePart", "SparePartID", cascadeDelete: true);
            AddForeignKey("FleetManagement.ServiceSpare", "ServiceSpare_ServiceSpareID", "FleetManagement.ServiceSpare", "ServiceSpareID");
            AddForeignKey("FleetManagement.SalvageStore", "SparePartID", "FleetManagement.SparePart", "SparePartID", cascadeDelete: true);
            AddForeignKey("FleetManagement.OutsourceMaintenanceApproval", "OutsourceMaintenanceId", "FleetManagement.OutsourceMaintenance", "OutsourceMaintenanceID", cascadeDelete: true);
            AddForeignKey("FleetManagement.JobOrderDetail", "JobOrderID", "FleetManagement.JobOrder", "JobOrderID", cascadeDelete: true);
            AddForeignKey("FleetManagement.SalvageStoreGoodReceiving", "SparePartID", "FleetManagement.SparePart", "SparePartID", cascadeDelete: true);
            AddForeignKey("Construction.ProjectEstmnIndexLabour", "ProjectJobTitleId", "Construction.ProjectJobTitle", "ProjectJobTitleId", cascadeDelete: true);
            AddForeignKey("Construction.ProjectJobTitle", "StepId", "HRM.Step", "StepId");
            AddForeignKey("Construction.ProjectJobTitle", "RankId", "HRM.Rank", "RankId");
            AddForeignKey("FleetManagement.EquipmentType", "Fleets_FleetsID", "FleetManagement.Fleets", "FleetsID");
            AddForeignKey("FleetManagement.EquipmentIdentity", "Fleets_FleetsID", "FleetManagement.Fleets", "FleetsID");
            MoveTable(name: "CRM.LineCreditSettlement", newSchema: "EXCEED");
        }
    }
}
