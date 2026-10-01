namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fleetandmaaintenacechanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Maintenance.AccesseryInfo",
                c => new
                    {
                        AccesseryInfoId = c.Int(nullable: false, identity: true),
                        VehricleReceptionFormId = c.Int(nullable: false),
                        AccName = c.String(),
                        AccModel = c.String(),
                        Description = c.String(),
                        NumberOfAcc = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.AccesseryInfoId);
            
            CreateTable(
                "FleetManagement.CouponFuel",
                c => new
                    {
                        CouponFuelId = c.Int(nullable: false, identity: true),
                        Requestor = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        FleetId = c.String(),
                        IssueItem = c.String(),
                        UOM = c.String(),
                        RequestedQuantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiffKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ConsumptionPerKM = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelStation = c.String(),
                        CouponNumber = c.String(),
                        IsApproved = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.CouponFuelId);
            
            CreateTable(
                "Maintenance.DescriptionOfAchievement",
                c => new
                    {
                        DescrtiptionOfAchievementId = c.Int(nullable: false, identity: true),
                        ServiceCrewWorkLogId = c.String(),
                        Descrtiption = c.String(),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.DescrtiptionOfAchievementId);
            
            CreateTable(
                "FleetManagement.DriverTransferDetail",
                c => new
                    {
                        DriverTransferDetailId = c.Int(nullable: false, identity: true),
                        DriverTransferId = c.String(),
                        Type = c.String(),
                        SerialNo = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        DamagedParts = c.String(),
                        MissingParts = c.String(),
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
                .PrimaryKey(t => t.DriverTransferDetailId);
            
            CreateTable(
                "FleetManagement.EquipmentOperationPlan",
                c => new
                    {
                        EquipmentOperationPlanId = c.Int(nullable: false, identity: true),
                        CategoryId = c.String(),
                        EquipmentTypeId = c.String(),
                        EquipmentNameId = c.String(),
                        FleetId = c.String(),
                        SerialNumber = c.String(),
                        OperationHR = c.Decimal(precision: 18, scale: 2),
                        IdleHR = c.Decimal(precision: 18, scale: 2),
                        DownHR = c.Decimal(precision: 18, scale: 2),
                        IsFuel = c.Boolean(nullable: false),
                        FuelConsumption = c.Decimal(precision: 18, scale: 2),
                        GLFiscalYearId = c.Int(nullable: false),
                        GLPeriodId = c.Int(),
                        EquipmentOperationHR = c.Int(nullable: false),
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
                .PrimaryKey(t => t.EquipmentOperationPlanId);
            
            CreateTable(
                "FleetManagement.EquipmentOperationPlanValidation",
                c => new
                    {
                        EquipmentOperationPlanValidationId = c.Int(nullable: false, identity: true),
                        EquipmentOperationPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.EquipmentOperationPlanValidationId);
            
            CreateTable(
                "Maintenance.ExternalServiceAttachement",
                c => new
                    {
                        ExternalServiceAttachementId = c.Int(nullable: false, identity: true),
                        ServiceCenterJobId = c.String(),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ExternalServiceAttachementId);
            
            CreateTable(
                "FleetManagement.FleetAnnualPlanDaily",
                c => new
                    {
                        FleetAnnualPlanDailyId = c.Int(nullable: false, identity: true),
                        FleetAnnualPlanDetailId = c.Int(nullable: false),
                        NumberOfEquipmentInDay = c.Int(nullable: false),
                        WorkingHourPerDay = c.Int(nullable: false),
                        WorkingDay = c.Int(nullable: false),
                        InefficiencyPercentile = c.Decimal(nullable: false, precision: 18, scale: 2),
                        EfficiencyPercentile = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HourForInefficiencyPerDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ChargingRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedEffectiveWorkingHrPerDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpectedDailyIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                        GlPeriodId = c.Int(nullable: false),
                        OperationDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetAnnualPlanDailyId);
            
            CreateTable(
                "FleetManagement.FleetAvailabilityTreshold",
                c => new
                    {
                        FleetAvailabilityTresholdId = c.Int(nullable: false, identity: true),
                        CategoryId = c.String(),
                        TypeId = c.String(),
                        NameId = c.String(),
                        FleetId = c.String(),
                        GLFiscalYearId = c.Int(nullable: false),
                        AvailabilityTreshold = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                .PrimaryKey(t => t.FleetAvailabilityTresholdId);
            
            CreateTable(
                "FleetManagement.FleetAvailabilityTresholdValidation",
                c => new
                    {
                        FleetAvailabilityTresholdValidationId = c.Int(nullable: false, identity: true),
                        FleetAvailabilityTresholdId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetAvailabilityTresholdValidationId);
            
            CreateTable(
                "FleetManagement.FleetInsurancePaymentRequest",
                c => new
                    {
                        FleetInsurancePaymentRequestId = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        DurationFrom = c.DateTime(nullable: false),
                        DurationTo = c.DateTime(nullable: false),
                        ReasonofPayment = c.String(),
                        RequestDate = c.DateTime(nullable: false),
                        FleetInsuranceId = c.Int(nullable: false),
                        EquipmentCategoryId = c.Guid(nullable: false),
                        EquipmentTypeId = c.Guid(nullable: false),
                        EquipmentNameId = c.Guid(nullable: false),
                        Payment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FleetsID = c.Guid(nullable: false),
                        InsuranceCoverTypeId = c.Int(),
                        InsuranceType = c.Int(nullable: false),
                        InsuranceCompanyId = c.Int(nullable: false),
                        IsBudgetApproved = c.Boolean(nullable: false),
                        BudgetApprovedBy = c.String(),
                        BudgetApprovedTime = c.DateTime(),
                        Refference = c.String(),
                        Paid = c.Boolean(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsFinanceSendForApproval = c.Boolean(nullable: false),
                        FinanceSendForApprovalBy = c.String(),
                        FinanceSendForApprovalTime = c.DateTime(),
                        IsFinanceApproved = c.Boolean(nullable: false),
                        FinanceApprovedBy = c.String(),
                        FinanceApprovedTime = c.DateTime(),
                        IsAuthorized = c.Boolean(nullable: false),
                        AuthorizedBy = c.String(),
                        AuthorizedTime = c.DateTime(),
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
                .PrimaryKey(t => t.FleetInsurancePaymentRequestId);
            
            CreateTable(
                "FleetManagement.FleetInsurancePaymentRequestValidation",
                c => new
                    {
                        FleetInsurancePaymentRequestValidationId = c.Int(nullable: false, identity: true),
                        FleetInsurancePaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetInsurancePaymentRequestValidationId);
            
            CreateTable(
                "FleetManagement.FleetInsuranceValidation",
                c => new
                    {
                        FleetInsuranceValidationId = c.Int(nullable: false, identity: true),
                        FleetInsuranceId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.FleetInsuranceValidationId);
            
            CreateTable(
                "Insurance.InsuranceCoverTypes",
                c => new
                    {
                        InsuranceCoverTypeId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                        Remark = c.String(maxLength: 500),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InsuranceCoverTypeId);
            
            CreateTable(
                "FleetManagement.InternalRentalRate",
                c => new
                    {
                        InternalRentalRateId = c.Int(nullable: false, identity: true),
                        CategoryId = c.String(nullable: false),
                        EquipmentTypeId = c.String(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MarketPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentType = c.Int(nullable: false),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.InternalRentalRateId);
            
            CreateTable(
                "FleetManagement.MaintenanceCostDetail",
                c => new
                    {
                        MaintenanceCostDetailId = c.Int(nullable: false, identity: true),
                        TimeSheetId = c.Int(nullable: false),
                        RepairTypeId = c.String(),
                        MechanicName = c.String(),
                        SparePartCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OtherCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GreaseConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MiscellaneousCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GreaseCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ServiceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LabourCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TimeTaken = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsLightVehicle = c.Boolean(nullable: false),
                        IsMachinery = c.Boolean(nullable: false),
                        IsDumpTruck = c.Boolean(nullable: false),
                        IsWagonDrill = c.Boolean(nullable: false),
                        IsDryLoading = c.Boolean(nullable: false),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceCostDetailId);
            
            CreateTable(
                "Maintenance.MaintenanceDailyPlan",
                c => new
                    {
                        MaintenanceDailyPlanId = c.Int(nullable: false, identity: true),
                        MaintenancePlanDetailId = c.Int(nullable: false),
                        GlPeriodId = c.Int(nullable: false),
                        Date = c.DateTime(),
                        CMQuantity = c.Int(nullable: false),
                        PMQuantity = c.Int(nullable: false),
                        CMCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PMCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CMMaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CMLaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PMMaterialCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PMLaborCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenancePlanId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceDailyPlanId);
            
            CreateTable(
                "Maintenance.MaintenanceStage",
                c => new
                    {
                        MaintenanceStageId = c.Int(nullable: false, identity: true),
                        JobRequestId = c.String(),
                        ServiceCenterJobId = c.String(),
                        Description = c.String(),
                        Complain = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.MaintenanceStageId);
            
            CreateTable(
                "EXCEED.NonFuelItemSetting",
                c => new
                    {
                        NonFuelItemSettingId = c.Int(nullable: false, identity: true),
                        ItemCategoryCode = c.String(nullable: false, maxLength: 128),
                        ItemCode = c.String(nullable: false, maxLength: 128),
                        IsLubricant = c.Boolean(nullable: false),
                        IsSparePart = c.Boolean(nullable: false),
                        IsBattery = c.Boolean(nullable: false),
                        IsTire = c.Boolean(nullable: false),
                        IsOil = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.NonFuelItemSettingId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode, cascadeDelete: true)
                .ForeignKey("EXCEED.ItemCategory", t => t.ItemCategoryCode, cascadeDelete: true)
                .Index(t => t.ItemCategoryCode)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "FleetManagement.RentalAdvancePaymentRequest",
                c => new
                    {
                        RentalAdvancePaymentRequestId = c.Int(nullable: false, identity: true),
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
                        AdvancePaymentId = c.Int(nullable: false),
                        FleetId = c.String(),
                        PlateNumber = c.String(),
                        AgreementStartDate = c.DateTime(nullable: false),
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
                .PrimaryKey(t => t.RentalAdvancePaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalAdvancePaymentRequestBudgetValidation",
                c => new
                    {
                        RentalAdvancePaymentRequestBudgetValidationId = c.Int(nullable: false, identity: true),
                        RentalAdvancePaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalAdvancePaymentRequestBudgetValidationId)
                .ForeignKey("FleetManagement.RentalAdvancePaymentRequest", t => t.RentalAdvancePaymentRequestId, cascadeDelete: true)
                .Index(t => t.RentalAdvancePaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalAdvancePaymentRequestDepartmentValidation",
                c => new
                    {
                        RentalAdvancePaymentRequestDepartmentValidationId = c.Int(nullable: false, identity: true),
                        RentalAdvancePaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalAdvancePaymentRequestDepartmentValidationId)
                .ForeignKey("FleetManagement.RentalAdvancePaymentRequest", t => t.RentalAdvancePaymentRequestId, cascadeDelete: true)
                .Index(t => t.RentalAdvancePaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalAdvancePaymentRequestValidation",
                c => new
                    {
                        RentalAdvancePaymentRequestValidationId = c.Int(nullable: false, identity: true),
                        RentalAdvancePaymentRequestId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalAdvancePaymentRequestValidationId)
                .ForeignKey("FleetManagement.RentalAdvancePaymentRequest", t => t.RentalAdvancePaymentRequestId, cascadeDelete: true)
                .Index(t => t.RentalAdvancePaymentRequestId);
            
            CreateTable(
                "FleetManagement.RentalEquipmentAdvancePayment",
                c => new
                    {
                        RentalEquipmentAdvancePaymentId = c.Int(nullable: false, identity: true),
                        RentFromExternalEquipmentId = c.Int(nullable: false),
                        AdvancePayed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tot = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvanceWithTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FleetId = c.String(),
                        CostSheetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsFullPayment = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentalEquipmentAdvancePaymentId);
            
            CreateTable(
                "FleetManagement.RentalEquipmentPaymentSummaryDocument",
                c => new
                    {
                        RentalEquipmentPaymentSummaryDocumentId = c.Int(nullable: false, identity: true),
                        RentedEquipmentMonthlyPaymentSummaryId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.RentalEquipmentPaymentSummaryDocumentId);
            
            CreateTable(
                "FleetManagement.RentedEquipmentDailyOperation",
                c => new
                    {
                        RentedEquipmentDailyOperationId = c.Int(nullable: false, identity: true),
                        RentedEquipmentMonthlyPaymentSummaryId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        WorkedHR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayedIdleHR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalPayedHR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MorningTimeStart = c.DateTime(),
                        MorningTimeEnd = c.DateTime(),
                        AfternoonTimeStart = c.DateTime(),
                        AfternoonTimeEnd = c.DateTime(),
                        NightTimeStart = c.DateTime(),
                        NightTimeEnd = c.DateTime(),
                        InitialEngineHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinalEngineHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DifferenceEngineHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DownHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleReasonList = c.Int(nullable: false),
                        DownReasonList = c.Int(nullable: false),
                        OperatorId = c.Int(nullable: false),
                        OperatorName = c.String(),
                        SparePartCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilLubCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SiteFormanId = c.Int(nullable: false),
                        SiteFormanName = c.String(),
                        FuelInLiter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelKMReading = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Activity = c.String(),
                        Unit = c.String(),
                        Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.RentedEquipmentDailyOperationId);
            
            CreateTable(
                "FleetManagement.RentedEquipmentMonthlyPaymentSummary",
                c => new
                    {
                        RentedEquipmentMonthlyPaymentSummaryId = c.Int(nullable: false, identity: true),
                        GlPeriodId = c.Int(nullable: false),
                        EquipmentTypeId = c.String(),
                        EquipmentType = c.String(),
                        FleetId = c.String(),
                        IsPaymentLetterAttached = c.Boolean(nullable: false),
                        PlateNo = c.String(),
                        OperationDrilledHoleDepth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfHolesDrilled = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomerName = c.String(),
                        OperatorName = c.String(),
                        OperatorId = c.Int(nullable: false),
                        BranchId = c.Int(nullable: false),
                        BranchName = c.String(),
                        AgreementNo = c.String(),
                        RentalRateHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RentalRateDay = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AgreementDate = c.DateTime(nullable: false),
                        TimeSheetPreparedBy = c.String(),
                        TimeSheetPreparedDate = c.DateTime(nullable: false),
                        TimeSheetIsPrepared = c.Boolean(nullable: false),
                        TimeSheetCheckedBy = c.String(),
                        TimeSheetCheckedDate = c.DateTime(nullable: false),
                        TimeSheetIsChecked = c.Boolean(nullable: false),
                        IsRejected = c.Boolean(nullable: false),
                        TimeSheetApprovedBy = c.String(),
                        TimeSheetApprovalDate = c.DateTime(nullable: false),
                        TimeSheetIsApproved = c.Boolean(nullable: false),
                        TimeSheetAuthorizedBy = c.String(),
                        TimeSheetAuthorizedDate = c.DateTime(nullable: false),
                        TimeSheetIsAuthorized = c.Boolean(nullable: false),
                        TimeSheetDGMApprovedBy = c.String(),
                        TimeSheetDGMApprovalDate = c.DateTime(nullable: false),
                        TimeSheetIsDGMApproved = c.Boolean(nullable: false),
                        GrossBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWithVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdvancePayedWithVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Deductable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetPayable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWorkedHR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsWagonDrill = c.Boolean(nullable: false),
                        TimeSheetEquipmentAdminOfficerPreparedBy = c.String(),
                        TimeSheetEquipmentAdminOfficerPreparedDate = c.DateTime(),
                        TimeSheetEquipmentAdminOfficerPreparedBySign = c.String(),
                        TimeSheetIsEquipmentAdminOfficerPrepared = c.Boolean(nullable: false),
                        TimeSheetEquipmentAdminCheckedBy = c.String(),
                        TimeSheetEquipmentAdminCheckedDate = c.DateTime(),
                        TimeSheetEquipmentAdminCheckedBySign = c.String(),
                        TimeSheetIsEquipmentAdminChecked = c.Boolean(nullable: false),
                        TimeSheetIsOfficeEngineerChecked = c.Boolean(nullable: false),
                        TimeSheetIsConstructionEngineerApproved = c.Boolean(nullable: false),
                        TimeSheetIsDepartmentManagerApproved = c.Boolean(nullable: false),
                        TimeSheetOfficeEngineerCheckedBy = c.String(),
                        TimeSheetOfficeEngineerCheckedDate = c.DateTime(),
                        TimeSheetOfficeEngineerCheckedBySign = c.String(),
                        TimeSheetConstructionEngineerApprovedBy = c.String(),
                        TimeSheetConstructionEngineerApprovedDate = c.DateTime(),
                        TimeSheetConstructionEngineerApprovedBySign = c.String(),
                        TimeSheetDepartmentManagerApprovedBy = c.String(),
                        TimeSheetDepartmentManagerApprovedDate = c.DateTime(),
                        TimeSheetDepartmentManagerApprovedBySign = c.String(),
                        Refference = c.String(),
                        Paid = c.Boolean(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SparePartCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilLubCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsFinanceSendForApproval = c.Boolean(nullable: false),
                        FinanceSendForApprovalBy = c.String(),
                        FinanceSendForApprovalTime = c.DateTime(),
                        IsFinanceApproved = c.Boolean(nullable: false),
                        FinanceApprovedBy = c.String(),
                        FinanceApprovedTime = c.DateTime(),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentedEquipmentMonthlyPaymentSummaryId);
            
            CreateTable(
                "FleetManagement.RentedEquipmentMonthlyPaymentSummaryValidation",
                c => new
                    {
                        RentedEquipmentMonthlyPaymentSummaryValidationId = c.Int(nullable: false, identity: true),
                        RentedEquipmentMonthlyPaymentSummaryId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentedEquipmentMonthlyPaymentSummaryValidationId);
            
            CreateTable(
                "FleetManagement.RentFromExternalCustomerChange",
                c => new
                    {
                        RentFromExternalCustomerChangeId = c.Int(nullable: false, identity: true),
                        RentFromExternalId = c.Guid(nullable: false),
                        ProcurementSupplierID = c.Int(nullable: false),
                        IsCurent = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentFromExternalCustomerChangeId);
            
            CreateTable(
                "FleetManagement.RentFromExternalDocument",
                c => new
                    {
                        RentFromExternalDocumentId = c.Int(nullable: false, identity: true),
                        RentFromExternalId = c.String(),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.RentFromExternalDocumentId);
            
            CreateTable(
                "Maintenance.SatisfactionNote",
                c => new
                    {
                        SatisfactionNoteId = c.Int(nullable: false, identity: true),
                        ServiceCenterJobId = c.Guid(nullable: false),
                        WorkDescription = c.String(),
                        Quantity = c.String(),
                        Unit = c.String(),
                        Remark = c.String(),
                        RefNo = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SatisfactionNoteId);
            
            CreateTable(
                "FleetManagement.SelfCareSatisfaction",
                c => new
                    {
                        SelfCareSatisfactionId = c.Int(nullable: false, identity: true),
                        RequestedEquipmentId = c.Guid(nullable: false),
                        SatisfactionNote = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.SelfCareSatisfactionId);
            
            CreateTable(
                "EXCEED.StoreRequisitionSecondValidation",
                c => new
                    {
                        StoreRequisitionSecondValidationId = c.Int(nullable: false, identity: true),
                        StoreRequisitionId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Remark = c.String(),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.StoreRequisitionSecondValidationId)
                .ForeignKey("EXCEED.StoreRequisition", t => t.StoreRequisitionId, cascadeDelete: true)
                .Index(t => t.StoreRequisitionId);
            
            CreateTable(
                "FleetManagement.TimeSheetConsumptionItem",
                c => new
                    {
                        TimeSheetConsumptionItemId = c.Int(nullable: false, identity: true),
                        DumpTruckTimeSheetId = c.Int(),
                        DryLoadingTimeSheetId = c.Int(),
                        LightVehicleTimeSheetId = c.Int(),
                        MachineryWorkingTimeSheetId = c.Int(),
                        WagonDrillTimesheetId = c.Int(),
                        OilConsumption = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilType = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TimeSheetConsumptionItemId);
            
            CreateTable(
                "Maintenance.TyreSerialInfo",
                c => new
                    {
                        TyreSerialInfoId = c.Int(nullable: false, identity: true),
                        VehicleReceptionFormId = c.Int(nullable: false),
                        TireSize = c.String(),
                        TireModel = c.String(),
                        TireName = c.String(),
                        NumberOfTire = c.String(),
                        SerialNumber = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.TyreSerialInfoId);
            
            CreateTable(
                "Maintenance.VehicleReceptionDoucment",
                c => new
                    {
                        VehicleReceptionDocumentId = c.Int(nullable: false, identity: true),
                        VehicleReceptionFormId = c.Int(nullable: false),
                        FilePath = c.String(),
                        FileName = c.String(),
                        ShortPath = c.String(),
                        FileExtension = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.VehicleReceptionDocumentId);
            
            CreateTable(
                "FleetManagement.WagonDrillDailyOperation",
                c => new
                    {
                        WagonDrillDailyOperationId = c.Int(nullable: false, identity: true),
                        RentedEquipmentMonthlyPaymentSummaryId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        PlateNumber = c.String(),
                        Category = c.String(),
                        EquipmentName = c.String(),
                        TotalTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WorkedHR = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Activity = c.String(),
                        OperationDrilledHoleDepth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NumberOfHolesDrilled = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperationRateForMeter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OperationAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PayedIdleHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleRateHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InitialEngineHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinalEngineHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DifferenceEngineHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DownHour = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IdleReasonList = c.Int(nullable: false),
                        DownReasonList = c.Int(nullable: false),
                        OperatorId = c.Int(nullable: false),
                        OperatorName = c.String(),
                        SiteFormanId = c.Int(nullable: false),
                        SiteFormanName = c.String(),
                        GrossBeforeVat = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VAT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TOT = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SparePartCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OilLubCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaintenanceCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelInLiter = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FuelKMReading = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(),
                        Volume = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.WagonDrillDailyOperationId);
            
            CreateTable(
                "Maintenance.WorkshopFieldWorkTool",
                c => new
                    {
                        WorkshopFieldWorkToolId = c.Int(nullable: false, identity: true),
                        VehicleDemandId = c.String(),
                        ItemCategoryCode = c.String(),
                        ItemCode = c.String(),
                        SentQuantity = c.Int(nullable: false),
                        LostQuantity = c.Int(nullable: false),
                        IsSent = c.Boolean(nullable: false),
                        ReponsibleMechanic = c.String(),
                        ReturnedQuantity = c.Int(nullable: false),
                        IsReturned = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.WorkshopFieldWorkToolId);
            
            AddColumn("FleetManagement.Fleets", "IsContractTerminated", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Fleets", "VehicleReceptionFormId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.Fleets", "IsReserved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.Fleets", "Volume", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.Fleets", "Unit", c => c.Int());
            AddColumn("FleetManagement.Fleets", "IsExternalCustomer", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.FleetInsurances", "Payment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.FleetInsurances", "InsuranceCoverTypeId", c => c.Int());
            AddColumn("FleetManagement.FleetInsurances", "IsPaymentRequestGenerated", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.TireIssue", "IsCreatedOnFleet", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "ExternalOperatorName", c => c.String());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "TotalTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "SparePartCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "RentalAgreementNum", c => c.Int());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "OverTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "BatteryCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "OilCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "GreaseConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "GreaseCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "AsphaltWithOut", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "GravelWithOut", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "FuelType", c => c.String());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "OilType", c => c.String());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "MaintenanceFuel", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.DryLoadingTimeSheet", "ReasonForDown", c => c.Int());
            AddColumn("FleetManagement.DryLoadingTimeSheet", "IdleReason", c => c.Int());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "FuelType", c => c.String());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "RentalAgreementNum", c => c.Int());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "OilType", c => c.String());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "SparePartCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "OverTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "BatteryCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "MorningTimeStart", c => c.DateTime());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "MorningTimeEnd", c => c.DateTime());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "AfternoonTimeStart", c => c.DateTime());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "AfternoonTimeEnd", c => c.DateTime());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "NightTimeStart", c => c.DateTime());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "NightTimeEnd", c => c.DateTime());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "Volume", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "Unit", c => c.Int());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "TotalTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "GreaseCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "OilCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "ExternalOperatorName", c => c.String());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "LoadingStatus", c => c.Int(nullable: false));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "InitialKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "FinalKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "DifferenceKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "FuelKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "MaintenanceFuel", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "DownReason", c => c.Int());
            AddColumn("FleetManagement.DumpTruckTimeSheet", "IdleReason", c => c.Int());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsBudgetApproved", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "BudgetApprovedBy", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "BudgetApprovedTime", c => c.DateTime());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsFinanceSendForApproval", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceSendForApprovalBy", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceSendForApprovalTime", c => c.DateTime());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsFinanceApproved", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceApprovedBy", c => c.String());
            AddColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceApprovedTime", c => c.DateTime());
            AddColumn("FleetManagement.FleetCheckList", "DriverVehicleTransferID", c => c.String());
            AddColumn("EXCEED.FleetInsuranceHistory", "Payment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.FleetInsuranceHistory", "InsuranceCoverTypeId", c => c.Int());
            AddColumn("EXCEED.FleetInsuranceHistory", "IsPaymentRequestGenerated", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FuelItemSetting", "IsLubricant", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FuelItemSetting", "IsSparePart", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FuelItemSetting", "IsBattery", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FuelItemSetting", "IsTire", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.FuelItemSetting", "IsOil", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrder", "RemarkAfterDelivery", c => c.String());
            AddColumn("Maintenance.RepairType", "IsGeneric", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.JobOrderServiceCenterDetail", "DescriptionOfJobs", c => c.String());
            AddColumn("Maintenance.JobRequests", "CustomerName", c => c.String());
            AddColumn("FleetManagement.LightVehicleTimesheet", "TotalTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "RentalAgreementNum", c => c.Int());
            AddColumn("FleetManagement.LightVehicleTimesheet", "InitialKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "FinalKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "DifferenceKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "FuelKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "GreaseCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "SparePartCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "OverTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "BatteryCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "ExternalOperatorName", c => c.String());
            AddColumn("FleetManagement.LightVehicleTimesheet", "FuelType", c => c.String());
            AddColumn("FleetManagement.LightVehicleTimesheet", "OilType", c => c.String());
            AddColumn("FleetManagement.LightVehicleTimesheet", "OilCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "OilConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "MaintenanceFuel", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "ReasonForDown", c => c.Int());
            AddColumn("FleetManagement.LightVehicleTimesheet", "ReasonForIdle", c => c.Int());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "ExternalOperatorName", c => c.String());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "Volume", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "Unit", c => c.Int());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "SparePartCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "RentalAgreementNum", c => c.Int());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "OverTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "GreaseCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "BatteryCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "TotalTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "OilCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "FuelType", c => c.String());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "OilType", c => c.String());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "InitialEHR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "FinalEHR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "DifferenceEHR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "FuelKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "MaintenanceFuel", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "ReasonForDown", c => c.Int());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "ReasonForIdle", c => c.Int());
            AddColumn("Maintenance.MaintenancePlan", "EquipmentNameId", c => c.String());
            AddColumn("Maintenance.MaintenancePlan", "FleetId", c => c.String());
            AddColumn("Maintenance.MaintenancePlan", "ServiceCenterId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.OwningandOperatingCost", "CostDate", c => c.DateTime());
            AddColumn("Maintenance.PmSchedulers", "FleetId", c => c.String());
            AddColumn("FleetManagement.RentalInspection", "Volume", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalInspection", "Unit", c => c.Int());
            AddColumn("FleetManagement.RentalPaymentRequest", "WithHoldingTax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalPaymentRequest", "IsAuthorized", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentalPaymentRequest", "AuthorizedBy", c => c.String());
            AddColumn("FleetManagement.RentalPaymentRequest", "AuthorizedTime", c => c.DateTime());
            AddColumn("FleetManagement.RentalPaymentRequestLine", "CostSheetSummaryId", c => c.Int(nullable: false));
            AddColumn("FleetManagement.RentalPaymentRequestLine", "GrossBeforeVat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalPaymentRequestLine", "SparePartCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalPaymentRequestLine", "OilLubCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalPaymentRequestLine", "MaintenanceCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalPaymentRequestLine", "FuelCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalPaymentRequestLine", "VAT", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentalPaymentRequestLine", "WithholdingAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentFromExternalEquipment", "IsContractTerminated", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.RentFromExternalEquipment", "PayableIdleHrPercent", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentFromExternalEquipment", "Vat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentFromExternalEquipment", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsOperationApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "OperationApprvedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "OperationApprovalDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "IsDepartmentApproved", c => c.Boolean(nullable: false));
            AddColumn("FleetManagement.SitesVehicleTransfer", "DGMApprovedBy", c => c.String());
            AddColumn("FleetManagement.SitesVehicleTransfer", "DGMApprovalDate", c => c.DateTime());
            AddColumn("FleetManagement.SitesVehicleTransfer", "TransferStatus", c => c.Int());
            AddColumn("FleetManagement.TimeSheet", "RentFromExternalID", c => c.Guid());
            AddColumn("FleetManagement.TimeSheet", "RentFromExternalEquipmentId", c => c.Int());
            AddColumn("FleetManagement.TimeSheet", "RentToExternalID", c => c.Guid());
            AddColumn("FleetManagement.TimeSheet", "ExternalRentedEquipmentId", c => c.Guid());
            AddColumn("FleetManagement.TimeSheet", "ChargeMethod", c => c.Int());
            AddColumn("FleetManagement.TimeSheet", "CubicValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.VehicleDemand", "ServiceCenterJobId", c => c.String());
            AddColumn("FleetManagement.VehicleDistribution", "ParentId", c => c.String());
            AddColumn("Maintenance.VehiclePart", "ApplyForAll", c => c.Boolean(nullable: false));
            AddColumn("Maintenance.VehicleReceptionForm", "CustomerName", c => c.String());
            AddColumn("Maintenance.VehicleReceptionForm", "DriverTransferId", c => c.String());
            AddColumn("FleetManagement.WagonDrillTimesheet", "LabourCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "SparePartCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "ExternalOperatorName", c => c.String());
            AddColumn("FleetManagement.WagonDrillTimesheet", "RentalAgreementNum", c => c.Int());
            AddColumn("FleetManagement.WagonDrillTimesheet", "BasicSalary", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "OverTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "BatteryCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "TotalTime", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "Volume", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "Unit", c => c.Int());
            AddColumn("FleetManagement.WagonDrillTimesheet", "OilConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "OilCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "GreaseConsumption", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "GreaseCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "FuelType", c => c.String());
            AddColumn("FleetManagement.WagonDrillTimesheet", "OilType", c => c.String());
            AddColumn("FleetManagement.WagonDrillTimesheet", "MorningTimeStart", c => c.DateTime());
            AddColumn("FleetManagement.WagonDrillTimesheet", "MorningTimeEnd", c => c.DateTime());
            AddColumn("FleetManagement.WagonDrillTimesheet", "AfternoonTimeStart", c => c.DateTime());
            AddColumn("FleetManagement.WagonDrillTimesheet", "AfternoonTimeEnd", c => c.DateTime());
            AddColumn("FleetManagement.WagonDrillTimesheet", "NightTimeStart", c => c.DateTime());
            AddColumn("FleetManagement.WagonDrillTimesheet", "NightTimeEnd", c => c.DateTime());
            AddColumn("FleetManagement.WagonDrillTimesheet", "InitialEHR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "FinalEHR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "DifferenceEHR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "FuelKMR", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "MaintenanceFuel", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("FleetManagement.WagonDrillTimesheet", "DownReason", c => c.Int());
            AddColumn("FleetManagement.WagonDrillTimesheet", "IdleReason", c => c.Int());
            AlterColumn("FleetManagement.Accident", "OperatorId", c => c.Guid());
            AlterColumn("FleetManagement.Fleets", "RentId", c => c.Guid());
            AlterColumn("FleetManagement.DriverAssignment", "SecondOperatorId", c => c.Guid());
            AlterColumn("FleetManagement.DryLoadingTimeSheet", "ReasonForIdle", c => c.String());
            AlterColumn("FleetManagement.DumpTruckTimeSheet", "ReasonForIdle", c => c.String());
            AlterColumn("FleetManagement.DumpTruckTimeSheet", "ReasonForDown", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "Model", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "ChasisSerialNo", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "EngineModel", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "EngineSerialNo", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "ManufacturingYear", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "Capacity", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "OperatorLicenceNo", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "Lessor", c => c.String());
            AlterColumn("FleetManagement.RentalInspection", "MeterReading", c => c.String());
            AlterColumn("FleetManagement.RentFromExternal", "EndDate", c => c.DateTime());
            AlterColumn("FleetManagement.RequestedEquipment", "AsphaltKm", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("FleetManagement.RequestedEquipment", "GravelKm", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("FleetManagement.SitesVehicleTransfer", "SecondOperatorId", c => c.Guid());
            AlterColumn("FleetManagement.TimeSheet", "CustomerName", c => c.String());
            DropColumn("FleetManagement.DumpTruckTimeSheet", "LubricantCost");
            DropColumn("FleetManagement.LightVehicleTimesheet", "LubricantCost");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "LubricantCost");
            DropColumn("Maintenance.PmSchedulers", "PlateNumber");
            DropColumn("FleetManagement.RentFromExternal", "WorkingCapacity");
            DropColumn("FleetManagement.WagonDrillTimesheet", "LubricantCost");
        }
        
        public override void Down()
        {
            AddColumn("FleetManagement.WagonDrillTimesheet", "LubricantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.RentFromExternal", "WorkingCapacity", c => c.String(nullable: false));
            AddColumn("Maintenance.PmSchedulers", "PlateNumber", c => c.String());
            AddColumn("FleetManagement.MachineryWorkingTimeSheet", "LubricantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.LightVehicleTimesheet", "LubricantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("FleetManagement.DumpTruckTimeSheet", "LubricantCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("EXCEED.StoreRequisitionSecondValidation", "StoreRequisitionId", "EXCEED.StoreRequisition");
            DropForeignKey("FleetManagement.RentalAdvancePaymentRequestValidation", "RentalAdvancePaymentRequestId", "FleetManagement.RentalAdvancePaymentRequest");
            DropForeignKey("FleetManagement.RentalAdvancePaymentRequestDepartmentValidation", "RentalAdvancePaymentRequestId", "FleetManagement.RentalAdvancePaymentRequest");
            DropForeignKey("FleetManagement.RentalAdvancePaymentRequestBudgetValidation", "RentalAdvancePaymentRequestId", "FleetManagement.RentalAdvancePaymentRequest");
            DropForeignKey("EXCEED.NonFuelItemSetting", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.NonFuelItemSetting", "ItemCode", "EXCEED.InventoryItem");
            DropIndex("EXCEED.StoreRequisitionSecondValidation", new[] { "StoreRequisitionId" });
            DropIndex("FleetManagement.RentalAdvancePaymentRequestValidation", new[] { "RentalAdvancePaymentRequestId" });
            DropIndex("FleetManagement.RentalAdvancePaymentRequestDepartmentValidation", new[] { "RentalAdvancePaymentRequestId" });
            DropIndex("FleetManagement.RentalAdvancePaymentRequestBudgetValidation", new[] { "RentalAdvancePaymentRequestId" });
            DropIndex("EXCEED.NonFuelItemSetting", new[] { "ItemCode" });
            DropIndex("EXCEED.NonFuelItemSetting", new[] { "ItemCategoryCode" });
            AlterColumn("FleetManagement.TimeSheet", "CustomerName", c => c.String(nullable: false));
            AlterColumn("FleetManagement.SitesVehicleTransfer", "SecondOperatorId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.RequestedEquipment", "GravelKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.RequestedEquipment", "AsphaltKm", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("FleetManagement.RentFromExternal", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "MeterReading", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "Lessor", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "OperatorLicenceNo", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "Capacity", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "ManufacturingYear", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "EngineSerialNo", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "EngineModel", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "ChasisSerialNo", c => c.String(nullable: false));
            AlterColumn("FleetManagement.RentalInspection", "Model", c => c.String(nullable: false));
            AlterColumn("FleetManagement.DumpTruckTimeSheet", "ReasonForDown", c => c.String(nullable: false));
            AlterColumn("FleetManagement.DumpTruckTimeSheet", "ReasonForIdle", c => c.String(nullable: false));
            AlterColumn("FleetManagement.DryLoadingTimeSheet", "ReasonForIdle", c => c.String(nullable: false));
            AlterColumn("FleetManagement.DriverAssignment", "SecondOperatorId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.Fleets", "RentId", c => c.Guid(nullable: false));
            AlterColumn("FleetManagement.Accident", "OperatorId", c => c.Guid(nullable: false));
            DropColumn("FleetManagement.WagonDrillTimesheet", "IdleReason");
            DropColumn("FleetManagement.WagonDrillTimesheet", "DownReason");
            DropColumn("FleetManagement.WagonDrillTimesheet", "MaintenanceFuel");
            DropColumn("FleetManagement.WagonDrillTimesheet", "FuelKMR");
            DropColumn("FleetManagement.WagonDrillTimesheet", "DifferenceEHR");
            DropColumn("FleetManagement.WagonDrillTimesheet", "FinalEHR");
            DropColumn("FleetManagement.WagonDrillTimesheet", "InitialEHR");
            DropColumn("FleetManagement.WagonDrillTimesheet", "NightTimeEnd");
            DropColumn("FleetManagement.WagonDrillTimesheet", "NightTimeStart");
            DropColumn("FleetManagement.WagonDrillTimesheet", "AfternoonTimeEnd");
            DropColumn("FleetManagement.WagonDrillTimesheet", "AfternoonTimeStart");
            DropColumn("FleetManagement.WagonDrillTimesheet", "MorningTimeEnd");
            DropColumn("FleetManagement.WagonDrillTimesheet", "MorningTimeStart");
            DropColumn("FleetManagement.WagonDrillTimesheet", "OilType");
            DropColumn("FleetManagement.WagonDrillTimesheet", "FuelType");
            DropColumn("FleetManagement.WagonDrillTimesheet", "GreaseCost");
            DropColumn("FleetManagement.WagonDrillTimesheet", "GreaseConsumption");
            DropColumn("FleetManagement.WagonDrillTimesheet", "OilCost");
            DropColumn("FleetManagement.WagonDrillTimesheet", "OilConsumption");
            DropColumn("FleetManagement.WagonDrillTimesheet", "Unit");
            DropColumn("FleetManagement.WagonDrillTimesheet", "Volume");
            DropColumn("FleetManagement.WagonDrillTimesheet", "TotalTime");
            DropColumn("FleetManagement.WagonDrillTimesheet", "BatteryCost");
            DropColumn("FleetManagement.WagonDrillTimesheet", "OverTime");
            DropColumn("FleetManagement.WagonDrillTimesheet", "BasicSalary");
            DropColumn("FleetManagement.WagonDrillTimesheet", "RentalAgreementNum");
            DropColumn("FleetManagement.WagonDrillTimesheet", "ExternalOperatorName");
            DropColumn("FleetManagement.WagonDrillTimesheet", "SparePartCost");
            DropColumn("FleetManagement.WagonDrillTimesheet", "LabourCost");
            DropColumn("Maintenance.VehicleReceptionForm", "DriverTransferId");
            DropColumn("Maintenance.VehicleReceptionForm", "CustomerName");
            DropColumn("Maintenance.VehiclePart", "ApplyForAll");
            DropColumn("FleetManagement.VehicleDistribution", "ParentId");
            DropColumn("FleetManagement.VehicleDemand", "ServiceCenterJobId");
            DropColumn("FleetManagement.TimeSheet", "CubicValue");
            DropColumn("FleetManagement.TimeSheet", "ChargeMethod");
            DropColumn("FleetManagement.TimeSheet", "ExternalRentedEquipmentId");
            DropColumn("FleetManagement.TimeSheet", "RentToExternalID");
            DropColumn("FleetManagement.TimeSheet", "RentFromExternalEquipmentId");
            DropColumn("FleetManagement.TimeSheet", "RentFromExternalID");
            DropColumn("FleetManagement.SitesVehicleTransfer", "TransferStatus");
            DropColumn("FleetManagement.SitesVehicleTransfer", "DGMApprovalDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "DGMApprovedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsDepartmentApproved");
            DropColumn("FleetManagement.SitesVehicleTransfer", "OperationApprovalDate");
            DropColumn("FleetManagement.SitesVehicleTransfer", "OperationApprvedBy");
            DropColumn("FleetManagement.SitesVehicleTransfer", "IsOperationApproved");
            DropColumn("FleetManagement.RentFromExternalEquipment", "IsOnlineApproved");
            DropColumn("FleetManagement.RentFromExternalEquipment", "Vat");
            DropColumn("FleetManagement.RentFromExternalEquipment", "PayableIdleHrPercent");
            DropColumn("FleetManagement.RentFromExternalEquipment", "IsContractTerminated");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "WithholdingAmount");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "VAT");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "FuelCost");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "MaintenanceCost");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "OilLubCost");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "SparePartCost");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "GrossBeforeVat");
            DropColumn("FleetManagement.RentalPaymentRequestLine", "CostSheetSummaryId");
            DropColumn("FleetManagement.RentalPaymentRequest", "AuthorizedTime");
            DropColumn("FleetManagement.RentalPaymentRequest", "AuthorizedBy");
            DropColumn("FleetManagement.RentalPaymentRequest", "IsAuthorized");
            DropColumn("FleetManagement.RentalPaymentRequest", "WithHoldingTax");
            DropColumn("FleetManagement.RentalInspection", "Unit");
            DropColumn("FleetManagement.RentalInspection", "Volume");
            DropColumn("Maintenance.PmSchedulers", "FleetId");
            DropColumn("FleetManagement.OwningandOperatingCost", "CostDate");
            DropColumn("Maintenance.MaintenancePlan", "ServiceCenterId");
            DropColumn("Maintenance.MaintenancePlan", "FleetId");
            DropColumn("Maintenance.MaintenancePlan", "EquipmentNameId");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "ReasonForIdle");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "ReasonForDown");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "MaintenanceFuel");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "FuelKMR");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "DifferenceEHR");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "FinalEHR");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "InitialEHR");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "OilType");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "FuelType");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "OilCost");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "TotalTime");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "BatteryCost");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "GreaseCost");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "OverTime");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "BasicSalary");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "RentalAgreementNum");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "SparePartCost");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "LabourCost");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "Unit");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "Volume");
            DropColumn("FleetManagement.MachineryWorkingTimeSheet", "ExternalOperatorName");
            DropColumn("FleetManagement.LightVehicleTimesheet", "ReasonForIdle");
            DropColumn("FleetManagement.LightVehicleTimesheet", "ReasonForDown");
            DropColumn("FleetManagement.LightVehicleTimesheet", "MaintenanceFuel");
            DropColumn("FleetManagement.LightVehicleTimesheet", "OilConsumption");
            DropColumn("FleetManagement.LightVehicleTimesheet", "OilCost");
            DropColumn("FleetManagement.LightVehicleTimesheet", "OilType");
            DropColumn("FleetManagement.LightVehicleTimesheet", "FuelType");
            DropColumn("FleetManagement.LightVehicleTimesheet", "ExternalOperatorName");
            DropColumn("FleetManagement.LightVehicleTimesheet", "BatteryCost");
            DropColumn("FleetManagement.LightVehicleTimesheet", "OverTime");
            DropColumn("FleetManagement.LightVehicleTimesheet", "BasicSalary");
            DropColumn("FleetManagement.LightVehicleTimesheet", "SparePartCost");
            DropColumn("FleetManagement.LightVehicleTimesheet", "LabourCost");
            DropColumn("FleetManagement.LightVehicleTimesheet", "GreaseCost");
            DropColumn("FleetManagement.LightVehicleTimesheet", "FuelKMR");
            DropColumn("FleetManagement.LightVehicleTimesheet", "DifferenceKMR");
            DropColumn("FleetManagement.LightVehicleTimesheet", "FinalKMR");
            DropColumn("FleetManagement.LightVehicleTimesheet", "InitialKMR");
            DropColumn("FleetManagement.LightVehicleTimesheet", "RentalAgreementNum");
            DropColumn("FleetManagement.LightVehicleTimesheet", "TotalTime");
            DropColumn("Maintenance.JobRequests", "CustomerName");
            DropColumn("Maintenance.JobOrderServiceCenterDetail", "DescriptionOfJobs");
            DropColumn("Maintenance.RepairType", "IsGeneric");
            DropColumn("Maintenance.JobOrder", "RemarkAfterDelivery");
            DropColumn("EXCEED.FuelItemSetting", "IsOil");
            DropColumn("EXCEED.FuelItemSetting", "IsTire");
            DropColumn("EXCEED.FuelItemSetting", "IsBattery");
            DropColumn("EXCEED.FuelItemSetting", "IsSparePart");
            DropColumn("EXCEED.FuelItemSetting", "IsLubricant");
            DropColumn("EXCEED.FleetInsuranceHistory", "IsPaymentRequestGenerated");
            DropColumn("EXCEED.FleetInsuranceHistory", "InsuranceCoverTypeId");
            DropColumn("EXCEED.FleetInsuranceHistory", "Payment");
            DropColumn("FleetManagement.FleetCheckList", "DriverVehicleTransferID");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceApprovedTime");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceApprovedBy");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsFinanceApproved");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceSendForApprovalTime");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "FinanceSendForApprovalBy");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsFinanceSendForApproval");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "BudgetApprovedTime");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "BudgetApprovedBy");
            DropColumn("Maintenance.ExternalMaintenancePaymentRequest", "IsBudgetApproved");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "IdleReason");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "DownReason");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "MaintenanceFuel");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "FuelKMR");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "DifferenceKMR");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "FinalKMR");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "InitialKMR");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "LoadingStatus");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "ExternalOperatorName");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "OilCost");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "GreaseCost");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "TotalTime");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "Unit");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "Volume");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "NightTimeEnd");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "NightTimeStart");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "AfternoonTimeEnd");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "AfternoonTimeStart");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "MorningTimeEnd");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "MorningTimeStart");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "BatteryCost");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "OverTime");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "BasicSalary");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "SparePartCost");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "OilType");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "RentalAgreementNum");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "FuelType");
            DropColumn("FleetManagement.DumpTruckTimeSheet", "LabourCost");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "IdleReason");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "ReasonForDown");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "MaintenanceFuel");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "OilType");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "FuelType");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "GravelWithOut");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "AsphaltWithOut");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "GreaseCost");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "GreaseConsumption");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "OilCost");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "BatteryCost");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "OverTime");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "BasicSalary");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "RentalAgreementNum");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "SparePartCost");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "TotalTime");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "LabourCost");
            DropColumn("FleetManagement.DryLoadingTimeSheet", "ExternalOperatorName");
            DropColumn("FleetManagement.TireIssue", "IsCreatedOnFleet");
            DropColumn("FleetManagement.FleetInsurances", "IsPaymentRequestGenerated");
            DropColumn("FleetManagement.FleetInsurances", "InsuranceCoverTypeId");
            DropColumn("FleetManagement.FleetInsurances", "Payment");
            DropColumn("FleetManagement.Fleets", "IsExternalCustomer");
            DropColumn("FleetManagement.Fleets", "Unit");
            DropColumn("FleetManagement.Fleets", "Volume");
            DropColumn("FleetManagement.Fleets", "IsReserved");
            DropColumn("FleetManagement.Fleets", "VehicleReceptionFormId");
            DropColumn("FleetManagement.Fleets", "IsContractTerminated");
            DropTable("Maintenance.WorkshopFieldWorkTool");
            DropTable("FleetManagement.WagonDrillDailyOperation");
            DropTable("Maintenance.VehicleReceptionDoucment");
            DropTable("Maintenance.TyreSerialInfo");
            DropTable("FleetManagement.TimeSheetConsumptionItem");
            DropTable("EXCEED.StoreRequisitionSecondValidation");
            DropTable("FleetManagement.SelfCareSatisfaction");
            DropTable("Maintenance.SatisfactionNote");
            DropTable("FleetManagement.RentFromExternalDocument");
            DropTable("FleetManagement.RentFromExternalCustomerChange");
            DropTable("FleetManagement.RentedEquipmentMonthlyPaymentSummaryValidation");
            DropTable("FleetManagement.RentedEquipmentMonthlyPaymentSummary");
            DropTable("FleetManagement.RentedEquipmentDailyOperation");
            DropTable("FleetManagement.RentalEquipmentPaymentSummaryDocument");
            DropTable("FleetManagement.RentalEquipmentAdvancePayment");
            DropTable("FleetManagement.RentalAdvancePaymentRequestValidation");
            DropTable("FleetManagement.RentalAdvancePaymentRequestDepartmentValidation");
            DropTable("FleetManagement.RentalAdvancePaymentRequestBudgetValidation");
            DropTable("FleetManagement.RentalAdvancePaymentRequest");
            DropTable("EXCEED.NonFuelItemSetting");
            DropTable("Maintenance.MaintenanceStage");
            DropTable("Maintenance.MaintenanceDailyPlan");
            DropTable("FleetManagement.MaintenanceCostDetail");
            DropTable("FleetManagement.InternalRentalRate");
            DropTable("Insurance.InsuranceCoverTypes");
            DropTable("FleetManagement.FleetInsuranceValidation");
            DropTable("FleetManagement.FleetInsurancePaymentRequestValidation");
            DropTable("FleetManagement.FleetInsurancePaymentRequest");
            DropTable("FleetManagement.FleetAvailabilityTresholdValidation");
            DropTable("FleetManagement.FleetAvailabilityTreshold");
            DropTable("FleetManagement.FleetAnnualPlanDaily");
            DropTable("Maintenance.ExternalServiceAttachement");
            DropTable("FleetManagement.EquipmentOperationPlanValidation");
            DropTable("FleetManagement.EquipmentOperationPlan");
            DropTable("FleetManagement.DriverTransferDetail");
            DropTable("Maintenance.DescriptionOfAchievement");
            DropTable("FleetManagement.CouponFuel");
            DropTable("Maintenance.AccesseryInfo");
        }
    }
}
