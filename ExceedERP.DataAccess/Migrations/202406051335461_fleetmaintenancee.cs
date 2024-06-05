namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetmaintenancee : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "FleetManagement.FleetInsurance", newName: "FleetInsurances");
            RenameTable(name: "FleetManagement.JobRequest", newName: "JobRequests");
            MoveTable(name: "FleetManagement.AdministrativeJobApproval", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.AdvanceMeasure", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.DailyCareCategory", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.DailyCareDetailItem", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.DailyCareDetailLog", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.DailyCare", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.DailyCareLog", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ExternalComponent", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ExternalComponentDetail", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ExternalCrew", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ExternalMaterial", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.FailureReason", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.FollowUp", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.InventorySparePartMapping", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.JobCauses", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.JobOrder", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.JobOrderDetail", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.Service", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.Planner", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.RepairType", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.WorkUnitSection", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.JobRequests", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.JobStoreRequisition", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.LogisticPlanDetail", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.LogisticPlan", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.MaintenaceInspectionLog", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.MaintenanceCenter", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.MaintenanceTool", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ServiceTool", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.MaintenanceType", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.MechanicCrew", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ServiceCrew", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.MechanicOnHand", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.OtherCompanyMaintenaceApproval", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.OtherCompanyMaintenance", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.OutsourceMaintenance", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.OutsourceMaintenanceApproval", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.PMScheduleDetail", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.PMSchedule", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ServiceCrewWorkLog", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.ServiceSpare", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.SparePart", newSchema: "Maintenance");
            MoveTable(name: "FleetManagement.TechnicalJobApproval", newSchema: "Maintenance");
            DropForeignKey("FleetManagement.CheckList", "EquipmentTypeId", "FleetManagement.EquipmentIdentity");
            DropForeignKey("FleetManagement.DriverTransferApproval", "DriverVehicleTransferId", "FleetManagement.DriverVehicleTransfer");
            DropForeignKey("FleetManagement.EquipmentArrival", "FleetsId", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.JobOrder", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.JobOrderDetail", "ServiceID", "FleetManagement.Service");
            DropForeignKey("FleetManagement.LogisticPlanDetail", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("FleetManagement.RentOtherSupplierApproval", "RentOtherSupplier_RentOtherSuppliersID", "FleetManagement.RentOtherSuppliers");
            DropForeignKey("FleetManagement.ServiceCrewWorkLog", "JobOrderDetailId", "FleetManagement.JobOrderDetail");
            DropForeignKey("FleetManagement.ServiceSpare", "InventoryItems_ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("FleetManagement.Operator", "FleetsID", "FleetManagement.Fleets");
            DropIndex("FleetManagement.Operator", new[] { "FleetsID" });
            DropIndex("FleetManagement.CheckList", new[] { "EquipmentTypeId" });
            DropIndex("FleetManagement.DriverTransferApproval", new[] { "DriverVehicleTransferId" });
            DropIndex("FleetManagement.EquipmentArrival", new[] { "FleetsId" });
            DropIndex("FleetManagement.FleetIFuelConsumption", new[] { "EquipmentTypeId" });
            DropIndex("Maintenance.JobOrder", new[] { "FleetsID" });
            DropIndex("Maintenance.JobOrderDetail", new[] { "ServiceID" });
            DropIndex("Maintenance.LogisticPlanDetail", new[] { "ItemCode" });
            DropIndex("FleetManagement.RentOtherSupplierApproval", new[] { "RentOtherSupplier_RentOtherSuppliersID" });
            DropIndex("Maintenance.ServiceCrewWorkLog", new[] { "JobOrderDetailId" });
            DropIndex("Maintenance.ServiceSpare", new[] { "InventoryItems_ItemCode" });
            //DropPrimaryKey("FleetManagement.FleetInsurances");
            CreateTable(
                "Maintenance.AdditionalWorkShopDetail",
                c => new
                    {
                        AdditionalWorkShopDetailId = c.Int(nullable: false, identity: true),
                        OtherCosts = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        ServiceCenterJobId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.AdditionalWorkShopDetailId);
            
            CreateTable(
                "FleetManagement.AssignedEquipmentAdditionalInfo",
                c => new
                    {
                        AssignedEquipmentAdditionalInfoId = c.Int(nullable: false, identity: true),
                        AssignedEquipmentId = c.Guid(),
                        PlateNo = c.String(),
                        KmTravelledPerLitre = c.Single(nullable: false),
                        KmTravelledTotal = c.Single(nullable: false),
                        KmInterSite = c.Single(nullable: false),
                        KmGrandTotal = c.Single(nullable: false),
                        ReqFuelInLiter = c.Single(nullable: false),
                        TotalInLitre = c.Single(nullable: false),
                        FromDeppot = c.Single(nullable: false),
                        InCash = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FromProject = c.Single(nullable: false),
                        OilReqForEngine = c.Single(nullable: false),
                        OilReqForOther = c.Single(nullable: false),
                        GreaseReq = c.Single(nullable: false),
                        TotalBirr = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPerdium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InCashLiter = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.AssignedEquipmentAdditionalInfoId);
            
            CreateTable(
                "EXCEED.CostSheetApproval",
                c => new
                    {
                        CostSheetApprovalId = c.Int(nullable: false, identity: true),
                        IsApproved = c.Boolean(nullable: false),
                        CostSheetId = c.Int(),
                        Status = c.String(),
                        Remark = c.String(),
                        TimeSheetType = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.CostSheetApprovalId);
            
            CreateTable(
                "FleetManagement.DryLoadingTimeSheet",
                c => new
                    {
                        DryLoadingTimeSheetId = c.Int(nullable: false, identity: true),
                        ProcurementSupplierID = c.Int(),
                        WorkItem = c.String(nullable: false),
                        WorkingHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TripStartPlace = c.String(nullable: false),
                        TriplArrivalPlace = c.String(nullable: false),
                        TripStartDate = c.DateTime(nullable: false, storeType: "date"),
                        TripArrivalDate = c.DateTime(nullable: false, storeType: "date"),
                        Asphalt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gravel = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalKmTravelled = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelInLiter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PerDiem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReasonForIdle = c.String(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        NetworkingHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TireCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Category = c.Int(nullable: false),
                        NoOfTripe = c.Int(nullable: false),
                        AsphaltRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GravelRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoadedType = c.String(),
                        LoadingUnloadingCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
                        OperatorId = c.Guid(),
                        ExternalOperator = c.String(),
                        TimeSheetSendForAproval = c.Boolean(nullable: false),
                        CostSheetSendForAproval = c.Boolean(nullable: false),
                        TimeSheetSendForApprovalBy = c.String(),
                        CostSheetSendForApprovalBy = c.String(),
                        TimeSheetSendForApprovalTime = c.DateTime(),
                        CostSheetSendForApprovalTime = c.DateTime(),
                        TimeSheetApproved = c.Boolean(nullable: false),
                        CostSheetApproved = c.Boolean(nullable: false),
                        TimeSheetApprovedBy = c.String(),
                        CostSheetApprovedBy = c.String(),
                        TimeSheetApprovedDate = c.DateTime(),
                        CostSheetApprovedDate = c.DateTime(),
                        TimeSheetStatus = c.String(),
                        CostSheetStatus = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DryLoadingTimeSheetId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .ForeignKey("EXCEED.ProcurementSupplier", t => t.ProcurementSupplierID)
                .Index(t => t.ProcurementSupplierID)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "FleetManagement.DumpTruckTimeSheetDetail",
                c => new
                    {
                        DumpTruckTimeSheetDetailId = c.Int(nullable: false, identity: true),
                        LoadingLocationFrom = c.String(nullable: false),
                        LoadingLocationTo = c.String(nullable: false),
                        NumberOfTrip = c.Int(nullable: false),
                        TypeOfWork = c.String(nullable: false),
                        ActualDistance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalHaulage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Capacity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DumpTruckTimeSheetId = c.Int(nullable: false),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.DumpTruckTimeSheetDetailId);
            
            CreateTable(
                "FleetManagement.DumpTruckTimeSheet",
                c => new
                    {
                        DumpTruckTimeSheetId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        TypeOfWorkExcuted = c.String(),
                        ProcurementSupplierID = c.Int(),
                        LoadingLocationFrom = c.String(),
                        LoadingLOactionTo = c.String(),
                        Capacity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ActualDistance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfTrip = c.Int(nullable: false),
                        TotalHaulage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GreaseConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReasonForIdle = c.String(nullable: false),
                        DownHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReasonForDown = c.String(nullable: false),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdealRentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FleetsID = c.Guid(nullable: false),
                        NetworkingHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TireCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
                        OperatorId = c.Guid(),
                        ExternalOperator = c.String(),
                        TimeSheetSendForAproval = c.Boolean(nullable: false),
                        CostSheetSendForAproval = c.Boolean(nullable: false),
                        TimeSheetSendForApprovalBy = c.String(),
                        CostSheetSendForApprovalBy = c.String(),
                        TimeSheetSendForApprovalTime = c.DateTime(),
                        CostSheetSendForApprovalTime = c.DateTime(),
                        TimeSheetApproved = c.Boolean(nullable: false),
                        CostSheetApproved = c.Boolean(nullable: false),
                        TimeSheetApprovedBy = c.String(),
                        CostSheetApprovedBy = c.String(),
                        TimeSheetApprovedDate = c.DateTime(),
                        CostSheetApprovedDate = c.DateTime(),
                        TimeSheetStatus = c.String(),
                        CostSheetStatus = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DumpTruckTimeSheetId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "FleetManagement.EquipmentCheckList",
                c => new
                    {
                        EquipmentCheckListId = c.Int(nullable: false, identity: true),
                        FleetsID = c.Guid(nullable: false),
                        EquipmentLineCheckListId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Code = c.String(),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EquipmentCheckListId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "Maintenance.EquipmentInformation",
                c => new
                    {
                        EquipmentInformationId = c.Guid(nullable: false),
                        GroupName = c.String(),
                        LicensePlate = c.String(),
                        DeviceNumber = c.String(),
                        CustomerId = c.Int(nullable: false),
                        InitialOdometer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OdometerOnLastMaintenance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentOdometer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RegularMaintenanceInterval = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NotifyUpComingMaintenanceBefore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OdometerOnNextRegularMaintenance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RemainingKmForUpComingMaintenance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FutureMaintenanceAlert = c.Int(nullable: false),
                        Status = c.String(),
                        LastMaintained = c.DateTime(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentInformationId);
            
            CreateTable(
                "Maintenance.ExternalServiceCenterPayment",
                c => new
                    {
                        ExternalServiceCenterPaymentId = c.Int(nullable: false, identity: true),
                        CostType = c.String(),
                        Date = c.DateTime(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceCenterJobId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ExternalServiceCenterPaymentId);
            
            CreateTable(
                "FleetManagement.FleetAnnualPlanDetail",
                c => new
                    {
                        FleetAnnualPlanDetailId = c.Int(nullable: false, identity: true),
                        NumberOfEquipmentInMonth = c.Int(nullable: false),
                        WorkingHourPerDay = c.Int(nullable: false),
                        WorkingDaysInMonth = c.Int(nullable: false),
                        WorkingHourPerMonth = c.Int(nullable: false),
                        InefficiencyPercentile = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EfficiencyPercentile = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourForInefficiencyPerMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedEffectiveWorkingHrPerMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChargingRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedMonthlyIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TyreCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LiterPerHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalLiter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCostPerLiter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalFuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perduim = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalExpectedExpense = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedTotalProfit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FleetAnnualPlanId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetAnnualPlanDetailId)
                .ForeignKey("FleetManagement.FleetAnnualPlan", t => t.FleetAnnualPlanId, cascadeDelete: true)
                .Index(t => t.FleetAnnualPlanId);
            
            CreateTable(
                "FleetManagement.FleetAnnualPlan",
                c => new
                    {
                        FleetAnnualPlanId = c.Int(nullable: false, identity: true),
                        EquipmentCategoryId = c.Guid(nullable: false),
                        EquipmentTypeId = c.Guid(nullable: false),
                        EquipmentNameId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
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
                .PrimaryKey(t => t.FleetAnnualPlanId);
            
            CreateTable(
                "FleetManagement.FleetBatteryAndTire",
                c => new
                    {
                        FleetBatteryAndTireId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        FleetBatteryOrTireType = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetBatteryAndTireId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "FleetManagement.FleetBoloRegistration",
                c => new
                    {
                        FleetBoloRegistrationId = c.Int(nullable: false, identity: true),
                        FleetId = c.Guid(nullable: false),
                        BoloRegistrationDate = c.DateTime(nullable: false),
                        BoloExpireDate = c.DateTime(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetBoloRegistrationId);
            
            CreateTable(
                "FleetManagement.FleetCheckList",
                c => new
                    {
                        FleetCheckListId = c.Int(nullable: false, identity: true),
                        EquipmentIdentityID = c.Guid(nullable: false),
                        EquipmentTypeID = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        IsMaintenace = c.Boolean(nullable: false),
                        Code = c.String(),
                        Description = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetCheckListId);
            
            CreateTable(
                "FleetManagement.FleetLineCheckList",
                c => new
                    {
                        FleetLineCheckListId = c.Int(nullable: false, identity: true),
                        FleetCheckListId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Code = c.String(),
                        IsActive = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.FleetLineCheckListId)
                .ForeignKey("FleetManagement.FleetCheckList", t => t.FleetCheckListId, cascadeDelete: true)
                .Index(t => t.FleetCheckListId);
            
            CreateTable(
                "EXCEED.InsuranceCompany",
                c => new
                    {
                        InsuranceCompanyId = c.Int(nullable: false, identity: true),
                        InsuranceCompanyName = c.String(),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
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
                .PrimaryKey(t => t.InsuranceCompanyId);
            
            CreateTable(
                "Maintenance.JobOrderServiceCenterDetail",
                c => new
                    {
                        JobOrderServiceCenterDetailId = c.Guid(nullable: false),
                        ServiceCenterJobId = c.Guid(nullable: false),
                        SubCategoryCode = c.String(),
                        ServiceGroup = c.Int(nullable: false),
                        ServiceID = c.Guid(),
                        ServiceName = c.String(),
                        JobCausesId = c.Guid(nullable: false),
                        RepairTypeID = c.Guid(nullable: false),
                        ApprovalStatus = c.Int(nullable: false),
                        StartingDate = c.DateTime(),
                        ToDate = c.DateTime(),
                        IsApproved = c.Boolean(nullable: false),
                        Odometer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceCenterId = c.Int(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
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
                .PrimaryKey(t => t.JobOrderServiceCenterDetailId)
                .ForeignKey("Maintenance.JobCauses", t => t.JobCausesId, cascadeDelete: true)
                .ForeignKey("Maintenance.RepairType", t => t.RepairTypeID, cascadeDelete: true)
                .Index(t => t.JobCausesId)
                .Index(t => t.RepairTypeID);
            
            CreateTable(
                "FleetManagement.LightVehicleTimesheet",
                c => new
                    {
                        LightVehicleTimeSheetId = c.Int(nullable: false, identity: true),
                        FleetsID = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        MorningTimeStart = c.DateTime(nullable: false),
                        MorningTimeEnd = c.DateTime(nullable: false),
                        AfternoonTimeStart = c.DateTime(nullable: false),
                        AfternoonTimeEnd = c.DateTime(nullable: false),
                        NightTimeStart = c.DateTime(nullable: false),
                        NightTimeEnd = c.DateTime(nullable: false),
                        NumberOfHoles = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HoleDepth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdealRentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GreaseConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleReason = c.String(),
                        DownHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DownReason = c.String(),
                        DownHourRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkType = c.String(),
                        Remark = c.String(),
                        ProcurementSupplierID = c.Int(),
                        OperationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmountBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceRecharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TireCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
                        OperatorId = c.Guid(nullable: false),
                        GreaseRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeSheetSendForAproval = c.Boolean(nullable: false),
                        CostSheetSendForAproval = c.Boolean(nullable: false),
                        TimeSheetSendForApprovalBy = c.String(),
                        CostSheetSendForApprovalBy = c.String(),
                        TimeSheetSendForApprovalTime = c.DateTime(),
                        CostSheetSendForApprovalTime = c.DateTime(),
                        TimeSheetApproved = c.Boolean(nullable: false),
                        CostSheetApproved = c.Boolean(nullable: false),
                        TimeSheetApprovedBy = c.String(),
                        CostSheetApprovedBy = c.String(),
                        TimeSheetApprovedDate = c.DateTime(),
                        CostSheetApprovedDate = c.DateTime(),
                        TimeSheetStatus = c.String(),
                        CostSheetStatus = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.LightVehicleTimeSheetId);
            
            CreateTable(
                "FleetManagement.MachineryWorkingTimeSheet",
                c => new
                    {
                        MachineryWorkingTimeSheetId = c.Int(nullable: false, identity: true),
                        FleetsID = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        MorningTimeStart = c.DateTime(nullable: false),
                        MorningTimeEnd = c.DateTime(nullable: false),
                        AfternoonTimeStart = c.DateTime(nullable: false),
                        AfternoonTimeEnd = c.DateTime(nullable: false),
                        NightTimeStart = c.DateTime(nullable: false),
                        NightTimeEnd = c.DateTime(nullable: false),
                        NumberOfHoles = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HoleDepth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GreaseRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdealRentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GreaseConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleReason = c.String(),
                        DownHours = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DownReason = c.String(),
                        DownHourRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkType = c.String(),
                        Remark = c.String(),
                        ProcurementSupplierID = c.Int(),
                        OperationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmountBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceRecharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TireCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
                        OperatorId = c.Guid(),
                        ExternalOperator = c.String(),
                        TimeSheetSendForAproval = c.Boolean(nullable: false),
                        CostSheetSendForAproval = c.Boolean(nullable: false),
                        TimeSheetSendForApprovalBy = c.String(),
                        CostSheetSendForApprovalBy = c.String(),
                        TimeSheetSendForApprovalTime = c.DateTime(),
                        CostSheetSendForApprovalTime = c.DateTime(),
                        TimeSheetApproved = c.Boolean(nullable: false),
                        CostSheetApproved = c.Boolean(nullable: false),
                        TimeSheetApprovedBy = c.String(),
                        CostSheetApprovedBy = c.String(),
                        TimeSheetApprovedDate = c.DateTime(),
                        CostSheetApprovedDate = c.DateTime(),
                        TimeSheetStatus = c.String(),
                        CostSheetStatus = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MachineryWorkingTimeSheetId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            CreateTable(
                "Maintenance.MaintenanceIntervals",
                c => new
                    {
                        MaintenanceIntervalId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceIntervalId);
            
            CreateTable(
                "Maintenance.MaintenanceToolIssueAndReturn",
                c => new
                    {
                        MaintenanceToolIssueAndReturnId = c.Guid(nullable: false),
                        MaintenanceToolID = c.Guid(nullable: false),
                        MechanicCrewID = c.Guid(nullable: false),
                        IssuedQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IssueDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(),
                        Remark = c.String(),
                        Status = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceToolIssueAndReturnId)
                .ForeignKey("Maintenance.MaintenanceTool", t => t.MaintenanceToolID, cascadeDelete: true)
                .ForeignKey("Maintenance.MechanicCrew", t => t.MechanicCrewID, cascadeDelete: true)
                .Index(t => t.MaintenanceToolID)
                .Index(t => t.MechanicCrewID);
            
            CreateTable(
                "EXCEED.MechanicTypeClass",
                c => new
                    {
                        MechanicTypeClassId = c.Int(nullable: false, identity: true),
                        MechanicTypeTitle = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.MechanicTypeClassId);
            
            CreateTable(
                "EXCEED.PeriodicEquipmentInspection",
                c => new
                    {
                        PeriodicEquipmentInspectionId = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Owner = c.String(),
                        PlateNumber = c.String(),
                        VehicleType = c.String(),
                        Capacity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.PeriodicEquipmentInspectionId);
            
            CreateTable(
                "EXCEED.RentalCompany",
                c => new
                    {
                        RentalCompanyId = c.Int(nullable: false, identity: true),
                        RentalCompanyName = c.String(),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
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
                .PrimaryKey(t => t.RentalCompanyId);
            
            CreateTable(
                "FleetManagement.RentalInspectionCriteria",
                c => new
                    {
                        RentalInspectionCriteriaId = c.Int(nullable: false, identity: true),
                        Index = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CriteriaCategory = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalInspectionCriteriaId);
            
            CreateTable(
                "FleetManagement.RentalInspectionCriteriaDetail",
                c => new
                    {
                        RentalInspectionCriteriaDetailId = c.Int(nullable: false, identity: true),
                        Index = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CriteriaName = c.String(),
                        RentalInspectionCriteriaId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalInspectionCriteriaDetailId)
                .ForeignKey("FleetManagement.RentalInspectionCriteria", t => t.RentalInspectionCriteriaId, cascadeDelete: true)
                .Index(t => t.RentalInspectionCriteriaId);
            
            CreateTable(
                "FleetManagement.RentalInspection",
                c => new
                    {
                        RentalInspectionId = c.Int(nullable: false, identity: true),
                        EquipmentTypeID = c.Guid(nullable: false),
                        EquipmentNameID = c.Guid(),
                        Model = c.String(nullable: false),
                        ChasisSerialNo = c.String(nullable: false),
                        EngineModel = c.String(nullable: false),
                        EngineSerialNo = c.String(nullable: false),
                        ManufacturingYear = c.String(nullable: false),
                        Capacity = c.String(nullable: false),
                        Manufacturer = c.String(),
                        Hp = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperatorLicenceNo = c.String(nullable: false),
                        Lessor = c.String(nullable: false),
                        PlateNo = c.String(nullable: false),
                        BranchId = c.Int(nullable: false),
                        MeterReading = c.String(nullable: false),
                        CCKW = c.String(),
                        IsMiddleOnlineApproved = c.Boolean(nullable: false),
                        MiddleOnlineApprovedBy = c.String(),
                        MiddleOnlineApprovedTime = c.DateTime(),
                        IsMiddleSendForApproval = c.Boolean(nullable: false),
                        MiddleSendForApprovalBy = c.String(),
                        MiddleSendForApprovalTime = c.DateTime(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
                        IsSecondVoid = c.Boolean(nullable: false),
                        SecondVoidBy = c.String(),
                        SecondVoidTime = c.DateTime(),
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
                .PrimaryKey(t => t.RentalInspectionId)
                .ForeignKey("HRM.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("FleetManagement.EquipmentName", t => t.EquipmentNameID)
                .ForeignKey("FleetManagement.EquipmentType", t => t.EquipmentTypeID, cascadeDelete: true)
                .Index(t => t.EquipmentTypeID)
                .Index(t => t.EquipmentNameID)
                .Index(t => t.BranchId);
            
            CreateTable(
                "FleetManagement.RentalInspectionValidation",
                c => new
                    {
                        RentalInspectionValidationId = c.Int(nullable: false, identity: true),
                        RentalInspectionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.RentalInspectionValidationId)
                .ForeignKey("FleetManagement.RentalInspection", t => t.RentalInspectionId, cascadeDelete: true)
                .Index(t => t.RentalInspectionId);
            
            CreateTable(
                "FleetManagement.RentalMVInspection",
                c => new
                    {
                        RentalMVInspectionId = c.Int(nullable: false, identity: true),
                        RentalInspectionCriteriaId = c.Int(nullable: false),
                        RentalInspectionId = c.Int(nullable: false),
                        RentalInspectionCriteriaDetailId = c.Int(nullable: false),
                        Index = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HeadOfficeGoodCondtion = c.Int(nullable: false),
                        HeadOfficePoorCondtion = c.Int(nullable: false),
                        BranchOfficeGoodCondtion = c.Int(nullable: false),
                        BranchOfficePoorCondtion = c.Int(nullable: false),
                        IsMiddleOnlineApproved = c.Boolean(nullable: false),
                        MiddleOnlineApprovedBy = c.String(),
                        MiddleOnlineApprovedTime = c.DateTime(),
                        IsMiddleSendForApproval = c.Boolean(nullable: false),
                        MiddleSendForApprovalBy = c.String(),
                        MiddleSendForApprovalTime = c.DateTime(),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
                        IsSecondVoid = c.Boolean(nullable: false),
                        SecondVoidBy = c.String(),
                        SecondVoidTime = c.DateTime(),
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
                .PrimaryKey(t => t.RentalMVInspectionId)
                .ForeignKey("FleetManagement.RentalInspection", t => t.RentalInspectionId, cascadeDelete: true)
                .ForeignKey("FleetManagement.RentalInspectionCriteria", t => t.RentalInspectionCriteriaId, cascadeDelete: true)
                .Index(t => t.RentalInspectionCriteriaId)
                .Index(t => t.RentalInspectionId);
            
            CreateTable(
                "FleetManagement.RentalPaymentRequestBudgetValidation",
                c => new
                    {
                        RentalPaymentRequestBudgetValidationId = c.Int(nullable: false, identity: true),
                        RentalPaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalPaymentRequestBudgetValidationId)
                .ForeignKey("FleetManagement.RentalPaymentRequest", t => t.RentalPaymentRequestId, cascadeDelete: true)
                .Index(t => t.RentalPaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalPaymentRequest",
                c => new
                    {
                        RentalPaymentRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        ProcurementSupplierID = c.Int(),
                        GLPeriodId = c.Int(nullable: false),
                        GLFiscalYearId = c.Int(nullable: false),
                        DurationFrom = c.DateTime(nullable: false),
                        DurationTo = c.DateTime(nullable: false),
                        OrgStructureId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(nullable: false),
                        ReasonofPayment = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        Refference = c.String(),
                        Paid = c.Boolean(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsBudgetApproved = c.Boolean(nullable: false),
                        BudgetApprovedBy = c.String(),
                        BudgetApprovedTime = c.DateTime(),
                        IsFinanceSendForApproval = c.Boolean(nullable: false),
                        FinanceSendForApprovalBy = c.String(),
                        FinanceSendForApprovalTime = c.DateTime(),
                        IsFinanceApproved = c.Boolean(nullable: false),
                        FinanceApprovedBy = c.String(),
                        FinanceApprovedTime = c.DateTime(),
                        IsDepartmentOnlineApproved = c.Boolean(nullable: false),
                        DepartmentOnlineApprovedBy = c.String(),
                        DepartmentOnlineApprovedTime = c.DateTime(),
                        IsDepartmentSendForApproval = c.Boolean(nullable: false),
                        DepartmentSendForApprovalBy = c.String(),
                        DepartmentSendForApprovalTime = c.DateTime(),
                        ForwardedFrom = c.String(),
                        ForwardedTo = c.String(),
                        IsFinalApproval = c.Boolean(nullable: false),
                        CurrentApprovalStep = c.Int(),
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
                .PrimaryKey(t => t.RentalPaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalPaymentRequestDepartmentValidation",
                c => new
                    {
                        RentalPaymentRequestDepartmentValidationId = c.Int(nullable: false, identity: true),
                        RentalPaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalPaymentRequestDepartmentValidationId)
                .ForeignKey("FleetManagement.RentalPaymentRequest", t => t.RentalPaymentRequestId, cascadeDelete: true)
                .Index(t => t.RentalPaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalPaymentRequestLine",
                c => new
                    {
                        RentalPaymentRequestLineId = c.Int(nullable: false, identity: true),
                        RentalPaymentRequestId = c.Int(nullable: false),
                        EquipmnetTypeId = c.Guid(nullable: false),
                        EquipmentNameId = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GLTaxRateID = c.Int(nullable: false),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubTotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Commission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvancedPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.RentalPaymentRequestLineId)
                .ForeignKey("FleetManagement.RentalPaymentRequest", t => t.RentalPaymentRequestId, cascadeDelete: true)
                .Index(t => t.RentalPaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalPaymentRequestValidation",
                c => new
                    {
                        RentalPaymentRequestValidationId = c.Int(nullable: false, identity: true),
                        RentalPaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalPaymentRequestValidationId)
                .ForeignKey("FleetManagement.RentalPaymentRequest", t => t.RentalPaymentRequestId, cascadeDelete: true)
                .Index(t => t.RentalPaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentFromExternalEquipment",
                c => new
                    {
                        RentFromExternalEquipmentId = c.Int(nullable: false, identity: true),
                        EquipmentIdentityID = c.Guid(nullable: false),
                        EquipmentTypeID = c.Guid(nullable: false),
                        EquipmentNameID = c.Guid(nullable: false),
                        FleetId = c.Guid(nullable: false),
                        RentType = c.Int(nullable: false),
                        RentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleRentalRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentFromExternalId = c.Guid(nullable: false),
                        Status = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentFromExternalEquipmentId);
            
            CreateTable(
                "FleetManagement.RentToExternalApproval",
                c => new
                    {
                        RentToExternalApprovalID = c.Int(nullable: false, identity: true),
                        RentToExternalId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentToExternalApprovalID);
            
            CreateTable(
                "FleetManagement.RentToExternals",
                c => new
                    {
                        RentToExternalID = c.Guid(nullable: false),
                        EquipmentIdentityID = c.Guid(),
                        EquipmentTypeID = c.Guid(),
                        WorkingCapacity = c.String(nullable: false),
                        RentalRateIncludingVAT = c.Boolean(nullable: false),
                        WithDriver = c.Boolean(nullable: false),
                        GoodCondition = c.Boolean(nullable: false),
                        NormalCondition = c.Boolean(nullable: false),
                        BadCondition = c.Boolean(nullable: false),
                        Address = c.String(),
                        Quantity = c.Int(),
                        FuelCoveredBy = c.String(nullable: false),
                        EquipmentName = c.String(),
                        ProjectLocation = c.String(),
                        UnitOfWorkMeasurement = c.String(nullable: false),
                        Description = c.String(),
                        Condition = c.String(nullable: false),
                        NameBankBranch = c.String(),
                        BankFullAddress = c.String(),
                        BankSlipNo = c.String(),
                        AdvancePayment = c.Int(),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.RentToExternalID);
            
            CreateTable(
                "Maintenance.ServiceCenterJob",
                c => new
                    {
                        ServiceCenterJobId = c.Guid(nullable: false),
                        Num = c.Int(nullable: false),
                        JobOrderNum = c.Int(),
                        JobOrderID = c.Guid(nullable: false),
                        IsRework = c.Boolean(nullable: false),
                        FleetsID = c.Guid(nullable: false),
                        EquipmentInformationId = c.Guid(nullable: false),
                        Type = c.String(),
                        EquipmentName = c.String(),
                        OperatorName = c.String(),
                        AssetNum = c.String(),
                        IsExternal = c.Boolean(nullable: false),
                        MaintenanceCenter = c.String(),
                        MaintenanceCenterId = c.Guid(),
                        ServiceCenterId = c.Int(),
                        ProjectName = c.String(),
                        PercentCompleted = c.String(),
                        Odometer = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Site = c.String(),
                        FailureReason = c.String(),
                        AdvanceMeasure = c.String(),
                        EquipmentNo = c.String(),
                        EquipmentType = c.String(),
                        CurrentFuel = c.String(),
                        Referance = c.Int(),
                        DateIssued = c.DateTime(),
                        DateCompleted = c.DateTime(),
                        InspectedBy = c.String(),
                        IsPM = c.Boolean(nullable: false),
                        InspectorComments = c.String(),
                        LicensePlate = c.String(),
                        RejectionReason = c.String(),
                        IsSentToWorkShop = c.Boolean(nullable: false),
                        IsSubmittedToServiceCenter = c.Boolean(nullable: false),
                        OnlineAssignedTime = c.DateTime(),
                        OnlineSubmittedTime = c.DateTime(),
                        WorkShopRejectedReason = c.String(),
                        ServiceCenterStatus = c.Int(nullable: false),
                        IsExternalMaintenanceCenter = c.Boolean(nullable: false),
                        IsInsured = c.Boolean(nullable: false),
                        IsSecondOnlineApproved = c.Boolean(nullable: false),
                        SecondOnlineApprovedBy = c.String(),
                        SecondOnlineApprovedTime = c.DateTime(),
                        IsSecondSendForApproval = c.Boolean(nullable: false),
                        SecondSendForApprovalBy = c.String(),
                        SecondSendForApprovalTime = c.DateTime(),
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
                .PrimaryKey(t => t.ServiceCenterJobId);
            
            CreateTable(
                "Maintenance.ServiceCenter",
                c => new
                    {
                        ServiceCenterId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        MaintenanceCenterId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceCenterId);
            
            CreateTable(
                "Maintenance.SpareMaterialCategory",
                c => new
                    {
                        SpareMaterialCategoryId = c.Guid(nullable: false),
                        Description = c.String(),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SpareMaterialCategoryId);
            
            CreateTable(
                "EXCEED.TimeSheetApprovalNew",
                c => new
                    {
                        TimeSheetApprovalNewId = c.Int(nullable: false, identity: true),
                        IsApproved = c.Boolean(nullable: false),
                        TimeSheetId = c.Int(),
                        Status = c.String(),
                        Remark = c.String(),
                        TimeSheetType = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TimeSheetApprovalNewId);
            
            CreateTable(
                "Maintenance.VehiclePart",
                c => new
                    {
                        VehiclePartId = c.Int(nullable: false, identity: true),
                        VehicleCategoryId = c.Guid(),
                        Code = c.String(),
                        Name = c.String(),
                        Remark = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehiclePartId);
            
            CreateTable(
                "Maintenance.VehiclePartDamageType",
                c => new
                    {
                        VehiclePartDamageTypeId = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Remark = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehiclePartDamageTypeId);
            
            CreateTable(
                "Maintenance.VehiclePartManagement",
                c => new
                    {
                        VehiclePartManagementId = c.Int(nullable: false, identity: true),
                        VehiclePartId = c.Int(nullable: false),
                        VehiclePartDamageTypeId = c.Int(),
                        VehicleReceptionFormId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehiclePartManagementId);
            
            CreateTable(
                "Maintenance.VehicleReceptionForm",
                c => new
                    {
                        VehicleReceptionFormId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(),
                        RecipientId = c.Int(nullable: false),
                        VehicleCategoryId = c.Guid(),
                        TechnicianAssignedId = c.Int(),
                        PlateNo = c.String(),
                        VehicleType = c.String(),
                        VehicleModel = c.String(),
                        Mileage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Fuel = c.Int(nullable: false),
                        Remark = c.String(),
                        Source = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.VehicleReceptionFormId);
            
            CreateTable(
                "FleetManagement.WagonDrillTimesheet",
                c => new
                    {
                        WagonDrillTimesheetId = c.Int(nullable: false, identity: true),
                        ProcurementSupplierID = c.Int(),
                        FleetsID = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        WorkItem = c.String(),
                        OperationNetWorkingHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperationDrilledHoleDepth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfHolesDrilled = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperationRateForMeter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleRateHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DownTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmountBeforeVAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelInLiter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceCharge = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LubricantCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TireCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Perdium = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Salary = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReasonForDown = c.String(),
                        ReasonForIdle = c.String(),
                        BranchId = c.Int(nullable: false),
                        BusinessUnitId = c.Int(),
                        OperatorId = c.Guid(),
                        ExternalOperator = c.String(),
                        RateInMeter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeSheetSendForAproval = c.Boolean(nullable: false),
                        CostSheetSendForAproval = c.Boolean(nullable: false),
                        TimeSheetSendForApprovalBy = c.String(),
                        CostSheetSendForApprovalBy = c.String(),
                        TimeSheetSendForApprovalTime = c.DateTime(),
                        CostSheetSendForApprovalTime = c.DateTime(),
                        TimeSheetApproved = c.Boolean(nullable: false),
                        CostSheetApproved = c.Boolean(nullable: false),
                        TimeSheetApprovedBy = c.String(),
                        CostSheetApprovedBy = c.String(),
                        TimeSheetApprovedDate = c.DateTime(),
                        CostSheetApprovedDate = c.DateTime(),
                        TimeSheetStatus = c.String(),
                        CostSheetStatus = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WagonDrillTimesheetId)
                .ForeignKey("FleetManagement.Fleets", t => t.FleetsID, cascadeDelete: true)
                .Index(t => t.FleetsID);
            
            AddColumn("FleetManagement.Accident", "OperatorId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.Accident", "TimeOfAccident", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.Accident", "OtherPartyFullName", c => c.String());
            AddColumn("FleetManagement.Accident", "OtherPartyAddress", c => c.String());
            AddColumn("FleetManagement.Accident", "PoliceReportDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.Accident", "PoliceReportAddress", c => c.String());
            AddColumn("FleetManagement.Fleets", "AssetId", c => c.Int());
            AddColumn("FleetManagement.Fleets", "EquipmentNameID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.Fleets", "RentFromExternalNum", c => c.Int());
            AddColumn("FleetManagement.Fleets", "WayOfSupplyenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "manufaturerenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "Colorenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "yearenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "TypeOfFuelenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "StateOfOdoMeterenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "HorsePower", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.EquipmentIdentity", "FixedAssetCategoryId", c => c.Int());
            AddColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentNameId", c => c.String());
            AddColumn("FleetManagement.EquipmentType", "FixedAssetSubCategoryId", c => c.Int());
            AddColumn("FleetManagement.EquipmentType", "TimeSheetType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "InsuranceCompanyId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "PolicyNo", c => c.String());
            AddColumn("FleetManagement.FleetInsurances", "HazardOfExpiryDate", c => c.DateTime());
            AddColumn("FleetManagement.FleetInsurances", "ThirdParty", c => c.String());
            AddColumn("FleetManagement.FleetInsurances", "InsuranceType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "ThirdPartyInsurance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.FleetInsurances", "ComprehensiveInsurance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.Operator", "IsExternal", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Operator", "TypeofEmploymentenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Operator", "Postionenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Operator", "Genderenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Operator", "SalaryIncentiveenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Operator", "LicenseTypeenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Operator", "LicenseLevelenum", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Operator", "COCResult", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireIssue", "EquipmentIdentityID", c => c.Guid());
            AddColumn("FleetManagement.TireIssue", "EquipmentTypeID", c => c.Guid());
            AddColumn("FleetManagement.TireIssue", "EquipmentNameID", c => c.Guid());
            AddColumn("FleetManagement.TireIssue", "IssueType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireIssue", "IssueId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireIssue", "IssueItemId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireReturn", "ReturnType", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireReturn", "ReturnId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.TireReturn", "ReturnItemId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "RequestedEquipmentId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "EquipmentNameID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "OperatorId", c => c.Guid());
            AddColumn("FleetManagement.AssignedEquipment", "FleetId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "NumberHourPerKm", c => c.String());
            AddColumn("FleetManagement.AssignedEquipment", "Destination", c => c.String());
            AddColumn("FleetManagement.AssignedEquipment", "NumberofPersonOrTon", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.AssignedEquipment", "StartOdometr", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.AssignedEquipment", "EndOdometer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.AssignedEquipment", "NetOdometer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.AssignedEquipment", "Returned", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "ReturnDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "VehicleDemandId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.CheckList", "EquipmentIdentityID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverAssignment", "BranchId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.DriverVehicleTransfer", "FleetId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.DriverTransferApproval", "DriverAssignmentId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.EquipmentArrival", "EquipmentIdentityId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.EquipmentArrival", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.EquipmentArrival", "EquipmentNameId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.EquipmentName", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.EquipmentName", "FixedAssetSubCategoryNameId", c => c.Int());
            AddColumn("FleetManagement.ExternalRentedEquipment", "EquipmentIdentityID", c => c.Guid());
            AddColumn("FleetManagement.ExternalRentedEquipment", "EquipmentTypeID", c => c.Guid());
            AddColumn("FleetManagement.ExternalRentedEquipment", "EquipmentNameID", c => c.Guid());
            AddColumn("FleetManagement.FleetIFuelConsumption", "UoMName", c => c.String(maxLength: 128));
            AddColumn("Maintenance.FollowUp", "ServiceCrewWorkLogId", c => c.Guid(nullable: false));
            AddColumn("Maintenance.JobOrder", "IsRework", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrder", "EquipmentInformationId", c => c.Guid(nullable: false));
            AddColumn("Maintenance.JobOrder", "MaintenanceCenterId", c => c.Guid());
            AddColumn("Maintenance.JobOrder", "LicensePlate", c => c.String());
            AddColumn("Maintenance.JobOrder", "RejectionReason", c => c.String());
            AddColumn("Maintenance.JobOrder", "IsSentToWorkShop", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrder", "IsSubmittedToServiceCenter", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrder", "OnlineAssignedTime", c => c.DateTime());
            AddColumn("Maintenance.JobOrder", "OnlineSubmittedTime", c => c.DateTime());
            AddColumn("Maintenance.JobOrder", "WorkShopRejectedReason", c => c.String());
            AddColumn("Maintenance.JobOrder", "ServiceCenterStatus", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "BranchId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "RequesterId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrder", "IsInsured", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrderDetail", "SubCategoryCode", c => c.String());
            AddColumn("Maintenance.JobOrderDetail", "ServiceGroup", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobOrderDetail", "RepairTypeID", c => c.Guid(nullable: false));
            AddColumn("Maintenance.JobOrderDetail", "ServiceCenterId", c => c.Int());
            AddColumn("Maintenance.RepairType", "CategoryCode", c => c.String(nullable: false));
            AddColumn("Maintenance.RepairType", "ServiceGroup", c => c.Int(nullable: false));
            AddColumn("Maintenance.RepairType", "SubCategoryCode", c => c.String(nullable: false));
            AddColumn("Maintenance.RepairType", "StandaredTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "AutomotiveStandaredHour", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "PayableTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "Difference", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "LabourRateAtCompany", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "LabourRateAtCustomer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "TotalAmountCompany", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "TotalAmountCustomer", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.RepairType", "Remark", c => c.String());
            AddColumn("Maintenance.RepairType", "Reason", c => c.String());
            AddColumn("Maintenance.RepairType", "Place", c => c.String());
            AddColumn("Maintenance.RepairType", "Time", c => c.String());
            AddColumn("Maintenance.RepairType", "Process", c => c.String());
            AddColumn("Maintenance.JobRequests", "VeheicleReceptionFormId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "CustomerId", c => c.Int());
            AddColumn("Maintenance.JobRequests", "Source", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "EquipmentInformationId", c => c.Guid());
            AddColumn("Maintenance.JobRequests", "LicensePlate", c => c.String());
            AddColumn("Maintenance.JobRequests", "MaintenanceCenter", c => c.String());
            AddColumn("Maintenance.JobRequests", "MaintenanceCenterId", c => c.Guid(nullable: false));
            AddColumn("Maintenance.JobRequests", "RequesterId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "BranchId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "CreatedAtBranchId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "CreatedAtBusinessUnitId", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "HasInsurance", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobRequests", "IsHighPriority", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobRequests", "JobOrderStatus", c => c.Int(nullable: false));
            AddColumn("Maintenance.JobRequests", "DeliveredDate", c => c.DateTime());
            AddColumn("FleetManagement.LicenseLevel", "OperatorId", c => c.Guid(nullable: false));
            AddColumn("Maintenance.MaintenanceCenter", "IsExternal", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.MaintenanceCenter", "BusinessUnitId", c => c.Int());
            AddColumn("Maintenance.MaintenanceCenter", "BranchId", c => c.Int(nullable: false));
            AddColumn("Maintenance.MechanicCrew", "MechanicType", c => c.Int(nullable: false));
            AddColumn("Maintenance.MechanicCrew", "MechanicTypeClassId", c => c.Int(nullable: false));
            AddColumn("Maintenance.MechanicCrew", "Postionenum", c => c.Int(nullable: false));
            AddColumn("Maintenance.ServiceCrew", "MechanicType", c => c.Int(nullable: false));
            AddColumn("Maintenance.ServiceCrew", "TotalLabourHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceCrew", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceCrew", "TotalLabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentFromExternal", "ProcurementSupplierID", c => c.Int(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "BranchId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "Hour", c => c.Double(nullable: false));
            AddColumn("FleetManagement.RentFromExternal", "HP_CC", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "EquipmentIdentityID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "EquipmentTypeID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "EquipmentNameId", c => c.Guid());
            AddColumn("FleetManagement.RequestedEquipment", "NumberHourPerKm", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "Destination", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "NumberofPersonOrTon", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "NumberHourKM", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "NumberofPersonston", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "Remarks", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "VehicleDemandId", c => c.Int(nullable: false));
            AddColumn("Maintenance.ServiceCrewWorkLog", "SubCategoryCode", c => c.String());
            AddColumn("Maintenance.ServiceCrewWorkLog", "ServiceGroup", c => c.Int(nullable: false));
            AddColumn("Maintenance.ServiceCrewWorkLog", "WorkShopType", c => c.Int(nullable: false));
            AddColumn("Maintenance.ServiceCrewWorkLog", "HourlyWage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceCrewWorkLog", "TotalLabourHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceCrewWorkLog", "StandaredHours", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceCrewWorkLog", "TotalLabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Maintenance.ServiceSpare", "SparePartID", c => c.String());
            AddColumn("Maintenance.ServiceSpare", "SpareMaterialCategoryId", c => c.String());
            AddColumn("Maintenance.ServiceSpare", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.SitesVehicleTransfer", "EquipmentIdentityID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "EquipmentTypeID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "EquipmentNameID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "BranchId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "OperatorId", c => c.Guid(nullable: false));
            AddColumn("Maintenance.SparePart", "SpareMaterialCategoryId", c => c.String());
            AddColumn("Maintenance.SparePart", "SpareMaterialCategory_SpareMaterialCategoryId", c => c.Guid());
            AddColumn("FleetManagement.VehicleDemand", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "BranchId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "BusinessUnitId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "ReasonForTravel", c => c.String(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "RequestDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "IsAssigned", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "OrgStructureId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.VehicleDemand", "ForwardedTo", c => c.String());
            AddColumn("FleetManagement.VehicleDemand", "ForwardedFrom", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "PlateNumber", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "ManufacturingYear", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "EmployeeId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.VehicleDistribution", "ReasonForTravel", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "RequestDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.VehicleDistribution", "AssignmentDate", c => c.DateTime(nullable: false));
            AddColumn("FleetManagement.VehicleDistribution", "IsAssigned", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.VehicleDistribution", "Reference", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "NumberHourKM", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "EquipmentType", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "EquipmentName", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "Destination", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "NumberofPersonston", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.VehicleDistribution", "Remarks", c => c.String());
            AddColumn("FleetManagement.VehicleDistributionApproval", "ApprovedBy", c => c.String());
            AlterColumn("FleetManagement.Fleets", "UnitNum", c => c.String());
           // AlterColumn("FleetManagement.FleetInsurances", "FleetInsuranceId", c => c.Int(nullable: false, identity: true));
            AlterColumn("FleetManagement.FleetInsurances", "DueDate", c => c.DateTime());
            AlterColumn("FleetManagement.FleetInsurances", "ExpireDate", c => c.DateTime());
            AlterColumn("FleetManagement.Operator", "FleetsID", c => c.Guid());
            AlterColumn("FleetManagement.TireIssue", "FleetId", c => c.Guid());
            AlterColumn("FleetManagement.TireIssue", "TireType", c => c.String());
            AlterColumn("FleetManagement.AssignedEquipment", "EquipmentName", c => c.String());
            AlterColumn("FleetManagement.DriverAssignment", "ToDate", c => c.DateTime());
            AlterColumn("FleetManagement.EquipmentArrival", "MeansOfTravelling", c => c.Int(nullable: false));
            AlterColumn("FleetManagement.EquipmentName", "Name", c => c.String());
            AlterColumn("Maintenance.JobOrder", "ProjectName", c => c.String());
            //AlterColumn("Maintenance.LogisticPlanDetail", "ItemCode", c => c.String());
            AlterColumn("Maintenance.MaintenanceTool", "Location", c => c.String());
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursMorningFrom", c => c.DateTime());
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursMorningto", c => c.DateTime());
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursAfternoonFrom", c => c.DateTime());
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursAfternoonTo", c => c.DateTime());
            AlterColumn("FleetManagement.SitesVehicleTransfer", "InitialSite", c => c.String());
            AlterColumn("FleetManagement.SitesVehicleTransfer", "DestinationSite", c => c.String());
            AlterColumn("Maintenance.SparePart", "PartNo", c => c.String());
            AlterColumn("Maintenance.SparePart", "ReceiptNo", c => c.Int());
            //AddPrimaryKey("FleetManagement.FleetInsurances", "FleetInsuranceId");
            CreateIndex("FleetManagement.Operator", "FleetsID");
            CreateIndex("FleetManagement.FleetIFuelConsumption", "EquipmentTypeID");
            CreateIndex("FleetManagement.FleetIFuelConsumption", "UoMName");
            CreateIndex("Maintenance.JobOrderDetail", "RepairTypeID");
            CreateIndex("Maintenance.MechanicCrew", "EmployeeId");
            CreateIndex("Maintenance.SparePart", "SpareMaterialCategory_SpareMaterialCategoryId");
            AddForeignKey("FleetManagement.FleetIFuelConsumption", "UoMName", "EXCEED.UoM", "UoMName");
            AddForeignKey("Maintenance.JobOrderDetail", "RepairTypeID", "Maintenance.RepairType", "RepairTypeID", cascadeDelete: false);
            AddForeignKey("Maintenance.MechanicCrew", "EmployeeId", "HRM.Employee", "EmployeeId", cascadeDelete: false);
            AddForeignKey("Maintenance.SparePart", "SpareMaterialCategory_SpareMaterialCategoryId", "Maintenance.SpareMaterialCategory", "SpareMaterialCategoryId");
            AddForeignKey("FleetManagement.Operator", "FleetsID", "FleetManagement.Fleets", "FleetsID");
            DropColumn("FleetManagement.Accident", "Num");
            DropColumn("FleetManagement.FleetInsurances", "InsuranceCompany");
            DropColumn("FleetManagement.FleetInsurances", "PhoneNo");
            DropColumn("FleetManagement.FleetInsurances", "Email");
            DropColumn("FleetManagement.FleetInsurances", "City");
            DropColumn("FleetManagement.FleetInsurances", "InsuranceTypeId");
            DropColumn("FleetManagement.FleetInsurances", "InsuranceCategoryId");
            DropColumn("FleetManagement.AssignedEquipment", "EquipmentID");
            DropColumn("FleetManagement.AssignedEquipment", "Type");
            DropColumn("FleetManagement.AssignedEquipment", "CategoryRef");
            DropColumn("FleetManagement.CheckList", "EquipmentCategory");
            DropColumn("Maintenance.JobOrder", "EquipmentTypeId");
            DropColumn("Maintenance.RepairType", "Description");
            DropColumn("Maintenance.JobRequests", "FleetsID");
            DropColumn("Maintenance.JobRequests", "AssetNum");
            DropColumn("Maintenance.JobRequests", "ProjectName");
            DropColumn("FleetManagement.RequestedEquipment", "EquipmentID");
            DropColumn("FleetManagement.RequestedEquipment", "EquipmentName");
            DropColumn("FleetManagement.RequestedEquipment", "SubEquipmentName");
            DropColumn("FleetManagement.RequestedEquipment", "Type");
            DropColumn("FleetManagement.RequestedEquipment", "CategoryRef");
            //DropColumn("Maintenance.ServiceSpare", "InventoryItems_ItemCode");
            DropColumn("FleetManagement.VehicleDistribution", "Referance");
            DropTable("FleetManagement.RentOtherSupplierApproval");
            DropTable("FleetManagement.RentOtherSuppliers");
        }
        
        public override void Down()
        {
            CreateTable(
                "FleetManagement.RentOtherSuppliers",
                c => new
                    {
                        RentOtherSuppliersID = c.Guid(nullable: false),
                        EquipmentIdentityID = c.Guid(nullable: false),
                        EquipmentTypeID = c.Guid(nullable: false),
                        WorkingCapacity = c.String(nullable: false),
                        RentalRateIncludingVAT = c.Boolean(nullable: false),
                        WithDriver = c.Boolean(nullable: false),
                        GoodCondition = c.Boolean(nullable: false),
                        NormalCondition = c.Boolean(nullable: false),
                        BadCondition = c.Boolean(nullable: false),
                        Address = c.String(),
                        Status = c.String(),
                        Quantity = c.Int(),
                        FuelCoveredBy = c.String(nullable: false),
                        EquipmentName = c.String(nullable: false),
                        ProjectLocation = c.String(nullable: false),
                        UnitOfWorkMeasurement = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Condition = c.String(nullable: false),
                        NameBankBranch = c.String(),
                        BankFullAddress = c.String(),
                        BankSlipNo = c.String(),
                        AdvancePayment = c.Int(nullable: false),
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
                .PrimaryKey(t => t.RentOtherSuppliersID);
            
            CreateTable(
                "FleetManagement.RentOtherSupplierApproval",
                c => new
                    {
                        RentOtherSupplierApprovalID = c.Int(nullable: false, identity: true),
                        RentOtherSupplierId = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        RentOtherSupplier_RentOtherSuppliersID = c.Guid(),
                    })
                .PrimaryKey(t => t.RentOtherSupplierApprovalID);
            
            AddColumn("FleetManagement.VehicleDistribution", "Referance", c => c.String());
            AddColumn("Maintenance.ServiceSpare", "InventoryItems_ItemCode", c => c.String(maxLength: 128));
            AddColumn("FleetManagement.RequestedEquipment", "CategoryRef", c => c.String());
            AddColumn("FleetManagement.RequestedEquipment", "Type", c => c.String(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "SubEquipmentName", c => c.String(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "EquipmentName", c => c.String(nullable: false));
            AddColumn("FleetManagement.RequestedEquipment", "EquipmentID", c => c.Guid(nullable: false));
            AddColumn("Maintenance.JobRequests", "ProjectName", c => c.String());
            AddColumn("Maintenance.JobRequests", "AssetNum", c => c.String());
            AddColumn("Maintenance.JobRequests", "FleetsID", c => c.Guid(nullable: false));
            AddColumn("Maintenance.RepairType", "Description", c => c.String());
            AddColumn("Maintenance.JobOrder", "EquipmentTypeId", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.CheckList", "EquipmentCategory", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "CategoryRef", c => c.String());
            AddColumn("FleetManagement.AssignedEquipment", "Type", c => c.String(nullable: false));
            AddColumn("FleetManagement.AssignedEquipment", "EquipmentID", c => c.Guid(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "InsuranceCategoryId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "InsuranceTypeId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "City", c => c.String(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "Email", c => c.String());
            AddColumn("FleetManagement.FleetInsurances", "PhoneNo", c => c.String(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "InsuranceCompany", c => c.String(nullable: false));
            AddColumn("FleetManagement.Accident", "Num", c => c.Int(nullable: false, identity: true));
            DropForeignKey("FleetManagement.Operator", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.WagonDrillTimesheet", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("Maintenance.SparePart", "SpareMaterialCategory_SpareMaterialCategoryId", "Maintenance.SpareMaterialCategory");
            DropForeignKey("FleetManagement.RentalPaymentRequestValidation", "RentalPaymentRequestId", "FleetManagement.RentalPaymentRequest");
            DropForeignKey("FleetManagement.RentalPaymentRequestLine", "RentalPaymentRequestId", "FleetManagement.RentalPaymentRequest");
            DropForeignKey("FleetManagement.RentalPaymentRequestDepartmentValidation", "RentalPaymentRequestId", "FleetManagement.RentalPaymentRequest");
            DropForeignKey("FleetManagement.RentalPaymentRequestBudgetValidation", "RentalPaymentRequestId", "FleetManagement.RentalPaymentRequest");
            DropForeignKey("FleetManagement.RentalMVInspection", "RentalInspectionCriteriaId", "FleetManagement.RentalInspectionCriteria");
            DropForeignKey("FleetManagement.RentalMVInspection", "RentalInspectionId", "FleetManagement.RentalInspection");
            DropForeignKey("FleetManagement.RentalInspectionValidation", "RentalInspectionId", "FleetManagement.RentalInspection");
            DropForeignKey("FleetManagement.RentalInspection", "EquipmentTypeID", "FleetManagement.EquipmentType");
            DropForeignKey("FleetManagement.RentalInspection", "EquipmentNameID", "FleetManagement.EquipmentName");
            DropForeignKey("FleetManagement.RentalInspection", "BranchId", "HRM.Branch");
            DropForeignKey("FleetManagement.RentalInspectionCriteriaDetail", "RentalInspectionCriteriaId", "FleetManagement.RentalInspectionCriteria");
            DropForeignKey("Maintenance.MaintenanceToolIssueAndReturn", "MechanicCrewID", "Maintenance.MechanicCrew");
            DropForeignKey("Maintenance.MechanicCrew", "EmployeeId", "HRM.Employee");
            DropForeignKey("Maintenance.MaintenanceToolIssueAndReturn", "MaintenanceToolID", "Maintenance.MaintenanceTool");
            DropForeignKey("FleetManagement.MachineryWorkingTimeSheet", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("Maintenance.JobOrderServiceCenterDetail", "RepairTypeID", "Maintenance.RepairType");
            DropForeignKey("Maintenance.JobOrderServiceCenterDetail", "JobCausesId", "Maintenance.JobCauses");
            DropForeignKey("Maintenance.JobOrderDetail", "RepairTypeID", "Maintenance.RepairType");
            DropForeignKey("FleetManagement.FleetLineCheckList", "FleetCheckListId", "FleetManagement.FleetCheckList");
            DropForeignKey("FleetManagement.FleetIFuelConsumption", "UoMName", "EXCEED.UoM");
            DropForeignKey("FleetManagement.FleetBatteryAndTire", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("FleetManagement.FleetBatteryAndTire", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("FleetManagement.FleetAnnualPlanDetail", "FleetAnnualPlanId", "FleetManagement.FleetAnnualPlan");
            DropForeignKey("FleetManagement.EquipmentCheckList", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.DumpTruckTimeSheet", "FleetsID", "FleetManagement.Fleets");
            DropForeignKey("FleetManagement.DryLoadingTimeSheet", "ProcurementSupplierID", "EXCEED.ProcurementSupplier");
            DropForeignKey("FleetManagement.DryLoadingTimeSheet", "FleetsID", "FleetManagement.Fleets");
            DropIndex("FleetManagement.WagonDrillTimesheet", new[] { "FleetsID" });
            DropIndex("Maintenance.SparePart", new[] { "SpareMaterialCategory_SpareMaterialCategoryId" });
            DropIndex("FleetManagement.RentalPaymentRequestValidation", new[] { "RentalPaymentRequestId" });
            DropIndex("FleetManagement.RentalPaymentRequestLine", new[] { "RentalPaymentRequestId" });
            DropIndex("FleetManagement.RentalPaymentRequestDepartmentValidation", new[] { "RentalPaymentRequestId" });
            DropIndex("FleetManagement.RentalPaymentRequestBudgetValidation", new[] { "RentalPaymentRequestId" });
            DropIndex("FleetManagement.RentalMVInspection", new[] { "RentalInspectionId" });
            DropIndex("FleetManagement.RentalMVInspection", new[] { "RentalInspectionCriteriaId" });
            DropIndex("FleetManagement.RentalInspectionValidation", new[] { "RentalInspectionId" });
            DropIndex("FleetManagement.RentalInspection", new[] { "BranchId" });
            DropIndex("FleetManagement.RentalInspection", new[] { "EquipmentNameID" });
            DropIndex("FleetManagement.RentalInspection", new[] { "EquipmentTypeID" });
            DropIndex("FleetManagement.RentalInspectionCriteriaDetail", new[] { "RentalInspectionCriteriaId" });
            DropIndex("Maintenance.MechanicCrew", new[] { "EmployeeId" });
            DropIndex("Maintenance.MaintenanceToolIssueAndReturn", new[] { "MechanicCrewID" });
            DropIndex("Maintenance.MaintenanceToolIssueAndReturn", new[] { "MaintenanceToolID" });
            DropIndex("FleetManagement.MachineryWorkingTimeSheet", new[] { "FleetsID" });
            DropIndex("Maintenance.JobOrderServiceCenterDetail", new[] { "RepairTypeID" });
            DropIndex("Maintenance.JobOrderServiceCenterDetail", new[] { "JobCausesId" });
            DropIndex("Maintenance.JobOrderDetail", new[] { "RepairTypeID" });
            DropIndex("FleetManagement.FleetLineCheckList", new[] { "FleetCheckListId" });
            DropIndex("FleetManagement.FleetIFuelConsumption", new[] { "UoMName" });
            DropIndex("FleetManagement.FleetIFuelConsumption", new[] { "EquipmentTypeID" });
            DropIndex("FleetManagement.FleetBatteryAndTire", new[] { "ItemCode" });
            DropIndex("FleetManagement.FleetBatteryAndTire", new[] { "ItemCategoryCode" });
            DropIndex("FleetManagement.FleetAnnualPlanDetail", new[] { "FleetAnnualPlanId" });
            DropIndex("FleetManagement.EquipmentCheckList", new[] { "FleetsID" });
            DropIndex("FleetManagement.DumpTruckTimeSheet", new[] { "FleetsID" });
            DropIndex("FleetManagement.DryLoadingTimeSheet", new[] { "FleetsID" });
            DropIndex("FleetManagement.DryLoadingTimeSheet", new[] { "ProcurementSupplierID" });
            DropIndex("FleetManagement.Operator", new[] { "FleetsID" });
            DropPrimaryKey("FleetManagement.FleetInsurances");
            AlterColumn("Maintenance.SparePart", "ReceiptNo", c => c.Int(nullable: false));
            AlterColumn("Maintenance.SparePart", "PartNo", c => c.String(nullable: false));
            AlterColumn("FleetManagement.SitesVehicleTransfer", "DestinationSite", c => c.String(nullable: false));
            AlterColumn("FleetManagement.SitesVehicleTransfer", "InitialSite", c => c.String(nullable: false));
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursAfternoonTo", c => c.DateTime(nullable: false));
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursAfternoonFrom", c => c.DateTime(nullable: false));
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursMorningto", c => c.DateTime(nullable: false));
            AlterColumn("Maintenance.ServiceCrewWorkLog", "WorkingHoursMorningFrom", c => c.DateTime(nullable: false));
            AlterColumn("Maintenance.MaintenanceTool", "Location", c => c.String(nullable: false));
            //AlterColumn("Maintenance.LogisticPlanDetail", "ItemCode", c => c.String(maxLength: 128));
            AlterColumn("Maintenance.JobOrder", "ProjectName", c => c.String(nullable: false));
            AlterColumn("FleetManagement.EquipmentName", "Name", c => c.String(nullable: false));
            AlterColumn("FleetManagement.EquipmentArrival", "MeansOfTravelling", c => c.String(nullable: false));
            AlterColumn("FleetManagement.DriverAssignment", "ToDate", c => c.DateTime(nullable: false));
            AlterColumn("FleetManagement.AssignedEquipment", "EquipmentName", c => c.String(nullable: false));
            AlterColumn("FleetManagement.TireIssue", "TireType", c => c.String(nullable: false));
            AlterColumn("FleetManagement.TireIssue", "FleetId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.Operator", "FleetsID", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.FleetInsurances", "ExpireDate", c => c.DateTime(nullable: false));
            AlterColumn("FleetManagement.FleetInsurances", "DueDate", c => c.DateTime(nullable: false));
            AlterColumn("FleetManagement.FleetInsurances", "FleetInsuranceId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.Fleets", "UnitNum", c => c.String(nullable: false));
            DropColumn("FleetManagement.VehicleDistributionApproval", "ApprovedBy");
            DropColumn("FleetManagement.VehicleDistribution", "Remarks");
            DropColumn("FleetManagement.VehicleDistribution", "NumberofPersonston");
            DropColumn("FleetManagement.VehicleDistribution", "Destination");
            DropColumn("FleetManagement.VehicleDistribution", "EquipmentName");
            DropColumn("FleetManagement.VehicleDistribution", "EquipmentType");
            DropColumn("FleetManagement.VehicleDistribution", "NumberHourKM");
            DropColumn("FleetManagement.VehicleDistribution", "Reference");
            DropColumn("FleetManagement.VehicleDistribution", "IsAssigned");
            DropColumn("FleetManagement.VehicleDistribution", "AssignmentDate");
            DropColumn("FleetManagement.VehicleDistribution", "RequestDate");
            DropColumn("FleetManagement.VehicleDistribution", "ReasonForTravel");
            DropColumn("FleetManagement.VehicleDistribution", "EmployeeId");
            DropColumn("FleetManagement.VehicleDistribution", "ManufacturingYear");
            DropColumn("FleetManagement.VehicleDistribution", "PlateNumber");
            DropColumn("FleetManagement.VehicleDemand", "ForwardedFrom");
            DropColumn("FleetManagement.VehicleDemand", "ForwardedTo");
            DropColumn("FleetManagement.VehicleDemand", "OrgStructureId");
            DropColumn("FleetManagement.VehicleDemand", "IsAssigned");
            DropColumn("FleetManagement.VehicleDemand", "RequestDate");
            DropColumn("FleetManagement.VehicleDemand", "ReasonForTravel");
            DropColumn("FleetManagement.VehicleDemand", "BusinessUnitId");
            DropColumn("FleetManagement.VehicleDemand", "BranchId");
            DropColumn("FleetManagement.VehicleDemand", "EmployeeId");
            DropColumn("Maintenance.SparePart", "SpareMaterialCategory_SpareMaterialCategoryId");
            DropColumn("Maintenance.SparePart", "SpareMaterialCategoryId");
            DropColumn("FleetManagement.SitesVehicleTransfer", "OperatorId");
            DropColumn("FleetManagement.SitesVehicleTransfer", "BranchId");
            DropColumn("FleetManagement.SitesVehicleTransfer", "EquipmentNameID");
            DropColumn("FleetManagement.SitesVehicleTransfer", "EquipmentTypeID");
            DropColumn("FleetManagement.SitesVehicleTransfer", "EquipmentIdentityID");
            DropColumn("Maintenance.ServiceSpare", "TotalPrice");
            DropColumn("Maintenance.ServiceSpare", "SpareMaterialCategoryId");
            DropColumn("Maintenance.ServiceSpare", "SparePartID");
            DropColumn("Maintenance.ServiceCrewWorkLog", "TotalLabourCost");
            DropColumn("Maintenance.ServiceCrewWorkLog", "StandaredHours");
            DropColumn("Maintenance.ServiceCrewWorkLog", "TotalLabourHours");
            DropColumn("Maintenance.ServiceCrewWorkLog", "HourlyWage");
            DropColumn("Maintenance.ServiceCrewWorkLog", "WorkShopType");
            DropColumn("Maintenance.ServiceCrewWorkLog", "ServiceGroup");
            DropColumn("Maintenance.ServiceCrewWorkLog", "SubCategoryCode");
            DropColumn("FleetManagement.RequestedEquipment", "VehicleDemandId");
            DropColumn("FleetManagement.RequestedEquipment", "Remarks");
            DropColumn("FleetManagement.RequestedEquipment", "NumberofPersonston");
            DropColumn("FleetManagement.RequestedEquipment", "NumberHourKM");
            DropColumn("FleetManagement.RequestedEquipment", "NumberofPersonOrTon");
            DropColumn("FleetManagement.RequestedEquipment", "Destination");
            DropColumn("FleetManagement.RequestedEquipment", "NumberHourPerKm");
            DropColumn("FleetManagement.RequestedEquipment", "EquipmentNameId");
            DropColumn("FleetManagement.RequestedEquipment", "EquipmentTypeID");
            DropColumn("FleetManagement.RequestedEquipment", "EquipmentIdentityID");
            DropColumn("FleetManagement.RentFromExternal", "HP_CC");
            DropColumn("FleetManagement.RentFromExternal", "Hour");
            DropColumn("FleetManagement.RentFromExternal", "BranchId");
            DropColumn("FleetManagement.RentFromExternal", "ProcurementSupplierID");
            DropColumn("Maintenance.ServiceCrew", "TotalLabourCost");
            DropColumn("Maintenance.ServiceCrew", "LabourCost");
            DropColumn("Maintenance.ServiceCrew", "TotalLabourHours");
            DropColumn("Maintenance.ServiceCrew", "MechanicType");
            DropColumn("Maintenance.MechanicCrew", "Postionenum");
            DropColumn("Maintenance.MechanicCrew", "MechanicTypeClassId");
            DropColumn("Maintenance.MechanicCrew", "MechanicType");
            DropColumn("Maintenance.MaintenanceCenter", "BranchId");
            DropColumn("Maintenance.MaintenanceCenter", "BusinessUnitId");
            DropColumn("Maintenance.MaintenanceCenter", "IsExternal");
            DropColumn("FleetManagement.LicenseLevel", "OperatorId");
            DropColumn("Maintenance.JobRequests", "DeliveredDate");
            DropColumn("Maintenance.JobRequests", "JobOrderStatus");
            DropColumn("Maintenance.JobRequests", "IsHighPriority");
            DropColumn("Maintenance.JobRequests", "HasInsurance");
            DropColumn("Maintenance.JobRequests", "CreatedAtBusinessUnitId");
            DropColumn("Maintenance.JobRequests", "CreatedAtBranchId");
            DropColumn("Maintenance.JobRequests", "OrgStructureId");
            DropColumn("Maintenance.JobRequests", "BusinessUnitId");
            DropColumn("Maintenance.JobRequests", "BranchId");
            DropColumn("Maintenance.JobRequests", "RequesterId");
            DropColumn("Maintenance.JobRequests", "MaintenanceCenterId");
            DropColumn("Maintenance.JobRequests", "MaintenanceCenter");
            DropColumn("Maintenance.JobRequests", "LicensePlate");
            DropColumn("Maintenance.JobRequests", "EquipmentInformationId");
            DropColumn("Maintenance.JobRequests", "Source");
            DropColumn("Maintenance.JobRequests", "CustomerId");
            DropColumn("Maintenance.JobRequests", "VeheicleReceptionFormId");
            DropColumn("Maintenance.RepairType", "Process");
            DropColumn("Maintenance.RepairType", "Time");
            DropColumn("Maintenance.RepairType", "Place");
            DropColumn("Maintenance.RepairType", "Reason");
            DropColumn("Maintenance.RepairType", "Remark");
            DropColumn("Maintenance.RepairType", "TotalAmountCustomer");
            DropColumn("Maintenance.RepairType", "TotalAmountCompany");
            DropColumn("Maintenance.RepairType", "LabourRateAtCustomer");
            DropColumn("Maintenance.RepairType", "LabourRateAtCompany");
            DropColumn("Maintenance.RepairType", "Difference");
            DropColumn("Maintenance.RepairType", "PayableTime");
            DropColumn("Maintenance.RepairType", "AutomotiveStandaredHour");
            DropColumn("Maintenance.RepairType", "StandaredTime");
            DropColumn("Maintenance.RepairType", "SubCategoryCode");
            DropColumn("Maintenance.RepairType", "ServiceGroup");
            DropColumn("Maintenance.RepairType", "CategoryCode");
            DropColumn("Maintenance.JobOrderDetail", "ServiceCenterId");
            DropColumn("Maintenance.JobOrderDetail", "RepairTypeID");
            DropColumn("Maintenance.JobOrderDetail", "ServiceGroup");
            DropColumn("Maintenance.JobOrderDetail", "SubCategoryCode");
            DropColumn("Maintenance.JobOrder", "IsInsured");
            DropColumn("Maintenance.JobOrder", "RequesterId");
            DropColumn("Maintenance.JobOrder", "CreatedAtBusinessUnitId");
            DropColumn("Maintenance.JobOrder", "CreatedAtBranchId");
            DropColumn("Maintenance.JobOrder", "OrgStructureId");
            DropColumn("Maintenance.JobOrder", "BusinessUnitId");
            DropColumn("Maintenance.JobOrder", "BranchId");
            DropColumn("Maintenance.JobOrder", "ServiceCenterStatus");
            DropColumn("Maintenance.JobOrder", "WorkShopRejectedReason");
            DropColumn("Maintenance.JobOrder", "OnlineSubmittedTime");
            DropColumn("Maintenance.JobOrder", "OnlineAssignedTime");
            DropColumn("Maintenance.JobOrder", "IsSubmittedToServiceCenter");
            DropColumn("Maintenance.JobOrder", "IsSentToWorkShop");
            DropColumn("Maintenance.JobOrder", "RejectionReason");
            DropColumn("Maintenance.JobOrder", "LicensePlate");
            DropColumn("Maintenance.JobOrder", "MaintenanceCenterId");
            DropColumn("Maintenance.JobOrder", "EquipmentInformationId");
            DropColumn("Maintenance.JobOrder", "IsRework");
            DropColumn("Maintenance.FollowUp", "ServiceCrewWorkLogId");
            DropColumn("FleetManagement.FleetIFuelConsumption", "UoMName");
            DropColumn("FleetManagement.ExternalRentedEquipment", "EquipmentNameID");
            DropColumn("FleetManagement.ExternalRentedEquipment", "EquipmentTypeID");
            DropColumn("FleetManagement.ExternalRentedEquipment", "EquipmentIdentityID");
            DropColumn("FleetManagement.EquipmentName", "FixedAssetSubCategoryNameId");
            DropColumn("FleetManagement.EquipmentName", "EquipmentTypeId");
            DropColumn("FleetManagement.EquipmentArrival", "EquipmentNameId");
            DropColumn("FleetManagement.EquipmentArrival", "EquipmentTypeId");
            DropColumn("FleetManagement.EquipmentArrival", "EquipmentIdentityId");
            DropColumn("FleetManagement.DriverTransferApproval", "DriverAssignmentId");
            DropColumn("FleetManagement.DriverVehicleTransfer", "FleetId");
            DropColumn("FleetManagement.DriverAssignment", "BranchId");
            DropColumn("FleetManagement.CheckList", "EquipmentIdentityID");
            DropColumn("FleetManagement.AssignedEquipment", "VehicleDemandId");
            DropColumn("FleetManagement.AssignedEquipment", "ReturnDate");
            DropColumn("FleetManagement.AssignedEquipment", "Returned");
            DropColumn("FleetManagement.AssignedEquipment", "NetOdometer");
            DropColumn("FleetManagement.AssignedEquipment", "EndOdometer");
            DropColumn("FleetManagement.AssignedEquipment", "StartOdometr");
            DropColumn("FleetManagement.AssignedEquipment", "NumberofPersonOrTon");
            DropColumn("FleetManagement.AssignedEquipment", "Destination");
            DropColumn("FleetManagement.AssignedEquipment", "NumberHourPerKm");
            DropColumn("FleetManagement.AssignedEquipment", "FleetId");
            DropColumn("FleetManagement.AssignedEquipment", "OperatorId");
            DropColumn("FleetManagement.AssignedEquipment", "EquipmentNameID");
            DropColumn("FleetManagement.AssignedEquipment", "RequestedEquipmentId");
            DropColumn("FleetManagement.TireReturn", "ReturnItemId");
            DropColumn("FleetManagement.TireReturn", "ReturnId");
            DropColumn("FleetManagement.TireReturn", "ReturnType");
            DropColumn("FleetManagement.TireIssue", "IssueItemId");
            DropColumn("FleetManagement.TireIssue", "IssueId");
            DropColumn("FleetManagement.TireIssue", "IssueType");
            DropColumn("FleetManagement.TireIssue", "EquipmentNameID");
            DropColumn("FleetManagement.TireIssue", "EquipmentTypeID");
            DropColumn("FleetManagement.TireIssue", "EquipmentIdentityID");
            DropColumn("FleetManagement.Operator", "COCResult");
            DropColumn("FleetManagement.Operator", "LicenseLevelenum");
            DropColumn("FleetManagement.Operator", "LicenseTypeenum");
            DropColumn("FleetManagement.Operator", "SalaryIncentiveenum");
            DropColumn("FleetManagement.Operator", "Genderenum");
            DropColumn("FleetManagement.Operator", "Postionenum");
            DropColumn("FleetManagement.Operator", "TypeofEmploymentenum");
            DropColumn("FleetManagement.Operator", "IsExternal");
            DropColumn("FleetManagement.FleetInsurances", "ComprehensiveInsurance");
            DropColumn("FleetManagement.FleetInsurances", "ThirdPartyInsurance");
            DropColumn("FleetManagement.FleetInsurances", "InsuranceType");
            DropColumn("FleetManagement.FleetInsurances", "ThirdParty");
            DropColumn("FleetManagement.FleetInsurances", "HazardOfExpiryDate");
            DropColumn("FleetManagement.FleetInsurances", "PolicyNo");
            DropColumn("FleetManagement.FleetInsurances", "InsuranceCompanyId");
            DropColumn("FleetManagement.EquipmentType", "TimeSheetType");
            DropColumn("FleetManagement.EquipmentType", "FixedAssetSubCategoryId");
            DropColumn("FleetManagement.EquipmentPlanandEvaluations", "EquipmentNameId");
            DropColumn("FleetManagement.EquipmentIdentity", "FixedAssetCategoryId");
            DropColumn("FleetManagement.Fleets", "HorsePower");
            DropColumn("FleetManagement.Fleets", "StateOfOdoMeterenum");
            DropColumn("FleetManagement.Fleets", "TypeOfFuelenum");
            DropColumn("FleetManagement.Fleets", "yearenum");
            DropColumn("FleetManagement.Fleets", "Colorenum");
            DropColumn("FleetManagement.Fleets", "manufaturerenum");
            DropColumn("FleetManagement.Fleets", "WayOfSupplyenum");
            DropColumn("FleetManagement.Fleets", "RentFromExternalNum");
            DropColumn("FleetManagement.Fleets", "EquipmentNameID");
            DropColumn("FleetManagement.Fleets", "AssetId");
            DropColumn("FleetManagement.Accident", "PoliceReportAddress");
            DropColumn("FleetManagement.Accident", "PoliceReportDate");
            DropColumn("FleetManagement.Accident", "OtherPartyAddress");
            DropColumn("FleetManagement.Accident", "OtherPartyFullName");
            DropColumn("FleetManagement.Accident", "TimeOfAccident");
            DropColumn("FleetManagement.Accident", "OperatorId");
            DropTable("FleetManagement.WagonDrillTimesheet");
            DropTable("Maintenance.VehicleReceptionForm");
            DropTable("Maintenance.VehiclePartManagement");
            DropTable("Maintenance.VehiclePartDamageType");
            DropTable("Maintenance.VehiclePart");
            DropTable("EXCEED.TimeSheetApprovalNew");
            DropTable("Maintenance.SpareMaterialCategory");
            DropTable("Maintenance.ServiceCenter");
            DropTable("Maintenance.ServiceCenterJob");
            DropTable("FleetManagement.RentToExternals");
            DropTable("FleetManagement.RentToExternalApproval");
            DropTable("FleetManagement.RentFromExternalEquipment");
            DropTable("FleetManagement.RentalPaymentRequestValidation");
            DropTable("FleetManagement.RentalPaymentRequestLine");
            DropTable("FleetManagement.RentalPaymentRequestDepartmentValidation");
            DropTable("FleetManagement.RentalPaymentRequest");
            DropTable("FleetManagement.RentalPaymentRequestBudgetValidation");
            DropTable("FleetManagement.RentalMVInspection");
            DropTable("FleetManagement.RentalInspectionValidation");
            DropTable("FleetManagement.RentalInspection");
            DropTable("FleetManagement.RentalInspectionCriteriaDetail");
            DropTable("FleetManagement.RentalInspectionCriteria");
            DropTable("EXCEED.RentalCompany");
            DropTable("EXCEED.PeriodicEquipmentInspection");
            DropTable("EXCEED.MechanicTypeClass");
            DropTable("Maintenance.MaintenanceToolIssueAndReturn");
            DropTable("Maintenance.MaintenanceIntervals");
            DropTable("FleetManagement.MachineryWorkingTimeSheet");
            DropTable("FleetManagement.LightVehicleTimesheet");
            DropTable("Maintenance.JobOrderServiceCenterDetail");
            DropTable("EXCEED.InsuranceCompany");
            DropTable("FleetManagement.FleetLineCheckList");
            DropTable("FleetManagement.FleetCheckList");
            DropTable("FleetManagement.FleetBoloRegistration");
            DropTable("FleetManagement.FleetBatteryAndTire");
            DropTable("FleetManagement.FleetAnnualPlan");
            DropTable("FleetManagement.FleetAnnualPlanDetail");
            DropTable("Maintenance.ExternalServiceCenterPayment");
            DropTable("Maintenance.EquipmentInformation");
            DropTable("FleetManagement.EquipmentCheckList");
            DropTable("FleetManagement.DumpTruckTimeSheet");
            DropTable("FleetManagement.DumpTruckTimeSheetDetail");
            DropTable("FleetManagement.DryLoadingTimeSheet");
            DropTable("EXCEED.CostSheetApproval");
            DropTable("FleetManagement.AssignedEquipmentAdditionalInfo");
            DropTable("Maintenance.AdditionalWorkShopDetail");
            AddPrimaryKey("FleetManagement.FleetInsurances", "FleetInsuranceID");
            CreateIndex("Maintenance.ServiceSpare", "InventoryItems_ItemCode");
            CreateIndex("Maintenance.ServiceCrewWorkLog", "JobOrderDetailId");
            CreateIndex("FleetManagement.RentOtherSupplierApproval", "RentOtherSupplier_RentOtherSuppliersID");
            CreateIndex("Maintenance.LogisticPlanDetail", "ItemCode");
            CreateIndex("Maintenance.JobOrderDetail", "ServiceID");
            CreateIndex("Maintenance.JobOrder", "FleetsID");
            CreateIndex("FleetManagement.FleetIFuelConsumption", "EquipmentTypeId");
            CreateIndex("FleetManagement.EquipmentArrival", "FleetsId");
            CreateIndex("FleetManagement.DriverTransferApproval", "DriverVehicleTransferId");
            CreateIndex("FleetManagement.CheckList", "EquipmentTypeId");
            CreateIndex("FleetManagement.Operator", "FleetsID");
            AddForeignKey("FleetManagement.Operator", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            AddForeignKey("FleetManagement.ServiceSpare", "InventoryItems_ItemCode", "EXCEED.InventoryItem", "ItemCode");
            AddForeignKey("FleetManagement.ServiceCrewWorkLog", "JobOrderDetailId", "FleetManagement.JobOrderDetail", "JobOrderDetailID", cascadeDelete: true);
            AddForeignKey("FleetManagement.RentOtherSupplierApproval", "RentOtherSupplier_RentOtherSuppliersID", "FleetManagement.RentOtherSuppliers", "RentOtherSuppliersID");
            AddForeignKey("FleetManagement.LogisticPlanDetail", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
            AddForeignKey("FleetManagement.JobOrderDetail", "ServiceID", "FleetManagement.Service", "ServiceID", cascadeDelete: true);
            AddForeignKey("FleetManagement.JobOrder", "FleetsID", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            AddForeignKey("FleetManagement.EquipmentArrival", "FleetsId", "FleetManagement.Fleets", "FleetsID", cascadeDelete: true);
            AddForeignKey("FleetManagement.DriverTransferApproval", "DriverVehicleTransferId", "FleetManagement.DriverVehicleTransfer", "DriverVehicleTransferID", cascadeDelete: true);
            AddForeignKey("FleetManagement.CheckList", "EquipmentTypeId", "FleetManagement.EquipmentIdentity", "EquipmentIdentityID", cascadeDelete: true);
            MoveTable(name: "Maintenance.TechnicalJobApproval", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.SparePart", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ServiceSpare", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ServiceCrewWorkLog", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.PMSchedule", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.PMScheduleDetail", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.OutsourceMaintenanceApproval", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.OutsourceMaintenance", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.OtherCompanyMaintenance", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.OtherCompanyMaintenaceApproval", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.MechanicOnHand", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ServiceCrew", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.MechanicCrew", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.MaintenanceType", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ServiceTool", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.MaintenanceTool", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.MaintenanceCenter", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.MaintenaceInspectionLog", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.LogisticPlan", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.LogisticPlanDetail", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.JobStoreRequisition", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.JobRequests", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.WorkUnitSection", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.RepairType", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.Planner", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.Service", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.JobOrderDetail", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.JobOrder", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.JobCauses", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.InventorySparePartMapping", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.FollowUp", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.FailureReason", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ExternalMaterial", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ExternalCrew", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ExternalComponentDetail", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.ExternalComponent", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.DailyCareLog", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.DailyCare", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.DailyCareDetailLog", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.DailyCareDetailItem", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.DailyCareCategory", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.AdvanceMeasure", newSchema: "FleetManagement");
            MoveTable(name: "Maintenance.AdministrativeJobApproval", newSchema: "FleetManagement");
            RenameTable(name: "FleetManagement.JobRequests", newName: "JobRequest");
            RenameTable(name: "FleetManagement.FleetInsurances", newName: "FleetInsurance");
        }
    }
}
